using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    [Serializable]
    public class BannerItem
    {
        private int _bannerID;
        private string _url;
        private string _imageUrl;

        public BannerItem()
        {
        }

        public BannerItem(int bannerID, string imageUrl, string url)
        {
            _bannerID = bannerID;
            _url = url;
            _imageUrl = imageUrl;
        }

        public int BannerID
        {
            get { return _bannerID; }
            set { _bannerID = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }
    }
}
