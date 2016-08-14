using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Coupon
    {
        private int _couponID;
        private string _name;
        private double _discount;
        private string _code;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private CouponType _couponType;
        private List<CouponObject> _objects;

        public Coupon()
        {
        }

        public Coupon(int couponID, string name, double discount, string code, DateTime dateFrom, DateTime dateTo, CouponType couponType, List<CouponObject> objects)
        {
            _couponID = couponID;
            _name = name;
            _discount = discount;
            _code = code;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _couponType = couponType;
        }

        
public int CouponID
        {
            get { return _couponID; }
            set { _couponID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set { _dateFrom = value; }
        }

        public DateTime DateTo
        {
            get { return _dateTo; }
            set { _dateTo = value; }
        }

        public CouponType CouponType
        {
            get { return _couponType; }
            set { _couponType = value; }
        }

        public List<CouponObject> Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }
    }
}
