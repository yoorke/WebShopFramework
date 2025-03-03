using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Payment
    {
        private int _paymentID;
        private string _name;
        private string _description;

        public Payment()
        {
        }

        public Payment(int paymentID, string name)
        {
            _paymentID = paymentID;
            _name = name;
            _description = string.Empty;
        }

        public Payment(int paymentID, string name, string description)
        {
            _paymentID = paymentID;
            _name = name;
            _description = description;
        }

        public int PaymentID
        {
            get { return _paymentID; }
            set { _paymentID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
