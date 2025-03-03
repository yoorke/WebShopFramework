using eshopBE;
using eshopBLInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class DeliveryCostCalculatorByWeightAndSupplier : IDeliveryCostCalculator
    {
        public double CalculateDeliveryCost(Order order, Settings settings, int deliveryServiceID)
        {
            double deliveryCost = 0;

            return deliveryCost;
        }

        public double CalculateDeliveryCost(Package package, Settings settings, int deliveryServiceID)
        {
            double deliveryCost = 0;

            return deliveryCost;
        }
    }
}
