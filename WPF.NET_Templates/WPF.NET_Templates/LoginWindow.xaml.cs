using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.NET_Templates.Classes;
// using WPF.NET_Templates.Components;
// using System.ServiceModel;
// using System.ServiceModel.Description;
// using WPF.NET_Templates.ServiceReference3;



namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class LoginWindow : Window
    {

        public static ServiceReference3.UserManagementClient client;
        public LoginWindow()
        {
            InitializeComponent();
            gifImage.StartAnimation();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private async void Button_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            client = new ServiceReference3.UserManagementClient();
            // string uid = null;
            string userName = Textbox_UserName.Text;
            string password = PasswordBox_Password.Password;

            string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";

            ServiceReference3.User[] user = { };
            try
            {
                user = client.UserList(query);
                
                if (user == null)
                {
                MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                return;
                }
            }
                catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return;
                }
                else
                {
                MessageBox.Show("Hiba történt, amelynek a részletei a következők:\n" + ex.ToString(), caption: "Error message");
                    return;
                }
            }
             

            // login check
            //if (Textbox_UserName.Text == "admin" && PasswordBox_Password.Password == "admin")
            if (user.Length == 1)
            {
                Button_Register.Click -= Button_Register_Click; // remove event handlers
                Button_Login.Click -= Button_Login_ClickAsync;

                // Image_Login.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                //this.WebBrowser.Source = new Uri(String.Format("file:///
                gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
                gifImage.Width = 65;
                gifImage.StopAnimation();
                gifImage.StartAnimation();

                TextBlock_Login.Text = $"Hello {Textbox_UserName.Text}, you are logged in.";
                TextBlock_Login.Foreground = Brushes.LightGreen;
                Button_Login.Foreground = Brushes.Green;
                Button_Login.Content = "OK";
                Shared.startWindow_With_PinPanels.button_login.Foreground = Brushes.Green;
                Shared.startWindow_With_PinPanels.button_login.Content = "Log out";

                // how to run a method as an Action: https://stackoverflow.com/questions/13260322/how-to-use-net-action-to-execute-a-method-with-unknown-number-of-parameters
                await Shared.Delay(() => CloseWindow(), 2500);

            }
            else
            {
                TextBlock_Login.Text = "Wrong login data, please retry!";
                TextBlock_Login.Foreground = Brushes.LightSalmon;
            }

        }


        private void CloseWindow()
        {
            Close();
            Shared.logRegWindowsClosed = true;
        }

        // Fade-in-out animation

        private bool closeCompleted = false;


        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            closeCompleted = true;
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

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            Button_Register.Click -= Button_Register_Click; // remove event handler
            Button_Login.Click -= Button_Login_ClickAsync;

            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Shared.logRegWindowsClosed = false;
            Close();
        }

        private void Textbox_UserName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Textbox_UserName.Text == "Username")
            {
                Textbox_UserName.Text = "";
            }
        }

        private void PasswordBox_Password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordBox_Password.Password == "Password")
            {
                PasswordBox_Password.Password = "";
            }
        }



        /*
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AlreadyFaded = false;
        }

        private bool AlreadyFaded;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!AlreadyFaded)
            {
                 AlreadyFaded = true;
                 e.Cancel = true;
                 DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(4));
                 WindowFadeOut.Completed += new EventHandler(WindowFadeOut_Completed);
                 BeginAnimation(OpacityProperty, anim);
            }
        }

        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            Close();
        }
        */

    }
}

