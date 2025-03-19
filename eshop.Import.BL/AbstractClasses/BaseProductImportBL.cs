using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using eshop.Import.BE;
using eshop.Import.BL.Interfaces;
using eshop.Import.DL.AbstractClasses;
using eshop.Import.DL.Interfaces;
using eshopBE;
using eshopBL;
using eshopDL;
using eshopUtilities;

namespace eshop.Import.BL.AbstractClasses
{
    public abstract class BaseProductImportBL : IProductImportBL
    {
        private IProductImportDL _productImportDL;
        private string _supplierCode;
        private AttributeBL _attributeBL = new AttributeBL();
        private IProductImportDescriptionBL _productImportDescriptionBL = new ProductImportDescriptionBL();
        private Settings _settings;

        public BaseProductImportBL(IProductImportDL productImportDL, string supplierCode)
        {
            _productImportDL = productImportDL;
            _supplierCode = supplierCode;
            _settings = new SettingsBL().GetSettings();
        }

        //public void DownloadProducts(GetParameter getParameter)
        //{
        //    _productImportDL.DownloadProducts(getParameter);
        //}

        public abstract List<ProductImport> ParseProducts(string category, List<string> subCategories, GetParameter getParameter, bool updatePriceAndStock, bool saveToDatabase);
        public abstract int UpdatePriceAndStock(string category, List<string> subCategories);
        //public abstract void UpdatePriceAndStock(XmlDocument xmlDoc);
        protected abstract List<ProductImage> parseProductImages(string importImages);
        protected abstract List<eshopBE.AttributeValue> parseProductAttributes(string importAttributes, int categoryID);

        protected double calculatePrice(double supplierPrice, double percent, double priceFixedAmount, string currency = "RSD")
        {
            if(currency == "EUR")
            {
                supplierPrice = supplierPrice * _settings.ExchangeRate;
            }

            double price = ((int)((supplierPrice * (percent / 100 + 1) + priceFixedAmount) * 1.2));
            int roundIndex = getRoundIndex(price);

            return double.Parse(((int)price / roundIndex * roundIndex + roundIndex - getRoundSubstractValue(price)).ToString());
        }

        private int getRoundIndex(double price)
        {
            if(price < 1000)
            {
                return 10;
            }

            return 100;
        }

        private int getRoundSubstractValue(double price)
        {
            if(price < 1000)
            {
                return 0;
            }

            return 10;
        }

