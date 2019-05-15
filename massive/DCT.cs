using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class DCT : Form
    {
        public int xcord;
        public int ycord;
        public int[,] Rpln;
        public int[,] Gpln;
        public int[,] Bpln;
        public int[,] C2Gpln;
        public int[,] DCTpln;
        

        public DCT(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            int[,] C2G = new int[xdim, ydim];
            Bitmap graymap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = graymap;

            xcord = xdim;
            ycord = ydim;
            Rpln = Rdim;
            Gpln = Gdim;
            Bpln = Bdim;
            C2Gpln = C2G; 
        }

        private void button1_Click(object sender, EventArgs e)//DCT
        {
            double pi=3.14159265424;
            double temp;
            double [] a = new double[8];
            
            int [,] Fdct = new int [xcord, ycord];
            Bitmap dctmap = new Bitmap(xcord, ycord);
            a[0] = 0.3535533906; //square root of (1/8)
            for (int k = 1; k < 8; k++)
            {                  
                a[k] = 0.5; //square root of (2/8)
            }
            
            for (int u = 0; u < ycord; u++)
            {
                int us = (int)(u / 8) * 8;              
                int ue = (int)(u % 8);
                for (int v = 0; v < xcord; v++)
                {
                    int vs = (int)(v / 8) * 8;
                    int ve = (int)(v % 8);
                    temp = 0;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp += (C2Gpln[vs + x, us + y] - 128) * Math.Cos((2 * x + 1) * ve * pi / 16) * Math.Cos((2 * y + 1) * ue * pi / 16);
                        }
                    }
                    temp = temp * a[ue] * a[ve];
                    Fdct[u, v] = (int)(temp);
                }
            } 
          
            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    int offset = 0;
                    if (Fdct[i, j] > 255) {  offset = Fdct[i,j] - 255; }
                    if (Fdct[i, j] < 0) { offset = Fdct[i, j]; }
                    dctmap.SetPixel(i, j, Color.FromArgb(Fdct[i, j] - offset, Fdct[i, j] - offset, Fdct[i, j]- offset));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)dctmap.Width, (int)dctmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = dctmap;

            DCTpln = Fdct;
            
        }

        private void button2_Click(object sender, EventArgs e)//inverse DCT
        {
            double pi = 3.14159265424;
            double temp;
            double[] a = new double[8];
            double signal = 0;
            double noise = 0;
            double SNR;
            int[,] Idct = new int[xcord, ycord];
            Bitmap dctmap = new Bitmap(xcord, ycord);
            a[0] = 0.3535533906; //square roott of (1/8) 
            for (int i = 1; i < 8; i++)
            {              
                a[i] = 0.5; //square roott of (2/8)
            }

            for (int u = 0; u < ycord; u++)
            {
                int us = (int)(u / 8) * 8;
                int ue = (int)(u % 8);
                for (int v = 0; v < xcord; v++)
                {
                    int vs = (int)(v / 8) * 8;
                    int ve = (int)(v % 8);
                    temp = 0;
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            temp += a[y] * a[x] * DCTpln[vs + x, us + y] * Math.Cos((2 * ve) * x * pi / 16) * Math.Cos((2 * ue) * y * pi / 16);
                        }
                    }
                    temp = temp + 128;
                    Idct[u, v] = (int)(temp);
                }
            }

            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    if (Idct[i, j] > 255) { Idct[i, j] = 255; }
                    if (Idct[i, j] < 0) { Idct[i, j] = 0; }
                    signal += C2Gpln[i, j] * C2Gpln[i, j];
                    noise += (Idct[i, j] - C2Gpln[i, j]) * (Idct[i, j] - C2Gpln[i, j]);
                    dctmap.SetPixel(j, i, Color.FromArgb(Idct[i, j], Idct[i, j], Idct[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)dctmap.Width, (int)dctmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = dctmap;                    
        }

        private void DCT_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    graymap.SetPixel(j, i, Color.FromArgb(C2Gpln[i, j], C2Gpln[i, j], C2Gpln[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = graymap;
        }
    }
}