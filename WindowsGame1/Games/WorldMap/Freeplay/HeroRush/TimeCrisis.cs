using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_TimeCrisis : Challenge_HeroRush
    {
        static readonly Challenge_TimeCrisis instance = new Challenge_TimeCrisis();
        public static new Challenge_TimeCrisis Instance { get { return instance; } }

        static bool GoalMet = false;
        public override bool GetGoalMet() { return GoalMet; }
        public override void SetGoalMet(bool value) { GoalMet = value; }

        protected Challenge_TimeCrisis()
        {
            ID = new Guid("3b5144f2-285a-4713-7398-0c96b6ebdda1");
            MenuName = "Time Crisis";
            Name = MenuName;
            MenuPic = "menupic_escalation";
            HighScore = SaveGroup.TimeCrisisHighScore;
            HighLevel = SaveGroup.TimeCrisisHighLevel;
            Goal = 35;
        }

        protected override void ShowEndScreen()
        {
            Tools.CurGameData.AddGameObject(new GameOverPanel(HighScore, HighLevel, StringWorld,
                null, null));
                //score => Awardments.CheckForAward_TimeCrisis_Score(score),
                //level => { if (level >= Goal) SetGoalMet(true); }));
        }

        protected override BobPhsx GetHero(int i)
        {
            return BobPhsxNormal.Instance;
        }

        protected override void PreStart_Tutorial(bool TemporarySkip)
        {
            HeroRush_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new TimeCrisis_Tutorial(this));
        }
    }
}