        protected string saveImageFromUrl(string imageUrl, int count)
        {
            bool exists = true;
            WebClient webClient = new WebClient();
            bool useWebpImages = bool.Parse(ConfigurationManager.AppSettings["useWebpImages"]);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            try
            {
                string filename = Path.GetFileName(imageUrl);
                string extension = filename.Substring(filename.LastIndexOf('.'));
                string fullPath = HttpContext.Current.Server.MapPath("~") + new ProductBL().CreateNewImageName(count);
                string path = fullPath.Substring(0, fullPath.LastIndexOf('/'));

                webClient.DownloadFile(imageUrl, fullPath + extension);

                exists = new ProductBL().CreateProductImages(fullPath + extension);

                string newFullPath = fullPath + (!useWebpImages ? extension : ".webp");

                return exists ? 
                    newFullPath.Substring(fullPath.LastIndexOf('/') + 1) 
                    : string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public List<ProductImport> GetProducts(string category, List<string> subCategories, string manufacturer)
        {
            return _productImportDL.GetProducts(category, subCategories, manufacturer == "Svi" ? null : manufacturer);
        }

        public void SaveProduct(string supplierProductCode, bool isApproved, bool isActive, int categoryID)
        {
            ProductImport productImport = _productImportDL.GetProductBySupplierCode(_supplierCode, supplierProductCode);
            Category category = new CategoryBL().GetCategory(categoryID);
            //List<eshopBE.Attribute> attributes = new AttributeBL().GetAttributesForCategory(categoryID);
            Supplier supplier = new SupplierBL().GetSupplier(_supplierCode);
            double weight = 0;

            if(productImport.B2BPrice == 0 && !bool.Parse(ConfigurationManager.AppSettings["allowImportWithoutPrice"]))
            {
                throw new BLException("Proizvod nema nabavnu cenu.");
            }

            Product product = new Product()
            {
                IsApproved = isApproved,
                IsActive = isActive,
                SupplierCode = productImport.Code,
                SupplierID = supplier.SupplierID,
                VatID = 4,
                Categories = new List<Category>() { category },
                Specification = "",
                IsInStock = true,
                Code = supplierProductCode,
                Description = productImport.Description,
                Declaration = productImport.Declaration,
                WeightRangeID = 1,
                Comment = string.Empty,
                CanBeDelivered = true,
                Name = productImport.Name,
                Ean = productImport.Ean,
                SupplierPrice = double.Parse(productImport.B2BPrice.ToString()),
                UnitOfMeasure = new UnitOfMeasure(2, "Komad", "kom"),
                ShortDescription = string.Empty,
                //Attributes = _attributeBL.GetAttributes(_productImportDescriptionBL.GetDescriptionAttributes(productImport.Description))
                Attributes = new List<AttributeValue>(),
                Weight = double.TryParse(productImport.Weight, out weight) ? weight : 0
            };

            //ProductUpdatePrice productUP = new ProductBL().GetProductBySupplierCode(product.SupplierCode);
            ProductUpdatePrice productUP = new ProductBL().GetProductBySupplierAndProductCode(_supplierCode, product.SupplierCode);
            product.ProductID = productUP != null ? productUP.ID : -1;
            product.IsLocked = productUP != null ? productUP.IsLocked : false;
            product.IsPriceLocked = productUP != null ? productUP.IsPriceLocked : false;

            Brand brand = new BrandBL().GetBrandByName(productImport.Manufacturer);
            if(brand == null)
            {
                brand = new Brand();
                brand.Name = productImport.Manufacturer;
                brand.LogoUrl = string.Empty;
                brand.BrandID = new BrandBL().SaveBrand(brand);
            }
            product.Brand = brand;

            if(!product.IsPriceLocked)
            {
                List<double> categoryBrandPrice = new CategoryBrandBL().GetPricePercent(categoryID, product.Brand.BrandID);

                product.Price = calculatePrice(productImport.B2BPrice, categoryBrandPrice[0], category.PriceFixedAmount, productImport.Currency);
                product.WebPrice = calculatePrice(productImport.B2BPrice, categoryBrandPrice[1], category.PriceFixedAmount, productImport.Currency);
            }

            product.Images = parseProductImages(productImport.ImageUrls);

            product.Attributes = parseProductAttributes(productImport.Attributes, categoryID);

            if(bool.Parse(ConfigurationManager.AppSettings["saveProductWithoutImage"]) || product.Images.Count > 0)
            {
                new ProductBL().SaveProduct(product);
            }
        }

        public void SaveProducts(List<ProductImport> products)
        {
            _productImportDL.SaveProducts(products, "");
        }

        public int UpdateProducts(List<ProductImport> products)
        {
            ProductDL productDL = new ProductDL();
            Supplier supplier = new SupplierDL().GetSupplier(_supplierCode);
            ProductUpdatePrice productUpdatePrice;
            Category categoryItem;
            CategoryBrandBL categoryBrandBL = new CategoryBrandBL();
            int updatedCount = 0;

            //productDL.SetInStock(supplier.SupplierID, false, -1, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));

            foreach(var product in products)
            {
                if((productUpdatePrice = productDL.GetProductBySupplierAndProductCode(_supplierCode, product.Code)) != null)
                {
                    if(!productUpdatePrice.IsLocked)
                    {
                        if(!productUpdatePrice.IsPriceLocked && product.B2BPrice > 0)
                        {
                            categoryItem = new CategoryBL().GetCategory(productUpdatePrice.CategoryID);
                            List<double> categoryBrandPrices = categoryBrandBL.GetPricePercent(productUpdatePrice.CategoryID, productUpdatePrice.BrandID);
                            double price = calculatePrice(product.B2BPrice, categoryBrandPrices[0], categoryItem.PriceFixedAmount, product.Currency);
                            double webPrice = calculatePrice(product.B2BPrice, categoryBrandPrices[1], categoryItem.PriceFixedAmount, product.Currency);
                            productDL.UpdatePriceAndStock(productUpdatePrice.ID, price, webPrice, true, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
                        }
                        else
                        {
                            if(!product.Quantity.Equals("0"))
                            {
                                productDL.SetIsInStock(productUpdatePrice.ID, true);
                            }
                        }
                    }

                    updatedCount++;
                }
            }

            this.SetStock(_supplierCode, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));

            return updatedCount;
        }

        public List<ManufacturerImport> GetManufacturers(List<string> subCategories, bool addSelectAll = true)
        {
            List<ManufacturerImport> manufacturers = _productImportDL.GetManufacturers(subCategories);

            if(addSelectAll)
            {
                manufacturers.Insert(0, new ManufacturerImport()
                {
                    Name = "Svi"
                });
            }
            return manufacturers;
        }

        protected eshopBE.Attribute getAttribute(List<eshopBE.Attribute> attributes, string attributeName, int categoryID)
        {
            AttributeBL attributeBL = new AttributeBL();
            eshopBE.Attribute attribute = attributes.Find(a => a.Name == attributeName);
            //int attributeID;

            if(attribute != null && attribute.AttributeID > 0)
            {
                return attribute;
            }

            attribute = new eshopBE.Attribute()
            {
                Name = attributeName,
                Filter = false,
                AttributeID = -1,
                DisplayName = attributeName
            };

            attribute.AttributeID = attributeBL.SaveAttribute(attribute);

            attributeBL.SaveAttributeForCategory(categoryID, attribute.AttributeID);

            return attribute;
        }

        protected AttributeValue getAttributeValue(int attributeID, string value)
        {
            AttributeBL attributeBL = new AttributeBL();
            List<AttributeValue> attributeValues = attributeBL.GetAttributeValues(attributeID);
            AttributeValue attributeValue;

            attributeValue = attributeValues.Find(a => a.Value == value);

            if(attributeValue != null && attributeValue.AttributeValueID > 0)
            {
                return attributeValue;
            }

            attributeValue = new AttributeValue()
            {
                AttributeID = attributeID,
                AttributeValueID = -1,
                Value = value
            };

            attributeValue.AttributeValueID = attributeBL.SaveAttributeValue(attributeValue, false);

            return attributeValue;
        }

        public void SetStock(string supplierCode, bool showIfNotInStock)
        {
            _productImportDL.SetStock(supplierCode, showIfNotInStock);
        }
    }
}
