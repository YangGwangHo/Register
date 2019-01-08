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
    public partial class Form_Option : Form
    {
        public Form_Option()
        {
            InitializeComponent();

            StringBuilder getstr = new StringBuilder();
            Form_Main.GetPrivateProfileString("OPTION", "SellDeleteCheck", null, getstr, 1000, Form_Main.path);
            if (getstr.ToString() == "1")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            Form_Main.GetPrivateProfileString("OPTION", "BuyDeleteCheck", null, getstr, 1000, Form_Main.path);
            if (getstr.ToString() == "1")
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;

            Form_Main.GetPrivateProfileString("OPTION", "SellDeleteTime", null, getstr, 1000, Form_Main.path);
            textBox1.Text = getstr.ToString();
            Form_Main.GetPrivateProfileString("OPTION", "BuyDeleteTime", null, getstr, 1000, Form_Main.path);
            textBox2.Text = getstr.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                Form_Main.WritePrivateProfileString("OPTION", "SellDeleteCheck", "1", Form_Main.path);
            else
                Form_Main.WritePrivateProfileString("OPTION", "SellDeleteCheck", "0", Form_Main.path);
            
            if (checkBox2.Checked == true)
                Form_Main.WritePrivateProfileString("OPTION", "BuyDeleteCheck", "1", Form_Main.path);
            else
                Form_Main.WritePrivateProfileString("OPTION", "BuyDeleteCheck", "0", Form_Main.path);


            Form_Main.WritePrivateProfileString("OPTION", "SellDeleteTime", textBox1.Text, Form_Main.path);
            Form_Main.WritePrivateProfileString("OPTION", "BuyDeleteTime", textBox2.Text, Form_Main.path);

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
