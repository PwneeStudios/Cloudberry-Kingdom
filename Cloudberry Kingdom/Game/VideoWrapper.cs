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

namespace CloudberryKingdom
{
    class VideoWrapper
    {
		public enum VideoPlayerType { Xna, DirectShow };
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.DirectShow;

		public static bool IsPlaying
		{
			get
			{
				switch (CurrentVideoPlayerType)
				{
					case VideoPlayerType.Xna: return XnaVideo.IsPlaying;
					case VideoPlayerType.DirectShow: return DirectShowVideo.IsPlaying;
					default: return false;
				}
			}
		}

		public static void Pause()
		{
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.Pause(); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.Pause(); break;
			}
		}

		public static void Resume()
		{
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.Resume(); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.Resume(); break;
			}
		}

        public static void StartVideo_CanSkipIfWatched(string MovieName)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched(MovieName); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.StartVideo_CanSkipIfWatched(MovieName); break;
			}
        }

        public static void StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(string MovieName, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip); break;
			}
        }

        public static void StartVideo(string MovieName, bool CanSkipVideo, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip); break;
			}
        }

		public static bool Draw()
		{
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: return XnaVideo.Draw();
				case VideoPlayerType.DirectShow: return DirectShowVideo.Draw();
				default: return false;
			}

		}
    }
}
