using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Awards;

#if XBOX
#else
using CloudberryKingdom.Viewer;
#endif

#if WINDOWS
using Forms = System.Windows.Forms;
#endif

namespace CloudberryKingdom
{
    public partial class CloudberryKingdomGame
    {
        /// <summary>
        /// The version of the game we are working on now (+1 over the last version uploaded to Steam).
        /// MajorVersion is 0 for beta, 1 for release.
        /// MinorVersion increases with substantial change.
        /// SubVersion increases with any pushed change.
        /// </summary>
        public static Version GameVersion = new Version(0, 2, 4);

        /// <summary>
        /// The command line arguments.
        /// </summary>
        public static string[] args;

        public static bool StartAsBackgroundEditor = false;
        public static bool StartAsTestLevel = false;
        public static bool StartAsBobAnimationTest = false;
        public static bool StartAsFreeplay = false;
#if INCLUDE_EDITOR
        public static bool LoadDynamic = true;
#else
        public static bool LoadDynamic = false;
#endif
        public static bool ShowSongInfo = true;

        public static string TileSetToTest = "cave";
        public static string ModRoot = "Standard";
        public static bool AlwaysSkipDynamicArt = false;

        public static bool HideGui = false;
        public static bool HideForeground = false;
        public static bool UseNewBob = false;

        //public static SimpleGameFactory TitleGameFactory = TitleGameData_Intense.Factory;
        public static SimpleGameFactory TitleGameFactory = TitleGameData_MW.Factory;
        //public static SimpleGameFactory TitleGameFactory = TitleGameData_Forest.Factory;

        bool ShowFPS = false;
        public static float fps;

        public int DrawCount, PhsxCount;

        public ResolutionGroup Resolution;
        public ResolutionGroup[] Resolutions = new ResolutionGroup[4];

#if WINDOWS
        QuadClass MousePointer, MouseBack;
        bool _DrawMouseBackIcon = false;
        public bool DrawMouseBackIcon { get { return _DrawMouseBackIcon; } set { _DrawMouseBackIcon = value; } }
#endif

#if DEBUG || INCLUDE_EDITOR
        public static bool AlwaysGiveTutorials = true;
        public static bool UnlockAll = true;
        public static bool SimpleLoad = true;
        public static bool SimpleAiColors = false;
        public static bool BuildDebug = false;
#else
        public static bool AlwaysGiveTutorials = false;
        public static bool SimpleAiColors = false;
        public static bool RecordIntro = false;
        public static bool UnlockAll = true;
        public static bool SimpleLoad = false;
        public static bool BuildDebug = false;
#endif

        bool LogoScreenUp;

        /// <summary>
        /// When true the initial loading screen is drawn even after loading is finished
        /// </summary>
        public bool LogoScreenPropUp;

        /// <summary>
        /// True when we are still loading resources during the game's initial load.
        /// This is wrapped in a class so that it can be used as a lock.
        /// </summary>
        public WrappedBool LoadingResources;
        public int LoadingOffset;
        public WrappedFloat ResourceLoadedCountRef;

        /// <summary>
        /// The game's initial loading screen. Different than the in-game loading screens seen before levels.
        /// </summary>
        public InitialLoadingScreen LoadingScreen;

        public GraphicsDevice MyGraphicsDevice;
        public GraphicsDeviceManager MyGraphicsDeviceManager;

        int ScreenWidth, ScreenHeight;

        /// <summary>
        /// Font used to display debug info on the screen.
        /// </summary>
        SpriteFont DebugFont;

        Camera MainCamera;

        public event EventHandler<EventArgs> Exiting
        {
            add
            {
                Tools.GameClass.Exiting += value;
            }
            remove
            {
                Tools.GameClass.Exiting -= value;
            }
        }

        public void Exit()
        {
            Tools.GameClass.Exit();
        }

