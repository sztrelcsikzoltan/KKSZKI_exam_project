﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Microsoft.Win32;

namespace FrontendWPF
{

    public partial class ManageProductsWindow : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<StockService.Product> dbProductsList { get; set; }

        System.Collections.IList selectedItems;
        List<StockService.Product> filterProductsList { get; set; }
        List<StockService.Product> filteredProductsList { get; set; }
        List<StockService.Product> selectedProductsList { get; set; }
        List<StockService.Product> importList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private List<Product> productsList { get; set; }
        private int[] fieldsEntered = new int[3]; // Name, BuyUnitPrice, SellUnitPrice
        ScrollViewer scrollViewer;
        string lastName = "";
        int lastNameIndex = 1;
        int? lastBuyUnitPrice = null;
        int? lastSellUnitPrice = null;
        string input = "";
        string opId = "=";
        string opBuyUnitPrice = "=";
        string opSellUnitPrice = "=";
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;

        public ManageProductsWindow()
        {
            InitializeComponent();
            if (Shared.layout !="")
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                if (Shared.layout.Contains("Products"))
                {
                    window.Left = 0;
                    window.Top = 0;
                    window.Height = 1 * Shared.screenHeight;
                    window.Width = 0.4 * Shared.screenWidth;
                    // window.Topmost = false;
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
            
            // query all products from database
            dbProductsList = Product.GetProducts("", "", "", "", "", "", "");
            if (dbProductsList == null) { IsEnabled = false; Close(); return; } // stop on any error
            TextBlock_message.Text = $"{dbProductsList.Count} products loaded.";

            // close window and stop if no product is retrieved
            if (dbProductsList.Count == 0)
            {
                IsEnabled = false;
                closeCompleted = true;
                IsEnabled = false; // so that window cannot be opened
                Close();
                return;
            }







            dataGrid1.ItemsSource = dbProductsList;

            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            filterProductsList = new List<StockService.Product>();

            Dispatcher.InvokeAsync(() => {
                double stretch = Math.Max((borderLeft.ActualWidth - 10 - 74) / (550 - 10 - 128), 1); // Border width - left margin - a bit more because first column remains unchanged
                dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                dataGrid0.Width = dataGrid1.Width;
                dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

                stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Height
                
                // stretch columns to dataGrid1 width
                for (int i = 1; i < dataGrid1.Columns.Count; i++)
                {
                    dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * stretch;
                    dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                    // dataGrid0.Columns[i].MaxWidth = dataGrid1.Columns[i].ActualWidth * stretch;
                }
                dataGrid1.FontSize = 14 * Math.Min(stretch, Shared.layout == "" ? 1 : stretch);
                dataGrid1.Items.Refresh();
                ScrollDown();
                selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
            }, DispatcherPriority.Loaded);

            ScrollDown();

            // create/reset product_filter item and add it to filter dataGrid0
            product_filter = new StockService.Product()
            {
                Id = null,
                Name = "",
                BuyUnitPrice = null,
                SellUnitPrice = null
            };
            filterProductsList.Clear();
            filterProductsList.Add(product_filter);
            dataGrid0.ItemsSource = null; // to avoid IsEditingItem error
            dataGrid0.ItemsSource = filterProductsList;
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

            // in update mode  update selectedcells and product_edited (when SelecionUnit is Cell)
            // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
            if (edit_mode == "update")
            {
                // https://stackoverflow.com/questions/4714325/wpf-datagrid-selectionchanged-event-isnt-raised-when-selectionunit-cell
                IList<DataGridCellInfo> selectedcells = e.AddedCells;
                if (selectedcells.Count > 0) // ignore new selection when button is pressed and selection becomes 0; 
                {
                    product_edited = (StockService.Product)selectedcells[0].Item;
                    product_edited0 = product_edited;
                }
            }
        }

        private void Button_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode == "update")
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridCellInfo currentCell = dataGrid1.CurrentCell;

