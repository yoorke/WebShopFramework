using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Order
    {
        private int _orderID;
        private DateTime _date;
        private User _user;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _address;
        private string _city;
        private string _phone;
        List<OrderItem> _items;
        private Coupon _coupon;
        private Payment _payment;
        private Delivery _delivery;        
        private string _name;
        private string _pib;
        private OrderStatus _orderStatus;
        /*private UserType _userType;*/
        private string _zip;
        private string _comment;
        /*private Payment _payment;*/
        private string _code;
        private string _cartID;

        public int OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public string Firstname
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string Lastname
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public List<OrderItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public Coupon Coupon
        {
            get { return _coupon; }
            set { _coupon = value; }
        }

        public Payment Payment
        {
            get { return _payment; }
            set { _payment = value; }
        }

        public Delivery Delivery
        {
            get { return _delivery; }
            set { _delivery = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Pib
        {
            get { return _pib; }
            set { _pib = value; }
        }

        public OrderStatus OrderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value; }
        }

        /*public UserType UserType
        {
            get { return _userType; }
            set { _userType = value; }
        }*/

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string CartID
        {
            get { return _cartID; }
            set { _cartID = value; }
        }
    }
}
