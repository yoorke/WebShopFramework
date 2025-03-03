using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Data;
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

        public double GetDeliveryPriceByWeight(int deliveryServiceID, double weight)
        {
            return new DeliveryServiceDL().GetDeliveryPriceByWeight(deliveryServiceID, weight);
        }

        public DataTable GetDeliveryPrices(int deliveryServiceID)
        {
            return new DeliveryServiceDL().GetDeliveryPrices(deliveryServiceID);
        }

        public int GetDeliveryServiceIDByZip(string zip)
        {
            return new DeliveryServiceDL().GetDeliveryServiceIDByZip(zip);
        }

        public int GetActiveDeliveryServiceID()
        {
            return new DeliveryServiceDL().GetActiveDeliveryServiceID();
        }
    }
}
