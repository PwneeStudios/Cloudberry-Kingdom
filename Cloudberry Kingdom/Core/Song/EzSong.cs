using System;
using XnaMedia = Microsoft.Xna.Framework.Media;
using CloudberryKingdom;

namespace CoreEngine
{
    public class XnaSong : BaseSong
	{
		public XnaMedia.Song song;

        public override double Play(bool DisplayInfo)
        {
            base.Play(DisplayInfo);

            try
            {
				MediaPlayer.Instance.Stop();
				XnaMedia.MediaPlayer.Play(song);
            }
            catch (Exception e)
            {
                Tools.SongWad.Stop();
                return 1000000;
            }

            return song.Duration.TotalSeconds;
        }

        protected override void LoadSong(string name)
        {
			song = Tools.GameClass.Content.LoadTillSuccess<XnaMedia.Song>("Music\\" + name);
			//song = Tools.GameClass.Content.LoadTillSuccess<XnaMedia.Song>("Music\\" + "Blue_Chair^Blind_Digital.ogg");
        }
	}

    public abstract class BaseSong
    {
        public string Name, SongName, ArtistName, FileName;
        public bool Enabled, AlwaysLoaded;

        public float Volume;

        public bool DisplayInfo = true;

        public BaseSong()
        {
            Volume = 1f;
            Enabled = true;
        }

        public virtual double Play(bool DisplayInfo)
        {
            DisplayInfo = Play_SetPlayerParams(DisplayInfo);

            return 0;
        }

        private bool Play_SetPlayerParams(bool DisplayInfo)
        {
            Tools.CurSongVolume = Volume;

            if (Tools.SongWad.SuppressNextInfoDisplay)
                Tools.SongWad.SuppressNextInfoDisplay = DisplayInfo = false;

            if (DisplayInfo && !CloudberryKingdomGame.CustomMusicPlaying)
                Tools.SongWad.DisplaySongInfo(this);

            return DisplayInfo;
        }

        public bool Loaded = false;
        public virtual void LoadSong_IfNotLoaded(string name)
        {
            if (!Loaded)
            {
                LoadSong(name);
            }

            Loaded = true;
        }

        protected virtual void LoadSong(string name)
        {
        }
    }
}