using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Controls;

namespace BMS.Model
{
    class commoditymodel:basemodel
    {
        String comname,uom,tdate, compp, comsp, comops;
        int cid;
        bool validate;
        Hashtable e;
         public commoditymodel()
         {}
        public commoditymodel(String comname, String uom, String compp, String comsp, String comops, String tdate) 
        {
            this.comname = comname;
            this.uom = uom;
            this.compp = compp;
            this.comsp = comsp;
            this.comops = comops;
            this.tdate = tdate;
            validate = false;
        }
        public commoditymodel(String comname, String uom, String compp, String comsp, String comops, String tdate,int cid)
        {
            this.comname = comname;
            this.uom = uom;
            this.compp = compp;
            this.comsp = comsp;
            this.comops = comops;
            this.tdate = tdate;
            this.cid = cid;
            validate = false;
        }
        public commoditymodel(int cid)
        {
            this.cid = cid;
        }
        public void checkvalid()
        {
            e = new Hashtable();
            e.Add(validation(comname, new String[] { "Required" }, "coname"), "text");
            e.Add(validation(uom, new String[] { "Required" }, "uom"), "combobox");
            e.Add(validation(compp, new String[] { "Required" ,"isnumber"}, "copp"), "text");
            e.Add(validation(comsp, new String[] { "Required", "isnumber" }, "cosp"), "text");
            e.Add(validation(comops, new String[] { "Required", "isnumber" }, "coops"), "text");
            e.Add(validation(tdate, new String[] { "Required" }, "date"), "date");
            validate = checkvalidation(e);
        }

        public bool insert(UserControl u)
        {
            checkvalid();
           if (validate)
            {
                OleDbTransaction olt;
                con.Open();
                olt = con.BeginTransaction();
                query = " INSERT into [stock](com_name,com_uom,com_pp,com_sp,com_opstock,com_balstock) ";
                query += " VALUES(@comname,@comuom,@compp,@comsp,@comopstock,@combalstock);";
                OleDbCommand command = new OleDbCommand(query, con, olt);
                command.Parameters.AddWithValue("comname", comname);
                command.Parameters.AddWithValue("comuom", uom);
                command.Parameters.AddWithValue("compp", compp);
                command.Parameters.AddWithValue("comsp", comsp);
                command.Parameters.AddWithValue("comopstock", comops);
                command.Parameters.AddWithValue("combalstock", comops);
                int amt = int.Parse(compp) * int.Parse(comops);
                int x = 9; int y = 5;
                transactionmodel tm = new transactionmodel(x,y,amt,amt,tdate);
                OleDbCommand command1 = tm.tinsert(olt,con);
                try
                {
             
                    command.ExecuteNonQuery();
                    command1.ExecuteNonQuery();
                    olt.Commit();
                    return true;
                }
                catch (Exception a)
                {
                    olt.Rollback();
                    System.Windows.MessageBox.Show(a.ToString());
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
           else
           {
               showerror(e, u);
               return false;
           }
        }
        public OleDbDataReader getcom()
        {
            OleDbCommand cmd = new OleDbCommand("select com_id,com_name,com_uom,com_pp,com_sp,com_opstock,com_balstock from [stock] ", con);
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
        public bool update(UserControl uc)
        {

                OleDbCommand command = new OleDbCommand("Update [stock] set com_name=@comname,com_uom=@com_uom,com_pp=@com_pp,com_sp=@com_sp where com_id = @com_id ", con);
                command.Parameters.AddWithValue("comname", comname);
                command.Parameters.AddWithValue("com_uom", uom);
                command.Parameters.AddWithValue("com_pp", compp);
                command.Parameters.AddWithValue("com_sp", comsp);
                command.Parameters.AddWithValue("com_id", cid);
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
                        e["coname!"] = "duplicate";
                        showerror(e,uc);
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
      
        public Double getquanty()
        {

            OleDbCommand command = new OleDbCommand("Select com_balstock from [stock] where com_id = @cid", con);
            command.Parameters.AddWithValue("cid", cid);
            try
            {
                con.Open();
                object a = command.ExecuteScalar();
                if (a == null)
                {
                    return 0;
                }
                return Double.Parse(a.ToString());
            }
            catch (Exception a)
            {
                System.Windows.MessageBox.Show(a.ToString());
                return -1;
            }
            finally
            {
                con.Close();
            }
        }
        public OleDbCommand setbalquanty_sale(Double qua,OleDbTransaction ot ,OleDbConnection con)
        {
            Double a = getquanty();
            Double combal = a-qua ;
            query = "update [stock] set com_balstock = @combal where com_id = @comid;";
            OleDbCommand cmd = new OleDbCommand(query,con,ot);
            cmd.Parameters.AddWithValue("combal", combal);
            cmd.Parameters.AddWithValue("comid",  cid);
            return cmd;
        }
        public OleDbCommand setbalquanty_purchase(Double qua, OleDbTransaction ot, OleDbConnection con)
        {
            Double a = getquanty();
            Double combal = a + qua;
            query = "update [stock] set com_balstock = @combal where com_id = @comid;";
            OleDbCommand cmd = new OleDbCommand(query, con, ot);
            cmd.Parameters.AddWithValue("combal", combal);
            cmd.Parameters.AddWithValue("comid", cid);
            return cmd;
        }

        public List<stock> getstock() 
        {
            List<stock> s = new List<stock>();
            OleDbCommand command = new OleDbCommand("select com_name, com_balstock from stock", con);
            try
            {
                con.Open();
                OleDbDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    stock ss = new stock()
                    {
                        name = dr.GetValue(0).ToString(),
                        balstock =double.Parse(dr.GetValue(1).ToString())
                    };
                    s.Add(ss);
                }
                return s;
            }
            catch (Exception a)
            {
                System.Windows.MessageBox.Show(a.ToString());
                return s;
            }
            finally
            {
                con.Close();
            }
        }
    }
    class stockdata
    {
        public int Cid { get; set; }
        public String Cname { get; set; }
        public String Cuom { get; set; }
        public String Cpp { get; set; }
        public String Csp { get; set; }
        public String Copstock { get; set; }
        public String Cbalstock { get; set; }

    }
}
