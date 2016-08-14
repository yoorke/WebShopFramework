using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Banner
    {
        private int _bannerPositionID;
       
        private List<BannerItem> _banners;

        public int BannerPositionID
        {
            get { return _bannerPositionID; }
            set { _bannerPositionID = value; }
        }

        /*public string Position
        {
            get { return _position; }
            set { _position = value; }
        }*/

        public List<BannerItem> Banners
        {
            get { return _banners; }
            set { _banners = value; }
        }
    }
}
