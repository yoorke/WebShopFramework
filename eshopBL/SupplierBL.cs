using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class SupplierBL
    {
        public List<Supplier> GetSuppliers(bool allSelection)
        {
            SupplierDL supplierDL = new SupplierDL();
            List<Supplier> suppliers = supplierDL.GetSuppliers();

            if (allSelection && suppliers != null)
                suppliers.Insert(0, new Supplier(-1, "Sve"));

            return suppliers;
        }

        public int SaveSupplier(Supplier supplier)
        {
            SupplierDL supplierDL = new SupplierDL();
            if (supplier.SupplierID > 0)
                return supplierDL.UpdateSupplier(supplier);
            else
                return supplierDL.SaveSupplier(supplier);
        }

        public int DeleteSupplier(int supplierID)
        {
            SupplierDL supplierDL = new SupplierDL();
            return supplierDL.DeleteSupplier(supplierID);
        }
    }
}
