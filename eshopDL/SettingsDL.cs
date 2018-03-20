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
                            settings.CompanyName = reader.GetString(1);
                            settings.Phone = reader.GetString(2);
                            settings.WorkingHours = reader.GetString(3);
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

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
