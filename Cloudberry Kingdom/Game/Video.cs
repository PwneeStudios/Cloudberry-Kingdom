using System;
using System.IO;
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

        static bool CanSkip;
        static float LengthUntilUserCanSkip;

        public static void StartVideo_CanSkipIfWatched(string MovieName)
        {
            bool CanSkip = UserPowers.WatchedVideo[MovieName];
            StartVideo(MovieName, CanSkip, 100000);
        }

        public static void StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(string MovieName, float LengthUntilCanSkip)
        {
            bool CanSkip = UserPowers.WatchedVideo[MovieName];
            StartVideo(MovieName, CanSkip, LengthUntilCanSkip);
        }

        private static void StartVideo(string MovieName, bool CanSkipVideo, float LengthUntilCanSkip)
        {
            CanSkip = CanSkipVideo;
            LengthUntilUserCanSkip = LengthUntilCanSkip;

            UserPowers.WatchedVideo += MovieName;
            UserPowers.SetToSave();

            Playing = true;
            Cleaned = false;

            CurrentVideo = Tools.GameClass.Content.Load<Video>(Path.Combine("Movies", MovieName));

            VPlayer = new VideoPlayer();
            VPlayer.IsLooped = false;
            VPlayer.Play(CurrentVideo);

            Duration = CurrentVideo.Duration.TotalSeconds;
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Returns the length of time the video has already been playing in seconds.
        /// </summary>
        /// <returns></returns>
        static double ElapsedTime()
        {
            return (DateTime.Now - StartTime).TotalSeconds;
        }

        public static void UserInput()
        {
            // End the video if the user presses a key
            if (CanSkip && PlayerManager.Players != null && ElapsedTime() > .3f ||
                ElapsedTime() > LengthUntilUserCanSkip)
            {
                // Update songs
                if (Tools.SongWad != null)
                    Tools.SongWad.PhsxStep();

                ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();

                if (ButtonCheck.AnyKey())
                    Playing = false;

                ButtonCheck.UpdateControllerAndKeyboard_EndOfStep(Tools.TheGame.Resolution);
            }
        }

        public static bool Draw()
        {
            if (!Playing) { Finish(); return false; }

            Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);

            UserInput();

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

        static bool Cleaned = true;
        public static void Finish()
        {
            Playing = false;

            if (Cleaned) return;

            VPlayer.Dispose();
            CurrentVideo = null;

            Cleaned = true;
        }
    }
}
