using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using eshopUtilities;
using eshopBE;
using System.Web.Configuration;

namespace eshopDL
{
    public class WishListDL
    {
        public int SaveProdutToWishList(int userID, int productID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("wishList_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<Product> GetWishListProducts(int userID)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("wishList_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.ProductID = reader.GetInt32(0);
                            product.Code = reader.GetString(1);
                            product.Name = reader.GetString(4);
                            product.Description = reader.GetString(5);
                            product.Price = reader.GetDouble(6);
                            product.WebPrice = reader.GetDouble(7);
                            product.Brand = new Brand(reader.GetInt32(3), reader.GetString(8));
                            product.Categories = new List<Category>();
                            product.Categories.Add(new Category(reader.GetInt32(19), reader.GetString(20), null, reader.GetString(21), string.Empty, 0, 0, 0, string.Empty, true, 0));
                            product.Images = new ProductDL().GetProductImages(product.ProductID);

                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public int DeleteFromWishList(int userID, int productID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("wishList_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
