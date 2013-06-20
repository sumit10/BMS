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
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace BMS.Views
{
    /// <summary>
    /// Interaction logic for home.xaml
    /// </summary>
    public partial class home : UserControl
    {
        List<stock> sd;
        List<ledgerbalance> dr;
        List<ledgerbalance> cr;
        public home()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             dr = application.tb.Where(l => l.BalanceType == "dr" && (l.Lgroup==3 || l.Lgroup==2) && l.Balance!=0)
                                                    .OrderByDescending(l => l.Balance).ToList<ledgerbalance>();
             cr = application.tb.Where(l => l.BalanceType == "cr" && (l.Lgroup == 3 || l.Lgroup == 2) && l.Balance!=0)
                                                   .OrderByDescending(l => l.Balance).ToList<ledgerbalance>();
            List<ledgerbalance> cb = application.tb.Where(l => l.Lgroup == 1 || l.Lgroup == 14).ToList<ledgerbalance>();
            lst_dr.ItemsSource = dr;
            lst_cr.ItemsSource = cr;
            lst_cash.ItemsSource = cb;
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            sd = new commoditymodel().getstock().OrderBy(s=>s.balstock).ToList<stock>();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lst_stock.ItemsSource = sd; 
        }

        private void expand(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["debtor"]).Begin();
            
        }

        private void back_top(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["debtor_reverse"]).Begin();
        }

        private void creditors_expand(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["creditor"]).Begin();

        }

        private void cr_back_top(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["creditor_reverse"]).Begin();

        }

        private void top_debtor_MouseEnter(object sender, MouseEventArgs e)
        {
            //print_btn.Visibility = Visibility.Visible;
            //expand_btn.Visibility = Visibility.Visible;
        }

        private void top_debtor_MouseLeave(object sender, MouseEventArgs e)
        {
           //print_btn.Visibility = Visibility.Hidden ;
           //expand_btn.Visibility = Visibility.Hidden;
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            try
             {
                    report.report_cr_dr p = new BMS.report.report_cr_dr();
                p.lst_balance.ItemsSource = dr;
                p.r_date.Content = DateTime.Now.Date.ToShortDateString();
                p.r_name.Content = "Top Debitors";
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry some system error has occour please try again");
            }
        }

        private void print_cr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                report.report_cr_dr p = new BMS.report.report_cr_dr();
                p.lst_balance.ItemsSource = cr;
                p.r_date.Content = DateTime.Now.Date.ToShortDateString();
                p.r_name.Content = "Top Creditors";
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
            catch (Exception ex)
            {
                MessageBox.Show("Sorry some system error has occour please try again"); 
            }
        }
    }
    
}
