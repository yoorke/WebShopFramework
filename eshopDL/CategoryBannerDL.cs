using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace eshopDL
{
    public class CategoryBannerDL
    {
        public int SaveCategoryBanner(CategoryBanner categoryBanner)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBanner_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = categoryBanner.Name;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = categoryBanner.ImageUrl;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = categoryBanner.Url;

                    status = int.Parse(objComm.ExecuteScalar().ToString());
                }
            }
            return status;
        }

        public int UpdateCategoryBanner(CategoryBanner categoryBanner)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBanner_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = categoryBanner.Name;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = categoryBanner.ImageUrl;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = categoryBanner.Url;
                    objComm.Parameters.Add("@categoryBannerID", SqlDbType.Int).Value = categoryBanner.CategoryBannerID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public CategoryBanner GetCategoryBanner(int categoryBannerID)
        {
            CategoryBanner categoryBanner = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBanner_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryBannerID", SqlDbType.Int).Value = categoryBannerID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categoryBanner = new CategoryBanner();
                            categoryBanner.CategoryBannerID = categoryBannerID;
                            categoryBanner.Name = reader.GetString(0);
                            categoryBanner.ImageUrl = reader.GetString(1);
                            categoryBanner.Url = reader.GetString(2);
                        }
                    }
                }
            }
            return categoryBanner;
        }

        public List<CategoryBanner> GetCategoryBanners()
        {
            List<CategoryBanner> categoryBanners = new List<CategoryBanner>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryBanner_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categoryBanners.Add(new CategoryBanner(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                        }
                    }
                }
            }
            return categoryBanners;
        }
    }
}
