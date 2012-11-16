using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace CloudberryKingdom
{
    public class EzSound
    {
        public SoundEffect sound;
        public string Name;
        public int MaxInstances;
        public float DefaultVolume;
        public int DelayTillNextSoundCanPlay;
        int LastPlayedStamp;

        public EzSound()
        {
            DelayTillNextSoundCanPlay = 1;

            DefaultVolume = 1f;
        }

        public void Play()
        {
            if (EzSoundWad.SuppressSounds) return;

            if (Tools.DrawCount - LastPlayedStamp <= DelayTillNextSoundCanPlay)
                return;

            sound.Play(Tools.SoundVolume.Val * DefaultVolume, 0, 0);

            LastPlayedStamp = Tools.DrawCount;
        }

        /// <summary>
        /// Plays the sound with a random modulation to the pitch.
        /// </summary>
        /// <param name="PitchModulationRange"></param>
        public void PlayModulated(float PitchModulationRange)
        {
            if (EzSoundWad.SuppressSounds) return;

            Play(1, Tools.GlobalRnd.RndFloat(-PitchModulationRange, PitchModulationRange), 0);
        }

        public void Play(float volume)
        {
            if (EzSoundWad.SuppressSounds) return;

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, 0, 0);
        }

        public void Play(float volume, float pitch, float pan)
        {
            if (EzSoundWad.SuppressSounds) return;

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, CoreMath.Restrict(-1, 1, pitch), CoreMath.Restrict(-1, 1, pan));
        }
    }
}