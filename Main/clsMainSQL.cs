using InvoiceSystem.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace InvoiceSystem.Main
{
    internal class clsMainSQL
    {

        /// <summary>
        /// Method for returning a string containing an SQL Statement that when executed will return the information on the invoice with the given invoiceNum
        /// </summary>
        /// <param name="invoiceNum">An integer containing the InvoiceNum of the desired invoice information.</param>
        /// <returns>Returns a string containing and SQL statement that, when executed, will retrieve all the information found in the Invoices table
        ///     associated with the given invoiceNum.</returns>
        public static string SQLGetInvoiceByInvoiceNum (int invoiceNum)
        {
            try
            {
                string getInvoiceCmd = "SELECT * FROM Invoices WHERE InvoiceNum = " + invoiceNum;

                return getInvoiceCmd;
            }
            catch (Exception ex)
            {
                //if an exception is raised, grab the information of the last inserted invoice
                return "SELECT * FROM Invoices WHERE InvoiceNum = (SELECT MAX(InvoiceNum) FROM Invoices)";
            }
        }


        /// <summary>
        /// Static method for returning a string that contains an SQL statement that will grab all line items associated with a given invoice.
        /// </summary>
        /// <param name="invoice">A clsInvoice containing a valid InvoiceNum that will be used to retrieve the associated items.</param>
        /// <returns>Returns a SQL statement that when executed will retrieve a list of items associated with the given invoice.</returns>
        public static string SQLGetItemsByInvoice(clsInvoice invoice)
        {
            try 
            {
                string getItemsCmd = "SELECT * FROM ItemDesc id RIGHT JOIN LineItems li ON id.ItemCode = li.ItemCode WHERE li.InvoiceNum = " + invoice.sInvoiceNumber;
                // (li.LineItemNum, id.ItemCode, id.ItemDesc, id.Cost)
                return getItemsCmd;
            }
            catch (Exception e)
            {
                //if an exception is raised, return a statement that will grab the invoice items from the last created invoice
                return "SELECT * FROM ItemDesc id RIGHT JOIN LineItems li ON id.ItemCode = li.ItemCode WHERE li.InvoiceNum = (SELECT MAX(InvoiceNum) FROM Invoices)";
            }

        }

        
        /// <summary>
        /// Method for returning an SQL statement that will retrieve all items with listings in the database.
        /// </summary>
        /// <returns>Returns a statement that when executed will retrieve all items found in the ItemDesc table in the database.</returns>
        public static string SQLGetAllItems()
        {
            try
            {
                return "SELECT * FROM ItemDesc";
            }
            catch (Exception e)
            {
                //if somehow an exception is raised, try again
                return "SELECT * FROM ItemDesc";
            }
        }

        /// <summary>
        /// Method for generating and returning an SQL statement that will return all Invoice numbers from Invoices
        /// </summary>
        /// <returns>Returns an SQL statement that, when executed, will retrieve all invoice numbers found in the Invoices table.</returns>
        public static string SQLGetAllInvoiceNums()
        {
            try
            {
                return "SELECT InvoiceNum FROM Invoices";
            }
            catch (Exception e)
            {
                //if somehow an exception is raised, try again
                return "SELECT InvoiceNum FROM Invoices";
            }
        }
        
        /// <summary>
        /// Method for returning an SQL statement that, when executed, will update the total cost number and invoice date in the database.
        /// </summary>
        /// <param name="invoice">The invoice whose information will be updated in the database.</param>
        /// <returns>Returns an SQL statement that will update the total cost and invoice date of the given invoice in the database.</returns>
        public static string SQLUpdateInvoiceInformation(clsInvoice invoice)
        {
            try
            {
                return "UPDATE Invoices SET TotalCost = \"$" + invoice.sTotalCost + "\", InvoiceDate = \"" + invoice.sInvoiceDate + "\" WHERE InvoiceNum = " + invoice.sInvoiceNumber;
            }
            catch (Exception e)
            {
                //there is no alternative to updating
                return "";
            }
        }

        /// <summary>
        /// Method for generating an SQL statement that inserts an entry into LineItems with the provided information.
        /// </summary>
        /// <param name="invoice">A clsInvoice object that contains the information of the invoice associated with the LineItem entry that will be created.</param>
        /// <param name="item">A clsItem object that contains the information of the item associated with the LineItem entry that will be created.</param>
        /// <param name="itemLine">The LineItem line number associated with this LineItem entry that will be created.</param>
        /// <returns>Returns an SQL statement that when executed will insert an entry into LineItems with the provided information.</returns>
        public static string SQLInsertInvoiceItemList(clsInvoice invoice, clsItem item, int itemLine)
        {
            try
            {
                return "INSERT INTO LineItems (InvoiceNum, ItemCode, LineItemNum) VALUES (" + invoice.sInvoiceNumber + ", \"" + item.sItemCode + "\", " + itemLine + ")";
            }
            catch (Exception e)
            {
                //there is no alternative
                return "";
            }
        }

        /// <summary>
        /// Method for deleting all entries in the LineItems table associated with the given invoice.
        /// </summary>
        /// <param name="invoice">A clsInvoice object containing the invoice information of the entries that will be deleted from the LineItems table.</param>
        /// <returns>Returns an SQL statement that when executed will delete all entries from the LineItems table that have an invoiceNum in common with the provided invoice.</returns>
        public static string SQLDeleteInvoiceItemList(clsInvoice invoice)
        {
            try
            {
                return "DELETE * FROM LineItems WHERE InvoiceNum = " + invoice.sInvoiceNumber;
            }
            catch (Exception e)
            {
                //there is no alternative course of action
                return "";
            }
        }

        /// <summary>
        /// Method for isnerting a new invoice entry using a provided clsInvoice object's invoiceDate and totalCost.
        /// </summary>
        /// <param name="invoice">A clsInvoice obejct containing the information that will be inserted into the database.</param>
        /// <returns>Returns an SQL statement that when executed will insert a new entry into the Invoices table with the given invoice date and total cost.</returns>
        public static string SQLInsertNewInvoice(clsInvoice invoice)
        {
            try
            {
                return "INSERT INTO Invoices (InvoiceDate, TotalCost) VALUES (\"" + invoice.sInvoiceDate + "\", \"$" + invoice.sTotalCost + "\")";
            }
            catch (Exception e)
            {
                //there is no alternative course of action
                return "";
            }
        }

        /// <summary>
        /// Method for retrieving the last inserted invoice from the Invoices table in the database.
        /// </summary>
        /// <returns>Returns an SQL statement that when executed will retrieve the highest InvoiceNum found in the Invoices table (or the invoiceNum of the last inserted invoice).</returns>
        public static string SQLGetLastInvoiceNum()
        {
            try
            {
                return "SELECT MAX(InvoiceNum) FROM Invoices";
            }
            catch (Exception e)
            {
                //try A G A I N
                return "SELECT MAX(InvoiceNum) FROM Invoices";
            }
        }

        
    }
}
