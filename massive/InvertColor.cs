using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class InvertColor : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;

        public int[,] SCgrid;

        public InvertColor(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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
            latitude = xdim;
            longitude = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            SCgrid = C2G;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap inversemap = new Bitmap(latitude, longitude);
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    inversemap.SetPixel(j, i, Color.FromArgb(255 - Rgrid[i, j], 255 - Ggrid[i, j], 255 - Bgrid[i, j]));
                    signal += ((Rgrid[i, j] * Rgrid[i, j]) + (Ggrid[i, j] * Ggrid[i, j]) + (Bgrid[i, j] * Bgrid[i, j]));
                    noise += ((255 - (2 * Rgrid[i, j])) * (255 - (2 * Rgrid[i, j]))) + ((255 - (2 * Ggrid[i, j])) * (255 - (2 * Ggrid[i, j]))) + ((255 - (2 * Bgrid[i, j])) * (255 - (2 * Bgrid[i, j])));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)inversemap.Width, (int)inversemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = inversemap;//put the map into picturebox
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap inversemap = new Bitmap(latitude, longitude);
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    inversemap.SetPixel(j, i, Color.FromArgb(255 - SCgrid[i, j], 255 - SCgrid[i, j], 255 - SCgrid[i, j]));
                    signal += (SCgrid[i, j] * SCgrid[i, j]);
                    noise += (255 - (2 * SCgrid[i, j])) * (255 - (2 * SCgrid[i, j]));
                }

            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)inversemap.Width, (int)inversemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = inversemap;//put the map into picturebox

        }

        private void InvertColor_Load(object sender, EventArgs e)
        {

        }
    }
}