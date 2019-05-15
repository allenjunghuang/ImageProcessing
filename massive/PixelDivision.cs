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
    public partial class PixelDivision : Form
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

        public PixelDivision(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
    
            fstxdim = xdim;
            fstydim = ydim;
            fstR = Rdim;
            fstG = Gdim;
            fstB = Bdim;

        }

        private void PixelDivision_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            ImagePath = GetImage();
           
            OpenPcx(out scdlayer, out scdxdim, out scdydim, out scdR, out scdG, out scdB);
            
            Bitmap scdmap = new Bitmap(scdxdim, scdydim);

            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    scdmap.SetPixel(j, i, Color.FromArgb(scdR[i, j], scdG[i, j], scdB[i, j]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)scdmap.Width, (int)scdmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = scdmap;     
            
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
            int divr; 
            int divg;
            int divb;
            Bitmap divmap = new Bitmap(scdxdim, scdydim);
            double signal = 0;
            double noise = 0;
            double SNR = 0;
            for (int i = 0; i < scdydim; i++)
            {
                for (int j = 0; j < scdxdim; j++)
                {
                    divr = 255 - (fstR[i, j] - scdR[i, j]);
                    if (divr > 255) { divr = 255; }
                    if (divr < 0) { divr = 0; }
                    divg = 255 - (fstG[i, j] - scdG[i, j]);
                    if (divg > 255) { divg = 255; }
                    if (divg < 0) { divg = 0; }
                    divb = 255 - (fstB[i, j] - scdB[i, j]);
                    if (divb > 255) { divb = 255; }
                    if (divb < 0) { divb = 0; }
                    signal += ((fstR[i, j] * fstR[i, j])) + ((fstG[i, j] * fstG[i, j])) + ((fstB[i, j] * fstB[i, j]));
                    noise += ((divr - fstR[i, j]) * (divr - fstR[i, j])) + ((divb - fstB[i, j]) * (divb - fstB[i, j])) + ((divg - fstG[i, j]) * (divg - fstG[i, j])); 
                    divmap.SetPixel(j, i, Color.FromArgb(divr, divg, divb));
                }
            }
            SNR = Math.Round(10 * Math.Log10(signal / noise), 2);
            label8.Text = "" + SNR;
            pictureBox2.Size = new System.Drawing.Size((int)divmap.Width, (int)divmap.Height);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = divmap;

        
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
    }
}