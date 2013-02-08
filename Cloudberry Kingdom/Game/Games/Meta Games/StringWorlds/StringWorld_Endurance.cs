using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class StringWorldEndurance : StringWorldGameData
    {
        GUI_Score MyGUI_Score;
        GUI_Level MyGUI_Level;

        GUI_LivesLeft Gui_LivesLeft;
        GUI_Lives Gui_Lives;
        GUI_NextLife Gui_NextLife;
        CoinScoreMultiplierObject MyCoinScoreMultiplier;

        public StringWorldEndurance(Func<int, LevelSeedData> GetSeed, GUI_LivesLeft Gui_LivesLeft, int NextLife)
            : base(GetSeed)
        {
            // Lives
            this.Gui_LivesLeft = Gui_LivesLeft;
            Gui_Lives = new GUI_Lives(Gui_LivesLeft);
            Gui_NextLife = new GUI_NextLife(NextLife, Gui_LivesLeft);

            // Coin score multiplier
            MyCoinScoreMultiplier = new CoinScoreMultiplierObject();

            // Level and Score
            MyGUI_Score = new GUI_Score(false);
            MyGUI_Level = new GUI_Level(false);

            // Add game objects, including 'Perfect' watcher
            OnSwapToFirstLevel += (data) =>
            {
                data.MyGame.AddGameObject(Gui_LivesLeft, Gui_NextLife, Gui_Lives,
                    MyCoinScoreMultiplier, new PerfectScoreObject(false, true, false)
                    , MyGUI_Score
                    //, MyGUI_Level
                    );
            };
        }

        public override void Release()
        {
            base.Release();

            Gui_LivesLeft.ForceRelease();
            Gui_NextLife.ForceRelease();
        }

        public override void AdditionalSwapToLevelProcessing(GameData game)
        {
            base.AdditionalSwapToLevelProcessing(game);

            //game.TakeOnce = true;

            // Should we give the player points for coins even if they die?
            //game.AlwaysGiveCoinScore = true;
            game.AlwaysGiveCoinScore = false;
        }
    }
}