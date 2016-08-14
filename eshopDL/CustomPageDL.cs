using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using eshopUtilities;

namespace eshopDL
{
    public class CustomPageDL
    {
        public int Save(CustomPage customPage)
        {
            int status = 0;
            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    using (SqlCommand objComm = new SqlCommand("customPageSave", objConn))
                    {
                        objConn.Open();
                        objComm.CommandType = CommandType.StoredProcedure;
                        objComm.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = customPage.Title;
                        objComm.Parameters.Add("@description", SqlDbType.NVarChar, 500).Value = customPage.Description;
                        objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = customPage.Url;
                        objComm.Parameters.Add("@content", SqlDbType.NVarChar).Value = customPage.Content;
                        objComm.Parameters.Add("@heading", SqlDbType.NVarChar, 50).Value = customPage.Heading;
                        objComm.Parameters.Add("@insertDate", SqlDbType.DateTime).Value = customPage.InsertDate;
                        //objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = customPage.UpdateDate;
                        objComm.Parameters.Add("@head", SqlDbType.NVarChar).Value = customPage.Heading;
                        objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = customPage.SortIndex;
                        objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = customPage.ImageUrl;
                        

                        SqlParameter customPageID = new SqlParameter("@customPageID", SqlDbType.Int);
                        customPageID.Direction = ParameterDirection.Output;
                        objComm.Parameters.Add(customPageID);

                        objComm.Parameters.Add("@customPageCategoryID", SqlDbType.Int).Value = customPage.CustomPageCategoryID;
                        objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = customPage.IsActive;

                        status = objComm.ExecuteNonQuery();

                        customPage.CustomPageID = int.Parse(objComm.Parameters["@customPageID"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex);
                throw new DLException("Error while saving custom page", ex);
            }
            return customPage.CustomPageID;
        }

        public int Update(CustomPage customPage)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("customPageUpdate", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@customPageID",SqlDbType.Int).Value=customPage.CustomPageID;
                    objComm.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = customPage.Title;
                    objComm.Parameters.Add("@description", SqlDbType.NVarChar, 500).Value = customPage.Description;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = customPage.Url;
                    objComm.Parameters.Add("@content", SqlDbType.NVarChar).Value = customPage.Content;
                    objComm.Parameters.Add("@heading", SqlDbType.NVarChar, 50).Value = customPage.Heading;
                    objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = customPage.UpdateDate;
                    objComm.Parameters.Add("@head", SqlDbType.NVarChar).Value = customPage.Head;
                    objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = customPage.SortIndex;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = customPage.ImageUrl;
                    objComm.Parameters.Add("@customPageCategoryID", SqlDbType.Int).Value = customPage.CustomPageCategoryID;
                    objComm.Parameters.Add("@isActive", SqlDbType.Bit).Value = customPage.IsActive;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return customPage.CustomPageID; ;
        }

        public CustomPage GetCustomPage(int customPageID)
        {
            CustomPage customPage = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCustomPageByID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@customPageID", SqlDbType.Int).Value = customPageID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            customPage = new CustomPage();
                        while (reader.Read())
                        {
                            customPage.CustomPageID = customPageID;
                            customPage.Title = reader.GetString(0);
                            customPage.Description = reader.GetString(1);
                            customPage.Url = reader.GetString(2);
                            customPage.Content = reader.GetString(3);
                            customPage.Heading = reader.GetString(4);
                            customPage.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(5));
                            customPage.UpdateDate = Common.ConvertToLocalTime(reader.GetDateTime(6));
                            customPage.Head = reader.GetString(7);
                            customPage.SortIndex = reader.GetInt32(8);
                            customPage.ImageUrl = reader.GetString(9);
                            customPage.CustomPageCategoryID = reader.GetInt32(11);
                            customPage.IsActive = reader.GetBoolean(12);
                        }
                    }
                }
            }
            return customPage;
        }

        public CustomPage GetCustomPage(string url)
        {
            CustomPage customPage = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCustomPageByUrl", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = url;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            customPage = new CustomPage();
                        while (reader.Read())
                        {
                            customPage.CustomPageID = reader.GetInt32(10);
                            customPage.Title = reader.GetString(0);
                            customPage.Description = reader.GetString(1);
                            customPage.Url = reader.GetString(2);
                            customPage.Content = reader.GetString(3);
                            customPage.Heading = reader.GetString(4);
                            customPage.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(5));
                            customPage.UpdateDate = Common.ConvertToLocalTime(reader.GetDateTime(6));
                            customPage.Head = reader.GetString(7);
                            customPage.SortIndex = reader.GetInt32(8);
                            customPage.ImageUrl = reader.GetString(9);
                            customPage.CustomPageCategoryID = reader.GetInt32(11);
                            customPage.IsActive = reader.GetBoolean(12);
                        }
                    }
                }
            }
            return customPage;
        }

        public List<CustomPage> GetCustomPagesForCustomPageCategory(int customPageCategoryID)
        {
            List<CustomPage> customPages = null;
            CustomPage customPage = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("customPageGetForCustomPageCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@customPageCategoryID", SqlDbType.NVarChar, 50).Value = customPageCategoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            customPages = new List<CustomPage>();
                        while (reader.Read())
                        {
                            customPage = new CustomPage();
                            customPage.CustomPageID = reader.GetInt32(0);
                            customPage.Title = reader.GetString(1);
                            customPage.Description = reader.GetString(2);
                            customPage.Url = reader.GetString(3);
                            customPage.Content = reader.GetString(4);
                            customPage.Heading = reader.GetString(5);
                            customPage.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(7));
                            customPage.UpdateDate = Common.ConvertToLocalTime(reader.GetDateTime(8));
                            customPage.Head = reader.GetString(6);
                            customPage.SortIndex = reader.GetInt32(11);
                            customPage.ImageUrl = reader.GetString(9);
                            customPage.CustomPageCategoryID = reader.GetInt32(10);
                            customPage.IsActive = reader.GetBoolean(12);

                            customPages.Add(customPage);
                        }
                    }
                }
            }
            return customPages;
        }

        public List<CustomPageCategory> GetCustomPagesCategories()
        {
            List<CustomPageCategory> categories = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCustomPageCategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            categories = new List<CustomPageCategory>();
                        while (reader.Read())
                            categories.Add(new CustomPageCategory(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return categories;
        }

        public int Delete(int customPageID)
        {
            int status = 0;
            
            using(SqlConnection objConn=new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm=new SqlCommand("customPageDelete",objConn))
                {
                    objConn.Open();
                    objComm.CommandType=CommandType.StoredProcedure;
                    objComm.Parameters.Add("@customPageID",SqlDbType.Int).Value=customPageID;

                    status=objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<CustomPage> GetCustomPages()
        {
            List<CustomPage> customPages = null;
            CustomPage customPage=null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCustomPages", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            customPages = new List<CustomPage>();
                        while (reader.Read())
                        {
                            customPage = new CustomPage();
                            customPage.CustomPageID = reader.GetInt32(0);
                            customPage.Title = reader.GetString(1);
                            customPage.Description = reader.GetString(2);
                            customPage.Url = reader.GetString(3);
                            customPage.Content = reader.GetString(4);
                            customPage.Head = reader.GetString(5);
                            customPage.Heading = reader.GetString(6);
                            customPage.InsertDate = Common.ConvertToLocalTime(reader.GetDateTime(7));
                            customPage.UpdateDate = Common.ConvertToLocalTime(reader.GetDateTime(8));
                            customPage.SortIndex = reader.GetInt32(9);
                            customPage.CustomPageCategoryID = reader.GetInt32(10);
                            customPage.ImageUrl = reader.GetString(11);
                            customPage.IsActive = reader.GetBoolean(12);

                            customPages.Add(customPage);
                        }
                    }
                }
            }
            return customPages;
        }
    }
}
