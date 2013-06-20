using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace BMS.Model
{
    class transactionmodel:basemodel
    {
        int tid, dr, cr;
        Double dramt, cramt;
        string tdate,naration;
        public transactionmodel(int dr, int cr, Double dramt, Double cramt, string tdate)
        {
            this.dr = dr;
            this.cr = cr;
            this.dramt = dramt;
            this.cramt = cramt;
            this.tdate = tdate;
            this.tid = getid();
            this.naration = "";
        }
        public transactionmodel(int tid, int dr, int cr, Double dramt, Double cramt, string tdate)
        {
            this.dr = dr;
            this.cr = cr;
            this.dramt = dramt;
            this.cramt = cramt;
            this.tdate = tdate;
            this.tid = tid;
            this.naration = "";
        }
        public transactionmodel( int dr, int cr, Double dramt, Double cramt, string tdate,string naration)
        {
            this.dr = dr;
            this.cr = cr;
            this.dramt = dramt;
            this.cramt = cramt;
            this.tdate = tdate;
            this.tid = getid();
            this.naration = naration;
        }
        public int gettid()
        {
            return tid;
        }
        public transactionmodel()
        { }
        public int getid()
        {
            int z=0;
            OleDbCommand cmd = new OleDbCommand("Select max(t_id) from [transaction]",con);
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
            finally
            {
                con.Close();
            }
            return z;  
        }
        public override bool insert()
        {
             OleDbCommand command = new OleDbCommand("INSERT into [transaction](t_id,t_dr,t_dramt,t_cr,t_cramt,t_date,t_naration) VALUES(@id,@dr,@dra,@cr,@cra,@dt,@n)", con);
                command.Parameters.AddWithValue("id",tid);
                command.Parameters.AddWithValue("dr", dr);
                command.Parameters.AddWithValue("dra", dramt);
                command.Parameters.AddWithValue("cr", cr);
                command.Parameters.AddWithValue("cra", cramt);
                command.Parameters.AddWithValue("dt", tdate);
                command.Parameters.AddWithValue("n", naration);
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
                    System.Windows.MessageBox.Show(a.ToString());
                    return false;
                }
                finally
                {
                   
                        con.Close();
                }  
       }
        public OleDbCommand tinsert(OleDbTransaction ot,OleDbConnection con)
        {
            OleDbCommand command = new OleDbCommand("INSERT into [transaction](t_id,t_dr,t_dramt,t_cr,t_cramt,t_date,t_naration) VALUES(@id,@dr,@dra,@cr,@cra,@dt,@n)", con, ot);
            command.Parameters.AddWithValue("id", tid);
            command.Parameters.AddWithValue("dr", dr);
            command.Parameters.AddWithValue("dra", dramt);
            command.Parameters.AddWithValue("cr", cr);
            command.Parameters.AddWithValue("cra", cramt);
            command.Parameters.AddWithValue("dt", tdate);
            command.Parameters.AddWithValue("n", naration);
            return command;
        } 
      }
}
