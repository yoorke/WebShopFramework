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
          
    }
}
