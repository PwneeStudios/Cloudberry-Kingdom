using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Awards;
using CloudberryKingdom.InGameObjects;
using CloudberryKingdom.Obstacles;

#if WINDOWS && DEBUG
using CloudberryKingdom.Viewer;
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

        public static float fps;

        public int DrawCount, PhsxCount;

        public ResolutionGroup Resolution;
        public ResolutionGroup[] Resolutions = new ResolutionGroup[4];

#if WINDOWS
        public QuadClass MousePointer, MouseBack;
        bool _DrawMouseBackIcon = false;
        public bool DrawMouseBackIcon { get { return _DrawMouseBackIcon; } set { _DrawMouseBackIcon = value; } }
#endif

#if DEBUG || INCLUDE_EDITOR
        public static bool AlwaysGiveTutorials = true;
        public static bool UnlockAll = true;
        public static bool SimpleAiColors = false;
#else
        public static bool AlwaysGiveTutorials = false;
        public static bool UnlockAll = true;
        public static bool SimpleAiColors = false;
#endif

        bool LogoScreenUp;

        /// <summary>
        /// When true the initial loading screen is drawn even after loading is finished
        /// </summary>
        public bool LogoScreenPropUp;

        /// <summary>
        /// The game's initial loading screen. Different than the in-game loading screens seen before levels.
        /// </summary>
        public InitialLoadingScreen LoadingScreen;

        public GraphicsDevice MyGraphicsDevice;
        public GraphicsDeviceManager MyGraphicsDeviceManager;

        int ScreenWidth, ScreenHeight;

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

#if DEBUG
            //// 640x480
            //MyGraphicsDeviceManager.PreferredBackBufferWidth = 640;
            //MyGraphicsDeviceManager.PreferredBackBufferHeight = 480;
            //// 1280x720
            MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            MyGraphicsDeviceManager.IsFullScreen = false;
#endif
            MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            MyGraphicsDeviceManager.IsFullScreen = false;


#if WINDOWS
            Tools.GameClass.SetBorder(Tools.WindowBorder);
#endif

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

        public void LoadContent()
        {
            var Ck = Tools.TheGame;

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

            LogoScreenUp = true;

            Tools.Render.MySpriteBatch = new SpriteBatch(MyGraphicsDevice);

            ScreenWidth = MyGraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = MyGraphicsDevice.PresentationParameters.BackBufferHeight;

            MainCamera = new Camera(ScreenWidth, ScreenHeight);

            MainCamera.Update();

            // Pre load. This happens before anything appears.
            Resources.LoadAssets(true);

            // Initialize players
            PlayerManager.Init();

            // Initialize heroes
            BobPhsx.CustomPhsxData.InitStatic();

            // Load saved files
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            SaveGroup.Initialize();

            // Localization
            Localization.SetLanguage(Localization.Language.English);
            //Localization.SetLanguage(Localization.Language.Portuguese);

            // Benchmarking and preprocessing
            //PreprocessArt();
            //BenchmarkAll();
            //Tools.Warning(); return;

            //_LoadThread(); return;

            // Create the initial loading screen
            LoadingScreen = new InitialLoadingScreen(Tools.GameClass.Content, Resources.ResourceLoadedCountRef);

            // Load resource thread
            Resources.LoadResources();

#if WINDOWS
            Thread.Sleep(2);
#endif

            MainVideo.StartVideo_CanSkipIfWatched("LogoSalad");
        }

        protected void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
#if PC_DEBUG || (WINDOWS && DEBUG) || INCLUDE_EDITOR
            // Debug tools
            if (DebugModePhsx())
                return;
#endif

            // Do game update.
            if (!Tools.StepControl || (Tools.Keyboard.IsKeyDownCustom(Keys.Enter) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.Enter)))
            {
                DoGameDataPhsx();
            }
            else if (Tools.CurLevel != null)
                Tools.CurLevel.IndependentDeltaT = 0;

#if WINDOWS
            // Quick Spawn
            CheckForQuickSpawn_PC();
#endif
#else
            DoGameDataPhsx();
#endif

            // Quick Spawn: Note, we must check this for PC version too, since PC players may use game pads.
            CheckForQuickSpawn_Xbox();

            // Finish updating the controlls; swap current to previous.
            ButtonCheck.UpdateControllerAndKeyboard_EndOfStep(Resolution);

            // Update the fireball textures.
            Fireball.TexturePhsx();
        }
        
#if WINDOWS
        private void CheckForQuickSpawn_PC()
        {
            // Should implement a GameObject that marshalls quickspawns instead.
            Tools.Warning();

            if (!Tools.ViewerIsUp && !KeyboardExtension.Freeze && Tools.CurLevel.ResetEnabled() &&
                Tools.Keyboard.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey) && !Tools.PrevKeyboard.IsKeyDownCustom(ButtonCheck.Quickspawn_KeyboardKey.KeyboardKey))
                DoQuickSpawn();
        }
