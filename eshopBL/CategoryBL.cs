using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopDL;
using eshopBE;
using System.Data;
using eshopUtilities;
using System.Configuration;

namespace eshopBL
{
    public class CategoryBL
    {
        /*public List<Category> GetNestedCategoriesList()
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories();

            List<Category> list = new List<Category>();
            list.Add(new Category((int)categoriesDT.Rows[0]["categoryID"], categoriesDT.Rows[0]["name"].ToString(), 0, categoriesDT.Rows[0]["url"].ToString(), categoriesDT.Rows[0]["imageUrl"].ToString()));
            Category temp = list[0];

            for (int i = 1; i < categoriesDT.Rows.Count; i++)
            {
                //while ((int)categoriesDT.Rows[i]["parentID"] == temp.CategoryID)
                if ((int)categoriesDT.Rows[i]["parentID"] == temp.CategoryID)
                {
                    if (temp.SubCategory == null)
                        temp.SubCategory = new List<Category>();

                    temp.SubCategory.Add(new Category((int)categoriesDT.Rows[i]["categoryID"], categoriesDT.Rows[i]["name"].ToString(), temp.CategoryID, categoriesDT.Rows[i]["url"].ToString(), categoriesDT.Rows[i]["imageUrl"].ToString()));

                }
                else
                {
                    if ((int)categoriesDT.Rows[i]["parentID"] > temp.CategoryID)
                    {

                        temp = temp.SubCategory[temp.SubCategory.Count - 1];
                        i--;
                    }
                    else
                    {
                        temp = list[0];
                        i--;
                    }
                }

            }

            return list[0].SubCategory;
        }*/

        
        /*public DataTable GetNestedCategoriesDataTable()
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories();
            int tempCategoryID = int.Parse(categoriesDT.Rows[0]["categoryID"].ToString());
            string baseString = "--";

            for (int i = 1; i < categoriesDT.Rows.Count; i++)
            {
                if (int.Parse(categoriesDT.Rows[i]["parentID"].ToString()) == tempCategoryID)
                    categoriesDT.Rows[i]["name"] = baseString + categoriesDT.Rows[i]["name"].ToString();
                else
                    if (int.Parse(categoriesDT.Rows[i]["parentID"].ToString()) > tempCategoryID)
                    {
                        tempCategoryID = int.Parse(categoriesDT.Rows[i-1]["categoryID"].ToString());
                        baseString += "--";
                        i--;
                    }
                    else
                    {
                        tempCategoryID = int.Parse(categoriesDT.Rows[0]["categoryID"].ToString());
                        baseString = "--";
                        i--;
                    }
            }

            return categoriesDT;
        }*/

        /*public List<Category> GetCategories2()
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories();
            int level = 1;

            List<Category> list = new List<Category>();
            list.Add(new Category((int)categoriesDT.Rows[0]["categoryID"], categoriesDT.Rows[0]["name"].ToString(), (int)categoriesDT.Rows[0]["parentID"], categoriesDT.Rows[0]["url"].ToString(), categoriesDT.Rows[0]["imageUrl"].ToString()));
            Category temp = list[0];

            for (int i = 1; i < categoriesDT.Rows.Count; i++)
            {
                for (int j = 0; j < categoriesDT.Rows.Count; j++)
                {
                    if ((int)categoriesDT.Rows[j]["parentID"] == temp.CategoryID)
                    {
                        if (temp.SubCategory == null)
                            temp.SubCategory = new List<Category>();
                        temp.SubCategory.Add(new Category((int)categoriesDT.Rows[j]["categoryID"], categoriesDT.Rows[j]["name"].ToString(), (int)categoriesDT.Rows[j]["parentID"], categoriesDT.Rows[j]["url"].ToString(), categoriesDT.Rows[j]["imageUrl"].ToString()));
                    }
                }
                if (level < 2)
                {
                    temp = temp.SubCategory[i - 1];
                    level++;
                }
                else
                {
                    if (i < 10)
                        temp = list[0].SubCategory[i - 1];
                    //level--;
                }
            }

            return list[0].SubCategory;
        }*/

