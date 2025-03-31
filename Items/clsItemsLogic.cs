using InvoiceSystem.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Items
{
    internal class clsItemsLogic
    {
        
        /// <summary>
        /// Create object clsDataAccess to access the database
        /// </summary>
        clsDataAccess dataAccess;
        /// <summary>
        /// List to hold Items retrived from database
        /// </summary>
        List<clsItem> allItemList = new List<clsItem>();
        /// <summary>
        /// List to hold Invoices where an Item appears
        /// for CheckItemInvoices()
        /// </summary>
        List<clsInvoice> itemInvoice = new List<clsInvoice>();
        /// <summary>
        /// Store Item information
        /// </summary>
        clsItem Item;
        /// <summary>
        /// Store invoice information
        /// </summary>
        clsInvoice Invoice;


        /// <summary>
        /// Run an execute query
        /// and fill the itemList with what is retrieved
        /// </summary>
        /// <returns>allItemList</returns>
        public List<clsItem> GetAllItems()
        {
            dataAccess = new clsDataAccess();

            int iRetVal = 0;                    // Number of return values
            DataSet dataset = new DataSet();    // Dataset to hold returned values
            clsItem Item;                      // Load values into the DataGrid
            
            // Clear allItemList of any entries
            // This is so that the old itemlist is updated with
            // the latest query results
            if (allItemList.Count > 0)
            {
                allItemList.Clear();
            }

            // SQL Query
            string sSQL = clsItemsSQL.GetItems();
            // Execute
            dataset = dataAccess.ExecuteSQLStatement(sSQL, ref iRetVal);

            // Add results to a list
            for (int i = 0; i < iRetVal; i++)
            {
                Item = new clsItem();
                // Add Code, Item Description, Cost
                Item.sItemCode = dataset.Tables[0].Rows[i][0].ToString();
                Item.sDescription = dataset.Tables[0].Rows[i]["ItemDesc"].ToString();
                Item.sCost = dataset.Tables[0].Rows[i]["Cost"].ToString();
                allItemList.Add(Item);
            }


            return allItemList;
        }
        /// <summary>
        /// This method will get the item information given the selected itemCode
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <returns>Item</returns>
        public clsItem GetItemInfo(string sItemCode)
        {
            return Item;
        }

        /// <summary>
        /// Method to add an Item to the database
        /// Item will contain all fields needed to 
        /// successfully add to the database
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(clsItem item)
        {
            dataAccess = new clsDataAccess();
            int status;

            // SQL Statement 
            string sSQL = clsItemsSQL.InsertItem(item.sItemCode, item.sDescription, item.sCost);
            // Execute
            status = dataAccess.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Method to update an Item value within the database
        /// **Accepted modified values will be description and cost
        /// **Item Code should not be changed!
        /// </summary>
        /// <param name="newItem"></param>
        public void UpdateItem(clsItem newItem)
        {
            dataAccess = new clsDataAccess();
            int status;    // Success

            // Query to update the seat number
            string sSQL = clsItemsSQL.UpdateItem(newItem.sItemCode, newItem.sDescription, newItem.sCost);
            // Execute
            status = dataAccess.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Method to delete a selected Item given itemCode
        /// **No invoice must have item in their invoice
        /// </summary>
        /// <param name="sItemCode"></param>
        public void DeleteItem(string sItemCode)
        {
            // No invoices have this item, OK to delete
            dataAccess = new clsDataAccess();
            int status;    // Success

            // Query to delete the passenger (must delete link first)
            string sSQL = clsItemsSQL.DeleteItem(sItemCode);
            // Execute
            status = dataAccess.ExecuteNonQuery(sSQL);
        }
        
        /// <summary>
        /// This method checks if the item is attached to an invoice
        /// returns a list of invoices that the item appears on
        /// to display to user
        /// </summary>
        /// <param name="iItemCode"></param>
        /// <returns></returns>
        public List<clsInvoice> CheckItemInvoice(string sItemCode)
        {
            dataAccess = new clsDataAccess();

            int iRetVal = 0;                    // Number of return values
            DataSet dataset = new DataSet();    // Dataset to hold returned values
            clsInvoice Invoice;

            // Clear itemInvoice of any entries
            // This is so that the old itemlist is updated with
            // the latest query results
            if (itemInvoice.Count > 0)
            {
                itemInvoice.Clear();
            }

            // SQL Query
            string sSQL = clsItemsSQL.GetInvoices(sItemCode);
            // Execute
            dataset = dataAccess.ExecuteSQLStatement(sSQL, ref iRetVal);

            // Add results to a list
            for (int i = 0; i < iRetVal; i++)
            {
                Invoice = new clsInvoice();
                // Add returned invoice number
                Invoice.sInvoiceNumber = dataset.Tables[0].Rows[i][0].ToString();
                itemInvoice.Add(Invoice);
            }

            return itemInvoice;
        }
    }
}
