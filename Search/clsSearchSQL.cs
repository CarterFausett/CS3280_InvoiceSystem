using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.IO;
using System.Data.OleDb;

namespace InvoiceSystem.Search
{
    internal class clsSearchSQL
    {
        /// <summary>
        /// Connection string to connect to the database
        /// </summary>
        private readonly string sConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Directory.GetCurrentDirectory() + "\\Invoice.accdb";


        /// <summary>
        /// Builds a SQL query to get all invoices
        /// </summary>
        /// <returns></returns>
        public string GetAllInvoicesQuery()
        {
            return "SELECT * FROM Invoices";
        }

        /// <summary>
        /// Builds a SQL query to get all unique incoice nums
        /// </summary>
        /// <returns></returns>
        public string GetDistinctInvoiceNumbersQuery()
        {
            return "SELECT DISTINCT InvoiceNum FROM Invoices ORDER BY InvoiceNum";
        }

        /// <summary>
        /// Builds a SQL query to get all unique invoice dates
        /// </summary>
        /// <returns></returns>
        public string GetDistinctInvoiceDatesQuery()
        {
            return "SELECT DISTINCT InvoiceDate FROM Invoices ORDER BY InvoiceDate";
        }

        /// <summary>
        /// Builds a SQl query to get all unique total costs invoices
        /// </summary>
        /// <returns></returns>
        public string GetDistinctTotalCostsQuery()
        {
            return "SELECT DISTINCT TotalCost FROM Invoices ORDER BY TotalCost";
        }


        /// <summary>
        /// Builds a SQL query to filter based on invoiceNum, invoiceDate, and totalCost
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        public string BuildFilteredQuery(int? invoiceNum, DateTime? invoiceDate, decimal? totalCost)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM Invoices WHERE 1=1");

            if (invoiceNum.HasValue)
                query.Append($" AND InvoiceNum = {invoiceNum.Value}");

            if (invoiceDate.HasValue)
                query.Append($" AND InvoiceDate = #{invoiceDate.Value:MM/dd/yyyy}#");

            if (totalCost.HasValue)
                query.Append($" AND TotalCost = {totalCost.Value}");

            return query.ToString();
        }



    }
}
