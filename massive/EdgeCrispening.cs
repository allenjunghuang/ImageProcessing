using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class EdgeCrispening : Form
    {
        public int fstxdim;
        public int fstydim;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public EdgeCrispening(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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
        
        private void radioButton1_Click(object sender, EventArgs e)
        {
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            int[,] G2F = new int[fstxdim, fstydim];
            int crisper;
            double signal = 0;
            double noise = 0;
            double SNR;
            int m11 = 0; int m12 = -1; int m13 = 0;
            int m21 = -1; int m22 = 5; int m23 = -1;
            int m31 = 0; int m32 = -1; int m33 = 0;

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
                    crisper = (int)(((-1) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + (-1) * C2Ggrid[b, c] +
                                    (-1) * C2Ggrid[a, j] + (8) * C2Ggrid[i, j] + (-1) * C2Ggrid[b, j] +
                                    (-1) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + (-1) * C2Ggrid[b, d]) / 9);
                    if (crisper < 0) { crisper = 0; }
                    if (crisper > 255) { crisper = 255; }
                    G2F[i, j] = crisper;
                    filteredmap.SetPixel(j, i, Color.FromArgb(crisper, crisper, crisper));
                    signal += (C2Ggrid[i, j] * C2Ggrid[i, j]);
                    noise += (C2Ggrid[i, j] - G2F[i, j]) * (C2Ggrid[i, j] - G2F[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label10.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;

            label1.Text = "" + m11; label2.Text = "" + m12; label3.Text = "" + m13;
            label4.Text = "" + m21; label5.Text = "" + m22; label6.Text = "" + m23;
            label7.Text = "" + m31; label8.Text = "" + m32; label9.Text = "" + m33;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            int[,] G2F = new int[fstxdim, fstydim];
            int crisper;
            double signal = 0;
            double noise = 0;
            double SNR;
            int m11 = -1; int m12 = -1; int m13 = -1;
            int m21 = -1; int m22 = 9; int m23 = -1;
            int m31 = -1; int m32 = -1; int m33 = -1;

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

                    crisper = (int)(((-1) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + (-1) * C2Ggrid[b, c] +
                                    (-1) * C2Ggrid[a, j] + (8) * C2Ggrid[i, j] + (-1) * C2Ggrid[b, j] +
                                    (-1) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + (-1) * C2Ggrid[b, d]) / 9);
                    if (crisper < 0) { crisper = 0; }
                    if (crisper > 255) { crisper = 255; }
                    G2F[i, j] = crisper;
                    filteredmap.SetPixel(j, i, Color.FromArgb(crisper, crisper, crisper));
                    signal += (C2Ggrid[i, j] * C2Ggrid[i, j]);
                    noise += (C2Ggrid[i, j] - G2F[i, j]) * (C2Ggrid[i, j] - G2F[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label10.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;
            label1.Text = "" + m11; label2.Text = "" + m12; label3.Text = "" + m13;
            label4.Text = "" + m21; label5.Text = "" + m22; label6.Text = "" + m23;
            label7.Text = "" + m31; label8.Text = "" + m32; label9.Text = "" + m33;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            Bitmap filteredmap = new Bitmap(fstxdim, fstydim);
            int[,] G2F = new int[fstxdim, fstydim];
            int crisper;
            double signal = 0;
            double noise = 0;
            double SNR;
            int m11 = 1; int m12 = -2; int m13 = 1;
            int m21 = -2; int m22 = 5; int m23 = -2;
            int m31 = 1; int m32 = -2; int m33 = 1;

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

                    crisper = (int)(((-1) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + (-1) * C2Ggrid[b, c] +
                                    (-1) * C2Ggrid[a, j] + (8) * C2Ggrid[i, j] + (-1) * C2Ggrid[b, j] +
                                    (-1) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + (-1) * C2Ggrid[b, d]) / 9);
                    if (crisper < 0) { crisper = 0; }
                    if (crisper > 255) { crisper = 255; }
                    G2F[i, j] = crisper;
                    filteredmap.SetPixel(j, i, Color.FromArgb(crisper, crisper, crisper));
                    signal += (C2Ggrid[i, j] * C2Ggrid[i, j]);
                    noise += (C2Ggrid[i, j] - G2F[i, j]) * (C2Ggrid[i, j] - G2F[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label10.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)filteredmap.Width, (int)filteredmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = filteredmap;

            label1.Text = "" + m11; label2.Text = "" + m12; label3.Text = "" + m13;
            label4.Text = "" + m21; label5.Text = "" + m22; label6.Text = "" + m23;
            label7.Text = "" + m31; label8.Text = "" + m32; label9.Text = "" + m33;
        }

        private void EdgeCrispening_Load(object sender, EventArgs e)
        {

        }
    }
}