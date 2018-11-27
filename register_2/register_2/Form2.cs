using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace register_2
{
    public partial class Form2 : Form 
    {


        public Form2()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form3"] is Form3 form3)
            {
                form3.Focus();
                return;
            }
            form3 = new Form3();

            form3.FormSendEvent += new Form3.FormSendDataHandler(IdUpdateEventMethod);
            
            form3.ShowDialog();

        }
        private void IdUpdateEventMethod(object sender)
        {
            ListViewItem lvt = new ListViewItem(sender.ToString());
            accountList.Items.Add(lvt);
        }

               

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void Button3_Click(object sender, EventArgs e)
        {
            string a = accountList.SelectedItems[0].Text;
            /*
            int i = 0;

            StringBuilder getstr = new StringBuilder();
            while (Form1.GetPrivateProfileString("LOGIN", "ID" + i, null, getstr, 1000, Form1.path) != 0)
            {
                i++;
            }
            int selectRow = accountList.SelectedItems[0].Index;

            Form1.WritePrivateProfileString("LOGIN", "ID" + selectRow, null, Form1.path);
            Form1.WritePrivateProfileString("LOGIN", "PWD" + selectRow, null, Form1.path);
            

            while(selectRow < i)
            {
                selectRow++;
                Form1.GetPrivateProfileString("LOGIN", "ID" + selectRow, null, getstr, 1000, Form1.path);
                Form1.WritePrivateProfileString("LOGIN", "ID" + (selectRow-1), getstr.ToString(), Form1.path);
                Form1.GetPrivateProfileString("LOGIN", "PWD" + selectRow, null, getstr, 1000, Form1.path);
                Form1.WritePrivateProfileString("LOGIN", "PWD" + (selectRow-1), getstr.ToString(), Form1.path);                
            }

            Form1.WritePrivateProfileString("LOGIN", "ID" + (i-1), null, Form1.path);
            Form1.WritePrivateProfileString("LOGIN", "PWD" + (i-1), null, Form1.path);

            ListViewItem lvi = accountList.SelectedItems[0];
            accountList.Items.Remove(lvi);*/

            try
            {
                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                

                string strsql2 = "DELETE FROM account WHERE ID='"+ a +"'";
                SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                cmd.ExecuteNonQuery();
                sqliteConn.Close();

                ListViewItem lvi2 = accountList.SelectedItems[0];
                accountList.Items.Remove(lvi2);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            /*
            accountList.Items.Clear();
            int i = 0;
            StringBuilder getstr = new StringBuilder();
            while (Form1.GetPrivateProfileString("LOGIN", "ID" + i, null, getstr, 1000, Form1.path) != 0)
            {
                string accountID = getstr.ToString();
                Form1.GetPrivateProfileString("LOGIN", "PWD" + i, null, getstr, 1000, Form1.path);
                string accountPWD = getstr.ToString();
                string[] strs = new string[] { accountID, accountPWD };
                ListViewItem lvi = new ListViewItem(strs);
                accountList.Items.Add(lvi);
                i++;
            }*/
            
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strqry = "SELECT * FROM account";
                SQLiteCommand cmd = new SQLiteCommand(strqry, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListViewItem lvt = new ListViewItem(reader["ID"].ToString());
                        accountList.Items.Add(lvt);
                    }
                }

                reader.Close();
                sqliteConn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
