using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using eshopBE;
using eshopUtilities;
using System.Web.Configuration;

namespace eshopDL
{
    public class BrandDL
    {
        public List<Brand> GetBrands()
        {
            List<Brand> brands = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT brandID, name FROM brand ORDER BY name", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            brands = new List<Brand>();
                        while (reader.Read())
                            brands.Add(new Brand(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return brands;
        }

        public List<Brand> GetBrands(int categoryID, bool includeChildrenCategories = false)
        {
            List<Brand> brands = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT brand.brandID, brand.name, COUNT(*) FROM brand INNER JOIN product ON brand.brandID=product.brandID INNER JOIN productCategory ON product.productID=productCategory.productID WHERE ", objConn))
                {
                    objConn.Open();
                    List<int> categories = new CategoryDL().GetChildrenCategories(categoryID);

                    objComm.CommandText += " (categoryID=@categoryID";

                    if (includeChildrenCategories)
                        for (int i = 0; i < categories.Count; i++)
                            objComm.CommandText += " OR categoryID = @categoryID" + (i + 1);

                    objComm.CommandText += ")";

                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;

                    if(includeChildrenCategories)
                        for (int i = 0; i < categories.Count; i++)
                            objComm.Parameters.Add("@categoryID" + (i + 1), SqlDbType.Int).Value = categories[i];

                    objComm.CommandText += " AND product.isActive=1 AND product.isApproved=1 GROUP BY brand.brandID, brand.name ORDER BY brand.name";
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            brands = new List<Brand>();
                        while (reader.Read())
                            brands.Add(new Brand(reader.GetInt32(0), reader.GetString(1) + " (" + reader.GetInt32(2).ToString() + ")"));
                    }
                }
            }
            return brands;
        }

        public Brand GetBrandByName(string name)
        {
            Brand brand = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT brandID, name FROM brand WHERE name=@name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            brand = new Brand();
                            brand.BrandID = reader.GetInt32(0);
                            brand.Name = reader.GetString(1);
                        }
                    }
                }
            }
            return brand;
        }

        public int SaveBrand(Brand brand)
        {
            int brandID;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO brand (name) VALUES (@name);SELECT SCOPE_IDENTITY()", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = brand.Name;
                    brandID = int.Parse(objComm.ExecuteScalar().ToString());
                }
            }
            return brandID;
        }

        public int UpdateBrand(Brand brand)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("updateBrand", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = brand.Name;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brand.BrandID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteBrand(int brandID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteBrand", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@brandID", SqlDbType.Int).Value = brandID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
