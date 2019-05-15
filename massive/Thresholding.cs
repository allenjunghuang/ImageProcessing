using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Thresholding : Form
    {
        public int cordx;
        public int cordy;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;
        int alpha;
        int otsu;

        public Thresholding(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            Bitmap graymap = new Bitmap(xdim, ydim);
            int[,] C2G = new int[xdim, ydim];

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

            //histogram
            int[] fstCount = new int[256];
            float[] fstPDF = new float[256];
            float[] fstCDF = new float[256];
            float pixelmean = 0;
            float[] pixelvarn = new float[256];                      

            for (int i = 0; i < 256; i++)
            {
                fstCount[i] = 0;
                fstPDF[i] = 0;
                fstCDF[i] = 0;              
            }

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2G[i, j] == k)
                        {
                            fstCount[k]++;
                        }
                    }
                }
            }
            for (int i = 0; i < 256; i++)
            {
                fstPDF[i] = fstCount[i] / (ydim * xdim);
                pixelmean += fstCount[i] * i;
            }
            for (int t = 0; t < 256; t++)
            {
                int u1 = 0; int u2 = 0;
                int u1percent = 0; int u2percent = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (i <= t)
                    {
                        u1 += fstCount[i] * i;
                        u1percent += fstCount[i];
                    }

                    if (i > t)
                    {
                        u2 += fstCount[i] * i;
                        u2percent += fstCount[i];
                    }
                }
                if (u1 != 0) { u1 = u1 / u1percent; }
                if (u2 != 0) { u2 = u2 / u2percent; }
                pixelvarn[t] = u1percent * (u1 - pixelmean) * (u1 - pixelmean) + u2percent * (u2 - pixelmean) * (u2 - pixelmean);
            }
            float thresh = 0;      
            for (int i = 0; i < 256; i++)
            {
                if (i == 0)
                {
                    thresh = pixelvarn[i];
                    otsu = 0;
                }
                if (pixelvarn[i] < thresh)
                {
                    thresh = pixelvarn[i];
                    otsu = i;
                }
            }
            label4.Text = "" + otsu;
            label3.Text = "" + otsu;
            trackBar1.Value = otsu;
            //SNR
            double signal = 0;
            double noise = 0;
            double SNR;
            Bitmap otsumap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    int pixels = C2G[i, j];
                    if (pixels < otsu) { pixels = 0; }
                    if (pixels > otsu) { pixels = 255; }
                    otsumap.SetPixel(j, i, Color.FromArgb(pixels, pixels, pixels));
                    signal += C2G[i, j] * C2G[i, j];
                    noise += (pixels - C2G[i, j]) * (pixels - C2G[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox2.Size = new System.Drawing.Size((int)otsumap.Width, (int)otsumap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = otsumap;

            //------------------
            cordx = xdim;
            cordy = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G;
        }

        private void Thresholding_Load(object sender, EventArgs e)
        {

        }       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            alpha = trackBar1.Value;
            label3.Text = trackBar1.Value.ToString();
            double signal = 0;
            double noise = 0;
            double SNR=0;
            Bitmap otsumap = new Bitmap(cordx, cordy);          
            for (int i = 0; i < cordy; i++)
            {
                for (int j = 0; j < cordx; j++)
                {
                    int pixels = C2Ggrid[i, j];
                    if (pixels < alpha) { pixels = 0; }
                    if (pixels > alpha) { pixels = 255; }
                    otsumap.SetPixel(j, i, Color.FromArgb(pixels, pixels, pixels));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pixels - C2Ggrid[i, j]) * (pixels - C2Ggrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox2.Size = new System.Drawing.Size((int)otsumap.Width, (int)otsumap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = otsumap;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            int beta = otsu;
            trackBar1.Value = otsu;
            label3.Text = beta.ToString();
            double signal = 0;
            double noise = 0;
            double SNR;
            Bitmap otsumap = new Bitmap(cordx, cordy);
            for (int i = 0; i < cordy; i++)
            {
                for (int j = 0; j < cordx; j++)
                {
                    int pixels = C2Ggrid[i, j];
                    if (pixels < beta) { pixels = 0; }
                    if (pixels > beta) { pixels = 255; }
                    otsumap.SetPixel(j, i, Color.FromArgb(pixels, pixels, pixels));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pixels - C2Ggrid[i, j]) * (pixels - C2Ggrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox2.Size = new System.Drawing.Size((int)otsumap.Width, (int)otsumap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = otsumap;
        }
    }
}