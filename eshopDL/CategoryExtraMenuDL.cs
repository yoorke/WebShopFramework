using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using eshopBE;
using System.Web.Configuration;
using System.Data;

namespace eshopDL
{
    public class CategoryExtraMenuDL
    {
        public List<CategoryExtraMenu> GetCategoryExtraMenus()
        {
            List<CategoryExtraMenu> extraMenus = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryExtraMenu_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            extraMenus = new List<CategoryExtraMenu>();
                        while (reader.Read())
                        {
                            extraMenus.Add(new CategoryExtraMenu(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }
            return extraMenus;
        }

        public List<CategoryExtraMenuCategory> GetCategoryExtraMenusForCategory(int categoryID)
        {
            List<CategoryExtraMenuCategory> extraMenus = new List<CategoryExtraMenuCategory>();
            using(SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryExtraMenuCategory_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            extraMenus = new List<CategoryExtraMenuCategory>();
                        while(reader.Read())
                            extraMenus.Add(new CategoryExtraMenuCategory(reader.GetString(0), new CategoryDL().GetBrandsForCategoryExtraMenu(reader.GetInt32(1), categoryID)));
                    }
                }
            }
            return extraMenus;
        }
    }
}
