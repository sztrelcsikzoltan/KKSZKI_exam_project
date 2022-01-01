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

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class LoginWindow : Window
    {

        // private RegisterWindow registerWindow;
        
            public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        
        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {


            // login check
            if (Textbox_UserName.Text == "admin" && PasswordBox_Password.Password == "admin")
            {
                Image_Login.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                TextBlock_Login.Text = $"Hello {Textbox_UserName.Text}, you are logged in.";
                TextBlock_Login.Foreground = Brushes.LightGreen;
                Button_Login.Foreground = Brushes.Green;
                Button_Login.Content = "OK";
                //MessageBox.Show("OK, you are logged in.");
                Close();
            }
            else
            {
                TextBlock_Login.Text = "Wrong login data, please retry!";
                TextBlock_Login.Foreground = Brushes.LightSalmon;
            }
            


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
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
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

