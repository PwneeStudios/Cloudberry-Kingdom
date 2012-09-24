using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
#if WINDOWS
    public class BmpWriter
    {
        byte[] textureData;
        System.Drawing.Bitmap bmp;
        System.Drawing.Imaging.BitmapData bitmapData;
        IntPtr safePtr;
        System.Drawing.Rectangle rect;
        public System.Drawing.Imaging.ImageFormat imageFormat;

        public int Width, Height;

        public BmpWriter(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            textureData = new byte[4 * width * height];

            bmp = new System.Drawing.Bitmap(
                           width, height,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            rect = new System.Drawing.Rectangle(0, 0, width, height);

            imageFormat = System.Drawing.Imaging.ImageFormat.Png;
        }

        public void TextureToBmp(Texture2D texture, String filename, bool DelaySave = false)
        {
            texture.GetData<byte>(textureData);
            byte blue;
            for (int i = 0; i < textureData.Length; i += 4)
            {
                blue = textureData[i];
                textureData[i] = textureData[i + 2];
                textureData[i + 2] = blue;
            }

            bitmapData = bmp.LockBits(
                           rect,
                           System.Drawing.Imaging.ImageLockMode.WriteOnly,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            safePtr = bitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
            bmp.UnlockBits(bitmapData);

            if (DelaySave)
                Tools.Nothing();
            else
                bmp.Save(filename, imageFormat);
        }
    }
#endif
}