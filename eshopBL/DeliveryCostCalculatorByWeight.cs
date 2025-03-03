using eshopBE;
using eshopBLInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class DeliveryCostCalculatorByWeight : IDeliveryCostCalculator
    {
        public double CalculateDeliveryCost(Order order, Settings settings, int deliveryServiceID)
        {
            double deliveryCost = 0;
            double totalWeight = -1;
            bool isFreeDelivery = true;

            foreach(var item in order.Items)
            {
                isFreeDelivery = isFreeDelivery && item.Product.IsFreeDelivery;
            }

            if(isFreeDelivery)
            {
                return 0;
            }

            if((totalWeight = order.TotalWeight) > 0)
            {
                deliveryCost = new DeliveryServiceBL().GetDeliveryPriceByWeight(deliveryServiceID, totalWeight);
            }
            else
            {
                if(order.TotalValue < settings.FreeDeliveryTotalValue)
                {
                    deliveryCost = settings.DeliveryCost;
                }
            }

            return deliveryCost;
        }

        public double CalculateDeliveryCost(Package package, Settings settings, int deliveryServiceID)
        {
            double deliveryCost = 0;
            double totalWeight = -1;
            //bool isFreeDelivery = true;
            bool isFreeDelivery = false;

            foreach(var item in package.Items)
            {
                //isFreeDelivery = isFreeDelivery && item.Product.IsFreeDelivery;
                isFreeDelivery = item.Product.IsFreeDelivery == true ? true : isFreeDelivery;
            }

            if(isFreeDelivery)
            {
                return 0;
            }

            if(package.TotalValue >= settings.FreeDeliveryTotalValue)
            {
                return deliveryCost;
            }

            if ((totalWeight = package.TotalWeight) > 0)
            {
                deliveryCost = new DeliveryServiceBL().GetDeliveryPriceByWeight(deliveryServiceID, totalWeight);
            }
            else
            {
                if(package.TotalValue < settings.FreeDeliveryTotalValue)
                {
                    deliveryCost = settings.DeliveryCost;
                }
            }

            return deliveryCost;
        }
    }
}
