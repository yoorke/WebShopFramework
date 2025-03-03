using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BE
{
    public class ProductImport
    {
        public int ID { get; set; }
        public string SupplierCode { get; set; }
        public string Code { get; set; }
        public string Ean { get; set; }
        public string Name { get; set; }
        public double Pdv { get; set; }
        public string Category { get; set; }
        public string ParentCategory { get; set; }
        public string Manufacturer { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Model { get; set; }
        public string Quantity { get; set; }
        public string Currency { get; set; }
        public double B2BPrice { get; set; }
        public double WebPrice { get; set; }
        public double Price { get; set; }
        public string EnergyClass { get; set; }
        public string Declaration { get; set; }
        public string Description { get; set; }
        public string ImageUrls { get; set; }
        public string Attributes { get; set; }
        public bool Imported { get; set; }
        public string Timestamp { get; set; }
        public DateTime InsertDate { get; set; }
        public double Stock { get; set; }
        public double StockReservation { get; set; }
        public string Weight { get; set; }
    }
}
