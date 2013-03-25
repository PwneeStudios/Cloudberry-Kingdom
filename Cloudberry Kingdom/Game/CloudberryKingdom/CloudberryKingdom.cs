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
using Microsoft.Xna.Framework.Net;
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
	public enum MainMenuTypes { PC, Xbox, PS3, WiiU, Vita };

    public partial class CloudberryKingdomGame
    {
#if DEBUG
        public const bool FinalRelease = false;
#else
		public const bool FinalRelease = true;
#endif

        public const bool PropTest = false;

        /// <summary>
        /// The version of the game we are working on now (+1 over the last version uploaded to Steam).
        /// MajorVersion is 0 for beta, 1 for release.
        /// MinorVersion increases with substantial change.
        /// SubVersion increases with any pushed change.
        /// </summary>
        public static Version GameVersion = new Version(0, 2, 4);


        public static bool GodMode = !FinalRelease;
		public static bool AsianButtonSwitch = false;

#if PC_VERSION
        // Steam Beta
		//public static bool HideLogos = true;
		//public static bool LockCampaign = true;
		//public static bool SimpleMainMenu = true;
		//public static bool PS3MainMenu = false;
		//public static bool PS3MainMenu = false;
		//public static bool SimpleLeaderboards = true;
		//public static bool FakeAwardments = false;

		// PC Beta
		public static bool HideLogos = false;
		public static bool LockCampaign = false;
		public static bool SimpleMainMenu = true;
		public static MainMenuTypes MainMenuType = MainMenuTypes.PC;
		public static bool SimpleLeaderboards = false;
		public static bool FakeAwardments = false;
		public static float GuiSqueeze = 0;
#elif XBOX
        public static bool HideLogos = false || PropTest;
		public static bool LockCampaign = false;
		public static bool SimpleMainMenu = false;
		public static MainMenuTypes MainMenuType = MainMenuTypes.Xbox;
		public static bool SimpleLeaderboards = false;
		public static bool FakeAwardments = false;
		public static float GuiSqueeze = .3f;
#elif CAFE
		public static bool HideLogos = false;
		public static bool LockCampaign = false;
		public static bool SimpleMainMenu = true;
		public static MainMenuTypes MainMenuType = MainMenuTypes.WiiU;
		public static bool SimpleLeaderboards = true;
		public static bool FakeAwardments = false;
		public static float GuiSqueeze = 1f;
#elif PS3
		public static bool HideLogos = false;
		public static bool LockCampaign = false;
		public static bool SimpleMainMenu = false;
		public static MainMenuTypes MainMenuType = MainMenuTypes.PS3;
		public static bool SimpleLeaderboards = false;
		public static bool FakeAwardments = false;
		public static float GuiSqueeze = 1f;
#endif


        public static EzText SavingText = null;
        public static int ShowSavingDuration = 0;
        const int ShowSavingLength = 80;

        public static void ShowSaving()
        {
            //if (PlayerManager.GetNumPlayers() > 2)
            {
                ShowSavingDuration = ShowSavingLength;

				SavingText = new EzText(Localization.Words.Saving, Resources.Font_Grobold42, false);
				SavingText.FixedToCamera = true;
				SavingText.Pos = new Vector2(1110, -750);
				SavingText.Scale = .37f;
				StartMenu.SetTextSelected_Red(SavingText);
            }
        }

        void DrawSavingText()
        {
            if (ShowSavingDuration > 0)
            {
                ShowSavingDuration--;

                if (Tools.CurCamera != null)
                {
					const float FadeInLength = 6;
					const float FadeOutLength = 9;

                    if (ShowSavingDuration < FadeOutLength)
						SavingText.Alpha = 1f - (FadeOutLength - ShowSavingDuration) / FadeOutLength;
					else if (ShowSavingDuration > ShowSavingLength - FadeInLength)
						SavingText.Alpha = 1f - (FadeInLength - (ShowSavingLength - ShowSavingDuration)) / FadeInLength;

                    SavingText.Draw(Tools.CurCamera);
					Tools.QDrawer.Flush();
                }
            }
        }






		public static bool ForFrapsRecording = false;

#if DEBUG
        public static bool AlwaysGiveTutorials = true;
        public static bool Unlock_Customization = true;
        public static bool Unlock_Levels = true;
#else
        public static bool AlwaysGiveTutorials = false;
        public static bool Unlock_Customization = true;
        public static bool Unlock_Levels = false;
#endif

		public static bool ChoseNotToSave = false;
		public static bool PastPressStart = false;
		public static bool CanSave()
		{
			if (IsDemo) return false;

			if (ChoseNotToSave) return false;

			if (!PastPressStart) return false;

			return true;
		}

		public static bool CanSave(PlayerIndex index)
		{
			if (!CanSave()) return false;

#if WINDOWS
			return true;
#endif

			if (EzStorage.Device[(int)index] == null) return false;

			if (EzStorage.Device[(int)index].IsReady) return true;

			return false;
		}

		public static void ShowError_CanNotSaveNoDevice()
		{
			ShowError(Localization.Words.Err_StorageDeviceRequired, Localization.Words.Err_NoSaveDevice, Localization.Words.Err_Ok, null);
		}


		public static bool ProfilesAvailable()
		{
#if XBOX
			return Gamer.SignedInGamers.Count > 0;
#else
			return true;
#endif
		}

#if XBOX
        public static SignedInGamer IndexToSignedInGamer(PlayerIndex index)
        {
            var gamers = Gamer.SignedInGamers;

            if (gamers.Count == 0) return null;

            foreach (var gamer in gamers)
            {
                if (gamer.IsSignedInToLive && gamer.PlayerIndex == index) return gamer;
            }

            return null;
        }
#endif

        public static bool OnlineFunctionalityAvailable(PlayerIndex index)
        {
#if DEBUG && WINDOWS && XBOX
			return true;
#elif XBOX
            var gamer = IndexToSignedInGamer(index);

            if (gamer == null) return false;

            return true;
#else
			return true;
#endif
        }

		public static bool OnlineFunctionalityAvailable()
        {
#if XDK
            var gamers = Gamer.SignedInGamers;

            if (gamers.Count == 0) return false;

            if (GuideExtensions.IsNetworkCableUnplugged) return false;

            foreach (var gamer in gamers)
            {
                if (gamer.IsSignedInToLive) return true;
            }

			return false;
#else
			return true;
#endif
		}

		public static void BeginShowMarketplace()
		{
#if XDK
            ShowMarketplace = false;

            if (CloudberryKingdomGame.OnlineFunctionalityAvailable(ShowFor))
            {
                //Tools.Warning();
                //ulong offerID = 0;
                //GuideExtensions.ShowMarketplace(ShowFor, offerID);

                try
                {
                    SignedInGamer gamer = null;
                    foreach (SignedInGamer _gamer in Gamer.SignedInGamers)
                    {
                        if (_gamer.PlayerIndex == ShowFor)
                            gamer = _gamer;
                    }

                    if (gamer != null)
                    {
                        if (gamer.Privileges.AllowPurchaseContent)
                        {
                            GuideExtensions.ShowMarketplace(ShowFor, 0x584113C800000001);
                        }
                        else
                        {
                            CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Xbox_NoPermissionToBuy);
                        }
                    }
                }
                catch
                {
                    // Already shown.
                }
            }
            else
            {
                //CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Err_MustBeSignedIn);
                CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Err_MustBeSignedInToLive);
            }
