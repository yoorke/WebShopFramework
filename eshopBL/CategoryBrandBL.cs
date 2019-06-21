using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eshopBE;
using eshopDL;
using System.Data;
using eshopUtilities;

namespace eshopBL
{
    public class CategoryBrandBL
    {
        public void Save(CategoryBrand categoryBrand)
        {
            if (categoryBrand.PricePercent < 0 || categoryBrand.PricePercent > 100)
                throw new BLException("Cena [%] ne može biti manja od 0 niti veća od 100.");
            if (categoryBrand.WebPricePercent < 0 || categoryBrand.WebPricePercent > 100)
                throw new BLException("Web cena [%] ne može biti manja od 0 niti veća od 100.");
            new CategoryBrandDL().Save(categoryBrand);
        }

        public void Delete(int categoryID, int brandID)
        {
            new CategoryBrandDL().Delete(categoryID, brandID);
        }

        public List<CategoryBrand> Get(int categoryID)
        {
            return new CategoryBrandDL().Get(categoryID);
        }

        public DataTable GetDataTable(int categoryID)
        {
            return new CategoryBrandDL().GetDataTable(categoryID);
        }

        public List<double> GetPricePercent(int categoryID, int brandID)
        {
            return new CategoryBrandDL().GetPricePercent(categoryID, brandID);
        }
    }
}
