using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class CustomPage
    {
        private int _customPageID;
        private string _title;
        private string _description;
        private string _content;
        private string _url;
        private string _heading;
        private int _customPageCategoryID;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private string _head;
        private string _firstPage;
        private bool _showOnFirstPage;
        private int _sortIndex;
        private string _imageUrl;
        private bool _isActive;

        public int CustomPageID
        {
            get { return _customPageID; }
            set { _customPageID = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }

        public int CustomPageCategoryID
        {
            get { return _customPageCategoryID; }
            set { _customPageCategoryID = value; }
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

        public string Head
        {
            get { return _head; }
            set { _head = value; }
        }

        public string FirstPage
        {
            get { return _firstPage; }
            set { _firstPage = value; }
        }

        public bool ShowOnFirstPage
        {
            get { return _showOnFirstPage; }
            set { _showOnFirstPage = value; }
        }

        public int SortIndex
        {
            get { return _sortIndex; }
            set { _sortIndex = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
    }
}
