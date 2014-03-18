using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XnaMedia = Microsoft.Xna.Framework.Media;
using CloudberryKingdom;

namespace CoreEngine
{
	public abstract class MediaPlayer
	{
		//public static MediaPlayer Instance = new EmptyMediaPlayer();
		public static MediaPlayer Instance = new XnaMediaPlayer();

		public static XnaMedia.MediaState State = XnaMedia.MediaState.Stopped;

		public static BaseSong CurrentSong;

		public abstract void Stop ();
		public abstract bool IsRepeating { get; set; }
		public abstract float Volume { get; set; }
		public abstract void Pause ();
		public abstract void Resume ();

		public abstract BaseSong NewSong();
	}

	public class XnaMediaPlayer : MediaPlayer
	{
		public override BaseSong NewSong ()
		{
			return new XnaSong();
		}

		public override void Stop()
		{
			XnaMedia.MediaPlayer.Stop ();
		}

		public override bool IsRepeating
		{
			get {
				return XnaMedia.MediaPlayer.IsRepeating;
			}

			set {
				XnaMedia.MediaPlayer.IsRepeating = value;
			}
		}

		public override float Volume {
			get {
				return XnaMedia.MediaPlayer.Volume;
			}
			set {
				XnaMedia.MediaPlayer.Volume = value;
			}
		}

		public override void Pause ()
		{
			XnaMedia.MediaPlayer.Pause ();
		}

		public override void Resume ()
		{
			XnaMedia.MediaPlayer.Resume ();
		}
	}

	public class EmptyMediaPlayer : MediaPlayer
	{
		class EmptySong : BaseSong
		{
			protected override void LoadSong (string name)
			{
				base.LoadSong (name);
			}

			public override double Play (bool DisplayInfo)
			{
				return base.Play (DisplayInfo);
			}
		}

		public override BaseSong NewSong ()
		{
			return new EmptySong ();
		}

		public override void Stop()
		{

		}

		public override bool IsRepeating
		{
			get {
				return false;
			}

			set {

			}
		}

		public override float Volume {
			get {
				return 0;
			}
			set {

			}
		}

		public override void Pause ()
		{

		}

		public override void Resume ()
		{

		}
	}

    public class EzSongWad
    {
        public bool PlayNext = false, PlayerControl, DisplayInfo;
        public bool Fading;
        public float Fade;

        public List<BaseSong> SongList;
        public List<BaseSong> PlayList;

        public int CurIndex;

        public bool StartingSong;

        public EzText SongInfoText;
        public bool DisplayingInfo;
        public Camera DefaultCam;

        public EzSongWad()
        {
            SongList = new List<BaseSong>();

            CurIndex = 0;

            StartingSong = false;

            DefaultCam = new Camera();

			MediaPlayer.Instance.IsRepeating = false;
        }
        
        public void FadeOut()
        {
            Fading = true;
        }

        public void Stop()
        {
			try
			{
				MediaPlayer.Instance.Stop();
			}
			catch
			{
			}

            PlayNext = false;
        }

        public bool Paused = false;
        public void Pause()
        {
            Paused = true;

			try
			{
				MediaPlayer.Instance.Pause();
			}
			catch
			{
			}
        }

        public void Unpause()
        {
            Paused = false;
            
			try
			{
				MediaPlayer.Instance.Resume();
			}
			catch
			{
			}
        }

        public void DisplaySongInfo(BaseSong song)
        {
            if (!DisplayInfo) return;
            if (!song.DisplayInfo) return;

            string songname = song.SongName;
            songname = songname.Replace('_', ' ');
            string artistname = song.ArtistName;
            artistname = song.ArtistName.Replace('_', ' ');
            //SongInfoText = new EzText(songname + "\n" + artistname, Resources.LilFont, true, true);
            SongInfoText = new EzText(songname + "\n" + artistname, Resources.Font_Grobold42_2, 1000.0f, true, true, .85f);
            SongInfoText._Pos = new Vector2(-850, -790);
            SongInfoText.MyFloatColor = new Vector4(.9f, .9f, .9f, 4.5f);
            SongInfoText.Scale = .3f;
            SongInfoText.Alpha = -.45f;
            SongInfoText.FixedToCamera = true;

            DisplayingInfo = true;
        }

