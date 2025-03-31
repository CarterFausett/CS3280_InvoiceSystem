using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Common
{
    class clsInvoice
    {
        /// <summary>
        /// Hold data - Invoice Number
        /// </summary>
        public string sInvoiceNumber { get; set; }
        /// <summary>
        /// Hold data - Invoice Date
        /// </summary>
        public string sInvoiceDate { get; set; }
        /// <summary>
        /// Hold data - Total Cost
        /// </summary>
        public string sTotalCost { get; set; }
        /// <summary>
        /// Hold data - Items in Invoice
        /// </summary>
        public List<clsItem> sItems {  get; set; }


        public clsInvoice()
        {
            this.sInvoiceNumber = "";
            this.sInvoiceDate = "";
            this.sTotalCost = "";
            this.sItems = new List<clsItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <param name="items"></param>
        public clsInvoice(string invoiceNum, string invoiceDate, string totalCost, List<clsItem> items)
        {
            this.sInvoiceNumber = invoiceNum;
            this.sInvoiceDate = invoiceDate;
            this.sTotalCost = totalCost;
            this.sItems = items;
        }

        public clsInvoice(string invoiceNum, string invoiceDate)
        {
            this.sInvoiceNumber= invoiceNum;
            this.sInvoiceDate = invoiceDate;
            this.sTotalCost = "";
            this.sItems = new List<clsItem>();
        }

        public clsInvoice(string invoiceNum, DateOnly invoiceDate, string totalCost, List<clsItem> items)
        {
            this.sInvoiceNumber = invoiceNum;
            this.sInvoiceDate = invoiceDate.ToString();   
            this.sTotalCost = totalCost;
            this.sItems = items;
        }

        public clsInvoice(string invoiceNum, DateOnly invoiceDate)
        {
            this.sInvoiceNumber = invoiceNum;
            this.sInvoiceDate = invoiceDate.ToString();
            this.sTotalCost = "";
            this.sItems = new List<clsItem>();
        }

        public clsInvoice (clsInvoice invoice)
        {
            this.sInvoiceNumber = invoice.sInvoiceNumber;
            this.sInvoiceDate = invoice.sInvoiceDate;
            this.sTotalCost = invoice.sTotalCost;
            this.sItems = new List<clsItem>(invoice.sItems);
        }

        public override string ToString()
        {
            return this.sInvoiceNumber;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;

            clsInvoice invoice = (clsInvoice)obj;

            // First, compare the number of items
            if (this.sItems.Count != invoice.sItems.Count) return false;

            // Compare each item in sequence
            for (int i = 0; i < this.sItems.Count; i++)
            {
                if (!this.sItems[i].Equals(invoice.sItems[i]))
                {
                    return false;
                }
            }

            // Now compare the other properties
            return (this.sInvoiceNumber == invoice.sInvoiceNumber &&
                    this.sInvoiceDate == invoice.sInvoiceDate &&
                    this.sTotalCost == invoice.sTotalCost);
        }
    }
}
