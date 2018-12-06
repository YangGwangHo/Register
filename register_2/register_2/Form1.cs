using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Collections;

namespace register_2
{
    public partial class Form1 : Form
    {
        public static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        public static void LogWrite(string str)
        {
            string FilePath = Environment.CurrentDirectory + @"\Log\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string DirPath = Environment.CurrentDirectory + @"\Log";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch
            {
            }
        }


        [DllImport("kernel32")]

        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]

        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        
        public static string path = Environment.CurrentDirectory + "\\option.ini";

        public static CookieContainer cookie = new CookieContainer();
        static string[] RegistID;
        public static Thread regist = new Thread(Regist);
        public static Thread deleteLoop = new Thread(DeleteLoop);
        static ListBox listBox1 = new ListBox();
        static int listviewcount = 0;
        public static int updateNum = -1;
        public static DataTable updateTable = new DataTable();
        public static bool sellDeleteCheck = false;
        public static int sellDeleteTime = 0;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            try
            {

                string FilePath = Environment.CurrentDirectory + "\\option.ini";
                string DirPath = Environment.CurrentDirectory;

                DirectoryInfo di = new DirectoryInfo(DirPath);
                FileInfo fi = new FileInfo(FilePath);

                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    WritePrivateProfileString("OPTION", "SellDeleteCheck", "0", path);
                    WritePrivateProfileString("OPTION", "SellDeleteTime", "0", path);
                    WritePrivateProfileString("OPTION", "BuyDeleteCheck", "0", path);
                    WritePrivateProfileString("OPTION", "BuyDeleteTime", "0", path);
                }
            }
            catch
            {
            }

            panel4.Controls.Add(listBox1);
            listBox1.Dock = System.Windows.Forms.DockStyle.Fill;


            //DB생성
            string DbFile = "data.dat";
            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);

            try
            {
                if (!System.IO.File.Exists(DbFile))
                {
                    SQLiteConnection.CreateFile(DbFile);  // SQLite DB 생성
                }


                // 테이블 생성 코드
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString, true);
                sqliteConn.Open();

                string strsql = "CREATE TABLE IF NOT EXISTS regist (등록 TEXT, 아이디 TEXT,매매구분 TEXT, 게임명 TEXT,서버명 TEXT,물품종류 TEXT,일반분할흥정 TEXT,제목 TEXT,내용 TEXT,가격 TEXT,거래수량 TEXT,거래수량단위 TEXT,최대수량 TEXT,최소수량 TEXT,분할단위 TEXT,즉시구매 TEXT,전달서버 TEXT,캐릭터명 TEXT,기준가격 TEXT,최저가격체크 TEXT,최저가격 TEXT,이미지 TEXT)";
                string strsql2 = "CREATE TABLE IF NOT EXISTS account (ID TEXT,PWD TEXT, PRIMARY KEY (ID))";
                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                cmd.ExecuteNonQuery();
                SQLiteCommand cmd2 = new SQLiteCommand(strsql2, sqliteConn);
                cmd2.ExecuteNonQuery();
                sqliteConn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            updateTable.Columns.Add(new DataColumn("등록", typeof(string)));
            updateTable.Columns.Add(new DataColumn("아이디", typeof(string)));
            updateTable.Columns.Add(new DataColumn("매매구분", typeof(string)));
            updateTable.Columns.Add(new DataColumn("게임명", typeof(string)));
            updateTable.Columns.Add(new DataColumn("서버명", typeof(string)));
            updateTable.Columns.Add(new DataColumn("물품종류", typeof(string)));
            updateTable.Columns.Add(new DataColumn("일반분할흥정", typeof(string)));
            updateTable.Columns.Add(new DataColumn("제목", typeof(string)));
            updateTable.Columns.Add(new DataColumn("내용", typeof(string)));
            updateTable.Columns.Add(new DataColumn("가격", typeof(string)));
            updateTable.Columns.Add(new DataColumn("거래수량", typeof(string)));
            updateTable.Columns.Add(new DataColumn("거래수량단위", typeof(string)));
            updateTable.Columns.Add(new DataColumn("최대수량", typeof(string)));
            updateTable.Columns.Add(new DataColumn("최소수량", typeof(string)));
            updateTable.Columns.Add(new DataColumn("분할단위", typeof(string)));
            updateTable.Columns.Add(new DataColumn("즉시구매", typeof(string)));
            updateTable.Columns.Add(new DataColumn("전달서버", typeof(string)));
            updateTable.Columns.Add(new DataColumn("캐릭터명", typeof(string)));
            updateTable.Columns.Add(new DataColumn("기준가격", typeof(string)));
            updateTable.Columns.Add(new DataColumn("최저가격체크", typeof(string)));
            updateTable.Columns.Add(new DataColumn("최저가격", typeof(string)));
            updateTable.Columns.Add(new DataColumn("이미지", typeof(string)));            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strsql = "SELECT 등록,아이디,매매구분,게임명,서버명,제목,물품종류,가격,최대수량,분할단위,최소수량 FROM regist";

                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        ListViewItem lvi = new ListViewItem(reader["등록"].ToString());
                        lvi.SubItems.Add(reader["아이디"].ToString());
                        lvi.SubItems.Add(reader["매매구분"].ToString());
                        lvi.SubItems.Add(reader["게임명"].ToString());
                        lvi.SubItems.Add(reader["서버명"].ToString());
                        lvi.SubItems.Add(reader["제목"].ToString());
                        lvi.SubItems.Add(reader["물품종류"].ToString());
                        lvi.SubItems.Add(reader["가격"].ToString());
                        lvi.SubItems.Add(reader["최대수량"].ToString());
                        lvi.SubItems.Add(reader["분할단위"].ToString());
                        lvi.SubItems.Add(reader["최소수량"].ToString());

                        listView1.Items.Add(lvi);
                        if (listView1.Items[i].Text == "체크")
                        {
                            listView1.Items[i].Checked = true;
                        }
                        i++;
                    }
                }

                reader.Close();
                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            listviewcount = listView1.Items.Count;
            label1.Text = "물품개수 : " +listView1.Items.Count.ToString() + "개";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form2"] is Form2 form2)
            {
                form2.Focus();
                return;
            }
            form2 = new Form2();
            form2.ShowDialog();
        }

        private void AddProduct_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form4"] is Form4 form4)
            {
                form4.Focus();
                return;
            }
            form4 = new Form4();
            form4.FormSendEvent += new Form4.FormSendDataHandler(ItemAddEventMethod);
            form4.ShowDialog();
        }

        private void ItemAddEventMethod(DataTable sender)
        {
            int j = 0;
            foreach (DataRow dr in sender.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = dr[0].ToString();
                for (int i = 1; i < sender.Columns.Count; i++)
                {
                    lvi.SubItems.Add(dr[i].ToString());
                }
                listView1.Items.Add(lvi);
                if (listView1.Items[j].Text == "체크")
                    listView1.Items[j].Checked = true;
                else
                    listView1.Items[j].Checked = false;
                j++;
            }
            label1.Text = "물품개수 : " + listView1.Items.Count.ToString() + "개";
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

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }


        private void listView1_Click(object sender, EventArgs e)
        {
            Point mousePos = listView1.PointToClient(Control.MousePosition);

            ListViewHitTestInfo hitTest = listView1.HitTest(mousePos);
            if (hitTest.Item.SubItems.IndexOf(hitTest.SubItem) == 0)
            {
                if (listView1.Items[hitTest.Item.Index].Checked == false)
                {
                    try
                    {
                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql2 = "UPDATE regist SET 등록='체크' where rowid IN (SELECT rowid FROM regist LIMIT " + hitTest.Item.Index + ",1)";
                        SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                        cmd.ExecuteNonQuery();
                        sqliteConn.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                else
                {
                    try
                    {
                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql2 = "UPDATE regist SET 등록='' where rowid IN (SELECT rowid FROM regist LIMIT " + hitTest.Item.Index + ",1)";
                        SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                        cmd.ExecuteNonQuery();
                        sqliteConn.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            /*
            int index = listView1.SelectedItems.Count;
            int i = 0;
            string check = "";
            while (index > i)
            {
                if (listView1.Items[i].Checked == true)
                    check = "체크";
                else
                    check = "";

                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 등록='" + check + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }*/
        }
        private void Button2_Click(object sender, EventArgs e) //삭제
        {
            if(MessageBox.Show(listView1.SelectedItems.Count.ToString() + "개의 물품을 삭제하시겠습니까?","삭제",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                int index = listView1.SelectedItems.Count;
                int i = 0;
                while (index > i)
                {
                    Remove(i);
                    i++;
                }
                label1.Text = "물품개수 : " + listView1.Items.Count.ToString() + "개";
            }
        }

        private void Remove(int index)
        {

            try
            {
                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();


                string strsql2 = "DELETE FROM regist where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[0] + ",1)";
                SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                cmd.ExecuteNonQuery();
                sqliteConn.Close();


                ListViewItem lvi = listView1.SelectedItems[0];
                listView1.Items.Remove(lvi);


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strsql = "SELECT 아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지 FROM regist LIMIT " + listView1.SelectedIndices[0] + ",1";

                SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                updateTable.Load(reader);

                reader.Close();
                sqliteConn.Close();
                updateNum = listView1.SelectedIndices[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (Application.OpenForms["Form4"] is Form4 form4)
            {
                form4.Focus();
                return;
            }
            form4 = new Form4();
            form4.FormSendEvent += new Form4.FormSendDataHandler(ItemUpdateEventMethod);
            form4.ShowDialog();

            Debug.WriteLine(listView1.SelectedItems[0].Text);
            if (listView1.SelectedItems[0].Text == "체크")
                listView1.Items[0].Checked = true;
            else
                listView1.Items[0].Checked = false;
        }
        private void ItemUpdateEventMethod(DataTable sender)
        {
            foreach (DataRow dr in sender.Rows)
            {
                for (int i = 1; i < sender.Columns.Count; i++)
                {
                    listView1.SelectedItems[0].SubItems[i].Text = dr[i].ToString();
                }
            }
        }

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right)) //우클릭
            {
                ContextMenu m = new ContextMenu();
                MenuItem m0 = new MenuItem();
                MenuItem m1 = new MenuItem();
                MenuItem m2 = new MenuItem();
                MenuItem m3 = new MenuItem();
                MenuItem m4 = new MenuItem();
                MenuItem m5 = new MenuItem();
                MenuItem m6 = new MenuItem();
                MenuItem m7 = new MenuItem();
                MenuItem m8 = new MenuItem();
                MenuItem m9 = new MenuItem();
                MenuItem m10 = new MenuItem();
                MenuItem m11 = new MenuItem();
                MenuItem m12 = new MenuItem();

                m0.Text = "등록계정 변경";
                m1.Text = "제목 변경";
                m2.Text = "내용 변경";
                m3.Text = "수량 변경";
                m4.Text = "최대 수량 변경";
                m5.Text = "최소 수량 변경";
                m6.Text = "가격 변경";
                m7.Text = "가격 +100원(Ctrl+숫자패드8)";
                m8.Text = "가격 -100원(Ctrl+숫자패드2)";
                m9.Text = "가격 +10원(숫자패드8)";
                m10.Text = "가격 -10원(숫자패드2)";
                m11.Text = "체크";
                m12.Text = "체크해제";


                m.MenuItems.Add(m0);
                m.MenuItems.Add(m1);
                m.MenuItems.Add(m2);
                m.MenuItems.Add(m3);
                m.MenuItems.Add(m4);
                m.MenuItems.Add(m5);
                m.MenuItems.Add(m6);
                m.MenuItems.Add(m7);
                m.MenuItems.Add(m8);
                m.MenuItems.Add(m9);
                m.MenuItems.Add(m10);
                m.MenuItems.Add(m11);
                m.MenuItems.Add(m12);

                m0.Click += (senders, es) =>
                {
                    if (Application.OpenForms["account"] is UpdateAccount account)
                    {
                        account.Focus();
                        return;
                    }
                    account = new UpdateAccount();
                    account.FormSendEvent += new UpdateAccount.FormSendDataHandler(AccountUpdate);
                    account.ShowDialog();
                };
                m1.Click += (senders, es) =>
                {
                    if (Application.OpenForms["title"] is UpdateTitle title)
                    {
                        title.Focus();
                        return;
                    }
                    title = new UpdateTitle();
                    title.FormSendEvent += new UpdateTitle.FormSendDataHandler(TitleUpdate);
                    title.ShowDialog();
                };
                m2.Click += (senders, es) =>
                {
                    if (Application.OpenForms["text"] is UpdateText text)
                    {
                        text.Focus();
                        return;
                    }
                    text = new UpdateText();
                    text.FormSendEvent += new UpdateText.FormSendDataHandler(TextUpdate);
                    text.ShowDialog();
                };
                m3.Click += (senders, es) =>
                {
                    if (Application.OpenForms["nomalquantity"] is UpdateNomalQuantity nomalquantity)
                    {
                        nomalquantity.Focus();
                        return;
                    }
                    nomalquantity = new UpdateNomalQuantity();
                    nomalquantity.FormSendEvent += new UpdateNomalQuantity.FormSendDataHandler(NomalQuantityUpdate);
                    nomalquantity.ShowDialog();
                };
                m4.Click += (senders, es) =>
                {
                    if (Application.OpenForms["quantityMax"] is UpdateQantityMax quantityMax)
                    {
                        quantityMax.Focus();
                        return;
                    }
                    quantityMax = new UpdateQantityMax();
                    quantityMax.FormSendEvent += new UpdateQantityMax.FormSendDataHandler(QuantityMaxUpdate);
                    quantityMax.ShowDialog();
                };
                m5.Click += (senders, es) =>
                {
                    if (Application.OpenForms["quantityMin"] is UpdateQuantityMin quantityMin)
                    {
                        quantityMin.Focus();
                        return;
                    }
                    quantityMin = new UpdateQuantityMin();
                    quantityMin.FormSendEvent += new UpdateQuantityMin.FormSendDataHandler(QuantityMinUpdate);
                    quantityMin.ShowDialog();
                };
                m6.Click += (senders, es) =>
                {
                    if (Application.OpenForms["price"] is UpdatePrice price)
                    {
                        price.Focus();
                        return;
                    }
                    price = new UpdatePrice();
                    price.FormSendEvent += new UpdatePrice.FormSendDataHandler(PriceUpdate);
                    price.ShowDialog();
                };
                m7.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + 100;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }

                };
                m8.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) - 100;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                };
                m9.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + 10;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                };
                m10.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {
                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) - 10;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                };

                m11.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;

                    while (index > i)
                    {
                        try
                        {
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 등록='체크' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].Checked = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                };
                m12.Click += (senders, es) =>
                {
                    int index = listView1.SelectedItems.Count;
                    int i = 0;

                    while (index > i)
                    {
                        try
                        {
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 등록='' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].Checked = false;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                };
                m.Show(listView1, new Point(e.X, e.Y));

            }
        }
        private void AccountUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 아이디='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                    listView1.SelectedItems[i].SubItems[1].Text = sender.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void TitleUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 제목='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                    listView1.SelectedItems[i].SubItems[5].Text = sender.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }

        private void TextUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 내용='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void NomalQuantityUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 거래수량='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void QuantityMaxUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 최대수량='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                    listView1.SelectedItems[i].SubItems[8].Text = sender.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void QuantityMinUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 최소수량='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                    listView1.SelectedItems[i].SubItems[10].Text = sender.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void PriceUpdate(object sender)
        {
            int index = listView1.SelectedItems.Count;
            int i = 0;
            while (index > i)
            {
                try
                {
                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strsql2 = "UPDATE regist SET 가격='" + sender.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                    SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                    cmd.ExecuteNonQuery();
                    sqliteConn.Close();
                    listView1.SelectedItems[i].SubItems[7].Text = sender.ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                i++;
            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("[" + DateTime.Now + "]" + " 자동 등록이 시작되었습니다.");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            LogWrite(" 자동 등록이 시작되었습니다.");
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            addProduct.Enabled = false;
            
            regist = new Thread(Regist);
            regist.Start();

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            regist.Abort();
            deleteLoop.Abort();
            listBox1.Items.Add("[" + DateTime.Now + "]" + " 자동 등록이 정지되었습니다.");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            LogWrite(" 자동 등록이 정지되었습니다.");
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            addProduct.Enabled = true;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Thread money = new Thread(Money);
            money.Start();
        }

        static void Regist()
        {
            deleteLoop = new Thread(DeleteLoop);
            deleteLoop.Start();
            int j = 0;//REGIST.ini파일에 있는 아이디 가져오기
            int q = 0;//Form2의 아이디목록 카운트

            DataTable registTable = new DataTable();
            try
            {

                string DbFile = "data.dat";
                string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                sqliteConn.Open();
                string strqry = "SELECT * FROM regist";
                SQLiteCommand cmd = new SQLiteCommand(strqry, sqliteConn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                registTable.Load(reader);

                reader.Close();
                sqliteConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

            q = accountTable.Rows.Count;

            string[] IDlist = new string[q];
            string[] PWDlist = new string[q];

            int IDinsert = 0;

            foreach (DataRow dr in accountTable.Rows)
            {

                IDlist[IDinsert] = dr["ID"].ToString();
                PWDlist[IDinsert] = dr["PWD"].ToString();
                IDinsert++;

            }

            int w = registTable.Rows.Count;


            string[] b = new string[w];
            string[] c = new string[w];

            int Distinct = 0;
            foreach (DataRow dr in registTable.Rows)
            {
                b[Distinct] = dr["아이디"].ToString();
                b[Distinct] = b[Distinct] + dr["매매구분"];
                b[Distinct] = b[Distinct] + dr["게임명"];
                b[Distinct] = b[Distinct] + dr["서버명"];
                Distinct++;
            }

            if (RegistID == null)
            {
                RegistID = new string[w];
                for (int IDdefault = 0; IDdefault < w; IDdefault++)
                {
                    RegistID[IDdefault] = "없음";
                }
            }

            c = b.Distinct().ToArray();

            //배열 b에 모든물품의 "아이디매매구분게임명서버명"을 가져와서 중복제거한것을 배열c에 넣고 등록그룹을 구분
            //2차원 배열 d에 등록구분별로 400개씩 배열할당
            //dlength로 등록구분별로 몇번째 물품인지 카운트 dlength[등록그룹,0]에는 등록그룹별 글 총갯수 dlength[등록그룹,1]에는 1씩 증가하며
            //dlength[등록그룹,0]과dlength[등록그룹,1]이 같아지면 dlength[등록그룹,1]을 0으로 초기화
            string[] stopPoint = new string[c.Length];

            string[][] d = new string[c.Length][];
            int[,] dlength = new int[c.Length, 2];
            for (int t = 0; t < c.Length; t++)
            {
                d[t] = new string[400];
                for (int tt = 0; tt < w; tt++)
                {
                    if (c[t] == b[tt])
                    {
                        int ttt = 0;
                        while (d[t][ttt] != null)
                        {
                            ttt++;
                        }
                        d[t][ttt] = tt.ToString();
                    }
                }
                d[t] = d[t].Distinct().ToArray();
                dlength[t, 0] = d[t].Length - 1;
                dlength[t, 1] = 0;
                stopPoint[t] = "0";
            }

            while (true)
            {
                for (int qqq = 0; qqq < c.Length; qqq++)
                {
                    string[] stopPoint2 = new string[2];

                    stopPoint2 = stopPoint.Distinct().ToArray();

                    if (DateTime.Now.ToString("mm") == "00")
                    {
                        for (int q1 = 0; q1 < stopPoint.Length; q1++)
                        {
                            stopPoint[q1] = "0";
                        }
                    }

                    if (stopPoint2[0] == "1" && stopPoint2.Length == 1)
                    {
                        Delay((60 - Int32.Parse(DateTime.Now.ToString("mm"))) * 60000);
                        for (int q1 = 0; q1 < stopPoint.Length; q1++)
                        {
                            stopPoint[q1] = "0";
                        }
                    }

                    if (stopPoint[qqq] == "1")
                    {
                        continue;
                    }
                    if (dlength[qqq, 0] == dlength[qqq, 1])
                    {
                        dlength[qqq, 1] = 0;
                        j = Convert.ToInt16(d[qqq][dlength[qqq, 1]]);
                        dlength[qqq, 1] = dlength[qqq, 1] + 1;
                    }
                    else
                    {
                        j = Convert.ToInt16(d[qqq][dlength[qqq, 1]]);
                        dlength[qqq, 1] = dlength[qqq, 1] + 1;
                    }

                    if (registTable.Rows[j]["등록"].ToString() == "")
                    {
                        stopPoint[qqq] = "1";
                        continue;
                    }

                    string nowID = registTable.Rows[j]["아이디"].ToString();

                    int k = 0;//Form2의 아이디목록과 REGIST.ini파일의 아이디 비교
                    while (IDlist[k] != nowID)
                    {
                        k++;
                    }

                    string sellorbuy = registTable.Rows[j]["매매구분"].ToString();

                    if (RegistID[j] == "없음")
                    {
                        try
                        {
                            if (sellorbuy == "판매")
                            {
                                string sendData = "user_id=" + IDlist[k] + "&user_password=" + PWDlist[k];
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                                req.Method = "POST";
                                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                req.CookieContainer = cookie;

                                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                                writer.Write(sendData);
                                writer.Close();

                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                req.Abort();


                                string game_code_text = registTable.Rows[j]["게임명"].ToString();
                                string game_code = Game_code(game_code_text);
                                string server_code_text = registTable.Rows[j]["서버명"].ToString();
                                string server_code = Server_code(game_code, server_code_text);
                                string user_goods = registTable.Rows[j]["물품종류"].ToString();
                                string user_goods_type = registTable.Rows[j]["일반분할흥정"].ToString();
                                string user_price = registTable.Rows[j]["가격"].ToString();
                                string user_quantity = registTable.Rows[j]["거래수량"].ToString();
                                string gamemoney_unit = registTable.Rows[j]["거래수량단위"].ToString();
                                string user_quantity_min = registTable.Rows[j]["최소수량"].ToString();
                                string user_quantity_max = registTable.Rows[j]["최대수량"].ToString();
                                string user_division_unit = registTable.Rows[j]["분할단위"].ToString();
                                string user_division_price = registTable.Rows[j]["기준가격"].ToString();
                                string chk_user_deny_use = registTable.Rows[j]["최저가격체크"].ToString();
                                string user_price_limit = registTable.Rows[j]["최저가격"].ToString();
                                string df_server_code_text = registTable.Rows[j]["전달서버"].ToString();
                                string df_server_code = Server_code(game_code, df_server_code_text);
                                string user_character = registTable.Rows[j]["캐릭터명"].ToString();
                                string user_title = registTable.Rows[j]["제목"].ToString();
                                string user_text = registTable.Rows[j]["내용"].ToString();
                                string user_screen = registTable.Rows[j]["이미지"].ToString();
                                string user_cell_check = "on";
                                string security_service_userinfo = "N";
                                string security_type = "none";
                                string user_premium_use = "0";
                                string user_sms = "1";

                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/sell/");
                                req2.Method = "GET";
                                req2.ContentType = "application/x-www-form-urlencoded";
                                req2.CookieContainer = cookie;
                                req2.CookieContainer.Add(resp.Cookies);
                                resp.Close();

                                HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                                req2.Abort();

                                if (user_screen == "")
                                {
                                    sendData = "user_goods_type=" + user_goods_type + "&user_sms=" + user_sms + "&game_code=" + game_code + "&game_code_text=" + game_code_text +
                                    "&server_code=" + server_code + "&server_code_text=" + server_code_text + "&df_server_code=" + df_server_code + "&df_server_code_text=" +
                                    df_server_code_text + "&user_price=" + user_price + "&user_goods=" + user_goods + "&user_quantity=" + user_quantity + "&gamemoney_unit=" +
                                    gamemoney_unit + "&user_division_unit=" + user_division_unit + "&user_division_price=" + user_division_price + "&user_quantity_min=" +
                                    user_quantity_min + "&user_quantity_max=" + user_quantity_max + "&user_title=" + user_title + "&user_character=" + user_character +
                                    "&user_text=" + user_text + "&user_cell_check=" + user_cell_check + "&security_service_userinfo=" + security_service_userinfo +
                                    "&security_type=" + security_type + "&user_premium_use=" + user_premium_use;


                                    HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/sell/index_ok.php");
                                    req3.Method = "POST";
                                    req3.ContentType = "application/x-www-form-urlencoded";
                                    req3.Referer = "http://trade.itemmania.com/sell/";
                                    req3.CookieContainer = cookie;
                                    req3.CookieContainer.Add(resp2.Cookies);
                                    resp2.Close();
                                    StreamWriter writer3 = new StreamWriter(req3.GetRequestStream());
                                    writer3.Write(sendData);
                                    writer3.Close();

                                    HttpWebResponse result = (HttpWebResponse)req3.GetResponse();
                                    Encoding encode2 = Encoding.GetEncoding("utf-8");
                                    Stream strReceiveStream2 = result.GetResponseStream();
                                    StreamReader reqStreamReader2 = new StreamReader(strReceiveStream2, encode2);
                                    String strResult2 = reqStreamReader2.ReadToEnd();
                                    strReceiveStream2.Close();
                                    reqStreamReader2.Close();
                                    req3.Abort();
                                    result.Close();

                                    int id_index1 = strResult2.IndexOf("value=") + 7;
                                    int id_index2 = strResult2.IndexOf("\"></form>");

                                    try
                                    {
                                        byte[] bytetest = Convert.FromBase64String(strResult2.Substring(id_index1, id_index2 - id_index1));
                                        string param = Encoding.UTF8.GetString(bytetest);

                                        RegistID[j] = param.Substring(param.IndexOf("trade_id") + 16, (param.IndexOf("type") - 7) - (param.IndexOf("trade_id") + 16));


                                        listBox1.Invoke(new MethodInvoker(delegate ()//크로스 스레드 예외 처리
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                            param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                            param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));

                                        LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                            param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                            param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");

                                    }
                                    catch (Exception ex)
                                    {

                                        if (strResult2.Contains("물품 개수가 초과"))
                                        {
                                            stopPoint[qqq] = "1";
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                        }
                                        else if (strResult2.Contains("해당서버에 아직"))
                                        {
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                        }
                                        else if (strResult2.Contains("물품의 수가 초과"))
                                        {
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                        }
                                        else if (strResult2.Contains("로그인 후 이용해주세요"))
                                        {
                                            dlength[qqq, 1]--;
                                            qqq--;
                                            Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                        }
                                        else if (strResult2.Contains("등록에 실패"))
                                        {
                                            dlength[qqq, 1]--;
                                            qqq--;
                                            Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                        }
                                        else
                                        {
                                            Debug.WriteLine(ex.Message);
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        }
                                    }



                                }
                                else
                                {
                                    NameValueCollection isendData = new NameValueCollection();
                                    isendData.Add("user_goods_type", user_goods_type);
                                    isendData.Add("user_sms", user_sms);
                                    isendData.Add("game_code", game_code);
                                    isendData.Add("game_code_text", game_code_text);
                                    isendData.Add("server_code", server_code);
                                    isendData.Add("server_code_text", server_code_text);
                                    isendData.Add("df_server_code", df_server_code);
                                    isendData.Add("df_server_code_text", df_server_code_text);
                                    isendData.Add("user_price", user_price);
                                    isendData.Add("user_goods", user_goods);
                                    isendData.Add("user_quantity", user_quantity);
                                    isendData.Add("gamemoney_unit", gamemoney_unit);
                                    isendData.Add("user_division_unit", user_division_unit);
                                    isendData.Add("user_division_price", user_division_price);
                                    isendData.Add("user_quantity_min", user_quantity_min);
                                    isendData.Add("user_quantity_max", user_quantity_max);
                                    isendData.Add("user_title", user_title);
                                    isendData.Add("user_character", user_character);
                                    isendData.Add("user_text", user_text);
                                    isendData.Add("user_cell_check", user_cell_check);
                                    isendData.Add("security_service_userinfo", security_service_userinfo);
                                    isendData.Add("security_type", security_type);
                                    isendData.Add("user_premium_use", user_premium_use);

                                    string[] image = user_screen.Split('|');

                                    HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/sell/index_ok.php");
                                    req3.Method = "POST";
                                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                                    byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                                    byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                                    req3.ContentType = "multipart/form-data; boundary=" + boundary;
                                    req3.Referer = "http://trade.itemmania.com/sell/";
                                    req3.CookieContainer = cookie;
                                    req3.CookieContainer.Add(resp2.Cookies);
                                    resp2.Close();

                                    Stream requestStream = req3.GetRequestStream();


                                    foreach (string key in isendData.Keys)
                                    {
                                        byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, isendData[key]));
                                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                                        requestStream.Write(formItemBytes, 0, formItemBytes.Length);
                                    }
                                    try
                                    {

                                        foreach (string imageString in image)
                                        {
                                            if (File.Exists(imageString))
                                            {
                                                int bytesRead = 0;
                                                byte[] buffer = new byte[2048];
                                                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: image/jpeg\r\n\r\n", "user_screen[]", imageString));
                                                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                                                requestStream.Write(formItemBytes, 0, formItemBytes.Length);

                                                using (FileStream fileStream = new FileStream(imageString, FileMode.Open, FileAccess.Read))
                                                {
                                                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                                    {
                                                        // Write file content to stream, byte by byte
                                                        requestStream.Write(buffer, 0, bytesRead);
                                                    }

                                                    fileStream.Close();
                                                }
                                            }
                                        }
                                    }

                                    catch { }


                                    requestStream.Write(trailer, 0, trailer.Length);
                                    requestStream.Close();



                                    HttpWebResponse result = (HttpWebResponse)req3.GetResponse();

                                    Encoding encode2 = Encoding.GetEncoding("utf-8");
                                    Stream strReceiveStream2 = result.GetResponseStream();
                                    StreamReader reqStreamReader2 = new StreamReader(strReceiveStream2, encode2);
                                    string strResult2 = reqStreamReader2.ReadToEnd();
                                    strReceiveStream2.Close();
                                    reqStreamReader2.Close();
                                    req3.Abort();
                                    result.Close();

                                    int id_index1 = strResult2.IndexOf("value=") + 7;
                                    int id_index2 = strResult2.IndexOf("\"></form>");

                                    try
                                    {
                                        byte[] bytetest = Convert.FromBase64String(strResult2.Substring(id_index1, id_index2 - id_index1));
                                        string param = Encoding.UTF8.GetString(bytetest);

                                        RegistID[j] = param.Substring(param.IndexOf("trade_id") + 16, (param.IndexOf("type") - 7) - (param.IndexOf("trade_id") + 16));


                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                            param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                            param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));

                                        LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                            param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                            param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");

                                    }
                                    catch (Exception ex)
                                    {

                                        if (strResult2.Contains("물품 개수가 초과"))
                                        {
                                            stopPoint[qqq] = "1";
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                        }
                                        else if (strResult2.Contains("해당서버에 아직"))
                                        {
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                        }
                                        else if (strResult2.Contains("물품의 수가 초과"))
                                        {
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                        }
                                        else if (strResult2.Contains("로그인 후 이용해주세요"))
                                        {
                                            dlength[qqq, 1]--;
                                            qqq--;
                                            Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                        }
                                        else if (strResult2.Contains("등록에 실패"))
                                        {
                                            dlength[qqq, 1]--;
                                            qqq--;
                                            Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                        }
                                        else
                                        {
                                            Debug.WriteLine(ex.Message);
                                            listBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                            }));
                                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        }
                                    }
                                }
                                Debug.WriteLine(server_code_text);
                                Debug.WriteLine(user_title);
                                HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                                req4.Method = "GET";
                                req4.CookieContainer = cookie;
                                HttpWebResponse response = (HttpWebResponse)req4.GetResponse();
                                Stream stReadData = response.GetResponseStream();
                                req4.Abort();
                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            string game_code_text = registTable.Rows[j]["게임명"].ToString();
                            string server_code_text = registTable.Rows[j]["서버명"].ToString();
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                        }


                        try
                        {
                            if (sellorbuy == "구매")
                            {
                                string sendData = "user_id=" + IDlist[k] + "&user_password=" + PWDlist[k];
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                                req.Method = "POST";
                                req.ContentLength = sendData.Length;
                                req.ContentType = "application/x-www-form-urlencoded;";
                                req.CookieContainer = cookie;
                                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                                writer.Write(sendData);
                                writer.Close();

                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                req.Abort();

                                string game_code_text = registTable.Rows[j]["게임명"].ToString();
                                string game_code = Game_code(game_code_text);
                                string server_code_text = registTable.Rows[j]["서버명"].ToString();
                                string server_code = Server_code(game_code, server_code_text);
                                string user_goods = registTable.Rows[j]["물품종류"].ToString();
                                string user_goods_type = registTable.Rows[j]["일반분할흥정"].ToString();
                                string user_price = registTable.Rows[j]["가격"].ToString();
                                string user_quantity = registTable.Rows[j]["거래수량"].ToString();
                                string gamemoney_unit = registTable.Rows[j]["거래수량단위"].ToString();
                                string user_quantity_min = registTable.Rows[j]["최소수량"].ToString();
                                string user_quantity_max = registTable.Rows[j]["최대수량"].ToString();
                                string user_division_unit = registTable.Rows[j]["분할단위"].ToString();
                                string user_division_price = registTable.Rows[j]["기준가격"].ToString();
                                string df_server_code_text = registTable.Rows[j]["전달서버"].ToString();
                                string df_server_code = Server_code(game_code, df_server_code_text);
                                string user_character = registTable.Rows[j]["캐릭터명"].ToString();
                                string user_title = registTable.Rows[j]["제목"].ToString();
                                string user_text = registTable.Rows[j]["내용"].ToString();
                                string direct_reg_trade = registTable.Rows[j]["즉시구매"].ToString();

                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/buy/");
                                req2.Method = "GET";
                                req2.ContentType = "application/x-www-form-urlencoded";
                                req2.CookieContainer = cookie;
                                req2.CookieContainer.Add(resp.Cookies);
                                resp.Close();
                                HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                                Encoding encode = Encoding.GetEncoding("utf-8");
                                Stream strReceiveStream = resp2.GetResponseStream();
                                StreamReader reqStreamReader = new StreamReader(strReceiveStream, encode);
                                String buyGet = reqStreamReader.ReadToEnd();
                                strReceiveStream.Close();
                                reqStreamReader.Close();
                                req2.Abort();

                                int index1 = buyGet.IndexOf("certify_pay\" value=") + 20;
                                int index2 = buyGet.IndexOf("<input type=\"hidden\" id=\"sise\" name=\"sise\" value=\"false\">") - 4;

                                string certify_pay = buyGet.Substring(index1, index2 - index1);

                                sendData = "user_goods_type=" + user_goods_type + "&game_code=" + game_code + "&game_code_text=" + game_code_text +
                                "&server_code=" + server_code + "&server_code_text=" + server_code_text + "&df_server_code=" + df_server_code + "&df_server_code_text=" +
                                df_server_code_text + "&user_goods=" + user_goods + "&user_quantity=" + user_quantity + "&gamemoney_unit=" + gamemoney_unit + "&user_division_unit=" +
                                user_division_unit + "&user_division_price=" + user_division_price + "&user_quantity_min=" + user_quantity_min + "&user_quantity_max=" + user_quantity_max +
                                "&user_title=" + user_title + "&user_price=" + user_price + "&user_character=" + user_character + "&user_text=" + user_text + "&direct_reg_trade=" + direct_reg_trade +
                                "&direct_condition_credit=1" + "&certify_pay=" + certify_pay + "&user_premium_use=0";

                                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/buy/index_ok.php");
                                req3.Method = "POST";
                                req3.ContentType = "application/x-www-form-urlencoded";
                                req3.Referer = "http://trade.itemmania.com/buy/";
                                req3.CookieContainer = cookie;
                                req3.CookieContainer.Add(resp2.Cookies);
                                resp2.Close();
                                StreamWriter writer3 = new StreamWriter(req3.GetRequestStream());
                                writer3.Write(sendData);
                                writer3.Close();

                                HttpWebResponse result = (HttpWebResponse)req3.GetResponse();
                                Encoding encode2 = Encoding.GetEncoding("utf-8");
                                Stream strReceiveStream2 = result.GetResponseStream();
                                StreamReader reqStreamReader2 = new StreamReader(strReceiveStream2, encode2);
                                String strResult2 = reqStreamReader2.ReadToEnd();
                                strReceiveStream2.Close();
                                reqStreamReader2.Close();
                                req3.Abort();
                                result.Close();

                                int id_index1 = strResult2.IndexOf("value=") + 7;
                                int id_index2 = strResult2.IndexOf("\"></form>");

                                try
                                {
                                    byte[] bytetest = Convert.FromBase64String(strResult2.Substring(id_index1, id_index2 - id_index1));

                                    string param = Encoding.UTF8.GetString(bytetest);

                                    RegistID[j] = param.Substring(param.IndexOf("trade_id") + 16, (param.IndexOf("type") - 7) - (param.IndexOf("trade_id") + 16));

                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                        param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                        param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));

                                    LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  등록 완료(ID:" +
                                        param.Substring(param.IndexOf("ID로") + 4, (param.IndexOf("IP에서") - 4) - (param.IndexOf("ID로") + 4)) + " & IP:" +
                                        param.Substring(param.IndexOf("IP에서") + 5, (param.IndexOf("추가등록") - 4) - (param.IndexOf("IP에서") + 5)) + ")");
                                }
                                catch (Exception ex)
                                {
                                    if (strResult2.Contains("정상적인 경로를 이용하세요"))
                                    {
                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 정상적인 경로를 이용하세요.[0x003]");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));
                                        LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 정상적인 경로를 이용하세요.[0x003]");
                                    }
                                    else if (strResult2.Contains("물품 개수가 초과"))
                                    {
                                        stopPoint[qqq] = "1";
                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));
                                        LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                    }
                                    else if (strResult2.Contains("해당서버에 아직"))
                                    {
                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));
                                        LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 해당서버에 아직 등록 하실 수 없습니다.\n\n잠시 후 다시 등록하시기 바랍니다.");
                                    }
                                    else if (strResult2.Contains("물품의 수가 초과"))
                                    {
                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));
                                        LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                    }
                                    else if (strResult2.Contains("로그인 후 이용해주세요"))
                                    {
                                        dlength[qqq, 1]--;
                                        qqq--;
                                        Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                    }
                                    else if (strResult2.Contains("등록에 실패"))
                                    {
                                        dlength[qqq, 1]--;
                                        qqq--;
                                        Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                    }
                                    else
                                    {
                                        Debug.WriteLine(ex.Message);
                                        listBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                        }));
                                        LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                    }
                                }


                                HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                                req4.Method = "GET";
                                req4.CookieContainer = cookie;
                                HttpWebResponse response = (HttpWebResponse)req4.GetResponse();
                                Stream stReadData = response.GetResponseStream();

                                req4.Abort();
                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            string game_code_text = registTable.Rows[j]["게임명"].ToString();
                            string server_code_text = registTable.Rows[j]["서버명"].ToString();
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");

                        }
                    }



                    else
                    {
                        try
                        {
                            if (sellorbuy == "판매")
                            {

                                string sendData = "user_id=" + IDlist[k] + "&user_password=" + PWDlist[k];
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                                req.Method = "POST";
                                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                req.CookieContainer = cookie;

                                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                                writer.Write(sendData);
                                writer.Close();

                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                req.Abort();

                                string game_code_text = registTable.Rows[j]["게임명"].ToString();
                                string game_code = Game_code(game_code_text);
                                string server_code_text = registTable.Rows[j]["서버명"].ToString();
                                string server_code = Server_code(game_code, server_code_text);
                                string user_goods = registTable.Rows[j]["물품종류"].ToString();
                                string user_goods_type = registTable.Rows[j]["일반분할흥정"].ToString();
                                string user_price = registTable.Rows[j]["가격"].ToString();
                                string user_quantity = registTable.Rows[j]["거래수량"].ToString();
                                string gamemoney_unit = registTable.Rows[j]["거래수량단위"].ToString();
                                string user_quantity_min = registTable.Rows[j]["최소수량"].ToString();
                                string user_quantity_max = registTable.Rows[j]["최대수량"].ToString();
                                string user_division_unit = registTable.Rows[j]["분할단위"].ToString();
                                string user_division_price = registTable.Rows[j]["기준가격"].ToString();
                                string chk_user_deny_use = registTable.Rows[j]["최저가격체크"].ToString();
                                string user_price_limit = registTable.Rows[j]["최저가격"].ToString();
                                string df_server_code_text = registTable.Rows[j]["전달서버"].ToString();
                                string df_server_code = Server_code(game_code, df_server_code_text);
                                string user_character = registTable.Rows[j]["캐릭터명"].ToString();
                                string user_title = registTable.Rows[j]["제목"].ToString();
                                string user_text = registTable.Rows[j]["내용"].ToString();
                                string user_screen = registTable.Rows[j]["이미지"].ToString();
                                string user_cell_check = "on";
                                string security_service_userinfo = "N";
                                string security_type = "none";
                                string user_premium_use = "0";
                                string user_sms = "1";

                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_re_reg.html?id=" + RegistID[j]);
                                req2.Method = "GET";
                                req2.ContentType = "application/x-www-form-urlencoded";
                                req2.CookieContainer = cookie;
                                req2.CookieContainer.Add(resp.Cookies);
                                resp.Close();

                                HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                                req2.Abort();

                                sendData = "user_goods_type=" + user_goods_type + "&user_sms=" + user_sms + "&game_code=" + game_code + "&game_code_text=" + game_code_text +
                                "&server_code=" + server_code + "&server_code_text=" + server_code_text + "&df_server_code=" + df_server_code + "&df_server_code_text=" +
                            df_server_code_text + "&user_price=" + user_price + "&user_goods=" + user_goods + "&user_quantity=" + user_quantity + "&gamemoney_unit=" +
                            gamemoney_unit + "&user_division_unit=" + user_division_unit + "&user_division_price=" + user_division_price + "&user_quantity_min=" +
                            user_quantity_min + "&user_quantity_max=" + user_quantity_max + "&user_title=" + user_title + "&user_character=" + user_character +
                            "&user_text=" + user_text + "&user_cell_check=" + user_cell_check + "&security_service_userinfo=" + security_service_userinfo +
                            "&security_type=" + security_type + "&user_premium_use=" + user_premium_use + "&id=" + RegistID[j];


                                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_re_reg_ok.php");
                                req3.Method = "POST";
                                req3.ContentType = "application/x-www-form-urlencoded";
                                req3.Referer = "http://trade.itemmania.com/sell/sell_re_reg.html?id=" + RegistID[j];
                                req3.CookieContainer = cookie;
                                req3.CookieContainer.Add(resp2.Cookies);
                                resp2.Close();

                                StreamWriter writer3 = new StreamWriter(req3.GetRequestStream());
                                writer3.Write(sendData);
                                writer3.Close();
                                HttpWebResponse result = (HttpWebResponse)req3.GetResponse();
                                Encoding encode2 = Encoding.GetEncoding("utf-8");
                                Stream strReceiveStream2 = result.GetResponseStream();
                                StreamReader reqStreamReader2 = new StreamReader(strReceiveStream2, encode2);
                                String strResult2 = reqStreamReader2.ReadToEnd();
                                strReceiveStream2.Close();
                                reqStreamReader2.Close();
                                req3.Abort();
                                if (strResult2.Contains("물품 개수가 초과"))
                                {
                                    result.Close();
                                    stopPoint[qqq] = "1";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                }
                                else if (strResult2.Contains("해당서버에 아직"))
                                {
                                    dlength[qqq, 1]--;
                                    qqq--;
                                }
                                else if (strResult2.Contains("물품이 정상적으로"))
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  재등록 완료(ID:" + strResult2.Substring(strResult2.IndexOf("해당 ID로") + 7, strResult2.IndexOf(" / IP에서") - (strResult2.IndexOf("해당 ID로") + 8)) + " & IP:" + strResult2.Substring(strResult2.IndexOf("IP에서") + 5, strResult2.IndexOf("개까지 추가등록") - (strResult2.IndexOf("IP에서") + 5)) + ")");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  재등록 완료(ID:" + strResult2.Substring(strResult2.IndexOf("해당 ID로") + 7, strResult2.IndexOf(" / IP에서") - (strResult2.IndexOf("해당 ID로") + 8)) + " & IP:" + strResult2.Substring(strResult2.IndexOf("IP에서") + 5, strResult2.IndexOf("개까지 추가등록") - (strResult2.IndexOf("IP에서") + 5)) + ")");

                                    HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.html?strRelationType=regist");
                                    req4.Method = "GET";
                                    req4.CookieContainer = cookie;
                                    req4.CookieContainer.Add(result.Cookies);
                                    result.Close();

                                    HttpWebResponse response2 = (HttpWebResponse)req4.GetResponse();
                                    Stream stReadData3 = response2.GetResponseStream();
                                    StreamReader srReadData3 = new StreamReader(stReadData3, encode2);
                                    string strResult3 = srReadData3.ReadToEnd();
                                    stReadData3.Close();
                                    req4.Abort();
                                    response2.Close();

                                    RegistID[j] = strResult3.Substring(strResult3.IndexOf("check[]\" value=") + 16, strResult3.IndexOf("\" style=\"border") - (strResult3.IndexOf("check[]\" value=") + 16));


                                }
                                else if (strResult2.Contains("로그인 후 이용해주세요"))
                                {
                                    result.Close();
                                    dlength[qqq, 1]--;
                                    qqq--;
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("/myroom/sell/sell_regist.html?strRelationType=regist"))
                                {
                                    result.Close();
                                    RegistID[j] = "없음";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("등록에 실패"))
                                {
                                    result.Close();
                                    dlength[qqq, 1]--;
                                    qqq--;
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("물품의 수가 초과"))
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                }
                                else
                                {
                                    result.Close();
                                    RegistID[j] = "없음";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }

                                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                                req5.Method = "GET";
                                req5.CookieContainer = cookie;
                                HttpWebResponse response = (HttpWebResponse)req5.GetResponse();
                                Stream stReadData = response.GetResponseStream();
                                req5.Abort();
                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {

                            string game_code_text = registTable.Rows[j]["게임명"].ToString();
                            string server_code_text = registTable.Rows[j]["서버명"].ToString();
                            Debug.WriteLine(ex.Message);
                            RegistID[j] = "없음";
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");

                        }


                        try
                        {
                            if (sellorbuy == "구매")
                            {
                                string sendData = "user_id=" + IDlist[k] + "&user_password=" + PWDlist[k];
                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                                req.Method = "POST";
                                req.ContentLength = sendData.Length;
                                req.ContentType = "application/x-www-form-urlencoded;";
                                req.CookieContainer = cookie;
                                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                                writer.Write(sendData);
                                writer.Close();

                                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                                req.Abort();
                                string game_code_text = registTable.Rows[j]["게임명"].ToString();
                                string game_code = Game_code(game_code_text);
                                string server_code_text = registTable.Rows[j]["서버명"].ToString();
                                string server_code = Server_code(game_code, server_code_text);
                                string user_goods = registTable.Rows[j]["물품종류"].ToString();
                                string user_goods_type = registTable.Rows[j]["일반분할흥정"].ToString();
                                string user_price = registTable.Rows[j]["가격"].ToString();
                                string user_quantity = registTable.Rows[j]["거래수량"].ToString();
                                string gamemoney_unit = registTable.Rows[j]["거래수량단위"].ToString();
                                string user_quantity_min = registTable.Rows[j]["최소수량"].ToString();
                                string user_quantity_max = registTable.Rows[j]["최대수량"].ToString();
                                string user_division_unit = registTable.Rows[j]["분할단위"].ToString();
                                string user_division_price = registTable.Rows[j]["기준가격"].ToString();
                                string df_server_code_text = registTable.Rows[j]["전달서버"].ToString();
                                string df_server_code = Server_code(game_code, df_server_code_text);
                                string user_character = registTable.Rows[j]["캐릭터명"].ToString();
                                string user_title = registTable.Rows[j]["제목"].ToString();
                                string user_text = registTable.Rows[j]["내용"].ToString();
                                string direct_reg_trade = registTable.Rows[j]["즉시구매"].ToString();

                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_re_reg.html?id=" + RegistID[j]);
                                req2.Method = "GET";
                                req2.ContentType = "application/x-www-form-urlencoded";
                                req2.CookieContainer = cookie;
                                req2.CookieContainer.Add(resp.Cookies);
                                resp.Close();
                                HttpWebResponse resp2 = (HttpWebResponse)req2.GetResponse();
                                Encoding encode = Encoding.GetEncoding("utf-8");
                                Stream strReceiveStream = resp2.GetResponseStream();
                                StreamReader reqStreamReader = new StreamReader(strReceiveStream, encode);
                                String buyGet = reqStreamReader.ReadToEnd();
                                strReceiveStream.Close();
                                reqStreamReader.Close();
                                req2.Abort();

                                sendData = "user_goods_type=" + user_goods_type + "&game_code=" + game_code + "&game_code_text=" + game_code_text +
                                "&server_code=" + server_code + "&server_code_text=" + server_code_text + "&df_server_code=" + df_server_code + "&df_server_code_text=" +
                                df_server_code_text + "&user_goods=" + user_goods + "&user_quantity=" + user_quantity + "&gamemoney_unit=" + gamemoney_unit + "&user_division_unit=" +
                                user_division_unit + "&user_division_price=" + user_division_price + "&user_quantity_min=" + user_quantity_min + "&user_quantity_max=" + user_quantity_max +
                                "&user_title=" + user_title + "&user_price=" + user_price + "&user_character=" + user_character + "&user_text=" + user_text + "&direct_reg_trade=" + direct_reg_trade + "&direct_condition_credit=1" + "&id=" + RegistID[j];

                                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_re_reg_ok.php");
                                req3.Method = "POST";
                                req3.ContentType = "application/x-www-form-urlencoded";
                                req3.Referer = "http://trade.itemmania.com/myroom/buy/buy_re_reg.html?id=" + RegistID[j];
                                req3.CookieContainer = cookie;
                                req3.CookieContainer.Add(resp2.Cookies);
                                resp2.Close();
                                StreamWriter writer3 = new StreamWriter(req3.GetRequestStream());
                                writer3.Write(sendData);
                                writer3.Close();

                                HttpWebResponse result = (HttpWebResponse)req3.GetResponse();
                                Encoding encode2 = Encoding.GetEncoding("utf-8");
                                Stream strReceiveStream2 = result.GetResponseStream();
                                StreamReader reqStreamReader2 = new StreamReader(strReceiveStream2, encode2);
                                String strResult2 = reqStreamReader2.ReadToEnd();
                                strReceiveStream2.Close();
                                reqStreamReader2.Close();
                                req3.Abort();

                                if (strResult2.Contains("정상적인 경로를 이용하세요"))
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 정상적인 경로를 이용하세요.[0x003]");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 정상적인 경로를 이용하세요.[0x003]");
                                }
                                else if (strResult2.Contains("물품 개수가 초과"))
                                {
                                    stopPoint[qqq] = "1";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => " + strResult2.Substring(strResult2.IndexOf("confirm") + 8, strResult2.IndexOf(")) location") - (strResult2.IndexOf("confirm") + 8)));
                                }
                                else if (strResult2.Contains("해당서버에 아직"))
                                {
                                    dlength[qqq, 1]--;
                                    qqq--;
                                }
                                else if (strResult2.Contains("물품이 정상적으로"))
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  재등록 완료(ID:" + strResult2.Substring(strResult2.IndexOf("해당 ID로") + 7, strResult2.IndexOf(" / IP에서") - (strResult2.IndexOf("해당 ID로") + 8)) + " & IP:" + strResult2.Substring(strResult2.IndexOf("IP에서") + 5, strResult2.IndexOf("개까지 추가등록") - (strResult2.IndexOf("IP에서") + 5)) + ")");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ")  =>  재등록 완료(ID:" + strResult2.Substring(strResult2.IndexOf("해당 ID로") + 7, strResult2.IndexOf(" / IP에서") - (strResult2.IndexOf("해당 ID로") + 8)) + " & IP:" + strResult2.Substring(strResult2.IndexOf("IP에서") + 5, strResult2.IndexOf("개까지 추가등록") - (strResult2.IndexOf("IP에서") + 5)) + ")");

                                    HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.html?strRelationType=regist");
                                    req4.Method = "GET";
                                    req4.CookieContainer = cookie;
                                    req4.CookieContainer.Add(result.Cookies);
                                    result.Close();
                                    HttpWebResponse response2 = (HttpWebResponse)req4.GetResponse();

                                    Stream stReadData3 = response2.GetResponseStream();
                                    StreamReader srReadData3 = new StreamReader(stReadData3, encode2);
                                    string strResult3 = srReadData3.ReadToEnd();
                                    stReadData3.Close();
                                    req4.Abort();
                                    response2.Close();

                                    RegistID[j] = strResult3.Substring(strResult3.IndexOf("check[]\" value=") + 16, 16);
                                }
                                else if (strResult2.Contains("로그인 후 이용해주세요"))
                                {
                                    result.Close();
                                    dlength[qqq, 1]--;
                                    qqq--;
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("/myroom/sell/sell_regist.html?strRelationType=regist"))
                                {
                                    result.Close();
                                    RegistID[j] = "없음";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 판매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("등록에 실패"))
                                {
                                    result.Close();
                                    dlength[qqq, 1]--;
                                    qqq--;
                                    Debug.WriteLine(DateTime.Now + "\n" + strResult2);
                                }
                                else if (strResult2.Contains("물품의 수가 초과"))
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 현재 고객님께서 등록하신 물품의 수가 초과되어 더 이상 물품 등록이 불가능 합니다.\n\n물품을 삭제 하신 후 등록 하시기 바랍니다.\n\n1인당 보유할 수 있는 최대 물품 수는 판매등록:400개, 구매등록:100개 입니다.");
                                }
                                else
                                {
                                    RegistID[j] = "없음";
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite(" 일반/분할구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                }


                                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                                req5.Method = "GET";
                                req5.CookieContainer = cookie;
                                HttpWebResponse response = (HttpWebResponse)req5.GetResponse();
                                Stream stReadData = response.GetResponseStream();
                                req5.Abort();
                                response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            RegistID[j] = "없음";
                            string game_code_text = registTable.Rows[j]["게임명"].ToString();
                            string server_code_text = registTable.Rows[j]["서버명"].ToString();
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + DateTime.Now + "]" + " 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite(" 일반/분할 구매 (" + IDlist[k] + ", " + game_code_text + ", " + server_code_text + ", " + RegistID[j] + ") => 거래중 물품 또는 등록할 수 없는 상태입니다. 다음 주기 때 신규 등록합니다.");
                        }
                    }
                }
                int delay = 0;
                if (c.Length > 15)
                    delay = 0;
                else
                    delay = 20000 - (c.Length * 1000);

                Delay(delay);
            }
        }

        void Money()
        {
            int q = 0;//Form2의 아이디목록 카운트
            StringBuilder getstr = new StringBuilder();

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

            q = accountTable.Rows.Count;

            string[] IDlist = new string[q];
            string[] PWDlist = new string[q];

            int IDinsert = 0;

            foreach (DataRow dr in accountTable.Rows)
            {

                IDlist[IDinsert] = dr["ID"].ToString();
                PWDlist[IDinsert] = dr["PWD"].ToString();
                IDinsert++;

            }

            string[] money = new string[q];
            string moneyresult = "";
            int j = 0;//잔고 가져오기
            try
            {

                while (j != q)
                {
                    string sendData = "user_id=" + IDlist[j] + "&user_password=" + PWDlist[j];


                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                    req.Method = "POST";
                    req.ContentLength = sendData.Length;
                    req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    req.CookieContainer = cookie;
                    StreamWriter writer = new StreamWriter(req.GetRequestStream());
                    writer.Write(sendData);
                    writer.Close();

                    HttpWebResponse result = (HttpWebResponse)req.GetResponse();

                    Encoding encode = Encoding.GetEncoding("utf-8");
                    Stream strReceiveStream = result.GetResponseStream();
                    StreamReader reqStreamReader = new StreamReader(strReceiveStream, encode);
                    String strResult = reqStreamReader.ReadToEnd();
                    strReceiveStream.Close();
                    reqStreamReader.Close();

                    req = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/mileage/my_mileage/");
                    req.Method = "GET";
                    req.CookieContainer = cookie;
                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    Stream stReadData1 = response.GetResponseStream();
                    StreamReader srReadData1 = new StreamReader(stReadData1, encode);
                    string strResult1 = srReadData1.ReadToEnd();

                    int index1 = strResult1.IndexOf("td") + 46;
                    int index2 = strResult1.IndexOf("원");
                    int index3 = strResult1.IndexOf("원", index2 + 1);

                    money[j] = strResult1.Substring(index1, index3 - index1 - 1);
                    moneyresult = moneyresult + IDlist[j] + " : " + money[j] + "원\r";


                    req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                    req.Method = "GET";
                    req.CookieContainer = cookie;
                    HttpWebResponse response2 = (HttpWebResponse)req.GetResponse();
                    Stream stReadData2 = response2.GetResponseStream();
                    req.Abort();
                    j++;
                }

            }
            catch { }            
            MessageBox.Show(moneyresult);
        }

        static string Game_code(string game_text)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/_xml/gamelist.xml");
                req.Method = "GET";
                req.CookieContainer = Form3.cookie;
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Encoding encode = Encoding.GetEncoding("utf-8");
                Stream stReadData1 = response.GetResponseStream();
                StreamReader srReadData1 = new StreamReader(stReadData1, encode);
                string strResult1 = srReadData1.ReadToEnd();

                int index1 = strResult1.IndexOf("id=\"");
                int index2 = strResult1.IndexOf("\" name=") + 8;
                int index3 = strResult1.IndexOf("\" level");

                while (strResult1.Substring(index2, index3 - index2) != game_text)
                {
                    index1 = strResult1.IndexOf("id=\"", index1 + 1);
                    index2 = strResult1.IndexOf("\" name=", index2) + 8;
                    index3 = strResult1.IndexOf("\" level", index3 + 1);
                }
                index1 = index1 + 4;
                index2 = index2 - 8;


                return strResult1.Substring(index1, index2 - index1);
            }
            catch { return ""; }
        }

        static string Server_code(string game_code, string server_text)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/_xml/serverlist.php" + "?game=" + game_code);
                req.Method = "GET";
                req.CookieContainer = Form3.cookie;
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Encoding encode = Encoding.GetEncoding("utf-8");
                Stream stReadData1 = response.GetResponseStream();
                StreamReader srReadData1 = new StreamReader(stReadData1, encode);
                string strResult1 = srReadData1.ReadToEnd();
                int index1 = strResult1.IndexOf("ID=\"", strResult1.IndexOf("ID=\"") + 1);
                int index2 = strResult1.IndexOf("\" NAME=") + 8;
                int index3 = strResult1.IndexOf("\" MONEY=");
                while (strResult1.Substring(index2, index3 - index2) != server_text)
                {
                    index1 = strResult1.IndexOf("ID=\"", index1 + 1);
                    index2 = strResult1.IndexOf("\" NAME=", index2) + 8;
                    index3 = strResult1.IndexOf("\" MONEY=", index3 + 1);
                }
                index1 = index1 + 4;
                index2 = index2 - 8;

                if (server_text == "기타")
                {
                    index2 = strResult1.IndexOf(" TYPE", strResult1.IndexOf(" TYPE") + 1) - 1;
                }
                return strResult1.Substring(index1, index2 - index1);
            }
            catch { return ""; }
        }

        private void Button6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left)) //우클릭
            {
                ContextMenu m = new ContextMenu();

                MenuItem m1 = new MenuItem();
                MenuItem m2 = new MenuItem();

                m1.Text = "저장하기";
                m2.Text = "불러오기";

                m1.Click += (senders, es) =>
                {
                    DataTable table = new DataTable();

                    try
                    {

                        string DbFile = "data.dat";
                        string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                        SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                        sqliteConn.Open();
                        string strsql = "SELECT 등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지 FROM regist";
                        SQLiteCommand cmd = new SQLiteCommand(strsql, sqliteConn);
                        SQLiteDataReader reader = cmd.ExecuteReader();

                        table.Load(reader);

                        sqliteConn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    saveFile.Title = "Excel 저장위치 지정";
                    saveFile.DefaultExt = "xlsx";
                    saveFile.Filter = "Xlsx files(*.xlsx)|*.xlsx|Xls files(*.xls)|*.xls";
                    saveFile.ShowDialog();
                    if(saveFile.FileName != "")
                        MessageBox.Show("저장중");

                    Excel.Application ap = new Excel.Application();
                    Excel.Workbook excelWorkBook = ap.Workbooks.Add();

                    Excel.Worksheet ws = excelWorkBook.Worksheets.get_Item(1) as Excel.Worksheet;

                    for (int columnHeaderIndex = 1; columnHeaderIndex <= table.Columns.Count; columnHeaderIndex++)
                    {
                        ws.Cells[1, columnHeaderIndex] = table.Columns[columnHeaderIndex - 1].ColumnName;
                    }
                    for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                    {
                        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                        {
                            ws.Cells[rowIndex + 2, columnIndex + 1] = table.Rows[rowIndex].ItemArray[columnIndex].ToString();
                        }
                    }
                    if (saveFile.FileName.Length > 0)
                    {
                        foreach (string filename in saveFile.FileNames)
                        {
                            string savePath = filename;
                            if (Path.GetExtension(savePath) == ".xls")
                            {
                                excelWorkBook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookNormal);
                            }
                            else if (Path.GetExtension(savePath) == ".xlsx")
                            {
                                excelWorkBook.SaveAs(savePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                            }
                            excelWorkBook.Close(true);
                            ap.Quit();
                        }
                    }
                    if (saveFile.FileName != "")
                        MessageBox.Show("저장완료");

                };

                m2.Click += (sendes, es) =>
                {
                    int count1 = 0;

                    string DbFile = "data.dat";
                    string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    string strqry = "SELECT COUNT(rowid) FROM regist";
                    SQLiteCommand cmd = new SQLiteCommand(strqry, sqliteConn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            count1 = Int32.Parse(reader[0].ToString());
                        }
                    }

                    DataTable table = new DataTable();
                    OleDbConnection excelConn = null;
                    string xlsfilename;

                    try
                    {
                        FileDialog fileDialog = new OpenFileDialog();
                        fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        fileDialog.Filter = "Xlsx files(*.xlsx)|*.xlsx|Xls files(*.xls)|*.xls";
                        fileDialog.RestoreDirectory = true;
                        if (fileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlsfilename = fileDialog.FileName;
                            string strCon;
                            if (xlsfilename.IndexOf(".xlsx") > -1)
                            {
                                strCon = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsfilename + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\";";
                            }
                            else
                            {

                                strCon = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + xlsfilename + ";Extended Properties=\"Excel 8.0;HDR=Yes\";";
                            }
                            excelConn = new OleDbConnection(strCon);

                            excelConn.Open();
                            string excelSql = "SELECT * FROM [Sheet1$]";

                            OleDbDataAdapter excelAdapter = new OleDbDataAdapter(excelSql, excelConn);

                            excelAdapter.Fill(table);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("파일 가져오기 실패 :" + ex.Message);
                    }
                    int rowCount = table.Rows.Count;


                    DbFile = "data.dat";
                    ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                    sqliteConn = new SQLiteConnection(ConnectionString);
                    sqliteConn.Open();
                    int i = listView1.Items.Count;
                    foreach (DataRow dr in table.Rows)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = dr["등록"].ToString();
                        lvi.SubItems.Add(dr["아이디"].ToString());
                        lvi.SubItems.Add(dr["매매구분"].ToString());
                        lvi.SubItems.Add(dr["게임명"].ToString());
                        lvi.SubItems.Add(dr["서버명"].ToString());
                        lvi.SubItems.Add(dr["제목"].ToString());
                        lvi.SubItems.Add(dr["물품종류"].ToString());
                        lvi.SubItems.Add(dr["가격"].ToString());
                        lvi.SubItems.Add(dr["최대수량"].ToString());
                        lvi.SubItems.Add(dr["분할단위"].ToString());
                        lvi.SubItems.Add(dr["최소수량"].ToString());
                        string strsql = "INSERT INTO regist (등록,아이디,매매구분,게임명,서버명,물품종류,일반분할흥정,제목,내용,가격,거래수량,거래수량단위,최대수량,최소수량,분할단위,즉시구매,전달서버,캐릭터명,기준가격,최저가격체크,최저가격,이미지) values ('" + dr["등록"] + "','" + dr["아이디"] + "','" + dr["매매구분"] + "','" + dr["게임명"] + "','" + dr["서버명"] + "','" + dr["물품종류"] + "','" + dr["일반분할흥정"] + "','" + dr["제목"] + "','" + dr["내용"] + "','" + dr["가격"] + "','" + dr["거래수량"] + "','" + dr["거래수량단위"] + "','" + dr["최대수량"] + "','" + dr["최소수량"] + "','" + dr["분할단위"] + "','" + dr["즉시구매"] + "','" + dr["전달서버"] + "','" + dr["캐릭터명"] + "','" + dr["기준가격"] + "','" + dr["최저가격체크"] + "','" + dr["최저가격"] + "','" + dr["이미지"] + "')";
                        cmd = new SQLiteCommand(strsql, sqliteConn);
                        cmd.ExecuteNonQuery();
                        listView1.Items.Add(lvi);
                        listView1.Items[i].Checked = true;
                        i++;
                    }

                    sqliteConn.Close();
                    label1.Text = "물품개수 : " + listView1.Items.Count.ToString() + "개";
                };
                m.MenuItems.Add(m1);
                m.MenuItems.Add(m2);

                m.Show(button6, new Point(e.X, e.Y));

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            regist.Abort();
        }

        private void ListView1_ControlAdded(object sender, ControlEventArgs e)
        {
            listviewcount = listView1.Items.Count;
        }

        private void ListView1_ControlRemoved(object sender, ControlEventArgs e)
        {
            listviewcount = listView1.Items.Count;
        }

        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control && (e.KeyCode == Keys.NumPad8 || e.KeyCode == Keys.NumPad2))
            {
                if (e.KeyCode == Keys.NumPad8)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + 100;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                }
                if (e.KeyCode == Keys.NumPad2)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + -100;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                }
            }
            else
            {
                if (e.KeyCode == Keys.NumPad8)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + 10;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                }
                if (e.KeyCode == Keys.NumPad2)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    int index = listView1.SelectedItems.Count;
                    int i = 0;
                    while (index > i)
                    {
                        try
                        {

                            int price = Int32.Parse(listView1.SelectedItems[i].SubItems[7].Text) + -10;
                            string DbFile = "data.dat";
                            string ConnectionString = string.Format("Data Source={0};Version=3;", DbFile);
                            SQLiteConnection sqliteConn = new SQLiteConnection(ConnectionString);
                            sqliteConn.Open();
                            string strsql2 = "UPDATE regist SET 가격='" + price.ToString() + "' where rowid IN (SELECT rowid FROM regist LIMIT " + listView1.SelectedIndices[i] + ",1)";
                            SQLiteCommand cmd = new SQLiteCommand(strsql2, sqliteConn);
                            cmd.ExecuteNonQuery();
                            sqliteConn.Close();
                            listView1.SelectedItems[i].SubItems[7].Text = price.ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        i++;
                    }
                }
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["DeleteForm"] is DeleteForm DeleteForm)
            {
                DeleteForm.Focus();
                return;
            }
            DeleteForm = new DeleteForm();
            DeleteForm.ShowDialog();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["OptionForm"] is OptionForm OptionForm)
            {
                OptionForm.Focus();
                return;
            }
            OptionForm = new OptionForm();
            OptionForm.ShowDialog();
        }
        static void DeleteLoop()
        {
            while (true)
            {
                StringBuilder getstr = new StringBuilder();
                GetPrivateProfileString("OPTION", "SellDeleteCheck", null, getstr, 1000, path);
                string sellDeleteCheck = getstr.ToString();
                GetPrivateProfileString("OPTION", "BuyDeleteCheck", null, getstr, 1000, path);
                string buyDeleteCheck = getstr.ToString();
                GetPrivateProfileString("OPTION", "SellDeleteTime", null, getstr, 1000, path);
                int sellDeleteTime = Int32.Parse(getstr.ToString());
                GetPrivateProfileString("OPTION", "BuyDeleteTime", null, getstr, 1000, path);
                int buyDeleteTime = Int32.Parse(getstr.ToString());

                Delay(sellDeleteTime * 30000);
                
                regist.Suspend();

                while (true)
                {
                    if (regist.ThreadState.ToString() == "Suspended")
                    {
                        break;
                    }
                }

                HttpWebRequest logoutReq = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                logoutReq.Method = "GET";
                logoutReq.CookieContainer = cookie;
                HttpWebResponse logoutResp = (HttpWebResponse)logoutReq.GetResponse();
                logoutReq.Abort();
                logoutResp.Close();
                try
                {
                    if (sellDeleteCheck == "1")
                    {
                        DataTable registTable2 = new DataTable();
                        try
                        {

                            string DbFile2 = "data.dat";
                            string ConnectionString2 = string.Format("Data Source={0};Version=3;", DbFile2);
                            SQLiteConnection sqliteConn2 = new SQLiteConnection(ConnectionString2);
                            sqliteConn2.Open();
                            string strqry2 = "SELECT DISTINCT account.ID,account.PWD FROM account JOIN regist ON account.ID=regist.아이디";
                            SQLiteCommand cmd2 = new SQLiteCommand(strqry2, sqliteConn2);
                            SQLiteDataReader reader2 = cmd2.ExecuteReader();

                            registTable2.Load(reader2);

                            reader2.Close();
                            sqliteConn2.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        string[] deleteID = new string[registTable2.Rows.Count];
                        string[] deletePWD = new string[registTable2.Rows.Count];

                        int IDinsert = 0;
                        foreach (DataRow dr in registTable2.Rows)
                        {
                            deleteID[IDinsert] = dr["ID"].ToString();
                            deletePWD[IDinsert] = dr["PWD"].ToString();
                            IDinsert++;
                        }
                        string[][] registID = new string[registTable2.Rows.Count][];
                        string[][] registTime = new string[registTable2.Rows.Count][];
                        int[] count = new int[registTable2.Rows.Count];
                        for (int i = 0; i < registTable2.Rows.Count; i++)
                        {
                            registID[i] = new string[400];
                            registTime[i] = new string[400];
                            count[i] = 0;
                        }

                        for (int i = 0; i < registTable2.Rows.Count; i++)
                        {

                            string sendData = "user_id=" + deleteID[i] + "&user_password=" + deletePWD[i];
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                            req.Method = "POST";
                            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                            req.CookieContainer = cookie;

                            Debug.WriteLine(deleteID[i]);
                            StreamWriter writer = new StreamWriter(req.GetRequestStream());
                            writer.Write(sendData);
                            writer.Close();

                            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                            req.Abort();
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + deleteID[i] + "] " + "판매등록 물품 정보 읽기 시작");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite("[" + deleteID[i] + "] " + "판매등록 물품 정보 읽기 시작");

                            for (int page = 1; page < 41; page++)
                            {
                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.html?page=" + page.ToString() + "&strRelationType=regist");
                                req2.Method = "GET";
                                req2.CookieContainer = cookie;
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
                                int index3 = strResult.IndexOf("onclick=\"reInsert") - 39;
                                int index4 = index3 + 11;
                                
                                listBox1.Invoke(new MethodInvoker(delegate ()
                                {
                                    listBox1.Items.Add("[" + deleteID[i] + "] " + page + "페이지 읽음");
                                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                }));
                                LogWrite("[" + deleteID[i] + "] " + page + "페이지 읽음");
                                int j;
                                for (j = 0; j < 10; j++)
                                {
                                    try
                                    {
                                        registTime[i][count[i]] = strResult.Substring(index3, index4 - index3);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    DateTime date = new DateTime(Int32.Parse(DateTime.Now.ToString("yyyy")), Int32.Parse(registTime[i][count[i]].Substring(0, 2)), Int32.Parse(registTime[i][count[i]].Substring(3, 2)), Int32.Parse(registTime[i][count[i]].Substring(6, 2)), Int32.Parse(registTime[i][count[i]].Substring(9, 2)), 00);
                                    TimeSpan time = DateTime.Now - date;
                                    index3 = strResult.IndexOf("onclick=\"reInsert", index3 + 40) - 39;
                                    index4 = index3 + 11;
                                    if (Math.Truncate(time.TotalMinutes) < sellDeleteTime)
                                    {
                                        index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                                        index2 = strResult.IndexOf("\" style=\"border", index2 + 1);
                                        continue;
                                    }
                                    registID[i][count[i]] = strResult.Substring(index1, index2 - index1);
                                    index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                                    index2 = strResult.IndexOf("\" style=\"border", index2 + 1);
                                    count[i]++;
                                }
                                req2.Abort();
                                resp2.Close();
                                if (j != 10)
                                    break;
                            }

                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + deleteID[i] + "] " + "판매등록 물품 정보 읽기 종료");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite("[" + deleteID[i] + "] " + "판매등록 물품 정보 읽기 종료");

                            HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.html?strRelationType=regist");
                            req3.Method = "GET";
                            req3.CookieContainer = cookie;
                            req3.CookieContainer.Add(resp.Cookies);
                            resp.Close();
                            HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();
                            req3.Abort();
                            for (int j = 0; j < registID[i].Length; j++)
                            {
                                if (registID[i][j] == null)
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + deleteID[i] + "] " + "판매등록 삭제 완료");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite("[" + deleteID[i] + "] " + "판매등록 삭제 완료");
                                    break;
                                }
                                try
                                {
                                    string sendData2 = "process=deleteSelect&check[]=" + registID[i][j];
                                    HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/sell/sell_regist.php");
                                    req4.Referer = "http://trade.itemmania.com/myroom/sell/sell_regist.html?strRelationType=regist";
                                    req4.Method = "POST";
                                    req4.CookieContainer = cookie;
                                    req4.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                    req4.CookieContainer.Add(resp3.Cookies);


                                    StreamWriter writer2 = new StreamWriter(req4.GetRequestStream());
                                    writer2.Write(sendData2);
                                    writer2.Close();

                                    HttpWebResponse resp4 = (HttpWebResponse)req4.GetResponse();

                                    req4.Abort();
                                    resp4.Close();
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + deleteID[i] + "] " + "판매등록 " + registID[i][j] + " 물품 삭제 완료");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite("[" + deleteID[i] + "] " + "판매등록 삭제 완료");
                                }
                                catch
                                {
                                    Debug.WriteLine("예외");
                                }
                            }
                            resp3.Close();
                            HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                            req5.Method = "GET";
                            req5.CookieContainer = cookie;
                            HttpWebResponse response = (HttpWebResponse)req5.GetResponse();
                            req5.Abort();
                            response.Close();
                        }
                    }
                }
                catch
                {
                    listBox1.Invoke(new MethodInvoker(delegate ()
                    {
                        listBox1.Items.Add("판매삭제 중 에러발생");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }));
                    LogWrite("판매삭제 중 에러발생");
                }


                try
                {
                    if (buyDeleteCheck == "1")
                    {
                        DataTable registTable2 = new DataTable();
                        try
                        {

                            string DbFile2 = "data.dat";
                            string ConnectionString2 = string.Format("Data Source={0};Version=3;", DbFile2);
                            SQLiteConnection sqliteConn2 = new SQLiteConnection(ConnectionString2);
                            sqliteConn2.Open();
                            string strqry2 = "SELECT DISTINCT account.ID,account.PWD FROM account JOIN regist ON account.ID=regist.아이디";
                            SQLiteCommand cmd2 = new SQLiteCommand(strqry2, sqliteConn2);
                            SQLiteDataReader reader2 = cmd2.ExecuteReader();

                            registTable2.Load(reader2);

                            reader2.Close();
                            sqliteConn2.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        string[] deleteID = new string[registTable2.Rows.Count];
                        string[] deletePWD = new string[registTable2.Rows.Count];

                        int IDinsert = 0;
                        foreach (DataRow dr in registTable2.Rows)
                        {
                            deleteID[IDinsert] = dr["ID"].ToString();
                            deletePWD[IDinsert] = dr["PWD"].ToString();
                            IDinsert++;
                        }
                        string[][] registID = new string[registTable2.Rows.Count][];
                        string[][] registTime = new string[registTable2.Rows.Count][];
                        int[] count = new int[registTable2.Rows.Count];
                        for (int i = 0; i < registTable2.Rows.Count; i++)
                        {
                            registID[i] = new string[400];
                            registTime[i] = new string[400];
                            count[i] = 0;
                        }

                        for (int i = 0; i < registTable2.Rows.Count; i++)
                        {

                            string sendData = "user_id=" + deleteID[i] + "&user_password=" + deletePWD[i];
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.itemmania.com/portal/user/login_form_ok.php");
                            req.Method = "POST";
                            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                            req.CookieContainer = cookie;

                            Debug.WriteLine(deleteID[i]);
                            StreamWriter writer = new StreamWriter(req.GetRequestStream());
                            writer.Write(sendData);
                            writer.Close();

                            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                            req.Abort();
                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + deleteID[i] + "] " + "구매등록 물품 정보 읽기 시작");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite("[" + deleteID[i] + "] " + "구매등록 물품 정보 읽기 시작");

                            for (int page = 1; page < 11; page++)
                            {
                                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.html?page=" + page.ToString() + "&strRelationType=regist");
                                req2.Method = "GET";
                                req2.CookieContainer = cookie;
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
                                int index3 = strResult.IndexOf("onclick=\"reInsert") - 41;
                                int index4 = index3 + 11;

                                listBox1.Invoke(new MethodInvoker(delegate ()
                                {
                                    listBox1.Items.Add("[" + deleteID[i] + "] " + page + "페이지 읽음");
                                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                }));
                                LogWrite("[" + deleteID[i] + "] " + page + "페이지 읽음");
                                int j;
                                for (j = 0; j < 10; j++)
                                {
                                    try
                                    {
                                        registTime[i][count[i]] = strResult.Substring(index3, index4 - index3);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    Debug.WriteLine(registTime[i][count[i]]);
                                    DateTime date = new DateTime(Int32.Parse(DateTime.Now.ToString("yyyy")), Int32.Parse(registTime[i][count[i]].Substring(0, 2)), Int32.Parse(registTime[i][count[i]].Substring(3, 2)), Int32.Parse(registTime[i][count[i]].Substring(6, 2)), Int32.Parse(registTime[i][count[i]].Substring(9, 2)), 00);
                                    TimeSpan time = DateTime.Now - date;
                                    index3 = strResult.IndexOf("onclick=\"reInsert", index3 + 42) - 41;
                                    index4 = index3 + 11;
                                    if (Math.Truncate(time.TotalMinutes) < sellDeleteTime)
                                    {
                                        index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                                        index2 = index1 + 16;
                                        continue;
                                    }
                                    registID[i][count[i]] = strResult.Substring(index1, index2 - index1);
                                    index1 = strResult.IndexOf("check[]\" value=", index1 + 1) + 16;
                                    index2 = index1 + 16;
                                    count[i]++;
                                }
                                req2.Abort();
                                resp2.Close();
                                if (j != 10)
                                    break;
                            }

                            listBox1.Invoke(new MethodInvoker(delegate ()
                            {
                                listBox1.Items.Add("[" + deleteID[i] + "] " + "구매등록 물품 정보 읽기 종료");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            }));
                            LogWrite("[" + deleteID[i] + "] " + "구매등록 물품 정보 읽기 종료");

                            HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.html?strRelationType=regist");
                            req3.Method = "GET";
                            req3.CookieContainer = cookie;
                            req3.CookieContainer.Add(resp.Cookies);
                            resp.Close();
                            HttpWebResponse resp3 = (HttpWebResponse)req3.GetResponse();
                            req3.Abort();
                            for (int j = 0; j < registID[i].Length; j++)
                            {
                                if (registID[i][j] == null)
                                {
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + deleteID[i] + "] " + "구매등록 삭제 완료");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite("[" + deleteID[i] + "] " + "구매등록 삭제 완료");
                                    break;
                                }
                                try
                                {
                                    string sendData2 = "process=deleteSelect&check[]=" + registID[i][j];
                                    HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create("http://trade.itemmania.com/myroom/buy/buy_regist.php");
                                    req4.Referer = "http://trade.itemmania.com/myroom/buy/buy_regist.html?strRelationType=regist";
                                    req4.Method = "POST";
                                    req4.CookieContainer = cookie;
                                    req4.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                                    req4.CookieContainer.Add(resp3.Cookies);


                                    StreamWriter writer2 = new StreamWriter(req4.GetRequestStream());
                                    writer2.Write(sendData2);
                                    writer2.Close();

                                    HttpWebResponse resp4 = (HttpWebResponse)req4.GetResponse();

                                    req4.Abort();
                                    resp4.Close();
                                    listBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        listBox1.Items.Add("[" + deleteID[i] + "] " + "구매등록 " + registID[i][j] + " 물품 삭제 완료");
                                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                    }));
                                    LogWrite("[" + deleteID[i] + "] " + "구매등록 삭제 완료");
                                }
                                catch
                                {
                                    Debug.WriteLine("예외");
                                }
                            }
                            resp3.Close();
                            HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create("http://www.itemmania.com/portal/user/logout_ok.html");
                            req5.Method = "GET";
                            req5.CookieContainer = cookie;
                            HttpWebResponse response = (HttpWebResponse)req5.GetResponse();
                            req5.Abort();
                            response.Close();
                        }
                    }
                }
                catch
                {
                    listBox1.Invoke(new MethodInvoker(delegate ()
                    {
                        listBox1.Items.Add("구매삭제 중 에러발생");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }));
                    LogWrite("구매삭제 중 에러발생");
                }
                regist.Resume();
            }
        }
    }
}