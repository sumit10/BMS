using System;
using System.Collections.Generic;
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
	/// Interaction logic for report_cr_dr.xaml
	/// </summary>
	public partial class report_cr_dr : UserControl
	{
		public report_cr_dr()
		{
			this.InitializeComponent();
            this.RemoveLogicalChild(layout);
		}
	}
}