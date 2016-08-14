using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    [Serializable]
    public class CouponObject
    {
        public int CouponID { get; set; }
        public int ObjectID { get; set; }
        public int ObjectTypeID { get; set; }
        public string Name { get; set; }

        public CouponObject()
        {

        }

        public CouponObject(int couponID, int objectID, int objectTypeID, string name)
        {
            this.CouponID = couponID;
            this.ObjectID = objectID;
            this.ObjectTypeID = objectTypeID;
            this.Name = name;
        }
    }
}
