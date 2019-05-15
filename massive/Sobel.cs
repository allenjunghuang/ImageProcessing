using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Sobel : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;
      

        public Sobel(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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

            latitude = xdim;
            longitude = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G;        
            
        }

        private void Sobel_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for(int i =0;i<longitude;i++)
            {
                for(int j=0;j<latitude;j++)
                    {
                    int a=i-1;
			        int b=i+1;
			        int c=j-1;
			        int d=j+1;

         			if(a==-1) {a=longitude-1;}
		        	if(b==longitude) {b=0;}
			        if(c==-1) {c=latitude-1;}
			        if(d==latitude) {d=0;}

                    avg = (int)((-1) * C2Ggrid[a,c] + (-2) * C2Ggrid[i,c] + (-1) * C2Ggrid[b,c] +
                                  0 * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 0 * C2Ggrid[b,j] +
                                  1 * C2Ggrid[a, d] + 2 * C2Ggrid[i, d] + 1 * C2Ggrid[b,d]);
			
			        if(avg>255) {avg=255;}
		        	if(avg<0) {avg=0;}

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
		        	signal += C2Ggrid[i,j] * C2Ggrid[i,j];
			        noise += (avg-C2Ggrid[i,j]) * (avg-C2Ggrid[i,j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise),2);         
            label3.Text= ""+ SNR;
            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;


        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = longitude - 1; }
                    if (b == longitude) { b = 0; }
                    if (c == -1) { c = latitude - 1; }
                    if (d == latitude) { d = 0; }

                    avg = (int)((-1) * C2Ggrid[a, c] + 0 * C2Ggrid[i, c] + 1 * C2Ggrid[b, c] +
                                (-2) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 2 * C2Ggrid[b, j] +
                                (-1) * C2Ggrid[a, d] + 0 * C2Ggrid[i, d] + 1 * C2Ggrid[b, d]);

                    if (avg > 255) { avg = 255; }
                    if (avg < 0) { avg = 0; }

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (avg - C2Ggrid[i, j]) * (avg - C2Ggrid[i, j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avgvert;
            int avghorz;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = longitude - 1; }
                    if (b == longitude) { b = 0; }
                    if (c == -1) { c = latitude - 1; }
                    if (d == latitude) { d = 0; }

                    avgvert = (int)((-1) * C2Ggrid[a, c] + (-2) * C2Ggrid[i, c] + (-1) * C2Ggrid[b, c] +
                                       0 * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 0 * C2Ggrid[b, j] +
                                       1 * C2Ggrid[a, d] + 2 * C2Ggrid[i, d] + 1 * C2Ggrid[b, d]);

                    avghorz = (int)((-1) * C2Ggrid[a, c] + 0 * C2Ggrid[i, c] + 1 * C2Ggrid[b, c] +
                                    (-2) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 2 * C2Ggrid[b, j] +
                                    (-1) * C2Ggrid[a, d] + 0 * C2Ggrid[i, d] + 1 * C2Ggrid[b, d]);

                    avg = (int) Math.Sqrt((avgvert * avgvert) + (avghorz * avghorz));

                    if (avg > 255) { avg = 255; }
                    if (avg < 0) { avg = 0; }

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (avg - C2Ggrid[i, j]) * (avg - C2Ggrid[i, j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = longitude - 1; }
                    if (b == longitude) { b = 0; }
                    if (c == -1) { c = latitude - 1; }
                    if (d == latitude) { d = 0; }

                    avg = (int)((-2) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + 0 * C2Ggrid[b, c] +
                                (-1) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 1 * C2Ggrid[b, j] +
                                  0 * C2Ggrid[a, d] + 1 * C2Ggrid[i, d] + 2 * C2Ggrid[b, d]);

                    if (avg > 255) { avg = 255; }
                    if (avg < 0) { avg = 0; }

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (avg - C2Ggrid[i, j]) * (avg - C2Ggrid[i, j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;

        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = longitude - 1; }
                    if (b == longitude) { b = 0; }
                    if (c == -1) { c = latitude - 1; }
                    if (d == latitude) { d = 0; }

                    avg = (int)(  0 * C2Ggrid[a, c] + 1 * C2Ggrid[i, c] + 2 * C2Ggrid[b, c] +
                                (-1) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 1 * C2Ggrid[b, j] +
                                (-2) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + 0 * C2Ggrid[b, d]);

                    if (avg > 255) { avg = 255; }
                    if (avg < 0) { avg = 0; }

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (avg - C2Ggrid[i, j]) * (avg - C2Ggrid[i, j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;

        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            double signal = 0;
            double noise = 0;
            double SNR;
            int avg45;
            int avg135;
            int avg;
            Bitmap sobelmap = new Bitmap(latitude, longitude);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    int a = i - 1;
                    int b = i + 1;
                    int c = j - 1;
                    int d = j + 1;

                    if (a == -1) { a = longitude - 1; }
                    if (b == longitude) { b = 0; }
                    if (c == -1) { c = latitude - 1; }
                    if (d == latitude) { d = 0; }

                    avg45 = (int)((-2) * C2Ggrid[a, c] + (-1) * C2Ggrid[i, c] + 0 * C2Ggrid[b, c] +
                                  (-1) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 1 * C2Ggrid[b, j] +
                                    0 * C2Ggrid[a, d] + 1 * C2Ggrid[i, d] + 2 * C2Ggrid[b, d]);

                    avg135 = (int)( 0 * C2Ggrid[a, c] + 1 * C2Ggrid[i, c] + 2 * C2Ggrid[b, c] +
                                  (-1) * C2Ggrid[a, j] + 0 * C2Ggrid[i, j] + 1 * C2Ggrid[b, j] +
                                  (-2) * C2Ggrid[a, d] + (-1) * C2Ggrid[i, d] + 0 * C2Ggrid[b, d]);

                    avg = (int)Math.Sqrt((avg45 * avg45) + (avg135 * avg135));

                    if (avg > 255) { avg = 255; }
                    if (avg < 0) { avg = 0; }

                    sobelmap.SetPixel(j, i, Color.FromArgb(avg, avg, avg));
                    signal += C2Ggrid[i, j] * C2Ggrid[i, j];
                    noise += (avg - C2Ggrid[i, j]) * (avg - C2Ggrid[i, j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);

            label3.Text = "" + SNR;

            pictureBox1.Size = new System.Drawing.Size((int)sobelmap.Width, (int)sobelmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sobelmap;
        }
    }
}