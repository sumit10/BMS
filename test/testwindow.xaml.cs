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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Configuration;
using test.Properties;

namespace test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class testwindow : Window
    {
        ObservableCollection<Superhero> s = new ObservableCollection<Superhero>();
        int i;
        public testwindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //System.Configuration.Configuration config =  ConfigurationManager.OpenExeConfiguration  (ConfigurationUserLevel.None);
          //  config.AppSettings.Settings.Add("ModificationDate", DateTime.Now.ToLongTimeString() + " ");
         //   config.Save(ConfigurationSaveMode.Modified);
            //string appPath = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location);
            //string configFile = System.IO.Path.Combine(appPath, "App.config");
            //ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            //configFileMap.ExeConfigFilename = configFile;
            //System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            //config.AppSettings.Settings["name"].Value = "hahahaha";
            //config.Save();
            //ConfigurationManager.RefreshSection("appSettings");
            //ShowConfig();
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "\\masterdb.mdb";
            con.Open();
            OleDbCommand cmd = new OleDbCommand("SELECT s_name,s_sid FROM superhero", con);
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    s.Add(new Superhero() { Name = dr.GetValue(0).ToString(), Secretid = dr.GetValue(1).ToString() });
                }
            }
            lstbox.ItemsSource = s;
        }
  
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            test2 t2 = new test2();
            frame1.Navigate(t2);
            
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox l = (ListBox)sender;
            Superhero s = (Superhero)l.SelectedValue;
            System.Windows.MessageBox.Show(s.Name + " " + s.Secretid);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            s.Add(new Superhero() { Name = textBox1.Text, Secretid = textBox2.Text });
            textBox2.Text = "";
            textBox1.Text = "";
        }

        private void button1_LostFocus(object sender, RoutedEventArgs e)
        {
            var a = FocusManager.GetFocusedElement(sd);
            MessageBox.Show(a.ToString());
        }
    }
}
