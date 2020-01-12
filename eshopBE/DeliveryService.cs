using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class DeliveryService
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public DeliveryService()
        {

        }

        public DeliveryService(int ID, string name)
        {
            this.ID = ID;
            this.Name = name;
        }
    }
}
