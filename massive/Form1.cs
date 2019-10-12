using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

using System.IO;
using System.Threading;

namespace massive
{
    public partial class MainForm : Form
    {
        OpenFileDialog path = new OpenFileDialog();
        
        public Bitmap imagefile;
        public int [,] Rdim;
        public int [,] Gdim;
        public int [,] Bdim;
        public int xdim;
        public int ydim;
        
        public Bitmap C_palette256 = new Bitmap(64, 64);       
        string ImagePath = "";
        //select and cuting tool
        int[,] selet;
        public Bitmap copymap;
        public Bitmap synthmap;
        public Bitmap routmap;
  
        Graphics cntur;
        Graphics routpath;
        Pen pencil = new Pen(Color.White, 1);
        int cutmode;
        int startx; int starty;
        int x0; int y0;
        int endx; int endy;
        int sx0; int sy0; int sxn; int syn;
        int tracksize;

        ArrayList circleX = new ArrayList();
        ArrayList circleY = new ArrayList();
        ArrayList trackX = new ArrayList();
        ArrayList trackY = new ArrayList();  
                    
        public MainForm()
        {
            InitializeComponent();
            Thread th = new Thread(new ThreadStart(splashscreen));
            th.Start();
            Thread.Sleep(1000);
            th.Abort();
            Thread.Sleep(1000);
        }

        private void splashscreen()
        {
            SplashScreen splashapp = new SplashScreen();
            splashapp.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cntur = this.pictureBox1.CreateGraphics();
        } 

        public void 開啟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImagePath = GetImage();
            OpenPcx(out imagefile, out xdim, out ydim, out Rdim, out Gdim, out Bdim);
            pictureBox1.Size = new System.Drawing.Size((int)imagefile.Width, (int)imagefile.Height);//box長寬與圖相同，上限360*320
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//put the image in PictureBox
            pictureBox1.Image = imagefile;         
        }

        private String GetImage()
        {
            path.InitialDirectory = Application.StartupPath;//default path
            path.Filter = "Image Files|*.jpg;*.pcx;*.png;*.png|All Files|*.*";
            path.ShowDialog();
            return path.FileName;
        }