#endif
		}

		public enum Presence { TitleScreen, Escalation, TimeCrisis, HeroRush, HeroRush2, Freeplay, Campaign, Arcade };
		public static Presence CurrentPresence = Presence.TitleScreen;
		public static void SetPresence(Presence presence)
		{
			CurrentPresence = presence;

#if XDK
			string Mode;

			switch (CurrentPresence)
			{
				case Presence.TitleScreen: Mode = "TitleScreen"; break;
				case Presence.Escalation: Mode = "Escalation"; break;
				case Presence.TimeCrisis: Mode = "TimeCrisis"; break;
				case Presence.HeroRush: Mode = "HeroRush"; break;
				case Presence.HeroRush2: Mode = "HeroRush2"; break;
				case Presence.Campaign: Mode = "Campaign"; break;
				case Presence.Freeplay: Mode = "Freeplay"; break;
                case Presence.Arcade: Mode = "Arcade"; break;
				default: Mode = "TitleScreen"; break;
			}

			List<PlayerData> CopyOfExistingPlayers = new List<PlayerData>(PlayerManager.ExistingPlayers);
			foreach (PlayerData player in CopyOfExistingPlayers)
			{
				SignedInGamer sig = player.MyGamer;
				if (sig != null && player.MyGamer.IsSignedInToLive)
				{
                    player.MyGamer.Presence.SetPresenceModeString(Mode);
				}
			}

            foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            {
                int index = (int)gamer.PlayerIndex;
                if (PlayerManager.Players[index] == null || !PlayerManager.Players[index].Exists)
                {
                    gamer.Presence.SetPresenceModeString("Idle");
                }
            }
#else
            Tools.Warning();
            return;
#endif
		}

