using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CategoryExtraMenu
    {
        public int CategoryExtraMenuID { get; set; }
        public string Name { get; set; }

        public CategoryExtraMenu()
        {
        }

        public CategoryExtraMenu(int categoryExtraMenuID, string name)
        {
            this.CategoryExtraMenuID = categoryExtraMenuID;
            this.Name = name;
        }
    }
}
