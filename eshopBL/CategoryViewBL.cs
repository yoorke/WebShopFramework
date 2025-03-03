using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class CategoryViewBL
    {
        public List<CategoryView> GetNestedCategoriesList()
        {
            DataTable categoriesDT = new CategoryViewDL().GetCategories(false, "category.sortOrder");

            return getCategoriesList(categoriesDT, 1, string.Empty);
        }

        private List<CategoryView> getCategoriesList(DataTable categoriesDT, int parentID, string rootUrl, int levelLimit = 999)
        {
            if (categoriesDT == null || categoriesDT.Rows.Count == 0)
                return new List<CategoryView>();

            List<CategoryView> list = new List<CategoryView>();

            DataView dv = new DataView(categoriesDT);
            dv.RowFilter = "parentCategoryID=" + parentID.ToString();

            string parentUrl = string.Empty;
            //if(bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]))
            //{
                //DataView dvParent = new DataView(categoriesDT);
                //dvParent.RowFilter = "categoryID=" + parentID.ToString();
                //parentUrl = rootUrl + dvParent[0]["url"].ToString();
            //}

            foreach(DataRowView row in dv)
            {
                list.Add(new CategoryView()
                {
                    CategoryID = (int)row["categoryID"],
                    Name = row["name"].ToString(),
                    Url = row["fullUrl"].ToString(),
                    FullUrl = row["fullUrl"].ToString(),
                    ImageUrl = row["imageUrl"].ToString(),
                    SubCategories = getCategoriesList(categoriesDT, (int)row["categoryID"], parentUrl)
                });
            }

            return list;
        }

        public List<CategoryView> GetCategoriesForFirstPage()
        {
            return new CategoryViewDL().GetCategoriesForFirstPage();
        }
    }
}
