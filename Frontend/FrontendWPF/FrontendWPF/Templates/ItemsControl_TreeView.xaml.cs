﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrontendWPF.Templates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ItemsCotrol_TreeView : Window
    {
        public ItemsCotrol_TreeView()
        {
            InitializeComponent();
        }

        private void chkIncludeGA(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeItem = new TreeViewItem();

            treeItem.Header = "Georgia";

            treeItem.Items.Add(new TreeViewItem() { Header = "Atlanta" });
            treeItem.Items.Add(new TreeViewItem() { Header = "Athens" });
            treeItem.Items.Add(new TreeViewItem() { Header = "Augusta" });

            tvStates.Items.Add(treeItem);


        }
    }
}
