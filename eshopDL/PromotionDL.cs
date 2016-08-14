using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using eshopBE;
using System.Web.Configuration;
using System.Data;
using eshopUtilities;

namespace eshopDL
{
    public class PromotionDL
    {
        public int SavePromotion(Promotion promotion)
        {
            int promotionID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO promotion (name, value, imageUrl, showOnFirstPage, dateFrom, dateTo, url, showOnMenu) VALUES (@name, @value, @imageUrl, @showOnFirstPage, @dateFrom, @dateTo, @url, @showOnMenu); SELECT SCOPE_IDENTITY()", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = promotion.Name;
                    objComm.Parameters.Add("@value", SqlDbType.Float).Value = promotion.Value;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = promotion.ImageUrl;
                    objComm.Parameters.Add("@showOnFirstPage", SqlDbType.Bit).Value = promotion.ShowOnFirstPage;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = promotion.DateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = promotion.DateTo;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = promotion.Url;
                    objComm.Parameters.Add("@showOnMenu", SqlDbType.Bit).Value = promotion.ShowOnMenu;

                    promotionID = int.Parse(objComm.ExecuteScalar().ToString());
                }
            }
            return promotionID;
        }

        public int UpdatePromotion(Promotion promotion)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE promotion SET name=@name, value=@value, imageUrl=@imageUrl, showOnFirstPage=@showOnFirstPage, dateFrom=@dateFrom, dateTo=@dateTo, url = @url, showOnMenu = @showOnMenu WHERE promotionID=@promotionID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = promotion.Name;
                    objComm.Parameters.Add("@value", SqlDbType.Float).Value = promotion.Value;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = promotion.ImageUrl;
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotion.PromotionID;
                    objComm.Parameters.Add("@showOnFirstPage", SqlDbType.Bit).Value = promotion.ShowOnFirstPage;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.Date).Value = promotion.DateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.Date).Value = promotion.DateTo;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = promotion.Url;
                    objComm.Parameters.Add("@showOnMenu", SqlDbType.Bit).Value = promotion.ShowOnMenu;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<Promotion> GetPromotions(bool? showOnFirstPage, bool? showOnMenu)
        {
            List<Promotion> promotions = new List<Promotion>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT promotionID, name, value, url FROM promotion", objConn))
                {
                    objConn.Open();
                    bool exists = false;
                    if (showOnFirstPage != null)
                    {
                        objComm.CommandText += " WHERE showOnFirstPage=@showOnFirstPage";
                        objComm.Parameters.Add("@showOnFirstPage", SqlDbType.Bit).Value = showOnFirstPage;
                        
                        exists = true;
                    }
                    if(showOnMenu != null)
                    {
                        objComm.CommandText += exists ? " AND showOnMenu = @showOnMenu" : " WHERE showOnMenu = @showOnMenu";
                        objComm.Parameters.Add("@showOnMenu", SqlDbType.Bit).Value = showOnMenu;
                    }
                    if(showOnFirstPage != null || showOnMenu != null)
                    {
                        objComm.CommandText += " AND dateFrom<=GETUTCDATE() AND dateTo>=GETUTCDATE()";
                    }
                    objComm.CommandText += " ORDER BY name";
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            promotions = new List<Promotion>();
                        while (reader.Read())
                            promotions.Add(new Promotion(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), string.Empty, 0, false, DateTime.MinValue, DateTime.Now.AddDays(1), !Convert.IsDBNull(reader[3]) ? reader.GetString(3) : string.Empty, false));
                    }
                }
            }
            return promotions;
        }

        public Promotion GetPromotion(int promotionID)
        {
            Promotion promotion = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT promotionID, name, value, imageUrl, showOnFirstPage, dateFrom, dateTo, url, showOnMenu FROM promotion WHERE promotionID=@promotionID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            promotion = new Promotion(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetString(3), 0, reader.GetBoolean(4), Common.ConvertToLocalTime(reader.GetDateTime(5)), Common.ConvertToLocalTime(reader.GetDateTime(6)), !Convert.IsDBNull(reader[7]) ? reader.GetString(7) : string.Empty, reader.GetBoolean(8));
                    }
                }
            }
            return promotion;
        }

        public int DeletePromotion(int promotionID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM promotion WHERE promotionID=@promotionID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@promotionID", SqlDbType.Int).Value = promotionID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetActive()
        {
            DataTable promotions = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("promotion_get_active", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        promotions.Load(reader);
                    }
                }
            }
            return Common.ConvertToLocalTime(ref promotions);
        }

        public Promotion GetPromotion(string url)
        {
            Promotion promotion = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT promotionID, name, value, imageUrl, showOnFirstPage, dateFrom, dateTo, url, showOnMenu FROM promotion WHERE url = @url", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = url;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            promotion = new Promotion(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetString(3), 0, reader.GetBoolean(4), Common.ConvertToLocalTime(reader.GetDateTime(5)), Common.ConvertToLocalTime(reader.GetDateTime(6)), reader.GetString(7), reader.GetBoolean(8));
                    }
                }
            }
            return promotion;
        }

        
    }
}
