using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class City
    {
        private int _cityID;
        private string _name;
        private string _zip;

        public int CityID
        {
            get { return _cityID; }
            set { _cityID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public City()
        {

        }
        
        public City(int cityID, string name, string zip)
        {
            this._cityID = cityID;
            this._name = name;
            this._zip = zip;
        }
    }
}
