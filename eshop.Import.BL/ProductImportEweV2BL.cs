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
    public class ProductImportEweV2BL : BaseProductImportBL, IProductImportBL
    {
        private IProductImportDL _productImportDL = new ProductImportEweV2DL();
        private IProductImportDescriptionBL _productImportDescriptionBL = new ProductImportDescriptionBL();

        public ProductImportEweV2BL() : base(new ProductImportEweV2DL(), "ewe")
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
                    string url = importImage.SelectSingleNode("acImage").InnerText.Trim();
                    string imageUrl = saveImageFromUrl(url, images.Count);
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
            List<eshopBE.Attribute> categoryAttributes = new AttributeBL().GetAttributesForCategory(categoryID);
            XmlDocument xmlDoc = new XmlDocument();
            eshopBE.Attribute attribute;
            string groupName;
            string attributeName;
            string attributeValue;

            if(categoryAttributes == null)
            {
                return attributes;
            }

            xmlDoc.LoadXml(importAttributes);
            foreach(XmlNode importSpecification in xmlDoc.DocumentElement.SelectNodes("specification"))
            {
                groupName = importSpecification.SelectSingleNode("acGroupName").InnerText.Trim();

                foreach (XmlNode importAttribute in importSpecification.SelectNodes("filterSet"))
                {
                    attributeName = importAttribute.SelectSingleNode("acFilterSet").InnerText.Trim();
                    attributeValue = importAttribute.SelectSingleNode("filters").InnerText.Trim();

                    attribute = getAttribute(categoryAttributes, $"{groupName}-{attributeName}", categoryID);
                    if (categoryAttributes.Find(ca => ca.AttributeID == attribute.AttributeID) == null)
                    {
                        categoryAttributes.Add(attribute);
                    }

                    attributes.Add(getAttributeValue(attribute.AttributeID, attributeValue));
                }
            }

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
                    string supplierProductCode = xmlProduct.SelectSingleNode("acProduct").InnerText.Trim();
                    ProductUpdatePrice product;
                    double.TryParse(xmlProduct.SelectSingleNode("anStock").InnerText.Trim(), out quantity);
                    double.TryParse(xmlProduct.SelectSingleNode("anReserved").InnerText.Trim(), out quantityReservation);
                    b2bPrice = 0;

                    isInStock = quantity - quantityReservation > 0;

                    if((product = productDL.GetProductBySupplierAndProductCode("ewe", supplierProductCode)) != null)
                    {
                        if (!product.IsLocked)
                        {
                            //double.TryParse(xmlProduct.SelectSingleNode("anPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice);
                            b2bPrice = getB2BPrice(xmlProduct);
                            if(!product.IsPriceLocked && b2bPrice > 0)
                            {
                                categoryItem = new CategoryBL().GetCategory(product.CategoryID);
                                List<double> categoryBrandPrices = categoryBrandBL.GetPricePercent(product.CategoryID, product.BrandID);
                                double price = this.calculatePrice(b2bPrice, categoryBrandPrices[0], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                double webPrice = this.calculatePrice(b2bPrice, categoryBrandPrices[1], categoryItem.PriceFixedAmount, supplier.CurrencyCode);
                                productDL.UpdatePriceAndStock(product.ID, price, webPrice, isInStock, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
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
            List<ProductImport> products = new List<ProductImport>();
            ProductImport product;
            Supplier supplier = new SupplierDL().GetSupplier("ewe");
            XmlDocument xmlDoc = null;

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
                            Code = xmlProduct.SelectSingleNode("acProduct").InnerText.Trim(),
                            Ean = xmlProduct.SelectSingleNode("acEan").InnerText.Trim(),
                            Name = xmlProduct.SelectSingleNode("acName").InnerText.Trim(),
                            Pdv = 20,
                            Category = xmlProduct.SelectSingleNode("acSubCategory").InnerText.Trim(),
                            ParentCategory = xmlProduct.SelectSingleNode("acCategory").InnerText.Trim(),
                            Manufacturer = xmlProduct.SelectSingleNode("acDept").InnerText.Trim(),
                            UnitOfMeasure = "kom",
                            Model = string.Empty,
                            Quantity = string.Empty,
                            Currency = supplier.CurrencyCode,
                            //B2BPrice = double.TryParse(xmlProduct.SelectSingleNode("anPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice) ? b2bPrice : 0,
                            B2BPrice = getB2BPrice(xmlProduct),
                            //WebPrice = double.TryParse(xmlProduct.SelectSingleNode("anPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out webPrice) ? webPrice : 0,
                            WebPrice = 0,
                            //Price = double.TryParse(xmlProduct.SelectSingleNode("anPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price) ? price : 0,
                            Price = 0,
                            EnergyClass = string.Empty,
                            Declaration = getDeclaration(xmlProduct),
                            //Description = !string.IsNullOrEmpty(xmlProduct.SelectSingleNode("acProductDescription").InnerText.Trim()) ? xmlProduct.SelectSingleNode("acProductDescription").InnerText.Trim() : xmlProduct.SelectSingleNode("acInlineSpecification").InnerText.Trim(),
                            Description = xmlProduct.SelectSingleNode("acProductDescription").InnerText.Trim(),
                            ImageUrls = getImages(xmlProduct),
                            //Attributes = xmlProduct.SelectSingleNode("acInlineSpecification").InnerText.Trim(),
                            Attributes = getAttributes(xmlProduct),
                            Timestamp = timestamp,
                            InsertDate = DateTime.UtcNow,
                            Stock = double.TryParse(xmlProduct.SelectSingleNode("anStock").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stock) ? stock : 0,
                            StockReservation = double.TryParse(xmlProduct.SelectSingleNode("anReserved").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out stockReservation) ? stockReservation : 0,
                            Weight = getWeight(xmlProduct)
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

        private double getB2BPrice(XmlNode xmlProduct)
        {
            double b2bPrice = 0;
            bool isPromotion = xmlProduct.SelectSingleNode("isPromotion").InnerText.Trim().Equals("1");

            if(!isPromotion)
            {
                double.TryParse(xmlProduct.SelectSingleNode("anPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice);
            }
            else if(isPromotion)
            {
                double.TryParse(xmlProduct.SelectSingleNode("anPromoPrice").InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out b2bPrice);
            }

            return b2bPrice;
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
            bool hasWidth = false;
            bool hasHeight = false;
            bool hasDepth = false;

            if (xmlProduct.SelectSingleNode("acCountry") != null)
            {
                declaration.Append($"<strong>Zemlja porekla:</strong> {xmlProduct.SelectSingleNode("acCountry").InnerText.Trim()}");
                //declaration.Append(Environment.NewLine);
                declaration.Append("<br>");
            }

            if(xmlProduct.SelectSingleNode("acSupplier") != null)
            {
                declaration.Append($"<strong>Dobavljač:</strong> {xmlProduct.SelectSingleNode("acSupplier").InnerText.Trim()}");
                //declaration.Append(Environment.NewLine);
                declaration.Append("<br>");
            }

            declaration.Append("<strong>Dimenzije:</strong> ");

            if(xmlProduct.SelectSingleNode("anDimWidth") != null)
            {
                declaration.Append($"{xmlProduct.SelectSingleNode("anDimWidth").InnerText.Trim()} mm");
                //declaration.Append(Environment.NewLine);
                //declaration.Append("<br>");
                hasWidth = true;
            }

            if(xmlProduct.SelectSingleNode("anDimHeight") != null)
            {
                if (hasWidth)
                {
                    declaration.Append(" x ");
                }
                declaration.Append($"{xmlProduct.SelectSingleNode("anDimHeight").InnerText.Trim()} mm");
                //declaration.Append(Environment.NewLine);
                //declaration.Append("<br>");
                hasHeight = true;
            }

            if (xmlProduct.SelectSingleNode("anDimDepth") != null)
            {
                if (hasWidth || hasHeight)
                {
                    declaration.Append(" x ");
                }
                declaration.Append($"{xmlProduct.SelectSingleNode("anDimDepth").InnerText.Trim()} mm");
                //declaration.Append(Environment.NewLine);
                //declaration.Append("<br>");
            }

            if(!hasWidth && !hasHeight && !hasDepth)
            {
                declaration.Append(" NP");
                //declaration.Append("<br>");
            }

            if (xmlProduct.SelectSingleNode("anDimWeightBrutto") != null)
            {
                declaration.Append("<br>");
                declaration.Append($"<strong>Težina:</strong> {xmlProduct.SelectSingleNode("anDimWeightBrutto").InnerText.Trim()} kg");
            }

            return declaration.ToString();
        }

        private string getImages(XmlNode xmlProduct)
        {
            StringBuilder images = new StringBuilder();

            images.Append("<images>");

            foreach(XmlNode image in xmlProduct.SelectNodes("urlImages"))
            {
                images.Append("<image>");
                images.Append(image.InnerXml);
                images.Append("</image>");
            }

            images.Append("</images>");

            return images.ToString();
        }

        private string getAttributes(XmlNode xmlProduct)
        {
            StringBuilder attributes = new StringBuilder();

            attributes.Append("<specifications>");

            foreach(XmlNode attribute in xmlProduct.SelectNodes("specification"))
            {
                attributes.Append(attribute.OuterXml);
            }

            attributes.Append("</specifications>");

            return attributes.ToString();
        }

        private string getWeight(XmlNode xmlProduct)
        {
            string weight = "0";

            if(xmlProduct.SelectSingleNode("anDimWeightBrutto") != null)
            {
                weight = xmlProduct.SelectSingleNode("anDimWeightBrutto").InnerText.Trim();
            }

            return weight;
        }
    }
}
