using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace eshopDL
{
    public class ThreegDL
    {
        public DataTable GetCategories(int? parentCategoryID)
        {
            DataTable categories = new DataTable();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("threegCategory_getByParentID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("parentID", SqlDbType.Int).Value = parentCategoryID;
                    objComm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                        categories.Load(reader);
                }
            }

            return categories;
        }

        public DataTable GetProducts(int category1, int category2, int[] subCategories)
        {
            DataTable products = new DataTable();
            DataTable subCategoryProducts = new DataTable();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("threegProduct_getByThreegCategoryID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    for(int i = 0; i < subCategories.Length; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("threegCategoryID1", SqlDbType.Int).Value = category1;
                        objComm.Parameters.Add("threegCategoryID2", SqlDbType.Int).Value = category2;
                        objComm.Parameters.Add("threegCategoryID3", SqlDbType.Int).Value = subCategories[i];

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            subCategoryProducts.Rows.Clear();
                            subCategoryProducts.Load(reader);
                            products.Merge(subCategoryProducts);
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
                using (SqlCommand objComm = new SqlCommand("threegProduct_getBySupplierCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("supplierCode", SqlDbType.VarChar, 50).Value = supplierCode;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        product.Load(reader);
                }
            }

            return product;
        }

        public int SaveProducts(DataTable products)
        {
            DeleteAllProducts(products.Rows[0]["timestamp"].ToString());
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "dbo.threegProduct";
                    sqlBulkCopy.ColumnMappings.Add(0, "id");
                    sqlBulkCopy.ColumnMappings.Add(1, "sifra");
                    sqlBulkCopy.ColumnMappings.Add(2, "naziv");
                    sqlBulkCopy.ColumnMappings.Add(3, "kategorija1");
                    sqlBulkCopy.ColumnMappings.Add(4, "kategorija2");
                    sqlBulkCopy.ColumnMappings.Add(5, "kategorija3");
                    sqlBulkCopy.ColumnMappings.Add(6, "vpCena");
                    sqlBulkCopy.ColumnMappings.Add(7, "mpCena");
                    sqlBulkCopy.ColumnMappings.Add(8, "rabat");
                    sqlBulkCopy.ColumnMappings.Add(9, "dostupan");
                    sqlBulkCopy.ColumnMappings.Add(10, "naAkciji");
                    sqlBulkCopy.ColumnMappings.Add(11, "opis");
                    sqlBulkCopy.ColumnMappings.Add(12, "barkod");
                    sqlBulkCopy.ColumnMappings.Add(13, "slike");
                    sqlBulkCopy.ColumnMappings.Add(14, "timestamp");

                    sqlBulkCopy.WriteToServer(products);
                    status = 1;

                    SaveBrands();
                }
            }
            return status;
        }

        public void DeleteAllProducts(string timestamp)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("threegProduct_deleteAll", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@timestamp", SqlDbType.VarChar, 50).Value = timestamp;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void SaveBrands()
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("threegProduct_saveBrand", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void SaveThreegCategoryForCategory(int categoryID, int threegCategoryID, bool isCategory1, bool isCategory2)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryThreegCategory_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@threegCategoryID", SqlDbType.Int).Value = threegCategoryID;
                    objComm.Parameters.Add("@isCategory1", SqlDbType.Bit).Value = isCategory1;
                    objComm.Parameters.Add("@isCategory2", SqlDbType.Bit).Value = isCategory2;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteThreegCategoryForCategory(int categoryID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopCOnnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryThreegCategory_deleteForCategoryID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public int GetCategory1ForCategory(int categoryID)
        {
            int category1ID = -1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryThreegCategory_getCategory1", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            category1ID = reader.GetInt32(0);
                    }
                }
            }
            return category1ID;
        }

        public int GetCategory2ForCategory(int categoryID)
        {
            int category2ID = -1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryThreegCategory_getCategory2", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            category2ID = reader.GetInt32(0);
                    }
                }
            }
            return category2ID;
        }

        public DataTable GetThreegCategoriesForCategory(int categoryID)
        {
            DataTable threegCategories = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryThreegCategory_getThreegCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        threegCategories.Load(reader);
                }
            }
            return threegCategories;
        }

        public DataTable GetProductByCategory3ID(int category3ID)
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("threegProduct_getByCategory3", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@category3ID", SqlDbType.Int).Value = category3ID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                        products.Load(reader);
                }
            }

            return products;
        }
    }
}
