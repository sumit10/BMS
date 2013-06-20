using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;

namespace BMS.Model
{
    class salesitemmodel:basemodel
    {
        int tid,scomid,lid,vid;
        String sdate,suom,srate;
        Double samt, vtotal,sqnty;
        public salesitemmodel(int tid,Double samt,int scomid,String srate,int lid,string suom,Double sqnty,int vid,Double vtotal,string sdate)
        {
            this.tid = tid;
            this.samt = samt;
           this.scomid = scomid;
           this.srate=srate;
           this.lid = lid;
           this.suom= suom;
          this.sqnty= sqnty;
          this.vid = vid;
          this.vtotal = vtotal;
          this.sdate = sdate;
        }

        public OleDbCommand insert(OleDbTransaction ot , OleDbConnection con)
        {
            query += " INSERT into [sale](t_id,s_date,s_amt,s_comid,s_rate,l_id,s_uom,s_quantity,v_id,v_total)";
            query += " VALUES(@tid,@sdate,@samt,@scomid,@srate,@lid,@suom,@sqnty,@vid,@vtotal);";
            OleDbCommand command = new OleDbCommand(query, con,ot);
            command.Parameters.AddWithValue("tid", tid);
            command.Parameters.AddWithValue("sdate", sdate);
            command.Parameters.AddWithValue("samt", samt);
            command.Parameters.AddWithValue("scomid", scomid);
            command.Parameters.AddWithValue("srate", srate);
            command.Parameters.AddWithValue("lid", lid);
            command.Parameters.AddWithValue("suom", suom);
            command.Parameters.AddWithValue("sqnty", sqnty);
            command.Parameters.AddWithValue("vid", vid);
            command.Parameters.AddWithValue("vtotal", vtotal);
            return command;
        }
       
    }

    public class salesitem
    {
         public int tid { get; set; }
         public int scomid { get; set; }
         public String scomname { get; set;}
         public Double samt { get; set; }
         public int lid { get; set; }
         public String suom { get; set; }
         public Double sqnty { get; set; }
         public String srate { get; set; }
         public int vid { get; set; }
         public Double vtotal { get; set; }
         public String sdate { get; set; }
         public String ex_type { get; set; }
    }

    class salesmodel : basemodel
    {
      List<salesitem> si;
		Double total;
    //  int tid, lid,sdate;
      public salesmodel(List<salesitem> si,Double total)
      {
          this.si = si;
          this.total = total;
      }
      public salesmodel() {     }
      public int getvid()
      {
          int z;
          OleDbCommand cmd = new OleDbCommand("Select max(v_id) from [sale]", con);
          try
          {
              con.Open();
              object a = cmd.ExecuteScalar();
              if (a.ToString() == "")
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
      public override bool insert()
      {
          OleDbTransaction ot;
          con.Open();
          ot = con.BeginTransaction();
          try
          {
              int j = 1;
              transactionmodel tm = new transactionmodel(si[0].lid, 3, total, total, si[0].sdate);
              tm.tinsert(ot, con).ExecuteNonQuery();
              foreach (salesitem i in si)
              {
                  if (i.scomid != 0)
                  {
                      salesitemmodel sim = new salesitemmodel(i.tid, (Double)i.samt, i.scomid, i.srate, i.lid, i.suom, (Double)i.sqnty, i.vid, (Double)i.vtotal, i.sdate);
                      sim.insert(ot, con).ExecuteNonQuery();
                      commoditymodel com = new commoditymodel(i.scomid);
                      com.setbalquanty_sale((Double)i.sqnty, ot, con).ExecuteNonQuery();
                  }
                  else if (i.ex_type == "+")
                  {
                      i.tid = i.tid + j;
                     salesitemmodel sim = new salesitemmodel(i.tid,i.samt, 0, i.srate, i.lid, "", 0, i.vid, i.vtotal, i.sdate);
                     sim.insert(ot, con).ExecuteNonQuery();
                     transactionmodel tm1 = new transactionmodel(i.tid,si[0].lid,i.lid , (int)i.samt, (int)i.samt, i.sdate);
                     tm1.tinsert(ot, con).ExecuteNonQuery();
                      j++;
                  }
                  //else
                  //  {
                  //    i.tid = i.tid + i;
                  //   salesitemmodel sim = new salesitemmodel(i.tid,(int)i.samt, i.scomid, i.srate, i.lid, i.suom, int.Parse(i.sqnty), i.vid, i.vtotal, i.sdate);
                  //   sim.insert(ot, con).ExecuteNonQuery();
                  //   transactionmodel tm = new transactionmodel(i.lid, 3, i.samt, i.samt, i.sdate);
                  //   tm.tinsert(ot, con).ExecuteNonQuery();
                  //    i++;
                  //  }
              }
              
              ot.Commit();
              MessageBox.Show("Save ho gaya ");
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
