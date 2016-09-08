using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class AdminMenuItem
    {
        private int _adminMenuItemID;
        private string _name;
        private string _url;
        private string _icon;
        private int? _parentMenuItemID;
        private bool _isVisible;
        private int _sortOrder;
        private List<AdminMenuItem> _subMenu;

        public int AdminMenuItemID
        {
            get { return _adminMenuItemID; }
            set { _adminMenuItemID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public int? ParentMenuItemID
        {
            get { return _parentMenuItemID; }
            set { _parentMenuItemID = value; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public List<AdminMenuItem> SubMenu
        {
            get { return _subMenu; }
            set { _subMenu = value; }
        }

        public AdminMenuItem()
        {

        }

        public AdminMenuItem(int adminMenuItemID, string name, string url, string icon, int? parentMenuItemID, bool isVisible, int sortOrder)
        {
            _adminMenuItemID = adminMenuItemID;
            _name = name;
            _url = url;
            _icon = icon;
            _parentMenuItemID = parentMenuItemID;
            _isVisible = isVisible;
            _sortOrder = sortOrder;
        }
    }
}
