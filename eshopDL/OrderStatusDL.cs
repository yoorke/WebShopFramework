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
    public class OrderStatusDL
    {
        public List<OrderStatus> GetAll()
        {
            List<OrderStatus> orderStatuses = new List<OrderStatus>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("orderStatus_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            orderStatuses.Add(new OrderStatus(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3)));
                    }
                }
            }
            return orderStatuses;
        }

        public OrderStatus GetByID(int ID)
        {
            OrderStatus orderStatus = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("orderStatus_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@id", SqlDbType.Int).Value = ID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            orderStatus = new OrderStatus(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), reader.GetBoolean(3));
                    }
                }
            }
            return orderStatus;
        }
    }
}
