using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace register_2
{
    public partial class OptionForm : Form
    {
        public OptionForm()
        {
            InitializeComponent();

            StringBuilder getstr = new StringBuilder();
            Form1.GetPrivateProfileString("OPTION", "SellDeleteCheck", null, getstr, 1000, Form1.path);
            if (getstr.ToString() == "1")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            Form1.GetPrivateProfileString("OPTION", "BuyDeleteCheck", null, getstr, 1000, Form1.path);
            if (getstr.ToString() == "1")
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;

            Form1.GetPrivateProfileString("OPTION", "SellDeleteTime", null, getstr, 1000, Form1.path);
            textBox1.Text = getstr.ToString();
            Form1.GetPrivateProfileString("OPTION", "BuyDeleteTime", null, getstr, 1000, Form1.path);
            textBox2.Text = getstr.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                Form1.WritePrivateProfileString("OPTION", "SellDeleteCheck", "1", Form1.path);
            else
                Form1.WritePrivateProfileString("OPTION", "SellDeleteCheck", "0", Form1.path);
            
            if (checkBox2.Checked == true)
                Form1.WritePrivateProfileString("OPTION", "BuyDeleteCheck", "1", Form1.path);
            else
                Form1.WritePrivateProfileString("OPTION", "BuyDeleteCheck", "0", Form1.path);


            Form1.WritePrivateProfileString("OPTION", "SellDeleteTime", textBox1.Text, Form1.path);
            Form1.WritePrivateProfileString("OPTION", "BuyDeleteTime", textBox2.Text, Form1.path);

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
