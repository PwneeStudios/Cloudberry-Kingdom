using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using CoreEngine;

namespace CloudberryKingdom
{
    class MainVideo
    {
        public static bool Playing = false;

        static Video TestVideo;
        static VideoPlayer VPlayer;
        
        static EzTexture VEZTexture = new EzTexture();

        public static void Load()
        {
            return;

            Playing = true;

            //TestVideo = Tools.GameClass.Content.Load<Video>("Movies//TestCinematic");
            TestVideo = Tools.GameClass.Content.Load<Video>("Movies//LogoSalad");

            VPlayer = new VideoPlayer();
            VPlayer.IsLooped = false;
            VPlayer.Play(TestVideo);
        }

        static bool timed = false;
        public static bool Draw()
        {
            if (!Playing) return false;

            if (!timed)
            {
                Tools.Write(string.Format("First movie draw is {0}", System.DateTime.Now));
                timed = true;
            }

            VEZTexture.Tex = VPlayer.GetTexture();
            VEZTexture.Width = VEZTexture.Tex.Width;
            VEZTexture.Height = VEZTexture.Tex.Height;

            Vector2 Pos = Tools.CurCamera.Pos;
            Tools.QDrawer.DrawToScaleQuad(Pos, Color.White, 3580, VEZTexture, Tools.BasicEffect);
            Tools.QDrawer.Flush();

            return true;
        }
    }
}
