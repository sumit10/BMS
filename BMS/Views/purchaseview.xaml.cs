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
using System.Data;
using System.ComponentModel;
using System.Configuration;
using WindowsInput;
using System.Windows.Media.Animation;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for purchaseview.xaml
    /// </summary>
    public partial class purchaseview : UserControl
    {
        int lid, cid, vid, orderid, comitoid;
        Double vat, addcharges, roundof, combal;
        Double total, t2;
        DataTable dr, dr1;
        bool order, invoicedetail;
        Fillcombo f;
        ObservableCollection<purchaseitem> pc = new ObservableCollection<purchaseitem>(), pce = new ObservableCollection<purchaseitem>();
        ObservableCollection<purchasereport> pr,por;
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public purchaseview()
        {
            InitializeComponent();
            vat = 0; addcharges = 0; roundof = 0;
            total = 0;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            autocomplete a = new autocomplete(txtcustomer, list_customer, dr, "l_name", "l_id", layout);
            txtcustomer.KeyUp += new KeyEventHandler(a.tkupid);
            list_customer.MouseUp += new MouseButtonEventHandler(a.lmd);
            list_customer.KeyDown += new KeyEventHandler(a.lkd);
            txtcustomer.LostFocus += new RoutedEventHandler(a.tlf);
            a.nameseleceted += new EventHandler(cusnameselected);
            lst_report.ItemsSource = pr;
            autocomplete b = new autocomplete(txtcomname, list_comname, dr1, "com_name", "com_id,com_balstock,com_sp", layout, "com_sp");
            txtcomname.KeyUp += new KeyEventHandler(b.tkupcom);
            list_comname.MouseUp += new MouseButtonEventHandler(b.lmd);
            list_comname.KeyDown += new KeyEventHandler(b.lkd);
            txtcomname.LostFocus += new RoutedEventHandler(b.tlf);
            b.nameseleceted += new EventHandler(comnameselected);
            listBox1.ItemsSource = pc;
            datePicker1.SelectedDate = DateTime.Now;
            //   displayseting();
            //   txtvno.Text = vid.ToString();
            txtorderno.Text = orderid.ToString();
            f.fillcombo(cmb_uom);
            invoicedetail = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            dr = new autocomplete().getdatatable("select l_name,l_id from ledger where l_groupid = 3", "ledger");
            dr1 = new autocomplete().getdatatable("select com_name , com_id,com_balstock,com_sp from [stock]", "[stock]");
            pr = new purchasemodel().getpurchasereport();
            por = new purchasemodel().getpurchaseorderreport();
            vid = new purchasemodel().getvid();
            f = new Fillcombo("u_sym", "uom");
          orderid = new purchasemodel().getordervid();
        }
        void cusnameselected(object sender, EventArgs e)
        {
            txtcomname.IsEnabled = true;
            txtcomname.Focus();
            nameid ni = (nameid)list_customer.Items.GetItemAt(0);
            lid = int.Parse(ni.id);
        }
        void comnameselected(object sender, EventArgs e)
        {
            toggelcom(true);
            txtquantity.Focus();
            stock ni = (stock)list_comname.Items.GetItemAt(0);
            lbl_remainingbal.Content = ni.quantity;
            txtrate.Text = ni.rate;
            combal = double.Parse(ni.quantity);
            if (combal < 100)
            {
                lbl_remainingbal.Foreground = Brushes.Red;
            }
            else
            {
                lbl_remainingbal.Foreground = Brushes.Black;
            }
            cid = int.Parse(ni.id);
        }
        void toggelcom(bool a)
        {
            txtquantity.IsEnabled = a;
            txtrate.IsEnabled = a;
            cmb_uom.IsEnabled = a;
        }
        private void txtquantity_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;

            Double a = -1, b = -1;
            Double n;
            if (txtquantity.Text == "") { a = 0; }
            if (txtrate.Text == "") { b = 0; }

            if (!(Double.TryParse(txtquantity.Text, out n)) && a != 0)
            {
                Show("Please Enter Number",2);
                txtquantity.Text = "0";
            }

            if (!(Double.TryParse(txtrate.Text, out n)) && b != 0)
            {
                Show("Please Enter Number",2);
                txtrate.Text = "0";
            }
            if (a != 0) { a = Double.Parse(txtquantity.Text); }
            if (b != 0) { b = Double.Parse(txtrate.Text); }
            Double c = Math.Round(a, 2) * Math.Round(b, 2);


            txtamount.Text = c.ToString();
            if (t.Name == txtquantity.Name)
            {
                Double bal = (combal + a);
                lbl_remainingbal.Content = bal.ToString();
                if (bal < 100)
                {
                    lbl_remainingbal.Foreground = Brushes.Red;
                }
                else
                {
                    lbl_remainingbal.Foreground = Brushes.Black;
                }
            }
            if (e.Key == Key.Enter && txtamount.Text != "0" && cmb_uom.SelectedItem != null)
            {
                int pid;
                if (order)
                {
                    pid = orderid;
                }
                else
                {
                    pid = vid;
                }
                purchaseitem s = new purchaseitem()
                {
                    pid = pid,
                    pcomname = txtcomname.Text,
                    pcomid = int.Parse(cid.ToString()),
                    pqnty = Double.Parse(txtquantity.Text),
                    puom = cmb_uom.SelectedValue.ToString(),
                    prate = txtrate.Text,
                    pamt = Double.Parse(txtamount.Text),
                };
                pc.Add(s);
                //listBox1.Items.Add(s);
                total += Double.Parse(txtamount.Text);
                txttotalamount.Text = total.ToString();
                txtamount.Text = "";
                txtcomname.Text = "";
                txtquantity.Text = "";
                txtrate.Text = "";
                txtcomname.Focus();
                toggelcom(false);
                lbl_remainingbal.Content = "";
                combal = 0;
                lbl_remainingbal.Foreground = Brushes.Red;
            }
        }

        void reset()
        {
            lid = cid = 0;
            vat = addcharges = roundof = 0;
            pc.Clear();
            //sr.Clear();
            pce.Clear();
            txtcustomer.Text = "";
            txtcomname.Text = "";
            txttotalamount.Text = "";
            lbl_remainingbal.Content = "";
            txtroundof.Text = "";
            txtquantity.Text = "";
            txtrate.Text = "";
            txtvatrate1.Text = "";
            //txtac.Text = "";
            txtdisrate.Text = "";
            txtroundof.Text = "";
            txtacrate.Text = "";
            txtvat.Text = "";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (pc.Count != 0)
            {
                int tid = new transactionmodel().getid();
                purchase p = new purchase()
                {
                    pid = vid,
                    tid = tid,
                    lid = lid,
                    ptotal = total,
                    pdate = datePicker1.ToString(),
                    tnsp_chg = addcharges.ToString(),
                    vat_chg = vat.ToString(),
                    roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                    discount = "0",
                    sinvoiceno = int.Parse(txtvno.Text)
                };
                List<purchaseitem> pl = pc.ToList<purchaseitem>();
                purchasemodel pm = new purchasemodel(p, pl, total);
                if (pm.insert())
                {
                    Show("Data Saved", 2);
                    int i = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                    application.tb[i].Balance = application.tb[i].Balance + Double.Parse(txttotalamount.Text);
                    application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                    application.tb[i].BalanceType = "dr";
                    pc.Clear();
                    reset();
                    vid = pm.getvid();
                }
            }
            else
            {
               Show("Fields Cannot be Empty",2);
            }

        }
        private void txtrate_GotFocus(object sender, RoutedEventArgs e)
        {
            txtrate.SelectAll();
        }
        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            list_comname.Visibility = Visibility.Hidden;
            if (listBox1.SelectedValue != null && e.Key == Key.Delete)
            {
                purchaseitem s = (purchaseitem)listBox1.SelectedValue;
                txttotalamount.Text = (int.Parse(txttotalamount.Text) - s.pamt).ToString();
                pc.Remove(s);
            }
        }


        private void txtacrate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtacrate.Text != "")
            {
                if (addcharges != 0)
                {
                    txttotalamount.Text = (Double.Parse(txttotalamount.Text) - addcharges).ToString();
                }
                Double a = -1;
                Double n;
                if (txtacrate.Text == "") { a = 0; }

                if (!(Double.TryParse(txtacrate.Text, out n)) && a != 0)
                {
                    Show("Please Enter Number",2);
                    txtacrate.Text = "0";
                    a = 0;
                }
                if (a != 0)
                {
                    a = Double.Parse(txtacrate.Text);
                    txttotalamount.Text = (Double.Parse(txttotalamount.Text) + Double.Parse(txtacrate.Text)).ToString();
                    t2 = Double.Parse(txttotalamount.Text);
                }

                InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);

                addcharges = Double.Parse(txtacrate.Text);
            }
        }

        private void txtvatrate1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtvatrate1.Text != "")
            {
                if (vat != 0)
                {
                    txttotalamount.Text = (Double.Parse(txttotalamount.Text) - vat).ToString();
                }
                Double a = -1;
                Double n;
                if (txtvatrate1.Text == "") { a = 0; }

                if (!(Double.TryParse(txtvatrate1.Text, out n)) && a != 0)
                {
                    Show("Please Enter Number",2);
                    txtvatrate1.Text = "0";
                    a = 0;
                }
                if (a != 0)
                {
                    a = Double.Parse(txtvatrate1.Text);
                    vat = Math.Round(total * a / 100, 2);
                    txttotalamount.Text = (Double.Parse(txttotalamount.Text) + vat).ToString();
                    txtvat.Text = vat.ToString();
                }

                txtroundof.Focus();
            }
        }

        private void txtdisrate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtdisrate.Text != "")
            {
                Double dp = total * Double.Parse(txtdisrate.Text) / 100;
                txttotalamount.Text = (int.Parse(txttotalamount.Text) - dp).ToString();
                txtdisrate.Text = "";
                t2 = Double.Parse(txttotalamount.Text);
                if (txtvatrate1.Visibility != Visibility.Hidden)
                {
                    txtvatrate1.Focus();
                }
                else
                {
                    txtroundof.Focus();
                }
            }
        }
        private void txtroundof_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txttotalamount.Text.IndexOf('.') != -1)
            {
                int a = int.Parse(txttotalamount.Text.Split('.')[1]);
                roundof = Double.Parse("0." + a);
                if (roundof > 0.5)
                {
                    roundof = 1.00 - roundof;
                    txtroundof.Text = "+ " + roundof.ToString();
                }
                else
                {
                    txtroundof.Text = "- " + roundof.ToString();
                }
                txttotalamount.Text = Math.Round(Double.Parse(txttotalamount.Text), 0).ToString();

            }
            else
            {
                txtroundof.Text = "0";
                roundof = 0.0;
            }
        }
        //private void lst_report_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (lst_report.SelectedValue != null)
        //    {
        //        purchasereport sr = (purchasereport)lst_report.SelectedValue;
        //        purchase s = new purchasemodel().getinvoicedetail(int.Parse(sr.vid));
        //        listBox2.ItemsSource = new purchaseitemmodel().getinvoicedetail(int.Parse(sr.vid));
        //        rp_datePicker.Text = s.pdate;
        //        rp_customer.Content = s.lname;
        //        rp_round.Content = s.roundof;
        //        rp_tnsp.Content = s.tnsp_chg;
        //        rp_total.Content = s.ptotal;
        //        rp_vatrate.Content = s.vat_chg;
        //        rp_vno.Content = s.pid.ToString();
        //        rp_vat.Content = (Math.Round(double.Parse(s.vat_chg) / s.ptotal * 100, 2)).ToString();
        //        purchase_report.Visibility = Visibility.Hidden;
        //        invoice_details.Visibility = Visibility.Visible;
        //    }

        //}
        private void rback_Click(object sender, RoutedEventArgs e)
        {
            purchase_report.Visibility = Visibility.Visible;
            invoice_details.Visibility = Visibility.Hidden;
            //((Storyboard)this.Resources["purchase_wheel"]).Begin();


        }
        private void txtsvno_KeyUp(object sender, KeyEventArgs e)
        {
            lst_report.ItemsSource = null;
            lst_report.Items.Clear();
            TextBox t = (TextBox)sender;
            String str = t.Text;
            if (str == " ")
            {
                lst_report.ItemsSource = pr;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < pr.Count; i++)
                {
                    string a = pr[i].vid;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        lst_report.Items.Add(pr[i]);
                        j++;
                    }
                }
                if (j == 0)
                {
                    lblnoinvoice.Visibility = Visibility.Visible;
                }
                if (j > 0)
                {
                    lblnoinvoice.Visibility = Visibility.Hidden;
                }
            }

        }
        private void txtscusname_KeyUp(object sender, KeyEventArgs e)
        {

            lst_report.ItemsSource = null;
            lst_report.Items.Clear();
            TextBox t = (TextBox)sender;
            String str = t.Text;
            if (str == " ")
            {
                lst_report.ItemsSource = pr;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < pr.Count; i++)
                {
                    string a = pr[i].customername;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        lst_report.Items.Add(pr[i]);
                        j++;
                    }
                }
                if (j == 0)
                {
                    lblnoinvoice.Visibility = Visibility.Visible;
                }
                if (j > 0)
                {
                    lblnoinvoice.Visibility = Visibility.Hidden;
                }
            }
        }

        private void back(object sender, RoutedEventArgs e)
        {

        
        }

        private void setting_click(object sender, RoutedEventArgs e)
        {

            Setting.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Hidden;
        }

        private void Button_purchase_Order(object sender, RoutedEventArgs e)
        {
            reset();
            purchase.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Collapsed;
            lbl_header.Content = "Purchase Order";
            txtcustomer.IsEnabled = true;
            btnorder.Visibility = Visibility.Visible;
            button1.Visibility = Visibility.Hidden;
            order = true;
            txtvno.Visibility = Visibility.Hidden;
            txtorderno.Visibility = Visibility.Visible;
            label1.Content = "Order Number";
            Aditional_Chatges.Visibility = Visibility.Hidden;
            btncomitorder.Visibility = Visibility.Hidden;
        }

        private void purchasereportmenu_Click(object sender, RoutedEventArgs e)
        {
            Button_Wheel1.Visibility = Visibility.Collapsed;
            purchase_report.Visibility = Visibility.Visible;
           
            
        }

        private void purchase_Button_Click(object sender, RoutedEventArgs e)
        {
            //((Storyboard)this.Resources["purchase_order"]).Begin();
            purchase.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Collapsed;
            //lbl_header.Content = "Credit Purchase";
            txtcustomer.IsEnabled = true;
            invoicenumber();
            reset();
            txtvno.Focus();

        }

        private void purchase_Button_Click_1(object sender, RoutedEventArgs e)
        {
            ////////////////////////////((Storyboard)this.Resources["purchase_order"]).Begin();
            purchase.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Hidden;
            // lbl_header.Content = "Cash Purchase";
            invoicenumber();
            reset();
            txtcomname.IsEnabled = true;
            txtcustomer.IsEnabled = false;
            lid = 1;
            txtvno.Focus();
        }
        public void invoicenumber()
        {
            order = false;
            txtvno.Visibility = Visibility.Visible;
            txtorderno.Visibility = Visibility.Hidden;
            label1.Content = "Supplier Number";
            txtcustomer.IsEnabled = true;
            btnorder.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Visible;
            Aditional_Chatges.Visibility = Visibility.Visible;
            btncomitorder.Visibility = Visibility.Hidden;
        }
        private void back_click(object sender, RoutedEventArgs e)
        {
           ///////////////////////////((Storyboard)this.Resources["purchase_wheel"]).Begin();
            purchase.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;

        }

        private void rpback_Click(object sender, RoutedEventArgs e)
        {
            purchase_report.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;


        }

        #region settings options

        private void cb_additionalcharges_Checked(object sender, RoutedEventArgs e)
        {

            txtac.IsEnabled = true;
            btnac.IsEnabled = true;
            config.AppSettings.Settings["add_charges"].Value = "yes";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            //txtac1.Visibility = Visibility.Visible;
            txtacrate.Visibility = Visibility.Visible;
            lblac.Visibility = Visibility.Visible;
        }

        private void cb_additionalcharges_Unchecked(object sender, RoutedEventArgs e)
        {
            txtac.IsEnabled = false;
            btnac.IsEnabled = false;
            //txtac1.Visibility = Visibility.Hidden;
            txtacrate.Visibility = Visibility.Hidden;
            lblac.Visibility = Visibility.Hidden;
            config.AppSettings.Settings["add_charges"].Value = "no";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void btnac_Click(object sender, RoutedEventArgs e)
        {
            ledgermodel l = new ledgermodel(txtac.Text, 11, "", "", "");
            if (l.insert(this))
            {
                Show("Saved Susessfully",1);
                txtac.Text = "";
            }
        }

        private void cb_discount_Checked(object sender, RoutedEventArgs e)
        {
            //  txtdiscount.IsEnabled = true;
            //  btndiscount.IsEnabled = true;
            config.AppSettings.Settings["discount"].Value = "yes";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            lbldc.Visibility = Visibility.Visible;
            //txtdis.Visibility = Visibility.Visible;
            txtdisrate.Visibility = Visibility.Visible;
        }

        private void cb_discount_Unchecked(object sender, RoutedEventArgs e)
        {
            //  txtdiscount.IsEnabled = false;
            //  btndiscount.IsEnabled = false;
            config.AppSettings.Settings["discount"].Value = "no";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            lbldc.Visibility = Visibility.Hidden;
            //txtdis.Visibility = Visibility.Hidden;
            txtdisrate.Visibility = Visibility.Hidden;
        }


        void displayseting()
        {
            if (ConfigurationSettings.AppSettings["discount"] == "no")
            {
                cb_discount.IsChecked = false;
                lbldc.Visibility = Visibility.Hidden;
                //txtdis.Visibility = Visibility.Hidden;
                txtdisrate.Visibility = Visibility.Hidden;
            }
            else
            {
                cb_discount.IsChecked = true;
                lbldc.Visibility = Visibility.Visible;
                //txtdis.Visibility = Visibility.Visible;
                txtdisrate.Visibility = Visibility.Visible;
            }
            if (ConfigurationSettings.AppSettings["vat"] == "no")
            {
                lblvat.Visibility = Visibility.Hidden;
                cb_vat.IsChecked = false;
                //txtvat1.Visibility = Visibility.Hidden;
                txtvatrate1.Visibility = Visibility.Hidden;
            }
            else
            {
                cb_vat.IsChecked = true;
                //txtvat1.Visibility = Visibility.Visible;
                txtvatrate1.Visibility = Visibility.Visible;
                lblvat.Visibility = Visibility.Visible;
            }
            if (ConfigurationSettings.AppSettings["add_charges"] == "no")
            {
                cb_additionalcharges.IsChecked = false;
                //txtac1.Visibility = Visibility.Hidden;
                txtacrate.Visibility = Visibility.Hidden;
                lblac.Visibility = Visibility.Hidden;
            }
            else
            {
                cb_additionalcharges.IsChecked = true;
                //txtac1.Visibility = Visibility.Visible;
                txtacrate.Visibility = Visibility.Visible;
                lblac.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void cb_vat_Checked(object sender, RoutedEventArgs e)
        {
            //txtvat.IsEnabled = true;
            //btnvat.IsEnabled = true;
            config.AppSettings.Settings["vat"].Value = "yes";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            //txtvat1.Visibility = Visibility.Visible;
            txtvatrate1.Visibility = Visibility.Visible;
            lblvat.Visibility = Visibility.Visible;
        }

        private void cb_vat_Unchecked(object sender, RoutedEventArgs e)
        {
            //txtvat.IsEnabled = false;
            //btnvat.IsEnabled = false;
            config.AppSettings.Settings["vat"].Value = "no";
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            lblvat.Visibility = Visibility.Hidden;
            //txtvat1.Visibility = Visibility.Hidden;
            txtvatrate1.Visibility = Visibility.Hidden;
        }

        //private void btndiscount_Click(object sender, RoutedEventArgs e)
        //{
        //    ledgermodel l = new ledgermodel(txtdiscount.Text, "Indirect Expenses", "", "", "");
        //    if (l.insert(this))
        //    {
        //        MessageBox.Show("Saved Susessfull");
        //        txtdiscount.Text = "";
        //    } 
        //}

        //private void btnvat_Click(object sender, RoutedEventArgs e)
        //{
        //    ledgermodel l = new ledgermodel(txtvat.Text, "Duties & Taxes", "", "", "");
        //    if (l.insert(this))
        //    {
        //        MessageBox.Show("Saved Susessfull");
        //        txtvat.Text = "";
        //    }
        //}

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Setting.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;
            ((Storyboard)this.Resources["purchase_wheel"]).Begin();

        }
        #endregion

        private void btnsaleorder_Click(object sender, RoutedEventArgs e)
        {
            if (lst_report.ItemsSource == null)
            {
                lst_report.Items.Clear();
            }
            lst_report.ItemsSource = por;
            //   btnsaleorder.Visibility = Visibility.Hidden;
            //btnsalereport.Visibility = Visibility.Visible;
            invoicedetail = false;
        }

        private void btnsalereport_Click(object sender, RoutedEventArgs e)
        {
            if (lst_report.ItemsSource == null)
            {
                lst_report.Items.Clear();
            }
            lst_report.ItemsSource = pr;
            //    btnsalereport.Visibility = Visibility.Hidden;
            //    btnsaleorder.Visibility = Visibility.Visible;
            invoicedetail = true;
        }

        private void btnorder1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lst_report_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_report.SelectedValue != null)
            {

                lbl_header.Content = "Purchase Invoice";
                purchasereport sr = (purchasereport)lst_report.SelectedValue;
                purchase s;
                if (invoicedetail)
                {
                    listBox2.ItemsSource = new purchaseitemmodel().getinvoicedetail(int.Parse(sr.vid));
                    s = new purchasemodel().getinvoicedetail(int.Parse(sr.vid));
                    rp_datePicker.Text = s.pdate;
                    rp_customer.Content = s.lname;
                    rp_round.Content = s.roundof;
                    rp_tnsp.Content = s.tnsp_chg;
                    rp_total.Content = s.ptotal;
                    rp_vatrate.Content = s.vat_chg;
                    rp_vno.Content = s.pid.ToString();
                    rp_vat.Content = (Math.Round(double.Parse(s.vat_chg) / s.ptotal * 100, 2)).ToString();
                    purchase_report.Visibility = Visibility.Hidden;
                    invoice_details.Visibility = Visibility.Visible;
                }
                else
                {
                    reset();
                    List<purchaseitem> soc = new purchaseitemmodel().getorderedetail(int.Parse(sr.vid));
                    foreach (var item in soc)
                    {

                        purchaseitem sa = new purchaseitem()
                        {
                            pid = vid,
                            pcomname = item.pcomname,
                            pcomid = item.pcomid,
                            pqnty = item.pqnty,
                            puom = item.puom,
                            prate = item.prate,
                            pamt = item.pamt,
                        };
                        pc.Add(sa);
                    }
                    s = new purchasemodel().getorderdetail(int.Parse(sr.vid));
                    purchase_report.Visibility = Visibility.Hidden;
                    purchase.Visibility = Visibility.Visible;
                    invoicenumber();
                    btncomitorder.Visibility = Visibility.Visible;
                    button1.Visibility = Visibility.Hidden;
                    txtcustomer.Text = s.lname;
                    txttotalamount.Text = s.nettotal.ToString();
                    total = s.ptotal;
                    lid = s.lid;
                    comitoid = s.pid;
                    txtacrate.Focus();
                }
            }

        }

        private void btnorder_Click(object sender, RoutedEventArgs e)
        {
            if (pc.Count != 0)
            {
                // int tid = new transactionmodel().getid();
                purchase s = new purchase()
                {

                    //   tid = tid,
                    lid = lid,
                    ptotal = total,
                    pdate = datePicker1.ToString(),
                    pid = int.Parse(txtorderno.Text),
                    /// tnsp_chg = addcharges.ToString(),
                    //////// vat_chg = vat.ToString(),
                    //  roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                    //  discount = "0"
                };
                List<purchaseitem> sl = pc.ToList<purchaseitem>();
                purchasemodel sm = new purchasemodel(s, sl, total);
                if (sm.orderinsert())
                {
                    Show("Data Saved", 1);
                    pc.Clear();
                    reset();
                    txtorderno.Text = sm.getordervid().ToString();
                    orderid = int.Parse(txtorderno.Text);
                }
            }
            else
            {
               Show("Fields Cannot be empty",2);
            }

        }

        private void btncomitorder_Click(object sender, RoutedEventArgs e)
        {
            if (pc.Count != 0)
            {
                int tid = new transactionmodel().getid();
                purchase s = new purchase()
                {
                    tid = tid,
                    lid = lid,
                    ptotal = total,
                    pdate = datePicker1.ToString(),
                    pid = vid,
                    tnsp_chg = addcharges.ToString(),
                    vat_chg = vat.ToString(),
                    roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                    discount = "0"
                };
                List<purchaseitem> sl = pc.ToList<purchaseitem>();
                purchasemodel sm = new purchasemodel(s, sl, total);
                if (sm.comitorder(comitoid))
                {
                    Show("Data Saved",1);
                    int i = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                    application.tb[i].Balance = application.tb[i].Balance + Double.Parse(txttotalamount.Text);
                    application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                    application.tb[i].BalanceType = "dr";
                    pc.Clear();
                    reset();
                    vid = int.Parse(sm.getvid().ToString());
                }
            }
            else
            {
                Show("Fields Cannot be empty",2);
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
