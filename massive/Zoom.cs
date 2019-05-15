using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Zoom : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;

        public Zoom(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            Bitmap sourcemap = new Bitmap (xdim, ydim);
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

        private void Zoom_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)//zoom in (duplication)
        {
            string factor = textBox1.Text;
            int scaling = Convert.ToInt16(factor);
            Bitmap scalingmap = new Bitmap(latitude*scaling, longitude*scaling);

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int h = 0; h < scaling; h++)
                    {
                        for (int k = 0; k < scaling; k++)
                        {
                            scalingmap.SetPixel(j * scaling + k, i * scaling + h, Color.FromArgb(Rgrid[i, j], Ggrid[i, j], Bgrid[i, j]));
                        }
                    }
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)scalingmap.Width, (int)scalingmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = scalingmap;//put the map into picturebox
        }

        private void button2_Click(object sender, EventArgs e) //zoom in (interpolation)
        {
            string factor = textBox1.Text;
            int scaling = Convert.ToInt16(factor);
            Bitmap scalingmap = new Bitmap(latitude * scaling, longitude * scaling);
            int[,] ZR = new int[latitude * scaling, longitude * scaling];
            int[,] ZG = new int[latitude * scaling, longitude * scaling];
            int[,] ZB = new int[latitude * scaling, longitude * scaling];
            for (int i = 0; i < longitude; i++) //建立原始放大影像(未填滿)
            {
                for (int j = 0; j < latitude; j++)
                {
                    ZR[i * scaling, j * scaling] = Rgrid[i,j];
                    ZG[i * scaling, j * scaling] = Ggrid[i,j];
                    ZB[i * scaling, j * scaling] = Bgrid[i,j];
                }
            }

            for (int i = 0; i < latitude; i++)
            {
                for (int j = 0; j < longitude * scaling; j++)
                {
                    for (int h = 1; h < scaling; h++)
                    {
                        if ((i * scaling) + scaling < latitude * scaling)
                        {
                            ZR[i * scaling + h, j] = ZR[i * scaling, j] + (h * ((ZR[i * scaling + scaling, j] - ZR[i * scaling, j]) / scaling));
                            ZG[i * scaling + h, j] = ZG[i * scaling, j] + (h * ((ZG[i * scaling + scaling, j] - ZG[i * scaling, j]) / scaling));
                            ZB[i * scaling + h, j] = ZB[i * scaling, j] + (h * ((ZB[i * scaling + scaling, j] - ZB[i * scaling, j]) / scaling));
                        }
                        else //邊緣會缺最後一個像素做內插，故將原始像素中最後一個像素與第一個像素做平均
                        {
                            ZR[i * scaling + h, j] = (ZR[0, j] + ZR[i * scaling, j]) / 2;
                            ZG[i * scaling + h, j] = (ZG[0, j] + ZG[i * scaling, j]) / 2;
                            ZB[i * scaling + h, j] = (ZB[0, j] + ZB[i * scaling, j]) / 2;
                        }
                    }
                }
            }

            for (int i = 0; i < latitude * scaling; i++)
            {
                for (int j = 0; j < longitude; j++)
                {
                    for (int k = 1; k < scaling; k++)
                    {
                        if ((j * scaling) + scaling < longitude * scaling)
                        {
                            ZR[i, j * scaling + k] = ZR[i, j * scaling] + (k * ((ZR[i, j * scaling + scaling] - ZR[i, j * scaling]) / scaling));
                            ZG[i, j * scaling + k] = ZG[i, j * scaling] + (k * ((ZG[i, j * scaling + scaling] - ZG[i, j * scaling]) / scaling));
                            ZB[i, j * scaling + k] = ZB[i, j * scaling] + (k * ((ZB[i, j * scaling + scaling] - ZB[i, j * scaling]) / scaling));
                        }
                        else //邊緣會缺最後一個像素做內插，故將原始像素中最後一個像素與第一個像素做平均
                        {
                            ZR[i, j * scaling + k] = (ZR[i, 0] + ZR[i, j * scaling]) / 2;
                            ZG[i, j * scaling + k] = (ZG[i, 0] + ZG[i, j * scaling]) / 2;
                            ZB[i, j * scaling + k] = (ZB[i, 0] + ZB[i, j * scaling]) / 2;
                        }
                    }
                }
            }

            for (int i = 0; i < longitude * scaling; i++)
            {
                for (int j = 0; j < latitude * scaling; j++)
                {
                    scalingmap.SetPixel(j, i, Color.FromArgb(ZR[i,j], ZG[i,j], ZB[i,j]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)scalingmap.Width, (int)scalingmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = scalingmap;//put the map into picturebox

        }

        private void button3_Click(object sender, EventArgs e) //zoom out (decimation)
        {
            string factor = textBox1.Text;
            int scaling = Convert.ToInt16(factor);
            Bitmap scalingmap = new Bitmap(latitude / scaling, longitude / scaling);

            for (int i = 0; i < longitude/scaling; i++)
            {
                for (int j = 0; j < latitude/scaling; j++)
                {
                    scalingmap.SetPixel(j, i, Color.FromArgb(Rgrid[i * scaling, j * scaling], Ggrid[i * scaling, j * scaling], Bgrid[i * scaling, j * scaling]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)scalingmap.Width, (int)scalingmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = scalingmap;//put the map into picturebox

        }

        private void button4_Click(object sender, EventArgs e) // zoom out (average)
        {
            string factor = textBox1.Text;
            int scaling = Convert.ToInt16(factor);
            Bitmap scalingmap = new Bitmap(latitude * scaling, longitude * scaling);

            int[,] ZR = new int[latitude * scaling, longitude * scaling];
            int[,] ZG = new int[latitude * scaling, longitude * scaling];
            int[,] ZB = new int[latitude * scaling, longitude * scaling];

            for (int i = 0; i < longitude / scaling; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int h = 0; h < scaling; h++)
                    {
                        if (i * scaling + h + 1 < longitude)
                        {
                            ZR[i, j] = (int)((Rgrid[i * scaling + h, j] + Rgrid[i * scaling + h + 1, j]) / 2);
                            ZG[i, j] = (int)((Ggrid[i * scaling + h, j] + Ggrid[i * scaling + h + 1, j]) / 2);
                            ZB[i, j] = (int)((Bgrid[i * scaling + h, j] + Bgrid[i * scaling + h + 1, j]) / 2);
                        }
                        else
                        {
                            ZR[i, j] = (int)((Rgrid[i * scaling + h, j] + Rgrid[0, j]) / 2);
                            ZG[i, j] = (int)((Ggrid[i * scaling + h, j] + Ggrid[0, j]) / 2);
                            ZB[i, j] = (int)((Bgrid[i * scaling + h, j] + Bgrid[0, j]) / 2);
                        }
                    }
                }
            }

            for (int i = 0; i < longitude/ scaling; i++)
            {
                for (int j = 0; j < latitude / scaling; j++)
                {
                    for (int k = 0; k < scaling; k++)
                    {
                        if (j * scaling + k + 1 < latitude)
                        {
                            ZR[i, j] = (int)((ZR[i, j * scaling + k] + ZR[i, j * scaling + k + 1]) / 2);
                            ZG[i, j] = (int)((ZG[i, j * scaling + k] + ZG[i, j * scaling + k + 1]) / 2);
                            ZB[i, j] = (int)((ZB[i, j * scaling + k] + ZB[i, j * scaling + k + 1]) / 2);
                        }
                        else
                        {
                            ZR[i, j] = (int)((ZR[i, j * scaling + k] + ZR[i, 0]) / 2);
                            ZG[i, j] = (int)((ZG[i, j * scaling + k] + ZG[i, 0]) / 2);
                            ZB[i, j] = (int)((ZB[i, j * scaling + k] + ZB[i, 0]) / 2);
                        }
                    }
                }
            }

            for (int i = 0; i < longitude / scaling; i++)
            {
                for (int j = 0; j < latitude / scaling; j++)
                {
                    scalingmap.SetPixel(j, i, Color.FromArgb(ZR[i,j], ZG[i,j], ZB[i,j]));
                }
            }
            
            pictureBox1.Size = new System.Drawing.Size((int)scalingmap.Width, (int)scalingmap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = scalingmap;//put the map into picturebox

        }

        
    }
}