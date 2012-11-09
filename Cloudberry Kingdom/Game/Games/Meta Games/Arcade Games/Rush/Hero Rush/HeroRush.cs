using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_HeroRush : Rush
    {
        static readonly Challenge_HeroRush instance = new Challenge_HeroRush();
        public static Challenge_HeroRush Instance { get { return instance; } }

        protected Challenge_HeroRush()
        {
            GameTypeId = 1;
            MenuName = Name = Localization.Words.HeroRush;
        }

        // The progression of max time and start time for increasing difficulty
        static int[] MaxTime_ByDifficulty = { 20, 15, 10, 10, 10 };
        static int[] StartTime_ByDifficulty = { 15, 12, 10, 10, 10 };

        void SetTimerProperties(int Difficulty)
        {
            Difficulty = Math.Min(Difficulty, MaxTime_ByDifficulty.Length - 1);
            
            Timer.CoinTimeValue = (int)(62 * 1.75f);
            
            if (Difficulty >= MaxTime_ByDifficulty.Length)
                Timer.MaxTime = MaxTime_ByDifficulty[MaxTime_ByDifficulty.Length - 1];
            else
                Timer.MaxTime = 62 * (MaxTime_ByDifficulty[Difficulty] + 0) - 1;
        }

        protected virtual void PreStart_Tutorial(bool TemporarySkip)
        {
            HeroRush_Tutorial.TemporarySkip = TemporarySkip;
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new HeroRush_Tutorial(this));
        }

        protected virtual void MakeExitDoorIcon(int levelindex)
        {
            Vector2 shift = new Vector2(0, 470);

            Tools.CurGameData.AddGameObject(new DoorIcon(GetHero(levelindex + 1 - StartIndex),
                        Tools.CurLevel.FinalDoor.Pos + shift, 1));

            // Delete the exit sign
            foreach (ObjectBase obj in Tools.CurLevel.Objects)
                if (obj is Sign)
                    obj.Core.MarkedForDeletion = true;
        }

        class ScoreMultiplierHelper : Lambda_1<GameData>
        {
            float multiplier;

            public ScoreMultiplierHelper(float multiplier)
            {
                this.multiplier = multiplier;
            }

            public void Apply(GameData game)
            {
                game.ScoreMultiplier *= multiplier;
            }
        }

        int LevelsPerDifficulty = 20;
        protected override void AdditionalPreStart()
        {
            base.AdditionalPreStart();

            // Set timer values
            int Difficulty = (StartIndex + 1) / LevelsPerDifficulty;
            SetTimerProperties(Difficulty);

            if (Difficulty >= StartTime_ByDifficulty.Length)
                Timer.Time = 62 * StartTime_ByDifficulty[StartTime_ByDifficulty.Length - 1];
            else
                Timer.Time = 62 * StartTime_ByDifficulty[Difficulty];

            // Tutorial
            PreStart_Tutorial(StartIndex > 0);

            // When a new level is swapped to...
            MyStringWorld.OnSwapToLevel += levelindex =>
            {
                Awardments.CheckForAward_HeroRush2Unlock(levelindex - StartIndex);

                // Add hero icon to exit door
                MakeExitDoorIcon(levelindex);

                // Score multiplier, x1, x1.5, x2, ... for levels 0, 20, 40, ...
                float multiplier = 1 + ((levelindex + 1) / LevelsPerDifficulty) * .5f;
                Tools.CurGameData.OnCalculateScoreMultiplier.Add(new ScoreMultiplierHelper(multiplier));

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
            float difficulty = CoreMath.MultiLerpRestrict(Index / (float)LevelsPerDifficulty, -.5f, 0f, 1f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f);
            var seed = Make(Index, difficulty);

            return seed;
        }

        static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[]
            { BobPhsxNormal.Instance, BobPhsxJetman.Instance, BobPhsxDouble.Instance,
              BobPhsxSmall.Instance, BobPhsxWheel.Instance, BobPhsxSpaceship.Instance,
              BobPhsxBouncy.Instance, BobPhsxBig.Instance });

        protected virtual BobPhsx GetHero(int i)
        {
            return HeroTypes[i % HeroTypes.Count];
        }

        int LevelsPerTileset = 4;
        static string[] tilesets = { "sea", "hills", "forest", "cloud", "cave", "castle" };
        //static string[] tilesets = { "hills", "forest", "cloud", "cave", "castle", "sea" };

        protected virtual TileSet GetTileSet(int i)
        {
            return tilesets[(i / LevelsPerTileset) % tilesets.Length];
        }

        int LevelLength_Short = 2150;
        int LevelLength_Long = 3900;
        protected virtual LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index - StartIndex);

            // Adjust the length. Longer for higher levels.
            int Length;
            if (Index == 0 || (Index + 1) % LevelsPerDifficulty == 0)
                Length = LevelLength_Short;
            else
            {
                float t = (((Index + 1) % LevelsPerDifficulty) / 5 + 1) / 5f;
                Length = CoreMath.LerpRestrict(LevelLength_Short, LevelLength_Long, t);
            }

            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(Difficulty, hero, Length);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                // Shorten the initial computer delay
                piece.Style.ComputerWaitLengthRange = new Vector2(4, 23);

                // Only one path
                piece.Paths = 1; piece.LockNumOfPaths = true;

                piece.Style.MyModParams = (level, p) =>
                    {
                        Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                        Params.FillType = Coin_Parameters.FillTypes.Rush;
                    };
            }

            return data;
        }
    }
}