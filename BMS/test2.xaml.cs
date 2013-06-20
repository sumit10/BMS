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
using BMS.Model;

namespace BMS
{
    /// <summary>
    /// Interaction logic for test2.xaml
    /// </summary>
    public partial class test2 : Window
    {
        public test2()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<stock> sc = new commoditymodel().getstock();
            List<chartdata> cd = new List<chartdata>();
            foreach (stock s in sc)
            {
                int a = int.Parse(Math.Round(s.balstock, 0).ToString());
                chartdata c = new chartdata() { name = s.name, value = a };
                cd.Add(c);
            }
            barseries.ItemsSource = cd;
        }
    }
    class chartdata
    {
        public string name { get; set; }
        public int value { get; set; }
    }
}
