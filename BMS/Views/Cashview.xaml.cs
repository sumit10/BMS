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
using BMS.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for Cashview.xaml
    /// </summary>
    public partial class Cashview : UserControl
    {
        string type;
        List<ledgerbalance> ca,lc, ls, li, le;
        List<cash> cs;
        bool dr;
        int cid, lid,gid;
        autocomplete ac;
        string ctype;
        ObservableCollection<cash> patmentrecived, paymentgiven, income, expance;
       // BackgroundWorker worker;
        string date;
        public Cashview()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            //MessageBox.Show(application.l[0].Lname.ToString());
            this.ca = (from a in application.tb where a.Lgroup == 1 || a.Lgroup == 14 select a).ToList();
            this.lc = (from a in application.tb where a.Lgroup == 2 select a).ToList();
            this.ls = (from a in application.tb where a.Lgroup == 3 select a).ToList();
            this.li = (from a in application.tb where a.Lgroup == 15 select a).ToList();
            this.le = (from a in application.tb where a.Lgroup == 12 select a).ToList();
            ac = new autocomplete(txtac, lstname, lc, cashtrans);
            txtac.KeyUp += new KeyEventHandler(ac.tkuplid);
            lstname.MouseUp += new MouseButtonEventHandler(ac.lmd);
            lstname.KeyDown += new KeyEventHandler(ac.lkd);
            txtac.LostFocus += new RoutedEventHandler(ac.tlf);
            ac.nameseleceted += new EventHandler(acnameselected);
            autocomplete act = new autocomplete(txtacshac, lstcashac, ca, cashtrans);
            txtacshac.KeyUp += new KeyEventHandler(act.tkuplid);
            lstcashac.MouseUp += new MouseButtonEventHandler(act.lmd);
            lstcashac.KeyDown += new KeyEventHandler(act.lkd);
            txtacshac.LostFocus += new RoutedEventHandler(act.tlf);
             act.nameseleceted += new EventHandler(cashacselected);
             //date = dpcradate.Text.ToString();
             //worker.RunWorkerAsync();
             dpcradate.Text = DateTime.Now.Date.ToString();
             dpcashfdate.Text = DateTime.Now.Date.ToString();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            cs = new cashmodel().getcashdata(date);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            patmentrecived = new ObservableCollection<cash>((from a in cs where a.crgid == 2 select a).ToList());
            paymentgiven = new ObservableCollection<cash>((from a in cs where a.drgid == 3 select a).ToList());
            income = new ObservableCollection<cash>((from a in cs where a.crgid == 15 select a).ToList());
            expance = new ObservableCollection<cash>((from a in cs where a.drgid == 12 select a).ToList());
            lstpaymentresived.ItemsSource = patmentrecived;
            lstpaymentgiven.ItemsSource = paymentgiven;
            lstincome.ItemsSource = income;
            lstexpanse.ItemsSource = expance;
            loading.Visibility = Visibility.Hidden;
        }

        void cashacselected(object sender, EventArgs e)
        {
            txtac.Focus();
            ledgerbalance l = (ledgerbalance)lstcashac.Items.GetItemAt(0);
            cid = l.Lid;
            gid = l.Lgroup;
        }

        void acnameselected(object sender, EventArgs e)
        {
            txtamt.Focus();
            ledgerbalance l = (ledgerbalance)lstname.Items.GetItemAt(0);
            lblbal.Content = l.Balance;
            lid = l.Lid;
        }
        private void btncacreate_Click(object sender, RoutedEventArgs e)
        {
            if (type == "null")
            { 
                if (cbcactype.SelectedItem == null)
                {
                    return;
                } 
                type = cbcactype.Text;
            }
            if (txtcacname.Text.Trim() != "" && txtcacob.Text.Trim() != ""  )
            {
                cashmodel c =  new cashmodel();
                if (c.createaccount(txtcacname.Text,double.Parse(txtcacob.Text),type,dpcradate.ToString()))
                {
                    CrCashAc.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                Show("Please Enter Correct Data",2);
            }
        }

        private void btn_creatcash_Click(object sender, RoutedEventArgs e)
        {
            type = "null";
            lbl_atype.Visibility = Visibility.Visible;
            cbcactype.Visibility = Visibility.Visible;
            CrCashAc.Visibility = Visibility.Visible;
        }

        private void createexpance_Click(object sender, RoutedEventArgs e)
        {
            type = "expance";
            lbl_atype.Visibility = Visibility.Hidden;
            cbcactype.Visibility = Visibility.Hidden;
            CrCashAc.Visibility = Visibility.Visible;
        }

        private void createincomeac_Click(object sender, RoutedEventArgs e)
        {

            type = "income";
            lbl_atype.Visibility = Visibility.Hidden;
            cbcactype.Visibility = Visibility.Hidden;
            CrCashAc.Visibility = Visibility.Visible;
        }

        private void btnpayrec_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["revised"]).Begin();
            ctype = "crecive";
            dr = true;
            lblname1.Content = "To A\\c.";
            lblname2.Content = "From A\\c.";
            
            cashtrans.Visibility = Visibility.Visible;
            ac.setlist(lc);
            txtacshac.Focus();
            clrct();
        }

        private void btnpaygiv_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["given"]).Begin();
            ctype = "cgiven";
            dr = false;
            lblname1.Content = "From A\\c.";
            lblname2.Content ="To A\\c.";
            cashtrans.Visibility = Visibility.Visible;
            ac.setlist(ls);
            txtacshac.Focus();
            clrct();
        }

        private void btnexpance_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["expense"]).Begin();
            ctype = "expance";
            dr = false;
            lblname1.Content = "From A\\c.";
            lblname2.Content = "To A\\c.";
            cashtrans.Visibility = Visibility.Visible;
            ac.setlist(le);
            txtacshac.Focus();
            clrct();
        }

        private void btnincome_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["income"]).Begin();
            ctype = "income";
            dr = true;
            lblname1.Content = "To A\\c.";
            lblname2.Content = "From A\\c.";
            cashtrans.Visibility = Visibility.Visible;
            ac.setlist(li);
            txtacshac.Focus();
            clrct();
        }

        private void ctclose_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["close"]).Begin();
            cashtrans.Visibility = Visibility.Hidden;
        }

        private void cacclose_Click(object sender, RoutedEventArgs e)
        {
            CrCashAc.Visibility = Visibility.Hidden;
        }

        private void addct_Click(object sender, RoutedEventArgs e)
        {
          //  MessageBox.Show(lid + " "+cid);
            if (txtamt.Text != "" && txtacshac.Text != "" && txtac.Text != "")
	        { 
                cashmodel ct = new cashmodel();
		          if (ct.incertcash(cid,lid,Double.Parse(txtamt.Text),txtnaration.Text,dr,dpcashfdate.ToString()))
                  { 
                     Show("Succesfull",1);
                    cashtrans.Visibility = Visibility.Hidden;
                      cash c;
                    if (dr)
                    {
                       c = new cash()
                        {
                            tid = ct.tid,  drname = txtacshac.Text,  drgid = cid,  crname = txtac.Text,
                            crid = lid,
                            amt = Double.Parse(txtamt.Text),
                            naration = txtnaration.Text,
                        };
                       int i = application.tb.Where(x => x.Lid == cid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                       application.tb[i].Balance = application.tb[i].Balance + Double.Parse(txtamt.Text);
                       application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                       //application.tb[i].BalanceType = "dr";
                       int j = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                       application.tb[j].Balance = application.tb[j].Balance - Double.Parse(txtamt.Text);
                       application.tb[j].CurrBalance = application.tb[j].Balance.ToString("C2");
                      /// application.tb[i].BalanceType = "dr";
                    }
                    else
                    {
                       c = new cash()
                        {
                            tid = ct.tid,
                            drname = txtac.Text,
                            drgid = lid,
                            crname = txtacshac.Text ,
                            crid = cid,
                            amt = Double.Parse(txtamt.Text),
                            naration = txtnaration.Text,
                        };
                       int i = application.tb.Where(x => x.Lid == cid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                       application.tb[i].Balance = application.tb[i].Balance -Double.Parse(txtamt.Text);
                       application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                       //application.tb[i].BalanceType = "dr";
                       int j = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                       application.tb[j].Balance = application.tb[j].Balance + Double.Parse(txtamt.Text);
                       application.tb[j].CurrBalance = application.tb[j].Balance.ToString("C2");
                    }
                    if (ctype == "crecive")
                    {
                        patmentrecived.Add(c);
                    }
                    else if (ctype == "cgiven")
                    {
                        paymentgiven.Add(c);
                    }
                    else if (ctype == "income")
                    {
                        income.Add(c);
                    }
                    else if (ctype == "expance")
                    {
                        expance.Add(c);
                    }
                      
                  }
	        }
            else  {
                Show("Fields Cannot be Empty",2);
        	}
        }

        void clrct()
        {
            txtac.Text = "";
            txtacshac.Text = "";
            txtamt.Text = "";
            txtnaration.Text = "";
            lblbal.Content = "";
        }

        private void dpcashfdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            date = dpcashfdate.ToString();
            worker.RunWorkerAsync();
        }

        private void cash_slide(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["cash_slide"]).Begin();
        }

        private void slide_out(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["slide_out"]).Begin();
        }

        private void txtnaration_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtamt.Text != "" && txtacshac.Text != "" && txtac.Text != "")
            {
                String n = "Being ";
                if (ctype == "crecive")
                {
                    n += txtamt.Text + " amount recived from " + txtac.Text + " (Customer)";
                }
                else if (ctype == "cgiven")
                {
                    n += txtamt.Text + " amount given to " + txtac.Text + " (Supplier)";
                }
                else if (ctype == "income")
                {
                    n += txtamt.Text + " amount recived from " + txtac.Text + " (Income)";
                }
                else if (ctype == "expance")
                {
                    n += txtamt.Text + " amount given for " + txtac.Text + " (Expence)";
                }
                if (gid == 1)
                {
                    n += " by " + txtacshac.Text ;
                }
                else
                {
                    n += " by " + txtacshac.Text + " A/c. where chequeno. = ";
                }
                txtnaration.Text = n;
                txtnaration.SelectionStart = txtnaration.Text.Length;
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
