using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{   
    [Serializable]
    public class SliderItem
    {
        private int _sliderItemID;
        private int _sliderID;
        private string _imageUrl;
        private int _sortIndex;
        private string _url;

        public SliderItem()
        {
        }

        public SliderItem(int sliderItemID, int sliderID, string imageUrl, int sortIndex, string url)
        {
            _sliderItemID = sliderItemID;
            _sliderID = sliderID;
            _imageUrl = imageUrl;
            _sortIndex = sortIndex;
            _url = url;
        }

        public int SliderItemID
        {
            get { return _sliderItemID; }
            set { _sliderItemID = value; }
        }

        public int SliderID
        {
            get { return _sliderID; }
            set { _sliderID = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public int SortIndex
        {
            get { return _sortIndex; }
            set { _sortIndex = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
