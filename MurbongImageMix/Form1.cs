using MaterialSkin.Controls;
using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MurbongImageMix
{

    public partial class Form1 : MaterialForm
    {

        public Form1()
        {
            InitializeComponent();

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;

            materialSkinManager.ColorScheme = new ColorScheme(
        Primary.Blue400, Primary.Blue500,
        Primary.Blue500, Accent.LightBlue200,
        TextShade.BLACK
    );
        }

        private string OpenBitMap()
        {
            Bitmap img;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "불러오기";
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.DefaultExt = "png";
            openFileDialog.Filter = "png(*.png)|*.png";
            string path;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                img = new Bitmap(path);
            }
            else
            {
                img = null;
                path = "";
            }

            return path;
        }

        private Bitmap GammaCorrection(Bitmap img, double gamma, double c = 1d)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData srcData = img.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb); // BitmapData를 새로 만든다. PixelFormat.Format32bppArgb
            int bytes = srcData.Stride * srcData.Height;//byte의 크기를 구하는 코드 Stride = width
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            img.UnlockBits(srcData);
            int current = 0;
            int cChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * srcData.Stride + x * 4;
                    for (int i = 0; i < cChannels; i++)
                    {
                        double range = (double)buffer[current + i] / 255;
                        double correction = c * Math.Pow(range, gamma); // Gamma = Fade*(RGB^Gamma)
                        result[current + i] = (byte)(correction * 255); // I = R,G,B Number
                    }
                    result[current + 3] = 255; // Alpha Number;
                }
            }
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap png = new Bitmap(OpenBitMap());
            Bitmap png2 = new Bitmap(OpenBitMap());
            png = GammaCorrection(png, 0.023);
            png2 = GammaCorrection(png2, 1, 0.6);
            Bitmap out1 = new Bitmap(png.Size.Width * 2, png.Size.Height * 2);

            for (int i = 0; i < out1.Size.Width; i++)
            {
                for (int j = 0; j < out1.Size.Height; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        out1.SetPixel(i, j, png.GetPixel(i / 2, j / 2));
                    }
                    else
                    {
                        out1.SetPixel(i, j, png2.GetPixel(i / 2, j / 2));
                    }

                }
            }

            out1.Save("HELLO2.png");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
