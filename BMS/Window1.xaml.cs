 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Threading;

namespace BMS
{
    public partial class Window1 : Window
    {
        OleDbConnection con;
        public Window1()
        {

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            SplashScreen splash = new SplashScreen("splash.png");
            splash.Show(true);
            Thread.Sleep(2000);
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          //  comreg cform = new comreg();
           // cform.Show();
          //  this.Hide() ;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Model.loginmodel login = new Model.loginmodel();
            if (login.login(txt_cname.Text, password.Password))
            {
                dashboard NewWindow = new dashboard();
                NewWindow.Show();
                this.Close();
            }
            else
            {
                password.Password = "";
                System.Windows.MessageBox.Show("Invalid user name or password");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to close this window?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
          this.DragMove();
        }     
    }
}