        /// <summary>
        /// Process the command line arguments.
        /// This is used to load different tools, such as the background editor, instead of the main game.
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessArgs(string[] args)
        {
#if DEBUG
            // Artifically simulate different command line arguments.
            //args = new string[] { "test_bob_animation", "mod_root", "Bob" };
            //args = new string[] { "test_level" }; AlwaysSkipDynamicArt = true;
            //args = new string[] { "background_editor" }; //AlwaysSkipDynamicArt = true;
            //args = new string[] { "test_all" }; AlwaysSkipDynamicArt = false;
            //StartAsTestLevel = true;
#endif
            //args = new string[] { "test_all" }; AlwaysSkipDynamicArt = false;
            
            LoadDynamic = true;
            AlwaysSkipDynamicArt = false;


            CloudberryKingdomGame.args = args;

            var list = new List<string>(args); list.Reverse();
            var stack = new Stack<string>(list);
            
            while (stack.Count > 0)
            {
                var arg = stack.Pop();

                switch (arg)
                {
                    case "background_editor": StartAsBackgroundEditor = true; LoadDynamic = true; break;
                    case "test_level": StartAsTestLevel = true; LoadDynamic = true; break;
                    case "test_bob_animation": StartAsBobAnimationTest = true; LoadDynamic = false; break;
                    case "test_all":
                        ShowSongInfo = false;
                        UseNewBob = true;
                        StartAsFreeplay = true; LoadDynamic = true; break;
                    case "test_all_old_bob":
                        ShowSongInfo = false;
                        StartAsFreeplay = true; LoadDynamic = true; break;

                    case "mod_root":
                        LoadDynamic = true;
                        if (stack.Count > 0)
                            ModRoot = stack.Pop();
                        break;

                    default: break;
                }
            }
        }

        public CloudberryKingdomGame()
        {
#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
            Components.Add(new GamerServicesComponent(this));
#endif
            ResourceLoadedCountRef = new WrappedFloat();

            MyGraphicsDeviceManager = new GraphicsDeviceManager(Tools.GameClass);
            MyGraphicsDeviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Tools.GameClass.Content.RootDirectory = "Content";

            Tools.TheGame = this;
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            //graphics.PreferMultiSampling = false;
            //graphics.MyGraphicsDevice.PresentationParameters.MultiSampleCount = 16;

            //graphics.PreferMultiSampling = true;
            //graphics.MyGraphicsDevice.PresentationParameters.MultiSampleCount = 16;
        }

        public void Initialize()
        {
#if WINDOWS
            KeyboardHandler.EventInput.Initialize(Tools.GameClass.Window);
#endif
            Globals.ContentDirectory = Tools.GameClass.Content.RootDirectory;


            

            Tools.LoadEffects(Tools.GameClass.Content, true);

            ButtonString.Init();
            ButtonCheck.Reset();

            // Volume control
            Tools.SoundVolume = new WrappedFloat();
            Tools.SoundVolume.MinVal = 0;
            Tools.SoundVolume.MaxVal = 1;
            Tools.SoundVolume.Val = .7f;

            Tools.MusicVolume = new WrappedFloat();
            Tools.MusicVolume.MinVal = 0;
            Tools.MusicVolume.MaxVal = 1;
            Tools.MusicVolume.Val = 1;
            Tools.MusicVolume.SetCallback = () => Tools.UpdateVolume();

#if DEBUG || INCLUDE_EDITOR
            Tools.SoundVolume.Val = 0;
            Tools.MusicVolume.Val = 0;
#endif

#if PC_VERSION
            // The PC version let's the player specify resolution, key mapping, and so on.
            // Try to load these now.
            PlayerManager.RezData rez;

            //PlayerManager.SavePlayerData = new _SavePlayerData();
            //PlayerManager.SavePlayerData.ContainerName = "PlayerData";
            //PlayerManager.SavePlayerData.FileName = "PlayerData.hsc";
            //PlayerManager.SaveRezAndKeys();
            //rez = PlayerManager.LoadRezAndKeys();
            //Tools.Warning();
            try
            {
                rez = PlayerManager.LoadRezAndKeys();
            }
            catch
            {
                rez = new PlayerManager.RezData();
                rez.Custom = false;
            }
#elif WINDOWS
            PlayerManager.RezData rez = new PlayerManager.RezData();
            rez.Custom = true;
#if DEBUG || INCLUDE_EDITOR
            rez.Fullscreen = false;
#else
            rez.Fullscreen = true;
#endif
            rez.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            rez.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif

            // Some possible resolutions.
            Resolutions[0] = new ResolutionGroup();
            Resolutions[0].Backbuffer = new IntVector2(1280, 800);
            Resolutions[0].Bob = new IntVector2(135, 0);
            Resolutions[0].TextOrigin = Vector2.Zero;
            Resolutions[0].LineHeightMod = 1f;

            Resolutions[0].CopyTo(ref Resolutions[1]);
            Resolutions[1].Backbuffer = new IntVector2(1280, 786);

            Resolutions[0].CopyTo(ref Resolutions[2]);
            Resolutions[2].Backbuffer = new IntVector2(1280, 720);

            Resolutions[0].CopyTo(ref Resolutions[3]);
            Resolutions[3].Backbuffer = new IntVector2(640, 360);

            // Set the default resolution
            Resolution = Resolutions[2];

            MyGraphicsDeviceManager.PreferredBackBufferWidth = Resolution.Backbuffer.X;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = Resolution.Backbuffer.Y;
            MyGraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;

            // Set the actual graphics device,
            // based on the resolution preferences established above.
#if PC_VERSION || WINDOWS
            if (rez.Custom)
            {
                if (!rez.Fullscreen)
                {
                    rez.Height = (int)((720f / 1280f) * rez.Width);
                }

                MyGraphicsDeviceManager.PreferredBackBufferWidth = rez.Width;
                MyGraphicsDeviceManager.PreferredBackBufferHeight = rez.Height;
                MyGraphicsDeviceManager.IsFullScreen = rez.Fullscreen;
            }
            else
            {
                MyGraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                MyGraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                MyGraphicsDeviceManager.IsFullScreen = false;
            }
#if DEBUG || INCLUDE_EDITOR
            if (!MyGraphicsDeviceManager.IsFullScreen)
            {
                MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
                MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            }
#endif
#endif

            //// 640x480
            //MyGraphicsDeviceManager.PreferredBackBufferWidth = 640;
            //MyGraphicsDeviceManager.PreferredBackBufferHeight = 480;
            //// 1280x720
            //MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            //MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;

            MyGraphicsDeviceManager.ApplyChanges();

            // Fill the pools
            ComputerRecording.InitPool();

            fps = 0;
        }

#if NOT_PC && (XBOX || XBOX_SIGNIN)
        void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
        {
            SaveGroup.SaveAll();

            if (Tools.CurGameData != null)
                Tools.CurGameData.OnSignOut(e);
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            int Index = (int)e.Gamer.PlayerIndex;
            string Name = e.Gamer.Gamertag;

            PlayerData data = new PlayerData();
            data.Init(Index);
            PlayerManager.Players[Index] = data;

            SaveGroup.LoadGamer(Name, data);
        }
#endif

