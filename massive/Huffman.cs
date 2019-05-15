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
    public partial class Huffman : Form
    {
        public int xcord;
        public int ycord;
        public int[,] plnR;
        public int[,] plnG;
        public int[,] plnB;
        public int[,] plnGry;
        int[] PDF = new int[256];
        int[] pixcnt = new int[256];
        int[] pixvlu = new int[256];
        int[] pixlist;
        string [] codelist;
        string[,] hufmtable;
        string[,] encG;
        

        public Huffman(int xdim, int ydim, int[,] Rdim, int[,] Gdim, int[,] Bdim)
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
            //histogram  
            int cntmax = 0;
            for (int i = 0; i < 256; i++)
            {
                pixcnt[i] = 0;
                PDF[i] = 0;
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
            for (int p = 0; p < 256; p++)
            {
                pixvlu[p] = p;
            }
            xcord = xdim;
            ycord = ydim;
            plnR = Rdim;
            plnG = Gdim;
            plnB = Bdim;
            plnGry = C2G; 
        }

        private void Huffman_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //huffman coder
        {
            pictureBox1.Image = null; //清除圖片
            int cntsum = xcord * ycord;
            Array.Sort(pixcnt, pixvlu);
            List<int> hufmnode = new List <int>();
            List<int> hufmfrq = new List <int>();
            List<int> hufmpix = new List<int>();
            List<int> hufmtree = new List<int>();  

            for (int i = 0; i < 256; i++)
            {
                hufmfrq.Add(pixcnt[i]);  //已排序(sort)的陣列
                hufmpix.Add(pixvlu[i]);
            }
            int pix1 = 0;
            for (int i = 0; i < 256; i++) //搜尋沒出現過的像素
            {
                if (pixcnt[i] > 0) { pix1 = i; break; }
            }
            int codecont = 256-pix1;
            hufmfrq.RemoveRange(0, pix1); //刪除沒出現的像素
            hufmpix.RemoveRange(0, pix1);
            hufmtable = new string[codecont, 4]; //建立像素對應的編碼
            string[] hufmbit = new string [2];
            int tmpcnt = 0;

            for (int i = 0; i < codecont-1; i++)
            {
                int loop = -i - 1;
                if (i == 0)
                {
                    int[] frqarry = new int[hufmfrq.Count];
                    int[] pixarry = new int[hufmpix.Count];
                    hufmfrq.CopyTo(frqarry);
                    hufmpix.CopyTo(pixarry);
                    hufmfrq.Clear();
                    hufmpix.Clear();
                    Array.Sort(frqarry, pixarry);
                    for (int k = 0; k < frqarry.Length; k++)
                    {
                        hufmfrq.Add(frqarry[k]);
                        hufmpix.Add(pixarry[k]);
                    }
                    tmpcnt = frqarry[0] + frqarry[1];
                    hufmnode.Add(pixarry[0]);
                    hufmnode.Add(pixarry[1]);
                    hufmtree.Add(loop);
                    hufmtree.Add(loop);
                    hufmfrq.RemoveRange(0, 2);
                    hufmpix.RemoveRange(0, 2);
                    hufmfrq.Insert(0, tmpcnt);
                    hufmpix.Insert(0, loop);
                    hufmtable[0, 0] = Convert.ToString(pixarry[0]);
                    hufmtable[1, 0] = Convert.ToString(pixarry[1]);
                    hufmtable[0, 1] = "0";
                    hufmtable[1, 1] = "1";
                    hufmtable[0, 2] = Convert.ToString(frqarry[0]);
                    hufmtable[1, 2] = Convert.ToString(frqarry[1]);
                }
                else
                {
                    int[] frqarry = new int[hufmfrq.Count];
                    int[] pixarry = new int[hufmpix.Count];                
                    hufmfrq.CopyTo(frqarry); //將List 複製到Array 
                    hufmpix.CopyTo(pixarry);
                    hufmfrq.Clear();
                    hufmpix.Clear();
                    Array.Sort(frqarry, pixarry);
                    for (int k = 0; k < frqarry.Length; k++)
                    {
                        hufmfrq.Add(frqarry[k]);
                        hufmpix.Add(pixarry[k]);
                    }
                    if (pixarry[0] >=0 && pixarry[1] >=0)
                    {
                        tmpcnt = frqarry[0] + frqarry[1];            
                        //舊節點改變編碼
                        //將新進霍夫曼節點加入霍夫曼表
                        int srtindx = 0;
                        for (int k = 0; k < codecont; k++)
                        {
                            if (hufmtable[k, 0] == null) { srtindx = k; break; }
                        }
                        hufmtable[srtindx, 0] = Convert.ToString(pixarry[0]);
                        hufmtable[srtindx, 1] = "0";
                        hufmtable[srtindx, 2] = Convert.ToString(frqarry[0]);
                        hufmtable[srtindx+1, 0] = Convert.ToString(pixarry[1]);
                        hufmtable[srtindx+1, 1] = "1";
                        hufmtable[srtindx+1, 2] = Convert.ToString(frqarry[1]);
                        //將出現次數最小的兩個像素的次數相加，並編號
                        hufmfrq.RemoveRange(0, 2);
                        hufmpix.RemoveRange(0, 2);
                        hufmfrq.Insert(0, tmpcnt);
                        hufmpix.Insert(0, loop);
                        hufmtree.Add(loop);
                        hufmtree.Add(loop);
                        hufmnode.Add(pixarry[0]);
                        hufmnode.Add(pixarry[1]);                                
                    }

                    else if (pixarry[0] < 0 && pixarry[1] >= 0)
                    {
                        tmpcnt = frqarry[0] + frqarry[1];
                        //將新進霍夫曼節點加入霍夫曼表
                        int srtindx = 0;
                        for (int k = 0; k < codecont; k++)
                        {
                            if (hufmtable[k, 0] == null) { srtindx = k; break; }
                        }
                        hufmtable[srtindx, 0] = Convert.ToString(pixarry[1]);
                        hufmtable[srtindx, 1] = "1";
                        hufmtable[srtindx, 2] = Convert.ToString(frqarry[1]);
                        //將出現次數最小的兩個像素的次數相加，並編號
                        hufmfrq.RemoveRange(0, 2);
                        hufmpix.RemoveRange(0, 2);
                        hufmfrq.Insert(0, tmpcnt);
                        hufmpix.Insert(0, loop);
                        hufmnode.Add(pixarry[1]);
                        //建立像素出現次數累加的歷史
                        int[] seqarry1 = SearchNodeIndex(hufmtree, pixarry[0]);
                        for (int u = 0; u < seqarry1.Length - 1; u++)
                        {
                            int seqindx1 = seqarry1[u]-1;
                            //增加bit給pixel <0 的舊像素 
                            hufmbit[0] = hufmtable[seqindx1, 1];
                            hufmbit[1] = "0";
                            hufmtable[seqindx1, 1] = hufmbit[1] + hufmbit[0];
                            //增加新進像素
                            hufmtree.RemoveAt(seqindx1);
                            hufmtree.Insert(seqindx1, loop);
                        }
                        hufmtree.Add(loop);                                
                    }

                    else if (pixarry[0] >= 0 && pixarry[1] < 0)
                    {
                        tmpcnt = frqarry[0] + frqarry[1];
                        //新節點加入霍夫曼表
                        int srtindx = 0;
                        for (int k = 0; k < codecont; k++)
                        {
                            if (hufmtable[k, 0] == null) { srtindx = k; break; }
                        }
                        hufmtable[srtindx, 0] = Convert.ToString(pixarry[0]);
                        hufmtable[srtindx, 1] = "1";
                        hufmtable[srtindx, 2] = Convert.ToString(frqarry[0]);
                        //將出現次數最小的兩個像素的次數相加，並編號     
                        hufmfrq.RemoveRange(0, 2);
                        hufmpix.RemoveRange(0, 2);
                        hufmfrq.Insert(0, tmpcnt);
                        hufmpix.Insert(0, loop);
                        hufmnode.Add(pixarry[0]);
                        //建立像素出現次數累加的歷史
                        int[] seqarry2 = SearchNodeIndex(hufmtree, pixarry[1]);
                        for (int u = 0; u < seqarry2.Length - 1; u++)
                        {
                            int seqindx2 = seqarry2[u]-1;
                            //增加bit給pixel <0 的舊像素 
                            hufmbit[0] = hufmtable[seqindx2, 1];
                            hufmbit[1] = "0";
                            hufmtable[seqindx2, 1] = hufmbit[1] + hufmbit[0];
                            //增加新進像素
                            hufmtree.RemoveAt(seqindx2);
                            hufmtree.Insert(seqindx2, loop);
                        }
                        hufmtree.Add(loop);
                    }

                    else //像素值皆為負號，沒有新像素加入節點
                    {
                        tmpcnt = frqarry[0] + frqarry[1];
                        hufmfrq.RemoveRange(0, 2);
                        hufmpix.RemoveRange(0, 2);
                        hufmfrq.Insert(0, tmpcnt);
                        hufmpix.Insert(0, loop);
                        int[] seqarry3 = SearchNodeIndex(hufmtree, pixarry[0]);
                        for (int u = 0; u < seqarry3.Length - 1; u++)
                        {
                            int seqindx3 = seqarry3[u]-1;
                            //增加bit給pixel <0 的舊像素 
                            hufmbit[0] = hufmtable[seqindx3, 1];
                            hufmbit[1] = "0";
                            hufmtable[seqindx3, 1] = hufmbit[1] + hufmbit[0];

                            hufmtree.RemoveAt(seqindx3);
                            hufmtree.Insert(seqindx3, loop);
                        }
                        int[] seqarry4 = SearchNodeIndex(hufmtree, pixarry[1]);
                        for (int u = 0; u < seqarry4.Length - 1; u++)
                        {
                            int seqindx4 = seqarry4[u] - 1;
                            //增加bit給pixel <0 的舊像素 
                            hufmbit[0] = hufmtable[seqindx4, 1];
                            hufmbit[1] = "1";
                            hufmtable[seqindx4, 1] = hufmbit[1] + hufmbit[0];

                            hufmtree.RemoveAt(seqindx4);
                            hufmtree.Insert(seqindx4, loop);
                        }  
                    }
                }
                toolStripProgressBar1.PerformStep();
            }
            pixlist = new int[codecont];
            codelist = new string[codecont];
            int newsize = 0;
            for (int r = 0; r < codecont; r++)
            {
                int codelngth = hufmtable[r, 1].Length;
                hufmtable[r, 3] = Convert.ToString(codelngth);
                pixlist[r] = Convert.ToInt16(hufmtable[r, 0]);
                codelist[r] = hufmtable[r, 1];
                newsize += codelngth * Convert.ToInt16(hufmtable[r, 2]);
            }
            //huffman table complete
            //hufmtable[k, 0]: pixel, hufmtable[k, 1]: huffman code, hufmtable[k, 2]: pixel count, hufmtable[k, 3]: huffman code length
            double cmpratio = Math.Round((double)(xcord * ycord * 8) / (double)newsize, 2);
            label3.Text = "" + cmpratio;
            //show huffman table in listview
            listView1.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("pixel", 45, HorizontalAlignment.Center);
            listView1.Columns.Add("Huffman code", 110, HorizontalAlignment.Left);          
            listView1.Columns.Add("count", 45, HorizontalAlignment.Center);
            listView1.Columns.Add("code length", 75, HorizontalAlignment.Center);

            ListViewItem[] table = new ListViewItem[codecont];

            for (int k = 0; k < codecont; k++)
            {
                table[k] = new ListViewItem(new string[] { hufmtable[k, 0], hufmtable[k, 1], hufmtable[k, 2], hufmtable[k, 3] });
                listView1.Items.AddRange(new ListViewItem[] { table[k] });
            }
            //encoding image plane
            encG = new string [xcord, ycord];
            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int pixindx = Array.IndexOf(pixlist, plnGry[i, j]);
                    encG[i, j] = hufmtable[pixindx, 1];
                }
            }
            toolStripStatusLabel1.Text = "Complete!";
        }

        private void button2_Click(object sender, EventArgs e) //huffman decoder
        {
            int [,] decG = new int[xcord, ycord];
            Bitmap hufmap = new Bitmap(xcord, ycord);

            for (int i = 0; i < ycord; i++)
            {
                for (int j = 0; j < xcord; j++)
                {
                    int pixindx = Array.IndexOf(codelist, encG[i, j]);
                    decG[i, j] = pixlist[pixindx];
                    hufmap.SetPixel(j,i, Color.FromArgb(decG[i, j],decG[i, j],decG[i, j]));
                }
            }

            pictureBox1.Size = new System.Drawing.Size((int)hufmap.Width, (int)hufmap.Height);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = hufmap;
        }

        private int[] SearchNodeIndex(List<int> seqlist, int targt)
        {
            List<int> indxlist = new List<int>();
            seqlist.Insert(0, 0);
            indxlist.Clear();
            int indx = 1;
            while (indx > 0)
            {
                indx = seqlist.IndexOf(targt, indx);
                indxlist.Add(indx);
                indx++;
            }
            seqlist.Remove(0);
            int[] indxarry = indxlist.ToArray();
            return indxarry;
        }

    }
}