        /*public List<Category> GetCategories3()
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories();

            List<Category> list = new List<Category>();

            for (int i = 0; i < categoriesDT.Rows.Count; i++)
            {
                if ((int)categoriesDT.Rows[i]["parentID"] == 2)
                    list.Add(new Category((int)categoriesDT.Rows[i]["categoryID"], categoriesDT.Rows[i]["name"].ToString(), (int)categoriesDT.Rows[i]["parentID"], categoriesDT.Rows[i]["url"].ToString(), categoriesDT.Rows[i]["imageUrl"].ToString()));
            }

            for (int i = 0; i < list.Count; i++)
                for (int j = 0; j < categoriesDT.Rows.Count; j++)
                    if (list[i].CategoryID == (int)categoriesDT.Rows[j]["parentID"])
                    {
                        if (list[i].SubCategory == null)
                            list[i].SubCategory = new List<Category>();
                        list[i].SubCategory.Add(new Category((int)categoriesDT.Rows[j]["categoryID"], categoriesDT.Rows[j]["name"].ToString(), (int)categoriesDT.Rows[j]["parentID"], categoriesDT.Rows[j]["url"].ToString(), categoriesDT.Rows[j]["imageUrl"].ToString()));
                    }

            return list;
        }*/

        public List<Category> GetNestedCategoriesList(bool showNotActive = false)
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories("sortOrder", showNotActive);

            return GetCategoriesList(categoriesDT, 1, string.Empty);
        }

