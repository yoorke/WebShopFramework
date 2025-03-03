using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data.SqlClient;
using System.Web.Configuration;
using eshopUtilities;
using System.Data;

namespace eshopDL
{
    public class SupplierDL
    {
        public List<Supplier> GetSuppliers()
        {
            List<Supplier> suppliers = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT supplierID, name, currencyCode FROM supplier ORDER BY name", objConn))
                {
                    try
                    {
                        objConn.Open();
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                suppliers = new List<Supplier>();
                            while (reader.Read())
                                suppliers.Add(new Supplier(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading suppliers list", ex);
                    }
                }
            }
            return suppliers;
        }

        public int SaveSupplier(Supplier supplier)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveSupplier", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = supplier.Name;

                    SqlParameter supplierID = new SqlParameter("@supplierID", SqlDbType.Int);
                    supplierID.Direction = ParameterDirection.Output;
                    objComm.Parameters.Add(supplierID);

                    status = objComm.ExecuteNonQuery();

                    supplier.SupplierID = int.Parse(objComm.Parameters["@supplierID"].Value.ToString());
                }
            }
            return supplier.SupplierID;
        }

        public int UpdateSupplier(Supplier supplier)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("updateSupplier", objConn))
                {
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = supplier.Name;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteSupplier(int supplierID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteSupplier", objConn))
                {
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierID", SqlDbType.Int).Value = supplierID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public Supplier GetSupplier(string code)
        {
            Supplier supplier = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("supplier_getByCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@code", SqlDbType.VarChar, 10).Value = code;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            supplier = new Supplier()
                            {
                                SupplierID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Code = reader.GetString(2),
                                CurrencyCode = reader.GetString(3)
                            };
                        }
                    }
                }
            }

            return supplier;
        }
    }
}
