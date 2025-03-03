using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopUtilities;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace eshopDL
{
    public class OrderDL
    {
        public int SaveOrder(Order order)
        {
            int status = 0;
            SqlTransaction transaction = null;
            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = objConn.CreateCommand())
                    {
                        objComm.Connection = objConn;
                        //objComm.CommandText = "saveOrder";
                        objComm.CommandText = "order_insert";
                        objComm.CommandType = CommandType.StoredProcedure;
                        using (transaction = objConn.BeginTransaction())
                        {
                            objComm.Transaction = transaction;

                            objComm.Parameters.Add("@date", SqlDbType.DateTime).Value = order.Date;
                            objComm.Parameters.Add("@userID", SqlDbType.Int).Value = order.User.UserID;
                            objComm.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = order.Firstname;
                            objComm.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = order.Lastname;
                            objComm.Parameters.Add("@address", SqlDbType.NVarChar, 100).Value = order.Address;
                            objComm.Parameters.Add("@city", SqlDbType.NVarChar, 50).Value = order.City;
                            objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = order.Phone;
                            objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = order.Email;
                            objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = order.Coupon.CouponID;
                            objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = order.Name;
                            objComm.Parameters.Add("@pib", SqlDbType.NChar, 9).Value = order.Pib;
                            objComm.Parameters.Add("@paymentID", SqlDbType.Int).Value = order.Payment.PaymentID;
                            objComm.Parameters.Add("@deliveryID", SqlDbType.Int).Value = order.Delivery.DeliveryID;
                            objComm.Parameters.Add("@statusID", SqlDbType.Int).Value = order.OrderStatus.OrderStatusID;
                            objComm.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = order.Zip;
                            objComm.Parameters.Add("@comment", SqlDbType.NVarChar).Value = order.Comment;
                            objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = order.CartID;
                            objComm.Parameters.Add("@userDiscountValue", SqlDbType.Float).Value = order.UserDiscountValue;
                            

                            SqlParameter orderID = new SqlParameter("@orderID", SqlDbType.Int);
                            orderID.Direction = ParameterDirection.Output;
                            objComm.Parameters.Add(orderID);

                            SqlParameter code = new SqlParameter("@code", SqlDbType.Int);
                            code.Direction = ParameterDirection.Output;
                            objComm.Parameters.Add(code);

                            status = objComm.ExecuteNonQuery();
                            order.OrderID = int.Parse(objComm.Parameters["@orderID"].Value.ToString());
                            order.Code = objComm.Parameters["@code"].Value.ToString() + "/" + order.Date.Year.ToString();

                            if (status > 0)
                            {
                                objComm.CommandText = "saveOrderItem";
                                for (int i = 0; i < order.Items.Count; i++)
                                {
                                    objComm.Parameters.Clear();
                                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = order.OrderID;
                                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = order.Items[i].Product.ProductID;
                                    objComm.Parameters.Add("@productPrice", SqlDbType.Float).Value = order.Items[i].ProductPrice;
                                    objComm.Parameters.Add("@userPrice", SqlDbType.Float).Value = order.Items[i].UserPrice;
                                    objComm.Parameters.Add("@quantity", SqlDbType.Float).Value = order.Items[i].Quantity;

                                    status = objComm.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorLog.LogError(ex);
                transaction.Rollback();
                throw new DLException("Error while saving order.", ex);
            }
            return status;
        }

        public int Insert(Order order)
        {
            int status = 0;
            SqlTransaction transaction = null;
            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = objConn.CreateCommand())
                    {
                        objComm.Connection = objConn;
                        objComm.CommandText = "order_insert";
                        objComm.CommandType = CommandType.StoredProcedure;
                        using (transaction = objConn.BeginTransaction())
                        {
                            objComm.Transaction = transaction;

                            objComm.Parameters.Add("@date", SqlDbType.DateTime).Value = order.Date;
                            objComm.Parameters.Add("@userID", SqlDbType.Int).Value = order.User.UserID;
                            objComm.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = order.Firstname;
                            objComm.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = order.Lastname;
                            objComm.Parameters.Add("@address", SqlDbType.NVarChar, 100).Value = order.Address;
                            objComm.Parameters.Add("@city", SqlDbType.NVarChar, 50).Value = order.City;
                            objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = order.Phone;
                            objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = order.Email;
                            objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = order.Coupon.CouponID;
                            objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = order.Name;
                            objComm.Parameters.Add("@pib", SqlDbType.NChar, 9).Value = order.Pib;
                            objComm.Parameters.Add("@paymentID", SqlDbType.Int).Value = order.Payment.PaymentID;
                            objComm.Parameters.Add("@deliveryID", SqlDbType.Int).Value = order.Delivery.DeliveryID;
                            objComm.Parameters.Add("@statusID", SqlDbType.Int).Value = order.OrderStatus.OrderStatusID;
                            objComm.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = order.Zip;
                            objComm.Parameters.Add("@comment", SqlDbType.NVarChar).Value = order.Comment;
                            objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = order.CartID;
                            objComm.Parameters.Add("@userDiscountValue", SqlDbType.Float).Value = order.UserDiscountValue;
                            objComm.Parameters.Add("@confirmed", SqlDbType.Bit).Value = order.Confirmed;


                            SqlParameter orderID = new SqlParameter("@orderID", SqlDbType.Int);
                            orderID.Direction = ParameterDirection.Output;
                            objComm.Parameters.Add(orderID);

                            SqlParameter code = new SqlParameter("@code", SqlDbType.Int);
                            code.Direction = ParameterDirection.Output;
                            objComm.Parameters.Add(code);

                            status = objComm.ExecuteNonQuery();
                            order.OrderID = int.Parse(objComm.Parameters["@orderID"].Value.ToString());
                            order.Code = objComm.Parameters["@code"].Value.ToString() + "/" + order.Date.Year.ToString();

                            if (status > 0)
                            {
                                this.insertOrderItems(order.Items, order.OrderID, objComm);
                            }
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorLog.LogError(ex);
                transaction.Rollback();
                throw new DLException("Error while saving order.", ex);
            }

            return status;
        }

        public int Update(Order order)
        {
            int status = 0;
            SqlTransaction transaction = null;
            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = objConn.CreateCommand())
                    {
                        objComm.Connection = objConn;
                        objComm.CommandText = "order_update";
                        objComm.CommandType = CommandType.StoredProcedure;
                        using (transaction = objConn.BeginTransaction())
                        {
                            objComm.Transaction = transaction;

                            objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = order.OrderID;
                            objComm.Parameters.Add("@userID", SqlDbType.Int).Value = order.User.UserID;
                            objComm.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = order.Firstname;
                            objComm.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = order.Lastname;
                            objComm.Parameters.Add("@address", SqlDbType.NVarChar, 100).Value = order.Address;
                            objComm.Parameters.Add("@city", SqlDbType.NVarChar, 50).Value = order.City;
                            objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = order.Phone;
                            objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = order.Email;
                            objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = order.Coupon.CouponID;
                            objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = order.Name;
                            objComm.Parameters.Add("@pib", SqlDbType.NChar, 9).Value = order.Pib;
                            objComm.Parameters.Add("@paymentID", SqlDbType.Int).Value = order.Payment.PaymentID;
                            objComm.Parameters.Add("@deliveryID", SqlDbType.Int).Value = order.Delivery.DeliveryID;
                            objComm.Parameters.Add("@statusID", SqlDbType.Int).Value = order.OrderStatus.OrderStatusID;
                            objComm.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = order.Zip;
                            objComm.Parameters.Add("@comment", SqlDbType.NVarChar).Value = order.Comment;
                            objComm.Parameters.Add("@userDiscountValue", SqlDbType.Float).Value = order.UserDiscountValue;

                            status = objComm.ExecuteNonQuery();

                            //if (status > 0)
                            //{
                                this.deleteOrderItems(order.OrderID);
                                this.insertOrderItems(order.Items, order.OrderID, objComm);
                            //}

                            transaction.Commit();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorLog.LogError(ex);
                transaction.Rollback();
                throw new DLException("Error while saving order.", ex);
            }

            return status;
        }

        private void insertOrderItems(List<OrderItem> items, int orderID, SqlCommand objComm)
        {
            objComm.CommandText = "orderItem_insert";

            foreach(OrderItem item in items)
            {
                this.insertOrderItem(item, orderID, objComm);
            }
        }

        private void insertOrderItem(OrderItem item, int orderID, SqlCommand objComm)
        {
            objComm.Parameters.Clear();
            objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
            objComm.Parameters.Add("@productID", SqlDbType.Int).Value = item.Product.ProductID;
            objComm.Parameters.Add("@productPrice", SqlDbType.Float).Value = item.ProductPrice;
            objComm.Parameters.Add("@userPrice", SqlDbType.Float).Value = item.UserPrice;
            objComm.Parameters.Add("@quantity", SqlDbType.Float).Value = item.Quantity;

            objComm.ExecuteNonQuery();
        }

        private void deleteOrderItems(int orderID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("orderItem_deleteAll", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public Order GetOrder(int orderID, int code = -1)
        {
            Order order = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_select", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.Parameters.Add("@code", SqlDbType.Int).Value = code;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using(SqlDataReader reader=objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            order = new Order();
                            while (reader.Read())
                            {
                                order.OrderID = reader.GetInt32(0);
                                order.Date = Common.ConvertToLocalTime(reader.GetDateTime(1));
                                order.User = new User(reader.GetInt32(2), string.Empty, string.Empty, reader.GetString(3), string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty, DateTime.Now, string.Empty, 0, 1);
                                order.Firstname = reader.GetString(4);
                                order.Lastname = reader.GetString(5);
                                order.Address = reader.GetString(6);
                                order.City = reader.GetString(7);
                                order.Phone = reader.GetString(8);
                                order.Email = reader.GetString(9);
                                order.Name = reader.GetString(10);
                                order.Pib = reader.GetString(11);
                                order.Payment = new Payment(reader.GetInt32(12), reader.GetString(13));
                                order.Delivery = new Delivery(reader.GetInt32(14), reader.GetString(15));
                                if (Convert.IsDBNull(reader[16]) == false)
                                    order.Zip = reader.GetString(16);
                                order.Items = getOrderItems(order.OrderID);
                                order.Coupon = new Coupon(reader.GetInt32(17), reader.GetString(18), reader.GetDouble(19), string.Empty, reader.GetDateTime(24), reader.GetDateTime(25), new CouponType(reader.GetInt32(26), string.Empty), null);
                                order.OrderStatus = new OrderStatus(reader.GetInt32(20), reader.GetString(21), false);
                                order.Code = reader.GetString(23);
                                order.UserDiscountValue = reader.GetDouble(27);
                                order.DeliveryServiceID = !Convert.IsDBNull(reader[28]) ? reader.GetInt32(28) : -1;
                                order.TrackCode = !Convert.IsDBNull(reader[29]) ? reader.GetString(29) : string.Empty;
                                order.Confirmed = !Convert.IsDBNull(reader[30]) ? reader.GetBoolean(30) : false;

                                if (Convert.IsDBNull(reader[22]) == false)
                                    order.Comment = reader.GetString(22);
                            }
                        }
                    }
                }
            }
            return order;
        }

        private List<OrderItem> getOrderItems(int orderID)
        {
            List<OrderItem> items = null;
            //Order order = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getOrderItems", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            items = new List<OrderItem>();
                        while (reader.Read())
                        {
                            items.Add(new OrderItem(reader.GetInt32(0), reader.GetInt32(1), new ProductDL().GetProduct(reader.GetInt32(2), string.Empty, false, string.Empty), reader.GetDouble(3), reader.GetDouble(4), reader.GetDouble(5)));
                        }
                    }
                }
            }
            return items;
        }

        public DataTable GetOrderItemsFull(int orderID)
        {
            DataTable items = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getOrderItemsFull", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            items = new DataTable();
                            items.Load(reader);
                        }
                    }
                }
            }
            return items;
        }

        public DataTable GetOrders(int statusID, DateTime dateFrom, DateTime dateTo, int? UserID)
        {
            DataTable orders = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                //using (SqlCommand objComm = new SqlCommand("getOrders", objConn))
                using(SqlCommand objComm = new SqlCommand("order_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderStatusID", SqlDbType.Int).Value = statusID;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom.ToUniversalTime();
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo.ToUniversalTime();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = UserID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            orders = new DataTable();
                            orders.Load(reader);
                        }
                    }
                }
            }
            return Common.ConvertToLocalTime(ref orders);
        }

        public DataTable GetOrderStatuses()
        {
            DataTable statuses = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getOrderStatuses", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            statuses = new DataTable();
                            statuses.Load(reader);
                        }
                    }
                }
            }
            return statuses;
        }
         
        public int UpdateOrderStatus(int orderID, int orderStatusID, DeliveryService deliveryService, string trackCode)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("updateOrderStatus", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.Parameters.Add("@orderStatusID", SqlDbType.Int).Value = orderStatusID;
                    objComm.Parameters.Add("@deliveryServiceID", SqlDbType.Int).Value = deliveryService != null ? (int?)(deliveryService.ID) : null;
                    objComm.Parameters.Add("@trackCode", SqlDbType.VarChar).Value = trackCode;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public void UpdateOrderPaymentStatus(int orderID, string paymentStatusCode)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("orderPaymentStatus_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.Parameters.Add("@paymentStatusCode", SqlDbType.Char, 3).Value = paymentStatusCode;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void UpdateConfirmed(int orderID, bool confirmed)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_setConfirmed", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.Parameters.Add("@confirmed", SqlDbType.Bit).Value = confirmed;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public int DeleteOrder(int orderID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteOrder", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetLast10(int userID)
        {
            DataTable orders = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_get_last_10", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        orders.Load(reader);
                    }
                }
            }

            //for (int i = 0; i < orders.Rows.Count; i++)
                //for (int j = 0; j < orders.Columns.Count; j++)
                    //if (orders.Rows[i][j].GetType() == typeof(System.DateTime))
                        //orders.Rows[i][j] = Common.ConvertToLocalTime(DateTime.Parse(orders.Rows[i][j].ToString()));
 
            return Common.ConvertToLocalTime(ref orders);
        }

        public DataTable GetNumberOfOrdersByDay(DateTime dateFrom, DateTime dateTo)
        {
            DataTable orders = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_getNumberOfOrdersByDay", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom.Date;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo.Date;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        orders.Load(reader);
                    }
                }
            }
            return orders;
        }

        public DataTable GetNumberOfOrdersByMonth(DateTime dateFrom, DateTime dateTo)
        {
            DataTable orders = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_getNumberOfOrdersByMonth", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom.Date;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo.Date;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        orders.Load(reader);
                    }
                }
            }
            return orders;
        }

        public DataTable GetValuesByMonth(DateTime dateFrom, DateTime dateTo)
        {
            DataTable values = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_getValuesByMonth", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom.Date;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo.Date;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        values.Load(reader);
                    }
                }
            }
            return values;
        }

        public DataTable GetValuesByDay(DateTime dateFrom, DateTime dateTo)
        {
            DataTable values = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_getValuesByDay", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        values.Load(reader);
                    }
                }
            }
            return values;
        }

        public DataTable GetCategoryPercentage(DateTime dateFrom, DateTime dateTo)
        {
            DataTable orders = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("order_getCategoryPercentage", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo;

                    using(SqlDataReader reader = objComm.ExecuteReader())
                    {
                        orders.Load(reader);
                    }

                }
            }
            return orders;
        }

        public DataTable GetCategoryValue(DateTime dateFrom, DateTime dateTo)
        {
            DataTable orders = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_getCategoryValue", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = dateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = dateTo;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        orders.Load(reader);
                    }
                }
            }
            return orders;
        }

        public int SetDiscount(int orderID, double userDiscountValue)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("order_setDiscount", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@orderID", SqlDbType.Int).Value = orderID;
                    objComm.Parameters.Add("@userDiscountValue", SqlDbType.Float).Value = userDiscountValue;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
