﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FrontendWPF.Classes;


namespace FrontendWPF.Logs
{
    public partial class LogWindowProducts : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<ProductLog> logList { get; set; }

        System.Collections.IList selectedItems;
        List<ProductLog> filterProductsList { get; set; }
        List<ProductLog> filteredProductsList { get; set; }

        string edit_mode;
        private List<Product> productsList { get; set; }
        private int[] fieldsEntered = new int[6]; // LogDate, LogUser, LogOperation, (product) Name, BuyUnitPrice, SellUnitPrice
        ScrollViewer scrollViewer;
        string input = "";
        string opLogDate = "=";
        string opId = "=";
        string opBuyUnitPrice = "=";
        string opSellUnitPrice = "=";
        bool pickStartDate = false;
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;

        DateTime startDate = DateTime.Now.Date.AddDays(-30); // set an initial limit of 29 days
        DateTime endDate = DateTime.Now; //
        // public double columnFontSize { get; set; }

        public LogWindowProducts()
        {
            InitializeComponent();
            logList = new List<ProductLog>();
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

            //  https://www.codeproject.com/Questions/155935/how-to-add-rows-to-WPF-datagrids
            /*
            DataGridTextColumn col1 = new DataGridTextColumn();
            dataGrid1.Columns.Add(col1);
            col1.Binding = new Binding("id");
            col1.Header = "ID";
            */

            // get log file content
            logList = ProductLog.GetProductsLog(startDate, endDate);
            if (logList == null) { IsEnabled = false; Close(); return; } // stop on any error
            dataGrid1.ItemsSource = logList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            TextBlock_message.Text = $"{logList.Count} records loaded.";

            // close window and stop if no purchase is retrieved
            /*
            if (logList.Count == 0)
            {
                closeCompleted = true;
                IsEnabled = false; // so that window cannot be opened
                Close();
                return;
            }
            */

            // if (window.IsLoaded == false) // run on the first time when window is not loaded
            if (true)
            {
                filterProductsList = new List<ProductLog>();

                Dispatcher.InvokeAsync(() =>
                {
                    double stretch = Math.Max((borderLeft.ActualWidth - 10 - 140) / (650 - 10 - 140), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
                    dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                    dataGrid0.Width = dataGrid1.Width;

                    stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Height

                    // stretch columns to dataGrid1 width
                    for (int i = 0; i < dataGrid1.Columns.Count; i++)
                    {
                        dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 3 || i == 5 || i == 6 ? 0.5 : 1) + 1); // resize Quantity row only by 50%
                        dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                    }
                    dataGrid1.FontSize = 12 * Math.Min(stretch, 1.001); // reset font size to initial stretch value on large window width
                    dataGrid1.Items.Refresh();
                    ScrollDown();
                    selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
                }, DispatcherPriority.Loaded);
            }
            ScrollDown();

            // create/reset product_filter item and add it to filter dataGrid0
            product_filter = new ProductLog()
            {
                LogDate = null,
                LogUsername = "",
                LogOperation = "",
                Id = null,
                Name = "",
                BuyUnitPrice = null,
                SellUnitPrice = null,
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

        private void Button_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_message.Text = "Delete log file's content?";
            TextBlock_message.Foreground = Brushes.Salmon;
            MessageBoxResult result = MessageBox.Show("Are you sure to delete all records in the the log file?", caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // DELETE log file's content

                    StreamWriter sw = new StreamWriter(@".\Logs\manageProducts.log", append: false, encoding: Encoding.UTF8);
                    sw.WriteLine("LogDate;LogUsername;LogOperation;Id;Name;BuyUnitPrice;SellUnitPrice");
                    sw.Close();
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
                TextBlock_message.Text = "Log file's content deleted.";

                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // show gifImage
                gifImage.StartAnimation();
                MessageBox.Show("Log file's content deleted.", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                ReloadData();
            }
            else
            {
                TextBlock_message.Text = "";
                TextBlock_message.Foreground = Brushes.White;
            }
        }


        DataGridRow row0;
        DataGridColumn column0;
        DataGridCell cell;
        TextBox textBox;
        string old_value;
        string new_value = "";
        int filterc_index;
        string changed_property_name;
        ProductLog product_filter;


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
            filteredProductsList = new List<ProductLog>();

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
            row0 = e.Row;
            column0 = e.Column;
        }

