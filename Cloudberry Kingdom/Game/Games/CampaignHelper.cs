using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;

using CoreEngine;

namespace CloudberryKingdom
{
    public class CampaignHelper
    {
        public static string GetName(int difficulty)
        {
            return EzText.ColorToMarkup(DifficultyColor[difficulty]) +
                Localization.WordString(DifficultyNames[difficulty]).ToLower(CultureInfo.InvariantCulture) + 
                EzText.ColorToMarkup(Color.White);
        }
        public static Color[] DifficultyColor = { new Color(44, 44, 44), new Color(144, 200, 225), new Color(44, 203, 48), new Color(248, 136, 8), new Color(90, 90, 90), new Color(0, 255, 255) };
        public static Localization.Words[] DifficultyNames = { Localization.Words.Custom, Localization.Words.Training, Localization.Words.Unpleasant, Localization.Words.Abusive, Localization.Words.Hardcore, Localization.Words.Masochistic };
    }
}