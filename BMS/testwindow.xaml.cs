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
using System.Collections;
using System.Data.OleDb;
using System.Data;
using BMS.Model;
using WindowsInput;
using System.Windows.Markup;


namespace BMS
{
    /// <summary>
    /// Interaction logic for testwindow.xaml
    /// </summary>
    public partial class testwindow : Window
    {
        //DataTable dt;
        List<testmodel> ledger;
        public testwindow()
        {
            InitializeComponent();
            ledger l = new ledger() {Lname = "hello" , Lid = 45 };
            listBox1.Items.Add(l);
            ledger l2 = new ledger() { Lname = "yo", Lid = 755252};
            listBox1.Items.Add(l2);
            double a = 12050314;
            textBox1.Text = a.ToString("C2");
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            //int n;
            //bool a = Int32.TryParse("14ssd",out n);
            //textBox1.Text = (Int32.TryParse("14ssd", out n)) ? "yo" : "no";
            //String a = "Max:1";
            //int z = a.IndexOf(":");
            //String s = a.Substring(z+1);
            //textBox1.Text = s;

            //PrintDialog pd = new PrintDialog();
            //if (pd.ShowDialog() != true) return;
            //FixedDocument document = new FixedDocument();
            //document.DocumentPaginator.PageSize = new Size(96*8.5 ,96*11);
            //FixedPage page1 = new FixedPage();
            //page1.Width = document.DocumentPaginator.PageSize.Width;
            //page1.Height = document.DocumentPaginator.PageSize.Height;
            //report.print_invoice p = new BMS.report.print_invoice();
            
            //Canvas can = p.layout;
            
            //page1.Children.Add(can);
            //PageContent page1Content = new PageContent();
            //((IAddChild)page1Content).AddChild(page1);
            //document.Pages.Add(page1Content);
            //pd.PrintDocument(document.DocumentPaginator, "My first document");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //commoditymodel cm = new commoditymodel(14);
            //textBox1.Text = cm.getquanty().ToString();
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("Neel", 50000));
            valueList.Add(new KeyValuePair<string, int>("Raj", 2000));
            valueList.Add(new KeyValuePair<string, int>("Tester", 0));
            valueList.Add(new KeyValuePair<string, int>("QA", 3000));
            valueList.Add(new KeyValuePair<string, int>("Project Manager", 40));
            valueList.Add(new KeyValuePair<string, int>("Yo", 40000));
            valueList.Add(new KeyValuePair<string, int>("JO", 4000));
            //Setting data for column chart
            chart1.DataContext = valueList;
        } 
    }
}

