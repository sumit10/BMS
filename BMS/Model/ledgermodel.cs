using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Collections;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace BMS.Model
{
    class ledgermodel:basemodel
    {
        public int lid,lgroup;
        Hashtable e;
        bool validate;
        public String lname,laddress,lphone,lemail;
        
        public void checkvalid()
        {   
            e = new Hashtable();
            e.Clear();
            e.Add(validation(lname, new String[] { "Required" }, "lname"), "text");
            //e.Add(validation(laddress, new String[] { "Required" }, "ladd"), "text");
           // e.Add(validation(lphone, new String[] { "Required" }, "lno"), "text");
            //e.Add(validation(lemail, new String[] { "Required" }, "lmail"), "text");
            validate = checkvalidation(e);
            //return e;
        }
       
        //public int lidm = 
        public ledgermodel(string lname,int lgroup,string laddress,string lphone,String lemail)
        {
            this.lname = lname;
            this.lgroup = lgroup;
            this.laddress = laddress;
            this.lphone = lphone;
            this.lemail = lemail;
            this.lid = lastid() + 1;
        }
        public ledgermodel(string lname, int lgroup, string laddress, string lphone, String lemail, int lid)
        {
            this.lname = lname;
            this.lgroup = lgroup;
            this.laddress = laddress;
            this.lphone = lphone;
            this.lemail = lemail;
            this.lid = lid;
        }
        public ledgermodel()
        { }
        public int getlid()
        {
            return lid;
        }
        public ledger getprintdetail(int lid)
        {
            OleDbCommand cmd = new OleDbCommand("Select l_address from ledger where l_id = @lid", con);
            cmd.Parameters.AddWithValue("lid", lid);
            ledger l = new ledger();
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    l.Laddress = dr.GetValue(0).ToString();
                }
                return l;
            }
            catch (Exception)
            {

                return l;
            }
            finally
            { con.Close(); }
        }
        public  bool insert(UserControl u)
        {
            checkvalid();
            if (validate)
            {
                OleDbCommand command = new OleDbCommand("INSERT into ledger(l_id,l_name,l_groupid,l_address,l_phone,l_email) VALUES(@lid,@l_name,@l_gid,@l_add,@l_no,@l_email)", con);
                command.Parameters.AddWithValue("lid", lid);                
                command.Parameters.AddWithValue("l_name", lname);
                command.Parameters.AddWithValue("l_gid", lgroup);
                command.Parameters.AddWithValue("l_add", laddress);
                command.Parameters.AddWithValue("l_no", lphone);
                command.Parameters.AddWithValue("l_email", lemail);
                try
                {
                    con.Open();
                    int a = command.ExecuteNonQuery();
                    if (a == -1)
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception a)
                {
                    if (a.ToString().IndexOf("duplicate") != -1)
                    {
                        System.Windows.MessageBox.Show("Duplicate value found in Database");
                        e["lname!"] = "duplicate";
                        showerror(e,u);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(a.ToString());
                    }
                    return false;
                }
                finally
                {
                    con.Close();
                   
                }
                
            }
            else
            {
                showerror(e,u);
                return false;
            }
        }
        
       
        public OleDbCommand tinsert(OleDbConnection con,OleDbTransaction ot)
        {
            OleDbCommand command = new OleDbCommand("INSERT into ledger(l_id,l_name,l_groupid,l_address,l_phone,l_email) VALUES(@lid,@l_name,@l_gid,@l_add,@l_no,@l_email)", con,ot);
            command.Parameters.AddWithValue("lid", lid);
            command.Parameters.AddWithValue("l_name", lname);
            command.Parameters.AddWithValue("l_gid", lgroup);
            command.Parameters.AddWithValue("l_add", laddress);
            command.Parameters.AddWithValue("l_no", lphone);
            command.Parameters.AddWithValue("l_email", lemail);
            return command;
        }
        public bool update(UserControl u)
        {
            checkvalid();
            if (validate)
            {
                OleDbCommand command = new OleDbCommand("Update ledger set l_name=@l_name,l_address=@l_add,l_phone=@l_no,l_email=@l_email where l_id = @l_id", con);
                command.Parameters.AddWithValue("l_name", lname);
                command.Parameters.AddWithValue("l_add", laddress);
                command.Parameters.AddWithValue("l_no", lphone);
                command.Parameters.AddWithValue("l_email", lemail);
                command.Parameters.AddWithValue("l_id", lid);

                try
                {
                    con.Open();
                    int a = command.ExecuteNonQuery();
                    if (a == -1)
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception a)
                {
                    if (a.ToString().IndexOf("duplicate") != -1)
                    {
                        System.Windows.MessageBox.Show("Duplicate value found in Database");
                        e["lname!"] = "duplicate";
                        showerror(e, u);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(a.ToString());
                    }
                    return false;
                }
                finally
                {
                    con.Close();

                }
            }
            else {
                showerror(e, u);
                return false;
            }
        }
        
        public OleDbDataReader getgroup(int groupid)
        {
            OleDbCommand cmd = new OleDbCommand("Select l_id,l_name,l_address,l_phone,l_email from ledger where l_groupid = "+groupid, con);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                return dr;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                throw;
            }     
        }
        public int lastid()
        {
            OleDbCommand cmd = new OleDbCommand("Select max(l_id) from ledger", con);
            try
            {
                con.Open();
                int dr = (int)cmd.ExecuteScalar();
                return dr;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                throw;
            }
            finally
            {
                con.Close();
            }
        }
        public ObservableCollection<ledgerbalance> gettrialbalance()
        {
            ObservableCollection<ledgerbalance> tb = new ObservableCollection<ledgerbalance>();
            String q = "select l.l_id,l.l_name,l.l_groupid,iif(dr.dramt is null ,0 , dr.dramt) as damt , ";
            q += "  iif(cr.cramt is null , 0 ,cr.cramt) as cramt , ";
            q += "( iif(dr.dramt is null ,0 , dr.dramt) - iif(cr.cramt is null , 0 ,cr.cramt))as balance , l.l_phone ,l.l_email";
            q += " from ((SELECT  t_dr, SUM(t_dramt) AS dramt FROM  [transaction] GROUP BY t_dr ORDER BY t_dr ) dr ";
            q += " right outer  join ledger l on dr.t_dr = l.l_id )";
            q += " left outer  join  (SELECT  t_cr, SUM(t_cramt) AS cramt FROM  [transaction] ";
            q += "  GROUP BY t_cr ORDER BY t_cr ) cr on l.l_id = cr.t_cr";
            OleDbCommand cmd = new OleDbCommand(q, con);
            try
            {
                con.Open();
                OleDbDataReader dr =  cmd.ExecuteReader();
                while (dr.Read())
                {
                    int gid;
                    String phone, email;
                    string baltype = "";
                    if (double.Parse(dr.GetValue(5).ToString()) < 0)
                    {
                        baltype = "dr";
                    }
                    else if (double.Parse(dr.GetValue(5).ToString()) == 0)
                    {
                        baltype = "";
                    }
                    else
                    {
                        baltype = "cr";
                    }
                    gid = (dr.GetValue(2)) == DBNull.Value ? 0 : int.Parse(dr.GetValue(2).ToString());
                    phone = (dr.GetValue(6)) == DBNull.Value ? "" : dr.GetValue(6).ToString();
                    email = (dr.GetValue(7)) == DBNull.Value ? "" : dr.GetValue(7).ToString();
                    ledgerbalance lb = new ledgerbalance()
                    {
                        Lid = int.Parse(dr.GetValue(0).ToString()),
                        Lname = dr.GetValue(1).ToString(),
                        Lgroup = gid,
                        Debittotal  = double.Parse(dr.GetValue(3).ToString()),
                        Credittotal = double.Parse(dr.GetValue(4).ToString()),
                        Balance = Math.Abs(Double.Parse(dr.GetValue(5).ToString())),
                        CurrBalance = Math.Abs(Double.Parse(dr.GetValue(5).ToString())).ToString("C2"),
                        BalanceType= baltype,
                        Lphoneno = phone,
                        Lemail = email
                    };
                    tb.Add(lb);
                }
                return tb;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return tb;
            }
            finally
            {
                con.Close();
            }
        }
        public List<ladgerreport> getledgerreport(int lid)
        {

            List<ladgerreport> lr = new List<ladgerreport>();
            String q = "SELECT t.t_id, t.t_dramt, t.t_dr,  t.t_cr, t.t_date, t.t_naration, l.l_name AS drname, ";
            q += "ll.l_name AS crname FROM (([transaction] t INNER JOIN  ledger l ON t.t_dr = l.l_id) INNER JOIN";
            q += "  ledger ll ON t.t_cr = ll.l_id)";
            q += "WHERE (t.t_dr = @id) OR (t.t_cr = @lid)";
            OleDbCommand cmd = new OleDbCommand(q, con);
            cmd.Parameters.AddWithValue("id",lid);
            cmd.Parameters.AddWithValue("lid", lid);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string naration;
                    naration = (dr.GetValue(5)) == DBNull.Value ? " " : dr.GetValue(5).ToString();
                    ladgerreport lb = new ladgerreport()
                    {
                      tid = int.Parse(dr.GetValue(0).ToString()),
                      amt = int.Parse(dr.GetValue(1).ToString()),
                      drlid = int.Parse(dr.GetValue(2).ToString()),
                      crlid = int.Parse(dr.GetValue(3).ToString()),
                      tdate = DateTime.Parse(dr.GetValue(4).ToString()).ToShortDateString(),
                      naration = naration,
                      drlname = dr.GetValue(6).ToString(),
                      crlname = dr.GetValue(7).ToString()
                    };
                    lr.Add(lb);
                }
                return lr;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return lr;
            }
            finally
            {
                con.Close();
            }
        }
        
    }

    class ledger
    {
        public int Lid { get; set; }
        public String Lname { get; set; }
        public int Lgroup { get; set; }
        public String Laddress { get; set; }
        public String Lphoneno { get; set; }
        public String Lemail { get; set; }
        public String Limage { get; set; }
        public string Balance { get; set; }
        public string BalanceType { get; set; }
        public override string ToString()
        {
            return Lname;
        }
    }

    class ladgerreport
    {
        public int tid { get; set; }
        public string tdate { get; set; }
        public int drlid { get; set; }
        public String drlname { get; set; }
        public int crlid { get; set; }
        public String crlname { get; set; }
        public double amt { get; set; }
        public String naration { get; set; }
    }

    class ledgerbalance
    {
        public int Lid { get; set; }
        public String Lname { get; set; }
        public int Lgroup { get; set; }
        public double Debittotal { get; set; }
        public double Credittotal { get; set; }
        public double Balance { get; set; }
        public string CurrBalance { get; set; }
        public String Lphoneno { get; set; }
        public String Lemail { get; set; }
        public string BalanceType { get; set; }
        public override string ToString()
        {
            return Lname;
        }
    }
}
