using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Coins;
using CloudberryKingdom.Particles;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Goombas;
using Drawing;

namespace CloudberryKingdom.Levels
{
    public delegate void FinishedLevelHandler();
    public partial class Level : IViewable
    {
        public virtual string[] GetViewables()
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
        public event FinishedLevelHandler FinishedLevel;

        public bool PieceAtEnd;
        public int MaxAttempts, NumAttempts, PieceAttempts;

        public BobPhsx DefaultHeroType;

        bool ShouldSwitch = false;
        public void SwitchHeroType(Bob bob, BobPhsx hero)
        {
            // Switch the given player
            if (bob != null)
            {
                bob.SwitchHero(hero);
                DefaultHeroType = hero;
            }

            // On reset make sure EVERY player has been switched
            if (MyGame != null)
            {
                ShouldSwitch = true;
                MyGame.ToDoOnReset.Add(() =>
                {
                    foreach (Bob b in Bobs)
                        b.SwitchHero(hero);

                    // Make sure we don't do this again
                    ShouldSwitch = false;
                    if (!ShouldSwitch) return;
                });
            }
        }

        /// <summary>
        /// True if the level generation algorithm returned early.
        /// </summary>
        public bool ReturnedEarly;



        /// <summary>
        /// If true the player can watch the computer replay.
        /// Use the External bool if suppressing watch from outside the level class.
        /// </summary>
        public bool CanWatchComputer, CanWatchComputer_External = true;

        /// <summary>
        /// Whether the computer replay can be watched once the player is far from the spawn point.
        /// </summary>
        public bool CanWatchComputerFromAfar_External = true;

        /// <summary>
        /// Whether watching the computer replay is enabled.
        /// </summary>
        public bool WatchComputerEnabled()
        {
            // Check if players are close to the start
//            if (CloseToStart())
//                return CanWatchComputer && CanWatchComputer_External;
//            else
                return CanWatchComputer && CanWatchComputer_External && CanWatchComputerFromAfar_External;
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
            Campaign.Attempts++;
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
        //public Vector2 ModZoom = new Vector2(-1, 1);

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

        public BlockBase FinalPlat;
        public CameraZone FinalCamZone;
        public bool LevelCleared;

        public float ScrollSpeed;

        public List<LevelPiece> LevelPieces;
        public LevelPiece CurPiece;

        public bool Popping;

        public Vector2 LastPoint;

        public int RndSeed;

        public ParticleEmitter MainEmitter;
        public Particle PopTemplate;

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
        const int FirstSecondDrawLayer = 6;
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

        public void ShuffleLayer(int i)
        {
            DrawLayer[i] = Rnd.Shuffle(DrawLayer[i]);
        }

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

        public Level(string filename, GameData game)
        {
            MyGame = game;
            Init(false);
            Load(filename);
        }

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
            MyTileSet = TileSets.Castle;

            CanWatchComputer = CanWatchReplay = false;

            LevelPieces = new List<LevelPiece>();

            if (!NoParticles)
                MainEmitter =
                    ParticleEmitter.Pool.Get();
            
            PopTemplate = new Particle();
            PopTemplate.MyQuad.Init();
            PopTemplate.MyQuad.MyEffect = Tools.EffectWad.FindByName("Shell");//Basic");
            PopTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("White");//Pop");
            PopTemplate.SetSize(85);
            PopTemplate.SizeSpeed = new Vector2(10, 10);
            PopTemplate.AngleSpeed = 0;// .013f;
            PopTemplate.Life = 20;
            PopTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
            PopTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);


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

            MyBackground = null;

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


        public Door FinalDoor
        {
            get { return FindIObject(LevelConnector.EndOfLevelCode) as Door; }
        }

        public Door StartDoor
        {
            get { return FindIObject(LevelConnector.StartOfLevelCode) as Door; }
        }

