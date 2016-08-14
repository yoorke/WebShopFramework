using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopUtilities;
using System.Data.SqlClient;
using eshopBE;
using System.Web.Configuration;

namespace eshopDL
{
    public class VatDL
    {
        public List<Vat> GetVats()
        {
            List<Vat> vats = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT vatID, vat FROM vat ORDER BY vat", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            vats = new List<Vat>();
                        while (reader.Read())
                            vats.Add(new Vat(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
            }
            return vats;
        }
    }
}
