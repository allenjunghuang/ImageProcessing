using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace massive
{
    public partial class FractalCoding : Form
    {
        OpenFileDialog path = new OpenFileDialog();
        string ImagePath = "";
        public int xcord, ycord, rngcont, bloksiz = 8, iternum = 10;
        public int[,] plnR, plnG, plnB, plnF, plnY;
        public int[, ,] rngB;
        public int[, , ,] domB;
        double signal;
        double[] SNR;
        public double [,] fracode;
        Bitmap[] frcmap;

        public FractalCoding(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            int[,] C2G = new int[xdim, ydim];           
            Bitmap graymap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++) {
                for (int j = 0; j < xdim; j++) {
                    C2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    signal += C2G[i, j] * C2G[i, j];
                    graymap.SetPixel(j, i, Color.FromArgb(C2G[i, j], C2G[i, j], C2G[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)graymap.Width, (int)graymap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = graymap;
            xcord = xdim;
            ycord = ydim;
            plnF = C2G;
        }

        private void FractalCoding_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e) //encoding
        {
            rngcont = (xcord / bloksiz) * (ycord / bloksiz); //creat range block
            toolStripProgressBar1.Maximum = rngcont;
            rngB = new int[bloksiz, bloksiz, rngcont];
            fracode = new double[rngcont, 4];
            int rngendx = xcord - bloksiz + 1;
            int rngendy = ycord - bloksiz + 1;
            for (int y = 0; y < rngendy; y += bloksiz) {
                for (int x = 0; x < rngendx; x += bloksiz) {
                    int z = (y / bloksiz) * (xcord / bloksiz) + (x / bloksiz);
                    for (int j = 0; j < bloksiz; j++) {
                        for (int i = 0; i < bloksiz; i++) {
                            rngB[i, j, z] = plnY[x + i, y + j];
                        }
                    }
                }
            }
            int w = 0, u = 0, v = 0;//creat domain block
            int domendx = xcord - (bloksiz * 2) + 1;
            int domendy = ycord - (bloksiz * 2) + 1;
            int domcont = domendx * domendy;
            domB = new int[bloksiz, bloksiz, 8, domcont];
            for (int y = 0; y < domendy; y++) {
                for (int x = 0; x < domendx; x++) {
                    w = y * domendx + x;
                    for (int j = 0; j < bloksiz; j++) {
                        v = y + (2 * j);
                        for (int i = 0; i < bloksiz; i++) {
                            u = x + (2 * i);
                            domB[i, j, 0, w] = plnY[u, v]; //sub-sampling
                        }
                    }
                    for (int j = 0; j < bloksiz; j++) {
                        for (int i = 0; i < bloksiz; i++) {
                            domB[i, j, 1, w] = domB[j, (bloksiz-1) - i, 0, w]; //rotate 90 (deg)
                            domB[i, j, 2, w] = domB[(bloksiz - 1) - i, (bloksiz - 1) - j, 0, w]; //rotate 180(deg)
                            domB[i, j, 3, w] = domB[(bloksiz - 1) - j, i, 0, w]; //rotate 270(deg)
                            domB[i, j, 4, w] = domB[(bloksiz - 1) - i, j, 0, w]; //mirror 90 (deg)
                            domB[i, j, 5, w] = domB[i, (bloksiz - 1) - j, 0, w]; //mirror 180(deg)
                            domB[i, j, 6, w] = domB[(bloksiz - 1) - j, (bloksiz - 1) - i, 0, w]; //mirror 45 (deg)
                            domB[i, j, 7, w] = domB[j, i, 0, w]; //mirror 135(deg)
                        }
                    }
                }
            }
            double[,] errarry; //find range-domain relation  
            double err = 0;
            for (int r = 0; r < rngcont; r++) {
                errarry = new double[domcont, 8];
                for (int s = 0; s < domcont; s++) {
                    for (int t = 0; t < 8; t++) {
                        err=0;
                        for (int j = 0; j < bloksiz; j++) {
                            for (int i = 0; i < bloksiz; i++) {
                                err+= Math.Pow((rngB[i, j, r] - domB[i, j, t, s]),2);

                            }
                        }
                        errarry[s, t] = err;
                    }
                }
                double maxerr = 255 * 255 * bloksiz * bloksiz;
                int domindx = 0;
                int trnindx = 0;
                for (int p = 0; p < domcont; p++) {
                    for (int q = 0; q < 8; q++) {
                        if (errarry[p, q] < maxerr) {
                            domindx = p;
                            trnindx = q;
                            maxerr = errarry[p, q];
                        }
                    }
                }
                double ab = 0;
                double ak = 0;
                double bk = 0;
                double ak2 = 0;
                for (int n = 0; n < bloksiz; n++) {
                    for (int m = 0; m < bloksiz; m++) {
                        ab += domB[m, n, trnindx, domindx] * rngB[m, n, r];
                        ak += domB[m, n, trnindx, domindx];
                        bk += rngB[m, n, r];
                        ak2 += Math.Pow(domB[m, n, trnindx, domindx], 2);
                    }
                }
                int blokpixn = bloksiz * bloksiz;
                double si = ((blokpixn * ab) - (ak * bk)) / ((blokpixn * ak2) - Math.Pow(ak, 2));
                double oi = (bk - (si * ak)) / blokpixn;
                fracode[r, 0] = domindx;
                fracode[r, 1] = trnindx;
                fracode[r, 2] = si;
                fracode[r, 3] = oi;
                toolStripProgressBar1.PerformStep();
            }
            
            toolStripStatusLabel1.Text = "Complete!";
            string svdir = Application.StartupPath; //save file
            string svname = "fractal" + ".dat";
            string svpath = Path.Combine(svdir, svname);
            using (BinaryWriter writer = new BinaryWriter(File.Open(svpath, FileMode.Create)))
            {
                for (int a = 0; a < rngcont; a++) {
                    for (int b = 0; b < 4; b++) {
                        writer.Write(fracode[a, b]);
                    }
                }
            }
        }
        
        private void button2_Click(object sender, EventArgs e) //decoding
        {
            toolStripProgressBar1.Maximum = iternum;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Value = 0;
            pictureBox1.Size = new System.Drawing.Size(xcord, ycord);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;           
            SNR = new double [iternum];
            frcmap = new Bitmap [iternum];
            int[,] frcP = new int[xcord, ycord];
            int[,] domP, domT;
            int rngendx = xcord - bloksiz + 1;
            int rngendy = ycord - bloksiz + 1;
            int pixrow = xcord - (2*bloksiz) + 1;
            int u = 0, v = 0;
            for (int t = 0; t < iternum; t++) {
                frcmap[t] = new Bitmap(xcord, ycord);
                double noise = 0;
                for (int y = 0; y < rngendy; y += bloksiz) {
                    for (int x = 0; x < rngendx; x += bloksiz) {
                        int r = (y / bloksiz) * (xcord / bloksiz) + (x / bloksiz);
                        domP = new int[bloksiz, bloksiz];
                        domT = new int[bloksiz, bloksiz];
                        for (int j = 0; j < bloksiz; j++) {//define domain block
                            for (int i = 0; i < bloksiz; i++) {
                                int xdom;
                                int ydom = Math.DivRem((int)fracode[r, 0], pixrow, out xdom);
                                domP[i, j] = plnY[xdom + (2 * i), ydom + (2 * j)];
                            }
                        }
                        for (int j = 0; j < bloksiz; j++) {
                            for (int i = 0; i < bloksiz; i++) {
                                //define transfer function  
                                if (fracode[r, 1] == 0) { u = i; v = j; } //original
                                else if (fracode[r, 1] == 1) { u = j; v = (bloksiz - 1) - j; } //rotate 90(deg)
                                else if (fracode[r, 1] == 2) { u = (bloksiz - 1) - i; v = (bloksiz - 1) - j; }  //rotate 180(deg)
                                else if (fracode[r, 1] == 3) { u = (bloksiz - 1) - j; v = i; } //rotate 270(deg)
                                else if (fracode[r, 1] == 4) { u = (bloksiz - 1) - i; v = j; } //mirror 90 (deg)
                                else if (fracode[r, 1] == 5) { u = i; v = (bloksiz - 1) - j; } //mirror 180(deg)
                                else if (fracode[r, 1] == 6) { u = (bloksiz - 1) - j; v = (bloksiz - 1) - i; } //mirror 45 (deg)
                                else { u = j; v = i; } //mirror 135(deg)
                                //define transfered domain
                                domT[i, j] = domP[u, v];
                                frcP[x + i, y + j] = (int)((domT[i, j] * fracode[r, 2]) + fracode[r, 3]);
                            }
                        }
                    }
                }
                plnY = frcP;
                for (int i = 0; i < ycord; i++) {
                    for (int j = 0; j < xcord; j++) {
                        if (frcP[i, j] > 255) { frcP[i, j] = 255; }
                        if (frcP[i, j] < 0) { frcP[i, j] = 0; }
                        noise += (plnY[i, j] - plnF[i, j]) * (plnY[i, j] - plnF[i, j]);
                        frcmap[t].SetPixel(j, i, Color.FromArgb(frcP[i, j], frcP[i, j], frcP[i, j]));
                    }
                }
                SNR[t] = Math.Round(10 * Math.Log10(signal / noise), 2);           
                pictureBox1.Refresh();
                label4.Refresh();
                pictureBox1.Image = frcmap[t];
                label4.Text = "" + SNR[t];               
                toolStripProgressBar1.PerformStep();
                Thread.Sleep(500);
            }           
        }

        private void button3_Click(object sender, EventArgs e) //選取做為變換基底的圖片
        {      
            int[,] basP = new int[xcord, ycord];
            int[,] basR, basG, basB;
            ImagePath = GetImage();
            OpenPcx(out basR, out basG, out basB);
            Bitmap basemap = new Bitmap(xcord, ycord);
            for (int i = 0; i < ycord; i++) {
                for (int j = 0; j < xcord; j++) {
                    basP[i, j] = (basR[i, j] + basG[i, j] + basB[i, j]) / 3;
                    basemap.SetPixel(j, i, Color.FromArgb(basP[i, j], basP[i, j], basP[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)basemap.Width, (int)basemap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = basemap;
            plnY = basP;
        }

        private String GetImage()
        {
            path.InitialDirectory = Application.StartupPath;//預設開啟的位置(Debug)
            path.Filter = "Image Files|*.jpg;*.pcx;*.png;*.png|All Files|*.*";//會先顯示哪些檔
            path.ShowDialog();
            return path.FileName;
        }

        void OpenPcx(out int[,] R, out int[,] G, out int[,] B)
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
            byte[] palette = new byte[768];//Color palette setting
            //If a PCX file has a 256-color palette, it is found 768 bytes from the end of the file.            
            long infoLength = imageinfo.Length;
            int RleLength = (int)infoLength - 128 - 769;
            imageinfo.Seek(-768, SeekOrigin.End);//讀取最後768個bytes          
            int[] CP256_R = new int[256];
            int[] CP256_G = new int[256];
            int[] CP256_B = new int[256];
            int[] C_Combine256 = new int[256];
            for (int i = 0; i < 768; i++) {
                palette[i] = (byte)imageinfo.ReadByte(); //header裡的調色盤
            }
            int k = 0;
            for (int i = 0; i < 768; i = i + 3) {
                CP256_R[k] = palette[i];  //調色盤R 的資訊
                CP256_G[k] = palette[i + 1];  //調色盤G 的資訊
                CP256_B[k] = palette[i + 2];  //調色盤B 的資訊
                k++;
            }
            k = 0;
            imageinfo.Seek(128, SeekOrigin.Begin);// Set the stream position to the beginning of the file.
            
            char bitsPerPixel = BitConverter.ToChar(fileHeader, 3);
            byte nPlanes = fileHeader[65];
            int bytesPerLine = BitConverter.ToInt16(fileHeader, 66);
            int totalBytesPerLine = nPlanes * bytesPerLine;
            int linePaddingSize = ((bytesPerLine * nPlanes) * (8 / bitsPerPixel)) - ((xMax - xMin) + 1);
            byte[] scanLine = new byte[totalBytesPerLine];
            byte nRepeat, pColor;
            int pIndex = 0;
            int xSize = xMax - xMin + 1;
            int ySize = yMax - yMin + 1;
            byte[] imageBytes = new byte[totalBytesPerLine * ySize];
            R = new int[xSize, ySize];
            G = new int[xSize, ySize];
            B = new int[xSize, ySize];

            for (int iY = 0; iY < ySize; iY++) {
                int iX = 0;
                while (iX < totalBytesPerLine) {
                    nRepeat = (byte)imageinfo.ReadByte();
                    if (nRepeat > 192) {
                        nRepeat -= 192;
                        pColor = (byte)imageinfo.ReadByte();
                        for (int j = 0; j < nRepeat; j++) {
                            if (iX < scanLine.Length) {
                                scanLine[iX] = pColor;
                                imageBytes[pIndex] = pColor;
                            }
                            iX++;
                            pIndex++;
                        }
                    }
                    else {
                        if (iX < scanLine.Length) {
                            scanLine[iX] = nRepeat;
                            imageBytes[pIndex] = nRepeat;
                        }
                        iX++;
                        pIndex++;
                    }
                }
            }
            for (int i = 0; i < ySize; i++) {
                for (int j = 0; j < xSize; j++) {
                    R[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3];
                    G[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 1];
                    B[i, j] = (int)palette[(imageBytes[(j) + (i) * xSize]) * 3 + 2];
                }
            }
            imageinfo.Close();
            xcord = xSize;
            ycord = ySize;
        }

        private void button4_Click(object sender, EventArgs e) //load coded *.dat
        {
            OpenFileDialog loadpath = new OpenFileDialog();   
            loadpath.InitialDirectory = Application.StartupPath;
            loadpath.Filter = "dat Files|*.dat|All Files|*.*";//顯示dat檔
            loadpath.ShowDialog();
            string datpath = loadpath.FileName;
            rngcont = (xcord / bloksiz) * (ycord / bloksiz);
            fracode = new double[rngcont, 4];

            using (BinaryReader reader = new BinaryReader(File.Open(datpath, FileMode.Open)))
            {
                for (int a = 0; a < rngcont; a++) {
                    for (int b = 0; b < 4; b++) {
                        fracode[a, b] = reader.ReadDouble();
                    }
                }

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1) { bloksiz = 8; }
            if (comboBox1.SelectedIndex == 2) { bloksiz = 4; }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 1) { iternum = 5; }
            if (comboBox2.SelectedIndex == 2) { iternum = 10; }
            if (comboBox2.SelectedIndex == 3) { iternum = 15; }
           
        }
    }
}