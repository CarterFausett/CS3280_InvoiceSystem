using InvoiceSystem.Common;
using System;
using System.Collections.Generic;
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

namespace InvoiceSystem.Items
{
    /// <summary>
    /// Interaction logic for wndItems.xaml
    /// </summary>
    public partial class wndItems : Window
    {
        /// <summary>
        /// Determine which mode items window is in
        /// </summary>
        bool bAddStatus, bUpdateStatus, bDeleteStatus;
        /// <summary>
        /// Will be set to true when item has been successfully added/updated/deleted
        /// Main window uses to check if items need to be refreshed
        /// </summary>
        bool bItemChanged;
        /// <summary>
        /// Property that the Main Window has access to 
        /// determine if item has changed and needs to update
        /// invoices
        /// </summary>
        public bool BItemChanged { get => bItemChanged; set => bItemChanged = value; }

        /// <summary>
        /// Create a reference of clsItemsLogic
        /// </summary>
        clsItemsLogic ItemManager;

        public wndItems()
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            // Load in item data from database onto DataGrid
            RefreshGrid();
            //ItemDataGrid.ItemsSource = ItemManager.GetAllItems();
        }
        /// <summary>
        /// Method to update the DataGrid with items
        /// </summary>
        private void RefreshGrid()
        {
            try
            {
                // Create new instance of clsItemsLogic
                ItemManager = new clsItemsLogic();
                //get the list of all items in the database and bind them to the item combobox
                this.ItemDataGrid.ItemsSource = this.ItemManager.GetAllItems();
            }
            catch (Exception ex)
            {
                //if an exception is raised, put a blank list in there
                this.ItemDataGrid.ItemsSource = new List<clsItem>();
            }
        }
        /// <summary>
        /// This will handle any prep to add an item
        /// Set mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemAddBtn_Click(object sender, RoutedEventArgs e)
        { 
            try
            {
                // Clear all fields 
                ItemCodeTextbox.Text = "";
                ItemCodeTextbox.IsEnabled = true;
                ItemCostTextbox.Text = "";
                ItemCostTextbox.IsEnabled = true;
                ItemDescTextbox.Text = "";
                ItemDescTextbox.IsEnabled = true;
                InstructionLabel.Content = "To add a new item, enter a unique Item Code, Cost, and a Description:";
                ItemInvoiceTextbox.Visibility = Visibility.Hidden;
                // Set mode
                bAddStatus = true;
                bUpdateStatus = false;
                bDeleteStatus = false;

                // Enable save/cancel buttons
                SaveItemBtn.IsEnabled = true;
                CancelBtn.IsEnabled = true;

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// This will handle any prep to edit an item
        /// Make changes to description and cost ONLY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditItemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set instruction label
                InstructionLabel.Content = "Edit an existing item's Cost and Description: ";

                // Select an item before moving on
                if (ItemCodeTextbox.Text.Length > 0 && ItemCostTextbox.Text.Length > 0 && ItemDescTextbox.Text.Length > 0)
                {
                    // Set mode
                    bAddStatus = false;
                    bUpdateStatus = true;
                    bDeleteStatus = false;

                    // Disable Item Code field so that it cannot be edited + enable others
                    ItemInvoiceTextbox.Visibility = Visibility.Hidden;
                    ItemCodeTextbox.IsEnabled = false;
                    ItemCostTextbox.IsEnabled = true;
                    ItemDescTextbox.IsEnabled = true;
                    // Enable save/cancel buttons
                    SaveItemBtn.IsEnabled = true;
                    CancelBtn.IsEnabled = true;
                }
                else
                {
                    InstructionLabel.Content = "Select an Item to Edit: ";
                }

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// This will handle selected item deletes
        /// Will also set the bItemChanged to true for Main Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ItemCodeTextbox.Text.Length > 0 && ItemCostTextbox.Text.Length > 0 && ItemDescTextbox.Text.Length > 0)
                {
                    // set mode to delete
                    bAddStatus = false;
                    bUpdateStatus = false;
                    bDeleteStatus = true;

                    // Display a messageBox to get confirmation
                    string messageBoxText = "Are you sure you want to delete this Item?";
                    string caption = "Confirm Delete";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result;

                    result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                    // If yes, check that the item does not exist on any invoices 
                    if (result == MessageBoxResult.Yes)
                    {
                        // Check that there are no invoices that have this item
                        List<clsInvoice> invoiceList = ItemManager.CheckItemInvoice(ItemCodeTextbox.Text);

                        if (invoiceList.Count == 0)
                        {
                            // Set the bItemChanged to true
                            bItemChanged = true;

                            // UI changes
                            ItemInvoiceTextbox.Visibility = Visibility.Hidden;
                            ItemManager.DeleteItem(ItemCodeTextbox.Text);
                            InstructionLabel.Content = "Item successfully deleted.";
                            // Refresh DataGrid
                            RefreshGrid();
                        }
                        else
                        {
                            // Loop through invoice list for display: 
                            ItemInvoiceTextbox.Visibility = Visibility.Visible;
                            ItemInvoiceTextbox.Text = "Item cannot be deleted because there are invoices attached, review and try again:\n Item on Invoice #: \n";
                            CancelBtn.IsEnabled = true;
                            for (int i = 0; i < invoiceList.Count; i++)
                            {
                                ItemInvoiceTextbox.Text += invoiceList[i].sInvoiceNumber + "\n";
                            }
                        }
                    }
                    else if(result == MessageBoxResult.No)
                    {
                        // Cancel, no longer deleting
                        bAddStatus = false;
                        bUpdateStatus = false;
                        bDeleteStatus = false;
                    }
                }
                else
                {
                    InstructionLabel.Content = "Select an Item to delete: ";
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// This will save any updates or changes made to the items
        /// May also set the bItemChanged 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Handle any insert / update actions
                if (bAddStatus)
                {
                    if (ItemCodeTextbox.Text.Length > 0 && ItemCostTextbox.Text.Length > 0 && ItemDescTextbox.Text.Length > 0)
                    {
                        // Set bHasItemsChanged to true for the main window
                        bItemChanged = true;
                        InstructionLabel.Content = "";
                        // UI - Save field values in an Item Object
                        clsItem AddItem = new clsItem();
                        AddItem.sItemCode = ItemCodeTextbox.Text;
                        AddItem.sCost = ItemCostTextbox.Text;
                        AddItem.sDescription = ItemDescTextbox.Text;

                        // Insert the item on the DB
                        ItemManager.AddItem(AddItem);
                        // UI - update the DataGrid view
                        InstructionLabel.Content = "New Item Successfully Saved.";
                        // Refresh Grid with new changes
                        RefreshGrid();

                        // UI - Clear all fields 
                        ItemCodeTextbox.Text = "";
                        ItemCodeTextbox.IsEnabled = false;
                        ItemCostTextbox.Text = "";
                        ItemCostTextbox.IsEnabled = false;
                        ItemDescTextbox.Text = "";
                        ItemDescTextbox.IsEnabled = false;
                        ItemInvoiceTextbox.Visibility = Visibility.Hidden;
                        // Disable save/cancel buttons
                        SaveItemBtn.IsEnabled = false;
                        CancelBtn.IsEnabled = false;

                    }
                    else
                    {
                        InstructionLabel.Content = "One or more fields are blank!";
                    }

                }
                else if (bUpdateStatus)
                {
                    bItemChanged = true;
                    InstructionLabel.Content = "";
                    clsItem UpdateItem = new clsItem();
                    UpdateItem.sItemCode = ItemCodeTextbox.Text;
                    UpdateItem.sCost= ItemCostTextbox.Text;
                    UpdateItem.sDescription= ItemDescTextbox.Text;

                    // Update the item on the DB
                    ItemManager.UpdateItem(UpdateItem);
                    // UI - update the DataGrid View
                    InstructionLabel.Content = "Item Sucessfully Updated.";
                    RefreshGrid();


                }
                
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// This will cancel any updates/edits made to items
        /// Reset fields and modes
        /// May also set the bItemChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set bHasItemsChanged to false
                bItemChanged = false;
                // Reset mode
                bAddStatus = false;
                bUpdateStatus = false;
                bDeleteStatus = false;

                // Clear all fields 
                InstructionLabel.Content = "";
                ItemCodeTextbox.Text = "";
                ItemCodeTextbox.IsEnabled = false;
                ItemCostTextbox.Text = "";
                ItemCostTextbox.IsEnabled = false;
                ItemDescTextbox.Text = "";
                ItemDescTextbox.IsEnabled = false;
                ItemInvoiceTextbox.Visibility = Visibility.Hidden;

                // Disable save/cancel buttons
                SaveItemBtn.IsEnabled = false;
                CancelBtn.IsEnabled = false;


            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// Update the fields with the datagrid info
        /// Store the current item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Get the selected row's data + Store in an item object
                clsItem selectedItem = (clsItem)ItemDataGrid.SelectedItem;

                // Populate Field Details
                if (selectedItem != null)
                {
                    ItemCodeTextbox.Text = selectedItem.sItemCode;
                    ItemCostTextbox.Text = selectedItem.sCost;
                    ItemDescTextbox.Text = selectedItem.sDescription;
                }
                else
                {
                    ItemCodeTextbox.Text = "";
                    ItemCostTextbox.Text = "";
                    ItemDescTextbox.Text = "";
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
        
        /// <summary>
        /// Error Handling
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="sMessage"></param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

       
    }
}