#endif

        private void CheckForQuickSpawn_Xbox()
        {
            // Check for quick spawn on Xbox. This allows the player to reset a level rapidly.
            // For XBox this is done by holding both shoulder buttons.
            bool ShortReset = false;
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Get(i).Exists)
                {
                    if (Tools.GamepadState[i].Buttons.LeftShoulder == ButtonState.Pressed && Tools.GamepadState[i].Buttons.RightShoulder == ButtonState.Pressed &&
                        (Tools.PrevGamepadState[i].Buttons.LeftShoulder != ButtonState.Pressed
                         || Tools.PrevGamepadState[i].Buttons.RightShoulder != ButtonState.Pressed))
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
                        if (Tools.GamepadState[i].Buttons.LeftShoulder != ButtonState.Pressed &&
                            Tools.GamepadState[i].Buttons.RightShoulder != ButtonState.Pressed)
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

        public bool RunningSlowly = false;
        public void Update()
        {
            //var TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f / 60f));
            //var TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f / 10f));
            //Tools.GameClass.TargetElapsedTime = TargetElapsedTime;
            //Tools.GameClass.IsFixedTimeStep = true;
        }

        /// <summary>
        /// The main draw loop.
        /// Sets all the rendering up and determines which sub-function to call (game, loading screen, nothing, etc).
        /// Also updates the game logic. TODO: Seperate this from the draw function?
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
#if DEBUG_OBJDATA
            ObjectData.UpdateWeak();
#endif
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;

            // Accelerate asset loading
            if (LogoScreenUp)
                Resources.LoadThread.Join(1);

            // Prepare to draw
            Tools.DrawCount++;
            if (SetupToRender()) return;

            // Main Video
            if (MainVideo.Draw()) return;

            // Fps
            UpdateFps(gameTime);

            // Draw nothing if Xbox guide is up
#if XBOX || XBOX_SIGNIN
            if (Guide.IsVisible) return;
#endif

            // What to do
            if (LogoScreenUp)
                LogoPhsx();
            else if (LogoScreenPropUp)
                LoadingScreen.PhsxStep();
            if (!LogoScreenUp && !Tools.CurGameData.Loading)
                GameUpdate(gameTime);

            // What to draw
            if (LogoScreenUp || LogoScreenPropUp)
                LoadingScreen.Draw();
            else if (Tools.ShowLoadingScreen)
                DrawLoading();
            else if (Tools.CurGameData != null)
                DrawGame();
            else
                DrawNothing();

            DrawExtra();
        }

        /// <summary>
        /// Non-game drawing, such as debug info and tool drawing.
        /// </summary>
        private void DrawExtra()
        {
#if DEBUG
            if (ShowFPS || Tools.ShowNums)
                DrawDebugInfo();
#endif

#if DEBUG && INCLUDE_EDITOR
            if (Tools.background_viewer != null)
                Tools.background_viewer.Draw();
#endif
            Tools.Nothing();
        }

        /// <summary>
        /// Draws the load screen, assuming the game should not be drawn this frame.
        /// </summary>
        private void DrawLoading()
        {
            Tools.CurrentLoadingScreen.PreDraw();
            Tools.CurrentLoadingScreen.Draw(MainCamera);
        }

        /// <summary>
        /// Draws nothing (black). Called when the game shouldn't be shown, nor anything else, such as load screens.
        /// </summary>
        private void DrawNothing()
        {
            MyGraphicsDevice.Clear(Color.Black);
        }

        /// <summary>
        /// Draws the actual the game, not any loading screens or other non-game graphics.
        /// </summary>
        private void DrawGame()
        {
            Tools.CurGameData.Draw();
            Tools.CurGameData.PostDraw();

            if (Tools.CurLevel != null)
            {
                Tools.QDrawer.Flush();
                Tools.StartGUIDraw();

#if PC_VERSION
                if (!Tools.ShowLoadingScreen && ShowMouse)
                    MouseDraw();
                ShowMouse = false;
#endif

                if (Tools.SongWad != null)
                    Tools.SongWad.Draw();

                Tools.EndGUIDraw();
                Tools.QDrawer.Flush();
            }
        }

        /// <summary>
        /// The update function called for the actual game, not for loading screens or other non-game functions.
        /// </summary>
        /// <param name="gameTime"></param>
        private void GameUpdate(GameTime gameTime)
        {
#if WINDOWS
            // Do nothing if editors are open.
            if (Tools.Dlg != null || Tools.DialogUp) return;
#endif

            // Update controller/keyboard states
            ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();

            // Update sounds
            if (!LogoScreenUp)
                Tools.SoundWad.Update();

            // Update songs
            if (Tools.SongWad != null)
                Tools.SongWad.PhsxStep();

            fps = .3f * fps + .7f * (1000f / (float)Math.Max(.00000231f, gameTime.ElapsedGameTime.TotalMilliseconds));

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

        private void UpdateFps(GameTime gameTime)
        {
            // Track time, changes in time, and FPS
            Tools.gameTime = gameTime;
            DrawCount++;

            // Update fps
            float new_t = (float)gameTime.TotalGameTime.TotalSeconds;
            Tools.dt = new_t - Tools.t;
            Tools.t = new_t;
        }

        /// <summary>
        /// Sets up the renderer. Returns true if no additional drawing should be done, because the game does not have focus.
        /// </summary>
        /// <returns></returns>
        private bool SetupToRender()
        {
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

#if WINDOWS
            if (!ActiveInactive())
                return true;
#endif

            // Make the actual view port we draw to, and clear it
            Tools.Render.MakeInnerViewport();
            MyGraphicsDevice.Clear(Color.Black);

            MyGraphicsDevice.Viewport = Tools.Render.MainViewport;

            // Default camera
            Vector4 cameraPos = new Vector4(MainCamera.Data.Position.X, MainCamera.Data.Position.Y, MainCamera.Zoom.X, MainCamera.Zoom.Y);//.001f, .001f);

            Tools.Render.SetStandardRenderStates();

            Tools.QDrawer.SetInitialState();
            ComputeFire();

            Tools.EffectWad.SetCameraPosition(cameraPos);

            Tools.SetDefaultEffectParams(MainCamera.AspectRatio);

            Tools.Render.SetStandardRenderStates();

            return false;
        }

        /// <summary>
        /// Draw the fireball textures to memory.
        /// </summary>
        private void ComputeFire()
        {
            if (!LogoScreenUp && !LogoScreenPropUp)
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
