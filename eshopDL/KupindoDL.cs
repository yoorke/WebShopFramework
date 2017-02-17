using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace eshopDL
{
    public class KupindoDL
    {
        public DataTable GetCategories()
        {
            DataTable categories = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("kupindo_getCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            categories.Load(reader);
                    }
                }
            }
            return categories;
        }

        public DataTable GetMappedCategories()
        {
            DataTable categories = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_getMappedCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            categories.Load(reader);
                    }
                }
            }
            return categories;
        }

        public int SaveMapping(int categoryID, int kupindoCategoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_insertMappedCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@kupindoCategoryID", SqlDbType.Int).Value = kupindoCategoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetKupindoCategoryForCategory(int categoryID)
        {
            DataTable category = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_getKupindoCategoryForCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            category.Load(reader);
                    }
                }
            }
            return category;
        }

        public DataTable GetKupindoAttributes(int kupindoCategoryID)
        {
            DataTable attributes = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_getAttributes", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@kupindoCategoryID", SqlDbType.Int).Value = kupindoCategoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            attributes.Load(reader);
                    }
                }
            }
            return attributes;
        }

        public int SaveKupindoAttributeForAttribute(int attributeID, int kupindoAttributeID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_saveKupindoAttributeForAttribute", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                    objComm.Parameters.Add("@kupindoAttributeID", SqlDbType.Int).Value = kupindoAttributeID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable LoadSettings()
        {
            DataTable settings = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_loadSettings", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            settings.Load(reader);
                    }
                }
            }
            return settings;
        }

        public void SaveSettings(DataTable settings)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("kupindo_saveSettings", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    foreach(DataRow row in settings.Rows)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@kupindoSettingsID", SqlDbType.Int).Value = row["kupindoSettingsID"].ToString();
                        objComm.Parameters.Add("@value", SqlDbType.NVarChar).Value = row["value"].ToString();

                        objComm.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
