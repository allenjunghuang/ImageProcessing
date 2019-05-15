using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Histogram : Form
    {
        public int latitude;
        public int longitude;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;
        public int[,] C2Ggrid;

        public Histogram(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();

            int[,] C2G = new int[xdim, ydim];            

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;                  
                }
            }

            latitude = xdim;
            longitude = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
            C2Ggrid = C2G;        
        }

        private void Histogram_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            int[] Rcount = new int[256];
            Bitmap histogram = new Bitmap(latitude, longitude);
            Color colorplane = Color.Red;

            for (int i = 0; i < 256; i++)
            {
                Rcount[i] = 0; 
            }

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (Rgrid[i, j] == k)
                        {
                            Rcount[k]++;
                        }
                    }
                }
            }

            int Rmax = 0;
            for (int i = 0; i < 256; i++)
            {
                if (Rcount[i] > Rmax)
                    Rmax = Rcount[i];
            }
            DrawHistogram(Rcount, Rmax, colorplane);

        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            int[] Gcount = new int[256];
            Bitmap histogram = new Bitmap(latitude, longitude);
            Color colorplane = Color.Green;

            for (int i = 0; i < 256; i++)
            {
                Gcount[i] = 0;
            }

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (Ggrid[i, j] == k)
                        {
                            Gcount[k]++;
                        }
                    }
                }
            }

            int Gmax = 0;
            for (int i = 0; i < 256; i++)
            {
                if (Gcount[i] > Gmax)
                    Gmax = Gcount[i];
            }
            DrawHistogram(Gcount, Gmax, colorplane);
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            
            int[] Bcount = new int[256];
            Bitmap histogram = new Bitmap(latitude, longitude);
            Color colorplane = Color.Blue;

            for (int i = 0; i < 256; i++)
            {
                Bcount[i] = 0;
            }

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (Bgrid[i, j] == k)
                        {
                            Bcount[k]++;
                        }
                    }
                }
            }

            int Bmax = 0;
            for (int i = 0; i < 256; i++)
            {
                if (Bcount[i] > Bmax)
                    Bmax = Bcount[i];
            }
            DrawHistogram(Bcount, Bmax, colorplane);          
           
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            
            int[] C2Gcount = new int[256];
            Bitmap histogram = new Bitmap(latitude, longitude);
            Color colorplane = Color.Gray;

            for (int i = 0; i < 256; i++)
            {
                C2Gcount[i] = 0;
            }

            for (int i = 0; i < longitude; i++)
            {
                for (int j = 0; j < latitude; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2Ggrid[i, j] == k)
                        {
                            C2Gcount[k]++;
                        }
                    }
                }
            }

            int C2Gmax = 0;
            for (int i = 0; i < 256; i++)
            {
                if (C2Gcount[i] > C2Gmax)
                    C2Gmax = C2Gcount[i];
            }
            DrawHistogram(C2Gcount, C2Gmax, colorplane);
                      
        }

       

        public void DrawHistogram(int[] PixCount, int maxCount, Color pencilcolor)
        {
            this.Refresh();
            Font chartFont = new Font("Arial", 10);
            int axisOffset = 20;
            float yUnit = (float)(this.pictureBox1.Height - (2 * axisOffset)) / maxCount;
            float xUnit = (float)(this.pictureBox1.Width - (2 * axisOffset)) / 255;                               
          
            Graphics chart = this.pictureBox1.CreateGraphics();
            Pen pencil = new Pen(pencilcolor, 2);
            int xPnt, yPnt;

            for (int i = 0; i < PixCount.Length; i++)
            {
                xPnt = axisOffset + (int)(i * xUnit);
                yPnt = this.pictureBox1.Height - axisOffset;
                chart.DrawLine(pencil, new PointF(xPnt, yPnt), new PointF(xPnt, yPnt - (int)(PixCount[i] * yUnit)));
            }

            chart.DrawRectangle(new System.Drawing.Pen(new SolidBrush(pencilcolor), 2), axisOffset, axisOffset, this.pictureBox1.Width - axisOffset * 2, this.pictureBox1.Height - axisOffset * 2);
            chart.DrawString("0", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset, this.pictureBox1.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
            chart.DrawString("255", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset + (256 * xUnit) - chart.MeasureString("255", chartFont).Width, this.pictureBox1.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
        }

        

      

      
    }
}