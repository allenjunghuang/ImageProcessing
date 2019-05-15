using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class BitPlanes : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] SCgrid;
        public int[,] GCgrid;

        public BitPlanes(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            Bitmap sourcemap = new Bitmap(xdim, ydim);
            int[,] C2G = new int[xdim, ydim];
            int[,] GCM = new int[xdim, ydim];
            
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;

                    int bit8 = (((0X80) & (C2G[i, j])) / 128);
                    int bit7 = (((0X40) & (C2G[i, j])) / 64);
                    int bit6 = (((0X20) & (C2G[i, j])) / 32);
                    int bit5 = (((0X10) & (C2G[i, j])) / 16);
                    int bit4 = (((0X08) & (C2G[i, j])) / 8);
                    int bit3 = (((0X04) & (C2G[i, j])) / 4);
                    int bit2 = (((0X02) & (C2G[i, j])) / 2);
                    int bit1 = ((0X01) & (C2G[i, j]));

                    if (bit8 == bit7)
                        bit7 = 0;
                    else
                        bit7 = 1;

                    if (bit7 == bit6)
                        bit6 = 0;
                    else
                        bit6 = 1;
                    
                    if (bit6 == bit5)
                        bit5 = 0;
                    else
                        bit5 = 1;
                    
                    if (bit5 == bit4)
                        bit4 = 0;
                    else
                        bit4 = 1;
                    
                    if (bit4 == bit3)
                        bit3 = 0;
                    else
                        bit3 = 1;
                    
                    if (bit3 == bit2)
                        bit2 = 0;
                    else
                        bit2 = 1;
                    
                    if (bit2 == bit1)
                        bit1 = 0;
                    else
                        bit1 = 1;

                    GCM[i, j] = ((bit8 * 128) + (bit7 * 64) + (bit6 * 32) + (bit5 * 16) + (bit4 * 8) + (bit3 * 4) + (bit2 * 2) + bit1);
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            latitude = xdim;
            longitude = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            SCgrid = C2G;
            GCgrid = GCM;
            
        }

        private void BitPlanes_Load(object sender, EventArgs e)
        {
            
        }
     
        private void radioButton1_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X01) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X01) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X02) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X02) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X04) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X04) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X08) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X08) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X10) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X10) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X20) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X20) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X40) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X40) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            Bitmap straitcodemap = new Bitmap(latitude, longitude);
            Bitmap graycodemap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    if (((0X80) & (SCgrid[i, j])) == 0)
                        straitcodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        straitcodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));

                    if (((0X80) & (GCgrid[i, j])) == 0)
                        graycodemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        graycodemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = straitcodemap;

            pictureBox2.Size = new System.Drawing.Size((int)straitcodemap.Width, (int)straitcodemap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = straitcodemap;
        }




    }
}