                dataGrid1.SelectedItems.Add(product_edited); // this triggers SelectionChanged and sets new selectedItems
            }

            if (selectedItems.Count > 0)
            {
                selectedProductsList = new List<StockService.Product>();
                foreach (StockService.Product product in selectedItems)
                {
                    selectedProductsList.Add(product);
                }
                dataGrid1.ItemsSource = selectedProductsList;

                // waits to render dataGrid1 and sets row background color to Salmon 
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    for (int i = 0; i < selectedProductsList.Count; i++)
                    {
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }

                    int selectedProducts = selectedProductsList.Count;
                    string deleteMessage = selectedProducts == 1 ? "Are you sure to delete the selected product?" : $"Are you sure to delete the selected {selectedProducts} products?";

                    TextBlock_message.Text = selectedProducts == 1 ? "Delete product?" : $"Delete {selectedProducts} product?";
                    TextBlock_message.Foreground = Brushes.Salmon;
                    MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = selectedProducts - 1; i >= 0; i--)
                        {
                            try
                            {
                                // DELETE product(s) from database
                                deleteMessage = stockClient.RemoveProduct(Shared.uid, selectedProductsList[i].Id.ToString());
                                if (deleteMessage == "Product successfully removed!")
                                {
                                    product_edited = selectedProductsList[i]; // required to write the log
                                    Log("delete"); // write log to file
                                    dbProductsList.Remove(selectedProductsList[i]); // remove product also from dbProductsList
                                    selectedProductsList.RemoveAt(i);
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

                        if (selectedProductsList.Count == 0)
                        {
                            deleteMessage = selectedProducts == 1 ? "The product has been deleted." : "The products have been deleted.";
                            TextBlock_message.Text = selectedProducts == 1 ? "Product deleted." : "Products deleted.";
                        }
                        else
                        {
                            deleteMessage = selectedProductsList.Count == 1 ? "The product shown in the table could not be deleted, as reported in the error message." : "The products shown in the table could not be deleted, as reported in the error message.";
                        }
                        // list the products that could not be deleted (empty if all deleted)
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = selectedProductsList;

                        checkBox_fadeInOut.IsChecked = false;
                        checkBox_fadeInOut.IsChecked = true; // show gifImage
                        gifImage.StartAnimation();
                        MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                    // dataGrid1.Focus();
                    dataGrid1.ItemsSource = dbProductsList;
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
                MessageBox.Show("Nothing is selected. Please select at least one product. ", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dataGrid1.CanUserSortColumns = true;
        }


        private void Button_UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void UpdateProduct()
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
                TextBlock_message.Text = "Update product.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
            }
        }


        private void Button_AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct();
        }

        private void AddProduct()
        {
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;
            Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

            if (edit_mode == "read" || edit_mode == "update") // if read mode (window just opened) or update mode, switch to insert mode
            {

                // in db select last product with highest Id
                int? highestId = dbProductsList.Count > 0 ? dbProductsList.Max(u => u.Id) : 0;
                product_edited = new StockService.Product() // create new product with suggested values
                {
                    Id = highestId + 1,
                    Name = lastName != "" ? lastName : "",
                    BuyUnitPrice = lastBuyUnitPrice != null ? lastBuyUnitPrice : 1,
                    SellUnitPrice = lastSellUnitPrice != null ? lastSellUnitPrice : 1,
                    
                };
                product_edited0 = product_edited;

                dbProductsList.Add(product_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbProductsList;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the product to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    // style the id cell of the new product
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                },
                DispatcherPriority.Background); // Background to avoid row = null error

                edit_mode = "insert";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                TextBlock_message.Text = "Add product.";
                TextBlock_message.Foreground = Brushes.White;
            }
            else
            {
                MessageBox.Show("Please fill in all product data, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
        StockService.Product product_edited, product_edited0, product_filter;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // exit insert mode if 'Update product' is clicked
                if (Button_UpdateProduct.IsKeyboardFocused)
                {
                    edit_mode = "read";
                    dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                    dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                    dbProductsList.RemoveAt(dbProductsList.Count - 1);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbProductsList;
                    UpdateProduct();
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
                //product_edited = row.Item as StockService.Product; //  product_edited and product_edited0 are already defined in UpdateProduct and AddProduct (read out current (old) values from the row, because the entry is a new value)

                cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
                textBox = (TextBox)cell.Content;
                new_value = textBox.Text;

                changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
                if (changed_property_name == "Purchase price") { changed_property_name = "BuyUnitPrice"; }
                if (changed_property_name == "Sales price") { changed_property_name = "SellUnitPrice"; }
                // get old property value of product by property name
                // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
                old_value = product_edited.GetType().GetProperty(changed_property_name).GetValue(product_edited).ToString();

                // check data correctness
                string stopMessage = "";
                if (new_value == "") // if new value is empty
                {
                    stopMessage = "New value cannot be empty!";
                }
                else
                if (changed_property_name == "Name" && new_value != old_value && dbProductsList.Any(p => p.Name == new_value)) // stop if product already exists in database, AND if new name is different
                {
                    stopMessage = $"The name '{new_value}' is already taken, please enter another name!";
                }
                else if (changed_property_name == "Name" && new_value.Length < 5)
                {
                    stopMessage = $"The name must be at least 5 charachters long!";
                }
                else if (changed_property_name.Contains("price")) // if wrong BuyUnitPrice or SellUnitPrice value is entered
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || int_val <= 0)
                    {
                        stopMessage = $"Please enter a correct value for the price!";
                    }
                    else if (int_val > 10000000)
                    {
                        stopMessage = $"Price cannot exceed 10,000,000!";
                    }
                }

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
                        Button_AddProduct.Focus();

                        SelectTextBox();

                        // restore event handler
                        (sender as DataGrid).CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                    }, DispatcherPriority.Loaded);
                    return;
                }
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Name, BuyUnitPrice, SellUnitPrice can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index - 1] == 1 || edit_mode == "update")) // && column_index < 4
                {
                    MoveToNextCell();
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


                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index == 1) // // update string-type fields with new value (Name)
                {
                    product_edited.GetType().GetProperty(changed_property_name).SetValue(product_edited, new_value);
                }
                else // update int?-type fields with new value (BuyUnitPrice, SellUnitPrice)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    product_edited.GetType().GetProperty(changed_property_name).SetValue(product_edited, Convert.ToInt32(int_val));

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
                            hostMessage = stockClient.AddProduct(Shared.uid, product_edited.Name, product_edited.BuyUnitPrice.ToString(), product_edited.SellUnitPrice.ToString());
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "Product successfully added!")
                            {
                                MessageBox.Show(hostMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                product_edited = product_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            hostMessage = stockClient.UpdateProduct(Shared.uid, product_edited.Id.ToString(), product_edited.Name, product_edited.BuyUnitPrice.ToString(), product_edited.SellUnitPrice.ToString());
                            if (hostMessage != "Product successfully updated!")
                            {
                                MessageBox.Show(hostMessage + " Field was not updated.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                product_edited = product_edited0;
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
                        lastNameIndex++; // running number suffix to Product name 
                        lastName = lastNameIndex == 2 ? product_edited.Name + " " + lastNameIndex : product_edited.Name.Substring(0, product_edited.Name.Length - lastNameIndex.ToString().Length - 1) + " " + lastNameIndex; // save last data to suggest them for next record
                        lastBuyUnitPrice = product_edited.BuyUnitPrice;
                        lastSellUnitPrice = product_edited.SellUnitPrice;

                        // set background color of added product to green
                        for (int i = 0; i < dataGrid1.Columns.Count; i++)
                        {
                            cell = dataGrid1.Columns[i].GetCellContent(row).Parent as DataGridCell;
                            cell.Background = Brushes.OliveDrab;
                        }
                        TextBlock_message.Text = $"The product '{product_edited.Name}' has been added.";
                        Array.Clear(fieldsEntered, 0, fieldsEntered.Length);
                        Log("insert"); // write log to file
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() =>
                        {
                            Button_AddProduct.Focus(); // set focus to allow repeatedly add product on pressing the Add product button
                        },
                        DispatcherPriority.Loaded);

                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The product '{product_edited.Name}' has been updated with {(changed_property_name == "BuyUnitPrice" ? "Purchase price" : changed_property_name == "SellUnitPrice" ? "Sales price" : changed_property_name)  }.";
                        Log("update"); // write log to file
                        cell.Background = Brushes.OliveDrab;
                        // Shared.ChangeColor(cell, Colors.OliveDrab, Colors.Transparent);
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
                    dataGrid1.Dispatcher.InvokeAsync(() =>
                    {

                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                        // select next unchanged column; if last 'SellUnitPrice' column is reached, return to first 'Name' column
                        int column_shift = 0;
                        while (fieldsEntered[column_index + column_shift - 1] != 0)
                        {
                            column_shift = column_index + column_shift == 3 ? -column_index + 1 : column_shift + 1;
                        }
                        cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                        // turn off eventual editing mode caused e.g. by tab key on data entry
                        if (cell.IsEditing) { cell.IsEditing = false; }

                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        Button_AddProduct.Focus();

                        SelectTextBox();



                    },
                DispatcherPriority.Loaded); // style the id cell of the new product
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
            dataGrid1.Dispatcher.InvokeAsync(() =>
            {
                // select next  column; if last 'SellUnitPrice' column is reached, return to first 'Name' column
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
            filteredProductsList = new List<StockService.Product>();

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
            if ((ASCII > 31 && ASCII < 256) == false) { return; } // stop if not number or digit
            // if (ASCII == 43 || ASCII == 60 || ASCII == 61 || ASCII == 62) { return; } // stop if +, <, =, >
            bool key = e.Key == Key.Back;

            // stop on most function keys
            if (e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Oem102 && e.Key != Key.Subtract && ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) == false)
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
            if (new_value != "" && new_value.Length == 1 && (textBox.Text == "<" || textBox.Text == ">" || textBox.Text == "=") && e.Key != Key.Back && e.Key != Key.Delete) { return; } // stop if < or > in empty cell (but continue to recalculate if last key was Key.Back or Key.Delete)
            if (new_value != "" && new_value.Length < 3 && (textBox.Text.Substring(1) == "=")) { return; } // stop if '=' when there are no more characters       

            string firstChar = new_value != "" ? new_value.ToString().Substring(0, 1) : "";
            string op = firstChar != ">" && firstChar != "<" ? "=" : firstChar;
            if (new_value.Length > 1 && new_value.ToString().Substring(1, 1) == "=")
            {
                op = op == ">" ? ">=" : op == "<" ? "<=" : op; // setting >= or <= operator values
            }

            changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();
            if (changed_property_name == "Purchase price") { changed_property_name = "BuyUnitPrice"; }
            if (changed_property_name == "Sales price") { changed_property_name = "SellUnitPrice"; }

            // remove operator for integer columns Id and UnitPrice
            if (changed_property_name == "Id" || changed_property_name == "BuyUnitPrice" || changed_property_name == "SellUnitPrice")
            {
                if (op != "=" || (new_value !="" && new_value.ToString().Substring(0, 1) == "=")) { new_value = new_value.Substring(op.Length); } // remove entered operator

                switch (changed_property_name)
                {
                    case "Id": opId = op; break;
                    case "BuyUnitPrice": opBuyUnitPrice = op; break;
                    case "SellUnitPrice": opSellUnitPrice = op; break;
                    default: break;
                }
            }

            // if any product_filter value is null, set it temporarily to -999 to avoid error when setting old value         
            if (changed_property_name == "Id" && product_filter.Id == null) product_filter.Id = -999;
            if (changed_property_name == "BuyUnitPrice" && product_filter.BuyUnitPrice == null) product_filter.BuyUnitPrice = -999;
            if (changed_property_name == "SellUnitPrice" && product_filter.SellUnitPrice == null) product_filter.SellUnitPrice = -999;
            //get old property value of Product by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = product_filter.GetType().GetProperty(changed_property_name).GetValue(product_filter).ToString();
            if (changed_property_name == "Id" && product_filter.Id == -999) product_filter.Id = null;
            if (changed_property_name == "BuyUnitPrice" && product_filter.BuyUnitPrice == -999) product_filter.BuyUnitPrice = null;
            if (changed_property_name == "SellUnitPrice" && product_filter.SellUnitPrice == -999) product_filter.SellUnitPrice = null;

            string stopMessage = "";
            if (old_value == "-999" || op != "=")
            {
                Dispatcher.InvokeAsync(() => {
                    // for some reason, cursor goes to the front of the cell when inputting into empty integer-type cell; therefore, set cursor to the end; skip if an operator is entered into cell

                    if (op != "=" && stopMessage == "") { textBox.Text = op + new_value; } // restore operator into cell, only if there is no error message (because it restores the old value);

                    Shared.SendKey(Key.End);
                }, DispatcherPriority.Input);
            }

            // check data correctness
            if (changed_property_name == "Id")
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if ((new_value != "" && int_val == null) || (int_val < 0 || int_val > 10000000))
                {
                    stopMessage = $"The Id '{new_value}' does not exist, please enter a correct value for the Id!";
                }
            }
            else if (changed_property_name == "UnitPrice" && new_value != "") // if wrong UnitPrice value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if ((new_value != "" && int_val == null) || int_val < 0)
                {
                    stopMessage = $"The Unit price '{new_value}' does not exist, please enter a correct value for the Unit price!";
                }
                else if (int_val > 10000000)
                {
                    stopMessage = $"Price cannot exceed 10,000,000!";
                }
            }

            if (stopMessage != "")  // warn user, and stop
            {
                MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (old_value != "-999")
                {
                    textBox.Text = op == "=" ? old_value : op + old_value; // restore correct cell value if old value is not null, plus the operator if any
                    Shared.SendKey(Key.End);
                }
                return;
            }

            if (filterc_index == 1 ) // // update string-type fields with new value (Name)
            {
                product_filter.GetType().GetProperty(changed_property_name).SetValue(product_filter, new_value);
            }
            else // update int?-type fields with new value (Id, BuyUnitPrice, SellUnitPrice)
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                product_filter.GetType().GetProperty(changed_property_name).SetValue(product_filter, int_val);

            }

            // filter
            filteredProductsList.Clear();
            foreach (var product in dbProductsList)
            {

                if ((product_filter.Id == null || Compare(product.Id, product_filter.Id, opId)) && (product_filter.Name == "" || product.Name.ToLower().Contains(product_filter.Name.ToLower())) && (product_filter.BuyUnitPrice == null || Compare(product.BuyUnitPrice, product_filter.BuyUnitPrice, opBuyUnitPrice)) && (product_filter.SellUnitPrice == null || Compare(product.SellUnitPrice, product_filter.SellUnitPrice, opSellUnitPrice)))
                {
                    filteredProductsList.Add(product);
                    continue;
                }
            }
            // update dataGrid1 with filtered items                    
            dataGrid1.ItemsSource = filteredProductsList;
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

        private void SetUserAccess()
        {
            // 0-2: view only 3-5: +insert/update 6-8: +delete 9: +user management (admin)
            if (Shared.loggedInUser.Permission < 6)
            {
                Button_DeleteProduct.IsEnabled = false;
                Button_DeleteProduct.Foreground = Brushes.Gray;
                Button_DeleteProduct.ToolTip = "You do not have rights to delete data!";
            }
            if (Shared.loggedInUser.Permission < 3)
            {
                Button_AddProduct.IsEnabled = false;
                Button_AddProduct.Foreground = Brushes.Gray;
                Button_AddProduct.ToolTip = "You do not have rights to add data!";
                Button_UpdateProduct.IsEnabled = false;
                Button_UpdateProduct.Foreground = Brushes.Gray;
                Button_UpdateProduct.ToolTip = "You do not have rights to import data!";
                Button_Import.IsEnabled = false;
                Button_Import.Foreground = Brushes.Gray;
                Button_Import.ToolTip = "You do not have rights to import data!";
            }
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Comma separated text file (*.csv)|*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "dbProducts",
                Title = "Save products data as:"
            };

            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog
            if (result == true)
            {
                // create file content
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName, append: false, encoding: Encoding.UTF8);
                // write file header line
                string header_row = "Id;Name;BuyUnitPrice;SellUnitPrice";
                sr.WriteLine(header_row);

                // write file rows
                string rows = "";
                StockService.Product product;
                int i = 0;
                for (i = 0; i < dataGrid1.Items.Count; i++)
                {
                    product = dataGrid1.Items[i] as StockService.Product;
                    rows += $"{product.Id};{product.Name};{product.BuyUnitPrice};{product.SellUnitPrice}\n";
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
                Title = "Open file for import to 'Products' table"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                // check header correctness
                string header_row = sr.ReadLine();
                int first_colum = header_row.Split(';').Length == 2 ? 0 : 1; // 1 if Id column is provided

                if (header_row != "Id;Name;BuyUnitPrice;SellUnitPrice" && header_row != "Name;BuyUnitPrice;SellUnitPrice")
                {
                    MessageBox.Show($"Incorrect file content! Expected header is 'Id;Name;BuyUnitPrice;SellUnitPrice' (Id is optional), but received '{header_row}'");
                    return;
                }

                StockService.Product product;
                importList = new List<StockService.Product>();
                int row_index = 0;
                string[] row;
                int productsAdded = 0;
                string hostMessage = "";
                string errorMessage = "";
                int? id = dbProductsList.Max(u => u.Id) + 1;
                while (sr.EndOfStream == false)
                {
                    string error = "";
                    row = sr.ReadLine().Split(';');
                    if (row.Length != 3 + first_colum) // skip row if number of columns is incorrect
                    {
                        continue;
                    }

                    string name = row[first_colum];
                    string buyUnitPrice = row[first_colum + 1];
                    string sellUnitPrice = row[first_colum + 2];
                    
                    // check data correctness
                    if (dbProductsList.Any(p => p.Name == name)) // if product already exists in database
                    {
                        error += $"'{name}': Product already exists in database!\n";
                    }
                    if (name.Length < 5)
                    {
                        error += $"'{name}': Name must be et least 5 characters long!\n";
                    }
                    int? int_val = Int32.TryParse(buyUnitPrice, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || int_val <= 0)
                    {
                        error += $"'{name}': BuyUnitPrice '{buyUnitPrice}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"'{name}': BuyUnitPrice '{buyUnitPrice}' cannot exceed 1000,000,000!\n";
                    }
                    int_val = Int32.TryParse(buyUnitPrice, out var tempVal1) ? tempVal1 : (int?)null;
                    if (int_val == null || int_val <= 0)
                    {
                        error += $"'{name}': SellUnitPrice '{sellUnitPrice}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"'{name}': SellUnitPrice '{sellUnitPrice}' cannot exceed 1000,000,000!\n";
                    }

                    errorMessage += error;
                    if (error != "") { continue; } // continue on error

                    // ADD into database
                    hostMessage = stockClient.AddProduct(Shared.uid, name, buyUnitPrice, sellUnitPrice);
                    if (hostMessage == "Unauthorized user!")
                    {
                        Shared.Logout(); // stop on unauthorized user
                        IsEnabled = false;
                        Close();
                        return;
                    }
                    else if (hostMessage != "Product successfully added!")
                    {
                        errorMessage += $"'{name}': {hostMessage}\n";
                        continue;
                    }

                    product = new StockService.Product
                    {
                        Id = id,
                        Name = name,
                        BuyUnitPrice = int.Parse(buyUnitPrice),
                        SellUnitPrice = int.Parse(sellUnitPrice)
                    };

                    productsAdded++;
                    importList.Add(product);
                    id++;
                    row_index++;
                }
                sr.Close();

                if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                if (importList.Count > 0)
                {
                    dataGrid1.ItemsSource = importList;

                    TextBlock_message.Text = $"{productsAdded} {(productsAdded == 1 ? "record" : "records")} added into the 'Products' table.";
                    TextBlock_message.Foreground = Brushes.LightGreen;
                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 - 74) / (550 - 10 - 128), 1); // Border width - left margin - a bit more because first column remains unchanged
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;
            dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

            stackPanel1.Height = 442 + window.ActualHeight - 500 - stackPanel1.Margin.Top + 45; // original window.Height

            // stretch columns to dataGrid1 width
            for (int i = 1; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * stretch;
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                // dataGrid0.Columns[i].MaxWidth = dataGrid1.Columns[i].ActualWidth * stretch;
            }
            dataGrid1.FontSize = 14 * Math.Max(stretch * 0.95, 0.9);
        }

        private void Log(string operation)
        {
            string row = "";
            // save operation into log file
            StreamWriter sr = new StreamWriter(@".\Logs\manageProducts.log", append: true, encoding: Encoding.UTF8);
            // write file header line
            // string header_row = "LogDate;LogUsername;LogOperation;Id;Name;BuyUnitPrice;SellUnitPrice";
            // sr.WriteLine(header_row);

            // write file rows
            StockService.Product product;
            product = product_edited;

            if (operation == "update") // in update mode add the old value in a new line
            {
                int index = column_index;
                row = $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{product.Id};{(column_index == 1 ? old_value : null)};{(column_index == 2 ? old_value : null)};{(column_index == 3 ? old_value : null)}\n";
            }

            row += $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{product.Id};{product.Name};{product.BuyUnitPrice};{product.SellUnitPrice}";
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

        Logs.LogWindowProducts LogWindowProducts;
        private void Button_LogWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(LogWindowProducts))
            {
                LogWindowProducts = new Logs.LogWindowProducts();
                if (LogWindowProducts.IsEnabled) LogWindowProducts.Show();
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


