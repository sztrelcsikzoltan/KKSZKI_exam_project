﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class StockLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public DateTime? LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public int? Id { set; get; }
        public string Product { set; get; }
        public string Stock { set; get; }
        public int? Quantity { set; get; }
        public string Location { set; get; }

        public StockLog()
        {
        }

        public StockLog(DateTime? logDate, string logUsername, string logOperation, int id, string product, int? quantity, string location)
        {
            LogDate = logDate;
            LogUsername = logUsername;
            LogOperation = logOperation;
            Id = id;
            Product = product;
            Quantity = quantity;
            Location = location;
        }

        // returns a list of stocks log
        public static List<StockLog> GetStocksLog(DateTime startDate, DateTime endDate)
        {
            string filename = @".\Logs\manageStocks.log";

            StreamReader sr = new StreamReader(filename);
            // check header correctness
            string header_row = sr.ReadLine();
            if (header_row != "LogDate;LogUsername;LogOperation;Id;Product;Quantity;Location")
            {
                MessageBox.Show($"Incorrect file content! Expected header is 'LogDate;LogUsername;LogOperation;Id;Product;Quantity;Location', but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            StockLog stockLog;
            List<StockLog> logList = new List<StockLog>();
            List<UserService.User> dbUsersList = new List<UserService.User>();

            int row_index = 0;
            string[] row;
            int logAdded = 0;
            string errorMessage = "";

            while (sr.EndOfStream == false)
            {
                row_index++;
                string error = "";
                row = sr.ReadLine().Split(';');
                if (row.Length != 7) // skip row if number of columns is incorrect
                {
                    errorMessage += $"Stock in line {row_index}: The required {7} fields are not available!\n";
                    continue;
                }

                string logDate = row[0];
                string logUsername = row[1];
                string logOperation = row[2];
                string id = row[3];
                string product = row[4];
                string quantity = row[5];
                string location = row[6];

                // check data correctness
                bool dateExists = DateTime.TryParse(logDate, out DateTime date_entered);
                if (dateExists == false)
                {
                    error += $"Stock in line {row_index}: Log date value '{logDate}' is incorrect!\n";
                }
                else if (date_entered > DateTime.Now)
                {
                    error += $"Stock in line {row_index}: Date '{logDate}' cannot be a future date!\n";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    error += $"Stock in line {row_index}: Date '{logDate}' cannot be earlier than 3650 days!\n";
                }
                if (logOperation != "update")
                {
                    if (product.Length < 5) // if wrong product name value is entered
                    {
                        error += $"Stock in line {row_index}: Product name must be at least 5 characters!\n";
                    }
                    int? int_val = Int32.TryParse(quantity, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"Stock in line {row_index}: Quantity '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000)
                    {
                        error += $"Stock in line {row_index}: Quantity '{quantity}' cannot exceed 1,000,000!\n";
                    }
                    if (location.Length < 3) // if wrong location name value is entered
                    {
                        error += $"Stock in line {row_index}: Location name must be at least 3 characters!\n";
                    }

                }
                errorMessage += error;


                if (error != "") { continue; } // skip on error
                if (DateTime.Parse(logDate) < startDate || DateTime.Parse(logDate) > endDate) { continue; } // skip log date before and after limit dates

                int? quantityVal = Int32.TryParse(quantity, out var tempVal1) ? tempVal1 : (int?)null;
                stockLog = new StockLog
                {
                    LogDate = DateTime.Parse(logDate),
                    LogUsername = logUsername,
                    LogOperation = logOperation,
                    Id = int.Parse(id),
                    Product = product,
                    Quantity = quantityVal,
                    Location = location,
                };
                logAdded++;
                logList.Add(stockLog);

            }
            sr.Close();

            if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }

            return logList;
        }

    }
}