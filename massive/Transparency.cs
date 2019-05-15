using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Collections;


namespace massive
{
    public partial class Transparency : Form
    {
        OpenFileDialog path = new OpenFileDialog();

        string ImagePath = "";
        int alpha;
        
        public Bitmap fstlayer;
        public int fstxdim;
        public int fstydim;
        public int[,] fstR;
        public int[,] fstG;
        public int[,] fstB;

        public Bitmap scdlayer;
        public int scdxdim;
        public int scdydim;
        public int[,] scdR;
        public int[,] scdG;
        public int[,] scdB;
        Bitmap[] ovlmap;
        int shaft;
        // cutting function
        public int[,] cutR;
        public int[,] cutG;
        public int[,] cutB;
        Graphics canvas;
        Pen pencil = new Pen(Color.White, 1);
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
        int xcord;
        int ycord;

        public Transparency(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            Bitmap fstmap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    fstmap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            fstxdim = xdim;
            fstydim = ydim;
            xcord = fstxdim;
            ycord = fstydim;
            fstR = Rdim;
            fstG = Gdim;
            fstB = Bdim;
        }

        private void Transparency_Load(object sender, EventArgs e)
        {
            canvas = this.pictureBox1.CreateGraphics();
            painter = new Bitmap(xcord, ycord);    
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            alpha = trackBar1.Value;
            label3.Text = trackBar1.Value.ToString();
            int ovlxdim = fstxdim;
            int ovlydim = fstydim;
            int[,] ovlR = new int[ovlxdim, ovlydim];
            int[,] ovlG = new int[ovlxdim, ovlydim];
            int[,] ovlB = new int[ovlxdim, ovlydim];
            Bitmap ovlmaps = new Bitmap(ovlxdim, ovlydim);
            if (cutmode == 0)
            {
                    for (int i = 0; i < ovlxdim; i++)
                    {
                        for (int j = 0; j < ovlydim; j++)
                        {
                            ovlR[i, j] = (int)(((100 - alpha) * fstR[i, j] + alpha * scdR[i, j]) / 100);
                            ovlG[i, j] = (int)(((100 - alpha) * fstG[i, j] + alpha * scdG[i, j]) / 100);
                            ovlB[i, j] = (int)(((100 - alpha) * fstB[i, j] + alpha * scdB[i, j]) / 100);
                            ovlmaps.SetPixel(j, i, Color.FromArgb(ovlR[i, j], ovlG[i, j], ovlB[i, j]));
                        }
                    }                
            }
            else
            {
                    for (int i = 0; i < ovlxdim; i++)
                    {
                        for (int j = 0; j < ovlydim; j++)
                        {
                            ovlR[i, j] = (int)(((100 - alpha) * fstR[i, j] + alpha * cutR[i, j]) / 100);
                            ovlG[i, j] = (int)(((100 - alpha) * fstG[i, j] + alpha * cutG[i, j]) / 100);
                            ovlB[i, j] = (int)(((100 - alpha) * fstB[i, j] + alpha * cutB[i, j]) / 100);
                            ovlmaps.SetPixel(j, i, Color.FromArgb(ovlR[i, j], ovlG[i, j], ovlB[i, j]));
                        }
                    }               
            }
            pictureBox2.Size = new System.Drawing.Size((int)ovlmaps.Width, (int)ovlmaps.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = ovlmaps;           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImagePath = GetImage();

            OpenPcx(out scdlayer, out scdxdim, out scdydim, out scdR, out scdG, out scdB);

            pictureBox1.Size = new System.Drawing.Size((int)scdlayer.Width, (int)scdlayer.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = scdlayer;
        }

        private String GetImage()
        {
            path.InitialDirectory = Application.StartupPath;//預設開啟的位置(Debug)
            path.Filter = "Image Files|*.jpg;*.pcx;*.png;*.png|All Files|*.*";//會先顯示哪些檔
            path.ShowDialog();
            return path.FileName;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        void OpenPcx(out Bitmap pcximage, out int xSize, out int ySize, out int[,] R, out int[,] G, out int[,] B)
        {
                                       
            byte[] fileHeader = new byte[128]; //.pcx 影像檔 header為128byte
            FileStream imageinfo = new FileStream(ImagePath, FileMode.Open, FileAccess.Read, FileShare.None);//將檔案打開後讀取
            imageinfo.Read(fileHeader, 0, 128);

            int xMin = BitConverter.ToInt16(fileHeader, 4); //傳回從位元組陣列中指定位置的兩個 byte所轉換的16bit帶正負號的整數 (Signed Integer)
            int yMin = BitConverter.ToInt16(fileHeader, 6);
            int xMax = BitConverter.ToInt16(fileHeader, 8);
            int yMax = BitConverter.ToInt16(fileHeader, 10);
            int hdpi = BitConverter.ToInt16(fileHeader, 12);
            int vdpi = BitConverter.ToInt16(fileHeader, 14);

            byte version = fileHeader[1];
            byte[] palette = new byte[768];//調色盤Color palette setting

           
            if (version < 5)//A PCX file has space in its header for a 16 color (3*16=48 bytes) palette. 
            {
                
                long infoLength = imageinfo.Length;
                int RleLength = (int)infoLength - 128;
               
                Bitmap C_palette16 = new Bitmap(4, 4);
                int[] CP16_R = new int[16];
                int[] CP16_G = new int[16];
                int[] CP16_B = new int[16];
                int[] C_Combine16 = new int[16];
                for (int i = 0; i < 48; i = i + 3)
                {
                    
                    CP16_R[i] = fileHeader[i + 16];
                    CP16_G[i] = fileHeader[i + 17];
                    CP16_B[i] = fileHeader[i + 18];
                    C_Combine16[i] = CP16_R[i] << 16 + CP16_G[i] << 8 + CP16_B[i];
                }
                for (int i = 0; i < 48; i++)
                {
                    C_palette16.SetPixel(i, i / 4, Color.FromArgb(C_Combine16[i]));
                }
            }

            
            if (version > 4) //If a PCX file has a 256-color palette, it is found 768 bytes from the end of the file.
            {
                
                long infoLength = imageinfo.Length;
                int RleLength = (int)infoLength - 128 - 769;               

                imageinfo.Seek(-768, SeekOrigin.End);//讀取最後768個bytes          

                int[] CP256_R = new int[256];
                int[] CP256_G = new int[256];
                int[] CP256_B = new int[256];
                int[] C_Combine256 = new int[256];

                for (int i = 0; i < 768; i++)
                {
                    palette[i] = (byte)imageinfo.ReadByte(); //header裡的調色盤
                }

                int j = 0;
                for (int i = 0; i < 768; i = i + 3)
                {
                    CP256_R[j] = palette[i];  //調色盤R 的資訊
                    CP256_G[j] = palette[i + 1];  //調色盤G 的資訊
                    CP256_B[j] = palette[i + 2];  //調色盤B 的資訊
                    j++;
                }

                j = 0;
               
                imageinfo.Seek(128, SeekOrigin.Begin);// Set the stream position to the beginning of the file.

            }

            //---------------------------------------------------------------------------------------------------------
            char bitsPerPixel = BitConverter.ToChar(fileHeader, 3);
            byte nPlanes = fileHeader[65];
            int bytesPerLine = BitConverter.ToInt16(fileHeader, 66);

            int totalBytesPerLine = nPlanes * bytesPerLine;

            int linePaddingSize = ((bytesPerLine * nPlanes) * (8 / bitsPerPixel)) - ((xMax - xMin) + 1);

            byte[] scanLine = new byte[totalBytesPerLine];
            byte nRepeat;
            byte pColor;
            int pIndex = 0;
            
            xSize = xMax - xMin + 1;
            ySize = yMax - yMin + 1;
            pcximage = new Bitmap(xSize, ySize);

            byte[] imageBytes = new byte[totalBytesPerLine * ySize];           

            R = new int[xSize, ySize];
            G = new int[xSize, ySize];
            B = new int[xSize, ySize];

            for (int iY = 0; iY < ySize; iY++)
            {
                int iX = 0;
                while (iX < totalBytesPerLine)
                {
                    nRepeat = (byte)imageinfo.ReadByte();
                    if (nRepeat > 192)
                    {
                        nRepeat -= 192; 
                        pColor = (byte)imageinfo.ReadByte();
                        for (int j = 0; j < nRepeat; j++)
                        {
                            if (iX < scanLine.Length)
                            {
                                scanLine[iX] = pColor;
                                imageBytes[pIndex] = pColor;
                            }
                            iX++;
                            pIndex++;
                        }
                    }
                    else
                    {
                        if (iX < scanLine.Length)
                        {
                            scanLine[iX] = nRepeat;
                            imageBytes[pIndex] = nRepeat;
                        }
                        iX++;
                        pIndex++;
                    }
                }
            }


            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    R[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3];
                    G[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 1];
                    B[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 2];
                }
            }

            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    pcximage.SetPixel(j, i, Color.FromArgb(R[i, j], G[i, j], B[i, j]));
                }
            }

