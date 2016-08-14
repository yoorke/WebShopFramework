using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class OrderStatus
    {
        private int _orderStatusID;
        private string _name;

        public OrderStatus()
        {
        }
        
        public OrderStatus(int orderStatusID, string name)
        {
            _orderStatusID=orderStatusID;
            _name=name;
        }

        public int OrderStatusID
        {
            get { return _orderStatusID; }
            set { _orderStatusID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
