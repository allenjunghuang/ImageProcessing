using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class OutlierFilter : Form
    {
        public int cordx;
        public int cordy;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public OutlierFilter(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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

        private void OutlierFilter_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double noise1 = 0;
            double noise2 = 0;
            double SNR;
            Bitmap outliermap = new Bitmap(cordx, cordy);
            
            double score = Convert.ToDouble(textBox1.Text);
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

                    int pixelavg = (int)(C2Ggrid[a, c] + C2Ggrid[i, c] + C2Ggrid[b, c] +
                                         C2Ggrid[a, j] + C2Ggrid[b, j] +
                                         C2Ggrid[a, d] + C2Ggrid[i, d] + C2Ggrid[b, d]) / 8;
                    if (pixelavg < 0) { pixelavg = 0; }
                    if (pixelavg > 255) { pixelavg = 255; }

                    double pixelstd = Math.Sqrt((((C2Ggrid[a, c] - pixelavg) * (C2Ggrid[a, c] - pixelavg)) + ((C2Ggrid[i, c] - pixelavg) * (C2Ggrid[i, c] - pixelavg)) +
                                                 ((C2Ggrid[b, c] - pixelavg) * (C2Ggrid[b, c] - pixelavg)) + ((C2Ggrid[a, j] - pixelavg) * (C2Ggrid[a, j] - pixelavg)) +
                                                 ((C2Ggrid[b, j] - pixelavg) * (C2Ggrid[b, j] - pixelavg)) + ((C2Ggrid[a, d] - pixelavg) * (C2Ggrid[a, d] - pixelavg)) +
                                                 ((C2Ggrid[i, d] - pixelavg) * (C2Ggrid[b, d] - pixelavg)) + ((C2Ggrid[b, d] - pixelavg) * (C2Ggrid[b, d] - pixelavg))) / (8 - 1));
                    
                    double thresh = score * pixelstd;

                    if ((C2Ggrid[i, j] - pixelavg) > thresh || (C2Ggrid[i, j] - pixelavg) < (-thresh))
                    {
                        outliermap.SetPixel(j, i, Color.FromArgb(pixelavg, pixelavg, pixelavg));
                        noise1 += (pixelavg - C2Ggrid[i, j]) * (pixelavg - C2Ggrid[i, j]);
                    }
                    else
                    {
                        outliermap.SetPixel(j, i, Color.FromArgb(C2Ggrid[i, j], C2Ggrid[i, j], C2Ggrid[i, j]));
                        noise2 += (C2Ggrid[i, j] - C2Ggrid[i, j]) * (C2Ggrid[i, j] - C2Ggrid[i, j]);
                    }
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise = noise1 + noise2;
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)outliermap.Width, (int)outliermap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = outliermap;

        }
    }
}