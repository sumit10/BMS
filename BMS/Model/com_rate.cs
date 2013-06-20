using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Web;


namespace BMS.Model
{
    class com_rate
    {
        String goldurl = "http://www.moneycontrol.com/commodity/gold-price.html";
        String silverurl = "http://www.moneycontrol.com/commodity/silver-price.html";
        String copperurl = "http://www.moneycontrol.com/commodity/copper-price.html";
        String aluminiumurl = "http://www.moneycontrol.com/commodity/aluminium-price.html";
        String nickelurl = "http://www.moneycontrol.com/commodity/nickel-price.html";
        String zincurl = "http://www.moneycontrol.com/commodity/zinc-price.html";

        private string GetSource(string url)
        {
            GetSource objGetSource = new GetSource();
            String a = "";
            try
            {
                a = objGetSource.GetHTML(url);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            return a;
        }
        public string findrate(string product)
        {
            String b = "";
            if (product == "gold")
            {
                b = goldurl;
            }
            else if (product == "silver")
            {
                b = silverurl;
            }
            else if(product == "alluminium")
            {
                b = aluminiumurl;
            }
            else if (product == "copper")
            {
                b = copperurl;
            }
            else if (product == "nickel")
            {
                b = nickelurl;
            }
            else if (product == "zinc")
            {
                b = zincurl; 
            }

            String a = GetSource(b.ToString());
            
            if (a != "" && a != null )
            {
                int a1 = a.IndexOf("Rs");
               return a.Substring(a1 + 3, 6);
            }
            return null;
        }
    }
    public class GetSource
    {
        public string GetHTML(string Url)
        {
            String result = "";
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

            }
            catch (Exception)
            {
                //System.Windows.MessageBox.Show(ex.Message.ToString());
                return null;
            }
            return result;
        }
    }
}