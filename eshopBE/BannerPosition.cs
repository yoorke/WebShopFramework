using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class BannerPosition
    {
        private int _bannerPositionID;
        private string _name;

        public BannerPosition()
        {
        }

        public BannerPosition(int bannerPositionID, string name)
        {
            _bannerPositionID = bannerPositionID;
            _name = name;
        }

        public int BannerPositionID
        {
            get { return _bannerPositionID; }
            set { _bannerPositionID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
