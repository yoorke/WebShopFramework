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
    public class UnitOfMeasureDL
    {
        public List<UnitOfMeasure> GetUnitsOfMeasure()
        {
            List<UnitOfMeasure> unitsOfMeasure = new List<UnitOfMeasure>();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("unitOfMeasure_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            unitsOfMeasure.Add(new UnitOfMeasure(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }
            }
            return unitsOfMeasure;
        }

        public UnitOfMeasure GetUnitOfMeasure(int unitOfMeasureID)
        {
            UnitOfMeasure unitOfMeasure = null;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("unitOfMeasure_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@unitOfMeasureID", SqlDbType.Int).Value = unitOfMeasureID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            unitOfMeasure = new UnitOfMeasure(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    }
                }
            }
            return unitOfMeasure;
        }
    }
}
