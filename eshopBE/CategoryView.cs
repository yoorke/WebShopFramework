using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class CategoryView
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string FullUrl { get; set; }
        public string ImageUrl { get; set; }
        public List<CategoryView> SubCategories { get; set; }
    }
}
