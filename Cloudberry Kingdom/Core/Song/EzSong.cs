using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using CloudberryKingdom;

namespace CoreEngine
{
    public class EzSong
    {
        public Song song;
        public string Name, SongName, ArtistName, FileName;
        public bool Enabled, AlwaysLoaded;

        public float Volume;

        public bool DisplayInfo = true;

        public EzSong()
        {
            Volume = 1f;
            Enabled = true;
        }

        public double Play(bool DisplayInfo)
        {
            Tools.CurSongVolume = Volume;

			try
			{
				MediaPlayer.Stop();
				MediaPlayer.Play(song);
			}
			catch
			{
			}

			if (Tools.SongWad.SuppressNextInfoDisplay)
				Tools.SongWad.SuppressNextInfoDisplay = DisplayInfo = false;

			if (DisplayInfo && !CloudberryKingdomGame.CustomMusicPlaying)
                Tools.SongWad.DisplaySongInfo(this);

            return song.Duration.TotalSeconds;
        }
    }
}