using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CouponType
    {
        public int CouponTypeID { get; set; }
        public string Name { get; set; }

        public CouponType()
        {

        }

        public CouponType(int couponTypeID, string name)
        {
            this.CouponTypeID = couponTypeID;
            this.Name = name;
        }
    }
}
