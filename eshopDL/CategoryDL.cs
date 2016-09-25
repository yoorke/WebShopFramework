using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using eshopBE;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using eshopUtilities;

namespace eshopDL
{
    public class CategoryDL
    {
        public DataTable GetCategories(string sortBy="categoryID, parentCategoryID", bool showNotActive = true)
        {
            DataTable categoriesDT = new DataTable();
            categoriesDT.Columns.Add("categoryID", typeof(int));
            categoriesDT.Columns.Add("name", typeof(string));
            categoriesDT.Columns.Add("parentID", typeof(int));
            categoriesDT.Columns.Add("url", typeof(string));
            categoriesDT.Columns.Add("imageUrl", typeof(string));
            categoriesDT.Columns.Add("sortOrder", typeof(int));
            categoriesDT.Columns.Add("categoryBannerID", typeof(int));

            //loading flattened datatable category without nested categories
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlCommand objComm = new SqlCommand("SELECT categoryID, name, parentCategoryID, url, imageUrl, sortOrder, categoryBannerID FROM category"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;
                        if (!showNotActive)
                            objComm.CommandText += " WHERE active = 1";

                        objComm.CommandText += " ORDER BY " + sortBy;
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            DataRow newRow;

                            while (reader.Read())
                            {
                                newRow = categoriesDT.NewRow();
                                newRow[0] = reader.GetInt32(0);
                                newRow[1] = reader.GetString(1);
                                if (Convert.IsDBNull(reader[2]) == false)
                                    newRow[2] = reader.GetInt32(2);
                                else
                                    newRow[2] = 0;
                                newRow[3] = "/proizvodi/" + reader.GetString(3);
                                newRow[4] = reader.GetString(4) != string.Empty ? "/images/" + reader.GetString(4) : string.Empty;
                                newRow[5] = (Convert.IsDBNull(reader[5]) == false) ? reader.GetInt32(5) : 0;
                                newRow[6] = (!Convert.IsDBNull(reader[6])) ? reader.GetInt32(6) : -1;

                                categoriesDT.Rows.Add(newRow);
                            }
                        }
                    }
                    catch(SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading categories data table.", ex);
                    }
                }

