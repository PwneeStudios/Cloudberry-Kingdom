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

        static Video CurrentVideo;
        static VideoPlayer VPlayer;
        
        static EzTexture VEZTexture = new EzTexture();

        static double Duration;
        static DateTime StartTime;

        public static void Load()
        {
            Playing = true;

            CurrentVideo = Tools.GameClass.Content.Load<Video>("Movies//LogoSalad");

            VPlayer = new VideoPlayer();
            VPlayer.IsLooped = false;
            VPlayer.Play(CurrentVideo);

            Duration = CurrentVideo.Duration.TotalSeconds;
            StartTime = DateTime.Now;
        }

        static double ElapsedTime()
        {
            return (DateTime.Now - StartTime).TotalSeconds;
        }

        public static bool Draw()
        {
            if (!Playing) return false;

            if (ElapsedTime() > Duration)
                Playing = false;

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
