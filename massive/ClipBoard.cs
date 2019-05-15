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
    public partial class ClipBoard : Form
    {
        public int xcord;
        public int ycord;
        public int[,] Rplane;
        public int[,] Gplane;
        public int[,] Bplane;

        Graphics canvas;
        Pen pencil = new Pen(Color.White,1);
        Bitmap painter;
        int cutmode;
        int startx; int starty;
        int x0; int y0;
        int endx; int endy;
        int tracksize;

        ArrayList circleX = new ArrayList();
        ArrayList circleY = new ArrayList();  
        
        ArrayList trackX = new ArrayList();
        ArrayList trackY = new ArrayList();  
        

        public ClipBoard(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();

            int[,] C2G = new int[xdim, ydim];
            Bitmap sourcemap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {                  
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)sourcemap.Width, (int)sourcemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = sourcemap;


            xcord = xdim;
            ycord = ydim;
            Rplane = Rdim;
            Gplane = Gdim;
            Bplane = Bdim;
            
        }

        private void ClipBoard_Load(object sender, EventArgs e)
        {
            canvas = this.pictureBox1.CreateGraphics();
            painter = new Bitmap(xcord, ycord);    

            
        }

        

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {               
            int trackerx = e.X; int trackery = e.Y;
            string stingtrackerx, stingtrackery;
            stingtrackerx = trackerx.ToString();
            stingtrackery = trackery.ToString();
            toolStripStatusLabel2.Text = stingtrackerx;
            toolStripStatusLabel3.Text = stingtrackery;

            if (e.Button == MouseButtons.Left) // mouse left down
            {
                canvas.DrawImage(painter, 0, 0);
                //Rectangle rect = new Rectangle(Math.Min(startx, e.X), Math.Min(starty, e.Y), Math.Abs(e.X - startx), Math.Abs(e.Y - starty));

                switch (cutmode)
                {
                    case 1:
                        //canvas.DrawRectangle(pencil, rect);
                        break;

                    case 2:
                        //canvas.DrawEllipse(pencil, rect);
                        break;

                    case 3:

                        Graphics hands = Graphics.FromImage(pictureBox1.Image);
                        hands.DrawLine(pencil, x0, y0, e.X, e.Y);

                        x0 = e.X; y0 = e.Y;
                        pictureBox1.Refresh();

                        trackX.Add(e.X);
                        trackY.Add(e.Y);
                        tracksize = trackX.Count;
                        toolStripStatusLabel11.Text = "" + tracksize;

                        break;
                }
            }
            
                
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startx = e.X; starty = e.Y;
            x0 = e.X; y0 = e.Y;
            string stingstartx, stingstarty;
            stingstartx = startx.ToString();
            stingstarty = starty.ToString();
            toolStripStatusLabel5.Text = stingstartx;
            toolStripStatusLabel6.Text = stingstarty;

            trackX.Clear();
            trackY.Clear();
            circleX.Clear();
            circleY.Clear();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {            
            endx = e.X; endy = e.Y;

            string stingendx, stingendy;
            stingendx = endx.ToString();
            stingendy = endy.ToString();
            toolStripStatusLabel8.Text = stingendx;
            toolStripStatusLabel9.Text = stingendy;

            Bitmap clipmap = new Bitmap(xcord, ycord);
            int clipxdim = Math.Abs(startx - endx);
            int clipydim = Math.Abs(starty - endy);
            int clipx0 = startx - (startx - endx) / 2;
            int clipy0 = starty - (starty - endy) / 2;

            Rectangle rect = new Rectangle(Math.Min(startx, endx), Math.Min(starty, endy), Math.Abs(endx - startx), Math.Abs(endy - starty));

            switch (cutmode)
            {
                case 1: //rectangle
                    
                    canvas.DrawRectangle(pencil, rect);                 
                    int[,] domain = new int[xcord, ycord];
                    
                    for (int j = 0; j < ycord; j++)
                    {
                        if (j < starty || j > endy)
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                domain[i, j] = 0;
                            }
                        }

                        else
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                if (i > startx && i < endx)
                                { domain[i, j] = 1; }
                                else
                                { domain[i, j] = 0; }
                            }
                        }
                    }

                    for (int j = 0; j < ycord; j++)
                    {
                        for (int i = 0; i < xcord; i++)
                        {

                            clipmap.SetPixel(j, i, Color.FromArgb(Rplane[i, j] * domain[j, i], Gplane[i, j] * domain[j, i], Bplane[i, j] * domain[j, i]));
                        }
                    }
                    pictureBox2.Size = new System.Drawing.Size((int)clipmap.Width, (int)clipmap.Height);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image = clipmap;         

                    break;

                case 2: //circle
                    canvas.DrawEllipse(pencil, rect);   
               
                    int elipa = clipxdim / 2;
                    int elipb = clipydim / 2;
                    
                    for (int theta = 0; theta < 360; theta++)
                    {
                        double radian = theta *(Math.PI/ 180);
                        int tempx = (int)(Math.Round(clipx0 + elipa * Math.Cos(radian)));
                        int tempy = (int)(Math.Round(clipy0 - elipb * Math.Sin(radian)));                     
                        circleX.Add(tempx);
                        circleY.Add(tempy);                      
                    }            

                    CheckCircleContinue();

                    circleX.Insert(0, 0);
                    circleY.Insert(0, 0);                 

                    int[] validpoint;
                    int[,] sphere = new int[xcord, ycord];                   
                    int[] sphereY = new int[circleY.Count];

                    int[,] Rcut = new int[xcord, ycord];
                    int[,] Gcut = new int[xcord, ycord];
                    int[,] Bcut = new int[xcord, ycord];

                    circleY.CopyTo(sphereY);
                    Array.Sort(sphereY);

                    int maxelipy = sphereY[sphereY.Length - 1];
                    int minelipy = sphereY[1];

                    for (int j = 0; j < ycord; j++)
                    {
                        if (j < minelipy || j > maxelipy)
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                sphere[i, j] = 0;
                               
                            }
                        }

                        else
                        {
                            validpoint = SearchArray(circleY, j);
                            Array.Sort(validpoint);

                            int cindx1 = (int)validpoint[validpoint.Length - 1];
                            int cindx2 = (int)validpoint[1];
                            int crtcx1 = (int)circleX[cindx1];
                            int crtcx2 = (int)circleX[cindx2];

                            int mincrx = Math.Min(crtcx1, crtcx2);
                            int maxcrx = Math.Max(crtcx1, crtcx2);

                            for (int i = 0; i < xcord; i++)
                            {
                                if (i > mincrx && i < maxcrx)
                                { sphere[i, j] = 1; }
                                    
                                
                                else
                                { sphere[i, j] = 0; }                              
                                
                            }
                        }

                    }       
                   

                    for (int j = 0; j < xcord; j++)
                    {
                        for (int i = 0; i < ycord; i++)
                        {

                            clipmap.SetPixel(j, i, Color.FromArgb(Rplane[i, j] * sphere[j, i], Gplane[i, j] * sphere[j, i], Bplane[i, j] * sphere[j, i]));
                            
                        }
                    }
                    pictureBox2.Size = new System.Drawing.Size((int)clipmap.Width, (int)clipmap.Height);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image = clipmap;

                    break;

                case 3: //freehand

                    CheckContinue(); // check if the dots are continuous

                    trackX.Insert(0, 0);
                    trackY.Insert(0, 0);                    
                    
                    int [] loopX = new int [trackX.Count];
                    int [] loopY = new int[trackY.Count];
                    int [] validY;
                    int [] validX;
                    int [,] xscanner = new int[xcord, ycord];
                    int[,] yscanner = new int[xcord, ycord];
 
                    trackX.CopyTo(loopX);
                    trackY.CopyTo(loopY);

                    //scan y-direction first ,than scan x-direction 
                    Array.Sort(loopY);
            
                    int maxscany = loopY[loopY.Length -1];
                    int minscany = loopY[1];
                    
                    for (int j = 0; j < ycord; j++)
                    {
                        if(j < minscany || j > maxscany)
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                xscanner[i, j] = 0;
                            }
                        }

                        else
                        {
                            validY=SearchArray(trackY,j);
                            Array.Sort(validY);

                            int findx1 = (int)validY[validY.Length - 1];
                            int findx2 = (int)validY[1];
                            int scanx1 = (int)trackX[findx1];
                            int scanx2 = (int)trackX[findx2];
                            int maxsnx = Math.Max(scanx1, scanx2);
                            int minsnx = Math.Min(scanx1, scanx2);

                            for (int i = 0; i < xcord; i++)
                            {
                                if(i > minsnx && i < maxsnx)
                                { xscanner[i, j] = 1; }
                                else
                                { xscanner[i, j] = 0; }
                            }
                        }
                    }

                    //scan x-direction first ,than scan y-direction

                    /*Array.Sort(loopX);

                    int maxscanx = loopX[loopX.Length - 1];
                    int minscanx = loopX[1];

                    for (int i = 0; i < xcord; i++)
                    {
                        if (i < minscanx || i > maxscanx)
                        {
                            for (int j = 0; j < xcord; j++)
                            {
                                yscanner[i, j] = 0;
                            }
                        }

                        else
                        {
                            validX = SearchArray(trackX, i);
                            Array.Sort(validX);

                            int findy1 = (int)validX[validX.Length - 1];
                            int findy2 = (int)validX[1];
                            int scany1 = (int)trackY[findy1];
                            int scany2 = (int)trackY[findy2];
                            int maxsny = Math.Max(scany1, scany2);
                            int minsny = Math.Min(scany1, scany2);

                            for (int j = 0; j < ycord; j++)
                            {
                                if (j > minsny && j < maxsny)
                                { yscanner[i, j] = 1; }
                                else
                                { yscanner[i, j] = 0; }
                            }
                        }
                    }*/

                    for (int j = 0; j < xcord; j++)
                    {
                        for (int i = 0; i < ycord; i++)
                        {
                            int select = xscanner[j, i]; //* yscanner[j, i];
                            if (select > 1) { select = 1; }
                            clipmap.SetPixel(j, i, Color.FromArgb(Rplane[i, j] * select, Gplane[i, j] * select, Bplane[i, j] * select));
                        }
                    }
                    pictureBox2.Size = new System.Drawing.Size((int)clipmap.Width, (int)clipmap.Height);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Image = clipmap;
                            

                    break;
            }

            
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            cutmode = 1;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            cutmode = 2;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            cutmode = 3;
        }


        private void button1_Click(object sender, EventArgs e) //clear images
        {
            Bitmap clearmap = new Bitmap(xcord, ycord);
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    clearmap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
            pictureBox2.Size = new System.Drawing.Size((int)clearmap.Width, (int)clearmap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = clearmap;

            trackX.Clear();
            trackY.Clear();
        }

        
        private int [] SearchArray(ArrayList list, int target)
        {
            ArrayList arryindx = new ArrayList();
            arryindx.Clear();
            int indx = 1;
            while (indx > 0)
            {
          
                indx = list.IndexOf(target, indx);
                arryindx.Add(indx);
                indx++;

            }

            int [] indxlist = (int []) arryindx.ToArray(typeof (int));
            return indxlist;
        }

        void CheckContinue()
        {
            int j=0;
            for (int i = 0; i < trackX.Count; i++)
            {
                int interx;
                if ((i + 1) > trackX.Count -1)
                { interx = (int)trackX[i] - (int)trackX[0]; }
                else
                { interx = (int)trackX[i] - (int)trackX[i + 1]; }
                

                if (interx > 1 || interx < -1)
                {
                    int insrtx = (int)trackX[i] - (interx / Math.Abs(interx));                  
                    trackX.Insert(i + 1, insrtx);
                    trackY.Insert(i + 1, trackY[j]);
                }
                j++;
            }

            int n=0;
            for (int m = 0; m < trackY.Count; m++)
            {
                int intery;
                if ((m + 1) > trackY.Count -1)
                { intery = (int)trackY[m] - (int)trackY[0]; }
                else
                { intery = (int)trackY[m] - (int)trackY[m + 1]; }

                
                if (intery > 1 || intery < -1)
                {
                    int insrty = (int)trackY[m] - (intery / Math.Abs(intery));
                    trackY.Insert(m + 1, insrty);
                    trackX.Insert(n + 1, trackX[n]);
                }
                n++;
            }


        }

        void CheckCircleContinue()
        {
            int j = 0;
            for (int i = 0; i < circleX.Count; i++)
            {
                int interx;
                if ((i + 1) > circleX.Count - 1)
                { interx = (int)circleX[i] - (int)circleX[0]; }
                else
                { interx = (int)circleX[i] - (int)circleX[i + 1]; }

                if (interx > 1 || interx < -1)
                {
                    int insrtx = (int)circleX[i] - (interx / Math.Abs(interx));
                    circleX.Insert(i + 1, insrtx);
                    circleY.Insert(j + 1, circleY[j]);
                }
                j++;
            }

            int n = 0;
            for (int m = 0; m < circleY.Count; m++)
            {
                int intery;
                if ((m + 1) > circleY.Count - 1)
                { intery = (int)circleY[m] - (int)circleY[0]; }
                else
                { intery = (int)circleY[m] - (int)circleY[m + 1]; }

                if (intery > 1 || intery < -1)
                {
                    int insrty = (int)circleY[m] - (intery / Math.Abs(intery));
                    circleY.Insert(m + 1, insrty);
                    circleX.Insert(n + 1, circleX[n]);
                }
                n++;
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }





    }
}