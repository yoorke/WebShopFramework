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
    }
}
