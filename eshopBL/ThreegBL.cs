﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using eshopDL;
using eshopBE;
using System.Xml;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web;

namespace eshopBL
{
    public class ThreegBL
    {
        public DataTable GetCategories(int? parentCategoryID, bool addSelectAll)
        {
            DataTable categories = new ThreegDL().GetCategories(parentCategoryID);
            if(addSelectAll)
            {
                DataRow newRow = categories.NewRow();
                newRow["name"] = "Odaberi";
                newRow["id"] = "-1";
                categories.Rows.InsertAt(newRow, 0);
            }
            return categories;
        }

        public DataTable GetProducts(int category1, int category2, string[] subCategories)
        {
            int[] subCategoriesIDs = new int[subCategories.Length];
            for (int i = 0; i < subCategories.Length; i++)
                subCategoriesIDs[i] = int.Parse(subCategories[i]);
            return new ThreegDL().GetProducts(category1, category2, subCategoriesIDs);
        }

        public bool SaveProduct(string supplierCode, bool isApproved, bool isActive, int categoryID)
        {
            DataTable threegProduct = new ThreegDL().GetProductBySupplierCode(supplierCode);

            Category category = new CategoryBL().GetCategory(categoryID);

            Product product = new Product();
            product.IsApproved = isApproved;
            product.IsActive = isActive;
            product.SupplierID = 1014;
            product.SupplierCode = supplierCode;
            product.VatID = 4;
            product.Categories = new List<Category>();
            product.Categories.Add(category);
            product.Specification = string.Empty;
            product.IsInStock = true;
            bool isNew = false;
            bool isLocked = false;
            product.Code = product.SupplierCode;
            product.Description = string.Empty;

            product.ProductID = new ProductBL().GetProductIDBySupplierCode(product.SupplierCode);
            if(product.ProductID <= 0)
                isNew = true;
            isLocked = new ProductBL().IsLocked(product.ProductID);

            Brand brand = new BrandBL().GetBrandByName(threegProduct.Rows[0]["brand"].ToString());
            if(brand == null)
            {
                brand = new Brand();
                brand.Name = threegProduct.Rows[0]["brand"].ToString();
                brand.LogoUrl = string.Empty;
                brand.BrandID = new BrandBL().SaveBrand(brand);
            }
            if (product.Brand == null)
                product.Brand = new Brand();
            product.Brand = brand;

            product.Name = threegProduct.Rows[0]["naziv"].ToString();
            product.Price = calculatePrice(double.Parse(threegProduct.Rows[0]["vpCena"].ToString()), category.PricePercent);
            product.WebPrice = calculatePrice(double.Parse(threegProduct.Rows[0]["vpCena"].ToString()), category.WebPricePercent);
            product.Ean = threegProduct.Rows[0]["barkod"].ToString();
            product.SupplierPrice = double.Parse(threegProduct.Rows[0]["vpCena"].ToString());
            product.UnitOfMeasure = new UnitOfMeasure(2, "Komad", "kom");

            bool hasImages = false;

            if(threegProduct.Rows[0]["slike"] != null && threegProduct.Rows[0]["slike"].ToString() != string.Empty)
            {
                hasImages = true;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(threegProduct.Rows[0]["slike"].ToString());
                foreach(XmlNode image in xmlDoc.SelectSingleNode("slike").ChildNodes)
                {
                    if (product.Images == null)
                        product.Images = new List<ProductImage>();
                    if(image.Name == "slika")
                    {
                        string imageUrl = saveImageFromUrl(image.InnerText.Trim(), product.Images != null ? product.Images.Count : 0);
                        if (imageUrl != string.Empty)
                            product.Images.Add(new ProductImage(imageUrl, product.Images.Count + 1));
                    }
                }
            }
            else
            {
                if (!bool.Parse(ConfigurationManager.AppSettings["saveProductWithoutImage"]))
                    throw new Exception("No image");
            }

            if (!isLocked && (bool.Parse(ConfigurationManager.AppSettings["saveProductWithoutImage"]) || hasImages))
                if (new ProductBL().SaveProduct(product) > 0)
                    return true;
            return false;

        }

        private double calculatePrice(double supplierPrice, double percent)
        {
            Settings settings = new SettingsBL().GetSettings();
            supplierPrice = supplierPrice * settings.ExchangeRate;
            return double.Parse(((int)(supplierPrice * (percent / 100 + 1) * 1.2) / 10 * 10 - 10).ToString());
        }

        private string saveImageFromUrl(string url, int count)
        {
            try
            {
                bool exists = true;
                WebClient webClient = new WebClient();
                string filename = Path.GetFileName(url);
                string extension = filename.Substring(filename.LastIndexOf('.'));
                string fullPath = HttpContext.Current.Server.MapPath("~") + new ProductBL().CreateNewImageName(count) + extension;
                string path = fullPath.Substring(0, fullPath.LastIndexOf('/'));

                webClient.DownloadFile(url, fullPath);
                exists = new ProductBL().CreateProductImages(fullPath);

                return exists ? fullPath.Substring(fullPath.LastIndexOf('/') + 1) : string.Empty;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public int SaveProducts(DataTable products)
        {
            return new ThreegDL().SaveProducts(products);
        }

        public void DeleteAllProducts(string timestamp)
        {
            new ThreegDL().DeleteAllProducts(timestamp);
        }

        public void SaveBrands()
        {
            new ThreegDL().SaveBrands();
        }

        public void SaveThreegCategoryForCategory(int categoryID, int threegCategoryID, bool isCategory1, bool isCategory2)
        {
            new ThreegDL().SaveThreegCategoryForCategory(categoryID, threegCategoryID, isCategory1, isCategory2);
        }

        public void DeleteThreegCategoryForCategory(int categoryID)
        {
            new ThreegDL().DeleteThreegCategoryForCategory(categoryID);
        }

        public int GetCategory1ForCategory(int categoryID)
        {
            return new ThreegDL().GetCategory1ForCategory(categoryID);
        }

        public int GetCategory2ForCategory(int categoryID)
        {
            return new ThreegDL().GetCategory2ForCategory(categoryID);
        }

        public DataTable GetThreegCategoriesForCategory(int categoryID)
        {
            return new ThreegDL().GetThreegCategoriesForCategory(categoryID);
        }

        public int[] UpdateProducts(int categoryID, int threegCategoryID)
        {
            ProductDL productDL = new ProductDL();

            productDL.SetInStock(1014, false, categoryID, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
            DataTable threegProducts = GetProductsByCategory3ID(threegCategoryID);
            int updatedCount = 0;
            Category category = new CategoryBL().GetCategory(categoryID);

            foreach(DataRow product in threegProducts.Rows)
            {
                int productID = 0; //new ProductBL().GetProductIDBySupplierCode(product["sifra"].ToString());
                if((productID = new ProductBL().GetProductIDBySupplierCode(product["sifra"].ToString())) > 0)
                {
                    if(!productDL.IsLocked(productID))
                    {
                        double price = calculatePrice(double.Parse(product["vpCena"].ToString()), category.PricePercent);
                        double webPrice = calculatePrice(double.Parse(product["vpCena"].ToString()), category.WebPricePercent);
                        updatedCount += productDL.UpdatePriceAndStock(productID, price, webPrice, true, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
                    }
                }
            }

            return new int[] { threegProducts.Rows.Count - updatedCount, updatedCount };
        }

        public DataTable GetProductsByCategory3ID(int category3ID)
        {
            return new ThreegDL().GetProductByCategory3ID(category3ID);
        }
    }
}
