using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_Escalation : Challenge
    {
        //static int[] NumLives = { 15, 12, 10, 5, 1 };
        static int[] NumLives = { 15, 15, 15, 15, 15 };

        static readonly Challenge_Escalation instance = new Challenge_Escalation();
        public static Challenge_Escalation Instance { get { return instance; } }

        public GUI_LivesLeft Gui_LivesLeft;

        protected Challenge_Escalation()
        {
            GameTypeId = 0;
            MenuName = Name = Localization.Words.Escalation;
        }

        protected void OnOutOfLives(GUI_LivesLeft Lives)
        {
            GameData game = Lives.MyGame;
            Level level = game.MyLevel;

            level.PreventReset = true;
            level.SetToReset = false;

            // End the level
            level.EndLevel();

            // Special explode
            foreach (Bob bob in level.Bobs)
                ParticleEffects.PiecePopFart(level, bob.Core.Data.Position);

            // Add the Game Over panel, check for Awardments
            game.WaitThenDo(50, new ShowEndScreenProxy(this));
        }

        protected int i = 0;
        protected int StartIndex = 0;
        protected StringWorldEndurance MyStringWorld;
        public override void Start(int StartLevel)
        {
            base.Start(StartLevel);

            PlayerManager.CoinsSpent = -100;

            i = StartIndex = StartLevel;

            // Create the lives counter
            Gui_LivesLeft = new GUI_LivesLeft(GetLives());

            // Set the time expired function
            Gui_LivesLeft.OnOutOfLives += OnOutOfLives;

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldEndurance(GetSeed, Gui_LivesLeft, 25);
            
            Escalation_Tutorial.WatchedOnce = true;
            if (!Escalation_Tutorial.WatchedOnce)
                MyStringWorld.FirstDoorAction = false;

            MyStringWorld.StartLevelMusic = null;

            // Start menu
            MyStringWorld.OnLevelBegin = new OnBeginLambda();

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init(StartLevel);
        }

        class OnBeginLambda : LambdaFunc_1<Level, bool>
        {
            public OnBeginLambda()
            {
            }

            public bool Apply(Level level)
            {
                level.MyGame.AddGameObject(HelpMenu.MakeListener());
                level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
                return false;
            }
        }

        protected virtual void PreStart_Tutorial()
        {
            MyStringWorld.OnSwapToFirstLevel.Add(new OnSwapLambda(this));
        }

        class OnSwapLambda : Lambda_1<LevelSeedData>
        {
            Challenge_Escalation ch;
            public OnSwapLambda(Challenge_Escalation ch)
            {
                this.ch = ch;
            }

            public void Apply(LevelSeedData data)
            {
                data.MyGame.AddGameObject(new Escalation_Tutorial(ch));
            }
        }

        int GetLives()
        {
            int Difficulty = (StartIndex + 1) / 50;
            return NumLives[Difficulty];
        }

        protected int LevelsPerDifficulty = 20;
        
        protected int LevelsPerTileset = 5;

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

        class AdditionalPreStartOnSwapToLevelHelper : Lambda_1<int>
        {
            Challenge_Escalation ce;

            public AdditionalPreStartOnSwapToLevelHelper(Challenge_Escalation ce)
            {
                this.ce = ce;
            }

            public void Apply(int levelindex)
            {
                Awardments.CheckForAward_Escalation_Level(levelindex - ce.StartIndex);

                // Score multiplier, x1, x1.5, x2, ... for levels 0, 20, 40, ...
                float multiplier = 1 + ((levelindex + 1) / ce.LevelsPerDifficulty) * .5f;
                Tools.CurGameData.OnCalculateScoreMultiplier.Add(new ScoreMultiplierHelper(multiplier));

                ce.OnSwapTo_GUI(levelindex);
            }
        }

        protected virtual void AdditionalPreStart()
        {
            // Tutorial
            PreStart_Tutorial();

            // When a new level is swapped to...
            MyStringWorld.OnSwapToLevel.Add(new AdditionalPreStartOnSwapToLevelHelper(this));
        }

        private void OnSwapTo_GUI(int levelindex)
        {
            // Multiplier increase text
            if ((levelindex + 1) % LevelsPerDifficulty == 0)
                Tools.CurGameData.AddGameObject(new MultiplierUp());

            // Mod number of coins

            // Reset time after death
            Tools.CurGameData.SetDeathTime(GameData.DeathTime.Normal);

            if (levelindex > StartIndex)
            {
                Tools.Warning();
                var title = new LevelTitle(string.Format("{1} {0}", levelindex + 1, Localization.WordString(Localization.Words.Level)));
                Tools.CurGameData.AddGameObject(title);

                if ((levelindex + 1) % LevelsPerDifficulty == 0)
                    CkColorHelper._x_x_Red(title.text);
            }

            // Hero title
            //var g = Tools.CurGameData;
            //g.AddGameObject(new LevelTitle(g.MyLevel.DefaultHeroType.Name, new Vector2(150, -300), .7f, true));
        }

        public override LevelSeedData GetSeed(int Index)
        {
            float difficulty = CoreMath.MultiLerpRestrict(Index / (float)LevelsPerDifficulty, -.5f, 0f, 1f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f);
            var seed = Make(Index, difficulty);

            return seed;
        }

        int LevelLength_Short = 5500;
        int LevelLength_Long = 7500;

        static string[] tilesets = { "hills", "forest", "cloud", "cave", "castle", "sea" };

        protected virtual TileSet GetTileSet(int i)
        {
            return tilesets[(i / LevelsPerTileset) % tilesets.Length];
        }

        protected virtual BobPhsx GetHero(int i)
        {
            return Challenge.ChosenHero;
        }

        class MakeMyModParamsHelper : Lambda_2<Level, PieceSeedData>
        {
            public void Apply(Level level, PieceSeedData p)
            {
                Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                Params.StartFrame = 90;
                Params.FillType = Coin_Parameters.FillTypes.Regular;
            }
        }

        protected virtual LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index);

            // Adjust the length. Longer for higher levels.
            int Length;
            float t = ((Index - StartIndex) % LevelsPerTileset) / (float)(LevelsPerTileset - 1);
            Length = CoreMath.LerpRestrict(LevelLength_Short, LevelLength_Long, t);

            if (hero is BobPhsxSpaceship)
                Length += 2000;

            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(Difficulty, hero, Length);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                // Shorten the initial computer delay
                piece.Style.ComputerWaitLengthRange = new Vector2(8, 35);//38);

                piece.Style.MyModParams.Add(new MakeMyModParamsHelper());
            }

            return data;
        }
    }
}