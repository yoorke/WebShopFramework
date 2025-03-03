using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class BrandBL
    {
        public List<Brand> GetBrands(bool allSelection)
        {
            BrandDL brandDL = new BrandDL();
            List<Brand> brands = brandDL.GetBrands();

            if (allSelection && brands != null)
                brands.Insert(0, new Brand(-1, "Sve", string.Empty));

            return brands;
        }

        public List<BrandFilter> GetBrands(string categoryUrl, bool includeChildrenCategories = false)
        {
            Category category = null;
            CategoryDL categoryDL = new CategoryDL();
            if (!categoryUrl.Contains('/'))            
                category = categoryDL.GetCategoryByUrl(categoryUrl);
            else
            {
                string[] categoryUrlArray = categoryUrl.Split('/');
                category = categoryDL.GetCategoryByUrl(categoryUrlArray.Length > 2 ? categoryUrlArray[categoryUrlArray.Length - 3] : string.Empty, categoryUrlArray[categoryUrlArray.Length - 2], categoryUrlArray[categoryUrlArray.Length - 1], false);
            }

            BrandDL brandDL = new BrandDL();
            return brandDL.GetBrands(category.CategoryID, includeChildrenCategories);
        }

        public List<BrandFilter> GetBrands(int categoryID, bool includeChildrenCategories = false)
        {
            return new BrandDL().GetBrands(categoryID, includeChildrenCategories);
        }

        public Brand GetBrandByName(string name)
        {
            BrandDL brandDL = new BrandDL();
            return brandDL.GetBrandByName(name);
        }

        public Brand GetBrand(int id)
        {
            return new BrandDL().GetBrand(id);
        }

        public int SaveBrand(Brand brand)
        {
            BrandDL brandDL=new BrandDL();
            if (brand.BrandID > 0)
            {
                return brandDL.UpdateBrand(brand);
            }
            else
                return brandDL.SaveBrand(brand);
        }

        public int DeleteBrand(int brandID)
        {
            BrandDL brandDL = new BrandDL();
            return brandDL.DeleteBrand(brandID);
        }
    }
}
