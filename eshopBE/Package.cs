using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class Package
    {
        public Supplier Supplier { get; set; }
        public List<OrderItem> Items { get; set; }
        public double DeliveryCost { get; set; }

        public double TotalValue
        {
            get { return getTotalValue(); }
        }

        public double TotalWeight
        {
            get { return getTotalWeight(); }
        }

        private double getTotalValue()
        {
            return Items.Sum(item => item.UserPrice * item.Quantity);
        }

        private double getTotalWeight()
        {
            double totalWeight = 0;

            foreach(var item in Items)
            {
                if(item.Product.Weight == 0)
                {
                    return -1;
                }

                totalWeight += item.Product.Weight * item.Quantity;
            }

            return totalWeight;
        }
    }
}
