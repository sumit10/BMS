using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace BMS.Model
{
    class basemodel
    {
        public static string Constring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "\\masterdb.mdb;Jet OLEDB:Database Password=9987854959shrameeks3";
        protected OleDbConnection con;
        protected String query;
        public basemodel()
        {
            con = new OleDbConnection();
            con.ConnectionString = Constring;
        }

        virtual public bool insert()
        {
            return true;
        }
        public DataTable getdatatable(String q, String table)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(q,con);
            DataSet ds = new DataSet();
            da.Fill(ds, table);
            DataTable dt = ds.Tables[table];
            return dt;
        }
        //Validation code for all
        public String validation(String value,String[] rules,String name)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                switch (rules[i])
                {
                    case "Required" :
                    case "required" :
                        if (value == "" || value == null || value == " ")
                        {
                            return name;
                        }
                        break;
                    case "isnumber":
                        int n;
                        if (!(Int32.TryParse(value,out n)))
                        {
                            return name;
                        }
                        break;
                    case "duplicate":
                    case "Duplicate":
                        return name+"#" ;
                    default:
                        //return name +"!" ;
                        break;
                }
                if (rules[i].Substring(0, 3) == "max" || rules[i].Substring(0, 3) == "Max")
                {
                    int x = rules[i].IndexOf(':');
                    String z = rules[i].Substring(x + 1);
                    if (value.Length > int.Parse(z))
                    {
                        return name;
                    }
                }
                if (rules[i].Substring(0, 3) == "min" || rules[i].Substring(0, 3) == "Min")
                {
                    int x = rules[i].IndexOf(':');
                    String z = rules[i].Substring(x+1);
                    int y = int.Parse(z);
                    if (value.Length < int.Parse(z))
                    {
                        return name;
                    }
                }
            }
            return name + "!";
        }
        protected bool checkvalidation(Hashtable e)
        {
            foreach (DictionaryEntry error in e)
            {
                String check = error.Key.ToString();
                if (check.Substring(check.Length - 1, 1) != "!")
                {
                    return false; 
                }
            }
            return true;
        }
        public void showerror(Hashtable errors,UserControl a)
        {
            foreach (DictionaryEntry error in errors)
            {
                String check = error.Key.ToString();
                if (error.Value.ToString() == "text")
                {
                    if (check.Substring(check.Length - 1, 1) != "!" )
                    {
                        TextBox e = (TextBox)a.FindName("txt" + check);
                        e.BorderBrush = Brushes.Red; 
                    }
                    else
                    {
                        TextBox e = (TextBox)a.FindName("txt"+check.Substring(0,check.Length-1));
                        e.BorderBrush = Brushes.SkyBlue;
                    }      
                }
                if (error.Value.ToString() == "password")
                {
                    if (check.Substring(check.Length - 1, 1) != "!")
                    {
                        PasswordBox e = (PasswordBox)a.FindName("pb" + error.Key);
                        e.BorderBrush = Brushes.Red;
                    }
                    else
                    {
                        PasswordBox e = (PasswordBox)a.FindName("pb" + check.Substring(0, check.Length - 1));
                        e.ClearValue(TextBox.BorderBrushProperty);
                        e.Password = "";
                    }
                }
                if (error.Value.ToString() == "duplicate")
                {
                    TextBox e = (TextBox)a.FindName("txt" + check.Substring(0, check.Length - 1));
                    e.BorderBrush = Brushes.Red; 
                }
            }
        }
        //end validation
    }
}