#if XBOX
		static bool IsTrial = false;
#endif

		public static bool WasNotDemoOnce = false;
        public static bool FakeDemo = false && !FinalRelease;
        public static bool IsDemo
        {
            get
            {
#if DEBUG
				return FakeDemo;
#endif

				if (WasNotDemoOnce) return false;
				if (FakeDemo) return true;

#if XBOX
				IsTrial = Guide.IsTrialMode;

                if (!IsTrial)
                    WasNotDemoOnce = true; // Once this is set to true the game will always think it is a full version until restarted.

				return IsTrial;
#else
				WasNotDemoOnce = true; // Once this is set to true the game will always think it is a full version until restarted.
                return false;
#endif
            }
        }
		public static int Freeplay_Count = 0;
		public static int Freeplay_Max = 3;

#if XBOX
        public static void OfferToBuy(SignedInGamer gamer)
        {
            if (gamer.Privileges.AllowPurchaseContent)
            {
                try
                {
                    Guide.ShowMarketplace(gamer.PlayerIndex);
                }
                catch
                {
                }
                return;
            }

            foreach (SignedInGamer _gamer in Gamer.SignedInGamers)
            {
                if (_gamer.Privileges.AllowPurchaseContent)
                {
                    try
                    {
                        Guide.ShowMarketplace(_gamer.PlayerIndex);
                    }
                    catch
                    {
                    }
                    return;
                }
            }
        }
#else
		public static void OfferToBuy()
		{
		}
#endif






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

#if DEBUG
        public static bool OutputLoadingInfo = false;
#endif

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
            if (FakeDemo)
            {
#if XBOX || XBOX_SIGNIN
                Guide.SimulateTrialMode = true;
#endif
            }

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

            //ButtonString.Init();
            //ButtonCheck.Reset();
 
            //// Fill the pools
            //ComputerRecording.InitPool();

            EzStorage.StartAsyncUpdate();
        }

        public void InitialResolution()
        {
            Tools.Write("InitialResolution");
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
#if PC_VERSION

			EzStorage.Device[0] = new EasyStorage.PlayerSaveDevice(PlayerIndex.One);

			var d = EzStorage.Device[0];
			Tools.GameClass.Components.Add(d);

			d.PromptForDevice();

#if PC_VERSION
			PlayerManager.Players = new PlayerData[4];
			PlayerManager.Players[0] = new PlayerData();
			PlayerManager.Players[0].Init();
#else
#endif

			PlayerManager.Player.ContainerName = "SaveData";
			PlayerManager.Player.FileName = "SaveData.bam";
			PlayerManager.Player.Load(PlayerManager.Player.MyPlayerIndex);

			SaveGroup.LoadAll();
			PlayerManager.Player.Load(PlayerIndex.One);

#endif
            rez = PlayerManager.LoadRezAndKeys();
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

#if XBOX
#if WINDOWS
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
            //MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            //MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            //MyGraphicsDeviceManager.IsFullScreen = false;
#endif
            MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
            MyGraphicsDeviceManager.IsFullScreen = false;

            
            
            // For recording
            if (ForFrapsRecording)
            {
                MyGraphicsDeviceManager.PreferredBackBufferWidth = 1920;
                MyGraphicsDeviceManager.PreferredBackBufferHeight = 1080;
                MyGraphicsDeviceManager.IsFullScreen = true;
            }


#if WINDOWS && !EDITOR
            Tools.GameClass.SetBorder(Tools.WindowBorder);
#endif

            //MyGraphicsDeviceManager.ApplyChanges();
#else
            MyGraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            MyGraphicsDeviceManager.PreferredBackBufferHeight = 720;
#endif
#endif
            fps = 0;
            Tools.Write("BackBuffer set");
        }

