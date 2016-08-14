using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class User
    {
        private int _userID;
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _password;
        private string _address;
        private string _city;
        private string _phone;
        private UserType _userType;
        private string _email;
        private string _salt;
        private DateTime _insertDate;
        private string _zip;


        public User()
        {
        }

        public User(int userID, string firstName, string lastName, string username, string password, string address, string city, string phone, UserType userType, string email, string salt, DateTime insertDate, string zip)
        {
            _userID = userID;
            _firstName = firstName;
            _lastName = lastName;
            _username = username;
            _password = password;
            _address = address;
            _city = city;
            _phone = phone;
            _userType = userType;
            _email = email;
            _salt = salt;
            _insertDate = insertDate;
            _zip = zip;
        }

        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
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

        public UserType UserType
        {
            get { return _userType; }
            set { _userType = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Salt
        {
            get { return _salt; }
            set { _salt = value; }
        }

        public DateTime InsertDate
        {
            get { return _insertDate; }
            set { _insertDate = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }
    }
}
