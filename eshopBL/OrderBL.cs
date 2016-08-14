using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;
using System.Data;
using eshopUtilities;

namespace eshopBL
{
    public class OrderBL
    {
        public int SaveOrder(Order order)
        {
            OrderDL orderDL = new OrderDL();
            int status = orderDL.SaveOrder(order);
            Common.SendOrder();
            return status;
        }

        public Order GetOrder(int orderID)
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.GetOrder(orderID);
        }

        public DataTable GetOrders()
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.GetOrders(-1, DateTime.MinValue, DateTime.Now, null);
        }

        public DataTable GetOrders(int statusID, DateTime dateFrom, DateTime dateTo, int? userID)
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.GetOrders(statusID, dateFrom, dateTo, userID);
        }
        

        public DataTable GetOrderStatuses(bool allSelection)
        {
            OrderDL orderDL = new OrderDL();
            DataTable orderStatuses = orderDL.GetOrderStatuses();
            if (allSelection)
            {
                DataRow newRow = orderStatuses.NewRow();
                newRow["orderStatusID"] = -1;
                newRow["name"] = "Sve";
                orderStatuses.Rows.InsertAt(newRow, 0);
            }
            return orderStatuses;
        }

        public DataTable GetOrderItemsFull(int orderID)
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.GetOrderItemsFull(orderID);
        }

        public int UpdateOrderStatus(int orderID, int orderStatusID)
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.UpdateOrderStatus(orderID, orderStatusID);
        }

        public int DeleteOrder(int orderID)
        {
            OrderDL orderDL = new OrderDL();
            return orderDL.DeleteOrder(orderID);
        }

        public DataTable GetLast10()
        {
            return new OrderDL().GetLast10(int.Parse(System.Web.Security.Membership.GetUser().ProviderUserKey.ToString()));
        }

        public DataTable GetNumberOfOrdersByDay(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetNumberOfOrdersByDay(dateFrom, dateTo);
            //int index = 0;
            //bool exists=false;

            //foreach (DateTime day in Common.EachDay(dateFrom, dateTo))
            //{
                //for(int i = 0; i<orders.Rows.Count; i++)
                //{
                    //if (((DateTime)orders.Rows[i][1]).Date.Equals(day.Date))
                    //{
                        //exists = true;
                        //break;
                    //}
                //}
                //if (!exists)
                //{
                    //DataRow newRow = orders.NewRow();
                    //newRow[0] = 0;
                    //newRow[1] = day.Date;
                    //orders.Rows.InsertAt(newRow, index);
                    //exists = false;
                //}
                //index++;
                //exists = false;
            //}

            //return orders;
        }

        public DataTable GetNumberOfOrdersByMonth(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetNumberOfOrdersByMonth(dateFrom, dateTo);
        }

        public DataTable GetValuesByMonth(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetValuesByMonth(dateFrom, dateTo);
        }

        public DataTable GetValuesByDay(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetValuesByDay(dateFrom, dateTo);
            //int index = 0;
            //bool exists = false;

            //foreach (DateTime day in Common.EachDay(dateFrom, dateTo))
            //{
                //for (int i = 0; i < values.Rows.Count; i++)
                //{
                    //if (((DateTime)values.Rows[i][1]).Date.Equals(day.Date))
                    //{
                        //exists = true;
                        //break;
                    //}
                //}
                //if (!exists)
                //{
                    //DataRow newRow = values.NewRow();
                    //newRow[0] = 0;
                    //newRow[1] = day.Date;
                    //values.Rows.InsertAt(newRow, index);
                    //exists = false;
                //}
                //index++;
                //exists = false;
            //}
            //return values;
        }

        public DataTable GetCategoryPercentage(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetCategoryPercentage(dateFrom, dateTo);
        }

        public DataTable GetCategoryValue(DateTime dateFrom, DateTime dateTo)
        {
            return new OrderDL().GetCategoryValue(dateFrom, dateTo);
        }
    }
}
