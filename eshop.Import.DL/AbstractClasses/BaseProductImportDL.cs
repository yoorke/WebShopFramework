using eshop.Import.BE;
using eshop.Import.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;
using eshopUtilities.Extensions;
using System.Configuration;
using System.Net;
using System.IO;

namespace eshop.Import.DL.AbstractClasses
{
    public abstract class BaseProductImportDL : IProductImportDL
    {
        private string _supplierCode;

        public BaseProductImportDL(string supplierCode)
        {
            _supplierCode = supplierCode;
        }

        protected abstract XmlDocument getXml(string category, string subcategory, GetParameter getParameter);

        protected XmlDocument getDataFromUrl(string url)
        {
            int timeoutValue = 180000;
            XmlDocument xmlDoc = new XmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            int.TryParse(ConfigurationManager.AppSettings["httpRequestTimeoutValue"], out timeoutValue);
            request.Timeout = timeoutValue;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(responseStream);
                    xmlDoc.Load(reader);
                }
            }

            return xmlDoc;
        }

        public XmlDocument DownloadProducts(string category, string subCategory, GetParameter getParameter)
        {
            return this.getXml(category, subCategory, getParameter);
        }

        public List<ProductImport> GetProducts(string _parentCategory, List<string> categories, string manufacturer)
        {
            List<ProductImport> products = new List<ProductImport>();
            string parentCategoryName = string.Empty;
            string categoryName = string.Empty;

            Console.WriteLine("BaseProductImprotDL GetProducts");

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.productImport_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["commandTimeout"]);
                    objComm.CommandType = CommandType.StoredProcedure;

                    foreach(string category in categories)
                    {
                        parentCategoryName = category.Split('-')[0];
                        categoryName = category.Split('-')[1];

                        //if(parentCategoryName.Equals(categoryName))
                        //{
                            //parentCategoryName = string.Empty;
                        //}

                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@category", SqlDbType.NVarChar, 100).Value = categoryName;
                        objComm.Parameters.Add("@parentCategory", SqlDbType.NVarChar, 100).Value = parentCategoryName;
                        objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;
                        objComm.Parameters.Add("@manufacturer", SqlDbType.NVarChar, 100).Value = manufacturer;

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                products.Add(new ProductImport()
                                {
                                    ID = reader.GetInt32(0),
                                    SupplierCode = reader.GetString(1),
                                    Code = reader.GetString(2),
                                    Ean = reader.GetString(3),
                                    Name = reader.GetString(4),
                                    Pdv = reader.GetDouble(5),
                                    Category = reader.GetString(6),
                                    ParentCategory = reader.GetString(7),
                                    Manufacturer = reader.GetString(8),
                                    UnitOfMeasure = reader.GetString(9),
                                    Model = reader.GetString(10),
                                    Quantity = reader.GetString(11),
                                    Currency = reader.GetString(12),
                                    B2BPrice = reader.GetDouble(13),
                                    WebPrice = reader.GetDouble(14),
                                    Price = reader.GetDouble(15),
                                    EnergyClass = reader.GetString(16),
                                    Declaration = reader.GetString(17),
                                    Description = reader.GetString(18),
                                    ImageUrls = reader.GetString(19),
                                    Attributes = reader.GetString(20),
                                    Imported = reader.GetBoolean(21),
                                    Stock = reader.GetDouble(22),
                                    StockReservation = reader.GetDouble(23),
                                    Weight = !Convert.IsDBNull(reader[24]) ? reader.GetString(24) : "0"
                                });
                            }
                        }
                    }
                }
            }

            return products;
        }

        protected SupplierSettings getSupplierSettings(string supplierCode)
        {
            SupplierSettings supplierSettings = new SupplierSettings();

            if(supplierCode.Equals("uspon"))
            {
                supplierSettings.Url = "https://www.uspon.rs/api/v1/partner-export-xml";
                supplierSettings.Username = "pinservice";
                supplierSettings.Password = "pinservice2018";
            }
            else if(supplierCode.Equals("dsc"))
            {
                supplierSettings.Url = "https://www.dsc.rs/api/v1/partner-export-xml";
                supplierSettings.Username = "pinservis";
                supplierSettings.Password = "pinzr";
            }
            //else if(supplierCode.Equals("ewe"))
            //{
            //    supplierSettings.Url = "http://api.ewe.rs/share/backend_231/";
            //    supplierSettings.Username = "pinservis";
            //    supplierSettings.Password = "754fc";
            //}
            else if(supplierCode.Equals("ewe"))
            {
                supplierSettings.Url = "http://apicatalog.ewe.rs:5001/api/";
                supplierSettings.Username = "pinservis";
                supplierSettings.Password = "754fc";
            }

            return supplierSettings;
        }

        public void SaveProducts(List<ProductImport> products, string category)
        {
            deleteProducts(category, products[0].Timestamp);

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(objConn))
                {
                    objConn.Open();
                    sqlBulkCopy.BatchSize = 1000;
                    sqlBulkCopy.BulkCopyTimeout = 3600;
                    sqlBulkCopy.DestinationTableName = "import.productImport";
                    sqlBulkCopy.ColumnMappings.Add("SupplierCode", "supplierCode");
                    sqlBulkCopy.ColumnMappings.Add("Code", "code");
                    sqlBulkCopy.ColumnMappings.Add("Ean", "ean");
                    sqlBulkCopy.ColumnMappings.Add("Name", "name");
                    sqlBulkCopy.ColumnMappings.Add("Pdv", "pdv");
                    sqlBulkCopy.ColumnMappings.Add("Category", "category");
                    sqlBulkCopy.ColumnMappings.Add("ParentCategory", "parentCategory");
                    sqlBulkCopy.ColumnMappings.Add("Manufacturer", "manufacturer");
                    sqlBulkCopy.ColumnMappings.Add("UnitOfMeasure", "unitOfMeasure");
                    sqlBulkCopy.ColumnMappings.Add("Model", "model");
                    sqlBulkCopy.ColumnMappings.Add("Quantity", "quantity");
                    sqlBulkCopy.ColumnMappings.Add("Currency", "currency");
                    sqlBulkCopy.ColumnMappings.Add("B2BPrice", "b2bPrice");
                    sqlBulkCopy.ColumnMappings.Add("WebPrice", "webPrice");
                    sqlBulkCopy.ColumnMappings.Add("Price", "price");
                    sqlBulkCopy.ColumnMappings.Add("EnergyClass", "energyClass");
                    sqlBulkCopy.ColumnMappings.Add("Declaration", "declaration");
                    sqlBulkCopy.ColumnMappings.Add("Description", "description");
                    sqlBulkCopy.ColumnMappings.Add("ImageUrls", "imageUrls");
                    sqlBulkCopy.ColumnMappings.Add("Attributes", "attributes");
                    sqlBulkCopy.ColumnMappings.Add("Timestamp", "timestamp");
                    sqlBulkCopy.ColumnMappings.Add("InsertDate", "insertDate");
                    sqlBulkCopy.ColumnMappings.Add("Stock", "stock");
                    sqlBulkCopy.ColumnMappings.Add("StockReservation", "stockReservation");
                    sqlBulkCopy.ColumnMappings.Add("Weight", "weight");

                    sqlBulkCopy.WriteToServer(products.ToDataTable());
                }
            }
        }

        private void deleteProducts(string category, string timestamp)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.productImport_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = _supplierCode;
                    objComm.Parameters.Add("@timestamp", SqlDbType.VarChar, 50).Value = timestamp;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        public ProductImport GetProductBySupplierCode(string supplierCode, string supplierProductCode)
        {
            ProductImport product = new ProductImport();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.productImport_getBySupplierCode", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 50).Value = supplierCode;
                    objComm.Parameters.Add("@supplierProductCode", SqlDbType.VarChar, 50).Value = supplierProductCode;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                product = new ProductImport()
                                {
                                    Code = reader.GetString(0),
                                    Ean = reader.GetString(1),
                                    Name = reader.GetString(2),
                                    Pdv = reader.GetDouble(3),
                                    Category = reader.GetString(4),
                                    ParentCategory = reader.GetString(5),
                                    Manufacturer = reader.GetString(6),
                                    UnitOfMeasure = reader.GetString(7),
                                    Model = reader.GetString(8),
                                    Quantity = reader.GetString(9),
                                    Currency = reader.GetString(10),
                                    B2BPrice = reader.GetDouble(11),
                                    WebPrice = reader.GetDouble(12),
                                    Price = reader.GetDouble(13),
                                    EnergyClass = reader.GetString(14),
                                    Declaration = reader.GetString(15),
                                    Description = reader.GetString(16),
                                    ImageUrls = reader.GetString(17),
                                    Attributes = reader.GetString(18)
                                };
                            }
                        }
                    }
                }

                return product;
            }
        }

        public List<ManufacturerImport> GetManufacturers(List<string> subCategories)
        {
            List<ManufacturerImport> manufacturers = new List<ManufacturerImport>();

            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("import.manufacturer_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    foreach(string subCategory in subCategories)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@subCategory", SqlDbType.NVarChar, 100).Value = subCategory;

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                if(manufacturers.Find(item => item.Name == reader.GetString(0)) == null)
                                {
                                    manufacturers.Add(new ManufacturerImport()
                                    {
                                        Name = reader.GetString(0)
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return manufacturers;
        }

        public void SetStock(string supplierCode, bool showIfNotInStock)
        {
            using(SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm = new SqlCommand("import.product_setStock", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@supplierCode", SqlDbType.VarChar, 10).Value = supplierCode;
                    objComm.Parameters.Add("@showIfNotInStock", SqlDbType.Bit).Value = showIfNotInStock;

                    objComm.ExecuteNonQuery();
                }
            }
        }
    }
}
