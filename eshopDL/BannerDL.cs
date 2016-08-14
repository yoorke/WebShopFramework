using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace eshopDL
{
    public class BannerDL
    {
        public int SaveBanner(Banner banner)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveBanner", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    for (int i = 0; i < banner.Banners.Count; i++)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("bannerPositionID", SqlDbType.Int).Value = banner.BannerPositionID;
                        objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = banner.Banners[i].ImageUrl;
                        objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = banner.Banners[i].Url;

                        status = objComm.ExecuteNonQuery();
                    }
                }

            }
            return status;
        }

        public List<BannerItem> GetBannersForPosition(int bannerPositionID)
        {
            List<BannerItem> banners=null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getBannersForPosition", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@bannerPositionID", SqlDbType.Int).Value = bannerPositionID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            banners = new List<BannerItem>();

                            while (reader.Read())
                                banners.Add(new BannerItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                        }
                    }
                }
            }
            return banners;
        }

        public BannerItem GetBanner(string bannerPosition)
        {
            BannerItem banner = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getBanner", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@bannerPosition", SqlDbType.NVarChar, 50).Value = bannerPosition;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            banner = new BannerItem();
                            while (reader.Read())
                            {
                                banner.BannerID = reader.GetInt32(0);
                                banner.ImageUrl = reader.GetString(1);
                                banner.Url = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            return banner;
        }

        public int DeleteBanner(int bannerID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteBanner", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@bannerID", SqlDbType.Int).Value = bannerID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<BannerPosition> GetBannerPositions()
        {
            List<BannerPosition> bannerPositions = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getBannerPositions", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            bannerPositions = new List<BannerPosition>();
                            while (reader.Read())
                            {
                                bannerPositions.Add(new BannerPosition(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            return bannerPositions;
        }

        public int SaveBannerItem(BannerItem item, int bannerPositionID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("bannerItem_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@bannerPositionID", SqlDbType.Int).Value = bannerPositionID;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = item.ImageUrl;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = item.Url;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            status = reader.GetInt32(0);
                    }
                        
                }
            }
            return status;
        }

        public int UpdateBannerItem(BannerItem item, int bannerPositionID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("bannerItem_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@bannerPositionID", SqlDbType.Int).Value = bannerPositionID;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = item.ImageUrl;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = item.Url;
                    objComm.Parameters.Add("@bannerID", SqlDbType.Int).Value = item.BannerID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
