using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MurbongImageMix.lib
{
    public static class Util
    {
        public static Bitmap GammaCorrection(Bitmap img, double gamma, double c = 1d)
        {
            int width = img.Width;
            int height = img.Height;
            BitmapData srcData = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb); // BitmapData를 새로 만든다. PixelFormat.Format32bppArgb
            int bytes = srcData.Stride * srcData.Height;//byte의 크기를 구하는 코드 Stride = width
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes); // memcpy
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

        public static Bitmap MergeBitmap(Bitmap img1, Bitmap img2)
        {
            int width = img1.Width;
            int height = img1.Height;

            BitmapData srcData = img1.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb); // BitmapData를 새로 만든다. PixelFormat.Format32bppArgb
            BitmapData srcData2 = img2.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb); // BitmapData를 새로 만든다. PixelFormat.Format32bppArgb


            int bytes = srcData.Stride * srcData.Height;//byte의 크기를 구하는 코드 Stride = width

            byte[] buffer = new byte[bytes];
            byte[] buffer2 = new byte[bytes];
            byte[] result = new byte[bytes * 4];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes); // memcpy
            Marshal.Copy(srcData2.Scan0, buffer2, 0, bytes);
            img1.UnlockBits(srcData);
            img2.UnlockBits(srcData);
            int current = 0;
            int cChannels = 4;
            Debug.WriteLine(srcData.Width + " " + srcData.Stride);
            for (int y = 0; y < height * 2; y++)
            {
                for (int x = 0; x < width * 2; x++)
                {

                    current = y / 2 * srcData.Stride + x / 2 * 4;
                    int current2 = y * srcData.Stride * 2 + x * 4;



                    for (int i = 0; i < cChannels; i++)
                    {
                        double range;
                        if (x % 2 == 0 && y % 2 == 0)
                        {
                             range = (double)buffer[current + i];
                            
                        }
                        else {
                            range = (double)buffer2[current + i];
                        }
                        result[current2 + i] = (byte)(range); // I = R,G,B Number
                    }
                    //result[current2 + 3] = 255; // Alpha Number;

                }
            }
            Bitmap resImg = new Bitmap(width * 2, height * 2);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes * 4);
            resImg.UnlockBits(resData);
            return resImg;

        }
    }
}
