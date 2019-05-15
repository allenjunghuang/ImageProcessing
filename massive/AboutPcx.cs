using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace massive
{
    public partial class AboutPcx : Form
    {
        public AboutPcx()
        {
            InitializeComponent();
        }

        private void AboutPcx_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) //pcx history
        {
            string txdir = Application.StartupPath;
            string txname = "Wiki\\PCX-History.txt";
            string txpath = Path.Combine(txdir, txname);

            textBox1.Text = File.ReadAllText(txpath);
 
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) //pcx format
        {
            string txdir = Application.StartupPath;
            string txname = "Wiki\\PCX-Format.txt";
            string txpath = Path.Combine(txdir, txname);

            textBox1.Text = File.ReadAllText(txpath);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) //run-length encoding
        {
            string txdir = Application.StartupPath;
            string txname = "Wiki\\PCX-RLE.txt";
            string txpath = Path.Combine(txdir, txname);

            textBox1.Text = File.ReadAllText(txpath);
        }
    }
}