using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class AttributeValue
    {
        private int _attributeValueID;
        private string _value;
        private int _attributeID;
        private int _sort;
        private string _kimtecValue;
        private int _count;

        public int AttributeValueID
        {
            get { return _attributeValueID; }
            set { _attributeValueID = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int AttributeID
        {
            get { return _attributeID; }
            set { _attributeID = value; }
        }

        public int Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        public string KimtecValue
        {
            get { return _kimtecValue; }
            set { _kimtecValue = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }


        public AttributeValue()
        {
        }

        public AttributeValue(int attributeValueID, string value, int attributeID, int sort, string kimtecValue, int count)
        {
            _attributeValueID = attributeValueID;
            _value = value;
            _attributeID = attributeID;
            _sort = sort;
            _kimtecValue = kimtecValue;
            _count = count;
        }
    }
}
