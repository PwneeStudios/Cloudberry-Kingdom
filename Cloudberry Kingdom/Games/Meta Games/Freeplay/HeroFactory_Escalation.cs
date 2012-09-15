using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_HeroFactoryEscalation : Challenge_Escalation
    {
        public static BobPhsx FactoryHero = BobPhsxNormal.Instance;

        static readonly Challenge_HeroFactoryEscalation instance = new Challenge_HeroFactoryEscalation();
        public static Challenge_HeroFactoryEscalation Instance { get { return instance; } }

        protected Challenge_HeroFactoryEscalation()
        {
            ID = new Guid("1a8141f9-aa5e-3f1e-8298-0c36b6ebdec3");
            Name = "Hero Factory";
            //MenuPic = "menupic_HeroFactoryEscalation";
            MenuPic = "menupic_classic";
            HighScore = SaveGroup.HeroFactoryEscalationHighScore;
            HighLevel = SaveGroup.HeroFactoryEscalationHighLevel;
            Goal_Level = 35;
        }

        protected override void ShowEndScreen()
        {
            Tools.CurGameData.AddGameObject(new GameOverPanel(HighScore, HighLevel, StringWorld, null, null));
        }

        protected override BobPhsx GetHero(int i)
        {
            return FactoryHero;
        }
    }
}