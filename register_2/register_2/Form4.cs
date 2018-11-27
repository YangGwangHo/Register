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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace register_2
{
    public partial class Form4 : Form
    {
        DataTable sendItem = new DataTable();
        DataRow row = null;

        public delegate void FormSendDataHandler(DataTable sendItem);
        public event FormSendDataHandler FormSendEvent;
        public Form4()
        {
            InitializeComponent();
            btnSell.Checked = btnItem.Checked = btnNomal.Checked = true;
            ItemNomal1.BringToFront();
            ItemNomal2.BringToFront();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/_xml/gamelist.xml");
            req.Method = "GET";
            req.CookieContainer = Form3.cookie;
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Encoding encode = Encoding.GetEncoding("utf-8");
            Stream stReadData1 = response.GetResponseStream();
            StreamReader srReadData1 = new StreamReader(stReadData1, encode);
            string strResult1 = srReadData1.ReadToEnd();
            int index1 = strResult1.IndexOf("name=\"")+6;
            int index2 = strResult1.IndexOf("\" level");
            while (index1 > 10)
            {
                comboGame.Items.Add(strResult1.Substring(index1, index2 - index1));
                index1 = strResult1.IndexOf("name=\"", index1) + 6;
                index2 = strResult1.IndexOf("\" level", index2 + 1);
            }
            req.Abort();
            /*
            int selectRow = 0;
            StringBuilder getstr = new StringBuilder();
            while (Form1.GetPrivateProfileString("LOGIN", "ID" + selectRow, null, getstr, 1000, Form1.path) != 0)
            {
                Form1.GetPrivateProfileString("LOGIN", "ID" + selectRow, null, getstr, 1000, Form1.path);
                comboID.Items.Add(getstr.ToString());
                selectRow++;
            }
            */

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
                        comboID.Items.Add(reader["ID"].ToString());
                    }
                }

                reader.Close();


                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            sendItem.Columns.Add(new DataColumn("등록", typeof(string)));
            sendItem.Columns.Add(new DataColumn("아이디", typeof(string)));
            sendItem.Columns.Add(new DataColumn("매매구분", typeof(string)));
            sendItem.Columns.Add(new DataColumn("게임명", typeof(string)));
            sendItem.Columns.Add(new DataColumn("서버명", typeof(string)));
            sendItem.Columns.Add(new DataColumn("제목", typeof(string)));
            sendItem.Columns.Add(new DataColumn("물품종류", typeof(string)));
            sendItem.Columns.Add(new DataColumn("가격", typeof(string)));
            sendItem.Columns.Add(new DataColumn("최대수량", typeof(string)));
            sendItem.Columns.Add(new DataColumn("분할단위", typeof(string)));
            sendItem.Columns.Add(new DataColumn("최소수량", typeof(string)));
            button9.Enabled = true;
            button13.Enabled = false;

            if(Form1.updateNum != -1)
            {
                button13.Enabled = true;
                button9.Enabled = false;
                comboID.Text = Form1.updateTable.Rows[0]["아이디"].ToString();
                comboID.Enabled = false;
                if (Form1.updateTable.Rows[0]["매매구분"].ToString() == "판매")
                    btnSell.Checked = true;
                else
                    btnBuy.Checked = true;
                groupBox1.Enabled = false;
                comboGame.Text = Form1.updateTable.Rows[0]["게임명"].ToString();
                comboServer.Text = Form1.updateTable.Rows[0]["서버명"].ToString();
                if (Form1.updateTable.Rows[0]["물품종류"].ToString() == "item")
                    btnItem.Checked = true;
                else if (Form1.updateTable.Rows[0]["물품종류"].ToString() == "money")
                    btnMoney.Checked = true;
                else
                    btnOther.Checked = true;
                groupBox2.Enabled = false;
                if (Form1.updateTable.Rows[0]["일반분할흥정"].ToString() == "general")
                    btnNomal.Checked = true;
                else if (Form1.updateTable.Rows[0]["일반분할흥정"].ToString() == "division")
                    btnDivide.Checked = true;
                else
                    btnHaggle.Checked = true;
                groupBox3.Enabled = false;

                if (btnItem.Checked && btnNomal.Checked)
                {
                    ItemNomal1.BringToFront();
                    ItemNomal2.BringToFront();
                    price1.Text = Form1.updateTable.Rows[0]["가격"].ToString();
                    name1.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    Dserver1.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    title1.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content1.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox2.Items.Add(image[i]);
                }
                else if (btnItem.Checked && btnDivide.Checked)
                {
                    ItemDivide1.BringToFront();
                    ItemDivide2.BringToFront();
                    dividePrice1.Text = Form1.updateTable.Rows[0]["분할단위"].ToString();
                    criteria1.Text = Form1.updateTable.Rows[0]["기준가격"].ToString();
                    max2.Text = Form1.updateTable.Rows[0]["최대수량"].ToString();
                    min2.Text = Form1.updateTable.Rows[0]["최소수량"].ToString();
                    name2.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    Dserver2.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    title2.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content2.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox1.Items.Add(image[i]);
                }
                else if (btnItem.Checked && btnHaggle.Checked)
                {
                    ItemHaggle1.BringToFront();
                    ItemHaggle2.BringToFront();
                    price2.Text = Form1.updateTable.Rows[0]["가격"].ToString();
                    lowestCheck.Text = Form1.updateTable.Rows[0]["최저가격체크"].ToString();
                    lowestPrice.Text = Form1.updateTable.Rows[0]["최저가격"].ToString();
                    name3.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    title3.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content3.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox5.Items.Add(image[i]);
                }
                else if (btnOther.Checked && btnNomal.Checked)
                {
                    ItemNomal1.BringToFront();
                    ItemNomal2.BringToFront();
                    price1.Text = Form1.updateTable.Rows[0]["가격"].ToString();
                    name1.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    Dserver1.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    title1.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content1.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox2.Items.Add(image[i]);
                }
                else if (btnOther.Checked && btnDivide.Checked)
                {
                    ItemDivide1.BringToFront();
                    ItemDivide2.BringToFront();
                    dividePrice1.Text = Form1.updateTable.Rows[0]["분할단위"].ToString();
                    criteria1.Text = Form1.updateTable.Rows[0]["기준가격"].ToString();
                    max2.Text = Form1.updateTable.Rows[0]["최대수량"].ToString();
                    min2.Text = Form1.updateTable.Rows[0]["최소수량"].ToString();
                    name2.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    Dserver2.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    title2.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content2.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox1.Items.Add(image[i]);
                }
                else if (btnOther.Checked && btnHaggle.Checked)
                {
                    ItemHaggle1.BringToFront();
                    ItemHaggle2.BringToFront();
                    price2.Text = Form1.updateTable.Rows[0]["가격"].ToString();
                    lowestCheck.Text = Form1.updateTable.Rows[0]["최저가격체크"].ToString();
                    lowestPrice.Text = Form1.updateTable.Rows[0]["최저가격"].ToString();
                    name3.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    title3.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content3.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox5.Items.Add(image[i]);
                }
                else if (btnMoney.Checked && btnNomal.Checked)
                {
                    MoneyNomal1.BringToFront();
                    MoneyNomal2.BringToFront();
                    if (Form1.updateTable.Rows[0]["거래수량단위"].ToString() == "1")
                        btnNon.Checked = true;
                    else if (Form1.updateTable.Rows[0]["거래수량단위"].ToString() == "만")
                        btnMan.Checked = true;
                    else
                        btnUk.Checked = true;
                    user_quantity.Text = Form1.updateTable.Rows[0]["거래수량"].ToString();
                    price3.Text = Form1.updateTable.Rows[0]["가격"].ToString();
                    name4.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    title4.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content4.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    Dserver4.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    if (Form1.updateTable.Rows[0]["즉시구매"].ToString() == "1")
                        rightoff.Checked = true;
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox3.Items.Add(image[i]);
                }
                else if (btnMoney.Checked && btnDivide.Checked)
                {
                    MoneyDivide1.BringToFront();
                    MoneyDivide2.BringToFront();
                    if (Form1.updateTable.Rows[0]["거래수량단위"].ToString() == "1")
                        btnNon2.Checked = true;
                    else if (Form1.updateTable.Rows[0]["거래수량단위"].ToString() == "만")
                        btnMan2.Checked = true;
                    else
                        btnUk2.Checked = true;
                    dividePrice2.Text = Form1.updateTable.Rows[0]["분할단위"].ToString();
                    criteria2.Text = Form1.updateTable.Rows[0]["기준가격"].ToString();
                    max1.Text = Form1.updateTable.Rows[0]["최대수량"].ToString();
                    min1.Text = Form1.updateTable.Rows[0]["최소수량"].ToString();
                    name5.Text = Form1.updateTable.Rows[0]["캐릭터명"].ToString();
                    Dserver5.Text = Form1.updateTable.Rows[0]["전달서버"].ToString();
                    title5.Text = Form1.updateTable.Rows[0]["제목"].ToString();
                    content5.Text = Form1.updateTable.Rows[0]["내용"].ToString();
                    string[] image;
                    image = Form1.updateTable.Rows[0]["이미지"].ToString().Split('|');
                    for (int i = 0; i < image.Length; i++)
                        listBox4.Items.Add(image[i]);
                }

                if (btnSell.Checked)
                {
                    panel5.Visible = false;
                    panel6.Visible = false;
                    btnHaggle.Visible = true;
                }
                else if (btnBuy.Checked)
                {
                    panel5.Visible = true;
                    panel6.Visible = true;
                    btnHaggle.Visible = false;
                }
                Form1.updateTable.Rows.Remove(Form1.updateTable.Rows[0]);
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSell_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSell.Checked)
            {
                panel5.Visible = false;
                panel6.Visible = false;
            }
            else if (btnBuy.Checked)
            {
                panel5.Visible = true;
                panel6.Visible = true;
            }
            btnHaggle.Visible = true;
        }

        private void BtnBuy_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSell.Checked)
            {
                panel5.Visible = false;
                panel6.Visible = false;
            }
            else if (btnBuy.Checked)
            {
                panel5.Visible = true;
                panel6.Visible = true;
            }
            btnHaggle.Visible = false;

            if (btnHaggle.Checked)
            {
                btnItem.Checked = true;
                btnNomal.Checked = true;
            }
        }

        private void BtnItem_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
            btnHaggle.Visible = true;
        }
        private void BtnMoney_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
            btnHaggle.Visible = false;
        }
        private void BtnOther_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
            btnHaggle.Visible = true;
        }
        private void BtnNomal_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
        }
        private void BtnDivide_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
        }
        private void BtnHaggle_CheckedChanged(object sender, EventArgs e)
        {
            ShowPanel();
        }
        private void ShowPanel()
        {
            if (btnItem.Checked && btnNomal.Checked)
            {
                ItemNomal1.BringToFront();
                ItemNomal2.BringToFront();
            }
            else if (btnItem.Checked && btnDivide.Checked)
            {
                ItemDivide1.BringToFront();
                ItemDivide2.BringToFront();
            }
            else if (btnItem.Checked && btnHaggle.Checked)
            {
                ItemHaggle1.BringToFront();
                ItemHaggle2.BringToFront();
            }
            else if (btnOther.Checked && btnNomal.Checked)
            {
                ItemNomal1.BringToFront();
                ItemNomal2.BringToFront();
            }
            else if (btnOther.Checked && btnDivide.Checked)
            {
                ItemDivide1.BringToFront();
                ItemDivide2.BringToFront();
            }
            else if (btnOther.Checked && btnHaggle.Checked)
            {
                ItemHaggle1.BringToFront();
                ItemHaggle2.BringToFront();
            }
            else if (btnMoney.Checked && btnNomal.Checked)
            {
                MoneyNomal1.BringToFront();
                MoneyNomal2.BringToFront();
            }
            else if (btnMoney.Checked && btnDivide.Checked)
            {
                MoneyDivide1.BringToFront();
                MoneyDivide2.BringToFront();
            }
        }

        private void ComboGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboServer.Items.Clear();
            comboServer.Items.Add("전체 추가");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/_xml/gamelist.xml");
            req.Method = "GET";
            req.CookieContainer = Form3.cookie;
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            Encoding encode = Encoding.GetEncoding("utf-8");
            Stream stReadData1 = response.GetResponseStream();
            StreamReader srReadData1 = new StreamReader(stReadData1, encode);
            string strResult1 = srReadData1.ReadToEnd();
            int index1 = strResult1.IndexOf("id=\"");
            int index2 = strResult1.IndexOf("\" name=")+8;
            int index3 = strResult1.IndexOf("\" level");
            
            while (strResult1.Substring(index2, index3 - index2) != comboGame.Text)
            {
                index1 = strResult1.IndexOf("id=\"",index1+1);
                index2 = strResult1.IndexOf("\" name=",index2)+8;
                index3 = strResult1.IndexOf("\" level",index3+1);
            }
            index1 = index1 + 4;
            index2 = index2 - 8;

            req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/_xml/serverlist.php?game="+ strResult1.Substring(index1, index2 - index1));
            req.Method = "GET";
            req.CookieContainer = Form3.cookie;
            response = (HttpWebResponse)req.GetResponse();
            stReadData1 = response.GetResponseStream();
            srReadData1 = new StreamReader(stReadData1, encode);
            strResult1 = srReadData1.ReadToEnd();
            index1 = strResult1.IndexOf("NAME=\"") + 6;
            index2 = strResult1.IndexOf("\" MONEY");

            while (index1 > 10)
            { 
                comboServer.Items.Add(strResult1.Substring(index1, index2 - index1));
                index1 = strResult1.IndexOf("NAME=\"", index1) + 6;
                index2 = strResult1.IndexOf("\" MONEY=", index2 + 1);
            }
            req.Abort();
            comboServer.Items.Remove("서버전체");

            if(comboGame.Text == "던전앤파이터")
            {
                Dserver2.Visible = Dserver3.Visible = Dserver4.Visible = Dserver5.Visible = Dserver1.Visible = dfLabel1.Visible = dfLabel2.Visible = dfLabel3.Visible = dfLabel4.Visible = dfLabel5.Visible = true;
            }
            else
            {

                Dserver2.Visible = Dserver3.Visible = Dserver4.Visible = Dserver5.Visible = Dserver1.Visible = dfLabel1.Visible = dfLabel2.Visible = dfLabel3.Visible = dfLabel4.Visible = dfLabel5.Visible = false;
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "열기";
            openFile.Filter = "Images (*.JPG,*.GIF)|*.JPG;*.GIF";
            DialogResult result = openFile.ShowDialog();
            if(result == DialogResult.OK)
            {
                listBox4.Items.Add(openFile.FileName);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "열기";
            openFile.Filter = "Images (*.JPG,*.GIF)|*.JPG;*.GIF";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                listBox2.Items.Add(openFile.FileName);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "열기";
            openFile.Filter = "Images (*.JPG,*.GIF)|*.JPG;*.GIF";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                listBox1.Items.Add(openFile.FileName);
            }
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "열기";
            openFile.Filter = "Images (*.JPG,*.GIF)|*.JPG;*.GIF";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                listBox5.Items.Add(openFile.FileName);
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "열기";
            openFile.Filter = "Images (*.JPG,*.GIF)|*.JPG;*.GIF";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                listBox3.Items.Add(openFile.FileName);
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            listBox4.Items.Remove(listBox4.SelectedItem);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            listBox5.Items.Remove(listBox5.SelectedItem);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            listBox3.Items.Remove(listBox3.SelectedItem);
        }

        private void Button9_Click(object sender, EventArgs e)
        {

            string 등록 = "체크";
            string 아이디 = comboID.Text;
            string 매매구분 = "";
            if (btnSell.Checked == true)
            {
                매매구분 = "판매";
            }
            else if (btnBuy.Checked == true)
            {
                매매구분 = "구매";
            }
            string 게임명 = comboGame.Text;
            string 서버명 = comboServer.Text;
            string 물품종류 = "";
            if (btnItem.Checked == true)
            {
                물품종류 = "item";
            }
            else if (btnMoney.Checked == true)
            {
                물품종류 = "money";
            }
            else if (btnOther.Checked == true)
            {
                물품종류 = "etc";
            }
            string 일반분할흥정 = "";
            if (btnNomal.Checked == true)
            {
                일반분할흥정 = "general";
            }
            else if (btnDivide.Checked == true)
            {
                일반분할흥정 = "division";
            }
            else if (btnHaggle.Checked == true)
            {
                일반분할흥정 = "bargain";
            }
            string 제목 = title1.Text + title2.Text + title3.Text + title4.Text + title5.Text;
            string 내용 = content1.Text + content2.Text + content3.Text + content4.Text + content5.Text;
            string 가격 = price1.Text + price2.Text + price3.Text;
            string 거래수량 = user_quantity.Text;
            string 거래수량단위 = "";
            if (btnNon.Checked == true || btnNon2.Checked == true)
            {
                거래수량단위 = "1";
            }
            else if (btnMan.Checked == true || btnMan2.Checked == true)
            {
                거래수량단위 = "만";
            }
            else if (btnUk.Checked == true || btnUk2.Checked == true)
            {
                거래수량단위 = "억";
            }
            else
            {
                거래수량단위 = "";
            }
            string 최대수량 = max1.Text + max2.Text;
            string 최소수량 = min1.Text + min2.Text;
            string 분할단위 = dividePrice1.Text + dividePrice2.Text;
            string 즉시구매 = "";
            if (rightoff.Checked == true)
            {
                즉시구매 = "1";
            }
            else
            {
                즉시구매 = "";
            }
            string 전달서버 = Dserver1.Text + Dserver2.Text + Dserver3.Text + Dserver4.Text + Dserver5.Text;
            string 캐릭터명 = name1.Text + name2.Text + name3.Text + name4.Text + name5.Text;
            string 기준가격 = criteria1.Text + criteria2.Text;
            string 최저가격체크 = "";
            if (lowestCheck.Checked == true)
            {
                최저가격체크 = "1";
            }
            else
            {
                최저가격체크 = "";
            }
            string 최저가격 = lowestPrice.Text;
            string 이미지 = "";
            if (listBox1.Items.Count != 0)
            {
                int x = listBox1.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString() + "|" + listBox1.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox1.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }
            if (listBox2.Items.Count != 0)
            {
                int x = listBox2.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString() + "|" + listBox2.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox2.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox3.Items.Count != 0)
            {
                int x = listBox3.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString() + "|" + listBox3.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox3.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }


            if (listBox4.Items.Count != 0)
            {
                int x = listBox4.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString() + "|" + listBox4.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox4.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox5.Items.Count != 0)
            {
                int x = listBox5.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString() + "|" + listBox5.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox5.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (아이디 == "")
                MessageBox.Show("아이디를 선택해주세요");
            else if (게임명 == "")
                MessageBox.Show("게임명을 선택해주세요");
            else if (서버명 == "")
                MessageBox.Show("서버명을 선택해주세요");

            else if (일반분할흥정 == "general")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {
                    if (comboServer.Text == "전체 추가")
                    {
                        int j = 1;
                        int i = comboServer.Items.Count;

                        while (i > j)
                        {
                            comboServer.SelectedIndex = j;
                            서버명 = comboServer.Text;
                            try
                            {

                                string DbFile = "data.dat";
                                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                                sqliteConn.Open();

                                string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                                cmd.ExecuteNonQuery();
                                sqliteConn.Close();
                                row = sendItem.NewRow();

                                row["등록"] = 등록;
                                row["아이디"] = 아이디;
                                row["매매구분"] = 매매구분;
                                row["게임명"] = 게임명;
                                row["서버명"] = 서버명;
                                row["제목"] = 제목;
                                row["물품종류"] = 물품종류;
                                row["가격"] = 가격;
                                row["최대수량"] = 최대수량;
                                row["분할단위"] = 분할단위;
                                row["최소수량"] = 최소수량;

                                sendItem.Rows.Add(row);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            j++;
                        }

                        this.FormSendEvent(sendItem);

                        this.Close();
                    }
                    else
                    {
                        try
                        {

                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                            SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                            cmd.ExecuteNonQuery();

                            sqliteConn.Close();
                            row = sendItem.NewRow();
                            row["등록"] = 등록;
                            row["아이디"] = 아이디;
                            row["매매구분"] = 매매구분;
                            row["게임명"] = 게임명;
                            row["서버명"] = 서버명;
                            row["제목"] = 제목;
                            row["물품종류"] = 물품종류;
                            row["가격"] = 가격;
                            row["최대수량"] = 최대수량;
                            row["분할단위"] = 분할단위;
                            row["최소수량"] = 최소수량;

                            sendItem.Rows.Add(row);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        this.FormSendEvent(sendItem);

                        this.Close();
                    }
                }
            }
            else if (일반분할흥정 == "division")
            {
                if (분할단위 == "")
                    MessageBox.Show("분할단위를 입력해주세요");
                else if (기준가격 == "")
                    MessageBox.Show("기준가격을 입력해주세요");
                else if (최대수량 == "")
                    MessageBox.Show("최대수량을 입력해주세요");
                else if (최소수량 == "")
                    MessageBox.Show("최소수량을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {
                    if (comboServer.Text == "전체 추가")
                    {
                        int j = 1;
                        int i = comboServer.Items.Count;

                        while (i > j)
                        {
                            comboServer.SelectedIndex = j;
                            서버명 = comboServer.Text;
                            try
                            {

                                string DbFile = "data.dat";
                                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                                sqliteConn.Open();
                                string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                                cmd.ExecuteNonQuery();

                                sqliteConn.Close();
                                row = sendItem.NewRow();
                                row["등록"] = 등록;
                                row["아이디"] = 아이디;
                                row["매매구분"] = 매매구분;
                                row["게임명"] = 게임명;
                                row["서버명"] = 서버명;
                                row["제목"] = 제목;
                                row["물품종류"] = 물품종류;
                                row["가격"] = 가격;
                                row["최대수량"] = 최대수량;
                                row["분할단위"] = 분할단위;
                                row["최소수량"] = 최소수량;

                                sendItem.Rows.Add(row);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            j++;
                        }


                        this.FormSendEvent(sendItem);

                        this.Close();
                    }
                    else
                    {
                        try
                        {

                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                            SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                            cmd.ExecuteNonQuery();

                            sqliteConn.Close();
                            row = sendItem.NewRow();
                            row["등록"] = 등록;
                            row["아이디"] = 아이디;
                            row["매매구분"] = 매매구분;
                            row["게임명"] = 게임명;
                            row["서버명"] = 서버명;
                            row["제목"] = 제목;
                            row["물품종류"] = 물품종류;
                            row["가격"] = 가격;
                            row["최대수량"] = 최대수량;
                            row["분할단위"] = 분할단위;
                            row["최소수량"] = 최소수량;

                            sendItem.Rows.Add(row);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        this.FormSendEvent(sendItem);

                        this.Close();

                    }
                }
            }
            else if (일반분할흥정 == "bargain")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {

                    if (comboServer.Text == "전체 추가")
                    {
                        int j = 1;
                        int i = comboServer.Items.Count;

                        while (i > j)
                        {
                            comboServer.SelectedIndex = j;
                            서버명 = comboServer.Text;
                            try
                            {

                                string DbFile = "data.dat";
                                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                                sqliteConn.Open();
                                string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                                cmd.ExecuteNonQuery();

                                sqliteConn.Close();
                                row = sendItem.NewRow();
                                row["등록"] = 등록;
                                row["아이디"] = 아이디;
                                row["매매구분"] = 매매구분;
                                row["게임명"] = 게임명;
                                row["서버명"] = 서버명;
                                row["제목"] = 제목;
                                row["물품종류"] = 물품종류;
                                row["가격"] = 가격;
                                row["최대수량"] = 최대수량;
                                row["분할단위"] = 분할단위;
                                row["최소수량"] = 최소수량;

                                sendItem.Rows.Add(row);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            j++;
                        }


                        this.FormSendEvent(sendItem);

                        this.Close();
                    }
                    else
                    {
                        try
                        {

                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                            SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                            cmd.ExecuteNonQuery();

                            sqliteConn.Close();
                            row = sendItem.NewRow();
                            row["등록"] = 등록;
                            row["아이디"] = 아이디;
                            row["매매구분"] = 매매구분;
                            row["게임명"] = 게임명;
                            row["서버명"] = 서버명;
                            row["제목"] = 제목;
                            row["물품종류"] = 물품종류;
                            row["가격"] = 가격;
                            row["최대수량"] = 최대수량;
                            row["분할단위"] = 분할단위;
                            row["최소수량"] = 최소수량;

                            sendItem.Rows.Add(row);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                        this.FormSendEvent(sendItem);

                        this.Close();
                    }
                }
            }
        }

        private void RegistProduct()
        {
            string 등록 = "체크";
            string 아이디 = comboID.Text;
            string 매매구분 ="";
            if (btnSell.Checked == true)
            {
                매매구분 = "판매";
            }
            else if (btnBuy.Checked == true)
            {
                매매구분 = "구매";
            }
            string 게임명 = comboGame.Text;
            string 서버명 = comboServer.Text;
            string 물품종류 ="";
            if (btnItem.Checked == true)
            {
                물품종류 = "item";
            }
            else if (btnMoney.Checked == true)
            {
                물품종류 = "money";
            }
            else if (btnOther.Checked == true)
            {
                물품종류 = "etc";
            }
            string 일반분할흥정 ="";
            if (btnNomal.Checked == true)
            {
                일반분할흥정 = "general";
            }
            else if (btnDivide.Checked == true)
            {
                일반분할흥정 = "division";
            }
            else if (btnHaggle.Checked == true)
            {
                일반분할흥정 = "bargain";
            }
            string 제목 = title1.Text + title2.Text + title3.Text + title4.Text + title5.Text;
            string 내용 = content1.Text + content2.Text + content3.Text + content4.Text + content5.Text;
            string 가격 = price1.Text + price2.Text + price3.Text;
            string 거래수량 = user_quantity.Text;
            string 거래수량단위 ="";
            if (btnNon.Checked == true || btnNon2.Checked == true)
            {
                거래수량단위 = "1";
            }
            else if (btnMan.Checked == true || btnMan2.Checked == true)
            {
                거래수량단위 = "만";
            }
            else if (btnUk.Checked == true || btnUk2.Checked == true)
            {
                거래수량단위 = "억";
            }
            else
            {
                거래수량단위 = "";
            }
            string 최대수량 = max1.Text + max2.Text;
            string 최소수량 = min1.Text + min2.Text;
            string 분할단위 = dividePrice1.Text + dividePrice2.Text;
            string 즉시구매 ="";
            if (rightoff.Checked == true)
            {
                즉시구매 =  "1";
            }
            else
            {
                즉시구매 = "";
            }
            string 전달서버 = Dserver1.Text + Dserver2.Text + Dserver3.Text + Dserver4.Text + Dserver5.Text;
            string 캐릭터명 = name1.Text + name2.Text + name3.Text + name4.Text + name5.Text;
            string 기준가격 = criteria1.Text + criteria2.Text;
            string 최저가격체크 ="";
            if (lowestCheck.Checked == true)
            {
                최저가격체크 =  "1";
            }
            else
            {
                최저가격체크 = "";
            }
            string 최저가격 = lowestPrice.Text;
            string 이미지 ="";
            if (listBox1.Items.Count != 0)
            {
                int x = listBox1.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString() + "|" + listBox1.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox1.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }
            if (listBox2.Items.Count != 0)
            {
                int x = listBox2.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString() + "|" + listBox2.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox2.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox3.Items.Count != 0)
            {
                int x = listBox3.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString() + "|" + listBox3.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 =listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox3.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }


            if (listBox4.Items.Count != 0)
            {
                int x = listBox4.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString() + "|" + listBox4.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox4.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox5.Items.Count != 0)
            {
                int x = listBox5.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString() + "|" + listBox5.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox5.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (아이디 == "")
                MessageBox.Show("아이디를 선택해주세요");
            else if (게임명 == "")
                MessageBox.Show("게임명을 선택해주세요");
            else if (서버명 == "")
                MessageBox.Show("서버명을 선택해주세요");

            if (일반분할흥정 == "general")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
            }
            else
            {
                try
                {

                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                    SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                    cmd.ExecuteNonQuery();

                    sqliteConn.Close();
                    row = sendItem.NewRow();

                    row["등록"] = 등록;
                    row["아이디"] = 아이디;
                    row["매매구분"] = 매매구분;
                    row["게임명"] = 게임명;
                    row["서버명"] = 서버명;
                    row["제목"] = 제목;
                    row["물품종류"] = 물품종류;
                    row["가격"] = 가격;
                    row["최대수량"] = 최대수량;
                    row["분할단위"] = 분할단위;
                    row["최소수량"] = 최소수량;

                    sendItem.Rows.Add(row);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (일반분할흥정 == "division")
            {
                if (분할단위 == "")
                    MessageBox.Show("분할단위를 입력해주세요");
                else if (기준가격 == "")
                    MessageBox.Show("기준가격을 입력해주세요");
                else if (최대수량 == "")
                    MessageBox.Show("최대수량을 입력해주세요");
                else if (최소수량 == "")
                    MessageBox.Show("최소수량을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
            }
            else
            {
                try
                {

                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                    SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                    cmd.ExecuteNonQuery();

                    sqliteConn.Close();
                    row = sendItem.NewRow();

                    row["등록"] = 등록;
                    row["아이디"] = 아이디;
                    row["매매구분"] = 매매구분;
                    row["게임명"] = 게임명;
                    row["서버명"] = 서버명;
                    row["제목"] = 제목;
                    row["물품종류"] = 물품종류;
                    row["가격"] = 가격;
                    row["최대수량"] = 최대수량;
                    row["분할단위"] = 분할단위;
                    row["최소수량"] = 최소수량;

                    sendItem.Rows.Add(row);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (일반분할흥정 == "bargain")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
            }
            else
            {
                try
                {

                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + 등록 + "','" + 아이디 + "','" + 매매구분 + "','" + 게임명 + "','" + 서버명 + "','" + 물품종류 + "','" + 일반분할흥정 + "','" + 제목 + "','" + 내용 + "','" + 가격 + "','" + 거래수량 + "','" + 거래수량단위 + "','" + 최대수량 + "','" + 최소수량 + "','" + 분할단위 + "','" + 즉시구매 + "','" + 전달서버 + "','" + 캐릭터명 + "','" + 기준가격 + "','" + 최저가격체크 + "','" + 최저가격 + "','" + 이미지 + "')";
                    SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                    cmd.ExecuteNonQuery();

                    sqliteConn.Close();
                    row = sendItem.NewRow();

                    row["등록"] = 등록;
                    row["아이디"] = 아이디;
                    row["매매구분"] = 매매구분;
                    row["게임명"] = 게임명;
                    row["서버명"] = 서버명;
                    row["제목"] = 제목;
                    row["물품종류"] = 물품종류;
                    row["가격"] = 가격;
                    row["최대수량"] = 최대수량;
                    row["분할단위"] = 분할단위;
                    row["최소수량"] = 최소수량;

                    sendItem.Rows.Add(row);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string 아이디 = comboID.Text;
            string 매매구분 = "";
            if (btnSell.Checked == true)
            {
                매매구분 = "판매";
            }
            else if (btnBuy.Checked == true)
            {
                매매구분 = "구매";
            }
            string 게임명 = comboGame.Text;
            string 서버명 = comboServer.Text;
            string 물품종류 = "";
            if (btnItem.Checked == true)
            {
                물품종류 = "item";
            }
            else if (btnMoney.Checked == true)
            {
                물품종류 = "money";
            }
            else if (btnOther.Checked == true)
            {
                물품종류 = "etc";
            }
            string 일반분할흥정 = "";
            if (btnNomal.Checked == true)
            {
                일반분할흥정 = "general";
            }
            else if (btnDivide.Checked == true)
            {
                일반분할흥정 = "division";
            }
            else if (btnHaggle.Checked == true)
            {
                일반분할흥정 = "bargain";
            }
            string 제목 = title1.Text + title2.Text + title3.Text + title4.Text + title5.Text;
            string 내용 = content1.Text + content2.Text + content3.Text + content4.Text + content5.Text;
            string 가격 = price1.Text + price2.Text + price3.Text;
            string 거래수량 = user_quantity.Text;
            string 거래수량단위 = "";
            if (btnNon.Checked == true || btnNon2.Checked == true)
            {
                거래수량단위 = "1";
            }
            else if (btnMan.Checked == true || btnMan2.Checked == true)
            {
                거래수량단위 = "만";
            }
            else if (btnUk.Checked == true || btnUk2.Checked == true)
            {
                거래수량단위 = "억";
            }
            else
            {
                거래수량단위 = "";
            }
            string 최대수량 = max1.Text + max2.Text;
            string 최소수량 = min1.Text + min2.Text;
            string 분할단위 = dividePrice1.Text + dividePrice2.Text;
            string 즉시구매 = "";
            if (rightoff.Checked == true)
            {
                즉시구매 = "1";
            }
            else
            {
                즉시구매 = "";
            }
            string 전달서버 = Dserver1.Text + Dserver2.Text + Dserver3.Text + Dserver4.Text + Dserver5.Text;
            string 캐릭터명 = name1.Text + name2.Text + name3.Text + name4.Text + name5.Text;
            string 기준가격 = criteria1.Text + criteria2.Text;
            string 최저가격체크 = "";
            if (lowestCheck.Checked == true)
            {
                최저가격체크 = "1";
            }
            else
            {
                최저가격체크 = "";
            }
            string 최저가격 = lowestPrice.Text;
            string 이미지 = "";
            if (listBox1.Items.Count != 0)
            {
                int x = listBox1.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString() + "|" + listBox1.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString() + "|" + listBox1.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString() + "|" + listBox1.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox1.Items[0].ToString() + "|" + listBox1.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox1.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }
            if (listBox2.Items.Count != 0)
            {
                int x = listBox2.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString() + "|" + listBox2.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString() + "|" + listBox2.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString() + "|" + listBox2.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox2.Items[0].ToString() + "|" + listBox2.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox2.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox3.Items.Count != 0)
            {
                int x = listBox3.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString() + "|" + listBox3.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString() + "|" + listBox3.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString() + "|" + listBox3.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox3.Items[0].ToString() + "|" + listBox3.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox3.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }


            if (listBox4.Items.Count != 0)
            {
                int x = listBox4.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString() + "|" + listBox4.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString() + "|" + listBox4.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString() + "|" + listBox4.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox4.Items[0].ToString() + "|" + listBox4.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox4.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }

            if (listBox5.Items.Count != 0)
            {
                int x = listBox5.Items.Count;
                if (x == 5)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString() + "|" + listBox5.Items[4].ToString();
                }
                else if (x == 4)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString() + "|" + listBox5.Items[3].ToString();
                }
                else if (x == 3)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString() + "|" + listBox5.Items[2].ToString();
                }
                else if (x == 2)
                {
                    이미지 = listBox5.Items[0].ToString() + "|" + listBox5.Items[1].ToString();
                }
                else if (x == 1)
                {
                    이미지 = listBox5.Items[0].ToString();
                }
                else
                {
                    이미지 = "";

                }
            }
            if (아이디 == "")
                MessageBox.Show("아이디를 선택해주세요");
            else if (게임명 == "")
                MessageBox.Show("게임명을 선택해주세요");
            else if (서버명 == "")
                MessageBox.Show("서버명을 선택해주세요");
            else if (일반분할흥정 == "general")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {

                    try
                    {

                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql = "UPDATE regist SET 아이디='" + 아이디 + "',매매구분='" + 매매구분 + "',게임명='" + 게임명 + "',서버명='" + 서버명 + "',물품종류='" + 물품종류 + "',일반분할흥정='" + 일반분할흥정 + "',제목='" + 제목 + "',내용='" + 내용 + "',가격='" + 가격 + "',거래수량='" + 거래수량 + "',거래수량단위='" + 거래수량단위 + "',최대수량='" + 최대수량 + "',최소수량='" + 최소수량 + "',분할단위='" + 분할단위 + "',즉시구매='" + 즉시구매 + "',전달서버='" + 전달서버 + "',캐릭터명='" + 캐릭터명 + "',기준가격='" + 기준가격 + "',최저가격체크='" + 최저가격체크 + "',최저가격='" + 최저가격 + "',이미지='" + 이미지 + "'  where rowid IN (SELECT rowid FROM regist LIMIT " + Form1.updateNum + ",1)";
                        SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                        cmd.ExecuteNonQuery();

                        sqliteConn.Close();
                        row = sendItem.NewRow();

                        row["아이디"] = 아이디;
                        row["매매구분"] = 매매구분;
                        row["게임명"] = 게임명;
                        row["서버명"] = 서버명;
                        row["제목"] = 제목;
                        row["물품종류"] = 물품종류;
                        row["가격"] = 가격;
                        row["최대수량"] = 최대수량;
                        row["분할단위"] = 분할단위;
                        row["최소수량"] = 최소수량;

                        sendItem.Rows.Add(row);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Form1.updateNum = -1;
                    Form1.updateTable = new DataTable();
                    this.FormSendEvent(sendItem);

                    this.Close();
                }
            }
            else if(일반분할흥정 == "division")
            {
                if(분할단위 =="")
                    MessageBox.Show("분할단위를 입력해주세요");
                else if (기준가격 == "")
                    MessageBox.Show("기준가격을 입력해주세요");
                else if (최대수량 == "")
                    MessageBox.Show("최대수량을 입력해주세요");
                else if (최소수량 == "")
                    MessageBox.Show("최소수량을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {

                    try
                    {

                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql = "UPDATE regist SET 아이디='" + 아이디 + "',매매구분='" + 매매구분 + "',게임명='" + 게임명 + "',서버명='" + 서버명 + "',물품종류='" + 물품종류 + "',일반분할흥정='" + 일반분할흥정 + "',제목='" + 제목 + "',내용='" + 내용 + "',가격='" + 가격 + "',거래수량='" + 거래수량 + "',거래수량단위='" + 거래수량단위 + "',최대수량='" + 최대수량 + "',최소수량='" + 최소수량 + "',분할단위='" + 분할단위 + "',즉시구매='" + 즉시구매 + "',전달서버='" + 전달서버 + "',캐릭터명='" + 캐릭터명 + "',기준가격='" + 기준가격 + "',최저가격체크='" + 최저가격체크 + "',최저가격='" + 최저가격 + "',이미지='" + 이미지 + "'  where rowid IN (SELECT rowid FROM regist LIMIT " + Form1.updateNum + ",1)";
                        SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                        cmd.ExecuteNonQuery();

                        sqliteConn.Close();
                        row = sendItem.NewRow();

                        row["아이디"] = 아이디;
                        row["매매구분"] = 매매구분;
                        row["게임명"] = 게임명;
                        row["서버명"] = 서버명;
                        row["제목"] = 제목;
                        row["물품종류"] = 물품종류;
                        row["가격"] = 가격;
                        row["최대수량"] = 최대수량;
                        row["분할단위"] = 분할단위;
                        row["최소수량"] = 최소수량;

                        sendItem.Rows.Add(row);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Form1.updateNum = -1;
                    Form1.updateTable = new DataTable();
                    this.FormSendEvent(sendItem);

                    this.Close();
                }
            }
            else if(일반분할흥정 == "bargain")
            {
                if (가격 == "")
                    MessageBox.Show("가격을 입력해주세요");
                else if (캐릭터명 == "")
                    MessageBox.Show("캐릭터명을 입력해주세요");
                else if (제목 == "")
                    MessageBox.Show("제목을 입력해주세요");
                else if (내용 == "")
                    MessageBox.Show("내용을 입력해주세요");
                else
                {

                    try
                    {

                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql = "UPDATE regist SET 아이디='" + 아이디 + "',매매구분='" + 매매구분 + "',게임명='" + 게임명 + "',서버명='" + 서버명 + "',물품종류='" + 물품종류 + "',일반분할흥정='" + 일반분할흥정 + "',제목='" + 제목 + "',내용='" + 내용 + "',가격='" + 가격 + "',거래수량='" + 거래수량 + "',거래수량단위='" + 거래수량단위 + "',최대수량='" + 최대수량 + "',최소수량='" + 최소수량 + "',분할단위='" + 분할단위 + "',즉시구매='" + 즉시구매 + "',전달서버='" + 전달서버 + "',캐릭터명='" + 캐릭터명 + "',기준가격='" + 기준가격 + "',최저가격체크='" + 최저가격체크 + "',최저가격='" + 최저가격 + "',이미지='" + 이미지 + "'  where rowid IN (SELECT rowid FROM regist LIMIT " + Form1.updateNum + ",1)";
                        SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                        cmd.ExecuteNonQuery();

                        sqliteConn.Close();
                        row = sendItem.NewRow();

                        row["아이디"] = 아이디;
                        row["매매구분"] = 매매구분;
                        row["게임명"] = 게임명;
                        row["서버명"] = 서버명;
                        row["제목"] = 제목;
                        row["물품종류"] = 물품종류;
                        row["가격"] = 가격;
                        row["최대수량"] = 최대수량;
                        row["분할단위"] = 분할단위;
                        row["최소수량"] = 최소수량;

                        sendItem.Rows.Add(row);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Form1.updateNum = -1;
                    Form1.updateTable = new DataTable();
                    this.FormSendEvent(sendItem);

                    this.Close();
                }
            }
        }
    }
}
