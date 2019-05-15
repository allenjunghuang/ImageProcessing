using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class PseudoMedian : Form
    {
        public int cordx;
        public int cordy;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public PseudoMedian(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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

            cordx = xdim;
            cordy = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G;
        }

        private void PseudoMedian_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;         
            double SNR=0;
            Bitmap pseudmap = new Bitmap(cordx, cordy);
            
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

                    int pixel1 = C2Ggrid[i, j];
                    int pixel2 = C2Ggrid[i, c];
                    int pixel3 = C2Ggrid[i, d];
                    int pixel4 = C2Ggrid[a, j];
                    int pixel5 = C2Ggrid[b, j];

                    int pseudvalue = MaxMin(Math.Min(pixel1, Math.Min(pixel2, pixel3)), Math.Min(pixel1, Math.Min(pixel2, pixel4)), Math.Min(pixel1, Math.Min(pixel2, pixel5)), Math.Min(pixel1, Math.Min(pixel3, pixel4)), Math.Min(pixel1, Math.Min(pixel3, pixel5)),
                                            Math.Min(pixel1, Math.Min(pixel4, pixel5)), Math.Min(pixel2, Math.Min(pixel3, pixel4)), Math.Min(pixel2, Math.Min(pixel3, pixel5)), Math.Min(pixel2, Math.Min(pixel4, pixel5)), Math.Min(pixel3, Math.Min(pixel4, pixel5)));

                    if (pseudvalue < 0) { pseudvalue = 0; }
                    if (pseudvalue > 255) { pseudvalue = 255; }
                    pseudmap.SetPixel(j, i, Color.FromArgb(pseudvalue, pseudvalue, pseudvalue));

                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pseudvalue - C2Ggrid[i, j]) * (pseudvalue - C2Ggrid[i, j]); 
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)pseudmap.Width, (int)pseudmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = pseudmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR =0;
            Bitmap pseudmap = new Bitmap(cordx, cordy);

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

                    int pixel1 = C2Ggrid[i, j];
                    int pixel2 = C2Ggrid[i, c];
                    int pixel3 = C2Ggrid[i, d];
                    int pixel4 = C2Ggrid[a, j];
                    int pixel5 = C2Ggrid[b, j];

                    int pseudvalue = MinMax(Math.Max(pixel1, Math.Max(pixel2, pixel3)), Math.Max(pixel1, Math.Max(pixel2, pixel4)), Math.Max(pixel1, Math.Max(pixel2, pixel5)), Math.Max(pixel1, Math.Max(pixel3, pixel4)), Math.Max(pixel1, Math.Max(pixel3, pixel5)),
                                            Math.Max(pixel1, Math.Max(pixel4, pixel5)), Math.Max(pixel2, Math.Max(pixel3, pixel4)), Math.Max(pixel2, Math.Max(pixel3, pixel5)), Math.Max(pixel2, Math.Max(pixel4, pixel5)), Math.Max(pixel3, Math.Max(pixel4, pixel5)));

                    if (pseudvalue < 0) { pseudvalue = 0; }
                    if (pseudvalue > 255) { pseudvalue = 255; }
                    pseudmap.SetPixel(j, i, Color.FromArgb(pseudvalue, pseudvalue, pseudvalue));

                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pseudvalue - C2Ggrid[i, j]) * (pseudvalue - C2Ggrid[i, j]); ;
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)pseudmap.Width, (int)pseudmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = pseudmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR=0;
            Bitmap pseudmap = new Bitmap(cordx, cordy);

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

                    int pixel1 = C2Ggrid[i, j];
                    int pixel2 = C2Ggrid[i, c];
                    int pixel3 = C2Ggrid[i, d];
                    int pixel4 = C2Ggrid[a, j];
                    int pixel5 = C2Ggrid[b, j];

                    int pseudvalue1 = MaxMin(Math.Min(pixel1, Math.Min(pixel2, pixel3)), Math.Min(pixel1, Math.Min(pixel2, pixel4)), Math.Min(pixel1, Math.Min(pixel2, pixel5)), Math.Min(pixel1, Math.Min(pixel3, pixel4)), Math.Min(pixel1, Math.Min(pixel3, pixel5)),
                                             Math.Min(pixel1, Math.Min(pixel4, pixel5)), Math.Min(pixel2, Math.Min(pixel3, pixel4)), Math.Min(pixel2, Math.Min(pixel3, pixel5)), Math.Min(pixel2, Math.Min(pixel4, pixel5)), Math.Min(pixel3, Math.Min(pixel4, pixel5)));


                    int pseudvalue2 = MinMax(Math.Max(pixel1, Math.Max(pixel2, pixel3)), Math.Max(pixel1, Math.Max(pixel2, pixel4)), Math.Max(pixel1, Math.Max(pixel2, pixel5)), Math.Max(pixel1, Math.Max(pixel3, pixel4)), Math.Max(pixel1, Math.Max(pixel3, pixel5)),
                                             Math.Max(pixel1, Math.Max(pixel4, pixel5)), Math.Max(pixel2, Math.Max(pixel3, pixel4)), Math.Max(pixel2, Math.Max(pixel3, pixel5)), Math.Max(pixel2, Math.Max(pixel4, pixel5)), Math.Max(pixel3, Math.Max(pixel4, pixel5)));

                    int pseudvalue = (int)((0.5 * pseudvalue1) + (0.5 * pseudvalue2));
                    if (pseudvalue < 0) { pseudvalue = 0; }
                    if (pseudvalue > 255) { pseudvalue = 255; }
                    pseudmap.SetPixel(j, i, Color.FromArgb(pseudvalue, pseudvalue, pseudvalue));

                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (pseudvalue - C2Ggrid[i, j]) * (pseudvalue - C2Ggrid[i, j]); ;
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)pseudmap.Width, (int)pseudmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = pseudmap;
        }

        public static int MaxMin(int x1, int x2, int x3, int x4, int x5, int x6, int x7, int x8, int x9, int x10)
        {
            
            return Math.Max(x1, Math.Max(x2, Math.Max(x3, Math.Max(x4, Math.Max(x5, Math.Max(x6, Math.Max(x7, Math.Max(x8, Math.Max(x9,x10)))))))));
        }

        public static int MinMax(int x1, int x2, int x3, int x4, int x5, int x6, int x7, int x8, int x9, int x10)
        {
            return Math.Min(x1, Math.Min(x2, Math.Min(x3, Math.Min(x4, Math.Min(x5, Math.Min(x6, Math.Min(x7, Math.Min(x8, Math.Min(x9, x10)))))))));
        }
    }
}