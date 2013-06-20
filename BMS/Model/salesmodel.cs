using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace BMS.Model
{
    class salesitemmodel:basemodel
    {
        salesitem s;
        public salesitemmodel(salesitem s)
        {
            this.s = s;
        }
        public salesitemmodel()
        { }
        public OleDbCommand insert(OleDbTransaction ot , OleDbConnection con)
        {
            query += " INSERT into [sale_detail](s_id,com_id,com_quantity,com_uom,com_rate,s_total)";
            query += " VALUES(@sid,@comid,@coqty,@cuom,@crate,@stotal);";
            OleDbCommand command = new OleDbCommand(query, con,ot);
            command.Parameters.AddWithValue("sid", s.sid);
            command.Parameters.AddWithValue("comid", s.scomid);
            command.Parameters.AddWithValue("coqty", s.sqnty);
            command.Parameters.AddWithValue("cuom", s.suom);
            command.Parameters.AddWithValue("crate", s.srate);
            command.Parameters.AddWithValue("stotal", s.samt);
            return command;
        }
        public OleDbCommand orderinsert(OleDbTransaction ot, OleDbConnection con)
        {
            query += " INSERT into [saleorder_detail](so_id,com_id,com_quantity,com_uom,com_rate,s_total)";
            query += " VALUES(@sid,@comid,@coqty,@cuom,@crate,@stotal);";
            OleDbCommand command = new OleDbCommand(query, con, ot);
            command.Parameters.AddWithValue("sid", s.sid);
            command.Parameters.AddWithValue("comid", s.scomid);
            command.Parameters.AddWithValue("coqty", s.sqnty);
            command.Parameters.AddWithValue("cuom", s.suom);
            command.Parameters.AddWithValue("crate", s.srate);
            command.Parameters.AddWithValue("stotal", s.samt);
            return command;
        }
        public ObservableCollection<salesitem> getinvoicedetail(int sid)
        {
            ObservableCollection<salesitem> sd = new  ObservableCollection<salesitem>();
            query = "select   s.s_id, s.com_id, s.com_quantity, s.com_uom, s.s_total, s.com_rate, c.com_name";
            query += " from  (sale_detail s INNER JOIN stock c ON s.com_id = c.com_id)";
            query += " where s.s_id = @sid";
            OleDbCommand cmd = new OleDbCommand(query,con);
            cmd.Parameters.AddWithValue("sid",sid);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    salesitem s = new salesitem() 
                    {
                        sid = int.Parse(dr.GetValue(0).ToString()),
                        scomid = int.Parse(dr.GetValue(1).ToString()),
                        sqnty = double.Parse(dr.GetValue(2).ToString()),
                        suom = dr.GetValue(3).ToString(),
                        samt = double.Parse(dr.GetValue(4).ToString()),
                        srate = dr.GetValue(5).ToString(),
                        scomname = dr.GetValue(6).ToString()
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
        public List<salesitem> getorderedetail(int sid)
        {
            List<salesitem> sd = new List<salesitem>();
            query = "select   s.so_id, s.com_id, s.com_quantity, s.com_uom, s.s_total, s.com_rate, c.com_name";
            query += " from  (saleorder_detail s INNER JOIN stock c ON s.com_id = c.com_id)";
            query += " where s.so_id = @sid";
            OleDbCommand cmd = new OleDbCommand(query, con);
            cmd.Parameters.AddWithValue("sid", sid);
            try
            {
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    salesitem s = new salesitem()
                    {
                        sid = int.Parse(dr.GetValue(0).ToString()),
                        scomid = int.Parse(dr.GetValue(1).ToString()),
                        sqnty = double.Parse(dr.GetValue(2).ToString()),
                        suom = dr.GetValue(3).ToString(),
                        samt = double.Parse(dr.GetValue(4).ToString()),
                        srate = dr.GetValue(5).ToString(),
                        scomname = dr.GetValue(6).ToString()
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

    public class salesitem
    {
        public int sid { get; set; } 
         public int scomid { get; set; }
         public String scomname { get; set;}
         public Double samt { get; set; }
         public String suom { get; set; }
         public Double sqnty { get; set; }
         public String srate { get; set; }
    }
    public class salesreport
    {
        public String sdate { get; set; }
        public String vid { get; set; }
        public String customername { get; set; }
        public String vtotal { get; set; }
    }

    public class sales
    {
        public int sid { get; set; }
        public int tid { get; set; }
        public String sdate { get; set; }
        public int lid { get; set; }
        public String tnsp_chg { get; set; }
        public String vat_chg { get; set; }
        public String discount { get; set; }
        public String roundof { get; set; }
        public Double stotal { get; set; }
        public Double nettotal { get; set; }
        public string lname { get; set; }
    }

    class salesmodel : basemodel
    {
      List<salesitem> si;
      sales sm;
		Double total;
    //  int tid, lid,sdate;
      public salesmodel(sales sm,List<salesitem> si,Double total)
      {
          this.si = si;
          this.total = total;
          this.sm = sm;
      }
      public salesmodel() {     }
      public int getvid()
      {
          int z;
          OleDbCommand cmd = new OleDbCommand("Select max(s_id) from [sale]", con);
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
          OleDbCommand cmd = new OleDbCommand("Select max(so_id) from [sale_order]", con);
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
      public ObservableCollection<salesreport> getsalesreport()
      { 
         ObservableCollection<salesreport> sr = new ObservableCollection<salesreport>();
         String q = "SELECT  s.s_date, s.s_id,s.s_nettotal, l.l_name  FROM ";
               q += "  sale as s ";
               q +=  " INNER JOIN ledger as l ON s.l_id = l.l_id";
        OleDbCommand cmd = new OleDbCommand(q, con);
        try
        {
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime d = DateTime.Parse(dr.GetValue(0).ToString());
                salesreport s = new salesreport()
                {
                    sdate = d.Date.ToLongDateString(),
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

     public ObservableCollection<salesreport> getsalesorderreport()
      { 
         ObservableCollection<salesreport> sr = new ObservableCollection<salesreport>();
         String q = "SELECT  s.s_date, s.so_id,s.s_nettotal, l.l_name  FROM ";
               q += "  sale_order as s ";
               q +=  " INNER JOIN ledger as l ON s.l_id = l.l_id";
        OleDbCommand cmd = new OleDbCommand(q, con);
        try
        {
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime d = DateTime.Parse(dr.GetValue(0).ToString());
                salesreport s = new salesreport()
                {
                    sdate = d.Date.ToLongDateString(),
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
      public sales getinvoicedetail(int sid)
      {
         query = "select TOP 1  s.s_id , s.t_id, s.s_date, s.l_id, s.tnsp_chg, s.vat_chg, s.discount, s.round_of, s.s_total, s.s_nettotal , l.l_name";
         query += " from sale s inner join ledger l on s.l_id = l.l_id  where s.s_id = @sid ";
         OleDbCommand cmd = new OleDbCommand(query,con);
          sales s = new sales() ;
         cmd.Parameters.AddWithValue("sid",sid);
         try
         {
             con.Open();
             OleDbDataReader dr = cmd.ExecuteReader();
             dr.Read();
             
             s.sid = int.Parse(dr.GetValue(0).ToString());
             s.tid = int.Parse(dr.GetValue(1).ToString());
             s.sdate = dr.GetValue(2).ToString();
             s.lid = int.Parse(dr.GetValue(3).ToString());
             s.tnsp_chg = dr.GetValue(4).ToString();
             s.vat_chg = dr.GetValue(5).ToString();
             s.discount = dr.GetValue(6).ToString();
             s.roundof = dr.GetValue(7).ToString();
             s.stotal = double.Parse(dr.GetValue(8).ToString());
             s.nettotal = double.Parse(dr.GetValue(9).ToString());
             s.lname = dr.GetValue(10).ToString();
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
      public sales getorderdetail(int sid)
      {
          query = "select TOP 1  s.so_id , s.t_id, s.s_date, s.l_id, s.s_total, s.s_nettotal , l.l_name";
          query += " from sale_order s inner join ledger l on s.l_id = l.l_id  where s.so_id = @sid ";
          OleDbCommand cmd = new OleDbCommand(query, con);
          sales s = new sales();
          cmd.Parameters.AddWithValue("sid", sid);
          try
          {
              con.Open();
              OleDbDataReader dr = cmd.ExecuteReader();
              dr.Read();

              s.sid = int.Parse(dr.GetValue(0).ToString());
              s.tid = int.Parse(dr.GetValue(1).ToString());
              s.sdate = dr.GetValue(2).ToString();
              s.lid = int.Parse(dr.GetValue(3).ToString());
              s.stotal = double.Parse(dr.GetValue(4).ToString());
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
          query += " INSERT into [sale](s_id,t_id,s_date,l_id,tnsp_chg,vat_chg,discount,round_of,s_total,s_nettotal)";
          query += " VALUES(@sid,@tid,@sdate,@lid,@tnsp_chg,@vat,@discount,@roundof,@stotal,@snettotal);";
          ot = con.BeginTransaction();
          OleDbCommand command = new OleDbCommand(query, con, ot);
          command.Parameters.AddWithValue("sid", sm.sid);
          command.Parameters.AddWithValue("tid", sm.tid);
          command.Parameters.AddWithValue("sdate", sm.sdate);
          command.Parameters.AddWithValue("lid", sm.lid);
          command.Parameters.AddWithValue("tnsp_chg", sm.tnsp_chg);
          command.Parameters.AddWithValue("vat", sm.vat_chg);
          command.Parameters.AddWithValue("discount", sm.discount);
          command.Parameters.AddWithValue("roundof", sm.roundof);
          command.Parameters.AddWithValue("stotal", sm.stotal);
          command.Parameters.AddWithValue("snettotal", sm.nettotal);
          try
          {
             // int j = 1;
              command.ExecuteNonQuery();
              transactionmodel tm = new transactionmodel(sm.lid, 3, sm.nettotal, sm.nettotal, sm.sdate,"Being good sold  vid = "+sm.sid.ToString());
              tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (salesitem i in si)
              {
                   salesitemmodel sim = new salesitemmodel(i);
                   sim.insert(ot, con).ExecuteNonQuery();
                   commoditymodel com = new commoditymodel(i.scomid);
                    com.setbalquanty_sale((Double)i.sqnty, ot, con).ExecuteNonQuery();
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
          public  bool orderinsert()
      {
          OleDbTransaction ot;
          con.Open();
          query += " INSERT into [sale_order](so_id,t_id,s_date,l_id,s_total,s_nettotal)";
          query += " VALUES(@sid,@tid,@sdate,@lid,@stotal,@snettotal);";
          ot = con.BeginTransaction();
          OleDbCommand command = new OleDbCommand(query, con, ot);
          command.Parameters.AddWithValue("sid", sm.sid);
          command.Parameters.AddWithValue("tid", sm.tid);
          command.Parameters.AddWithValue("sdate", sm.sdate);
          command.Parameters.AddWithValue("lid", sm.lid);
          command.Parameters.AddWithValue("stotal", sm.stotal);
          command.Parameters.AddWithValue("snettotal", sm.nettotal);
          try
          {
              // int j = 1;
              command.ExecuteNonQuery();
             // transactionmodel tm = new transactionmodel(sm.lid, 3, sm.nettotal, sm.nettotal, sm.sdate);
             // tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (salesitem i in si)
              {
                  salesitemmodel sim = new salesitemmodel(i);
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
              String s = "DELETE FROM sale_order WHERE so_id=@soid";
              String s1 = "DELETE FROM saleorder_detail WHERE so_id=@soid";
              query += " INSERT into [sale](s_id,t_id,s_date,l_id,tnsp_chg,vat_chg,discount,round_of,s_total,s_nettotal)";
              query += " VALUES(@sid,@tid,@sdate,@lid,@tnsp_chg,@vat,@discount,@roundof,@stotal,@snettotal);";
              ot = con.BeginTransaction();
              OleDbCommand command = new OleDbCommand(query, con, ot);
              OleDbCommand command1 = new OleDbCommand(s, con, ot);
              OleDbCommand command2 = new OleDbCommand(s1, con, ot);
              command.Parameters.AddWithValue("sid", sm.sid);
              command.Parameters.AddWithValue("tid", sm.tid);
              command.Parameters.AddWithValue("sdate", sm.sdate);
              command.Parameters.AddWithValue("lid", sm.lid);
              command.Parameters.AddWithValue("tnsp_chg", sm.tnsp_chg);
              command.Parameters.AddWithValue("vat", sm.vat_chg);
              command.Parameters.AddWithValue("discount", sm.discount);
              command.Parameters.AddWithValue("roundof", sm.roundof);
              command.Parameters.AddWithValue("stotal", sm.stotal);
              command.Parameters.AddWithValue("snettotal", sm.nettotal);
              command1.Parameters.AddWithValue("soid", oid);
              command2.Parameters.AddWithValue("soid", oid);
              try
              {
                  // int j = 1;
                  command.ExecuteNonQuery();
                  command1.ExecuteNonQuery();
                  command2.ExecuteNonQuery();
                  transactionmodel tm = new transactionmodel(sm.lid, 3, sm.nettotal, sm.nettotal, sm.sdate, "Being good sold  vid = " + sm.sid.ToString() +" by sale order ="+oid);
                  tm.tinsert(ot, con).ExecuteNonQuery();
                  foreach (salesitem i in si)
                  {
                      salesitemmodel sim = new salesitemmodel(i);
                      sim.insert(ot, con).ExecuteNonQuery();
                      commoditymodel com = new commoditymodel(i.scomid);
                      com.setbalquanty_sale((Double)i.sqnty, ot, con).ExecuteNonQuery();
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