        public void LoadInfo()
        {
            Tools.Write("Starting to load info...");
            var t = new System.Diagnostics.Stopwatch();
            t.Start();

            PieceQuad c;

            // Moving block
            c = PieceQuad.MovingBlock = new PieceQuad();
            c.Center.MyTexture = "blue_small";

            // Falling block
            var Fall = new AnimationData_Texture("FallingBlock", 1, 4);
            PieceQuad.FallingBlock = new PieceQuad();
            PieceQuad.FallingBlock.Center.SetTextureAnim(Fall);
            PieceQuad.FallGroup = new BlockGroup();
            PieceQuad.FallGroup.Add(100, PieceQuad.FallingBlock);
            PieceQuad.FallGroup.SortWidths();

            // Bouncy block
            var Bouncy = new AnimationData_Texture("BouncyBlock", 1, 2);
            PieceQuad.BouncyBlock = new PieceQuad();
            PieceQuad.BouncyBlock.Center.SetTextureAnim(Bouncy);
            PieceQuad.BouncyGroup = new BlockGroup();
            PieceQuad.BouncyGroup.Add(100, PieceQuad.BouncyBlock);
            PieceQuad.BouncyGroup.SortWidths();

            // Moving block
            PieceQuad.MovingGroup = new BlockGroup();
            PieceQuad.MovingGroup.Add(100, PieceQuad.MovingBlock);
            PieceQuad.MovingGroup.SortWidths();

            // Elevator
            PieceQuad.Elevator = new PieceQuad();
            PieceQuad.Elevator.Center.Set("palette");
            PieceQuad.Elevator.Center.SetColor(new Color(210, 210, 210));
            PieceQuad.ElevatorGroup = new BlockGroup();
            PieceQuad.ElevatorGroup.Add(100, PieceQuad.Elevator);
            PieceQuad.ElevatorGroup.SortWidths();


//#if INCLUDE_EDITOR
            //if (LoadDynamic)
            {
                //if (!AlwaysSkipDynamicArt)
                //    Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Art);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Backgrounds);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Animations);
                //Tools.TextureWad.LoadAllDynamic(Content, EzTextureWad.WhatToLoad.Tilesets);
            }
//#endif

            t.Stop();
            Tools.Write("... done loading info!");
            Tools.Write("Total seconds: {0}", t.Elapsed.TotalSeconds);
        }

