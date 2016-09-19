using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    [Serializable]
    public class ProductImage
    {
        private string _imageUrl;
        private int _sortOrder;

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public ProductImage()
        {

        }

        public ProductImage(string imageUrl, int sortOrder)
        {
            _imageUrl = imageUrl;
            _sortOrder = sortOrder;
        }
    }
}
