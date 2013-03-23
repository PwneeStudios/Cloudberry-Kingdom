using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.InGameObjects;

#if XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

namespace CloudberryKingdom
{
    public class StringWorldGameData : GameData
    {
        public static new GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return null;
        }

#if XBOX || XBOX_SIGNIN
        public override void OnSignOut(SignedOutEventArgs e)
        {
            // Do nothing. No player to remove, this is a stringworldgame not a normalgame.
            //Tools.CurGameData.RemovePlayer((int)e.Gamer.PlayerIndex);
        }
#endif
        public override void Release()
        {
            if (Released) return;

			if (CurLevelSeed != null)
			{
				if (CurLevelSeed.MyGame != null)
				{
					CurLevelSeed.MyGame.Release();
					CurLevelSeed.Release();
				}
			}

            if (NextLevelSeed != null && NextLevelSeed.Loaded != null)
            {
                if (LevelIsLoaded(NextLevelSeed))
                {
					if (NextLevelSeed.MyGame != null)
					{
						NextLevelSeed.MyGame.Release();
					}
                    NextLevelSeed.Release();
                }
                else
                    NextLevelSeed.ReleaseWhenLoaded = true;
            }
            NextLevelSeed = null;

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

            base.Release();
        }

        public int Count;

        protected Func<int, LevelSeedData> GetSeedFunc;
        protected LevelSeedData NextLevelSeed, CurLevelSeed;

        public int NextLevelIndex = 0, CurLevelIndex = 0;
        public bool FirstLevelHasLoaded = false;
        public bool FirstLevelHasBegun = false;

        /// <summary>
        /// Called when a level begins loading
        /// </summary>
        public Action OnBeginLoad;

        public LevelSeedData GetSeed(int Index)
        {
            if (GetSeedFunc == null)
                return null;
            else
                return GetSeedFunc(Index);
        }

        /// <summary>
        /// Returns whether the StringWorld is ready to switch to the next level
        /// </summary>
        public bool NextIsReady()
        {
            if (NextLevelSeed == null) return false;

            lock (NextLevelSeed.Loaded)
            {
                return NextLevelSeed.Loaded.val;
            }
        }

        /// <summary>
        /// Called during LevelBegin, adds relevant GameObjects to the level's game.
        /// </summary>
        public Func<Level, bool> OnLevelBegin;

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

            if (OnLevelBegin != null)
            {
                bool result = OnLevelBegin(level);
                if (result)
                {
                    return;
                }
            }

            BeginningCloseDoor(level);

