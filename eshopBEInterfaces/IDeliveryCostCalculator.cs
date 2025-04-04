﻿using eshopBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBLInterfaces
{
    public interface IDeliveryCostCalculator
    {
        double CalculateDeliveryCost(Order order, Settings settings, int deliveryServiceID);
        double CalculateDeliveryCost(Package package, Settings settings, int deliveryServiceID);
    }
}
