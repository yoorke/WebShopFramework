using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class BrandFilter : Brand
    {
        public int Count { get; set; }

        public BrandFilter(int brandID, string name, string logoUrl, string url, int count)
        {
            this.BrandID = brandID;
            this.Name = name;
            this.LogoUrl = logoUrl;
            this.Url = url;
            this.Count = count;
        }
    }
}
