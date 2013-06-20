using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Controls;
using System.Data.OleDb;

namespace BMS.Model
{
    class Fillcombo:basemodel
    {
        DataTable dt;
        string coloum;
        public Fillcombo(string coloum, string table)
        {
            OleDbConnection con;
            con = new OleDbConnection();
            con.ConnectionString = Constring;
            OleDbDataAdapter da  = new OleDbDataAdapter("SELECT " + coloum + " FROM [" + table + "]", con);
            DataSet ds = new DataSet();
            da.Fill(ds, table);
            this.dt =ds.Tables[table];
            this.coloum = coloum;
        }

        public void fillcombo(ComboBox c)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                c.Items.Add(this.dt.Rows[i][coloum].ToString());
            }
        }

    }
}
