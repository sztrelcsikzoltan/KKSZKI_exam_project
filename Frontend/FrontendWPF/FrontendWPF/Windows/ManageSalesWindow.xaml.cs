using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FrontendWPF.Classes;
using Microsoft.Win32;

namespace FrontendWPF.Windows
{
    public partial class ManageSalesWindow : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<StockService.SalePurchase> dbSalesList { get; set; }

        System.Collections.IList selectedItems;
        List<Classes.SalePurchase> filterSalesList { get; set; }
        List<StockService.SalePurchase> filteredSalesList { get; set; }
        List<StockService.SalePurchase> selectedSalesList { get; set; }
        List<StockService.Product> dbProductsList { get; set; }
        List<UserService.User> dbUsersList { get; set; }
        List<StockService.SalePurchase> importList { get; set; }
        List<LocationService.Store> dbLocationsList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private List<SalePurchase> salesList { get; set; }
        private int[] fieldsEntered = new int[6]; // (product) Name, Quantity, TotalPrice, Date, Location, User(name)
        ScrollViewer scrollViewer;
        string input = "";
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;
        string opId = "=";
        string opQuantity = "=";
        string opTotalPrice = "=";
        string opDate = "=";
        string lastProduct = "";
        int? lastQuantity = null;
        int? lastTotalPrice = null;
        DateTime? lastDate = null;
        string lastLocation = "";
        string lastUsername = "";
        bool pickStartDate = false;

        DateTime startDate = DateTime.Now.Date.AddDays(-30); // set an initial limit of 29 days
        DateTime endDate = DateTime.Now; //

        // public double columnFontSize { get; set; }

