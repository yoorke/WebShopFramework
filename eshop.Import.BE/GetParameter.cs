using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BE
{
    public class GetParameter
    {
        public bool Description { get; set; }
        public bool Attributes { get; set; }
        public bool Images { get; set; }
        public bool InStock { get; set; }
        public string Category { get; set; }
        public List<string> SubCategories { get; set; }
        public string Manufacturer { get; set; }
        public string Currency { get; set; }
        public string Keyword { get; set; }
        public bool Short { get; set; }
    }
}