        public void Draw()
        {
            if (CloudberryKingdomGame.ShowSongInfo && DisplayInfo && DisplayingInfo && SongInfoText != null && !Tools.ShowLoadingScreen &&
                Tools.CurGameData != null && !Tools.CurGameData.PauseGame && !Tools.CurGameData.SuppressSongInfo)
            {
                SongInfoText.Draw(DefaultCam);
            }
        }

        public void PhsxStep()
        {
			if (Paused && MediaPlayer.State != XnaMedia.MediaState.Paused)
                Pause();

            FadePhsx();

            DisplayPhsx();

            CheckForNext();
        }

        void FadePhsx()
        {
            if (Fading)
            {
                Fade -= .015f;
                if (Fade <= 0)
                {
                    PlayNext = false;
					
					try
					{
						MediaPlayer.Instance.Stop();
					}
					catch
					{
					}
                    
					Fading = false;
                }
            }

            Tools.VolumeFade = Fade;
        }

        void DisplayPhsx()
        {
            if (DisplayingInfo && SongInfoText != null)
            {
                SongInfoText.MyFloatColor.W -= .02f;
                SongInfoText.OutlineColor.W = SongInfoText.MyFloatColor.W;
                SongInfoText.OutlineColor = new Vector4(0, 0, 0, SongInfoText.OutlineColor.W);
                if (SongInfoText.MyFloatColor.W < 1)
                    SongInfoText.MyFloatColor = new Vector4(.9f * SongInfoText.MyFloatColor.W);

                if (SongInfoText.Alpha < 1) SongInfoText.Alpha += .03f;
                if (SongInfoText.MyFloatColor.W <= 0)
                    DisplayingInfo = false;
            }
        }

        public void UpdateElapsedTime()
        {
            Elapsed += Tools.TheGame.DeltaT;
        }

        double Duration = 0, Elapsed = 0;
        void CheckForNext()
        {
            if (StartingSong)
            {
				if (MediaPlayer.State == XnaMedia.MediaState.Playing)
                    StartingSong = false;
                else
                    return;
            }

            UpdateElapsedTime();
            if (!Fading)
            {
                // Whether the player can change the current song
                bool CanControl = PlayerControl;
                if (CanControl)
                    if (PlayList == null || PlayList.Count <= 1)
                        CanControl = false;

				// With player song switching enabled
				//if (CanControl && ButtonCheck.State(ControllerButtons.RT, -1).Pressed)
				//    Next();
				//else if (CanControl && ButtonCheck.State(ControllerButtons.LT, -1).Pressed)
				//    Prev();
                
				//// Switch to the next song if the current song is over
				//else if (PlayNext && Elapsed > Duration)
				//    Next();

				// WITHOUT player song switching enabled
				// Switch to the next song if the current song is over
				if (PlayNext && Elapsed > Duration)
					Next();
            }
        }

        public bool IsPlaying()
        {
			if (MediaPlayer.State == XnaMedia.MediaState.Playing)
                return true;
            else
                return false;
        }

        public void Next(BaseSong song)
        {
			bool HoldSuppress = SuppressNextInfoDisplay;

			//Start(true);

			SuppressNextInfoDisplay = HoldSuppress;

            CurIndex = PlayList.IndexOf(song);
            if (CurIndex < 0) CurIndex = 0;

            SetSong(CurIndex);
        }

        public void Next()
        {
            CurIndex++;

            if (CurIndex < 0) CurIndex = PlayList.Count - 1;
            if (CurIndex >= PlayList.Count) CurIndex = 0;

            SetSong(CurIndex);
        }

        public void Prev()
        {
            CurIndex--;

            if (CurIndex < 0) CurIndex = PlayList.Count - 1;
            if (CurIndex >= PlayList.Count) CurIndex = 0;

            SetSong(CurIndex);
        }

        /// <summary>
        /// Shuffles the current play list
        /// </summary>
        public void Shuffle()
        {
            PlayList = Tools.GlobalRnd.Shuffle(PlayList);
        }

        /// <summary>
        /// Set the play list and start playing it.
        /// </summary>
        public void SetPlayList(List<BaseSong> songs)
        {
            PlayList = new List<BaseSong>(songs);
        }
        public void SetPlayList(string name)
        {
            SetPlayList(FindByName(name));
        }

