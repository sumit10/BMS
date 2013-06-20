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
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for supplierview.xaml
    /// </summary>
    public partial class supplierview : UserControl
    {
        ObservableCollection<ledger> lc = new ObservableCollection<ledger>();
        ledger l = new ledger();
        List<ladgerreport> lr;
        string url;
        public supplierview()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ledgermodel l = new ledgermodel();
            url = null;
            OleDbDataReader dr = l.getgroup(3);
            while (dr.Read())
            {
                string img = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + dr.GetValue(1).ToString() + ".jpg";
                if (!File.Exists(img))
                {
                    img = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\default.jpg";
                }
                ledgerbalance x = (from a in application.tb where a.Lid == int.Parse(dr.GetValue(0).ToString()) select a).First<ledgerbalance>();
                String y = x.Balance.ToString();
                if (y == "0")
                {
                    y = "Nil";
                }

                ledger lb = new ledger()
                {
                    Lid = int.Parse(dr.GetValue(0).ToString()),
                    Lname = dr.GetValue(1).ToString(),
                    Laddress = dr.GetValue(2).ToString(),
                    Lphoneno = dr.GetValue(3).ToString(),
                    Lemail = dr.GetValue(4).ToString(),
                    Limage = img,
                    Balance = y,
                    BalanceType = x.BalanceType
                };
                lc.Add(lb);
               
            }
            listBox1.ItemsSource = lc;
        }

         private void button1_Click(object sender, RoutedEventArgs e)
          {
              ledgermodel l = new ledgermodel(txtlname.Text, 3, txtladd.Text, txtlno.Text, txtlmail.Text);
            //Hashtable error = l.checkvalid();
            //int a = l.
              if (txtladd.Text.Trim().ToString() == "" || txtlmail.Text.Trim().ToString() == "" || txtlname.Text.Trim().ToString() == "" || txtlno.Text.Trim().ToString()=="")
              {
                  Show("Please Enter all Data of Supplier",2);
                  return;
              }
              if (!(Regex.IsMatch(txtlno.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")) || txtlno.Text.Length < 6 || txtlno.Text.Length > 10)
              {
                  Show("Please Enter Valid Phone No.",2);
                  return;
              }
              if (!Regex.IsMatch(txtlmail.Text, @"^\S+@\S+$"))
              {
                  Show("Please Enter Valid Email id No.",2);
                  return;
              }

            if (l.insert(this))
            {
                Show("Supplier Saved",1);
                txtlname.ClearValue(TextBox.BorderBrushProperty);
                int sid = l.lastid();
                MessageBox.Show(sid.ToString());
                ledger nlb = new ledger()
                {
                    Lid = sid,
                    Lname= txtlname.Text,
                    Lgroup = 3,
                    Laddress = txtladd.Text,
                    Lphoneno = txtlno.Text,
                    Lemail = txtlmail.Text
                };
                ledgerbalance lb = new ledgerbalance() { Lid = sid, Lname = txtlname.Text, Balance = 0 };
                application.tb.Add(lb);
                if (url != null)
                {
                    string targetPath = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + txtlname.Text + ".jpg";
                    System.IO.File.Copy(url, targetPath, true);
                    nlb.Limage = targetPath;
                }
                else
                {
                    nlb.Limage = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\default.jpg";
                }
                lc.Add(nlb);
                clr();
            }
            else
            {
                Show("Supplier Not Saved",2);
            }
            //   l.showerror(error,this);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            textBox1.ClearValue(TextBox.BorderBrushProperty);
            l = (ledger)listBox1.SelectedValue;
            txtlname.Text = l.Lname;
            txtladd.Text = l.Laddress;
            txtlno.Text = l.Lphoneno;
            txtlmail.Text = l.Lemail;
            edittoggle(false);
            save.Visibility = Visibility.Hidden;
            can_edit.Visibility = Visibility.Visible;
            image1.Source = new BitmapImage(new Uri(l.Limage, UriKind.Absolute)); 
        }

        private void textBox1_Keyup(object sender, KeyEventArgs e)
        {
            listBox1.ItemsSource = null;
            listBox1.Items.Clear();
            TextBox t = (TextBox)sender;
            String str = t.Text;
            if (str == " ")
            {
                listBox1.ItemsSource = lc;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < lc.Count; i++)
                {
                    string a = lc[i].Lname;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        listBox1.Items.Add(lc[i]);
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
        public void clr()
        {
            txtlname.Text = " ";
            txtladd.Text = " ";
            txtlno.Text = " ";
            txtlmail.Text = " ";
            image1.Source = null;
            url = null;
        }
        public void edittoggle(bool a)
        {
            txtlname.IsEnabled = a;
            txtladd.IsEnabled = a;
            txtlno.IsEnabled = a;
            txtlmail.IsEnabled = a;
            txtlmail.IsEnabled = a;
            btnimage.IsEnabled = a;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            edittoggle(true);
            btn_update.Visibility = Visibility.Visible;
            can_edit.Visibility = Visibility.Hidden;
        }

        private void btn_newuser_Click(object sender, RoutedEventArgs e)
        {
            can_edit.Visibility = Visibility.Hidden;
            clr();
            btn_update.Visibility = Visibility.Hidden;
            save.Visibility = Visibility.Visible;
            btn_edit.IsEnabled = true;
            edittoggle(true);
        }
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            if (txtladd.Text.Trim().ToString() == "" || txtlmail.Text.Trim().ToString() == "" || txtlname.Text.Trim().ToString() == "" || txtlno.Text.Trim().ToString() == "")
            {
                Show("Please Enter all Data of Supplier", 2);
                return;
            }
            if (!(Regex.IsMatch(txtlno.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")) || txtlno.Text.Length < 6 || txtlno.Text.Length > 10)
            {
                Show("Please Enter Valid Phone No.", 2);
                return;
            }
            if (!Regex.IsMatch(txtlmail.Text, @"^\S+@\S+$"))
            {
                Show("Please Enter Valid Email id ", 2);
                return;
            }
            ledgermodel lm = new ledgermodel(txtlname.Text, 3, txtladd.Text, txtlno.Text, txtlmail.Text, l.Lid);
            if (lm.update(this))
            {
               Show("Supplier Updated",1);
                if (url != null)
                {
                    string targetPath = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + txtlname.Text + ".jpg";
                    System.IO.File.Copy(url, targetPath, true);
                }
                btn_edit.IsEnabled = true;
                can_edit.Visibility = Visibility.Visible;
                btn_update.Visibility = Visibility.Hidden;
                edittoggle(false);
                var x = lc.FirstOrDefault(i => i.Lid == l.Lid);
                x.Lname = txtlname.Text;

            }
        }

        private void btnimage_Click(object sender, RoutedEventArgs e)
        {
            Stream checkStream = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Images |*.jpg;*.png;*.gif";
             try
                {
                    if ((bool)openFileDialog.ShowDialog())
                    {

                        if ((checkStream = openFileDialog.OpenFile()) != null)
                        {
                            image1.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                            url = openFileDialog.FileName;
                        }
                    }
                    else
                    {
                        Show("Problem occured, try again later", 2);
                    }
                }
                catch (Exception ex)
                {
                    Show("Error: Could not read file from disk. Original error: " + ex.Message,2);
                }
          
        }
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            lblname.Content = "";
            //lst_cr.Items.Clear();
            //lst_dr.Items.Clear();
            suplier.Visibility = Visibility.Visible;
            Report.Visibility = Visibility.Hidden;
        }

        private void btnacreport_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker w = new BackgroundWorker();
            w.DoWork += new DoWorkEventHandler(w_DoWork);
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
            w.RunWorkerAsync();
            this.Cursor = Cursors.Wait;
            Report.Visibility = Visibility.Visible;
            suplier.Visibility = Visibility.Hidden;
            lblname.Content = txtlname.Text;
        }

        void w_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            lst_dr.ItemsSource = (from a in lr where a.crlid == l.Lid select a).ToList();
            lst_cr.ItemsSource = (from a in lr where a.drlid == l.Lid select a).ToList();
            this.Cursor = Cursors.Arrow;
        }

        void w_DoWork(object sender, DoWorkEventArgs e)
        {
            lr = new ledgermodel().getledgerreport(l.Lid);
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
