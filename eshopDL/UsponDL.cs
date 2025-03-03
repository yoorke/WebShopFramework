using eshopDL.Interfaces;
using eshopUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;

namespace eshopDL
{
    public class UsponDL// : IProductImportDL
    {
        public XmlDocument GetXml(string category, string subcategory, bool images, bool attributes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                string url = ConfigurationManager.AppSettings["usponConnectionString"] + "?korisnickoIme=" + ConfigurationManager.AppSettings["korisnickoIme"] + "&lozinka=" + ConfigurationManager.AppSettings["lozinka"];
                if (images)
                    url += "&slike=1";
                if (attributes)
                    url += "&opis=1";

                xmlDoc.Load(url);
            }
            catch(Exception ex)
            {
                //return null;
                ErrorLog.LogError(ex);
            }

            return xmlDoc;
        }

        public DataTable GetUsponCategories(int? parentCategoryID, int? categoryID)
        {
            DataTable categories = new DataTable();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("usponCategory_get", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@parentID", SqlDbType.Int).Value = parentCategoryID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        categories.Load(reader);
                    }
                }
            }

            return categories;
        }

        public int SaveCategory(string category, string parentCategory)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspo.category_save", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@category", SqlDbType.NVarChar, 100).Value = category;
                    objComm.Parameters.Add("@parentCategory", SqlDbType.NVarChar, 100).Value = parentCategory;
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }

            return status;
        }

        public int SaveSelected(string[] categoryIDs, string[] selected)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.category_save_selected", objConn))
                {
                    objConn.Open();

                    for(int i = 0; i < categoryIDs.Length; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@selected", SqlDbType.Bit).Value = selected[i];
                        objComm.Parameters.Add("@usponCategoryID", SqlDbType.Int).Value = categoryIDs[i];
                        status = objComm.ExecuteNonQuery();
                    }
                }
            }

            return status;
        }

        public DataTable GetNewCategories()
        {
            DataTable categories = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.category_get_new", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            categories = new DataTable();
                            categories.Load(reader);
                        }
                    }
                }
            }

            return categories;
        }

        public int GetUsponCategoryForCategory(int categoryID)
        {
            int usponCategoryID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.category_get_by_categoryID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            usponCategoryID = reader.GetInt32(0);
                    }
                }
            }

            return usponCategoryID;
        }

        public int SaveUsponCategoryForCategory(int categoryID, int usponCategoryID, bool isCategory)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.save_uspon_category_for_category", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@usponCategoryID", SqlDbType.Int).Value = usponCategoryID;
                    objComm.Parameters.Add("@isCategory", SqlDbType.Bit).Value = isCategory;

                    status = objComm.ExecuteNonQuery();
                }
            }

            return status;
        }

        public int DeleteUsponCategories()
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.delete_categories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }

            return status;
        }

        public int DeleteCategoryUsponCategory(int categoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.delete_category_uspon_category", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }

            return status;
        }

        public int SaveProducts(DataTable products, string usponCategory, int categoryID, int usponCategoryID)
        {
            deleteUsponProducts(usponCategory, categoryID, usponCategoryID);

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "uspon.product";
                    sqlBulkCopy.ColumnMappings.Add(0, "code");
                    sqlBulkCopy.ColumnMappings.Add(1, "brand");
                    sqlBulkCopy.ColumnMappings.Add(2, "name");
                    sqlBulkCopy.ColumnMappings.Add(3, "price");
                    sqlBulkCopy.ColumnMappings.Add(4, "priceRebate");
                    sqlBulkCopy.ColumnMappings.Add(5, "vat");
                    sqlBulkCopy.ColumnMappings.Add(6, "category");
                    sqlBulkCopy.ColumnMappings.Add(7, "ean");
                    sqlBulkCopy.ColumnMappings.Add(8, "images");
                    sqlBulkCopy.ColumnMappings.Add(9, "specification");
                    sqlBulkCopy.ColumnMappings.Add(10, "subcategory");

                    sqlBulkCopy.WriteToServer(products);
                }
            }

            return products.Rows.Count;
        }

        private bool deleteUsponProducts(string usponCategory, int categoryID, int usponCategoryID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.uspon_products_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@usponCategory", SqlDbType.NVarChar, 50).Value = usponCategory;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@usponCategoryID", SqlDbType.Int).Value = usponCategoryID;

                    objComm.ExecuteNonQuery();
                }
            }

            return true;
        }

        public DataTable GetProducts(string category, string[] subcategories)
        {
            DataTable products = new DataTable();
            DataTable subcategoryProducts = new DataTable();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.product_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    for(int i = 0; i < subcategories.Length; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@category", SqlDbType.NVarChar, 50).Value = category;
                        objComm.Parameters.Add("@subcategory", SqlDbType.NVarChar, 50).Value = subcategories[i];

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            subcategoryProducts.Rows.Clear();
                            subcategoryProducts.Load(reader);
                            products.Merge(subcategoryProducts);
                        }
                    }
                }
            }

            return products;
        }

        public DataTable GetProductBySupplierCode(string supplierCode)
        {
            DataTable product = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.product_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.NVarChar, 50).Value = supplierCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        product.Load(reader);
                    }
                }
            }

            return product;
        }

        public string[] GetUsponCategory(int usponCategoryID)
        {
            string[] usponCategory = new string[2] { "0", "0" };
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("uspon.uspon_category_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@usponCategoryID", SqlDbType.Int).Value = usponCategoryID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            usponCategory[0] = reader.GetInt32(0).ToString();
                            usponCategory[1] = reader.GetString(1);
                        }
                    }
                }
            }

            return usponCategory;
        }
    }
}
