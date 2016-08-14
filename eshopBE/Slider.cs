using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Slider
    {
        private int _sliderID;
        private string _name;
        private List<SliderItem> _items;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private bool _active;

        public Slider()
        {
        }

        public Slider(int sliderID, string name, DateTime insertDate, DateTime updateDate, bool active)
        {
            _sliderID = sliderID;
            _name = name;
            _insertDate = insertDate;
            _updateDate = updateDate;
            _active = active;
        }

        public int SliderID
        {
            get { return _sliderID; }
            set { _sliderID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<SliderItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public DateTime InsertDate
        {
            get { return _insertDate; }
            set { _insertDate = value; }
        }

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
    }
}