        public ManageSalesWindow()
        {
            InitializeComponent();

            if (Shared.layout != "")
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                if (Shared.layout.Contains("Products"))
                {
                    int overlay = Shared.layout.Contains("Overlay") ? 250 : 0;
                    Window productsWindow = Application.Current.Windows.OfType<Window>().Where(w => w.Title == "Manage products Window").FirstOrDefault();

                    window.Left = productsWindow.Width - overlay;
                    window.Top = 0;
                    window.Width = Shared.screenWidth - productsWindow.Width + overlay;
                    window.Height = 1 * Shared.screenHeight;
                }
                else if (Shared.layout.Contains("PurchasesSales"))
                {
                    int overlay = Shared.layout.Contains("Overlay") ? 250 : 0;
                    Window purchasesWindow = Application.Current.Windows.OfType<Window>().Where(w => w.Title == "Manage purchases Window").FirstOrDefault();

                    window.Left = purchasesWindow.Width - overlay;
                    window.Top = 0;
                    window.Width = Shared.screenWidth - purchasesWindow.Width + overlay;
                    window.Height = 1 * Shared.screenHeight;
                }
            }
            ReloadData();
        }

        private void Button_ReloadData_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            edit_mode = "read";
            dataGrid1.IsReadOnly = true;
            dataGrid1.CanUserSortColumns = true;
            dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
            dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
            TextBlock_message.Foreground = Brushes.White;

            // query sales from database
            dbSalesList = SalePurchase.GetSalesPurchases(type: "sale", id: "", product: "", qOver: "", qUnder: "", priceOver: "", priceUnder: "", before: endDate.ToString(), after: startDate.ToString(), location: "", user: "", limit: "");
            if (dbSalesList == null) { IsEnabled = false; Close(); return; } // stop on any error
            TextBlock_message.Text = $"{dbSalesList.Count} sales loaded.";


            dataGrid1.ItemsSource = dbSalesList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            filterSalesList = new List<Classes.SalePurchase>();

            Dispatcher.InvokeAsync(() =>
            {
                double stretch = Math.Max((borderLeft.ActualWidth - 10 - 111) / (550 - 10 - 43), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
                dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                dataGrid0.Width = dataGrid1.Width;
                dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

                stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Heigth

                // stretch columns to dataGrid1 width
                for (int i = 0; i < dataGrid1.Columns.Count; i++)
                {
                    dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 0 || i == 2 ? 0.5 : 1) + 1); // resize Id and Quantity row only by 50%
                    dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                }
                dataGrid1.FontSize = 14 * Math.Min(stretch, 1.0663); // reset font size to initial stretch value on large window width
                // dataGrid1.Columns[2].Header = stretch < 1.18 ? "Quantity" : "Quant.";
                dataGrid1.Items.Refresh();
                ScrollDown();
                selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
            }, DispatcherPriority.Loaded);
            EnableButtons();

            // create/reset sale_filter item and add it to filter dataGrid0
            sale_filter = new Classes.SalePurchase()
            {
                Id = "",
                Product = "", // Name of product
                Quantity = "",
                TotalPrice = "",
                Date = "",
                Location = "",
                Username = ""
            };
            filterSalesList.Clear();
            filterSalesList.Add(sale_filter);
            dataGrid0.ItemsSource = null; // to avoid IsEditingItem error
            dataGrid0.ItemsSource = filterSalesList;
            dataGrid0.Items.Refresh();

            SetUserAccess();
        }

        // https://stackoverflow.com/questions/16956251/sort-a-wpf-datagrid-programmatically
        private static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;
        }

        private void dataGrid1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedItems = dataGrid1.SelectedItems;

            // in update mode  update selectedcells and sale_edited (when SelecionUnit is Cell)
            // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
            if (edit_mode == "update")
            {
                // https://stackoverflow.com/questions/4714325/wpf-datagrid-selectionchanged-event-isnt-raised-when-selectionunit-cell
                IList<DataGridCellInfo> selectedcells = e.AddedCells;
                if (selectedcells.Count > 0) // ignore new selection when button is pressed and selection becomes 0; 
                {
                    sale_edited = (StockService.SalePurchase)selectedcells[0].Item;
                    sale_edited0 = sale_edited;
                }
            }
        }

        private void Button_DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode == "update")
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridCellInfo currentCell = dataGrid1.CurrentCell;

                dataGrid1.SelectedItems.Add(sale_edited); // this triggers SelectionChanged and sets new selectedItems
            }

            if (selectedItems.Count > 0)
            {
                selectedSalesList = new List<StockService.SalePurchase>();
                foreach (StockService.SalePurchase sale in selectedItems)
                {
                    selectedSalesList.Add(sale);
                }
                dataGrid1.ItemsSource = selectedSalesList;

                // waits to render dataGrid1 and sets row background color to Salmon 
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    for (int i = 0; i < selectedSalesList.Count; i++)
                    {
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }

                    int selectedSales = selectedSalesList.Count;
                    string deleteMessage = selectedSales == 1 ? "Are you sure to delete the selected sale?" : $"Are you sure to delete the selected {selectedSales} sales?";

                    TextBlock_message.Text = selectedSales == 1 ? "Delete sale?" : $"Delete {selectedSales} sales?";
                    TextBlock_message.Foreground = Brushes.Salmon;
                    MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = selectedSales - 1; i >= 0; i--)
                        {
                            try
                            {
                                // DELETE sale(s) from database
                                deleteMessage = stockClient.RemoveSalePurchase(Shared.uid, type: "sale", id: selectedSalesList[i].Id.ToString(), location: selectedSalesList[i].Location.ToString());
                                if (deleteMessage == "Sale(s)/purchase(s) successfully removed!")
                                {
                                    sale_edited = selectedSalesList[i]; // required to write the log
                                    Log("delete"); // write log to file
                                    dbSalesList.Remove(selectedSalesList[i]); // remove sale also from dbSalesList
                                    selectedSalesList.RemoveAt(i);
                                }
                                else if (deleteMessage == "Unauthorized user!")
                                {
                                    Shared.Logout(); // stop on unauthorized user
                                    IsEnabled = false;
                                    Close();
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show(deleteMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("An error occurred, with the following details:\n" + ex.Message, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        if (selectedSalesList.Count == 0)
                        {
                            deleteMessage = selectedSales == 1 ? "The sale has been deleted." : "The sales have been deleted.";
                            TextBlock_message.Text = selectedSales == 1 ? "Sale deleted." : "Sales deleted.";
                        }
                        else
                        {
                            deleteMessage = selectedSalesList.Count == 1 ? "The sale shown in the table could not be deleted, as reported in the error message." : "The sales shown in the table could not be deleted, as reported in the error message.";
                        }
                        // list the sales that could not be deleted (empty if all deleted)
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = selectedSalesList;

                        checkBox_fadeInOut.IsChecked = false;
                        checkBox_fadeInOut.IsChecked = true; // show gifImage
                        gifImage.StartAnimation();
                        MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // dataGrid1.Focus();
                    dataGrid1.ItemsSource = dbSalesList;
                    // for some reason the sorting gets improper, so sort again by Id
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

                    ScrollDown();
                    TextBlock_message.Text = "Select an option.";
                    TextBlock_message.Foreground = Brushes.White;
                },
                DispatcherPriority.Loaded); // just a bit lower priority than Render (Loaded = 6, Render = 7)
            }
            else
            {
                MessageBox.Show("Nothing is selected. Please select at least one sale. ", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dataGrid1.CanUserSortColumns = true;
        }

        private void Button_UpdateSale_Click(object sender, RoutedEventArgs e)
        {
            UpdateSale();
        }

        private void UpdateSale()
        {
            if (edit_mode != "update") // if not in update mode, switch to update mode
            {
                if (edit_mode == "insert") // remove incomplete added user 
                {
                    dbSalesList.Remove(sale_edited);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbProductsList;
                    EnableButtons();
                }
                if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
                {
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                }
                dataGrid1.CanUserSortColumns = false;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                dataGrid1.SelectionMode = DataGridSelectionMode.Single;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.Cell;
                TextBlock_message.Text = "Update sale.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();

                edit_mode = "update";
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
            }
        }

        private void Button_AddSale_Click(object sender, RoutedEventArgs e)
        {
            AddSale();
        }

        private void AddSale()
        {
            if (edit_mode != "insert") // if not in insert mode, switch to insert mode
            {
                Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

                // in db select last sale with highest Id
                int? highestId = dbSalesList.Count > 0 ? dbSalesList.Max(u => u.Id) : 0;
                sale_edited = new StockService.SalePurchase() // create new sale with suggested values
                {
                    Id = highestId + 1,
                    Product = lastProduct != "" ? lastProduct : "",
                    Quantity = lastQuantity != null ? lastQuantity : 1,
                    TotalPrice = lastTotalPrice != null ? lastTotalPrice : 0,
                    Date = lastDate != null ? lastDate : DateTime.Now,
                    Location = lastLocation != "" ? lastLocation : Shared.loggedInUser.Location,
                    Username = lastUsername != "" ? lastUsername : Shared.loggedInUser.Username
                };
            
                sale_edited0 = sale_edited;

                dbSalesList.Add(sale_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbSalesList;

                if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
                {
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                }
                dataGrid1.CanUserSortColumns = false;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the sale to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    // style the id cell of the new sale
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }, DispatcherPriority.Background); // Background to avoid row = null error

                edit_mode = "insert";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                TextBlock_message.Text = "Add sale.";
                TextBlock_message.Foreground = Brushes.White;

                Button_AddSale.IsEnabled = false;
                Button_DeleteSale.IsEnabled = false;
                Button_Filter.IsEnabled = false;
                Button_Export.IsEnabled = false;
                Button_Import.IsEnabled = false;
                Button_LogWindow.IsEnabled = false;
            }
        }

        DataGridRow row;
        DataGridColumn column;
        DataGridCell cell;
        TextBox textBox;
        string old_value;
        string new_value = "";
        int row_index;
        int column_index;
        int filterc_index;
        string changed_property_name;
        StockService.SalePurchase sale_edited, sale_edited0;
        Classes.SalePurchase sale_filter;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (CellEditEnding_setup(e) == null) { return; } // setup rules: stop on null

                string stopMessage = CellEditEnding_checkInput(); // check data correctness
                if (stopMessage == "stop") { return; } // stop on database error
                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value
                    // cell.Content = old_value;

                    Dispatcher.InvokeAsync(() => 
                    {
                        // select edited row/cell if user selected another row/cell
                        SelectEditedCell();

                        // remove event handler from wrong new cell (BegindEdit can also be removed if needed)
                        (sender as DataGrid).CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);

                        // select empty cell (if user eventually selected another one
                        Button_AddSale.Focus();

                        SelectTextBox();

                        // restore event handler
                        (sender as DataGrid).CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                    }, DispatcherPriority.Loaded);
                    return;
                }
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Quantity, TotalPrice, Date, Location and Username can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index - 1] == 1 || edit_mode == "update")) // && column_index < 5
                {
                    CellEditEnding_nextCell_update();
                    return;
                }

                Dispatcher.InvokeAsync(() => 
                {
                    SelectEditedCell(); // select edited row/cell if user selected another row/cell after data entry
                }, DispatcherPriority.Loaded);

                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                    Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }

                bool ProductChanged = changed_property_name == "Product" && sale_edited.Product != "" && sale_edited.Product != new_value ? true : false;
                bool QuantityChanged = changed_property_name == "Quantity" && sale_edited.Quantity != null && sale_edited.Quantity != Int32.Parse(new_value) ? true : false;

                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index == 1 || column_index == 5 || column_index == 6) // // update string-type fields with new value ( (Product) Name, Location, Username )
                {
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, new_value);
                }
                else if (column_index == 4) // // update Date fields with new value
                {
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, DateTime.Parse(new_value));
                }
                else // update int?-type fields with new value (Quantity, TotalPrice)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, Convert.ToInt32(new_value));
                }

                // if Name or Quantity is entered AND purchase_edited.TotalPrice is 0, OR Product changed OR Quantity changed, calculate and enter suggested TotalPrice
                if ((sale_edited.Product != "" && sale_edited.Quantity != null && sale_edited.TotalPrice == 0) || ProductChanged || QuantityChanged)
                {
                    dbProductsList = Product.GetProducts("", sale_edited.Product, "", "", "", "", "");
                    if (dbProductsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                    if (dbProductsList.Count == 1)
                    {
                        int? totalPrice = sale_edited.Quantity * dbProductsList[0].BuyUnitPrice;
                        sale_edited.TotalPrice = totalPrice;
                    }
                }

                // check if all properties are entered, then insert into database
                if (fieldsEntered.All(n => n == 1) || edit_mode == "update") // if all fields have been updated OR update mode for one field
                {
                    string hostMessage = "";
                    try
                    {
                        if (edit_mode == "insert")
                        {
                            // ADD into database
                            hostMessage = stockClient.AddSalePurchase(Shared.uid, type: "sale", sale_edited.Product, sale_edited.Quantity.ToString(), sale_edited.Location, sale_edited.TotalPrice.ToString(), sale_edited.Date.ToString());
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage.Contains("FOREIGN KEY (`productId`)"))
                            {
                                MessageBox.Show($"The product does not exist in the database. Please check product name.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            else if (hostMessage != "Sale/Purchase successfully added!")
                            {
                                MessageBox.Show(hostMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                sale_edited = sale_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            hostMessage = stockClient.UpdateSalePurchase(Shared.uid, sale_edited.Id.ToString(), type: "sale", sale_edited.Product, sale_edited.Quantity.ToString(), sale_edited.TotalPrice.ToString(), sale_edited.Date.ToString(), sale_edited.Location, sale_edited.Username);
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "Sale/Purchase successfully updated!")
                            {
                                MessageBox.Show(hostMessage + " Field was not updated.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                sale_edited = sale_edited0;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured, with the following details:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (edit_mode == "insert")
                    {
                        lastProduct = sale_edited.Product; // save last data to suggest them for next record
                        lastQuantity = sale_edited.Quantity;
                        lastTotalPrice = sale_edited.TotalPrice;
                        lastDate = sale_edited.Date;
                        lastUsername = sale_edited.Username;
                        lastLocation = sale_edited.Location;
                        lastUsername = sale_edited.Username;

                        // set background color of added sale to green
                        for (int i = 0; i < dataGrid1.Columns.Count; i++)
                        {
                            cell = dataGrid1.Columns[i].GetCellContent(row).Parent as DataGridCell;
                            cell.Background = Brushes.OliveDrab;
                        }
                        TextBlock_message.Text = $"The sale of id '{sale_edited.Id}' has been added.";
                        Array.Clear(fieldsEntered, 0, fieldsEntered.Length);
                        Log("insert"); // write log to file
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() => {
                            Button_AddSale.Focus(); // set focus to allow repeatedly add sale on pressing the Add sale button
                        }, DispatcherPriority.Loaded);

                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The sale of id '{sale_edited.Id}' has been updated with {(changed_property_name == "TotalPrice" ? "Total price" : changed_property_name)}.";
                        Log("update"); // write log to file
                        cell.Background = Brushes.OliveDrab;
                        // Shared.ChangeColor(cell, Colors.OliveDrab, Colors.Transparent);
                        CellEditEnding_nextCell_update();
                    }
                    old_value = new_value; // update old_value after successful update
                    TextBlock_message.Foreground = Brushes.LightGreen;


                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
                else // move to next cell when inserting
                {
                    CellEditEnding_nextCell_insert();
                }
            }
            else
            {
                return;
            }
        }

        private string CellEditEnding_setup(DataGridCellEditEndingEventArgs e)
        {
            if (edit_mode != "update" && Button_UpdateSale.IsKeyboardFocused) // switch to insert mode if 'Update' is clicked
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                dbSalesList.RemoveAt(dbSalesList.Count - 1);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbSalesList;
                EnableButtons();
                UpdateSale();
                return null;
            }
            else if (edit_mode != "insert" && Button_AddSale.IsKeyboardFocused) // switch to insert mode if 'Add' is clicked
            {
                edit_mode = "read";
                EnableButtons();
                AddSale();
                return null;
            }
            else if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
            {
                return null;
            }
            else if (Button_Close.IsKeyboardFocused)
            {
                CloseWindow();
                return null;
            }

            row = e.Row;
            row_index = row.GetIndex();
            column = e.Column;
            column_index = column.DisplayIndex;
            //sale_edited = row.Item as StockService.SalePurchase; //  sale_edited and sale_edited0 are already defined in UpdateSale and AddSale (read out current (old) values from the row, because the entry is a new value)

            cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;

            changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
            if (changed_property_name == "Product name") { changed_property_name = "Product"; }
            if (changed_property_name == "Quant.") { changed_property_name = "Quantity"; }
            if (changed_property_name == "Total price") { changed_property_name = "TotalPrice"; }
            if (changed_property_name == "User name") { changed_property_name = "Username"; }
            // get old property value of sale by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection

            old_value = sale_edited.GetType().GetProperty(changed_property_name).GetValue(sale_edited).ToString();
            return "OK";
        }

        private string CellEditEnding_checkInput()
        {
            string stopMessage = "";
            if (new_value == "") // if new value is empty
            {
                stopMessage = "New value cannot be empty!";
            }
            else if (changed_property_name == "Product") // if wrong (product) Name value is entered
            {
                if (new_value.Length < 5)
                {
                    stopMessage = $"Product name must be at least 5 characters!";
                }
                else
                {
                    dbProductsList = Product.GetProducts("", "", "", "", "", "", "");
                    if (dbProductsList == null) { IsEnabled = false; Close(); return "stop"; } // stop on any error
                    if (dbProductsList.Any(p => p.Name == new_value) == false)
                    {
                        stopMessage = $"The product does not exist in the database. Please check product name.";
                    }
                }
            }
            else if (changed_property_name == "Quantity") // if wrong Active value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val <= 0))
                {
                    stopMessage = $"Please enter a correct value for the Quantity!";
                }
                else if (int_val > 1000000)
                {
                    stopMessage = $"Quantity cannot exceed 1,000,000!";
                }
            }
            else if (changed_property_name == "TotalPrice") // if wrong TotalPrice value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val <= 0))
                {
                    stopMessage = $"Please enter a correct value for the Total price!";
                }
                else if (int_val > 1000000000)
                {
                    stopMessage = $"Total price cannot exceed 1,000,000,000!";
                }
            }
            else if (changed_property_name == "Date") // if wrong (product) Name value is entered
            {
                old_value = old_value.Substring(0, old_value.Length - 3);
                bool dateExists = DateTime.TryParse(new_value, out DateTime date_entered);
                if (dateExists == false)
                {
                    stopMessage = $"Please enter a correct value for the date value!";
                }
                else if (date_entered > DateTime.Now)
                {
                    stopMessage = $"Date cannot be a future date!";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    stopMessage = $"Date cannot be earlier than 3650 days!";
                }
            }
            else if (changed_property_name == "Location") // if wrong Location name is entered
            {
                dbLocationsList = Location.GetLocations("", "", "", "");
                if (dbLocationsList == null) { IsEnabled = false; Close(); return "stop"; } // stop on any error
                if (dbLocationsList.Any(p => p.Name == new_value) == false)
                {
                    stopMessage = $"The location '{new_value}' does not exist, please enter the correct location!";
                }
            }
            else if (changed_property_name == "Username" && new_value.Length < 5)
            {
                stopMessage = $"The username must be at least 5 charachters long!";
            }
            else if (changed_property_name == "Username" && new_value != old_value)
            {
                dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
                if (dbUsersList.Any(p => p.Username == new_value) == false) // stop if user does not exist in database, AND if new username is different
                {
                    stopMessage = $"The user '{new_value}' does not exist, please enter another username!";
                }
            }
            return stopMessage;
        }

        private void CellEditEnding_nextCell_update()
        {
            dataGrid1.Dispatcher.InvokeAsync(() => {
                // select next  column; if last 'UnitPrice' column is reached, return to first 'Name' column
                if (column_index == dataGrid1.Columns.Count - 1)
                {
                    column_index = 1;
                    // move 1 row down if it is not the last row
                    if (row_index < dataGrid1.Items.Count - 1)
                    {
                        row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index + 1]) as DataGridRow;
                        row_index++;
                    }
                }
                else
                {
                    column_index++;
                }

                cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;

                // go into edit mode if in insert mode
                cell.Focus(); // set focus on cell
                if (edit_mode == "insert")
                {
                    SelectTextBox();
                }

                if (edit_mode == "update") dataGrid1.SelectedCells.Clear();
                SelectEditedCell();
            }, DispatcherPriority.Loaded);
        }

        private void CellEditEnding_nextCell_insert()
        {
            dataGrid1.Focus();
            dataGrid1.Dispatcher.InvokeAsync(() => {

                cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                // select next unchanged column; if last 'UserId' column is reached, return to first (product) 'Name' column
                int column_shift = 0;
                while (fieldsEntered[column_index + column_shift - 1] != 0)
                {
                    column_shift = column_index + column_shift == 6 ? -column_index + 1 : column_shift + 1;
                }
                cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                // turn off eventual editing mode caused e.g. by tab key on data entry
                if (cell.IsEditing) { cell.IsEditing = false; }

                // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                Button_AddSale.Focus();
                SelectTextBox();
            }, DispatcherPriority.Loaded);
        }

        private void SelectEditedCell()
        {
            // select edited row/cell if user selected another row/cell during data entry
            if (cell.IsSelected == false)
            {
                // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
                if (edit_mode == "update")
                {
                    cell.IsSelected = true;
                }
                else
                {
                    dataGrid1.SelectedItem = dataGrid1.Items[row_index];
                }
            }
        }

        private void EnableButtons()
        {
            Button_AddSale.IsEnabled = true;
            Button_DeleteSale.IsEnabled = true;
            Button_Filter.IsEnabled = true;
            Button_Export.IsEnabled = true;
            Button_Import.IsEnabled = true;
            Button_LogWindow.IsEnabled = true;
        }

        private void StopAnimation()
        {
            // to avoid error "The calling thread cannot access this object because a different thread owns it"
            // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            this.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(2000);
                //Thread.Sleep(5000);
                gifImage.StopAnimation();
            }, DispatcherPriority.ContextIdle);
        }

        private void dataGrid1_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

            // in insert mode, do not allow user to edit a different row, and restore selection, focus and editing
            if (edit_mode == "insert" && e.Row.GetIndex() != row_index)
            {
                e.Cancel = true;
                SelectEditedCell();
                SelectTextBox();
            }
        }

        // https://stackoverflow.com/questions/27744097/wpf-fade-out-animation-cant-change-opacity-any-more
        private void ChangeOpacity()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(5),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => cell.Opacity = 0;

            cell.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void SelectTextBox()
        {
            cell.Focus();
            cell.IsEditing = true;

            cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move focus to textBox
            cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            textBox = (TextBox)cell.Content;
            // Keyboard.Focus(textBox);
            textBox.SelectAll();
        }

        private void ScrollDown()
        {
            // dataGrid1.Focus();
            scrollViewer = Shared.GetScrollViewer(dataGrid1);
            if (scrollViewer != null) scrollViewer.ScrollToEnd();
        }

        private void MessageFadeOut_Completed(object sender, EventArgs e)
        {
            TextBlock_message.Text = "";  // the Storyboard restores visibility due to FillBehavior="Stop", therefore the text is cleared to remain hidden
            // TextBlock_message.Opacity = 1;
            if (edit_mode == "read")
            {
                TextBlock_message.Foreground = Brushes.White;
            }
            gifImage.StopAnimation();
        }

        // make window draggable
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Button_Close.IsEnabled = false;
            TextBlock_message.Text = "Closing window.";
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeCompleted)
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }

        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            closeCompleted = true;
            Close();
        }

        // show/hide dataGrid0 with filter row
        private void Button_Filter_Click(object sender, RoutedEventArgs e)
        {
            filteredSalesList = new List<StockService.SalePurchase>();

            // show filter dataGrid0
            if (stackPanel1.Height == 442 + window.ActualHeight - 500)
            {
                stackPanel1.Margin = new Thickness(0, 45 + 30, 0, 0);
                stackPanel1.Height = 442 - 30 + window.ActualHeight - 500;
                ScrollDown();
                TextBlock_message.Text = "Enter filter value(s).";

            }
            else
            {
                stackPanel1.Margin = new Thickness(0, 45, 0, 0);
                stackPanel1.Height = 442 + window.ActualHeight - 500;
                TextBlock_message.Text = "Select an option.";
            }
        }

        private void dataGrid0_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            row = e.Row;
            column = e.Column;
        }

        private void dataGrid0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text; // get character entered
        }

        int? sale_filterId = null;
        int? sale_filterQuantity = null;
        int? sale_filterTotalPrice = null;
        DateTime? sale_filterDate = null;
        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        // private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row == null || column == null) { return; } // stop if column or row is not selected or not in edit mode

            // check whether the pressed key is digit or number, otherwise stop
            if (input.Length == 0)
            {
                return; // should not happen
            }
            int ASCII = (int)input[0];
            input = " "; // reset input to empty to avoid false value, becasuse KeyUp event may run on function keys as well
            if (((ASCII > 31 && ASCII < 256) || ASCII == 336 || ASCII == 337 || ASCII == 368 || ASCII == 369) == false) { return; } // stop if not number or digit expect Ő(336), ő(337), Ű(368), ű(369)
            // if (ASCII == 43 || ASCII == 60 || ASCII == 61 || ASCII == 62) { return; } // stop if +, <, =, >
            bool key = e.Key == Key.Back;

            // stop on most function keys, expect back, delete, <+í(Oem102), -, ., é(Oem1), ü(Oem2), ö(Oem3), ő(Oem4), Ű(Oem5), ú(Oem6), á(Oem7), Ó(OemPlus)
            if (e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Oem102 && e.Key != Key.Subtract && e.Key != Key.OemPeriod && e.Key != Key.Oem1 && e.Key != Key.Oem2 && e.Key != Key.Oem3 && e.Key != Key.Oem4 && e.Key != Key.Oem5 && e.Key != Key.Oem6 && e.Key != Key.Oem7 && e.Key != Key.OemPlus && ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) == false)
            {
                return;
            }

            // row = e.Row;
            // column = e.Column;
            filterc_index = column.DisplayIndex;
            cell = dataGrid1.Columns[filterc_index].GetCellContent(row).Parent as DataGridCell;
            if (cell.IsEditing == false) { return; } // stop if cell is not editing
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;
            if (new_value != "" && new_value.Length == 1 && (textBox.Text == "<" || textBox.Text == ">" || textBox.Text == "=") && e.Key != Key.Back && e.Key != Key.Delete) { return; } // stop if < or > in empty cell (but continue to recalculate if last key was Key.Back or Key.Delete
            if (new_value != "" && new_value.Length < 3 && (textBox.Text.Substring(1) == "=")) { return; } // stop if '=' when there are no more characters       

            string firstChar = new_value != "" ? new_value.ToString().Substring(0, 1) : "";
            string op = firstChar != ">" && firstChar != "<" ? "=" : firstChar;
            if (new_value.Length > 1 && new_value.ToString().Substring(1, 1) == "=")
            {
                op = op == ">" ? ">=" : op == "<" ? "<=" : op; // setting >= or <= operator values
            }

            changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();
            if (changed_property_name == "Product name") { changed_property_name = "Product"; }
            if (changed_property_name == "Quant.") { changed_property_name = "Quantity"; }
            if (changed_property_name == "Total price") { changed_property_name = "TotalPrice"; }
            if (changed_property_name == "User name") { changed_property_name = "Username"; }

            // set operator value for specific column
            if (changed_property_name == "Id" || changed_property_name == "Quantity" || changed_property_name == "TotalPrice" || changed_property_name == "Date")
            {
                switch (changed_property_name)
                {
                    case "Id": opId = op; break;
                    case "Quantity": opQuantity = op; break;
                    case "TotalPrice": opTotalPrice = op; break;
                    case "Date": opDate = op; break;
                    default: break;
                }
            }
            else { op = ""; } // clear operator for string columns

            //get old property value of sale by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = sale_filter.GetType().GetProperty(changed_property_name).GetValue(sale_filter).ToString();

            
            // check data correctness
            string stopMessage = "";
            bool minutesExist = false;
            if (new_value != "")
            {
                if (changed_property_name == "Id")
                {
                    string sale_filterId0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    sale_filterId = int.TryParse(sale_filterId0, out var tempVal1) ? tempVal1 : (int?)null;
                    if ((sale_filterId0 != "" && sale_filterId == null) || (sale_filterId < 0 || sale_filterId > 10000000))
                    {
                        stopMessage = $"The Id '{sale_filterId0}' does not exist, please enter a correct value for the Id!";
                    }
                }
                else if (changed_property_name == "Quantity") // if wrong Quantity value is entered
                {
                    string sale_filterQuantity0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    sale_filterQuantity = int.TryParse(sale_filterQuantity0, out var tempVal2) ? tempVal2 : (int?)null;
                    if ((sale_filterQuantity0 != "" && sale_filterQuantity == null) || (sale_filterQuantity < 0))
                    {
                        stopMessage = $"Please enter a correct value for the Quantity!";
                    }
                    else if (sale_filterQuantity > 1000000)
                    {
                        stopMessage = $"Quantity cannot exceed 1,000,000!";
                    }
                }
                else if (changed_property_name == "TotalPrice") // if wrong TotalPrice value is entered
                {
                    string sale_filterTotalPrice0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    sale_filterTotalPrice = int.TryParse(sale_filterTotalPrice0, out var tempVal3) ? tempVal3 : (int?)null;
                    if ((sale_filterTotalPrice0 != "" && sale_filterTotalPrice == null) || (sale_filterTotalPrice < 0))
                    {
                        stopMessage = $"Please enter a correct value for the Total price!";
                    }
                    else if (sale_filterTotalPrice > 1000000000)
                    {
                        stopMessage = $"Total price cannot exceed 1,000,000,000!";
                    }
                }
                else if (changed_property_name == "Date") // if wrong Date value is entered
                {
                    string sale_filterDate0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    sale_filterDate = DateTime.TryParse(sale_filterDate0, out var tempVal4) ? tempVal4 : (DateTime?)null;

                    if (changed_property_name == "Date" && ((sale_filterDate0.Length < 8 && sale_filterDate0.Length >= 0) || (sale_filterDate0.Length > 8 && sale_filterDate0.Length < 14))) { return; } // stop if date length is < 8 OR when time is edited (a value is deleted), otherwise sale_filter.Date will be set to null
                    if (sale_filterDate0 != "" && sale_filterDate == null)
                    {
                        stopMessage = $"Please enter a correct value for the Date!";
                    }
                    else
                    {
                        // checks if minutes are entered; if not, minutues in the record will be ignored
                        minutesExist = ((DateTime)sale_filterDate).Minute > 0;
                    }
                }
            }

            if (stopMessage != "")  // warn user, and stop
            {
                MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = old_value; // restore correct cell value if old value is not null
                Shared.SendKey(Key.End);
                return;
            }

            // update filter fields
            {
                sale_filter.GetType().GetProperty(changed_property_name).SetValue(sale_filter, new_value);
            }
            
            // filter
            filteredSalesList.Clear();
            foreach (var sale in dbSalesList)
            {
                if ((sale_filterId == null || Compare(sale.Id, sale_filterId, opId)) && (sale_filter.Product == "" || sale.Product.ToLower().Contains(sale_filter.Product.ToLower())) && (sale_filterQuantity == null || Compare(sale.Quantity, sale_filterQuantity, opQuantity)) && (sale_filterTotalPrice == null || Compare(sale.TotalPrice, sale_filterTotalPrice, opTotalPrice)) && (sale_filterDate == null || (minutesExist ? Compare(sale.Date, sale_filterDate, opDate) : Compare(sale.Date.Value.Date, sale_filterDate, opDate))) && (sale_filter.Location == "" || sale.Location.ToLower().Contains(sale_filter.Location.ToLower())) && (sale_filter.Username == "" || sale.Username.ToLower().Contains(sale_filter.Username.ToLower())))
                {
                    filteredSalesList.Add(sale);
                }
            }

            // update dataGrid1 with filtered items                    
            dataGrid1.ItemsSource = filteredSalesList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            dataGrid1.Items.Refresh();
        }

        private bool Compare(int? a, int? b, string op)
        {
            switch (op)
            {
                case "=": return a == b;
                case ">": return a > b;
                case "<": return a < b;
                case ">=": return a >= b;
                case "<=": return a <= b;
                default: return false;
            }
        }

        private bool Compare(DateTime? a, DateTime? b, string op)
        {
            switch (op)
            {
                case "=": return a == b;
                case ">": return a > b;
                case "<": return a < b;
                case ">=": return a >= b;
                case "<=": return a <= b;
                default: return false;
            }
        }

        private void SetUserAccess()
        {
            // 0-2: view only 3-5: +insert/update 6-8: +delete 9: +user management (admin)
            if (Shared.loggedInUser.Permission < 6)
            {
                Button_DeleteSale.IsEnabled = false;
                Button_DeleteSale.Foreground = Brushes.Gray;
                Button_DeleteSale.ToolTip = "You do not have rights to delete data!";
            }
            if (Shared.loggedInUser.Permission < 3)
            {
                Button_AddSale.IsEnabled = false;
                Button_AddSale.Foreground = Brushes.Gray;
                Button_AddSale.ToolTip = "You do not have rights to add data!";
                Button_UpdateSale.IsEnabled = false;
                Button_UpdateSale.Foreground = Brushes.Gray;
                Button_UpdateSale.ToolTip = "You do not have rights to update data!";
                Button_Import.IsEnabled = false;
                Button_Import.Foreground = Brushes.Gray;
                Button_Import.ToolTip = "You do not have rights to update data!";
            }
        }

        private void Button_DatePicker_Click(object sender, RoutedEventArgs e)
        {
            pickStartDate = !pickStartDate;
            Button_DatePicker.Foreground = pickStartDate ? Brushes.LightSkyBlue : Brushes.CornflowerBlue;
            TextBlock_datePicker.Foreground = pickStartDate ? Brushes.White : Brushes.LightSkyBlue;
            TextBlock_datePicker.Text = pickStartDate ? "Pick start date" : "Pick end date";

            Button_Restore.Visibility = Visibility.Hidden;
            Button_Maximize.Visibility = Visibility.Hidden;
            TextBlock_datePicker.Visibility = Visibility.Visible;

            // remove/add event handler 
            datePicker.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);
            datePicker.SelectedDate = pickStartDate ? startDate : endDate;
            datePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);

            datePicker.Visibility = Visibility.Visible;
            datePicker.IsDropDownOpen = true;
        }

        private void datePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                System.Windows.Controls.Primitives.DatePickerTextBox datePickerTextBox = FindVisualChild<System.Windows.Controls.Primitives.DatePickerTextBox>(datePicker);
                if (datePickerTextBox != null)
                {

                    ContentControl watermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                    if (watermark != null)
                    {
                        watermark.Content = startDate.ToShortDateString(); // "Pick start date"; // string.Empty;
                    }
                }
            }
        }
        private T FindVisualChild<T>(DependencyObject depencencyObject) where T : DependencyObject
        {
            if (depencencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depencencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depencencyObject, i);
                    T result = (child as T) ?? FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDate != (DateTime)datePicker.SelectedDate && endDate != (DateTime)datePicker.SelectedDate)  // to avoid set change two times due to false double selection change from datePicker databox editing
            {


                if (pickStartDate)
                {
                    startDate = (DateTime)datePicker.SelectedDate;
                }
                else
                {
                    endDate = (DateTime)datePicker.SelectedDate;
                    endDate = endDate.Hour == 0 && endDate.Minute == 0 && endDate.Second == 0 ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
                    // endDate = endDate.ToString().Substring(10, 8) == "00:00:00" ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
                }

                ReloadData();

                string startOrEnd = pickStartDate ? "Start" : "End";
                // int trimZeros = datePicker.SelectedDate.ToString().Substring(10, 8) == "00:00:00" ? 10 : 3;
                TextBlock_message.Text = $"{startOrEnd} date changed to {(pickStartDate ? startDate : endDate).ToString().Substring(0, (pickStartDate ? startDate : endDate).ToString().Length - 3)}.";

                pickStartDate = !pickStartDate; // toggle 
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
            }
        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            datePicker.Visibility = Visibility.Hidden;
            TextBlock_datePicker.Text = "";
            Button_Restore.Visibility = Visibility.Visible;
            Button_Maximize.Visibility = Visibility.Visible;
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Comma separated text file (*.csv)|*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "dbSales",
                Title = "Save sales data as:"
            };
            
            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog
            if (result == true)
            {
                // create file content
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName, append: false, encoding: Encoding.UTF8);
                // write file header line
                string header_row = "Id;Product;Quantity;TotalPrice;Date;Location;Username";
                sr.WriteLine(header_row);

                // write file rows
                string rows = "";
                StockService.SalePurchase sale;
                int i = 0;
                for (i = 0; i < dataGrid1.Items.Count; i++)
                {
                    sale = dataGrid1.Items[i] as StockService.SalePurchase;
                    rows += $"{sale.Id};{sale.Product};{sale.Quantity};{sale.TotalPrice};{sale.Date.ToString().Substring(0, sale.Date.ToString().Length - 3)};{sale.Location};{sale.Username}\n";
                }
                sr.Write(rows);
                sr.Close();

                TextBlock_message.Text = $"Database content ({i} records) printed to '{saveFileDialog.FileName}' file.";
                TextBlock_message.Foreground = Brushes.LightGreen;
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
            }
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Comma separated file (*.csv) |*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                Title = "Open file for import to 'Sales' table"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName.ToLower().Contains("purchase"))
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure to import Sale data from the file '{openFileDialog.FileName}'?", caption: "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    bool encrypt = result == MessageBoxResult.Yes;
                    if (result == MessageBoxResult.No) { return; }
                }

                StreamReader sr = new StreamReader(openFileDialog.FileName);
                // check header correctness
                string header_row = sr.ReadLine();
                int first_colum = header_row.Split(';').Length == 6 ? 0 : 1; // 1 if Id column is provided

                if (header_row != "Id;Product;Quantity;TotalPrice;Date;Location;Username" && header_row != "Product;Quantity;TotalPrice;Date;Location;Username")
                {
                    MessageBox.Show($"Incorrect file content! Expected header is 'Id;Product;Quantity;TotalPrice;Date;Location;Username' (Id is optional), but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                StockService.SalePurchase sale;
                importList = new List<StockService.SalePurchase>();
                int row_index = 0;
                string[] row;
                int salesAdded = 0;
                string registerMessage = "";
                string errorMessage = "";
                int? id = dbSalesList.Max(u => u.Id) + 1;
                dbLocationsList = Location.GetLocations("", "", "", "");
                if (dbLocationsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                dbProductsList = Product.GetProducts("", "", "", "", "", "", "");
                dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
                while (sr.EndOfStream == false)
                {
                    row_index++;
                    string error = "";
                    row = sr.ReadLine().Split(';');
                    if (row.Length != 6 + first_colum) // skip row if number of columns is incorrect
                    {
                        errorMessage += $"Sale in line {row_index}: The required {6 + first_colum} fields are not available!\n";
                        continue;
                    }

                    string product = row[first_colum];
                    string quantity = row[first_colum + 1];
                    string totalPrice = row[first_colum + 2];
                    string date = row[first_colum + 3];
                    string location = row[first_colum + 4];
                    string username = row[first_colum + 5];

                    // check data correctness
                    if (product.Length < 5) // if wrong (product) Name value is entered
                    {
                        error += $"Sale in line {row_index}: Product name must be at least 5 characters!\n";
                    }
                    if (dbProductsList.Any(p => p.Name == product) == false)
                    {
                        error += $"Sale in line {row_index}: Product '{product}' does not exist!\n";
                    }
                    int? int_val = Int32.TryParse(quantity, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val <= 0))
                    {
                        error += $"Sale in line {row_index}: Quantity '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Sale in line {row_index}: Quantity '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    int_val = Int32.TryParse(totalPrice, out var tempVal1) ? tempVal1 : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"Purchase in line {row_index}: Total price '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Purchase in line {row_index}: Total price '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    bool dateExists = DateTime.TryParse(date, out DateTime date_entered);
                    if (dateExists == false)
                    {
                        error += $"Sale in line {row_index}: Date value '{date}' is incorrect!\n";
                    }
                    else if (date_entered > DateTime.Now)
                    {
                        error += $"Sale in line {row_index}: Date '{date}' cannot be a future date!\n";
                    }
                    else if ((DateTime.Now - date_entered).TotalDays > 3650)
                    {
                        error += $"Sale in line {row_index}: Date '{date}' cannot be earlier than 3650 days!\n";
                    }
                    if (dbLocationsList.Any(p => p.Name == location) == false) // if wrong Location name is entered
                    {
                        error += $"Sale in line {row_index}: Location '{location}' does not exist!\n";
                    }
                    if (username.Length < 5)
                    {
                        error += $"Sale in line {row_index}: Username '{username}' must be at least 5 charachters long!\n";
                    }
                    if (dbUsersList.Any(p => p.Username == username) == false) // if user does not exist in database
                    {
                        error += $"Sale in line {row_index}: User '{username}' does not exist!\n";
                    }

                    errorMessage += error;
                    if (error != "") { continue; } // skip on error

                    // ADD into database
                    registerMessage = stockClient.AddSalePurchase(Shared.uid, type: "sale", product, quantity, location, totalPrice, date);
                    if (registerMessage.Contains("FOREIGN KEY (`productId`)"))
                    {
                        errorMessage += $"Sale in line'{column_index + 1}': {registerMessage}\n";
                        continue;
                    }

                    sale = new StockService.SalePurchase
                    {
                        Id = id,
                        Product = product,
                        Quantity = int.Parse(quantity),
                        TotalPrice = int.Parse(totalPrice),
                        Date = DateTime.Parse(date),
                        Location = location,
                        Username = username
                    };
                    salesAdded++;
                    importList.Add(sale);
                    Log("insert"); // write log to file
                    id++;
                }
                sr.Close();

                if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                if (importList.Count > 0)
                {
                    dataGrid1.ItemsSource = importList;

                    TextBlock_message.Text = $"{salesAdded} {(salesAdded == 1 ? "record" : "records")} added into the 'Sales' table.";
                    TextBlock_message.Foreground = Brushes.LightGreen;
                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 - 110) / (550 - 10 - 43), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;

            stackPanel1.Height = 442 + window.ActualHeight - 500 - stackPanel1.Margin.Top + 45; // original window.Heigth

            // stretch columns to dataGrid1 width
            for (int i = 0; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 0 || i == 2 ? 0.5 : 1) + 1); // resize Id and Quantity row only by 50%
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
            }
            dataGrid1.FontSize = 14 * Math.Max(stretch, 0.9);
            dataGrid1.Columns[2].Header = stretch < 1.18 ? "Quantity" : "Quant.";
        }

        private void Log(string operation)
        {
            string row = "";
            // save operation into log file
            StreamWriter sr = new StreamWriter(@".\Logs\manageSales.log", append: true, encoding: Encoding.UTF8);
            // write file header line
            // string header_row = "Date;Username;Operation;Id;Product;Quantity;TotalPrice;Date;Location;Username;";
            // sr.WriteLine(header_row);

            // write file rows
            StockService.SalePurchase sale;
            sale = sale_edited;

            if (operation == "update") // in update mode add the old value in a new line
            {
                int index = column_index;
                row = $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{sale.Id};{(column_index == 1 ? old_value : null)};{(column_index == 2 ? old_value : null)};{(column_index == 3 ? old_value : null)};{(column_index == 4 ? old_value.Substring(0, sale.Date.ToString().Length - 3) : null)};{(column_index == 5 ? old_value : null)};{(column_index == 6 ? old_value : null)}\n";
            }
            DateTime date = sale.Date == null ? DateTime.Now : (DateTime)sale.Date;
            row += $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{sale.Id};{sale.Product};{sale.Quantity};{sale.TotalPrice};{(sale.Date == null ? null : date.ToString("yy.MM.dd HH:mm: ss"))};{sale.Location};{sale.Username}";
            sr.WriteLine(row);
            sr.Close();
        }
        private void Button_Maximize_Click(object sender, RoutedEventArgs e)
        {
            windowWidth0 = window.Width;
            windowHeight0 = window.Height;
            windowLeft0 = window.Left;
            windowTop0 = window.Top;
            window.Width = Shared.screenWidth;
            window.Height = Shared.screenHeight;
            window.Left = 0;
            window.Top = 0;
            Button_Restore.IsEnabled = true;
            Button_Maximize.IsEnabled = false;

        }

        private void Button_Restore_Click(object sender, RoutedEventArgs e)
        {
            window.Width = windowWidth0;
            window.Height = windowHeight0;
            window.Left = windowLeft0;
            window.Top = windowTop0;
            Button_Restore.IsEnabled = false;
            Button_Maximize.IsEnabled = true;
        }
        
        Logs.LogWindowSales LogWindowSales;
        private void Button_LogWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(LogWindowSales))
            {
                LogWindowSales = new Logs.LogWindowSales();
                if (LogWindowSales.IsEnabled) LogWindowSales.Show();
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.Dispatcher.InvokeAsync(async () => {
                await Task.Delay(2000);
                // fals is needed to display colored rows properly
                dataGrid1.EnableRowVirtualization = false; // must be delayed, otherwise animation does not work properly
            }, DispatcherPriority.SystemIdle);
        }

    }
}


