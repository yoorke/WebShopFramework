using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class OrderStatusBL
    {
        public List<OrderStatus> GetAll()
        {
            return new OrderStatusDL().GetAll();
        }

        public OrderStatus GetByID(int ID)
        {
            return new OrderStatusDL().GetByID(ID);
        }
    }
}
