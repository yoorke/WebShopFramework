using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Settings
    {
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string WorkingHours { get; set; }
        public double DeliveryCost { get; set; }
        public double FreeDeliveryTotalValue { get; set; }
        public double ExchangeRate { get; set; }
        public string UnknownBrandName { get; set; }
        public string AIDescriptionSystemText { get; set; }
        public string AIDescriptionUserText { get; set; }
    }
}
