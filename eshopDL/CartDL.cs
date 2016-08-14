using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace eshopDL
{
    public class CartDL
    {
        public int AddProductToCart(int productID, string cartID, double quantity, double productPrice, double userPrice)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("cartAddItem", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@quantity", SqlDbType.Float).Value = quantity;
                    objComm.Parameters.Add("@productPrice", SqlDbType.Float).Value = productPrice;
                    objComm.Parameters.Add("@userPrice", SqlDbType.Float).Value = userPrice;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = 1;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteProductFromCart(int productID, string cartID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("cartDeleteItem", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetProducts(string cartID)
        {
            DataTable products = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("cartGetItems", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            products.Load(reader);
                    }
                }
            }
            return products;
        }

        public int GetProductsCount(string cartID)
        {
            int count = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getItemsCount", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    count = int.Parse(objComm.ExecuteScalar().ToString());
                }
            }
            return count;
        }

        public int UpdateCartProduct(string cartID, int productID, double quantity, double productPrice, double userPrice, int couponID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("updateCartItem", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    objComm.Parameters.Add("@quantity", SqlDbType.Float).Value = quantity;
                    objComm.Parameters.Add("@productPrice", SqlDbType.Float).Value = productPrice;
                    objComm.Parameters.Add("@userPrice", SqlDbType.Float).Value = userPrice;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public double GetCartDiscount(string cartID)
        {
            double discount=0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCartDiscount", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            discount = reader.GetDouble(0);
                    }
                }
            }
            return discount;
        }

        public int SaveCartCoupon(string cartID, int couponID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("addCartCoupon", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                    
                }
            }
            return status;
        }

        public int GetCartCoupon(string cartID)
        {
            int couponID = 1;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCartCoupon", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            if (Convert.IsDBNull(reader[0]) == false)
                                couponID = reader.GetInt32(0);
                    }
                }
            }
            return couponID;
        }

        public void RemoveCoupon(string cartID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("cart_removeCoupon", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@cartID", SqlDbType.NVarChar, 50).Value = cartID;

                    objComm.ExecuteReader();
                }
            }
        }
    }
}
