using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CategoryBanner
    {
        private int _categoryBannerID;
        private string _name;
        private string _imageUrl;
        private string _url;

        public int CategoryBannerID
        {
            get { return _categoryBannerID; }
            set { _categoryBannerID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public CategoryBanner()
        {
        }

        public CategoryBanner(int categoryBannerID, string name, string imageUrl, string url)
        {
            _categoryBannerID = categoryBannerID;
            _name = name;
            _imageUrl = imageUrl;
            _url = url;
        }
    }
}
