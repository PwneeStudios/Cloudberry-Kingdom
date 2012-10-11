using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CampaignSequence : LevelSequence
    {
        static readonly CampaignSequence instance = new CampaignSequence();
        public static CampaignSequence Instance { get { return instance; } }

        protected CampaignSequence()
        {
        }
    }
}