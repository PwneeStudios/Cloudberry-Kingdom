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

        protected Challenge_TimeCrisis()
        {
            GameTypeId = 1;
            MenuName = Name = Localization.Words.TimeCrisis;
        }

        protected override BobPhsx GetHero(int i)
        {
            return Challenge.ChosenHero;
        }

        protected override void PreStart_Tutorial(bool TemporarySkip)
        {
            HeroRush_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel.Add(new OnSwapLambda(this));
        }

        class OnSwapLambda : Lambda_1<LevelSeedData>
        {
            Challenge_TimeCrisis ch;
            public OnSwapLambda(Challenge_TimeCrisis ch)
            {
                this.ch = ch;
            }

            public void Apply(LevelSeedData data)
            {
                data.MyGame.AddGameObject(new TimeCrisis_Tutorial(ch));
            }
        }
    }
}