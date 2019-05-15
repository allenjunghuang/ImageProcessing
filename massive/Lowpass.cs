using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Lowpass : Form
    {
        public int fstxdim;
        public int fstydim;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public Lowpass(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            int[,] C2G = new int[xdim, ydim];
            Bitmap sourcemap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }

            fstxdim = xdim;
            fstydim = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G; 
        }

        private void Lowpass_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            int avgR;
            int avgG;
            int avgB;
            int[,] PSR = new int[fstxdim, fstydim];
            int[,] PSG = new int[fstxdim, fstydim];
            int[,] PSB = new int[fstxdim, fstydim];
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < fstydim; i++)
            {
                for (int j = 0; j < fstxdim; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;
                    if (a == -1) { a = fstydim - 1; }
                    if (b == fstydim) { b = 0; }
                    if (c == -1) { c = fstxdim - 1; }
                    if (d == fstxdim) { d = 0; }
                    avgR = (int)((Rgrid[a, c] + Rgrid[i, c] + Rgrid[b, c] + 
                                  Rgrid[a, j] + Rgrid[i, j] + Rgrid[b, j] + 
                                  Rgrid[a, d] + Rgrid[i, d] + Rgrid[b, d]) / 9);
                    if (avgR < 0) { avgR = 0; }
                    if (avgR > 255) { avgR = 255; }
                    avgG = (int)((Ggrid[a, c] + Ggrid[i, c] + Ggrid[b, c] + 
                                  Ggrid[a, j] + Ggrid[i, j] + Ggrid[b, j] + 
                                  Ggrid[a, d] + Ggrid[i, d] + Ggrid[b, d]) / 9);
                    if (avgG < 0) { avgG = 0; }
                    if (avgG > 255) { avgG = 255; }
                    avgB = (int)((Bgrid[a, c] + Bgrid[i, c] + Bgrid[b, c] + 
                                  Bgrid[a, j] + Bgrid[i, j] + Bgrid[b, j] + 
                                  Bgrid[a, d] + Bgrid[i, d] + Bgrid[b, d]) / 9);
                    if (avgB < 0) { avgB = 0; }
                    if (avgB > 255) { avgB = 255; }
                    PSR[i, j] = avgR;
                    PSG[i, j] = avgG;
                    PSB[i, j] = avgB;
                    filteredmap.SetPixel(j, i, Color.FromArgb(PSR[i, j], PSG[i, j], PSB[i, j]));
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Rgrid[i, j] - PSR[i, j]) * (Rgrid[i, j] - PSR[i, j]) + (Ggrid[i, j] - PSG[i, j]) * (Ggrid[i, j] - PSG[i, j]) + (Bgrid[i, j] - PSB[i, j]) * (Bgrid[i, j] - PSB[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;
	        
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            int avgR;
            int avgG;
            int avgB;
            int[,] PSR = new int[fstxdim, fstydim];
            int[,] PSG = new int[fstxdim, fstydim];
            int[,] PSB = new int[fstxdim, fstydim];
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < fstydim; i++)
            {
                for (int j = 0; j < fstxdim; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;
                    int k = i - 2;
                    int f = i + 2;
                    int g = j - 2;
                    int h = j + 2;
                    if (a == -1) { a = fstydim - 1; }
                    if (b == fstydim) { b = 0; }
                    if (c == -1) { c = fstxdim - 1; }
                    if (d == fstxdim) { d = 0; }
                    if (k == -1) { k = fstydim - 1; }
                    if (k == -2) { k = fstydim - 2; }
                    if (f == fstydim) { f = 0; }
                    if (f == fstydim + 1) { f = 1; }
                    if (g == -1) { g = fstxdim - 1; }
                    if (g == -2) { g = fstxdim - 2; }
                    if (h == fstxdim) { h = 0; }
                    if (h == fstxdim + 1) { h = 1; }
                    avgR = (int)((Rgrid[k, g] + Rgrid[a, g] + Rgrid[i, g] + Rgrid[b, g] + Rgrid[f, g] +
                                  Rgrid[k, c] + Rgrid[a, c] + Rgrid[i, c] + Rgrid[b, c] + Rgrid[f, c] +
                                  Rgrid[k, j] + Rgrid[a, j] + Rgrid[i, j] + Rgrid[b, j] + Rgrid[f, j] +
                                  Rgrid[k, d] + Rgrid[a, d] + Rgrid[i, d] + Rgrid[b, d] + Rgrid[f, d] +
                                  Rgrid[k, h] + Rgrid[a, h] + Rgrid[i, h] + Rgrid[b, h] + Rgrid[f, h]) / 25);
                    if (avgR < 0) { avgR = 0; }
                    if (avgR > 255) { avgR = 255; }

                    avgG = (int)((Ggrid[k, g] + Ggrid[a, g] + Ggrid[i, g] + Ggrid[b, g] + Ggrid[f, g] +
                                  Ggrid[k, c] + Ggrid[a, c] + Ggrid[i, c] + Ggrid[b, c] + Ggrid[f, c] +
                                  Ggrid[k, j] + Ggrid[a, j] + Ggrid[i, j] + Ggrid[b, j] + Ggrid[f, j] +
                                  Ggrid[k, d] + Ggrid[a, d] + Ggrid[i, d] + Ggrid[b, d] + Ggrid[f, d] +
                                  Ggrid[k, h] + Ggrid[a, h] + Ggrid[i, h] + Ggrid[b, h] + Ggrid[f, h]) / 25);
                    if (avgG < 0) { avgG = 0; }
                    if (avgG > 255) { avgG = 255; }

                    avgB = (int)((Bgrid[k, g] + Bgrid[a, g] + Bgrid[i, g] + Bgrid[b, g] + Bgrid[f, g] +
                                  Bgrid[k, c] + Bgrid[a, c] + Bgrid[i, c] + Bgrid[b, c] + Bgrid[f, c] +
                                  Bgrid[k, j] + Bgrid[a, j] + Bgrid[i, j] + Bgrid[b, j] + Bgrid[f, j] +
                                  Bgrid[k, d] + Bgrid[a, d] + Bgrid[i, d] + Bgrid[b, d] + Bgrid[f, d] +
                                  Bgrid[k, h] + Bgrid[a, h] + Bgrid[i, h] + Bgrid[b, h] + Bgrid[f, h]) / 25);
                    if (avgB < 0) { avgB = 0; }
                    if (avgB > 255) { avgB = 255; }

                    PSR[i, j] = avgR;
                    PSG[i, j] = avgG;
                    PSB[i, j] = avgB;
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Rgrid[i, j] - PSR[i, j]) * (Rgrid[i, j] - PSR[i, j]) + (Ggrid[i, j] - PSG[i, j]) * (Ggrid[i, j] - PSG[i, j]) + (Bgrid[i, j] - PSB[i, j]) * (Bgrid[i, j] - PSB[i, j]);
                    filteredmap.SetPixel(j, i, Color.FromArgb(PSR[i, j], PSG[i, j], PSB[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            int avgR;
            int avgG;
            int avgB;
            int[,] PSR = new int[fstxdim, fstydim];
            int[,] PSG = new int[fstxdim, fstydim];
            int[,] PSB = new int[fstxdim, fstydim];
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < fstydim; i++)
            {
                for (int j = 0; j < fstxdim; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;
                    int k = i - 2;
                    int f = i + 2;
                    int g = j - 2;
                    int h = j + 2;
                    int m = i - 3;
                    int n = i + 3;
                    int p = j - 3;
                    int q = j + 3;
                    if (a == -1) { a = fstydim - 1; }
                    if (b == fstydim) { b = 0; }
                    if (c == -1) { c = fstxdim - 1; }
                    if (d == fstxdim) { d = 0; }
                    if (k == -1) { k = fstydim - 1; }
                    if (k == -2) { k = fstydim - 2; }
                    if (f == fstydim) { f = 0; }
                    if (f == fstydim + 1) { f = 1; }
                    if (g == -1) { g = fstxdim - 1; }
                    if (g == -2) { g = fstxdim - 2; }
                    if (h == fstxdim) { h = 0; }
                    if (h == fstxdim + 1) { h = 1; }
                    if (m==-1) {m=fstydim-1;}
                    if (m==-2) {m=fstydim-2;}
			        if (m==-3) {m=fstydim-3;}
			        if (n==fstydim) {n=0;}
			        if (n==fstydim+1) {n=1;}
			        if (n==fstydim+2) {n=2;}
			        if (p==-1) {p=fstxdim-1;}
			        if (p==-2) {p=fstxdim-2;}
			        if (p==-3) {p=fstxdim-3;}
			        if (q==fstxdim) {q=0;}
			        if (q==fstxdim+1) {q=1;} 
			        if (q==fstxdim+2) {q=2;}
                    avgR = (int)((Rgrid[m, p] + Rgrid[k, p] + Rgrid[a, p] + Rgrid[i, p] + Rgrid[b, p] + Rgrid[f, p] + Rgrid[n, p] +
                                  Rgrid[m, g] + Rgrid[k, g] + Rgrid[a, g] + Rgrid[i, g] + Rgrid[b, g] + Rgrid[f, g] + Rgrid[n, g] +
                                  Rgrid[m, c] + Rgrid[k, c] + Rgrid[a, c] + Rgrid[i, c] + Rgrid[b, c] + Rgrid[f, c] + Rgrid[n, c] +
                                  Rgrid[m, j] + Rgrid[k, j] + Rgrid[a, j] + Rgrid[i, j] + Rgrid[b, j] + Rgrid[f, j] + Rgrid[n, j] +
                                  Rgrid[m, d] + Rgrid[k, d] + Rgrid[a, d] + Rgrid[i, d] + Rgrid[b, d] + Rgrid[f, d] + Rgrid[n, d] +
                                  Rgrid[m, h] + Rgrid[k, h] + Rgrid[a, h] + Rgrid[i, h] + Rgrid[b, h] + Rgrid[f, h] + Rgrid[n, h] +
                                  Rgrid[m, q] + Rgrid[k, q] + Rgrid[a, q] + Rgrid[i, q] + Rgrid[b, q] + Rgrid[f, q] + Rgrid[n, q]) / 49);
                    if (avgR < 0) { avgR = 0; }
                    if (avgR > 255) { avgR = 255; }
                    avgG = (int)((Ggrid[m, p] + Ggrid[k, p] + Ggrid[a, p] + Ggrid[i, p] + Ggrid[b, p] + Ggrid[f, p] + Ggrid[n, p] +
                                  Ggrid[m, g] + Ggrid[k, g] + Ggrid[a, g] + Ggrid[i, g] + Ggrid[b, g] + Ggrid[f, g] + Ggrid[n, g] +
                                  Ggrid[m, c] + Ggrid[k, c] + Ggrid[a, c] + Ggrid[i, c] + Ggrid[b, c] + Ggrid[f, c] + Ggrid[n, c] +
                                  Ggrid[m, j] + Ggrid[k, j] + Ggrid[a, j] + Ggrid[i, j] + Ggrid[b, j] + Ggrid[f, j] + Ggrid[n, j] +
                                  Ggrid[m, d] + Ggrid[k, d] + Ggrid[a, d] + Ggrid[i, d] + Ggrid[b, d] + Ggrid[f, d] + Ggrid[n, d] +
                                  Ggrid[m, h] + Ggrid[k, h] + Ggrid[a, h] + Ggrid[i, h] + Ggrid[b, h] + Ggrid[f, h] + Ggrid[n, h] +
                                  Ggrid[m, q] + Ggrid[k, q] + Ggrid[a, q] + Ggrid[i, q] + Ggrid[b, q] + Ggrid[f, q] + Ggrid[n, q]) / 49);
                    if (avgG < 0) { avgG = 0; }
                    if (avgG > 255) { avgG = 255; }
                    avgB = (int)((Bgrid[m, p] + Bgrid[k, p] + Bgrid[a, p] + Bgrid[i, p] + Bgrid[b, p] + Bgrid[f, p] + Bgrid[n, p] +
                                  Bgrid[m, g] + Bgrid[k, g] + Bgrid[a, g] + Bgrid[i, g] + Bgrid[b, g] + Bgrid[f, g] + Bgrid[n, g] +
                                  Bgrid[m, c] + Bgrid[k, c] + Bgrid[a, c] + Bgrid[i, c] + Bgrid[b, c] + Bgrid[f, c] + Bgrid[n, c] +
                                  Bgrid[m, j] + Bgrid[k, j] + Bgrid[a, j] + Bgrid[i, j] + Bgrid[b, j] + Bgrid[f, j] + Bgrid[n, j] +
                                  Bgrid[m, d] + Bgrid[k, d] + Bgrid[a, d] + Bgrid[i, d] + Bgrid[b, d] + Bgrid[f, d] + Bgrid[n, d] +
                                  Bgrid[m, h] + Bgrid[k, h] + Bgrid[a, h] + Bgrid[i, h] + Bgrid[b, h] + Bgrid[f, h] + Bgrid[n, h] +
                                  Bgrid[m, q] + Bgrid[k, q] + Bgrid[a, q] + Bgrid[i, q] + Bgrid[b, q] + Bgrid[f, q] + Bgrid[n, q]) / 49);
                    if (avgB < 0) { avgB = 0; }
                    if (avgB > 255) { avgB = 255; }
                    PSR[i, j] = avgR;
                    PSG[i, j] = avgG;
                    PSB[i, j] = avgB;
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Rgrid[i, j] - PSR[i, j]) * (Rgrid[i, j] - PSR[i, j]) + (Ggrid[i, j] - PSG[i, j]) * (Ggrid[i, j] - PSG[i, j]) + (Bgrid[i, j] - PSB[i, j]) * (Bgrid[i, j] - PSB[i, j]);
                    filteredmap.SetPixel(j, i, Color.FromArgb(PSR[i, j], PSG[i, j], PSB[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;
        }
    }
}