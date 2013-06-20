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
using System.Configuration;
using System.Xml;

namespace test
{
    /// <summary>
    /// Interaction logic for appconfig.xaml
    /// </summary>
    public partial class appconfig : Window
    {
        public appconfig()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["name"]))
            {
                this.Title = ConfigurationSettings.AppSettings["name"];
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            UpdateKey("name", textBox1.Text);
            textBox1.Text = string.Empty;
        }
        public void UpdateKey(string strKey, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\test.exe.config");
            if (!KeyExists(strKey))
            {
                throw new ArgumentNullException("Key", "<" + strKey + "> does not exist in the configuration. Update failed.");
            }
            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    childNode.Attributes["value"].Value = newValue;
            }
            xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "App.config");
            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }
        public bool KeyExists(string strKey)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\test.exe.config");
            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    return true;
            }
            return false;
        }
    }
}
