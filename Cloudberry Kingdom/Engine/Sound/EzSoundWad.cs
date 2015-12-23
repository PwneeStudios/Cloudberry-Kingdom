using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using CloudberryKingdom;

namespace CoreEngine
{
    public class CoreSoundWad
    {
        /// <summary>
        /// When true all new sounds to be played are suppressed.
        /// </summary>
        public static bool SuppressSounds = false;

        public List<CoreSound> SoundList;
        public int MaxInstancesPerSound;

        public CoreSoundWad(int MaxInstancesPerSound)
        {
            this.MaxInstancesPerSound = MaxInstancesPerSound;

            SoundList = new List<CoreSound>();
        }

        public void Update()
        {
            //   foreach (CoreSound sound in SoundList)
            //     sound.Update();
        }

        public CoreSound FindByName(string name)
        {
            foreach (CoreSound Snd in SoundList)
                if (String.Compare(Snd.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Snd;

#if DEBUG
            Tools.Break();
#endif

            return SoundList[0];
        }

        public void AddSound(SoundEffect sound, string Name)
        {
            CoreSound NewSound = new CoreSound();
            NewSound.Name = Name;
            NewSound.sound = sound;
            NewSound.MaxInstances = MaxInstancesPerSound;

            SoundList.Add(NewSound);
        }
    }
}