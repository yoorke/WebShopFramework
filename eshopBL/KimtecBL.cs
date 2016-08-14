using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.Web;
using System.IO;
using eshopDL;
using eshopBE;
using System.Net;
using System.Drawing;
using System.Web.UI.WebControls;
using eshopUtilities;

namespace eshopBL
{
    public class KimtecBL
    {
        

        

        public void SaveToFile(DataSet ds)
        {
            //DataSet products = GetProductsList(string.Empty, string.Empty);
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("~") + "/" + ds.Tables[0].TableName + ".txt");

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                sw.Write(ds.Tables[0].Columns[i].ColumnName + " | ");
            sw.Write(System.Environment.NewLine);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                for (int i = 0; i < row.ItemArray.Count(); i++)
                    sw.Write(row[i].ToString() + " | ");
                sw.Write(System.Environment.NewLine);
            }

            sw.Close();
        }

        

        public int DeleteKimtecCategory(int? kimtecCategoryID)
        {
            return new KimtecDL().DeleteKimtecCategory(kimtecCategoryID);
        }

        public int SaveCategory(int categoryID, string name, int parentID)
        {
            return new KimtecDL().SaveCategory(categoryID, name, parentID);
        }

        public DataTable GetCategories(int? parentID, int? categoryID, bool addSelect)
        {
            DataTable categories = new KimtecDL().GetCategories(parentID, categoryID);

            if(addSelect)
            {
                DataRow newRow = categories.NewRow();
                newRow[0] = 0;
                newRow[1] = "Odaberi kategoriju";
                newRow[2] = 0;
                newRow[3] = 0;
                newRow[4] = 0;
                categories.Rows.InsertAt(newRow, 0);
            }
            return categories;
        }

        public DataSet GetProductsList(string productCode, string productType)
        {
            return new KimtecDL().GetProductsList(productCode, productType);
        }

        public DataSet GetCategoriesFromKimtec()
        {
            return new KimtecDL().GetCategoriesFromKimtec();
        }

        public string SaveProductsFromKimtec()
        {
            StringBuilder status = new StringBuilder();
            KimtecDL kimtecDL = new KimtecDL();
            int productsCount = kimtecDL.SaveProducts();
            int priceCount = kimtecDL.SavePrice();
            int barcodeCount = kimtecDL.SaveBarcode();
            int categoriesCount = kimtecDL.SaveCategoryForProduct();

            status.Append("Preuzeto " + productsCount.ToString() + " proizvoda.");
            return status.ToString();
        }

        public int SavePrice()
        {
            return new KimtecDL().SavePrice();
        }

        public int SaveBarcode()
        {
            return new KimtecDL().SaveBarcode();
        }

        public DataTable GetProductsFromDatabase(string[] subcategories, string[] subcategoriesID)
        {
            return new KimtecDL().GetProductsFromDatabase(subcategories, subcategoriesID);
        }

        public DataTable GetKimtecSubCategoriesByCategoryID(int categoryID)
        {
            return new KimtecDL().GetKimtecSubCategoriesByCategoryID(categoryID);
        }

        public int[] SaveProduct(System.Web.UI.WebControls.GridViewRow row, int categoryID, bool isApproved, bool isActive, int kimtecCategoryID)
        {
            Category category = new CategoryBL().GetCategory(categoryID);
            ProductBL productBL = new ProductBL();
            Product product = new Product();
            product.ProductID = productBL.GetProductIDBySupplierCode(((Label)row.FindControl("lblCode")).Text);
            bool isLocked = productBL.IsLocked(product.ProductID);
            int newProducts = 0;
            int updatedProducts = 0;
            bool isNew = product.ProductID <= 0;

            if (!isLocked)
            {
                product.Code = ((Label)row.FindControl("lblCode")).Text;
                product.SupplierCode = ((Label)row.FindControl("lblCode")).Text;
                product.Brand = GetBrand(((Label)row.FindControl("lblBrand")).Text);
                product.Name = ((Label)row.FindControl("lblName")).Text;
                product.Description = ((Label)row.FindControl("lblDescription")).Text;
                product.Price = calculatePrice(double.Parse(((Label)row.FindControl("lblPartnerPrice")).Text != string.Empty ? ((Label)row.FindControl("lblPartnerPrice")).Text : "0,00"), category.PricePercent);
                product.WebPrice = calculatePrice(double.Parse(((Label)row.FindControl("lblPartnerPrice")).Text != string.Empty ? ((Label)row.FindControl("lblPartnerPrice")).Text : "0,00"), category.WebPricePercent);
                product.Images = new List<string>();
                product.Images.Add(saveProductImage(((Label)row.FindControl("lblImageUrl")).Text));
                product.Attributes = GetProductAttributes(product.Code, kimtecCategoryID);
                product.Categories = new List<Category>();
                product.Categories.Add(category);
                product.SupplierID = 1004;
                product.IsApproved = isApproved;
                product.IsActive = isActive;
                product.VatID = 4;
                product.Specification = string.Empty;
                product.IsInStock = int.Parse(((Label)row.FindControl("lblStock")).Text != string.Empty ? ((Label)row.FindControl("lblStock")).Text : "0") > 0 ? true : false;
                product.Ean = ((Label)row.FindControl("lblBarcode")).Text;

                if (productBL.SaveProduct(product) > 0)
                {
                    if (isNew)
                        newProducts++;
                    else
                        updatedProducts++;
                }
            }
            return new int[] { newProducts, updatedProducts };
        }

        public bool SaveProduct(string code, bool isActive, bool isApproved, int categoryID, int kimtecCategoryID)
        {
            //int newProducts = 0;
            //int updatedProducts = 0;

            DataTable kimtecProduct = new KimtecDL().GetProductBySupplierCode(code);
            Category category = new CategoryDL().GetCategory(categoryID);

            Product product = new Product();
            product.IsApproved = isApproved;
            product.IsActive = isActive;
            product.SupplierID = 1004;
            product.SupplierCode = code;
            product.VatID = 4;
            product.Categories = new List<Category>();
            product.Categories.Add(category);
            product.Specification = string.Empty;
            product.IsInStock = true;
            bool isNew = false;
            bool isLocked = false;
            product.Code = code;

            product.ProductID = new ProductDL().GetProductIDBySupplierCode(code);
            if (product.ProductID <= 0)
                isNew = true;
            isLocked = new ProductDL().IsLocked(product.ProductID);

            Brand brand = new BrandDL().GetBrandByName(kimtecProduct.Rows[0]["brand"].ToString());
            if(brand == null)
            {
                brand = new Brand();
                brand.Name = kimtecProduct.Rows[0]["brand"].ToString();
                brand.BrandID = new BrandDL().SaveBrand(brand);
            }
            if (product.Brand == null)
                product.Brand = new Brand();
            product.Brand = brand;

            product.Name = kimtecProduct.Rows[0]["name"].ToString();
            product.Price = calculatePrice(double.Parse(kimtecProduct.Rows[0]["partnerPrice"].ToString()), category.PricePercent);
            product.WebPrice = calculatePrice(double.Parse(kimtecProduct.Rows[0]["partnerPrice"].ToString()), category.WebPricePercent);
            product.Ean = kimtecProduct.Rows[0]["barcodeValue"].ToString();
            product.SupplierPrice = double.Parse(kimtecProduct.Rows[0]["partnerPrice"].ToString());
            product.Images = new List<string>();
            product.Images.Add(saveProductImage(kimtecProduct.Rows[0]["imageUrl"].ToString()));
            product.Attributes = GetProductAttributes(code, kimtecCategoryID);
            product.Description = kimtecProduct.Rows[0]["marketingDescription"].ToString();

            if (!isLocked)
                if (new ProductBL().SaveProduct(product) > 0)
                    return true;

            return false;

            //return new int[] { newProducts, updatedProducts };
        }

        private Brand GetBrand(string brandName)
        {
            BrandBL brandBL = new BrandBL();
            Brand brand = brandBL.GetBrandByName(brandName);
            if (brand == null)
            {
                brand = new Brand();
                brand.Name = brandName;
                brand.BrandID = brandBL.SaveBrand(brand);
                //product.Brand.BrandID = brand.BrandID;
            }
            return brand;
        }

        private double calculatePrice(double supplierPrice, double percent)
        {
            
            return double.Parse(((int)(supplierPrice * (percent / 100 + 1) * 1.2) / 100 * 100 - 10).ToString());
        }

        private string saveProductImage(string url)
        {
            if (url == string.Empty)
                return "no-image.jpg";
            CertificateWebClient webClient = new CertificateWebClient();
            string filename = Path.GetFileName(url);
            string path = HttpContext.Current.Server.MapPath("~/images/");

            //if (!File.Exists(path + filename))
            //{
                //webClient.UseDefaultCredentials = false;
                //webClient.Credentials = new NetworkCredential("pinservis", "*54GrrL13Dp!");
                //webClient.Headers.Add("User-Agent: Other");
                
                webClient.DownloadFile(url, path + filename);

                System.Drawing.Image original = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/images/" + filename));

                //System.Drawing.Image thumb = original.GetThumbnailImage(290, 232, null, IntPtr.Zero);
                System.Drawing.Image thumb = Common.CreateThumb(original, 290, 232);
                thumb.Save(path + filename.Substring(0, filename.IndexOf(".jpg")) + "-main.jpg");

                //thumb = original.GetThumbnailImage(110, 75, null, IntPtr.Zero);
                thumb = Common.CreateThumb(original, 160, 110);
                thumb.Save(path + filename.Substring(0, filename.IndexOf(".jpg")) + "-list.jpg");

                //thumb = original.GetThumbnailImage(30, 24, null, IntPtr.Zero);
                thumb = Common.CreateThumb(original, 50, 40);
                thumb.Save(path + filename.Substring(0, filename.IndexOf(".jpg")) + "-thumb.jpg");
            //}
            return filename;
        }

        public int SaveProductSpecification()
        {
            return new KimtecDL().SaveProductSpecification();
        }

        private List<Category> GetCategoriesList(DataTable categoriesDT, int parentID)
        {
            List<Category> list = null;

            DataView dv = new DataView(categoriesDT);
            dv.RowFilter = "parentID=" + parentID.ToString();
            Category category;

            if (dv.Count > 0)
                list = new List<Category>();

            foreach (DataRowView row in dv)
            {
                category = new Category();
                category.CategoryID = (int)row["kimtecCategoryID"];
                category.Name = row["name"].ToString();
                category.ParentCategoryID = (int)row["parentID"];
                //showOnFirstPage isporiscen za isRoot
                category.ShowOnFirstPage = int.Parse(row["isRoot"].ToString()) == 1 ? true : false;

                category.SubCategory = GetCategoriesList(categoriesDT, (int)row["kimtecCategoryID"]);

                list.Add(category);
            }
            return list;
        }

        public List<Category> GetNestedCategoriesList()
        {
            return GetCategoriesList(GetAllCategories(), 0);
            
        }

        public DataTable GetAllCategories()
        {
            return new KimtecDL().GetAllCategories();
        }

        public DataTable GetNestedCategoriesDataTable()
        {
            List<Category> list = GetNestedCategoriesList();
            DataTable categoriesDT = new DataTable();
            categoriesDT.Columns.Add("kimtecCategoryID", typeof(int));
            categoriesDT.Columns.Add("name", typeof(string));
            categoriesDT.Columns.Add("parentID", typeof(int));
            categoriesDT.Columns.Add("isRoot", typeof(bool));
            DataRow newRow;

            if (list != null)
            {
                foreach (Category category in list)
                {
                    newRow = categoriesDT.NewRow();
                    newRow["kimtecCategoryID"] = category.CategoryID;
                    newRow["name"] = "--" + category.Name;
                    newRow["parentID"] = category.ParentCategoryID;
                    //showOnFirstPage iskoriscen za isRoot
                    newRow["isRoot"] = category.ShowOnFirstPage;
                    categoriesDT.Rows.Add(newRow);
                    if (category.SubCategory != null)
                    {
                        foreach (Category childCategory in category.SubCategory)
                        {
                            newRow = categoriesDT.NewRow();
                            newRow["kimtecCategoryID"] = childCategory.CategoryID;
                            newRow["name"] = "----" + childCategory.Name;
                            newRow["parentID"] = childCategory.ParentCategoryID;
                            newRow["isRoot"] = childCategory.ShowOnFirstPage;
                            categoriesDT.Rows.Add(newRow);
                            if (childCategory.SubCategory != null)
                            {
                                foreach (Category childCategory1 in childCategory.SubCategory)
                                {
                                    newRow = categoriesDT.NewRow();
                                    newRow["kimtecCategoryID"] = childCategory1.CategoryID;
                                    newRow["name"] = "--------" + childCategory1.Name;
                                    newRow["parentID"] = childCategory1.ParentCategoryID;
                                    newRow["isRoot"] = childCategory1.ShowOnFirstPage;
                                    categoriesDT.Rows.Add(newRow);
                                    if (childCategory1.SubCategory != null)
                                    {
                                        foreach (Category childCategory2 in childCategory1.SubCategory)
                                        {
                                            newRow = categoriesDT.NewRow();
                                            newRow["kimtecCategoryID"] = childCategory2.CategoryID;
                                            newRow["name"] = "----------" + childCategory2.Name;
                                            newRow["parentID"] = childCategory2.ParentCategoryID;
                                            newRow["isRoot"] = childCategory2.ShowOnFirstPage;
                                            categoriesDT.Rows.Add(newRow);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return categoriesDT;
        }

        public int SetIsRoot(int kimtecCategoryID, bool isRoot)
        {
            return new KimtecDL().SetIsRoot(kimtecCategoryID, isRoot);
        }

        public int GetKimtecCategoryForCategory(int categoryID)
        {
            return new KimtecDL().GetKimtecCategoryForCategory(categoryID);
        }

        public int SaveKimtecCategoryForCategory(int categoryID, int kimtecCategoryID, bool isCategory)
        {
            return new KimtecDL().SaveKimtecCategoryForCategory(categoryID, kimtecCategoryID, isCategory);
        }

        public int DeleteCategoryKimtecCategory(int categoryID)
        {
            return new KimtecDL().DeleteCategoryKimtecCategory(categoryID);
        }

        public int DeleteKimtecSpecification()
        {
            return new KimtecDL().DeleteKimtecSpecification();
        }

        public DataTable GetKimtecAttributesForCategory(int kimtecCategoryID, bool addSelect)
        {
            DataTable kimtecAttributes = new KimtecDL().GetKimtecAttributesForCategory(kimtecCategoryID);
            if(addSelect)
            {
                DataRow newRow = kimtecAttributes.NewRow();
                newRow["kimtecAttributeID"] = 0;
                newRow["name"] = "Odaberi";
                kimtecAttributes.Rows.InsertAt(newRow, 0);
            }
            return kimtecAttributes;
        }

        public DataTable GetKimtecCategoriesWithAttribute()
        {
            return new KimtecDL().GetCategoriesWithAttribute();
        }

        public int SaveKimtecAttribute()
        {
            return new KimtecDL().SaveKimtecAttribute();
        }

        public int SaveKimtecAttributeForAttribute(int kimtecAttributeID, int attributeID)
        {
            return new KimtecDL().SaveKimtecAttributeForAttribute(kimtecAttributeID, attributeID);
        }

        public int GetKimtecCategoryCount()
        {
            return new KimtecDL().GetKimtecCategoryCount();
        }

        public int DeleteKimtecAttributeForAttribute(int categoryID, int kimtecCategoryID)
        {
            return new KimtecDL().DeleteKimtecAttributeForAttribute(categoryID, kimtecCategoryID);
        }

        private List<eshopBE.AttributeValue> GetProductAttributes(string productCode, int kimtecCategoryID)
        {
            DataTable specification = new KimtecDL().GetSpecificationForProductCode(productCode);
            DataTable attributesForKimtecCategory = new AttributeDL().GetAttributesForKimtecCategory(kimtecCategoryID);
            List<eshopBE.AttributeValue> attributes = new List<AttributeValue>();

            if (specification != null && attributesForKimtecCategory != null)
            {
                for (int i = 0; i < specification.Rows.Count; i++)
                {
                    for (int j = 0; j < attributesForKimtecCategory.Rows.Count; j++)
                    {
                        if (specification.Rows[i][0].ToString().Equals(attributesForKimtecCategory.Rows[j][2].ToString()))
                        {
                            int attributeID = int.Parse(attributesForKimtecCategory.Rows[j][0].ToString());
                            string value = specification.Rows[i][1].ToString();
                            value = value.Substring(value.IndexOf("<Value>") + 7, value.IndexOf("</Value>") - value.IndexOf("<Value>") - 7);

                            int attributeValueID = getAttributeKimtecValue(attributeID, value);

                            attributes.Add(new AttributeValue(attributeValueID, value, attributeID, 0, string.Empty, 0));
                            break;
                        }
                    }
                }
            }

            return attributes;
        }

        private int getAttributeKimtecValue(int attributeID, string value)
        {
            DataTable attributeValuesForKimtec = new AttributeDL().GetAttributeValuesForKimtec(attributeID);
            int attributeValueID = 0;
            bool exists = false;

            if (attributeValuesForKimtec != null)
            {
                for (int i = 0; i < attributeValuesForKimtec.Rows.Count; i++)
                {
                    if (attributeValuesForKimtec.Rows[i]["kimtecValue"].ToString().Equals(value) || attributeValuesForKimtec.Rows[i]["value"].ToString().Equals(value))
                    {
                        attributeValueID = int.Parse(attributeValuesForKimtec.Rows[i]["attributeValueID"].ToString());
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    attributeValueID = new AttributeBL().SaveAttributeValue(new AttributeValue(-1, value, attributeID, 0, string.Empty, 0), true);
                }
            }

            return attributeValueID;
        }
    }
}
