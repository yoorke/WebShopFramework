using eshopBE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopDL
{
    public class ProductViewDL
    {
        public List<ProductFPView> GetProducts(
                                        int categoryID,
                                        List<string> brands,
                                        List<AttributeValue> attributeValues,
                                        string sort,
                                        double priceFrom,
                                        double priceTo,
                                        bool includeChildrenCategoriesProducts = false)
        {
            List<ProductFPView> products = null;
            int tempAttributeID;
            int tableIndex;

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand(@"
                                                SELECT product.productID, code, brand.name, product.name, 
                                                       product.price, webPrice, promotionProduct.price, promotion.imageUrl, 
                                                       productImageUrl.imageUrl, isInStock, category.name, 
                                                            (SELECT ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + 
                                                                (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                                ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + 
                                                                (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                                category.url 
                                                                FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID 
                                                                LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID 
                                                                WHERE category.categoryID = (SELECT TOP 1 categoryID 
                                                                                             FROM productCategory 
                                                                                             WHERE productID = product.productID AND isMainCategory = 1)),
                                                        canBeDelivered, 
                                                        CAST(CASE WHEN EXISTS(SELECT * FROM productVariant WHERE productID = product.productID) THEN 1 ELSE 0 END as bit), 
                                                        isFreeDelivery,
                                                        product.listDescription,
                                                        product.sortIndex
                                                FROM product INNER JOIN brand ON product.brandID = brand.brandID 
                                                INNER JOIN productImageUrl ON product.productID = productImageUrl.productID 
                                                INNER JOIN productCategory ON product.productID = productCategory.productID 
                                                LEFT JOIN promotionProduct ON product.productID = promotionProduct.productID 
                                                LEFT JOIN promotion ON promotionProduct.promotionID = promotion.promotionID 
                                                INNER JOIN category ON productCategory.categoryID = category.categoryID", objConn))
                {
                    if(attributeValues.Count > 0)
                    {
                        tempAttributeID = 0;
                        tableIndex = 0;
                        for(int i = 0; i < attributeValues.Count; i++)
                        {
                            if(attributeValues[i].AttributeID != tempAttributeID)
                            {
                                tableIndex++;
                                objComm.CommandText += @" INNER JOIN productAttributeValue as a" + tableIndex.ToString() +
                                                        " ON product.productID = a" + tableIndex.ToString() + ".productID";
                                tempAttributeID = attributeValues[i].AttributeID;
                            }
                        }
                    }

                    objComm.CommandText += " WHERE productImageUrl.sortOrder = 1 AND product.isActive = 1 AND product.isApproved = 1";

                    if(includeChildrenCategoriesProducts)
                    {
                        List<int> childrenCategories = new CategoryDL().GetChildrenCategories(categoryID);
                        objComm.CommandText += " AND (productCategory.categoryID = @categoryID";
                        for (int i = 0; i < childrenCategories.Count; i++)
                            objComm.CommandText += " OR productCategory.categoryID = @childrenCategoryID" + (i + 1);
                        objComm.CommandText += ")";
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        for (int i = 0; i < childrenCategories.Count; i++)
                            objComm.Parameters.Add("@childrenCategoryID" + (i + 1), SqlDbType.Int).Value = childrenCategories[i];
                    }
                    else
                    {
                        objComm.CommandText += " AND productCategory.categoryID = @categoryID";
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    }

                    if(priceFrom > 0)
                    {
                        objComm.CommandText += " AND webPrice >= @priceFrom";
                        objComm.Parameters.Add("@priceFrom", SqlDbType.Float).Value = priceFrom;
                    }
                    if(priceTo > 0)
                    {
                        objComm.CommandText += " AND webPrice <= @priceTo";
                        objComm.Parameters.Add("@priceTo", SqlDbType.Float).Value = priceTo;
                    }

                    if(brands.Count > 0)
                    {
                        for(int i = 0; i < brands.Count; i++)
                        {
                            if (i == 0)
                                objComm.CommandText += " AND (brand.brandID = @brandID" + (i + 1).ToString();
                            else
                                objComm.CommandText += " OR brand.brandID = @brandID" + (i + 1).ToString();
                            objComm.Parameters.Add("@brandID" + (i + 1).ToString(), SqlDbType.Int).Value = brands[i];

                            if (i == brands.Count - 1)
                                objComm.CommandText += ")";
                        }
                    }

                    tempAttributeID = 0;
                    tableIndex = 0;
                    for(int i = 0; i < attributeValues.Count; i++)
                    {
                        if (attributeValues[i].AttributeID != tempAttributeID)
                        {
                            tableIndex++;
                            objComm.CommandText += " AND (a" + tableIndex.ToString() + ".attributeValueID = @attributeValueID" + (i + 1).ToString();
                            tempAttributeID = attributeValues[i].AttributeID;
                        }
                        else
                            objComm.CommandText += " OR a" + tableIndex.ToString() + ".attributeValueID = @attributeValueID" + (i + 1).ToString();

                        if (i < attributeValues.Count - 1)
                            if (tempAttributeID != attributeValues[i + 1].AttributeID)
                                objComm.CommandText += ")";
                        if (i == attributeValues.Count - 1)
                            objComm.CommandText += ")";

                        objComm.Parameters.Add("@attributeValueID" + (i + 1).ToString(), SqlDbType.Int).Value = attributeValues[i].AttributeValueID;
                    }

                    objComm.CommandText += " ORDER BY product.isInStock DESC, " + sort;

                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products = new List<ProductFPView>();
                        while(reader.Read())
                        {
                            products.Add(new ProductFPView()
                            {
                                ProductID = reader.GetInt32(0),
                                Code = reader.GetString(1),
                                BrandName = reader.GetString(2),
                                Name = reader.GetString(3),
                                Price = reader.GetDouble(4),
                                WebPrice = reader.GetDouble(5),
                                PromotionPrice = !Convert.IsDBNull(reader[6]) ? reader.GetDouble(6) : 0,
                                PromotionImageUrl = !Convert.IsDBNull(reader[7]) ? reader.GetString(7) : string.Empty,
                                ImageUrl = !Convert.IsDBNull(reader[8]) ? reader.GetString(8) : string.Empty,
                                IsInStock = reader.GetBoolean(9),
                                CategoryName = reader.GetString(10),
                                FullCategoryUrl = reader.GetString(11),
                                CanBeDelivered = !Convert.IsDBNull(reader[12]) ? reader.GetBoolean(12) : true,
                                HasVariants = !Convert.IsDBNull(reader[13]) ? reader.GetBoolean(13) : false,
                                IsFreeDelivery = !Convert.IsDBNull(reader[14]) ? reader.GetBoolean(14) : false,
                                ListDescription = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty,
                                SortIndex = !Convert.IsDBNull(reader[16]) ? reader.GetInt32(16) : 0
                            });
                        }
                    }
                }
            }

            return products;
        }

        public List<ProductFPView> GetProductsForFirstPage(int categoryID, int brandID, int numberOfProducts, ProductOrderByType orderBy, int supplierID = -1)
        {
            List<ProductFPView> products = null;
            bool existsWhere = false;

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand(@"
                                                            SELECT TOP " + numberOfProducts.ToString() + " product.productID, product.code, brand.name, product.name, " +
                                                            @" product.price, webPrice, promotionProduct.price, promotion.imageUrl, 
                                                            productImageUrl.imageUrl, isInStock, category.name, 
                                                                (SELECT ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + 
                                                                (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                                ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + 
                                                                (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                                category.url 
                                                                FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID 
                                                                LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID 
                                                                WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1)),
                                                            canBeDelivered, 
                                                            CAST(CASE WHEN EXISTS(SELECT * FROM productVariant WHERE productID = product.productID) THEN 1 ELSE 0 END as bit), 
                                                            isFreeDelivery,
                                                            product.listDescription,
                                                            product.sortIndex,
                                                            (SELECT category.name FROM category WHERE categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1))
                                                            FROM product INNER JOIN brand ON product.brandID = brand.brandID 
                                                            INNER JOIN productImageUrl ON product.productID = productImageUrl.productID 
                                                            INNER JOIN productCategory ON product.productID = productCategory.productID 
                                                            LEFT JOIN promotionProduct ON product.productID = promotionProduct.productID 
                                                            LEFT JOIN promotion ON promotionProduct.promotionID = promotion.promotionID 
                                                            INNER JOIN category ON productCategory.categoryID = category.categoryID 
                                                            INNER JOIN supplier ON product.supplierID = supplier.supplierID " + 
                                                            getTableJoinStringByOrderBy(orderBy), objConn))
                {
                    objConn.Open();
                    if(categoryID > 0)
                    {
                        objComm.CommandText += " WHERE category.categoryID = @categoryID";
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        existsWhere = true;
                    }

                    if(brandID > 0)
                    {
                        objComm.CommandText += ((existsWhere) ? " AND" : " WHERE") + " brand.brandID = @brandID";
                        objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;
                        existsWhere = true;
                    }

                    if(supplierID > 0)
                    {
                        objComm.CommandText += ((existsWhere) ? " AND" : " WHERE") + " supplier.supplierID = @supplierID";
                        objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;
                        existsWhere = true;
                    }

                    int featuredProductsCategory = ConfigurationManager.AppSettings["featuredProductsCategory"] != null ? int.Parse(ConfigurationManager.AppSettings["featuredProductsCategory"]) : 0;
                    objComm.CommandText += ((existsWhere) ? " AND" : " WHERE") + "  isActive = 1 AND isApproved = 1 AND productImageUrl.sortOrder = 1" 
                        + (!featuredProductsCategory.Equals(categoryID) ? " AND isInStock = 1" : string.Empty);

                    if (categoryID <= 0)
                        objComm.CommandText += " AND isMainCategory = 1";

                    objComm.CommandText += getGroupByStringByOrderBy(orderBy);

                    objComm.CommandText += " ORDER BY " + getOrderByByOrderByType(orderBy);

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products = new List<ProductFPView>();

                        while(reader.Read())
                        {
                            products.Add(new ProductFPView()
                            {
                                ProductID = reader.GetInt32(0),
                                Code = reader.GetString(1),
                                BrandName = reader.GetString(2),
                                Name = reader.GetString(3),
                                Price = reader.GetDouble(4),
                                WebPrice = reader.GetDouble(5),
                                PromotionPrice = !Convert.IsDBNull(reader[6]) ? reader.GetDouble(6) : 0,
                                PromotionImageUrl = !Convert.IsDBNull(reader[7]) ? reader.GetString(7) : string.Empty,
                                ImageUrl = !Convert.IsDBNull(reader[8]) ? reader.GetString(8) : string.Empty,
                                IsInStock = reader.GetBoolean(9),
                                //CategoryName = reader.GetString(10),
                                CategoryName = reader.GetString(17),
                                FullCategoryUrl = reader.GetString(11),
                                CanBeDelivered = !Convert.IsDBNull(reader[12]) ? reader.GetBoolean(12) : true,
                                HasVariants = !Convert.IsDBNull(reader[13]) ? reader.GetBoolean(13) : false,
                                IsFreeDelivery = !Convert.IsDBNull(reader[14]) ? reader.GetBoolean(14) : false,
                                ListDescription = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty,
                                SortIndex = !Convert.IsDBNull(reader[16]) ? reader.GetInt32(16) : 0
                            });
                        }
                    }
                }
            }

            return products;
        }

        private string getTableJoinStringByOrderBy(ProductOrderByType orderBy)
        {
            switch (orderBy)
            {
                case ProductOrderByType.Order: {
                        return " LEFT JOIN productAccess ON product.productID = productAccess.productID ";
                            //+
                               //" GROUP BY product.productID, code, brand.name, product.name, product.price, " +
                               //" webPrice, promotionProduct.price, promotion.imageUrl, productImageUrl.imageUrl, " +
                               //" isInStock, category.name";
                               break;
                    }
                case ProductOrderByType.Access:
                    {
                        return " LEFT JOIN orderItem ON product.productID = orderItem.productID ";
                            //+
                               //" GROUP BY product.productID, code, brand.name, product.name, product.price, " +
                               //" webPrice, promotionProduct.price, promotion.imageUrl, productImageUrl.imageUrl, " +
                               //" isInStock, category.name";
                               break;
                    }
                default: { return string.Empty; break; }
            }
        }

        private string getGroupByStringByOrderBy(ProductOrderByType orderBy)
        {
            switch(orderBy)
            {
                case ProductOrderByType.Order:
                    {
                        return " GROUP BY product.productID, product.code, brand.name, product.name, product.price, webPrice, promotionProduct.price, " +
                               " promotion.imageUrl, productImageUrl.imageUrl, isInStock, category.name, canBeDelivered, isFreeDelivery, listDescription, sortIndex";
                        break;
                    }
                case ProductOrderByType.Access:
                    {
                        return " GROUP BY product.productID, product.code, brand.name, product.name, product.price, webPrice, promotionProduct.price, " +
                               " promotion.imageUrl, productImageUrl.imageUrl, isInStock, category.name, canBeDelivered, isFreeDelivery, listDescription, sortIndex";
                        break;
                    }
                default: { return string.Empty; break; }
            }
        }

        private string getOrderByByOrderByType(ProductOrderByType orderBy)
        {
            switch(orderBy)
            {
                case ProductOrderByType.Access: { return " COUNT(*) DESC"; }
                case ProductOrderByType.New: { return " insertDate DESC"; }
                case ProductOrderByType.Order: { return " COUNT(*) DESC"; }
                case ProductOrderByType.Price: { return " webPrice"; }
                case ProductOrderByType.PriceDesc: { return " webPrice DESC"; }
                case ProductOrderByType.Random: { return " NEWID()"; }
                default: { return " price"; }
            }
        }
    }
}
