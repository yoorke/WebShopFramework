using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace eshopDL
{
    public class KimtecDL
    {
        public int DeleteKimtecCategory(int? kimtecCategoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategory_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecCategoryID",SqlDbType.Int).Value=kimtecCategoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveCategory(int categoryID, string name, int parentID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategory_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = name;
                    objComm.Parameters.Add("@parentID", SqlDbType.Int).Value = parentID;
                    objComm.Parameters.Add("@insertDate", SqlDbType.DateTime).Value = DateTime.Now;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveCategories(DataTable categories)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategories_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.AddWithValue("@kimtecCategoryTable", categories);
                    objComm.ExecuteNonQuery();
                }
            }
            return 0;
        }

        public DataTable GetCategories(int? parentID, int? categoryID)
        {
            DataTable categories = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategory_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@parentID", SqlDbType.Int).Value = parentID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        categories.Load(reader);
                }
            }
            return categories;
        }

        private rs.kimtec.b2b.B2BProductService getService()
        {
            rs.kimtec.b2b.B2BProductService b2b = new rs.kimtec.b2b.B2BProductService();
            X509Certificate cert = new X509Certificate(HttpContext.Current.Server.MapPath("~/b2b_pin_servis_stkr.p12"), "1046", X509KeyStorageFlags.MachineKeySet);
            
            if (cert != null)
                b2b.ClientCertificates.Add(cert);
            
            return b2b;
        }

        public DataSet GetProductsList(string productCode, string productType)
        {
            DataSet products = null;
            //B2BProductService.B2BProductServiceSoapClient b2b = new eshopBL.B2BProductService.B2BProductServiceSoapClient();

            //rs.kimtec.b2b.B2BProductService b2b = getService();
            //b2b.ClientCertificates[0].Reset();
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            //b2b.B2BProductServiceSoapClient b2b = new eshopDL.b2b.B2BProductServiceSoapClient();
            
            rs.kimtec.b2b.B2BProductService b2b = new rs.kimtec.b2b.B2BProductService();
            //b2b.Proxy = new System.Net.WebProxy("rs.kimtec.b2b.B2BProductService", true);
                        
            X509Certificate cert = new X509Certificate(HttpContext.Current.Server.MapPath("~/b2b_pin_servis_stkr.p12"), "1046", X509KeyStorageFlags.MachineKeySet);

            if (cert != null)
                b2b.ClientCertificates.Add(cert);
            products = b2b.GetProductsList(productCode, productType);
            //SaveToFile(products);
            //}
            return products;
        }

        public DataSet GetCategoriesFromKimtec()
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            rs.kimtec.b2b.B2BProductService b2b = new eshopDL.rs.kimtec.b2b.B2BProductService();
            X509Certificate cert = new X509Certificate(HttpContext.Current.Server.MapPath("~/b2b_pin_servis_stkr.p12"), "1046", X509KeyStorageFlags.MachineKeySet);
            if (cert != null)
                b2b.ClientCertificates.Add(cert);
            DataSet categories = b2b.GetCategoriesList(1);
            //SaveToFile(categories);

            DeleteKimtecCategory(null);
            //for (int i = 0; i < categories.Tables[0].Rows.Count; i++)
            //{
                //SaveCategory(int.Parse(categories.Tables[0].Rows[i][0].ToString()), categories.Tables[0].Rows[i][1].ToString(), categories.Tables[0].Rows[i][2].ToString() != string.Empty ? int.Parse(categories.Tables[0].Rows[i][2].ToString()) : 0);
            //}
            SaveCategories(categories.Tables[0]);
            return categories;
        }

        public DataSet GetProductPrice()
        {
            rs.kimtec.b2b.B2BProductService b2b = getService();
            return b2b.GetProductsPriceList(string.Empty, string.Empty, string.Empty, false);
        }

        public DataSet GetProductBarcode()
        {
            rs.kimtec.b2b.B2BProductService b2b = getService();
            return b2b.GetProductsBarcodes(string.Empty);
        }

        public DataSet GetCategoriesForProduct()
        {
            rs.kimtec.b2b.B2BProductService b2b = getService();
            return b2b.GetProductsCategory(1, string.Empty);
        }

        public DataSet GetProductSpecificationFromKimtec()
        {
            rs.kimtec.b2b.B2BProductService b2b = getService();
            return b2b.GetProductsSpecification(string.Empty, string.Empty);
        }

        public int SaveProducts()
        {
            string type = "";
            DataSet products = GetProductsList(string.Empty, type);
            deleteKimtecProducts();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                //using (SqlCommand objComm = new SqlCommand("kimtecProduct_insert", objConn))
                //{
                    //objConn.Open();
                    //objComm.CommandType = CommandType.StoredProcedure;
                    //for (int i = 0; i < products.Tables[0].Rows.Count; i++)
                    //{
                        //objComm.Parameters.Clear();
                        //objComm.Parameters.Add("@code", SqlDbType.NVarChar, 10).Value = products.Tables[0].Rows[i][0].ToString();
                        //objComm.Parameters.Add("@name", SqlDbType.NVarChar, 60).Value = products.Tables[0].Rows[i][1].ToString();
                        //objComm.Parameters.Add("@type", SqlDbType.NVarChar, 255).Value = products.Tables[0].Rows[i][2].ToString();
                        //objComm.Parameters.Add("@brand", SqlDbType.NVarChar, 255).Value = products.Tables[0].Rows[i][3].ToString();
                        //objComm.Parameters.Add("@model", SqlDbType.NVarChar, 40).Value = products.Tables[0].Rows[i][4].ToString();
                        //objComm.Parameters.Add("@partNo", SqlDbType.NVarChar, 50).Value = products.Tables[0].Rows[i][5].ToString();
                        //objComm.Parameters.Add("@warranty", SqlDbType.Int).Value = products.Tables[0].Rows[i][6].ToString();
                        //objComm.Parameters.Add("@technicalDescription", SqlDbType.NVarChar).Value = products.Tables[0].Rows[i][11].ToString();
                        //objComm.Parameters.Add("@marketingDescription", SqlDbType.NVarChar).Value = products.Tables[0].Rows[i][12].ToString();
                        //objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar).Value = products.Tables[0].Rows[i][13].ToString();

                        //objComm.ExecuteNonQuery();

                    //}
                //}

                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "dbo.kimtecProduct";
                    sqlBulkCopy.ColumnMappings.Add(0, "code");
                    sqlBulkCopy.ColumnMappings.Add(1, "name");
                    sqlBulkCopy.ColumnMappings.Add(2, "type");
                    sqlBulkCopy.ColumnMappings.Add(3, "brand");
                    sqlBulkCopy.ColumnMappings.Add(4, "model");
                    sqlBulkCopy.ColumnMappings.Add(5, "partNo");
                    sqlBulkCopy.ColumnMappings.Add(6, "warranty");
                    sqlBulkCopy.ColumnMappings.Add(11, "technicalDescription");
                    sqlBulkCopy.ColumnMappings.Add(12, "marketingDescription");
                    sqlBulkCopy.ColumnMappings.Add(13, "imageUrl");
                    sqlBulkCopy.WriteToServer(products.Tables[0]);
                }
            }
            return products.Tables[0].Rows.Count;
        }

        public int SavePrice()
        {
            DataSet price = GetProductPrice();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecProductPrice_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    //for (int i = 0; i < price.Tables[0].Rows.Count; i++)
                    //{
                        //objComm.Parameters.Clear();
                        //objComm.Parameters.Add("@listPrice", SqlDbType.Float).Value = double.Parse(price.Tables[0].Rows[i][1].ToString());
                        //objComm.Parameters.Add("@discount", SqlDbType.Float).Value = double.Parse(price.Tables[0].Rows[i][2].ToString());
                        //objComm.Parameters.Add("@partnerPrice", SqlDbType.Float).Value = double.Parse(price.Tables[0].Rows[i][3].ToString());
                        //objComm.Parameters.Add("@recommendedPrice", SqlDbType.Float).Value = double.Parse(price.Tables[0].Rows[i][5].ToString());
                        //objComm.Parameters.Add("@availability", SqlDbType.Int).Value = int.Parse(price.Tables[0].Rows[i][6].ToString());
                        //objComm.Parameters.Add("@promotion", SqlDbType.Bit).Value = int.Parse(price.Tables[0].Rows[i][7].ToString()) == 0 ? false : true;
                        //objComm.Parameters.Add("@code", SqlDbType.NVarChar, 10).Value = price.Tables[0].Rows[i][0].ToString();

                        //objComm.ExecuteNonQuery();
                    //}
                    objComm.Parameters.AddWithValue("@kimtecProductPriceTable", price.Tables[0]);
                    objComm.ExecuteNonQuery();
                }
            }
            return price.Tables[0].Rows.Count;
        }

        public int SaveBarcode()
        {
            DataSet barcode = GetProductBarcode();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecProductBarcode_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    //for (int i = 0; i < barcode.Tables[0].Rows.Count; i++)
                    //{
                        //objComm.Parameters.Clear();
                        //objComm.Parameters.Add("@code", SqlDbType.NVarChar, 10).Value = barcode.Tables[0].Rows[i][0].ToString();
                        //objComm.Parameters.Add("@barcodeType", SqlDbType.NVarChar, 50).Value = barcode.Tables[0].Rows[i][1].ToString();
                        //objComm.Parameters.Add("@barcodeValue", SqlDbType.NVarChar, 50).Value = barcode.Tables[0].Rows[i][2].ToString();

                        //objComm.ExecuteNonQuery();
                    //}

                    objComm.Parameters.AddWithValue("@kimtecProductBarcodeTable", barcode.Tables[0]);

                    objComm.ExecuteNonQuery();
                }
            }
            return barcode.Tables[0].Rows.Count;
        }

        public DataTable GetProductsFromDatabase(string[] subcategories, string[] subcategoriesID)
        {
            DataTable subcategoryList = new DataTable();
            subcategoryList.Columns.Add("subcategoryID",typeof(int));
            subcategoryList.Columns.Add("subcategoryName",typeof(string));
            for(int i=0;i<subcategories.Count();i++)
                subcategoryList.Rows.Add(int.Parse(subcategoriesID[i]), subcategories[i]);

            foreach (string subcategoryID in subcategoriesID)
            {
                string[] subCat = getAllSubcategoriesForCategory(int.Parse(subcategoryID));
                foreach (string catID in subCat)
                    subcategoryList.Rows.Add(int.Parse(catID), string.Empty);
            }

            DataTable products = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecProducts_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    SqlParameter subcategory = objComm.Parameters.AddWithValue("@subcategoryList",subcategoryList);
                    subcategory.SqlDbType = SqlDbType.Structured;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            products = new DataTable();
                            products.Load(reader);
                        }
                    }
                }
            }
            return products;
        }

        public int SaveCategoryForProduct()
        {
            DataSet categories = GetCategoriesForProduct();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecProductCategory_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    //for(int i=0;i<categories.Tables[0].Rows.Count;i++)
                    //{
                        //objComm.Parameters.Clear();
                        //objComm.Parameters.Add("@categoryID", SqlDbType.NVarChar, 200).Value = categories.Tables[0].Rows[i]["categoryID"].ToString();
                        //objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = categories.Tables[0].Rows[i]["productCode"];

                        //objComm.ExecuteNonQuery();
                    //}

                    objComm.Parameters.AddWithValue("@kimtecProductCategoryTable", categories.Tables[0]);

                    objComm.ExecuteNonQuery();
                }
            }
            return categories.Tables[0].Rows.Count;
        }

        public int SaveProductSpecification()
        {
            DeleteKimtecSpecification();
            DataSet specification = GetProductSpecificationFromKimtec();
            specification.WriteXml(HttpContext.Current.Server.MapPath("~/specification.xml"),XmlWriteMode.WriteSchema);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                /*using (SqlCommand objComm = new SqlCommand("kimtecProductSpecification_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    for (int i = 0; i < specification.Tables[0].Rows.Count; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = specification.Tables[0].Rows[i]["productCode"].ToString();
                        objComm.Parameters.Add("@specificationGroup", SqlDbType.NVarChar, 255).Value = specification.Tables[0].Rows[i]["specificationGroup"].ToString();
                        objComm.Parameters.Add("@specificationItemNo", SqlDbType.Int).Value = specification.Tables[0].Rows[i]["specificationItemNo"].ToString();
                        objComm.Parameters.Add("@specificationItemName", SqlDbType.NVarChar, 255).Value = specification.Tables[0].Rows[i]["specificationItemName"].ToString();
                        objComm.Parameters.Add("@specificationItemValues", SqlDbType.NVarChar).Value = specification.Tables[0].Rows[i]["specificationItemValues"].ToString();

                        objComm.ExecuteNonQuery();
                    }
                }*/
                
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "dbo.kimtecSpecification";
                    sqlBulkCopy.ColumnMappings.Add("productCode", "productCode");
                    sqlBulkCopy.ColumnMappings.Add("specificationGroup", "specificationGroup");
                    sqlBulkCopy.ColumnMappings.Add("specificationItemNo", "specificationItemNo");
                    sqlBulkCopy.ColumnMappings.Add("specificationItemName", "specificationItemName");
                    sqlBulkCopy.ColumnMappings.Add("specificationItemValues", "specificationItemValues");
                    sqlBulkCopy.WriteToServer(specification.Tables[0]);
                }
            }
            return 1;
        }

        public DataTable GetKimtecSubCategoriesByCategoryID(int categoryID)
        {
            DataTable subcategories = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecSubCategoryByCategoryID_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            subcategories = new DataTable();
                            subcategories.Load(reader);
                        }
                    }
                }
            }
            return subcategories;
        }

        public DataTable GetAllCategories()
        {
            DataTable categories=null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategories_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            categories = new DataTable();
                            categories.Load(reader);
                        }
                    }
                }
            }
            return categories;
        }

        public int SetIsRoot(int kimtecCategoryID, bool isRoot)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategory_setIsRoot", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    objComm.Parameters.Add("@isRoot", SqlDbType.Bit).Value = isRoot;

                    objComm.ExecuteNonQuery();
                }
            }
            return 1;
        }

        public int GetKimtecCategoryForCategory(int categoryID)
        {
            int kimtecCategoryID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getKimtecCategoryForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            kimtecCategoryID = reader.GetInt32(0);
                    }
                }
            }
            return kimtecCategoryID;
        }

        public int SaveKimtecCategoryForCategory(int categoryID, int kimtecCategoryID, bool isCategory)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveKimtecCategoryForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    objComm.Parameters.Add("@isCategory", SqlDbType.Bit).Value = isCategory;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteCategoryKimtecCategory(int categoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteCategoryKimtecCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteKimtecSpecification()
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteKimtecProductSpecification", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetKimtecAttributesForCategory(int kimtecCategoryID)
        {
            DataTable kimtecAttributes = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecAttribute_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            kimtecAttributes = new DataTable();
                            kimtecAttributes.Load(reader);
                        }
                    }
                }
            }
            return kimtecAttributes;
        }

        public DataTable GetCategoriesWithAttribute()
        {
            DataTable kimtecCategories = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategoryWithAttribute_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            kimtecCategories = new DataTable();
                            kimtecCategories.Load(reader);
                        }
                    }
                }
            }
            return kimtecCategories;
        }

        public int SaveKimtecAttribute()
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecAttribute_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveKimtecAttributeForAttribute(int kimtecAttributeID, int attributeID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecAttributeForAttribute_save", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecAttributeID", SqlDbType.Int).Value = kimtecAttributeID;
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        private string[] getAllSubcategoriesForCategory(int categoryID)
        {
            string subcategoriesID = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecGetAllSubcategoriesForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            subcategoriesID += reader.GetInt32(0) + ",";
                    }
                }
            }
            return subcategoriesID.Length > 0 ? subcategoriesID.Substring(0, subcategoriesID.Length - 1).Split(',') : new string[0];
        }

        private int deleteKimtecProducts()
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecProduct_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int GetKimtecCategoryCount()
        {
            int count = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecCategory_getCount", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            count = reader.GetInt32(0);
                    }
                }
            }
            return count;
        }

        public int DeleteKimtecAttributeForAttribute(int categoryID, int kimtecCategoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecAttributeForAttribute_delete", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetSpecificationForProductCode(string productCode)
        {
            DataTable specification = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kimtecSpecification_getByProductCode", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@productCode", SqlDbType.NVarChar, 50).Value = productCode;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            specification = new DataTable();
                            specification.Load(reader);
                        }
                    }
                }
            }
            return specification;
        }

        public DataTable GetProductBySupplierCode(string supplierCode)
        {
            DataTable product = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("kimtecProduct_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = supplierCode;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        product.Load(reader);
                }
            }
            return product;
        }
    }
}
