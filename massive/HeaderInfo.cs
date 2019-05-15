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
    public partial class HeaderInfo : Form
    {
        Bitmap C_palette256 = new Bitmap(64, 64);  
        public HeaderInfo(string ImagePath)
        {
            InitializeComponent();          
            char bitsPerPixel;
            int hdpi;
            int vdpi;
            //read header
            byte[] fileHeader = new byte[128]; //.pcx 影像檔 header為128byte
            FileStream imageinfo = new FileStream(ImagePath, FileMode.Open, FileAccess.Read, FileShare.None);//將檔案打開後讀取
            imageinfo.Read(fileHeader, 0, 128);
            byte isPCX = fileHeader[0];
            label3.Text = "Manufacturer :" + fileHeader[0];
            /* Version information
            0 = Ver. 2.5 of PC Paintbrush                
            2 = Ver. 2.8 w/palette information
            3 = Ver. 2.8 w/o palette information
            4 = PC Paintbrush for Windows(Plus for Windows uses Ver 5)
            5 = Ver. 3.0 and > of PC Paintbrush and PC Paintbrush +, includes Publisher’s Paintbrush. Includes 24-bit .PCX files */

            byte version = fileHeader[1];
            label4.Text = "Version : " + fileHeader[1];
            label5.Text = "Encoding : " + fileHeader[2];

            bitsPerPixel = BitConverter.ToChar(fileHeader, 3); //每個pixel使用多少bit(1, 2, 4, or 8) 

            label6.Text = "Bits per Pixel : " + fileHeader[3];
            int xMin = BitConverter.ToInt16(fileHeader, 4); //傳回從位元組陣列中指定位置的兩個 byte所轉換的16bit帶正負號的整數 (Signed Integer)
            int yMin = BitConverter.ToInt16(fileHeader, 6);
            int xMax = BitConverter.ToInt16(fileHeader, 8);
            int yMax = BitConverter.ToInt16(fileHeader, 10);
            label7.Text = "Image Dimensions : Xmin= " + xMin + "  Ymin= " + yMin + "  Xmax= " + xMax + "  Ymax= " + yMax;
            hdpi = BitConverter.ToInt16(fileHeader, 12);//水平DPI
            label8.Text = "Horizontal dpi: " + hdpi;
            vdpi = BitConverter.ToInt16(fileHeader, 14);//垂直DPI
            label9.Text = "Vertical dpi: " + vdpi;
            byte[] palette = new byte[768];//調色盤Color palette setting

            //palette in header
            if (version < 5)//A PCX file has space in its header for a 16 color (3*16=48 bytes) palette. 
            {
                //-----------------------------------------------------------
                long infoLength = imageinfo.Length;
                int RleLength = (int)infoLength - 128;
                //---------------------------------------------------------------


                label10.Text = "Color palette: " + 16 + " colors";
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


                imageinfo.Seek(-768, SeekOrigin.End);//讀取最後768個bytes
                label10.Text = "Color palette: " + 256 + " colors";


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
                for (int i = 0; i < 1024; i = i + 4) //建立調色盤
                {

                    C_palette256.SetPixel((i % 64), (i / 64) * 4, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 1, (i / 64) * 4, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 2, (i / 64) * 4, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 3, (i / 64) * 4, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));

                    C_palette256.SetPixel((i % 64), (i / 64) * 4 + 1, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 1, (i / 64) * 4 + 1, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 2, (i / 64) * 4 + 1, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 3, (i / 64) * 4 + 1, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));

                    C_palette256.SetPixel((i % 64), (i / 64) * 4 + 2, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 1, (i / 64) * 4 + 2, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 2, (i / 64) * 4 + 2, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 3, (i / 64) * 4 + 2, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));

                    C_palette256.SetPixel((i % 64), (i / 64) * 4 + 3, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 1, (i / 64) * 4 + 3, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 2, (i / 64) * 4 + 3, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));
                    C_palette256.SetPixel((i % 64) + 3, (i / 64) * 4 + 3, Color.FromArgb(CP256_R[j], CP256_G[j], CP256_B[j]));

                    j++;
                }



                pictureBox2.Image = C_palette256;
                // imageinfo.Seek(128, SeekOrigin.Begin);// Set the stream position to the beginning of the file.

            }


            byte nPlanes = fileHeader[65];//Number of bit planes
            label11.Text = "Number of Bit Planes : " + fileHeader[65];
            int bytesPerLine = BitConverter.ToInt16(fileHeader, 66);//Number of bytes to allocate for a scanline plane
            label12.Text = "Bytes per Scan-line : " + bytesPerLine;
        }

        private void HeaderInfo_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (C_palette256 != null) //顯示滑鼠游標位置的RGB
            {
                Color PixelColor = C_palette256.GetPixel(e.X, e.Y);
                int Pix_R = PixelColor.R;
                int Pix_G = PixelColor.G;
                int Pix_B = PixelColor.B;
                toolStripStatusLabel8.Text = " R(" + Pix_R + ")";
                toolStripStatusLabel8.ForeColor = Color.Red;
                toolStripStatusLabel9.Text = " G(" + Pix_G + ")";
                toolStripStatusLabel9.ForeColor = Color.Green;
                toolStripStatusLabel10.Text = " B(" + Pix_B + ")";
                toolStripStatusLabel10.ForeColor = Color.Blue;
            }
        }


    }
}