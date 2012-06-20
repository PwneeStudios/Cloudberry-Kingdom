using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class StringWorldGameData : GameData
    {
        public static new GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return null;
        }

        public override void Release()
        {
            base.Release();

            if (Released) return;

            foreach (LevelSeedData data in LevelSeeds)
            {
                if (data != null && data.MyGame != null && !data.MyGame.Released)
                {
                    data.MyGame.Release();
                    data.Release();
                }
                /*
                if (data != null)
                {
                    lock (data.Loaded)
                    {
                        if (data.Loaded.val)
                        {
                            if (data.MyGame != null)
                            {
                                if (!data.MyGame.Released)
                                {
                                    data.MyGame.Release();
                                    data.Release();
                                }
                            }
                        }
                        else
                            data.ReleaseWhenLoaded = true;
                    }
                }*/
            }
            LevelSeeds = null;

            CurLevelSeed = null;
            LastLevelSeedSet = null;
            OnBeginLoad = null;
            OnLevelBegin = null;
            OnSwapToLevel = null;
            OnSwapToFirstLevel = null;
            OnSwapToLastLevel = null;
            StartLevelMusic = null;

            if (Tools.WorldMap == this)
                Tools.WorldMap = null;
        }

        public int Count;

        public List<LevelSeedData> LevelSeeds;
        public int CurLevelIndex = 0;
        public bool FirstLevelHasLoaded = false;
        public bool FirstLevelHasBegun = false;
        LevelSeedData CurLevelSeed; // May be differet than LevelSeeds[CurLevelIndex]

        /// <summary>
        /// Called when a level begins loading
        /// </summary>
        public Action OnBeginLoad;

        public LevelSeedData GetSeed(int Index)
        {
            return LevelSeeds[Index];
        }

        public int GetIndex(Level level)
        {
            return LevelSeeds.FindIndex(data => data != null && data.MyGame != null && data.MyGame.MyLevel == level);
        }
        public int GetIndex(GameData game)
        {
            return LevelSeeds.FindIndex(data => data.MyGame == game);
        }
        public int GetIndex(LevelSeedData data)
        {
            return LevelSeeds.FindIndex(_data => data == _data);
        }

        protected virtual LevelSeedData MakeLevelSeed(int LevelNum)
        {
            LevelSeedData data;

            data = new LevelSeedData();
            data.Seed = MyLevel.Rnd.Rnd.Next();
            data.SetTileSet(TileSets.Terrace);
            data.DefaultHeroType = BobPhsxNormal.Instance;
            data.MyGameFlags.IsTethered = false;

            data.PreInitialize(NormalGameData.Factory, 100, (int)3, (int)7000, delegate(PieceSeedData piece)
            //data.PreInitialize(GameType.Normal, 100, (int)1, (int)1000, delegate(PieceSeedData piece)
            {
                RndDifficulty.ZeroUpgrades(piece.MyUpgrades1);
                piece.MyUpgrades1[Upgrade.FlyBlob] = 3;
                piece.MyUpgrades1[Upgrade.BouncyBlock] = 5;
                piece.MyUpgrades1[Upgrade.MovingBlock] = 3;
                //piece.MyUpgrades1[Upgrade.Cloud] = 10;
                piece.MyUpgrades1[Upgrade.Jump] = 8;
                piece.MyUpgrades1[Upgrade.General] = 3;
                piece.MyUpgrades1[Upgrade.Speed] = 3;
                piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

                RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
                piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
                piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

                if (piece.MyPieceIndex == 0)
                    piece.Style.MyInitialPlatsType = StyleData.InitialPlatsType.LandingZone;
            });

            return data;
        }

        void MakeLevelSeeds()
        {
            LevelSeeds = new List<LevelSeedData>();
            
            for (int i = 0; i < 100; i++)
            {
                LevelSeedData data = MakeLevelSeed(i);
                LevelSeeds.Add(data);
            }
        }


        /// <summary>
        /// Returns whether the level of the specified index has finished loading.
        /// </summary>
        public bool IsLoaded(int Index)
        {
            LevelSeedData data = LevelSeeds[Index];

            lock (data.Loaded)
            {
                return data.Loaded.val;
            }
        }

        /// <summary>
        /// Called during LevelBegin, adds relevant GameObjects to the level's game.
        /// </summary>
        public Action<Level> OnLevelBegin;

        /// <summary>
        /// How long to wait before opening the initial door.
        /// </summary>
        //int WaitLengthToOpenDoor = 0;
        int WaitLengthToOpenDoor = 6;
        //int WaitLengthToOpenDoor = 10;

        /// <summary>
        /// How long to wait before opening the initial door on the first level.
        /// </summary>
        protected int WaitLengthToOpenDoor_FirstLevel = 6;
        

        /// <summary>
        /// Various details that must occur just before a level is played.
        /// </summary>
        public void LevelBegin(Level level)
        {
            Recycler.DumpMetaBin();
            //Recycle.Empty();

            if (OnLevelBegin != null)
                OnLevelBegin(level);

            BeginningCloseDoor(level);

            bool Hold_FirstLevelHasBegun = FirstLevelHasBegun;
            if (level.MyGame != null)
            {
                level.MyGame.AddToDo(() =>
                    {
                        //if (level.MyGame.SoftPause)
                        if (level.MyGame.PauseGame)
                            return false;

                        if (FirstDoorAction || Hold_FirstLevelHasBegun)
                            StartOfLevelDoorAction(level);

                        // Start music
                        if (StartLevelMusic != null)
                            StartLevelMusic(this);

                        // Start recording
                        //level.StartRecording();

                        return true;
                    });

                level.MyGame.PhsxStepsToDo += 2;
            }

            FirstLevelHasBegun = true;
        }

        /// <summary>
        /// Called at the start of a level to begin music.
        /// </summary>
        public Action<StringWorldGameData> StartLevelMusic = DefaultStartLevelMusic;

        static void DefaultStartLevelMusic(StringWorldGameData stringworld)
        {
            Tools.SongWad.SetPlayList(Tools.SongList_Standard);

            if (!stringworld.FirstLevelHasLoaded)
                Tools.SongWad.Next();
        }

        /// <summary>
        /// Closes the beginning door of the level and hides the players;
        /// </summary>
        public static void BeginningCloseDoor(Level level)
        {
            // Find the initial door
            Door door = level.FindIObject(LevelConnector.StartOfLevelCode) as Door;
            if (null == door) return;

            // Close the door
            door.SetLock(true, true, false);
            door.HideBobs();

            level.Finished = true;
        }

        public bool FirstDoorAction = true;
        void StartOfLevelDoorAction(Level level)
        {
            GameData game = level.MyGame;

            if (game == null) return;

            // Find the initial door
            Door door = level.FindIObject(LevelConnector.StartOfLevelCode) as Door;
            if (null == door) return;

            // Open the door and show players
            int Wait = WaitLengthToOpenDoor_FirstLevel;
            //if (FirstLevelHasBegun)
            if (CurLevelIndex > 0)
                Wait = WaitLengthToOpenDoor;
            game.WaitThenDo(Wait, () =>
                {
                    // Whether to play a sound for the door opening
                    bool sound = false;
                    if (CurLevelIndex == StartIndex)
                        sound = true;
                    
                    level.Finished = false;
                    door.SetLock(false, false, sound);
                    door.ShowBobs();
                }); //, "OpenDoor", true, false);
        }

        /// <summary>
        /// Whether the level is loaded.
        /// </summary>
        public bool LevelIsLoaded(LevelSeedData data)
        {
            lock (data.Loaded)
            {
                return data.Loaded.val;
            }                
        }

        /// <summary>
        /// Assuming a level is loaded, set that level as current.
        /// Begin loading next level.
        /// </summary>
        public void SetLevel(LevelSeedData data) { SetLevel(data, true); }
        public void SetLevel(LevelSeedData data, bool Fresh) { SetLevel(LevelSeeds.IndexOf(data), Fresh); }
        public void SetLevel(int Index) { SetLevel(Index, true); }
        public void SetLevel(int Index, bool Fresh)
        {
            CurLevelIndex = Index;
            LevelSeedData data = LevelSeeds[CurLevelIndex];

            if (Fresh && data.MyGame != null)
            {
                // Check for last level
                if (LevelSeeds.IndexOf(data) == LevelSeeds.Count - 1)
                {
                    ObjectBase obj = data.MyGame.MyLevel.FindIObject(LevelConnector.EndOfLevelCode);

                    Door door = obj as Door;
                    if (null != door)
                        door.OnOpen = EOG_DoorAction;
                }

                // Replace all Bobs with new Bobs (to handle newly joined players)
                data.MyGame.UpdateBobs();
                data.MyGame.Reset();
            }

            lock (data.Loaded)
            {
                // If game hasn't loaded yet, bring loading screen
                if (!data.Loaded.val)
                {
                    if (!data.LoadingBegun)
                        Load(CurLevelIndex);

                    if (!Tools.ShowLoadingScreen)
                    {
                        Tools.CurLevel = MyLevel;
                        Tools.CurGameData = this;
                        Tools.BeginLoadingScreen(true);
                    }
                }
                else
                {
                    SwapToLevel(data);
                    Tools.WorldMap = this;
                }
            }
        }

        public override void Finish(bool Replay)
        {
            base.Finish(Replay);
        }

        /// <summary>
        /// Called when the first level is swapped in.
        /// </summary>
        public event Action<LevelSeedData> OnSwapToFirstLevel;

        /// <summary>
        /// Called when the last level is swapped in.
        /// </summary>
        public event Action<LevelSeedData> OnSwapToLastLevel;

        /// <summary>
        /// Called when a level is swapped to. The parameter is the current level index.
        /// </summary>
        public event Action<int> OnSwapToLevel;

        /// <summary>
        /// True after the first level has been swapped in (and always true thereafter)
        /// </summary>
        bool FirstLevelSwappedIn = false;

        public void SwapToLevel(LevelSeedData data)
        {
            // Perform actions if this is the first level being swapped in.
            if (!FirstLevelSwappedIn)
            {
                if (OnSwapToFirstLevel != null)
                    OnSwapToFirstLevel(data);
                FirstLevelSwappedIn = true;
            }

            // Perform actions if this is the last level being swapped in.
            if (LevelSeeds.IndexOf(data) == LevelSeeds.Count - 1)
            {
                if (OnSwapToLastLevel != null)
                    OnSwapToLastLevel(data);
            }

            // Stores the GameObjects in the current game marked as 'PreventRelease'
            List<GameObject> ObjectsToSave = new List<GameObject>();

            if (CurLevelSeed != null && data != CurLevelSeed)
            {
                if (CurLevelSeed.MyGame.Loading)
                    throw new InvalidOperationException("Swapping from a level that hasn't finished loading!");

                foreach (GameObject obj in CurLevelSeed.MyGame.MyGameObjects)
                    if (obj.PreventRelease)
                        ObjectsToSave.Add(obj);

                CurLevelSeed.MyGame.Release();
                CurLevelSeed.Loaded.val = false;
                CurLevelSeed.Release();
            }


            //FirstLevelHasLoaded = true;

            CurLevelSeed = data;
            Tools.CurGameData = data.MyGame;
            Tools.CurLevel = data.MyGame.MyLevel;

            // Set end of game function
            Tools.CurGameData.EndGame = this.Finish;

            // Make Level Title
            //Tools.CurGameData.AddGameObject(new LevelTitle(String.Format("Level {0}", CurLevelIndex + 1)));

            // Add the saved objects
            foreach (GameObject obj in ObjectsToSave)
                Tools.CurGameData.AddGameObject(obj);

            // Additional processing
            AdditionalSwapToLevelProcessing(Tools.CurGameData);

            if (OnSwapToLevel != null) OnSwapToLevel(CurLevelIndex);

            // Burn one frame
            Tools.CurGameData.MyLevel.PhsxStep(true);
        }

        public virtual void AdditionalSwapToLevelProcessing(GameData game)
        {
        }


        public void Load(int Index)
        {
            if (Index >= LevelSeeds.Count) return;

            GameData.LockLevelStart = false;
            LevelSeedData data = LevelSeeds[Index];

            if (data.LoadingBegun) return;

            if (OnBeginLoad != null)
                OnBeginLoad();
            GameData.StartLevel(data, true);
        }

        public bool EndLoadingImmediately = false;

        Level LastLevelSeedSet = null;
        public override void BackgroundPhsx()
        {
            if (SkipBackgroundPhsx) return;

            LevelSeedData data = LevelSeeds[CurLevelIndex];
            if ((Tools.ShowLoadingScreen || EndLoadingImmediately) && Tools.CurGameData != data.MyGame)
            {
                // If the level is finished loading, end the loading screen
                lock (data.Loaded)
                {
                    if (data.Loaded.val)
                    {
                        if (EndLoadingImmediately)
                            Tools.EndLoadingScreen_Immediate();
                        else
                            Tools.EndLoadingScreen();
                        SetLevel(CurLevelIndex);
                    }
                }
            }
            else
            {
                // Begin loading next level
                if (CurLevelIndex + 1 < LevelSeeds.Count && FirstLevelHasLoaded)
                {
                    //if (!LevelSeeds[CurLevelIndex + 1].LoadingBegun)
                    Load(CurLevelIndex + 1);

                    // Set current level's next level
                    if (LastLevelSeedSet != Tools.CurLevel)
                    {
                        LastLevelSeedSet = Tools.CurLevel;

                        ILevelConnector connector = (ILevelConnector)Tools.CurLevel.FindIObject(LevelConnector.EndOfLevelCode);
                        if (connector != null)
                            connector.NextLevelSeedData = LevelSeeds[CurLevelIndex + 1];
                    }
                }

                //if (Tools.CurLevel.LevelCleared && Tools.CurLevel.Circle.FinishedCount > 20 && Tools.CurLevel.CurPhsxStep > 120 && CurLevelIndex + 1 < LevelSeeds.Count && LevelSeeds[CurLevelIndex + 1].Loaded.val)
                //  SetLevel(CurLevelIndex + 1);
            }

            
            data = LevelSeeds[CurLevelIndex];

            // If the level is finished loading, end the loading screen
            if (!FirstLevelHasLoaded)
            {
                lock (data.Loaded)
                {
                    if (data.Loaded.val)
                    {
                        //Tools.StartPlaylist();
                        //Tools.CurLevel.StartRecording();
                        LevelBegin(Tools.CurLevel);

                        FirstLevelHasLoaded = true;
                    }
                }
            }
        }

        public StringWorldGameData(List<LevelSeedData> LevelSeeds)
        {
            this.LevelSeeds = LevelSeeds;
        }

        public StringWorldGameData()
        {
        }

        public Level MakeLevel()
        {
            Level level = new Level(false);
            level.MainCamera = new Camera();

            return level;
        }

        /// <summary>
        /// The index of the first level played within the list of all LevelSeeds
        /// </summary>
        int StartIndex = 0;

        public override void Init() { Init(0); }
        public virtual void Init(int StartIndex)
        {
            this.StartIndex = StartIndex;

            DrawObjectText = true;

            //Tools.WorldMap = this;
            //Tools.CurGameData = this;

            //Tools.CurLevel =
            MyLevel = MakeLevel();
            MyLevel.MyGame = this;

            AllowQuickJoin = false;

            SuppressQuickSpawn = true;

            if (LevelSeeds == null)
                MakeLevelSeeds();

            //Load(StartIndex);
            SetLevel(StartIndex);

            FadeIn(.011f);
            BlackAlpha = 1.35f;

            base.Init();
        }

        public override void SetToReturnTo(int code)
        {
            base.SetToReturnTo(code);
        }

        public override void ReturnTo(int code)
        {
            base.ReturnTo(code);

            EndGame(false);
        }

        /// <summary>
        /// Attached to the last door of the last level.
        /// </summary>
        public DoorAction EOG_DoorAction = EOG_StandardDoorAction;
        public static void EOG_StandardDoorAction(Door door)
        {
            Tools.CurrentAftermath = new AftermathData();
            Tools.CurrentAftermath.Success = true;

            foreach (Bob bob in Tools.CurLevel.Bobs)
                bob.CollectSelf();

            //Tools.CurGameData.AddGameObject(new VictoryPanel());

            // Absorb game stats
            PlayerManager.AbsorbTempStats();
            PlayerManager.AbsorbLevelStats();
            PlayerManager.AbsorbGameStats();
        }

        /// <summary>
        /// Attached to each door at the end of a level, and used to link that door to the next level in the string.
        /// </summary>
        public void EOL_StringWorldDoorAction(Door door)
        {
            // Make sure that there is another level to go to
            if (CurLevelIndex >= LevelSeeds.Count - 1)
                return;
            else
            {
                GameData game = door.Core.MyLevel.MyGame;
                BaseDoorAction(door);

                // Close the door
                game.AddToDo(() =>
                    {
                        // Close the door
                        door.SetLock(true, false, true);

                        // Absorb level stats
                        PlayerManager.AbsorbTempStats();
                        PlayerManager.AbsorbLevelStats();
                    });

                // Tell the current Game to perform the following
                game.WaitThenAddToToDo(13, () =>
                    {
                        // If the next level is loaded, start the level
                        if (LevelIsLoaded(door.NextLevelSeedData))
                        {
                            SetLevel(door.NextLevelSeedData);
                            LevelBegin(Tools.CurLevel);

                            return true;
                        }
                        // Otherwise wait
                        else
                            return false;
                    });
            }
        }

        public static void BaseDoorAction(Door door)
        {
            GameData game = door.Core.MyLevel.MyGame;

            // Don't do anything if level has ended
            if (door.Core.MyLevel.Finished)
                return;

            // Ensure door isn't reused
            door.OnOpen = null;

            // End this level
            door.Core.MyLevel.EndLevel();

            // Give bonus to completing player
            door.Core.MyLevel.EndOfLevelBonus(door.InteractingBob.MyPlayerData);
        }

        public override void PhsxStep()
        {
            SuppressQuickSpawn = true;
        }


        public override void PostDraw()
        {
            //base.PostDraw();
        }
    }
}