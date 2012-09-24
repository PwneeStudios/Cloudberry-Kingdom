using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class LockManager : SaveLoad
    {
        static readonly LockManager instance = new LockManager();
        public static LockManager Instance { get { return instance; } }

        const int version = 1;

        /// <summary>
        /// The highest campaign difficulty that is unlocked.
        /// </summary>
        public static int CampaignLock = 2;

        LockManager()
        {

        }

        /// <summary>
        /// Saves the information that the player has beaten the campaign at a certain difficulty level.
        /// </summary>
        /// <param name="Difficulty"></param>
        public static void RegisterCampaignBeaten(int Difficulty)
        {
            CampaignLock = Math.Max(CampaignLock, Difficulty + 1);

            instance.Changed = true;
        }

        protected override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(version);
            writer.Write(CampaignLock);
        }

        protected override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            int fileversion = reader.ReadInt32();
            if (fileversion != version) return;

            CampaignLock = reader.ReadInt32();
        }
    }
}