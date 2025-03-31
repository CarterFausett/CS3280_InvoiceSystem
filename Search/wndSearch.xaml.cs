using InvoiceSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InvoiceSystem.Search
{

    public partial class wndSearch : Window
    {
        /// <summary>
        /// int type which holds the selected invoice ID if an invoice gets selected.
        /// If no invoice is selected the value is 0.
        /// </summary>
        private int sSelectInvoiceID = 0;

        /// <summary>
        /// int type which allows the main window to access the selected invoice ID.
        /// The main window will check this property to retrieve the invoice ID after the search window closes.
        /// </summary>
        public int SelectedInvoiceID { get; private set; } = -1;


        /// <summary>
        /// Holds the selected invoice and used in the filter
        /// </summary>
        private int? selectedInvoiceNumber = null;

        /// <summary>
        /// Holds the current selected date of the invoice and used in the filter
        /// </summary>
        private DateTime? selectedInvoiceDate = null;

        /// <summary>
        /// Holds the selected invoice total cost and used in the filter
        /// </summary>
        private decimal? selectedTotalCost = null;
       

        /// <summary>
        /// Instance of clsSearchLogic, used for getting and filtering invoices
        /// </summary>
        private clsSearchLogic searchLogic;

        public wndSearch()
        {
            InitializeComponent();
            searchLogic = new clsSearchLogic();
            PopulateComboBoxes();
            LoadAllInvoices();
        }

        /// <summary>
        /// Handles the selection of an invoice when the user click the select button. 
        /// sSelectInvoiceID will be updated with that InvoiceID, which will let it be accessed by the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridResults.SelectedItem is clsInvoice selectedInvoice)
            {
                if (int.TryParse(selectedInvoice.sInvoiceNumber, out int invoiceID))
                {
                    SelectedInvoiceID = invoiceID;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid invoice number format.");
                }
            }
            else
            {
                MessageBox.Show("Please select an invoice before clicking Select.");
            }
        }

        /// <summary>
        /// Button when it is clicked returns you to the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchCancelButton_Click(object sender, RoutedEventArgs e)
        {

            sSelectInvoiceID = 0;
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Will filter the invoices based on the Invoice Number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboOption1_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ComboOption1.SelectedItem != null)
            {
                selectedInvoiceNumber = (int)ComboOption1.SelectedItem;
            }
            else
            {
                selectedInvoiceNumber = null;
            }
            Filter();
        }

        /// <summary>
        /// Will filter the invoices based on the invoice date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboOption2_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ComboOption2.SelectedItem != null)
            {
                selectedInvoiceDate = DateTime.Parse(ComboOption2.SelectedItem.ToString());
            }
            else
            {
                selectedInvoiceDate = null;
            }
            Filter();
        }

        /// <summary>
        /// Will filter the invoice based on the total cost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboOption3_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ComboOption3.SelectedItem != null)
            {
                selectedTotalCost = decimal.Parse(ComboOption3.SelectedItem.ToString(), System.Globalization.NumberStyles.Currency);
            }
            else
            {
                selectedTotalCost = null;
            }
            Filter();
        }


        /// <summary>
        /// Fill the Combo boxes with the options to select from
        /// </summary>
        private void PopulateComboBoxes()
        {

            ComboOption1.Items.Clear();
            var invoiceNumbers = searchLogic.GetDistinctInvoiceNumbers();
            foreach (var num in invoiceNumbers)
            {
                ComboOption1.Items.Add(num);
            }
            ComboOption1.SelectedIndex = -1;


            ComboOption2.Items.Clear();
            var invoiceDates = searchLogic.GetDistinctInvoiceDates();
            foreach (var date in invoiceDates)
            {
                ComboOption2.Items.Add(date.ToShortDateString());
            }
            ComboOption2.SelectedIndex = -1;

            ComboOption3.Items.Clear();
            var totalCosts = searchLogic.GetDistinctTotalCosts();
            foreach (var cost in totalCosts)
            {
                ComboOption3.Items.Add(cost.ToString("C"));
            }
            ComboOption3.SelectedIndex = -1;
        }



        /// <summary>
        /// Loads all invoices into the data grid
        /// </summary>
        private void LoadAllInvoices()
        {
            try
            {
                var invoices = searchLogic.GetAllInvoices();
                if (invoices != null && invoices.Count > 0)
                {
                    DataGridResults.ItemsSource = invoices;
                }
                else
                {
                    MessageBox.Show("No invoices found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoices: " + ex.Message);
            }
        }



        /// <summary>
        /// Filters the displayed invoices in the data grid
        /// </summary>
        private void Filter()
        {
            try
            {
                var filteredInvoices = searchLogic.GetFilteredInvoices(selectedInvoiceNumber, selectedInvoiceDate, selectedTotalCost);
                DataGridResults.ItemsSource = filteredInvoices;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message);
            }
        }

        /// <summary>
        /// Resets the data grid to its initial state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboOption1.SelectedIndex = -1;
                ComboOption2.SelectedIndex = -1;
                ComboOption3.SelectedIndex = -1;

                selectedInvoiceNumber = null;
                selectedInvoiceDate = null;
                selectedTotalCost = null;

                LoadAllInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing selection: " + ex.Message);
            }
        }

        /// <summary>
        /// Handles when the window is loaded and refreshes the combo boxes and loads all invoices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PopulateComboBoxes();
                LoadAllInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data on window open: " + ex.Message);
            }
        }
        // [datatype] sSelectInvoiceID
        // This can hold invoiceID if selected, zero if not, datatype up to you

        // [datatype] SelectedInvoiceID
        // This is the property that the main window can access
        // to get the invoiceID - datatype up to you
    }
}
