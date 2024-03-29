﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class Product
    {
        // class requires getter-setter to be visible in DataGrid!
        public int? Id { set; get; }
        public string Name { set; get; }
        public int BuyUnitPrice { set; get; }
        public int SellUnitPrice { set; get; }

        public Product()
        {
        }

        public Product(int id, string name, int buyUnitPrice, int sellUnitPrice)
        {
            Id = id;
            Name = name;
            BuyUnitPrice = buyUnitPrice;
            SellUnitPrice = sellUnitPrice;
        }

        // returns a list of products
        public static List<StockService.Product> GetProducts(string id, string name, string buyOver, string buyUnder, string sellOver, string sellUnder, string limit)
        {
            StockService.StockServiceClient client = new StockService.StockServiceClient();

            StockService.Product[] productsArray;
            List<StockService.Product> productsList = new List<StockService.Product>();
 
            try
            {
                string hostMessage = client.ListProduct(Shared.uid, id, name, buyOver, buyUnder, sellOver, sellUnder, limit).Message;
                if (hostMessage.Contains("Unable to connect"))
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else if (hostMessage == "Unauthorized user!")
                {
                    Shared.Logout();
                    return null;
                }
                else
                {
                    // string query = $"WHERE name='{productName}' AND unitPrice='{CreateMD5(unitprice)}'";
                    productsArray = client.ListProduct(Shared.uid, id, name, buyOver, buyUnder, sellOver, sellUnder, limit).Products;
                    // UserService.Response_Product response_Product = new UserService.Response_Product();
                    // string uid = response_Product.Uid;
                    productsList = productsArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    return null;
                }
            }
            // return product.ToList();
            return productsList;
            
        }
        
    }
}