        protected void LoadMusic(bool CreateNewWad)
        {
            if (CreateNewWad)
                Tools.SongWad = new EzSongWad();

            Tools.SongWad.PlayerControl = Tools.SongWad.DisplayInfo = true;

            string path = Path.Combine(Globals.ContentDirectory, "Music");
            string[] files = Directory.GetFiles(path);

            foreach (String file in files)
            {
                int i = file.IndexOf("Music") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    if (CreateNewWad)
                        Tools.SongWad.AddSong(Tools.GameClass.Content.Load<Song>("Music\\" + name), name);
                    else
                        Tools.SongWad.FindByName(name).song = Tools.GameClass.Content.Load<Song>("Music\\" + name);
                }

                ResourceLoadedCountRef.MyFloat++;
            }


            Tools.Song_Happy = Tools.SongWad.FindByName("Happy");
            Tools.Song_Happy.DisplayInfo = false;

            Tools.Song_140mph = Tools.SongWad.FindByName("140 Mph in the Fog^Blind Digital");
            Tools.Song_140mph.Volume = .7f;

            Tools.Song_BlueChair = Tools.SongWad.FindByName("Blue Chair^Blind Digital");
            Tools.Song_BlueChair.Volume = .7f;

            Tools.Song_Evidence = Tools.SongWad.FindByName("Evidence^Blind Digital");
            Tools.Song_Evidence.Volume = .7f;

            Tools.Song_GetaGrip = Tools.SongWad.FindByName("Get a Grip^Peacemaker");
            Tools.Song_GetaGrip.Volume = 1f;

            Tools.Song_House = Tools.SongWad.FindByName("House^Blind Digital");
            Tools.Song_House.Volume = .7f;

            Tools.Song_Nero = Tools.SongWad.FindByName("Nero's Law^Peacemaker");
            Tools.Song_Nero.Volume = 1f;

            Tools.Song_Ripcurl = Tools.SongWad.FindByName("Ripcurl^Blind Digital");
            Tools.Song_Ripcurl.Volume = .7f;

            Tools.Song_FatInFire = Tools.SongWad.FindByName("The Fat is in the Fire^Peacemaker");
            Tools.Song_FatInFire.Volume = .9f;

            Tools.Song_Heavens = Tools.SongWad.FindByName("The Heavens Opened^Peacemaker");
            Tools.Song_Heavens.Volume = 1f;

            Tools.Song_TidyUp = Tools.SongWad.FindByName("Tidy Up^Peacemaker");
            Tools.Song_TidyUp.Volume = 1f;

            Tools.Song_WritersBlock = Tools.SongWad.FindByName("Writer's Block^Peacemaker");
            Tools.Song_WritersBlock.Volume = 1f;

            // Create the standard playlist
            Tools.SongList_Standard.AddRange(Tools.SongWad.SongList);
            Tools.SongList_Standard.Remove(Tools.Song_Happy);
            Tools.SongList_Standard.Remove(Tools.Song_140mph);
        }

        protected void LoadSound(bool CreateNewWad)
        {
            ContentManager manager = new ContentManager(Tools.GameClass.Content.ServiceProvider, Tools.GameClass.Content.RootDirectory);

            if (CreateNewWad)
            {
                Tools.SoundWad = new EzSoundWad(4);
                Tools.PrivateSoundWad = new EzSoundWad(4);
            }

            string path = Path.Combine(Globals.ContentDirectory, "Sound");
            string[] files = Directory.GetFiles(path);
            foreach (String file in files)
            {
                int i = file.IndexOf("Sound") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    if (CreateNewWad)
                        Tools.SoundWad.AddSound(manager.Load<SoundEffect>("Sound\\" + name), name);
                    else
                    {
                        SoundEffect NewSound = manager.Load<SoundEffect>("Sound\\" + name);

                        EzSound CurSound = Tools.SoundWad.FindByName(name);
                        foreach (EzSound ezsound in Tools.PrivateSoundWad.SoundList)
                            if (ezsound.sound == CurSound.sound)
                                ezsound.sound = NewSound;
                        CurSound.sound = NewSound;
                    }
                }

                ResourceLoadedCountRef.MyFloat++;
            }

            if (Tools.SoundContentManager != null)
            {
                Tools.SoundContentManager.Unload();
                Tools.SoundContentManager.Dispose();
            }

