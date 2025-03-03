using eshopBE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopDL
{
    public class CategoryViewDL
    {
        public DataTable GetCategories(bool includeNotActive = false, string sortBy = "category.categoryID, category.parentCategoryID")
        {
            DataTable categories = null;

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand(@"
                                                            SELECT category.categoryID, category.name, category.imageUrl, category.parentCategoryID, category.url, 
                                                            --CASE WHEN ppc.url <> 'proizvodi' AND pc.url <> 'proizvodi' THEN 'proizvodi/' ELSE '' END + 
                                                            CASE WHEN category.url <> 'proizvodi' THEN '/proizvodi/' ELSE '' END + 
                                                            ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + 
                                                            (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                            ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + 
                                                            (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                            category.url as fullUrl 
                                                            FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID 
                                                            LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID", objConn))
                {
                    if (!includeNotActive)
                        objComm.CommandText += " WHERE category.active = 1";

                    objComm.CommandText += " ORDER BY " + sortBy;

                    objConn.Open();

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows) {
                            categories = new DataTable();
                            categories.Load(reader);
                        }
                    }
                }
            }

            return categories;
        }

        public List<CategoryView> GetCategoriesForFirstPage()
        {
            List<CategoryView> categories = new List<CategoryView>();

            using(SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand(@"
                                                            SELECT category.categoryID, category.name, category.imageUrl, category.parentCategoryID, category.url, 
                                                            --CASE WHEN ppc.url <> 'proizvodi' AND pc.url <> 'proizvodi' THEN 'proizvodi/' ELSE '' END + 
                                                            CASE WHEN category.url <> 'proizvodi' THEN '/proizvodi/' ELSE '' END + 
                                                            ISNULL(CASE WHEN ppc.url <> 'proizvodi' THEN ppc.url ELSE '' END, '') + 
                                                            (CASE WHEN ppc.url IS NOT NULL AND ppc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                            ISNULL(CASE WHEN pc.url <> 'proizvodi' THEN pc.url ELSE '' END, '') + 
                                                            (CASE WHEN pc.url IS NOT NULL AND pc.url <> 'proizvodi' THEN '/' ELSE '' END) + 
                                                            category.url as fullUrl 
                                                            FROM category LEFT JOIN category pc ON category.parentCategoryID = pc.categoryID 
                                                            LEFT JOIN category ppc ON pc.parentCategoryID = ppc.categoryID
                                                            WHERE category.showOnFirstPage = 1
                                                            AND category.active = 1
                                                            ORDER BY category.sortOrder", objConn))
                {
                    objConn.Open();

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            categories.Add(new CategoryView()
                            {
                                CategoryID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ImageUrl = reader.GetString(2),
                                Url = reader.GetString(4),
                                FullUrl = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return categories;
        }
    }
}
