using eshopBE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace eshopDL
{
    public class DeliveryServiceDL
    {
        public List<DeliveryService> GetAll()
        {
            List<DeliveryService> deliveryServices = new List<DeliveryService>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            deliveryServices.Add(new DeliveryService(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return deliveryServices;
        }

        public DeliveryService GetByID(int id)
        {
            DeliveryService deliveryService = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            deliveryService = new DeliveryService(reader.GetInt32(0), reader.GetString(1));
                    }
                }
            }
            return deliveryService;
        }

        public double GetDeliveryPriceByWeight(int deliveryServiceID, double weight)
        {
            double price = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_getPriceByWeight", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@deliveryServiceID", SqlDbType.Int).Value = deliveryServiceID;
                    objComm.Parameters.Add("@weight", SqlDbType.Float).Value = weight;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            price = reader.GetDouble(0);
                    }
                }
            }

            return price;
        }

        public DataTable GetDeliveryPrices(int deliveryServiceID)
        {
            DataTable prices = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_getPrices", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@deliveryServiceID", SqlDbType.Int).Value = deliveryServiceID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        prices.Load(reader);
                    }
                }
            }

            return prices;
        }

        public int GetDeliveryServiceIDByZip(string zip)
        {
            int deliveryServiceID = -1;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_getByZip", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@zip", SqlDbType.Char, 5).Value = zip;

                    deliveryServiceID = int.Parse(objComm.ExecuteScalar().ToString());
                }
            }

            return deliveryServiceID;   
        }

        public int GetActiveDeliveryServiceID()
        {
            int deliveryServiceID = 0;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deliveryService_getActive", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            deliveryServiceID = reader.GetInt32(0);
                        }
                    }
                }
            }

            return deliveryServiceID;
        }
    }
}
