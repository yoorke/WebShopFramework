using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class CustomPageBL
    {
        public int Save(CustomPage customPage)
        {
            CustomPageDL customPageDL=new CustomPageDL();
            if (customPage.CustomPageID > 0)
                return customPageDL.Update(customPage);
            else
                return customPageDL.Save(customPage);
        }

        public CustomPage GetCustomPage(int customPageID)
        {
            CustomPageDL customPageDL = new CustomPageDL();
            return customPageDL.GetCustomPage(customPageID);
        }

        public CustomPage GetCustomPage(string url)
        {
            CustomPageDL customPageDL = new CustomPageDL();
            return customPageDL.GetCustomPage(url);
        }

        public List<CustomPage> GetCustomPagesForCustomPageCategory(int customPageCategoryID)
        {
            CustomPageDL customPageDL = new CustomPageDL();
            return customPageDL.GetCustomPagesForCustomPageCategory(customPageCategoryID);
        }

        public List<CustomPageCategory> GetCustomPageCategories()
        {
            CustomPageDL customPageDL = new CustomPageDL();
            return customPageDL.GetCustomPagesCategories();
        }

        public int Delete(int customPageID)
        {
            return new CustomPageDL().Delete(customPageID);
        }

        public List<CustomPage> GetCustomPages()
        {
            return new CustomPageDL().GetCustomPages();
        }
    }
}
