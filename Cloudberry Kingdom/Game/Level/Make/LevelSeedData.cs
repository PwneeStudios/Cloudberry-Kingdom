using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.InGameObjects;

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

    public partial class LevelSeedData
    {
        public bool Saveable = true;

        /// <summary>
        /// These special flags add additional properties to levels made by this seed.
        /// </summary>
        #region Special flags
        public bool HasWall = false; const string WallFlag = "wall";
        public bool FadeIn = false; public float FadeInSpeed = .032f; const string FadeInFlag = "fadein";
        public bool FadeOut = false; public float FadeOutSpeed = .02f; const string FadeOutFlag = "fadeout";
        public float WeatherIntensity = 1; const string WeatherIntensityFlag = "weather";
        public bool NoStartDoor = false; const string NoStartDoorFlag = "nostartdoor";
        public int LevelNum = -1; const string LevelFlag = "level";
        public bool NewHero = false; const string NewHeroFlag = "newhero";
        public bool Darkness = false; const string DarknessFlag = "darkness";

        /// <summary>
        /// How long to wait before opening the initial door.
        /// </summary>
        public int WaitLengthToOpenDoor = 6; const string WaitLengthToOpenDoorString = "opendoor";
        public bool AlwaysOverrideWaitDoorLength = false;
        public bool OpenDoorSound = false; const string OpenDoorSoundFlag = "opendoorsound";

        /// <summary>
        /// Song to play when this level starts. Regular sound track will resume on completion.
        /// </summary>
        public EzSong MySong = null; const string SongString = "song";

        public void ProcessSpecial()
        {
            if (HasWall)
            {
                var p = PieceSeeds[0];
                //p.Style.ComputerWaitLengthRange = new Vector2(4, 23);
                
                p.Style.MyModParams = _HasWall_Process;
            }

            if (NewHero) PostMake += _NewHero;

            if (NoStartDoor) PostMake += _NoStartDoor;

            if (FadeIn) PostMake += _FadeIn_Process;

            if (FadeOut) PostMake += _FadeOut_Process;

            if (WeatherIntensity != 1) PostMake += _SetWeather_Process;

            if (MySong != null) PostMake += _StartSong;
        }

        private void _StartSong(Level level)
        {
            Tools.SongWad.SetPlayList(Tools.SongList_Standard);
            Tools.SongWad.Next(MySong);
            Tools.SongWad.PlayNext = true;
        }

        private static void _HasWall_Process(Level level, PieceSeedData piece)
        {
            var Params = (NormalBlock_Parameters)piece.Style.FindParams(NormalBlock_AutoGen.Instance);
            Wall wall = Params.SetWall(LevelGeometry.Right);
            wall.Space = 20; wall.MyBufferType = Wall.BufferType.Space;
            piece.CamZoneStartAdd.X = -2000;
            wall.StartOffset = -600;
            wall.Speed = 17.5f;
            wall.InitialDelay = 72;
        }

        private static void _SetWeather_Process(Level level)
        {
            level.MyBackground.SetWeatherIntensity(level.MyLevelSeed.WeatherIntensity);
        }

        private static void _NoStartDoor(Level level)
        {
            var door = level.StartDoor; if (door == null) return;
            door.CollectSelf();
        }

        private static void _NewHero(Level level)
        {
            level.MyGame.AddGameObject(new NewHero(Localization.WordString(Localization.Words.NewHeroUnlocked) + "\n" + Localization.WordString(level.DefaultHeroType.Name)));
            level.MyLevelSeed.WaitLengthToOpenDoor = 150;
            level.MyLevelSeed.AlwaysOverrideWaitDoorLength = true;
        }

        private static void _FadeIn_Process(Level level)
        {
            level.MyGame.PhsxStepsToDo += 2;
            level.MyGame.AddGameObject(new FadeInObject());
        }

        private static void _FadeOut_Process(Level level)
        {
            StringWorldGameData stringworld = Tools.WorldMap as StringWorldGameData;
            var door = level.FindIObject(LevelConnector.EndOfLevelCode) as Door;

            if (null != stringworld && null != door)
            {
                door.OnEnter = stringworld.EOL_StringWorldDoorEndAction_WithFade;
            }
        }
        #endregion

        /// <summary>
        /// Set default parameters for this LevelSeedData assuming we are about to read in parameters from a string.
        /// </summary>
        public void DefaultRead(string str)
        {
            int i = Math.Abs(str.GetHashCode());

            // Length
            Length = PieceLength = (int)(((uint)(i * 997)) % 7000 + 5000);

            // Tileset
            i /= 2;
            //var tilesets = TileSets.NameLookup.Keys.ToList();
            var tileset = CustomLevel_GUI.FreeplayTilesets[(i + 23) % CustomLevel_GUI.FreeplayTilesets.Count];
            if (tileset == TileSets.Random)
                tileset = CustomLevel_GUI.FreeplayTilesets[(i + 2323) % CustomLevel_GUI.FreeplayTilesets.Count];
            if (tileset == TileSets.Random)
                tileset = CustomLevel_GUI.FreeplayTilesets[CustomLevel_GUI.FreeplayTilesets.Count - 1];
            SetTileSet(tileset);

            // Geometry, gametype
            i /= 3; i *= 2;
            MyGeometry = LevelGeometry.Right;
            MyGameType = NormalGameData.Factory;
            LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;
            
            // Hero
            DefaultHeroType = CustomLevel_GUI.FreeplayHeroes[(i + 555) % CustomLevel_GUI.FreeplayHeroes.Count];
            RandomHero(DefaultHeroType, i * i + 3 * i);

            // Pieces
            NumPieces = i % 2 + 1;

            // Seed
            Seed = i % 7777777;

            PieceHash = i * Seed;
        }

        /// <summary>
        /// Read in parameters from a string.
        /// </summary>
        public void ReadString(string str)
        {
            DefaultRead(str);
            UpgradeStrs.Clear();

            str = Tools.RemoveComment_SlashStyle(str);
            var bits = str.Split(';');

            for (int i = 0; i < bits.Length; i++)
            {
                string identifier, data;

                int index = bits[i].IndexOf(":");

                if (index <= 0)
                {
                    identifier = bits[i];
                    data = "";
                }
                else
                {
                    identifier = bits[i].Substring(0, index);
                    data = bits[i].Substring(index + 1);
                }
                
                string[] terms;

                switch (identifier.ToLower())
                {
                    // Seed [This must come first]
                    case "s":
                        try
                        {
                            Seed = int.Parse(data);
                        }
                        catch
                        {
                            Seed = data.GetHashCode();
                        }
                        break;

                    // Game type
                    case "g":
                        try
                        {
                            MyGameType = GameData.FactoryDict[data];
                        }
                        catch
                        {
                            MyGameType = NormalGameData.Factory;
                        }
                        break;

                    // Geometry
                    case "geo":
                        try
                        {
                            MyGeometry = (LevelGeometry)int.Parse(data);
                        }
                        catch
                        {
                            MyGeometry = LevelGeometry.Right;
                        }
                        break;

                    // Hero [This must come before "ph:"]
                    case "h":
                        terms = data.Split(',');
                        if (terms.Length == 4)
                            DefaultHeroType = BobPhsx.MakeCustom(terms[0], terms[1], terms[2], terms[3]);
                        else
                            DefaultHeroType = BobPhsxNormal.Instance;
                        break;

                    // Custom physics [This must come after "h:"]
                    case "ph":
                        var custom = new BobPhsx.CustomPhsxData();
                        custom.Init(data);
                        DefaultHeroType.SetCustomPhsx(custom);
                        break;

                    // Tileset
                    case "t":
                        MyTileSet = null;
                        if (data.Length > 0)
                        {
                            try
                            {
                                SetTileSet(data);
                            }
                            catch
                            {
                                MyTileSet = null;
                            }
                        }
                        if (MyTileSet == null)
                            SetTileSet("castle");

                        break;

                    // Number of pieces
                    case "n":
                        try
                        {
                            NumPieces = int.Parse(data);
                            NumPieces = CoreMath.Restrict(1, 5, NumPieces);
                        }
                        catch
                        {
                            NumPieces = 1;
                        }
                        break;

                    // Length
                    case "l":
                        try
                        {
                            Length = int.Parse(data);
                            Length = CoreMath.Restrict(2000, 50000, Length);
                            PieceLength = Length;
                        }
                        catch
                        {
                            PieceLength = Length = 5000;
                        }
                        break;

                    // Upgrades
                    case "u":
                        UpgradeStrs.Add(data);
                        break;

                    // Wall
                    case WallFlag: HasWall = true; break;

                    // Fade In
                    case FadeInFlag:
                        FadeIn = true;
                        float DefaultFadeInSpeed = FadeInSpeed;
                        try
                        {
                            FadeInSpeed = float.Parse(data);
                        }
                        catch
                        {
                            FadeInSpeed = DefaultFadeInSpeed;
                        }
                        break;

                    // Fade Out
                    case FadeOutFlag:
                        FadeOut = true;
                        float DefaultFadeOutSpeed = FadeOutSpeed;
                        try
                        {
                            FadeOutSpeed = float.Parse(data);
                        }
                        catch
                        {
                            FadeOutSpeed = DefaultFadeOutSpeed;
                        }
                        break;

                    // NewHero
                    case NewHeroFlag: NewHero = true; break;

                    // Darkness
                    case DarknessFlag: Darkness = true; break;

                    // No start door
                    case NoStartDoorFlag: NoStartDoor = true; break;

                    // Level number
                    case LevelFlag: LevelNum = int.Parse(data); break;

                    // Weather intensity
                    case WeatherIntensityFlag:
                        try
                        {
                            WeatherIntensity = float.Parse(data);
                        }
                        catch
                        {
                            WeatherIntensity = 1;
                        }
                        break;

                    // Wait length to open door
                    case WaitLengthToOpenDoorString:
                        try
                        {
                            WaitLengthToOpenDoor = int.Parse(data);
                        }
                        catch
                        {
                            WaitLengthToOpenDoor = 6;
                        }
                        break;

                    // Song to play at beginning of level
                    case SongString:
                        try
                        {
                            MySong = Tools.SongWad.FindByName(data);
                        }
                        catch
                        {
                            MySong = null;
                        }
                        break;

                    // Open door sound
                    case OpenDoorSoundFlag: OpenDoorSound = true; break;

                    default: break;
                }
            }

            // Error catch.
            if (DefaultHeroType is BobPhsxMeat) { MyGeometry = LevelGeometry.Up; NumPieces = 1; }
            if (DefaultHeroType is BobPhsxRocketbox) { MyGeometry = LevelGeometry.Right; NumPieces = 1; }

            // If no upgrade was provided, zero everything.
            if (UpgradeStrs.Count == 0)
            {
                UpgradeStrs.Add("");
                Initialize(ModPieceViaHash);
            }
            else
            {
                Initialize(ModPieceViaString);
            }

            ProcessSpecial();
        }

        public override string ToString()
        {
            int _version = 0;
            string version = _version.ToString() + ";";

            // Seed
            string seed = "s:" + Seed.ToString() + ";";

            // Game
            string game = "";
            if (MyGameType != NormalGameData.Factory)
            {
                try
                {
                    game = "g:" + GameData.FactoryDict.Keys.Where(k => GameData.FactoryDict[k] == MyGameType).First();
                    game += ";";
                }
                catch
                {
                    return "!This level can not be saved!";
                }
            }

            // Geometry
            string geometry = "";
            if (MyGeometry != LevelGeometry.Right)
                geometry = "geo:" + (int)MyGeometry + ";";

            // Hero
            string hero = "h:" + DefaultHeroType.Specification.ToString() + ";";

            // Custom phsx
            string customphsx = "";
            if (DefaultHeroType.CustomPhsx) customphsx = DefaultHeroType.MyCustomPhsxData.ToString();

            // Tileset
            string tileset = "t:" + MyTileSet.Name + ";";

            // Pieces
            string pieces = "n:" + NumPieces + ";";

            // Length
            string length = "l:" + Length.ToString() + ";";

            // Upgrades
            string upgrades = "";
            foreach (PieceSeedData p in PieceSeeds)
            {
                if (p.Ladder != Level.LadderType.None) continue;

                upgrades += "u:";

                float[] upgrade_levels = p.MyUpgrades1.UpgradeLevels;
                for (int i = 0; i < upgrade_levels.Length; i++)
                {
                    upgrades += upgrade_levels[i].ToString();
                    if (i + 1 < upgrade_levels.Length) upgrades += ",";
                }
                upgrades += ";";
            }

            // Build final string
            string str = version + seed + game + geometry + hero + customphsx + tileset + pieces + length + upgrades;

            // Add special flags
            if (HasWall) str += WallFlag + ";";

            return str;
        }        

        /// <summary>
        /// While reading in parameters from a string, the portion of the string storing upgrade data is stored in this string.
        /// </summary>
        List<string> UpgradeStrs = new List<string>();

        /// <summary>
        /// Modify a PieceSeedData to conform to the upgrade data stored in UpgradeStr.
        /// </summary>
        void ModPieceViaString(PieceSeedData piece)
        {
            // Break the data up by commas
            int index = CoreMath.Restrict(0, UpgradeStrs.Count - 1, piece.MyPieceIndex);
            var terms = UpgradeStrs[index].Split(',');

            // Try and load the data into the upgrade array.
            try
            {
                for (int i = 0; i < terms.Length; i++)
                    piece.MyUpgrades1.UpgradeLevels[i] = float.Parse(terms[i]);
            }
            catch
            {
                // If we fail, zero all the upgrades.
                piece.MyUpgrades1.Zero();
            }

            piece.StandardClose();
        }

        /// <summary>
        /// Modify a PieceSeedData to have random upgrades.
        /// </summary>
        void ModPieceViaHash(PieceSeedData piece)
        {
            PieceHash = Math.Abs(PieceHash);
            PieceHash *= 997;
            PieceHash %= 1024;
            PieceHash = Math.Abs(PieceHash);

            int Bias = (int)(75 - (uint)PieceHash % 50);

            for (int i = 0; i < piece.MyUpgrades1.UpgradeLevels.Length; i++)
            {
                int n1 = PieceHash | (2 + 8 + 32 + 128 + 512);
                int n2 = PieceHash | (1 + 4 + 16 + 64 + 256);

                int value = 0;
                if ((n1 * n2 + (int)Math.Exp(i)) % 100 > Bias)
                    value = (n1 + n1 * n2) % 10;

                piece.MyUpgrades1.UpgradeLevels[i] = CoreMath.Restrict(0, 10, value);
            }

            piece.MyUpgrades1[Upgrade.Firesnake] =
            piece.MyUpgrades1[Upgrade.Conveyor] = 0;

            piece.StandardClose();
        }
        int PieceHash = 1;

        /// <summary>
        /// Make a random hero.
        /// </summary>
        void RandomHero(BobPhsx Hero, int Hash)
        {
            int Length = BobPhsx.CustomPhsxData.Length;
            float[] vals = new float[Length];

            for (int i = 0; i < Length; i++)
            {
                var value =  i * PieceHash * PieceHash + PieceHash + i * i;

                vals[i] = CoreMath.Restrict(BobPhsx.CustomPhsxData.Bounds(i).MinValue, BobPhsx.CustomPhsxData.Bounds(i).MaxValue, value);
                vals[i] = CoreMath.Restrict(0, 1, value);
            }

            Hero.MyCustomPhsxData.Init(vals);
        }

        public string SuggestedName()
        {
            return DefaultHeroType.Name + "_" + Seed.ToString();
        }

        public static string GetNameFromSeedStr(string seed)
        {
            int index_name = seed.IndexOf("name:") + 5;
            int index_name_end = seed.IndexOf(";", index_name);
            string name = seed.Substring(index_name, index_name_end - index_name);

            return name;
        }

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

        public string Name = "";

        public Action<Level> PostMake;

        /// <summary>
        /// Adds the default GameObjects to a level.
        /// </summary>
        public static void AddGameObjects_Default(Level level, bool global, bool ShowMultiplier)
        {
            level.MyGame.AddGameObject(new HintGiver(),
                                       HelpMenu.MakeListener(),
                                       new PerfectScoreObject(global, ShowMultiplier));

            level.MyGame.AddGameObject(InGameStartMenu.MakeListener());
        }

        /// <summary>
        /// Adds the bare bones GameObjects to a level. (Menu, Perfect)
        /// </summary>
        public static void AddGameObjects_BareBones(Level level, bool global)
        {
            level.MyGame.AddGameObject(InGameStartMenu.MakeListener(),
                                       new PerfectScoreObject(global, true));
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

        /// <summary>
        /// Allow the user to load different levels from the menu within this level.
        /// </summary>
        public void PostMake_EnableLoad(Level level)
        {
            level.CanLoadLevels = true;
        }

        public void PostMake_StandardLoad(Level level)
        {
            LevelSeedData.PostMake_Standard(level, true, false);
            level.MyGame.MakeScore = () => new ScoreScreen(StatGroup.Level, level.MyGame);
        }

        public static void PostMake_Standard(Level level, bool StartMusic, bool ShowMultiplier)
        {
            AddGameObjects_Default(level, false, ShowMultiplier);

            if (StartMusic)
                level.MyGame.WaitThenDo(8, BOL_StartMusic);

            ILevelConnector door = (ILevelConnector)level.FindIObject(LevelConnector.EndOfLevelCode);
            door.OnOpen = d => GameData.EOL_DoorAction(d);

            level.StartRecording();
        }

        public static void PostMake_StringWorldStandard(Level level)
        {
            AddGameObjects_Default(level, false, true);

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

        public BackgroundTemplate MyBackgroundType;
        public TileSet MyTileSet;

        public void SetTileSet(string name)
        {
            if (name == null) SetTileSet((TileSet)null);
            else SetTileSet(TileSets.NameLookup[name]);
        }

        public void SetTileSet(TileSet tileset)
        {
            MyTileSet = tileset;
            MyBackgroundType = MyTileSet == null ? null : MyTileSet.MyBackgroundType;
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

        public LevelSeedData(LevelSeedData data)
        {
            Seed = data.Seed;

            MyGameType = data.MyGameType;
            MyBackgroundType = data.MyBackgroundType;
            MyTileSet = data.MyTileSet;
            MyTileSet = data.MyTileSet;
            
            DefaultHeroType = data.DefaultHeroType;
            MyGameFlags = data.MyGameFlags;

            Length = data.Length;
            PieceLength = data.PieceLength;
            NumPieces = data.NumPieces;

            Difficulty = data.Difficulty;

            MyGeometry = data.MyGeometry;

            BaseInit();
        }

        public LevelSeedData()
        {
            MyBackgroundType = BackgroundType.Random;

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

            if (MyGameType == ActionGameData.Factory) return;

            if (Length == 0)
                throw(new Exception("Invalid length. PreInitialize may not have been called."));

            Initialize(MyGameType, MyGeometry, NumPieces, Length, MyCustomDifficulty);
        }

        /// <summary>
        /// Prevent unreasonable or dangerous parameter combinations.
        /// </summary>
        void Sanitize()
        {
            int TestNumber;
            
            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Pre-sanitize: {0}", TestNumber));

            // Convert random tileset to an actual randomly chosen tileset
            // use global RND.
            if (MyTileSet == TileSets.Random)
            {
                MyTileSet = CustomLevel_GUI.FreeplayTilesets[Tools.GlobalRnd.RndInt(1, CustomLevel_GUI.FreeplayTilesets.Count - 1)];
                //MyTileSet = CustomLevel_GUI.FreeplayTilesets.Choose(Rnd);
                //MyTileSet = new TileSet[] { TileSets.Terrace, TileSets.Dungeon, TileSets.Castle }.Choose(Rnd);

                SetTileSet(MyTileSet);
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

            TestNumber = Rnd.RndInt(0, 1000);
            Tools.Write(string.Format("Post-sanitize: {0}", TestNumber));
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
                    DefaultHeroType.ModLadderPiece(Piece);
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
            //NewLevel.MyTileSet = NewLevel.MyBackground.MyTileSet;
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