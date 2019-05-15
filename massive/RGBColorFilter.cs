using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class RGBColorFilter : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;

        public RGBColorFilter(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            Bitmap sourcemap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            latitude = xdim;
            longitude = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
        }

        private void RGBColorFilter_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            Bitmap redmap = new Bitmap(latitude, longitude);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    redmap.SetPixel(j, i, Color.FromArgb(Rgrid[i, j], 0, 0));
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)redmap.Width, (int)redmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = redmap;//put the map into picturebox

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap greenmap = new Bitmap(latitude, longitude);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    greenmap.SetPixel(j, i, Color.FromArgb(0, Ggrid[i, j], 0));
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Rgrid[i, j] * Rgrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)greenmap.Width, (int)greenmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = greenmap;//put the map into picturebox

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bluemap = new Bitmap(latitude, longitude);
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    bluemap.SetPixel(j, i, Color.FromArgb(0,0,Bgrid[i, j]));
                    signal += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]);
                    noise += (Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)bluemap.Width, (int)bluemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = bluemap;//put the map into picturebox
            

        }
    }
}