using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class CategoryBrand
    {
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public double PricePercent { get; set; }
        public double WebPricePercent { get; set; }

        public CategoryBrand()
        {

        }

        public CategoryBrand(int categoryID, int brandID, double pricePercent, double webPricePercent)
        {
            this.CategoryID = categoryID;
            this.BrandID = brandID;
            this.PricePercent = pricePercent;
            this.WebPricePercent = webPricePercent;
        }
    }
}
