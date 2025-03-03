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
    public class ProductVariantDL
    {
        public void Save(ProductVariant productVariant)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productVariant_insert", objConn))
                {
                    objConn.Open();
                    using (SqlTransaction objTransaction = objConn.BeginTransaction())
                    { 
                        objComm.Transaction = objTransaction;
                        objComm.CommandType = CommandType.StoredProcedure;
                        objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = productVariant.Code;
                        objComm.Parameters.Add("@price", SqlDbType.Float).Value = productVariant.Price;
                        objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = productVariant.IsInStock;
                        objComm.Parameters.Add("@quantity", SqlDbType.Float).Value = productVariant.Quantity;
                        objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productVariant.ProductID;
                        SqlParameter productVariantID = new SqlParameter("@id", SqlDbType.Int);
                        productVariantID.Direction = ParameterDirection.Output;
                        objComm.Parameters.Add(productVariantID);

                        objComm.ExecuteNonQuery();
                        productVariant.ID = int.Parse(objComm.Parameters["@id"].Value.ToString());

                        saveAttributeValues(productVariant.ID, productVariant.Attributes, objConn, objTransaction);

                        objTransaction.Commit();
                    }
                }
            }
        }

        private void saveAttributeValues(int productVariantID, List<AttributeValue> attributeValues, SqlConnection objConn, SqlTransaction objTransaction)
        {
            if(attributeValues != null)
            {
                foreach(AttributeValue attributeValue in attributeValues)
                {
                    saveProductVariantAttributeValue(productVariantID, attributeValue, objConn, objTransaction);
                }
            }
        }

        private void saveProductVariantAttributeValue(int productVariantID, AttributeValue attributeValue,  SqlConnection objConn, SqlTransaction objTransaction)
        {
            using(SqlCommand objComm = new SqlCommand("productVariantAttributeValue_insert", objConn))
            {
                objComm.Transaction = objTransaction;
                objComm.CommandType = CommandType.StoredProcedure;
                objComm.Parameters.Add("@productVariantID", SqlDbType.Int).Value = productVariantID;
                objComm.Parameters.Add("@attributeValueID", SqlDbType.Int).Value = attributeValue.AttributeValueID;

                objComm.ExecuteNonQuery();
            }
        }

        public DataTable GetProductVariants(int productID)
        {
            DataTable productVariants = new DataTable();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productVariant_getListByProductID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        productVariants.Load(reader);
                    }
                }
            }

            return productVariants;
        }

        public void DeleteProductVariant(int id)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productVariant_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void SetIsInStock(int id, bool isInStock)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productVariant_setIsInStock", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    objComm.Parameters.Add("@isInStock", SqlDbType.Bit).Value = isInStock;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public List<ProductVariantAttributeValue> GetAttributeValues(int productID)
        {
            List<ProductVariantAttributeValue> attributeValues = new List<ProductVariantAttributeValue>();

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("productVariantAttributeValue_getByProductID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            attributeValues.Add(new ProductVariantAttributeValue()
                            {
                                AttributeID = reader.GetInt32(0),
                                AttributeValueID = reader.GetInt32(1),
                                Code = reader.GetString(2),
                                Name = reader.GetString(3),
                                Value = reader.GetString(4),
                                ValueCode = !Convert.IsDBNull(reader[5]) ? reader.GetString(5) : string.Empty,
                                ProductVariantID = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }

            return attributeValues;
        }
    }
}
