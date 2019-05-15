using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace massive
{
    public partial class HistogramSpecify : Form
    {
        
        int xcord;
        int ycord;
        int[,] C2Gpln;
        int[] CDF;
        int cnx =128; 
        int cny =128;       
        Pen pencil = new Pen(Color.Red, 2);
        ArrayList anchX = new ArrayList();
        ArrayList anchY = new ArrayList();    
        int[] ancharyX;
        int[] ancharyY;
       
        public HistogramSpecify(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();    
            int[,] C2G = new int[xdim, ydim];
            Bitmap graymap = new Bitmap(xdim, ydim);                        
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
            xcord = xdim;
            ycord = ydim;
            C2Gpln = C2G;
            // histogram
            int[] pixcnt = new int[256];
            int[] PDF = new int[256];
            CDF = new int[256];           
            int cntmax = 0;
            int CDFmax = 0;
            for (int i = 0; i < 256; i++)
            {
                pixcnt[i] = 0;
                PDF[i] = 0;
                CDF[i] = 0;
            }
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2G[i, j] == k)
                        {
                            pixcnt[k]++;
                        }
                    }
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (pixcnt[i] > cntmax)
                    cntmax = pixcnt[i];
            }
            for (int i = 1; i < 256; i++)
            {
                PDF[0] = pixcnt[0];
                PDF[i] = PDF[i - 1] + pixcnt[i];
            }
            for (int i = 0; i < 256; i++)
            {
                CDF[i] = (PDF[i] * (256 - 1) / (ydim * xdim));
            }
            for (int i = 0; i < 256; i++)
            {
                if (CDF[i] > CDFmax)
                    CDFmax = CDF[i];
            }
            anchX.Add(0); anchY.Add(255);
            anchX.Add(255); anchY.Add(0);
        }

        private void HistogramSpecify_Load(object sender, EventArgs e)
        {
           
        }

        private void radioButton1_Click(object sender, EventArgs e) //brightness images
        {
            Bitmap procesmap = new Bitmap(xcord, ycord);
            double [] inrPDF = new double [256];
            double [] inrCDF = new double[256];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            int[] Trn = new int[256];
            for (int i = 0; i < 256; i++) //desired PDF
            {
                inrPDF[i] = (double)i / 32640; //0+1+...+255 = (255+0)*255/2 = 32640
            }
            inrCDF[0] = inrPDF[0]; //desired CDF
            for (int k = 1; k < 256; k++)
            {
                inrCDF[k] = inrCDF[k-1] + inrPDF[k]*255;
            }
            
            for (int j = 0; j < 256; j++)
            {
                Trn[j] = 0;
                double minvalue = 255;
                for (int i = 0; i < 256; i++)
                {
                    if ((CDF[j] - inrCDF[i]) * (CDF[j] - inrCDF[i]) < minvalue)
                    {
                        minvalue = (CDF[j] - inrCDF[i]) * (CDF[j] - inrCDF[i]);
                        Trn[j] = i;
                    }
                }
            }
            
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int err = Trn[C2Gpln[i, j]];
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - err) * (C2Gpln[i, j] - err)); 
                    procesmap.SetPixel(j, i, Color.FromArgb(Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)procesmap.Width, (int)procesmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = procesmap;
            
        }

        private void radioButton2_Click(object sender, EventArgs e) //darkness images
        {
            Bitmap procesmap = new Bitmap(xcord, ycord);
            double[] dcrPDF = new double [256];
            double[] dcrCDF = new double [256];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            int [] Trn = new int [256];
            for (int i = 0; i < 256; i++) //desired PDF
            {
                dcrPDF[i] = (double)(255 - i) / 32640; //0+1+...+255 = (255+0)*255/2 = 32640
            }
            dcrCDF[0] = dcrPDF[0]; //desired CDF
            for (int k = 1; k < 256; k++)
            {
                dcrCDF[k] = dcrCDF[k - 1] + dcrPDF[k]*255;
            }
            for (int j = 0; j < 256; j++)
            {
                Trn[j] = 0;
                double minvalue = 255;
                for (int i = 0; i < 256; i++)
                {
                    if ((CDF[j] - dcrCDF[i]) * (CDF[j] - dcrCDF[i]) < minvalue)
                    {
                        minvalue = (CDF[j] - dcrCDF[i]) * (CDF[j] - dcrCDF[i]);
                        Trn[j] = i;
                    }
                }
            }
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int err = Trn[C2Gpln[i, j]];
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - err) * (C2Gpln[i, j] - err)); 
                    procesmap.SetPixel(j, i, Color.FromArgb(Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)procesmap.Width, (int)procesmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = procesmap;
        }

        private void radioButton3_Click(object sender, EventArgs e) //high contrast images
        {
            Bitmap procesmap = new Bitmap(xcord, ycord);
            double[] nchPDF = new double[256];
            double[] nchCDF = new double[256];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            int[] Trn = new int[256];
            for(int i=127; i>=0; i--)
            {            
                nchPDF[i] = (double)(128-i) / 16512;
            }
            for(int i=128;i<256;i++)
            {
                nchPDF[i] = (double)(i-127) / 16512;
            }
            for (int i = 1; i < 256; i++)
            {
                nchCDF[i] = 0;
                nchCDF[i] = nchCDF[i - 1] + nchPDF[i] * 255;
            }
            for (int j = 0; j < 256; j++)
            {
                Trn[j] = 0;
                double minvalue = 255;
                for (int i = 0; i < 256; i++)
                {
                    if ((CDF[j] - nchCDF[i]) * (CDF[j] - nchCDF[i]) < minvalue)
                    {
                        minvalue = (CDF[j] - nchCDF[i]) * (CDF[j] - nchCDF[i]);
                        Trn[j] = i;
                    }
                }
            }

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int err = Trn[C2Gpln[i, j]];
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - err) * (C2Gpln[i, j] - err)); 
                    procesmap.SetPixel(j, i, Color.FromArgb(Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)procesmap.Width, (int)procesmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = procesmap;

        }

        private void radioButton4_Click(object sender, EventArgs e) //low contrast images
        {
            Bitmap procesmap = new Bitmap(xcord, ycord);
            double[] bndPDF = new double[256];
            double[] bndCDF = new double[256];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            int[] Trn = new int[256];

            for (int i = 0; i <=127; i++)
            {
                bndPDF[i] = (double)(i+1) / 16512;
            }
            for (int i = 128; i < 256; i++)
            {
                bndPDF[i] = (double)(256-i) / 16512;
            }
            for (int i = 1; i < 256; i++)
            {
                bndCDF[i] = 0;
                bndCDF[i] = bndCDF[i - 1] + bndPDF[i] * 255;
            }
            for (int j = 0; j < 256; j++)
            {
                Trn[j] = 0;
                double minvalue = 255;
                for (int i = 0; i < 256; i++)
                {
                    if ((CDF[j] - bndCDF[i]) * (CDF[j] - bndCDF[i]) < minvalue)
                    {
                        minvalue = (CDF[j] - bndCDF[i]) * (CDF[j] - bndCDF[i]);
                        Trn[j] = i;
                    }
                }
            }

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int err = Trn[C2Gpln[i, j]];
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - err) * (C2Gpln[i, j] - err)); 
                    procesmap.SetPixel(j, i, Color.FromArgb(Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]]));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)procesmap.Width, (int)procesmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = procesmap;
        }
       
        private void button1_Click(object sender, EventArgs e) //show contrast
        {
            Bitmap procesmap = new Bitmap(xcord, ycord);
            int[,] DSG = new int[xcord, ycord];
            double[] dsrPDF = new double[256];
            double[] dsrCDF = new double[256];
            int[] Trn = new int[256];
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int j = 0; j < ancharyY.Length-1; j++)
            {
                int srtx = ancharyX[j];
                int endx = ancharyX[j + 1];
                int srty = ancharyY[j];
                double slope = ((ancharyY[j + 1]) - (ancharyY[j])) / (ancharyX[j + 1] - ancharyX[j]);
                for (int i = srtx; i < endx; i++)
                {
                    dsrPDF[i] = srty+(slope *(i-srtx));
                }
            }
            /*
            for (int j = 0; j < 256; j++)
            {
                Trn[j] = 0;
                double minvalue = 255;
                for (int i = 0; i < 256; i++)
                {
                    if ((CDF[j] - dsrPDF[i]) * (CDF[j] - dsrPDF[i]) < minvalue)
                    {
                        minvalue = (CDF[j] - dsrPDF[i]) * (CDF[j] - dsrPDF[i]);
                        Trn[j] = i;
                    }
                }
            }
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int err = Trn[C2Gpln[i, j]];
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - err) * (C2Gpln[i, j] - err));
                    procesmap.SetPixel(j, i, Color.FromArgb(Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]], Trn[C2Gpln[i, j]]));
                }
            }
            */
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2Gpln[i, j] == k)
                        {
                            DSG[i, j] = (int)dsrPDF[k];
                        }
                    }
                }
            }
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    if (DSG[i, j] < 0) { DSG[i, j] = 0; }
                    if (DSG[i, j] > 255) { DSG[i, j] = 255; }
                    signal += (C2Gpln[i, j] * C2Gpln[i, j]);
                    noise += ((C2Gpln[i, j] - DSG[i, j]) * (C2Gpln[i, j] - DSG[i, j]));
                    procesmap.SetPixel(j, i, Color.FromArgb(DSG[i, j], DSG[i, j], DSG[i, j]));
                }

            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox1.Size = new System.Drawing.Size((int)procesmap.Width, (int)procesmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = procesmap;           
            anchX.Clear();
            anchY.Clear();            
        }

        private void button2_Click(object sender, EventArgs e) //resume
        {
            Bitmap graymap = new Bitmap(xcord, ycord);

            for (int i = 0; i < xcord; i++)
            {
                for (int j = 0; j < ycord; j++)
                {
                    graymap.SetPixel(j, i, Color.FromArgb(C2Gpln[i, j], C2Gpln[i, j], C2Gpln[i, j]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = graymap;
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            cnx = e.X; cny = e.Y;
            anchX.Add(cnx);
            anchY.Add(cny);
            pictureBox2.Refresh();
            toolStripStatusLabel2.Text = "" + e.X;
            toolStripStatusLabel3.Text = "" + (255-e.Y);          
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics slope = e.Graphics;          

            int anchlength = anchX.Count;
            ancharyX = new int [anchlength];
            ancharyY = new int [anchlength];

            anchX.CopyTo(ancharyX);
            anchY.CopyTo(ancharyY);

            Array.Sort(ancharyX,ancharyY);
           
            for (int j = 0; j < anchlength-1; j++)
            {
                int x0 = ancharyX[j];
                int y0 = ancharyY[j];    
                int x1 = ancharyX[j+1];
                int y1 = ancharyY[j+1];
                slope.DrawLine(pencil, x0, y0, x1, y1);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            anchX.Clear();
            anchY.Clear();

            anchX.Add(0); anchY.Add(255);
            anchX.Add(255); anchY.Add(0);

            pictureBox2.Refresh();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel5.Text = "" + e.X;
            toolStripStatusLabel6.Text = "" + (255 - e.Y);        
        }


    }
}