using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class GrayScale : Form
    {
        public int xcord;
        public int ycord;
        int[,] C2Gpln;
        public GrayScale(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            C2Gpln = new int[xdim, ydim];
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2Gpln[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                }
            }
            xcord = xdim;
            ycord = ydim;                             
        }

        private void GrayScale_Load(object sender, EventArgs e)
        {
            
        }

        private void radioButton1_Click(object sender, EventArgs e) //1-bit
        {          
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i,j];
                    if (C2G[i, j] <= 125)
                    { C2G[i, j] = 0; }
                    else
                    { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton2_Click(object sender, EventArgs e)//2-bit
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];

                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 64;

                        if (C2G[i, j] < cls)
                        {
                            if (C2G[i, j] <= cls - 32)
                            { C2G[i, j] = cls - 64; }
                            else
                            { C2G[i, j] = cls; }
                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton3_Click(object sender, EventArgs e)//3-bit
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];
                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 32;

                        if (C2G[i,j] < cls)
                        {
                            if (C2G[i, j] <= cls - 16) 
                            { C2G[i, j] = cls - 32; }
                            else
                            { C2G[i, j] = cls; }
                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox

        }

        private void radioButton4_Click(object sender, EventArgs e)//4-bit
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];
                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 16;

                        if (C2G[i, j] < cls)
                        {
                            if (C2G[i, j] <= cls - 8)
                            { C2G[i, j] = cls - 16; }
                            else
                            { C2G[i, j] = cls; }
                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];

                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 8;
                        if (C2G[i, j] < cls)
                        {
                            if (C2G[i, j] <= cls - 4)
                            { C2G[i, j] = cls - 8; }
                            else
                            { C2G[i, j] = cls; }
                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];

                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 4;
                        if (C2G[i, j] < cls)
                        {
                            if (C2G[i, j] <= cls - 2)
                            { C2G[i, j] = cls - 4; }
                            else
                            { C2G[i, j] = cls; }

                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            int[,] C2G = new int[xcord, ycord];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    C2G[i, j] = C2Gpln[i, j];

                    int cls = 0;
                    while (cls < 255)
                    {
                        cls += 2;
                        if (C2G[i, j] < cls)
                        {
                            if (C2G[i, j] <= cls - 1)
                            { C2G[i, j] = cls - 2; }
                            else
                            { C2G[i, j] = cls; }
                            break;
                        }
                    }
                    if (C2G[i, j] > 255) { C2G[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += (C2Gpln[i, j] - C2G[i, j]) * (C2Gpln[i, j] - C2G[i, j]); 
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label3.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            Bitmap graymap = new Bitmap(xcord, ycord);
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    graymap.SetPixel(j, i, Color.FromArgb(C2Gpln[i, j], C2Gpln[i, j], C2Gpln[i, j]));
                }
            }

            label3.Text = "infinity";
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = graymap;//put the map into picturebox
        }       

        
    }
}