using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

using CoreEngine;

namespace CloudberryKingdom
{
    class MainVideo
    {
        static ContentManager Content = null;

        public static bool Playing = false;

        static Video CurrentVideo;
        static VideoPlayer VPlayer;
        
        static EzTexture VEZTexture = new EzTexture();

        static double Duration, Elapsed;
        static DateTime StartTime;

        static bool CanSkip;
        static float LengthUntilUserCanSkip;

        static List<Localization.SubtitleAction> Subtitles;
        static int SubtitleIndex;
        static EzText SubtitleText;

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
#if DEBUG
            CanSkipVideo = true;
#endif

            Subtitles = Localization.GetSubtitles(MovieName);

            if (Subtitles != null)
            {
                SubtitleIndex = 0;
            }

            if (SubtitleText != null)
            {
                SubtitleText.Release();
                SubtitleText = null;
            }

            if (Content == null)
            {
                Content = new ContentManager(Tools.GameClass.Services, "Content");
            }

            CanSkip = CanSkipVideo;
            LengthUntilUserCanSkip = LengthUntilCanSkip;

            UserPowers.WatchedVideo += MovieName;
            UserPowers.SetToSave();

            Playing = true;
            Cleaned = false;

            //CurrentVideo = Tools.GameClass.Content.Load<Video>(Path.Combine("Movies", MovieName));
            CurrentVideo = Content.Load<Video>(Path.Combine("Movies", MovieName));

            VPlayer = new VideoPlayer();
            VPlayer.IsLooped = false;
            VPlayer.Play(CurrentVideo);

            VPlayer.Volume = Math.Max(Tools.MusicVolume.Val, Tools.SoundVolume.Val);

            Elapsed = 0;
            Duration = CurrentVideo.Duration.TotalSeconds;
            StartTime = DateTime.Now;
        }

        public static void UpdateElapsedTime()
        {
            Elapsed += Tools.TheGame.DeltaT;
        }

        static bool Paused = false;
        static void UserInput()
        {
            ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();

//#if WINDOWS && DEBUG
//            if (ButtonCheck.State(Keys.P).Pressed)
//            {
//                if (Paused)
//                    VPlayer.Resume();
//                else
//                    VPlayer.Pause();
                
//                Paused = !Paused;
//            }

//            if (!(ButtonCheck.State(Keys.P).Down))
//#endif

            // End the video if the user presses a key
            if (CanSkip && PlayerManager.Players != null && Elapsed > .3f ||
                Elapsed > LengthUntilUserCanSkip)
            {
                // Update songs
                if (Tools.SongWad != null)
                    Tools.SongWad.PhsxStep();

                if (ButtonCheck.AnyKey())
                    Playing = false;
            }

            ButtonCheck.UpdateControllerAndKeyboard_EndOfStep(Tools.TheGame.Resolution);
        }

        static void Subtitle()
        {
            if (Subtitles == null) return;

            if (SubtitleText != null)
            {
                SubtitleText.Draw(Tools.CurCamera);
                Tools.QDrawer.Flush();
            }

            if (SubtitleIndex >= Subtitles.Count) return;

            var NextSubtitle = Subtitles[SubtitleIndex];
            if (Elapsed > NextSubtitle.Time)
            {
                switch (NextSubtitle.MyAction)
                {
                    case Localization.SubtitleAction.ActionType.Show:
                        SubtitleText = new EzText(NextSubtitle.Text, Resources.Font_Grobold42, 1433.333f, true, true, .666f);
                        SubtitleText.Show = true;
                        SubtitleText.Scale = .4000f;
                        SubtitleText.Pos = new Vector2(0, -830 + SubtitleText.Height / 2);
                        SubtitleText.MyFloatColor = ColorHelper.Gray(.925f);
                        SubtitleText.OutlineColor = ColorHelper.Gray(.100f);
                        
                        break;

                    case Localization.SubtitleAction.ActionType.Hide:
                        SubtitleText.Show = false;
                        SubtitleText.Release();

                        break;
                }

                SubtitleIndex++;
            }
        }

        public static bool Draw()
        {
            if (!Playing) { Finish(); return false; }

            Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);

            UpdateElapsedTime();
            UserInput();

            if (Elapsed > Duration)
                Playing = false;

            VEZTexture.Tex = VPlayer.GetTexture();
            VEZTexture.Width = VEZTexture.Tex.Width;
            VEZTexture.Height = VEZTexture.Tex.Height;

            Vector2 Pos = Tools.CurCamera.Pos;
            Tools.QDrawer.DrawToScaleQuad(Pos, Color.White, 3580, VEZTexture, Tools.BasicEffect);
            Tools.QDrawer.Flush();

            Subtitle();

//#if WINDOWS && DEBUG
//                Tools.StartSpriteBatch();
//                Tools.Render.MySpriteBatch.DrawString(Resources.LilFont.Font,
//                        ElapsedTime().ToString(),
//                        Tools.CurCamera.Pos + new Vector2(900, 100),
//                        Color.Orange, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
//                Tools.Render.EndSpriteBatch();
//#endif

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
