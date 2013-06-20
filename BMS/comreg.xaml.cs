using System;
using System.Collections;
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
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using BMS.Model;
namespace BMS
{
    /// <summary>
    /// Interaction logic for comreg.xaml
    /// </summary>
    public partial class comreg : Window
    {
        OleDbConnection con;
        Hashtable error;
        public comreg()
        {
            InitializeComponent();
            con = new OleDbConnection();
            con.ConnectionString = Model.basemodel.Constring;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         //   remove_error();
                loginmodel lm = new loginmodel(txtname.Text, pbpassword.Password, txtaddress.Text, txtphone.Text);
                error = lm.checkvalid();
                if (lm.insert())
                {
                    Window1 NewWindow = new Window1();
                    NewWindow.Show();
                    this.Hide();
                    System.Windows.MessageBox.Show("Account created");
                }
                else
                {
                  //  lm.showerror(error,t);
                }
    
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 a = new Window1();
            a.Show();
            
            this.Hide();
        }
    }
}
