namespace CloudberryKingdom
{
    class VideoWrapper
    {
		public enum VideoPlayerType { Xna, DirectShow, None };

		#if MONO
		//public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.Xna;
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.None;
		#elif SDL2
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.Xna;
		#else
		public static VideoPlayerType CurrentVideoPlayerType = VideoPlayerType.DirectShow;
		#endif

		public static bool IsPlaying
		{
			get
			{
				switch (CurrentVideoPlayerType)
				{
					case VideoPlayerType.Xna: return XnaVideo.IsPlaying;
#if !SDL2
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
				case VideoPlayerType.Xna: XnaVideo.Pause(); break;
#if !SDL2
				case VideoPlayerType.DirectShow: DirectShowVideo.Pause(); break;
#endif
			}
		}

		public static void Resume()
		{
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.Resume(); break;
#if !SDL2
				case VideoPlayerType.DirectShow: DirectShowVideo.Resume(); break;
#endif
			}
		}

        public static void StartVideo_CanSkipIfWatched(string MovieName)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched(MovieName); break;
#if !SDL2
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo_CanSkipIfWatched(MovieName);
					#if !MONO
					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo_CanSkipIfWatched(MovieName);
					}
					#endif
					break;
#endif
			}
        }

        public static void StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(string MovieName, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip); break;
#if !SDL2
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip);
					#if !MONO
					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(MovieName, LengthUntilCanSkip);
					}
					#endif
					break;
#endif
			}
        }

        public static void StartVideo(string MovieName, bool CanSkipVideo, float LengthUntilCanSkip)
        {
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: XnaVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip); break;
#if !SDL2
				case VideoPlayerType.DirectShow:
					DirectShowVideo.StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip);
					#if !MONO
					if (SeeSharp.Xna.Video.VideoPlayer.Broken)
					{
						CurrentVideoPlayerType = VideoPlayerType.Xna;
						StartVideo(MovieName, CanSkipVideo, LengthUntilCanSkip);
					}
					#endif
					break;
#endif
			}
        }

		public static bool Draw()
		{
			switch (CurrentVideoPlayerType)
			{
				case VideoPlayerType.Xna: return XnaVideo.Draw();
#if !SDL2
				case VideoPlayerType.DirectShow: return DirectShowVideo.Draw();
#endif

				default: return false;
			}

		}
    }
}
