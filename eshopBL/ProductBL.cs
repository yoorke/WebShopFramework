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
using eshopUtilities.Extensions;

namespace eshopBL
{
    public class ProductBL
    {
        ImageProcessor imageProc;

        public ProductBL()
        {
            imageProc = new ImageProcessor();
        }

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
            if (product.Images == null || product.Images.Count == 0)
            {
                if (product.Images == null)
                    product.Images = new List<ProductImage>();
                product.Images.Add(new ProductImage("0.jpg", 1));
            }

            for (int i = 0; i < product.Images.Count - 1; i++)
                for(int j = i + 1; j < product.Images.Count; j++)
                {
                    if(product.Images[j].SortOrder < product.Images[i].SortOrder)
                    {
                        ProductImage temp = product.Images[i];
                        product.Images[i] = product.Images[j];
                        product.Images[j] = temp;
                    }
                }
            for(int i = 0; i< product.Images.Count; i++)
            {
                product.Images[i].SortOrder = i + 1;
            }

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

        private bool? getInStock(string isInStockName)
        {
            bool? isInStock = null;
            switch(isInStockName)
            {
                case "Na stanju": { isInStock = true; break; }
                case "Nema na stanju": { isInStock = false; break; }
            }
            return isInStock;
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
            CategoryBL categoryBL = new CategoryBL();
            Category category = categoryBL.GetCategoryByUrl(categoryUrl);


            ProductDL productDL = new ProductDL();
            return productDL.GetProducts(category.CategoryID, brandsID, attributeValues, getSort(sortName), getPrice(priceFrom), getPrice(priceTo), includeChildrenCategoriesProducts || category.ShowProductsFromSubCategories);
        }

