using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eshopBE;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace eshopDL
{
    public class CategoryBrandDL
    {
        public void Save(CategoryBrand categoryBrand)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBrand_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryBrand.CategoryID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = categoryBrand.BrandID;
                    objComm.Parameters.Add("@pricePercent", SqlDbType.Float).Value = categoryBrand.PricePercent;
                    objComm.Parameters.Add("@webPricePercent", SqlDbType.Float).Value = categoryBrand.WebPricePercent;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int categoryID, int brandID)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBrand_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public List<CategoryBrand> Get(int categoryID)
        {
            List<CategoryBrand> categoryBrands = new List<CategoryBrand>();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBrand_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            categoryBrands.Add(new CategoryBrand(reader.GetInt32(0), reader.GetInt32(1), reader.GetDouble(2), reader.GetDouble(3)));
                    }
                }
            }
            return categoryBrands;
        }

        public DataTable GetDataTable(int categoryID)
        {
            DataTable categoryBrands = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBrand_getDataTable", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        categoryBrands.Load(reader);
                    }
                }
            }
            return categoryBrands;
        }

        public List<double> GetPricePercent(int categoryID, int brandID)
        {
            List<double> pricePercent = new List<double>();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBrand_getPricePercent", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read()) { 
                            pricePercent.Add(reader.GetDouble(0));
                            pricePercent.Add(reader.GetDouble(1));
                        }
                    }
                }
            }
            return pricePercent;
        }
    }
}
