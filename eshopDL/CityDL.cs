using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using eshopBE;
using System.Configuration;

namespace eshopDL
{
    public class CityDL
    {
        public List<City> GetCities()
        {
            List<City> cities = new List<City>();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("city_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            cities.Add(new City(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }
            }
            return cities;
        }

        public City GetCity(int id)
        {
            City city = null;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("city_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cityID", SqlDbType.Int).Value = id;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            city = new City(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    }
                }
            }
            return city;
        }
    }
}
