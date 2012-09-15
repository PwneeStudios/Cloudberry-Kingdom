using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class LevelMenu : DifficultyMenu
    {
        protected override void Launch(int index, int menuindex)
        {
            int Level = Levels[index];
            base.Launch(Level - 1, menuindex);
        }

        int[] Levels = { 1, 20, 40, 60 };
        string[] LevelStr = { "Normal", "Advanced", "Expert", "Master" };
        public override string[] GetNames()
        {
            string[] names = new string[Levels.Length];

            if (UseString)
                return LevelStr;

            for (int i = 0; i < Levels.Length; i++)
                //names[i] = string.Format("Level {0}", Levels[i]);
                names[i] = string.Format("{0:00}", Levels[i]);

            return names;
        }

        bool UseString;
        public LevelMenu(bool UseString, int HighestLevel, int[] Levels)
        {
            this.UseString = UseString;
            this.Levels = Levels;

            //HeaderText = "Start level:";
            HeaderText = "Level";
            FontScale = .85f;
            if (UseString) ItemFontScaleMod = .68f / .85f;

            // Allow user to choose amongst any start level in the array Levels,
            // assuming they have previously gotten to that level.
            IndexCutoff = 1;// int.MaxValue;
            for (int i = 0; i < Levels.Length; i++)
                if (HighestLevel >= Levels[i] || CloudberryKingdomGame.UnlockAll) IndexCutoff = i + 1;

            Initialize();

            MyMenu.Pos = new Vector2(176.2715f, -43.80966f);
            Header.Pos = new Vector2(-1168.333f, 864.2062f);
            Backdrop.Pos = new Vector2(-37.6936f, 24.60343f);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);
            text.Scale *= 1.1f;
        }
    }
}