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
    public partial class WaterMark : Form
    {
        OpenFileDialog path = new OpenFileDialog();

        string ImagePath = "";
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
        public int[,] wtrC2Ggrid;
        public int[,] fstC2Ggrid;
        public int[,] scdC2Ggrid;

        public WaterMark(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            pictureBox1.Location = new Point(11, 32);
            pictureBox2.Location = new Point(285, 32);
            pictureBox3.Location = new Point(11, 360);
            pictureBox4.Location = new Point(285, 360);
            label1.Location = new Point(14, 293);
            label2.Location = new Point(285, 293);
            panel1.Location = new Point(11, 313);
            panel2.Location = new Point(285, 313);
            int[,] fstC2G = new int[xdim, ydim];
            Bitmap fstmap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    fstC2G[i, j] = (Rdim[i, j] + Gdim[i, j] + Bdim[i, j]) / 3;
                    int fstbit8 = (((0X80) & (fstC2G[i, j])) / 128);
                    int fstbit7 = (((0X40) & (fstC2G[i, j])) / 64);
                    int fstbit6 = (((0X20) & (fstC2G[i, j])) / 32);
                    int fstbit5 = (((0X10) & (fstC2G[i, j])) / 16);
                    int fstbit4 = (((0X08) & (fstC2G[i, j])) / 8);
                    int fstbit3 = (((0X04) & (fstC2G[i, j])) / 4);
                    int fstbit2 = (((0X02) & (fstC2G[i, j])) / 2);
                    int fstbit1 = ((0X01) & (fstC2G[i, j]));
                    fstmap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            pictureBox1.Width = xdim; pictureBox1.Height = ydim; pictureBox1.Location = new Point(11, 32);
            pictureBox2.Width = xdim; pictureBox2.Height = ydim; pictureBox2.Location = new Point(285, 32);
            pictureBox3.Width = xdim; pictureBox3.Height = ydim; pictureBox3.Location = new Point(11, 360);
            pictureBox4.Width = xdim; pictureBox4.Height = ydim; pictureBox4.Location = new Point(285, 360);
            fstxdim = xdim;
            fstydim = ydim;
            fstR = Rdim;
            fstG = Gdim;
            fstB = Bdim;
            fstC2Ggrid = fstC2G;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImagePath = GetImage();

            OpenPcx(out scdlayer, out scdxdim, out scdydim, out scdR, out scdG, out scdB);

            int[,] scdC2G = new int[scdxdim, scdydim];
            Bitmap scdmap = new Bitmap(scdxdim, scdydim);
            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    scdC2G[i, j] = (scdR[i, j] + scdG[i, j] + scdB[i, j]) / 3;
                    int scdbit8 = (((0X80) & (scdC2G[i, j])) / 128);
                    int scdbit7 = (((0X40) & (scdC2G[i, j])) / 64);
                    int scdbit6 = (((0X20) & (scdC2G[i, j])) / 32);
                    int scdbit5 = (((0X10) & (scdC2G[i, j])) / 16);
                    int scdbit4 = (((0X08) & (scdC2G[i, j])) / 8);
                    int scdbit3 = (((0X04) & (scdC2G[i, j])) / 4);
                    int scdbit2 = (((0X02) & (scdC2G[i, j])) / 2);
                    int scdbit1 = ((0X01) & (scdC2G[i, j]));
                    scdmap.SetPixel(j, i, Color.FromArgb(scdR[i, j], scdG[i, j], scdB[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)scdmap.Width, (int)scdmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = scdmap;
            scdC2Ggrid = scdC2G;          
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
            int[,] wtrW2G = new int[scdxdim, scdydim];
            Bitmap wtrmap = new Bitmap(scdxdim, scdydim);
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    int wtrbit8 = (((0X80) & (fstC2Ggrid[i, j])) / 128);
                    int wtrbit7 = (((0X40) & (fstC2Ggrid[i, j])) / 64);
                    int wtrbit6 = (((0X20) & (fstC2Ggrid[i, j])) / 32);
                    int wtrbit5 = (((0X10) & (fstC2Ggrid[i, j])) / 16);
                    int wtrbit4 = (((0X08) & (fstC2Ggrid[i, j])) / 8);
                    int wtrbit3 = (((0X04) & (fstC2Ggrid[i, j])) / 4);
                    int wtrbit2 = (((0X02) & (fstC2Ggrid[i, j])) / 2);
                    int wtrbit1 = ((0X01) & (scdC2Ggrid[i, j]));
                    wtrW2G[i, j] = ((wtrbit8 * 128) + (wtrbit7 * 64) + (wtrbit6 * 32) + (wtrbit5 * 16) + (wtrbit4 * 8) + (wtrbit3 * 4) + (wtrbit2 * 2) + wtrbit1);
                    wtrmap.SetPixel(j, i, Color.FromArgb(wtrW2G[i, j], wtrW2G[i, j], wtrW2G[i, j]));
                    signal += fstC2Ggrid[i, j] * fstC2Ggrid[i, j];
                    noise += (wtrW2G[i, j] - fstC2Ggrid[i, j]) * (wtrW2G[i, j] - fstC2Ggrid[i, j]);
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox2.Size = new System.Drawing.Size((int)wtrmap.Width, (int)wtrmap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = wtrmap;

            wtrC2Ggrid = wtrW2G;
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

        private void WaterMark_Load(object sender, EventArgs e)
        {
            
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);
            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X01) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;

        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X02) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X04) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X08) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X10) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X20) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X40) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X80) & (scdC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = slicemap;
        }

        private void radioButton16_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X01) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton15_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X02) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton14_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X04) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton13_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X08) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton12_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X10) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton11_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X20) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton10_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X40) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

        private void radioButton9_Click(object sender, EventArgs e)
        {
            Bitmap slicemap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    if (((0X80) & (wtrC2Ggrid[i, j])) == 0)
                        slicemap.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                    else
                        slicemap.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox4.Size = new System.Drawing.Size((int)slicemap.Width, (int)slicemap.Height);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.Image = slicemap;
        }

       
    
    
    }
}