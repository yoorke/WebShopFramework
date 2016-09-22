using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class UnitOfMeasureBL
    {
        public List<UnitOfMeasure> GetUnitsOfMeasure(bool addSelectAll)
        {
            List<UnitOfMeasure> unitsOfMeasure = new UnitOfMeasureDL().GetUnitsOfMeasure();
            if (addSelectAll)
                unitsOfMeasure.Insert(0, new UnitOfMeasure(0, "Odaberi", string.Empty));
            return unitsOfMeasure;
        }

        public UnitOfMeasure GetUnitOfMeasure(int unitOfMeasureID)
        {
            return new UnitOfMeasureDL().GetUnitOfMeasure(unitOfMeasureID);
        }
    }
}
