using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_Madness : Rush
    {
        static readonly Challenge_Madness instance = new Challenge_Madness();
        public static Challenge_Madness Instance { get { return instance; } }

        protected Challenge_Madness()
        {
            GameTypeId = 4;
            MenuName = Name = Localization.Words.Madness;

			SetGameId();
        }

        const int TimePerLevel = 60 * 62 - 1;
        void SetTimerProperties(int Difficulty)
        {
            Timer.CoinTimeValue = 0;
            Timer.Time = TimePerLevel;
            Timer.MaxTime = TimePerLevel;
        }

        protected virtual void PreStart_Tutorial(bool TemporarySkip)
        {
            Madness_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new Madness_Tutorial(this));
        }

        protected virtual void MakeExitDoorIcon(int levelindex)
        {
            // Delete the exit sign
            foreach (ObjectBase obj in Tools.CurLevel.Objects)
                if (obj is Sign)
                    obj.Core.MarkedForDeletion = true;
        }

        protected virtual void AdditionalSwap(int levelindex)
        {
        }

        int LevelsPerDifficulty = 20;
        protected override void AdditionalPreStart()
        {
            base.AdditionalPreStart();

            // Set timer values
            int Difficulty = (StartIndex + 1) / LevelsPerDifficulty;
            SetTimerProperties(Difficulty);

            Timer.Time = TimePerLevel;

            // Tutorial
            PreStart_Tutorial(StartIndex > 0);

            // When a new level is swapped to...
            MyStringWorld.OnSwapToLevel += levelindex =>
            {
                AdditionalSwap(levelindex);

                Timer.Time = TimePerLevel;

                ArcadeMenu.CheckForArcadeUnlocks_OnSwapIn(levelindex);

                // Add hero icon to exit door
                MakeExitDoorIcon(levelindex);

                // Score multiplier, x1, x1.5, x2, ... for levels 0, 20, 40, ...
                float multiplier = 1 + ((levelindex + 1) / LevelsPerDifficulty) * .5f;
                Tools.CurGameData.OnCalculateScoreMultiplier +=
                    game => game.ScoreMultiplier *= multiplier;

                // Mod number of coins
                CoinMod mod = new CoinMod(Timer);
                mod.LevelMax = 17;
                mod.ParMultiplier_Start = 1.6f;
                mod.ParMultiplier_End = 1f;
                mod.CoinControl(Tools.CurGameData.MyLevel, (levelindex + 1) % LevelsPerDifficulty);

                // Reset sooner after death
                Tools.CurGameData.SetDeathTime(GameData.DeathTime.Fast);

                // Modify the timer
                SetTimerProperties(levelindex / LevelsPerDifficulty);

                OnSwapTo_GUI(levelindex);
            };
        }

        private void OnSwapTo_GUI(int levelindex)
        {
            // Multiplier increase text
            if ((levelindex + 1) % LevelsPerDifficulty == 0)
                Tools.CurGameData.AddGameObject(new MultiplierUp());

            // Cheering berries (20, 40, 60, ...)
            if ((levelindex + 1) % LevelsPerDifficulty == 0 && levelindex != StartIndex)
                Tools.CurGameData.AddGameObject(new SuperCheer(1));
        }

        public override LevelSeedData GetSeed(int Index)
        {
            float difficulty = CoreMath.MultiLerpRestrict(Index / (float)LevelsPerDifficulty, -.5f, 0f, 1f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5.0f, 6.0f);
            var seed = Make(Index, difficulty);

            return seed;
        }

        public override void Start(int StartLevel)
        {
			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Madness);

            base.Start(StartLevel);
        }

        static string[] tilesets = { "sea", "forest", "cave", "castle", "cloud", "hills",
                                     "sea_rain", "forest_snow", "hills_rain" };

        protected virtual TileSet GetTileSet(int i)
        {
            return tilesets[Tools.GlobalRnd.RndInt(0, tilesets.Length - 1)];
        }

		protected virtual int GetLength(int Index, float Difficulty)
		{
            return 3000;
		}


        protected virtual LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = BobPhsxNormal.Instance, hero2 = null;
            LevelGeometry Geometry = LevelGeometry.Right;

            switch (Index % 1) {
                case 0:
                    hero  = BobPhsxNormal.Instance;
                    hero2 = BobPhsxUpsideDown.Instance;
                    break;

                case 1:
                    hero  = BobPhsxNormal.Instance;
                    hero2 = BobPhsxNormal.Instance;
                    Geometry = LevelGeometry.Up;
                    break;

                case 2:
                    hero  = BobPhsxJetman.Instance;
                    hero2 = BobPhsxJetman.Instance;
                    Geometry = LevelGeometry.Up;
                    break;

                case 3:
                    hero  = BobPhsxJetman.Instance;
                    Geometry = LevelGeometry.Up;
                    break;
            }

            // Adjust the length. Longer for higher levels.
            int Length = GetLength(Index, Difficulty);

            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(Difficulty, hero, hero2, Geometry, Length, false);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                piece.MyMetaGameType = MetaGameType.TimeCrisis;
            }

            return data;
        }
    }
}