            imageinfo.Close();         

        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Start();
            shaft = 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Start();
            shaft = 2;

        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            int ovlxdim = fstxdim;
            int ovlydim = fstydim;
            int[,] ovlR = new int[ovlxdim, ovlydim];
            int[,] ovlG = new int[ovlxdim, ovlydim];
            int[,] ovlB = new int[ovlxdim, ovlydim];
            ovlmap = new Bitmap [20];
            if (cutmode == 0)
            {
                for (int alpha = 0; alpha < 100; alpha += 5)
                {
                    int indx = alpha / 5;
                    ovlmap[indx] = new Bitmap(ovlxdim, ovlydim);
                    for (int i = 0; i < ovlxdim; i++)
                    {
                        for (int j = 0; j < ovlydim; j++)
                        {
                            ovlR[i, j] = (int)(((100 - alpha) * fstR[i, j] + alpha * scdR[i, j]) / 100);
                            ovlG[i, j] = (int)(((100 - alpha) * fstG[i, j] + alpha * scdG[i, j]) / 100);
                            ovlB[i, j] = (int)(((100 - alpha) * fstB[i, j] + alpha * scdB[i, j]) / 100);
                            ovlmap[indx].SetPixel(j, i, Color.FromArgb(ovlR[i, j], ovlG[i, j], ovlB[i, j]));
                        }
                    }  
                    toolStripProgressBar1.PerformStep();                 
                }
            }
            else
            {
                for (int alpha = 0; alpha < 100; alpha += 5)
                {
                    int indx = alpha / 5;
                    ovlmap[indx] = new Bitmap(ovlxdim, ovlydim);
                    for (int i = 0; i < ovlxdim; i++)
                    {
                        for (int j = 0; j < ovlydim; j++)
                        {
                            ovlR[i, j] = (int)(((100 - alpha) * fstR[i, j] + alpha * cutR[i, j]) / 100);
                            ovlG[i, j] = (int)(((100 - alpha) * fstG[i, j] + alpha * cutG[i, j]) / 100);
                            ovlB[i, j] = (int)(((100 - alpha) * fstB[i, j] + alpha * cutB[i, j]) / 100);
                            ovlmap[indx].SetPixel(j, i, Color.FromArgb(ovlR[i, j], ovlG[i, j], ovlB[i, j]));
                        }
                    }                    
                    toolStripProgressBar1.PerformStep();
                }
            }
            toolStripStatusLabel1.Text = "Complete!";
            pictureBox2.Image = ovlmap[0];           
        }
        int inconter = 0;
        int deconter = 20;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (shaft == 1)
            {
                inconter++;
                if (inconter < 20)
                {
                    pictureBox2.Image = ovlmap[inconter];
                    trackBar1.Value = inconter*5;
                    label3.Text = "" + inconter*5;
                }
                if (inconter > 20)
                {
                    timer1.Stop();
                    inconter = 0;
                    
                }
            }

