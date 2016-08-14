using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CategoryExtraMenuCategory
    {
        public string Name { get; set; }
        public List<Brand> Brands { get; set; }

        public CategoryExtraMenuCategory()
        {
        }

        public CategoryExtraMenuCategory(string name, List<Brand> brands)
        {
            this.Name = name;
            this.Brands = brands;
        }
    }
}
