using eshop.Import.BL.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eshop.Import.DL;
using eshop.Import.DL.Interfaces;
using eshop.Import.BL.Interfaces;
using eshop.Import.BE;
using System.Xml;
using eshopBE;
using System.Globalization;
using eshopDL;
using eshopBL;
using System.Configuration;

namespace eshop.Import.BL
{
    public class ProductImportDSCBL : BaseProductImportBL, IProductImportBL
    {
        private IProductImportDL _productImportDL = new ProductImportDSCDL();
        private IProductImportDescriptionBL _productImportDescriptionBL = new ProductImportDescriptionBL();

        public ProductImportDSCBL() : base(new ProductImportDSCDL(), "dsc")
        {

        }

        protected override List<ProductImage> parseProductImages(string importImages)
        {
            List<ProductImage> images = new List<ProductImage>();
            XmlDocument xmlDoc = new XmlDocument();

            if (string.IsNullOrEmpty(importImages))
            {
                return images;
            }

            xmlDoc.LoadXml(importImages);
            foreach (XmlNode importImage in xmlDoc.SelectSingleNode("slike").ChildNodes)
            {
                if (importImage.Name.Equals("slika"))
                {
                    string imageUrl = saveImageFromUrl(importImage.InnerText.Trim(), images.Count);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        images.Add(new ProductImage(imageUrl, images.Count + 1));
                    }
                }
            }

            return images;
        }

        protected override List<eshopBE.AttributeValue> parseProductAttributes(string importAttributes, int categoryID)
        {
            List<eshopBE.AttributeValue> attributes = new List<eshopBE.AttributeValue>();

            return attributes;
        }

        private int updatePriceAndStock(XmlDocument xmlDoc)
        {
            ProductDL productDL = new ProductDL();
            CategoryBrandBL categoryBrandBL = new CategoryBrandBL();
            Category categoryItem;
            Supplier supplier = new SupplierDL().GetSupplier("dsc");
            double b2bPrice = 0;
            int updatedCount = 0;

            if(xmlDoc != null)
            {
                productDL.SetInStock(supplier.SupplierID, false, -1, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));

                XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("artikal");
                foreach(XmlNode xmlProduct in productList)
                {
                    string supplierProductCode = xmlProduct.SelectSingleNode("sifra").InnerText.Trim();
                    ProductUpdatePrice product;
                    string quantity = xmlProduct.SelectSingleNode("kolicina").InnerText.Trim();

                    if((product = productDL.GetProductBySupplierAndProductCode("dsc", supplierProductCode)) != null)
                    {
                        if(!product.IsLocked)
                        {
                            double.TryParse(xmlProduct.SelectSingleNode("cena").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice);
                            if (!product.IsPriceLocked && b2bPrice > 0)
                            {
                                categoryItem = new CategoryBL().GetCategory(product.CategoryID);
                                List<double> categoryBrandPrices = categoryBrandBL.GetPricePercent(product.CategoryID, product.BrandID);
                                //if(double.TryParse(xmlProduct.SelectSingleNode("cena").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice)
                                    //&& b2bPrice > 0)
                                //{
                                    double price = this.calculatePrice(b2bPrice, categoryBrandPrices[0], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                    double webPrice = this.calculatePrice(b2bPrice, categoryBrandPrices[1], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                    productDL.UpdatePriceAndStock(product.ID, price, webPrice, true, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
                                //}
                            }
                            else
                            {
                                if(!quantity.Equals("0"))
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

        public override List<ProductImport> ParseProducts(string category, List<string> subCategories, GetParameter getParameter, bool updatePriceAndStock = false, bool saveToDatabase = false)
        {
            XmlDocument xmlDoc = null;
            ProductImport product;
            List<ProductImport> products = new List<ProductImport>();
            double pdv = 0;
            double quantity = 0;
            double b2bPrice = 0;
            double webPrice = 0;
            double price = 0;
            string timestamp = DateTime.Now.ToString("ddMMyyyyHHmm");

            if (subCategories == null || subCategories.Count == 0)
            {
                subCategories = new List<string>();
                subCategories.Add("");
            }

            Console.WriteLine("Parse from DSC");

            foreach (var subCategory in subCategories)
            {
                xmlDoc = _productImportDL.DownloadProducts(category, subCategory, getParameter);

                if (xmlDoc != null)
                {
                    XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("artikal");
                    foreach (XmlNode xmlProduct in productList)
                    {
                        product = new ProductImport()
                        {
                            SupplierCode = "dsc",
                            Code = xmlProduct.SelectSingleNode("sifra").InnerText.Trim(),
                            Ean = xmlProduct.SelectSingleNode("barkod").InnerText.Trim(),
                            Name = xmlProduct.SelectSingleNode("naziv").InnerText.Trim(),
                            Pdv = double.TryParse(xmlProduct.SelectSingleNode("pdv").InnerText.Trim(), out pdv) ? pdv : 0,
                            Category = xmlProduct.SelectSingleNode("grupa").InnerText.Trim(),
                            ParentCategory = xmlProduct.SelectSingleNode("nadgrupa").InnerText.Trim(),
                            Manufacturer = xmlProduct.SelectSingleNode("proizvodjac").InnerText.Trim(),
                            UnitOfMeasure = xmlProduct.SelectSingleNode("jedinica_mere").InnerText.Trim(),
                            Model = xmlProduct.SelectSingleNode("model").InnerText.Trim(),
                            Quantity = xmlProduct.SelectSingleNode("kolicina").InnerText.Trim(),
                            Currency = xmlProduct.SelectSingleNode("valuta").InnerText.Trim(),
                            B2BPrice = double.TryParse(xmlProduct.SelectSingleNode("cena").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice) ? b2bPrice : 0,
                            WebPrice = double.TryParse(xmlProduct.SelectSingleNode("mpcena").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out webPrice) ? webPrice : 0,
                            Price = double.TryParse(xmlProduct.SelectSingleNode("mpcena").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price) ? price : 0,
                            EnergyClass = string.Empty,
                            Declaration = string.Empty,
                            Description = _productImportDescriptionBL.InsertClassToDescription(xmlProduct.SelectSingleNode("opis").InnerText.Trim()),
                            ImageUrls = xmlProduct.SelectSingleNode("slike").OuterXml.Trim(),
                            Attributes = xmlProduct.SelectSingleNode("karakteristike").InnerText.Trim(),
                            Timestamp = timestamp,
                            InsertDate = DateTime.UtcNow,
                            Stock = 0,
                            StockReservation = 0,
                            Weight = "0"
                        };

                        products.Add(product);
                    }
                }
            }

            if(saveToDatabase)
            {
                _productImportDL.SaveProducts(products, category);
            }

            if (updatePriceAndStock)
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

            foreach (string subCategory in subCategories)
            {
                xmlDoc = _productImportDL.DownloadProducts(category, subCategory, getParameter);

                updatedCount += updatePriceAndStock(xmlDoc);
            }

            return updatedCount;
        }
    }
}
