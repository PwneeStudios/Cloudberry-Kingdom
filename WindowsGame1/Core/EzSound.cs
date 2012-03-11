using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using CloudberryKingdom;

namespace Drawing
{
    public class EzSound
    {
        public SoundEffect sound;
        public string Name;
        public int MaxInstances;
        public float DefaultVolume;
        public int DelayTillNextSoundCanPlay;
        int LastPlayedStamp;

        //public List<SoundEffectInstance> Instances;

        public EzSound()
        {
            DelayTillNextSoundCanPlay = 1;

            DefaultVolume = 1f;
            //  Instances = new List<SoundEffectInstance>();
        }

        /*  void CutOffExtra()
          {
              if (Instances.Count == MaxInstances)
              {
                  Instances[0].Dispose();
                  Instances.Remove(Instances[0]);
              }
          }
          */
        public void Play()
        {
            if (EzSoundWad.SuppressSounds) return;

            if (Tools.DrawCount - LastPlayedStamp <= DelayTillNextSoundCanPlay)
                return;

            //CutOffExtra();

            sound.Play(Tools.SoundVolume.Val * DefaultVolume, 0, 0);
            //if (Instances.Count < MaxInstances)
            //  Instances.Add(sound.Play(Tools.SoundVolume.Val * DefaultVolume));

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
            //CutOffExtra();

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, 0, 0);
            //if (Instances.Count < MaxInstances)
            //  Instances.Add(sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume));
        }

        public void Play(float volume, float pitch, float pan)
        {
            if (EzSoundWad.SuppressSounds) return;
            //CutOffExtra();

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, pitch, pan);
            //if (Instances.Count == MaxInstances)
            //{
            //  Instances[0].Dispose();
            //Instances.Remove(Instances[0]);
            //}

            //if (Instances.Count < MaxInstances)
            //  Instances.Add(sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, pitch, pan, loop));
        }

        /*
        public void Update()
        {
            foreach (SoundEffectInstance instance in Instances)
                if (instance.State == SoundState.Stopped)
                    instance.Dispose();
            Instances.RemoveAll(delegate(SoundEffectInstance instance) { return instance.IsDisposed; });
        }*/
    }

    public class EzSoundWad
    {
        /// <summary>
        /// When true all new sounds to be played are suppressed.
        /// </summary>
        public static bool SuppressSounds = false;

        public List<EzSound> SoundList;
        public int MaxInstancesPerSound;

        public EzSoundWad(int MaxInstancesPerSound)
        {
            this.MaxInstancesPerSound = MaxInstancesPerSound;

            SoundList = new List<EzSound>();
        }

        public void Update()
        {
            //   foreach (EzSound sound in SoundList)
            //     sound.Update();
        }

        public EzSound FindByName(string name)
        {
            foreach (EzSound Snd in SoundList)
                if (String.Compare(Snd.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    //if (Snd.Name.CompareTo(name) == 0)
                    return Snd;
            return SoundList[0];
        }

        public void AddSound(SoundEffect sound, string Name)
        {
            EzSound NewSound = new EzSound();
            NewSound.Name = Name;
            NewSound.sound = sound;
            NewSound.MaxInstances = MaxInstancesPerSound;

            SoundList.Add(NewSound);
        }
    }
}