﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using FrontendWPF.Classes;


namespace FrontendWPF
{

    public partial class ManageSalesWindow : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<StockService.SalePurchase> dbSalesList { get; set; }

        System.Collections.IList selectedItems;
        List<StockService.SalePurchase> filterSalesList { get; set; }
        List<StockService.SalePurchase> filteredSalesList { get; set; }
        List<StockService.SalePurchase> selectedSalesList { get; set; }
        private List<StockService.Product> dbProductsList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private List<SalePurchase> salesList { get; set; }
        private int[] fieldsEntered = new int[5]; // (product) Name, Quantity, Date, Location, User(name)
        ScrollViewer scrollViewer;
        string lastProduct = "";
        int? lastQuantity = null;
        string lastLocation = "";
        string lastUsername = "";
                

        public ManageSalesWindow()
        {
            InitializeComponent();

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
            TextBlock_message.Text = "Select an option.";
            TextBlock_message.Foreground = Brushes.White;
            // query all sales from database
            dbSalesList = SalePurchase.GetSalesPurchases(type: "sale", id: "", product: "", qOver: "", qUnder: "", before: "", after: "", location: "", user: "", limit: "");

            // close window and stop if no sale is retrieved
            if (dbSalesList.Count == 0)
            {
                IsEnabled = false;
                closeCompleted = true;
                IsEnabled = false; // so that window cannot be opened
                Close();
                return;
            }







            dataGrid1.ItemsSource = dbSalesList;

            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            if (window.IsLoaded == false) // run on the first time when window is not loaded
            {
                filterSalesList = new List<StockService.SalePurchase>();

                Dispatcher.InvokeAsync(() => {
                    double stretch = (600 - 63) / dataGrid1.ActualWidth; // Border width - left margin - a bit more because first column remains unchanged
                    dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                    dataGrid0.Width = dataGrid1.Width;
                    // stretch columns to dataGrid1 width
                    for (int i = 1; i < dataGrid1.Columns.Count; i++)
                    {
                        dataGrid1.Columns[i].Width = dataGrid1.Columns[i].ActualWidth * stretch;
                        dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                    }
                    dataGrid1.Items.Refresh();
                    ScrollDown();
                    selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
                }, DispatcherPriority.Loaded);
            }
            ScrollDown();

            // create/reset sale_filter item and add it to filter dataGrid0
            sale_filter = new StockService.SalePurchase()
            {
                Id = -1,
                Product = "-1", // Name of product
                Quantity = -1,
                Date = null,
                Location = "-1",
                Username = "-1"
            };
            filterSalesList.Clear();
            filterSalesList.Add(sale_filter);
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
                dataGrid1.Dispatcher.InvokeAsync(() => {
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
                                    dbSalesList.Remove(selectedSalesList[i]); // remove sale also from dbSalesList
                                    selectedSalesList.RemoveAt(i);
                                }
                                else
                                {
                                    MessageBox.Show(deleteMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }
                            catch (Exception ex)
                            {
                                if (ex.ToString().Contains("XXXXX"))
                                {
                                    MessageBox.Show($"This will be a specific error. Details:\n{ex.Message}", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show("An error occurred, with the following details:\n" + ex.Message, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
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
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;


            if (edit_mode == "read") // if read mode (or window just opened), switch to update mode
            {
                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                edit_mode = "update";
                dataGrid1.SelectionMode = DataGridSelectionMode.Single;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.Cell;
                TextBlock_message.Text = "Update sale.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();
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
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;
            Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

            if (edit_mode == "read" || edit_mode == "update") // if read mode (window just opened) or update mode, switch to insert mode
            {

                // in db select last sale with highest Id
                int? highestId = dbSalesList.Max(u => u.Id);
                sale_edited = new StockService.SalePurchase() // create new sale with suggested values
                {
                    Id = highestId + 1,
                    Product = lastProduct != "" ? lastProduct : "",
                    Quantity = lastQuantity != null ? lastQuantity : 1,
                    Date = DateTime.Now,
                    Location = lastLocation != "" ? lastLocation : Shared.loggedInUser.Location,
                    Username = lastUsername != "" ? lastUsername : Shared.loggedInUser.Username
                };
                    
                    
            
                sale_edited0 = sale_edited;

                dbSalesList.Add(sale_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbSalesList;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the sale to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    // style the id cell of the new sale
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                },
                DispatcherPriority.Loaded);

                edit_mode = "insert";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                TextBlock_message.Text = "Add sale.";
                TextBlock_message.Foreground = Brushes.White;
            }
            else
            {
                MessageBox.Show("Please fill in all sale data, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
                dataGrid1.BeginEdit();
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
        StockService.SalePurchase sale_edited, sale_edited0, sale_filter;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // exit insert mode if 'Update sale' is clicked
                if (Button_UpdateSale.IsKeyboardFocused)
                {
                    edit_mode = "read";
                    dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                    dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                    dbSalesList.RemoveAt(dbSalesList.Count - 1);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbSalesList;
                    UpdateSale();
                    return;
                }
                else if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
                {
                    return;
                }
                else if (Button_Close.IsKeyboardFocused)
                {
                    CloseWindow();
                    return;
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
                if (changed_property_name == "User name") { changed_property_name = "Username"; }
                // get old property value of sale by property name
                // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
                old_value = sale_edited.GetType().GetProperty(changed_property_name).GetValue(sale_edited).ToString();

                // check data correctness
                string stopMessage = "";
                if (new_value == "") // if new value is empty
                {
                    stopMessage = "New value cannot be empty!";
                }
                else if (changed_property_name == "Product") // if wrong (product) Name value is entered
                {
                    if (new_value == "")
                    {
                        stopMessage = $"Please enter a correct value for the Product name!";
                    }
                    else
                    {
                        dbProductsList = Product.GetProducts("", "", "", "", "");
                        if (dbProductsList.Any(p => p.Name == new_value) == false)
                        {
                            stopMessage = $"The product does not exist in the database. Please check product name.";
                        }
                    }
                }
                else if (changed_property_name == "Name") // if wrong (product) Name value is entered
                {
                    if (new_value == "")
                    {
                        stopMessage = $"Please enter a correct value for the Product name!";
                    }
                }
                else if (changed_property_name == "Date") // if wrong (product) Name value is entered
                {
                    if (DateTime.TryParse(new_value, out _) == false)
                    {
                        stopMessage = $"Please enter a correct value for the date value!";
                    }
                }


                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value
                    // cell.Content = old_value;

                    Dispatcher.InvokeAsync(() => {

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
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Location, Permission and Active can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index - 1] == 1 || edit_mode == "update")) // && column_index < 3
                {
                    MoveToNextCell();
                    return;
                }

                Dispatcher.InvokeAsync(() => {
                    SelectEditedCell(); // select edited row/cell if user selected another row/cell after data entry
                }, DispatcherPriority.Loaded);

                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                    Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }


                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index == 1 || column_index == 4 || column_index == 5) // // update string-type fields with new value ( (product) Name, Location, User (name) )
                {
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, new_value);
                }
                else if (column_index == 3) // // update Date fields with new value
                {
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, DateTime.Parse(new_value));
                }
                else // update int?-type fields with new value ( (product) Name, Quantity, LocationId, UserId)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    
                    sale_edited.GetType().GetProperty(changed_property_name).SetValue(sale_edited, Convert.ToInt32(new_value));

                }

                // check if all properties are entered, then insert into database
                if (fieldsEntered.All(n => n == 1) || edit_mode == "update") // if all fields have been updated OR update mode for one field
                {
                    string registerMessage = "";
                    string updateMessage = "";
                    try
                    {
                        if (edit_mode == "insert")
                        {
                            // ADD into database
                            registerMessage = stockClient.AddSalePurchase(Shared.uid, type: "sale", sale_edited.Product, sale_edited.Quantity.ToString(), sale_edited.Location, sale_edited.Date.ToString());
                            if (registerMessage.Contains("FOREIGN KEY (`productId`)"))
                            {
                                MessageBox.Show($"The product does not exist in the database. Please check product name.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            else if (registerMessage != "Sale/Purchase successfully added!")
                            {
                                MessageBox.Show(registerMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                sale_edited = sale_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            updateMessage = stockClient.UpdateSalePurchase(Shared.uid, sale_edited.Id.ToString(), type: "sale", sale_edited.Product, sale_edited.Quantity.ToString(), sale_edited.Date.ToString(), sale_edited.Location, sale_edited.Username);
                            if (updateMessage != "Sale successfully updated!")
                            {
                                MessageBox.Show(updateMessage + " Field was not updated in the database!", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                sale_edited = sale_edited0;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("XXXXX"))
                        {
                            MessageBox.Show("This will be a specific error.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("An error occured, with the following details:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }


                    if (edit_mode == "insert")
                    {
                        lastProduct = sale_edited.Product; // save last data to suggest them for next record
                        lastQuantity = sale_edited.Quantity;
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
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() => {
                            Button_AddSale.Focus(); // set focus to allow repeatedly add sale on pressing the Add sale button
                        },
                        DispatcherPriority.Loaded);

                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The sale of id '{sale_edited.Id}' has been updated with {changed_property_name}.";

                        // cell.Background = Brushes.OliveDrab;
                        Shared.ChangeColor(cell, Colors.OliveDrab, Colors.Transparent);
                        MoveToNextCell();
                    }
                    old_value = new_value; // update old_value after successful update
                    TextBlock_message.Foreground = Brushes.LightGreen;


                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
                else // move to next cell
                {
                    dataGrid1.Focus();
                    dataGrid1.Dispatcher.InvokeAsync(() => {

                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                        // select next unchanged column; if last 'UserId' column is reached, return to first (product) 'Name' column
                        int column_shift = 0;
                        while (fieldsEntered[column_index + column_shift - 1] != 0)
                        {
                            column_shift = column_index + column_shift == 5 ? -column_index + 1 : column_shift + 1;
                        }
                        cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                        // turn off eventual editing mode caused e.g. by tab key on data entry
                        if (cell.IsEditing) { cell.IsEditing = false; }

                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        Button_AddSale.Focus();

                        SelectTextBox();



                    },
                DispatcherPriority.Loaded); // style the id cell of the new sale
                }

            }
            else
            {
                return;
            }
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


        private void dataGrid1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

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

        private void dataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid1 = sender as DataGrid;

            // https://stackoverflow.com/questions/3248023/code-to-check-if-a-cell-of-a-datagrid-is-currently-edited
            IEditableCollectionView itemsView = dataGrid1.Items;

            // prevent dataGrid to select lower cell on Enter if not editing (otherwise entire editing would stop
            if (e.Key == Key.Enter && (itemsView.IsAddingNew || itemsView.IsEditingItem) == false && edit_mode != "insert")
                if (e.Key == Key.Enter && itemsView.IsEditingItem == false && edit_mode != "insert")
                {
                    // e.Handled = true;
                    // MoveToNextCell();
                }
        }


        private void MoveToNextCell()
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

                // turn off eventual editing mode causes e.g. by tab key on data entry
                // if (cell.IsEditing) { cell.IsEditing = false; }


                // go into edit mode if in insert mode
                cell.Focus(); // set focus on cell
                if (edit_mode == "insert") // TODO: tesztelni!
                {
                    SelectTextBox();
                }

                if (edit_mode == "update") dataGrid1.SelectedCells.Clear(); // TODO: tesztelni!
                SelectEditedCell();
            },
            DispatcherPriority.Loaded);
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.Key == Key.Enter)
            {
                return;
            }
            */
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
            if (stackPanel1.Height == 442)
            {
                stackPanel1.Margin = new Thickness(0, 45 + 30, 0, 0);
                stackPanel1.Height = 442 - 30;
                ScrollDown();
                TextBlock_message.Text = "Enter filter value(s).";

            }
            else
            {
                stackPanel1.Margin = new Thickness(0, 45, 0, 0);
                stackPanel1.Height = 442;
                TextBlock_message.Text = "Select an option.";
            }
        }

        private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
                {
                    return;
                }
                else if (Button_Close.IsKeyboardFocused)
                {
                    CloseWindow();
                    return;
                }

                row = e.Row;
                column = e.Column;
                filterc_index = column.DisplayIndex;
                cell = dataGrid1.Columns[filterc_index].GetCellContent(row).Parent as DataGridCell;
                textBox = (TextBox)cell.Content;
                new_value = textBox.Text;
                changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();
                if (changed_property_name == "Product name") { changed_property_name = "Product"; }
                if (changed_property_name == "User name") { changed_property_name = "Username"; }

                //get old property value of sale by property name
                // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
                old_value = sale_filter.GetType().GetProperty(changed_property_name).GetValue(sale_filter).ToString();

                // check data correctness
                string stopMessage = "";
                if (changed_property_name == "Id")
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || int_val < -1)
                    {
                        stopMessage = $"Please enter a correct value for the Id!";
                    }
                }
                /*
                else if (new_value != "" && new_value != "-1" && changed_property_name == "Name" && new_value.Length < 5)
                {
                    stopMessage = $"The name must be at least 5 charachters long!";
                }
                */
                else if (changed_property_name == "Name") // if wrong (product) Name value is entered
                {
                    if (new_value == "")
                    {
                        stopMessage = $"Please enter a correct value for the Product name!";
                    }
                }

                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value
                    return;
                }

                /*
                // stop if new and old value are the same 
                else if (old_value == new_value)
                {
                    return;
                }
                */


                if (filterc_index == 1 || filterc_index == 4 || filterc_index == 5 ) // // update string-type fields with new value ( (product) Name, Location, User (name) )
                {
                    sale_filter.GetType().GetProperty(changed_property_name).SetValue(sale_filter, new_value);
                }
                else if (filterc_index == 3) // // update Date fields with new value
                {
                    sale_filter.GetType().GetProperty(changed_property_name).SetValue(sale_filter, DateTime.Parse(new_value));
                }
                else // update int?-type fields with new value (UnitPrice)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    sale_filter.GetType().GetProperty(changed_property_name).SetValue(sale_filter, Convert.ToInt32(new_value));

                }

                // filter
                filteredSalesList.Clear();
                foreach (var sale in dbSalesList)
                {

                    if ((sale_filter.Id == -1 || sale_filter.Id == null || sale.Id == sale_filter.Id) && (sale_filter.Product == "-1" || sale_filter.Product == "" || sale.Quantity == sale_filter.Quantity) && (sale_filter.Date == null || sale_filter.Date == null || sale.Date == sale_filter.Date))
                    {
                        filteredSalesList.Add(sale);
                        continue;
                    }
                }
                // update dataGrid1 with filtered items                    
                dataGrid1.ItemsSource = filteredSalesList;
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                dataGrid1.Items.Refresh();
            }
        }


        private void dataGrid0_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            /*
            // check wheter the pressed key ia digit or number
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string key = e.Key.ToString();
            }
            */
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
            }
        }

    }
}