            return categoriesDT;
        }

        public int SaveCategory(Category category)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            { 
                using (SqlCommand objComm = new SqlCommand("INSERT INTO category (name, parentCategoryID, url, imageUrl, sortOrder, pricePercent, webPricePercent, showOnFirstPage, numberOfProducts, firstPageSortOrder, firstPageOrderBy, description, active, sliderID, categoryBannerID, updateProductsFromExternalApplication, exportProducts) VALUES (@name, @parentCategoryID, @url, @imageUrl, @sortOrder, @pricePercent, @webPricePercent, @showOnFirstPage, @numberOfProducts, @firstPageSortOrder, @firstPageOrderBy, @description, @active, @sliderID, @categoryBannerID, @updateProductsFromExternalApplication, @exportProducts)"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;

                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = category.Name;
                        objComm.Parameters.Add("@parentCategoryID", SqlDbType.Int).Value = category.ParentCategoryID;
                        objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = category.Url;
                        objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = category.ImageUrl;
                        objComm.Parameters.Add("@sortOrder", SqlDbType.Int).Value = category.SortOrder;
                        objComm.Parameters.Add("@pricePercent", SqlDbType.Float).Value = category.PricePercent;
                        objComm.Parameters.Add("@webPricePercent", SqlDbType.Float).Value = category.WebPricePercent;
                        objComm.Parameters.Add("@showOnFirstPage", SqlDbType.Bit).Value = category.ShowOnFirstPage;
                        objComm.Parameters.Add("@numberOfProducts", SqlDbType.Int).Value = category.NumberOfProducts;
                        objComm.Parameters.Add("@firstPageSortOrder", SqlDbType.Int).Value = category.firstPageSortOrder;
                        objComm.Parameters.Add("@firstPageOrderBy", SqlDbType.NVarChar, 50).Value = category.firstPageOrderBy;
                        objComm.Parameters.Add("@description", SqlDbType.NVarChar, 2000).Value = category.Description != null ? category.Description : string.Empty;
                        objComm.Parameters.Add("@active", SqlDbType.Bit).Value = category.Active;
                        objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = category.Slider != null ? category.Slider.SliderID : -1;
                        objComm.Parameters.Add("categoryBannerID", SqlDbType.Int).Value = category.CategoryBannerID;
                        objComm.Parameters.Add("@updateProductsFromExternalApplication", SqlDbType.Bit).Value = category.UpdateProductsFromExternalApplication;
                        objComm.Parameters.Add("@exportProducts", SqlDbType.Bit).Value = category.ExportProducts;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving category", ex);
                    }
                }
            }
            return status;
        }

        public int UpdateCategory(Category category)
        {
            int status;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlCommand objComm = new SqlCommand("UPDATE category SET name=@name, parentCategoryID=@parentCategoryID, url=@url, imageUrl=@imageUrl, sortOrder=@sortOrder, pricePercent=@pricePercent, webPricePercent=@webPricePercent, showOnFirstPage=@showOnFirstPage, numberOfProducts=@numberOfProducts, firstPageSortOrder=@firstPageSortOrder, firstPageOrderBy=@firstPageOrderBy, description=@description, active = @active, sliderID = @sliderID, categoryBannerID = @categoryBannerID, updateProductsFromExternalApplication = @updateProductsFromExternalApplication, exportProducts = @exportProducts WHERE categoryID=@categoryID"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;

                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = category.Name;
                        objComm.Parameters.Add("@parentCategoryID", SqlDbType.Int).Value = category.ParentCategoryID;
                        objComm.Parameters.Add("@url", SqlDbType.NVarChar, 50).Value = category.Url;
                        objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = category.ImageUrl;
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = category.CategoryID;
                        objComm.Parameters.Add("@sortOrder", SqlDbType.Int).Value = category.SortOrder;
                        objComm.Parameters.Add("@pricePercent", SqlDbType.Float).Value = category.PricePercent;
                        objComm.Parameters.Add("@webPricePercent", SqlDbType.Float).Value = category.WebPricePercent;
                        objComm.Parameters.Add("@showOnFirstPage", SqlDbType.Bit).Value = category.ShowOnFirstPage;
                        objComm.Parameters.Add("@numberOfProducts", SqlDbType.Int).Value = category.NumberOfProducts;
                        objComm.Parameters.Add("@firstPageSortOrder", SqlDbType.Int).Value = category.firstPageSortOrder;
                        objComm.Parameters.Add("@firstPageOrderBy", SqlDbType.NVarChar, 50).Value = category.firstPageOrderBy;
                        objComm.Parameters.Add("@description", SqlDbType.NVarChar, 2000).Value = category.Description != null ? category.Description : string.Empty;
                        objComm.Parameters.Add("@active", SqlDbType.Bit).Value = category.Active;
                        objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = category.Slider.SliderID;
                        objComm.Parameters.Add("@categoryBannerID", SqlDbType.Int).Value = category.CategoryBannerID;
                        objComm.Parameters.Add("@updateProductsFromExternalApplication", SqlDbType.Bit).Value = category.UpdateProductsFromExternalApplication;
                        objComm.Parameters.Add("@exportProducts", SqlDbType.Bit).Value = category.ExportProducts;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while updating category", ex);
                    }
                }
            return status;
        }

        private Category GetCategory(int categoryID, string name)
        {
            Category category = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlCommand objComm = new SqlCommand("SELECT categoryID, name, parentCategoryID, url, imageUrl, sortOrder, pricePercent, webPricePercent, showOnFirstPage, numberOfProducts, firstPageSortOrder, firstPageOrderBy, description, active, sliderID, categoryBannerID, updateProductsFromExternalApplication, exportProducts FROM category"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;

                        if (categoryID > 0)
                        {
                            objComm.CommandText += " WHERE categoryID=@categoryID";
                            objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        }
                        else if (name != string.Empty)
                        {
                            objComm.CommandText += " WHERE url=@name";
                            objComm.Parameters.Add("@name", SqlDbType.NVarChar,50).Value = name;
                        }

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                category = new Category();

                            while (reader.Read())
                            {
                                category.CategoryID = reader.GetInt32(0);
                                category.Name = reader.GetString(1);
                                category.ParentCategoryID = !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : 0;
                                category.Url = reader.GetString(3);
                                category.ImageUrl = reader.GetString(4);
                                category.SortOrder = reader.GetInt32(5);
                                category.PricePercent = reader.GetDouble(6);
                                category.WebPricePercent = reader.GetDouble(7);
                                category.ShowOnFirstPage = reader.GetBoolean(8);
                                if (Convert.IsDBNull(reader[9]) == false)
                                    category.NumberOfProducts = reader.GetInt32(9);
                                if (Convert.IsDBNull(reader[10]) == false)
                                    category.firstPageSortOrder = reader.GetInt32(10);
                                if (Convert.IsDBNull(reader[11]) == false)
                                    category.firstPageOrderBy = reader.GetString(11);
                                if (Convert.IsDBNull(reader[12]) == false)
                                    category.Description = reader.GetString(12);
                                else
                                    category.Description = string.Empty;
                                if (Convert.IsDBNull(reader[13]) == false)
                                    category.Active = reader.GetBoolean(13);
                                if (Convert.IsDBNull(reader[14]) == false)
                                    category.Slider = new Slider(reader.GetInt32(14), string.Empty, DateTime.Now, DateTime.Now, true);
                                if(Convert.IsDBNull(reader[15]) == false)
                                    category.CategoryBannerID = reader.GetInt32(15);
                                category.UpdateProductsFromExternalApplication = reader.GetBoolean(16);
                                category.ExportProducts = reader.GetBoolean(17);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading category", ex);
                    }
                }
            return category;
        }

        public Category GetCategory(int categoryID)
        {
            return GetCategory(categoryID, string.Empty);
        }

        public Category GetCategory(string name)
        {
            return GetCategory(-1, name);
        }

        public Category GetCategoryByUrl(string categoryUrl)
        {
            Category category = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT categoryID, name, parentCategoryID, url, imageUrl, sortOrder, pricePercent, webPricePercent, description, active, sliderID, categoryBannerID, updateProductsFromExternalApplication, exportProducts FROM category WHERE url=@categoryUrl", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryUrl", SqlDbType.NVarChar, 50).Value = categoryUrl;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            category = new Category(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5), reader.GetDouble(6), reader.GetDouble(7), Convert.IsDBNull(reader[8]) == false ? reader.GetString(8) : string.Empty, Convert.IsDBNull(reader[9]) ? false : reader.GetBoolean(9), !Convert.IsDBNull(reader[11]) ? reader.GetInt32(11) : -1, reader.GetBoolean(12), reader.GetBoolean(13), new Slider(Convert.IsDBNull(reader[10]) == false ? reader.GetInt32(10) : 0, string.Empty, DateTime.Now, DateTime.Now, true));
                        }
                    }
                }
            }
            return category;
        }

        public DataTable GetCategoriesForFirstPage()
        {
            DataTable categories = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT categoryID, name, url, numberOfProducts, firstPageOrderBy FROM category WHERE showOnFirstPage=1 ORDER BY firstPageSortOrder", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            categories.Load(reader);
                    }
                }
            }
            return categories;
        }

        public int DeleteCategory(int categoryID)
        {
            int status = 0;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlCommand objComm = new SqlCommand("DELETE FROM category WHERE categoryID=@categoryID"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if(ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            throw new BLException("Nije moguće obrisati kategoriju. Postoje povezani proizvodi.", ex);
                        }
                        else
                        {
                            ErrorLog.LogError(ex);
                            throw new DLException("Error while deleting category", ex);
                        }
                    }
                }
            return status;
        }

        public int AddBrandToCategoryExtraMenu(int categoryExtraMenuID, int brandID, int categoryID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryExtraMenuCategoryBrand_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryExtraMenuID", SqlDbType.Int).Value = categoryExtraMenuID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteCategoryExtraMenuCategory(int categoryExtraMenuID, int categoryID, int brandID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryExtraMenuCategoryBrand_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryExtraMenuID", SqlDbType.Int).Value = categoryExtraMenuID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<Brand> GetBrandsForCategoryExtraMenu(int categoryExtraMenuID, int categoryID)
        {
            List<Brand> brands = new List<Brand>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("categoryExtraMenuCategoryBrand_getBrandsByCategoryExtraMenuCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryExtraMenuID", SqlDbType.Int).Value = categoryExtraMenuID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            //brands.Load(reader);
                            while (reader.Read())
                                brands.Add(new Brand(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return brands;
        }

        public List<Category> GetAllSubCategories(int categoryID)
        {
            List<Category> categories = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("category_getSubcategories", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            categories = new List<Category>();
                        while (reader.Read())
                            categories.Add(new Category(reader.GetInt32(0), reader.GetString(1), !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : -1, reader.GetString(4), string.Empty, 0, 0, 0, string.Empty, true, -1, false, false, null));
                    }
                }
            }
            return categories;
        }

        public List<int> GetChildrenCategories(int categoryID)
        {
            List<int> categories = new List<int>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("category_getChildren", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            categories.Add(reader.GetInt32(0));
                    }
                }
            }
            return categories;
        }

        public List<Category> GetCategoriesForProductUpdate()
        {
            List<Category> categories = new List<Category>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("category_getForProductUpdate", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            categories.Add(new Category(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3), !Convert.IsDBNull(reader[4]) ? reader.GetString(4) : string.Empty, reader.GetInt32(5), reader.GetDouble(6), reader.GetDouble(7), !Convert.IsDBNull(reader[8]) ? reader.GetString(8) : string.Empty, reader.GetBoolean(9), !Convert.IsDBNull(reader[10]) ? reader.GetInt32(10) : -1, reader.GetBoolean(11), reader.GetBoolean(12), null));
                    }
                }
            }
            return categories;
        }
    }
}
