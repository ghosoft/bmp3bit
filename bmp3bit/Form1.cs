using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

//using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;

namespace bmp3bit
{
    public partial class Form1 : Form
    {
        public Bitmap bmpOrigin;
        public Bitmap bmpConvert;

        public Form1()
        {
            InitializeComponent();
        }

        private Color bmpPixel3bitConvert(Color c)
        {
            return Color.FromArgb((c.R > 127 ? 255 : 0), (c.G > 127 ? 255 : 0), (c.B > 127 ? 255 : 0));
        }

        private string ColorToString(Color c)
        {
            string retVal = "";

            retVal += (c.R > 127 ? "1" : "0");
            retVal += (c.G > 127 ? "1" : "0");
            retVal += (c.B > 127 ? "1" : "0");
            
            return retVal;
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                System.Console.WriteLine("open file {0}", fileName);

                try
                {
                    this.pictureBox_origin.Load(fileName);

                    bmpOrigin = new Bitmap(fileName);
                    
                    System.Console.WriteLine("image info: x:{0} y:{1} bbp{2}", bmpOrigin.Width, bmpOrigin.Height, bmpOrigin.Palette);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    bmpConvert = new Bitmap(fileName);
                    for(int y = 0; y < bmpConvert.Height; y++)
                    {
                        for(int x = 0; x < bmpConvert.Width; x++)
                        {
                            bmpConvert.SetPixel(x, y, bmpPixel3bitConvert( bmpConvert.GetPixel(x, y) ) );
                        }
                    }



                    //this.pictureBox_convert.Load(fileName);

                    this.pictureBox_convert.Image = (Image)bmpConvert;

                    //bmpOrigin = new Bitmap(fileName);

                    //imageBmp.Height
                    System.Console.WriteLine("image info: x:{0} y:{1} bbp{2}", bmpOrigin.Width, bmpOrigin.Height, bmpOrigin.Palette);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.pictureBox_origin.Image = null;
            this.pictureBox_convert.Image = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.pictureBox_origin.Image = null;
            this.pictureBox_convert.Image = null;

            //this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            
            
            dialog.DefaultExt = ".c";
            dialog.Title = "保存图片编码";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 保存文件的文件名, 含路径
                string saveFilename = dialog.FileName;

                FileInfo myFile = new FileInfo(saveFilename);
                StreamWriter sw = myFile.CreateText();

                string imageArrayName = "const unsigned char gImage_" + Path.GetFileNameWithoutExtension(saveFilename);

                sw.Write(imageArrayName);
                sw.Write(" = { \n");
                for (int y = 0; y < bmpOrigin.Height; y++)
                {
                    string lineBufferBinString = "";
                    for (int x = 0; x < bmpOrigin.Width; x++)
                    {
                        lineBufferBinString += ColorToString(bmpOrigin.GetPixel(x, y));
                    }
                    // 到这里, 是完整的384bit

                    for (int b = 0; b < bmpOrigin.Width * 3/ 8; b++) {
                        sw.Write("0b");
                        // 把没8bit分成一组
                        sw.Write(lineBufferBinString.Substring(b * 8, 8));
                        sw.Write(", ");
                    }

                    sw.Write('\n');
                }
                sw.WriteLine("};");

                sw.Close();
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("注册成功");
        }
    }
}