            Tools.SoundContentManager = manager;
        }

        protected void LoadAssets(bool CreateNewWads)
        {
            // Load the art!
            PreloadArt();

            // Load the music!
            LoadMusic(CreateNewWads);

            // Load the sound!
            LoadSound(CreateNewWads);
        }

        Thread LoadThread;
        public void LoadContent()
        {
            //BenchmarkLoadSize();
            //Tools.Warning();

            MyGraphicsDevice = MyGraphicsDeviceManager.GraphicsDevice;

            Tools.LoadBasicArt(Tools.GameClass.Content);

            Tools.Render = new MainRender(MyGraphicsDevice);

            Tools.QDrawer = new QuadDrawer(MyGraphicsDevice, 1000);
            Tools.QDrawer.DefaultEffect = Tools.EffectWad.FindByName("NoTexture");
            Tools.QDrawer.DefaultTexture = Tools.TextureWad.FindByName("White");

            Tools.Device = MyGraphicsDevice;
            Tools.t = 0;

            LoadingResources = new WrappedBool(false);
            LoadingResources.MyBool = true;
            LogoScreenUp = true;

            Tools.Render.MySpriteBatch = new SpriteBatch(MyGraphicsDevice);

            ScreenWidth = MyGraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = MyGraphicsDevice.PresentationParameters.BackBufferHeight;

            MainCamera = new Camera(ScreenWidth, ScreenHeight);

            MainCamera.Update();

            // Pre load. This happens before anything appears.
            LoadAssets(true);

            // Benchmarking and preprocessing
            //PreprocessArt();
            //BenchmarkAll();
            //Tools.Warning(); return;
            
            //_LoadThread(); return;

            // Create the initial loading screen
            FontLoad();
            LoadingScreen = new InitialLoadingScreen(Tools.GameClass.Content, ResourceLoadedCountRef);

            // Load resource thread
            LoadThread = Tools.EasyThread(5, "LoadThread", _LoadThread);
        }

        private void _LoadThread()
        {
            Tools.Write("Start");

            Fireball.PreInit();

            // Load art
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Environments");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Bob");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Buttons");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Characters");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Coins");
            //Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Effects");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "HeroItems");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Initial");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Level");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Menu");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Old Art Holdover");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Title");
            Tools.Write("ArtMusic done...");

            // Load the infowad and boxes
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            LoadInfo();
            Tools.Write("Infowad done...");

            TileSets.Init();

            Fireball.InitRenderTargets(MyGraphicsDevice, MyGraphicsDevice.PresentationParameters, 300, 200);

            ParticleEffects.Init();

            PlayerManager.Init();
            Awardments.Init();

            // Load saved files
            SaveGroup.Initialize();

#if NOT_PC && (XBOX || XBOX_SIGNIN)
                        SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
                        SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
#endif


#if PC_VERSION
            // Mouse pointer
            MousePointer = new QuadClass();
            MousePointer.Quad.MyTexture = Tools.TextureWad.FindByName("Hand_Open");
            MousePointer.ScaleToTextureSize();
            MousePointer.Scale(1.5f);

            // Mouse back icon
            MouseBack = new QuadClass();
            MouseBack.Quad.MyTexture = Tools.TextureWad.FindByName("charmenu_larrow_1");
            MouseBack.ScaleYToMatchRatio(40);
            MouseBack.Quad.SetColor(new Color(255, 150, 150, 100));
