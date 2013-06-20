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
using System.Configuration;
using WindowsInput;
using System.Data;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for saleview.xaml
    /// </summary>
    public partial class saleview : UserControl
    {
        int lid, cid,vid,orderid,comitoid;
        Double vat, addcharges, roundof, combal;
        Double total, t2;
        DataTable dr, dr1;
        Fillcombo f;
        bool order,invoicedetail;
        ObservableCollection<salesitem> sc = new ObservableCollection<salesitem>() ,sce = new ObservableCollection<salesitem>();
        ObservableCollection<salesreport> sr,sor;
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public saleview()
        {
            InitializeComponent();
          //  autocomplete.fillcombo(cmb_uom, "u_sym", "uom");
            vat = 0; addcharges = 0; roundof = 0;
            total = 0;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            autocomplete a = new autocomplete(txtcustomer, list_customer, dr, "l_name", "l_id", layout);
            txtcustomer.KeyUp += new KeyEventHandler(a.tkupid);
            list_customer.MouseUp += new MouseButtonEventHandler(a.lmd);
            list_customer.KeyDown += new KeyEventHandler(a.lkd);
            txtcustomer.LostFocus += new RoutedEventHandler(a.tlf);
            a.nameseleceted += new EventHandler(cusnameselected);
            lst_report.ItemsSource = sr;
            autocomplete b = new autocomplete(txtcomname, list_comname, dr1, "com_name", "com_id,com_balstock,com_sp", layout, "com_sp");
            txtcomname.KeyUp += new KeyEventHandler(b.tkupcom);
            list_comname.MouseUp += new MouseButtonEventHandler(b.lmd);
            list_comname.KeyDown += new KeyEventHandler(b.lkd);
            txtcomname.LostFocus += new RoutedEventHandler(b.tlf);
            b.nameseleceted += new EventHandler(comnameselected);
            listBox1.ItemsSource = sc;
            datePicker1.SelectedDate = DateTime.Now;
            displayseting();
            txtvno.Text = vid.ToString();
            txtorderno.Text = orderid.ToString();
            f.fillcombo(cmb_uom);
            invoicedetail = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            dr = new autocomplete().getdatatable("select l_name,l_id from ledger where l_groupid = 2", "ledger");
            dr1 = new autocomplete().getdatatable("select com_name , com_id,com_balstock,com_sp from [stock]", "[stock]");
            sr = new salesmodel().getsalesreport();
            sor = new salesmodel().getsalesorderreport();
            vid = new salesmodel().getvid();
            f = new Fillcombo("u_sym", "uom");
            orderid = new salesmodel().getordervid();
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
                Double bal = (combal - a);
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
                if (int.Parse(lbl_remainingbal.Content.ToString()) <= 0)
                {
                    Show("insufficient balance ",2);
                    txtquantity.Focus();
                    return;
                }
                int sid;
                if (order)
                {
                    sid = int.Parse(txtorderno.Text);
                }
                else
                {
                    sid = int.Parse(txtvno.Text);
                }
                salesitem s = new salesitem()
                {
                    sid = sid,
                    scomname = txtcomname.Text,
                    scomid = int.Parse(cid.ToString()),
                    sqnty = Double.Parse(txtquantity.Text),
                    suom = cmb_uom.SelectedValue.ToString(),
                    srate = txtrate.Text,
                    samt = Double.Parse(txtamount.Text),
                };
                sc.Add(s);
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


        void toggelcom(bool a)
        {
            txtquantity.IsEnabled = a;
            txtrate.IsEnabled = a;
            cmb_uom.IsEnabled = a;
        }

        void reset()
        {
            lid = cid = 0;
            vat = addcharges = roundof = 0;
            sc.Clear();
            //sr.Clear();
            sce.Clear();
            txtcustomer.Text = "";
            txtcomname.Text = "";
            txttotalamount.Text = "";
            lbl_remainingbal.Content = "";
            txtroundof.Text = "";
            txtquantity.Text = "";
            txtrate.Text = "";
            txtvatrate1.Text = "";
            txtac.Text = "";
            txtdisrate.Text = "";
            txtroundof.Text = "";
            txtacrate.Text = "";
            txtvat.Text = "";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (sc.Count != 0)
            {
                int tid = new transactionmodel().getid();
                sales s = new sales()
                {
                    tid = tid,
                    lid = lid,
                    stotal = total,
                    sdate = datePicker1.ToString(),
                    sid = int.Parse(txtvno.Text),
                    tnsp_chg = addcharges.ToString(),
                    vat_chg = vat.ToString(),
                    roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                    discount = "0"
                };
                List<salesitem> sl = sc.ToList<salesitem>();
                salesmodel sm = new salesmodel(s, sl, total);
                if (sm.insert())
                {
                    Show("Saved Succesfully",1);
                    int i = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                    application.tb[i].Balance = application.tb[i].Balance + Double.Parse(txttotalamount.Text);
                    application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                    application.tb[i].BalanceType = "cr";
                    sc.Clear();
                    reset();
                    txtvno.Text = sm.getvid().ToString();
                }
            }
            else
            {
               Show("Fields cannot be empty",2);
            }

        }

        private void txtrate_GotFocus(object sender, RoutedEventArgs e)
        {
            txtrate.SelectAll();
        }
        # region menu button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["common_sale"]).Begin();
            ((Storyboard)this.Resources["wheel_fade"]).Begin();
            sales.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Collapsed;
            lbl_header.Content = "Credit Memo";
            txtcustomer.IsEnabled = true;
            reset();
            ////////sc.Clear();
            txtcustomer.Focus();
            invoicenumber();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["common_sale"]).Begin();
            sales.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Collapsed;
            lbl_header.Content = "Cash Memo";
            reset();
            txtcomname.IsEnabled = true;
            txtcustomer.IsEnabled = false;
            lid = 1;
            txtcomname.Focus();
            //sc.Clear();
            invoicenumber();
        }
        public void invoicenumber()
        {
            order = false;
            txtvno.Visibility = Visibility.Visible;
            txtorderno.Visibility = Visibility.Hidden;
            label1.Content = "Invoice Number";
            txtcustomer.IsEnabled = true;
            btnorder.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Visible;
            Aditional_Chatges.Visibility = Visibility.Visible;
            btncomitorder.Visibility = Visibility.Hidden;
        }
        private void Button_Sales_Order(object sender, RoutedEventArgs e)
        {
            reset();
            ((Storyboard)this.Resources["common_sale"]).Begin();
            sales.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Collapsed;
            lbl_header.Content = "Sales Order";
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

        private void back_click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["sale_fade"]).Begin();
            ((Storyboard)this.Resources["wheel_fade"]).Begin();
            sales.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Setting.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;
            ((Storyboard)this.Resources["wheel_fade"]).Begin();
        }

        private void setting_click(object sender, RoutedEventArgs e)
        {
            Setting.Visibility = Visibility.Visible;
            Button_Wheel1.Visibility = Visibility.Hidden;
        }
        #endregion
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
            ledgermodel l = new ledgermodel(txtac.Text,11,"","","");
            if (l.insert(this))
            {
                Show("Saved Susessfull",1);
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
        #endregion

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            list_comname.Visibility = Visibility.Hidden;
            if (listBox1.SelectedValue != null && e.Key == Key.Delete)
            {
                salesitem s = (salesitem)listBox1.SelectedValue;
                txttotalamount.Text = (int.Parse(txttotalamount.Text) - s.samt).ToString();
                sc.Remove(s);
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
                    Show("Please Enter any Number",2);
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
                    Show("Please Enter any Number!!!!!", 2);
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
        private void lst_report_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_report.SelectedValue != null)
            {
               
                lbl_header.Content = "Sales Invoice";
                salesreport sr = (salesreport)lst_report.SelectedValue;
                sales s;
                if (invoicedetail)
                {
                    listBox2.ItemsSource = new salesitemmodel().getinvoicedetail(int.Parse(sr.vid));
                    s = new salesmodel().getinvoicedetail(int.Parse(sr.vid));
                    rp_datePicker.Text = s.sdate;
                    rp_customer.Content = s.lname;
                    rp_round.Content = s.roundof;
                    rp_tnsp.Content = s.tnsp_chg;
                    rp_total.Content = s.stotal;
                    rp_vatrate.Content = s.vat_chg;
                    rp_vno.Content = s.sid.ToString();
                    lid = s.lid;
                    rp_vat.Content = (Math.Round(double.Parse(s.vat_chg) / s.stotal * 100, 2)).ToString();
                    sales_report.Visibility = Visibility.Hidden;
                    invoice_details.Visibility = Visibility.Visible;
                }
                else
                {
                    reset();
                    List<salesitem> soc = new salesitemmodel().getorderedetail(int.Parse(sr.vid));
                    foreach (var item in soc)
                    {
                    
                        salesitem sa = new salesitem()
                        {
                            sid = int.Parse(txtvno.Text),
                            scomname = item.scomname,
                            scomid = item.scomid,
                            sqnty = item.sqnty,
                             suom = item.suom,
                            srate = item.srate,
                            samt = item.samt,
                        };
                        sc.Add(sa);
                    }
                    s = new salesmodel().getorderdetail(int.Parse(sr.vid));
                    sales_report.Visibility = Visibility.Hidden;
                    ((Storyboard)this.Resources["common_sale"]).Begin();
                    invoicenumber();
                    btncomitorder.Visibility = Visibility.Visible;
                    button1.Visibility = Visibility.Hidden;
                    txtcustomer.Text = s.lname;
                    txttotalamount.Text = s.nettotal.ToString();
                    total = s.stotal;
                    lid = s.lid;
                    comitoid = s.sid;
                    txtacrate.Focus();
                }
            }

        }
        private void print_Click(object sender, RoutedEventArgs e)
         {
             ledger l = new ledgermodel().getprintdetail(lid);
             report.print_invoice p = new BMS.report.print_invoice();
             p.tax_no.Content = rp_vno.Content;
             p.challan_no.Content = rp_vno.Content;
             p.lbl_cusname.Content = rp_customer.Content;
             p.grand_tot.Content = rp_total.Content;
             p.freight.Content = rp_tnsp.Content;
             p.roundoff.Content = rp_round.Content;
             p.vat_Copy.Content = rp_vatrate.Content;
             p.vat.Content = rp_vat.Content;
             p.tax_date.Content = rp_datePicker.Text;
             p.challan_date.Content = rp_datePicker.Text;
             p.lbl_address.Content = l.Laddress;
             // p.lbl_vatrate.Content = txtvat.Text;
             //p.lbl_aworld.Text = NumberToWords(int.Parse(txttotalamount.Text));
             p.total_amt.Content = rp_total.Content; ;
             p.lst_item.ItemsSource = listBox2.Items;
             PrintDialog pd = new PrintDialog();
             if (pd.ShowDialog() != true) return;
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


        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " lack ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        private void rback_Click(object sender, RoutedEventArgs e)
        {
            sales_report.Visibility = Visibility.Visible;
            invoice_details.Visibility = Visibility.Hidden;

        }
        private void rpback_Click(object sender, RoutedEventArgs e)
        {
            sales_report.Visibility = Visibility.Hidden;
            Button_Wheel1.Visibility = Visibility.Visible;
        }
        private void txtsvno_KeyUp(object sender, KeyEventArgs e)
        {
            lst_report.ItemsSource = null;
            lst_report.Items.Clear();
            TextBox t = (TextBox)sender;
            String str = t.Text;
            if (str == " ")
            {
                lst_report.ItemsSource = sr;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < sr.Count; i++)
                {
                    string a = sr[i].vid;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        lst_report.Items.Add(sr[i]);
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
                lst_report.ItemsSource = sr;
            }
            else
            {
                int j = 0;
                for (int i = 0; i < sr.Count; i++)
                {
                    string a = sr[i].customername;
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        lst_report.Items.Add(sr[i]);
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

        private void salereportmenu_Click(object sender, RoutedEventArgs e)
        {
            lbl_header.Content = "Report";
            sales_report.Visibility = Visibility.Visible;
           Button_Wheel1.Visibility = Visibility.Hidden;
        }

        private void close_all(object sender, RoutedEventArgs e)
        {

        }

        private void btnorder1_Click(object sender, RoutedEventArgs e)
        {
            if (sc.Count != 0)
            {
               // int tid = new transactionmodel().getid();
                sales s = new sales()
                {
                //   tid = tid,
                    lid = lid,
                    stotal = total,
                    sdate = datePicker1.ToString(),
                    sid = int.Parse(txtorderno.Text),
                  /// tnsp_chg = addcharges.ToString(),
                   //////// vat_chg = vat.ToString(),
                  //  roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                  //  discount = "0"
                };
                List<salesitem> sl = sc.ToList<salesitem>();
                salesmodel sm = new salesmodel(s, sl, total);
                if (sm.orderinsert())
                {
                    Show("Saved Sucessfully",1);
                    sc.Clear();
                    reset();
                    txtorderno.Text = sm.getordervid().ToString();
                }
            }
            else
            {
               Show("Please enter some details",2);
            }

        }

        private void btnsaleorder_Click(object sender, RoutedEventArgs e)
        {
            if (lst_report.ItemsSource == null)
            {
                lst_report.Items.Clear();
            }
            lst_report.ItemsSource = sor;
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
            lst_report.ItemsSource = sr;
        //    btnsalereport.Visibility = Visibility.Hidden;
        //    btnsaleorder.Visibility = Visibility.Visible;
            invoicedetail = true;
        }

        private void btncomitorder_Click(object sender, RoutedEventArgs e)
        {
            if (sc.Count != 0)
            {
                int tid = new transactionmodel().getid();
                sales s = new sales()
                {
                    tid = tid,
                    lid = lid,
                    stotal = total,
                    sdate = datePicker1.ToString(),
                    sid = int.Parse(txtvno.Text),
                    tnsp_chg = addcharges.ToString(),
                    vat_chg = vat.ToString(),
                    roundof = roundof.ToString(),
                    nettotal = Double.Parse(txttotalamount.Text),
                    discount = "0"
                };
                List<salesitem> sl = sc.ToList<salesitem>();
                salesmodel sm = new salesmodel(s, sl, total);
                if (sm.comitorder(comitoid))
                {
                    Show("Saved Succesfully",1);
                    int i = application.tb.Where(x => x.Lid == lid).Select<ledgerbalance, int>(x => application.tb.IndexOf(x)).Single<int>();
                    application.tb[i].Balance = application.tb[i].Balance + Double.Parse(txttotalamount.Text);
                    application.tb[i].CurrBalance = application.tb[i].Balance.ToString("C2");
                    application.tb[i].BalanceType = "cr";
                    sc.Clear();
                    reset();
                    txtvno.Text = sm.getvid().ToString();
                }
            }
            else
            {
                Show("Feilds Cannot Be empty",2);
            }
            
        }

        private void msgclick(object sender, RoutedEventArgs e)
        {
            message_box.Visibility = Visibility.Hidden;
        }

        void Show(string msg,int a)
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
