using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace eshopDL
{
    public class AdminMenuDL
    {
        public List<AdminMenuItem> Get()
        {
            List<AdminMenuItem> adminMenuList = new List<AdminMenuItem>();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("adminMenu_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            adminMenuList.Add(new AdminMenuItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetBoolean(5), reader.GetInt32(6)));
                    }
                }
            }
            return adminMenuList;
        }

        public DataTable GetDataTable()
        {
            DataTable adminMenu = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("adminMenu_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        adminMenu.Load(reader);
                    }
                }
            }
            return adminMenu;
        }
    }
}
