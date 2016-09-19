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

        public List<Product> GetProducts(int categoryID, int supplierID, string isApprovedName, string isActiveName, int? brandID, int? promotionID)
        {
            ProductDL productDL = new ProductDL();
            return productDL.GetProducts(categoryID, supplierID, getApproved(isApprovedName), getActive(isActiveName), brandID, promotionID);
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
            string sort = " brand.name";
            switch (sortName)
            {
                case "name": { sort = " brand.name"; break; }
                case "priceDesc": { sort = " product.price DESC"; break; }
                case "priceAsc": { sort = " product.price"; break; }
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
            double promotionPrice = ((int)(price / (value / 100 + 1))) / 100 * 100 - 10;
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
            return exist;
        }

        public bool SaveProductFromExternalApplication(string barcode, string name, double quantity, double price, bool insertIfNew)
        {
            Product product = new ProductDL().GetProduct(-1, string.Empty, false, barcode);
            bool save = false;

            if (product != null)
            {
                product.Name = name != "none" ? name : product.Name;
                product.Price = price;
                product.WebPrice = price;
                product.IsInStock = quantity > 0;
                save = true;
            }
            else if (insertIfNew && name != "none")
            {
                product = new Product();
                product.Code = barcode;
                product.Name = name;
                product.Price = price;
                product.WebPrice = price;

                product.Brand = new Brand(0, "Nepoznat");
                product.Categories = new List<Category>();
                product.Categories.Add(new Category(0, "Nepoznat", null, string.Empty, string.Empty, 0, 0, 0, string.Empty, false, -1));
                product.Description = string.Empty;
                product.Ean = string.Empty;
                product.Images = new List<ProductImage>();
                product.Images.Add(new ProductImage("0.jpg", 1));
                product.IsActive = false;
                product.IsApproved = false;
                product.IsInStock = quantity > 0;
                product.IsLocked = false;
                product.Specification = string.Empty;
                product.SupplierCode = string.Empty;
                product.SupplierID = 0;
                product.VatID = 4;
                save = true;
            }
            int status = 0;
            if (save)
                status = SaveProduct(product);

            return status > 0;
        }
    }
}
