using InvoiceSystem.Common;
using InvoiceSystem.Items;
using InvoiceSystem.Search;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace InvoiceSystem.Main
{
    /// <summary>
    /// Interaction logic for wndMain.xaml
    /// </summary>
    public partial class wndMain : Window
    {
        /// <summary>
        /// A boolean representing whether or not a change to the currently open invoice has been made
        /// </summary>
        private bool bChangeMadeSinceLastSave;

        /// <summary>
        /// class variable for the main logic class
        /// </summary>
        private clsMainLogic logic;

        /// <summary>
        /// class variable containing the currently loaded invoice
        /// </summary>
        private clsInvoice loadedInvoice;

        /// <summary>
        /// class variable for keeping track of whether or not the form is in edit mode, true is in edit mode, false is not in edit mode
        /// </summary>
        private bool bEditMode;

        /// <summary>
        /// class variable that is set to true after the form has been initialized, stops methods from running when they have no need to run
        /// </summary>
        private bool init;

        /// <summary>
        /// Class variable for keeping track of whether or not the user selected "new invoice."
        /// </summary>
        private bool bNewInvoiceSelected;

        /// <summary>
        /// The index of the currently loaded invoice in the invoice combo box list.
        /// </summary>
        private int bLoadedIndex;

        /// <summary>
        /// wndMain constructor
        /// </summary>
        public wndMain()
        {
            try
            {
                InitializeComponent();
                this.init = false;
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                //upon init, no menus need populating
                this.logic = new clsMainLogic();
                this.bChangeMadeSinceLastSave = false; //when first loaded, nothing has been changed so there is nothing to save
                this.dpInvoiceDatePicker.SelectedDate = DateTime.Today;
                //System.Diagnostics.Debug.WriteLine("Loading invoice list...");
                this.PopulateInvoiceComboBox();
                //System.Diagnostics.Debug.WriteLine("Done!");
                //System.Diagnostics.Debug.WriteLine("Loading item list...");
                this.PopulateItemComboBox();
                //System.Diagnostics.Debug.WriteLine("Done!");

                //disable controls exclusively for loaded invoices

                this.bEditMode = false;
                this.ChangeEditMode();
                this.buttonEditInvoice.IsEnabled = false;
                this.bNewInvoiceSelected = false;
                this.init = true;
            }
            catch 
            {
                //if an exception is raised during init, close the program
                this.Close();
            }

        }

        /// <summary>
        /// Method for enabling/disabling controls based on whether or not the form is in edit mode.
        /// </summary>
        private void ChangeEditMode()
        {
            try
            {
                if (this.bEditMode)
                {
                    this.dpInvoiceDatePicker.IsEnabled = true;
                    this.cbItemList.IsEnabled = true;
                    this.bAddItem.IsEnabled = true;
                    this.cbLoadedItemList.IsEnabled = true;
                    this.bDeleteItem.IsEnabled = true;
                    this.buttonSaveInvoice.IsEnabled = true;
                    this.tbInvoiceNumber.IsEnabled = true;
                    this.tbTotalCost.IsEnabled = true;
                    this.tbItemCost.IsEnabled = true;
                    this.buttonEditInvoice.IsEnabled = false;
                }
                else
                {
                    this.dpInvoiceDatePicker.IsEnabled = false;
                    this.cbItemList.IsEnabled = false;
                    this.bAddItem.IsEnabled = false;
                    this.cbLoadedItemList.IsEnabled = false;
                    this.bDeleteItem.IsEnabled = false;
                    this.buttonSaveInvoice.IsEnabled = false;
                    this.tbInvoiceNumber.IsEnabled = false;
                    this.tbTotalCost.IsEnabled = false;
                    this.tbItemCost.IsEnabled = false;
                    this.buttonEditInvoice.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                //if at first you don't succeed...
                //try AGAIN.
                this.ChangeEditMode();
            }
        }

        /// <summary>
        /// Method for loading an entirely new invoice if the user selects the new invoice option
        /// </summary>
        private void LoadNewInvoice(Object sender, RoutedEventArgs e)
        {
            try
            {
                //clear the currently loaded invoice and its visible data
                this.cbInvoiceList.SelectedItem = null;
                this.cbItemList.SelectedItem = null;

                //change the loadedInvoice
                this.loadedInvoice = this.logic.BuildNewInvoice();
                //grab the necessary invoice information from the database and display it
                //loaded invoice will take the form of a clsInvoice object
                //the list of invoice items will take the form of a list of clsItem objects
                //this.PopulateItemComboBox();
                this.EnterEditMode(null, null);

                this.dpInvoiceDatePicker.SelectedDate = DateTime.Parse(this.loadedInvoice.sInvoiceDate);
                this.tbInvoiceNumber.Text = this.loadedInvoice.sInvoiceNumber;
                this.tbTotalCost.Text = "$" + this.loadedInvoice.sTotalCost;
                this.bNewInvoiceSelected = true;
                this.bChangeMadeSinceLastSave = false;
                this.bDeleteItem.IsEnabled = false;
                this.PopulateInvoiceItemListComboBox();
                this.PopulateInvoiceItemListDataGrid();
                this.ButtonNewInvoice.IsEnabled = false;
                //display all information
            }
            catch (Exception ex)
            {
                //load a blank invoice if an exception is raised
                this.loadedInvoice = new clsInvoice();
            }
        }

        /// <summary>
        /// Method for loading the invoice selected in the invoice combo box.
        /// </summary>
        /// <param name="sender">Object calling this method.</param>
        /// <param name="e">Additional arguments provided to this method.</param>
        private void LoadSelectedInvoice(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.cbInvoiceList.SelectedItem != null)
                {
                    this.loadedInvoice = new clsInvoice((clsInvoice)this.cbInvoiceList.SelectedItem);
                    this.bLoadedIndex = this.cbInvoiceList.Items.IndexOf(this.cbInvoiceList.SelectedItem);
                    this.bEditMode = false;
                    this.ChangeEditMode();
                    this.cbItemList.SelectedItem = null;
                    

                    this.dpInvoiceDatePicker.SelectedDate = DateTime.Parse(this.loadedInvoice.sInvoiceDate);
                    this.tbInvoiceNumber.Text = this.loadedInvoice.sInvoiceNumber;
                    this.tbTotalCost.Text = "$" + this.loadedInvoice.sTotalCost;

                    this.PopulateInvoiceItemListDataGrid();
                    this.PopulateInvoiceItemListComboBox();
                    this.bNewInvoiceSelected = false;
                    this.bChangeMadeSinceLastSave = false;
                    this.ButtonNewInvoice.IsEnabled = true;
                }
                else
                {
                    //do nothing
                }
            }
            catch (Exception ex)
            {
                this.loadedInvoice = new clsInvoice();
            }
        }

        /// <summary>
        /// Method for loading an existing invoice using a given invoice number
        /// </summary>
        /// <param name="newInvoiceId">The identification number of the desired invoice.</param>
        private void LoadInvoiceByID(int newInvoiceId)
        {
            try
            {
                //search for the desired invoice by first building a clsInvoice object with the ID
                clsInvoice potentialInvoice = this.logic.BuildExistingInvoice(newInvoiceId);
                foreach (clsInvoice invoice in this.cbInvoiceList.ItemsSource)
                        {
                            //if the invoice is the same as the desired invoice built from the ID, we've found a match
                            if (invoice.Equals(potentialInvoice))
                            {
                                this.cbInvoiceList.SelectedItem = invoice; //changing the selected item will invoke a LoadSelectedInvoice call (I hope)
                                //this.LoadSelectedInvoice(null, null); //uncomment this line if everything breaks as soon as the above line is executed
                                return;
                            }
                        }
            }
            catch (Exception ex)
            {
                //load a blank fallback invoice if an exception is thrown
                this.LoadNewInvoice(new Object(), new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Method for populating the item combo box with a list of all items found in the database.
        /// </summary>
        private void PopulateItemComboBox()
        {
            try
            {
                //get the list of all items in the database and bind them to the item combobox
                this.cbItemList.ItemsSource = this.logic.GetDatabaseItems();
            }
            catch (Exception ex)
            {
                //if an exception is raised, bind a blank list to the item list combo box
                this.cbItemList.ItemsSource = new List<clsItem>();
            }
        }

        /// <summary>
        /// Method for binding the list of all invoices in the database to the invoice list combo box
        /// </summary>
        private void PopulateInvoiceComboBox()
        {
            try
            {
                //get the list of all invoices in the database and bind them to the invoice combobox
                this.cbInvoiceList.ItemsSource = this.logic.GetAllInvoices();
            }
            catch (Exception ex)
            {
                //if an exception is thrown, bind a blank list to the combo box
                this.cbInvoiceList.ItemsSource = new List<clsInvoice>();
            }
        }

        /// <summary>
        /// Method for populating the invoice item list data grid with items from the currently selected invoice.
        /// </summary>
        private void PopulateInvoiceItemListDataGrid()
        {
            try
            {
                dgInvoiceItems.AutoGenerateColumns = true;
                dgInvoiceItems.CanUserAddRows = false;
                //generate a table with all of the necessary information for the item list
                DataTable dt = this.logic.GenerateInvoiceItemTable(this.loadedInvoice);

                //this.dgInvoiceItems.DataContext = dt.DefaultView;//this.logic.GenerateInvoiceItemTable(this.loadedInvoice); no idea why people on stack overflow said this would work
                this.dgInvoiceItems.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                this.dgInvoiceItems.ItemsSource = new DataTable().DefaultView; //if an exception is raised, make the datagrid blank
            }
        }


        /// <summary>
        /// Method for initiating saving the currently loaded invoice.
        /// </summary>
        private void SaveInvoice(object sender, RoutedEventArgs e)
        {
            try
            {
                //push the list of changes to the database when save is selected
                this.IsEnabled = false;
                this.logic.SaveInvoice(ref this.loadedInvoice);
                this.bChangeMadeSinceLastSave = false;
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //if an exception is raised, make sure the window is enabled and abort the save
                this.IsEnabled = true;
            }
        }

        /// <summary>
        /// Method for opening the items window and checking if there were any changes made in the items window.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments provided to this method.</param>
        private void OpenItems(object sender, RoutedEventArgs e)
        {
            try
            {
                //prompt user to save the currently loaded invoice if changes have been made
                if (this.bChangeMadeSinceLastSave)
                {
                    this.IsEnabled = false;
                    MessageBoxResult result = MessageBox.Show("Changes have been made since this invoice was last saved. Would you like to save now?", "Save Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.SaveInvoice(null, null);
                    }
                    this.IsEnabled = true;
                }

                //disable the main window, open items and pass any necessary data to it
                this.IsEnabled = false;
                wndItems itemWindow = new wndItems();
                itemWindow.ShowDialog();
                this.LoadSelectedInvoice(null, null);
                

                //if HasItemChanged is true, repopulate the item list
                if (itemWindow.BItemChanged)
                {
                    this.cbItemList.SelectedItem = null; //set selection to null to reset the selection
                    this.PopulateItemComboBox();
                }
                else
                {

                }
                this.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //repopulate the item combo box if something goes wrong
                this.PopulateItemComboBox();
                this.IsEnabled = true;
            }
        }

        /// <summary>
        /// Method for opening the search window and checking the newly selected invoiceNum from the search window after it closes.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments provided to this method.</param>
        private void OpenSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.bChangeMadeSinceLastSave)
                {
                    this.IsEnabled = false;
                    MessageBoxResult result = MessageBox.Show("Changes have been made since this invoice was last saved. Would you like to save now?", "Save Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.SaveInvoice(null, null);
                    }
                    this.IsEnabled = true;
                }

                // Disable main window and show search window
                this.IsEnabled = false;
                wndSearch searchWindow = new wndSearch();
                bool? dialogResult = searchWindow.ShowDialog();
                this.IsEnabled = true;

                if (dialogResult == true) //if an option is selected by the user in the search window
                {
                    int selectedInvoiceId = searchWindow.SelectedInvoiceID;
                    if (selectedInvoiceId != -1)
                    {
                        LoadInvoiceByID(selectedInvoiceId);
                        PopulateItemComboBox();
                    }
                }

                //if selected invoice ID has changed, change iSelectedInvoiceId and then call LoadInvoice() and PopulateItemList()
            }
            catch (Exception ex) //if an exception is thrown, reload the currently loaded invoice and repopulate the item combo box, make sure this window is enabled
            {
                this.LoadSelectedInvoice(null, null);
                this.PopulateItemComboBox();
                this.IsEnabled = true;
            }
        }

        /// <summary>
        /// Method for updating the currently loaded invoice's invoice date. Sets the date to the selected date in the dpInvoiceDatePicker Control.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments passed to this method.</param>
        private void UpdateLoadedInvoiceDate(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.init)
                {
                    DateTime newDate = this.dpInvoiceDatePicker.SelectedDate ?? DateTime.Today; //set the new invoice date to the selected date, or if the selected date is null, return today's date

                    this.loadedInvoice.sInvoiceDate = DateOnly.FromDateTime(newDate).ToString(); //set the loaded invoice's date to the newly determined date

                    this.bChangeMadeSinceLastSave = true;
                    this.ButtonNewInvoice.IsEnabled = true;
                }

            }
            catch (Exception ex) 
            {
                //if an exception is raised, do not change the invoice date
                return;
            }
        }

        /// <summary>
        /// Method for updating the displayed cost of the item selected in the item combo box.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments provided to this method.</param>
        private void UpdateDisplayedItemCost(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.cbItemList.SelectedIndex != -1)
                {
                    this.tbItemCost.Text = "$" + ((clsItem)this.cbItemList.SelectedItem).sCost;
                    return;
                }
                this.tbItemCost.Text = "";
            }
            catch (Exception ex)
            {
                this.tbItemCost.Text = ""; //likely to catch a NullReferenceException, ignore and set a blank cost
            }
        }

        /// <summary>
        /// Method for adding an item to the currently loaded invoice's associated item list.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments passed to this method.</param>
        private void AddItemToLoadedInvoiceItemList(object sender, RoutedEventArgs e)
        {
            try
            {
                this.loadedInvoice.sItems.Add(new clsItem((clsItem)this.cbItemList.SelectedItem)); //only clsItems will be found in the itemlist combo box
                this.PopulateInvoiceItemListDataGrid();
                this.PopulateInvoiceItemListComboBox();
                decimal decTotalCost = decimal.Parse(this.loadedInvoice.sTotalCost);
                decimal decItemCost = decimal.Parse(((clsItem)this.cbItemList.SelectedItem).sCost);

                //I do not like this solution but it works
                this.loadedInvoice.sTotalCost = (decTotalCost + decItemCost).ToString();
                this.tbTotalCost.Text = "$" + this.loadedInvoice.sTotalCost;
                this.bChangeMadeSinceLastSave = true;
                this.ButtonNewInvoice.IsEnabled = true;
                if (this.loadedInvoice.sItems.Count > 0) this.bDeleteItem.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //if an exception is thrown, repopulate the item lists and do not bother updating the cost
                this.PopulateInvoiceItemListDataGrid();
                this.PopulateInvoiceItemListComboBox();
            }
        }

        /// <summary>
        /// Method for deleting an item from the currently loaded invoice.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments passed to this method.</param>
        private void DeleteItemFromLoadedInvoiceList(object sender, RoutedEventArgs e)
        {
            try
            {
                int lineItemNumberToDelete = this.cbLoadedItemList.SelectedIndex;

                decimal decDeletedItemCost = decimal.Parse(this.loadedInvoice.sItems[lineItemNumberToDelete].sCost);
                decimal decTotalCost = decimal.Parse(this.loadedInvoice.sTotalCost);
                this.loadedInvoice.sTotalCost = (decTotalCost - decDeletedItemCost).ToString();
                this.tbTotalCost.Text = "$" + this.loadedInvoice.sTotalCost;

                this.loadedInvoice.sItems = clsMainLogic.RemoveAndShiftItems(lineItemNumberToDelete, this.loadedInvoice.sItems);

                this.PopulateInvoiceItemListDataGrid();
                this.PopulateInvoiceItemListComboBox();
                this.bChangeMadeSinceLastSave = true;
                this.ButtonNewInvoice.IsEnabled = true;
                if (this.loadedInvoice.sItems.Count < 1) this.bDeleteItem.IsEnabled = false;
            }
            catch (Exception ex)
            {
                //similar to adding an item, repopulate the lists but do not edit totals
                this.PopulateInvoiceItemListDataGrid();
                this.PopulateInvoiceItemListComboBox();
            }
        }

        /// <summary>
        /// Method for populating the InvoiceItemList combo box with the currently loaded invoice's items
        /// </summary>
        private void PopulateInvoiceItemListComboBox()
        {
            try
            {
                //I am not proud of this solution but I did not see another way out of this at this time
                this.cbLoadedItemList.Items.Clear();

                int lineNumber = 1;
                foreach (clsItem item in this.loadedInvoice.sItems)
                {
                    this.cbLoadedItemList.Items.Add(item.ToString(lineNumber));
                    lineNumber++;
                }
            }
            catch (Exception ex)
            {
                this.cbLoadedItemList.Items.Clear(); //clear the combobox of items and deal with it
            }
        }

        /// <summary>
        /// Method that initiates loading of the loaded invoice.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments passed to this method.</param>
        private void SaveLoadedInvoice(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                //System.Diagnostics.Debug.WriteLine("Saving invoice...");
                this.logic.SaveInvoice(ref this.loadedInvoice); //hope this works
                //System.Diagnostics.Debug.WriteLine("Done!");
                this.labelDateError.Visibility = Visibility.Collapsed;

                //get the index of the loaded invoice
                //int loadedIndex = this.cbInvoiceList.SelectedIndex;
                //System.Diagnostics.Debug.WriteLine(loadedIndex.ToString());
                //reload the invoice list
                //this.PopulateInvoiceComboBox();
                //System.Diagnostics.Debug.WriteLine(this.bNewInvoiceSelected.ToString());
                if (this.bNewInvoiceSelected)
                {
                    this.PopulateInvoiceComboBox();
                    this.cbInvoiceList.SelectedIndex = cbInvoiceList.Items.Count - 1; //load the last invoice in the list
                    this.bNewInvoiceSelected = false;
                }
                else
                {
                    List<clsInvoice> items = this.cbInvoiceList.ItemsSource as List<clsInvoice>;
                    items[this.bLoadedIndex] = this.loadedInvoice;
                    this.cbInvoiceList.ItemsSource = items;
                    this.cbInvoiceList.SelectedItem = this.loadedInvoice;
                }
                this.bChangeMadeSinceLastSave = false;
                this.ButtonNewInvoice.IsEnabled = true;
                this.IsEnabled = true;
                //disable edit mode
                this.bEditMode = false;
                this.ChangeEditMode();
            }
            catch (Exception ex)
            {
                //abort save if an exception is thrown, make sure edit mode is disabled and this window is enabled
                this.bEditMode = false;
                this.ChangeEditMode();
                this.IsEnabled = true;
            }
        }

        /// <summary>
        /// Method for entering edit mode. Sets bEditMode to true and calls ChangeEditMode().
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional parameters passed to this method.</param>
        private void EnterEditMode(object sender, RoutedEventArgs e)
        {
            try
            {
                this.bEditMode = true;
                this.ChangeEditMode();
            }
            catch (Exception ex)
            {
                //if an exception is thrown, fallback to non edit mode
                this.bEditMode = false;
                this.ChangeEditMode();
            }
        }

        /// <summary>
        /// Method for displaying an error message when the value entered into the date picker cannot be parsed.
        /// </summary>
        /// <param name="sender">The object calling this method.</param>
        /// <param name="e">Additional arguments passed to this method.</param>
        private void DisplayErrorMessage(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            try
            {
                //this.buttonSaveInvoice.IsEnabled = false;
                if (sender == this.dpInvoiceDatePicker)
                {
                    this.labelDateError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                //if an exception is thrown, return and the date picker should not allow the input
                return;
            }
        }
    }
}
