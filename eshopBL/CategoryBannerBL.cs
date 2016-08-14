using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class CategoryBannerBL
    {
        public int SaveCategoryBanner(CategoryBanner categoryBanner)
        {
            if (categoryBanner.CategoryBannerID <= 0)
                return new CategoryBannerDL().SaveCategoryBanner(categoryBanner);
            else
                return new CategoryBannerDL().UpdateCategoryBanner(categoryBanner);
        }

        public CategoryBanner GetCategoryBanner(int categoryBannerID)
        {
            return new CategoryBannerDL().GetCategoryBanner(categoryBannerID);
        }

        public List<CategoryBanner> GetCategoryBanners(bool addSelect)
        {
            List<CategoryBanner> categoryBanners = new CategoryBannerDL().GetCategoryBanners();
            if(addSelect)
                categoryBanners.Insert(0, new CategoryBanner(-1, "Odaberi", string.Empty, string.Empty));
            return categoryBanners;
        }
    }
}