        /// <summary>
        /// Set the play list to a single song and start playing it.
        /// </summary>
        public void SetPlayList(BaseSong song)
        {
            List<BaseSong> list = new List<BaseSong>();
            list.Add(song);

            SetPlayList(list);
            CurIndex = 0;
        }

        /// <summary>
        /// The play list currently playing
        /// (possibly different than the set play list, if it hasn't been started yet).
        /// </summary>
        List<BaseSong> CurrentPlayingList = null;

        bool SamePlayList(List<BaseSong> list1, List<BaseSong> list2)
        {
            if (list1 == list2) return true;

            if (list1 == null || list2 == null) return false;

            if (list1.Count == 1 && list2.Count == 1 && list1[0] == list2[0])
                return true;

            return false;
        }

        /// <summary>
        /// Starts the play list if it ISN'T already playing
        /// </summary>
        public void Start(bool PlayNext)
        {
            Unpause();

            this.PlayNext = PlayNext;

            if (!SamePlayList(CurrentPlayingList, PlayList))
            {
                CurrentPlayingList = PlayList;
                CurIndex = 0;
                Restart(PlayNext);
                return;
            }

            if (!IsPlaying())
            {
                Restart(PlayNext);
                return;
            }
        }

        /// <summary>
        /// Starts or restarts the current song, eliminating all fading.
        /// </summary>
        public void Restart(bool PlayNext) { Restart(PlayNext, true); }
        public void Restart(bool PlayNext, bool DisplayInfo)
        {
            this.PlayNext = PlayNext;
            CurrentPlayingList = PlayList;
            SetSong(CurIndex, DisplayInfo);
            Fading = false;
            Fade = Tools.VolumeFade = 1f;
        }

        public void SetSong(string name)
        {
            SetSong(PlayList.IndexOf(FindByName(name)));
        }

        public void SetSong(BaseSong song)
        {
            SetSong(PlayList.IndexOf(song));
        }

		public bool _SuppressNextInfoDisplay = false;
		public bool SuppressNextInfoDisplay
		{
			get { return _SuppressNextInfoDisplay; }
			set
			{
				_SuppressNextInfoDisplay = value;
			}
		}

        public void SetSong(int Index) { SetSong(Index, true); }
        public void SetSong(int Index, bool DisplayInfo)
        {
            CurIndex = Index;
            StartingSong = true;
            Fade = 1;
            Fading = false;

			try
			{
				MediaPlayer.Instance.Stop();
			}
			catch
			{
			}

            // Suppress next info display
            if (DisplayInfo && SuppressNextInfoDisplay)
                DisplayInfo = SuppressNextInfoDisplay = false;

            Play(Index, DisplayInfo);
        }

        public void Play(int Index, bool DisplayInfo)
        {
            PlayList[CurIndex].LoadSong_IfNotLoaded(PlayList[CurIndex].FileName);

            Elapsed = 0;
            Duration = PlayList[CurIndex].Play(DisplayInfo);
        }

        public BaseSong FindByName(string name)
        {
            foreach (BaseSong Sng in SongList)
                if (String.Compare(Sng.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Sng;

#if DEBUG
            Tools.Break();
#endif

            return SongList[0];
        }

        public void AddSong(string Name)
        {
			BaseSong NewSong = MediaPlayer.Instance.NewSong ();

            int Index = Name.IndexOf("^");

            String SongName, AristName;
            if (Index > 0)
            {
                SongName = Name.Substring(0, Index);
                AristName = Name.Substring(Index + 1, Name.Length - Index - 1);
            }
            else
            {
                SongName = Name;
                AristName = "Unknown artist";
            }

			SongName = SongName.Replace ('$', '\'');

            NewSong.FileName = Name;
            NewSong.Name = Name;
            NewSong.SongName = SongName;
            NewSong.ArtistName = AristName;
            NewSong.AlwaysLoaded = false;

            NewSong.LoadSong_IfNotLoaded(NewSong.FileName);

            SongList.Add(NewSong);
        }

        public void LoopSong(BaseSong song)
        {
            Stop();
            SetPlayList(song);
            Start(true);
        }
    }
}