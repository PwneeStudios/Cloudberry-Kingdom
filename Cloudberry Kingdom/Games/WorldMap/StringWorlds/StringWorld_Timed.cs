﻿using System.Collections.Generic;

using CloudberryKingdom.Levels;

using System.Threading;

namespace CloudberryKingdom
{
    public class StringWorldTimed : StringWorldGameData
    {
        GUI_Timer MyGUI_Timer;
        TimerWarning Warning;
        public GUI_Score MyGUI_Score;
        public GUI_Level MyGUI_Level;
        CoinScoreMultiplierObject MyCoinScoreMultiplier;

        public StringWorldTimed(List<LevelSeedData> LevelSeeds, GUI_Timer Timer)
        {
            this.LevelSeeds = LevelSeeds;

            MyGUI_Timer = Timer;

            Warning = new TimerWarning();
            Warning.MyTimer = Timer;

            Timer.OnTimeExpired += (timer) => MyGUI_Score.SlideOut(GUI_Panel.PresetPos.Top);
            Timer.OnTimeExpired += (timer) => MyGUI_Level.SlideOut(GUI_Panel.PresetPos.Top);

            MyGUI_Score = new GUI_Score(1);//0);
            MyGUI_Level = new GUI_Level();

            // Coin score multiplier
            MyCoinScoreMultiplier = new CoinScoreMultiplierObject();

            // Add 'Perfect' watcher
            OnSwapToFirstLevel += (data) =>
                {
                    data.MyGame.AddGameObject(MyGUI_Timer, Warning, MyGUI_Score, MyGUI_Level,
                        MyCoinScoreMultiplier, new PerfectScoreObject(false));
                };
        }

        public override void Release()
        {
            base.Release();

            MyGUI_Timer.ForceRelease();
        }

        public override void AdditionalSwapToLevelProcessing(GameData game)
        {
            base.AdditionalSwapToLevelProcessing(game);

            MyGUI_Level.SetLevel(CurLevelIndex + 1);

            game.TakeOnce = true;
        }
    }
}