#if NOT_PC && (XBOX || XBOX_SIGNIN)
        void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
        {
            Tools.Write("Signout event");

            if (Tools.CurGameData == null) return;

            // If the gamer is still signed in this was a mis-fire.
            foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            {
                if (gamer.Gamertag == e.Gamer.Gamertag)
                    return;
            }

            int index = (int)e.Gamer.PlayerIndex;

            if (EzStorage.Device[index] != null)
            {
                Tools.GameClass.Components.Remove(EzStorage.Device[index]);
                EzStorage.Device[index] = null;
            }

            if (Tools.CurGameData != null && !CharacterSelectManager.IsShowing)
                Tools.CurGameData.OnSignOut(e);

            var data = PlayerManager.Players[index] = new PlayerData();
            data.Init(index);

            if (PlayerManager.NumExistingPlayers() == 0)
            {
                Tools.SongWad.Stop();
                CharacterSelectManager.SuddenCleanup();

                Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
                if (Tools.CurrentLoadingScreen != null)
                {
                    Tools.EndLoadingScreen();
                }
            }
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            Tools.Write("Signin event");

            if (Tools.CurGameData == null) return;

            // If the gamer is already signed in this was a mis-fire.
            for (int i = 0; i < 4; i++)
            {
                if (PlayerManager.Players[i] != null &&
                    PlayerManager.Players[i]._MyGamer != null &&
                    PlayerManager.Players[i]._MyGamer.Gamertag == e.Gamer.Gamertag)
                {
                    return;
                }
            }

            if (EzStorage.Device[(int)e.Gamer.PlayerIndex] != null)
                Tools.GameClass.Components.Remove(EzStorage.Device[(int)e.Gamer.PlayerIndex]);

            int Index = (int)e.Gamer.PlayerIndex;
            string Name = e.Gamer.Gamertag;

            PlayerData data = new PlayerData();
            data.Init(Index);
            //data.Exists = true;
            PlayerManager.Players[Index] = data;

            if (!CanSave()) return;

            SaveGroup.LoadGamer(data);
            
            // Gamers that signin after the game launches should always be immediately prompted to select a storage device.
            if (EzStorage.Device[Index] != null)
                EzStorage.Device[Index].NeedsConnection = true;

            // Used to go back to main menu on signin, but not needed anymore now that Quick Join is disabled.
            //if (CurrentPresence == Presence.TitleScreen)
            //{
            //    // Nothing extra to do
            //}
            //else
            //{
            //    // Otherwise we're in a game and should return to the title screen
            //    Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
            //}
        }
