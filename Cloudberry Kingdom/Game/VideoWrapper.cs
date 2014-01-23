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
		public enum VideoPlayerType { Xna, DirectShow, None };

		#if MONO
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.None;
		#else
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.DirectShow;
		#endif

		public static bool IsPlaying
		{
			get
			{
				switch (CurrentVideoPlayerType)
				{
					#if !MONO
					case VideoPlayerType.Xna: return XnaVideo.IsPlaying;
					case VideoPlayerType.DirectShow: return DirectShowVideo.IsPlaying;
					#endif
					default: return false;
				}
			}
		}

		public static void Pause()
		{
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: XnaVideo.Pause(); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.Pause(); break;
				#endif
			}
		}

		public static void Resume()
		{
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: XnaVideo.Resume(); break;
				case VideoPlayerType.DirectShow: DirectShowVideo.Resume(); break;
				#endif
			}
		}

        public static void StartVideo_CanSkipIfWatched(string MovieName)
        {
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched(MovieName); break;
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo_CanSkipIfWatched(MovieName);
					
					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo_CanSkipIfWatched(MovieName);
					}

					break;
				#endif
			}
        }

        public static void StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(string MovieName, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip); break;
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip);
					
					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip);
					}
					
					break;
				#endif
			}
        }

        public static void StartVideo(string MovieName, bool CanSkipVideo, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: XnaVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip); break;
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip);

					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip);
					}

					break;
				#endif
			}
        }

		public static bool Draw()
		{
			switch (CurrentVideoPlayerType)
			{
				#if !MONO
				case VideoPlayerType.Xna: return XnaVideo.Draw();
				case VideoPlayerType.DirectShow: return DirectShowVideo.Draw();
				#endif
				default: return false;
			}

		}
    }
}
