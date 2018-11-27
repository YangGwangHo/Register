using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinHttp;
using System.Net;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace register_2
{
    public partial class Form3 : Form
    {
        
        public static CookieContainer cookie = new CookieContainer();
        public delegate void FormSendDataHandler(string sendstring);
        public event FormSendDataHandler FormSendEvent;




        public Form3()
        {
            InitializeComponent();
            button1.Enabled = false;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            
            string id = txtID.Text;
            string pwd = txtPWD.Text;
            string sendData = "user_id=" + id + "&user_password=" + pwd;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.CookieContainer = cookie;
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.Write(sendData);
            writer.Close();

            HttpWebResponse result = (HttpWebResponse)req.GetResponse();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                Encoding encode = Encoding.GetEncoding("utf-8");
                Stream strReceiveStream = result.GetResponseStream();
                StreamReader reqStreamReader = new StreamReader(strReceiveStream, encode);
                String strResult = reqStreamReader.ReadToEnd();
                strReceiveStream.Close();
                reqStreamReader.Close();

                req = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom");
                req.Method = "GET";
                req.CookieContainer = cookie;
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream stReadData1 = response.GetResponseStream();
                StreamReader srReadData1 = new StreamReader(stReadData1, encode);
                string strResult1 = srReadData1.ReadToEnd();

                if (strResult1.Contains("로그아웃"))
                {
                    MessageBox.Show("로그인 인증 완료");
                    button1.Enabled = true;
                }
                else
                {
                    MessageBox.Show("로그인 인증 실패");
                    button1.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("로그인 인증 실패");
                button1.Enabled = false;
            }
            
            req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
            req.Method = "GET";
            req.CookieContainer = cookie;
            HttpWebResponse response2 = (HttpWebResponse)req.GetResponse();
            Stream stReadData2 = response2.GetResponseStream();
            req.Abort();

        }

        private void Button1_Click(object sender, EventArgs e)
        {/*
            int i = 0;
            StringBuilder getstr = new StringBuilder();
            while(Form1.GetPrivateProfileString("LOGIN", "ID" + i, null, getstr, 1000, Form1.path) != 0)
            {
                i++;
            }

            Form1.WritePrivateProfileString("LOGIN", "ID"+i, txtID.Text, Form1.path);
            Form1.WritePrivateProfileString("LOGIN", "PWD"+i, txtPWD.Text, Form1.path);
            */
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();

                string strsql2 = "INSERT INTO account (ID,PWD) values ('" + txtID.Text + "','" + txtPWD.Text + "')";
                SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                cmd.ExecuteNonQuery();
                sqliteConn.Close();

                this.FormSendEvent(txtID.Text);
            }
            catch
            {
                MessageBox.Show("이미 등록된 아이디입니다.");
            }

            this.Close();
        }
    }
}

