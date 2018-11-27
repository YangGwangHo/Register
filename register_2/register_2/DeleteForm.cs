using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace register_2
{
    public partial class DeleteForm : Form
    {
        public DeleteForm()
        {
            InitializeComponent();

        }
        Boolean[] removeCount;

        Thread delete;
        private void DeleteForm_Load(object sender, EventArgs e)
        {
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strsql = "SELECT * FROM account";

                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = "";
                        lvi.SubItems.Add(reader["ID"].ToString());
                        lvi.SubItems.Add("");
                        listView1.Items.Add(lvi);
                    }
                }

                reader.Close();
                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            removeCount = new Boolean[listView1.Items.Count];
            for(int i = 0;i < listView1.Items.Count;i++)
            {
                removeCount[i] = false;
            }
        }
        private void ListView1_DrawColumnHeader_1(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                bool value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception) { }
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.Left + 4, e.Bounds.Top + 4), value ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void ListView1_DrawItem_1(object sender, DrawListViewItemEventArgs e)
        {

            e.DrawDefault = true;
        }

        private void ListView1_DrawSubItem_1(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListView1_Click(object sender, EventArgs e)
        {
            Point mousePos = listView1.PointToClient(Control.MousePosition);

            ListViewHitTestInfo hitTest = listView1.HitTest(mousePos);
            if (hitTest.Item.SubItems.IndexOf(hitTest.SubItem) == 0)
            {
                if (listView1.Items[hitTest.Item.Index].Checked == false)
                {
                    removeCount[hitTest.Item.Index] = true;
                }
                else
                {
                    removeCount[hitTest.Item.Index] = false;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            delete = new Thread(Delete);
            delete.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();            
        }
        void Delete()
        {
            while(true)
            {
                if(Form1.regist.IsAlive == false)
                {
                    break;
                }
            }
            
            HttpWebRequest logoutReq = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
            logoutReq.Method = "GET";
            logoutReq.CookieContainer = Form1.cookie;
            HttpWebResponse logoutResp = (HttpWebResponse)logoutReq.GetResponse();
            logoutReq.Abort();
            logoutResp.Close();

            DataTable accountTable = new DataTable();
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strqry = "SELECT * FROM account";
                SQLiteCommand cmd = new SQLiteCommand(strqry, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                accountTable.Load(reader);

                reader.Close();
                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            int q = accountTable.Rows.Count;

            string[] IDlist = new string[q];
            string[] PWDlist = new string[q];
            string[][] registID = new string[q][];
            int[] count = new int[q];
            for (int i = 0; i < q; i++)
            {
                registID[i] = new string[400];
                count[i] = 0;
            }
            int IDinsert = 0;

            foreach (DataRow dr in accountTable.Rows)
            {
                IDlist[IDinsert] = dr["ID"].ToString();
                PWDlist[IDinsert] = dr["PWD"].ToString();
                IDinsert++;
            }
            for (int i = 0; i < q; i++)
            {
                listView1.Invoke(new MethodInvoker(delegate ()
                {
                    listView1.Items[i].SubItems[2].Text = "판매 물품 삭제중";
                }));
                if (removeCount[i] == false)
                    continue;

                string sendData = "user_id=" + IDlist[i] + "&user_password=" + PWDlist[i];
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                req.CookieContainer = Form1.cookie;

                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.Write(sendData);
                writer.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                req.Abort();
                listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                {
                    listBox1.Items.Add("[" + IDlist[i] + "] " + "판매등록 물품 정보 읽기 시작");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }));
                for (int page = 1; page < 41; page++)
                {
                    HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.html?page=" + page.ToString() + "&strRelationType=regist");
                    req2.Method = "GET";
                    req2.CookieContainer = Form1.cookie;
                    req2.CookieContainer.Add(resp.Cookies);

                    HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                    Stream stReadData = resp2.GetResponseStream();
                    Encoding encode = Encoding.GetEncoding("utf-8");
                    StreamReader srReadData = new StreamReader(stReadData, encode);
                    string strResult = srReadData.ReadToEnd();
                    stReadData.Close();
                    req2.Abort();
                    resp2.Close();
                    int index1 = strResult.IndexOf("check[]\" value=") + 16;
                    int index2 = strResult.IndexOf("\" style=\"border");
                    listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                    {
                        listBox1.Items.Add("[" + IDlist[i] + "] " + page + "페이지 읽음");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }));
                    try
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            registID[i][count[i]] = strResult.Substring(index1, index2 - index1);
                            index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                            index2 = strResult.IndexOf("\" style=\"border", index2 + 1);
                            count[i]++;
                        }
                    }
                    catch
                    {
                        req2.Abort();
                        resp2.Close();
                        break;
                    }
                }
                listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                {
                    listBox1.Items.Add("[" + IDlist[i] + "] " + "판매등록 물품 정보 읽기 종료");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }));

                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.html?strRelationType=regist");
                req3.Method = "GET";
                req3.CookieContainer = Form1.cookie;
                req3.CookieContainer.Add(resp.Cookies);
                resp.Close();
                HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();
                req3.Abort();
                for (int j = 0; j < registID[i].Length; j++)
                {
                    if (registID[i][j] == null)
                    {
                        listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                        {
                            listBox1.Items.Add("[" + IDlist[i] + "] " + "판매등록 삭제 완료");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }));
                        break;
                    }
                    try
                    {
                        string sendData2 = "process=deleteSelect&check[]=" + registID[i][j];
                        HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.php");
                        req4.Referer = "http://trade.itemmania.com/myroom/sell/sell_regist.html?strRelationType=regist";
                        req4.Method = "POST";
                        req4.CookieContainer = Form1.cookie;
                        req4.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        req4.CookieContainer.Add(resp3.Cookies);


                        StreamWriter writer2 = new StreamWriter(req4.GetRequestStream());
                        writer2.Write(sendData2);
                        writer2.Close();

                        HttpWebResponse resp4 = (HttpWebResponse)req4.GetResponse();

                        req4.Abort();
                        resp4.Close();
                        listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                        {
                            listBox1.Items.Add("[" + IDlist[i] + "] " + "판매등록 " + registID[i][j] + " 물품 삭제 완료");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }));
                    }
                    catch
                    {
                        Debug.WriteLine("예외");
                    }
                }

                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                req5.Method = "GET";
                req5.CookieContainer = Form1.cookie;
                HttpWebResponse resp5 = (HttpWebResponse)req5.GetResponse();
                req5.Abort();
                resp5.Close();
            }

            for (int i = 0; i < q; i++)
            {
                registID[i] = new string[100];
                count[i] = 0;
            }

            for (int i = 0; i < q; i++)
            {
                listView1.Invoke(new MethodInvoker(delegate ()
                {
                    listView1.Items[i].SubItems[2].Text = "구매 물품 삭제중";
                }));
                if (removeCount[i] == false)
                    continue;

                string sendData = "user_id=" + IDlist[i] + "&user_password=" + PWDlist[i];
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                req.CookieContainer = Form1.cookie;

                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.Write(sendData);
                writer.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                req.Abort();
                listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                {
                    listBox1.Items.Add("[" + IDlist[i] + "] " + "구매등록 물품 정보 읽기 시작");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }));
                for (int page = 1; page < 11; page++)
                {
                    HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.html?page=" + page.ToString() + "&strRelationType=regist");
                    req2.Method = "GET";
                    req2.CookieContainer = Form1.cookie;
                    req2.CookieContainer.Add(resp.Cookies);

                    HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                    Stream stReadData = resp2.GetResponseStream();
                    Encoding encode = Encoding.GetEncoding("utf-8");
                    StreamReader srReadData = new StreamReader(stReadData, encode);
                    string strResult = srReadData.ReadToEnd();
                    stReadData.Close();
                    req2.Abort();
                    resp2.Close();
                    int index1 = strResult.IndexOf("check[]\" value=") + 16;
                    int index2 = index1 + 16;
                    if (index1 == 15)
                        break;
                    listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                    {
                        listBox1.Items.Add("[" + IDlist[i] + "] " + page + "페이지 읽음");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }));
                    try
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (index1 == 15)
                                break;
                            registID[i][count[i]] = strResult.Substring(index1, index2 - index1);
                            Debug.WriteLine(registID[i][count[i]]);
                            index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                            index2 = index1 + 16;
                            count[i]++;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("예외");
                        req2.Abort();
                        resp2.Close();
                        break;
                    }
                }
                listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                {
                    listBox1.Items.Add("[" + IDlist[i] + "] " + "구매등록 물품 정보 읽기 종료");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }));

                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.html?strRelationType=regist");
                req3.Method = "GET";
                req3.CookieContainer = Form1.cookie;
                req3.CookieContainer.Add(resp.Cookies);
                resp.Close();
                HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();
                req3.Abort();
                for (int j = 0; j < registID[i].Length; j++)
                {
                    if (registID[i][j] == null)
                    {
                        listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                        {
                            listBox1.Items.Add("[" + IDlist[i] + "] " + "구매등록 삭제 완료");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }));
                        break;
                    }
                    try
                    {
                        string sendData2 = "process=deleteSelect&check[]=" + registID[i][j];
                        HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.php");
                        req4.Referer = "http://trade.itemmania.com/myroom/buy/buy_regist.html?strRelationType=regist";
                        req4.Method = "POST";
                        req4.CookieContainer = Form1.cookie;
                        req4.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        req4.CookieContainer.Add(resp3.Cookies);


                        StreamWriter writer2 = new StreamWriter(req4.GetRequestStream());
                        writer2.Write(sendData2);
                        writer2.Close();

                        HttpWebResponse resp4 = (HttpWebResponse)req4.GetResponse();

                        req4.Abort();
                        resp4.Close();
                        listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                        {
                            listBox1.Items.Add("[" + IDlist[i] + "] " + "구매등록 " + registID[i][j] + " 물품 삭제 완료");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        }));
                    }
                    catch
                    {
                        Debug.WriteLine("예외");
                    }
                }

                listView1.Invoke(new MethodInvoker(delegate ()
                {
                    listView1.Items[i].SubItems[2].Text = "삭제완료";
                }));

                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                req5.Method = "GET";
                req5.CookieContainer = Form1.cookie;
                HttpWebResponse resp5 = (HttpWebResponse)req5.GetResponse();
                req5.Abort();
                resp5.Close();
            }            
        }

        private void DeleteForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(delete != null)
            {
                delete.Abort();
            }
        }
    }
}
