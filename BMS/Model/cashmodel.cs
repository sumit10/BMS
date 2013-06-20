using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;


namespace BMS.Model
{
    class cashmodel:basemodel
    {
        public int tid { get; set; }
        public cashmodel() { }
        public bool createaccount(string name, double ob, string type, string date)
        {
                ledgermodel l;
                int a = -1;
                transactionmodel t;
                bool ldr;
                if (type == "Cash")
                {
                    a = 1;
                    ldr = true;
                }
                else if(type == "Bank A/c")
                {
                    a = 14;
                    ldr = true;
                }
                else if (type == "expance")
                {
                    a = 12;
                    ldr = false;
                }
                else
                {
                    a = 15;
                    ldr = true;
                }
                l = new ledgermodel(name, a, "", "", "");
                int lid = l.getlid();
                if (ldr)
                {
                    t = new transactionmodel(lid, 76, ob, ob, date);
                }
                else
                {
                    t = new transactionmodel(76, lid, ob, ob, date);
                }
                OleDbTransaction olt;
          try
            {
                con.Open();
                olt = con.BeginTransaction();
                l.tinsert(con, olt).ExecuteNonQuery();
                t.tinsert(olt, con).ExecuteNonQuery();
                olt.Commit();
                MessageBox.Show("Account Created Succesfull");
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error " + e.ToString());
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public bool incertcash(int cid,int lid,double amt,string naration,bool cdr,string date)
        {
            transactionmodel t;
            if (cdr)
            {
                t = new transactionmodel(cid,lid,amt,amt,date,naration);
            }
            else
            {
                t = new transactionmodel(lid, cid, amt, amt, date, naration);
            }

            if (t.insert())
            {
                tid = t.gettid();
                return true;
            }
            return false;
        }
        public List<cash> getcashdata(string date)
        {
            List<cash> cd = new List<cash>();
            query = " select t.t_id, t.t_dr, t.t_cr,t.t_naration, l.l_name AS dr_name, ll.l_name AS cr_name, ";
            query += "l.l_groupid AS dr_gid, ll.l_groupid AS cr_gid ,t.t_dramt ";
            query += " FROM (([transaction] t INNER JOIN  ledger l ON t.t_dr = l.l_id) ";
            query +=  "INNER JOIN  ledger ll ON t.t_cr = ll.l_id) ";
            query += " where t.t_date = @date and  (l.l_groupid = 1 OR l.l_groupid = 14 OR ll.l_groupid = 1 OR ll.l_groupid = 14) ";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("date", date);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    cash s = new cash()
                    {
                        tid = int.Parse(dr.GetValue(0).ToString()),
                        drid = int.Parse(dr.GetValue(1).ToString()),
                        crid = int.Parse(dr.GetValue(2).ToString()),
                        naration = dr.GetValue(3).ToString(),
                        drname = dr.GetValue(4).ToString(),
                        crname = dr.GetValue(5).ToString(),
                        drgid = int.Parse(dr.GetValue(6).ToString()),
                        crgid = int.Parse(dr.GetValue(7).ToString()),
                        amt = double.Parse(dr.GetValue(8).ToString()),
                    };
                    cd.Add(s);
                }
                return cd;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return cd;
            }
            finally
            {
                con.Close();
            }
        }
    }
    class cash
    {
        public int tid { get; set; }
        public int drid { get; set; }
        public int crid { get; set; }
        public string drname { get; set; }
        public string crname { get; set; }
        public int drgid { get; set; }
        public int crgid { get; set; }
        public double amt { get; set; }
        public string naration { get; set; }
    }
}
