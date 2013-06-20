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

namespace BMS.report
{
    /// <summary>
    /// Interaction logic for print_invoice.xaml
    /// </summary>
    public partial class print_invoice : UserControl
    {
        public print_invoice()
        {
            InitializeComponent();
            this.RemoveLogicalChild(layout);
        }
    }
}