            bool Hold_FirstLevelHasBegun = FirstLevelHasBegun;
            if (level.MyGame != null)
            {
                level.MyGame.AddToDo(() =>
                    {
                        if (level.MyGame.PauseGame)
                            return false;

                        if (FirstDoorAction || Hold_FirstLevelHasBegun)
                            StartOfLevelDoorAction(level);

                        // Start music
                        if (StartLevelMusic != null)
                            StartLevelMusic(this);

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
			Tools.SongWad.Shuffle();

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
            if (null == door)
            {

                return;
            }

            // Open the door and show players
            int Wait = WaitLengthToOpenDoor_FirstLevel;

            if (CurLevelIndex > 0 || CurLevelSeed.AlwaysOverrideWaitDoorLength)
                Wait = CurLevelSeed.WaitLengthToOpenDoor;
			game.WaitThenDo(Wait, () => _StartOfLevelDoorAction__OpenAndShow(level, door, CurLevelSeed.OpenDoorSound));
        }

        private void _StartOfLevelDoorAction__OpenAndShow(Level level, Door door, bool OpenDoorSound)
        {
            // Whether to play a sound for the door opening
            bool sound = false;
            if (CurLevelIndex == StartIndex || OpenDoorSound)
                sound = true;

            level.Finished = false;
            door.SetLock(false, false, sound);
            door.ShowBobs();
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

		protected virtual void AdditionalSetLevel()
		{
		}

        /// <summary>
        /// Assuming a level is loaded, set that level as current.
        /// Begin loading next level.
        /// </summary>
        public void SetLevel()
        {
            if (NextLevelSeed.MyGame != null)
            {
                // Check for last level
                if (NextIsLast())
                {
                    ObjectBase obj = NextLevelSeed.MyGame.MyLevel.FindIObject(LevelConnector.EndOfLevelCode);

                    Door door = obj as Door;
                    if (null != door)
                        door.OnOpen = EOG_DoorAction;
                }

                // Replace all Bobs with new Bobs (to handle newly joined players)
				AdditionalSetLevel();
                NextLevelSeed.MyGame.UpdateBobs();
                NextLevelSeed.MyGame.Reset();
            }

            lock (NextLevelSeed.Loaded)
            {
                // If game hasn't loaded yet, bring loading screen
                if (!NextLevelSeed.Loaded.val)
                {
                    if (!NextLevelSeed.LoadingBegun)
                        Tools.Break();

                    if (!Tools.ShowLoadingScreen)
                    {
                        Tools.CurLevel = MyLevel;
                        Tools.CurGameData = this;
                        Tools.BeginLoadingScreen(true);
                    }
                }
                else
                {
                    SwapToLevel();
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

        public bool NextIsLast() { return NextLevelSeed.Name == "Last"; }

        public void SwapToLevel()
        {
            Tools.CurGameData = NextLevelSeed.MyGame;
            Tools.CurLevel = NextLevelSeed.MyGame.MyLevel;

            // Perform actions if this is the first level being swapped in.
            if (!FirstLevelSwappedIn)
            {
                if (OnSwapToFirstLevel != null)
                    OnSwapToFirstLevel(NextLevelSeed);
                FirstLevelSwappedIn = true;
            }

            // Perform actions if this is the last level being swapped in.
            if (NextIsLast())
            {
                if (OnSwapToLastLevel != null)
                    OnSwapToLastLevel(NextLevelSeed);
            }

            // Stores the GameObjects in the current game marked as 'PreventRelease'
            List<GameObject> ObjectsToSave = new List<GameObject>();

            if (CurLevelSeed != null && NextLevelSeed != CurLevelSeed)
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

            CurLevelSeed = NextLevelSeed;
            CurLevelIndex = NextLevelIndex;

            //Tools.CurGameData = CurLevelSeed.MyGame;
            //Tools.CurLevel = CurLevelSeed.MyGame.MyLevel;

            // Set end of game function
            Tools.CurGameData.EndGame = this.Finish;

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
            GameData.LockLevelStart = false;

            NextLevelIndex = Index;
            NextLevelSeed = GetSeed(NextLevelIndex);

            if (NextLevelSeed == null) return;

            if (CurLevelSeed == null) CurLevelSeed = NextLevelSeed;

            if (OnBeginLoad != null)
                OnBeginLoad();

            GameData.StartLevel(NextLevelSeed, true);
        }

        public bool EndLoadingImmediately = false;

        Level LastLevelSeedSet = null;
        public override void BackgroundPhsx()
        {
            if (SkipBackgroundPhsx) return;

            // ActionGames immediately switch to next game when they are done.
            var ActionGame = Tools.CurGameData as ActionGameData;
            if (null != ActionGame && ActionGame.Done)
                TellGameToBringNext(0, ActionGame);

            // The following code handles beginning the loading of new levels.
            if ((Tools.ShowLoadingScreen || EndLoadingImmediately) && Tools.CurGameData != CurLevelSeed.MyGame)
            {
                // If the level is finished loading, end the loading screen
                lock (CurLevelSeed.Loaded)
                {
                    if (CurLevelSeed.Loaded.val)
                    {
                        if (EndLoadingImmediately)
                            Tools.EndLoadingScreen_Immediate();
                        else
                            Tools.EndLoadingScreen();
                        SetLevel();
                    }
                }
            }
            else
            {
                // Begin loading next level
                if (FirstLevelHasLoaded)
                {
                    if (CurLevelSeed == NextLevelSeed)
                    {
                        Load(CurLevelIndex + 1);

                        // Set current level's next level
                        if (LastLevelSeedSet != Tools.CurLevel)
                        {
                            LastLevelSeedSet = Tools.CurLevel;

                            ILevelConnector connector = (ILevelConnector)Tools.CurLevel.FindIObject(LevelConnector.EndOfLevelCode);
                            if (connector != null)
                                connector.NextLevelSeedData = NextLevelSeed;
                        }
                    }
                }
            }
            
            // If the level is finished loading, end the loading screen
            if (!FirstLevelHasLoaded)
            {
                lock (CurLevelSeed.Loaded)
                {
                    if (CurLevelSeed.Loaded.val)
                    {
                        LevelBegin(Tools.CurLevel);

                        FirstLevelHasLoaded = true;
                    }
                }
            }
        }

        public StringWorldGameData()
        {
        }

        public StringWorldGameData(Func<int, LevelSeedData> GetSeed)
        {
            this.GetSeedFunc = GetSeed;
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

            MyLevel = MakeLevel();
            MyLevel.MyGame = this;

            AllowQuickJoin = false;

            SuppressQuickSpawn = true;

            Load(StartIndex);
            SetLevel();

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
            if (NextLevelSeed == null)
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

                if (door.OnEnter != null) door.OnEnter(door);
            }
        }

        /// <summary>
        /// Attached to each door at the end of a level, executed after the door is closed.
        /// This sets the next level to be switched to.
        /// </summary>
        public void EOL_StringWorldDoorEndAction(Door door)
        {
            GameData game = door.Core.MyLevel.MyGame;

            // Tell the current Game to perform the following
            TellGameToBringNext(13, game);
        }

        /// <summary>
        /// Attached to each door at the end of a level, executed after the door is closed.
        /// This sets the next level to be switched to, but first fades the current level to black.
        /// </summary>
        public void EOL_StringWorldDoorEndAction_WithFade(Door door)
        {
            GameData game = door.Core.MyLevel.MyGame;

            Tools.SongWad.FadeOut();
            float fadespeed = door.MyLevel.MyLevelSeed == null ? .02f : door.MyLevel.MyLevelSeed.FadeOutSpeed;
            game.FadeToBlack(fadespeed, 47);

            // Tell the current Game to perform the following
            //TellGameToBringNext(98, game);
            //TellGameToBringNext(165, game);
            TellGameToBringNext(115 + (int)(1f / fadespeed), game);
        }

        bool WaitingForNext = false;
        private void TellGameToBringNext(int delay, GameData game)
        {
            if (WaitingForNext) return;

            WaitingForNext = true;

            game.WaitThenAddToToDo(delay, () =>
            {
                // If the next level is loaded, start the level
                if (NextIsReady())
                {
                    WaitingForNext = false;

                    SetLevel();
                    LevelBegin(Tools.CurLevel);

                    return true;
                }
                // Otherwise wait
                else
                    return false;
            });
        }

        public static void BaseDoorAction(Door door)
        {
            GameData game = door.Core.MyLevel.MyGame;

            game.CompleteLevelEvent();

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