        private void dataGrid0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text; // get character entered
        }

        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        // private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row0 == null || column0 == null) { return; } // stop if column or row is not selected or not in edit mode

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

            filterc_index = column0.DisplayIndex;
            cell = dataGrid1.Columns[filterc_index].GetCellContent(row0).Parent as DataGridCell;
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
            if (changed_property_name == "date") { changed_property_name = "LogDate"; }
            if (changed_property_name == "user name") { changed_property_name = "LogUsername"; }
            if (changed_property_name == "operation") { changed_property_name = "LogOperation"; }
            if (changed_property_name == "Purchase price" || changed_property_name == "Purchase pr") { changed_property_name = "BuyUnitPrice"; }
            if (changed_property_name == "Sales price") { changed_property_name = "SellUnitPrice"; }

            // remove operator for integer columns Id and UnitPrice
            if (changed_property_name == "LogDate" || changed_property_name == "Id" || changed_property_name == "BuyUnitPrice" || changed_property_name == "SellUnitPrice")
            {
                if (op != "=" || (new_value != "" && new_value.ToString().Substring(0, 1) == "=")) { new_value = new_value.Substring(op.Length); } // remove entered operator

                switch (changed_property_name)
                {
                    case "LogDate": opLogDate = op; break;
                    case "Id": opId = op; break;
                    case "BuyUnitPrice": opBuyUnitPrice = op; break;
                    case "SellUnitPrice": opSellUnitPrice = op; break;
                    default: break;
                }
            }

            if (changed_property_name == "LogDate" && ((new_value.Length < 8 && new_value.Length > 0) || (new_value.Length > 8 && new_value.Length < 14))) { return; } // stop if date length is < 8 OR when time is edited (a character is deleted), otherwise product_filter.Date will be set to null

            // if any product_filter value is null, set it temporarily to -999 to avoid error when setting old value 
            if (changed_property_name == "LogDate" && product_filter.LogDate == null) product_filter.LogDate = DateTime.Parse("01.01.01 01:01:01");
            if (changed_property_name == "Purchase price" || changed_property_name == "Purchase pr") { changed_property_name = "BuyUnitPrice"; }
            if (changed_property_name == "Sales price") { changed_property_name = "SellUnitPrice"; }
            if (changed_property_name == "Id" && product_filter.Id == null) product_filter.Id = -999;
            if (changed_property_name == "BuyUnitPrice" && product_filter.BuyUnitPrice == null) product_filter.BuyUnitPrice = -999;
            if (changed_property_name == "SellUnitPrice" && product_filter.SellUnitPrice == null) product_filter.SellUnitPrice = -999;

            //get old property value of purchase by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = product_filter.GetType().GetProperty(changed_property_name).GetValue(product_filter).ToString();
            if (changed_property_name == "LogDate" && product_filter.LogDate == DateTime.Parse("01.01.01 01:01:01")) product_filter.LogDate = null;
            if (changed_property_name == "Id" && product_filter.Id == -999) product_filter.Id = null;
            if (changed_property_name == "BuyUnitPrice" && product_filter.BuyUnitPrice == -999) product_filter.BuyUnitPrice = null;
            if (changed_property_name == "SellUnitPrice" && product_filter.SellUnitPrice == -999) product_filter.SellUnitPrice = null;

            string stopMessage = "";
            if (old_value == "-999" || op != "=")
            {
                Dispatcher.InvokeAsync(() =>
                {
                    SelectTextBox(); // this + Background priority needed to avoid wrong Key.End selection
                }, DispatcherPriority.Input);
                Dispatcher.InvokeAsync(() =>
                {
                    // for some reason, cursor goes to the front of the cell when inputting into empty integer-type cell; therefore, set cursor to the end; skip if an operator is entered into cell

                    if (op != "=" && stopMessage == "") { textBox.Text = op + new_value; } // restore operator into cell, only if there is no error message (because it restores the old value);

                    Shared.SendKey(Key.End);
                }, DispatcherPriority.Background);
            }
            void SelectTextBox()
            {
                cell.Focus();
                cell.IsEditing = true;

                cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move focus to textBox
                cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                textBox = (TextBox)cell.Content;
                // Keyboard.Focus(textBox);
                textBox.SelectAll();
            }

            // check data correctness
            bool minutesExist = false;
            if (changed_property_name == "Id")
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if ((new_value != "" && int_val == null) || (int_val < 0 || int_val > 10000000))
                {
                    stopMessage = $"The Id '{new_value}' does not exist, please enter a correct value for the Id!";
                }
            }
            else if (new_value != "" && changed_property_name == "BuyUnitPrice") // if wrong Active value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val < 0))
                {
                    stopMessage = $"Please enter a correct value for the Purchase price!";
                }
                else if (int_val > 1000000000)
                {
                    stopMessage = $"Purchase price cannot exceed 1,000,000,000!";
                }
            }
            else if (new_value != "" && changed_property_name == "SellUnitPrice") // if wrong Active value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val < 0))
                {
                    stopMessage = $"Please enter a correct value for the Sales price!";
                }
                else if (int_val > 1000000000)
                {
                    stopMessage = $"Sales price cannot exceed 1,000,000,000!";
                }
            }
            else if (new_value != "" && (changed_property_name == "LogDate" || changed_property_name == "Date")) // if wrong Date value is entered
            {
                old_value = old_value.Substring(0, old_value.Length - 3);
                bool dateExists = DateTime.TryParse(new_value, out DateTime date_entered);
                if (dateExists == false)
                {
                    stopMessage = $"Please enter a correct value for the date value!";
                }
                else
                {
                    // checks if minutes are entered; if not, minutues in the record will be ignored
                    minutesExist = date_entered.Minute > 0;
                }
            }

            if (stopMessage != "")  // warn user, and stop
            {
                MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (old_value != "-999" && old_value != "01.01.01 01:01")
                {
                    textBox.Text = op == "=" ? old_value : op + old_value; // restore correct cell value if old value is not null, plus the operator if any
                    Shared.SendKey(Key.End);
                }
                return;
            }

            if (filterc_index == 1 || filterc_index == 2 || filterc_index == 4) // // update string-type fields with new value ( LogUsername, LogOperation, Name)
            {
                product_filter.GetType().GetProperty(changed_property_name).SetValue(product_filter, new_value);
            }
            else if (filterc_index == 0) // update LogDate with new value
            {
                DateTime? int_val = DateTime.TryParse(new_value, out var tempVal) ? tempVal : (DateTime?)null;
                product_filter.GetType().GetProperty(changed_property_name).SetValue(product_filter, int_val);
            }
            else // update int?-type fields with new value (Id, BuyUnitPrice, SellUnitPrice)
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                product_filter.GetType().GetProperty(changed_property_name).SetValue(product_filter, int_val);
            }

            // filter
            filteredProductsList.Clear();
            foreach (var product in logList)
            {
                DateTime roundedLogDate = (DateTime)product.LogDate;
                roundedLogDate = roundedLogDate.AddSeconds(-roundedLogDate.Second);
                if ((product_filter.LogDate == null || (minutesExist ? Compare(roundedLogDate, product_filter.LogDate, opLogDate) : Compare(product.LogDate.Value.Date, product_filter.LogDate, opLogDate))) && (product_filter.LogUsername == "" || product.LogUsername.ToLower().Contains(product_filter.LogUsername.ToLower())) && (product_filter.LogOperation == "" || product.LogOperation.ToLower().Contains(product_filter.LogOperation.ToLower())) && (product_filter.Id == null || Compare(product.Id, product_filter.Id, opId)) && (product_filter.Name == "" || product.Name.ToLower().Contains(product_filter.Name.ToLower())) && (product_filter.BuyUnitPrice == null || Compare(product.BuyUnitPrice, product_filter.BuyUnitPrice, opBuyUnitPrice)) && (product_filter.SellUnitPrice == null || Compare(product.SellUnitPrice, product_filter.SellUnitPrice, opSellUnitPrice)))
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
                Button_DeleteProduct.IsEnabled = false;
                Button_DeleteProduct.Foreground = Brushes.Gray;
                Button_DeleteProduct.ToolTip = "You do not have rights to delete data!";
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
                    //endDate = endDate.ToString().Substring(10, 8) == "00:00:00" ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
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

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 - 140) / (650 - 10 - 140), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;
            // dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

            stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Heigth

            // stretch columns to dataGrid1 width
            for (int i = 0; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 3 || i == 5 || i == 6 ? 0.5 : 1) + 1); // resize Id and BuyUnitPrice and SellUnitPrice only by 50%
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
            }
            dataGrid1.FontSize = 12 * Math.Max(stretch, 1);
            dataGrid1.Columns[5].Header = stretch < 1.15 ? "Purchase price" : "Purchase pr.";
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

        ProductLog productlog;
        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row; int rowIndex = row.GetIndex();
            productlog = dataGrid1.Items[row.GetIndex()] as ProductLog;
            if (productlog.SellUnitPrice == null || productlog.BuyUnitPrice == null)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else if (productlog.LogOperation == "insert")
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (productlog.LogOperation == "delete")
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Salmon);
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.Dispatcher.InvokeAsync(async () => {
                await Task.Delay(2000);
                // false is needed to display colored rows properly
                dataGrid1.EnableRowVirtualization = false; // must be delayed, otherwise animation does not work properly
            }, DispatcherPriority.SystemIdle);
        }
    }
}


