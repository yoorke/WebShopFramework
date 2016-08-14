using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class CouponBL
    {
        public Coupon GetCoupon(string code)
        {
            CouponDL couponDL = new CouponDL();
            return couponDL.GetCoupon(code);
        }

        public int Save(Coupon coupon)
        {
            if (coupon.CouponID <= 0)
                return new CouponDL().Save(coupon);
            else
                return new CouponDL().Update(coupon);
        }

        public List<Coupon> GetCoupons()
        {
            return new CouponDL().GetCoupons();
        }

        public Coupon GetCoupon(int couponID)
        {
            return new CouponDL().GetCoupon(couponID);
        }

        public List<CouponType> GetCouponTypes(bool addSelect)
        {
            List<CouponType> couponTypes = new CouponDL().GetCouponTypes();
            if (addSelect)
                couponTypes.Insert(0, new CouponType(-1, "Odaberi"));
            return couponTypes;
        }

        public int DeleteCoupon(int couponID)
        {
            return new CouponDL().DeleteCoupon(couponID);
        }

    }
}
