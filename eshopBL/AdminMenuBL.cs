using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;
using System.Data;

namespace eshopBL
{
    public class AdminMenuBL
    {
        public List<AdminMenuItem> GetNestedList()
        {
            return GetList(new AdminMenuDL().GetDataTable(), 0);
        }

        private List<AdminMenuItem> GetList(DataTable adminMenuDT, int parentID)
        {
            List<AdminMenuItem> list = null;

            DataView dv = new DataView(adminMenuDT);
            dv.RowFilter = "parentMenuID=" + parentID.ToString();
            AdminMenuItem adminMenuItem;

            if (dv.Count > 0)
                list = new List<AdminMenuItem>();
            foreach(DataRowView row in dv)
            {
                adminMenuItem = new AdminMenuItem();
                adminMenuItem.AdminMenuItemID = (int)row["adminMenuID"];
                adminMenuItem.Name = row["name"].ToString();
                adminMenuItem.Url = row["url"].ToString();
                adminMenuItem.Icon = row["icon"].ToString();
                adminMenuItem.ParentMenuItemID = (int)row["parentMenuID"];
                adminMenuItem.IsVisible = (bool)row["isVisible"];
                adminMenuItem.SortOrder = (int)row["sortOrder"];
                adminMenuItem.SubMenu = GetList(adminMenuDT, (int)row["adminMenuID"]);

                list.Add(adminMenuItem);
            }
            return list;
        }
    }
}
