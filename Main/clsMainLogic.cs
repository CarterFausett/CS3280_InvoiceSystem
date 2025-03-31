using InvoiceSystem.Common;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceSystem.Main
{
    internal class clsMainLogic
    {
        /// <summary>
        /// clsDataAccess class variable for ease of executing sql statements
        /// </summary>
        private clsDataAccess dDatabase;

        /// <summary>
        /// clsMainLogic constructor, sets up this instance's dDatabase object
        /// </summary>
        public clsMainLogic() 
        {
            //clsMainLogic Constructor
            //this.bChangeMadeSinceLastSave = false; //when first loaded, nothing has been changed so there is nothing to save
            this.dDatabase = new clsDataAccess();
        }

        /// <summary>
        /// Method for building a new invoice from scratch.
        /// </summary>
        /// <returns>Returns a clsInvoice object representing the newly created (but not inserted) invoice.</returns>
        public clsInvoice BuildNewInvoice()
        {
            try
            {
                //build an invoice, first get the invoice info from database
                //set invoice information to the information retrieved by the sql query
                //build clsInvoice object based on the retrieved information
                //build this new invoice's (blank) item list
                clsInvoice returnInvoice = new clsInvoice();
                returnInvoice.sInvoiceDate = DateOnly.FromDateTime(DateTime.Today).ToString();
                returnInvoice.sInvoiceNumber = "TBD";
                decimal zero = 0.00M;
                returnInvoice.sTotalCost = zero.ToString();
                returnInvoice.sItems = new List<clsItem>();


                return returnInvoice; //return the freshly created clsInvoice object
            }
            catch (Exception ex)
            {
                return new clsInvoice();
            }
        }

        /// <summary>
        /// Method for building a clsInvoice object by grabbing the invoice information using the given invoiceNum
        /// </summary>
        /// <param name="invoiceNum">The provided invoice number to generate a clsInvoice object from.</param>
        /// <returns>Returns a clsInvoice object assembled from the information gathered from the database using the provided invoiceNum.</returns>
        public clsInvoice BuildExistingInvoice(int invoiceNum)
        {
            try
            {
                int i = 0;
                
                DataSet ds = this.dDatabase.ExecuteSQLStatement(clsMainSQL.SQLGetInvoiceByInvoiceNum(invoiceNum), ref i);
                //ds.Tables[0].Columns[0]; //column 0 is invoiceNum, 1 is invoiceDate, 2 is TotalCost

               //System.Diagnostics.Debug.Write("Building base invoice\n");
                clsInvoice invoice = new clsInvoice(ds.Tables[0].Rows[0].Field<int>(ds.Tables[0].Columns[0]).ToString(), ds.Tables[0].Rows[0].Field<DateTime>(ds.Tables[0].Columns[1]).ToString()); //created with all but total cost
                //System.Diagnostics.Debug.Write("Now adding totalCost to invoice\n");
                invoice.sTotalCost = (ds.Tables[0].Rows[0].Field<decimal>(ds.Tables[0].Columns[2]) + 0.00M).ToString(); //add total cost before passing off to BuildInvoiceItemList

                this.BuildInvoiceItemList(ref invoice);
                //build clsInvoice object and return
                //clsInvoice invoice = new clsInvoice(); //will be loaded with information other than the list of items

                //DataSet itemList = this.dDatabase.ExecuteSQLStatement(clsMainSQL.SQLGetItemsByInvoice(invoice), ref i); //will contain enough information in the invoice to grab items (InvoiceNum)

                return invoice;
            }
            catch (Exception e)
            {
                return new clsInvoice(); //if an exception is raised, return an empty clsInvoice object
            }
        }

        /// <summary>
        /// Method for building the item list for a provided invoice reference.
        /// </summary>
        /// <param name="invoice">A clsInvoice object that will be used to gather associated items and have its item list added to it.</param>
        private void BuildInvoiceItemList(ref clsInvoice invoice)
        {
            try
            {
                int i = 0;
                //get the list of items associated with this invoice
                DataTable items = this.dDatabase.ExecuteSQLStatement(clsMainSQL.SQLGetItemsByInvoice(invoice), ref i).Tables[0];
                //build item objects and make a list
                //add clsItem list to invoice

                clsItem[] itemArray = new clsItem[i];
                List<clsItem> itemList = new List<clsItem>();
                foreach (DataRow row in items.Rows)
                {
                    //(li.LineItemNum, id.ItemCode, id.ItemDesc, id.Cost)
                    //itemCode, itemDesc, cost, invoiceNum, LineItemNum, ItemCode (again)
                    clsItem item = new clsItem();
                    item.sItemCode = row.Field<string>(items.Columns[0]);
                    item.sDescription = row.Field<string>(items.Columns[1]);
                    item.sCost = (row.Field<decimal>(items.Columns[2]) + 0.00M).ToString();

                    itemArray[row.Field<int>(items.Columns[4]) - 1] = item;
                    //itemList.Insert(row.Field<int>(items.Tables[0].Columns[3]), item); //this will leave itemList[0] empty
                }

                //itemList.Add(new clsItem());
                itemList.AddRange(itemArray);

                invoice.sItems = itemList; //set the provided invoice's sItems property to the newly created list of items
            }

            catch (Exception e)
            {
                invoice.sItems = new List<clsItem>();
            }


        }

        /// <summary>
        /// Method for building and returning a list of all invoices found in the database.
        /// </summary>
        /// <returns>Returns a list of clsInvoice items containing the information of all invoices found in the database.</returns>
        public List<clsInvoice> GetAllInvoices()
        {
            try
            {
                //get all invoices in the databse
                int i = 0;
                DataSet invoices = this.dDatabase.ExecuteSQLStatement(clsMainSQL.SQLGetAllInvoiceNums(), ref i);
                List<clsInvoice> invoiceList = new List<clsInvoice>();

                foreach (DataRow invoiceNum in invoices.Tables[0].Rows)
                {
                    invoiceList.Add(this.BuildExistingInvoice(invoiceNum.Field<int>(invoices.Tables[0].Columns[0]))); //build and add each invoice to the list
                }

                //convert DataSet to list of invoices
                //return list of invoices

                return invoiceList;
            }
            catch (Exception e)
            {
                return new List<clsInvoice>(); //return a blank list of clsInvoice objects if an exception is thrown
            }
        }

        /// <summary>
        /// Method for obtaining a list of clsItem objects representing all of the items found in the invoice database.
        /// </summary>
        /// <returns>Returns a list of items containing item objects for every item in the database.</returns>
        public List<clsItem> GetDatabaseItems() 
        {
            try
            {
                //grab all items from the ItemsDesc table in the database
                int i = 0;
                DataTable items = this.dDatabase.ExecuteSQLStatement(clsMainSQL.SQLGetAllItems(), ref i).Tables[0];
                /*foreach(DataColumn column in items.Columns)
                {
                    System.Diagnostics.Debug.WriteLine(column.ColumnName);
                }*/

                List<clsItem> listOfItems = new List<clsItem>();
                foreach (DataRow item in items.Rows)
                {
                    //columns
                    //0 is itemcode
                    //1 is itemdesc
                    //2 is cost
                    listOfItems.Add(new clsItem(item.Field<string>(items.Columns[0]), item.Field<string>(items.Columns[1]), (item.Field<decimal>(items.Columns[2]) + 0.00M).ToString()));

                }



                //turn DataSet of items into list of clsItems
                //return the list of items

                return listOfItems;
            }
            catch (Exception e)
            {
                return new List<clsItem>(); //if an exception is thrown, return a blank list of clsItem objects
            }
            
        }

        /// <summary>
        /// Method for generating a data table containing all the information relevant to the currently loaded invoice's list of items.
        /// </summary>
        /// <param name="invoice">A clsInvoice object whose list of items will be transferred to the returned data table.</param>
        /// <returns>Returns a data table with the information of each item associated with the given invoice in a format that can easily be displayed in a datagrid with proper column names.</returns>
        public DataTable GenerateInvoiceItemTable(clsInvoice invoice)
        {
            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add(new DataColumn("Line Number"));
                dt.Columns.Add(new DataColumn("Item Code"));
                dt.Columns.Add(new DataColumn("Item Description"));
                dt.Columns.Add(new DataColumn("Item Cost"));

                int lineNumber = 1;
                foreach (clsItem item in invoice.sItems)
                {
                    DataRow row = dt.NewRow();
                    row["Line Number"] = lineNumber;
                    lineNumber++;
                    row["Item Code"] = item.sItemCode;
                    row["Item Description"] = item.sDescription;
                    row["Item Cost"] = "$" + item.sCost;

                    dt.Rows.Add(row);

                }

                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable(); //return an empty data table if an exception is thrown
            }
        }

        /// <summary>
        /// Method for saving/updating the information in the database for the given invoice
        /// </summary>
        /// <param name="invoice"></param>
        public void SaveInvoice(ref clsInvoice invoice)
        {
            try
            {
                //the invoice given to this method will be the invoice in its updated state
                int iRet = 0;
                if (invoice.sInvoiceNumber == "TBD") //if the invoice is a new invoice
                {
                    iRet = this.dDatabase.ExecuteNonQuery(clsMainSQL.SQLInsertNewInvoice(invoice));

                    if (iRet == 1)
                        invoice.sInvoiceNumber = this.dDatabase.ExecuteScalarSQL(clsMainSQL.SQLGetLastInvoiceNum()); //set the loaded invoice number to the newly created invoice number
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(clsMainSQL.SQLUpdateInvoiceInformation(invoice)); //debug print
                    iRet = this.dDatabase.ExecuteNonQuery(clsMainSQL.SQLUpdateInvoiceInformation(invoice)); //iRet will be 1 if successful
                    this.dDatabase.ExecuteNonQuery(clsMainSQL.SQLDeleteInvoiceItemList(invoice)); //clear the existing list of items associated with this invoice
                }

                if (iRet > 0)
                {
                    this.SaveInvoiceItemList(invoice);
                }

                //set totalcost to the total cost of all items on invoice
                //update invoice information (date, totalcost)
                //loop through list of items on this invoice and update invoice item
                //delete any extra items that no longer exist in the invoice item list
            }
            catch (Exception ex)
            {
                //save failed if an exception is raised
                MessageBox.Show("An error ocurred while saving the invoice. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method for saving the line items of a given invoice to the database LineItems table.
        /// </summary>
        /// <param name="invoice">A clsInvoice object containing the list of items that will be saved to the database.</param>
        /// <exception cref="Exception">A new exception is raised if any exception is raised while running this method with the message "Exception raised when saving item information to LineItems."</exception>
        private void SaveInvoiceItemList(clsInvoice invoice)
        {
            try
            {
                int iRet = 0;
                for (int i = 0; i < invoice.sItems.Count; i++)
                {
                    iRet = this.dDatabase.ExecuteNonQuery(clsMainSQL.SQLInsertInvoiceItemList(invoice, invoice.sItems[i], i + 1));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception raised when saving item information to LineItems.");
            }
        }

        /// <summary>
        /// Static helper method for returning a list that is a copy of a given list with a specified index removed and all indexes with an index greater than the removed index will be moved to the next lower index.
        /// </summary>
        /// <param name="removeIndex">The index of the item that will be removed from the provided list.</param>
        /// <param name="items">A list of clsItems that will be used as the master list and have an index removed.</param>
        /// <returns>Returns a new list of clsItem objects that contains all the same items as the given list, minus the item at the removed index.</returns>
        public static List<clsItem> RemoveAndShiftItems (int removeIndex, List<clsItem> items)
        {
            try
            {
                List<clsItem> newList = new List<clsItem>();


                for (int i = 0; i < items.Count; i++)
                {
                    if (i != removeIndex)
                    {
                        newList.Add(items[i]);
                    }
                }

                return newList;
            }
            catch (Exception ex)
            {
                return items; //if an exception is raised, abort operations to create a new list and return the existing list
            }

        }

    }
}