        public ObjectBase FindIObject(string Code1)
        {
            foreach (ObjectBase obj in Objects)
                if (string.Compare(obj.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0)
                    return obj;

            return null;
        }

        public ObjectBase FindIObject(string Code1, string Code2)
        {
            foreach (ObjectBase obj in Objects)
                if (string.Compare(obj.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0 &&
                    string.Compare(obj.Core.EditorCode2, Code2, StringComparison.OrdinalIgnoreCase) == 0)
                    return obj;

            return null;
        }

        public ObjectBase FindIObject(string Code1, string Code2, string Code3)
        {
            foreach (ObjectBase obj in Objects)
                if ((Code1.Length == 0 || string.Compare(obj.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (Code2.Length == 0 || string.Compare(obj.Core.EditorCode2, Code2, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (Code3.Length == 0 || string.Compare(obj.Core.EditorCode3, Code3, StringComparison.OrdinalIgnoreCase) == 0))
                    return obj;

            return null;
        }

        public void HideAll(string Code1)
        {
            foreach (BlockBase block in Blocks)
                if (block.Core == Code1)
                    block.Core.Show = false;
        }

        public BlockBase FindBlock(string Code1)
        {
            foreach (BlockBase block in Blocks)
                if (string.Compare(block.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0)
                    return block;

            return null;
        }

        public BlockBase FindBlock(string Code1, string Code2)
        {
            foreach (BlockBase block in Blocks)
                if ((Code1.Length == 0 || string.Compare(block.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (Code2.Length == 0 || string.Compare(block.Core.EditorCode2, Code2, StringComparison.OrdinalIgnoreCase) == 0))
                    return block;

            return null;
        }

        public BlockBase FindBlock(string Code1, string Code2, string Code3)
        {
            foreach (BlockBase block in Blocks)
                if ((Code1.Length == 0 || string.Compare(block.Core.EditorCode1, Code1, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (Code2.Length == 0 || string.Compare(block.Core.EditorCode2, Code2, StringComparison.OrdinalIgnoreCase) == 0) &&
                    (Code3.Length == 0 || string.Compare(block.Core.EditorCode3, Code3, StringComparison.OrdinalIgnoreCase) == 0))
                    return block;

            return null;
        }

        //public int GetPhsxStep() { return CurPhsxStep + StartPhsxStep + 1; }
        public int GetPhsxStep() { return CurPhsxStep + 1; }
        public float GetIndependentPhsxStep() { return IndependentPhsxStep + 1; }

        /// <summary>
        /// The file this level was loaded from.
        /// </summary>
        public string SourceFile;

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
            FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
            Tools.CurLevel.Write(writer);
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// Load into the level information from a .lvl file.
        /// </summary>
        public void Load(String file)
        {
            SourceFile = file;

            // First move to standard directory for .lvl files
            string fullpath = Path.Combine(DefaultLevelDirectory(), file);

            // Now read the file
            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            Read(reader);
            reader.Close();
            stream.Close();

            // Initialize blocks
            AddedBlocks = new List<BlockBase>();
            Doodad doodad;
            foreach (BlockBase block in Blocks)
            {
                block.BlockCore.OnlyCollidesWithLowerLayers = false;
                block.Box.Invalidated = true;

                doodad = block as Doodad;
                if (null != doodad)// && doodad.Core.EditorCode1 != "")
                {
                    doodad.InitType();
                }
                else
                {
                    if (block.Core.MyTileSet == TileSets.CastlePiece)
                    {
                        if (string.Compare(block.Core.EditorCode3, "NotTopOnly", StringComparison.OrdinalIgnoreCase) == 0)
                            block.Box.TopOnly = false;
                        else
                            block.Box.TopOnly = true;
                        block.BlockCore.OnlyCollidesWithLowerLayers = true;
                    }

                    if (string.Compare(block.Core.EditorCode1, "DoNotUseTopOnlyTexture", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        block.BlockCore.UseTopOnlyTexture = false;
                        block.Box.TopOnly = true;
                        ((NormalBlock)block).ResetPieces();
                    }

                    if (string.Compare(block.Core.EditorCode1, "TextureOnly", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        block.BlockCore.UseTopOnlyTexture = false;
                        block.Box.TopOnly = true;
                        ((NormalBlock)block).ResetPieces();
                        block.Core.Real = false;
                    }
                }
            }

            foreach (BlockBase block in AddedBlocks)
                AddBlock(block);
            AddedBlocks.Clear();
        }

        public void PurgeEditables()
        {
            foreach (BlockBase block in Blocks)
                if (block.Core.EditHoldable)
                    Recycle.CollectObject(block);

            foreach (ObjectBase obj in Objects)
                if (obj.Core.EditHoldable)
                    Recycle.CollectObject(obj);
        }

        public void Read(BinaryReader reader)
        {
            int Num = reader.ReadInt32();

            for (int i = 0; i < Num; i++)
            {
                ObjectType type = (ObjectType)reader.ReadInt32();
                BlockBase block = (BlockBase)Recycle.GetObject(type, false);
                block.Read(reader);

                AddBlock(block);
            }

            Num = reader.ReadInt32();
             
            for (int i = 0; i < Num; i++)
            {
                ObjectType type = (ObjectType)reader.ReadInt32();
                Tools.Write(type);

                if (type == ObjectType.Undefined || (int)type > 1000)
                {
                    //continue;

                    type = ObjectType.Coin;
                    ObjectBase obj = (ObjectBase)Recycle.GetObject(type, false);
                    obj.Read(reader);
                    continue;
                }
                else
                {
                    //if (type == ObjectType.Undefined) type = ObjectType.Coin;
                    ObjectBase obj = (ObjectBase)Recycle.GetObject(type, false);
                    obj.Read(reader);

                    AddObject(obj, false);
                }
            }

            SortDrawLayers();
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



                
        public void AddPop(Vector2 pos)
        {
            AddPop(pos, 85, PopTemplate.MyQuad.MyTexture);
        }
        public void AddPop(Vector2 pos, float size)
        {
            AddPop(pos, size, PopTemplate.MyQuad.MyTexture);
        }
        public void AddPop(Vector2 pos, float size, EzTexture tex)
        {
            if (NoParticles) return;

            var p = MainEmitter.GetNewParticle(PopTemplate);
            p.Data.Position = pos;
            p.SetSize(size);
            p.MyQuad.MyTexture = tex;
        }
        public void AddPop(Vector2 pos, ref Particle Template)
        {
            if (NoParticles) return;

            var p = MainEmitter.GetNewParticle(Template);
            p.Data.Position = pos;
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

            if (MyGame != null) MyGame.FreeReset = FreeReset;
            FreeReset = false;

            // Don't automatically prevent resets. Every call to reset must itself check if resets are allowed.
            //if (PreventReset) return;

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

                if (MaxAttempts > 0 && NumAttempts >= MaxAttempts)
                {
                    Tools.WorldMap.ReturnTo(-1);
                    return;
                }

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



            Vector2 BobsCenter = new Vector2(0, 0);
            foreach (Bob bob in Bobs)
            {
                bob.EndSuckedIn();

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
                BobsCenter += bob.Core.Data.Position;

                bob.PlayerObject.BoxesOnly = BoxesOnly;
                bob.BoxesOnly = BoxesOnly;
            }
            if (Bobs.Count > 0)
                BobsCenter /= Bobs.Count;




            foreach (BlockBase block in Blocks) if (block.Core.RemoveOnReset) Recycle.CollectObject(block);
            CleanBlockList();
            
            foreach (BlockBase block in Blocks)
            {
                block.Reset(BoxesOnly);
                if (block.BlockCore.Objects != null)
                    block.BlockCore.Objects.Clear();
            }
            /*
            List<Block> NewBlocks = new List<Block>();
            foreach (Block block in Blocks)
            {
                Block newblock = (Block)Recycle.GetObject(block.Core.MyType, BoxesOnly);
                newblock.Clone(block);
                newblock.Reset(BoxesOnly);
                if (newblock.BlockCore.Objects != null)
                    newblock.BlockCore.Objects.Clear();

                NewBlocks.Add(newblock);
                Recycle.CollectObject(block);
            }
            foreach (Block block in NewBlocks) AddBlock(block);
            */
            
            foreach (ObjectBase obj in Objects) if (obj.Core.RemoveOnReset) Recycle.CollectObject(obj);
            CleanAllObjectLists();
            List<ObjectBase> NewObjects = new List<ObjectBase>();

            
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
                    obj.Core.ParentObject = NewObjects.Find(_obj =>
                        _obj.Core.MyGuid == obj.Core.ParentObjId);
                }
            }



            ClearAllObjectLists();
            foreach (ObjectBase obj in NewObjects)
            {
                AddObject(obj, false);
            }
            foreach (ObjectBase block in Blocks)
                ReAddObject(block);

            SortDrawLayers();

            // Create new active object list
            CreateActiveObjectList();


            MainCamera.Target = MainCamera.Data.Position = CurPiece.CamStartPos;

            if (AdditionalReset && MyGame != null)
                MyGame.AdditionalReset();

            if (PlayMode == 0 && !Watching && Recording)
                CurrentRecording.Record(this);

            if (PlayMode != 0)
                for (int i = 0; i < CurPiece.DelayStart; i++)
                    PhsxStep(false);
            else
            {
                PhsxStep(false, false);
            }
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

        static bool DoingSuck = false;
        public static void StartSuck()
        {
            SuckT = 0;
            DoingSuck = true;
        }

        public static void EndSuck()
        {
            DoingSuck = false;
            EndSuckEffect();
        }

        public static float SuckT = 0;
        static float SuckTime()
        {
            float speed = 1.85f;// 1.6;
            //return (float)(1f - Math.Cos(speed * SuckT * 3.14159f)) / 2;
            return (float)(1f - Math.Cos(SuckT * 3.14159f)) / 2;
        }

        static bool SuckActivated = false;
        public static void EnsureSuckEffect()
        {
            //if (ButtonCheck.State(Microsoft.Xna.Framework.Input.Keys.N).Down)
            //    StartSuck();
            if (SuckT > 2)
                EndSuck();
            SuckT += .165f * Tools.dt;

            if (DoingSuck && !SuckActivated)
            {
                Tools.QDrawer.Flush();
                //Tools.BasicEffect.effect.CurrentTechnique = Tools.BasicEffect.effect.Techniques["Suck"];
                Tools.BasicEffect.effect.CurrentTechnique = Tools.BasicEffect.effect.Techniques["PushOut"];
                Tools.BasicEffect.effect.Parameters["SuckTime"].SetValue(SuckTime());
                SuckActivated = true;
            }
        }

        public static void EndSuckEffect()
        {
            if (SuckActivated)
            {
                Tools.QDrawer.Flush();
                Tools.BasicEffect.effect.CurrentTechnique = Tools.BasicEffect.Simplest;
                SuckActivated = false;
            }
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

            if (i == FirstSecondDrawLayer)
            {
                if (Replay && !MainReplayOnly)
                    MySwarmBundle.Draw(CurPhsxStep, this);
            }


            foreach (ObjectBase obj in DrawLayer[i])
                if (obj.Core.Show
#if WINDOWS
                    || Tools.Editing)
#else
                    )
#endif
                    obj.Draw();
            Tools.QDrawer.Flush();

            if (MyGame != null && MyGame.DrawObjectText)
            {
                foreach (ObjectBase obj in DrawLayer[i])
                    obj.TextDraw();
                Tools.EndSpriteBatch();
            }

            if (!Replay || MainReplayOnly)
            {
                if (MyGame != null && MyGame.MyGameFlags.IsTethered)
                foreach (Bob Player in Bobs)
                {
                    if (Player.Core.DrawLayer == i && Player.MyBobLinks != null)
                        foreach (BobLink link in Player.MyBobLinks)
                        {
                            EndSuckEffect();
                            link.Draw();
                        }
                }

                foreach (Bob Bob in Bobs)
                {
                    if (Bob.DrawWithLevel && Bob.Core.DrawLayer == i)
                    {
                        EndSuckEffect();
                        Bob.Draw();
                    }
                }
                Tools.QDrawer.Flush();
                EnsureSuckEffect();
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
        static float[] BobLightRadiusByDifficulty = new float[] { 800, 70, 690, 630, 500 };
        public void SetBobLightRadius(int Difficulty)
        {
            BobLightRadius = BobLightRadiusByDifficulty[Difficulty];
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
                    Color c = new Color(1f, 1f, 1f, bob.LightSourceFade);
                    Tools.QDrawer.DrawLightSource(bob.Pos, 670, 5f, c);//new Color(.75f, .75f, .75f, .75f));
                }
                Tools.QDrawer.Flush();
            }

            Tools.QDrawer.Flush();
            Tools.Device.SetRenderTarget(Tools.DestinationRenderTarget);
            Tools.TheGame.GraphicsDevice.Clear(Color.Black);
            Tools.ResetViewport();
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

            EnsureSuckEffect();
            EndSuckEffect();

            if (MyBackground != null && Tools.DrawGraphics)
            {
//#if DEBUG
                //Background.Test = true;
                //Background.Test = false;
                if (Background.Test || Background.GreenScreen)
                    Background.DrawTest();
                else
//#endif
                MyBackground.Draw();
            }

            if (MyGame != null) MyGame.PreDraw();

            EnsureSuckEffect();

            MainCamera.SetVertexCamera();

            Vector2 HoldBL, HoldTR;
            HoldBL = MainCamera.BL;
            HoldTR = MainCamera.TR;
            if (DrawAll)
            {
                MainCamera.BL = new Vector2(-1000000, -1000000);
                MainCamera.TR = new Vector2(1000000, 1000000);
            }

            for (int i = StartLayer; i <= Tools.Restrict(0, Level.AfterParticlesDrawLayer - 1, EndLayer); i++)
                DrawGivenLayer(i);

            // Draw particles
            if (!NoParticles)
                MainEmitter.Draw();
            Tools.QDrawer.Flush();

            if (UseLighting && LightLayer == LightLayers.FrontOfLevel)
                DrawLighting();   

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
            //level.RemoveForeignObjects();
        }
        public void AddLevelBlocks(Level level, int Tag)
        {
            foreach (BlockBase block in level.Blocks) if (block.Core.Tag == Tag && !block.Core.DoNotScrollOut) AddBlock(block);
            level.RemoveForeignObjects();
        }
        public void AddLevelObjects(Level level)
        {
            AddLevelObjects(level, new Vector2(-10000000, -100000000), new Vector2(10000000, 10000000));
            //level.RemoveForeignObjects();
        }
        public void AddLevelObjects(Level level, Vector2 p1, Vector2 p2)
        {
            foreach (ObjectBase obj in level.Objects) if (IsBetween(obj.Core.Data.Position, p1, p2) && !obj.Core.DoNotScrollOut) AddObject(obj, false);
            //level.RemoveForeignObjects();
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

            //level.RemoveForeignObjects();
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

                // If no BSP check against all objects
                if (MyBSP == null)
                    CheckAgainst(obj, ObjList, MinDistFunc, metric, MustBeDifferent);
                else
                {
                    // Otherwise only check for objects known to be close
                    int j1 = -1, j2 = -1;
                    MyBSP.GetIndexBounds(obj.Core.Data.Position, ref j1, ref j2);

                    for (int j = j1; j <= j2; j++)
                        CheckAgainst(obj, MyBSP.Grid[j], MinDistFunc, metric, MustBeDifferent);
                }
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

            //AddComputerWatchDialogue();
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
            //foreach (IObject Object in Objects)
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
            //foreach (IObject Object in Objects)
            foreach (ObjectBase Object in ActiveObjectList)
            {
                if (!Object.Core.MarkedForDeletion)
                    Object.PhsxStep2();
            }
        }

        void UpdateBobs()
        {
            /*
            if (MyGame != null && MyGame.MyGameFlags.IsDoppleganger ||
                MySourceGame != null && MySourceGame.MyGameFlags.IsDoppleganger)
            {
                Bob xsource = Bobs.ArgMax(bob => Math.Abs(bob.MyPhsx.Pos.X - bob.MyPhsx.PrevPos.X));
                int ysource = Bobs.IndexMax(bob => bob.MyPhsx.Pos.Y - bob.MyPhsx.PrevPos.Y);
                float y;
                if (ysource == 0) y = Bobs[0].Pos.Y + Bobs[0].MyPhsx.Pos.Y - Bobs[0].MyPhsx.PrevPos.Y;
                else              y = Bobs[0].Pos.Y + Bobs[1].MyPhsx.Pos.Y - Bobs[1].MyPhsx.PrevPos.Y;
                float x = xsource.Pos.X;

                float velx = xsource.MyPhsx.xVel;
                float vely = Bobs.Max(bob => bob.MyPhsx.Vel.Y);

                for (int i = 0; i < 2; i++)
                {
                    Bob bob = Bobs[i];
                    bob.Pos = new Vector2(x, y + i * 1000);
                    bob.MyPhsx.xVel = velx;
                    bob.MyPhsx.yVel = vely;
                }
            }*/

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
                        if (PlayMode == 1 && !bob.SuckedIn || PlayMode == 0 && bob.CompControl && !Replay)
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


        //public Bob LowestBob;
        public void PhsxStep(bool NotDrawing) { PhsxStep(NotDrawing, true); }
        public void PhsxStep(bool NotDrawing, bool GUIPhsx)
        {
            //LowestBob = null;
            //LowestBob = Bobs.ArgMax(bob => bob.Pos.Y);

            if (!IndependentStepSetOnce)
                SetIndependentStep();

#if WINDOWS
            if (Tools.Editing)
            {
                EditingPhsxStep();
            }
#endif
            if (LevelReleased) return;

            //if (GUIPhsx && !SuppressReplayButtons)
            //    if (Replay || Watching)
            //        MyReplayGUI.ProcessInput(this);

            if (!SetToReset && Watching && Replay && !ReplayPaused && MySwarmBundle.EndCheck(this))
            {
                if (!MySwarmBundle.GetNextSwarm(this))
                {
                    // End the computer watch if we are video capturing
                    if (Tools.CapturingVideo)
                        EndReplay();
                    else
                        ReplayPaused = true;
                }
                else
                    SetToReset = true;
            }
            if (!SetToReset && Watching && !Replay && EndOfReplay())
            {
                // End the computer watch if we are video capturing
                if (Tools.CapturingVideo)
                    EndComputerWatch();
                else
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
            //CleanAllObjectLists();
            //if (BoxesOnly) Tools.RemoveAll(ActiveObjectList, obj => obj.Core.MarkedForDeletion);
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
            if (LevelCleared && !Replay && !Watching) MainCamera.MyZone = FinalCamZone;
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
                    IndependentPhsxStep = CurPhsxStep;
                    if (!IndependentStepSetOnce)
                        PrevIndependentPhsxStep = IndependentPhsxStep - 1;
                    break;

                case TimeTypes.xSync:
                    if (Bobs.Count > 0 && Bobs[0] != null)
                    {
                        float New = Bobs[0].Pos.X / 10;

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

        public enum TimeTypes { Unset, Regular, xSync };
        public TimeTypes TimeType = TimeTypes.Regular;


        private void EditingPhsxStep()
        {
            foreach (ObjectBase obj in Objects)
                if (obj is BlockBase)
                    Blocks.Add((BlockBase)obj);

            Objects.RemoveAll(obj => obj is BlockBase);
        }




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

            //Console.WriteLine("{0} : {1}", ActiveObjectList.Count, Objects.Count);
        }

        /// <summary>
        /// Set the active object list to all objects in the level
        /// </summary>
        void ResetActiveObjectList()
        {
            if (ActiveObjectList == Objects) return;

            ActiveObjectList.Clear();
            //ActiveObjectList.AddRange(Objects);
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

        public CameraZone LeftMostCameraZone()
        {
            return (CameraZone)GetObjectList(ObjectType.CameraZone).ArgMin(obj => {
                CameraZone zone = obj as CameraZone;
                return zone.Pos.X;
            });
        }

        public void EraseNonDoodadCoins()
        {
            Objects.FindAll(obj => obj is Coin && obj.Core != "DoodadCoin").ForEach(obj => obj.CollectSelf());
        }

        public void CountCoinsAndBlobs()
        {
            NumCoins = 0;
            TotalCoinScore = 0;
            foreach (ObjectBase obj in Objects)
            {
                Coin coin = obj as Coin;
                if (null != coin && !coin.Core.MarkedForDeletion)
                {
                    NumCoins++;
                    TotalCoinScore += coin.MyScoreValue;
                }

                Goomba blob = obj as Goomba;
                if (null != blob)
                {
                    NumBlobs++;
                }
            }
        }

        public void SetBack(int Steps)
        {
            CurPiece.StartPhsxStep -= Steps;
            //MyGame.WaitThenDo(5, () =>
            //    MyGame.ToDoOnReset.Add(() => CurPiece.StartPhsxStep += Steps));
            MyGame.WaitThenDo(2, () => CurPiece.StartPhsxStep += Steps);
        }
    }
}
