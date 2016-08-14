using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class UserType
    {
        private int _userTypeID;
        private string _name;

        public UserType()
        {

        }

        public UserType(int userTypeID, string name)
        {
            _userTypeID = userTypeID;
            _name = name;
        }

        public int UserTypeID
        {
            get { return _userTypeID; }
            set { _userTypeID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