        private void bMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savepath = new SaveFileDialog();
            savepath.Filter = "Bitmap Image|*.bmp";
            savepath.ShowDialog();
            string bmppath = savepath.FileName;
            if (synthmap != null)
            { synthmap.Save(bmppath, System.Drawing.Imaging.ImageFormat.Bmp); }
            else
            { imagefile.Save(bmppath, System.Drawing.Imaging.ImageFormat.Bmp); }
        }
       
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int routx = e.X;
            int routy = e.Y;
            string stngroutx, stngrouty;
            stngroutx = routx.ToString();
            stngrouty = routy.ToString();
            toolStripStatusLabel2.Text = stngroutx;
            toolStripStatusLabel3.Text = stngroutx;
            
            if (imagefile != null) //顯示滑鼠游標位置的RGB
            {
                Color PixelColor = imagefile.GetPixel(e.X, e.Y);
                int Pix_R = PixelColor.R;
                int Pix_G = PixelColor.G;
                int Pix_B = PixelColor.B;
                toolStripStatusLabel8.Text = " R(" + Pix_R + ")";
                toolStripStatusLabel8.ForeColor = Color.Red;
                toolStripStatusLabel9.Text = " G(" + Pix_G + ")";
                toolStripStatusLabel9.ForeColor = Color.Green;
                toolStripStatusLabel10.Text = " B(" + Pix_B + ")";
                toolStripStatusLabel10.ForeColor = Color.Blue;

                if (e.Button == MouseButtons.Left) // 畫圖
                { 
                    switch (cutmode)
                    {
                        case 1:
                            break;

                        case 2:
                            break;

                        case 3:                   
                            routpath.DrawLine(pencil, x0, y0, e.X, e.Y);                         
                            x0 = e.X; y0 = e.Y;
                            pictureBox1.BackgroundImage = routmap;                            
                            pictureBox1.CreateGraphics().DrawImage(routmap, 0, 0);
                            trackX.Add(e.X);
                            trackY.Add(e.Y);
                            tracksize = trackX.Count;        
                            break;

                        case 9:
                            synthmap = new Bitmap(xdim, ydim);
                            Graphics grc = Graphics.FromImage(synthmap);
                            
                            grc.DrawImage(imagefile, 0, 0);
                            grc.DrawImage(copymap,routx ,routy );
                            pictureBox1.BackgroundImage = synthmap;
                            pictureBox1.Refresh();                            
                            pictureBox1.Image = synthmap;
                            break;
                    }
                }
            }         
        }

        private void cutToolStripMenuItem_Click_1(object sender, EventArgs e) //cut
        { CutImage(); }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) //copy
        { CopyImage(); }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e) //paste
        { PasteImage(); }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e) //reset
        {
            pictureBox1.Size = new System.Drawing.Size(xdim, ydim);//控制box長寬與圖相同，上限360*320
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//設定圖片剛好放進pictureBox
            pictureBox1.Image = imagefile;
            routmap = null;
        }

        private void toolStripButton6_Click(object sender, EventArgs e) //reset
        {
            pictureBox1.Size = new System.Drawing.Size(xdim, ydim);//控制box長寬與圖相同，上限360*320
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//設定圖片剛好放進pictureBox
            pictureBox1.Image = imagefile;
            routmap = null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) //open file bottom
        {
            ImagePath = GetImage();
            OpenPcx(out imagefile, out xdim, out ydim, out Rdim, out Gdim, out Bdim);
            pictureBox1.Size = new System.Drawing.Size((int)imagefile.Width, (int)imagefile.Height);//控制box長寬與圖相同，上限360*320
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//設定圖片剛好放進pictureBox
            pictureBox1.Image = imagefile;        
        }

        private void toolStripButton2_Click(object sender, EventArgs e) //save file bottom
        {
            SaveFileDialog savepath = new SaveFileDialog();
            savepath.Filter = "Bitmap Image|*.bmp";
            savepath.ShowDialog();
            string bmppath = savepath.FileName;
            if (synthmap != null)
            { synthmap.Save(bmppath, System.Drawing.Imaging.ImageFormat.Bmp); }
            else
            { imagefile.Save(bmppath, System.Drawing.Imaging.ImageFormat.Bmp); }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) //cut bottom
        { CutImage(); }

        private void toolStripButton4_Click(object sender, EventArgs e) //copy bottom
        { CopyImage(); }

        private void toolStripButton5_Click(object sender, EventArgs e) //paste bottom
        { PasteImage(); }

        void CutImage()
        {
            Bitmap cutmap = new Bitmap(xdim, ydim);
            for (int j = 0; j < xdim; j++)
            {
                for (int i = 0; i < ydim; i++)
                {
                    int pixr = Rdim[i, j];
                    int pixg = Gdim[i, j];
                    int pixb = Bdim[i, j];
                    if (selet[j, i] == 1) { pixr = 255; pixg = 255; pixb = 255; }
                    cutmap.SetPixel(j, i, Color.FromArgb(pixr, pixg, pixb));
                }
            }
            pictureBox1.Size = new System.Drawing.Size(xdim, ydim);//控制box長寬與圖相同，上限360*320
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//設定圖片剛好放進pictureBox
            pictureBox1.Image = cutmap;      
        }

        void CopyImage()
        {
            int cxdim = sxn - sx0;
            int cydim = syn - sy0;
            copymap = new Bitmap(cxdim, cydim);

            for (int j = 0; j < cydim; j++)
            {
                for (int i = 0; i < cxdim; i++)
                {
                    int alph = 255;
                    int x = i + sx0;
                    int y = j + sy0;
                    if (selet[x, y] == 0) { alph = 0; }
                    copymap.SetPixel(i, j, Color.FromArgb(alph, Rdim[y, x] * selet[x, y], Gdim[y, x] * selet[x, y], Bdim[y, x] * selet[x, y]));
                }
            }
        }

        void PasteImage()
        {
            if (copymap != null)
            {               
                synthmap = new Bitmap(xdim, ydim);
                Graphics grc = Graphics.FromImage(synthmap);
                grc.DrawImage(imagefile, 0, 0);
                grc.DrawImage(copymap, 0, 0);
                pictureBox1.BackgroundImage = synthmap;
                pictureBox1.Refresh();
                //pictureBox1.CreateGraphics().DrawImage(synthmap, 0, 0);
                pictureBox1.Image = synthmap;
                cutmode = 9;
            }
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e) //rectangle select bottom
        { cutmode = 1; }

        private void eToolStripMenuItem_Click(object sender, EventArgs e) //ellipse select bottom
        { cutmode = 2; }

        private void freeHandToolStripMenuItem_Click(object sender, EventArgs e) //freehand select bottom
        { 
            cutmode = 3;
            routmap = new Bitmap(xdim, ydim);
            routpath = Graphics.FromImage(routmap);
            routpath.DrawImage(imagefile, 0, 0);
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
            selet = new int[xdim, ydim];
            int clipxdim = Math.Abs(startx - endx);
            int clipydim = Math.Abs(starty - endy);
            Rectangle rect = new Rectangle(Math.Min(startx, endx), Math.Min(starty, endy), Math.Abs(endx - startx), Math.Abs(endy - starty));

            switch (cutmode)
            {
                case 1: //rectangle

                    cntur.DrawRectangle(pencil, rect);                 
                    for (int j = 0; j < ydim; j++)
                    {
                        if (j < starty || j > endy)
                        {
                            for (int i = 0; i < xdim; i++)
                            {
                                selet[i, j] = 0;
                            }
                        }

                        else
                        {
                            for (int i = 0; i < xdim; i++)
                            {
                                if (i > startx && i < endx)
                                { selet[i, j] = 1; }
                                else
                                { selet[i, j] = 0; }
                            }
                        }
                    }
                    sx0 = startx; 
                    sy0 = starty;
                    sxn = endx;
                    syn = endy;
                    break;

                case 2: //circle
                    cntur.DrawEllipse(pencil, rect);

                    int clipx0 = startx - (startx - endx) / 2;
                    int clipy0 = starty - (starty - endy) / 2;
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
                    int[] sphereY = new int[circleY.Count];
                    circleY.CopyTo(sphereY);
                    Array.Sort(sphereY);
                    int maxelipy = sphereY[sphereY.Length - 1];
                    int minelipy = sphereY[1];

                    for (int j = 0; j < ydim; j++)
                    {
                        if (j < minelipy || j > maxelipy)
                        {
                            for (int i = 0; i < xdim; i++)
                            {
                                selet[i, j] = 0;

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

                            for (int i = 0; i < xdim; i++)
                            {
                                if (i > mincrx && i < maxcrx)
                                { selet[i, j] = 1; }
                                else
                                { selet[i, j] = 0; }
                            }
                        }
                    }
                    sx0 = startx;
                    sy0 = starty;
                    sxn = endx;
                    syn = endy;
                    break;

                case 3: //free hand

                    CheckContinue(); // check if the dots are continuous

                    trackX.Insert(0, 0);
                    trackY.Insert(0, 0);

                    int[] loopX = new int[trackX.Count];
                    int[] loopY = new int[trackY.Count];
                    int[] validY;
                    int[] validX;
                    int[,] xscanner = new int[xdim, ydim];
                    int[,] yscanner = new int[xdim, ydim];

                    trackX.CopyTo(loopX);
                    trackY.CopyTo(loopY);

                    //scan y-direction first ,than scan x-direction 
                    Array.Sort(loopY);

                    int maxscany = loopY[loopY.Length - 1];
                    int minscany = loopY[1];

                    for (int j = 0; j < ydim; j++)
                    {
                        if (j < minscany || j > maxscany)
                        {
                            for (int i = 0; i < xdim; i++)
                            {
                                xscanner[i, j] = 0;
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

                            for (int i = 0; i < xdim; i++)
                            {
                                if (i > minsnx && i < maxsnx)
                                { xscanner[i, j] = 1; }
                                else
                                { xscanner[i, j] = 0; }
                            }
                        }
                    }
                    //scan x-direction first ,than scan y-direction
                    Array.Sort(loopX);
                    int maxscanx = loopX[loopX.Length - 1];
                    int minscanx = loopX[1];
                    /*
                    for (int i = 0; i < xdim; i++)
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

                            for (int j = 0; j < ydim; j++)
                            {
                                if (j > minsny && j < maxsny)
                                { yscanner[i, j] = 1; }
                                else
                                { yscanner[i, j] = 0; }
                            }
                        }
                    }*/

                    for (int j = 0; j < ydim; j++)
                    {
                        for (int i = 0; i < xdim; i++)
                        {
                            selet[i, j] = xscanner[i, j];
                        }
                    }

                    sx0 = minscanx;
                    sy0 = minscany;
                    sxn = maxscanx;
                    syn = maxscany;
                    break;       
            } 
        }

        void OpenPcx(out Bitmap pcximage, out int xSize, out int ySize, out int [,]R, out int [,]G, out int [,]B)
        {
            char bitsPerPixel;         
            int hdpi;
            int vdpi;
                     
            //read header
            byte[] fileHeader = new byte[128]; //*.pcx image, header 128byte
            FileStream imageinfo = new FileStream(ImagePath, FileMode.Open, FileAccess.Read, FileShare.None);//open file
            imageinfo.Read(fileHeader, 0, 128);
            byte isPCX = fileHeader[0];        

            /* Version information
            0 = Ver. 2.5 of PC Paintbrush                
            2 = Ver. 2.8 w/palette information
            3 = Ver. 2.8 w/o palette information
            4 = PC Paintbrush for Windows(Plus for Windows uses Ver 5)
            5 = Ver. 3.0 and > of PC Paintbrush and PC Paintbrush +, includes Publisher’s Paintbrush. Includes 24-bit .PCX files */

            byte version = fileHeader[1];

            bitsPerPixel = BitConverter.ToChar(fileHeader, 3); //bit per pixel(1, 2, 4, or 8)           
            int xMin = BitConverter.ToInt16(fileHeader, 4); //return byte array從位元組陣列中指定位置的2 byte所轉換的16bit signed Integer
            int yMin = BitConverter.ToInt16(fileHeader, 6);
            int xMax = BitConverter.ToInt16(fileHeader, 8);
            int yMax = BitConverter.ToInt16(fileHeader, 10);
            
            hdpi = BitConverter.ToInt16(fileHeader, 12);//horizontal DPI       
            vdpi = BitConverter.ToInt16(fileHeader, 14);//vertical DPI
     
            byte[] palette = new byte[768];//Color palette setting

            //palette in header
            if (version < 5)//A PCX file has space in its header for a 16 color (3*16=48 bytes) palette. 
            {
                //-----------------------------------------------------------
                long infoLength = imageinfo.Length;
                int RleLength = (int)infoLength - 128;
                //---------------------------------------------------------------


                //label10.Text = "Color palette: " + 16 + " colors";
                Bitmap C_palette16 = new Bitmap(4, 4);
                int[] CP16_R = new int[16];
                int[] CP16_G = new int[16];
                int[] CP16_B = new int[16];
                int[] C_Combine16 = new int[16];
                for (int i = 0; i < 48; i = i + 3)
                {
                    //palette[i] = fileHeader[i + 16];
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

            //palette at end
            if (version > 4) //If a PCX file has a 256-color palette, it is found 768 bytes from the end of the file.
            {
                //-----------------------------------------------------------
                long infoLength = imageinfo.Length;
                int RleLength = (int)infoLength - 128 - 769;
                //-----------------------------------------------------------               
                
                imageinfo.Seek(-768, SeekOrigin.End);//read the last 768 bytes
                           
                int[] CP256_R = new int[256];
                int[] CP256_G = new int[256];
                int[] CP256_B = new int[256];
                int[] C_Combine256 = new int[256];

                for (int i = 0; i < 768; i++)
                {
                    palette[i] = (byte)imageinfo.ReadByte(); //header palette
                }

                int j = 0;
                for (int i = 0; i < 768; i = i + 3)
                {
                    CP256_R[j] = palette[i];  //palette R
                    CP256_G[j] = palette[i + 1];  //palette G
                    CP256_B[j] = palette[i + 2];  //palette B
                    j++;
                }

                j = 0;
                
                imageinfo.Seek(128, SeekOrigin.Begin);// Set the stream position to the beginning of the file.
               
            }

           
            byte nPlanes = fileHeader[65];//Number of bit planes          
            int bytesPerLine = BitConverter.ToInt16(fileHeader, 66);//Number of bytes to allocate for a scanline plane          

            //---------------------------------------------------------------------------------------------------------
 
            
            int totalBytesPerLine = nPlanes * bytesPerLine;
            
            int linePaddingSize = ((bytesPerLine * nPlanes) * (8 / bitsPerPixel)) - ((xMax - xMin) + 1);
            
            byte[] scanLine = new byte[totalBytesPerLine];
            byte nRepeat;
            byte pColor;
            int pIndex = 0;
           
            xSize = xMax - xMin + 1;
            ySize = yMax - yMin + 1;

            byte[] imageBytes = new byte[totalBytesPerLine * ySize];           
            
            pcximage = new Bitmap(xSize, ySize);
           

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
                        nRepeat -= 192; //nRepeat=nRepeat-192
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


            for (int j = 0; j < ySize; j++)
            {
                for (int i = 0; i < xSize; i++)
                {
                    R[i,j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3];
                    G[i,j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 1];
                    B[i,j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 2];
                }
            }

            for (int i = 0; i < ySize; i++)
            {
                for (int j = 0; j < xSize; j++)
                {
                    pcximage.SetPixel(j, i, Color.FromArgb(R[i,j], G[i,j], B[i,j]));
                }
            }        
            
            imageinfo.Close();
                        
        }

        private void bitPlanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BitPlanes form = new BitPlanes(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rotate form = new Rotate(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayScale form = new GrayScale(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void scalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Zoom form = new Zoom(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Histogram form = new Histogram(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void transparencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transparency form = new Transparency(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void thresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thresholding form = new Thresholding(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void rGBFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RGBColorFilter form = new RGBColorFilter(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void waterMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WaterMark form = new WaterMark(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void invertColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertColor form = new InvertColor(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void lowpassFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowpass form = new Lowpass(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 影像資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HeaderInfo form = new HeaderInfo(ImagePath);
            form.ShowDialog();
        }

        private void huffmanToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Huffman form = new Huffman(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void histogramEqualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistogramEqualize form = new HistogramEqualize(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void outlierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OutlierFilter form = new OutlierFilter(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void medianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedianFilter form = new MedianFilter(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void pseudoMedianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PseudoMedian form = new PseudoMedian(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void highpassFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighpassFilter form = new HighpassFilter(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void dCTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DCT form = new DCT(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

       

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RobertsOperator form = new RobertsOperator(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sobel form = new Sobel(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void prewittToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PrewittOperator form = new PrewittOperator(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void histogramSpecificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistogramSpecify form = new HistogramSpecify(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void highBoostFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighBoost form = new HighBoost(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void edgeCrispeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgeCrispening form = new EdgeCrispening(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        

        private void pixelDivisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PixelDivision form = new PixelDivision(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void fractalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FractalCoding form = new FractalCoding(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
        }

        private void aboutViewPcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutPcx form = new AboutPcx();
            form.ShowDialog();
        }

        private void dCTToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DCT form = new DCT(xdim, ydim, Rdim, Gdim, Bdim);
            form.ShowDialog();
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