using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopDL;
using eshopBE;

namespace eshopBL
{
    public class CityBL
    {
        public List<City> GetCities(bool addSelect, bool addChoose = false)
        {
            List<City> cities = new CityDL().GetCities();

            if (addSelect)
                cities.Insert(0, new City(-1, "Svi", string.Empty));

            if (addChoose)
                cities.Insert(0, new City(-1, "Odaberite mesto", string.Empty));

            return cities;
        }

        public City GetCity(int id)
        {
            return new CityDL().GetCity(id);
        }

        public List<City> GetCities(string name)
        {
            return new CityDL().GetCities(name);
        }
    }
}
