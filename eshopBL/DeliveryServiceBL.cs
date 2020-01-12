using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class DeliveryServiceBL
    {
        public List<DeliveryService> GetAll(bool addSelect = false)
        {
            List<DeliveryService> deliveryServices = new DeliveryServiceDL().GetAll();
            if (addSelect)
                deliveryServices.Insert(0, new DeliveryService(-1, "Odaberi"));
            return deliveryServices;
        }

        public DeliveryService GetByID(int id)
        {
            return new DeliveryServiceDL().GetByID(id);
        }
    }
}