        private string getSort(string sortName)
        {
            string sort = " product.name";
            switch (sortName)
            {
                case "name": { sort = " product.name" + (bool.Parse(ConfigurationManager.AppSettings["sortProductsByDescriptionAlso"].ToString()) ? ", product.Description" : string.Empty) ; break; }
                //case "priceDesc": { sort = " product.price DESC"; break; }
                //case "priceDesc": { sort = " CASE WHEN promotionProduct.price IN NOT NULL THEN promotionProduct.price ELSE product.webPrice END DESC"; break; }
                case "priceDesc": { sort = " 17 DESC"; break; }
                //case "priceAsc": { sort = " product.price"; break; }
                //case "priceAsc": { sort = " CASE WHEN promotionProduct.price IS NOT NULL THEN promotionProduct.price ELSE product.webPrice END"; break; }
                case "priceAsc": { sort = " 17"; break; }
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
            Category category = new CategoryBL().GetCategoryByUrl(categoryName);
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

        public List<Product> SearchProducts(string search, string sort, int categoryID, int productCountLimit = -1)
        {
            return new ProductDL().SearchProducts(search, getSort(sort), categoryID, productCountLimit);
        }

        public List<ProductFPView> SearchProductsView(string search, string sort, int categoryID, int productCountLimit = -1)
        {
            List<Product> products = new ProductDL().SearchProducts(search, getSort(sort), categoryID, productCountLimit);
            List<ProductFPView> productsView = new List<ProductFPView>();

            foreach(var product in products)
            {
                productsView.Add(new ProductFPView()
                {
                    BrandName = product.Brand.Name,
                    CanBeDelivered = product.CanBeDelivered,
                    CategoryName = product.Categories[0].Name,
                    Code = product.Code,
                    FullCategoryUrl = product.FullCategoryUrl,
                    HasVariants = product.HasVariants,
                    ImageUrl = product.Images[0].ImageUrl,
                    IsFreeDelivery = product.IsFreeDelivery,
                    IsInStock = product.IsInStock,
                    Name = product.Name,
                    Price = product.Price,
                    ProductID = product.ProductID,
                    PromotionImageUrl = product.Promotion != null ? product.Promotion.ImageUrl : string.Empty,
                    PromotionPrice = product.Promotion != null ? product.Promotion.Price : product.Price,
                    WebPrice = product.Promotion != null ? product.Promotion.Price : product.WebPrice
                });
            }

            return productsView;

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
                        Image original = null;
                        if (extension.Equals(".webp"))
                        {
                            original = WebpImage.FromFile(fullPath);
                            //File.Copy(fullPath, fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["mainName"] + extension);
                            //File.Copy(fullPath, fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["listName"] + extension);
                            //File.Copy(fullPath, fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["thumbName"] + extension);
                            //return true;
                        }
                        else
                        { 
                            original = Image.FromFile(fullPath);
                            if(bool.Parse(ConfigurationManager.AppSettings["useWebpImages"]))
                            {
                                original.SaveWebp(fullPath.Substring(0, fullPath.LastIndexOf('.')) + ".webp");
                            }
                        }

                        //Image thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["mainWidth"]), int.Parse(ConfigurationManager.AppSettings["mainHeight"]));
                        //thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["mainName"] + extension);

                        //thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["listWidth"]), int.Parse(ConfigurationManager.AppSettings["listHeight"]));
                        //thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["listName"] + extension);

                        //thumb = Common.CreateThumb(original, int.Parse(ConfigurationManager.AppSettings["thumbWidth"]), int.Parse(ConfigurationManager.AppSettings["thumbHeight"]));
                        //thumb.Save(fullPath.Substring(0, fullPath.LastIndexOf('.')) + "-" + ConfigurationManager.AppSettings["thumbName"] + extension);

                        imageProc.SaveThumb(original, int.Parse(ConfigurationManager.AppSettings["mainWidth"]), int.Parse(ConfigurationManager.AppSettings["mainHeight"]), ConfigurationManager.AppSettings["mainName"], fullPath.Substring(0, fullPath.LastIndexOf('.')), extension);
                        imageProc.SaveThumb(original, int.Parse(ConfigurationManager.AppSettings["listWidth"]), int.Parse(ConfigurationManager.AppSettings["listHeight"]), ConfigurationManager.AppSettings["listName"], fullPath.Substring(0, fullPath.LastIndexOf('.')), extension);
                        imageProc.SaveThumb(original, int.Parse(ConfigurationManager.AppSettings["thumbWidth"]), int.Parse(ConfigurationManager.AppSettings["thumbHeight"]), ConfigurationManager.AppSettings["thumbName"], fullPath.Substring(0, fullPath.LastIndexOf('.')), extension);
                    }
                    catch(Exception ex)
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

                newProduct.Brand = new Brand(0, "Nepoznat", string.Empty);
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

        public DataTable GetProductsDataTable(int? categoryID, int? supplierID, int? promotionID, int? brandID, string isActiveName, string isApprovedName, string search, string sort, string reverse, string hasImage, string isInStockName = "Sve")
        {
            return new ProductDL().GetProductsDataTable(categoryID, supplierID, promotionID, brandID, getActive(isActiveName), getApproved(isApprovedName), search, sort, reverse, hasImage.Equals("Sve") ? null : (bool?)(hasImage.Equals("Ima") ? true : false), getInStock(isInStockName));
        }

        public DataTable GetProductsDataTable()
        {
            return new ProductDL().GetProductsDataTable();
        }

        public List<Product> GetProductsForSitemap(int categoryID)
        {
            return new ProductDL().GetProductForSiteMap(categoryID);
        }

        public bool ChangeCategory(int productID, int newCategoryID)
        {
            return new ProductDL().ChangeCategory(productID, newCategoryID);
        }

        public bool ImageExistsInDatabase(string filename)
        {
            return new ProductDL().ImageExistsInDatabase(filename);
        }

        public DataTable ImagesTableExistsInDatabase(DataTable images)
        {
            return new ProductDL().ImagesTableExistsInDatabase(images);
        }

        public bool IsInStock(int productID)
        {
            return new ProductDL().IsInStock(productID);
        }

        public void SetPriceLocked(int productID, bool priceLocked)
        {
            new ProductDL().SetPriceLocked(productID, priceLocked);
        }

        public bool IsPriceLocked(int productID)
        {
            return new ProductDL().IsPriceLocked(productID);
        }

        public ProductUpdatePrice GetProductBySupplierCode(string supplierCode)
        {
            return new ProductDL().GetProductBySupplierCode(supplierCode);
        }

        public ProductUpdatePrice GetProductBySupplierAndProductCode(string supplierCode, string supplierProductCode)
        {
            return new ProductDL().GetProductBySupplierAndProductCode(supplierCode, supplierProductCode);
        }

        public List<ProductSearch> SearchProductsSimple(string search, string sort, int categoryID, int productCountLimit)
        {
            List<Product> products = new ProductDL().SearchProducts(search, sort, categoryID, productCountLimit);
            List<ProductSearch> productsSearch = new List<ProductSearch>();

            foreach(Product product in products)
            {
                ProductSearch productSearch = new ProductSearch();
                productSearch.ID = product.ProductID;
                productSearch.Name = product.Brand.Name + " " + product.Name;
                string filename = product.Images[0].ImageUrl.Substring(0, product.Images[0].ImageUrl.LastIndexOf('.'));
                string extension = product.Images[0].ImageUrl.Substring(product.Images[0].ImageUrl.LastIndexOf('.'));
                productSearch.ImageUrl = CreateImageDirectory(int.Parse(product.Images[0].ImageUrl.Substring(0, product.Images[0].ImageUrl.LastIndexOf('.')))) + filename + "-" + ConfigurationManager.AppSettings["thumbName"] + extension;
                productSearch.Url = product.Url;
                productSearch.WebPrice = product.WebPrice;

                productsSearch.Add(productSearch);
            }

            return productsSearch;
        }

        public List<ProductSimple> GetProductsByNameAndCode(string name)
        {
            return new ProductDL().GetProductsByNameAndCode(name);
        }

        public void CopyData(int oldID, int newID)
        {
            new ProductDL().CopyData(oldID, newID);
        }

        public string GetFullImageUrl(string imageUrl, string suffix)
        {
            string filename = imageUrl.Substring(0, imageUrl.LastIndexOf('.'));
            string extension = imageUrl.Substring(imageUrl.LastIndexOf('.'));

            //return CreateImageDirectory(int.Parse(filename)) + filename + (suffix != string.Empty ? "-" : string.Empty) + suffix + extension;

            string fullFilename = CreateImageDirectory(int.Parse(filename)) + filename + (suffix != string.Empty ? "-" : string.Empty) + suffix + extension;

            if (imageExists(fullFilename))
            {
                return fullFilename;
            }

            return "/images/no-image.jpg";
        }

        public string GetNewProductCode(int categoryID)
        {
            return new ProductDL().GetNewProductCode(categoryID);
        }

        private bool imageExists(string filename)
        {
            return File.Exists(HttpContext.Current.Server.MapPath("~/" + filename));
        }

        public bool UpdateProductImageUrl(int productImageUrlId, string imageUrl)
        {
            return new ProductDL().UpdateProductImageUrl(productImageUrlId, imageUrl);
        }

        public void SetSortIndex(int productID, int sortIndex)
        {
            new ProductDL().SetSortIndex(productID, sortIndex);
        }
    }
}
