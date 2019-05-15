using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class MedianFilter : Form
    {
        public int cordx;
        public int cordy;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public MedianFilter(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();

            Bitmap sourcemap = new Bitmap(xdim, ydim);
            int[,] C2G = new int[xdim, ydim];

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }

            cordx = xdim;
            cordy = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G;
        }

        private void MedianFilter_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            Bitmap medianmap = new Bitmap(cordx, cordy);
            double signal = 0;
            double noise = 0;
            double SNR;
            int[] pixarry = new int[9];
            

            for (int i = 0; i < cordy; i++)
            {
                for (int j = 0; j < cordx; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = cordy - 1; }
                    if (b == cordy) { b = 0; }
                    if (c == -1) { c = cordx - 1; }
                    if (d == cordx) { d = 0; }

                    pixarry[0] = C2Ggrid[a, c];
                    pixarry[1] = C2Ggrid[i, c];
                    pixarry[2] = C2Ggrid[b, c];
                    pixarry[3] = C2Ggrid[a, j];
                    pixarry[4] = C2Ggrid[b, j];
                    pixarry[5] = C2Ggrid[a, d];
                    pixarry[6] = C2Ggrid[i, d];
                    pixarry[7] = C2Ggrid[b, d];
                    pixarry[8] = C2Ggrid[i, j];

                    Array.Sort(pixarry);
                    int pixmedian = pixarry[4];
                    medianmap.SetPixel(j, i, Color.FromArgb(pixmedian, pixmedian, pixmedian));

                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pixarry[4] - C2Ggrid[i, j]) * (pixarry[4] - C2Ggrid[i, j]); ;
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)medianmap.Width, (int)medianmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = medianmap;

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Bitmap medianmap = new Bitmap(cordx, cordy);
            double signal = 0;
            double noise = 0;
            double SNR;
            int[] pixarry = new int[9];


            for (int i = 0; i < cordy; i++)
            {
                for (int j = 0; j < cordx; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;
                    int m = i - 2;
                    int n = i + 2;
                    int p = j - 2;
                    int q = j + 2;

                    if (a == -1) { a = cordy - 1; }
                    if (b == cordy) { b = 0; }
                    if (c == -1) { c = cordx - 1; }
                    if (d == cordx) { d = 0; }

                    if (m == -1) { m = cordy - 1; }
                    if (n == cordy) { n = 0; }
                    if (p == -1) { p = cordx - 1; }
                    if (q == cordx) { q = 0; }

                    if (m == -2) { m = cordy - 2; }
                    if (n == cordy + 1) { n = 1; }
                    if (p == -2) { p = cordx - 2; }
                    if (q == cordx + 1) { q = 1; }

                    pixarry[0] = C2Ggrid[i, p];
                    pixarry[1] = C2Ggrid[i, c];
                    pixarry[2] = C2Ggrid[i, d];
                    pixarry[3] = C2Ggrid[i, q];
                    pixarry[4] = C2Ggrid[i, j];
                    pixarry[5] = C2Ggrid[m, j];
                    pixarry[6] = C2Ggrid[a, j];
                    pixarry[7] = C2Ggrid[b, j];
                    pixarry[8] = C2Ggrid[n, j];

                    Array.Sort(pixarry);
                    int pixmedian = pixarry[4];
                    medianmap.SetPixel(j, i, Color.FromArgb(pixmedian, pixmedian, pixmedian));

                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pixarry[4] - C2Ggrid[i, j]) * (pixarry[4] - C2Ggrid[i, j]); ;
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)medianmap.Width, (int)medianmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = medianmap;

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

        }
    }
}