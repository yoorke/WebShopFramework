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
        private double _userDiscountValue;
        private int _deliveryServiceID;
        private string _trackCode;
        private double _deliveryCost;
        private double _totalWeight;
        private List<Package> _packages;
        private bool _isPaid;
        private string _paymentStatus;
        private bool _confirmed;
        private bool _createPackageForEverySupplier = true;

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

        public double UserDiscountValue
        {
            get { return _userDiscountValue; }
            set { _userDiscountValue = value; }
        }

        public int DeliveryServiceID
        {
            get { return _deliveryServiceID; }
            set { _deliveryServiceID = value; }
        }

        public string TrackCode
        {
            get { return _trackCode; }
            set { _trackCode = value; }
        }

        public double TotalWeight
        {
            get { return calculateWeight(); }
        }

        private double calculateWeight()
        {
            double weight = 0;

            foreach(var item in Items)
            {
                if(item.Product.Weight == 0)
                {
                    return -1;
                }

                weight += item.Product.Weight;
            }

            return weight;
        }

        public double TotalValue
        {
            get { return getTotalValue(); }
        }

        private double getTotalValue()
        {
            double total = _items.Sum(item => item.UserPrice * item.Quantity);
            return total - _userDiscountValue;
        }

        public int NumberOfSuppliers
        {
            get { return getNumberOfSuppliers(); }
        }

        private int getNumberOfSuppliers()
        {
            //int numberOfSuppliers = 0;
            //int currentSupplier = -1;
            List<int> suppliers = new List<int>();

            foreach(var item in _items)
            {
                if(!suppliers.Contains(item.Product.SupplierID))
                {
                    suppliers.Add(item.Product.SupplierID);
                    //numberOfSuppliers++;
                }
            }

            return suppliers.Count();
        }

        public List<Package> Packages
        {
            get { return getPackages(); }
        }

        public List<Package> getPackages()
        {
            if(_packages != null)
            {
                return _packages;
            }

            List<Package> packages = new List<Package>();

            foreach(var item in _items)
            {
                if(_createPackageForEverySupplier)
                {
                    if (!packageListContainsSupplierID(packages, item.Product.SupplierID))
                    {
                        addPackage(packages, item.Product.SupplierID);
                    }

                    ((Package)(packages.First(package => package.Supplier.SupplierID == item.Product.SupplierID))).Items.Add(item);
                }
                else
                {
                    if(packages.Count == 0)
                    {
                        addPackage(packages, 0);
                    }
                    ((Package)(packages.First())).Items.Add(item);
                }
            }

            _packages = packages;

            return packages;
        }

        public bool IsPaid
        {
            get { return _isPaid; }
            set { _isPaid = value; }
        }

        public string PaymentStatus
        {
            get { return _paymentStatus; }
            set { _paymentStatus = value; }
        }

        public bool Confirmed
        {
            get { return _confirmed; }
            set { _confirmed = value; }
        }

        public bool CreatePackageForEverySupplier
        {
            get { return _createPackageForEverySupplier; }
            set { _createPackageForEverySupplier = value; }
        }

        private bool packageListContainsSupplierID(List<Package> packages, int supplierID)
        {
            foreach(var package in packages)
            {
                if(package.Supplier.SupplierID == supplierID)
                {
                    return true;
                }
            }

            return false;
        }

        private void addPackage(List<Package> packages, int supplierID)
        {
            packages.Add(new Package()
            {
                DeliveryCost = 0,
                Items = new List<OrderItem>(),
                Supplier = new Supplier()
                {
                    SupplierID = supplierID,
                    Name = "Paket " + (packages.Count + 1).ToString()
                }
            });
        }
    }
}
