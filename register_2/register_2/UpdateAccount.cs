using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace register_2
{
    public partial class UpdateAccount : Form
    {
        public delegate void FormSendDataHandler(string sendstring);
        public event FormSendDataHandler FormSendEvent;
        public UpdateAccount()
        {
            InitializeComponent();
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strsql = "SELECT ID FROM account";

                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["ID"].ToString());
                    }
                }

                reader.Close();
                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.FormSendEvent(comboBox1.Text);
            this.Close();
        }        
    }
}