#endif

            Prototypes.LoadObjects();
            ObjectIcon.InitIcons();

            Tools.Write("Stickmen done...");

            Tools.padState = new GamePadState[4];
            Tools.PrevpadState = new GamePadState[4];
            //Tools.Render.SetStandardRenderStates();

            Tools.Write("Textures done...");

            Console.WriteLine("Total resources: {0}", ResourceLoadedCountRef.MyFloat);

            // Note that we are done loading.
            LoadingResources.MyBool = false;
            Tools.Write("Loading done!");
        }

        /// <summary>
        /// Load the necessary fonts.
        /// </summary>
        private void FontLoad()
        {
            Tools.Font_Grobold42 = new EzFont("Fonts/Grobold_42", "Fonts/Grobold_42_Outline", -50, 40);
            Tools.Font_Grobold42_2 = new EzFont("Fonts/Grobold_42", "Fonts/Grobold_42_Outline2", -50, 40);

            Tools.LilFont = new EzFont("Fonts/LilFont");

            Tools.TheGame.DebugFont = Tools.LilFont.Font;

            Tools.Write("Fonts done...");
        }

        protected void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public bool RunningSlowly = false;
        static TimeSpan _TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f));
        public void Update()
        {
        }

        void DoQuickSpawn()
        {
            if (Tools.CurLevel.ResetEnabled() && Tools.CurLevel.PlayMode == 0 && !Tools.CurLevel.Watching && !Tools.CurGameData.PauseGame && Tools.CurGameData.QuickSpawnEnabled())
            {
                // Note that quickspawn was used, hence we should not give hints to the player to use Quick Spawn in the future.
                Hints.SetQuickSpawnNum(999);

                // Don't count reset against player if it happens within the first half second
                if (Tools.CurLevel.CurPhsxStep < 30)
                    Tools.CurLevel.FreeReset = true;

                // Update player stats
                Tools.CurLevel.CountReset();

                Tools.CurLevel.SetToReset = true;
            }
        }

        /// <summary>
        /// The current game being played.
        /// </summary>
        GameData Game { get { return Tools.CurGameData; } }

        /// <summary>
        /// Update the current game.
        /// </summary>
        void DoGameDataPhsx()
        {
#if INCLUDE_EDITOR
            if (Tools.EditorPause) return;
#endif
            Tools.PhsxCount++;

            if (Tools.WorldMap != null)
                Tools.WorldMap.BackgroundPhsx();

            if (Game != null)
            {
                for (int i = 0; i < Game.PhsxStepsToDo; i++)
                {
                    EzSoundWad.SuppressSounds = (Game.SuppressSoundForExtraSteps && i < Game.PhsxStepsToDo - 1);
                    Game.PhsxStep();
                }
                Game.PhsxStepsToDo = 1;
            }
        }

        /// <summary>
        /// A list of actions to perform. Each time an action is peformed it is removed from the list.
        /// </summary>
        public List<Action> ToDo = new List<Action>();

        private void DoToDoList()
        {
            foreach (Action todo in ToDo)
                todo();
            ToDo.Clear();
        }

        protected void PhsxStep()
        {
            DoToDoList();

#if WINDOWS
            // Save the current keyboard state.
            if (Tools.PrevKeyboardState == null) Tools.PrevKeyboardState = Tools.keybState;

            // Debug tools
#if PC_DEBUG || (WINDOWS && DEBUG) || INCLUDE_EDITOR
            if (DebugModePhsx())
                return;
#endif

            // Do game update.
            if (!Tools.StepControl || (Tools.keybState.IsKeyDownCustom(Keys.Enter) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Enter)))
            {
                DoGameDataPhsx();
            }
            else if (Tools.CurLevel != null)
                Tools.CurLevel.IndependentDeltaT = 0;

            // Quick Spawn
            CheckForQuickSpawn_PC();

            // Determine if the mouse is in the window or not.
            Tools.MouseInWindow =
                Tools.CurMouseState.X > 0 && Tools.CurMouseState.X < Resolution.Backbuffer.X &&
                Tools.CurMouseState.Y > 0 && Tools.CurMouseState.Y < Resolution.Backbuffer.Y;

            // Calculate how much user has scrolled the mouse wheel and moved the mouse.
            Tools.DeltaScroll = Tools.CurMouseState.ScrollWheelValue - Tools.PrevMouseState.ScrollWheelValue;
            Tools.DeltaMouse = Tools.ToWorldCoordinates(new Vector2(Tools.CurMouseState.X, Tools.CurMouseState.Y), Tools.CurLevel.MainCamera) -
                               Tools.ToWorldCoordinates(new Vector2(Tools.PrevMouseState.X, Tools.PrevMouseState.Y), Tools.CurLevel.MainCamera);
            Tools.RawDeltaMouse = new Vector2(Tools.CurMouseState.X, Tools.CurMouseState.Y) -
                                  new Vector2(Tools.PrevMouseState.X, Tools.PrevMouseState.Y);

            Tools.PrevKeyboardState = Tools.keybState;
            Tools.PrevMouseState = Tools.CurMouseState;
#else
            DoGameDataPhsx();
#endif

            // Quick Spawn: Note, we must check this for PC version too, since PC players may use game pads.
            CheckForQuickSpawn_Xbox();

            // Store the previous states of the Xbox controllers.
            for (int i = 0; i < 4; i++)
                if (Tools.PrevpadState[i] != null)
                    Tools.PrevpadState[i] = Tools.padState[i];

            // Update the fireball textures.
            Fireball.TexturePhsx();
        }

        private void CheckForQuickSpawn_PC()
        {
            // Should implement a GameObject that marshalls quickspawns instead.
            Tools.Warning();

            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && Tools.CurLevel.ResetEnabled() &&
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey) && !Tools.PrevKeyboardState.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey))
                DoQuickSpawn();
        }

        private void CheckForQuickSpawn_Xbox()
        {
            // Check for quick spawn on Xbox. This allows the player to reset a level rapidly.
            // For XBox this is done by holding both shoulder buttons.
            bool ShortReset = false;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Get(i).Exists)
                {
                    if (Tools.padState[i].Buttons.LeftShoulder == ButtonState.Pressed && Tools.padState[i].Buttons.RightShoulder == ButtonState.Pressed &&
                        (Tools.PrevpadState[i].Buttons.LeftShoulder != ButtonState.Pressed
                         || Tools.PrevpadState[i].Buttons.RightShoulder != ButtonState.Pressed))
                        ShortReset = true;
                }
            }

            // Do the quick spawn if it was chosen by the player.
            if (ShortReset)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (PlayerManager.Get(i).Exists && PlayerManager.Get(i).IsAlive)
                    {
                        if (Tools.padState[i].Buttons.LeftShoulder != ButtonState.Pressed &&
                            Tools.padState[i].Buttons.RightShoulder != ButtonState.Pressed)
                            ShortReset = false;
                    }
                }

                if (ShortReset && Tools.CurLevel.ResetEnabled())
                    DoQuickSpawn();
            }
        }

