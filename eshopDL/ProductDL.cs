﻿using System;
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
using System.IO;
using System.Configuration;

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
                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, supplierCode, brand.brandID, product.name, description, product.price, webPrice, brand.name, isApproved, isActive, isLocked, isInStock, ISNULL((SELECT TOP 1 price FROM promotionProduct WHERE productID = product.productID), 0) as promotionPrice, insertDate," +
                    "(SELECT ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) + category.url" +
                    " FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" +
                    " LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                    " WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1))" +
                    " FROM product INNER JOIN brand ON product.brandID=brand.brandID", objConn))
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
                                product.Brand = new Brand(reader.GetInt32(3), reader.GetString(8), string.Empty);
                                product.Name = reader.GetString(4);
                                product.Description = reader.GetString(5);
                                product.Price = reader.GetDouble(6);
                                product.WebPrice = reader.GetDouble(7);
                                product.IsApproved = reader.GetBoolean(9);
                                product.IsActive = reader.GetBoolean(10);
                                product.IsLocked = reader.GetBoolean(11);
                                product.IsInStock = reader.GetBoolean(12);
                                product.Images = GetProductImages(product.ProductID);
                                product.Promotion = new Promotion(-1, string.Empty, 0, string.Empty, reader.GetDouble(13), false, DateTime.Now, DateTime.Now, string.Empty, false, string.Empty);
                                product.Categories = new List<Category>();
                                product.Categories.Add(new CategoryDL().GetCategory(categoryID));
                                product.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(14));
                                product.FullCategoryUrl = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty;

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
                //using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, promotion.dateFrom, promotion.dateTo, category.name, product.isInStock, insertDate" +
                //    ", (SELECT ppc.url + (CASE WHEN ppc.url IS NOT NULL THEN '/' ELSE '' END) + pc.url + (CASE WHEN pc.url IS NOT NULL THEN '/' ELSE '' END) + category.url" +
                //    " FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" +
                //    " LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                //    " WHERE category.categoryID = @urlCategoryID)" +
                //    " FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID INNER JOIN productCategory ON product.productID=productCategory.productID LEFT JOIN promotionProduct ON product.productID=promotionProduct.productID LEFT JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN category ON productCategory.categoryID=category.categoryID", objConn))

                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl," + 
                    " promotionProduct.price, promotion.imageUrl, promotion.dateFrom, promotion.dateTo, category.name, product.isInStock, insertDate," +
                        " (SELECT ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) +category.url" +
                        " FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" +
                        " LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                        " WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1))," +
                    " CASE WHEN promotionProduct.price IS NOT NULL THEN promotionProduct.price ELSE webPrice END" +
                    " FROM product INNER JOIN brand ON product.brandID = brand.brandID" +
                    " INNER JOIN productImageUrl ON product.productID = productImageUrl.productID" +
                    " INNER JOIN productCategory ON product.productID = productCategory.productID" +
                    " LEFT JOIN promotionProduct ON product.productID = promotionProduct.productID" +
                    " LEFT JOIN promotion ON promotionProduct.promotionID = promotion.promotionID" +
                    " INNER JOIN category ON productCategory.categoryID = category.categoryID", objConn))
                {
                    if (attributeValues.Count > 0)
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

                    objComm.Parameters.Add("@urlCategoryID", SqlDbType.Int).Value = categoryID;

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

                    objComm.CommandText += " ORDER BY product.isInStock DESC, " + sort;

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
                            product.Brand = new Brand(-1, reader.GetString(6), string.Empty);
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
                            product.Categories.Add(new Category(categoryID, reader.GetString(12), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false, 0, 0, 0));
                            product.Description = GetProductAttributeValues(product.ProductID, true);
                            product.IsInStock = reader.GetBoolean(13);
                            product.InsertDate = !Convert.IsDBNull(reader[14]) ? reader.GetDateTime(14) : DateTime.MinValue;
                            product.FullCategoryUrl = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty;
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public double GetActualPrice(int productID)
        {
            double price = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getActualPrice", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            price = reader.GetDouble(0);
                    }
                }
            }
            return price;
        }

        public List<Product> GetProductsForPromotion(int promotionID)
        {
            List<Product> products = null;
            Product product=null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT product.productID, product.code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, category.name, category.categoryID, product.isInStock, promotion.value, promotion.name" +
                    ", (SELECT ppc.url + (CASE WHEN ppc.url IS NOT NULL THEN '/' ELSE '' END) + pc.url + (CASE WHEN pc.url IS NOT NULL THEN '/' ELSE '' END) + category.url" +
                    " FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" +
                    " LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                    " WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1))" +
                    " FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID INNER JOIN promotionProduct ON product.productID=promotionProduct.productID INNER JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN productCategory ON productCategory.productID=product.productID INNER JOIN category ON productCategory.categoryID=category.categoryID WHERE promotion.promotionID=@promotionID AND isActive=1 AND isApproved=1 AND productImageUrl.sortOrder=1 and isMainCategory = 1 ORDER BY product.name", objConn))
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
                            product.Brand = new Brand(-1, reader.GetString(6), string.Empty);
                            product.Images = new List<ProductImage>();
                            //if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/images/" + reader.GetString(7))))
                                product.Images.Add(new ProductImage(reader.GetString(7), 1));
                            //else
                                //product.Images.Add("/images/no-image.jpg");
                            product.Promotion = new Promotion();
                            product.Promotion.Price = reader.GetDouble(8);
                            product.Promotion.ImageUrl = reader.GetString(9);
                            product.Promotion.Value = reader.GetDouble(13);
                            product.Promotion.Name = reader.GetString(14);
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(reader.GetInt32(11), reader.GetString(10), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false, 0, 0, 0));
                            product.IsInStock = reader.GetBoolean(12);
                            product.FullCategoryUrl = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty;

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
                using (SqlCommand objComm = new SqlCommand("SELECT TOP " + numberOfProducts.ToString() + " product.productID, product.code, product.name, product.description, product.price, webPrice, brand.name, productImageUrl.imageUrl, promotionProduct.price, promotion.imageUrl, promotion.dateFrom, promotion.dateTo, category.name, product.isInStock, promotion.name" +
                    ", (SELECT ppc.url + (CASE WHEN ppc.url IS NOT NULL THEN '/' ELSE '' END) + pc.url + (CASE WHEN pc.url IS NOT NULL THEN '/' ELSE '' END) + category.url" +
                    " FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" +
                    " LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                    " WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory = 1))" +
                    " FROM product INNER JOIN brand ON product.brandID=brand.brandID INNER JOIN productImageUrl ON product.productID=productImageUrl.productID LEFT JOIN promotionProduct ON product.productID=promotionProduct.productID LEFT JOIN promotion ON promotionProduct.promotionID=promotion.promotionID INNER JOIN productCategory ON product.productID=productCategory.productID INNER JOIN category ON productCategory.categoryID=category.categoryID ", objConn))
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
                    objComm.CommandText += " AND isActive = 1 AND isApproved = 1 AND productImageUrl.sortOrder = 1 AND isInStock = 1";
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
                            product.Brand = new Brand(-1, reader.GetString(6), string.Empty);
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
                                    product.Promotion.Name = reader.GetString(14);
                                }
                            }
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(categoryID, reader.GetString(12), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, true, 0, false, false, 0, 0, 0));
                            product.Description = GetProductAttributeValues(product.ProductID, true);
                            product.IsInStock = reader.GetBoolean(13);
                            product.Attributes = GetProductAttributes(product.ProductID);
                            product.FullCategoryUrl = !Convert.IsDBNull(reader[15]) ? reader.GetString(15) : string.Empty;

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

        public DataTable GetProductsDataTable(int? categoryID, int? supplierID, int? promotionID, int? brandID, bool? isActive, bool? isApproved, string search, string sort, string reverse, bool? hasImage, bool? isInStock = null)
        {
            DataTable products = new DataTable();
            //products.Columns.Add("productID", typeof(int));
            //products.Columns.Add("code", typeof(string));
            //products.Columns.Add("name", typeof(string));
            //products.Columns.Add("imageUrl", typeof(string));
            //products.Columns.Add("price", typeof(double));
            //products.Columns.Add("webPrice", typeof(double));
            //products.Columns.Add("promotionPrice", typeof(double));
            //products.Columns.Add("isActive", typeof(bool));
            //products.Columns.Add("isApproved", typeof(bool));
            //products.Columns.Add("isLocked", typeof(bool));
            //products.Columns.Add("isInStock", typeof(bool));
            //products.Columns.Add("brandName", typeof(string));
            //products.Columns.Add("insertDate", typeof(DateTime));

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;
                    objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = isActive;
                    objComm.Parameters.Add("@isApproved", SqlDbType.Bit).Value = isApproved;
                    objComm.Parameters.Add("@search", SqlDbType.NVarChar, 200).Value = search;
                    objComm.Parameters.Add("@sort", SqlDbType.NVarChar, 200).Value = sort;
                    objComm.Parameters.Add("@reverse", SqlDbType.NVarChar, 200).Value = reverse;
                    objComm.Parameters.Add("@hasImage", SqlDbType.Bit).Value = hasImage;
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isInStock;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        products.Load(reader);
                    }
                }
            }
            return products;
        }

        public List<Product> GetProductForSiteMap(int categoryID)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getForSitemap", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Product product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(-1, reader.GetString(3), -1, string.Empty, string.Empty, 0, 0, 0, string.Empty, false, 0, false, false, 0, 0, 0));
                            product.Name = reader.GetString(1);
                            product.Brand = new Brand(-1, reader.GetString(2), string.Empty);
                            product.FullCategoryUrl = !Convert.IsDBNull(reader[4]) ? reader.GetString(4) : string.Empty;
                            product.Description = !Convert.IsDBNull(reader[5]) ? reader.GetString(5) : string.Empty;
                            product.Description = product.Description.Replace("<p>", string.Empty).Replace("</p>", string.Empty).Trim();
                            product.ListDescription = !Convert.IsDBNull(reader[6]) ? reader.GetString(6) : string.Empty;

                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        #endregion

        #region SaveProduct

        public int SaveProduct(Product product)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO product (code, supplierCode, brandID, name, description, price, webPrice, isApproved, isActive, supplierID, vatID, insertDate, updateDate, specification, isLocked, isInStock, ean, supplierPrice, unitOfMeasureID, isPriceLocked, declaration, weight, weightRangeID, comment, canBeDelivered, shortDescription, isFreeDelivery, listDescription, sortIndex) VALUES (@code, @supplierCode, @brandID, @name, @description, @price, @webPrice, @isApproved, @isActive, @supplierID, @vatID, @insertDate, @updateDate, @specification, @isLocked, @isInStock, @ean, @supplierPrice, @unitOfMeasureID, @isPriceLocked, @declaration, @weight, @weightRangeID, @comment, @canBeDelivered, @shortDescription, @isFreeDelivery, @listDescription, @sortIndex); SELECT SCOPE_IDENTITY()", objConn))
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
                        objComm.Parameters.Add("@isPriceLocked", SqlDbType.Bit).Value = product.IsPriceLocked;
                        objComm.Parameters.Add("@declaration", SqlDbType.NVarChar, 2000).Value = product.Declaration;
                        objComm.Parameters.Add("@weight", SqlDbType.Float).Value = product.Weight;
                        objComm.Parameters.Add("@weightRangeID", SqlDbType.Int).Value = product.WeightRangeID;
                        if (product.WeightRangeID == null)
                            objComm.Parameters["@weightRangeID"].Value = DBNull.Value;
                        objComm.Parameters.Add("@comment", SqlDbType.NVarChar, 1000).Value = product.Comment;
                        objComm.Parameters.Add("@canBeDelivered", SqlDbType.Bit).Value = product.CanBeDelivered;
                        objComm.Parameters.Add("@shortDescription", SqlDbType.NVarChar, 500).Value = product.ShortDescription;
                        objComm.Parameters.Add("@isFreeDelivery", SqlDbType.Bit).Value = product.IsFreeDelivery;
                        objComm.Parameters.Add("@listDescription", SqlDbType.NVarChar, 200).Value = product.ListDescription;
                        objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = product.SortIndex;

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
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET code=@code, supplierCode=@supplierCode, brandID=@brandID, name=@name, description=@description, price=@price, webPrice=@webPrice, isApproved=@isApproved, isActive=@isActive, supplierID=@supplierID, vatID=@vatID, updateDate=@updateDate, specification=@specification, isLocked=@isLocked, isInStock=@isInStock, ean=@ean, supplierPrice = @supplierPrice, unitOfMeasureID = @unitOfMeasureID, isPriceLocked = @isPriceLocked, declaration = @declaration, weight = @weight, weightRangeID = @weightRangeID, comment = @comment, canBeDelivered = @canBeDelivered, shortDescription = @shortDescription, isFreeDelivery = @isFreeDelivery, listDescription = @listDescription, sortIndex = @sortIndex WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        if (product.Specification == string.Empty || product.Specification == null)
                            objComm.CommandText = "UPDATE product SET code=@code, supplierCode=@supplierCode, brandID=@brandID, name=@name, description=@description, price=@price, webPrice=@webPrice, isApproved=@isApproved, isActive=@isActive, supplierID=@supplierID, vatID=@vatID, updateDate=@updateDate, isLocked=@isLocked, isInStock=@isInStock, ean=@ean, supplierPrice = @supplierPrice, unitOfMeasureID = @unitOfMeasureID, isPriceLocked = @isPriceLocked, declaration = @declaration, weight = @weight, weightRangeID = @weightRangeID, comment = @comment, canBeDelivered = @canBeDelivered, shortDescription = @shortDescription, isFreeDelivery = @isFreeDelivery, listDescription = @listDescription, sortIndex = @sortIndex WHERE productID=@productID";

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
                        objComm.Parameters.Add("@isPriceLocked", SqlDbType.Bit).Value = product.IsPriceLocked;
                        objComm.Parameters.Add("@declaration", SqlDbType.NVarChar, 2000).Value = product.Declaration;
                        objComm.Parameters.Add("@weight", SqlDbType.Float).Value = product.Weight;
                        //objComm.Parameters.Add("@weightRangeID", SqlDbType.Int).Value = product.WeightRangeID;
                        objComm.Parameters.AddWithValue("@weightRangeID", product.WeightRangeID);
                        if (product.WeightRangeID == null)
                            objComm.Parameters["@weightRangeID"].Value = DBNull.Value;
                        objComm.Parameters.Add("@comment", SqlDbType.NVarChar, 1000).Value = product.Comment;
                        objComm.Parameters.Add("@canBeDelivered", SqlDbType.Bit).Value = product.CanBeDelivered;
                        objComm.Parameters.Add("@shortDescription", SqlDbType.NVarChar, 500).Value = product.ShortDescription;
                        objComm.Parameters.Add("@isFreeDelivery", SqlDbType.Bit).Value = product.IsFreeDelivery;
                        objComm.Parameters.Add("@listDescription", SqlDbType.NVarChar, 200).Value = product.ListDescription;
                        objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = product.SortIndex;

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
                    objComm.CommandText += " WHERE supplierID=@supplierID AND isLocked=0";

                    if(categoryID > 0)
                    {
                        objComm.CommandText += "  AND productID IN (SELECT productID FROM productCategory WHERE categoryID=@categoryID)";
                    }

                    objConn.Open();
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = inStock;
                    objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;
                    if(categoryID > 0)
                    {
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    }

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
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET isInStock=@isInStock, updateDate=@updateDate", objConn))
                {
                    if(!bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]))
                    {
                        objComm.CommandText += ", isActive = @isActive";
                        objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = isInStock;
                    }
                    objComm.CommandText += " WHERE productID=@productID";
                    objConn.Open();
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isInStock;
                    objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = DateTime.UtcNow;
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
                using (SqlCommand objComm = new SqlCommand("UPDATE product SET price=@price, isInStock=@isInStock, webPrice=@webPrice, updateDate=@updateDate", objConn))
                {
                    if(!showIfNotInStock)
                        objComm.CommandText += ", isActive = @isInStock";
                    objComm.CommandText += " WHERE productID=@productID";
                    objConn.Open();
                    objComm.Parameters.Add("@price", SqlDbType.Float).Value = price;
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isIsInStock;
                    objComm.Parameters.Add("@webPrice", SqlDbType.Float).Value = webPrice;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = DateTime.UtcNow;

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

        public bool UpdateProductImageUrl(int productImageUrlId, string imageUrl)
        {
            int status = -1;

            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    using (SqlCommand objComm = new SqlCommand("product_image_update_url", objConn))
                    {
                        objConn.Open();
                        objComm.CommandType = CommandType.StoredProcedure;
                        objComm.Parameters.Add("@productImageUrlId", SqlDbType.Int).Value = productImageUrlId;
                        objComm.Parameters.Add("@imageUrl", SqlDbType.VarChar, 100).Value = imageUrl;

                        status = objComm.ExecuteNonQuery();     
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex);
                return false;
            }

            return true;
        }

        #endregion SaveProduct

        #region DeleteProduct

        public int DeleteProduct(int productID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM product WHERE productID=@productID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;


                        if (bool.Parse(ConfigurationManager.AppSettings["deleteImagesFilesOnProductDelete"]))
                            deleteProductImagesFiles(productID);
                        status = objComm.ExecuteNonQuery();

                        
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        if (ex.Message.Contains("REFERENCE") && ex.Message.Contains("orderItem"))
                            throw new BLException("Nije moguće obrisati proizvod pošto se nalazi u porudžbenicama.");
                        throw new DLException("Error while deleting product", ex);
                    }
                }
            }
            return status;
        }

        private void deleteProductImagesFiles(int productID)
        {
            List<ProductImage> images = GetProductImages(productID);

            foreach(ProductImage image in images)
            {
                try
                { 
                    string path = createImageUrl(image.ImageUrl);
                    string name = path.Substring(0, path.IndexOf("."));
                    string extension = path.Substring(path.IndexOf("."));

                    try
                    { 
                        File.Delete(HttpContext.Current.Server.MapPath("~/" + path));
                    }
                    catch(Exception ex)
                    {
                        ErrorLog.LogError(ex);
                    }

                    try
                    { 
                        File.Delete(HttpContext.Current.Server.MapPath("~/" + name + "-" + ConfigurationManager.AppSettings["mainName"] + extension));
                    }
                    catch(Exception ex)
                    {
                        ErrorLog.LogError(ex);
                    }

                    try
                    { 
                        File.Delete(HttpContext.Current.Server.MapPath("~/" + name + "-" + ConfigurationManager.AppSettings["listName"] + extension));
                    }
                    catch(Exception ex)
                    {
                        ErrorLog.LogError(ex);
                    }

                    try
                    { 
                        File.Delete(HttpContext.Current.Server.MapPath("~/" + name + "-" + ConfigurationManager.AppSettings["thumbName"] + extension));
                    }
                    catch(Exception ex)
                    {
                        ErrorLog.LogError(ex);
                    }
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }
            }
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
                string query = $"SELECT " +
                    $"product.productID," +
                    $"code," +
                    $"supplierCode," +
                    $"brand.brandID," +
                    $"product.name," +
                    $"description," +
                    $"price," +
                    $"webPrice," +
                    $"brand.name," +
                    $"isApproved," +
                    $"isActive," +
                    $"insertDate," +
                    $"updateDate," +
                    $"vatID," +
                    $"supplierID," +
                    $"specification," +
                    $"isLocked," +
                    $"isInStock," +
                    $"ean," +
                    $"product.supplierPrice," +
                    $"unitOfMeasureID," +
                    $"brand.logoUrl," +
                    $"(SELECT ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') +" +
                    $"(CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) +" +
                    $"ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') +" +
                    $"(CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) +" +
                    $"category.url " +
                    $"FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID " +
                    $"LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID " +
                    $"WHERE category.categoryID = (SELECT TOP 1 categoryID FROM productCategory WHERE productID = product.productID AND isMainCategory= 1)), " +
                    $"isPriceLocked," +
                    $"declaration," +
                    $"weight," +
                    $"weightRangeID," +
                    $"comment," +
                    $"canBeDelivered, " +
                    $"CAST(CASE WHEN EXISTS(SELECT * FROM productVariant WHERE productID = product.productID) THEN 1 ELSE 0 END as bit), " +
                    $"shortDescription, " +
                    $"isFreeDelivery, " +
                    $"manualUrl, " +
                    $"listDescription, " +
                    $"sortIndex " +
                    $"FROM product INNER JOIN brand ON product.brandID = brand.brandID " +
                    $"INNER JOIN productCategory ON product.productID = productCategory.productID";

                //using (SqlCommand objComm = new SqlCommand("SELECT product.productID, code, supplierCode, brand.brandID, product.name, description, price, webPrice, brand.name, isApproved, isActive, insertDate, updateDate, vatID, supplierID, specification, isLocked, isInStock, ean, product.supplierPrice, unitOfMeasureID, brand.logoUrl" +
                    //", (SELECT ppc.url + (CASE WHEN ppc.url IS NOT NULL THEN '/' ELSE '' END) + pc.url + (CASE WHEN pc.url IS NOT NULL THEN '/' ELSE '' END) + category.url" +
                        //" FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID" + 
                        //" LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID" +
                        //" WHERE category.categoryID = productCategory.categoryID), isPriceLocked, declaration, weight, weightRangeID, comment, canBeDelivered" +
                    //" FROM product INNER JOIN brand ON product.brandID=brand.brandID" +
                    //" INNER JOIN productCategory ON product.productID = productCategory.productID", objConn))
                using (SqlCommand objComm = new SqlCommand(query, objConn))
                {
                    try
                    {
                        objConn.Open();

                        

                        if (productID > 0)
                        {
                            objComm.CommandText += " WHERE product.productID=@productID";
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

                        objComm.CommandText += " AND productCategory.isMainCategory = 1";

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                product = new Product();

                            while (reader.Read())
                            {
                                product.ProductID = reader.GetInt32(0);
                                product.Code = reader.GetString(1);
                                product.SupplierCode = reader.GetString(2);
                                product.Brand = new Brand(reader.GetInt32(3), reader.GetString(8), !Convert.IsDBNull(reader[21]) ? reader.GetString(21) : string.Empty);
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
                                product.FullCategoryUrl = !Convert.IsDBNull(reader[22]) ? reader.GetString(22) : string.Empty;
                                product.IsPriceLocked = !Convert.IsDBNull(reader[23]) ? reader.GetBoolean(23) : false;
                                product.Declaration = !Convert.IsDBNull(reader[24]) ? reader.GetString(24) : string.Empty;
                                product.Weight = !Convert.IsDBNull(reader[25]) ? reader.GetDouble(25) : 0;
                                product.WeightRangeID = !Convert.IsDBNull(reader[26]) ? reader.GetInt32(26) : -1;
                                product.Comment = !Convert.IsDBNull(reader[27]) ? reader.GetString(27) : string.Empty;
                                product.CanBeDelivered = !Convert.IsDBNull(reader[28]) ? reader.GetBoolean(28) : false;
                                product.HasVariants = !Convert.IsDBNull(reader[29]) ? reader.GetBoolean(29) : false;
                                product.ShortDescription = !Convert.IsDBNull(reader[30]) ? reader.GetString(30) : string.Empty;
                                product.IsFreeDelivery = !Convert.IsDBNull(reader[31]) ? reader.GetBoolean(31) : false;
                                product.ManualUrl = !Convert.IsDBNull(reader[32]) ? reader.GetString(32) : string.Empty;
                                product.ListDescription = !Convert.IsDBNull(reader[33]) ? reader.GetString(33) : string.Empty;
                                product.SortIndex = !Convert.IsDBNull(reader[34]) ? reader.GetInt32(34) : 0;

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
                                categories.Add(new Category(reader.GetInt32(0), reader.GetString(1), !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : -1, reader.GetString(3), reader.GetString(4), reader.GetInt32(5), 0, 0, string.Empty, Convert.IsDBNull(reader[6]) ? false : reader.GetBoolean(6), 0, false, false, 0, 0, 0));
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
                using (SqlCommand objComm = new SqlCommand("SELECT attribute.name, attributeValue.value FROM attribute INNER JOIN attributeValue ON attribute.attributeID=attributeValue.attributeID INNER JOIN productAttributeValue ON attributeValue.attributeValueID=productAttributeValue.attributeValueID INNER JOIN categoryAttribute ON attribute.attributeID=categoryAttribute.attributeID WHERE productAttributeValue.productID=@productID AND attributeValue.value<>'NP' AND categoryAttribute.categoryID = (SELECT categoryID FROM productCategory WHERE productID = @productID AND isMainCategory=1) ORDER BY position, attribute.name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    
                    string attributeGroup = string.Empty;
                    int i = 0;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            specification += "<table class='table table-striped table-condensed'><tbody>";
                        }

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

                        if(reader.HasRows)
                        {
                            specification += "</tbody></table>";
                        }
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
            string imageName = url.Substring(0, url.IndexOf("."));
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

        public List<Product> SearchProducts(string search, string sort, int? categoryID, int productCountLimit)
        {
            List<Product> products = new List<Product>();
            List<string> restrictedSearchItems = new List<string>() { };
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_search_new", objConn))
                {
                    DataTable searchTable = new DataTable();
                    searchTable.Columns.Add("search");
                    DataRow newRow;
                    newRow = searchTable.NewRow();
                    newRow["search"] = search;
                    searchTable.Rows.Add(newRow);
                    foreach (string searchItem in search.Split(' '))
                    { 
                        if(searchItem.Length > 1 && !restrictedSearchItems.Contains(searchItem))
                        { 
                            newRow = searchTable.NewRow();
                            newRow["search"] = searchItem;
                            searchTable.Rows.Add(newRow);
                        }
                    }

                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    //objComm.Parameters.Add("@search", SqlDbType.NVarChar, 50).Value = search;
                    objComm.Parameters.AddWithValue("@search", searchTable);
                    objComm.Parameters.Add("@sort", SqlDbType.NVarChar, 50).Value = sort;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID > 0 ? categoryID : null;
                    objComm.Parameters.Add("@productCountLimit", SqlDbType.Int).Value = productCountLimit;
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
                            product.Brand = new Brand(reader.GetInt32(6), reader.GetString(7), string.Empty);
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
                            product.FullCategoryUrl = reader.GetString(15);
                            product.CanBeDelivered = !Convert.IsDBNull(reader[16]) ? reader.GetBoolean(16) : false;
                            product.HasVariants = !Convert.IsDBNull(reader[17]) ? reader.GetBoolean(17) : true;
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

        public DataTable GetProductsDataTable()
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getAll", objConn))
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

        public bool ChangeCategory(int productID, int newCategoryID)
        {
            bool status = false;
            try
            { 
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    using (SqlCommand objComm = new SqlCommand("product_changeCategory", objConn))
                    {
                        objConn.Open();
                        objComm.CommandType = CommandType.StoredProcedure;
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                        objComm.Parameters.Add("@newCategoryID", SqlDbType.Int).Value = newCategoryID;

                        objComm.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(SqlException ex)
            {
                ErrorLog.LogError(ex);
                throw new DLException("Error: ProductChangeCategory", ex);
            }
        }

        public bool ImageExistsInDatabase(string filename)
        {
            bool status = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("image_existsInDatabase", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@filename", SqlDbType.VarChar, 100).Value = filename;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            status = true;
                    }
                }
            }
            return status;
        }

        public DataTable ImagesTableExistsInDatabase(DataTable images)
        {
            DataTable dtImages = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("images_table_existsInDatabase", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@images", SqlDbType.Structured).Value = images;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        dtImages.Load(reader);
                    }
                }
            }
            return dtImages;
        }

        public bool IsInStock(int productID)
        {
            bool isInStock = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_isInStock", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while(reader.Read())
                            isInStock = reader.GetBoolean(0);
                    }
                }
            }
            return isInStock;
        }

        public void SetPriceLocked(int productID, bool priceLocked)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_setPriceLocked", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@isPriceLocked", SqlDbType.Bit).Value = priceLocked;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public bool IsPriceLocked(int productID)
        {
            bool isPriceLocked = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_isPriceLocked", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        while (reader.Read())
                            isPriceLocked = reader.GetBoolean(0);
                }
            }
            return isPriceLocked;
        }

        public ProductUpdatePrice GetProductBySupplierCode(string supplierCode)
        {
            ProductUpdatePrice productUpdatePrice = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productUpdatePrice_getBySupplierCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 50).Value = supplierCode;
                    
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            productUpdatePrice = new ProductUpdatePrice();
                            productUpdatePrice.ID = reader.GetInt32(0);
                            productUpdatePrice.BrandID = reader.GetInt32(1);
                            productUpdatePrice.IsLocked = reader.GetBoolean(2);
                            productUpdatePrice.IsPriceLocked = !Convert.IsDBNull(reader[3]) ? reader.GetBoolean(3) : false;
                        }
                    }
                }
            }
            return productUpdatePrice;
        }

        public ProductUpdatePrice GetProductBySupplierAndProductCode(string supplierCode, string supplierProductCode)
        {
            ProductUpdatePrice productUpdatePrice = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productUpdatePrice_getBySupplierAndProductCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = supplierCode;
                    objComm.Parameters.Add("@supplierProductCode", SqlDbType.VarChar, 50).Value = supplierProductCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            productUpdatePrice = new ProductUpdatePrice();
                            productUpdatePrice.ID = reader.GetInt32(0);
                            productUpdatePrice.BrandID = reader.GetInt32(1);
                            productUpdatePrice.IsLocked = reader.GetBoolean(2);
                            productUpdatePrice.IsPriceLocked = !Convert.IsDBNull(reader[3]) ? reader.GetBoolean(3) : false;
                            productUpdatePrice.CategoryID = !Convert.IsDBNull(reader[4]) ? reader.GetInt32(4) : -1;
                        }
                    }
                }
            }

            return productUpdatePrice;
        }

        public List<ProductSimple> GetProductsByNameAndCode(string name)
        {
            List<ProductSimple> list = new List<ProductSimple>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getProductsByNameAndCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            list.Add(new ProductSimple(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                        }
                    }
                }
            }
            return list;
        }

        public void CopyData(int oldID, int newID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_copy_data", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@old_id", SqlDbType.Int).Value = oldID;
                    objComm.Parameters.Add("@new_id", SqlDbType.Int).Value = newID;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetAllProductImages(bool excludeNotApproved = true, bool excludeNotActive = true)
        {
            DataTable images = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_images_get_all", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@excludeNotApproved", SqlDbType.Bit).Value = excludeNotApproved;
                    objComm.Parameters.Add("@excludeNotActive", SqlDbType.Bit).Value = excludeNotActive;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        images.Load(reader);
                    }
                }
            }
            return images;
        }

        public string GetNewProductCode(int categoryID)
        {
            string code = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_getNewProductCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            code = reader.GetString(0);
                        }
                    }
                }
            }

            return code;
        }

        public void SetSortIndex(int productID, int sortIndex)
        {
            using(SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("product_setSortIndex", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = sortIndex;

                    objComm.ExecuteNonQuery();                    
                }
            }
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
