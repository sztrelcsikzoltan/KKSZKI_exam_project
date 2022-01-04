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

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class RegisterWindow : Window
    {
        public static ServiceReference3.UserManagementClient client;
        public RegisterWindow()
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

        private async void Button_Register_ClickAsync(object sender, RoutedEventArgs e)
        {
            client = new ServiceReference3.UserManagementClient();
            // string uid = null;
            string userName = Textbox_UserName.Text;
            string password = PasswordBox_Password.Password;


            // Register check
            string registerMessage = "";
            if (Textbox_UserName.Text.Length >= 4 && PasswordBox_Password.Password.Length >= 4)
            {
                // first check whether the user already exists, this is ALMOST the same as the Login code
                string query = $"WHERE Username='{userName}'";

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
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt, amelynek a részletei a következők:\n" + ex.ToString(), caption: "Error message");
                    }
                }
                if (user.Length == 1)
                {
                    TextBlock_Register.Text = "User name is already taken, please choose another name!";
                    TextBlock_Register.Foreground = Brushes.LightSalmon;
                    return;
                }

                try
                {
                    // registration into database
                    registerMessage = client.Register(userName, CreateMD5(password));
                    if (registerMessage == "There was a problem with your registration!")
                    {
                        MessageBox.Show(registerMessage, caption: "Error message");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("XXXXX"))
                    {
                        MessageBox.Show("This will be a specific error.", caption: "Error message");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt, amelynek a részletei a következők:\n" + ex.ToString(), caption: "Error message");
                        return;
                    }
                }

                Button_Register.Click -= Button_Register_ClickAsync; // remove event handlers
                Button_Login.Click -= Button_Login_Click;

                // Image_Register.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
                gifImage.Width = 65;
                gifImage.StopAnimation();
                gifImage.StartAnimation();

                TextBlock_Register.Text = $"Hello {Textbox_UserName.Text}, you are registered. Now you can log in.";
                TextBlock_Register.Foreground = Brushes.LightGreen;
                Button_Register.Foreground = Brushes.Green;
                Button_Register.Content = "OK";

                // how to run a method as an Action: https://stackoverflow.com/questions/13260322/how-to-use-net-action-to-execute-a-method-with-unknown-number-of-parameters
                await Shared.Delay(() => StopAnimation(), 3000);
                await Shared.Delay(() => CloseWindow(), 4500);
            }
            else
            {
                TextBlock_Register.Text = "User name and password must be at least 4 characters!";
                TextBlock_Register.Foreground = Brushes.LightSalmon;
            }


        }

        private void CloseWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
            Shared.logRegWindowsClosed = true;
        }
        private void StopAnimation()
        {
            gifImage.StopAnimation();
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

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            Button_Login.Click -= Button_Login_Click; // remove event handlers
            Button_Register.Click -= Button_Register_ClickAsync;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
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

