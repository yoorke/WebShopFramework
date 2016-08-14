using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using eshopBE;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web;
using eshopUtilities;

namespace eshopDL
{
    public class EweDL
    {
        public XmlDocument GetXml(string category, string subcategory, bool images, bool attributes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                /*WebRequest request = WebRequest.Create(@"http://www.ewe.rs/share/backend_231/?user=pinservis&secretcode=754fc&images=1&attributes=1&category=NOTEBOOK");
                WebResponse response = request.GetResponse();
                object xml = response.ResponseUri;*/
                

                //string xml = @"<products><product><id><![CDATA[ NOT04915 ]]></id><manufacturer><![CDATA[ TARGUS ]]></manufacturer><name><![CDATA[ CleanVu cleaning pad TXA002EU ]]></name><category><![CDATA[ NOTEBOOK ]]></category><subcategory><![CDATA[ DODATNA OPREMA ]]></subcategory><price><![CDATA[ 108.09 ]]></price><price_rebate><![CDATA[ 108.09 ]]></price_rebate><vat><![CDATA[ 20 ]]></vat><ean><![CDATA[ 5051794006100 ]]></ean><images><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_v.jpg ]]></image><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_1.jpg ]]></image><image><![CDATA[ http://www.ewe.rs/slike-proizvoda/NOT04915_2.jpg ]]></image></images><specifications><attribute_group name='Karakteristike'><attribute name='Tip'><value><![CDATA[ Cleaning pad ]]></value></attribute></attribute_group><attribute_group name='Fizičke karakteristike'><attribute name='Dimenzije'><value><![CDATA[ 76mm x 76mm ]]></value></attribute><attribute name='Masa'><value><![CDATA[ 44g ]]></value></attribute><attribute name='Boja'><value><!CDATA[ Crna ]]></value></attribute></attribute_group><attribute_group name='Garancija'><attribute name='Garancija'><value><![CDATA[ 2 godine ]]></value></attribute></attribute_group></specifications></product></products>";
                string url = @"http://www.ewe.rs/share/backend_231/?user=pinservis&secretcode=754fc";
                if (images)
                    url += "&images=1";
                if (attributes)
                    url += "&attributes=1";
                if (category != string.Empty)
                    url += "&category=" + HttpUtility.UrlEncode(category);
                if (subcategory != string.Empty)
                    url += "&subcategory=" + HttpUtility.UrlEncode(subcategory);

                //url = url.Substring(0, url.IndexOf("category=") + 9) + System.Web.HttpUtility.UrlEncode(url.Substring(url.IndexOf("category=") + 9));
                xmlDoc.Load(url);
                //xmlDoc.Load(HttpContext.Current.Server.MapPath("~") + "xml.xml");
            }
            catch(Exception e)
            {
                if (e.Message.Contains("Root element is missing"))
                    return null;
                //throw new BLException("Ne postoje podaci za kategoriju", e);
            }
            return xmlDoc;
        }

        public DataTable GetEweCategories(int? parentCategoryID, int? categoryID)
        {
            DataTable categories = new DataTable();
            
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getEweCategories", objConn))
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

        /*public DataTable GetEweCategoriesFromWeb()
        {
            
        }*/

        public int SaveCategory(string category, string parentCategory)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveEweCategory", objConn))
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
                using (SqlCommand objComm = new SqlCommand("UPDATE eweCategory SET selected=@selected WHERE eweCategoryID=@eweCategoryID", objConn))
                {
                    objConn.Open();
                    for (int i = 0; i < categoryIDs.Length; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@selected", SqlDbType.Bit).Value = selected[i];
                        objComm.Parameters.Add("@eweCategoryID", SqlDbType.Int).Value = categoryIDs[i];
                        status = objComm.ExecuteNonQuery();
                    }
                }
            }
            return status;
        }

        public DataTable GetNewCategories()
        {
            DataTable categories = null;
            using(SqlConnection objConn=new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getNewEweCategories", objConn))
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

        public int GetEweCategoryForCategory(int categoryID)
        {
            int eweCategoryID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getEweCategoryForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID",SqlDbType.Int).Value=categoryID;
                    using(SqlDataReader reader=objComm.ExecuteReader())
                    {
                        while(reader.Read())
                            eweCategoryID=reader.GetInt32(0);
                    }
                }
            }
            return eweCategoryID;
        }

        public int SaveEweCategoryForCategory(int categoryID, int eweCategoryID, bool isCategory)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveEweCategoryForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@eweCategoryID", SqlDbType.Int).Value = eweCategoryID;
                    objComm.Parameters.Add("@isCategory", SqlDbType.Bit).Value = isCategory;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteEweCategories()
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteEweCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteCategoryEweCategory(int categoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteCategoryEweCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int SaveProducts(DataTable products, string category)
        {
            deleteEweProducts(category);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "dbo.eweProduct";
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

        private bool deleteEweProducts(string category)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("eweProducts_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@category", SqlDbType.NVarChar, 50).Value = category;
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
                using (SqlCommand objComm = new SqlCommand("eweProduct_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    for (int i = 0; i < subcategories.Length; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("category", SqlDbType.NVarChar, 50).Value = category;
                        objComm.Parameters.Add("subcategory", SqlDbType.NVarChar, 50).Value = subcategories[i];
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
                using (SqlCommand objComm = new SqlCommand("eweProduct_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("supplierCode", SqlDbType.NVarChar, 50).Value = supplierCode;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        product.Load(reader);
                }
            }
            return product;
        }
    }
}
