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
using WPF.NET_Templates.Classes;

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            Shared.startWindow_With_PinPanels = new StartWindow_with_pinPanels();
            Shared.startWindow_With_PinPanels.Show();
            Close();
        }



    }
}