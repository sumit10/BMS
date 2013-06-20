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
using System.ComponentModel;
using System.IO;
using System.Windows.Markup;
using System.Text.RegularExpressions;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for customerview.xaml
    /// </summary>
    public partial class customerview : UserControl
    {
        ObservableCollection<ledger> lc = new ObservableCollection<ledger>();
        ledger l = new ledger();
        List<ladgerreport> lr;
        string url;
        public customerview()
        {
            InitializeComponent();
            
           
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
            url = null;
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            ledgermodel l = new ledgermodel();
            OleDbDataReader dr = l.getgroup(2);
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
		               y="Nil";
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
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.ItemsSource = lc;
            label2.Visibility = Visibility.Collapsed;
        }

      

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ledgermodel l = new ledgermodel(txtlname.Text,2,txtladd.Text,txtlno.Text,txtlmail.Text);
            //Hashtable error = l.checkvalid();
            //int a = l.
            if (txtlmail.Text.Trim().ToString() == "" || txtlname.Text.Trim().ToString() == "" || txtladd.Text.Trim().ToString() == "" || txtlno.Text.Trim().ToString() == "")
            {
                Show("Please enter in all field",2);
                return;
            }
            if (!(Regex.IsMatch(txtlno.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$")) || txtlno.Text.Length < 6 || txtlno.Text.Length > 10)
            {
                Show("Please Enter Valid Phone No.", 2);
                return;
            }
            if (!Regex.IsMatch(txtlmail.Text, @"^\S+@\S+$"))
            {
                Show("Please Enter Valid Email id No.", 2);
                return;
            }
            if (l.insert(this))
            {
                Show("Data Saved",1);
                txtlname.ClearValue(TextBox.BorderBrushProperty);
                int lid = l.lastid();
              //  MessageBox.Show(lid.ToString());
                
                ledger nlb = new ledger() {
                   Lid = lid, Lname = txtlname.Text , Lgroup = 2 , Laddress = txtladd.Text,Lphoneno = txtlno.Text,Lemail=txtlmail.Text ,Balance="Nil"
                };
                ledgerbalance lb = new ledgerbalance() { Lid = lid, Lname = txtlname.Text , Balance = 0  };
                application.tb.Add(lb); 
                if (url != null)
               {
                   string targetPath = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + txtlname.Text+".jpg";
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
            btn_edit.IsEnabled = true;
            btn_update.Visibility = Visibility.Hidden;
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
                int j=0;
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
            btnimage.IsEnabled = a;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            edittoggle(true);
            can_edit.Visibility = Visibility.Hidden;
            btn_update.Visibility = Visibility.Visible;
            btn_edit.IsEnabled = false;
        }

        private void btn_newuser_Click(object sender, RoutedEventArgs e)
        {
            can_edit.Visibility = Visibility.Hidden;
            clr();
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
                Show("Please Enter Valid Email id No.", 2);
                return;
            }
            ledgermodel lm = new ledgermodel(txtlname.Text,2,txtladd.Text,txtlno.Text,txtlmail.Text,l.Lid);
            if (lm.update(this))
            {
                Show("Successfully Updated",1);
                if (url != null)
                {
                    string targetPath = @System.AppDomain.CurrentDomain.BaseDirectory + "\\images\\" + txtlname.Text + ".jpg";
                    System.IO.File.Copy(url, targetPath, true);
                }
                btn_edit.IsEnabled = true;
                btn_update.Visibility = Visibility.Hidden;
                edittoggle(false);
                can_edit.Visibility = Visibility.Visible;
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
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((checkStream = openFileDialog.OpenFile()) != null)
                    {
                        image1.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                        url = openFileDialog.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Problem occured, try again later");
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            lblname.Content = "";
            //lst_cr.Items.Clear();
            //lst_dr.Items.Clear();
            customer.Visibility = Visibility.Visible;
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
            customer.Visibility = Visibility.Hidden;
            lblname.Content = txtlname.Text;
            ledgerbalance b = (from a in application.tb where a.Lid == l.Lid select a).First<ledgerbalance>();
           // ledgerbalance ll = (ledgerbalance)b;
            lblbal.Content = b.Balance.ToString() + " " +b.BalanceType;
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

        private void print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                report.reportledger p = new BMS.report.reportledger();
                List<ladgerreport> dr = (from a in lr where a.crlid == l.Lid select a).ToList();
                List<ladgerreport> cr = (from a in lr where a.drlid == l.Lid select a).ToList();
                ledgerbalance b = (from a in application.tb where a.Lid == l.Lid select a).First<ledgerbalance>();
                if (b.BalanceType == "dr")
                {
                    dr.Add(new ladgerreport()
                    {
                        amt = b.Balance,
                        tdate = DateTime.Now.ToString(),
                        drlname = "Closing Balance",
                    });
                }
                else
                {
                    cr.Add(new ladgerreport()
                    {
                        amt = b.Balance,
                        tdate = DateTime.Now.Date.ToString(),
                        crlname = "Closing Balance",
                    });
                }
                p.lst_dr.ItemsSource = dr;
                p.lst_cr.ItemsSource = cr;
                p.r_acame.Content = lblname.Content;
                p.r_date.Content = DateTime.Now.Date.ToString(); ;
                PrintDialog pd = new PrintDialog();
                FixedDocument document = new FixedDocument();
                document.DocumentPaginator.PageSize = new Size(96 * 8.5, 96 * 11);
                FixedPage page1 = new FixedPage();
                page1.Width = document.DocumentPaginator.PageSize.Width;
                page1.Height = document.DocumentPaginator.PageSize.Height;
                Canvas can = p.layout;
                page1.Children.Add(can);
                PageContent page1Content = new PageContent();
                ((IAddChild)page1Content).AddChild(page1);
                document.Pages.Add(page1Content);
                pd.PrintDocument(document.DocumentPaginator, "My first document");
            }
            catch (Exception)
            {
                
                
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