#endif

        public void LoadContent()
        {
            Tools.Write("Inside LoadContent");
            
            MyGraphicsDevice = MyGraphicsDeviceManager.GraphicsDevice;
            Tools.Write("MyGraphicsDevice set");

			AdditiveColor_NormalAlpha = new BlendState();
			AdditiveColor_NormalAlpha.ColorBlendFunction = BlendFunction.Add;
			AdditiveColor_NormalAlpha.ColorDestinationBlend = Blend.One;
			AdditiveColor_NormalAlpha.ColorSourceBlend = Blend.One;
			AdditiveColor_NormalAlpha.AlphaBlendFunction = BlendFunction.Add;
			AdditiveColor_NormalAlpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
			AdditiveColor_NormalAlpha.AlphaSourceBlend = Blend.One;
            Tools.Write("BlendState made");

            Tools.LoadBasicArt(Tools.GameClass.Content);
            Tools.Write("LoadBasicArt done");

            Tools.Render = new MainRender(MyGraphicsDevice);
            Tools.Write("Render made");

            Tools.QDrawer = new QuadDrawer(MyGraphicsDevice, 2000);
            Tools.QDrawer.DefaultEffect = Tools.EffectWad.FindByName("NoTexture");
            Tools.QDrawer.DefaultTexture = Tools.TextureWad.FindByName("White");
            Tools.Write("QDrawer made");

            Tools.Device = MyGraphicsDevice;
            Tools.t = 0;

            LogoScreenUp = true;

            ScreenWidth = MyGraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = MyGraphicsDevice.PresentationParameters.BackBufferHeight;

            MainCamera = new Camera(ScreenWidth, ScreenHeight);
            Tools.Write("Camera made");

            MainCamera.Update();
            Tools.Write("Camera updated");

            // Initialize the Gamepads
            Tools.GamepadState = new GamePadState[4];
            Tools.PrevGamepadState = new GamePadState[4];
            Tools.Write("Gamepads made");

			Tools.EasyThread(5, "PreLoad", Preload);
		}

		void Preload()
		{
            Tools.Write("Preload");

            //Tools.LoadEffects(Tools.GameClass.Content, true);

            ButtonString.Init();
            ButtonCheck.Reset();

            // Fill the pools
            ComputerRecording.InitPool();

			// Initialize players
			PlayerManager.Init();

			// Load saved files
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			SaveGroup.Initialize();

			StartLogoSalad();

            // Localization
            Tools.Write("Language ISO code : " + System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            Localization.Language default_language = Localization.IsoCodeToLanguage(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            Tools.Write("Default language is " + default_language);
            Localization.SetLanguage(default_language);
            Tools.Write("Language loaded");

            // Pre load. This happens before anything appears.
            Resources.LoadAssets(true);
            Tools.Write("Asset preload complete.");

			// Create the initial loading screen
			LoadingScreen = new InitialLoadingScreen(Tools.GameClass.Content, Resources.ResourceLoadedCountRef);

            // Initialize heroes
            BobPhsx.CustomPhsxData.InitStatic();

            HookSignInAndOut();
            Tools.Write("Sign in and out now hooked.");

            // Fireball texture
            Fireball.PreInit();

            // Set textures to be transparent until loaded.
            Tools.Write("Dump dummy texture into each texture.");
            for (int i = 0; i < Tools.TextureWad.TextureList.Count; i++)
            {
                var tex = Tools.TextureWad.TextureList[i];

                if (CloudberryKingdomGame.PropTest)
                {
                    tex.Tex = Tools.TextureWad.DefaultTexture.Tex;
                }
                else
                {
                    tex.Tex = Tools.Transparent.Tex;
                }

                Resources.ResourceLoadedCountRef.Val++;
            }
            Tools.Write("Dummy fill complete.");

            // Load resource thread
            Resources.LoadResources();
        }

		private void StartLogoSalad()
		{
			if (!HideLogos)
			{
#if XDK
                if (IsDemo && GuideExtensions.ConsoleRegion == ConsoleRegion.NorthAmerica)
                {
                    MainVideo.StartVideo_CanSkipIfWatched("LogoSalad_ESRB");
                }
                else
                {
                    MainVideo.StartVideo_CanSkipIfWatched("LogoSalad");
                }
#else
				MainVideo.StartVideo_CanSkipIfWatched("LogoSalad");
#endif
			}

			PreloadDone = true;
		}

		public static bool PreloadDone = false;

        void HookSignInAndOut()
        {
#if NOT_PC && (XBOX || XBOX_SIGNIN)
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
#endif
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

        protected void GodModePhxs()
        {
            // Write to leaderboard
            Tools.Warning();
            if (ButtonCheck.State(ControllerButtons.RJ, -2).Down && ButtonCheck.State(ControllerButtons.LS, -2).Pressed)
            {
                var se = new ScoreEntry(null, 0, 100, 200, 300, 400, 500, 600);
                Leaderboard.WriteToLeaderboard(se);
            }


            // Give 100,000 points to each player
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.I) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.I))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.RJ, -2).Pressed)
#endif
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                    bob.MyStats.Score += 100000;
            }

            // Kill everyone but Player One
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.U) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.U))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.X, -2).Down)
#endif
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    if (bob.MyPlayerIndex != 0 && !(bob.Dead || bob.Dying))
                    {
                        //Fireball.Explosion(bob.Core.Data.Position, bob.Core.MyLevel);
                        //Fireball.ExplodeSound.Play();

                        //bob.Core.Show = false;
                        //bob.Dead = true;

                        bob.Die(Bob.BobDeathType.Other, true, false);
                    }
                }
            }

            // Turn on/off trial.
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.D) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.D))
#else
            if (ButtonCheck.State(ControllerButtons.RJ, -2).Down && ButtonCheck.State(ControllerButtons.LS, -2).Pressed)
