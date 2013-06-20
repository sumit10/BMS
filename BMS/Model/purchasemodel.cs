using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace BMS.Model
{
    class purchaseitemmodel : basemodel
    {
        purchaseitem p;
        public purchaseitemmodel(purchaseitem p)
        {
            this.p = p;
        }
        public purchaseitemmodel()
        { }
        public OleDbCommand insert(OleDbTransaction ot , OleDbConnection con)
        {
            query += " INSERT into [purchase_detail]( p_id, com_id, com_quantity, com_uom, com_rate, p_total)";
            query += " VALUES(@pid,@comid,@coqty,@cuom,@crate,@ptotal);";
            OleDbCommand command = new OleDbCommand(query, con,ot);
            command.Parameters.AddWithValue("sid", p.pid);
            command.Parameters.AddWithValue("comid", p.pcomid);
            command.Parameters.AddWithValue("coqty", p.pqnty);
            command.Parameters.AddWithValue("cuom", p.puom);
            command.Parameters.AddWithValue("crate", p.prate);
            command.Parameters.AddWithValue("stotal", p.pamt);
            return command;
        }
        public OleDbCommand orderinsert(OleDbTransaction ot, OleDbConnection con)
        {
            query += " INSERT into [purchaseorder_detail](po_id,com_id,com_quantity,com_uom,com_rate,p_total)";
            query += " VALUES(@sid,@comid,@coqty,@cuom,@crate,@stotal);";
            OleDbCommand command = new OleDbCommand(query, con, ot);
            command.Parameters.AddWithValue("sid", p.pid);
            command.Parameters.AddWithValue("comid", p.pcomid);
            command.Parameters.AddWithValue("coqty", p.pqnty);
            command.Parameters.AddWithValue("cuom", p.puom);
            command.Parameters.AddWithValue("crate", p.prate);
            command.Parameters.AddWithValue("stotal", p.pamt);
            return command;
        }
        public ObservableCollection<purchaseitem> getinvoicedetail(int pid)
        {
            ObservableCollection<purchaseitem> pd = new  ObservableCollection<purchaseitem>();
            query = "select   p.p_id, p.com_id, p.com_quantity, p.com_uom ,p.p_total, p.com_rate, c.com_name";
            query += " from  (purchase_detail p INNER JOIN stock c ON p.com_id = c.com_id)";
            query += " where p.p_id = @pid";
            OleDbCommand cmd = new OleDbCommand(query,con);
            cmd.Parameters.AddWithValue("pid",pid);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    purchaseitem p = new purchaseitem() 
                    {
                        pid = int.Parse(dr.GetValue(0).ToString()),
                        pcomid = int.Parse(dr.GetValue(1).ToString()),
                        pqnty = double.Parse(dr.GetValue(2).ToString()),
                        puom = dr.GetValue(3).ToString(),
                        pamt = double.Parse(dr.GetValue(4).ToString()),
                        prate = dr.GetValue(5).ToString(),
                        pcomname = dr.GetValue(6).ToString()
                    };
                    pd.Add(p);
                }
                return pd;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return pd;
            }
            finally
            {
                con.Close();
            }
        }

        public List<purchaseitem> getorderedetail(int sid)
        {
            List<purchaseitem> sd = new List<purchaseitem>();
            query = "select   s.po_id, s.com_id, s.com_quantity, s.com_uom, s.p_total, s.com_rate, c.com_name";
            query += " from  (purchaseorder_detail s INNER JOIN stock c ON s.com_id = c.com_id)";
            query += " where s.po_id = @sid";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("sid", sid);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    purchaseitem s = new purchaseitem()
                    {
                        pid = int.Parse(dr.GetValue(0).ToString()),
                        pcomid = int.Parse(dr.GetValue(1).ToString()),
                        pqnty = double.Parse(dr.GetValue(2).ToString()),
                        puom = dr.GetValue(3).ToString(),
                        pamt = double.Parse(dr.GetValue(4).ToString()),
                        prate = dr.GetValue(5).ToString(),
                        pcomname = dr.GetValue(6).ToString()
                    };
                    sd.Add(s);
                }
                return sd;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return sd;
            }
            finally
            {
                con.Close();
            }
        }
       
    }
    public class purchaseitem
    {
        public int pid { get; set; }
        public int pcomid { get; set; }
        public String pcomname { get; set; }
        public Double pamt { get; set; }
        public String puom { get; set; }
        public Double pqnty { get; set; }
        public String prate { get; set; }
    }
    public class purchasereport
    {
        public String pdate { get; set; }
        public String vid { get; set; }
        public String customername { get; set; }
        public String vtotal { get; set; }
        public String invoiceno { get; set; }
    }

    public class purchase
    {
        public int pid { get; set; }
        public int sinvoiceno { get; set; }
        public int tid { get; set; }
        public String pdate { get; set; }
        public int lid { get; set; }
        public String tnsp_chg { get; set; }
        public String vat_chg { get; set; }
        public String discount { get; set; }
        public String roundof { get; set; }
        public Double ptotal { get; set; }
        public Double nettotal { get; set; }
        public string lname { get; set; }
    }

    class purchasemodel : basemodel
    {
      List<purchaseitem> pi;
      purchase pm;
		Double total;
    //  int tid, lid,sdate;
      public purchasemodel(purchase pm,List<purchaseitem> pi,Double total)
      {
          this.pi = pi;
          this.total = total;
          this.pm = pm;
      }
      public purchasemodel() { }
      public int getvid()
      {
          int z;
          OleDbCommand cmd = new OleDbCommand("Select max(p_id) from [purchase]", con);
          try
          {
              con.Open();
              object a = cmd.ExecuteScalar();
              if (a == DBNull.Value)
              {
                  z = 1;
              }
              else
              {
                  z = (int)a + 1;
              }
          }
          catch (Exception)
          {
              return -1;
          }
          return z;
      }
      public int getordervid()
      {
          int z;
          OleDbCommand cmd = new OleDbCommand("Select max(po_id) from [purchase_order]", con);
          try
          {
              con.Open();
              object a = cmd.ExecuteScalar();
              if (a == DBNull.Value)
              {
                  z = 1;
              }
              else
              {
                  z = (int)a + 1;
              }
          }
          catch (Exception)
          {
              return -1;
          }
          return z;
      }
      public ObservableCollection<purchasereport> getpurchasereport()
      {
          ObservableCollection<purchasereport> pr = new ObservableCollection<purchasereport>();
         String q = "SELECT  p.p_date,p.p_id,p.p_nettotal, l.l_name,p.s_invoiceno  FROM ";
               q += "  purchase as p ";
               q +=  " INNER JOIN ledger as l ON p.l_id = l.l_id";
        OleDbCommand cmd = new OleDbCommand(q, con);
        try
        {
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime d = DateTime.Parse(dr.GetValue(0).ToString());
                purchasereport s = new purchasereport()
                {
                    pdate = DateTime.Parse(d.Date.ToShortDateString()).ToLongDateString(),
                    vid = dr.GetValue(1).ToString(),
                    vtotal = dr.GetValue(2).ToString(),
                    customername = dr.GetValue(3).ToString(),
                    invoiceno = dr.GetValue(4).ToString()
                };
                pr.Add(s);
            }
            return pr;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
            return pr;
        }
        finally
        {
            con.Close();
        }
      }
      public ObservableCollection<purchasereport> getpurchaseorderreport()
      {
          ObservableCollection<purchasereport> sr = new ObservableCollection<purchasereport>();
          String q = "SELECT  s.p_date, s.po_id,s.p_nettotal, l.l_name  FROM ";
          q += "  purchase_order as s ";
          q += " INNER JOIN ledger as l ON s.l_id = l.l_id";
          OleDbCommand cmd = new OleDbCommand(q, con);
          try
          {
              con.Open();
              OleDbDataReader dr = cmd.ExecuteReader();
              while (dr.Read())
              {
                  DateTime d = DateTime.Parse(dr.GetValue(0).ToString());
                  purchasereport s = new purchasereport()
                  {
                      pdate = d.Date.ToLongDateString(),
                      vid = dr.GetValue(1).ToString(),
                      vtotal = dr.GetValue(2).ToString(),
                      customername = dr.GetValue(3).ToString()
                  };
                  sr.Add(s);
              }
              return sr;
          }
          catch (Exception e)
          {
              MessageBox.Show(e.ToString());
              return sr;
          }
          finally
          {
              con.Close();
          }
      }
      public purchase getinvoicedetail(int pid)
      {
         query = "select TOP 1  p.p_id , p.t_id, p.p_date, p.l_id, p.tnsp_chg, p.vat_chg, p.discount, p.round_of, p.p_total, p.p_nettotal , l.l_name";
         query += " from purchase p inner join ledger l on p.l_id = l.l_id  where p.p_id = @pid ";
         OleDbCommand cmd = new OleDbCommand(query,con);
          purchase p = new purchase() ;
         cmd.Parameters.AddWithValue("pid",pid);
         try
         {
             con.Open();
             OleDbDataReader dr = cmd.ExecuteReader();
             dr.Read();
             
             p.pid = int.Parse(dr.GetValue(0).ToString());
             p.tid = int.Parse(dr.GetValue(1).ToString());
             p.pdate = dr.GetValue(2).ToString();
             p.lid = int.Parse(dr.GetValue(3).ToString());
             p.tnsp_chg = dr.GetValue(4).ToString();
             p.vat_chg = dr.GetValue(5).ToString();
             p.discount = dr.GetValue(6).ToString();
             p.roundof = dr.GetValue(7).ToString();
             p.ptotal = double.Parse(dr.GetValue(8).ToString());
             p.nettotal = double.Parse(dr.GetValue(9).ToString());
             p.lname = dr.GetValue(10).ToString();
             return p;
         }
         catch (Exception e)
         {
             MessageBox.Show(e.ToString());
             return p;
         }
         finally
         {
             con.Close();
         }
      }
      public purchase getorderdetail(int sid)
      {
          query = "select TOP 1  s.po_id , s.t_id, s.p_date, s.l_id, s.p_total, s.p_nettotal , l.l_name";
          query += " from purchase_order s inner join ledger l on s.l_id = l.l_id  where s.po_id = @sid ";
          OleDbCommand cmd = new OleDbCommand(query, con);
          purchase s = new purchase();
          cmd.Parameters.AddWithValue("sid", sid);
          try
          {
              con.Open();
              OleDbDataReader dr = cmd.ExecuteReader();
              dr.Read();

              s.pid = int.Parse(dr.GetValue(0).ToString());
              s.tid = int.Parse(dr.GetValue(1).ToString());
              s.pdate = dr.GetValue(2).ToString();
              s.lid = int.Parse(dr.GetValue(3).ToString());
              s.ptotal = double.Parse(dr.GetValue(4).ToString());
              s.nettotal = double.Parse(dr.GetValue(5).ToString());
              s.lname = dr.GetValue(6).ToString();
              return s;
          }
          catch (Exception e)
          {
              MessageBox.Show(e.ToString());
              return s;
          }
          finally
          {
              con.Close();
          }
      }
      public override bool insert()
      {
          OleDbTransaction ot;
          con.Open();
          query += " INSERT into [purchase]( p_id,s_invoiceno, t_id, p_date, l_id, tnsp_chg, vat_chg, discount, round_of, p_total, p_nettotal)";
          query += " VALUES(@pid,@si,@tid,@pdate,@lid,@tnsp_chg,@vat,@discount,@roundof,@ptotal,@snettotal);";
          ot = con.BeginTransaction();
          OleDbCommand command = new OleDbCommand(query, con, ot);
          command.Parameters.AddWithValue("pid", pm.pid);
          command.Parameters.AddWithValue("si", pm.sinvoiceno);
          command.Parameters.AddWithValue("tid", pm.tid);
          command.Parameters.AddWithValue("sdate", pm.pdate);
          command.Parameters.AddWithValue("lid", pm.lid);
          command.Parameters.AddWithValue("tnsp_chg", pm.tnsp_chg);
          command.Parameters.AddWithValue("vat", pm.vat_chg);
          command.Parameters.AddWithValue("discount", pm.discount);
          command.Parameters.AddWithValue("roundof", pm.roundof);
          command.Parameters.AddWithValue("stotal", pm.ptotal);
          command.Parameters.AddWithValue("snettotal", pm.nettotal);
          try
          {
             // int j = 1;
              command.ExecuteNonQuery();
              transactionmodel tm = new transactionmodel(2,pm.lid, pm.nettotal, pm.nettotal, pm.pdate,"Being good purchase of v_id : "+pm.pid.ToString());
              tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (purchaseitem i in pi)
              {
                   purchaseitemmodel pim = new purchaseitemmodel(i);
                   pim.insert(ot, con).ExecuteNonQuery();
                   commoditymodel com = new commoditymodel(i.pcomid);
                   com.setbalquanty_purchase((Double)i.pqnty, ot, con).ExecuteNonQuery();
              }
              //if (sm.vat_chg != "" || sm.vat_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + 1, sm.lid, 89, Double.Parse(sm.vat_chg), Double.Parse(sm.vat_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              //if (sm.tnsp_chg != "" || sm.tnsp_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + j, sm.lid, 85, Double.Parse(sm.tnsp_chg), Double.Parse(sm.tnsp_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              ot.Commit();
              return true;
          }
          catch (Exception e)
          {
              MessageBox.Show(e.ToString());
              return false;
          }
          finally
          {
              con.Close();
          }
      }
      public bool orderinsert()
      {
          OleDbTransaction ot;
          con.Open();
          query += " INSERT into [purchase_order](po_id,t_id,p_date,l_id,p_total,p_nettotal)";
          query += " VALUES(@sid,@tid,@sdate,@lid,@stotal,@snettotal);";
          ot = con.BeginTransaction();
          OleDbCommand command = new OleDbCommand(query, con, ot);
          command.Parameters.AddWithValue("sid", pm.pid);
          command.Parameters.AddWithValue("tid", pm.tid);
          command.Parameters.AddWithValue("sdate", pm.pdate);
          command.Parameters.AddWithValue("lid", pm.lid);
          command.Parameters.AddWithValue("stotal", pm.ptotal);
          command.Parameters.AddWithValue("snettotal", pm.nettotal);
          try
          {
              // int j = 1;
              command.ExecuteNonQuery();
              // transactionmodel tm = new transactionmodel(sm.lid, 3, sm.nettotal, sm.nettotal, sm.sdate);
              // tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (purchaseitem i in pi)
              {
                  purchaseitemmodel sim = new purchaseitemmodel(i);
                  sim.orderinsert(ot, con).ExecuteNonQuery();
                  //  commoditymodel com = new commoditymodel(i.scomid);
                  //  com.setbalquanty_sale((Double)i.sqnty, ot, con).ExecuteNonQuery();
              }
              //if (sm.vat_chg != "" || sm.vat_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + 1, sm.lid, 89, Double.Parse(sm.vat_chg), Double.Parse(sm.vat_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              //if (sm.tnsp_chg != "" || sm.tnsp_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + j, sm.lid, 85, Double.Parse(sm.tnsp_chg), Double.Parse(sm.tnsp_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              ot.Commit();
              return true;
          }
          catch (Exception e)
          {
              MessageBox.Show(e.ToString());
              return false;
          }
          finally
          {
              con.Close();
          }
      }
      
      public bool comitorder(int oid)
      {
          OleDbTransaction ot;
          con.Open();
          String s = "DELETE FROM purchase_order WHERE po_id=@soid";
          String s1 = "DELETE FROM purchaseorder_detail WHERE po_id=@soid";
          query += " INSERT into [purchase](p_id,t_id,p_date,l_id,tnsp_chg,vat_chg,discount,round_of,p_total,p_nettotal)";
          query += " VALUES(@sid,@tid,@sdate,@lid,@tnsp_chg,@vat,@discount,@roundof,@stotal,@snettotal);";
          ot = con.BeginTransaction();
          OleDbCommand command = new OleDbCommand(query, con, ot);
          OleDbCommand command1 = new OleDbCommand(s, con, ot);
          OleDbCommand command2 = new OleDbCommand(s1, con, ot);
          command.Parameters.AddWithValue("sid", pm.pid);
          command.Parameters.AddWithValue("tid", pm.tid);
          command.Parameters.AddWithValue("sdate", pm.pdate);
          command.Parameters.AddWithValue("lid", pm.lid);
          command.Parameters.AddWithValue("tnsp_chg", pm.tnsp_chg);
          command.Parameters.AddWithValue("vat", pm.vat_chg);
          command.Parameters.AddWithValue("discount", pm.discount);
          command.Parameters.AddWithValue("roundof", pm.roundof);
          command.Parameters.AddWithValue("stotal", pm.ptotal);
          command.Parameters.AddWithValue("snettotal", pm.nettotal);
          command1.Parameters.AddWithValue("soid", oid);
          command2.Parameters.AddWithValue("soid", oid);
          try
          {
              // int j = 1;
              command.ExecuteNonQuery();
              command1.ExecuteNonQuery();
              command2.ExecuteNonQuery();
              transactionmodel tm = new transactionmodel(2, pm.lid, pm.nettotal, pm.nettotal, pm.pdate, "Being good purchase of v_id : " + pm.pid.ToString() +" of purchase order "+ oid.ToString());
              tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (purchaseitem i in pi)
              {
                  purchaseitemmodel pim = new purchaseitemmodel(i);
                  pim.insert(ot, con).ExecuteNonQuery();
                  commoditymodel com = new commoditymodel(i.pcomid);
                  com.setbalquanty_purchase((Double)i.pqnty, ot, con).ExecuteNonQuery();
              }
              //if (sm.vat_chg != "" || sm.vat_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + 1, sm.lid, 89, Double.Parse(sm.vat_chg), Double.Parse(sm.vat_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              //if (sm.tnsp_chg != "" || sm.tnsp_chg != null)
              //{
              //    transactionmodel tm1 = new transactionmodel(sm.tid + j, sm.lid, 85, Double.Parse(sm.tnsp_chg), Double.Parse(sm.tnsp_chg), sm.sdate);
              //    tm1.tinsert(ot, con).ExecuteNonQuery();
              //    j++;
              //}
              ot.Commit();
              return true;
          }
          catch (Exception e)
          {
              MessageBox.Show(e.ToString());
              return false;
          }
          finally
          {
              con.Close();
          }
      }
    }
}
