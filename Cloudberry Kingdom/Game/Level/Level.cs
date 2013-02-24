using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;
using CloudberryKingdom.Particles;
using CloudberryKingdom.InGameObjects;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom.Levels
{
    public partial class Level : ViewReadWrite
    {
        public override string[] GetViewables()
        {
            return new string[] { };
        }

        public Recycler Recycle
        {
            get
            {
                if (MySourceGame != null && !MySourceGame.Released)
                    return MySourceGame.Recycle;
                else if (MyGame != null && !MyGame.Released)
                    return MyGame.Recycle;
                else
                    Tools.Break();

                return null;
            }
        }

        public string Name = "";

        Rand _PrivateRnd = null;
        public Rand Rnd
        {
            get
            {
                if (MyLevelSeed == null)
                {
                    if (_PrivateRnd == null) _PrivateRnd = new Rand(0);
                    return _PrivateRnd;
                }
                else
                    return MyLevelSeed.Rnd;
            }
        }

        public bool SuppressSounds = false;

        public bool SuppressReplayButtons = false;

        public bool LevelReleased;

        public int NumAttempts, PieceAttempts;

        public BobPhsx DefaultHeroType;

        /// <summary>
        /// True if the level generation algorithm returned early.
        /// </summary>
        public bool ReturnedEarly;

        /// <summary>
        /// If true the user can load other levels from the start menu in this level.
        /// </summary>
        public bool CanLoadLevels = false;

        /// <summary>
        /// If true the user can save this level.
        /// </summary>        
        public bool CanSaveLevel = true;

        /// <summary>
        /// If true the player can watch the computer replay.
        /// Use the External bool if suppressing watch from outside the level class.
        /// </summary>
        public bool CanWatchComputer;

        /// <summary>
        /// Whether the computer replay can be watched once the player is far from the spawn point.
        /// </summary>
        public bool CanWatchComputerFromAfar_External = true;

        /// <summary>
        /// Whether watching the computer replay is enabled.
        /// </summary>
        public bool WatchComputerEnabled()
        {
            return CanWatchComputer && CanWatchComputerFromAfar_External;
        }

        public void CountReset()
        {
            foreach (Bob bob in Bobs)
            {
                if (!bob.Dead && !bob.Dying)
                {
                    bob.MyStats.DeathsBy[(int)Bob.BobDeathType.Other]++;
                    bob.MyStats.DeathsBy[(int)Bob.BobDeathType.Total]++;
                }
            }
        }

        /// <summary>
        /// Whether all the players are close to the start and still alive.
        /// </summary>
        public bool CloseToStart()
        {
            if (Bobs == null)
                return true;

            return Bobs.All(bob => (bob.Core.Data.Position - bob.Core.StartData.Position).Length() < 500 && !bob.Dead);
        }

        public bool CanWatchReplay;
        
        bool _PreventReset;
        public bool PreventReset { get { return _PreventReset; } set { _PreventReset = value; } }

        public bool PreventHelp = false;

        public bool FreezeCamera;

        /// <summary>
        /// Whether the level can be reset. Set 'SetToReset = True' to bypass.
        /// </summary>
        public bool ResetEnabled()
        {
            return !PreventReset;
        }

        /// <summary>
        /// Make sure the lava in this level (if it exists) is pushed below the given y-coordinate.
        /// </summary>
        public void PushLava(float y)
        {
            BlockBase lava = Blocks.Find(match => match is LavaBlock);
            if (null != lava)
                PushLava(y, lava as LavaBlock);
        }
        public void PushLava(float y, LavaBlock lava)
        {
            float newtop = Math.Min(lava.Box.Current.TR.Y, y);
            float shift = newtop - lava.Box.Current.TR.Y;
            lava.Move(new Vector2(0, shift));            
        }

        public GameData MyGame, MySourceGame;
        public int TimeLimit = 7200;
        public bool HaveTimeLimit = true;

        /// <summary>
        /// The length of time a timer is shown before the time limit expires.
        /// </summary>
        public int TimeLimitTimerLength = 62 * 10 - 1;

        /// <summary>
        /// Checks the current time limit. If it is almost up a timer is added to the GUI.
        /// </summary>
        void CheckTimeLimit()
        {
            if (!HaveTimeLimit || PlayMode != 0 || Watching || Replay || PlayerManager.AllDead()) return;

            if (CurPhsxStep == TimeLimit - TimeLimitTimerLength)
                MyGame.AddGameObject(new GUI_Timer_Simple(TimeLimitTimerLength));
        }

        /// <summary>
        /// Holds the LevelSeedData that generated this level.
        /// </summary>
        public LevelSeedData MyLevelSeed;

        public Vector2 ModZoom = Vector2.One;

        EzTexture LightTexture;
        RenderTarget2D LightRenderTarget;
        public ClosingCircle Circle;
        QuadClass LightQuad;
        public bool _UseLighting = false;
        
        public Background MyBackground;
        public TileSet MyTileSet;

        public TileSet MyTileSetInfo { get { return MyTileSet; } }
        public TileSet.TileSetInfo Info
        {
            get
            {
                return MyTileSet.MyTileSetInfo;
            }
        }

        public int Par;

        public CameraZone FinalCamZone;

        public List<LevelPiece> LevelPieces;
		public LevelPiece CurPiece;

        public ParticleEmitter MainEmitter;

        public int CurPhsxStep, StartPhsxStep;
        public int DelayReset;

        public bool SetToReset
        {
            get { return _SetToReset; }
            set { _SetToReset = value; }
        }
        public bool _SetToReset;

        /// <summary>
        /// Whether to count the next reset against the player (eg, deduct a life)
        /// </summary>
        public bool FreeReset;

        public bool ObjectsLocked;
        public List<ObjectBase> Objects, AddedObjects;

        /// <summary>
        /// Active draw layer. Used while editing a level. This is the layer a new item is placed in.
        /// </summary>
        public int CurEditorDrawLayer = -1;

        public static int NumDrawLayers = 12 + 1;
        public bool[] ShowDrawLayer;        
        
        /// <summary>
        /// This is the layer the player replays are drawn on.
        /// </summary>
        const int ReplayDrawLayer = 6;
        
        /// <summary>
        /// This is the layer drawn immediately after the last particles.
        /// </summary>
        public static int AfterParticlesDrawLayer = 10;
        
        /// <summary>
        /// This is the last draw layer drawn as part of the actual, physical level.
        /// </summary>
        public static int LastInLevelDrawLayer = 10;
        
        /// <summary>
        /// This draw layer is drawn after the Game class's post draw method.
        /// </summary>
        public static int AfterPostDrawLayer = 12;
        
        List<ObjectBase>[] DrawLayer = new List<ObjectBase>[NumDrawLayers];
        public ParticleEmitter[] ParticleEmitters = new ParticleEmitter[NumDrawLayers];

        public List<BlockBase> Blocks, AddedBlocks;
        public List<Bob> Bobs, HoldPlayerBobs;
        public Vector2 HoldCamPos;

        public bool ShowCoinsInReplay = true;
        public bool Watching, Replay, SuppressCheckpoints, GhostCheckpoints, MainReplayOnly, ReplayPaused;

        public Camera MyCamera;
        Camera HoldCamera;
        public Camera MainCamera
        {
            get { return MyCamera; }
            set
            {
                MyCamera = value;
                MyCamera.MyLevel = this;
                if (OnCameraChange != null) OnCameraChange();
            }
        }

        /// <summary>
        /// Event handler. Activates when the main camera is set to another camera instance.
        /// </summary>
        public event Action OnCameraChange;

        public Vector2 BL, TR;

        /// <summary>
        /// Whether to draw particles.
        /// </summary>
        public bool NoParticles;

        public Level()
        {
            Init(false);
        }
        public Level(bool ShowParticles)
        {
            Init(ShowParticles);
        }

        void Init(bool NoParticles)
        {
            this.NoParticles = NoParticles;
            MyTileSet = TileSets.TileList[0];

            CanWatchComputer = CanWatchReplay = false;

            LevelPieces = new List<LevelPiece>();

            if (!NoParticles)
                MainEmitter =
                    ParticleEmitter.Pool.Get();

            Blocks = new List<BlockBase>(2000);

            Objects = new List<ObjectBase>(2000);
            AddedObjects = new List<ObjectBase>(1000);
            ObjectsLocked = false;

            CreateActiveObjectList();

            ShowDrawLayer = new bool[NumDrawLayers];
            for (int i = 0; i < NumDrawLayers; i++)
            {
                ShowDrawLayer[i] = true;
                DrawLayer[i] = new List<ObjectBase>(300);
                if (!NoParticles)
                    ParticleEmitters[i] =
                        ParticleEmitter.Pool.Get();
                        //new ParticleEmitter(100);
            }


            Bobs = new List<Bob>();
            HoldPlayerBobs = new List<Bob>();

            SetToReset = false;
        }

        public void Release()
        {
            if (LevelReleased) return;

            if (CurMakeData != null)
                CurMakeData.Release();

            LevelReleased = true;

            MyGame = null;
            MyLevelSeed = null;
            MyReplayGUI = null;

            OnWatchComputer = null;
            OnEndReplay = null;

            if (MySwarmBundle != null)
                MySwarmBundle.Release();

            if (LevelPieces != null)
                foreach (LevelPiece piece in LevelPieces)
                    piece.Release();
            LevelPieces = null;
            CurPiece = null;

			if (MyBackground != null) MyBackground.Release(); MyBackground = null;

            if (MainEmitter != null)
            {
                MainEmitter.Release();
                MainEmitter = null;
            }
            if (ParticleEmitters != null)
                foreach (ParticleEmitter emitter in ParticleEmitters)
                    if (emitter != null)
                        emitter.Release();
            ParticleEmitters = null;

            if (CurrentRecording != null)
            {
                CurrentRecording.Release();
                CurrentRecording = null;
            }

            CurrentRecording = null;
            HoldPlayerBobs = null;
            LevelPieces = null;
            MySwarmBundle = null;
            
            if (Blocks != null)
                foreach (BlockBase block in Blocks)
                    block.Release();
            Blocks = null;

            if (Objects != null)
                foreach (ObjectBase obj in Objects)
                {
                    obj.Core.MyLevel = null;
                    obj.Release();
                }
            ActiveObjectList = Objects = null;

            if (Bobs != null)
                foreach (Bob bob in Bobs)
                    bob.Release();
            Bobs = null;
            DefaultHeroType = null;

            DrawLayer = null;

            LevelPieces = null;
            AddedObjects = null;
            AddedBlocks = null;
            HoldPlayerBobs = null;

            MyGame = MySourceGame = null;

            if (MyCamera != null) MyCamera.Release(); MyCamera = null;
            if (HoldCamera != null) HoldCamera.Release(); HoldCamera = null;
            OnCameraChange = null;
        }

        /// <summary>
        /// Get the final door of this level (the exit).
        /// </summary>
        public Door FinalDoor
        {
            get { return FindIObject(LevelConnector.EndOfLevelCode) as Door; }
        }

        /// <summary>
        /// Get the first door of this level (the entrance).
        /// </summary>
        public Door StartDoor
        {
            get { return FindIObject(LevelConnector.StartOfLevelCode) as Door; }
        }

        /// <summary>
        /// Find an object in this level by its code number.
        /// </summary>
        public ObjectBase FindIObject(string Code1)
        {
            foreach (ObjectBase obj in Objects)
                if (string.Compare(obj.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0)
                    return obj;

            return null;
        }

        public int GetPhsxStep() { return CurPhsxStep + 1; }
        public float GetIndependentPhsxStep() { return IndependentPhsxStep + 1; }

        /// <summary>
        /// Returns the current working directory for where .lvl files are stored.
        /// Do not save here if you wish to override a .lvl file in future builds.
        /// </summary>
        public static string DefaultLevelDirectory()
        {
            return Path.Combine(Globals.ContentDirectory, "Levels");
        }

        /// <summary>
        /// Returns the directory where the source .lvl files are stored.
        /// Save here if you wish to override a .lvl file in future builds.
        /// </summary>
        /// <returns></returns>
        public static string SourceLevelDirectory()
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))), "Content\\Levels");
        }

        /// <summary>
        /// Save the level to a .lvl file
        /// </summary>
        /// <param name="Bin">Whether the file is saved to the bin or the original project content directory.</param>
        public void Save(String file, bool Bin)
        {
            // First move to standard directory for .lvl files
            string fullpath;
            if (Bin)
                fullpath = Path.Combine(DefaultLevelDirectory(), file);
            else
                fullpath = Path.Combine(SourceLevelDirectory(), file);

            // Now write to file
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
            Tools.CurLevel.Write(writer);
            writer.Close();
            stream.Close();
        }

        void SortDrawLayers()
        {
            for (int i = 0; i < NumDrawLayers; i++)
                DrawLayer[i].Sort((A, B) => A.Core.DrawSubLayer.CompareTo(B.Core.DrawSubLayer));
        }

        public void Write(BinaryWriter writer)
        {
            // Save sub draw layers
            for (int i = 0; i < NumDrawLayers; i++)
            {
                for (int j = 0; j < DrawLayer[i].Count; j++)
                    DrawLayer[i][j].Core.DrawSubLayer = j;
            }

            // count number of blocks to save
            int Num = 0;
            foreach (BlockBase block in Blocks)
                if (block.Core.EditHoldable)
                    Num++;

            // record the number
            writer.Write(Num);

            foreach (BlockBase block in Blocks)
            {
                if (block.Core.EditHoldable)
                {
                    writer.Write((int)block.Core.MyType);
                    block.Write(writer);
                }
            }

            // count number of objects to save
            Num = 0;
            foreach (ObjectBase obj in Objects)
                if (obj.Core.EditHoldable)
                    Num++;

            // record the number
            writer.Write(Num);

            foreach (ObjectBase obj in Objects)
            {
                if (obj.Core.EditHoldable)
                {
                    writer.Write((int)obj.Core.MyType);
                    obj.Write(writer);
                }
            }
        }

        public void Move(Vector2 shift)
        {
            Move(shift, true);
        }
        public void Move(Vector2 shift, bool MoveBackground)
        {
            MainCamera.Move(shift);

            foreach (LevelPiece piece in LevelPieces)
            {
                for (int i = 0; i < piece.NumBobs; i++)
                    piece.StartData[i].Position += shift;
                piece.LastPoint += shift;
                piece.CamStartPos += shift;
                piece.Shift(shift);
            }

            foreach (BlockBase block in Blocks)
                block.Move(shift);

            foreach (ObjectBase obj in Objects)
                obj.Move(shift);

            foreach (Bob bob in Bobs)
                bob.Move(shift);

            if (MoveBackground && MyBackground != null)
                MyBackground.Move(shift);
        }

        /// <summary>
        /// After calling this function all coins that have been collected will no longer respawn when this level resets.
        /// This is used after a checkpoint is reached.
        /// </summary>
        public void KeepCoinsDead()
        {
            foreach (ObjectBase obj in Objects)
            {
                Coin coin = obj as Coin;
                if (null != coin)
                {
                    if (!coin.Core.Active)
                        Recycle.CollectObject(coin);
                }
            }
        }
        


        public void StopRecording()
        {
            Recording = false;
            if (CurrentRecording != null)
                CurrentRecording.MarkEnd(this);
        }

        void PrepareBundleToAddRecording()
        {
            if (MySwarmBundle == null)
                MySwarmBundle = new SwarmBundle();

            if (MySwarmBundle.CurrentSwarm == null)
            {
                MySwarmBundle.StartNewSwarm();
                MySwarmBundle.CurrentSwarm.MyLevelPiece = CurPiece;
            }
        }

        public void AddCurRecording()
        {
            PrepareBundleToAddRecording();

            if (CurrentRecording != null)
                MySwarmBundle.CurrentSwarm.AddRecord(CurrentRecording, CurPhsxStep);
        }

        /// <summary>
        /// Whether the level is allowed to record the players' attempts.
        /// </summary>
        public bool AllowRecording = false;

        public void CleanRecording()
        {
            if (MySwarmBundle != null) MySwarmBundle.Release(); MySwarmBundle = null;
            if (CurrentRecording != null)
            {
                CurrentRecording.Release();
                CurrentRecording = null;
            }
        }

        /// <summary>
        /// Starts a new player recording. Called at the beginning of the level and after each reset.
        /// </summary>
        public void StartRecording()
        {
            if (!AllowRecording || Watching || Replay) return;

            PrepareBundleToAddRecording();

            if (CurrentRecording != null)
                MySwarmBundle.CurrentSwarm.AddRecord(CurrentRecording, CurPhsxStep);

            if (MySwarmBundle.CurrentSwarm.MyLevelPiece != CurPiece)
            {
                MySwarmBundle.StartNewSwarm();
                MySwarmBundle.CurrentSwarm.MyLevelPiece = CurPiece;
            }

            Recording = true;
            CurrentRecording = new Recording(Bobs.Count, 8000);
        }

        /// <summary>
        /// Gets the objects associated with a GUID.
        /// If the object is marked for deletion then null is returned.
        /// </summary>
        public ObjectBase GuidToObj(UInt64 guid)
        {
            ObjectBase obj = LookupGUID(guid);
            if (obj != null && obj.Core.MarkedForDeletion)
                return null;
            else
                return obj;
        }

        /// <summary>
        /// Gets the objects associated with a list of GUIDs.
        /// If the object is marked for deletion then null is returned.
        /// </summary>
        public List<ObjectBase> GuidToObj(List<UInt64> guids)
        {
            List<ObjectBase> list = new List<ObjectBase>();
            foreach (UInt64 guid in guids) list.Add(GuidToObj(guid));
            return list;
        }

        /// <summary>
        /// Gets the object associated with a GUID, even if that object is marked for deletion.
        /// If no such object exists then null is returned.
        /// </summary>
        public ObjectBase LookupGUID(UInt64 guid)
        {
            ObjectBase FoundObj = Objects.Find(obj => obj.Core.MyGuid == guid);
            if (FoundObj != null)
                return FoundObj;

            return null;
        }

        public void SetCurrentPiece(int LevelPieceIndex) { SetCurrentPiece(LevelPieces[LevelPieceIndex]); }
        public void SetCurrentPiece(LevelPiece piece)
        {
            CurPiece = piece;

            // Change piece associated with each bob
            int Count = 0;
            foreach (Bob bob in Bobs)
            {
                bob.MyPiece = piece;
                bob.MyPieceIndex = Count % piece.NumBobs;

                Count++;
            }
        }

        void NonLambdaReset()
        {
        }

        public bool BoxesOnly;
        public void ResetAll(bool BoxesOnly) { ResetAll(BoxesOnly, true); }
        public void ResetAll(bool BoxesOnly, bool AdditionalReset)
        {
            if (BoxesOnly) Reset_BoxesOnly(AdditionalReset);
            else Reset_Graphical(AdditionalReset);
        }
        void Reset_BoxesOnly(bool AdditionalReset) { __Reset(true, AdditionalReset); }
        void Reset_Graphical(bool AdditionalReset) { __Reset(false, AdditionalReset); }
        void __Reset(bool BoxesOnly, bool AdditionalReset)
        {
            this.BoxesOnly = BoxesOnly;
            NonLambdaReset();

            if (MyGame != null) MyGame.FreeReset = FreeReset;
            FreeReset = false;

            // Clean up the particle emitters
            if (!NoParticles)
            {
                if (MainEmitter != null) MainEmitter.Clean();
                if (ParticleEmitters != null)
                    foreach (ParticleEmitter emitter in ParticleEmitters)
                        if (emitter != null)
                            emitter.Clean();
            }

            CleanAllObjectLists();

            // Check to see if this reset was from the player's all dying/failing.
            if (PlayMode == 0 && !Watching)
            {
                // The player's all died, so increment the number of attempts made on the level.
                PieceAttempts++;
                NumAttempts++;

                // Activate the game's retry event
                if (MyGame != null) MyGame.LevelRetryEvent();
            }

            MainCamera.MyZone = null;
            if (MainCamera.Shaking) MainCamera.EndShake();
            MainCamera.Data.Position = CurPiece.CamStartPos;
            MainCamera.ZoneLocked = false;
            MainCamera.Update();

            CurPhsxStep = StartPhsxStep = CurPiece.StartPhsxStep;
            IndependentStepSetOnce = false;

            // Save sub draw layers
            for (int i = 0; i < NumDrawLayers; i++)
            {
                for (int j = 0; j < DrawLayer[i].Count; j++)
                {
                    var Core = DrawLayer[i][j].Core;
                    if (!Core.FixSubLayer)
                        Core.DrawSubLayer = j;
                }
            }

            // Reset Bobs
            foreach (Bob bob in Bobs)
            {
                PhsxData StartData;
                if (bob.MyPiece != null)
                {
                    StartData = bob.MyPiece.StartData[bob.MyPieceIndex];
                    StartData.Position.X += 20 * bob.MyPieceIndexOffset;
                }
                else
                    StartData = CurPiece.StartData[0];
                if (MyGame != null)
                    bob.Init(BoxesOnly, StartData, MyGame);
                else
                    bob.Init(BoxesOnly, StartData, MySourceGame);

                // Start Bob's box off as tiny, so we can properly collide with ground if the start position is slightly off.
                bob.Box.Current.Size = Vector2.One;
                bob.Box.Target.Size = Vector2.One;
                bob.Box.Current.CalcBounds();
                bob.Box.Target.CalcBounds();
                bob.Box.CalcBounds();

                bob.PlayerObject.BoxesOnly = BoxesOnly;
                bob.BoxesOnly = BoxesOnly;
            }

            // Clean blocks
            foreach (BlockBase block in Blocks) if (block.Core.RemoveOnReset) Recycle.CollectObject(block);
            CleanBlockList();
            
            // Reset blocks
            foreach (BlockBase block in Blocks)
            {
                block.Reset(BoxesOnly);
                if (block.BlockCore.Objects != null)
                    block.BlockCore.Objects.Clear();
            }
            
            // Clean objects
            foreach (ObjectBase obj in Objects) if (obj.Core.RemoveOnReset) Recycle.CollectObject(obj);
            CleanAllObjectLists();
            List<ObjectBase> NewObjects = new List<ObjectBase>();

            // Reset objects
            foreach (ObjectBase obj in Objects)
            {
                obj.Reset(BoxesOnly);         
                ObjectBase NewObj;
                if (obj.Core.ResetOnlyOnReset)
                    NewObj = obj;
                else
                {
                    NewObj = Recycle.GetObject(obj.Core.MyType, BoxesOnly);
                    NewObj.Clone(obj);
                    NewObj.Core.BoxesOnly = BoxesOnly;

                    obj.Core.GenData.OnMarkedForDeletion = null;
                    Recycle.CollectObject(obj, false);
                }
                NewObjects.Add(NewObj);                
            }

            // Recover correct object pointers from GUIDs
            foreach (ObjectBase obj in NewObjects)
            {
                if (obj.Core.ParentObject != null)
                {
                    ObjectBase parent = FindParentObjectById(NewObjects, obj);

                    obj.Core.ParentObject = parent;
                }
            }

            // Re-add all objects 
            ClearAllObjectLists();
            foreach (ObjectBase obj in NewObjects)
                AddObject(obj, false);
            foreach (ObjectBase block in Blocks)
                ReAddObject(block);

            SortDrawLayers();

            // Create new active object list
            CreateActiveObjectList();

            // Reset camera
            MainCamera.Target = MainCamera.Data.Position = CurPiece.CamStartPos;

            // Reset game
            if (AdditionalReset && MyGame != null)
                MyGame.AdditionalReset();

            // Start recording (for player replays)
            if (PlayMode == 0 && !Watching && Recording)
                CurrentRecording.Record(this);

            // Burn a few phsx steps
            if (PlayMode != 0)
                for (int i = 0; i < CurPiece.DelayStart; i++)
                    PhsxStep(false);
            else
            {
                PhsxStep(false, false);
            }
        }

        private static ObjectBase FindParentObjectById(List<ObjectBase> ObjectList, ObjectBase obj)
        {
            ObjectBase FoundObj = ObjectList.Find(_obj =>
                _obj.Core.MyGuid == obj.Core.ParentObjId);

            return FoundObj;
        }

        public void SynchObject(ObjectBase obj)
        {
            if (obj.Core.MyLevel == null)
                obj.Core.StepOffset = 0;
            else
                obj.Core.StepOffset = obj.Core.MyLevel.GetPhsxStep() - GetPhsxStep();
        }

        public void MoveUpOneSublayer(ObjectBase obj)
        {
            int i = DrawLayer[obj.Core.DrawLayer].IndexOf(obj) + 1;
            int N = DrawLayer[obj.Core.DrawLayer].Count;
            if (i >= N) i = N - 1;
            DrawLayer[obj.Core.DrawLayer].Remove(obj);
            DrawLayer[obj.Core.DrawLayer].Insert(i, obj);
        }

        public void MoveToTopOfDrawLayer(ObjectBase obj)
        {
            int i = DrawLayer[obj.Core.DrawLayer].IndexOf(obj);
            int N = DrawLayer[obj.Core.DrawLayer].Count;
            if (i == N - 1) return;

            DrawLayer[obj.Core.DrawLayer].Remove(obj);
            DrawLayer[obj.Core.DrawLayer].Insert(N - 1, obj);
        }

        public void MoveDownOneSublayer(ObjectBase obj)
        {
            int i = DrawLayer[obj.Core.DrawLayer].IndexOf(obj) - 1;
            int N = DrawLayer[obj.Core.DrawLayer].Count;
            if (i < 0) i = 0;
            DrawLayer[obj.Core.DrawLayer].Remove(obj);
            DrawLayer[obj.Core.DrawLayer].Insert(i, obj);
        }

        public void ChangeObjectDrawLayer(ObjectBase obj, int DestinationLayer)
        {
            if (obj.Core.DrawLayer == DestinationLayer)
                return;

            DrawLayer[obj.Core.DrawLayer].Remove(obj);
            DrawLayer[DestinationLayer].Add(obj);
            obj.Core.DrawLayer = DestinationLayer;
        }

        public void RelayerObject(ObjectBase Obj, int NewLayer, bool Front)
        {
            DrawLayer[Obj.Core.DrawLayer].Remove(Obj);

            if (Front)
                DrawLayer[NewLayer].Insert(DrawLayer[NewLayer].Count, Obj);
            else
                DrawLayer[NewLayer].Insert(0, Obj);

            Obj.Core.DrawLayer = NewLayer;
        }

        //Dictionary<IObject, bool> AllObjects = new Dictionary<IObject, bool>();
        public void AddObject(ObjectBase NewObject)
        {
            AddObject(NewObject, true);
        }
        public void AddObject(ObjectBase NewObject, bool AddTimeStamp)
        {
            if (AddTimeStamp)
                NewObject.Core.AddedTimeStamp = CurPhsxStep;

            SynchObject(NewObject);
            NewObject.Core.MyLevel = this;

            if (NewObject.Core.DrawLayer >= 0)
            {
                if (NewObject.Core.ParentBlock == null || NewObject.Core.ParentBlock is NormalBlock || NewObject.Core.DoNotDrawWithParent)
                {
                    DrawLayer[NewObject.Core.DrawLayer].Add(NewObject);                    
                }
                else
                    NewObject.Core.ParentBlock.BlockCore.Objects.Add(NewObject);
            }

            // Add object a second time if it needs to be drawn twice (two different draw layers, for instance)
            if (NewObject.Core.DrawLayer2 > 0)
                DrawLayer[NewObject.Core.DrawLayer2].Add(NewObject);
            if (NewObject.Core.DrawLayer3 > 0)
                DrawLayer[NewObject.Core.DrawLayer3].Add(NewObject);

            if (ObjectsLocked)
            {
                AddedObjects.Add(NewObject);
            }
            else
            {
                Objects.Add(NewObject);
                if (Objects != ActiveObjectList)
                {
                    ActiveObjectList.Add(NewObject);
                }
            }
        }

        /// <summary>
        /// Call to add an object back to the level,
        /// assuming it was never deleted from the main object/block list, nor the main dictionary.
        /// </summary>
        public void ReAddObject(ObjectBase NewObject)
        {
            //AllObjects.Add(NewObject, true);
            DrawLayer[NewObject.Core.DrawLayer].Add(NewObject);

            if (NewObject.Core.DrawLayer2 > 0)
                DrawLayer[NewObject.Core.DrawLayer2].Add(NewObject);
            if (NewObject.Core.DrawLayer3 > 0)
                DrawLayer[NewObject.Core.DrawLayer3].Add(NewObject);
        }

        public List<ObjectBase> PreRecycleBin = new List<ObjectBase>(1000);

        void EmptyPreRecycleBin()
        {
            foreach (ObjectBase obj in PreRecycleBin)
            {
                //if (obj is MovingPlatform && ((MovingPlatform)obj).Parent != null) Tools.Write("!");
                obj.Core.MarkedForDeletion = false;
                obj.Core.MyLevel = null;
                Recycle.CollectObject(obj);
            }

            PreRecycleBin.Clear();
        }

        public void CleanAllObjectLists()
        {
            EmptyPreRecycleBin();

            CleanObjectList();
            CleanBlockList();

            //if (!BoxesOnly)
            CleanDrawLayers();
        }

        public void ClearAllObjectLists()
        {
            //AllObjects.Clear();
            Objects.Clear();
            for (int i = 0; i < NumDrawLayers; i++)
                DrawLayer[i].Clear();
        }

        public void CleanObjectList()
        {
            Tools.RemoveAll(Objects, obj => obj.Core.MarkedForDeletion);
        }

        public void CleanDrawLayers()
        {
            for (int i = 0; i < NumDrawLayers; i++)
            {
                Tools.RemoveAll(DrawLayer[i],
                    (obj, index) =>
                        obj.Core.MarkedForDeletion ||
                        (obj.Core.DrawLayer != i && obj.Core.DrawLayer2 != i && obj.Core.DrawLayer3 != i));
            }
        }

        public void CleanBlockList()
        {
            Tools.RemoveAll(Blocks, block => block.Core.MarkedForDeletion);
        }



        public void AddBlock(BlockBase block)
        {
            AddBlock(block, true);
        }
        public void AddBlock(BlockBase block, bool AddTimeStamp)
        {
            // Add a time stamp
            if (AddTimeStamp)
                block.Core.AddedTimeStamp = CurPhsxStep;

            // Set a default tile set if none is specified
            if (block.Core.MyTileSet == TileSets.None)
                block.Core.MyTileSet = MyTileSet;

            // Add the block to the block list
            SynchObject(block);
            Blocks.Add(block);
            block.BlockCore.MyLevel = this;
            TR = Vector2.Max(TR, block.Box.Current.TR);
            BL = Vector2.Min(BL, block.Box.Current.BL);

            // Add block to the draw lists
            ReAddObject(block);
        }


        public void AddBob(Bob bob)
        {
            bob.Core.MyLevel = this;
            Bobs.Add(bob);
        }

        /// <summary>
        /// While the level is drawing this is the current draw layer being drawn.
        /// </summary>
        public int CurrentDrawLayer;

        public void DrawGivenLayer(int i)
        {
            if (!ShowDrawLayer[i]) return;

            CurrentDrawLayer = i;

            if (CurEditorDrawLayer >= 0 && CurEditorDrawLayer != i) return;

            if (i == CharacterSelectManager.DrawLayer + 1)
                CharacterSelectManager.Draw();

            if (i == ReplayDrawLayer)
            {
                if (Replay && !MainReplayOnly)
                    MySwarmBundle.Draw(CurPhsxStep, this);
            }


            foreach (ObjectBase obj in DrawLayer[i])
                if (obj.Core.Show)
                    obj.Draw();

            Tools.QDrawer.Flush();

            if (MyGame != null && MyGame.DrawObjectText)
            {
                foreach (ObjectBase obj in DrawLayer[i])
                    obj.TextDraw();
                Tools.Render.EndSpriteBatch();
            }

            if (!Replay || MainReplayOnly)
            {
                if (MyGame != null && MyGame.MyGameFlags.IsTethered)
                foreach (Bob Player in Bobs)
                {
                    if (Player.Core.DrawLayer == i && Player.MyBobLinks != null)
                        foreach (BobLink link in Player.MyBobLinks)
                            link.Draw();
                }

                foreach (Bob Bob in Bobs)
                {
                    if (Bob.DrawWithLevel && Bob.Core.DrawLayer == i)
                        Bob.Draw();
                }
                Tools.QDrawer.Flush();
            }

            if (!NoParticles)
                ParticleEmitters[i].Draw();
        }

        public void MakeClosingCircle()
        {
            MakeClosingCircle(150, MainCamera.Data.Position);
        }
        public void MakeClosingCircle(float Frames, Vector2 Pos)
        {
            UseLighting = true;

            Circle = new ClosingCircle(MainCamera, Frames, Pos);
        }
        public void MakeClosingCircle(float Frames, IPos Pos)
        {
            UseLighting = true;

            Circle = new ClosingCircle(MainCamera, Frames, Pos);
        }


        public bool StickmanLighting = false;
        public bool UseLighting
        {
            get
            {
                return _UseLighting;
            }
            set
            {
                _UseLighting = value;

                if (_UseLighting)
                {
                    if (LightRenderTarget == null)
                        InitializeLighting();
                }
                else
                {
                    LightRenderTarget = null;
                    LightTexture = null;
                }
            }
        }

        public void InitializeLighting()
        {
            LightTexture = new EzTexture(); LightTexture.Name = "LightTexture";

            PresentationParameters pp = Tools.Device.PresentationParameters;
            LightRenderTarget = new RenderTarget2D(Tools.Device,
                                    pp.BackBufferWidth, pp.BackBufferHeight, false,
                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                   RenderTargetUsage.DiscardContents);

            LightQuad = new QuadClass();
            LightQuad.EffectName = "LightMap";
        }


        float BobLightRadius = 700; //670
        static float[] BobLightRadiusByDifficulty = new float[] { 800, 740, 690, 630, 500 };
        public void SetBobLightRadius(int Difficulty)
        {
            //BobLightRadius = BobLightRadiusByDifficulty[Difficulty];
            BobLightRadius = 900;
        }

        public void FadeBobLightSourcesIn()
        {
            foreach (Bob bob in Bobs)
                bob.SetLightSourceToFadeIn();
        }

        public enum LightLayers { FrontOfLevel, FrontOfEverything };
        public LightLayers LightLayer = LightLayers.FrontOfLevel;
        void PrepareLighting()
        {
            Tools.QDrawer.Flush();

            MainCamera.SetVertexCamera();
            Tools.QDrawer.WashTexture();
            Tools.Device.SetRenderTarget(LightRenderTarget);
            Tools.Device.Clear(Color.Transparent);

            // Closing circle
            if (Circle != null)
                Circle.Draw();
            Tools.QDrawer.Flush();

            // Stickmen lighting
            if (StickmanLighting)
            {
                foreach (Bob bob in Bobs)
                {
                    int DeadCount = CoreMath.Restrict(0, 1000, bob.DeadCount - 3);
                    float fade = CoreMath.Restrict(0f, 1f, bob.LightSourceFade - DeadCount * .0175f);
                    Color c = new Color(1f, 1f, 1f, fade);
                    int radius = (int)CoreMath.Restrict(0.0f, 1000.0f, 860.0f + DeadCount * 27.0f + CoreMath.Periodic(-30, 30, 40, CurPhsxStep));
                    Tools.QDrawer.DrawLightSource(bob.Pos, radius, 5f, c);//new Color(.75f, .75f, .75f, .75f));
                }
                Tools.QDrawer.Flush();
            }

            Tools.QDrawer.Flush();
            Tools.Device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.TheGame.MyGraphicsDevice.Clear(Color.Black);
            Tools.Render.ResetViewport();
            LightTexture.Tex = LightRenderTarget;
        }

        void DrawLighting()
        {
            MainCamera.SetVertexCamera();

            LightQuad.Quad.MyTexture = LightTexture;
            LightQuad.FullScreen(MainCamera);
            LightQuad.Draw();
        }

        public void Draw() { Draw(false); }
        public void Draw(bool DrawAll) { Draw(DrawAll, 0, 100); }
        public void Draw(bool DrawAll, int StartLayer, int EndLayer)
        {
            if (LevelReleased) return;

            if (ModZoom != Vector2.One) { Tools.EffectWad.ModZoom = ModZoom; Tools.EffectWad.ResetCameraPos(); }

            if (UseLighting)
                PrepareLighting();

            if (MyBackground != null && Tools.DrawGraphics)
            {
                if (Background.GreenScreen)
                    Background.DrawTest();
                else
                    MyBackground.Draw();
            }

            if (CloudberryKingdomGame.HideForeground) return;

            MainCamera.SetVertexCamera();

            Vector2 HoldBL, HoldTR;
            HoldBL = MainCamera.BL;
            HoldTR = MainCamera.TR;
            if (DrawAll)
            {
                MainCamera.BL = new Vector2(-1000000, -1000000);
                MainCamera.TR = new Vector2(1000000, 1000000);
            }

            for (int i = StartLayer; i <= CoreMath.Restrict(0, Level.AfterParticlesDrawLayer - 1, EndLayer); i++)
                DrawGivenLayer(i);

            // Draw particles
            if (!NoParticles)
                MainEmitter.Draw();
            Tools.QDrawer.Flush();

            if (UseLighting && LightLayer == LightLayers.FrontOfLevel)
                DrawLighting();

            // Draw background's foreground.
            if (MyBackground != null && Tools.DrawGraphics)
            {
                MyBackground.DrawForeground();
            }

            // Draw final DrawLayer
            if (ModZoom != Vector2.One) { Tools.EffectWad.ModZoom = Vector2.One; Tools.EffectWad.ResetCameraPos(); }
            Tools.StartGUIDraw();
            if (EndLayer >= Level.AfterParticlesDrawLayer)
            {
                DrawGivenLayer(Level.AfterParticlesDrawLayer);
                DrawGivenLayer(Level.AfterParticlesDrawLayer + 1);
            }

            if (DrawAll)
            {
                MainCamera.BL = HoldBL;
                MainCamera.TR = HoldTR;
            }
            
            if (UseLighting && LightLayer == LightLayers.FrontOfEverything)
                DrawLighting();   

            Tools.EndGUIDraw();
        }

        public void FinalizeBlocks()
        {
            foreach (BlockBase block in Blocks)
                block.BlockCore.Finalized = true;
        }

        public void TagAll(int Tag)
        {
            foreach (ObjectBase obj in Objects) obj.Core.Tag = Tag;
            foreach (BlockBase block in Blocks) block.Core.Tag = Tag;
        }

        // Take all objects in a different level and add them
        public void AddLevelBlocks(Level level)
        {
            foreach (BlockBase block in level.Blocks) AddBlock(block);
        }
        public void AddLevelBlocks(Level level, int Tag)
        {
            foreach (BlockBase block in level.Blocks) if (block.Core.Tag == Tag && !block.Core.DoNotScrollOut) AddBlock(block);
            level.RemoveForeignObjects();
        }
        public void AddLevelObjects(Level level)
        {
            AddLevelObjects(level, new Vector2(-10000000, -100000000), new Vector2(10000000, 10000000));
        }
        public void AddLevelObjects(Level level, Vector2 p1, Vector2 p2)
        {
            foreach (ObjectBase obj in level.Objects) if (IsBetween(obj.Core.Data.Position, p1, p2) && !obj.Core.DoNotScrollOut) AddObject(obj, false);
        }
        public void AddLevelObjects(Level level, int Tag)
        {
            foreach (ObjectBase obj in level.Objects) if (obj.Core.Tag == Tag && !obj.Core.DoNotScrollOut) AddObject(obj, false);
            level.RemoveForeignObjects();
        }

        public void AbsorbLevelVisuals(Level level)
        {
            if (MainEmitter != null && level.MainEmitter != null)
                MainEmitter.Absorb(level.MainEmitter);

            if (MyBackground != null && level.MyBackground != null)
                MyBackground.Absorb(level.MyBackground);
        }

        public void SetBackground(Background background)
        {
            MyBackground = background;
            MyBackground.SetLevel(this);
        }

        public void AbsorbLevel(Level level)
        {
            AddLevelBlocks(level);
            AddLevelObjects(level);
            foreach (LevelPiece piece in level.LevelPieces)
            {
                piece.MyLevel = this;
                LevelPieces.Add(piece);
            }

            AbsorbLevelVisuals(level);

            level.Objects = null;
            level.Blocks = null;

            foreach (Bob bob in level.Bobs)
                bob.MyRecord = null;            
            level.LevelPieces = null;
            level.Release();
        }
        
        // Remove all objects that belong to a different level
        public void RemoveForeignObjects()
        {
            Objects.RemoveAll(obj => obj.Core.MyLevel != this);
            for (int i = 0; i < NumDrawLayers; i++)
                DrawLayer[i].RemoveAll(obj => obj.Core.MyLevel != this);
            Blocks.RemoveAll(obj => obj.Core.MyLevel != this);
        }

        /// <summary>
        /// Get a list of all objects in the level of a given type.
        /// </summary>
        public List<ObjectBase> GetObjectList(ObjectType type)
        {
            List<ObjectBase> list = new List<ObjectBase>();

            foreach (ObjectBase obj in Objects)
                if (obj.Core.MyType == type && !obj.Core.MarkedForDeletion)
                    list.Add(obj);

            return list;
        }

        public delegate Vector2 Metric(ObjectBase A, ObjectBase B);
        static Vector2 DefaultMetric(ObjectBase A, ObjectBase B)
        {
            return new Vector2(
                Math.Abs(A.Core.Data.Position.X - B.Core.Data.Position.X),
                Math.Abs(A.Core.Data.Position.Y - B.Core.Data.Position.Y));
        }
        public delegate Vector2 CleanupCallback(Vector2 pos);
        public void Cleanup(ObjectType type, CleanupCallback MinDistFunc)
        {
            Cleanup(type, MinDistFunc, new Vector2(-100000000, -100000000), new Vector2(100000000, 100000000));
        }
        public void Cleanup(ObjectType type, CleanupCallback MinDistFunc, Vector2 BL, Vector2 TR)
        {
            Cleanup(type, MinDistFunc, BL, TR, DefaultMetric);
        }
        public void Cleanup(ObjectType type, CleanupCallback MinDistFunc, Vector2 BL, Vector2 TR, Metric metric)
        {
            List<ObjectBase> CleanupList = GetObjectList(type);

            Cleanup(CleanupList, MinDistFunc, false, BL, TR, metric);
        }

        public void Cleanup(List<ObjectBase> ObjList, CleanupCallback MinDistFunc, Vector2 BL, Vector2 TR)
        {
            Cleanup(ObjList, MinDistFunc, false, BL, TR);
        }
        // If MustBeDifferent is set, then only two objects of different types can force a deletion
        public void Cleanup(List<ObjectBase> ObjList, CleanupCallback MinDistFunc, bool MustBeDifferent, Vector2 BL, Vector2 TR)
        {
            Cleanup(ObjList, MinDistFunc, MustBeDifferent, BL, TR, DefaultMetric);
        }
        public void Cleanup(List<ObjectBase> ObjList, CleanupCallback MinDistFunc, bool MustBeDifferent, Vector2 BL, Vector2 TR, Metric metric)
        {
            if (ObjList == null) return;

            foreach (ObjectBase obj in ObjList)
            {
                if (obj.Core.GenData.EnforceBounds)
                if (obj.Core.Data.Position.X > TR.X || obj.Core.Data.Position.X < BL.X ||
                    obj.Core.Data.Position.Y > TR.Y || obj.Core.Data.Position.Y < BL.Y)
                    Recycle.CollectObject(obj);

                if (obj.Core.MarkedForDeletion)
                    continue;

                if (!obj.Core.GenData.LimitDensity) continue;

                CheckAgainst(obj, ObjList, MinDistFunc, metric, MustBeDifferent);
            }
        }


        public void Cleanup_xCoord(ObjectType ObjType, float MinDist)
        {
            List<ObjectBase> ObjList = GetObjectList(ObjType);

            foreach (ObjectBase obj in ObjList)
            {
                if (obj.Core.MarkedForDeletion)
                    continue;

                CheckAgainst_xCoord(obj, ObjList, MinDist);
            }
        }


        void CheckAgainst(ObjectBase obj, List<ObjectBase> ObjList, CleanupCallback MinDistFunc, Metric metric, bool MustBeDifferent)
        {
            foreach (ObjectBase obj2 in ObjList)
            {
                if (!obj2.Core.GenData.LimitDensity) return;

                if (obj.Core.IsAssociatedWith(obj2))
                {
                    if (obj.Core.GetAssociationData(obj2).UseWhenUsed)
                        return;
                }

                if (!obj.Core.MarkedForDeletion &&
                    !obj2.Core.MarkedForDeletion &&
                    obj != obj2 &&
                    !(MustBeDifferent && obj.Core.MyType == obj2.Core.MyType))
                {
                    Vector2 MinDist = (MinDistFunc(obj.Core.Data.Position) + MinDistFunc(obj2.Core.Data.Position)) / 2;

                    Vector2 d = metric(obj, obj2);

                    if (d.X < MinDist.X && d.Y < MinDist.Y)
                    {
                        int Choice = 0; // 0 -> Remove first object, 1 -> Remove second object

                        if (obj.Core.GenData.KeepIfUnused && obj2.Core.GenData.KeepIfUnused) return;
                        else if (obj.Core.GenData.KeepIfUnused) Choice = 1;
                        else if (obj2.Core.GenData.KeepIfUnused) Choice = 0;
                        else if (Rnd.Rnd.NextDouble() > .5) Choice = 1;

                        if (Choice == 0)
                            Recycle.CollectObject(obj);
                        else
                            Recycle.CollectObject(obj2);
                    }
                }
            }
        }

        void CheckAgainst_xCoord(ObjectBase obj, List<ObjectBase> ObjList, float MinDist)
        {
            foreach (ObjectBase obj2 in ObjList)
            {
                if (!obj.Core.MarkedForDeletion &&
                    !obj2.Core.MarkedForDeletion &&
                    obj != obj2)
                {
                    float d = (float)Math.Abs(obj.Pos.X - obj2.Pos.X);

                    if (d < MinDist)
                    {
                        int Choice = 0; // 0 -> Remove first object, 1 -> Remove second object

                        if (obj.Core.GenData.KeepIfUnused && obj2.Core.GenData.KeepIfUnused) return;
                        else if (obj.Core.GenData.KeepIfUnused) Choice = 1;
                        else if (obj2.Core.GenData.KeepIfUnused) Choice = 0;
                        else if (Rnd.Rnd.NextDouble() > .5) Choice = 1;

                        if (Choice == 0)
                            Recycle.CollectObject(obj);
                        else
                            Recycle.CollectObject(obj2);
                    }
                }
            }
        }

        public void StartPlayerPlay()
        {
            PieceAttempts--;
            NumAttempts--;

            Watching = false;
            SuppressCheckpoints = false;
            GhostCheckpoints = false;

            Bobs.Clear();
            Bobs.AddRange(HoldPlayerBobs);

            SetToReset = true;
        }

        void EvolveParticles()
        {
            // Evolve particles
            if (!NoParticles)
            {
                MainEmitter.Phsx();
                for (int i = 0; i < NumDrawLayers; i++)
                    ParticleEmitters[i].Phsx();
            }
        }

        void UpdateBlocks()
        {
            foreach (BlockBase block in Blocks)
                if (!block.Core.MarkedForDeletion)
                    block.PhsxStep();
        }

        void UpdateObjects()
        {
            foreach (ObjectBase Object in ActiveObjectList)
            {
                if (!Object.Core.IsGameObject && !Object.Core.MarkedForDeletion)
                    Object.PhsxStep();
            }
        }

        void UpdateBlocks2()
        {
            foreach (BlockBase block in Blocks)
                if (!block.Core.MarkedForDeletion)
                    block.PhsxStep2();
        }

        void UpdateObjects2()
        {
            foreach (ObjectBase Object in ActiveObjectList)
            {
                if (!Object.Core.MarkedForDeletion)
                    Object.PhsxStep2();
            }
        }

        void UpdateBobs()
        {
            foreach (Bob bob in Bobs)
            {
                bob.PhsxStep();
                if (!bob.Cinematic && !bob.ManualAnimAndUpdate)
                    bob.AnimAndUpdate();

                int i = bob.MyPieceIndex;
                if (RecordPosition)
                {
                    CurPiece.Recording[i].AutoLocs[CurPhsxStep - bob.IndexOffset] = bob.Core.Data.Position;
                    CurPiece.Recording[i].AutoVel[CurPhsxStep - bob.IndexOffset] = bob.Core.Data.Velocity;
                    CurPiece.Recording[i].AutoOnGround[CurPhsxStep - bob.IndexOffset] = bob.MyPhsx.OnGround;
                }
                else
                {
                    if (PlayMode == 0 && Replay && !ReplayPaused)
                    {
                        Vector2 IntendedLoc = MySwarmBundle.CurrentSwarm.MainRecord.Recordings[Bobs.IndexOf(bob)].AutoLocs[CurPhsxStep];
                        Vector2 IntendedVel = MySwarmBundle.CurrentSwarm.MainRecord.Recordings[Bobs.IndexOf(bob)].AutoVel[CurPhsxStep];
                        bob.Move(IntendedLoc - bob.Core.Data.Position);
                        bob.Core.Data.Velocity = IntendedVel;
                    }

                    if (bob.MyPiece != null && bob.MyPiece.Recording != null && !bob.CharacterSelect && !bob.CharacterSelect2)
                        if (PlayMode == 1 || PlayMode == 0 && bob.CompControl && !Replay)
                        {
                            int index = CurPhsxStep - bob.IndexOffset;
                            Vector2 a, b;
                            bool A, B;
                            a = bob.MyPiece.Recording[i].AutoLocs[index];
                            b = bob.Core.Data.Position;
                            A = bob.MyPiece.Recording[i].AutoOnGround[index];
                            B = bob.MyPhsx.OnGround;
                            Vector2 dif = a - b;
                            if (Math.Abs(dif.X) > .001f || Math.Abs(dif.Y) > .001f)
                            {
                                if (CurPhsxStep < bob.MyPiece.PieceLength - 15)
                                {
                                    CreationError = true;
                                }
                                if (a != Vector2.Zero)
                                {
                                    //bob.Core.Data.Position = a;
                                    bob.Move(a - bob.Core.Data.Position);
                                    bob.Core.Data.Velocity = bob.MyPiece.Recording[i].AutoVel[index];
                                    //Console.WriteLine("Bob[{0}]---> {1}/{2}:  {3}, {4}, {5}, {6}", Bobs.IndexOf(bob), GetPhsxStep(), bob.MyPiece.PieceLength, a, b, A, B);
                                }
                            }
                        }
                }
            }
        }

        void CalcObstaclsSeen()
        {
            var players = PlayerManager.ExistingPlayers;
            foreach (PlayerData player in players)
                player.Stats.ObstaclesSeen = NumObstacles;

            foreach (Bob bob in Bobs)
                Awardments.CheckForAward_Obstacles(bob);
        }

        public void PhsxStep(bool NotDrawing) { PhsxStep(NotDrawing, true); }
        public void PhsxStep(bool NotDrawing, bool GUIPhsx)
        {
            if (!IndependentStepSetOnce)
                SetIndependentStep();

            if (LevelReleased) return;

            if (!SetToReset && Watching && Replay && !ReplayPaused && MySwarmBundle.EndCheck(this))
            {
                if (!MySwarmBundle.GetNextSwarm(this))
                    ReplayPaused = true;
                else
                    SetToReset = true;
            }
            if (!SetToReset && Watching && !Replay && EndOfReplay())
            {
                ReplayPaused = true;
            }

           
            if (SetToReset)
            {
                if (DelayReset <= 0)
                {
                    SetToReset = false;
                    ResetAll(false);

                    if (LevelReleased) return;
                }
                else
                {
                    if (!NoParticles)
                        MainEmitter.Phsx();
                    DelayReset--;
                    return;
                }
            }

            if (ReplayPaused) return;

            if (CurPhsxStep == 300)
                CalcObstaclsSeen();

            EvolveParticles();

            CheckTimeLimit();

            ObjectsLocked = true;

            // Blocks update first, so that attached objects are positioned correctly
            UpdateBlocks();

            // Objects are updated afterward
            UpdateObjects();

            if (!SetToReset)
                UpdateBobs();


            // Blocks, final physics steps
            UpdateBlocks2();

            // Objects, final physics steps
            UpdateObjects2();

            ObjectsLocked = false;
            Objects.AddRange(AddedObjects);
            if (ActiveObjectList != Objects) ActiveObjectList.AddRange(AddedObjects);
            AddedObjects.Clear();

            int CleanPeriod = 20;
            if (!BoxesOnly || CurPhsxStep % CleanPeriod == 0)
            {
                // Remove deleted objects
                CleanAllObjectLists();

                // Reset active object list
                if (BoxesOnly)
                    ResetActiveObjectList();
            }

            // Update the active object list
            if (BoxesOnly && CurPhsxStep % CleanPeriod == 1)
                UpdateActiveObjectList();

            // Update the camera
            if (!FreezeCamera)
                MainCamera.PhsxStep();
            else
                MainCamera.Update();
        
            // Increase number of steps taken
            CurPhsxStep++;

            // Increase time (may not be related to number of steps taken)
            SetIndependentStep();

            // If the player is playing record this frame
            if (!(PlayMode != 0 || Watching))
                if (Recording) CurrentRecording.Record(this);
        }

        private void SetIndependentStep()
        {
            float PrevIndependentPhsxStep = IndependentPhsxStep;
            //TimeType = TimeTypes.Regular;
            //TimeType = TimeTypes.xSync;
            switch (TimeType)
            {
                case TimeTypes.Regular:
                    //IndependentPhsxStep = CurPhsxStep * 1.5f;
                    IndependentPhsxStep = CurPhsxStep * 1f;
                    if (!IndependentStepSetOnce)
                        PrevIndependentPhsxStep = IndependentPhsxStep - 1;
                    break;

                case TimeTypes.ySync:
                case TimeTypes.xSync:
                    if (Bobs.Count > 0 && Bobs[0] != null)
                    {
                        int NumAlive = 0;
                        Vector2 Pos = Vector2.Zero;
                        //Pos.X = -1000000;
                        foreach (var bob in Bobs)
                        {
                            if (bob.MyPlayerData.IsAlive)
                            {
                                NumAlive++;
                                //Pos.X = (float)Math.Max(Pos.X, bob.Pos.X);
                                Pos += bob.Pos;
                            }
                        }

                        if (NumAlive == 0)
                            break;

                        float New = 0;
                        if (TimeType == TimeTypes.xSync)
                            New = (Pos.X / NumAlive) / 10;
                        else
                            New = (Pos.Y / NumAlive) / 4.5f;

                        if (!IndependentStepSetOnce)
                            Prev = New;

                        IndependentPhsxStep = Math.Max(New, Prev);
                        Prev = New;

                        if (!IndependentStepSetOnce)
                            PrevIndependentPhsxStep = IndependentPhsxStep;
                    }
                    break;
            }

            IndependentDeltaT = IndependentPhsxStep - PrevIndependentPhsxStep;
            IndependentStepSetOnce = true;
        }

        public bool IndependentStepSetOnce = false;
        public float IndependentPhsxStep = 0, IndependentDeltaT = 0;
        float Prev = 0;

        public enum TimeTypes { Unset, Regular, xSync, ySync };
        public TimeTypes TimeType = TimeTypes.Regular;

        public List<ObjectBase> ActiveObjectList;

        void CreateActiveObjectList()
        {
            if (BoxesOnly)
            {
                ActiveObjectList = new List<ObjectBase>();
                ResetActiveObjectList();
            }
            else
                ActiveObjectList = Objects;
        }

        void UpdateActiveObjectList()
        {
            if (ActiveObjectList == Objects) return;

            ActiveObjectList.Clear();
            
            // Keep active all objects that didn't skip their phsx the previous step.
            foreach (ObjectBase obj in Objects)
            {
                if (!obj.Core.SkippedPhsx && !obj.Core.MarkedForDeletion)
                {
                    ActiveObjectList.Add(obj);
                }
            }
        }

        /// <summary>
        /// Set the active object list to all objects in the level
        /// </summary>
        void ResetActiveObjectList()
        {
            if (ActiveObjectList == Objects) return;

            ActiveObjectList.Clear();

            foreach (ObjectBase obj in Objects) ActiveObjectList.Add(obj);
        }


        public bool IsBetween(Vector2 Point, Vector2 p1, Vector2 p2)
        {
            if (Math.Sign(p1.X - Point.X) != Math.Sign(p2.X - Point.X) &&
                Math.Sign(p1.Y - Point.Y) != Math.Sign(p2.Y - Point.Y))
                return true;
            else
                return false;
        }

        public void CountCoinsAndBlobs()
        {
            NumCoins = 0;
            TotalCoinScore = 0;
            NumObstacles = 0;
            foreach (ObjectBase obj in Objects)
            {
                Coin coin = obj as Coin;
                if (null != coin && !coin.Core.MarkedForDeletion)
                {
                    NumCoins++;
                    TotalCoinScore += coin.MyScoreValue;
                }

                FlyingBlob blob = obj as FlyingBlob;
                if (null != blob)
                {
                    NumBlobs++;
                }

                if (obj is _Death)
                    NumObstacles++;
            }
        }

        public void SetBack(int Steps)
        {
            CurPiece.StartPhsxStep -= Steps;
            MyGame.WaitThenDo(2, () => CurPiece.StartPhsxStep += Steps);
        }
    }
}
