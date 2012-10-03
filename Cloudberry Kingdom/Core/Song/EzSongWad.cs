using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using CloudberryKingdom;

namespace CoreEngine
{
    public class EzSongWad
    {
        public bool PlayNext = false, PlayerControl, DisplayInfo;
        public bool Fading;
        public float Fade;

        public List<EzSong> SongList;
        public List<EzSong> PlayList;

        public int CurIndex;

        public bool StartingSong;

        public EzText SongInfoText;
        public bool DisplayingInfo;
        Camera DefaultCam;

        public EzSongWad()
        {
            SongList = new List<EzSong>();

            CurIndex = 0;

            StartingSong = false;

            DefaultCam = new Camera();

            MediaPlayer.IsRepeating = false;
        }
        
        public void FadeOut()
        {
            Fading = true;
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            PlayNext = false;
        }

        public bool Paused = false;
        public void Pause()
        {
            Paused = true;
            MediaPlayer.Pause();
        }

        public void Unpause()
        {
            Paused = false;
            MediaPlayer.Resume();
        }

        public void DisplaySongInfo(EzSong song)
        {
            if (!DisplayInfo) return;
            if (!song.DisplayInfo) return;

            SongInfoText = new EzText(song.SongName + "\n" + song.ArtistName, Tools.LilFont, true, true);
            SongInfoText._Pos = new Vector2(-850, -790);
            SongInfoText.MyFloatColor = new Vector4(1, 1, 1, 4.5f);
            SongInfoText.Alpha = -.45f;
            SongInfoText.FixedToCamera = true;

            DisplayingInfo = true;
        }

        public void Draw()
        {
            if (CloudberryKingdomGame.ShowSongInfo && DisplayInfo && DisplayingInfo && SongInfoText != null && !Tools.ShowLoadingScreen
                && !Tools.ScreenshotMode && !Tools.CapturingVideo)
            {
                SongInfoText.Draw(DefaultCam);
            }
        }

        public void PhsxStep()
        {
            if (Paused && MediaPlayer.State != MediaState.Paused)
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
                    MediaPlayer.Stop();
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
                if (MediaPlayer.State == MediaState.Playing)
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

                if (CanControl && ButtonCheck.State(ControllerButtons.RT, -1).Pressed)
                    Next();
                else if (CanControl && ButtonCheck.State(ControllerButtons.LT, -1).Pressed)
                    Prev();
                
                // Switch to the next song if the current song is over
                else if (PlayNext && Elapsed > Duration)
                    Next();
            }
        }

        public bool IsPlaying()
        {
            if (MediaPlayer.State == MediaState.Playing)
                return true;
            else
                return false;
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

        public void DisposeAllUnused()
        {
            foreach (EzSong song in SongList)
                if (song.song != null && SongList[CurIndex] != song && !SongList[CurIndex].AlwaysLoaded)
                {
                    song.song.Dispose();
                    song.song = null;
                }
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
        public void SetPlayList(List<EzSong> songs)
        {
            PlayList = new List<EzSong>(songs);
        }
        public void SetPlayList(string name)
        {
            SetPlayList(FindByName(name));
        }

        /// <summary>
        /// Set the play list to a single song and start playing it.
        /// </summary>
        public void SetPlayList(EzSong song)
        {
            List<EzSong> list = new List<EzSong>();
            list.Add(song);

            SetPlayList(list);
            CurIndex = 0;
        }

        /// <summary>
        /// The play list currently playing
        /// (possibly different than the set play list, if it hasn't been started yet).
        /// </summary>
        List<EzSong> CurrentPlayingList = null;

        bool SamePlayList(List<EzSong> list1, List<EzSong> list2)
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

        public void SetSong(EzSong song)
        {
            SetSong(PlayList.IndexOf(song));
        }

        public bool SuppressNextInfoDisplay = false;
        public void SetSong(int Index) { SetSong(Index, true); }
        public void SetSong(int Index, bool DisplayInfo)
        {
            CurIndex = Index;
            StartingSong = true;
            Fade = 1;
            Fading = false;

            MediaPlayer.Stop();

            // Suppress next info display
            if (DisplayInfo && SuppressNextInfoDisplay)
                DisplayInfo = SuppressNextInfoDisplay = false;

            Play(Index, DisplayInfo);
        }

        public void Play(int Index, bool DisplayInfo)
        {
            if (PlayList[CurIndex].song == null)
                PlayList[CurIndex].song = Tools.GameClass.Content.Load<Song>("Music\\" + PlayList[CurIndex].FileName);

            Elapsed = 0;
            Duration = PlayList[CurIndex].Play(DisplayInfo);
        }

        public EzSong FindByName(string name)
        {
            foreach (EzSong Sng in SongList)
                if (String.Compare(Sng.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Sng;

            return SongList[0];
        }

        public void AddSong(string Name)
        {
            EzSong NewSong = new EzSong();

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

            NewSong.FileName = Name;
            NewSong.Name = Name;
            NewSong.SongName = SongName;
            NewSong.ArtistName = AristName;
            NewSong.song = null;
            NewSong.AlwaysLoaded = false;

            SongList.Add(NewSong);
        }

        public void AddSong(Song song, string Name)
        {
            EzSong NewSong = new EzSong();

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

            NewSong.Name = Name;
            NewSong.SongName = SongName;
            NewSong.ArtistName = AristName;
            NewSong.song = song;
            NewSong.AlwaysLoaded = true;

            SongList.Add(NewSong);
        }

        public void LoopSong(EzSong song)
        {
            Stop();
            SetPlayList(song);
            Start(true);
        }
    }
}