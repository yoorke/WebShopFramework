using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Retail
    {
        private int _retailID;
        private string _name;
        private string _address;
        private City _city;
        private string _phone;
        private string _mobilePhone;
        private string _location;

        public int RetailID
        {
            get { return _retailID; }
            set { _retailID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public City City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { _mobilePhone = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public Retail()
        {

        }

        public Retail(int retailID, City city, string addres, string phone, string mobilePhone, string location, string name)
        {
            this._retailID = retailID;
            this._city = city;
            this._address = addres;
            this._phone = phone;
            this._mobilePhone = mobilePhone;
            this._location = location;
            this._name = name;
        }
    }
}
