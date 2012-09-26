using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_Escalation : Challenge
    {
        public override void CheckIfGoalMet(int level, int score)
        {
            base.CheckIfGoalMet(level, score);

            Awardments.CheckForAward_Escalation_Level(level - StartIndex);
        }

        static int[] NumLives = { 15, 12, 10, 5, 1 };

        static readonly Challenge_Escalation instance = new Challenge_Escalation();
        public static Challenge_Escalation Instance { get { return instance; } }

        public GUI_LivesLeft Gui_LivesLeft;



        protected Challenge_Escalation()
        {
            ID = new Guid("1a8141f9-525e-4e1e-8298-0c36b6ebdec3");
            MenuName = Name = "Escalation";
            MenuPic = "menupic_classic";
            HighScore = SaveGroup.EscalationHighScore;
            HighLevel = SaveGroup.EscalationHighLevel;
            Goal_Level = 35;
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
            game.WaitThenDo(50, ShowEndScreen);
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
            MyStringWorld = new StringWorldEndurance(GetSeeds(), Gui_LivesLeft, 25);
            
            Escalation_Tutorial.WatchedOnce = true;
            if (!Escalation_Tutorial.WatchedOnce)
                MyStringWorld.FirstDoorAction = false;
            MyStringWorld.OnBeginLoad += () => MyStringWorld.LevelSeeds.AddRange(this.GetMoreSeeds());
            MyStringWorld.StartLevelMusic = game => { };

            // Start menu
            MyStringWorld.OnLevelBegin += level =>
            {
                level.MyGame.AddGameObject(HelpMenu.MakeListener());
                level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
            };

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // Start
            MyStringWorld.Init(StartLevel);
        }

        protected virtual void PreStart_Tutorial()
        {
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new Escalation_Tutorial(this));
        }

        int GetLives()
        {
            int Difficulty = (StartIndex + 1) / 50;// LevelsPerDifficulty;
            return NumLives[Difficulty];
        }

        protected int LevelsPerDifficulty = 20;
        
        protected int LevelsPerTileset = 5;

        protected virtual void AdditionalPreStart()
        {
            // Tutorial
            PreStart_Tutorial();

            // When a new level is swapped to...
            MyStringWorld.OnSwapToLevel += levelindex =>
            {
                Awardments.CheckForAward_Escalation_Level(levelindex - StartIndex);

                // Score multiplier, x1, x1.5, x2, ... for levels 0, 20, 40, ...
                float multiplier = 1 + ((levelindex + 1) / LevelsPerDifficulty) * .5f;
                Tools.CurGameData.OnCalculateScoreMultiplier +=
                    game => game.ScoreMultiplier *= multiplier;

                OnSwapTo_GUI(levelindex);
            };
        }

        private void OnSwapTo_GUI(int levelindex)
        {
            // Multiplier increase text
            if ((levelindex + 1) % LevelsPerDifficulty == 0)
                Tools.CurGameData.AddGameObject(new MultiplierUp(string.Format("Multiplier Increased!", levelindex + 1)));

            // Mod number of coins

            // Reset time after death
            Tools.CurGameData.SetDeathTime(GameData.DeathTime.Normal);

            ////////// Level title (1, 5, 10, 15, ...)
            ////////if (levelindex == 0 || (levelindex + 1) % 5 == 0)
            if (levelindex > StartIndex)
            {
                var title = new LevelTitle(string.Format("Level {0}", levelindex + 1));
                Tools.CurGameData.AddGameObject(title);

                if ((levelindex + 1) % LevelsPerDifficulty == 0)
                    CampaignMenu._x_x_Red(title.text);

            }

            // Hero title
            //var g = Tools.CurGameData;
            //g.AddGameObject(new LevelTitle(g.MyLevel.DefaultHeroType.Name, new Vector2(150, -300), .7f, true));
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            for (i = 0; i < StartIndex; i++)
                MakeList.Add(() => null);
            for (i = StartIndex; i < StartIndex + 100; i++)
            {
                int Index = i; // Get the level number
                float difficulty = CoreMath.MultiLerpRestrict(Index / (float)LevelsPerDifficulty, -.5f, 0f, 1f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f);
                MakeList.Add(() => Make(Index, difficulty));
            }

            return MakeList;
        }
        protected override List<MakeSeed> MakeMoreMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            int n = i + 1;
            for (; i < n; i++)
            {
                int Index = i; // Get the level number
                MakeList.Add(() => Make(Index, 4f));
            }

            return MakeList;
        }

        //int LevelLength_Short = 5050;
        //int LevelLength_Long = 7300;

        int LevelLength_Short = 5500;
        int LevelLength_Long = 7500;

        //static TileSet[] tilesets = {
        //    TileSets.Terrace, TileSets.Dungeon, TileSets.Island,
        //    TileSets.Castle, TileSets.Rain, TileSets.Dungeon,
        //    TileSets._Night, TileSets._NightSky };

        //static string[] tilesets = { "sea", "hills", "forest", "cloud", "cave", "castle" };
        static string[] tilesets = { "hills", "forest", "cloud", "cave", "castle", "sea" };

        protected virtual TileSet GetTileSet(int i)
        {
            //return TileSets.NameLookup[tilesets[i % tilesets.Length]];
            return tilesets[(i / LevelsPerTileset) % tilesets.Length];
        }

        //static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[]
        //    { BobPhsxNormal.Instance });

        protected virtual BobPhsx GetHero(int i)
        {
            return StartMenu_MW_HeroSelect.ChosenHero;
            //return HeroTypes[i % HeroTypes.Count];
        }

        protected virtual LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index);

            // Adjust the length. Longer for higher levels.
            int Length;
            float t = ((Index - StartIndex) % LevelsPerTileset) / (float)(LevelsPerTileset - 1);
            Length = CoreMath.LerpRestrict(LevelLength_Short, LevelLength_Long, t);

            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(Difficulty, hero, Length);
            data.SetTileSet(GetTileSet(Index - StartIndex));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                // Shorten the initial computer delay
                piece.Style.ComputerWaitLengthRange = new Vector2(8, 35);//38);

                piece.Style.MyModParams = (level, p) =>
                {
                    Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                    Params.StartFrame = 90;
                    Params.FillType = Coin_Parameters.FillTypes.Regular;
                };
            }

            return data;
        }
    }
}