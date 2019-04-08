using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace eshopDL
{
    public class SettingsDL
    {
        public Settings GetSettings()
        {
            Settings settings = new Settings();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("settings_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            settings.CompanyName = !Convert.IsDBNull(reader[1]) ? reader.GetString(1) : string.Empty;
                            settings.Phone = !Convert.IsDBNull(reader[2]) ? reader.GetString(2) : string.Empty;
                            settings.WorkingHours = !Convert.IsDBNull(reader[3]) ? reader.GetString(3) : string.Empty;
                            settings.DeliveryCost = !Convert.IsDBNull(reader[4]) ? reader.GetDouble(4) : 0;
                            settings.FreeDeliveryTotalValue = !Convert.IsDBNull(reader[5]) ? reader.GetDouble(5) : 0;
                        }
                    }
                }
            }

            return settings;
        }

        public int SaveSettings(Settings settings)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("settings_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@companyName", SqlDbType.NVarChar, 100).Value = settings.CompanyName;
                    objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = settings.Phone;
                    objComm.Parameters.Add("@workingHours", SqlDbType.NVarChar, 200).Value = settings.WorkingHours;
                    objComm.Parameters.Add("@deliveryCost", SqlDbType.Float).Value = settings.DeliveryCost;
                    objComm.Parameters.Add("@freeDeliveryTotalValue", SqlDbType.Float).Value = settings.FreeDeliveryTotalValue;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
