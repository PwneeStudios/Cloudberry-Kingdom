using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CoreEngine;

using DirectShow = SeeSharp.Xna.Video;

namespace CloudberryKingdom
{
    class DirectShowVideo
    {
		public static bool IsPlaying
		{
			get
			{
				return CurrentVideo != null;
			}
		}

		public static void Pause()
		{
			;
		}

		public static void Resume()
		{
			;
		}

        static ContentManager Content = null;

        public static bool Playing = false;

		static DirectShow.VideoPlayer CurrentVideo;
        
        static EzTexture VEZTexture = new EzTexture();

        static double Duration, Elapsed;

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

        public static void StartVideo(string MovieName, bool CanSkipVideo, float LengthUntilCanSkip)
        {
#if DEBUG
            CanSkipVideo = true;
#endif

            if (Localization.CurrentLanguage != null)
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
            else
            {
                Content.Unload();
                Content = new ContentManager(Tools.GameClass.Services, "Content");
            }

            CanSkip = CanSkipVideo;
            LengthUntilUserCanSkip = LengthUntilCanSkip;

            UserPowers.WatchedVideo += MovieName;
            UserPowers.SetToSave();

            Playing = true;
            Cleaned = false;

            CurrentVideo = new DirectShow.VideoPlayer(Path.Combine("Movies", MovieName), Tools.GameClass.GraphicsDevice);

			CurrentVideo.Play();

			float VolumeMod = 1;
			switch (MovieName)
			{
				case "LogoSalad":
					VolumeMod = .8333f;
					break;

				default:
					VolumeMod = 1;
					break;
			}

			CurrentVideo.SetVolume(CoreMath.Restrict(0, 1, VolumeMod * Math.Max(Tools.MusicVolume.Val, Tools.SoundVolume.Val)));
            //VPlayer.Volume = CoreMath.Restrict(0, 1, Math.Max(Tools.MusicVolume.Val, Tools.SoundVolume.Val));

            Elapsed = 0;
            Duration = CurrentVideo.Duration;
        }

        public static void UpdateElapsedTime()
        {
            Elapsed += Tools.TheGame.DeltaT;
        }

        static bool Paused = false;
        static void UserInput()
        {
            ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();

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

		public static void Update()
		{
			if (CurrentVideo != null)
			{
				CurrentVideo.Update();
			}
		}

        public static bool Draw()
        {
            if (!Playing) { Finish(); return false; }

			for (int i = 0; i < 16; i++)
			{
				Tools.GameClass.GraphicsDevice.Textures[i] = null;
			}
			
			Update();

			Tools.EffectWad.SetCameraPosition( new Vector4(0, 0, .001f, .001f) );

            Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);

            UpdateElapsedTime();
            UserInput();

            if (Elapsed > Duration)
            {
                Playing = false;
                return true;
            }

			VEZTexture.Tex = CurrentVideo.GetTexture();
            VEZTexture.Width = VEZTexture.Tex.Width;
            VEZTexture.Height = VEZTexture.Tex.Height;

			Vector2 Pos = Vector2.Zero;
            Tools.QDrawer.DrawToScaleQuad(Pos, Color.White, 3580, VEZTexture, Tools.BasicEffect);
            Tools.QDrawer.Flush();

            Subtitle();

            return true;
        }

        static bool Cleaned = true;
        public static void Finish()
        {
            Playing = false;

            if (Cleaned) return;

			CurrentVideo.Dispose();
            CurrentVideo = null;

            Cleaned = true;
        }
    }
}