            if (shaft == 2)
            {
                deconter--;
                if (deconter > 0)
                {
                    pictureBox2.Image = ovlmap[deconter];
                    trackBar1.Value = deconter * 5;
                    label3.Text = "" + deconter * 5;
                }
                if (deconter < 0)
                {
                    timer1.Stop();
                    deconter = 20;                    
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int option = comboBox1.SelectedIndex;

            if (option == 2) { cutmode = 1; }
            if (option == 3) { cutmode = 2; }
            if (option == 4) { cutmode = 3; }
          
        }

        private void button5_Click(object sender, EventArgs e) //clear image
        {
            pictureBox1.Image = null;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startx = e.X; starty = e.Y;
            x0 = e.X; y0 = e.Y;
            string stingstartx, stingstarty;
            stingstartx = startx.ToString();
            stingstarty = starty.ToString();
            

            trackX.Clear();
            trackY.Clear();
            circleX.Clear();
            circleY.Clear();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int trackerx = e.X; int trackery = e.Y;
            string stingtrackerx, stingtrackery;
            stingtrackerx = trackerx.ToString();
            stingtrackery = trackery.ToString();
            toolStripStatusLabel3.Text = stingtrackerx;
            toolStripStatusLabel4.Text = stingtrackery;

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
                        

                        break;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            endx = e.X; endy = e.Y;

            string stingendx, stingendy;
            stingendx = endx.ToString();
            stingendy = endy.ToString();

            
            cutR = new int [xcord, ycord];
            cutG = new int[xcord, ycord];
            cutB = new int[xcord, ycord];
            int clipxdim = Math.Abs(startx - endx);
            int clipydim = Math.Abs(starty - endy);
            int clipx0 = startx - (startx - endx) / 2;
            int clipy0 = starty - (starty - endy) / 2;

            Rectangle rect = new Rectangle(Math.Min(startx, endx), Math.Min(starty, endy), Math.Abs(endx - startx), Math.Abs(endy - starty));

            switch (cutmode)
            {
                case 1: //rectangle

                    canvas.DrawRectangle(pencil, rect);

                    for (int j = 0; j < ycord; j++)
                    {
                        if (j < starty || j > endy)
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                
                                cutR[j, i] = 255;
                                cutG[j, i] = 255;
                                cutB[j, i] = 255;
                            }
                        }

                        else
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                if (i > startx && i < endx)
                                { 

                                    cutR[j, i] = scdR[j, i];
                                    cutG[j, i] = scdG[j, i];
                                    cutB[j, i] = scdB[j, i];
                                }
                                else
                                {
                 
                                    cutR[j, i] = 255;
                                    cutG[j, i] = 255;
                                    cutB[j, i] = 255;
                                }
                            }
                        }
                    }
                   
                    break;

                case 2: //circle
                    canvas.DrawEllipse(pencil, rect);

                    int elipa = clipxdim / 2;
                    int elipb = clipydim / 2;

                    for (int theta = 0; theta < 360; theta++)
                    {
                        double radian = theta * (Math.PI / 180);
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
                                cutR[j, i] = 255;
                                cutG[j, i] = 255;
                                cutB[j, i] = 255;

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
                                {
                                    cutR[j, i] = scdR[j, i];
                                    cutG[j, i] = scdG[j, i];
                                    cutB[j, i] = scdB[j, i];
                                }


                                else
                                {
                                    cutR[j, i] = 255;
                                    cutG[j, i] = 255;
                                    cutB[j, i] = 255;
                                }

                            }
                        }

                    }
             

                    break;

                case 3: //freehand

                    CheckContinue(); // check if the dots are continuous

                    trackX.Insert(0, 0);
                    trackY.Insert(0, 0);

                    int[] loopX = new int[trackX.Count];
                    int[] loopY = new int[trackY.Count];
                    int[] validY;
                    int[] validX;
                    int[,] xscanner = new int[xcord, ycord];
                    int[,] yscanner = new int[xcord, ycord];

                    trackX.CopyTo(loopX);
                    trackY.CopyTo(loopY);

                    //scan y-direction first ,than scan x-direction 
                    Array.Sort(loopY);

                    int maxscany = loopY[loopY.Length - 1];
                    int minscany = loopY[1];

                    for (int j = 0; j < ycord; j++)
                    {
                        if (j < minscany || j > maxscany)
                        {
                            for (int i = 0; i < xcord; i++)
                            {
                                cutR[j, i] = 255;
                                cutG[j, i] = 255;
                                cutB[j, i] = 255;
                            }
                        }

                        else
                        {
                            validY = SearchArray(trackY, j);
                            Array.Sort(validY);

                            int findx1 = (int)validY[validY.Length - 1];
                            int findx2 = (int)validY[1];
                            int scanx1 = (int)trackX[findx1];
                            int scanx2 = (int)trackX[findx2];
                            int maxsnx = Math.Max(scanx1, scanx2);
                            int minsnx = Math.Min(scanx1, scanx2);

                            for (int i = 0; i < xcord; i++)
                            {
                                if (i > minsnx && i < maxsnx)
                                {
                                   
                                    cutR[j, i] = scdR[j, i];
                                    cutG[j, i] = scdG[j, i];
                                    cutB[j, i] = scdB[j, i];
                                }
                                else
                                { 
                                    
                                    cutR[j, i] = 255;
                                    cutG[j, i] = 255;
                                    cutB[j, i] = 255;
                                }
                            }
                        }
                    }


                    break;
            }

        }

        private int[] SearchArray(ArrayList list, int target)
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

            int[] indxlist = (int[])arryindx.ToArray(typeof(int));
            return indxlist;
        }

        void CheckContinue()
        {
            int j = 0;
            for (int i = 0; i < trackX.Count; i++)
            {
                int interx;
                if ((i + 1) > trackX.Count - 1)
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

            int n = 0;
            for (int m = 0; m < trackY.Count; m++)
            {
                int intery;
                if ((m + 1) > trackY.Count - 1)
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


        
       
       





    }
}