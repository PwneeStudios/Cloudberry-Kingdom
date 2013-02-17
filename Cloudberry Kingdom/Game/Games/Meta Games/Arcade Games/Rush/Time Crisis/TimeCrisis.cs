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

		protected override int GetLength(int Index, float Difficulty)
		{
			int Length = base.GetLength(Index, Difficulty);

			if (Challenge.ChosenHero == BobPhsxRocketbox.Instance)
			{
				Length = (int)(Length * CoreMath.LerpRestrict(1.0f, 10.0f, Index / 1000.0f));
			}

			return Length;
		}

		protected override LevelSeedData Make(int Index, float Difficulty)
		{
			float difficulty = CoreMath.MultiLerpRestrict(Index / 20.0f, -.5f, 0f, 1.5f, 2.75f, 3.5f, 4.25f, 5.0f, 5.5f, 6.5f);
			LevelSeedData data = base.Make(Index, Difficulty);

			return data;
		}

        protected override void PreStart_Tutorial(bool TemporarySkip)
        {
            HeroRush_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new TimeCrisis_Tutorial(this));
        }

		public override void Start(int StartLevel)
		{
			base.Start(StartLevel);

            CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.TimeCrisis);
		}
    }
}