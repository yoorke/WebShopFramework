using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class CategoryExtraMenuBL
    {
        public List<CategoryExtraMenu> GetCategoryExtraMenus(bool addSelect)
        {
            List<CategoryExtraMenu> categoryExtraMenus = new CategoryExtraMenuDL().GetCategoryExtraMenus();
            if (addSelect)
                categoryExtraMenus.Insert(0, new CategoryExtraMenu(-1, "Odaberi"));
            return categoryExtraMenus;
        }

        public List<CategoryExtraMenuCategory> GetCategoryExtraMenusForCategory(int categoryID)
        {
            return new CategoryExtraMenuDL().GetCategoryExtraMenusForCategory(categoryID);
        }
    }
}
