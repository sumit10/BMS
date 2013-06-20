using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using BMS.Model;
using WindowsInput;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace BMS.Model
{
    class autocomplete:basemodel
    {
        TextBox t;
        ListBox l;
        DataTable dt;
        List<ledgerbalance> ol;
        Canvas can;
        String coloumname, coloumname2,coloumname3;
        public event EventHandler nameseleceted;
        public void setlist(List<ledgerbalance> l)
        {
            this.ol = l;
        }
        //public autocomplete(TextBox t, ListBox l, String tablename, String coloumname)
        //{ 
        //  this.t = t;
        //  this.l = l;

        //  this.dt = getdatatable("select "+coloumname+" from "+tablename+"", tablename);
        //  this.coloumname = coloumname;
        //}
        public autocomplete(TextBox t, ListBox l, DataTable dr, String coloumname, String coloumname2, Canvas can, String coloumname3)
        {
            this.t = t;
            this.l = l;

            this.dt = dr;//getdatatable("select " + coloumname + " , " + coloumname2 + " from " + tablename + "", tablename);
            this.coloumname = coloumname;
            this.coloumname2 = coloumname2;
            this.coloumname3 = coloumname3;
            this.can = can;
        }
        public autocomplete(TextBox t, ListBox l, DataTable dr, String coloumname, String coloumname2, Canvas can)
        {
            this.t = t;
            this.l = l;
            this.can = can;
            this.dt = dr;//getdatatable("select " + coloumname + " , " + coloumname2 + " from " + tablename + " where " + where, tablename);
            this.coloumname = coloumname;
            this.coloumname2 = coloumname2;
        }
        public autocomplete(TextBox t, ListBox l,List<ledgerbalance> ol, Canvas can)
        {
            this.t = t;
            this.l = l;
            this.can = can;
            this.ol = ol;
            //this.dt = dr;//getdatatable("select " + coloumname + " , " + coloumname2 + " from " + tablename + " where " + where, tablename);
        }
        //public autocomplete(TextBox t, ListBox l, String tablename, String coloumname,String where)
        //{
        //    this.t = t;
        //    this.l = l;

        //    this.dt = getdatatable("select " + coloumname + " from " + tablename + " where "+where, tablename);
        //    this.coloumname = coloumname;
        //}
        public autocomplete() { }
        //public autocomplete(TextBox t, ListBox l, String tablename, String coloumname,String coloumname2, String where , Canvas can)
        //{
        //    this.t = t;
        //    this.l = l;
        //    this.can = can;
        //    this.dt= getdatatable("select " +coloumname+" , "+coloumname2+ " from " + tablename + " where " + where, tablename);
        //    this.coloumname = coloumname;
        //    this.coloumname2 = coloumname2;
        //}
        //textbox event for key up
        public void tkup(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double currtop = (double)t.GetValue(Canvas.TopProperty);
            l.SetValue(Canvas.TopProperty, currtop);
            double currleft = (double)t.GetValue(Canvas.LeftProperty) + t.ActualWidth;
            l.SetValue(Canvas.LeftProperty, currleft);
            String str = t.Text;

            if (str != " ")
            {
                l.Visibility = Visibility.Visible;
                l.Items.Clear();
                int j = 0, b = 0;
                for (int i = 0; i < this.dt.Rows.Count; i++)
                {
                    String a = this.dt.Rows[i][coloumname].ToString();
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        l.Items.Add(this.dt.Rows[i][coloumname]);
                        j++; b = i;
                        t.Background = Brushes.White;
                    }
                }
                if (j == 0)
                {
                    t.Background = Brushes.Red;
                }
                if (j == 1)
                {
                    if (str == this.dt.Rows[b][coloumname].ToString())
                    {
                        if (this.nameseleceted != null)
                        {
                            this.nameseleceted(this, EventArgs.Empty);
                        }
                        l.Visibility = Visibility.Hidden;
                        
                    }
                }
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    Keyboard.Focus(l);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
                }
            }
        }
        //textbox for name with id
        public void tkupid(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double currtop = (double)t.GetValue(Canvas.TopProperty);
            l.SetValue(Canvas.TopProperty, currtop);
            double currleft = (double)t.GetValue(Canvas.LeftProperty) + t.ActualWidth;
            l.SetValue(Canvas.LeftProperty, currleft);
            String str = t.Text;
            
            if (str != " ")
            {
                l.Visibility = Visibility.Visible;
                l.Items.Clear();
                int j = 0, b = 0;
                for (int i = 0; i < this.dt.Rows.Count; i++)
                {
                    String a = this.dt.Rows[i][coloumname].ToString();
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        l.Items.Add(new nameid() { name = this.dt.Rows[i][coloumname].ToString(), id = this.dt.Rows[i][coloumname2].ToString() });
                        j++; b = i;
                        t.Background = Brushes.White;
                    }
                }
                if (j == 0)
                {
                    t.Background = Brushes.Red;
                }
                if (j == 1)
                {
                    if (str == this.dt.Rows[b][coloumname].ToString())
                    {
                        l.Visibility = Visibility.Hidden;
                        if (this.nameseleceted != null)
                        {
                            this.nameseleceted(this, EventArgs.Empty);
                        }
                    }
                }
                if (j > 1)
                {
                    for (int x = 0; x < l.Items.Count; x++)
                    {
                        if (str == l.Items[x].ToString())
                        {
                            // MessageBox.Show(str);
                            var z = l.Items[x];
                            l.Items.Clear();
                            l.Items.Add(z);
                            l.Visibility = Visibility.Hidden;
                            if (this.nameseleceted != null)
                            {
                                this.nameseleceted(this, EventArgs.Empty);
                            }
                        }
                    }   
                }
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    Keyboard.Focus(l);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
                }
            }
        }
        // txtbox up event for comodity
        public void tkupcom(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double currtop = (double)t.GetValue(Canvas.TopProperty);
            l.SetValue(Canvas.TopProperty, currtop);
            double currleft = (double)t.GetValue(Canvas.LeftProperty) + t.ActualWidth;
            l.SetValue(Canvas.LeftProperty, currleft);
            String str = t.Text;

            if (str != " ")
            {
                l.Visibility = Visibility.Visible;
                l.Items.Clear();
                int j = 0, b = 0;
                for (int i = 0; i < this.dt.Rows.Count; i++)
                {
                    String a = this.dt.Rows[i][coloumname].ToString();
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        l.Items.Add(new stock() { name = this.dt.Rows[i][coloumname].ToString(), id = this.dt.Rows[i]["com_id"].ToString(), quantity = this.dt.Rows[i]["com_balstock"].ToString(), rate = this.dt.Rows[i][coloumname3].ToString() });
                        j++; b = i;
                        t.Background = Brushes.White;
                    }
                }
                if (j == 0)
                {
                    t.Background = Brushes.Red;
                }
                if (j == 1)
                {
                    if (str == this.dt.Rows[b][coloumname].ToString())
                    {
                        l.Visibility = Visibility.Hidden;
                        if (this.nameseleceted != null)
                        {
                            this.nameseleceted(this, EventArgs.Empty);
                        }
                    }
                }
                if (j > 1)
                {
                    for (int x = 0; x < l.Items.Count; x++)
                    {
                        if (str == l.Items[x].ToString())
                        {
                            // MessageBox.Show(str);
                            var z = l.Items[x];
                            l.Items.Clear();
                            l.Items.Add(z);
                            l.Visibility = Visibility.Hidden;
                            if (this.nameseleceted != null)
                            {
                                this.nameseleceted(this, EventArgs.Empty);
                            }
                        }
                    }
                }
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    Keyboard.Focus(l);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
                }
            }
        }
        //txt
        public void tkuplid(object sender, System.Windows.Input.KeyEventArgs e)
        {
            double currtop = (double)t.GetValue(Canvas.TopProperty);
            l.SetValue(Canvas.TopProperty, currtop);
            double currleft = (double)t.GetValue(Canvas.LeftProperty) + t.ActualWidth;
            l.SetValue(Canvas.LeftProperty, currleft);
            String str = t.Text;

            if (str != " ")
            {
                l.Visibility = Visibility.Visible;
                l.Items.Clear();
                int j = 0, b = 0;
                for (int i = 0; i < ol.Count; i++)
                {
                    String a = this.ol[i].Lname.ToString();
                    if (a.ToLower().IndexOf(str.ToLower()) == 0)
                    {
                        l.Items.Add(new ledgerbalance() { Lname = this.ol[i].Lname.ToString(), Lid = this.ol[i].Lid, 
                              Balance = this.ol[i].Balance,Lgroup = this.ol[i].Lgroup });
                        j++; b = i;
                        t.Background = Brushes.White;
                    }
                }
                if (j == 0)
                {
                    t.Background = Brushes.Red;
                }
                if (j == 1)
                {
                    if (str == this.ol[b].Lname.ToString())
                    {
                        l.Visibility = Visibility.Hidden;
                        if (this.nameseleceted != null)
                        {
                            this.nameseleceted(this, EventArgs.Empty);
                        }
                    }
                }
                if (j > 1)
                {
                    for (int x = 0; x < l.Items.Count; x++)
                    {
                        if (str == l.Items[x].ToString())
                        {
                            // MessageBox.Show(str);
                            var z = l.Items[x];
                            l.Items.Clear();
                            l.Items.Add(z);
                            l.Visibility = Visibility.Hidden;
                            if (this.nameseleceted != null)
                            {
                                this.nameseleceted(this, EventArgs.Empty);
                            }
                        }
                    }
                }
                if (e.Key == Key.Down || e.Key == Key.Up)
                {
                    Keyboard.Focus(l);
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.DOWN);
                }
            }
        }

        //mouse down event for listbox 
        public void lmd(object sender, MouseButtonEventArgs e)
        {
            if (l.SelectedItem != null)
            {
                t.Text = l.SelectedItem.ToString();
                Keyboard.Focus(t);
                t.SelectionStart = t.Text.Length;
                l.Visibility = Visibility.Hidden;
               InputSimulator.SimulateKeyUp(VirtualKeyCode.RSHIFT);  
            }    
        }
        //key down event for listbox 
        public void lkd(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Enter)
            {
                Keyboard.Focus(t);
            }
            if (e.Key == Key.Enter )
            {
                t.Text = l.SelectedItem.ToString();
                Keyboard.Focus(t);
                t.SelectionStart = t.Text.Length;
               // InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                l.Visibility = Visibility.Hidden;
            }
        }

        //lost focus event of textboxs to hide list box       
        public void tlf(object sender, RoutedEventArgs e)
        {
          //  if (Keyboard.FocusedElement != l)
            //{
            //    l.Visibility = Visibility.Hidden;
            //}
            try
            {
                if (FocusManager.GetFocusedElement(can) != null)
                {
                    var a = FocusManager.GetFocusedElement(can);
                    if (a.ToString().IndexOf("ListBox") != -1 && a.ToString().IndexOf("BMS") == -1)
                    {
                        l.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        l.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception)
            {
                
               
            }
           
        }
        public static void fillcombo(ComboBox c, string coloum, string table)
        {
            OleDbConnection con;
            con = new OleDbConnection();
            con.ConnectionString = Constring;
            OleDbCommand cmd = new OleDbCommand("SELECT " + coloum + " FROM [" + table + "]", con);
            try
            {
                con.Open();
                OleDbDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    c.Items.Add(read.GetValue(0).ToString());
                }
            }
            finally
            {
                con.Close();
            }
        }
    }
    public class nameid
    {
        public string name{get;set;}
        public string id { get; set; }
        public string value { get; set; }
        public override string ToString()
        {
            return name;
        }
    }

    public class stock
    {
        public string name { get; set; }
        public string id { get; set; }
        public string quantity { get; set; }
        public double balstock { get; set; }
        public string rate { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}
