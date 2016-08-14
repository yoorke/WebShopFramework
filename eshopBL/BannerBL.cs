using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class BannerBL
    {
        public int SaveBanner(Banner banner)
        {
            //BannerDL bannerDL = new BannerDL();
            //return bannerDL.SaveBanner(banner);
            foreach(BannerItem item in banner.Banners)
            {
                if (item.BannerID > 0)
                    new BannerDL().UpdateBannerItem(item, banner.BannerPositionID);
                else
                    new BannerDL().SaveBannerItem(item, banner.BannerPositionID);
            }
            return 1;
        }

        public List<BannerItem> GetBannersForPosition(int bannerPositionID)
        {
            BannerDL bannerDL = new BannerDL();
            return bannerDL.GetBannersForPosition(bannerPositionID);
        }

        public BannerItem GetBanner(string bannerPosition)
        {
            BannerDL bannerDL = new BannerDL();
            return bannerDL.GetBanner(bannerPosition);
        }

        public int DeleteBanner(int bannerID)
        {
            BannerDL bannerDL = new BannerDL();
            return bannerDL.DeleteBanner(bannerID);
        }

        public List<BannerPosition> GetBannerPositions()
        {
            BannerDL bannerDL = new BannerDL();
            return bannerDL.GetBannerPositions();
        }
    }
}
