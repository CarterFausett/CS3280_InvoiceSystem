using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Items
{
    internal class clsItemsSQL
    {
        /// <summary>
        /// Query to get all invoices that have
        /// a selected item
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetInvoices(string sItemCode)
        {
            try
            {
                string sSQL = "SELECT DISTINCT(InvoiceNum) FROM LineItems " + 
                              "WHERE ItemCode = \'" + sItemCode + "\'";
                return sSQL;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Query to get all available items from ItemDesc Table
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetItems()
        {
            try
            {
                string sSQL = "SELECT ItemCode, ItemDesc, Cost FROM ItemDesc";
                return sSQL;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Query to update selected item
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="sItemCost"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string UpdateItem(string sItemCode, string sItemDesc, string sItemCost)
        {
            try
            {
                string sSQL = "UPDATE ItemDesc" + 
                              " SET ItemDesc = \'" + sItemDesc + "\', Cost = " + sItemCost +
                              " WHERE ItemCode = \'" + sItemCode + "\'";
                return sSQL;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }
        /// <summary>
        /// Query to Insert a new item into the ItemDesc table
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="sItemCost"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string InsertItem(string sItemCode, string sItemDesc, string sItemCost)
        {
            try
            {
                string sSQL = "INSERT INTO ItemDesc(ItemCode, ItemDesc, Cost)" +
                               " VALUES(\'" + sItemCode + "\', \'" + sItemDesc + "\', " + sItemCost + ")";
                return sSQL;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

        /// <summary>
        /// Query to Delete item from the ItemDesc table
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sItemDesc"></param>
        /// <param name="sItemCost"></param>
        /// <returns></returns>
        public static string DeleteItem(string sItemCode)
        {
            try
            {
                string sSQL = "DELETE FROM ItemDesc " +
                              "WHERE ItemCode = \'" + sItemCode + "\'";
                return sSQL;
            }
            catch (Exception ex)
            {
                throw new Exception(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." +
                                    MethodInfo.GetCurrentMethod().Name + " -> " + ex.Message);
            }
        }

    }
}
