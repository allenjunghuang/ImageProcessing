using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class HistogramEqualize : Form
    {
        int[] preCount;
        int[] postCount;
        int[] preCDF;
        int[] postCDF;
        int premax;
        int postmax;
        int preCDFmax;
        int postCDFmax;


        public HistogramEqualize(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            pictureBox1.Location = new Point(11, 32);
            pictureBox2.Location = new Point(285, 32);
            pictureBox4.Location = new Point(11, 328);
            pictureBox5.Location = new Point(285, 328);
            label2.Location = new Point(12, 305);
            label7.Location = new Point(170, 305);
            label8.Location = new Point(205, 303);
            panel2.Location = new Point(285, 300);
            int[,] C2G = new int[xdim, ydim];
            int[,] GEQ = new int[xdim, ydim];
            Bitmap graymap = new Bitmap(xdim, ydim);
            Bitmap equlmap = new Bitmap(xdim, ydim);                  
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
            int[] fstCount = new int[256];
            int[] fstPDF = new int[256];
            int[] fstCDF = new int[256];
            int[] scdCount = new int[256];
            int[] scdPDF = new int[256];
            int[] scdCDF = new int[256];
            int fstmax = 0;
            int fstCDFmax = 0;
            int scdmax = 0;
            int scdCDFmax = 0;

            for (int i = 0; i < 256; i++)
            {
                fstCount[i] = 0;
                fstPDF[i] = 0;
                fstCDF[i] = 0;
                scdCount[i] = 0;
                scdPDF[i] = 0;
                scdCDF[i] = 0;
            }

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2G[i, j] == k)
                        {
                            fstCount[k]++;
                        }
                    }
                }
            }

            for (int i = 0; i < 256; i++)
            {
                if (fstCount[i] > fstmax)
                    fstmax = fstCount[i];
            }

            //Cumulative Distibute Function
            for (int i = 1; i < 256; i++)
            {
                fstPDF[0] = fstCount[0];
                fstPDF[i] = fstPDF[i - 1] + fstCount[i];
            }
            for (int i = 0; i < 256; i++)
            {
                fstCDF[i] = (fstPDF[i] * (256 - 1) / (ydim * xdim));
            }
            
            for (int i = 0; i < 256; i++)
            {
                if (fstCDF[i] > fstCDFmax)
                    fstCDFmax = fstCDF[i];
            }
            
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (C2G[i,j] == k)
                        {
                            GEQ[i,j] = fstCDF[k];
                        }
                    }
                }
            }

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    equlmap.SetPixel(j, i, Color.FromArgb(GEQ[i, j], GEQ[i, j], GEQ[i, j]));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)equlmap.Width, (int)equlmap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = equlmap;

            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    for (int k = 0; k < 256; k++)
                    {
                        if (GEQ[i,j] == k)
                        {
                            scdCount[k]++;
                        }
                    }
                }
            }
            for (int i = 1; i < 256; i++)
            {
                scdPDF[0] = scdCount[0];
                scdPDF[i] = scdPDF[i - 1] + scdCount[i];
            }
            for (int i = 0; i < 256; i++)
            {
                scdCDF[i] = (scdPDF[i]) * 255 / (ydim * xdim);
            }
            for (int i = 0; i < 256; i++)
            {
                if (scdCount[i] > scdmax)
                    scdmax = scdCount[i];
            }
            for (int i = 0; i < 256; i++)
            {
                if (scdCDF[i] > scdCDFmax)
                    scdCDFmax = scdCDF[i];
            }
            preCount = fstCount;
            preCDF = fstCDF;
            premax = fstmax;
            postCount = scdCount;
            postCDF = scdCDF;
            postmax = scdmax;
            preCDFmax = fstCDFmax;
            postCDFmax = scdCDFmax;
            //SNR
            double signal = 0;
            double noise = 0;
            double SNR;
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    signal += C2G[i,j] * C2G[i,j];
                    noise += (GEQ[i,j] - C2G[i,j]) * (GEQ[i,j] - C2G[i,j]);
                }
            }

            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            

           
        }

        private void HistogramEqualize_Load(object sender, EventArgs e)
        {
            
        }        

       

        public void DrawOriginalChart(int[] PixCount, int maxCount, Color pencilcolor)
        {
            this.Refresh();
            Font chartFont = new Font("Arial", 10);
            int axisOffset = 20;
            float yUnit = (float)(this.pictureBox2.Height - (2 * axisOffset)) / maxCount;
            float xUnit = (float)(this.pictureBox2.Width - (2 * axisOffset)) / 255;

            Graphics chart = this.pictureBox2.CreateGraphics();
            Pen pencil = new Pen(pencilcolor, 2);
            int xPnt, yPnt;

            for (int i = 0; i < PixCount.Length; i++)
            {
                xPnt = axisOffset + (int)(i * xUnit);
                yPnt = this.pictureBox2.Height - axisOffset;
                chart.DrawLine(pencil, new PointF(xPnt, yPnt), new PointF(xPnt, yPnt - (int)(PixCount[i] * yUnit)));
            }

            chart.DrawRectangle(new System.Drawing.Pen(new SolidBrush(pencilcolor), 2), axisOffset, axisOffset, this.pictureBox2.Width - axisOffset * 2, this.pictureBox2.Height - axisOffset * 2);
            chart.DrawString("0", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset, this.pictureBox2.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
            chart.DrawString("255", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset + (256 * xUnit) - chart.MeasureString("255", chartFont).Width, this.pictureBox2.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
        }

        public void DrawProcessedChart(int[] PixCount, int maxCount, Color pencilcolor)
        {
            this.Refresh();
            Font chartFont = new Font("Arial", 10);
            int axisOffset = 20;
            float yUnit = (float)(this.pictureBox5.Height - (2 * axisOffset)) / maxCount;
            float xUnit = (float)(this.pictureBox5.Width - (2 * axisOffset)) / 255;

            Graphics chart = this.pictureBox5.CreateGraphics();
            Pen pencil = new Pen(pencilcolor, 2);
            int xPnt, yPnt;

            for (int i = 0; i < PixCount.Length; i++)
            {
                xPnt = axisOffset + (int)(i * xUnit);
                yPnt = this.pictureBox5.Height - axisOffset;
                chart.DrawLine(pencil, new PointF(xPnt, yPnt), new PointF(xPnt, yPnt - (int)(PixCount[i] * yUnit)));
            }

            chart.DrawRectangle(new System.Drawing.Pen(new SolidBrush(pencilcolor), 2), axisOffset, axisOffset, this.pictureBox5.Width - axisOffset * 2, this.pictureBox5.Height - axisOffset * 2);
            chart.DrawString("0", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset, this.pictureBox5.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
            chart.DrawString("255", chartFont, new SolidBrush(pencilcolor), new PointF(axisOffset + (256 * xUnit) - chart.MeasureString("255", chartFont).Width, this.pictureBox5.Height - chartFont.Height), System.Drawing.StringFormat.GenericDefault);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Color pencilcolor = Color.DarkBlue;
            DrawOriginalChart(preCount, premax, pencilcolor);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Color pencilcolor = Color.DarkCyan;
            DrawOriginalChart(preCDF, preCDFmax, pencilcolor);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Color pencilcolor = Color.DarkBlue;
            DrawProcessedChart(postCount, postmax, pencilcolor);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Color pencilcolor = Color.DarkCyan;
            DrawProcessedChart(postCDF, postCDFmax, pencilcolor);
        }

    }
}