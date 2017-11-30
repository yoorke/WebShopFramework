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
                            retails.Add(new Retail(reader.GetInt32(0), new City(reader.GetInt32(1), reader.GetString(2), reader.GetString(3)), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetBoolean(9)));
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

        public Retail GetRetail(int retailID)
        {
            Retail retail = null;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("retail_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@retailID", SqlDbType.Int).Value = retailID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            retail = new Retail(reader.GetInt32(0), new City(reader.GetInt32(3), reader.GetString(4), reader.GetString(8)), reader.GetString(2), reader.GetString(5), reader.GetString(6), reader.GetString(9), reader.GetString(1), reader.GetBoolean(7));
                        }
                    }
                }
            }
            return retail;
        }

        public int Insert(Retail retail)
        {
            int status = -1;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("retail_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = retail.Name;
                    objComm.Parameters.Add("@address", SqlDbType.NVarChar, 200).Value = retail.Address;
                    objComm.Parameters.Add("@cityID", SqlDbType.Int).Value = retail.City.CityID;
                    objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = retail.Phone;
                    objComm.Parameters.Add("@mobilePhone", SqlDbType.NVarChar, 50).Value = retail.MobilePhone;
                    objComm.Parameters.Add("@location", SqlDbType.NVarChar, 1000).Value = retail.Location;
                    objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = retail.IsActive;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int Update(Retail retail)
        {
            int status = -1;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("retail_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = retail.Name;
                    objComm.Parameters.Add("@address", SqlDbType.NVarChar, 200).Value = retail.Address;
                    objComm.Parameters.Add("@cityID", SqlDbType.Int).Value = retail.City.CityID;
                    objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = retail.Phone;
                    objComm.Parameters.Add("@mobilePhone", SqlDbType.NVarChar, 50).Value = retail.MobilePhone;
                    objComm.Parameters.Add("@location", SqlDbType.NVarChar, 1000).Value = retail.Location;
                    objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = retail.IsActive;
                    objComm.Parameters.Add("@retailID", SqlDbType.Int).Value = retail.RetailID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int Delete(int retailID)
        {
            int status = -1;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("retail_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@retailID", SqlDbType.Int).Value = retailID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
