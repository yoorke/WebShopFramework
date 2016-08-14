using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Delivery
    {
        private int _deliveryID;
        private string _name;

        public Delivery()
        {
        }

        public Delivery(int deliveryID, string name)
        {
            _deliveryID = deliveryID;
            _name = name;
        }

        public int DeliveryID
        {
            get { return _deliveryID; }
            set { _deliveryID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