        private List<Category> GetCategoriesList(DataTable categoriesDT, int parentID, string rootUrl)
        {
            if (categoriesDT == null || categoriesDT.Rows.Count == 0)
                return new List<Category>();
            List<Category> list = null;

            DataView dv = new DataView(categoriesDT);
            dv.RowFilter = "parentID=" + parentID.ToString();

            string parentUrl = string.Empty;
            if (bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]))
            { 
                DataView dvParent = new DataView(categoriesDT);
                dvParent.RowFilter = "categoryID=" + parentID.ToString();
                parentUrl = rootUrl + (dvParent.Count > 0 ? (!rootUrl.EndsWith("/") && !dvParent[0]["url"].ToString().StartsWith("/") ? "/" : string.Empty) + (dvParent.Count > 0 ? dvParent[0]["url"].ToString() : string.Empty) : string.Empty);
                parentUrl += parentUrl.EndsWith("/") ? string.Empty : "/";
            }

            Category category;

            if (dv.Count > 0)
                list = new List<Category>();
            foreach (DataRowView row in dv)
            {
                category = new Category();
                category.CategoryID = (int)row["categoryID"];
                category.Name = row["name"].ToString();
                category.Url = (bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"])) ? (parentUrl + row["url"].ToString()) : row["url"].ToString();
                category.ImageUrl = row["imageUrl"].ToString();
                category.SortOrder = (int)row["sortOrder"];
                category.CategoryBannerID = (int)row["categoryBannerID"];
                category.ParentCategoryID = (int)row["parentID"];
                if (parentID == 1)
                    category.CategoryExtraMenus = new CategoryExtraMenuBL().GetCategoryExtraMenusForCategory(category.CategoryID);
                else category.CategoryExtraMenus = new List<CategoryExtraMenuCategory>();

                category.SubCategory = GetCategoriesList(categoriesDT, (int)row["categoryID"], parentUrl);

                category.ImageUrlSource = (int)row["imageUrlSource"];
                category.ImageUrlPositionX = (int)row["imageUrlPositionX"];
                category.ImageUrlPositionY = (int)row["imageUrlPositionY"];
                category.Icon = (string)row["icon"];

                //foreach (Category childCategory in subCategory)
                    //childCategory.ParentCategoryID = category.CategoryID;

                list.Add(category);
            }
            return list;
        }

        /*public DataTable GetNestedCategoriesDataTable()
        {
            CategoryDL categoryDL = new CategoryDL();
            DataTable categoriesDT = categoryDL.GetCategories();

            return GetCategoriesDataTable(categoriesDT, 2);
        }*/

        public DataTable GetNestedCategoriesDataTable(bool showNotActive = false, bool addHomeCategory = false)
        {
            List<Category> list = GetNestedCategoriesList(showNotActive);
            DataTable categoriesDT = new DataTable();
            categoriesDT.Columns.Add("categoryID", typeof(int));
            categoriesDT.Columns.Add("name", typeof(string));
            categoriesDT.Columns.Add("parentID", typeof(int));
            categoriesDT.Columns.Add("sortOrder", typeof(int));
            DataRow newRow;

            if(addHomeCategory)
            {
                newRow = categoriesDT.NewRow();
                newRow["categoryID"] = 1;
                newRow["name"] = "Home";
                newRow["parentID"] = -1;
                newRow["sortOrder"] = 1;
                categoriesDT.Rows.Add(newRow);
            }

            if (list != null)
            {
                foreach (Category category in list)
                {
                    newRow = categoriesDT.NewRow();
                    newRow["categoryID"] = category.CategoryID;
                    newRow["name"] = "--" + category.Name;
                    newRow["sortOrder"] = category.SortOrder;
                    newRow["parentID"] = category.ParentCategoryID != null ? category.ParentCategoryID : -1;
                    categoriesDT.Rows.Add(newRow);
                    if (category.SubCategory != null)
                    {
                        foreach (Category childCategory in category.SubCategory)
                        {
                            newRow = categoriesDT.NewRow();
                            newRow["categoryID"] = childCategory.CategoryID;
                            newRow["name"] = "--------" + childCategory.Name;
                            newRow["sortOrder"] = childCategory.SortOrder;
                            newRow["parentID"] = childCategory.ParentCategoryID;
                            categoriesDT.Rows.Add(newRow);
                            if(childCategory.SubCategory != null)
                            {
                                foreach(Category childSubCategory in childCategory.SubCategory)
                                {
                                    newRow = categoriesDT.NewRow();
                                    newRow["categoryID"] = childSubCategory.CategoryID;
                                    newRow["name"] = "----------------" + childSubCategory.Name;
                                    newRow["sortOrder"] = childSubCategory.SortOrder;
                                    newRow["parentID"] = childSubCategory.ParentCategoryID;
                                    categoriesDT.Rows.Add(newRow);
                                }
                            }
                        }
                    }
                }
            }

            return categoriesDT;
        }

        public DataTable GetCategories(string sortBy = "categoryID, parentCategoryID", bool showNotActive = true)
        {
            CategoryDL categoryDL = new CategoryDL();
            return categoryDL.GetCategories(sortBy, showNotActive);
        }

        public int SaveCategory(Category category)
        {
            CategoryDL categoryDL = new CategoryDL();

            if (category.Name == string.Empty)
                throw new BLException("Unesite naziv kategorije");
            
            if (category.CategoryID > 0)
                return categoryDL.UpdateCategory(category);
            else
                return categoryDL.SaveCategory(category);
        }

        public Category GetCategory(int categoryID)
        {
            CategoryDL categoryDL = new CategoryDL();
            return categoryDL.GetCategory(categoryID);
        }

        public Category GetCategory(string name)
        {
            CategoryDL categoryDL = new CategoryDL();
            return categoryDL.GetCategory(name);
        }

        public Category GetCategoryByExternalID(int externalID)
        {
            return new CategoryDL().GetCategoryByExternalID(externalID);
        }

        public int DeleteCategory(int categoryID)
        {
            CategoryDL categoryDL = new CategoryDL();
            return categoryDL.DeleteCategory(categoryID);
        }

        public DataTable GetCategoriesForFirstPage()
        {
            CategoryDL categoryDL = new CategoryDL();
            return categoryDL.GetCategoriesForFirstPage();
        }

        public Category GetCategoryByUrl(string url, bool showActive = false)
        {
            if(!url.Contains('/') && !bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]))
                return new CategoryDL().GetCategoryByUrl(url);

            string[] urlArray = url.Split('/');
            //if (urlArray.Length < 2)
                //return null;
            return new CategoryDL().GetCategoryByUrl(urlArray.Length > 2 ? urlArray[urlArray.Length - 3] : string.Empty, urlArray.Length > 1 ? urlArray[urlArray.Length - 2] : string.Empty, urlArray[urlArray.Length - 1], showActive);
        }

        public List<Category> GetAllSubCategories(int categoryID, bool includeAllSubcategories)
        {
            return new CategoryDL().GetAllSubCategories(categoryID, includeAllSubcategories);
        }

        public int AddBrandToCategoryExtraMenu(int categoryExtraMenuID, int brandID, int categoryID)
        {
            //DeleteCategoryExtraMenuCategory(categoryExtraMenuID, categoryID);
            //for (int i = 0; i < brandIDs.Length; i++)
            //{
              return  new CategoryDL().AddBrandToCategoryExtraMenu(categoryExtraMenuID, brandID, categoryID);
            //}
            //return 1;
        }

        public int DeleteCategoryExtraMenuCategory(int categoryExtraMenuID, int categoryID, int brandID)
        {
            return new CategoryDL().DeleteCategoryExtraMenuCategory(categoryExtraMenuID, categoryID, brandID);
        }

        public List<Brand> GetBrandsForCategoryExtraMenu(int categoryExtraMenuID, int categoryID)
        {
            return new CategoryDL().GetBrandsForCategoryExtraMenu(categoryExtraMenuID, categoryID);
        }

        //public List<Category> GetChildrenCategories(int categoryID)
        //{
            //return new CategoryDL().GetSubcategories(categoryID);
        //}
 
        public List<Category> GetCategoriesForProductUpdate()
        {
            return new CategoryDL().GetCategoriesForProductUpdate();
        }

        public int GetMaxSortOrder(int parentCategoryID)
        {
            return new CategoryDL().GetMaxSortOrder(parentCategoryID);
        }

        public void ReorderCategory(int categoryID, int direction)
        {
            new CategoryDL().ReorderCategory(categoryID, direction);
        }

        public int SaveCategoryFromExternalApplication(int externalID, string name, int externalParentID)
        {
            int status = -1;

            Category category = GetCategoryByExternalID(externalID);
            if(category != null)
            {
                category.Name = name;
                category.ParentCategoryID = externalParentID == 0 ? 1 : GetCategoryByExternalID(externalParentID).CategoryID;
                status = SaveCategory(category) > 0 ? 2 : 0;
            }
            else
            {
                category = new Category();
                category.Name = name;
                category.Url = eshopUtilities.Common.CreateFriendlyUrl(name);
                category.CategoryBannerID = -1;
                category.Description = string.Empty;
                category.ExportProducts = false;
                category.firstPageOrderBy = string.Empty;
                category.firstPageSortOrder = 0;
                category.ImageUrl = string.Empty;
                category.NumberOfProducts = 0;
                category.PricePercent = 0;
                category.ShowOnFirstPage = false;
                category.SortOrder = 0;
                category.WebPricePercent = 0;
                category.ExternalID = externalID;
                category.ParentCategoryID = externalParentID == 0 ? 1 : GetCategoryByExternalID(externalParentID).CategoryID;
                category.ExternalParentID = externalParentID;
                category.Active = true;

                status = SaveCategory(category) > 0 ? 1 : 0;
            }

            

            return status;
        }

        public List<Category> GetCategoriesForFooter()
        {
            return new CategoryDL().GetCategoriesForFooter();
        }

        public List<Category> GetFirstLevelSubcategories(int categoryID)
        {
            return new CategoryDL().GetFirstLevelSubcategories(categoryID);
        }

        public DataTable Search(string searchText)
        {
            return new CategoryDL().Search(searchText);
        }

        public List<Category> GetNestedCategoriesListForFP()
        {
            return GetCategoriesList(new CategoryDL().GetCategoriesForFirstPage(), 1, string.Empty);
        }
    }
}