#if WINDOWS
        /// <summary>
        /// Whether the mouse should be allowed to be shown, usually only when a menu is active.
        /// </summary>
        public bool ShowMouse = false;

        /// <summary>
        /// Draw the mouse cursor.
        /// </summary>
        void MouseDraw()
        {
            if (!ButtonCheck.MouseInUse) return;
            if (MousePointer == null) return;

            Vector2 Pos = Tools.MouseWorldPos();

            // Draw the mouse hand
            MousePointer.Pos = Pos + new Vector2(.905f * MousePointer.Size.X, -.705f * MousePointer.Size.Y);
            MousePointer.Draw();

            // Draw the mouse dot
            Tools.QDrawer.DrawSquareDot(Pos, Color.Black, 8);

            // Draw the mouse back icon
            //if (DrawMouseBackIcon)
            //{
            //    MouseBack.Pos = MousePointer.Pos + new Vector2(44, 98);
            //    MouseBack.Draw();
            //}

            Tools.QDrawer.Flush();
        }
#endif

        /// <summary>
        /// Whether a song was playing prior to the game window going inactive
        /// </summary>
        public bool MediaPlaying_HoldState = false;

        /// <summary>
        /// Whether this is the first frame the window has been inactive
        /// </summary>
        public bool FirstInactiveFrame = true;

        /// <summary>
        /// Whether this is the first frame the window has been active
        /// </summary>
        public bool FirstActiveFrame = true;

        public double DeltaT = 0;
        public void Draw(GameTime gameTime)
        {
#if DEBUG_OBJDATA
            ObjectData.UpdateWeak();
#endif
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;


            // Set the viewport to the whole screen
            MyGraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = MyGraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = MyGraphicsDevice.PresentationParameters.BackBufferHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            // Clear whole screen to black
            MyGraphicsDevice.Clear(Color.Black);

            //lock (LoadThread)
            {
                LoadThread.Join(10);
            }

            //return;

#if WINDOWS
            if (!ActiveInactive())
                return;
#endif

            // Make the actual view port we draw to, and clear it
            Tools.Render.MakeInnerViewport();
            MyGraphicsDevice.Clear(Color.Black);

            MyGraphicsDevice.Viewport = Tools.Render.MainViewport;

            Tools.DrawCount++;

            if (LogoScreenUp) LogoPhsx();
            else if (LogoScreenPropUp) LoadingScreen.PhsxStep();
#if WINDOWS
            if (Tools.Dlg != null || Tools.DialogUp) return;
#endif

            bool DrawBool = true;
#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
            DrawBool = !Guide.IsVisible;
#endif

            bool DoShit = true;
            bool DrawShit = true;

            if (!LogoScreenUp)
                if (!Tools.CurGameData.Loading)
                    if (DrawBool)
                    {
                        if (DoShit)
                        {
                            // Update controller/keyboard states
                            ButtonCheck.UpdateControllerAndKeyboard();
                            
                            // Update sounds
                            if (!LogoScreenUp)
                                Tools.SoundWad.Update();

                            // Update songs
                            if (Tools.SongWad != null)
                                Tools.SongWad.PhsxStep();
                        }

                        // Track time, changes in time, and FPS
                        Tools.gameTime = gameTime;
                        DrawCount++;

                        float new_t = (float)gameTime.TotalGameTime.TotalSeconds;
                        Tools.dt = new_t - Tools.t;
                        Tools.t = new_t;

                        fps = .3f * fps + .7f * (1000f / (float)Math.Max(.00000231f, gameTime.ElapsedGameTime.TotalMilliseconds));

                        if (DoShit)
                        {
                            // Determine how many phsx steps to take
                            int Reps = 0;
                            if (Tools.PhsxSpeed == 0 && DrawCount % 2 == 0) Reps = 1;
                            else if (Tools.PhsxSpeed == 1) Reps = 1;
                            else if (Tools.PhsxSpeed == 2) Reps = 2;
                            else if (Tools.PhsxSpeed == 3) Reps = 4;

                            // Do the phsx
                            for (int i = 0; i < Reps; i++)
                            {
                                PhsxCount++;
                                PhsxStep();
                            }
                        }
                    }

            // Setup the rendering parameters
            SetupToRender();

            if (LogoScreenUp || LogoScreenPropUp)
            {
                LoadingScreen.Draw();
                return;
            }
            
            if (DrawShit)
            if (Tools.ShowLoadingScreen)
            {
                Tools.CurrentLoadingScreen.PreDraw();
                Tools.CurrentLoadingScreen.Draw(MainCamera);
                return;
            }
            else
            {
                if (Tools.CurGameData != null)
                {
                    Tools.CurGameData.Draw();
                    Tools.CurGameData.PostDraw();
                }
                else
                    MyGraphicsDevice.Clear(Color.Black);
            }

#if DEBUG
            if (BuildDebug || ShowFPS || Tools.ShowNums)
                DrawDebugInfo();
#endif

            if (Tools.CurLevel != null)
            {
                Tools.QDrawer.Flush();
                Tools.StartGUIDraw();
            }

#if PC_VERSION
            if (!Tools.ShowLoadingScreen && ShowMouse && !Tools.CapturingVideo)
                MouseDraw();
            ShowMouse = false;
#endif

            if (!Tools.CapturingVideo)
                if (Tools.SongWad != null)
                    Tools.SongWad.Draw();

            if (Tools.CurLevel != null)
            {
                Tools.EndGUIDraw();
                Tools.QDrawer.Flush();
            }

#if DEBUG && INCLUDE_EDITOR
            if (Tools.background_viewer != null)
                Tools.background_viewer.Draw();
#endif
        }

        private void SetupToRender()
        {
            Vector4 cameraPos = new Vector4(MainCamera.Data.Position.X, MainCamera.Data.Position.Y, MainCamera.Zoom.X, MainCamera.Zoom.Y);//.001f, .001f);

            Tools.Render.SetStandardRenderStates();

            Tools.QDrawer.SetInitialState();
            ComputeFire();

            Tools.EffectWad.SetCameraPosition(cameraPos);

            Tools.SetDefaultEffectParams(MainCamera.AspectRatio);

            Tools.Render.SetStandardRenderStates();
        }

        private void ComputeFire()
        {
            if (!LogoScreenUp)
            {
                if (!Tools.CurGameData.Loading && Tools.CurLevel.PlayMode == 0 && Tools.CurGameData != null && !Tools.CurGameData.Loading && (!Tools.CurGameData.PauseGame || CharacterSelectManager.IsShowing))
                {
                    // Compute fireballs textures
                    MyGraphicsDevice.BlendState = BlendState.Additive;
                    Fireball.DrawFireballTexture(MyGraphicsDevice, Tools.EffectWad);
                    Fireball.DrawEmitterTexture(MyGraphicsDevice, Tools.EffectWad);
                    
                    MyGraphicsDevice.BlendState = BlendState.AlphaBlend;
                }
            }
        }

