using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Challenge_SavePrincessRush : Rush
    {
        static readonly Challenge_SavePrincessRush instance = new Challenge_SavePrincessRush();
        public static Challenge_SavePrincessRush Instance { get { return instance; } }

        protected Challenge_SavePrincessRush()
        {
            ID = new Guid("3a5141f5-585e-3716-8392-3c96b6ebdec7");
            Name = "Save Princes Rush";
        }

        protected override void ShowEndScreen()
        {
            Tools.CurrentAftermath = new AftermathData();
            Tools.CurrentAftermath.Success = false;
            Tools.CurrentAftermath.Retry = true;

            Tools.CurGameData.EndGame(false);
        }

        // The progression of max time and start time for increasing difficulty
        static int[] MaxTime_ByDifficulty = { 40, 20, 18, 18, 18 };
        static int[] StartTime_ByDifficulty = { 12, 6, 3, 3, 3 };

        static float[] StartDifficulties = { .4f, .9f, 2f, 2.75f, 3.9f };
        static float[] EndDifficulties = { .8f, 1.5f, 2.8f, 3.5f, 4.3f };
        public static int[] NumLevels = { 11, 11, 11, 11, 11 };
        //public static int[] NumLevels = { 3, 3, 3, 3, 3 };

        int TotalLevels;
        float StartDifficulty, EndDifficulty;

        void SetTimerProperties(int Difficulty)
        {
            Difficulty = Math.Min(Difficulty, MaxTime_ByDifficulty.Length - 1);
            
            Timer.CoinTimeValue = (int)(62 * 1.75f);
            //Timer.CoinTimeValue = (int)(0);
            Timer.MaxTime = 62 * (MaxTime_ByDifficulty[Difficulty] + 0) - 1;
        }

        protected virtual void MakeExitDoorIcon(int levelindex)
        {
            var hero = GetHero(levelindex + 1 - StartIndex);
            //if (hero == BobPhsxNormal.Instance)
            //    return;

            Vector2 shift = new Vector2(0, 470);

            Tools.CurGameData.AddGameObject(new DoorIcon(hero,
                        Tools.CurLevel.FinalDoor.Pos + shift, 1));

            // Delete the exit sign
            foreach (ObjectBase obj in Tools.CurLevel.Objects)
                if (obj is Sign)
                    obj.Core.MarkedForDeletion = true;
        }


        protected void Intro()
        {
            MyStringWorld.OnSwapToFirstLevel += data => data.MyGame.AddGameObject(new SavePrincessRush_Intro(this));
        }

        protected override void AdditionalPreStart()
        {
            base.AdditionalPreStart();

            // Intro
            Intro();

            // Set timer values
            SetTimerProperties(DifficultySelected);
            Timer.Time = 62 * (StartTime_ByDifficulty[DifficultySelected] + 0);

            // When a new level is swapped to...
            MyStringWorld.OnSwapToLevel += levelindex =>
            {
                // Invert level
                if (levelindex % 2 == 1)
                    Tools.CurGameData.MyLevel.ModZoom.X = -1;

                // Add hero icon to exit door
                MakeExitDoorIcon(levelindex);

                // Mod number of coins
                CoinMod mod = new CoinMod(Timer);
                mod.LevelMax = 17;
                mod.ParMultiplier_Start = 1.6f;
                mod.ParMultiplier_End = 1f;
                mod.CoinControl(Tools.CurGameData.MyLevel, (levelindex + 1));

                // Reset sooner after death
                Tools.CurGameData.SetDeathTime(GameData.DeathTime.Fast);

                // Cheering berries (5, 10, ... )
                if ((levelindex + 1) % 5 == 0 && levelindex != StartIndex)
                    Tools.CurGameData.AddGameObject(new SuperCheer(1));

                // Level title (1, 5, 10, 15, ...)
                if (levelindex == 0 || (levelindex + 1) % 3 == 0)
                    Tools.CurGameData.AddGameObject(new LevelTitle(string.Format("Floor {0}", levelindex + 1)));
            };
        }

        public override void Start(int StartLevel)
        {
            if (NonCampaign)
                PlayerManager.CoinsSpent = 0;

            DifficultySelected = StartLevel;

            i = StartLevel;
            StartIndex = 0;

            // Create the timer
            Timer = new GUI_Timer();

            // Set the time expired function
            Timer.OnTimeExpired += OnTimeExpired;

            // Create the string world, and add the relevant game objects
            MyStringWorld = new StringWorldTimed(GetSeeds(), Timer);
            //MyStringWorld.OnBeginLoad += () => MyStringWorld.LevelSeeds.AddRange(this.GetMoreSeeds());
            MyStringWorld.EndMusicOnFinish = false;
            
            //MyStringWorld.MyGUI_Level.Core.Show = false;
            //MyStringWorld.MyGUI_Score.Core.Show = false;

            //MyStringWorld.MyGUI_Level.Release();
            //MyStringWorld.MyGUI_Level = new GUI_CampaignLevel();
            MyStringWorld.MyGUI_Score.Release();
            MyStringWorld.MyGUI_Score = new GUI_CampaignScore();


            MyStringWorld.MyGUI_Level.Prefix = "Floor ";
            MyStringWorld.StartLevelMusic = game => { };

            // Start menu
            MyStringWorld.OnLevelBegin += level =>
                    //level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
                    level.MyGame.AddGameObject(InGameStartMenu_CampaignLevel.MakeListener());

            // Additional preprocessing
            SetGameParent(MyStringWorld);
            AdditionalPreStart();

            // End of string
            MyStringWorld.EOG_DoorAction = d =>
            {
                StringWorldGameData.EOG_StandardDoorAction(d);
                d.Game.MyStatGroup = StatGroup.Game;
                d.Game.MakeScore = () =>
                {
                    d.Game.WaitThenDo(13, () => d.Game.EndGame(false));
                    return null;
                };
                GameData.EOL_DoorAction(d);
            };

            // Start
            MyStringWorld.Init(0);

            // Switch to cut-scene
            if (!CinematicShown)
            {
                Tools.WorldMap.SkipBackgroundPhsx = true;
                Tools.ShowLoadingScreen = false;
                Cinematic = new Campaign_PrincessOverLava(this);
                Campaign_String.MainString.LoadAsSubGame(Cinematic, false);
                CinematicShown = true;
            }
        }

        Campaign_PrincessOverLava Cinematic;
        public static bool CinematicShown = false;
        public void SwitchBackFromCinematic()
        {
            Tools.WorldMap = MyStringWorld;
            Tools.WorldMap.SkipBackgroundPhsx = false;
            Tools.CurGameData = MyStringWorld;
            //Tools.ShowLoadingScreen = true;
            MyStringWorld.EndLoadingImmediately = true;
            Tools.TheGame.ToDo.Add(() => Cinematic.Release());
            MyStringWorld.PhsxStepsToDo += 1;
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            // Calculate difficulty
            StartDifficulty = StartDifficulties[DifficultySelected];
            EndDifficulty = EndDifficulties[DifficultySelected];
            //TotalLevels = 1;
            TotalLevels = NumLevels[DifficultySelected];

            for (i = 0; i < TotalLevels; i++)
            {
                int Index = i; // Get the level number
                float difficulty = Tools.MultiLerpRestrict(Index / (float)TotalLevels, StartDifficulty, EndDifficulty);
                MakeList.Add(() => Make(Index, difficulty));
            }

            return MakeList;
        }

        static List<BobPhsx> HeroTypes = new List<BobPhsx>(new BobPhsx[]
            { BobPhsxNormal.Instance, BobPhsxJetman.Instance, BobPhsxDouble.Instance,
              BobPhsxSmall.Instance, BobPhsxWheel.Instance, BobPhsxSpaceship.Instance,
              //BobPhsxBox.Instance,
              BobPhsxBouncy.Instance,
              BobPhsxRocketbox.Instance,
              //BobPhsxBig.Instance
            });

        protected virtual BobPhsx GetHero(int i)
        {
            int NumNormal = 3;
            if (i < NumNormal) return BobPhsxNormal.Instance;

            return HeroTypes[(i - NumNormal + 1) % HeroTypes.Count];
        }

        protected virtual TileSet GetTileSet(int i)
        {
            return TileSets.Castle;
        }

        int LevelLength_Short = 2150;
        int LevelLength_Long = 6500;
        LevelSeedData Make(int Index, float Difficulty)
        {
            BobPhsx hero = GetHero(Index - StartIndex);

            // Adjust the length. Longer for higher levels.
            int Length = Tools.LerpRestrict(LevelLength_Short, LevelLength_Long, Index / (float)TotalLevels);


            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(Difficulty, hero, Length);
            data.SetTileSet(GetTileSet(Index));

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