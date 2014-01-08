using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    /// <summary>
    /// A GameFactory takes in seed information (a LevelSeedData instance), and creates an actual game.
    /// </summary>
    public delegate GameData GameFactory(LevelSeedData data, bool MakeInBackground);
    public delegate GameData SimpleGameFactory();

    public struct GameFlags
    {
        /// <summary>
        /// Whether the game has the players tethered
        /// </summary>
        public bool IsTethered;

        /// <summary>
        /// Whether the game is a Doppleganger Game
        /// </summary>
        public bool IsDoppleganger;

        /// <summary>
        /// Whether the Doppleganger Game is inverting the doppleganger
        /// </summary>
        public bool IsDopplegangerInvert;

        public void SetToDefault()
        {
            IsTethered = IsDoppleganger = IsDopplegangerInvert = false;
        }
    }

    /// <summary>
    /// This is the main super-class of the engine.
    /// Whenever a player is playing, they are in a 'Game'.
    /// A Game consists of at least one level (a Level class instance),
    /// as well as some goal or purpose: getting to a door, collecting coins, surviving, or building a level, etc.
    /// The game class holds the levels and purpose of the level.
    /// Different purposes are implemented as children classes of the main GameData class.
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// Dictionary to get a game factory from a string.
        /// </summary>
        public static Dictionary<string, GameFactory> FactoryDict = new Dictionary<string, GameFactory> { 
            { "normal", NormalGameData.Factory } };

        /// <summary>
        /// Used in the Background Editor to assign GUID to objects.
        /// </summary>
        public static int DataCounter = 0;
        public int MyDataNumber = DataCounter++;

        /// <summary>
        /// Whether the game has lava.
        /// </summary>
        public bool HasLava = false;

        protected void KillThread(object sender, System.EventArgs e)
        {
            Thread MakeThread = Thread.CurrentThread;
            if (MakeThread != null)
            {
                MakeThread.Abort();
            }
        }

        /// <summary>
        /// True when a single player has successfully navigated the level.
        /// </summary>
        public bool HasBeenCompleted = false;

        /// <summary>
        /// Function to be called that makes the game's score screen.
        /// </summary>
        public Func<GameObject> MakeScore;

        /// <summary>
        /// The statistics associated with this game.
        /// A standaone game (the default) has stats only associated with its specific level.
        /// A campaign game may have stats that spans multiple games.
        /// Likewise an arcade game (comprised of many levels) can have stats that span many levels.
        /// </summary>
        public StatGroup MyStatGroup = StatGroup.Level;

        /// <summary>
        /// Action to take when the player exits the final door of the level.
        /// </summary>
        /// <param name="door"></param>
        public static void EOL_DoorAction(Door door)
        {
            StatGroup group = door.Game.MyStatGroup;
            GameData game = door.CoreData.MyLevel.MyGame;

            game.HasBeenCompleted = true;

            // Give bonus to completing player
            door.CoreData.MyLevel.EndOfLevelBonus(door.InteractingBob.MyPlayerData);
                      
            // Close the door
            door.SetLock(true, false, true);

            // Aftermath
            Tools.CurrentAftermath = new AftermathData();
            Tools.CurrentAftermath.Success = true;

            // Ensure door isn't reused
            door.OnOpen = null;

            // End this level
            door.CoreData.MyLevel.EndLevel();

            // Calculate level stats
            PlayerManager.AbsorbTempStats();
            if (group == StatGroup.Game)
                PlayerManager.AbsorbLevelStats();

            // Hide the player
            door.InteractingBob.CoreData.Show = false;

            // Add the score
            ExplodeBobs explode = new ExplodeBobs(ExplodeBobs.Speed.Regular);
            door.CoreData.MyLevel.MyGame.AddGameObject(explode);
            explode.OnDone = () =>
            {
                if (game.MakeScore == null) return;

                GameObject ScoreObj = game.MakeScore();
                if (ScoreObj == null) return;
                door.CoreData.MyLevel.MyGame.AddGameObject(ScoreObj);

                // Absorb game stats
                PlayerManager.AbsorbLevelStats();
                PlayerManager.AbsorbGameStats();
            };
        }

        /// <summary>
        /// Recycle bin for this game.
        /// </summary>
        public Recycler Recycle;

        /// <summary>
        /// Whether this game was launched from Freeplay.
        /// </summary>
        public bool Freeplay = false;

        /// <summary>
        /// The GameData that created this game.
        /// </summary>
        public GameData ParentGame = null;
        
        /// <summary>
        /// Called to end the game and return to parent game.
        /// Return true if the game should be replayed.
        /// </summary>
        public Action<bool> EndGame;

        /// <summary>
        /// Call to finish the metagame and return to the game that created it.
        /// </summary>
        public virtual void Finish(bool Replay)
        {
            StandardFinish(Replay);

            // Start the world map music
			//Tools.PlayHappyMusic(ParentGame, 50);
		}

        public bool SetToRepaly = false;
        public bool EndMusicOnFinish = true;
        public virtual void StandardFinish(bool Replay)
        {
            // Stats
            PlayerManager.CleanTempStats();
            PlayerManager.AbsorbLevelStats();
            PlayerManager.AbsorbGameStats();

            // Switch to this game (may have been in a sub-game)
            Tools.CurLevel = MyLevel;
            Tools.CurGameData = this;

            // Fade the music
            if (EndMusicOnFinish)
                Tools.SongWad.FadeOut();

            // Return to the parent game
            ParentGame.SetToReturnTo(0);

            // Release this game
            ParentGame.AddToDo(() => this.Release());

            // Check if we should replay the current game
            if (Replay)
            {
                GameData parentgame = ParentGame;
                ParentGame.AddToDo(() => parentgame.PlayAgain());
            }

            // Save everything
            ParentGame.PhsxStepsToDo = 3;
            SaveGroup.SaveAll();
        }

        /// <summary>
        /// The previous action fed to PlayGame (typically a call to load a game)
        /// </summary>
        Action PreviousLoadFunction = null;

        public void ClearPreviousLoadFunction()
        {
            PreviousLoadFunction = null;
        }

        /// <summary>
        /// Does the specified action and saves the action for the ability to replay it later.
        /// </summary>
        public void PlayGame(Action LoadFunction)
        {
            PreviousLoadFunction = LoadFunction;

            if (LoadFunction != null) LoadFunction();
        }

        /// <summary>
        /// If the game has called another game, then this function calls it again.
        /// </summary>
        public void PlayAgain()
        {
            ExecutingPreviousLoadFunction = true;
            if (PreviousLoadFunction != null)
                PreviousLoadFunction();
            ExecutingPreviousLoadFunction = false;

            SetToRepaly = false;
        }
        public bool ExecutingPreviousLoadFunction = false;

        /// <summary>
        /// Add an item to the ToDo list that is executed after a delay.
        /// </summary>
        /// <param name="WaitLength">Number of frames to wait.</param>
        /// <param name="Func">Function to execute.</param>
        public void WaitThenDo(int WaitLength, Action f)
        {
            WaitThenDo(WaitLength, f, "");
        }
        
        public void WaitThenDo(int WaitLength, Action f, string Name)
        {
            WaitThenDo(WaitLength, f, Name, false, false);
        }
        
        public void WaitThenDo(int WaitLength, Action f, bool PauseOnPause)
        {
            WaitThenDo(WaitLength, f, "", PauseOnPause, false);
        }
        public void WaitThenDo_Pausable(int WaitLength, Action f)
        {
            WaitThenDo(WaitLength, f, "", true, false);
        }
        
        public void CinematicToDo(int WaitLength, Action f)
        {
            WaitThenDo(WaitLength, f, "", true, true);
        }
        public void CinematicToDo(Func<bool> f)
        {
            AddToDo(f, "", true, true);
        }
        public void CinematicToDo(int WaitLength, Func<bool> f)
        {
            CinematicToDo(WaitLength, () => CinematicToDo(f));
        }
        public void WaitThenDo(int WaitLength, Action f, string Name, bool PauseOnPause, bool RemoveOnReset)
        {
            if (WaitLength < 0)
                return;

            int Count = 0;

            AddToDo(() =>
            {
                if (Count >= WaitLength)
                {
                    f();
                    return true;
                }
                else
                {
                    Count++;
                    return false;
                }
            }, Name, PauseOnPause, RemoveOnReset);
        }
        public void WaitThenAddToToDo(int WaitLength, Func<bool> f)
        {
            // Create a function that after the specified time will add f to the ToDo list
            WaitThenDo(WaitLength, () => AddToDo(f));
        }

        public GameFlags MyGameFlags;

        public bool ShowHelpNotes = true, FadeToBlackBeforeReturn = false, FadingToReturn = false;

        public bool Loading;
        
        /// <summary>
        /// If true the player can not quick spawn.
        /// Use the External bool if suppressing quickspawn from outside the game class.
        /// </summary>
        public bool SuppressQuickSpawn, SuppressQuickSpawn_External;

        /// <summary>
        /// Whether quickspawn is enabled.
        /// </summary>
        public bool QuickSpawnEnabled()
        {
            return !SuppressQuickSpawn && !SuppressQuickSpawn_External;
        }

        public bool AllowQuickJoin = false;

        public bool DrawObjectText = false;

        public int CurPlayer;

        /// <summary>
        /// When true song info will not appear when a new song starts.
        /// </summary>
        public bool SuppressSongInfo = false;

        /// <summary>
        /// Main camera of this game.
        /// </summary>
        public Camera Cam
        {
            get
            {
                if (MyLevel == null) return null;
                return MyLevel.MainCamera;
            }
        }

        /// <summary>
        /// Random number generator for this game.
        /// All numbers generated in this game should come from this generator.
        /// </summary>
        public Rand Rnd { get { return MyLevel.Rnd; } }        

        /// <summary>
        /// The level immediately associated with this game.
        /// Note: some games have multiple levels associated to them via a list.
        /// Nonetheless, at all times, every game needs to have at least one level specifically singled out as it's actual level.
        /// </summary>
        public Level MyLevel;

        /// <summary>
        /// The position of the main camera.
        /// </summary>
        public Vector2 CamPos
        {
            get
            {
                if (MyLevel == null)
                    return Vector2.Zero;
                else
                    return MyLevel.MainCamera.Data.Position;
            }
        }

        QuadClass BlackQuad;
        BasePoint BlackBase;
        protected float BlackAlpha, FadeOutSpeed,FadeInSpeed;
        protected bool FadingToBlack, FadingIn;
        public FancyColor FadeColor;

        public bool IsFading()
        {
            return FadingToBlack || FadingIn;
        }

        /// <summary>
        /// Fade partially to black, do some action, then fade back in.
        /// </summary>
        public void PartialFade_InAndOut(int Delay, float TargetOpaqueness, int FadeOutLength, int FadeInLength, Action OnBlack)
        {
            // Wait then screen partially fade to black.
            WaitThenDo(Delay, () => FadeToBlack(TargetOpaqueness / FadeOutLength));

            // Wait for the apex of blackness, trigger the action and fade back in.
            WaitThenDo(Delay + FadeOutLength,
                () =>
                {
                    // Fade in and do action.
                    FadeIn(TargetOpaqueness / FadeOutLength);
                    BlackAlpha = TargetOpaqueness;
                    if (OnBlack != null) OnBlack();
                });
        }

        /// <summary>
        /// Transition to a black screen via a right-to-left screen swipe, then fade back in.
        /// When the screen is completely dark, just before fading back in, the OnBlack action is called.
        /// </summary>
        public void SlideOut_FadeIn(int Delay, Action OnBlack)
        {
            var black = new StartMenu_MW_Black();
            AddGameObject(black);

            // Wait then screen swipe to black.
            WaitThenDo(Delay, black.SlideFromRight, "SlideOut_FadeIn");

            // Wait for screen to be completely black, then fade in.
            WaitThenDo(Delay + 17,
                () =>
                {
                    // Get rid of black screen swipe.
                    black.MyPile.Alpha = 0; black.CollectSelf();

                    // Fade in and do action.
                    FadeIn(.025f);
                    if (OnBlack != null) OnBlack();
                }, "SlideOut_FadeIn");
        }

        public Door CurDoor;

        /// <summary>
        /// A collection of objects in the game that are not in the level, such as GUIs.
        /// </summary>
        public List<GameObject> MyGameObjects = new List<GameObject>(), NewGameObjects = new List<GameObject>();

        public void ReInitGameObjects()
        {
            Tools.StartGUIDraw_Fake();

			foreach (GameObject obj in MyGameObjects)
			{
				if (obj is SoundMenu) continue;

				obj.Init();
			}

            Tools.EndGUIDraw_Fake();
        }

        /// <summary>
        /// Remove all GameObjects with a specified tag.
        /// </summary>
        public void RemoveGameObjects(GameObject.Tag tag)
        {
            foreach (GameObject obj in MyGameObjects)
                if (obj.Tags[tag])
                    obj.Release();
        }

        /// <summary>
        /// Remove all game objects satisfying a restraint
        /// </summary>
        public void RemoveAllGameObjects(Func<ObjectBase, bool> func)
        {
            foreach (GameObject obj in MyGameObjects)
                if (func(obj))
                    obj.Release();
        }

        /// <summary>
        /// Remove all game objects satisfying a restraint, even if those objects request not to be removed.
        /// </summary>
        public void SudoRemoveAllGameObjects(Func<ObjectBase, bool> func)
        {
            foreach (GameObject obj in MyGameObjects)
                if (func(obj))
                {
                    obj.PreventRelease = false;
                    obj.Release();
                }
        }

        /// <summary>
        /// If true coins can be taken only once and are not respawned on a level reset.
        /// </summary>
        public bool TakeOnce = false;

        /// <summary>
        /// If true the coins score is permanent even if the coin respawns.
        /// </summary>
        public bool AlwaysGiveCoinScore = false;

        /// <summary>
        /// The score of coins collected are multiplied by this value.
        /// </summary>
        public float CoinScoreMultiplier = 1;
        /// <summary>
        /// Event handler. Activates when this game recalculates it's coin score multiplier.
        /// The multiplier is first reset to 1, then each registered callback can modify it.
        /// </summary>
        public event Action<GameData> OnCalculateCoinScoreMultiplier;
        /// <summary>
        /// Called at the beginning over every time step to calculate the coin score multiplier
        /// </summary>
        public void CalculateCoinScoreMultiplier()
        {
            CoinScoreMultiplier = 1;
            if (OnCalculateCoinScoreMultiplier != null)
                OnCalculateCoinScoreMultiplier(this);
        }

        /// <summary>
        /// Values added to a player's score are multiplied by this value.
        /// </summary>
        public float ScoreMultiplier = 1;
        /// <summary>
        /// Event handler. Activates when this game recalculates it's score multiplier.
        /// The multiplier is first reset to 1, then each registered callback can modify it.
        /// </summary>
        public event Action<GameData> OnCalculateScoreMultiplier;
        /// <summary>
        /// Called at the beginning over every time step to calculate the score multiplier
        /// </summary>
        void CalculateScoreMultiplier()
        {
            ScoreMultiplier = 1;
            if (OnCalculateScoreMultiplier != null)
                OnCalculateScoreMultiplier(this);

			foreach (GameObject obj in MyGameObjects)
			{
				PerfectScoreObject pso = obj as PerfectScoreObject;
				if (null != pso)
				{
					pso.UpdateScoreText();
				}
			}
        }

        /// <summary>
        /// Event handler. Activates when a Checkpoint is grabbed. Argument is the IObject that is a Checkpoint.
        /// </summary>
        public event Action<ObjectBase> OnCheckpointGrab;
        /// <summary>
        /// Call this when a Checkpoint is grabbed to activate the Checkpoint grabbed event handler.
        /// </summary>
        public void CheckpointGrabEvent(ObjectBase Checkpoint) { if (OnCheckpointGrab != null) OnCheckpointGrab(Checkpoint); }

        /// <summary>
        /// Event handler. Activates when a coin is grabbed. Argument is the IObject that is a coin.
        /// </summary>
        public event Action<ObjectBase> OnCoinGrab;
        /// <summary>
        /// Call this when a coin is grabbed to activate the coin grabbed event handler.
        /// </summary>
        public void CoinGrabEvent(ObjectBase coin) { if (OnCoinGrab != null) OnCoinGrab(coin); }

        /// <summary>
        /// Event handler. Activates when a level is completed.
        /// </summary>
        public event Action<Level> OnCompleteLevel;
        /// <summary>
        /// Call this when level is completed to activate the level complete event handler.
        /// </summary>
        public void CompleteLevelEvent()
        {
            HasBeenCompleted = true;
            if (OnCompleteLevel != null)
                OnCompleteLevel(MyLevel);
        }

        /// <summary>
        /// Event handler. Activates when all players die and the level is reset.
        /// </summary>
        public event Action OnLevelRetry;
        /// <summary>
        /// Call this when a coin is grabbed to activate the coin grabbed event handler.
        /// </summary>
        public void LevelRetryEvent() { if (OnLevelRetry != null) OnLevelRetry(); }

		/// <summary>
		/// Event handler. Activates when the player chooses to watch the computer.
		/// </summary>
		public event Action OnWatchComputer;
		/// <summary>
		/// Call this when a coin is grabbed to activate the coin grabbed event handler.
		/// </summary>
		public void WatchComputerEvent() { if (OnWatchComputer != null) OnWatchComputer(); }

        /// <summary>
        /// Event handler. Activates when this game is returned to from another game.
        /// </summary>
        public event Action OnReturnTo;
        public event Action OnReturnTo_OneOff;
        /// <summary>
        /// Call this when the game is returned to.
        /// </summary>
        public void ReturnToEvent()
        {
            if (OnReturnTo != null) OnReturnTo();
            if (OnReturnTo_OneOff != null) OnReturnTo_OneOff(); OnReturnTo_OneOff = null;
        }

        /// <summary>
        /// Add a nameless function to the to do list.
        /// The function should return true if it wishes to be removed from the queue after execution.
        /// </summary>
        public void AddToDo(Func<bool> FuncToDo)
        {
            ToDo.Add(new ToDoItem(FuncToDo, "", false, false));
        }

        /// <summary>
        /// Add a nameless function to the to do list.
        /// </summary>
        public void AddToDo(Action FuncToDo)
        {
            AddToDo(FuncToDo, "", false, false);
        }

        /// <summary>
        /// Add a named function to the to do list.
        /// The function should return true if it wishes to be removed from the queue after execution.
        /// </summary>
        public void AddToDo(Func<bool> FuncToDo, string name, bool PauseOnPause, bool RemoveOnReset)
        {
            ToDo.Add(new ToDoItem(FuncToDo, name, PauseOnPause, RemoveOnReset));
        }

        public void AddToDo(Action FuncToDo, string name, bool PauseOnPause, bool RemoveOnReset)
        {
            ToDo.Add(new ToDoItem(() => { FuncToDo(); return true; }, name, PauseOnPause, RemoveOnReset));
        }

        public List<ToDoItem> ToDo
        {
            get
            {
                return CurToDo;
            }
        }

        public List<Action> ToDoOnReset = new List<Action>();
        void DoToDoOnResetList()
        {
            List<Action> list = new List<Action>(ToDoOnReset);
            ToDoOnReset.Clear();

            foreach (Action f in list)
                f();

            list.Clear();
        }

        public static int CurItemStep;
        void DoToDoList()
        {
            if (CurToDo.Count > 0)
            {
                DoingToDoList = true;
                NextToDo.Clear();
                NextToDo.AddRange(CurToDo);
                CurToDo.Clear();
                foreach (ToDoItem item in NextToDo)
                {
                    // Skip deleted items
                    if (item.MarkedForDeletion)
                        continue;

                    bool Keep = true;
                    if (!(PauseGame && item.PauseOnPause))
                    {
                        // Execute the function
                        CurItemStep = item.Step;
                        Keep = !item.MyFunc();
                        item.Step++;
                    }

                    if (Keep)
                    {
                        // Keep the function if it returned false
                        if (CurToDo != null)
                            CurToDo.Add(item);
                        else
                            return;
                    }
                }
                DoingToDoList = false;
            }
        }

        /// <summary>
        /// Remove all ToDo items with the given name
        /// </summary>
        public void KillToDo(string name)
        {
            ToDoFindAll(name).ForEach(item => item.Delete());
        }

        /// <summary>
        /// Find all ToDo items with the given name
        /// </summary>
        public List<ToDoItem> ToDoFindAll(string name)
        {
            return ToDo.FindAll(match => string.Compare(match.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        bool DoingToDoList = false;

        /// <summary>
        /// Two lists of to do functions, used to allow for the actual ToDo list to have functions that add functions to the list of things to do.
        /// </summary>
        List<ToDoItem> NextToDo;
        List<ToDoItem> CurToDo;

        public BobPhsx DefaultHeroType = BobPhsxNormal.Instance;

        public bool Released = false;

        /// <summary>
        /// Clean up the game, remove all connections to everything to ensure proper garbage collection.
        /// </summary>
        public virtual void Release()
        {
            if (Released) return;

            Released = true;

            DefaultHeroType = null;

            ParentGame = null;
            EndGame = null;

            if (MyLevel != null) MyLevel.Release(); MyLevel = null;

            foreach (GameObject obj in MyGameObjects)
                obj.Release();
            NewGameObjects = MyGameObjects = null;

            if (FadeColor != null)
                FadeColor.Release();

            if (BlackQuad != null) BlackQuad.Release(); BlackQuad = null;
            if (CurDoor != null) CurDoor.Release();

            PreviousLoadFunction = null;
            OnCalculateCoinScoreMultiplier = null;
	        OnCalculateScoreMultiplier = null;
            OnCheckpointGrab = null;
	        OnCoinGrab = null;
            OnLevelRetry = null;
			OnWatchComputer = null;
            OnReturnTo = null;

            CurToDo = null; NextToDo = null;
            ToDoOnReset = null;
            ToDoOnDeath = null;
            ToDoOnDoneDying = null;

            Recycler.ReturnRecycler(Recycle);
            Recycle = null;
        }

        public static GameData Factory(LevelSeedData data, bool MakeInBackground) { return null; }

        public enum BankType { Campaign, Escalation, Infinite };
        public BankType MyBankType = BankType.Infinite;

        public int CreationTime = 0;
        public GameData()
        {
#if DEBUG_OBJDATA
            ObjectData.weakg.Add(new WeakReference(this));
#endif

            CreationTime = Tools.TheGame.DrawCount;

            Recycle = Recycler.GetRecycler();

            EndGame = this.Finish;

            Loading = false;

            CurToDo = new List<ToDoItem>();
            NextToDo = new List<ToDoItem>();
        }

        bool GameObjectsAreLocked = false;
        /// <summary>
        /// Lock or unlock the GameObject list
        /// </summary>
        /// <param name="Lock"></param>
        void LockGameObjects(bool Lock)
        {
            // If unlocking add new GameObjects
            if (!Lock)
            {
                MyGameObjects.AddRange(NewGameObjects);
                NewGameObjects.Clear();
            }

            GameObjectsAreLocked = Lock;
        }

        public void AddGameObject(params GameObject[] list)
        {
            foreach (GameObject obj in list)
                AddGameObject(obj);
        }

        public void AddGameObject(GameObject obj)
        {
            if (GameObjectsAreLocked)
                NewGameObjects.Add(obj);
            else
                MyGameObjects.Add(obj);

            if (MyLevel != null)
                MyLevel.AddObject(obj);

            obj.MyGame = this;

            obj.OnAdd();
        }

        public bool SkipBackgroundPhsx = false;
        public virtual void BackgroundPhsx() { }

        int SetToReturnToCode;

        bool _IsSetToReturnTo;
        bool IsSetToReturnTo
        {
            get { return _IsSetToReturnTo; }
            set { _IsSetToReturnTo = value; }
        }
        protected GameData PrevGame;
        public virtual void SetToReturnTo(int code)
        {
            if (IsSetToReturnTo)
                return;

            IsSetToReturnTo = true;
            SetToReturnToCode = code;

            PrevGame = Tools.CurGameData;

            Tools.CurGameData = this;
            Tools.CurLevel = MyLevel;
        }

        public static bool LockLevelStart = false;
        public bool ClearToDoOnReturnTo = true;
        public virtual void ReturnTo(int code)
        {
            // Clear todo list
            if (ClearToDoOnReturnTo)
                ToDo.Clear();

            // Remove players that have left
            if (MyLevel != null && MyLevel.Bobs != null)
                MyLevel.Bobs.RemoveAll(bob => !PlayerManager.Get((int)bob.MyPlayerIndex).Exists);

            // Create new players
            for (int i = 0; i < 4; i++)
                if (PlayerManager.Get(i).Exists && MyLevel.Bobs.All(bob => (int)bob.MyPlayerIndex != i))
                    CreateBob(i, false);

            // Revive all players
            ReviveAll();

            CleanLastLevel();

            // Fire event
            ReturnToEvent();
        }

        public void ReviveAll()
        {
            foreach (Bob bob in MyLevel.Bobs)
                PlayerManager.RevivePlayer(bob.MyPlayerIndex);
        }

        public void CleanLastLevel()
        {
            Level PrevLevel;

            if (!IsSetToReturnTo)
                PrevLevel = Tools.CurLevel;
            else
                PrevLevel = PrevGame.MyLevel;

            IsSetToReturnTo = false;

			if (PrevLevel != null)
			{
				if (PrevLevel.MyLevelSeed != null)
				{
					PrevLevel.MyLevelSeed.Release();
					PrevLevel.MyLevelSeed = null;
				}
				PrevLevel.Release();
			}
			else Tools.Nothing();

            Tools.PhsxSpeed = 1;
            LockLevelStart = false;
        }

        public virtual void Reset()
        {
            if (MyLevel != null)
                MyLevel.ResetAll(false);
        }

        public virtual void RevertCheckpoints()
        {
            foreach (ObjectBase obj in MyLevel.Objects)
            {
                Checkpoint checkpoint = obj as Checkpoint;
                if (null != checkpoint)
                    checkpoint.Revert();
            }
        }

        public virtual void RevertLevel()
        {
            // Revive the checkpoints
            RevertCheckpoints();

            // Start at the first level piece
            MyLevel.SetCurrentPiece(0);
        }

        public virtual void GotCheckpoint(Bob CheckpointBob)
        {
			// Don't let this last recording be sparse (since it is the 'real' bob)
			if (MyLevel.Recording)
			{
				for (int i = 0; i < MyLevel.CurrentRecording.Recordings.Length; i++)
				{
					MyLevel.CurrentRecording.Recordings[i].CanBeSparse = false;
				}
			}

            MyLevel.PieceAttempts = 0;

            foreach (Bob bob in MyLevel.Bobs)
            {
                if (!PlayerManager.IsAlive(bob.MyPlayerIndex))
                {
                    PlayerManager.RevivePlayer(bob.MyPlayerIndex);
                    bob.Dead = bob.Dying = false;
                    bob.DeadCount = 0;

                    bob.Init(false, bob.MyPiece.StartData[bob.MyPieceIndex], this);
                    bob.Move(CheckpointBob.CoreData.Data.Position - bob.CoreData.Data.Position);

                    ParticleEffects.AddPop(MyLevel, bob.CoreData.Data.Position, 155);
                }
            }

            PlayerManager.AbsorbTempStats();
        }

        /// <summary>
        /// When set to true the next reset doesn't count against the player's life counter.
        /// </summary>
        public bool FreeReset = false;

        /// <summary>
        /// Called by the level when finishing up it's reset routine.
        /// </summary>
        public virtual void AdditionalReset()
        {
            if (MyLevel != null && MyLevel.LevelReleased)
                return;

            // Remove marked todo items
            ToDo.ForEach(todo => { if (todo.RemoveOnReset) todo.MarkedForDeletion = true; });
            NextToDo.ForEach(todo => { if (todo.RemoveOnReset) todo.MarkedForDeletion = true; });

            // Perform additional actions
            DoToDoOnResetList();

            FreeReset = false;

            // Revive all players
            foreach (Bob bob in MyLevel.Bobs)
                PlayerManager.RevivePlayer(bob.MyPlayerIndex);

            // Clear the temporary stats
            PlayerManager.CleanTempStats();
        }

        public virtual void RemovePlayer(int PlayerIndex)
        {
            PlayerManager.Get(PlayerIndex).Exists = PlayerManager.Get(PlayerIndex).IsAlive = false;

            if (MyLevel != null && MyLevel.Bobs != null)
                MyLevel.Bobs.RemoveAll(bob =>
                {
                    if (!PlayerManager.Get((int)bob.MyPlayerIndex).Exists)
                    {
                        ParticleEffects.AddPop(MyLevel, bob.CoreData.Data.Position);
                        Tools.SoundWad.FindByName("Pop_2").Play();

                        return true;
                    }
                    else
                        return false;
                });

			SetAdditionalBobParameters(MyLevel.Bobs);

            if (PlayerManager.AllDead() && !MyLevel.PreventReset)
                MyLevel.ResetAll(false);
        }

        public virtual void SetCreatedBobParameters(Bob bob)
        {
            bob.Immortal = true;
            bob.ScreenWrap = true;
            bob.ScreenWrapToCenter = false;
        }

        public virtual void CreateBob(int i, bool Pop)
        {
            Bob TemplateBob = null;
            if (MyLevel.Bobs.Count > 0)
                TemplateBob = MyLevel.Bobs[0];

            PlayerManager.Get(i).IsAlive = PlayerManager.Get(i).Exists = true;

            //Bob Player = new Bob(Prototypes.bob[MyLevel.DefaultHeroType], false);
            Bob Player = new Bob(MyLevel.DefaultHeroType, false);

            Player.MyPlayerIndex = PlayerManager.Get(i).MyPlayerIndex;
            MyLevel.AddBob(Player);

            if (TemplateBob != null)
                Player.CanInteract = TemplateBob.CanInteract;

            Player.SetColorScheme(PlayerManager.Get(i).ColorScheme);

            PlayerManager.RevivePlayer(Player.MyPlayerIndex);

            SetCreatedBobParameters(Player);

#if INCLUDE_EDITOR
            Player.Immortal = true;            
#endif

            Player.MyPiece = MyLevel.CurPiece;

            int _i = Math.Min(i, MyLevel.CurPiece.StartData.Length - 1);
            PhsxData StartData = MyLevel.CurPiece.StartData[_i];
            Player.Init(false, MyLevel.CurPiece.StartData[0], this);
            Bob TargetBob = MyLevel.Bobs.Find(delegate(Bob bob) { return bob != Player && !bob.Dying; });
            if (TargetBob != null)
                Player.Move(new Vector2(20, 450) + TargetBob.CoreData.Data.Position - Player.CoreData.Data.Position);
            else
                Player.Move(new Vector2(0, 750) + MyLevel.MainCamera.Data.Position - Player.CoreData.Data.Position);

            if (Pop)
            {
                ParticleEffects.AddPop(MyLevel, Player.CoreData.Data.Position);
                Tools.SoundWad.FindByName("Pop_2").Play();
            }
        }

        public virtual void UpdateBobs()
        {
            foreach (Bob bob in MyLevel.Bobs)
                bob.Release();
            MyLevel.Bobs.Clear();

            MakeBobs(MyLevel);
            SetAdditionalBobParameters(MyLevel.Bobs);
        }

        public void QuickJoinPhsx()
        {
            if (CharacterSelectManager.FakeHide) return;
            if (MyLevel == null) return;
            if (MyLevel.Watching || MyLevel.Replay) return;
            if (PauseGame) return;
            if (PauseLevel) return;

            // Check for non-playing controllers pressing A
            for (int i = 0; i < 4; i++)
                if (!PlayerManager.Get(i).Exists &&
                    ButtonCheck.State(ControllerButtons.A, i).Pressed)
                {
                    CharacterSelectManager.Start(null, true);
                }
        }

#if NOT_PC && (XBOX || XBOX_SIGNIN)
        void UpdateSignedInPlayers()
        {
            if (CharacterSelectManager.IsShowing) return;
            if (MyLevel == null) return;
            if (MyLevel.Watching || MyLevel.Replay) return;
            if (PauseGame) return;
            if (PauseLevel) return;

            //foreach (PlayerData player in PlayerManager.ExistingPlayers)
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    var player = PlayerManager.Players[i];

                    if (player != null && player.Exists && player.StoredName.Length > 0 && player.MyGamer == null)
                    {
                        PlayerManager.GetNumPlayers();
                        if (PlayerManager.NumPlayers > 1)
                            RemovePlayer(player.MyIndex);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Write(e.Message);
            }
        }
#endif
        /// <summary>
        /// Whether the game is paused or not.
        /// </summary>
        public bool PauseGame;

        public virtual void UpdateGamePause()
        {
            PauseGame = MyGameObjects.Any(obj => obj.PauseGame);
            PauseGame |= CharacterSelectManager.IsShowing;
            PauseGame |= CloudberryKingdomGame.SuperPause;
        }

        /// <summary>
        /// Whether the level is paused or not.
        /// </summary>
        public bool PauseLevel;

        public void UpdateLevelPause()
        {
            PauseLevel = MyGameObjects.Any(obj => obj.PauseLevel);
        }

        /// <summary>
        /// Whether the level is paused or not.
        /// </summary>
        public bool SoftPause;

        public void UpdateSoftPause()
        {
            SoftPause = MyGameObjects.Any(obj => obj.SoftPause);
        }

        /// <summary>
        /// The number of phsx steps that should be executed by the application's main loop between draw frames.
        /// Always set back to 1 after each draw frame.
        /// </summary>
        public int PhsxStepsToDo = 2;

        /// <summary>
        /// When true all sounds are suppressed for phsxs steps that are done without being drawn.
        /// </summary>
        public bool SuppressSoundForExtraSteps;

        public int PhsxCount = 0;
		static int XButtonPressCount = 0;
        public virtual void PhsxStep()
        {
            if (Loading || Tools.ShowLoadingScreen) return;

			if (CloudberryKingdomGame.DigitalDayBuild)
			{
				//Tools.SongWad.Stop();

				if (ButtonCheck.State(ControllerButtons.Back, -2).Down)
				{
					XButtonPressCount++;

					if (XButtonPressCount > 150)
					{
						if (CharacterSelectManager.IsShowing)
						{
							CharacterSelectManager.SuddenCleanup();
						}

						// Start at Screen Saver
						ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
					}
				}
				else
				{
					XButtonPressCount = 0;
				}
			}

            // Update the socre and coin score multiplier
            CalculateScoreMultiplier();
            CalculateCoinScoreMultiplier();

            // GameObject physics
            if (MyLevel != null && !Tools.ViewerIsUp)
            {
                LockGameObjects(true);

                Tools.StartGUIDraw();
                foreach (GameObject obj in MyGameObjects)
                {
                    if (obj.CoreData.Released || obj.CoreData.MarkedForDeletion) { obj.CoreData.MarkedForDeletion = true; continue; }

                    // Update object if game is not paused,
                    // or if object updates regardless.
                    if (obj.CoreData.Active && !(PauseGame && obj.PauseOnPause))
                    {
                        if (obj.CoreData.MyLevel == null)
                            MyLevel.AddObject(obj);
                        obj.PhsxStep();
                    }
                }
                Tools.EndGUIDraw();

                LockGameObjects(false);
            }

            // Clean GameObjects
            foreach (GameObject obj in MyGameObjects)
            {
                if (obj.CoreData.MarkedForDeletion)
                    obj.Release();
            }
            CleanGameObjects();

            // Update pause
            UpdateGamePause();
            UpdateLevelPause();

            if (PauseGame) Tools.PhsxSpeed = 1;

            // Fading
            if (FadeColor != null)
            {
                FadeColor.Update();
            }

            if (FadingToReturn && BlackAlpha > 1)
            {
                Tools.WorldMap.SetToReturnTo(0);
                return;
            }

            if (IsSetToReturnTo)
            {
                ReturnTo(SetToReturnToCode);
                return;
            }

            if (MyLevel == null || MyLevel.LevelReleased)
                return;


            // Quick join
//#if XBOX
            if (AllowQuickJoin)
                QuickJoinPhsx();
//#endif


            DoToDoList();

            if (PauseGame)
            {
                // Clean the level, some GameObjects may have been deleted
                if (MyLevel != null)
                    MyLevel.CleanAllObjectLists();
            }

            if (PauseGame)
            {
                // Return and skip further actions, unless the character select is showing
                if (!CharacterSelectManager.IsShowing)
                {
                    if (MyLevel != null)
                    {
                        MyLevel.IndependentDeltaT = 0;
                    }
                    return;
                }
            }
            
            PhsxCount++;
            if (MyLevel != null)
            {
                if (!CharacterSelectManager.IsShowing)
                {
                    if (!PauseLevel)
                    {
                        // Do the level's phsx, suppressing sound if necessary
                        bool HoldSuppress = EzSoundWad.SuppressSounds;
                        EzSoundWad.SuppressSounds |= MyLevel.SuppressSounds;
                        MyLevel.PhsxStep(false);
                        EzSoundWad.SuppressSounds = HoldSuppress;

#if NOT_PC && (XBOX || XBOX_SIGNIN)
                        UpdateSignedInPlayers();
#endif
                    }
                }
                else
                {
                    foreach (Bob bob in MyLevel.Bobs)
                    {
                        if (bob.CharacterSelect2)
                        {
                            bob.PhsxStep();
                            bob.AnimAndUpdate();
                        }
                    }
                }
            }
            // Character select
            CharacterSelectManager.PhsxStep();


            if (FadingToBlack)
                BlackAlpha += FadeOutSpeed;
            if (FadingIn)
            {
                BlackAlpha -= FadeInSpeed;
                if (BlackAlpha < 0f || FadingToBlack)
                    FadingIn = false;
            }
        }

        private void CleanGameObjects()
        {
            MyGameObjects.RemoveAll(match => match.CoreData.MarkedForDeletion);
        }

        public virtual void Move(Vector2 shift)
        {
            if (MyLevel != null) MyLevel.Move(shift);
        }

        public virtual void Init()
        {
            BlackQuad = new QuadClass("White");
            BlackQuad.Quad.SetColor(Color.Black);

            BlackBase.e1 = new Vector2(45, 0);
            BlackBase.e2 = new Vector2(0, 45);

            BlackAlpha = 0;
            FadingToBlack = false;
        }

        /// <summary>
        /// Black out the entire screen.
        /// </summary>
        public void Black() { FadeIn(0); }
        
        /// <summary>
        /// Fade in, starting from pure black.
        /// </summary>
        /// <param name="FadeInSpeed"></param>
        public void FadeIn(float FadeInSpeed)
        {
            BlackAlpha = 1f;
            FadingIn = true;
            FadingToBlack = false;
            this.FadeInSpeed = FadeInSpeed;
        }

        /// <summary>
        /// Fade out to black, starting with no black.
        /// </summary>
        public void FadeToBlack() { FadeToBlack(.01f); }
        public void FadeToBlack(float FadeOutSpeed)
        {
            this.FadeOutSpeed = FadeOutSpeed;
            FadingToBlack = true;
            FadingIn = false;
            BlackAlpha = 0;
        }

        public void FadeToBlack(float FadeOutSpeed, int Delay)
        {
            WaitThenDo(Delay, () => FadeToBlack(FadeOutSpeed));
        }

        public virtual void Draw()
        {
            if (Loading) return;
            else
                if (MyLevel != null)
                {
                    if (MyLevel.LevelReleased)
                        return;

                    if (ForceLevelZoomBeforeDraw > 0)
                        Cam.Zoom = new Vector2(ForceLevelZoomBeforeDraw);
                    
                    CalculateForceZoom();

                    Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);

                    MyLevel.Draw();
                }
        }

        /// <summary>
        /// This is a hack.
        /// </summary>
        public float ForceLevelZoomBeforeDraw = 0;
        public bool DoForceZoom = false;
        public float ForceTargetZoom;

        void CalculateForceZoom()
        {
            if (!DoForceZoom) return;

            if (ForceLevelZoomBeforeDraw < ForceTargetZoom)
                ForceLevelZoomBeforeDraw += .00001f * (float)Math.Pow((ForceTargetZoom - ForceLevelZoomBeforeDraw) / (.00085f - .0007f), .5f);

            if (ForceLevelZoomBeforeDraw > ForceTargetZoom)
                ForceLevelZoomBeforeDraw -= .00001f * (float)Math.Pow((ForceLevelZoomBeforeDraw - ForceTargetZoom) / (.00085f - .0007f), .5f);
        }

        public virtual void PostDraw()
        {
            Tools.StartGUIDraw();

            if (FadingToBlack || FadingIn || FadeColor != null)
            {
                BlackQuad.FullScreen(Cam);

                if (FadeColor == null)
                    BlackQuad.Quad.SetColor(new Color(0, 0, 0, Tools.FloatToByte(BlackAlpha)));
                else
                    BlackQuad.Quad.SetColor(FadeColor.Color);
                BlackQuad.Draw();
                Tools.QDrawer.Flush();
            }

            for (int i = Level.AfterPostDrawLayer; i < Level.NumDrawLayers; i++)
                MyLevel.DrawGivenLayer(i);

            //MyLevel.MainCamera.RevertZoom();
            Tools.EndGUIDraw();
        }

        public virtual void BobDie(Level level, Bob bob)
        {
            if (bob.DieSound == null)
                Bob.DieSound_Default.Play(.3f);
            else
                bob.DieSound.Play(.3f);

            PlayerManager.KillPlayer(bob.MyPlayerIndex);
            
            if (PlayerManager.AllDead())
                DoToDoOnDeathList();
        }

        /// <summary>
        /// A list of actions to take immediately after the last player alive dies.
        /// </summary>
        public List<Action> ToDoOnDeath = new List<Action>();
        void DoToDoOnDeathList()
        {
            List<Action> list = new List<Action>(ToDoOnDeath);
            ToDoOnDeath.Clear();

            foreach (Action f in list)
                f();

            list.Clear();
        }

        /// <summary>
        /// How far below the screen a dead player must drop before being officially dead.
        /// </summary>
        public float DoneDyingDistance = 1200;

        /// <summary>
        /// How many frames after a player dies before the level can be reset
        /// </summary>
        public int DoneDyingCount = 60;

        public virtual void BobDoneDying(Level level, Bob bob)
        {
            if (PlayerManager.AllDead() && level.ResetEnabled())
            {
                level.SetToReset = true;

                DoToDoOnDoneDyingList();
            }
        }

        public List<Action> ToDoOnDoneDying = new List<Action>();
        void DoToDoOnDoneDyingList()
        {
            List<Action> list = new List<Action>(ToDoOnDoneDying);
            ToDoOnDoneDying.Clear();

            foreach (Action f in list)
                f();

            list.Clear();
        }

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
        /*
        public virtual void OnSignIn(SignedInEventArgs e)
        {
            if (AllowQuickJoin)
            {
                int i = (int)e.Gamer.PlayerIndex;
                CreateBob(i, true);
            }
        }*/

        public virtual void OnSignOut_ManualEvent(int index)
        {
            Tools.CurGameData.RemovePlayer(index);
        }

        public virtual void OnSignOut(SignedOutEventArgs e)
        {
            Tools.CurGameData.RemovePlayer((int)e.Gamer.PlayerIndex);
        }
#endif

        protected bool OnePast(float x)
        {
            bool OnePast = false;
            if (MyLevel.Bobs.Count > 0)
            {
                foreach (Bob bob in MyLevel.Bobs)
                    if (bob.CoreData.Data.Position.X > x)
                        OnePast = true;

                return OnePast;
            }
            else
                return false;
        }

        protected bool AllPast(float x)
        {
            bool AllPast = true;
            if (MyLevel.Bobs.Count > 0)
            {
                foreach (Bob bob in MyLevel.Bobs)
                    if (bob.CoreData.Data.Position.X < x)
                        AllPast = false;

                return AllPast;
            }
            else
                return false;
        }

        public PlayerData Mvp
        {
            get
            {
                var list = new List<PlayerData>(PlayerManager.ExistingPlayers);
                return list.ArgMax(p => p.CampaignStats.Score);
            }
        }
        public Bob MvpBob
        {
            get
            {
                foreach (Bob bob in MyLevel.Bobs)
                    if (bob.MyPlayerIndex == Mvp.MyPlayerIndex)
                        return bob;
                return MyLevel.Bobs[0];
            }
        }

        protected bool MvpOnly = false;
        public virtual void MakeBobs(Level level)
        {
            MyLevel.Bobs.Clear();

            int NumStarts = Math.Max(1, level.CurPiece.NumBobs);

            if (MvpOnly)
            {
                CreateBob(level, 1, 0, Mvp.MyIndex, 0);
            }
            else
            {
                int Count = 0;
                for (int i = 3; i >= 0; i--)
                {
                    if (PlayerManager.Get(i).Exists)
                    {
                        int NumBobs = MyGameFlags.IsDoppleganger ? 2 : 1;
                        for (int j = 0; j < NumBobs; j++)
                        {
                            Count = CreateBob(level, NumStarts, Count, i, j);
                        }
                    }
                }
            }

            foreach (Bob bob in MyLevel.Bobs)
                SetCreatedBobParameters(bob);
        }

        private int CreateBob(Level level, int NumStarts, int Count, int i, int j)
        {
            Bob Player = new Bob(level.DefaultHeroType, false);

            Player.MyPlayerIndex = PlayerManager.Get(i).MyPlayerIndex;
            MyLevel.AddBob(Player);

            Player.SetColorScheme(PlayerManager.Get(i).ColorScheme);

            Player.MyPiece = level.CurPiece;
            Player.MyPieceIndex = Count % NumStarts;
            Player.MyPieceIndexOffset = Count / NumStarts;

            if (MyGameFlags.IsDopplegangerInvert)
                Player.MoveData.InvertDirX = j == 0;

            Count++;

            // Check for invisible color scheme
            bool PartiallyInvisible = false, TotallyInvisible = false;
            ColorScheme scheme = PlayerManager.Get(i).ColorScheme;
            if (scheme.SkinColor.Clr.A == 0)
                PartiallyInvisible = true;
            if (PartiallyInvisible &&
                (scheme.HatData == Hat.None || scheme.HatData == Hat.NoHead) &&
                (scheme.CapeColor.Clr.A == 0 && scheme.CapeOutlineColor.Clr.A == 0))
                TotallyInvisible = true;

            if (!PartiallyInvisible) PlayerManager.PartiallyInvisible = false;
            if (!TotallyInvisible) PlayerManager.TotallyInvisible = false;
            return Count;
        }

        public void SetAdditionalBobParameters(Bob[] Bobs)
        {
            SetAdditionalBobParameters(new List<Bob>(Bobs));
        }
        public virtual void SetAdditionalBobParameters(List<Bob> Bobs)
        {
            // Hide corpses
            Bob.ShowCorpseAfterExplode = false;

            // Set Doppleganger
            if (MyGameFlags.IsDoppleganger)
            {
                Bobs.ForEach(bob => bob.Dopple = true);
            }

            if (MyGameFlags.IsTethered)
            {
                // Show corpses
                Bob.ShowCorpseAfterExplode = true;

                // Clear all existing links
                foreach (Bob bob in Bobs)
                    if (bob.MyBobLinks != null)
                        bob.MyBobLinks.Clear();

                // Link bobs together
                for (int i = 0; i < Bobs.Count - 1; i++)
                {
                    BobLink link = new BobLink();
                    link.Connect(Bobs[i], Bobs[i + 1]);
                }
            }
        }

        public static GameData StartLevel(LevelSeedData LevelSeed) { return StartLevel(LevelSeed, false); }
        public static GameData StartLevel(LevelSeedData LevelSeed, bool MakeInBackground)
        {
            if (LockLevelStart) return null;

            LockLevelStart = true;

            LevelSeed.LoadingBegun = true;

            LevelSeed.Init();

            if (!MakeInBackground)
            {
                Tools.CurGameType = LevelSeed.MyGameType;
                if (Tools.CurGameData != null)
                {
                    Tools.CurGameData.DefaultHeroType = LevelSeed.DefaultHeroType;
                }
            }

            GameData MadeGame = null;
            MadeGame = LevelSeed.MyGameType(LevelSeed, MakeInBackground);

            if (!MakeInBackground)
                Tools.CurGameData = MadeGame;

            return MadeGame;
        }

        public bool ModdedBlobGrace = false;
        public float BlobGraceY = 76;

        #region Helper functions for campaign
        public static void UseBobLighting(Level lvl, int difficulty)
        {
            lvl.UseLighting = true; lvl.StickmanLighting = true; lvl.SetBobLightRadius(difficulty);
            Tools.SongWad.SuppressNextInfoDisplay = true;
        }
        #endregion

        #region Helper functions for mini-games
        public void RemoveLastCoinText()
        {
            foreach (GameObject gameobj in MyGameObjects)
                if (gameobj.CoreData.AddedTimeStamp == MyLevel.CurPhsxStep)
                    gameobj.Release();
        }

        public enum DeathTime { FOREVER, SuperSlow, Slow, Normal, Fast, SuperFast };

        /// <summary>
        /// Make deaths quicker.
        /// </summary>
        public void SetDeathTime(DeathTime time)
        {
            switch (time) {
                case DeathTime.FOREVER:
                    DoneDyingDistance = 300000;
                    DoneDyingCount = 190000;
                    break;

                case DeathTime.SuperSlow:
                    DoneDyingDistance = 3000;
                    DoneDyingCount = 190;
                    break;

                case DeathTime.Slow:
                    DoneDyingDistance = 1800;
                    DoneDyingCount = 110;
                    break;

                case DeathTime.Normal:
                    DoneDyingDistance = 1200;
                    DoneDyingCount = 60;
                    break;

                case DeathTime.Fast:
                    //DoneDyingDistance = 165;
                    //DoneDyingCount = 36;
                    DoneDyingDistance = 125;
                    DoneDyingCount = 22;
                    break;

                case DeathTime.SuperFast:
                    DoneDyingDistance = 65;
                    DoneDyingCount = 12;
                    break;
            }
        }

        public virtual void EnterFrom(Door door) { EnterFrom(door, 20); }
        public virtual void EnterFrom(Door door, int Wait)
        {
            // Initially close door and hide bobs
            door.SetLock(true, true, false);
            door.HideBobs();
            MoveAndUpdateBobs();
            door.MoveBobs();

            // Open the door and show the bobs
            CinematicToDo(Wait, () =>
            {
                MyLevel.Bobs.ForEach(bob => bob.CoreData.Show = false);
                door.SetLock(false, false, true);
                door.MoveBobs();
                door.ShowBobs();

                MoveAndUpdateBobs();
                door.MoveBobs();
            });
        }

        static int[] DramaticEntryWait;
        static Vector2 DramaticEntryVel;
        void SetDramaticEntryParams() { DramaticEntryWait = new int[] { 172, 30, 90, 163 }; DramaticEntryVel = new Vector2(0, 2f); }
        public virtual int DramaticEntry(Door door, int Wait)
        {
            SetDramaticEntryParams();

            WaitThenDo(1, () =>
            {
                EnterFrom(door, DramaticEntryWait[0] + Wait);
                CinematicToDo(DramaticEntryWait[1] + Wait, () => door.Shake(19, 11, true));
                CinematicToDo(DramaticEntryWait[2] + Wait, () => door.Shake(19, 11, true));
                CinematicToDo(DramaticEntryWait[3] + Wait, () =>
                {
                    door.Shake(19, 11, true);
                    foreach (Bob bob in door.CoreData.MyLevel.Bobs)
                    {
                        bob.PlayerObject.EnqueueAnimation(2, 0, false, true, false, 10);
                        bob.CoreData.Data.Velocity += DramaticEntryVel;
                    }
                });
            });

            return DramaticEntryWait[0] + Wait;
        }

        void MoveAndUpdateBobs()
        {
            foreach (Bob bob in MyLevel.Bobs)
            {
                bob.CoreData.Data.Velocity = Vector2.Zero;
                bob.CoreData.Data.Acceleration = Vector2.Zero;

                bool HoldShow = bob.CoreData.Show;
                bob.CoreData.Show = true;
                //bob.PhsxStep();
                bob.AnimAndUpdate();
                //bob.PhsxStep2();
                bob.CoreData.Show = HoldShow;
            }
        }

        public void HideBobs()
        {
            MyLevel.Bobs.ForEach(bob => bob.CoreData.Show = false);
        }

        public void ShowBobs()
        {
            MyLevel.Bobs.ForEach(bob => bob.CoreData.Show = true);
        }
        #endregion

        #region Helper functions for making trailer
        protected void LoadRecording(string RecordingName)
        {
            Recording rec = new Recording(1, 10000);
            rec.Load(RecordingName);
            MyLevel.Bobs[0].MyRecord = rec.Recordings[0];
            MyLevel.Bobs[0].CompControl = true;
        }
        #endregion
    }
}
