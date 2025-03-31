using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSystem.Common
{
    class clsItem
    {
        /// <summary>
        /// Hold data - Item Code
        /// </summary>
        public string sItemCode {  get; set; }
        /// <summary>
        /// Hold data - Item Description
        /// </summary>
        public string sDescription { get; set; }
        /// <summary>
        /// Hold data - Item Cost
        /// </summary>
        public string sCost { get; set; }

        public clsItem() 
        {

        }

        public clsItem(string itemCode, string itemDesc, string itemCost)
        {
            this.sItemCode = itemCode;
            this.sDescription = itemDesc;
            this.sCost = itemCost;
        }

        public clsItem(clsItem existingItem)
        {
            this.sItemCode = existingItem.sItemCode;
            this.sDescription = existingItem.sDescription;
            this.sCost = existingItem.sCost;
        }

        public override string ToString()
        {
            return sItemCode + " " + sDescription;
        }

        public string ToString(int lineNum)
        {
            return lineNum.ToString() + " " + sItemCode + " " + sDescription;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is clsItem)) return false;

            clsItem item = (clsItem)obj;

            if (this.sItemCode == item.sItemCode && this.sDescription == item.sDescription && this.sCost == item.sCost) 
                return true;


            return false;
        }

    }

}