#endif
            {
                FakeDemo = !FakeDemo;
            }

            // Turn on/off flying.
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.O))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.A, -2).Pressed)
#endif
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    bob.Flying = !bob.Flying;
                    bob.Immortal = !bob.Immortal;
                }
            }

            // Go to last door
#if PC
            if (Tools.Keyboard.IsKeyDownCustom(Keys.P) && !Tools.PrevKeyboard.IsKeyDownCustom(Keys.P))
#else
            if (ButtonCheck.State(ControllerButtons.LJ, -2).Down && ButtonCheck.State(ControllerButtons.B, -2).Down)
#endif
            {
                // Find last door
                if (Tools.CurLevel != null)
                {
                    Door door = Tools.CurLevel.FindIObject(LevelConnector.EndOfLevelCode) as Door;

                    if (null != door)
                    {
                        foreach (Bob bob in Tools.CurLevel.Bobs)
                        {
                            bob.Immortal = true;
                            Tools.MoveTo(bob, door.Pos);
                        }

                        foreach (ObjectBase obj in Tools.CurLevel.Objects)
                        {
                            CameraZone zone = obj as CameraZone;
                            if (null != zone)
                            {
                                if (Tools.CurLevel.MainCamera.MyZone == null ||
                                    Tools.CurLevel.MainCamera.MyZone.Box.Current.BL.X <= zone.Box.Current.BL.X)
                                {
                                    Tools.CurLevel.MainCamera.MyZone = zone;
                                    Tools.CurLevel.MainCamera.Pos = zone.End;
                                }
                            }
                        }
                    }
                }
            }
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

	#if DEBUG
            GodModePhxs();
	#else
            if (GodMode)
                GodModePhxs();
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
	#if DEBUG
            GodModePhxs();
	#else
            if (GodMode)
                GodModePhxs();
	#endif

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
			//var TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f / 24f));
            //var TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(1000f / 10f));
			//Tools.GameClass.TargetElapsedTime = TargetElapsedTime;
			//Tools.GameClass.IsFixedTimeStep = true;
        }

		public static bool ShowMarketplace = false;

#if XBOX
        public static bool ShowKeyboard = false;
        public static bool KeyboardIsDone = false;
		public static bool ShowAchievements = false;
        public static PlayerIndex ShowFor = PlayerIndex.One;

		static void BeginShowAchievements()
		{
			ShowAchievements = false;
#if XDK
            try
            {
                GuideExtensions.ShowAchievements(ShowFor);
            }
            catch
            {
            }
#endif
		}
