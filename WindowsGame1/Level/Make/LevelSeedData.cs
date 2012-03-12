using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class LockableBool
    {
        bool _val = false;
        public bool val
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
            }
        }
    }
    public delegate void PostMakeAction(Level level);
    public partial class LevelSeedData
    {
        /// <summary>
        /// Called when the loading screen is created, just before the level creation algorithm starts.
        /// </summary>
        public Action OnBeginLoad;

        /// <summary>
        /// When true the campaign manager does not add a default PostMake action to this seed.
        /// </summary>
        public bool NoDefaultMake = false;

        /// <summary>
        /// When true this game will not start any music, or stop any previously playing music.
        /// </summary>
        public bool NoMusicStart = false;

        /// <summary>
        /// The created level will loop the given song, starting once the level loads.
        /// </summary>
        public void SetToStartSong(EzSong song) { SetToStartSong(song, 5); }
        public void SetToStartSong(EzSong song, int delay)
        {
            NoMusicStart = true;
            PostMake += lvl => lvl.MyGame.WaitThenDo(delay, () => Tools.SongWad.LoopSong(song));
        }

        public static int Title_WorldNum, Title_LevelNum;
        public static string GetTitle() { return string.Format("World {0}-{1}", Title_WorldNum, Title_LevelNum); }
        public static void NewWorld() { Title_WorldNum++; Title_LevelNum = 1; }
        public static void FirstWorld() { Title_WorldNum = Title_LevelNum = 1; }
        /// <summary>
        /// The created level will show the given title
        /// </summary>
        //public void SetToShowLevelTitle(int delay = 20, string title = null, bool show = true)
        public void SetToShowLevelTitle()
        {
            SetToShowLevelTitle(20, null, true);
        }
        public void SetToShowLevelTitle(int delay)
        {
            SetToShowLevelTitle(delay, null, true);
        }
        public void SetToShowLevelTitle(string title)
        {
            SetToShowLevelTitle(20, title, true);
        }
        public void SetToShowLevelTitle(bool show)
        {
            SetToShowLevelTitle(20, null, show);
        }
        public void SetToShowLevelTitle(int delay, string title, bool show)
        {
            if (title == null) title = GetTitle();

            Name = title;
            
            if (show)
                PostMake += lvl => lvl.MyGame.WaitThenDo(delay, () => lvl.MyGame.AddGameObject(new LevelTitle(title, new Vector2(0, -100), 1, false)), true);
            
            Title_LevelNum++;
        }

        public void DelayEntrance()
        {
            //int Wait = 35;
            int Wait = 38;

            PostMake += lvl => {
                GameData game = lvl.MyGame;

                lvl.PreventReset = true;
                game.AddToDo(() =>
                {
                    game.HideBobs();

                    // If there's a start door then enter through it
                    if (lvl.StartDoor != null)
                    {
                        game.EnterFrom(lvl.StartDoor, Wait);
                        game.CinematicToDo(Wait + 20,
                            () => lvl.PreventReset = false);
                    }
                });

                game.PhsxStepsToDo += 2;
            };

            SetBackFirstAttemp(Wait);
        }

        public void SetBackFirstAttemp(int Steps)
        {
            PostMake += lvl => lvl.SetBack(Steps);
        }

        public string Name = "";

        public PostMakeAction PostMake;

        /// <summary>
        /// Adds the default GameObjects to a level.
        /// </summary>
        public static void AddGameObjects_Default(Level level, bool global)
        {
            level.MyGame.AddGameObject(new HintGiver(),
                                       HelpMenu.MakeListener(),
                                       new PerfectScoreObject(global));

            if (Campaign.IsPlaying)
                level.MyGame.AddGameObject(InGameStartMenu_CampaignLevel.MakeListener());
            else
                level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
        }

        /// <summary>
        /// Adds the bare bones GameObjects to a level. (Menu, Perfect)
        /// </summary>
        public static void AddGameObjects_BareBones(Level level, bool global)
        {
            level.MyGame.AddGameObject(InGameStartMenu.MakeListener(),
                                       new PerfectScoreObject(global));
        }

        /// <summary>
        /// Standard beginning of level music start.
        /// Shuffles and starts the standard playlist.
        /// </summary>
        public static void BOL_StartMusic()
        {
            Tools.SongWad.SetPlayList(Tools.SongList_Standard);
            Tools.SongWad.Shuffle();
            Tools.SongWad.Start(true);
        }

        public static void PostMake_Standard(Level level, bool StartMusic)
        {
            AddGameObjects_Default(level, false);

            if (StartMusic)
                level.MyGame.WaitThenDo(8, BOL_StartMusic);

            ILevelConnector door = (ILevelConnector)level.FindIObject(LevelConnector.EndOfLevelCode);
            door.OnOpen = d => GameData.EOL_DoorAction(d);//, StatGroup.Level);

            level.StartRecording();
        }

        public static void PostMake_StringWorldStandard(Level level)
        {
            AddGameObjects_Default(level, false);

            level.MyGame.WaitThenDo(8, () =>
            {
                if (!Tools.SongWad.IsPlaying())
                {
                    Tools.SongWad.SetPlayList(Tools.SongList_Standard);
                    Tools.SongWad.Shuffle();
                    Tools.SongWad.Start(true);
                }
            });
        }
        
        public bool ReleaseWhenLoaded = false;

        public LockableBool Loaded;
        public bool LoadingBegun = false;
        public GameData MyGame;

        public LevelGeometry MyGeometry = LevelGeometry.Right;

        public int PieceLength = 3000;

        public BobPhsx DefaultHeroType;
        public PlaceTypes PlaceObjectType;

        public GameFlags MyGameFlags;

        public enum LavaMakeTypes { AlwaysMake, NeverMake, Random };
        public LavaMakeTypes LavaMake = LavaMakeTypes.Random;

        /// <summary>
        /// If true then seed used in the random number generator is never changed.
        /// The level will always be made the same way.
        /// </summary>
        public bool LockedSeed = false;

        /// <summary>
        /// The seed fed into the random number generator.
        /// </summary>
        public int Seed { get { return Rnd.MySeed; } set { Rnd = new Rand(value); } }
        public Rand Rnd;

        public GameFactory MyGameType = NormalGameData.Factory;

        public List<PieceSeedData> PieceSeeds;

        public BackgroundType MyBackgroundType;
        public TileSet MyTileSet;
        public TileSetInfo MyTileInfo;

        public void SetBackground(TileSet tileset)
        {
            SetBackground(TileSets.Get(tileset).MyBackgroundType);
        }

        public void SetBackground(BackgroundType type)
        {
            MyBackgroundType = type;
            SetTileSet(Background.GetTileSet(type));
        }

        public void SetTileSet(TileSet tileset)
        {
            MyTileSet = tileset;
            MyTileInfo = TileSets.Get(MyTileSet);
        }

        /// <summary>
        /// Reset the seed so that it may be used again.
        /// </summary>
        public void Reset()
        {
            Loaded.val = false;
            LoadingBegun = false;
            MyGame = null;
            
            ReleasePieces();
            PieceSeeds = new List<PieceSeedData>();
        }

        public void Release()
        {
            ReleasePieces();
            PieceSeeds = null;
            MyGame = null; MyGameType = null;
            PostMake = null;
        }

        public void ReleasePieces()
        {
            if (PieceSeeds != null)
                foreach (PieceSeedData data in PieceSeeds)
                    data.Release();
            PieceSeeds = null;
        }

        //public bool Preloadable { get { return SeedAction == null; } }
        //public Action SeedAction;
        //public LevelSeedData(Action SeedAction)
        //{
        //    this.SeedAction = SeedAction;
        //}


        public LevelSeedData(LevelSeedData data)
        {
            MyGameType = data.MyGameType;
            MyBackgroundType = data.MyBackgroundType;
            MyTileSet = data.MyTileSet;
            MyTileInfo = data.MyTileInfo;
            
            DefaultHeroType = data.DefaultHeroType;
            MyGameFlags = data.MyGameFlags;
            PlaceObjectType = data.PlaceObjectType;

            Length = data.Length;
            PieceLength = data.PieceLength;
            NumPieces = data.NumPieces;

            Difficulty = data.Difficulty;

            MyGeometry = data.MyGeometry;

            BaseInit();
        }

        public LevelSeedData()
        {
            MyBackgroundType = BackgroundType.Dungeon;

            MyGameType = null;
            Seed = Tools.GlobalRnd.Rnd.Next();

            BaseInit();
        }

        void BaseInit()
        {
            PieceSeeds = new List<PieceSeedData>();

            Loaded = new LockableBool();
        }

        public int Difficulty, NumPieces, Length;
        CustomDifficulty MyCustomDifficulty;
        public void PreInitialize(GameFactory Type, int Difficulty, int NumPieces, int Length, CustomDifficulty CustomDiff)
        {
            this.MyGameType = Type;
            this.Difficulty = Difficulty;
            this.NumPieces = NumPieces;
            this.Length = Length;
            this.MyCustomDifficulty = CustomDiff;
        }

        bool Initialized = false;
        public void Init()
        {
            if (Initialized)
                return;

            if (Length == 0)
                throw(new Exception("Invalid length. PreInitialize may not have been called."));

            //if (SeedAction != null) return;

            Initialize(MyGameType, MyGeometry, NumPieces, Length, MyCustomDifficulty);
        }

        /// <summary>
        /// Prevent unreasonable or dangerous parameter combinations.
        /// </summary>
        void Sanitize()
        {
            // Convert random tileset to an actual randomly chosen tileset
            if (MyTileSet == TileSet.Random)
            {
                MyTileSet = new TileSet[] { TileSet.Terrace, TileSet.Dungeon, TileSet.Castle }.Choose(MyGame.Rnd);
                SetBackground(MyTileSet);
            }

            // Convert random hero to an actual randomly chosen hero
            if (DefaultHeroType == BobPhsxRandom.Instance)
            {
                DefaultHeroType = BobPhsxRandom.ChooseHeroType();
            }

            // No spaceships or carts on vertical levels
            if (MyGeometry == LevelGeometry.Up || MyGeometry == LevelGeometry.Down)
            {
                if (DefaultHeroType is BobPhsxSpaceship)
                    DefaultHeroType = BobPhsxDouble.Instance;
                else if (DefaultHeroType is BobPhsxRocketbox)
                    DefaultHeroType = BobPhsxJetman.Instance;
            }

            // Prevent unusual hero types for Build levels
            if (MyGameType == PlaceGameData.Factory)
            {
                if (!PlaceGameData.AllowedHeros.Contains(DefaultHeroType))
                    DefaultHeroType = BobPhsxNormal.Instance;
            }
        }

        public void StandardInit(Action<PieceSeedData, Upgrades> CustomDiff)
        {
            Initialize((CustomDifficulty)(p => 
                {
                    CustomDiff(p, p.u);
                    p.MyUpgrades1.CalcGenData(p.MyGenData.gen1, p.Style);

                    RndDifficulty.ZeroUpgrades(p.MyUpgrades2);
                    p.MyUpgrades1.UpgradeLevels.CopyTo(p.MyUpgrades2.UpgradeLevels, 0);
                    p.MyUpgrades2.CalcGenData(p.MyGenData.gen2, p.Style);

                    p.Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
                    p.Style.MyFinalPlatsType = StyleData.FinalPlatsType.Door;
                }));

        }
        public void Initialize(CustomDifficulty CustomDiff)
        {
            Initialize(MyGameType, MyGeometry, NumPieces, PieceLength, CustomDiff);
        }
        public void Initialize(GameFactory factory, LevelGeometry geometry, int NumPieces, int Length, CustomDifficulty CustomDiff)
        {
            Initialized = true;

            MyGameType = factory;
            MyGeometry = geometry;

            this.NumPieces = NumPieces;
            this.PieceLength = this.Length = Length;

            Sanitize();

            if (factory == NormalGameData.Factory) InitNormal(false, CustomDiff);
            else if (factory == PlaceGameData.Factory) InitPlace(CustomDiff);
            else if (factory == SurvivalGameData.Factory)
            {
                InitNormal(false, CustomDiff);
                this.Length = Length;
            }
        }

        public float CalcPieceLength(PieceSeedData data)
        {
            if (data.Style.MyInitialPlatsType == StyleData.InitialPlatsType.LandingZone)
            {
                return 3000;
            }

            return 0;
        }

        public delegate void CustomDifficulty(PieceSeedData piece);
        public void InitNormal(bool Place, CustomDifficulty CustomDiff)
        { 
            PieceSeedData Piece;

            Vector2 Pos = Vector2.Zero;
            
            for (int i = 0; i < NumPieces; i++)
            {
                Piece = new PieceSeedData(i, MyGeometry, this);
                RndDifficulty.ZeroUpgrades(Piece.MyUpgrades1);
                RndDifficulty.ZeroUpgrades(Piece.MyUpgrades2);

                if (Place)
                {
                    Piece.Style.JumpType = StyleData._JumpType.Always;

                    Piece.Style.MyModParams = (level, p) =>
                        {
                            //NormalBlock_Parameters NParams = (NormalBlock_Parameters)p.Style.FindParams(NormalBlock_AutoGen.Instance);
                            //NParams.DoStage1Fill = false;

                            p.Style.FillxStep *= 3.1f;
                            p.Style.FillyStep *= 1.7f;
                        };
                }
                
                if (CustomDiff != null)
                    CustomDiff(Piece);
                else
                    RndDifficulty.IntToDifficulty(Piece, MyTileSet);
                if (Piece.Paths == -1)
                    Piece.Paths = RndDifficulty.ChoosePaths(Piece);
                
                Piece.Start = Pos;
                if (MyGeometry == LevelGeometry.Right)
                    Pos.X += CalcPieceLength(Piece) + PieceLength;
                else if (MyGeometry == LevelGeometry.Up)
                    Pos.Y += PieceLength;
                else if (MyGeometry == LevelGeometry.Down)
                    Pos.Y -= PieceLength;
                Piece.End = Pos;
                
                Piece.MyGenData.p1 = Piece.Start;
                Piece.MyGenData.p2 = Piece.End;
                
                if (i == 0)
                {
                    Piece.CheckpointsAtStart = false;
                    Piece.InitialCheckpointsHere = true;
                }
                else
                {
                    Piece.CheckpointsAtStart = true;
                    Piece.InitialCheckpointsHere = false;
                }
                PieceSeeds.Add(Piece);

                if (i < NumPieces - 1)
                {
                    Piece = new PieceSeedData(this);
                    Piece.Start = Pos;
                    Piece.Ladder = RndDifficulty.ChooseLadder(Difficulty);
                    Pos += Level.GetLadderSize(Piece.Ladder);
                    PieceSeeds.Add(Piece);
                }
            }          
        }

        public void InitPlace(CustomDifficulty CustomDiff)
        {
            InitNormal(true, CustomDiff);
        }

        Level MakeNewLevel(GameData game)
        {
            game.MyGameFlags = MyGameFlags;
            
            //// Get a new random seed
            //if (!LockedSeed)
            //    Seed = game.Rnd.Rnd.Next();


            //// Initialize the random number generator
            //game.Rnd.Rnd = new Random(Seed);

            // Create the level object
            Level NewLevel = new Level();
            NewLevel.MySourceGame = game;
            NewLevel.DefaultHeroType = DefaultHeroType;
            Camera cam = NewLevel.MainCamera = new Camera();
            cam.Update();

            // Set background and tileset
            NewLevel.MyBackground = Background.Get(MyBackgroundType);
            //NewLevel.MyTileSet = NewLevel.MyBackground.MyTileSet.Type;
            NewLevel.MyTileSet = MyTileSet;

            return NewLevel;
        }

        public GameData Create() { return Create(false); }
        public GameData Create(bool MakeInBackground)
        {
            GameData game = MyGameType(this, MakeInBackground);
            game.EndMusicOnFinish = !NoMusicStart;

            return game;
        }
    }
}