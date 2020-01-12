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
        private bool _sendDeliveryInfo;

        public OrderStatus()
        {
        }
        
        public OrderStatus(int orderStatusID, string name, bool sendDeliveryInfo)
        {
            _orderStatusID=orderStatusID;
            _name=name;
            _sendDeliveryInfo = sendDeliveryInfo;
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

        public bool SendDeliveryInfo
        {
            get { return _sendDeliveryInfo; }
            set { _sendDeliveryInfo = value; }
        }
    }
}
