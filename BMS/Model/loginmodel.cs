using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace BMS.Model
{
    class loginmodel: basemodel
    {
        string cname, cpassword,caddress,cnumber;
        bool validate;
        public loginmodel(String cname,String cpassword,String caddress,String cnumber)
        {
            this.cname = cname;
            this.cpassword = cpassword;
            this.caddress = caddress;
            this.cnumber = cnumber;
            validate = false;
        }
        public loginmodel() { }

        public Hashtable checkvalid()
        {
            Hashtable e = new Hashtable();
            e.Add(validation(cname, new String[]{ "Required" }, "name"),"text");
            e.Add(validation(cpassword, new String[] { "Required", "Min:4","Max:9" }, "password"), "password");
            e.Add(validation(caddress, new String[] { "Required" }, "address"), "text");
            e.Add(validation(cnumber, new String[] { "Required", "isnumber" }, "phone"), "text");
            validate = checkvalidation(e);
            return e;
            
        }

        public override bool insert()
        {  
            if (validate)
            {
                OleDbCommand command = new OleDbCommand("INSERT into login(com_name,[password],com_address,com_phone) VALUES(@c_name,@c_pass,@c_add,@c_no)", con);
                command.Parameters.AddWithValue("c_name", cname);
                command.Parameters.AddWithValue("c_pass", cpassword);
                command.Parameters.AddWithValue("c_add", caddress);
                command.Parameters.AddWithValue("c_no", cnumber);
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
            else
            {
                return false;
            }
        }


        public bool login(String cname, String cpassword)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM login where com_name = @user and [password] = @pass", con);
            cmd.Parameters.AddWithValue("@user",cname);
            cmd.Parameters.AddWithValue("@pass", cpassword);
            try
            {
                con.Open();
                OleDbDataReader read = cmd.ExecuteReader();
                if (read.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                con.Close();
            }
        }

        public bool update(int id,string oldpass,string newpass)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM login where ID = @user and [password] = @pass", con);
            cmd.Parameters.AddWithValue("@user", id);
            cmd.Parameters.AddWithValue("@pass", oldpass);
            OleDbCommand cmd1 = new OleDbCommand("Update  login set [password]=@newpass where [ID]=@id",con);
            cmd1.Parameters.AddWithValue("@pass", newpass);
            cmd1.Parameters.AddWithValue("@user", id);
            try
            {
                con.Open();
                OleDbDataReader read = cmd.ExecuteReader();
                if (read.HasRows)
                {
                    int a = cmd1.ExecuteNonQuery();
                    if (a == -1)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                con.Close();
            }
        }
    }
}
