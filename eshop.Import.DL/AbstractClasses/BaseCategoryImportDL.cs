using eshop.Import.BE;
using eshop.Import.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.DL.AbstractClasses
{
    public class BaseCategoryImportDL : ICategoryImportDL
    {
        private string _supplierCode;

        public BaseCategoryImportDL(string supplierCode)
        {
            _supplierCode = supplierCode;
        }

        public List<CategoryImport> GetCategories(string supplierCode, string parentCategory)
        {
            List<CategoryImport> categories = new List<CategoryImport>();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.categoryImport_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@parentCategory", SqlDbType.NVarChar, 100).Value = parentCategory;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = supplierCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            categories.Add(new CategoryImport()
                            {
                                ID = reader.GetInt32(0),
                                SupplierID = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                ParentID = !Convert.IsDBNull(reader[3]) ? reader.GetInt32(3) : -1,
                                InsertDate = reader.GetDateTime(4),
                                IsSelected = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }

            return categories;
        }

        public CategoryImport GetSupplierCategoryForCategoryID(int categoryID)
        {
            CategoryImport categoryImport = null;

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.categoryCategoryImport_getByCategoryID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            categoryImport = new CategoryImport()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ParentID = !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : -1,
                                ParentName = string.Empty,
                                InsertDate = reader.GetDateTime(3),
                                SupplierID = reader.GetInt32(4),
                                IsSelected = reader.GetBoolean(5)
                            };
                        }
                    }
                }
            }

            return categoryImport;
        }

        public List<CategoryImport> GetSupplierCategoriesForCategoryID(int categoryID)
        {
            List<CategoryImport> categories = new List<CategoryImport>();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.categoryCategoryImport_getCategoriesByCategoryID", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            categories.Add(new CategoryImport()
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ParentID = !Convert.IsDBNull(reader[2]) ? reader.GetInt32(2) : -1,
                                ParentName = !Convert.IsDBNull(reader[6]) ? reader.GetString(6) : reader.GetString(1),
                                InsertDate = reader.GetDateTime(3),
                                SupplierID = reader.GetInt32(4),
                                IsSelected = false
                            });
                        }
                    }
                }
            }

            return categories;
        }

        public void SaveSupplierCategory(string category, string parentCategory)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.saveSupplierCategory", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@category", SqlDbType.NVarChar, 100).Value = category;
                    objComm.Parameters.Add("@parentCategory", SqlDbType.NVarChar, 100).Value = parentCategory;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void SaveSupplierCategoryForCategory(int categoryID, int supplierCategoryID, bool isCategory)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.categoryCategoryImport_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@supplierCategoryID", SqlDbType.Int).Value = supplierCategoryID;
                    objComm.Parameters.Add("@isCategory", SqlDbType.Bit).Value = isCategory;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCategorySupplierCategory(int categoryID)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.categoryCategoryImport_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    objComm.ExecuteNonQuery();
                }
            }
        }
    }
}
