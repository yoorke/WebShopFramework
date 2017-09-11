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
    public class RetailDL
    {
        public List<Retail> GetRetails(int cityID, string retailName)
        {
            List<Retail> retails = new List<Retail>();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("retail_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("cityID", SqlDbType.Int).Value = cityID;
                    objComm.Parameters.Add("retailName", SqlDbType.NVarChar, 100).Value = retailName;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                            retails.Add(new Retail(reader.GetInt32(0), new City(reader.GetInt32(1), reader.GetString(2), reader.GetString(3)), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8)));
                    }
                }
            }
            return retails;
        }

        public List<string> GetDistinct()
        {
            List<string> retails = new List<string>();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("retail_getDistinct", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            retails.Add(reader.GetString(0));
                    }
                }
            }
            return retails;
        }
    }
}
