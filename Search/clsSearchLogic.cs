using InvoiceSystem.Common;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Search
{
    internal class clsSearchLogic
    {
        /*
         * Summary: Search for invoices, displayed as a list
         *          limit invoices displayed by dropdowns: 
         *          Invoice Number, Invoice Date, Selecting Total Charge (unique, sorted)
         *          
         * Note:    Must have comments about how the Invoice ID will be 
         *          stored when the user selects an invoice, 
         *          and how the main window will access that variable.
         */

        /// <summary>
        /// Connection string for the database
        /// </summary>
        private readonly string sConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Directory.GetCurrentDirectory() + "\\Invoice.accdb";

        /// <summary>
        /// This is the reference to the clsSearchSQL class. That holds all the SQL Statements for getting data.
        /// </summary>
        private readonly clsSearchSQL searchSQL = new clsSearchSQL();


        /// <summary>
        /// This is a varibale to hold the invoice ID after the user chooses an invoice from the search window
        /// </summary>
        private int selectedInvoiceID;


        /// <summary>
        /// Gets or sets the selected InvoiceID the InvoideID will be stored here after
        /// the user selects an invoice in the search window
        /// </summary>
        public int SelectedInvoiceID
        {
            get { return selectedInvoiceID; }
            private set { selectedInvoiceID = value; }
        }

        /// <summary>
        /// Sets the selected InvoiceId when a user chooses a invoice in the search window.
        /// The main window will be able to get this value through the SelectedInvoiceID property.
        /// </summary>
        /// <param name="invoiceID"></param>
        public void SetSelectedInvoiceID(int invoiceID)
        {
            SelectedInvoiceID = invoiceID;
        }


        /// <summary>
        /// Gets the distinct invoice numbers and runs the query
        /// </summary>
        /// <returns></returns>
        public List<int> GetDistinctInvoiceNumbers()
        {
            return ExecuteQueryAndRetrieveIntegers(searchSQL.GetDistinctInvoiceNumbersQuery());
        }

        /// <summary>
        /// Gets the distinct date for the invoice and runs the query
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetDistinctInvoiceDates()
        {
            return ExecuteQueryAndRetrieveDates(searchSQL.GetDistinctInvoiceDatesQuery());
        }

        /// <summary>
        /// Gets the distrinct total costs for the invoice and runs the query
        /// </summary>
        /// <returns></returns>
        public List<decimal> GetDistinctTotalCosts()
        {
            return ExecuteQueryAndRetrieveDecimals(searchSQL.GetDistinctTotalCostsQuery());
        }


        /// <summary>
        /// Executes the SQL query to the integers from the first column of each row
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<int> ExecuteQueryAndRetrieveIntegers(string query)
        {
            List<int> results = new List<int>();

            using (OleDbConnection connection = new OleDbConnection(sConnectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                try
                {
                    connection.Open();
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetInt32(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
            }

            return results;
        }


        /// <summary>
        /// Executes a SQL query to retrieve dates from the first column of every row.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<DateTime> ExecuteQueryAndRetrieveDates(string query)
        {
            List<DateTime> results = new List<DateTime>();

            using (OleDbConnection connection = new OleDbConnection(sConnectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                try
                {
                    connection.Open();
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetDateTime(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
            }

            return results;
        }

        /// <summary>
        /// Executes a SQL query to retrieve decimals from the first column.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private List<decimal> ExecuteQueryAndRetrieveDecimals(string query)
        {
            List<decimal> results = new List<decimal>();

            using (OleDbConnection connection = new OleDbConnection(sConnectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                try
                {
                    connection.Open();
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(reader.GetDecimal(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
            }

            return results;
        }



        /// <summary>
        /// Retrieves all invoices from the database.
        /// </summary>
        /// <returns>A list of invoices.</returns>
        public List<clsInvoice> GetAllInvoices()
        {
            List<clsInvoice> invoices = new List<clsInvoice>();
            string query = searchSQL.GetAllInvoicesQuery();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(sConnectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int invoiceNum = reader.GetInt32(reader.GetOrdinal("InvoiceNum"));
                            DateTime invoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate"));
                            decimal totalCost = reader.GetDecimal(reader.GetOrdinal("TotalCost"));

                            string sInvoiceNumber = invoiceNum.ToString();
                            string sInvoiceDate = invoiceDate.ToString("MM/dd/yyyy");
                            string sTotalCost = totalCost.ToString("F2");

                            clsInvoice invoice = new clsInvoice(sInvoiceNumber, sInvoiceDate, sTotalCost, new List<clsItem>());
                            invoices.Add(invoice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving invoices: " + ex.Message, ex);
            }

            return invoices;
        }

        /// <summary>
        /// Retrieves a list of invoices filtered by the invoiceNum, invoiceDate, and totalCost
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<clsInvoice> GetFilteredInvoices(int? invoiceNum, DateTime? invoiceDate, decimal? totalCost)
        {
            List<clsInvoice> invoices = new List<clsInvoice>();
            string query = searchSQL.BuildFilteredQuery(invoiceNum, invoiceDate, totalCost);

            try
            {
                using (OleDbConnection connection = new OleDbConnection(sConnectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int invNum = reader.GetInt32(reader.GetOrdinal("InvoiceNum"));
                            DateTime invDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate"));
                            decimal invCost = reader.GetDecimal(reader.GetOrdinal("TotalCost"));

                            string sInvNum = invNum.ToString();
                            string sInvDate = invDate.ToString("MM/dd/yyyy");
                            string sInvCost = invCost.ToString("F2");

                            clsInvoice invoice = new clsInvoice(sInvNum, sInvDate, sInvCost, new List<clsItem>());
                            invoices.Add(invoice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving filtered invoices: " + ex.Message, ex);
            }

            return invoices;
        }
    }
}

