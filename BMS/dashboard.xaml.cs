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
using System.Windows.Shapes;
using BMS.Views;
using System.Configuration;
using System.ComponentModel;
using BMS.Model;
using System.Windows.Media.Animation;

namespace BMS
{
    /// <summary>
    /// Interaction logic for dashboard.xaml
    /// </summary>
    class chart
    {
        public int value { get; set; }
        public string key { get; set; }
    }
    public partial class dashboard : Window
    {
        string copper, allunimium,zinc,nickel,lastupdate;
        int i;
        Storyboard update;
        bool offline;
        public dashboard()
        {
            InitializeComponent();
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = 0;
            this.Top = 0;
            i = 60;
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            this.WindowState = WindowState.Normal;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            home h = new home();
            placeholder.Content = h;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
          application app = new application();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            i++;
            if (i >= 60)
            {
                i = 0;
                BackgroundWorker worker = new BackgroundWorker();
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(rateworkcompleted);
                worker.DoWork += new DoWorkEventHandler(rateworker_DoWork);
                lbllastmodi.Visibility = Visibility.Hidden;
                lbloffline.Visibility = Visibility.Hidden;
                lbl_updating.Visibility = Visibility.Visible;
                update = (Storyboard)this.Resources["update"];
                update.Begin();
                worker.RunWorkerAsync();
            }
        }
        void rateworker_DoWork(object sender, DoWorkEventArgs e)
        {


            com_rate cr = new com_rate();
            copper = cr.findrate("copper");
            allunimium = cr.findrate("alluminium");
            zinc = cr.findrate("zinc");
            nickel = cr.findrate("nickel");
            if (copper == null)
            {
                copper = ConfigurationSettings.AppSettings["copper"];
                allunimium = ConfigurationSettings.AppSettings["alluminium"];
                zinc = ConfigurationSettings.AppSettings["zinc"];
                nickel = ConfigurationSettings.AppSettings["nickel"];
                lastupdate = ConfigurationSettings.AppSettings["lastmmodify"];
                offline = true;

            }
            else
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["copper"].Value = copper;
                config.AppSettings.Settings["alluminium"].Value = allunimium;
                config.AppSettings.Settings["zinc"].Value = zinc;
                config.AppSettings.Settings["nickel"].Value = nickel;
                ConfigurationSettings.AppSettings["lastmmodify"] = lastupdate;
                config.AppSettings.Settings["lastmmodify"].Value = DateTime.Now.ToString();
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                offline = false;
            }
        }
        void rateworkcompleted(object sender, RunWorkerCompletedEventArgs r)
        {
            lbl_alu.Content = allunimium;
            lbl_copper.Content = copper;
            lbl_nickel.Content = nickel;
            lbl_zinc.Content = zinc;
            update.Stop();
            lbl_updating.Visibility = Visibility.Hidden;
            if (offline)
            {
                lbloffline.Visibility = Visibility.Visible;
                lbllastmodi.Content = lastupdate;
                lbllastmodi.Visibility = Visibility.Visible;
            }
        }

       private void button1_Click_1(object sender, RoutedEventArgs e)
        {

            Window1 w = new Window1();
            w.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.customerview cb = new BMS.Views.customerview();
            placeholder.Content = cb;
            btn_close.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Views.supplierview sv = new BMS.Views.supplierview();
            placeholder.Content = sv;
            btn_close.Visibility = Visibility.Visible;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            stockview st = new stockview();
            placeholder.Content = st;
            btn_close.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           // System.Windows.MessageBox.Show("Hight =" + r1.ActualHeight.ToString() + " Width = " + r1.ActualWidth.ToString());
        }

        private void button1_Click_2(object sender, RoutedEventArgs e)
        {
            saleview sv = new saleview();
            placeholder.Content = sv;
            btn_close.Visibility = Visibility.Visible;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            purchaseview pv = new purchaseview();
            placeholder.Content = pv;
            btn_close.Visibility = Visibility.Visible;
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["logout"]).Begin();
            dash_setting.Visibility = Visibility.Visible;
            btnvpass.Focus();
        }

        private void dash_setting_LostFocus(object sender, RoutedEventArgs e)
        {
           // dash_setting.Visibility = Visibility.Hidden;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Cashview cv = new Cashview();
            placeholder.Content = cv;
            btn_close.Visibility = Visibility.Visible;
        }

        private void btn_home_Click(object sender, RoutedEventArgs e)
        {
            home h = new home();
            placeholder.Content = h;
        }

        private void btnclose(object sender, System.Windows.RoutedEventArgs e)
        {
            home h = new home();
            placeholder.Content = h;
            btn_close.Visibility = Visibility.Hidden;
        }

        private void changepassword(object sender, System.Windows.RoutedEventArgs e)
        {
            if (pd_oldpass.Password.Trim().ToString() == "" || pd_newpass.Password.Trim().ToString() == "" || pb_comform.Password.Trim().ToString() == "")
            {
                MessageBox.Show("Please fill all field");
                return;
            }
            if (pd_newpass.Password.Trim().ToString() != pb_comform.Password.Trim().ToString())
            {
                MessageBox.Show("New Password and confirm password does not match");
                return;
            }
            loginmodel l = new loginmodel();
            if (l.update(1,pd_oldpass.Password.Trim().ToString(),pd_newpass.Password.Trim().ToString()))
            {
                MessageBox.Show("Password Changed Sucessfully");   
            }
        }

        private void btn_cpass(object sender, System.Windows.RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["changepassword"]).Begin();
            changepass.Visibility = Visibility.Visible;
            dash_setting.Visibility = Visibility.Hidden;
        }

        private void btnsettinghide(object sender, System.Windows.RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["reverse_logout"]).Begin();
            dash_setting.Visibility = Visibility.Hidden;
        }

        private void btnchangepass(object sender, System.Windows.RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["reverse_password"]).Begin();
            changepass.Visibility = Visibility.Hidden;
        }
    }
}

