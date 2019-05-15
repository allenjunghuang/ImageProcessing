using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace massive
{
    public partial class Rotate : Form
    {

        public int xcord;
        public int ycord;
        public int[,] Rgrid;
        public int[,] Ggrid;
        public int[,] Bgrid;

        public Rotate(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
        {
            InitializeComponent();
            Bitmap sourcemap = new Bitmap(xdim, ydim);
            for (int i = 0; i < ydim; i++)
            {
                for (int j = 0; j < xdim; j++)
                {
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rdim[i, j], Gdim[i, j], Bdim[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)sourcemap.Width, (int)sourcemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = sourcemap;//put the map into picturebox
            xcord = xdim;
            ycord = ydim;
            Rgrid = Rdim;
            Ggrid = Gdim;
            Bgrid = Bdim;
        }

        private void Rotate_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //forward
        {            
            string rotatedegree = textBox1.Text;
            int degree = Convert.ToInt16(rotatedegree);            
            double radian = (3.14159) * degree / 180;
            double costheta = Math.Cos(radian);
            double sintheta = Math.Sin(radian);
            int rxdim = (int)(Math.Abs(ycord * sintheta) + Math.Abs(xcord * costheta));
            int rydim = (int)(Math.Abs(ycord * costheta) + Math.Abs(xcord * sintheta));                     
            Bitmap rotatemap = new Bitmap(rxdim+1, rydim+1);
            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    int x = (int)((j - ycord / 2) *  costheta + (i - xcord / 2) * sintheta);
                    int y = (int)((j - ycord / 2) * -sintheta + (i - xcord / 2) * costheta);
                    rotatemap.SetPixel(x + rxdim / 2, y + rydim / 2, Color.FromArgb(Rgrid[i,j], Ggrid[i,j], Bgrid[i,j]));                  
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)rotatemap.Width, (int)rotatemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = rotatemap;//put the map into picturebox
        }

        private void button2_Click(object sender, EventArgs e) //backward
        {
            string rotatedegree = textBox1.Text;
            int degree = Convert.ToInt16(rotatedegree);
            double radian = (3.14159) * degree / 180;
            double costheta = Math.Cos(radian);
            double sintheta = Math.Sin(radian);          
            int rxdim = (int)(Math.Abs(ycord * sintheta) + Math.Abs(xcord * costheta));
            int rydim = (int)(Math.Abs(ycord * costheta) + Math.Abs(xcord * sintheta));    

            Bitmap rotatemap = new Bitmap(rxdim+1, rydim+1);

            for (int j = 0; j < rydim; j++)
            {
                for (int i = 0; i < rxdim; i++)
                {
                    int y = (int)((j - rydim / 2) * costheta + (i - rxdim / 2) * -sintheta) + (ycord / 2);
                    int x = (int)((j - rydim / 2) * sintheta + (i - rxdim / 2) * costheta) + (xcord / 2);
                    if (y >= 0 && x >= 0 && y < ycord && x < ycord)
                    {
                        rotatemap.SetPixel(j , i , Color.FromArgb(Rgrid[x, y], Ggrid[x, y], Bgrid[x, y]));
                    }
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)rotatemap.Width, (int)rotatemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = rotatemap;//put the map into picturebox


        }

        private void button3_Click(object sender, EventArgs e) //vertical mirror 
        {
            Bitmap mirrormap = new Bitmap(xcord, ycord);

            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    mirrormap.SetPixel(j, i, Color.FromArgb(Rgrid[i, ycord - j - 1], Ggrid[i, ycord - j - 1], Bgrid[i, ycord - j - 1]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)mirrormap.Width, (int)mirrormap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = mirrormap;//put the map into picturebox
           
        }

        private void button4_Click(object sender, EventArgs e) //horizontal mirror
        {
            Bitmap mirrormap = new Bitmap(xcord, ycord);

            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    mirrormap.SetPixel(j, i, Color.FromArgb(Rgrid[xcord - i - 1, j], Ggrid[xcord - i - 1, j], Bgrid[xcord - i - 1, j]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)mirrormap.Width, (int)mirrormap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = mirrormap;//put the map into picturebox
           
           
        }

        private void button5_Click(object sender, EventArgs e) //45deg mirror
        {
            Bitmap mirrormap = new Bitmap(xcord, ycord);

            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {
                    mirrormap.SetPixel(j, i, Color.FromArgb(Rgrid[xcord - j - 1, ycord - i - 1], Ggrid[xcord - j - 1, ycord - i - 1], Bgrid[xcord - j - 1, ycord - i - 1]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)mirrormap.Width, (int)mirrormap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = mirrormap;//put the map into picturebox
        }

        private void button6_Click(object sender, EventArgs e) //135deg mirror
        {
            Bitmap mirrormap = new Bitmap(xcord, ycord);

            for (int j = 0; j < ycord; j++)
            {
                for (int i = 0; i < xcord; i++)
                {                 
                    mirrormap.SetPixel(j, i, Color.FromArgb(Rgrid[j, i], Ggrid[j, i], Bgrid[j, i]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)mirrormap.Width, (int)mirrormap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = mirrormap;//put the map into picturebox
        }

        private void button7_Click(object sender, EventArgs e) //resume
        {
            Bitmap sourcemap = new Bitmap(xcord, ycord);
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    sourcemap.SetPixel(j, i, Color.FromArgb(Rgrid[i, j], Ggrid[i, j], Bgrid[i, j]));
                }
            }
            pictureBox1.Size = new System.Drawing.Size((int)sourcemap.Width, (int)sourcemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = sourcemap;//put the map into picturebox

        }

        private void Rotate90() //rotate 90(deg)
        {

            Bitmap rotatemap = new Bitmap(xcord, ycord);

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int x = j;
                    int y = ycord - 1- i;

                    rotatemap.SetPixel(j, i, Color.FromArgb(Rgrid[x, y], Ggrid[x, y], Bgrid[x, y]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)rotatemap.Width, (int)rotatemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = rotatemap;//put the map into picturebox
          
        }

        private void Rotate180() //rotate 180(deg)
        {
            Bitmap rotatemap = new Bitmap(xcord, ycord);

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int x = xcord - 1 - i;
                    int y = ycord - 1 - j;

                    rotatemap.SetPixel(j, i, Color.FromArgb(Rgrid[x, y], Ggrid[x, y], Bgrid[x, y]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)rotatemap.Width, (int)rotatemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = rotatemap;//put the map into picturebox
        }

        private void Rotate270()//rotate 270(deg)
        {
            Bitmap rotatemap = new Bitmap(xcord, ycord);

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int x = xcord - 1 - j;
                    int y = i;

                    rotatemap.SetPixel(j, i, Color.FromArgb(Rgrid[x, y], Ggrid[x, y], Bgrid[x, y]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)rotatemap.Width, (int)rotatemap.Height); //control the picturebox dimension with map  
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//set the map dimension to fit the picturebox  
            pictureBox1.Image = rotatemap;//put the map into picturebox
        }
    }
}