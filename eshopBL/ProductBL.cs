using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;
using System.Data;
using System.Web;
using System.IO;
using System.Drawing;
using eshopUtilities;
using System.Configuration;

namespace eshopBL
{
    public class ProductBL
    {
        public Product GetProduct(int productID, string url, bool count, string code)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProduct(productID, url, count, string.Empty);
        }

        /*public List<Product> GetProduct(string code)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProduct(code);
        }*/

        public List<Product> GetProducts(List<AttributeValue> attributes)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProducts(attributes);
        }

        public int SaveProduct(Product product)
        {
            ProductDL productDL = new ProductDL();
            if (product.ProductID > 0)
            {
                product.UpdateDate = DateTime.Now.ToUniversalTime();
                return productDL.UpdateProduct(product);
            }
            else
            {
                product.InsertDate = DateTime.Now.ToUniversalTime();
                product.UpdateDate = DateTime.Now.ToUniversalTime();
                return productDL.SaveProduct(product);
            }
        }

        public List<Product> GetProductsForPromotion(int promotionID)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProductsForPromotion(promotionID);
        }

        public int DeleteProduct(int productID)
        {
            ProductDL productDL = new ProductDL();
            return productDL.DeleteProduct(productID);
        }

        public List<Product> GetAllProducts()
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetAllProducts();
        }

        public List<Product> GetProductsForCategory(int categoryID, bool isActive, bool isApproved)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProductsForCategory(categoryID, isActive, isApproved);
        }

        public int GetProductIDBySupplierCode(string supplierCode)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProductIDBySupplierCode(supplierCode);
        }

        public List<Product> GetProducts(int categoryID, int supplierID, string isApprovedName, string isActiveName, int? brandID, int? promotionID, string sort = "brand.name, product.name")
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProducts(categoryID, supplierID, getApproved(isApprovedName), getActive(isActiveName), brandID, promotionID, sort);
        }

        private bool? getApproved(string isApprovedName)
        {
            bool? isApproved = null;
            switch (isApprovedName)
            {
                case "Odobrene": { isApproved = true; break; }
                case "Neodobrene": { isApproved = false; break; }
            }
            return isApproved;
        }

        private bool? getActive(string isActiveName)
        {
            bool? isActive = null;
            switch (isActiveName)
            {
                case "Aktivne": { isActive = true; break; }
                case "Neaktivne": { isActive = false; break; }
            }
            return isActive;
        }

        public int SetApproved(int productID, bool isApproved)
        {
            ProductDL productDL = new ProductDL();
            return productDL.SetApproved(productID, isApproved);
        }

        public int SetActive(int productID, bool isActive)
        {
            ProductDL productDL = new ProductDL();
            return productDL.SetActive(productID, isActive);
        }

        public int SetLocked(int productID, bool isLocked)
        {
            ProductDL productDL = new ProductDL();
            return productDL.SetLocked(productID, isLocked);
        }

        public int SetIsInStock(int productID, bool isInStock)
        {
            ProductDL productDL = new ProductDL();
            return productDL.SetIsInStock(productID, isInStock);
        }

        public List<Product> GetProducts(string categoryUrl, List<string> brandsID, List<AttributeValue> attributeValues, string sortName, string priceFrom, string priceTo, bool includeChildrenCategoriesProducts = false)
        {
            /*string sort = string.Empty;
            switch (sortString)
            {
                case "Nazivu":
                    {
                        sort = " brand.name";
                        break;
                    }

                case "Ceni opadajuće":
                    {
                        sort = " product.price DESC";
                        break;
                    }

                case "Ceni rastuće":
                    {
                        sort = " product.price";
                        break;
                    }
            }*/
            CategoryDL categoryDL = new CategoryDL();
            Category category = categoryDL.GetCategoryByUrl(categoryUrl);


            ProductDL productDL = new ProductDL();
            return productDL.GetProducts(category.CategoryID, brandsID, attributeValues, getSort(sortName), getPrice(priceFrom), getPrice(priceTo), includeChildrenCategoriesProducts);
        }

        private string getSort(string sortName)
        {
            string sort = " product.name";
            switch (sortName)
            {
                case "name": { sort = " product.name" + (bool.Parse(ConfigurationManager.AppSettings["sortProductsByDescriptionAlso"].ToString()) ? ", product.Description" : string.Empty) ; break; }
                case "priceDesc": { sort = " product.price DESC"; break; }
                case "priceAsc": { sort = " product.price"; break; }
                case "sortIndex": { sort = " sortIndex"; break; }
            }
            return sort;
        }

        public bool IsLocked(int productID)
        {
            ProductDL productDL = new ProductDL();
            return productDL.IsLocked(productID);
        }

        private double getPrice(string priceString)
        {
            double price = 0;
            double.TryParse(priceString, out price);

            return price;
        }

        public int SetInStock(int supplierID, bool inStock, int categoryID)
        {
            ProductDL productDL = new ProductDL();
            return productDL.SetInStock(supplierID, inStock, categoryID, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
        }

        public int UpdatePriceAndStock(int productID, double price, double webPrice, bool isInStock)
        {
            ProductDL productDL = new ProductDL();
            return productDL.UpdatePriceAndStock(productID, price, webPrice, isInStock, bool.Parse(ConfigurationManager.AppSettings["showIfNotInStock"]));
        }

        public List<Product> GetProductsForFirstPage(int categoryID, int brandID, int numberOfProducts, string orderBy)
        {
            ProductDL productDL = new ProductDL();
            switch (orderBy)
            {
                case "Novi": { orderBy = "insertDate DESC"; break; }
                case "Ceni": { orderBy = "webPrice"; break; }
                case "Slučajni": { orderBy = "NEWID()"; break; }
            }
            return productDL.GetProductsForFirstPage(categoryID, brandID, numberOfProducts, orderBy);
        }

        public double GetMinPrice(int categoryID)
        {
            return new ProductDL().GetMinMaxPriceForCategory(categoryID)[0];
        }

        public double GetMaxPrice(int categoryID)
        {
            return new ProductDL().GetMinMaxPriceForCategory(categoryID)[1];
        }

        public double[] GetMinMaxPrice(string categoryName, bool includeChildrenCategories = false)
        {
            Category category = new CategoryBL().GetCategory(categoryName);
            return new ProductDL().GetMinMaxPriceForCategory(category.CategoryID, includeChildrenCategories);
        }

        public DataTable GetLast10()
        {
            return new ProductDL().GetLast10();
        }

        public int GetMaxImageID()
        {
            return new ProductDL().GetMaxImageID();
        }

        public DataTable GetTop10Access()
        {
            return new ProductDL().GetTop10Access();
        }

        public DataTable GetTop10Order()
        {
            return new ProductDL().GetTop10Order();
        }

        public List<Product> SearchProducts(string search, string sort)
        {
            return new ProductDL().SearchProducts(search, getSort(sort));
        }

        public void SetPromotionPrice(int productID, double price, double value, int promotionID)
        {
            double promotionPrice = value > 0 ? bool.Parse(ConfigurationManager.AppSettings["roundPromotionPrice"]) ? ((int)(price * (1 - value / 100))) / 100 * 100 - 10 : price * (1 - value / 100) : price;
            new ProductDL().SetPromotionPrice(productID, promotionID, promotionPrice);
        }

        public void DeleteFromPromotion(int productID, int promotionID)
        {
            new ProductDL().DeleteFromPromotion(productID, promotionID);
        }

        public string CreateNewImageName(int productImagesCount)
        {
            int id = new ProductDL().GetMaxImageID() + productImagesCount;
            string directory = CreateImageDirectory(id);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/" + directory)))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/" + directory));
            return directory + id.ToString();
        }

        public string CreateImageDirectory(int id)
        {
            StringBuilder directory = new StringBuilder();
            directory.Append("/images/p/");
            string imageId = id.ToString();
            for (int i = 0; i < imageId.Length; i++)
                directory.Append(imageId[i].ToString() + "/");
            return directory.ToString();
        }

        public bool CreateProductImages(string fullPath)
        {
            bool exist = false;
            if(File.Exists(fullPath))
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                if(fileInfo.Length > 0)
                    try
                    { 
                        exist = true;
                        string extension = fullPath.Substring(fullPath.LastIndexOf('.'));
                        Image original = Image.FromFile(fullPath);
                        Image thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["mainWidth"]), int.Parse(ConfigurationManager.AppSettings["mainHeight"]));
                        thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["mainName"] + extension);

                        thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["listWidth"]), int.Parse(ConfigurationManager.AppSettings["listHeight"]));
                        thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["listName"] + extension);

                        thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["thumbWidth"]), int.Parse(ConfigurationManager.AppSettings["thumbHeight"]));
                        thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["thumbName"] + extension);
                    }
                    catch
                    {
                        return false;
                    }
            }
            return exist;
        }

        public int SaveProductFromExternalApplication(string barcode, string name, double quantity, double price, int externalCategoryID, bool insertIfNew)
        {
            //Product product = new ProductDL().GetProduct(-1, string.Empty, false, barcode);
            List<Product> products = new ProductDL().GetProductsByBarcode(barcode);
            int status = -1;
            

            if (products != null && products.Count > 0)
            {
                foreach (Product product in products)
                {
                    if(product.Promotion == null)
                    { 
                        product.Name = name != "none" && products.Count == 1 ? name : product.Name;
                        product.Price = price;
                        product.WebPrice = price;
                        product.IsInStock = quantity > -1 ? (quantity > 0 ? true : false) : product.IsInStock;
                        if(externalCategoryID > 0)
                        {
                            Category category = new CategoryBL().GetCategoryByExternalID(externalCategoryID);
                            product.Categories = new List<Category>();
                            product.Categories.Add(category);
                        }
                        status = SaveProduct(product) > 0 ? 2 : 0;
                    }
                }
            }
            else if (insertIfNew && name != "none")
            {
                Product newProduct = new Product();
                newProduct.Code = barcode;
                newProduct.Name = name;
                newProduct.Price = price;
                newProduct.WebPrice = price;

                newProduct.Brand = new Brand(0, "Nepoznat");
                newProduct.Categories = new List<Category>();
                if (externalCategoryID <= 0)
                    newProduct.Categories.Add(new Category(9999, "Nepoznat", null, string.Empty, string.Empty, 0, 0, 0, string.Empty, false, -1, false, false, 0, 0, 0));
                else newProduct.Categories.Add(new CategoryBL().GetCategoryByExternalID(externalCategoryID));
                newProduct.Description = string.Empty;
                newProduct.Ean = string.Empty;
                newProduct.Images = new List<ProductImage>();
                newProduct.Images.Add(new ProductImage("0.jpg", 1));
                newProduct.IsActive = false;
                newProduct.IsApproved = false;
                newProduct.IsInStock = quantity > 0;
                newProduct.IsLocked = false;
                newProduct.Specification = string.Empty;
                newProduct.SupplierCode = string.Empty;
                newProduct.SupplierID = 0;
                newProduct.VatID = 4;
                newProduct.UnitOfMeasure = new UnitOfMeasure(1, "Nepoznata", "NN");
                status = SaveProduct(newProduct) > 0 ? 1 : 0;
            }
            return status;
        }

        public string GetProductSpecificationText(int productID)
        {
            return new ProductDL().GetProductSpecificationText(productID);
        }

        public DataTable GetProductsBarcodes()
        {
            return new ProductDL().GetProductsBarcodes();
        }

        public int SaveProductCategory(int productID, int categoryID)
        {
            return new ProductDL().SaveProductCategory(productID, categoryID);
        }

        public double GetActualPrice(int productID)
        {
            return new ProductDL().GetActualPrice(productID);
        }

        public DataTable GetProductsDataTable(int? categoryID, int? supplierID, int? promotionID, int? brandID, string isActiveName, string isApprovedName, string search)
        {
            return new ProductDL().GetProductsDataTable(categoryID, supplierID, promotionID, brandID, getActive(isActiveName), getApproved(isApprovedName), search);
        }

        public DataTable GetProductsDataTable()
        {
            return new ProductDL().GetProductsDataTable();
        }

        public List<Product> GetProductsForSitemap(int categoryID)
        {
            return new ProductDL().GetProductForSiteMap(categoryID);
        }
    }
}