#endif

        static bool ShowErrorMessage;

		public static void ShowError_LoadError()
		{
#if PC_VERSION
#else
			ShowError(Localization.Words.Err_CorruptLoadHeader, Localization.Words.Err_CorruptLoad, Localization.Words.Err_Ok, null);
#endif
		}

		public static void ShowError_MustBeSignedIn(Localization.Words word)
		{
#if PC_VERSION
#else
			ShowError(Localization.Words.Err_MustBeSignedIn_Header, word, Localization.Words.Err_Ok, null);
#endif
		}

        public static void ShowError_MustBeSignedInToLive(Localization.Words word)
        {
#if PC_VERSION
#else
            ShowError(Localization.Words.Err_MustBeSignedInToLive_Header, word, Localization.Words.Err_Ok, null);
#endif
        }

        public static void ShowError_MustBeSignedInToLiveForLeaderboard()
        {
#if PC_VERSION
#else
			if (CloudberryKingdomGame.IsDemo) return;
			
            ShowError(Localization.Words.Err_MustBeSignedInToLive_Header, Localization.Words.Err_MustBeSignedInToLiveForLeaderboards, Localization.Words.Err_Ok, null);
#endif
        }

        public static bool IsNetworkCableUnplugged()
        {
#if XDK
            return GuideExtensions.IsNetworkCableUnplugged;
#else
            return true;
#endif
        }

        static void ShowError(Localization.Words Header, Localization.Words Text, Localization.Words Option1, AsyncCallback callback)
        {
            ShowErrorMessage = true;

            Err_Header = Header;
            Err_Text = Text;
            Err_Callback = callback;
            Err_Options = new string[] { Localization.WordString(Option1) };
        }

        static Localization.Words Err_Header, Err_Text;
        static string[] Err_Options;
        static AsyncCallback Err_Callback;

        static void _ShowError()
        {
            ShowErrorMessage = false;

#if XDK || XBOX
            try
            {
                Guide.BeginShowMessageBox(Localization.WordString(Err_Header), Localization.WordString(Err_Text), Err_Options,
                    0, MessageBoxIcon.None, Err_Callback, null);
            }
            catch (Exception e)
            {
            }
#endif
        }

		bool DisconnectedController()
		{
#if PC_VERSION || DEBUG && WINDOWS
			//return false;
			if (ButtonCheck.MouseInUse || !ButtonCheck.ControllerInUse) return false;
#endif
			// True if an existing player is disconnected.
			for (int i = 0; i < 4; i++)
			{
				if (PlayerManager.Players[i] != null && PlayerManager.Players[i].Exists && !Tools.GamepadState[i].IsConnected)
				{
					return true;
				}
			}

            //// True if no one is connected
            //for (int i = 0; i < 4; i++)
            //{
            //    if (Tools.GamepadState[i].IsConnected)
            //    {
            //        return false;
            //    }
            //}

            //return true;

            return false;
		}

#if XBOX
        static bool ShowGamer;
        static PlayerIndex ShowGamer_Index;
        static Gamer ShowGamer_Gamer;
        public static void ShowGamerCard(PlayerIndex player, Gamer gamer)
        {
            ShowGamer = true;
            ShowGamer_Index = player;
            ShowGamer_Gamer = gamer;
        }

        void BeginShowGamer()
        {
            ShowGamer = false;
            if (ShowGamer_Gamer == null) return;

            Guide.ShowGamerCard(ShowGamer_Index, ShowGamer_Gamer);
        }
#endif

		public static bool ForceSuperPause = false;
		public static bool SuperPause
		{
			get
			{
				return SmallErrorMessage != null || ForceSuperPause;
			}
		}
        static SmallErrorMenu SmallErrorMessage;
        static void ShowSmallError()
        {
            if (SmallErrorMessage != null)
            {
                if (SmallErrorMessage.Core.Released)
                {
                    SmallErrorMessage = new SmallErrorMenu(Localization.Words.Err_ControllerNotConnected);
                }

                if (SmallErrorMessage.MyGame == null && Tools.CurGameData != null)
                {
                    Tools.CurGameData.AddGameObject(SmallErrorMessage);
                }

                return;
            }
			if (Tools.CurGameData == null) return;

			SmallErrorMessage = new SmallErrorMenu(Localization.Words.Err_ControllerNotConnected);
			Tools.CurGameData.AddGameObject(SmallErrorMessage);
        }

        public static bool CustomMusicPlaying = false;
        void UpdateCustomMusic()
        {
#if XDK
            if (!MediaPlayer.GameHasControl)
            {
                CustomMusicPlaying = true;
            }
            else
            {
                if (CustomMusicPlaying)
                {
                    if (Tools.SongWad != null)
                        Tools.SongWad.Restart(true, false);

                    CustomMusicPlaying = false;
                }
            }
#endif
        }

        /// <summary>
        /// If a gamer has no save device selected, ask them to select one.
        /// </summary>
        public static void PromptForDeviceIfNoneSelected()
        {
            if (CloudberryKingdomGame.IsDemo) return;

#if XBOX
            foreach (SignedInGamer gamer in Gamer.SignedInGamers)
            {
                int index = (int)gamer.PlayerIndex;
                if (PlayerManager.Players[index] != null && PlayerManager.Players[index].Exists &&
                    EzStorage.Device[index] != null && !EzStorage.Device[index].IsReady)
                {
                    EzStorage.Device[index].PromptForDevice();
                }
            }
#endif
        }

		void DrawWatermark()
		{
            if (FinalRelease) return;
			if (Tools.QDrawer == null) return;
			if (Resources.Font_Grobold42 == null) return;
			if (Resources.Font_Grobold42.HFont == null) return;
			if (Resources.Font_Grobold42.HOutlineFont == null) return;
			if (Tools.CurCamera == null) return;

			Camera cam = new Camera();
			cam.SetVertexCamera();
			Tools.QDrawer.DrawString(Resources.Font_Grobold42.HOutlineFont, "Version 0.9.9", new Vector2(1200, 870), Color.Black.ToVector4(),   new Vector2(.8f));
			Tools.QDrawer.DrawString(Resources.Font_Grobold42.HFont, "Version 0.9.9", new Vector2(1200, 870), Color.SkyBlue.ToVector4(), new Vector2(.8f));
			Tools.QDrawer.Flush();
		}

        /// <summary>
        /// The main draw loop.
        /// Sets all the rendering up and determines which sub-function to call (game, loading screen, nothing, etc).
        /// Also updates the game logic. TODO: Seperate this from the draw function?
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
			//Tools.Warning();
			//PlayerManager.Players[0].Exists = true;
			//PlayerManager.Players[1].Exists = true;
			//PlayerManager.Players[2].Exists = true;

