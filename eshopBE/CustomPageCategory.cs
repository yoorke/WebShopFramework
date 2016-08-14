using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CustomPageCategory
    {
        private int _customPageCategoryID;
        private string _name;

        public CustomPageCategory()
        {
        }

        public CustomPageCategory(int customPageCategoryID, string name)
        {
            _customPageCategoryID = customPageCategoryID;
            _name = name;
        }

        public int CustomPageCategoryID
        {
            get { return _customPageCategoryID; }
            set { _customPageCategoryID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
