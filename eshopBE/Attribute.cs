using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Attribute
    {
        private int _attributeID;
        private string _name;
        private bool _filter;
        List<AttributeValue> _values;
        private bool _isDescription;
        private int _position;

        public Attribute()
        {
        }

        public Attribute(int attributeID, string name, bool filter, bool isDescription, int position)
        {
            _attributeID = attributeID;
            _name = name;
            _filter = filter;
            _isDescription = isDescription;
            _position = position;
        }

        public int AttributeID
        {
            get { return _attributeID; }
            set { _attributeID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public List<AttributeValue> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public string NameScreen
        {
            get { return _name.Substring(_name.IndexOf("-") + 1, _name.Length - _name.IndexOf("-") - 1); }
        }

        public bool IsDescription
        {
            get { return _isDescription; }
            set { _isDescription = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }
}
