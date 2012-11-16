using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;


namespace CloudberryKingdom
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

            MediaPlayer.Stop();
            MediaPlayer.Play(song);

            if (DisplayInfo)
                Tools.SongWad.DisplaySongInfo(this);

            return song.Duration.TotalSeconds;
        }
    }
}