#if WINDOWS
        /// <summary>
        /// Decide if the game should be active or not.
        /// </summary>
        /// <returns>Returns true if the game is active.</returns>
        private bool ActiveInactive()
        {
            if (!Tools.GameClass.IsActive)
            {
                // The window isn't active, so
                // show the actual mouse (not our custom drawn mouse)
                Tools.GameClass.IsMouseVisible = true;

                if (FirstInactiveFrame)
                {
                    // If a song is playing, stop it,
                    // and note that we should resume once the window becomes active
                    if (Tools.SongWad != null && Tools.SongWad.IsPlaying())
                    {
                        MediaPlaying_HoldState = true;
                        MediaPlayer.Pause();
                    }

                    FirstInactiveFrame = false;
                }

                FirstActiveFrame = true;

                return false;
            }
            else
            {
                // The window is active, so
                // hide the actual mouse (we draw our own custom mouse in game)
                Tools.GameClass.IsMouseVisible = false;

                if (FirstActiveFrame)
                {
                    // If a song was playing previously when the window was active before,
                    // resume that song
                    if (MediaPlaying_HoldState)
                        MediaPlayer.Resume();

                    FirstActiveFrame = false;
                }

                FirstInactiveFrame = true;

                // If we are editing the background show the mouse
                if (Tools.ViewerIsUp)
                    Tools.GameClass.IsMouseVisible = true;

                return true;
            }
        }
#endif
    }
}
