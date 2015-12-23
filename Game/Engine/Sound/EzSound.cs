using Microsoft.Xna.Framework.Audio;
using CloudberryKingdom;

namespace CoreEngine
{
    public class CoreSound
    {
        public SoundEffect sound;
        public string Name;
        public int MaxInstances;
        public float DefaultVolume;
        public int DelayTillNextSoundCanPlay;
        int LastPlayedStamp;

        //public List<SoundEffectInstance> Instances;

        public CoreSound()
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
            if (CoreSoundWad.SuppressSounds) return;

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
            if (CoreSoundWad.SuppressSounds) return;

            Play(1, Tools.GlobalRnd.RndFloat(-PitchModulationRange, PitchModulationRange), 0);
        }

        public void Play(float volume)
        {
            if (CoreSoundWad.SuppressSounds) return;
            //CutOffExtra();

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, 0, 0);
            //if (Instances.Count < MaxInstances)
            //  Instances.Add(sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume));
        }

        public void Play(float volume, float pitch, float pan)
        {
            if (CoreSoundWad.SuppressSounds) return;
            //CutOffExtra();

            sound.Play(volume * Tools.SoundVolume.Val * DefaultVolume, CoreMath.Restrict(-1, 1, pitch), CoreMath.Restrict(-1, 1, pan));
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
}