#if DEBUG_OBJDATA
            ObjectData.UpdateWeak();
#endif
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;

            // Accelerate asset loading
            //if (LogoScreenUp)
            //{
            //    if (Resources.LoadThread != null)
            //    {
            //        Resources.LoadThread.Join(1);
            //    }
            //}

			// Stop now if the initial preload (and logosalad) hasn't started yet.
			if (!PreloadDone) { SetupToRender(); return; }

            // Prepare to draw
            Tools.DrawCount++;
			if (SetupToRender()) { DrawWatermark(); return; }

            // Main Video
			if (MainVideo.Draw()) { DrawWatermark(); return; }

            // Do not proceed if players do not exist yet.
            if (PlayerManager.Players == null) return;

            // Fps
            UpdateFps(gameTime);

            // Draw nothing if Xbox guide is up
#if XBOX || XBOX_SIGNIN
			if (Guide.IsVisible) { DrawNothing(); DrawWatermark(); return; }
            if (ShowKeyboard)
            {
                SaveLoadSeedMenu.BeginShowKeyboard();
            }
			else if (ShowAchievements)
			{
				BeginShowAchievements();
			}
            else if (ShowMarketplace)
            {
                BeginShowMarketplace();
            }
            else if (ShowErrorMessage)
            {
                _ShowError();
            }
            else if (ShowGamer)
            {
                BeginShowGamer();
            }
            else if (DisconnectedController())
            {
                ShowSmallError();
                //ButtonCheck.UpdateControllerAndKeyboard_StartOfStep();
                //return;
            }
            else
            {
                if (SmallErrorMessage != null)
                {
                    SmallErrorMessage.ReturnToCaller(true);
                    SmallErrorMessage = null;
                }
            }
#endif

			UpdateCustomMusic();

            // What to do
            if (LogoScreenUp)
            {
                if (LoadingScreen == null) return;
                LogoPhsx();
            }
            else if (LogoScreenPropUp)
                LoadingScreen.PhsxStep();
            if (!LogoScreenUp && !Tools.CurGameData.Loading)
                GameUpdate(gameTime);

            // What to draw
            if (LogoScreenUp || LogoScreenPropUp)
                LoadingScreen.Draw();
            else if (Tools.ShowLoadingScreen)
                DrawLoading();
            else if (Tools.CurGameData != null && !MainVideo.Playing)
                DrawGame();
            else
                DrawNothing();

            DrawExtra();

#if DEBUG && !XDK
            SaveScreenshotCode();
#endif

			DrawWatermark();
			DrawSavingText();
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
            if (!MediaPlayer.GameHasControl)
            {
                CustomMusicPlaying = true;
            }

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

		BlendState AdditiveColor_NormalAlpha;
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
					MyGraphicsDevice.BlendState = AdditiveColor_NormalAlpha;

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
