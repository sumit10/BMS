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
using System.Collections;
using BMS.Model;
using System.Collections.ObjectModel;
using Microsoft.Windows.Controls;
namespace BMS
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
    public partial class stockview : UserControl
    {
        ObservableCollection<stockdata> sc = new ObservableCollection<stockdata>();
        stockdata s = new stockdata();
        public stockview()
        {
            commoditymodel cm = new commoditymodel();
            InitializeComponent();
            autocomplete.fillcombo(cmbuom, "u_sym", "uom");
            OleDbDataReader dr = cm.getcom();
            while (dr.Read())
            {
                stockdata sd = new stockdata()
                {
                    Cid = int.Parse(dr.GetValue(0).ToString()),
                    Cname = dr.GetValue(1).ToString(),
                    Cuom = dr.GetValue(2).ToString(),
                    Cpp = dr.GetValue(3).ToString(),
                    Csp = dr.GetValue(4).ToString(),
                    Copstock = dr.GetValue(5).ToString(),
                    Cbalstock = dr.GetValue(6).ToString()
                };
                sc.Add(sd);

            }
            listBox1.ItemsSource = sc;
        }
        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            textBox1.ClearValue(TextBox.BorderBrushProperty);
            s = (stockdata)listBox1.SelectedValue;
            txtconame.Text = s.Cname;
            txtcmb.Text = s.Cuom;
            txtcopp.Text = s.Cpp;
            txtcosp.Text = s.Csp;
            txtcoops.Text = s.Copstock;
            txtbalstock.Text = s.Cbalstock;
            edittoggle(false);
            save.Visibility = Visibility.Hidden;
            can_edit.Visibility = Visibility.Visible;
            txtcoops.IsEnabled = false;
            dpdate.IsEnabled = false;
            btn_update.Visibility = Visibility.Hidden;
        }

        private void textBox1_Keyup(object sender, KeyEventArgs e)
        {
            listBox1.ItemsSource = null;
            listBox1.Items.Clear();
            TextBox t = (TextBox)sender;
            String str = t.Text;
            if (str == " ")
            {
                listBox1.ItemsSource = sc;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < sc.Count; i++)
                {
                    string a = sc[i].Cname;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        listBox1.Items.Add(sc[i]);
                        j++;
                    }
                }
                if (j == 0)
                {
                    label1.Visibility = Visibility.Visible;
                }
                if (j > 0)
                {
                    label1.Visibility = Visibility.Hidden;
                }
            }
        }
        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            if (cmbuom.SelectedItem != null && dpdate.SelectedDate != null)
            {
                cmbuom.ClearValue(ComboBox.BorderBrushProperty);
                dpdate.ClearValue(DatePicker.BorderBrushProperty);
                commoditymodel cm = new commoditymodel(txtconame.Text, cmbuom.SelectedValue.ToString(), txtcopp.Text, txtcosp.Text, txtcoops.Text, dpdate.Text);
                if (cm.insert(this))
                {
                    Show("Saved Succesfully",1);
                }
                else
                {
                    //dashboard d = new dashboard();
                    ////////////////////cm.showerror(error, this);
                }
            }
            else
            {
                cmbuom.BorderBrush = Brushes.Red;
                dpdate.BorderBrush = Brushes.Red;
            }
        }
        public void clr()
        {
            txtconame.Text = " ";
            cmbuom.SelectedIndex = 0;
            txtcoops.Text = " ";
            txtcosp.Text = " ";
            txtcopp.Text = " ";
        }
        public void edittoggle(bool a)
        {
            txtconame.IsEnabled = a;
            txtcmb.IsEnabled = a;
            txtcopp.IsEnabled = a;
            txtcosp.IsEnabled = a;
           // txtcoops.IsEnabled = a;
            if (a)
            {
                cmbuom.Visibility = Visibility.Visible;
                txtcmb.Visibility = Visibility.Collapsed;
            }
            else
            {
                cmbuom.Visibility = Visibility.Collapsed;
                txtcmb.Visibility = Visibility.Visible;

            }
        }
        private void edit_Click(object sender, RoutedEventArgs e)
        {
            edittoggle(true);
            btn_update.Visibility = Visibility.Visible;
            can_edit.Visibility = Visibility.Hidden;
            btn_edit.IsEnabled = false;
        }

        private void btn_newuser_Click(object sender, RoutedEventArgs e)
        {
            can_edit.Visibility = Visibility.Hidden;
            clr();
            save.Visibility = Visibility.Visible;
            btn_edit.IsEnabled = true;
            edittoggle(true);
            txtcoops.IsEnabled = true;
            dpdate.IsEnabled = true;
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            if (txtconame.Text == "" || cmbuom.SelectedValue == null || txtcopp.Text == "" || txtcosp.Text == "")
            {
                Show("Please fill all Field",2);
                return;
            }
            commoditymodel cm = new commoditymodel(txtconame.Text,cmbuom.SelectedValue.ToString(),txtcopp.Text,txtcosp.Text,"null","null",s.Cid);
            if (cm.update(this))
            {
               Show("Successfully Updated",1);
                btn_edit.IsEnabled = true;
                btn_update.Visibility = Visibility.Hidden;
                edittoggle(false);
                var x = sc.FirstOrDefault(i => i.Cid == s.Cid);
                x.Cname = txtconame.Text;

            }
        }
        private void msgclick(object sender, RoutedEventArgs e)
        {
            message_box.Visibility = Visibility.Hidden;
        }

        void Show(string msg, int a)
        {
            lbl_message.Content = msg;
            if (a == 1)
            {
                btn_ok.Visibility = Visibility.Visible;
                btn_error.Visibility = Visibility.Hidden;
            }
            else
            {
                btn_error.Visibility = Visibility.Visible;
                btn_ok.Visibility = Visibility.Hidden;
            }
            message_box.Visibility = Visibility.Visible;
        }
       
    }
}