using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopUtilities;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web;
using System.Text.RegularExpressions;

namespace eshopDL
{
    public class ProductDL
    {
        #region GetProducts

        private List<Product> GetProducts(int productID, string code, string supplierCode, List<eshopBE.AttributeValue> attributes, int categoryID, int supplierID, bool? isApproved, bool? isActive, int? brandID, int? promotionID, string sort = "brand.name, product.name")
        {
            List<Product> products = null;
            bool whereExists = false;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, supplierCode, brand.brandID, product.name, description, product.price, webPrice, brand.name, isApproved, isActive, isLocked, isInStock, ISNULL((SELECT TOP 1 price FROM promotionProduct WHERE productID = product.productID), 0) as promotionPrice, insertDate FROM product INNER JOIN brand ON product.brandID=brand.brandID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        if (promotionID != null)
                        { 
                            objComm.CommandText += " INNER JOIN promotionProduct ON product.productID = promotionProduct.productID WHERE promotionID = @promotionID";
                            objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                            whereExists = true;
                        }

                        if (productID > 0)
                        {
                            if (!whereExists)
                                objComm.CommandText += " WHERE product.productID=@productID";
                            else
                                objComm.CommandText += " AND product.productID = @productID";
                            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                            whereExists = true;
                        }
                        else if (code != string.Empty)
                        {
                            if (!whereExists)
                                objComm.CommandText += " WHERE product.code=@code";
                            else
                                objComm.CommandText += " AND product.code = @code";
                            objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = code;
                            whereExists = true;
                        }
                        else if (supplierCode != string.Empty)
                        {
                            if (!whereExists)
                                objComm.CommandText += " WHERE product.supplierCode=@supplierCode";
                            else
                                objComm.CommandText += " AND product.supplierCode = @supplierCode";
                            objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = supplierCode;
                            whereExists = true;
                        }
                        else if (attributes != null || categoryID > 0)
                        {
                            if (attributes != null)
                                for (int i = 0; i < attributes.Count; i++)
                                    objComm.CommandText += " INNER JOIN productAttributeValue as a" + (i + 1).ToString() + " ON product.productID=a" + (i + 1).ToString() + ".productID";

                            if (categoryID > 0)
                                objComm.CommandText += " INNER JOIN productCategory ON product.productID=productCategory.productID";

                            if (attributes != null)
                            {
                                for (int i = 0; i < attributes.Count; i++)
                                {
                                    if (!whereExists)
                                    {
                                        objComm.CommandText += " WHERE a" + (i + 1).ToString() + ".attributeValueID=@attributeValueID" + (i + 1).ToString();
                                        whereExists = true;
                                    }
                                    else
                                        objComm.CommandText += " AND a" + (i + 1).ToString() + ".attributeValueID=@attributeValueID" + (i + 1).ToString();

                                    objComm.Parameters.Add("@attributeValueID" + (i + 1).ToString(), SqlDbType.Int).Value = attributes[i].AttributeValueID;
                                }
                            }

                            if (categoryID > 0)
                            {
                                if (!whereExists)
                                {
                                    objComm.CommandText += " WHERE productCategory.categoryID=@categoryID";
                                    whereExists = true;
                                }
                                else
                                    objComm.CommandText += " AND productCategory.categoryID=@categoryID";

                                objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                            }
                        }
                        else if(categoryID <= 0)
                        {
                            //if (!whereExists)
                            //{
                                //objComm.CommandText += " WHERE isMainCategory = 1";
                                //whereExists = true;
                            //}
                            //else
                                //objComm.CommandText += " AND isMainCategory = 1";
                        }

                        if (supplierID > -1)
                        {
                            objComm.CommandText += (whereExists) ? " AND supplierID=@supplierID" : " WHERE supplierID=@supplierID";
                            objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;
                            whereExists = true;
                        }

                        if (isApproved != null)
                        {
                            objComm.CommandText += (whereExists) ? " AND isApproved=@isApproved" : " WHERE isApproved=@isApproved";
                            objComm.Parameters.Add("@isApproved", SqlDbType.Bit).Value = isApproved;
                            whereExists = true;
                        }

                        if (isActive != null)
                        {
                            objComm.CommandText += (whereExists) ? " AND isActive=@isActive" : " WHERE isActive=@isActive";
                            objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = isActive;
                            whereExists = true;
                        }

                        if (brandID != null)
                        {
                            objComm.CommandText += (whereExists) ? " AND brand.brandID=@brandID" : " WHERE brand.brandID=@brandID";
                            objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;
                            whereExists = true;
                        }

                        objComm.CommandText += " ORDER BY " + sort;

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            Product product;
                            if (reader.HasRows)
                                products = new List<Product>();

                            while (reader.Read())
                            {
                                product = new Product();
                                product.ProductID = reader.GetInt32(0);
                                product.Code = reader.GetString(1);
                                product.SupplierCode = reader.GetString(2);
                                product.Brand = new Brand(reader.GetInt32(3), reader.GetString(8));
                                product.Name = reader.GetString(4);
                                product.Description = reader.GetString(5);
                                product.Price = reader.GetDouble(6);
                                product.WebPrice = reader.GetDouble(7);
                                product.IsApproved = reader.GetBoolean(9);
                                product.IsActive = reader.GetBoolean(10);
                                product.IsLocked = reader.GetBoolean(11);
                                product.IsInStock = reader.GetBoolean(12);
                                product.Images = GetProductImages(product.ProductID);
                                product.Promotion = new Promotion(-1, string.Empty, 0, string.Empty, reader.GetDouble(13), false, DateTime.Now, DateTime.Now, string.Empty, false);
                                product.Categories = new List<Category>();
                                product.Categories.Add(new CategoryDL().GetCategory(categoryID));
                                product.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(14));

                                products.Add(product);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading products list", ex);
                    }

                }
            }
            return products;
        }

        public List<Product> GetAllProducts()
        {
            return GetProducts(-1, string.Empty, string.Empty, null, -1, -1, null, null, null, null);
        }

        public List<Product> GetProducts(List<AttributeValue> attributes)
        {
            return GetProducts(-1, string.Empty, string.Empty, attributes, -1, -1, null, null, null, null);
        }

        public List<Product> GetProductsForCategory(int categoryID, bool isActive, bool isApproved)
        {
            return GetProducts(-1, string.Empty, string.Empty, null, categoryID, -1, isApproved,  isActive, null, null);
        }

        public List<Product> GetProducts(int categoryID, int supplierID, bool? isApproved, bool? isActive, int? brandID, int? promotionID, string sort = "brand.name, product.name")
        {
            return GetProducts(-1, string.Empty, string.Empty, null, categoryID, supplierID, isApproved, isActive, brandID, promotionID, sort);
        }

        public List<Product> GetProducts(int categoryID, List<string> brandsID, List<AttributeValue> attributeValues, string sort, double priceFrom, double priceTo, bool includeChildrenCategoriesProducts = false)
        {
            List<Product> products = null;
            int tempAttributeID;
            int tableIndex;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, promotion.dateFrom, promotion.dateTo, category.name, product.isInStock FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID INNER JOIN productCategory ON product.productID=productCategory.productID LEFT JOIN promotionProduct ON product.productID=promotionProduct.productID LEFT JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN category ON productCategory.categoryID=category.categoryID", objConn))
                {
                    if(attributeValues.Count > 0)
                    {
                        tempAttributeID = 0;
                        tableIndex = 0;
                        for (int i = 0; i < attributeValues.Count; i++)
                        {
                            if (attributeValues[i].AttributeID != tempAttributeID)
                            {
                                tableIndex++;
                                objComm.CommandText += " INNER JOIN productAttributeValue AS a" + tableIndex.ToString() + " ON product.productID=a" + tableIndex.ToString() + ".productID";
                                tempAttributeID = attributeValues[i].AttributeID;
                            }
                        }
                    }

                    objComm.CommandText += " WHERE productImageUrl.sortOrder=1 AND product.isActive=1 AND product.isApproved=1";

                    if (includeChildrenCategoriesProducts)
                    { 
                        List<int> childrenCategories = new CategoryDL().GetChildrenCategories(categoryID);
                        objComm.CommandText += " AND (productCategory.categoryID=@categoryID";
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
                    
                    
                    

                    

                    if (priceFrom > 0)
                    {
                        objComm.CommandText += " AND webPrice>=@priceFrom";
                        objComm.Parameters.Add("@priceFrom", SqlDbType.Float).Value = priceFrom;
                    }
                    if (priceTo > 0)
                    {
                        objComm.CommandText += " AND webPrice<=@priceTo";
                        objComm.Parameters.Add("@priceTo", SqlDbType.Float).Value = priceTo;
                    }

                    if (brandsID.Count > 0)
                    {
                        for (int i = 0; i < brandsID.Count; i++)
                        {
                            if (i == 0)
                                objComm.CommandText += " AND (brand.brandID=@brandID" + (i + 1).ToString();
                            else
                                objComm.CommandText += " OR brand.brandID=@brandID" + (i + 1).ToString();

                            objComm.Parameters.Add("@brandID" + (i + 1).ToString(), SqlDbType.Int).Value = brandsID[i];

                            if (i == brandsID.Count - 1)
                                objComm.CommandText += ")";
                        }
                    }

                    tempAttributeID = 0;
                    tableIndex = 0;
                    for (int i = 0; i < attributeValues.Count; i++)
                    {
                        if (attributeValues[i].AttributeID != tempAttributeID)
                        {
                            tableIndex++;
                            objComm.CommandText += " AND (a" + tableIndex.ToString() + ".attributeValueID=@attributeValueID" + (i + 1).ToString();
                            tempAttributeID = attributeValues[i].AttributeID;

                        }
                        else
                            objComm.CommandText += " OR a" + tableIndex.ToString() + ".attributeValueID=@attributeValueID" + (i + 1).ToString();

                        if (i < attributeValues.Count - 1)
                            if (tempAttributeID != attributeValues[i + 1].AttributeID)
                                objComm.CommandText += ")";
                        if (i == attributeValues.Count - 1)
                            objComm.CommandText += ")";


                            //objComm.CommandText += " AND a" + (i + 1).ToString() + ".attributeValueID=@attributeValueID" + (i + 1).ToString();
                        objComm.Parameters.Add("@attributeValueID" + (i + 1).ToString(), SqlDbType.Int).Value = attributeValues[i].AttributeValueID;
                        
                    }

                    objComm.CommandText += " ORDER BY " + sort;

                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products = new List<Product>();
                        Product product;
                        while (reader.Read())
                        {
                            product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Code = reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.Description = reader.GetString(3);
                            product.Price = reader.GetDouble(4);
                            product.WebPrice = reader.GetDouble(5);
                            product.Brand = new Brand(-1, reader.GetString(6));
                            product.Images = new List<ProductImage>();
                            //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/" + reader.GetString(7))))
                            //product.Images.Add("/images/" + reader.GetString(7));
                            //else
                            //product.Images.Add("/images/no-image.jpg");
                            product.Images.Add(new ProductImage(reader.GetString(7), 1));
                            if (!Convert.IsDBNull(reader[8]))
                            {
                                if (reader.GetDateTime(10) < DateTime.UtcNow && reader.GetDateTime(11) > DateTime.UtcNow)
                                {
                                    product.Promotion = new Promotion();
                                    product.Promotion.Price = reader.GetDouble(8);
                                    product.Promotion.ImageUrl = reader.GetString(9);
                                }
                            }
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(categoryID, reader.GetString(12), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false));
                            product.Description = GetProductAttributeValues(product.ProductID, true);
                            product.IsInStock = reader.GetBoolean(13);
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public List<Product> GetProductsForPromotion(int promotionID)
        {
            List<Product> products = null;
            Product product=null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, product.code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, category.name, category.categoryID, product.isInStock, promotion.value FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID INNER JOIN promotionProduct ON product.productID=promotionProduct.productID INNER JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN productCategory ON productCategory.productID=product.productID INNER JOIN category ON productCategory.categoryID=category.categoryID WHERE promotion.promotionID=@promotionID AND isActive=1 AND isApproved=1 AND productImageUrl.sortOrder=1 and isMainCategory = 1 ORDER BY product.name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products = new List<Product>();
                        while (reader.Read())
                        {
                            product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Code = reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.Description = reader.GetString(3);
                            product.Price = reader.GetDouble(4);
                            product.WebPrice = reader.GetDouble(5);
                            product.Brand = new Brand(-1, reader.GetString(6));
                            product.Images = new List<ProductImage>();
                            //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/" + reader.GetString(7))))
                                product.Images.Add(new ProductImage(reader.GetString(7), 1));
                            //else
                                //product.Images.Add("/images/no-image.jpg");
                            product.Promotion = new Promotion();
                            product.Promotion.Price = reader.GetDouble(8);
                            product.Promotion.ImageUrl = reader.GetString(9);
                            product.Promotion.Value = reader.GetDouble(13);                            
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(reader.GetInt32(11), reader.GetString(10), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false));
                            product.IsInStock = reader.GetBoolean(12);

                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public List<Product> GetProductsForFirstPage(int categoryID, int brandID, int numberOfProducts, string orderBy)
        {
            List<Product> products = null;
            Product product = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT TOP " + numberOfProducts.ToString() + " product.productID, product.code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, promotion.dateFrom, promotion.dateTo, category.name, product.isInStock FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID LEFT JOIN promotionProduct ON product.productID=promotionProduct.productID LEFT JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN productCategory ON product.productID=productCategory.productID INNER JOIN category ON productCategory.categoryID=category.categoryID ", objConn))
                {
                    objConn.Open();
                    bool exist = false;
                    if(categoryID > 0)
                    {
                        objComm.CommandText += " WHERE category.categoryID=@categoryID";
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        exist = true;
                    }
                    if(brandID > 0) {
                        objComm.CommandText += ((exist) ? " AND " : " WHERE ") + " brand.brandID = @brandID";
                        objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;
                    }
                    objComm.CommandText += " AND isActive = 1 AND isApproved = 1 AND productImageUrl.sortOrder = 1";
                    if (categoryID <= 0)
                        objComm.CommandText += " AND isMainCategory = 1";
                    objComm.CommandText += " ORDER BY " + orderBy;
                    //objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products = new List<Product>();
                        while (reader.Read())
                        {
                            product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Code = reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.Description = reader.GetString(3);
                            product.Price = reader.GetDouble(4);
                            product.WebPrice = reader.GetDouble(5);
                            product.Brand = new Brand(-1, reader.GetString(6));
                            product.Images = new List<ProductImage>();
                            //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/" + reader.GetString(7))))
                                product.Images.Add(new ProductImage(reader.GetString(7), 1));
                            //else
                                //product.Images.Add("/images/no-image.jpg");
                            if (Convert.IsDBNull(reader[8]) == false)
                            {
                                if (reader.GetDateTime(10) <= DateTime.UtcNow && reader.GetDateTime(11) >= DateTime.UtcNow)
                                {
                                    product.Promotion = new Promotion();
                                    product.Promotion.Price = reader.GetDouble(8);
                                    product.Promotion.ImageUrl = reader.GetString(9);
                                }
                            }
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(categoryID, reader.GetString(12), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false));
                            product.Description = GetProductAttributeValues(product.ProductID, true);
                            product.IsInStock = reader.GetBoolean(13);
                            product.Attributes = GetProductAttributes(product.ProductID);

                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public DataTable GetProductsBarcodes()
        {
            DataTable barcodes = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("product_getBarcodes", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            barcodes.Load(reader);
                    }
                }
            }
            return barcodes;
        }

        #endregion

        #region SaveProduct

        public int SaveProduct(Product product)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO product (code, supplierCode, brandID, name, description, price, webPrice, isApproved, isActive, supplierID, vatID, insertDate, updateDate, specification, isLocked, isInStock, ean, supplierPrice, unitOfMeasureID) VALUES (@code, @supplierCode, @brandID, @name, @description, @price, @webPrice, @isApproved, @isActive, @supplierID, @vatID, @insertDate, @updateDate, @specification, @isLocked, @isInStock, @ean, @supplierPrice, @unitOfMeasureID); SELECT SCOPE_IDENTITY()", objConn))
                {
                    try
                    {
                        objConn.Open();


                        objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = product.Code;
                        objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = product.SupplierCode;
                        objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = product.Brand.BrandID;
                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = product.Name;
                        objComm.Parameters.Add("@description", SqlDbType.NVarChar).Value = product.Description;
                        objComm.Parameters.Add("@price", SqlDbType.Float).Value = product.Price;
                        objComm.Parameters.Add("@webPrice", SqlDbType.Float).Value = product.WebPrice;
                        objComm.Parameters.Add("@isApproved", SqlDbType.Bit).Value = product.IsApproved;
                        objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = product.IsActive;
                        objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = product.SupplierID;
                        objComm.Parameters.Add("@vatID", SqlDbType.Int).Value = product.VatID;
                        objComm.Parameters.Add("@insertDate", SqlDbType.DateTime).Value = product.InsertDate;
                        objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = product.UpdateDate;
                        objComm.Parameters.Add("@specification", SqlDbType.NVarChar).Value = product.Specification;
                        objComm.Parameters.Add("@isLocked", SqlDbType.Bit).Value = product.IsLocked;
                        objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = product.IsInStock;
                        objComm.Parameters.Add("@ean", SqlDbType.NVarChar, 50).Value = product.Ean;
                        objComm.Parameters.Add("@supplierPrice", SqlDbType.Float).Value = product.SupplierPrice;
                        objComm.Parameters.Add("@unitOfMeasureID", SqlDbType.Int).Value = product.UnitOfMeasure.UnitOfMeasureID;

                        product.ProductID = int.Parse(objComm.ExecuteScalar().ToString());

                        if (product.ProductID > 0)
                        {
                            if (product.Attributes != null)
                                SaveProductAttributes(product.Attributes, product.ProductID);
                            SaveProductCategories(product.Categories, product.ProductID);
                            if (product.Images != null)
                                SaveProductImages(product.Images, product.ProductID);
                            else
                                DeleteProductImages(product.ProductID);
                            if (product.Promotion != null)
                                saveProductPromotion(product.Promotion, product.ProductID);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving product", ex);
                    }
                }
            }
            return product.ProductID;
        }

        public int UpdateProduct(Product product)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET code=@code, supplierCode=@supplierCode, brandID=@brandID, name=@name, description=@description, price=@price, webPrice=@webPrice, isApproved=@isApproved, isActive=@isActive, supplierID=@supplierID, vatID=@vatID, updateDate=@updateDate, specification=@specification, isLocked=@isLocked, isInStock=@isInStock, ean=@ean, supplierPrice = @supplierPrice, unitOfMeasureID = @unitOfMeasureID WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        if (product.Specification == string.Empty || product.Specification == null)
                            objComm.CommandText = "UPDATE product SET code=@code, supplierCode=@supplierCode, brandID=@brandID, name=@name, description=@description, price=@price, webPrice=@webPrice, isApproved=@isApproved, isActive=@isActive, supplierID=@supplierID, vatID=@vatID, updateDate=@updateDate, isLocked=@isLocked, isInStock=@isInStock, ean=@ean, supplierPRice = @supplierPrice, unitOfMeasure = @unitOfMeasure WHERE productID=@productID";

                        objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = product.Code;
                        objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = product.SupplierCode;
                        objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = product.Brand.BrandID;
                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = product.Name;
                        objComm.Parameters.Add("@description", SqlDbType.NVarChar).Value = product.Description;
                        objComm.Parameters.Add("@price", SqlDbType.Float).Value = product.Price;
                        objComm.Parameters.Add("@webPrice", SqlDbType.Float).Value = product.WebPrice;
                        objComm.Parameters.Add("@isApproved", SqlDbType.Bit).Value = product.IsApproved;
                        objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = product.IsActive;
                        objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = product.SupplierID;
                        objComm.Parameters.Add("@vatID", SqlDbType.Int).Value = product.VatID;
                        objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = product.UpdateDate;
                        if (product.Specification != string.Empty && product.Specification != null)
                            objComm.Parameters.Add("@specification", SqlDbType.NVarChar).Value = product.Specification;
                        objComm.Parameters.Add("@isLocked", SqlDbType.Bit).Value = product.IsLocked;
                        objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = product.IsInStock;
                        objComm.Parameters.Add("@ean", SqlDbType.NVarChar, 50).Value = product.Ean;
                        objComm.Parameters.Add("@supplierPrice", SqlDbType.Float).Value = product.SupplierPrice;
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = product.ProductID;
                        objComm.Parameters.Add("@unitOfMeasureID", SqlDbType.Int).Value = product.UnitOfMeasure.UnitOfMeasureID;

                        status = objComm.ExecuteNonQuery();

                        if (status > 0)
                        {
                            if (product.Attributes != null)
                                SaveProductAttributes(product.Attributes, product.ProductID);
                            SaveProductCategories(product.Categories, product.ProductID);
                            if (product.Images != null)
                                SaveProductImages(product.Images, product.ProductID);
                            //else
                                //DeleteProductImages(product.ProductID);
                            if (product.Promotion != null)
                                saveProductPromotion(product.Promotion, product.ProductID);
                            else
                                DeleteProductPromotions(product.ProductID);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while updating product", ex);
                    }
                }
            }
            return product.ProductID;
        }

        private int[] SaveProductAttributes(List<AttributeValue> attributes, int productID)
        {
            int[] status = new int[attributes.Count];
            DeleteProductAttributes(productID);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO productAttributeValue (productID, attributeValueID) VALUES (@productID, @attributeValueID)", objConn))
                {
                    try
                    {
                        objConn.Open();

                        for (int i = 0; i < attributes.Count; i++)
                        {
                            objComm.Parameters.Clear();
                            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                            objComm.Parameters.Add("@attributeValueID", SqlDbType.Int).Value = attributes[i].AttributeValueID;

                            status[i] = objComm.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving product attributes", ex);
                    }
                }
            }

            return status;
        }

        private int[] SaveProductCategories(List<Category> categories, int productID)
        {
            int[] status = new int[categories.Count];
            DeleteProductCategories(productID);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO productCategory (productID, categoryID, isMainCategory) VALUES (@productID, @categoryID, @isMainCategory)", objConn))
                {
                    try
                    {
                        objConn.Open();

                        for (int i = 0; i < categories.Count; i++)
                        {
                            objComm.Parameters.Clear();
                            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                            objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categories[i].CategoryID;
                            objComm.Parameters.Add("@isMainCategory", SqlDbType.Bit).Value = i == 0 ? true : false;

                            status[i] = objComm.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving product categories", ex);
                    }
                }
            }
            return status;
        }

        private int[] SaveProductImages(List<ProductImage> images, int productID)
        {
            int[] status = new int[images.Count];
            DeleteProductImages(productID);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO productImageUrl (productID, imageUrl, sortOrder, productImageUrlID) VALUES (@productID, @imageUrl, @sortOrder, @productImageUrlID)", objConn))
                {
                    try
                    {
                        objConn.Open();

                        for (int i = 0; i < images.Count; i++)
                        {
                            objComm.Parameters.Clear();
                            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                            objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 100).Value = (images[i].ImageUrl.Contains("/images/")) ? images[i].ImageUrl.Substring(8, images[i].ImageUrl.Length - 8) : images[i].ImageUrl;
                            objComm.Parameters.Add("@sortOrder", SqlDbType.Int).Value = images[i].SortOrder;
                            objComm.Parameters.Add("@productImageUrlID", SqlDbType.Int).Value = int.Parse(images[i].ImageUrl.Substring(0, images[i].ImageUrl.LastIndexOf('.')));

                            status[i] = objComm.ExecuteNonQuery();
                        }
                    }
                    catch(SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving product images", ex);
                    }
                }
            }
            return status;
        }

        private int saveProductPromotion(Promotion promotion, int productID)
        {
            int status = 0;
            DeleteProductPromotions(productID);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO promotionProduct (promotionID, productID, price) VALUES (@promotionID, @productID, @price)", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotion.PromotionID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@price", SqlDbType.Float).Value = promotion.Price;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
        
        public int SetApproved(int productID, bool isApproved)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isApproved=@isApproved WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@isApproved", SqlDbType.Bit).Value = isApproved;
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while changing approved status", ex);
                    }
                }
            }
            return status;
        }

        public int SetActive(int productID, bool isActive)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isActive=@isActive WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = isActive;
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while changing active status", ex);
                    }
                }
            }
            return status;
        }

        public int SetInStock(int supplierID, bool inStock, int categoryID, bool showIfNotInStock)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isInStock=@isInStock", objConn))
                {
                    if (!showIfNotInStock)
                        objComm.CommandText += ", isActive = @isInStock";
                    objComm.CommandText += " WHERE supplierID=@supplierID AND isLocked=0 AND productID IN (SELECT productID FROM productCategory WHERE categoryID=@categoryID)";
                    objConn.Open();
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = inStock;
                    objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SetLocked(int productID, bool isLocked)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isLocked=@isLocked WHERE productID=@productID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@isLocked", SqlDbType.Bit).Value = isLocked;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SetIsInStock(int productID, bool isInStock)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isInStock=@isInStock WHERE productID=@productID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isInStock;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int UpdatePriceAndStock(int productID, double price, double webPrice, bool isIsInStock, bool showIfNotInStock)
        {
            int status = 0;
            using(SqlConnection objConn=new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET price=@price, isInStock=@isInStock, webPrice=@webPrice", objConn))
                {
                    if(!showIfNotInStock)
                        objComm.CommandText += ", isActive = @isInStock";
                    objComm.CommandText += " WHERE productID=@productID";
                    objConn.Open();
                    objComm.Parameters.Add("@price", SqlDbType.Float).Value = price;
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isIsInStock;
                    objComm.Parameters.Add("@webPrice", SqlDbType.Float).Value = webPrice;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveProductCategory(int productID, int categoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_saveCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@isMainCategory", SqlDbType.Bit).Value = false;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveProductVariation(int productID, int variationID, string value, string color)
        {
            return 0;
        }

        #endregion SaveProduct

        #region DeleteProduct

        public int DeleteProduct(int productID)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM product WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting product", ex);
                    }
                }
            }
            return status;
        }

        private int DeleteProductAttributes(int productID)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM productAttributeValue WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting product attributes", ex);
                    }
                }
            }
            return status;
        }

        private int DeleteProductCategories(int productID)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM productCategory WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch(SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting product categories", ex);
                    }
                }
            }
            return status;
        }

        private int DeleteProductImages(int productID)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM productImageUrl WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting product images", ex);
                    }
                }
            }
            return status;
        }

        private int DeleteProductPromotions(int productID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM promotionProduct WHERE productID=@productID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        #endregion DeleteProduct

        #region GetProduct

        public Product GetProduct(int productID, string url, bool count, string code)
        {
            Product product = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT productID, code, supplierCode, brand.brandID, product.name, description, price, webPrice, brand.name, isApproved, isActive, insertDate, updateDate, vatID, supplierID, specification, isLocked, isInStock, ean, product.supplierPrice, unitOfMeasureID FROM product INNER JOIN brand ON product.brandID=brand.brandID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        if (productID > 0)
                        {
                            objComm.CommandText += " WHERE productID=@productID";
                            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        }
                        else if (url != string.Empty)
                        {
                            objComm.CommandText += " WHERE url=@url";
                            objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = url;
                        }
                        else if(code != string.Empty)
                        {
                            objComm.CommandText += " WHERE code = @code";
                            objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = code;
                        }

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                product = new Product();

                            while (reader.Read())
                            {
                                product.ProductID = reader.GetInt32(0);
                                product.Code = reader.GetString(1);
                                product.SupplierCode = reader.GetString(2);
                                product.Brand = new Brand(reader.GetInt32(3), reader.GetString(8));
                                product.Name = reader.GetString(4);
                                product.Description = reader.GetString(5);
                                product.Price = reader.GetDouble(6);
                                product.WebPrice = reader.GetDouble(7);
                                product.IsApproved = reader.GetBoolean(9);
                                product.IsActive = reader.GetBoolean(10);
                                product.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(11));
                                product.UpdateDate = Common.ConvertToLocalTime(reader.GetDateTime(12));
                                product.VatID = reader.GetInt32(13);
                                product.SupplierID = reader.GetInt32(14);
                                //if (!Convert.IsDBNull(reader[15]))
                                    product.Specification = createProductSpecification(product.ProductID);//reader.GetString(15);
                                product.IsLocked = reader.GetBoolean(16);
                                product.IsInStock = reader.GetBoolean(17);
                                if (!Convert.IsDBNull(reader[18]))
                                    product.Ean = reader.GetString(18);
                                product.Categories = GetProductCategories(product.ProductID);
                                product.Attributes = GetProductAttributes(product.ProductID);
                                product.Images = GetProductImages(product.ProductID);
                                product.Promotion = getPromotions(product.ProductID);
                                if (product.Description == string.Empty)
                                    product.Description = GetProductAttributeValues(product.ProductID, true);
                                if(!Convert.IsDBNull(reader[19]))
                                    product.SupplierPrice = reader.GetDouble(19);
                                if(!Convert.IsDBNull(reader[20]))
                                    product.UnitOfMeasure = new UnitOfMeasureDL().GetUnitOfMeasure(reader.GetInt32(20));

                                //if (product.Specification == string.Empty)
                                    //product.Specification = createProductSpecification(product.ProductID);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading product", ex);
                    }

                    if (product != null && count)
                    {
                        objComm.CommandText = "productAccess_save";
                        objComm.CommandType = CommandType.StoredProcedure;
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = product.ProductID;
                        objComm.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                        objComm.ExecuteNonQuery();
                    }
                }
            }
            return product;
        }

        private List<Category> GetProductCategories(int productID)
        {
            List<Category> categories = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT category.categoryID, name, parentCategoryID, url, imageUrl, sortOrder, active FROM productCategory INNER JOIN category ON productCategory.categoryID=category.categoryID WHERE productID=@productID ORDER BY isMainCategory DESC", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                categories = new List<Category>();
                            while (reader.Read())
                            {
                                categories.Add(new Category(reader.GetInt32(0), reader.GetString(1), !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : -1, reader.GetString(3), reader.GetString(4), reader.GetInt32(5), 0, 0, string.Empty, Convert.IsDBNull(reader[6]) ? false : reader.GetBoolean(6), 0, false, false));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading product categories", ex);
                    }
                }
            }
            return categories;
        }

        private List<AttributeValue> GetProductAttributes(int productID)
        {
            List<AttributeValue> attributes = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT productAttributeValue.attributeValueID, value, attribute.attributeID FROM productAttributeValue INNER JOIN attributeValue ON productAttributeValue.attributeValueID=attributeValue.attributeValueID INNER JOIN attribute ON attributeValue.attributeID=attribute.attributeID INNER JOIN categoryAttribute ON attribute.attributeID = categoryAttribute.attributeID WHERE productID=@productID ORDER BY position, name", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                attributes = new List<AttributeValue>();
                            while (reader.Read())
                            {
                                attributes.Add(new AttributeValue(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), 0, string.Empty, 0));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading product attributes", ex);
                    }
                }
            }
            return attributes;
        }

        public List<ProductImage> GetProductImages(int productID)
        {
            List<ProductImage> images = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT imageUrl, sortOrder FROM productImageUrl WHERE productID=@productID ORDER BY sortOrder", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                images = new List<ProductImage>();
                            while (reader.Read())
                            {
                                //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/" + reader.GetString(0))))
                                    images.Add(new ProductImage(reader.GetString(0), reader.GetInt32(1)));
                                //else
                                    //images.Add("/images/no-image.jpg");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading product images", ex);
                    }
                }
            }
            return images;
        }

        public int GetProductIDBySupplierCode(string supplierCode)
        {
            int productID = -1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT productID FROM product WHERE supplierCode=@supplierCode", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = supplierCode;
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            while (reader.Read())
                                productID = reader.GetInt32(0);
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading product id", ex);
                    }
                }
            }
            return productID;
        }

        private Promotion getPromotions(int productID)
        {
            Promotion promotion = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT promotion.promotionID, price, imageUrl FROM promotionProduct INNER JOIN promotion ON promotionProduct.promotionID=promotion.promotionID WHERE productID=@productID AND promotion.dateFrom<=GETDATE() AND promotion.dateTo>=GETDATE()", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            promotion = new Promotion();
                            promotion.PromotionID = reader.GetInt32(0);
                            promotion.Price = reader.GetDouble(1);
                            promotion.ImageUrl = reader.GetString(2);
                        }
                    }
                }
            }
            return promotion;
        }

        public bool IsLocked(int productID)
        {
            bool isLocked = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT isLocked FROM product WHERE productID=@productID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            isLocked = reader.GetBoolean(0);
                    }
                }
            }
            return isLocked;
        }

        private string createProductSpecification(int productID)
        {
            string specification = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attribute.name, attributeValue.value FROM attribute INNER JOIN attributeValue ON attribute.attributeID=attributeValue.attributeID INNER JOIN productAttributeValue ON attributeValue.attributeValueID=productAttributeValue.attributeValueID INNER JOIN categoryAttribute ON attribute.attributeID=categoryAttribute.attributeID WHERE productAttributeValue.productID=@productID AND attributeValue.value<>'NP' ORDER BY position, attribute.name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    specification += "<table class='table table-striped'><tbody>";
                    string attributeGroup = string.Empty;
                    int i = 0;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0).Contains("-") && attributeGroup != reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-")))
                            {
                                specification += "<tr class='attributeGroup'><td colspan='2'>" + reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-")) + "</td></tr>";
                                attributeGroup = reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-"));
                            }
                            //else
                            //{
                                specification += (i++ % 2 == 0) ? "<tr class='gridAltRow'>" : "<tr class='gridRow'>";
                                specification += "<td class='attributeName'>" + (reader.GetString(0).Contains("-") ? reader.GetString(0).Substring(reader.GetString(0).IndexOf("-")+1) : reader.GetString(0)) + "</td><td>" + reader.GetString(1) + "</td></tr>";
                            //}
                        }
                        
                        specification += "</table>  ";
                    }
                }
            }
            return specification;
        }

        public string GetProductAttributeValues(int productID, bool? isDescription)
        {
            string values = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getProductAttributeValues", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@isDescription", SqlDbType.Bit).Value = isDescription;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            values += reader.GetString(0) + ", ";
                    }
                }
            }
            return (values != string.Empty) ? values.Substring(0, values.Length - 2) : values;
        }

        public double[] GetMinMaxPriceForCategory(int categoryID, bool includeChildrenCategories = false)
        {
            double[] prices = new double[2];

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getMinMaxPriceForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@includeChildrenCategories", SqlDbType.Int).Value = includeChildrenCategories;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prices[0] = !Convert.IsDBNull(reader[0]) ? reader.GetDouble(0) : 0;
                            prices[1] = !Convert.IsDBNull(reader[1]) ? reader.GetDouble(1) : 0;
                        }
                    }
                }
            }
            return prices;
        }

        private string createImageUrl(string url)
        {
            StringBuilder directory = new System.Text.StringBuilder();
            directory.Append("/images/p/");
            string imageName = url.Substring(0, url.IndexOf(".jpg"));
            for (int i = 0; i < imageName.Length; i++)
                directory.Append(imageName[i].ToString() + "/");
            directory.Append(url);

            return directory.ToString();
        }

        public DataTable GetLast10()
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_get_last_10", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        products.Load(reader);
                    }
                }
            }
            return Common.ConvertToLocalTime(ref products);
        }

        public int GetMaxImageID()
        {
            int id = 1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getMaxImageID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            id = reader.GetInt32(0);
                    }
                }
            }
            return id;
        }

        public DataTable GetTop10Access()
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_get_top_10_access", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        products.Load(reader);
                    }
                }
            }
            return products;
        }

        public DataTable GetTop10Order()
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_get_top_10_order", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        products.Load(reader);
                    }
                }
            }
            return products;
        }

        public List<Product> SearchProducts(string search, string sort)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_search", objConn))
                {
                    DataTable searchTable = new DataTable();
                    searchTable.Columns.Add("search");
                    DataRow newRow;
                    foreach (string searchItem in search.Split(' '))
                    { 
                        newRow = searchTable.NewRow();
                        newRow["search"] = searchItem;
                        searchTable.Rows.Add(newRow);
                    }

                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    //objComm.Parameters.Add("@search", SqlDbType.NVarChar, 50).Value = search;
                    objComm.Parameters.AddWithValue("@search", searchTable);
                    objComm.Parameters.Add("@sort", SqlDbType.NVarChar, 50).Value = sort;
                    objComm.Parameters[0].SqlDbType = SqlDbType.Structured;
                    using(SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Code = reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.Description = reader.GetString(3);
                            product.Price = reader.GetDouble(4);
                            product.WebPrice = reader.GetDouble(5);
                            product.Brand = new Brand(reader.GetInt32(6), reader.GetString(7));
                            product.Images = new List<ProductImage>();
                            //string directory = createImageUrl(reader.GetString(8));
                            //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~" + directory)))
                            //{
                            //product.Images.Add(directory);
                            //}
                            //else
                            //product.Images.Add("/images/no-image.jpg");
                            product.Images = GetProductImages(product.ProductID);
                            if (!Convert.IsDBNull(reader[9]))
                            {
                                if (reader.GetDateTime(11) < DateTime.UtcNow && reader.GetDateTime(12) > DateTime.UtcNow)
                                {
                                    product.Promotion = new Promotion();
                                    product.Promotion.Price = reader.GetDouble(9);
                                    product.Promotion.ImageUrl = reader.GetString(10);
                                }
                            }
                            product.Categories = new List<Category>();
                            product.Categories.Add(new CategoryDL().GetCategory(reader.GetInt32(13)));
                            product.IsInStock = reader.GetBoolean(14);
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public void SetPromotionPrice(int productID, int promotionID, double promotionPrice)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("promotionProduct_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                    objComm.Parameters.Add("@promotionPrice", SqlDbType.Float).Value = promotionPrice;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFromPromotion(int productID, int promotionID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_deleteFromPromotion", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public int GetMaxProductImageUrlID()
        {
            int id = 1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("product_getMaxImageID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            id = reader.GetInt32(0);
                    }
                }
            }
            return id;
        }

        public List<int> GetProductIDsByBarcode(string code)
        {
            List<int> productIds = new List<int>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getIdsByBarcode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = code;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            productIds.Add(reader.GetInt32(0));
                    }
                }
            }
            return productIds;
        }

        public List<Product> GetProductsByBarcode(string code)
        {
            List<Product> products = new List<Product>();
            foreach (int productID in GetProductIDsByBarcode(code))
                products.Add(GetProduct(productID, string.Empty, false, string.Empty));
            return products;
        }

        public DataTable GetProductsForExport(int type)
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getForExport", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@insertToKupindoAccess", SqlDbType.Bit).Value = type;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products.Load(reader);
                    }
                }
            }
            return products;
        }

        public string GetProductSpecificationText(int productID)
        {
            string specification = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getAttributes", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        //specification = "<br/>";
                        string attributeGroup = string.Empty;
                        while(reader.Read())
                        {
                            //specification += "<br/>";
                            if (reader.GetString(0).Contains("-") && reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-")) != attributeGroup)
                            { 
                                specification += "<br/>" + encodeText(reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-"))) + "<br/>";
                                attributeGroup = reader.GetString(0).Substring(0, reader.GetString(0).IndexOf("-"));
                            }
                            specification += (reader.GetString(0).Contains("-") ? encodeText(reader.GetString(0).Substring(reader.GetString(0).IndexOf("-") + 1)) : encodeText(reader.GetString(0))) + ": " + encodeText(reader.GetString(1));
                            specification += "<br/>";
                        }
                    }
                }
            }
            
            return specification;
        }

        private string encodeText(string text)
        {
            string[] chars = new string[] { "&", "<", ">", "\"", "'"};
            string[] replace = new string[] { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

            Regex regex;
            for(int i = 0; i < chars.Length; i ++)
            {
                regex = new Regex(chars[i]);
                text = regex.Replace(text, replace[i]);
            }

            return text;
        }

        #endregion GetProduct












        /*public Product GetProduct(int productID)
        {
            return GetProduct(pro
        }*/

        /*public List<Product> GetProduct(string code)
        {
            return GetProducts(-1, code, string.Empty, null, -1);
        }*/

        /*public List<Product> GetProducts(string supplierCode)
        {
            return GetProducts(-1, string.Empty, supplierCode, null);
        }*/
    }
}
