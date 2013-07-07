#if DEBUG && !XDK
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

using CoreEngine;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        static bool SaveScreenshot = false;
        private void SaveScreenshotCode()
        {
            if (!SaveScreenshot) return;

            int[] clr = new int[1280 * 720];
            MyGraphicsDevice.GetBackBufferData(clr);
            var tx = new Texture2D(MyGraphicsDevice, 1280, 720, false, MyGraphicsDevice.PresentationParameters.BackBufferFormat);
            tx.SetData(clr);

            if (Tools.CurCamera != null)
            {
                var ezt = new EzTexture();
                ezt.Tex = tx;
                var qd = new QuadClass();
                qd.Quad.MyTexture = ezt;
                qd.FullScreen(Tools.CurCamera);
                qd.Draw();
                //Tools.QDrawer.DrawSquareDot(Vector2.Zero, Color.White, 1000, ezt, Tools.BasicEffect);
                Tools.QDrawer.Flush();

                lock (NoMore)
                {
                    Tools.Nothing();
                }

                MakeThread(tx);
            }
        }

        private void MakeThread(Texture2D tx)
        {
            Tools.EasyThread(0, "savetexture", () =>
            {
                lock (NoMore)
                {
                    int frame = Tools.DrawCount;

                    Tools.Write("!!" + frame);
                    //Stream stream = File.OpenWrite(Path.Combine("VideoDump", string.Format("{0}.png", frame)));
                    //tx.SaveAsJpeg(stream, 1280, 720);
                    //stream.Close();
                    //stream.Dispose();
                    MyBmpWriter.TextureToBmp(tx, Path.Combine("VideoDump", string.Format("{0}.png", frame)));

                    tx.Dispose();
                    Tools.Write("??" + frame);
                }
            });
        }
        BmpWriter MyBmpWriter = new BmpWriter(1280, 720);
        LockableBool NoMore = new LockableBool();

        public class BmpWriter
        {
            byte[] textureData;
            System.Drawing.Bitmap bmp;
            System.Drawing.Imaging.BitmapData bitmapData;
            IntPtr safePtr;
            System.Drawing.Rectangle rect;
            public System.Drawing.Imaging.ImageFormat imageFormat;
            public BmpWriter(int width, int height)
            {
                textureData = new byte[4 * width * height];

                bmp = new System.Drawing.Bitmap(
                               width, height,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb
                             );

                rect = new System.Drawing.Rectangle(0, 0, width, height);

                imageFormat = System.Drawing.Imaging.ImageFormat.Png;
            }
            public void TextureToBmp(Texture2D texture, String filename)
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

                bmp.Save(filename, imageFormat);
            }
        }
    }
}
#endif