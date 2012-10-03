using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;
using System.Collections.Generic;
using System.Linq;

using CoreEngine;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class CampaignHelper
    {
        public static string GetName(int difficulty)
        {
            return EzText.ColorToMarkup(DifficultyColor[difficulty]) +
                DifficultyNames[difficulty].ToLower() + 
                EzText.ColorToMarkup(Color.White);
        }
        public static Color[] DifficultyColor = { new Color(44, 44, 44), new Color(144, 200, 225), new Color(44, 203, 48), new Color(248, 136, 8), new Color(90, 90, 90), new Color(0, 255, 255) };
        public static string[] DifficultyNames = { "Custom", "Training", "Unpleasant", "Abusive", "Hardcore", "Masochistic" };
    }
}