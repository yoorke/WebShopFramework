using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class UnitOfMeasure
    {
        private int _unitOfMeasureID;
        private string _name;
        private string _shortName;

        public int UnitOfMeasureID
        {
            get { return _unitOfMeasureID; }
            set { _unitOfMeasureID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        public string FullName
        {
            get { return _name + (_shortName != string.Empty ? " (" + _shortName + ")" : string.Empty); }
        }

        public UnitOfMeasure()
        {

        }

        public UnitOfMeasure(int unitOfMeasureID, string name, string shortName)
        {
            _unitOfMeasureID = unitOfMeasureID;
            _name = name;
            _shortName = shortName;
        }
    }
}
