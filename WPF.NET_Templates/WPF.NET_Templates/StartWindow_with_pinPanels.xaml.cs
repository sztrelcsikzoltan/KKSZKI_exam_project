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
using System.Windows.Shapes;

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for StartWindow_with_pinPanels.xaml
    /// </summary>
    public partial class StartWindow_with_pinPanels : Window
    {
        // Dummy columns for layers 0 and 1:
        ColumnDefinition colOneCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer1;

        private LoginWindow loginWindow;

        public StartWindow_with_pinPanels()
        {
            InitializeComponent();

            // Initialize the dummy (grouped) columns that are created during docking:
            colOneCopyForLayer0 = new ColumnDefinition();
            colOneCopyForLayer0.SharedSizeGroup = "column1";
            colTwoCopyForLayer0 = new ColumnDefinition();
            colTwoCopyForLayer0.SharedSizeGroup = "column2";
            colTwoCopyForLayer1 = new ColumnDefinition();
            colTwoCopyForLayer1.SharedSizeGroup = "column2";

            button_login.Foreground = Brushes.Red;
            button_login.FontWeight = FontWeights.Bold;
        }

        // Toggle panel 1 between docked and undocked states
        public void panel1Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Collapsed)
                UndockPane(1);
            else
                DockPane(1);
        }

        // Toggle panel 2 between docked and undocked states 
        public void panel2Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel2.Visibility == Visibility.Collapsed)
                UndockPane(2);
            else
                DockPane(2);
        }

        // Make panel 1 visible when hovering over its button
        public void button_panel1_MouseEnter(object sender, RoutedEventArgs e)
        {
            gridlayer1.Visibility = Visibility.Visible;

            // Adjust ZIndex order to ensure the pane is on top:
            parentGrid.Children.Remove(gridlayer1);
            parentGrid.Children.Add(gridlayer1);

            // Ensure the other pane is hidden if it is undocked
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Make panel 2 visible when hovering over its button
        public void button_panel2_MouseEnter(object sender, RoutedEventArgs e)
        {
            gridlayer2.Visibility = Visibility.Visible;

            // Adjust ZIndex order to ensure the pane is on top:
            parentGrid.Children.Remove(gridlayer2);
            parentGrid.Children.Add(gridlayer2);

            // Ensure the other pane is hidden if it is undocked
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
        }

        // Hide any undocked panes when the mouse enters Layer 0
        public void layer0_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 1
        public void panel1_MouseEnter(object sender, RoutedEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 2
        public void panel2_MouseEnter(object sender, RoutedEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
        }

        // Docks a pane and hides its button
        public void DockPane(int paneNumber)
        {
            if (paneNumber == 1)
            {
                button_panel1.Visibility = Visibility.Collapsed;
                panel1PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinVer1col.png", UriKind.Relative));

                // Add the cloned column to layer 0:
                layer0.ColumnDefinitions.Add(colOneCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 2 is docked:
                if (button_panel2.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(colTwoCopyForLayer1);
            }
            else if (paneNumber == 2)
            {
                button_panel2.Visibility = Visibility.Collapsed;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinVer1col.png", UriKind.Relative));

                // Add the cloned column to layer 0:
                layer0.ColumnDefinitions.Add(colTwoCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 1 is docked:
                if (button_panel1.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(colTwoCopyForLayer1);
            }
        }

        // Undocks a pane, which reveals the corresponding pane button
        public void UndockPane(int panelNbr)
        {
            if (panelNbr == 1)
            {
                gridlayer1.Visibility = Visibility.Collapsed;
                button_panel1.Visibility = Visibility.Visible;
                panel1PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinHor1col.png", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                layer0.ColumnDefinitions.Remove(colOneCopyForLayer0);
                // This won't always be present, but Remove silently ignores bad columns:
                gridlayer1.ColumnDefinitions.Remove(colTwoCopyForLayer1);
            }
            else if (panelNbr == 2)
            {
                gridlayer2.Visibility = Visibility.Collapsed;
                button_panel2.Visibility = Visibility.Visible;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinHor1col.png", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                layer0.ColumnDefinitions.Remove(colTwoCopyForLayer0);
                gridlayer1.ColumnDefinitions.Remove(colTwoCopyForLayer1);
            }
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_login_Click(object sender, RoutedEventArgs e)
        {
            loginWindow = new LoginWindow();
            loginWindow.Show();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
