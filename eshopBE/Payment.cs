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

        public Payment()
        {
        }

        public Payment(int paymentID, string name)
        {
            _paymentID = paymentID;
            _name = name;
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
    }
}
