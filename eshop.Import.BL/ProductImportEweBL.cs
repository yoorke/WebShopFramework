using eshop.Import.BE;
using eshop.Import.BL.AbstractClasses;
using eshop.Import.BL.Interfaces;
using eshop.Import.DL;
using eshop.Import.DL.Interfaces;
using eshopBE;
using eshopBL;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace eshop.Import.BL
{
    public class ProductImportEweBL : BaseProductImportBL, IProductImportBL
    {
        private IProductImportDL _productImportDL = new ProductImportEweDL();
        private IProductImportDescriptionBL _productImportDescriptionBL = new ProductImportDescriptionBL();

        public ProductImportEweBL() : base(new ProductImportEweDL(), "ewe")
        {

        }

        protected override List<ProductImage> parseProductImages(string importImages)
        {
            List<ProductImage> images = new List<ProductImage>();
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(importImages);
            foreach(XmlNode importImage in xmlDoc.SelectSingleNode("images").ChildNodes)
            {
                if(importImage.Name.Equals("image"))
                {
                    string imageUrl = saveImageFromUrl(importImage.InnerText.Trim(), images.Count);
                    if(!string.IsNullOrEmpty(imageUrl))
                    {
                        images.Add(new ProductImage(imageUrl, images.Count + 1));
                    }
                }
            }

            return images;
        }

        protected override List<AttributeValue> parseProductAttributes(string importAttributes, int categoryID)
        {
            List<AttributeValue> attributes = new List<AttributeValue>();

            return attributes;
        }

        private int updatePriceAndStock(XmlDocument xmlDoc)
        {
            ProductDL productDL = new ProductDL();
            CategoryBrandBL categoryBrandBL = new CategoryBrandBL();
            Category categoryItem;
            Supplier supplier = new SupplierDL().GetSupplier("ewe");
            double b2bPrice = 0;
            double quantity = 0;
            double quantityReservation = 0;
            bool isInStock = false;
            int updatedCount = 0;

            if(xmlDoc != null)
            {
                productDL.SetInStock(supplier.SupplierID, false, -1, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));

                XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("product");
                foreach(XmlNode xmlProduct in productList)
                {
                    string supplierProductCode = xmlProduct.SelectSingleNode("id").InnerText.Trim();
                    ProductUpdatePrice product;
                    double.TryParse(xmlProduct.SelectSingleNode("stock").InnerText.Trim(), out quantity);
                    double.TryParse(xmlProduct.SelectSingleNode("stockReservation").InnerText.Trim(), out quantityReservation);

                    isInStock = quantity - quantityReservation > 0;

                    if((product = productDL.GetProductBySupplierAndProductCode("ewe", supplierProductCode)) != null)
                    {
                        if(!product.IsLocked)
                        {
                            double.TryParse(xmlProduct.SelectSingleNode("price_rebate").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice);
                            if (!product.IsPriceLocked && b2bPrice > 0)
                            {
                                categoryItem = new CategoryBL().GetCategory(product.CategoryID);
                                List<double> categoryBrandPrices = categoryBrandBL.GetPricePercent(product.CategoryID, product.BrandID);
                                //if(double.TryParse(xmlProduct.SelectSingleNode("price_rebate").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice)
                                    //&& b2bPrice > 0)
                                //{
                                    double price = this.calculatePrice(b2bPrice, categoryBrandPrices[0], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                    double webPrice = this.calculatePrice(b2bPrice, categoryBrandPrices[1], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                    productDL.UpdatePriceAndStock(product.ID, price, webPrice, isInStock, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
                                //}
                            }
                            else
                            {
                                if(isInStock)
                                {
                                    productDL.SetIsInStock(product.ID, true);
                                }
                            }

                            updatedCount++;
                        }
                    }
                }
            }

            return updatedCount;
        }

        public override List<ProductImport> ParseProducts(string category, List<string> subCategories, GetParameter getParameter, bool updatePriceAndStock, bool saveToDatabase)
        {
            XmlDocument xmlDoc = null;
            ProductImport product;
            List<ProductImport> products = new List<ProductImport>();
            Supplier supplier = new SupplierDL().GetSupplier("ewe");

            double pdv = 0;
            double quantity = 0;
            double b2bPrice = 0;
            double webPrice = 0;
            double price = 0;
            string timestamp = DateTime.Now.ToString("ddMMyyyyHHmm");
            double stock = 0;
            double stockReservation = 0;

            if(subCategories == null || subCategories.Count == 0)
            {
                subCategories = new List<string>();
                subCategories.Add(string.Empty);
            }

            foreach(var subCategory in subCategories)
            {
                xmlDoc = _productImportDL.DownloadProducts(category, subCategory, getParameter);

                if(xmlDoc != null)
                {
                    XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("product");
                    foreach(XmlNode xmlProduct in productList)
                    {
                        product = new ProductImport()
                        {
                            SupplierCode = "ewe",
                            Code = xmlProduct.SelectSingleNode("id").InnerText.Trim(),
                            Ean = xmlProduct.SelectSingleNode("ean").InnerText.Trim(),
                            Name = xmlProduct.SelectSingleNode("name").InnerText.Trim(),
                            Pdv = 20,
                            Category = xmlProduct.SelectSingleNode("subcategory").InnerText.Trim(),
                            ParentCategory = xmlProduct.SelectSingleNode("category").InnerText.Trim(),
                            Manufacturer = xmlProduct.SelectSingleNode("manufacturer").InnerText.Trim(),
                            UnitOfMeasure = "Kom",
                            Model = string.Empty,
                            Quantity = string.Empty,
                            Currency = supplier.CurrencyCode,
                            B2BPrice = double.TryParse(xmlProduct.SelectSingleNode("price_rebate").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice) ? b2bPrice : 0,
                            WebPrice = double.TryParse(xmlProduct.SelectSingleNode("price").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out webPrice) ? webPrice : 0,
                            Price = double.TryParse(xmlProduct.SelectSingleNode("price").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price) ? price : 0,
                            EnergyClass = string.Empty,
                            Declaration = getDeclaration(xmlProduct),
                            Description = xmlProduct.SelectSingleNode("description") != null ? xmlProduct.SelectSingleNode("description").InnerText.Trim() : string.Empty,
                            ImageUrls = xmlProduct.SelectSingleNode("images") != null ? xmlProduct.SelectSingleNode("images").InnerXml.Trim() : string.Empty,
                            Attributes = xmlProduct.SelectSingleNode("specifications") != null ? xmlProduct.SelectSingleNode("specifications").InnerXml.Trim() : string.Empty,
                            Timestamp = timestamp,
                            InsertDate = DateTime.UtcNow,
                            Stock = double.TryParse(xmlProduct.SelectSingleNode("stock").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) ? stock : 0,
                            StockReservation = double.TryParse(xmlProduct.SelectSingleNode("stockReservation").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stockReservation) ? stockReservation : 0
                        };

                        products.Add(product);
                    }
                }
            }

            if(saveToDatabase)
            {
                _productImportDL.SaveProducts(products, category);
            }

            if(updatePriceAndStock)
            {
                this.updatePriceAndStock(xmlDoc);
            }

            return products;
        }

        public override int UpdatePriceAndStock(string category, List<string> subCategories)
        {
            XmlDocument xmlDoc;
            int updatedCount = 0;
            
            if(subCategories == null || subCategories.Count == 0)
            {
                subCategories = new List<string>();
                subCategories.Add("");
            }

            GetParameter getParameter = new GetParameter()
            {
                Attributes = false,
                Images = false,
                Description = false,
                InStock = false,
                Short = false
            };

            foreach(string subCategory in subCategories)
            {
                xmlDoc = _productImportDL.DownloadProducts(category, subCategory, getParameter);

                updatedCount += updatePriceAndStock(xmlDoc);
            }

            return updatedCount;
        }

        private string getDeclaration(XmlNode xmlProduct)
        {
            StringBuilder declaration = new StringBuilder();

            if(xmlProduct.SelectSingleNode("supplier") != null)
            {
                declaration.Append($"Dobavljač: {xmlProduct.SelectSingleNode("supplier").InnerText.Trim()}");
                declaration.Append(Environment.NewLine);
            }

            if (xmlProduct.SelectSingleNode("country") != null)
            {
                declaration.Append($"Država porekla: {xmlProduct.SelectSingleNode("country").InnerText.Trim()}");
                declaration.Append(Environment.NewLine);
            }

            if(xmlProduct.SelectSingleNode("depth") != null)
            { 
                declaration.Append($"Dubina: {xmlProduct.SelectSingleNode("depth").InnerText.Trim()}");
                declaration.Append(Environment.NewLine);
            }

            if(xmlProduct.SelectSingleNode("width") != null)
            {
                declaration.Append($"Širina: {xmlProduct.SelectSingleNode("width").InnerText.Trim()}");
                declaration.Append(Environment.NewLine);
            }

            if(xmlProduct.SelectSingleNode("height") != null)
            {
                declaration.Append($"Visina: {xmlProduct.SelectSingleNode("height").InnerText.Trim()}");
            }

            return declaration.ToString();
        }
    }
}
