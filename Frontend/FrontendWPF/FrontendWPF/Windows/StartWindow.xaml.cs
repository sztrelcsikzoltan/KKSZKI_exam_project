using FrontendWPF.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FrontendWPF.Windows
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        // Dummy columns for layers 0 and 1:
        private readonly ColumnDefinition _colOneCopyForLayer0;
        private readonly ColumnDefinition _colTwoCopyForLayer0;
        private readonly ColumnDefinition _colTwoCopyForLayer1;
        private bool _closeCompleted = false;

        private readonly List<BitmapImage> _imagesList = new List<BitmapImage>();
        private int imageNumber = 0;
        public DispatcherTimer pictureTimer = new DispatcherTimer();

        public StartWindow()
        {
            InitializeComponent();

            Shared.screenWidth = System.Windows.SystemParameters.WorkArea.Width;
            Shared.screenHeight = System.Windows.SystemParameters.WorkArea.Height;

            // Initialize the dummy (grouped) columns that are created during docking:
            _colOneCopyForLayer0 = new ColumnDefinition
            {
                SharedSizeGroup = "column1"
            };
            _colTwoCopyForLayer0 = new ColumnDefinition
            {
                SharedSizeGroup = "column2"
            };
            _colTwoCopyForLayer1 = new ColumnDefinition
            {
                SharedSizeGroup = "column2"
            };
        }

        // Toggle panel 1 between docked and undocked states
        public void Panel1Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Collapsed)
                UndockPane(1);
            else
                DockPane(1);
        }

        // Toggle panel 2 between docked and undocked states 
        public void Panel2Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel2.Visibility == Visibility.Collapsed)
                UndockPane(2);
            else
                DockPane(2);
        }

        // Make panel 1 visible when hovering over its button
        public void Button_panel1_MouseEnter(object sender, RoutedEventArgs e)
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
        public void Button_panel2_MouseEnter(object sender, RoutedEventArgs e)
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
        public void Layer0_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 1
        public void Panel1_MouseEnter(object sender, RoutedEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 2
        public void Panel2_MouseEnter(object sender, RoutedEventArgs e)
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
                layer0.ColumnDefinitions.Add(_colOneCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 2 is docked:
                if (button_panel2.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(_colTwoCopyForLayer1);
            }
            else if (paneNumber == 2)
            {
                button_panel2.Visibility = Visibility.Collapsed;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinVer1col.png", UriKind.Relative));

                // Add the cloned column to layer 0:
                layer0.ColumnDefinitions.Add(_colTwoCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 1 is docked:
                if (button_panel1.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(_colTwoCopyForLayer1);
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
                layer0.ColumnDefinitions.Remove(_colOneCopyForLayer0);
                // This won't always be present, but Remove silently ignores bad columns:
                gridlayer1.ColumnDefinitions.Remove(_colTwoCopyForLayer1);
            }
            else if (panelNbr == 2)
            {
                gridlayer2.Visibility = Visibility.Collapsed;
                button_panel2.Visibility = Visibility.Visible;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinHor1col.png", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                layer0.ColumnDefinitions.Remove(_colTwoCopyForLayer0);
                gridlayer1.ColumnDefinitions.Remove(_colTwoCopyForLayer1);
            }
        }


        private LoginWindow loginWindow = new LoginWindow();
        private void Button_login_Click(object sender, RoutedEventArgs e)
        {
            // open loginwindow if it logRegWindowsClosed and user not logged in    
            if (loginWindow.IsLoaded == false && button_login.Content.ToString() == "LOGIN")
            {
                loginWindow = new LoginWindow();
                loginWindow.Show();
                pictureTimer.Stop();
            }
            else // LOGOUT
            {
                button_login.Content = "LOGIN";
                button_login.Foreground = Brushes.LightSalmon;
                Shared.loggedInUser = null;
                Shared.loggedIn = false;
                button_ManageUsersWindow.IsEnabled = false;
                button_ManageUsersWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF9D9D9D");
                button_ManageProductsWindow.IsEnabled = false;
                button_ManageProductsWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                button_ManagePurchasesWindow.IsEnabled = false;
                button_ManagePurchasesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                button_ManageSalesWindow.IsEnabled = false;
                button_ManageSalesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C8C8C");
                button_ManageStocksWindow.IsEnabled = false;
                button_ManageStocksWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF868686");
                button_ManageLocationsWindow.IsEnabled = false;
                button_ManageLocationsWindow.Foreground = Brushes.Gray;
                button_Layouts.IsEnabled = false;
                button_Layouts.Foreground = Brushes.Gray;

                if (gridLayout.Visibility == Visibility.Visible)
                {
                    gridLayout.Visibility = Visibility.Hidden;
                    button_Layouts.Content = "Show layouts";
                    imageBackground.Visibility = Visibility.Visible;
                }

                pictureTimer.Start();


                UserService.UserServiceClient client = new UserService.UserServiceClient();
                
                try
                {
                    string responseLogout = client.LogoutUser(Shared.uid);

                    string errorMessage = "";
                    if (responseLogout.Contains("You have logged out!"))
                    {
                        MessageBox.Show("You have logged out.", caption: "Information", button: MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        errorMessage = responseLogout;
                    }

                    if (errorMessage != "")
                    {
                        MessageBox.Show(errorMessage, caption: "Error message");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("no endpoint listening") || ex.ToString().Contains("EndpointNotFoundException"))
                    {
                        MessageBox.Show("The remote host is not accessible. Please check your Internet connection, or contact the service provider.", caption: "Error message");
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"Logout failed! The details of the error are the following:\n{ex.Message}", caption: "Error message");
                        return;
                    }

                }

            }

        }

        private ManageUsersWindow _manageUsersWindow;
        private void Button_ManageUsersWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageUsersWindow))
            {
                _manageUsersWindow = new ManageUsersWindow();
                if (_manageUsersWindow.IsEnabled) _manageUsersWindow.Show();
            }
        }

        private ManageProductsWindow _manageProductsWindow;
        private void Button_ManageProductsWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageProductsWindow))
            {
                _manageProductsWindow = new ManageProductsWindow();
                if (_manageProductsWindow.IsEnabled) _manageProductsWindow.Show();
            }
        }

        ManagePurchasesWindow _managePurchasesWindow;
        private void Button_ManagePurchasesWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_managePurchasesWindow))
            {
                _managePurchasesWindow = new ManagePurchasesWindow();
                if (_managePurchasesWindow.IsEnabled) _managePurchasesWindow.Show();
            }
        }

        private ManageSalesWindow _manageSalesWindow;
        private void Button_ManageSalesWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageSalesWindow))
            {
                _manageSalesWindow = new ManageSalesWindow();
                if (_manageSalesWindow.IsEnabled) _manageSalesWindow.Show();
            }
        }

        private ManageStocksWindow _manageStocksWindow;
        private void Button_ManageStocksWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageStocksWindow))
            {
                _manageStocksWindow = new ManageStocksWindow();
                if (_manageStocksWindow.IsEnabled) _manageStocksWindow.Show();
            }
        }

        private ManageLocationsWindow _manageLocationsWindow;
        private void Button_ManageLocationsWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageLocationsWindow))
            {
                _manageLocationsWindow = new ManageLocationsWindow();
                if (_manageLocationsWindow.IsEnabled) _manageLocationsWindow.Show();
            }
        }

         private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }


        private void CloseWindow()
        {
            button_Exit.IsEnabled = false;
            button_login.Content = "Exiting...";
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_closeCompleted)
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }

        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            _closeCompleted = true;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowCollection collection = Application.Current.Windows;
            foreach (Window window in collection)
            {
                if (window.IsLoaded) window.Close();
            }
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Button_layoutProductsPurchases_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            Shared.layout = buttonName;
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageProductsWindow))
            {
                _manageProductsWindow = new ManageProductsWindow();
                if (_manageProductsWindow.IsEnabled) _manageProductsWindow.Show();
            }
            if(buttonName.Contains("Purchases") && !Application.Current.Windows.OfType<Window>().Contains(_managePurchasesWindow))
            {
                _managePurchasesWindow = new ManagePurchasesWindow();
                if (_managePurchasesWindow.IsEnabled) _managePurchasesWindow.Show();
            }
            else if (buttonName.Contains("Sales") && !Application.Current.Windows.OfType<Window>().Contains(_manageSalesWindow))
            {
                _manageSalesWindow = new ManageSalesWindow();
                if (_manageSalesWindow.IsEnabled) _manageSalesWindow.Show();
            }
        }

        private void Button_layoutPurchasesSales_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            Shared.layout = buttonName;
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(_managePurchasesWindow))
            {
                _managePurchasesWindow = new ManagePurchasesWindow();
                if (_managePurchasesWindow.IsEnabled) _managePurchasesWindow.Show();
            }
            if (!Application.Current.Windows.OfType<Window>().Contains(_manageSalesWindow))
            {
                _manageSalesWindow = new ManageSalesWindow();
                if (_manageSalesWindow.IsEnabled) _manageSalesWindow.Show();
            }
        }

        private void Button_Layouts_Click(object sender, RoutedEventArgs e)
        {
            if (gridLayout.Visibility == Visibility.Hidden)
            {
                gridLayout.Visibility = Visibility.Visible;
                button_Layouts.Content = "Hide layouts";
                imageBackground.Visibility = Visibility.Hidden;
            }
            else
            {
                gridLayout.Visibility = Visibility.Hidden;
                button_Layouts.Content = "Show layouts";
                imageBackground.Visibility = Visibility.Visible;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // create list of images for slideshow
            // http://csharphelper.com/blog/2020/04/use-code-to-make-a-slideshow-in-c-and-wpf/
            DirectoryInfo dir_info = new DirectoryInfo(Environment.CurrentDirectory + "\\Resources\\Images\\Slide");
            foreach (FileInfo file_info in dir_info.GetFiles())
            {
                if ((file_info.Extension.ToLower() == ".jpg") ||
                    (file_info.Extension.ToLower() == ".png"))
                {
                    _imagesList.Add(new BitmapImage(new Uri(file_info.FullName)));
                }
            }

            // Display the first image.
            imageBackground.Source = _imagesList[0];

            // Install a timer to show each image.
            pictureTimer.Interval = TimeSpan.FromSeconds(5);
            pictureTimer.Tick += Tick;
            pictureTimer.Start();
        }

        // Display the next image.
        private void Tick(object sender, System.EventArgs e)
        {
            imageNumber = (imageNumber + 1) % _imagesList.Count;
            ShowNextImage(imageBackground);
        }

        private void ShowNextImage(Image img)
        {
            const double transition_time = 0.9;
            Storyboard sb = new Storyboard();

            // ***************************
            // Animate Opacity 1.0 --> 0.0
            // ***************************
            DoubleAnimation fade_out = new DoubleAnimation(1.0, 0.0,
                TimeSpan.FromSeconds(transition_time));
            fade_out.BeginTime = TimeSpan.FromSeconds(0);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(fade_out, img);
            Storyboard.SetTargetProperty(fade_out,
                new PropertyPath(Image.OpacityProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(fade_out);


            // *********************************
            // Animate displaying the new image.
            // *********************************
            ObjectAnimationUsingKeyFrames new_image_animation =
                new ObjectAnimationUsingKeyFrames();
            // Start after the first animation has finisheed.
            new_image_animation.BeginTime = TimeSpan.FromSeconds(transition_time);

            // Add a key frame to the animation.
            // It should be at time 0 after the animation begins.
            DiscreteObjectKeyFrame new_image_frame =
                new DiscreteObjectKeyFrame(_imagesList[imageNumber], TimeSpan.Zero);
            new_image_animation.KeyFrames.Add(new_image_frame);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(new_image_animation, img);
            Storyboard.SetTargetProperty(new_image_animation,
                new PropertyPath(Image.SourceProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(new_image_animation);


            // ***************************
            // Animate Opacity 0.0 --> 1.0
            // ***************************
            // Start when the first animation ends.
            DoubleAnimation fade_in = new DoubleAnimation(0.0, 1.0,
                TimeSpan.FromSeconds(transition_time));
            fade_in.BeginTime = TimeSpan.FromSeconds(transition_time);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(fade_in, img);
            Storyboard.SetTargetProperty(fade_in,
                new PropertyPath(Image.OpacityProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(fade_in);

            // Start the storyboard on the img control.
            sb.Begin(img);
        }

        private void Button_OpenNotes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt|Rich text file (*.rtf) |*.rtf",
                DefaultExt = ".txt",
                Title = "Open notes file:"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                int filter = openFileDialog.FilterIndex;
                // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-save-load-and-print-richtextbox-content?view=netframeworkdesktop-4.8
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                FileStream fStream = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate);
                range.Load(fStream, (filter == 1 ? DataFormats.Text : DataFormats.Rtf));
                fStream.Close();
                /*
                StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                string fileContent = sr.ReadToEnd();

                */
            }
        }
        private void Button_SaveNotes_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt|Rich text file (*.rtf) |*.rtf",
                DefaultExt = "*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "Notes",
                Title = "Save notes as:"
            };
            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog

            if (result == true)
            {
                /*using ( FileStream myStream = new FileStream( myDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write ) ) {
                    TextRange myRange = new TextRange( rtbTraffic.Document.ContentStart, rtbTraffic.Document.ContentEnd );
                    myRange.Save( myStream, DataFormats.Rtf );
                    myStream.Close();
                }*/

                int filter = saveFileDialog.FilterIndex;
                richTextBox.SelectAll();
                richTextBox.Selection.Save(new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write), (filter == 1 ? DataFormats.Text : DataFormats.Rtf));
                button_SaveNotes.Content = "File saved";
                button_SaveNotes.IsEnabled = false;
                richTextBox.TextChanged += new TextChangedEventHandler(RichTextBox_TextChanged);
            }
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (button_SaveNotes.Content.ToString() == "File saved")
            {
                button_SaveNotes.Content = "Save notes";
                button_SaveNotes.IsEnabled = true;
                richTextBox.TextChanged -= new TextChangedEventHandler(RichTextBox_TextChanged);
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded)
            {
                // https://stackoverflow.com/questions/11874800/change-style-of-selected-text-in-richtextbox
                TextSelection ts = richTextBox.Selection;
                if(ts != null)
                ts.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush((Color)colorPickerFont.SelectedColor));
            }
        }
    }
}
