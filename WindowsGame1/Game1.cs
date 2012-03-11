using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if PC_VERSION
#else
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Drawing;

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
    public struct PhsxData
    {
        public Vector2 Position, Velocity, Acceleration;

        public void UpdatePosition() { Position += Velocity; }

        public void Integrate()
        {
            Velocity += Acceleration;
            Position += Velocity;
        }
    }

    public class CloudberryKingdomGame : Game
    {
        bool ShowFPS = false;
        public static float fps;

        public int DrawCount, PhsxCount;

        public ResolutionGroup Resolution;
        public ResolutionGroup []Resolutions = new ResolutionGroup[4];

#if WINDOWS
        QuadClass MousePointer, MouseBack;
        bool _DrawMouseBackIcon = false;
        public bool DrawMouseBackIcon { get { return _DrawMouseBackIcon; } set { _DrawMouseBackIcon = value; } }
#endif

        QuadDrawer QDrawer;
        EzEffectWad EffectWad;
        EzTextureWad TextureWad;


#if DEBUG
        public static bool AlwaysGiveTutorials = true;
        public static bool RecordIntro = false;
        public static bool UnlockAll = true;
        public static bool SimpleLoad = RecordIntro ? false : true;
        public static bool SimpleAiColors = RecordIntro ? true : false;
        public static bool BuildDebug = false;
#else
        public static bool AlwaysGiveTutorials = true;
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

        public WrappedBool LoadingResources;
        public int LoadingOffset;
        public WrappedFloat ResourceLoadedCountRef;
        public InitialLoadingScreen LoadingScreen;


        public GraphicsDeviceManager graphics;

        RenderTarget2D ScreenShotRenderTarget;

        GraphicsDevice device;
        int screenWidth;
        int screenHeight;

        SpriteFont Font1;

        Camera MainCamera;

        //public CharacterSelectManager CharSelectManager = new CharacterSelectManager();

        public CloudberryKingdomGame()
        {
            //var profilerGameComponent = new Indiefreaks.Xna.Profiler.ProfilerGameComponent(this, "Fonts/LilFont");
            //Indiefreaks.AOP.Profiler.ProfilingManager.Run = true;
            //Components.Add(profilerGameComponent);





            //List<int> ints = new List<int>(new int[] { 2, 2, 2, 1, 2, 3, 3, 3, 2, 2, 2, 2 });
            //Tools.RemoveAll(ints, num => num % 2 == 0);

            ResourceLoadedCountRef = new WrappedFloat();
#if PC_VERSION
#else
            Components.Add(new GamerServicesComponent(this));
#endif
                    
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Tools.TheGame = this;
        }

        protected override void Initialize()
        {
#if PC_VERSION
            KeyboardHandler.EventInput.Initialize(this.Window);
#endif
            Globals.ContentDirectory = Content.RootDirectory;

            Tools.LoadEffects(Content, true);
            EffectWad = Tools.EffectWad;

            ButtonString.Init();
            ButtonCheck.Reset();

#if PC_VERSION
            PlayerManager.RezData rez;
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
#if DEBUG
            rez.Fullscreen = false;
#else
            rez.Fullscreen = true;
#endif
            rez.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            rez.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif

            ////// Load
            ////Tools.Rnd = new Random();
            ////int seed = Tools.Rnd.Next();
            //////seed = 1266783283;
            ////Console.WriteLine("Seed: {0}", seed);
            ////Tools.Rnd = new Random(seed);

            ////// Load saved files
            ////PlayerManager.Init();
            ////SaveGroup.Initialize();
            

            // Set the possible resolutions
            Resolutions[0] = new ResolutionGroup();
            //Resolutions[0].Backbuffer = new IntVector2(1280, 720);
            //Resolutions[0].Backbuffer = new IntVector2(1400, 900);
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

            //Resolutions[0].CopyTo(ref Resolutions[1], new Vector2(.5f, .5f));
            //Resolutions[1].TextOrigin = new Vector2(-.5f, 0f);

            //Resolutions[2] = new ResolutionGroup();
            //Resolutions[2].Backbuffer = new IntVector2(640, 480);
            //Resolutions[2].Bob = new IntVector2(135, 0);
            //Resolutions[2].TextOrigin = new Vector2(-.5f, -.5f);
            //Resolutions[2].LineHeightMod = .5f;

            // Set the default resolution
            Resolution = Resolutions[2];

            graphics.PreferredBackBufferWidth = Resolution.Backbuffer.X;
            graphics.PreferredBackBufferHeight = Resolution.Backbuffer.Y;

#if PC_VERSION || WINDOWS
            if (rez.Custom)
            {
                if (!rez.Fullscreen)
                {
                    rez.Height = (int)((720f / 1280f) * rez.Width);
                }

                graphics.PreferredBackBufferWidth = rez.Width;
                graphics.PreferredBackBufferHeight = rez.Height;
                graphics.IsFullScreen = rez.Fullscreen;
            }
            else
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
#if DEBUG
            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }
#endif
#endif
            graphics.ApplyChanges();
            Window.Title = "Cloudberry Kingdom";

            // Fill the pools
            ComputerRecording.InitPool();

            fps = 0;

            base.Initialize();
        }

#if NOT_PC
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

        public void ReloadInfo()
        {
            InfoWad.Init();
            InfoWad.Read(Path.Combine(Globals.ContentDirectory, "InfoWad.infowad"));

            PieceQuad.LoadTemplates();

            PieceQuad c;
            
            //c = PieceQuad.Castle = new PieceQuad();
            //c.Init(null, Tools.BasicEffect);
            //c.Data.RepeatWidth = 10000;
            //c.Data.RepeatHeight = 362*1.8f;
            //c.Center.U_Wrap = false;
            //c.Center.V_Wrap = true;
            //c.Top.U_Wrap = c.Top.V_Wrap = false;
            //c.Data.TopWidth = 133*1.8f;
            //c.Data.MiddleOnly = true;
            //c.Bottom.Hide = true;
            //c.Data.UV_Multiples = new Vector2(1, 0);
            //c.Center.TextureName = "Castle_big_center";
            //c.Top.TextureName = "Castle_big_top";

            c = PieceQuad.Castle = new PieceQuad();
            c.Init(null, Tools.BasicEffect);
            c.Data.TopWidth = 133 * 1.8f;
            c.Data.LeftWidth = 625 * 1.4f;
            c.Data.RightWidth = 742 * 1.4f;
            c.Data.RepeatWidth = 578 * 1.4f;
            c.Data.RepeatHeight = 362 * 1.8f;
            c.Center.U_Wrap = c.Center.V_Wrap = true;
            c.Top.U_Wrap = true; c.Top.V_Wrap = false;
            c.Bottom.Hide = c.BL.Hide = c.BR.Hide = true;
            c.Data.UV_Multiples = new Vector2(1, 0);
            c.SetTexture("Castle");


            c = PieceQuad.Castle2 = new PieceQuad();
            c.Init(null, Tools.BasicEffect);
            c.Data.TopWidth = 133 * 1.8f;
            c.Data.LeftWidth = 625 * 1.4f;
            c.Data.RightWidth = 742 * 1.4f;
            c.Data.RepeatWidth = 578 * 1.4f;
            c.Data.RepeatHeight = 362 * 1.8f;
            c.Center.U_Wrap = c.Center.V_Wrap = true;
            c.Top.U_Wrap = true; c.Top.V_Wrap = false;
            c.Bottom.Hide = c.BL.Hide = c.BR.Hide = true;
            c.Data.UV_Multiples = new Vector2(1, 0);
            c.Data.Top_TR_Shift.Y = c.Data.TL_TR_Shift.Y = c.Data.TR_TR_Shift.Y =
            c.Data.Top_BL_Shift.Y = c.Data.TL_BL_Shift.Y = c.Data.TR_BL_Shift.Y = 
            c.Data.Center_TR_Shift.Y = c.Data.Left_TR_Shift.Y = c.Data.Right_TR_Shift.Y = 130;
            c.SetTexture("Castle");

            // Dark tile
            PieceQuad.DarkPillars = new PieceQuadGroup();
            PieceQuad.DarkPillars.InitPillars("darkpillar");
            PieceQuad.DarkPillars.SetCutoffs(60, 135, 210, 290, 390, 520);
            foreach (PieceQuad piece in PieceQuad.DarkPillars)
            {
                piece.Data.Center_TR_Shift = new Vector2(23, 26);
                piece.Data.Center_BL_Shift = new Vector2(-23, -20);
            }

            // Island
            var p = PieceQuad.Islands = new PieceQuadGroup();
            PieceQuad.Islands.InitPillars("Outside_Platform", new string[] { "xxsmall", "xsmall", "medium", "large" });
            PieceQuad.Islands.SetCutoffs(80, 130, 290);
            p[0].FixedHeight = 66*1.6f;
            p[1].FixedHeight = 99*1.65f;
            p[2].FixedHeight = 180*1.7f;
            p[3].FixedHeight = 209*1.75f;


            // Catwalk
            c = PieceQuad.Catwalk = new PieceQuad();
            c.Init(null, Tools.BasicEffect);
            c.Data.TopWidth = 73;
            c.Data.LeftWidth = 102;
            c.Data.RightWidth = 102;
            c.Data.RepeatWidth = 129;
            c.Data.RepeatHeight = 362 * 1.8f;
            c.Data.Top_TR_Shift.Y = c.Data.TL_TR_Shift.Y = c.Data.TR_TR_Shift.Y = 1;// 10;
            c.Center.U_Wrap = c.Center.V_Wrap = true;
            c.Top.U_Wrap = true; c.Top.V_Wrap = false;
            c.Bottom.Hide = c.BL.Hide = c.BR.Hide = true;
            c.Center.Hide = c.Left.Hide = c.Right.Hide = true;
            c.Data.UV_Multiples = new Vector2(1, 0);
            c.SetTexture("ledge");

            //PieceQuad.Castle = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Castle.boxes"));

            PieceQuad.Outside_Small = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Small.boxes"));
            PieceQuad.Outside_Smaller = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Smaller.boxes"));
            PieceQuad.Outside_Smallest = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Smallest.boxes"));
            PieceQuad.Outside_Medium = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Medium.boxes"));
            PieceQuad.Outside_Large = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Large.boxes"));
            PieceQuad.Outside_XLarge = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Outside_Pillar_Xlarge.boxes"));


            PieceQuad.Inside2_Small = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Small.boxes"));
            PieceQuad.Inside2_Smaller = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Smaller.boxes"));
            PieceQuad.Inside2_Smallest = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Smallest.boxes"));
            PieceQuad.Inside2_Medium = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Medium.boxes"));
            PieceQuad.Inside2_Large = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Large.boxes"));
            PieceQuad.Inside2_XLarge = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_block_Xlarge.boxes"));

            PieceQuad.Inside2_Pillar_Small = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Small.boxes"));
            PieceQuad.Inside2_Pillar_Smaller = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Smaller.boxes"));
            PieceQuad.Inside2_Pillar_Smallest = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Smallest.boxes"));
            PieceQuad.Inside2_Pillar_Medium = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Medium.boxes"));
            PieceQuad.Inside2_Pillar_Large = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Large.boxes"));
            PieceQuad.Inside2_Pillar_XLarge = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_pillar_Xlarge.boxes"));

            PieceQuad.Inside2_Thin = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_Thin.boxes"));
            PieceQuad.Inside2_Block = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Inside2_Block.boxes"));

            PieceQuad.SpeechBubble = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\SpeechBubble.boxes"));
            PieceQuad.SpeechBubbleRed = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\SpeechBubble_Red.boxes"));

            //PieceQuad.Menu = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Menu.boxes"));
            PieceQuad.TitleMenuPieces = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\TitleMenu.boxes"));
            PieceQuad.FreePlayMenu = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\freeplaymenu.boxes"));
            PieceQuad.CharMenu = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\CharMenu.boxes"));
            PieceQuad.CharMenu_Top = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\CharMenu_Top.boxes"));

            PieceQuad.MovingBlock = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\MovingBlock.boxes"));
            PieceQuad.BrickWall = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickWall.boxes"));
            PieceQuad.BrickPillar_Small = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickPillar_Small.boxes"));
            PieceQuad.BrickPillar_Medium = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickPillar_Medium.boxes"));
            PieceQuad.BrickPillar_Large = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickPillar_Large.boxes"));
            PieceQuad.BrickPillar_LargePlus = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickPillar_LargePlus.boxes"));
            PieceQuad.BrickPillar_Xlarge = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\BrickPillar_Xlarge.boxes"));
            PieceQuad.Floating_Small = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Floating_Small.boxes"));
            PieceQuad.Floating_Medium = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Floating_Medium.boxes"));
            PieceQuad.Floating_Large = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Floating_Large.boxes"));
            PieceQuad.Floating_Xlarge = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Floating_Xlarge_boxdata.boxes"));
            PieceQuad.OutsideBlock = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\OutsideBlock.boxes"));
            PieceQuad.TileBlock = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\TileBlock.boxes"));
            PieceQuad.Cement = new PieceQuad(Path.Combine(Globals.ContentDirectory, "Boxes\\Cement.boxes"));
        }

        protected void LoadMusic(bool CreateNewWad)
        {
            if (CreateNewWad)
                Tools.SongWad = new EzSongWad();

            Tools.SongWad.PlayerControl = Tools.SongWad.DisplayInfo = true;
            
            string path = Path.Combine(Globals.ContentDirectory, "Music");
            string[] files = Directory.GetFiles(path);

            //if (SimpleLoad)
            //{
            //    files = new string[] { "Music\\Happy.xnb",
            //                           "Music\\140 Mph in the Fog^Blind Digital.xnb",
            //                           "Music\\Blue Chair^Blind Digital.xnb",
            //                           "Music\\Ripcurl^Blind Digital.xnb",
            //                           "Music\\Evidence^Blind Digital.xnb" };
            //}

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
                        Tools.SongWad.AddSong(Content.Load<Song>("Music\\" + name), name);
                    else
                        Tools.SongWad.FindByName(name).song = Content.Load<Song>("Music\\" + name);
                }

                //lock (ResourceLoadedCountRef)
                {
                    ResourceLoadedCountRef.MyFloat++;
                }

                //if (SimpleLoad && Tools.SongWad.SongList.Count > 0) break;
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
            ContentManager manager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);

            if (CreateNewWad)
            {
                Tools.SoundWad = new EzSoundWad(4);
                Tools.PrivateSoundWad = new EzSoundWad(4);
            }

            string path = Path.Combine(Globals.ContentDirectory, "Sound");
            string []files = Directory.GetFiles(path);
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

                //lock (ResourceLoadedCountRef)
                {
                    ResourceLoadedCountRef.MyFloat++;
                }

                //if (SimpleLoad && Tools.SoundWad.SoundList.Count > 0) break;
            }

            if (Tools.SoundContentManager != null)
            {
                Tools.SoundContentManager.Unload();
                Tools.SoundContentManager.Dispose();
            }

            Tools.SoundContentManager = manager;
        }

        void LoadPacked()
        {
            PackedTexture Blob_Packed = new PackedTexture("Blob_Packed");
            Blob_Packed.Add("blob2_body", 0f, 0f, 0.1268222f, 0.8947368f);
            Blob_Packed.Add("blob2_body2", 0.1428571f, 0f, 0.2696793f, 0.8947368f);
            Blob_Packed.Add("blob2_body3", 0.2857143f, 0f, 0.4125364f, 0.8947368f);
            Blob_Packed.Add("blob2_body4", 0.4285714f, 0f, 0.5553936f, 0.8947368f);
            Blob_Packed.Add("blob2_body5", 0.5714286f, 0f, 0.6982507f, 0.8947368f);
            Blob_Packed.Add("blob2_rwing", 0.7142857f, 0f, 0.7769679f, 1f);
            Blob_Packed.Add("blob2_lwing", 0.8571429f, 0f, 1f, 0.8631579f);
            Tools.TextureWad.Add(Blob_Packed);

            PackedTexture Spike_Packed = new PackedTexture("Spike_Packed");
            Spike_Packed.Add("Spike", 0f, 0f, 0.5f, 1f);
            Spike_Packed.Add("SpikeBase", 0.5f, 0f, 0.925f, 0.61f);
            Tools.TextureWad.Add(Spike_Packed);

            PackedTexture SpikeyGuy_Packed = new PackedTexture("SpikeyGuy_Packed");
            SpikeyGuy_Packed.Add("SpikeyGuy", 0f, 0f, 0.25f, 1f);
            SpikeyGuy_Packed.Add("SpikeyGuy2", 0.25f, 0f, 0.5f, 1f);
            SpikeyGuy_Packed.Add("SpikeBlob", 0.5f, 0f, 0.6583334f, 0.5933333f);
            SpikeyGuy_Packed.Add("SpikeBlob2", 0.75f, 0f, 0.9083334f, 0.5933333f);
            Tools.TextureWad.Add(SpikeyGuy_Packed);

            PackedTexture SquareBlock_Packed = new PackedTexture("SquareBlock_Packed");
            SquareBlock_Packed.Add("BouncyBlock1", 0f, 0f, 0.1602067f, 0.9448819f);
            SquareBlock_Packed.Add("BouncyBlock2", 0.1666667f, 0f, 0.3268734f, 0.9448819f);
            SquareBlock_Packed.Add("FallingBlock1", 0.3333333f, 0f, 0.49354f, 0.9448819f);
            SquareBlock_Packed.Add("FallingBlock2", 0.5f, 0f, 0.6602067f, 0.9448819f);
            SquareBlock_Packed.Add("FallingBlock3", 0.6666667f, 0f, 0.8268734f, 0.9448819f);
            SquareBlock_Packed.Add("FallingBlock4", 0.8333333f, 0f, 1f, 1f);
            Tools.TextureWad.Add(SquareBlock_Packed);
        }

        protected void LoadArtMusicSound(bool CreateNewWads)
        {
            // Load the art!
            Tools.PreloadArt(Content);

            /*
            if (Tools.DebugConvenience)
                Tools.TextureWad.LoadAll(.2f, Content, ResourceLoadedCountRef);
            else
            {
                if (SimpleLoad)
                    Tools.TextureWad.LoadAll(.1f, Content, ResourceLoadedCountRef);
                else
                    Tools.TextureWad.LoadAll(0f, Content, ResourceLoadedCountRef);
            }*/

            LoadPacked();


            Tools.Write("Art done...");

            // Load the music!
            LoadMusic(CreateNewWads);
            Tools.Write("Music done...");

            // Load the sound!
            LoadSound(CreateNewWads);
            Tools.Write("Sound done...");

            Tools.ScoreTextFont = new EzFont("Fonts/ScoreTextFont");
        }

        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            Tools.LoadBasicArt(Content);

            TextureWad = Tools.TextureWad;

            Tools.QDrawer = QDrawer = new QuadDrawer(device, 1000);
            QDrawer.DefaultEffect = EffectWad.FindByName("NoTexture");
            QDrawer.DefaultTexture = TextureWad.FindByName("White");

            Tools.Rnd = new Random();
            int seed = Tools.Rnd.Next();
            //seed = 1266783283;
            Console.WriteLine("Seed: {0}", seed);
            Tools.Rnd = new Random(seed);
            
            Tools.EffectWad = EffectWad;
            Tools.TextureWad = TextureWad;
            Tools.Device = device;
            Tools.t = 0;

            LoadingResources = new WrappedBool(false);
            LoadingResources.MyBool = true;
            LogoScreenUp = true;
            
            Tools.spriteBatch = new SpriteBatch(GraphicsDevice);

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            PresentationParameters pp = Tools.Device.PresentationParameters;
            ScreenShotRenderTarget = new RenderTarget2D(Tools.Device,
            pp.BackBufferWidth, pp.BackBufferHeight, false,
                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
                                   RenderTargetUsage.DiscardContents);

            

            MainCamera = new Camera(screenWidth, screenHeight);
            
            MainCamera.Update();

            Tools.SoundVolume = new WrappedFloat();
            Tools.SoundVolume.MinVal = 0;
            Tools.SoundVolume.MaxVal = 1;
            Tools.SoundVolume.Val = 1;

            Tools.MusicVolume = new WrappedFloat();
            Tools.MusicVolume.MinVal = 0;
            Tools.MusicVolume.MaxVal = 1;
            Tools.MusicVolume.Val = 1;
            Tools.MusicVolume.SetCallback = () => Tools.UpdateVolume();

            // Create the initial loading screen
            FontLoad();
            LoadingScreen = new InitialLoadingScreen(Content, ResourceLoadedCountRef);

            // Load resource thread
            Thread LoadThread = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { 5 });
#endif
                        LoadThread = Thread.CurrentThread;
                        EventHandler<EventArgs> abort = (s, e) =>
                        {
                            if (LoadThread != null)
                            {
                                LoadThread.Abort();
                            }
                        };
                        Tools.TheGame.Exiting += abort;

                        Tools.Write("Start");

                        // Load art
                        LoadArtMusicSound(true);
                        if (!SimpleLoad)
                            Tools.TextureWad.LoadAllDirect(Content);

                        Tools.Write("ArtMusic done...");

                        // Load fonts
                        //Action fontaction = FontLoad;
                        //////Thread FontThread = Tools.EasyThread(1, "Fonts", fontaction);
                        //fontaction();

                        InitLava();

                        Tools.Write("Lava done...");

                        // Load the infowad and boxes
                        Action infoaction = () =>
                            {
                                ReloadInfo();
                                Tools.Write("Infowad done...");
                            };
                        //Thread InfoThread = Tools.EasyThread(3, "Infowad", infoaction);
                        infoaction();

                        Fireball.InitRenderTargets(device, device.PresentationParameters, 300, 200);

                        ParticleEffects.Init();

                        //ChallengeList.Init();
                        PlayerManager.Init();
                        Awardments.Init();
                        // Load saved files
                        SaveGroup.Initialize();

#if NOT_PC
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

                        Action objaction = () =>
                            {
                                Prototypes.LoadObjects();
                                ObjectIcon.InitIcons();

                                Tools.Write("Stickmen done...");
                            };
                        //Thread ObjThread = Tools.EasyThread(5, "prototypes", objaction);
                        objaction();


                        Tools.padState = new GamePadState[4];
                        Tools.PrevpadState = new GamePadState[4];            
                        Tools.SetStandardRenderStates();


                        // This must come at the very end, to avoid thread conflicts with the content manager
                        //Tools.TextureWad.LoadAll(Content);

                        //Tools.TextureWad.LoadAll_frac(0, 1);
                        //Tools.TextureWad.LoadAll_frac(1, 3);
                        //Tools.TextureWad.LoadAll_frac(2, 5);
                        //Tools.TextureWad.threads[0].Join();
                        //Tools.TextureWad.threads[1].Join();
                        //Tools.TextureWad.threads[2].Join();

                        Tools.Write("Textures done...");

                        //FontThread.Join();
                        //InfoThread.Join();
                        //ObjThread.Join();

                        Console.WriteLine("Total resources: {0}", ResourceLoadedCountRef.MyFloat);

                        //lock (LoadingResources)
                        {
                            LoadingResources.MyBool = false;
                            Tools.Write("Loading done!");
                        }

                        Tools.TheGame.Exiting -= abort;
                    }))
            {
                Name = "LoadThread",
#if WINDOWS
                Priority = ThreadPriority.Highest,
#endif
            };

            LoadThread.Start();
        }

        private void FontLoad()
        {
            Tools.LilFont = new EzFont("Fonts/LilFont");
            Tools.Font_Dylan15 = new EzFont("Fonts/LilFontBold");
            Tools.Font_Dylan20 = new EzFont("Fonts/Dylan20", "Fonts/DylanThinOutline20", -22, 40);
            Tools.Font_Dylan24 = new EzFont("Fonts/Dylan24", "Fonts/DylanThinOutline24", -22, 40);
            Tools.Font_Dylan28 = new EzFont("Fonts/Dylan28", "Fonts/DylanThinOutline28", -22, 40);
            //Tools.Font_Dylan42 = new EzFont("Fonts/BigFont2");
            //Tools.Font_Dylan60 = new EzFont("Fonts/BigFont");
            Tools.Font_Dylan42 = new EzFont("Fonts/Dylan42", "Fonts/DylanOutline42", -63, 40);
            //Tools.Font_DylanThin42 = new EzFont("Fonts/Dylan42", "Fonts/DylanThinOutline42", -68, .5f);//40);
            Tools.Font_DylanThin42 = new EzFont("Fonts/Dylan42", "Fonts/DylanThinOutline42", -68, 40);
            Tools.Font_Dylan60 = new EzFont("Fonts/Dylan60", "Fonts/DylanOutline60", -75, 40);

            Font1 = Tools.Font_Dylan60.Font;

            Tools.Font_DylanThick20 = new EzFont("Fonts/Dylan20", "Fonts/DylanOutline20", -22, 45);
            Tools.Font_DylanThick24 = new EzFont("Fonts/Dylan24", "Fonts/DylanOutline24", -22, 40);
            Tools.Font_DylanThick28 = new EzFont("Fonts/Dylan28", "Fonts/DylanOutline28", -22, 40);

            Tools.Font_DylanThick28 = new EzFont("Fonts/Dylan28", "Fonts/DylanOutline28", -22, 40);

            Tools.MonoFont = new EzFont("Fonts/MonoFont");

            Tools.Write("Fonts done...");
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        string VideoFolderName = "";
        public void BeginVideoCapture()
        {
            if (Tools.ScreenshotMode == false)
            {
                ChangeScreenshotMode();
                Tools.CapturingVideo = true;
                Tools.VideoFrame = 0;
            }
        }

        public void EndVideoCapture()
        {
            if (Tools.CapturingVideo)
            {
                ChangeScreenshotMode();
                Tools.CapturingVideo = false;
            }
        }

        void ChangeScreenshotMode()
        {
            Tools.ScreenshotMode = !Tools.ScreenshotMode;
            if (Tools.ScreenshotMode) Tools.DestinationRenderTarget = ScreenShotRenderTarget;
            else Tools.DestinationRenderTarget = null;
            Tools.ResetViewport();
        }

        void CountShortReset()
        {
            if (Tools.CurLevel.ResetEnabled() && Tools.CurLevel.PlayMode == 0 && !Tools.CurLevel.Watching && !Tools.CurGameData.PauseGame && Tools.CurGameData.QuickSpawnEnabled())
            {
                // Note that quickspawn was used, so that no hint is given about it
                Hints.QuickSpawn = 999;

                // Don't count reset against player if it happens within the first half second
                if (Tools.CurLevel.CurPhsxStep < 30)
                    Tools.CurLevel.FreeReset = true;

                // Update player stats
                Tools.CurLevel.CountReset();
                
                Tools.CurLevel.SetToReset = true;
            }
        }

        GameData Game { get { return Tools.CurGameData; } }
        void DoGameDataPhsx()
        {
            Tools.PhsxCount++;

            if (Tools.WorldMap != null)
                Tools.WorldMap.BackgroundPhsx();

            //GameData Game = Tools.CurGameData;
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

        public List<Action> ToDo = new List<Action>();
        public bool SetToBringSaveVideoDialog = false;
        public bool SetToRecordInput = false;
        protected void PhsxStep()
        {
            foreach (Action todo in ToDo)
                todo();
            ToDo.Clear();

            // WARNING!!!
            //if (ButtonCheck.State(ControllerButtons.Y, -1).Pressed)
            //    SaveGroup.SaveAll();

#if WINDOWS
            if (Tools.PrevKeyboardState == null) Tools.PrevKeyboardState = Tools.keybState;

#if PC_DEBUG || (WINDOWS && DEBUG)
            //if (Tools.keybState.IsKeyDownCustom(Keys.A))
            //{
            //    Tools.CurLevel.PlayMode = 0;
            //}

            //// Start Record
            //if (SetToRecordInput ||
            //    Tools.keybState.IsKeyDown(Keys.T) && !Tools.PrevKeyboardState.IsKeyDown(Keys.T))
            //{
            //    SetToRecordInput = false;

            //    Level lvl = Tools.CurLevel;
            //    lvl.NoCameraChange = true;
            //    lvl.CurPhsxStep = 0;
            //    lvl.CleanRecording();
            //    lvl.AllowRecording = true;
            //    lvl.StartRecording();
            //}

            //// Add recording
            //if (Tools.keybState.IsKeyDown(Keys.Y) && !Tools.PrevKeyboardState.IsKeyDown(Keys.Y))
            //{
            //    Level lvl = Tools.CurLevel;
            //    lvl.AddCurRecording();
            //    lvl.MainReplayOnly = true;
            //    lvl.WatchReplay(true);
            //}

            //// Save recording
            //if (Tools.keybState.IsKeyDown(Keys.G) && !Tools.PrevKeyboardState.IsKeyDown(Keys.G))
            //{
            //    Level lvl = Tools.CurLevel;
            //    lvl.CurrentRecording.Save("BeatBoss.rec", false);
            //}

            //// Give award
            //if (Tools.keybState.IsKeyDown(Keys.S) && !Tools.PrevKeyboardState.IsKeyDown(Keys.S))
            //{
            //    Awardments.GiveAward(Awardments.UnlockHeroRush2);
            //}

            // Viewer
            if ((Tools.viewer == null || Tools.viewer.IsDisposed)
                && Tools.keybState.IsKeyDown(Keys.B) && !Tools.PrevKeyboardState.IsKeyDown(Keys.B))
            {
                Tools.viewer = new Viewer.Viewer();
                Tools.viewer.Show();
            }
            if (Tools.viewer != null)
            {
                if (Tools.viewer.IsDisposed)
                    Tools.viewer = null;
                else
                    Tools.viewer.Input();
            }

            if (Tools.keybState.IsKeyDownCustom(Keys.F) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.F))
                ShowFPS = !ShowFPS;
#endif

#if PC_VERSION
            if (SimpleLoad)
            {
                if ((Tools.viewer == null || Tools.viewer.IsDisposed) && Tools.keybState.IsKeyDownCustom(Keys.B) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.B))
                {
                    Tools.viewer = new Viewer.Viewer();
                    Tools.viewer.Show();
                }
                if (Tools.viewer != null && !Tools.viewer.IsDisposed)
                    Tools.viewer.Input();
            }
#endif

            if ((Tools.keybState.IsKeyDownCustom(Keys.Space) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Space) ||
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Quickspawn_Secondary) && !Tools.PrevKeyboardState.IsKeyDownCustom(ButtonCheck.Quickspawn_Secondary))
                && Tools.CurLevel.ResetEnabled())
                CountShortReset();

#if PC_DEBUG
            if (Tools.FreeCam)
            {
                Vector2 pos = Tools.CurLevel.MainCamera.Data.Position;
                if (Tools.keybState.IsKeyDownCustom(Keys.Right)) pos.X += 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Left)) pos.X -= 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Up)) pos.Y += 130;
                if (Tools.keybState.IsKeyDownCustom(Keys.Down)) pos.Y -= 130;
                Tools.CurLevel.MainCamera.EffectivePos += pos - Tools.CurLevel.MainCamera.Data.Position;
                Tools.CurLevel.MainCamera.Data.Position = Tools.CurLevel.MainCamera.Target = pos;
                Tools.CurLevel.MainCamera.Update();
            }
#endif

            //if (Tools.keybState.IsKeyDownCustom(Keys.Space) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Space))
            //{
            //    LoadSound(false);

            //    ReloadInfo();

            //    foreach (Block block in Tools.CurLevel.Blocks)
            //    {
            //        NormalBlock nblock = block as NormalBlock;
            //        if (null != nblock) nblock.ResetPieces();

            //        MovingBlock mblock = block as MovingBlock;
            //        if (null != mblock) mblock.ResetPieces();
            //    }
            //}

#if PC_DEBUG || (WINDOWS && DEBUG)
            if (Tools.keybState.IsKeyDownCustom(Keys.D5) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D5))
            {
                //if (Tools.CurGameData != null) Tools.CurGameData.Release();
                GameData.LockLevelStart = false;
                LevelSeedData.ForcedReturnEarly = 0;
                MakeTestLevel(); return;
            }
            if (Tools.keybState.IsKeyDownCustom(Keys.D4) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D4))
            {
                //if (Tools.CurGameData != null) Tools.CurGameData.Release();
                GameData.LockLevelStart = false;
                LevelSeedData.ForcedReturnEarly = 1;
                MakeTestLevel(); return;
            }


            if (Tools.keybState.IsKeyDownCustom(Keys.OemComma))
            {
                Tools.CurLevel.MainCamera.Zoom *= .99f;
                Tools.CurLevel.MainCamera.EffectiveZoom *= .99f;
            }
            if (Tools.keybState.IsKeyDownCustom(Keys.OemPeriod))
            {
                Tools.CurLevel.MainCamera.Zoom /= .99f;
                Tools.CurLevel.MainCamera.EffectiveZoom /= .99f;
            }

            if (Tools.keybState.IsKeyDownCustom(Keys.D5) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.D5))
            //if (Tools.keybState.IsKeyDownCustom(Keys.H) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.H))
            {
                Tools.Editing = true;

                Hand NewHand = new Hand();
                NewHand.Core.Active = NewHand.Core.Show = true;
                NewHand.Core.Data.Position = Tools.CurLevel.MainCamera.Data.Position;
                Tools.CurLevel.AddObject(NewHand);

                Tools.CurLevel.Bobs.Clear();
            }

            if (Tools.keybState.IsKeyDownCustom(Keys.Escape) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Escape))
            {
                EndVideoCapture();
            }
            if (SetToBringSaveVideoDialog ||
                Tools.keybState.IsKeyDownCustom(Keys.U) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.U))
            {
                SetToBringSaveVideoDialog = false;

                Forms.FolderBrowserDialog ofd = new Forms.FolderBrowserDialog();
                string root = "C:\\Users\\Ez\\Desktop\\Cloudberry Kingdom\\Videos";
                ofd.SelectedPath = root + "\\Test";

                //Forms.SaveFileDialog ofd = new Forms.SaveFileDialog();
                //ofd.Title = "Save as...";
                //ofd.Filter = "Imaginary extension (*.iex)|*.iex";
                //ofd.InitialDirectory = "C:\\Users\\Ez\\Desktop\\Cloudberry Kingdom\\Videos";
                //ofd.CheckFileExists = false;

                Tools.DialogUp = true;

                // Check the user didn't cancel
                if (ofd.ShowDialog() != Forms.DialogResult.Cancel)
                {
                    Tools.DialogUp = false;

                    // Record video
                    BeginVideoCapture();
                    ofd.ShowNewFolderButton = true;
                    VideoFolderName = ofd.SelectedPath;
                    //VideoFolderName = ofd.FileName.Substring(0, ofd.FileName.LastIndexOf('.'));

                    // Make directory
                    if (VideoFolderName.Length <= root.Length) return;
                    if (System.IO.Directory.Exists(VideoFolderName))
                        System.IO.Directory.Delete(VideoFolderName, true);
                    System.IO.Directory.CreateDirectory(VideoFolderName);
                }

                Tools.DialogUp = false;
            }

            if (Tools.keybState.IsKeyDownCustom(Keys.I) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.I))
            {
                ChangeScreenshotMode();
            }

            if (Tools.keybState.IsKeyDownCustom(Keys.O) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.O))
            {
                foreach (Bob bob in Tools.CurLevel.Bobs)
                {
                    bob.Immortal = !bob.Immortal;
                }
            }
            if (Tools.keybState.IsKeyDownCustom(Keys.P) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.P))
                Tools.FreeCam = !Tools.FreeCam;
            if (Tools.keybState.IsKeyDownCustom(Keys.Q) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Q))
                Tools.DrawGraphics = !Tools.DrawGraphics;
            if (Tools.keybState.IsKeyDownCustom(Keys.W) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.W))
                Tools.DrawBoxes = !Tools.DrawBoxes;
            if (Tools.keybState.IsKeyDownCustom(Keys.E) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.E))
                Tools.StepControl = !Tools.StepControl;
            if (Tools.keybState.IsKeyDownCustom(Keys.R) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.R))
            {
                Tools.IncrPhsxSpeed();
            }
#endif
            if (!Tools.StepControl || (Tools.keybState.IsKeyDownCustom(Keys.Enter) && !Tools.PrevKeyboardState.IsKeyDownCustom(Keys.Enter)))
            {
                DoGameDataPhsx();
            }

            Tools.DeltaScroll = Tools.CurMouseState.ScrollWheelValue - Tools.PrevMouseState.ScrollWheelValue;
            Tools.DeltaMouse = Tools.ToWorldCoordinates(new Vector2(Tools.CurMouseState.X, Tools.CurMouseState.Y), Tools.CurLevel.MainCamera) -
                               Tools.ToWorldCoordinates(new Vector2(Tools.PrevMouseState.X, Tools.PrevMouseState.Y), Tools.CurLevel.MainCamera);

            Tools.PrevKeyboardState = Tools.keybState;
            Tools.PrevMouseState = Tools.CurMouseState;
#else
            DoGameDataPhsx();
#endif

            // Allow Back to exit the game if we are in test mode
            if (SimpleLoad && ButtonCheck.State(ControllerButtons.Back, -1).Down)
                Exit();

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
                    CountShortReset();
            }


            for (int i = 0; i < 4; i++)
                if (Tools.PrevpadState[i] != null)
                    Tools.PrevpadState[i] = Tools.padState[i];

            /* XBOX Debug buttons
            if (Tools.padState[0].Buttons.B == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.B != ButtonState.Pressed)
            {
                Tools.CurLevel.ResetAll(false);
                Tools.CurLevel.PlayMode = 0;
            }
            if (Tools.padState[0].Buttons.Y == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.Y != ButtonState.Pressed)
            {
                Tools.CurLevel.ResetAll(false);
                Tools.CurLevel.PlayMode = 1;
            }
            if (Tools.padState[0].Buttons.X == ButtonState.Pressed && Tools.PrevpadState[0].Buttons.X != ButtonState.Pressed)
            {
                Tools.PhsxSpeed += 1;
                if (Tools.PhsxSpeed > 4) Tools.PhsxSpeed = 1;
            }
            */
            
            Fireball.TexturePhsx();
        }


        public static string debugstring = "";
        StringBuilder MainString = new StringBuilder(100, 100);
        void DrawGC()
        {
            if (Tools.ScreenshotMode) return;

            Tools.StartSpriteBatch();

#if WINDOWS
            // GC
            //string str = GC.CollectionCount(0).ToString() + " " + fps.ToString() + debugstring;

            // Phsx count
            string str  = string.Format("CurLevel PhsxStep: {0}\n", Tools.CurLevel.CurPhsxStep);

            // Score
            //PlayerData p = PlayerManager.Get(0);
            //string str = string.Format("Death {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalDeaths, p.LevelStats.TotalDeaths, p.GameStats.TotalDeaths, p.CampaignStats.TotalDeaths, Campaign.Attempts);
            //Campaign.CalcScore();
            //string str2 = string.Format("Coins {0}, {1}, {2}, {3}, {4}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins, Campaign.Coins);
            //str += "\n\n" + str2;
            //string str3 = string.Format("Total {0}, {1}, {2}, {3}, {4}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins, Campaign.TotalCoins);
            //str += "\n\n" + str3;
            //string str4 = string.Format("Total {0}, {1}, {2}, {3}", p.TempStats.TotalBlobs, p.LevelStats.TotalBlobs, p.GameStats.TotalBlobs, p.CampaignStats.TotalBlobs);
            //str += "\n" + str4;
            //string str5 = string.Format(" {0}, {1}, {2}, {3}", p.TempStats.Blobs, p.LevelStats.Blobs, p.GameStats.Blobs, p.CampaignStats.Blobs);
            //str += "\n" + str5;

            // Coins
            //string str = string.Format("{0}, {1}, {2}, {3}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.CampaignStats.Coins);
            //string str2 = string.Format("{0}, {1}, {2}, {3}", p.TempStats.TotalCoins, p.LevelStats.TotalCoins, p.GameStats.TotalCoins, p.CampaignStats.TotalCoins);
            //str += "\n" + str2;
#else
            string str = debugstring;
#endif

            str = string.Format("{0,-5} {1,-5} {2,-5} {3,-5} {4,-5}", Level.Pre1, Level.Step1, Level.Pre2, Level.Step2, Level.Post);

            Tools.spriteBatch.DrawString(Font1,
                    str,
                    new Vector2(0, 100),
                    Color.Orange, 0, Vector2.Zero, .4f, SpriteEffects.None, 0);
            Tools.EndSpriteBatch();
        }

        bool DoInnerLogoPhsx = true;
        void LogoPhsx()
        {
            LoadingScreen.PhsxStep();
            if (!DoInnerLogoPhsx)
            {
                if (LoadingScreen.IsDone || SimpleLoad)
                    LogoScreenUp = false;

                return;
            }

            //return;

            //if (false)
            if (SimpleLoad)
            {
                Tools.SoundVolume.Val = 0;
                Tools.MusicVolume.Val = 0;
                //Tools.SoundVolume.Val = 0.5f;
                //Tools.MusicVolume.Val = 0.5f;// .07f;
            }
            else
            {
                Tools.SoundVolume.Val = 1f;
                Tools.MusicVolume.Val = .7f;
            }

            if (!LoadingResources.MyBool)
            {
                Tools.Write("+++++++++++++++++++ Resources all loaded!");

                //if (false)
                if (LoadingScreen.IsDone || SimpleLoad || !LoadingResources.MyBool)
                {
                    Tools.Write("+++++++++++++++++++ Resources all loaded!");

                    DoInnerLogoPhsx = false;
                    if (LoadingScreen.IsDone || SimpleLoad)
                        LogoScreenUp = false;

                    DrawCount = PhsxCount = 0;

                    PlayerManager.Get(0).IsAlive = PlayerManager.Get(0).Exists = true;
                    //PlayerManager.Get(1).IsAlive = PlayerManager.Get(1).Exists = true;
                    //for (int i = 0; i < 4; i++)
                    //    PlayerManager.Get(i).ColorScheme = ColorSchemeManager.ColorSchemes[i];


                    Campaign.InitCampaign(3);
                    Tools.CurGameData = new Campaign_DungeonFromAbove(null); return;


                    // Trailer scenes
                    //ScreenSaver Intro = new ScreenSaver(true); Intro.InitForTrailer(); return;



                    //IntroCinematic Intro = new IntroCinematic();
                    //Intro.Start();


                    //PlayerManager.Get(1).Exists = true;
                    //PlayerManager.Get(1).IsAlive = true;


                    LevelSeedData.ForcedReturnEarly = 1;
                    MakeTestLevel(); return;
#if DEBUG
                    if (RecordIntro)
                    {
                        ScreenSaver Intro = new ScreenSaver(); Intro.InitToRecord(); return;
                        //ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
                    }
                    else
                    {
                        if (!SimpleLoad)
                        {
                            ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
                        }
                        else
                        {
                            Tools.CurGameData = new TitleGameData(); return;
                        }
                    }
#else
                    // Full Game
                    ScreenSaver Intro = new ScreenSaver(); Intro.Init(); return;
#endif

                    // Record intro
                    //ScreenSaver Intro = new ScreenSaver(); Intro.InitToRecord(); return;



                    Campaign.InitCampaign(0); Tools.CurGameData = new Doom(); return;


                    // Test individual campaign levels
                    bool TestCampaign = true;
                    //TestCampaign = false;
                    if (TestCampaign)
                    {
                        //PlayerManager.Get(1).Exists = true;
                        //PlayerManager.Get(1).IsAlive = true;

                        Campaign.InitCampaign(3);
                        Tools.CurGameData = new Campaign_String(); return;
                        //Tools.CurGameData = new Campaign_Chaos(null); return;

                        
                        //Tools.CurGameData = new Campaign_GetCart(null); return;
                        //Tools.CurGameData = new Campaign_GetWheelie(null); return;
                        //Tools.CurGameData = new Campaign_CreditsLevel(); return;
                        //Tools.CurGameData = new Campaign_PrincessRoom(); return;
                        //Tools.CurGameData = new Campaign_BossNew(true); return;
                        //Tools.CurGameData = new Campaign_GetBouncy(null); return;
                        //Tools.CurGameData = new Campaign_GetJetpack(BackgroundType.Night, null); return;
                        //Tools.CurGameData = new Campaign_BossNew(true); return;
                        //Tools.CurGameData = new Doom(); return;
                        //Tools.CurGameData = new Campaign_IntroWorld(); return;
                        //Tools.CurGameData = new Campaign_World2(true); return;
                        //Tools.CurGameData = new Campaign_FlyIn(); return;
                        //Tools.CurGameData = new Campaign_TunnelOutside(); return;
                    }

                    //MakeTestLevel(); return;
                    //MakeTestSurvivalLevel();


                    //StringWorldGameData StringWorld = new StringWorldGameData();
                    //Tools.WorldMap = Tools.CurGameData = StringWorld;

                    /* Endurance 
                    Endurance endurance = new Endurance();
                    Tools.WorldMap = Tools.CurGameData = endurance;
                    Tools.CurLevel = endurance.MyLevel;
                    */

                    //Tools.CurGameType = GameType.DifficultySelect;                    
                    //Tools.CurGameData = new DifficultySelection(); return;
                    //Tools.CurGameData.DefaultHeroType = BobPhsxJetpack.Instance;
                    //Tools.BeginLoadingScreen();

                    //Generic.InitWorldData();
                    //Tools.CurGameData = new WorldGameData(1);

                    //Tools.CurGameData = new CharacterSelectGameData();

                    //Tools.CurGameData.FadeIn(.037f);

                    // Current Title Screen
                    //PlayerManager.Get(1).Exists = true;
                    //PlayerManager.Get(1).IsAlive = true;
                    Tools.CurGameData = new TitleGameData();
                    //return;
                    //PlayerManager.Get(1).Exists = true;
                    //PlayerManager.Get(1).IsAlive = true;

                    //Challenge_TimeCrisis.Instance.Start(1);
                    //HeroRushTutorial.ShowTitle = false; HeroRushTutorial.WatchedOnce = true;
                    //Challenge_HeroRush.Instance.Start(0);
                    //Challenge_Endurance.Instance.Start(1);
                    //Challenge_Place.Instance.Start(1);
                    //Challenge_Regular.Instance.Start(1);
                    //Challenge_Fireballs.Instance.Start(1);
                    //Challenge_Survival.Instance.Start(0);
                }
            }
        }

        void MakeTestLevel()
        {
            LevelSeedData data = new LevelSeedData();

            data.Seed = Tools.Rnd.Next();
            //data.Seed = 1;

            //data.MyBackgroundType = BackgroundType.Dungeon;

            //data.SetBackground(BackgroundType.Outside);
            //data.SetBackground(BackgroundType.Night);
            //data.SetBackground(BackgroundType.NightSky);
            data.SetBackground(BackgroundType.Dungeon);

            //var custom = (BobPhsxNormal)BobPhsxSmall.Instance.Clone();
            
            //var custom = (BobPhsxNormal)BobPhsxBouncy.Instance.Clone();
            //custom.NumJumps = 2;
            //data.DefaultHeroType = custom;

            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Bouncy, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Box, Hero_Shape.Oscillate, Hero_MoveMod.Double);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Oscillate, Hero_MoveMod.Double);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Double);

            //data.DefaultHeroType = BobPhsxNormal.Instance;
            //data.DefaultHeroType = BobPhsxInvert.Instance;
            data.DefaultHeroType = BobPhsxMeat.Instance;
            //data.DefaultHeroType = BobPhsxDouble.Instance;
            //data.DefaultHeroType = BobPhsxSpaceship.Instance;
            //data.DefaultHeroType = BobPhsxRocketbox.Instance;
            //data.DefaultHeroType = BobPhsxSmall.Instance;
            //data.DefaultHeroType = BobPhsxScale.Instance;
            //data.DefaultHeroType = BobPhsxJetman.Instance;
            //data.DefaultHeroType = BobPhsxBox.Instance;

            //data.PlaceObjectType = PlaceTypes.FallingBlock;
            data.PlaceObjectType = PlaceTypes.Princess;

            //data.MyGeometry = LevelGeometry.Right;
            data.MyGeometry = LevelGeometry.Up;
            data.PieceLength = 7000;
            data.NumPieces = 1;

            data.MyGameType = NormalGameData.Factory;
            //data.MyGameType = PlaceGameData.Factory;


            //data.MyGameFlags.IsTethered = true;
            //data.MyGameFlags.IsDoppleganger = true;
            //data.MyGameFlags.IsDopplegangerInvert = true;

            //type = GameType.Place;

            //data.Initialize(type, LevelGeometry.Big, (int)1, (int)20000, delegate(PieceSeedData piece)
            //data.Initialize(type, LevelGeometry.Up, (int)1, (int)20000, delegate(PieceSeedData piece)
            data.Initialize(TestLevelInit);

            // Add Landing Zone
            //data.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.LandingZone;

            data.PostMake = TestLevelPostMake;

            //Campaign.CarryPrinces(data);

                // Rumble
                //level.MyGame.AddGameObject(new Rumble());
            //};

            data.LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;

            GameData.StartLevel(data);
        }

        void TestLevelPostMake(Level level)
        {
            level.StartRecording();

            level.MyGame.AddGameObject(new HintGiver());
            level.MyGame.AddGameObject(HelpMenu.MakeListener());
            level.MyGame.AddGameObject(InGameStartMenu.MakeListener());

            //level.MyGame.AddGameObject(new LevelTitle(data.DefaultHeroType.Name));
            //level.MyGame.AddGameObject(new GUI_Quota(15));

            //var gui = new GUI_LivesLeft(20);
            //level.MyGame.AddGameObject(gui);
            //level.MyGame.AddGameObject(new GUI_NextLife(25, gui));
            //level.MyGame.AddGameObject(new GUI_Lives(gui));

            //level.MyGame.DramaticEntry(level.StartDoor, 20);
        }

        void TestLevelInit(PieceSeedData piece)
        {
            //piece.ZoomType = LevelZoom.Big;
            piece.ExtraBlockLength = 1000;


            //piece.PreStage2 = level =>
            //{
            //    foreach (Bob bob in level.Bobs)
            //    {
            //        PrincessBubble princess = new PrincessBubble(Vector2.Zero);
            //        level.AddObject(princess);
            //        princess.PickUp(bob);
            //    }
            //};

            //piece.Paths = RndDifficulty.ChoosePaths(piece);
            //piece.Style.AlwaysCurvyMove = true;
            RndDifficulty.ZeroUpgrades(piece.MyUpgrades1);

            piece.MyUpgrades1[Upgrade.Pinky] = 2;
            piece.MyUpgrades1[Upgrade.Laser] = 4;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 4;
            piece.MyUpgrades1[Upgrade.Speed] = 4;
            piece.MyUpgrades1[Upgrade.Jump] = 10;
            piece.MyUpgrades1[Upgrade.FallingBlock] = 7;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 7;
            piece.MyUpgrades1[Upgrade.FlyBlob] = 7;
            piece.MyUpgrades1[Upgrade.BouncyBlock] = 4;
            //piece.MyUpgrades1[Upgrade.GhostBlock] = 8;
            piece.MyUpgrades1[Upgrade.Speed] = 8;

            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

            piece.Paths = 1; piece.LockNumOfPaths = true;
            piece.Style.SinglePathType = StyleData._SinglePathType.Normal;

            /*
            piece.Paths = 2;

            SingleData style = piece.Style as SingleData;
            style.InitialDoorYRange = new Vector2(-800);
            style.DoublePathType = StyleData._DoublePathType.Gap;
            */

            piece.Style.MyModParams = TestLevelModParams;

            piece.Style.ChanceToKeepUnused = 0;

            RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            //piece.MyUpgrades2[Upgrade.Cloud] = 10;
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

            piece.Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
            piece.Style.MyFinalPlatsType = StyleData.FinalPlatsType.Door;
        }

        void TestLevelModParams(Level level, PieceSeedData p)
        {
            //var Params = (NormalBlock_Parameters)p.Style.FindParams(NormalBlock_AutoGen.Instance);
            //Wall wall = Params.SetWall(data.MyGeometry);
            //wall.Space = 20; wall.MyBufferType = Wall.BufferType.Space;
            //p.CamZoneStartAdd.X = -2000;
            //wall.StartOffset = -600;
            //wall.Speed = 17.5f;
            //wall.InitialDelay = 72;

            //Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
            //Params.Regular_Period = 15;
            //Params.FillType = Coin_Parameters.FillTypes.CoinGrab;
            //Params.Grid = false;
            //Params.DoCleanup = false;

            //level.CurMakeData.MidDivider = true;

            //FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
            //Params.Special.BorderFill = true;


            //Floater_Spin_Parameters FS_Params = (Floater_Spin_Parameters)p.Style.FindParams(Floater_Spin_AutoGen.Instance);
            //FS_Params.Special.Rockwheel = true;

            //p.Style.SetToMake_BouncyHallway(piece);


            float size = 90;
            var FParams = (FallingBlock_Parameters)p.Style.FindParams(FallingBlock_AutoGen.Instance);
            FParams.Width = size;
            var MParams = (MovingBlock_Parameters)p.Style.FindParams(MovingBlock_AutoGen.Instance);
            MParams.Size = size;
            MParams.Aspect = MovingBlock_Parameters.AspectType.Square;
            MParams.Motion = MovingBlock_Parameters.MotionType.Vertical;
            var BParams = (BouncyBlock_Parameters)p.Style.FindParams(BouncyBlock_AutoGen.Instance);
            BParams.Size = size;
            var GParams = (Goomba_Parameters)p.Style.FindParams(Goomba_AutoGen.Instance);
            //GParams.Counter
            var NParams = (NormalBlock_Parameters)p.Style.FindParams(NormalBlock_AutoGen.Instance);
            //NParams.Make = false;


            //Goomba_Parameters GParams = (Goomba_Parameters)p.Style.FindParams(Goomba_AutoGen.Instance);
            //GParams.KeepUnused = 1f;
            //GParams.FillWeight = 100;

            /* Goomba pinwheel */
            //Goomba_Parameters Params = (Goomba_Parameters)p.Style.FindParams(Goomba_AutoGen.Instance);
            //Params.Special.Pinwheel = true;
            //p.Paths = level.CurMakeData.NumInitialBobs = 1;



            /* Goomba hallway */
            //Goomba_Parameters Params = (Goomba_Parameters)p.Style.FindParams(Goomba_AutoGen.Instance);
            //p.Style.SuppressGroundCeiling(piece);
            //Params.Special.Tunnel = true;
            //Params.TunnelDisplacement = 100;
            //Params.TunnelMotionType = Goomba_Parameters.MotionType.Horizontal;
            //Params.TunnelCeiling = false;
            //p.Paths = level.CurMakeData.NumInitialBobs = 1;
            //level.CurMakeData.MoveData[0].MaxTargetY = 0;
            //level.CurMakeData.MoveData[0].MinTargetY = -300;



            // Special mixes: (rockcircle + bouncyhall + lasers)
        }
        
        void MakeTestSurvivalLevel()
        {
            LevelSeedData data = new LevelSeedData();

            data.Seed = Tools.Rnd.Next();

            data.MyBackgroundType = BackgroundType.Gray;
            data.DefaultHeroType = BobPhsxNormal.Instance;

            GameFactory type = SurvivalGameData.Factory;
            data.MyGameFlags.IsTethered = false;
            data.MyGameFlags.IsTethered = false;

            //type = GameType.Place;

            data.Initialize(type, LevelGeometry.OneScreen, (int)1, (int)1100, delegate(PieceSeedData piece)
            {
                //piece.Paths = RndDifficulty.ChoosePaths(piece);

                RndDifficulty.ZeroUpgrades(piece.MyUpgrades1);
                //piece.MyUpgrades1[Upgrade.FlyBlob] = 7;
                //piece.MyUpgrades1[Upgrade.BouncyBlock] = 10;
                piece.MyUpgrades1[Upgrade.Jump] = 4;
                piece.MyUpgrades1[Upgrade.Fireball] = 1;
                //piece.MyUpgrades1[Upgrade.Speed] = 7;
                //piece.MyUpgrades1[Upgrade.FireSpinner] = 4;
                //piece.MyUpgrades1[Upgrade.Laser] = 6;
                //piece.MyUpgrades1[Upgrade.Cloud] = 7;
                //piece.MyUpgrades1[Upgrade.Floater] = 10;
                //piece.MyUpgrades1[Upgrade.Floater_Spin] = 4;
                //piece.MyUpgrades1[Upgrade.BouncyBlock] = 5;
                //piece.MyUpgrades1[Upgrade.MovingBlock] = 10;
                //piece.MyUpgrades1[Upgrade.FallingBlock] = 4;
                //piece.MyUpgrades1[Upgrade.FlyBlob] = 4;
                piece.MyUpgrades1[Upgrade.Ceiling] = 2;
                piece.MyUpgrades1[Upgrade.General] = 2;
                piece.MyUpgrades1[Upgrade.Speed] = 4;
                piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

                piece.Style.MyModParams = (level, p) =>
                {
                    FireballEmitter_Parameters Params =  (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.Special.SurvivalFill = true;

                    p.Paths = level.CurMakeData.NumInitialBobs = 1; p.LockNumOfPaths = true;
                };

                RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
                piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
                piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);
            });

            // Add Landing Zone
            data.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.LandingZone;


            GameData.StartLevel(data);
        }

#if WINDOWS
        /// <summary>
        /// Whether the mouse should be allowed to be shown, usually only when a menu is active.
        /// </summary>
        public bool ShowMouse = false;
        
        /// <summary>
        /// Whether the user is using the mouse. False when the mouse hasn't been used since the arrow keys.
        /// </summary>
        public bool MouseInUse = false;
        public bool PrevMouseInUse = false;

        /// <summary>
        /// Draw the mouse cursor.
        /// </summary>
        void MouseDraw()
        {
            if (!MouseInUse) return;
            if (MousePointer == null) return;

            Vector2 Pos = Tools.MouseWorldPos();

            // Draw the mouse hand
            MousePointer.Pos = Pos + new Vector2(.905f * MousePointer.Size.X, -.705f * MousePointer.Size.Y);
            MousePointer.Draw();

            // Draw the mouse dot
            Tools.QDrawer.DrawSquareDot(Pos, Color.Black, 8);

            // Draw the mouse back icon
            if (DrawMouseBackIcon)
            {
                MouseBack.Pos = MousePointer.Pos + new Vector2(44, 98);
                MouseBack.Draw();
            }

            Tools.QDrawer.Flush();
        }

        /// <summary>
        /// Update the boolean flag MouseInUse
        /// </summary>
        void UpdateMouseUse()
        {
            if (Tools.keybState.IsKeyDownCustom(Keys.Up) ||
                Tools.keybState.IsKeyDownCustom(Keys.Down) ||
                Tools.keybState.IsKeyDownCustom(Keys.Left) ||
                Tools.keybState.IsKeyDownCustom(Keys.Right) ||
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Up_Secondary) ||
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Down_Secondary) ||
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Left_Secondary) ||
                Tools.keybState.IsKeyDownCustom(ButtonCheck.Right_Secondary) ||
#if PC_VERSION
                (PlayerManager.Players != null && PlayerManager.Player != null && ButtonCheck.GetMaxDir(true).Length() > .3f)
#else
                (PlayerManager.Players != null && ButtonCheck.GetMaxDir(true).Length() > .3f)
#endif
                )
                MouseInUse = false;

            if (Tools.DeltaMouse != Vector2.Zero ||
                Tools.CurMouseState.LeftButton == ButtonState.Pressed ||
                Tools.CurMouseState.RightButton == ButtonState.Pressed)
                MouseInUse = true;

            PrevMouseInUse = MouseInUse;
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


        public Viewport MainViewport;
        public float SpriteScaling = 1f;

        public double DeltaT = 0;
        protected override void Draw(GameTime gameTime)
        {
#if DEBUG_OBJDATA
ObjectData.UpdateWeak();
#endif
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;

            //if (Tools.SongWad == null) return;

#if WINDOWS
            if (!this.IsActive)
            {
                // The window isn't active, so

                // show the actual mouse (not our custom drawn mouse)
                this.IsMouseVisible = true;

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

                // Comment this line to allow the game to run while not in focus
                //return;
            }
            else
            {
                // The window is active, so

                // hide the actual mouse (we draw our own custom mouse in game)
                this.IsMouseVisible = false;

                if (FirstActiveFrame)
                {
                    // If a song was playing previously when the window was active before,
                    // resume that song
                    if (MediaPlaying_HoldState)
                        MediaPlayer.Resume();

                    FirstActiveFrame = false;
                }

                FirstInactiveFrame = true;
            }
#endif













            // set the viewport to the whole screen
            GraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = GraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = GraphicsDevice.PresentationParameters.BackBufferHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            // clear whole screen to black
            GraphicsDevice.Clear(Color.Black);


            MakeInnerViewport();

            // and clear that
            GraphicsDevice.Clear(Color.Black);

            graphics.GraphicsDevice.Viewport = MainViewport;
            //return;









//#if PC_VERSION
//            UpdateMouseUse();
//#endif

            Tools.DrawCount++;

            if (LogoScreenUp) LogoPhsx();
            else if (LogoScreenPropUp) LoadingScreen.PhsxStep();
#if WINDOWS
            if (Tools.Dlg != null || Tools.DialogUp) return;
#endif

            bool DrawBool = true;
#if PC_VERSION
#else
            DrawBool = !Guide.IsVisible;
#endif

            
            if (!LogoScreenUp)
            if (!Tools.CurGameData.Loading)
            if (DrawBool)
            {
                // Update controller/keyboard states
                if (!LogoScreenUp)
                {
#if WINDOWS
                    Tools.keybState = Keyboard.GetState();
                    Tools.CurMouseState = Mouse.GetState();
#endif           
                    Tools.padState[0] = GamePad.GetState(PlayerIndex.One);
                    Tools.padState[1] = GamePad.GetState(PlayerIndex.Two);
                    Tools.padState[2] = GamePad.GetState(PlayerIndex.Three);
                    Tools.padState[3] = GamePad.GetState(PlayerIndex.Four);

                    ButtonStats.Update();

                    Tools.UpdateVibrations();
                }

#if PC_VERSION
                UpdateMouseUse();
#endif

                // Update sounds
                if (!LogoScreenUp)
                    Tools.SoundWad.Update();

                // Update songs
                if (Tools.SongWad != null)
                    Tools.SongWad.PhsxStep();

                // Track time, changes in time, and FPS
                Tools.gameTime = gameTime;
                DrawCount++;

                float new_t = (float)gameTime.TotalGameTime.TotalSeconds;
                Tools.dt = new_t - Tools.t;
                Tools.t = new_t;
                fps = .3f * fps + .7f * (1000f / (float)Math.Max(.00000231f, gameTime.ElapsedGameTime.Milliseconds));

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


            Vector4 cameraPos = new Vector4(MainCamera.Data.Position.X, MainCamera.Data.Position.Y, MainCamera.Zoom.X, MainCamera.Zoom.Y);//.001f, .001f);

            // All these renderstates need to be ported to XNA 4.0
            /*
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.CullMode = CullMode.None;

            device.RenderState.DestinationBlend = Blend.One;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            */

            Tools.SetStandardRenderStates();


            if (!LogoScreenUp)
            {
                if (!LavaInitialized)
                    Lava();

                if (!Tools.CurGameData.Loading && Tools.CurLevel.PlayMode == 0 && Tools.CurGameData != null && !Tools.CurGameData.Loading && (!Tools.CurGameData.PauseGame || CharacterSelectManager.IsShowing))
                {
                    device.BlendState = BlendState.Additive;
                    Fireball.DrawFireballTexture(device, EffectWad);
                    Fireball.DrawEmitterTexture(device, EffectWad);
                    device.BlendState = BlendState.AlphaBlend;
                    Lava();
                }
            }

            EffectWad.SetCameraPosition(cameraPos);
            
            foreach (EzEffect fx in EffectWad.EffectList) fx.effect.Parameters["xCameraAspect"].SetValue(MainCamera.AspectRatio);
            foreach (EzEffect fx in EffectWad.EffectList) fx.effect.CurrentTechnique = fx.effect.Techniques["Simplest"];
            foreach (EzEffect fx in EffectWad.EffectList) fx.effect.Parameters["t"].SetValue(Tools.t);
            foreach (EzEffect fx in EffectWad.EffectList) fx.effect.Parameters["Illumination"].SetValue(1f);

            Tools.SetStandardRenderStates();
            GraphicsDevice.Clear(Color.Black);

            if (LogoScreenUp || LogoScreenPropUp)
            {
                LoadingScreen.Draw();
                return;
            }

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
                    GraphicsDevice.Clear(Color.Black);

                //if (Tools.CurSongWad != null)
                  //  Tools.CurSongWad.PhsxStep();
            }

            // Debug stat coins
            /*
            if (DrawCount % 20 == 0)
            {
                PlayerData p = PlayerManager.Get(0);
                Console.WriteLine("{0}, {1}, {2}, {3}", p.TempStats.Coins, p.LevelStats.Coins, p.GameStats.Coins, p.LifetimeStats.Coins);
            }*/
                
            //if (ShowFPS || Tools.DebugConvenience)
            if (BuildDebug)
                DrawGC();

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

            base.Draw(gameTime);

            if (!Tools.CapturingVideo)
                if (Tools.SongWad != null)
                    Tools.SongWad.Draw();

            if (Tools.CurLevel != null)
            {
                Tools.EndGUIDraw();
                Tools.QDrawer.Flush();
            }

            // Save screenshot
#if WINDOWS
            if (Tools.ScreenshotMode)
            {
                Tools.Device.SetRenderTarget(null);

                Tools.Screenshot = Tools.DestinationRenderTarget;

                string filename;
                if (Tools.CapturingVideo)
                {
                    Tools.VideoFrame++;
                    filename = VideoFolderName + "\\frame_" + Tools.VideoFrame.ToString() + ".png";
                }
                else
                {
                    Tools.Screenshots++;
                    filename = "Screenshot_" + Tools.Screenshots.ToString() + ".png";
                }

                int Width = Tools.Device.PresentationParameters.BackBufferWidth,
                    Height = Tools.Device.PresentationParameters.BackBufferHeight;
                if (bmpwriter == null ||
                    bmpwriter.Width != Width ||
                    bmpwriter.Height != Height)
                {
                    bmpwriter = new BmpWriter(Width, Height);
                }

                bmpwriter.TextureToBmp(Tools.Screenshot, filename);

                if (!Tools.CapturingVideo)
                    ChangeScreenshotMode();

                Tools.StartSpriteBatch();
                //Tools.spriteBatch.Draw(Tools.TextureWad.TextureList[0].Tex, Vector2.Zero, Color.White);
                Tools.spriteBatch.Draw(Tools.Screenshot, Vector2.Zero, Color.White);
                Tools.EndSpriteBatch();
            }
#endif
        }

        public void MakeInnerViewport()
        {
            float targetAspectRatio = 1280f / 720f;
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            SpriteScaling = width / 1280f;
            int height = (int)(width / targetAspectRatio + .5f);
            if (height > GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                height = GraphicsDevice.PresentationParameters.BackBufferHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            // set up the new viewport centered in the backbuffer
            MainViewport = GraphicsDevice.Viewport = new Viewport
            {
                X = GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - width / 2,
                Y = GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - height / 2,
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };
        }




        const int WaveN = 256;
        Texture2D WaveTexture;
        const int Int = 3;
        Color[] WaveHeightColor = new Color[Int * (WaveN - 1)];
        float[] WaveHeight = new float[WaveN];
        float[] WaveHeightv = new float[WaveN];
        float[] OldWaveHeight = new float[WaveN];
        float[] OldWaveHeightv = new float[WaveN];

        void InitLava()
        {
            WaveTexture = new Texture2D(device, Int * (WaveN - 1), 1, false, SurfaceFormat.Color);
            //WaveTexture = new Texture2D(device, Int * (WaveN - 1), 1, false, SurfaceFormat.Single);
            //WaveTexture = new Texture2D(device, Int * (WaveN - 1), 1, 1, TextureUsage.None, SurfaceFormat.Color);
        }

        public float LavaHeight(float u)
        {
            int i = (int)(WaveN * u);
            return OldWaveHeight[i];
        }

        public bool DoLavaUpdate = false;
        void Lava()
        {
            if (!DoLavaUpdate && LavaInitialized)
                return;
            else
                DoLavaUpdate = false;

            int step = 0;
            if (Tools.CurLevel != null)
                step = Tools.CurLevel.CurPhsxStep;

            if (LavaInitialized)
                LavaPhsx();
            else
            {
                for (int i = 0; i < 15; i++)
                    LavaPhsx();

                LavaInitialized = true;
            }

            for (int i = 0; i < WaveN - 1; i++)
                for (int j = 0; j < Int; j++)
                {
                    float val = (.85f * WaveHeight[i] + .056f) * (1f - j / (float)Int) +
                                (.85f * WaveHeight[i + 1] + .056f) * j / (float)Int;

                    WaveHeightColor[Int * i + j] = new Color(Tools.EncodeFloatRGBA(val));
                }

            EffectWad.FindByName("Lava").effect.Parameters["xHeight"].SetValue((Texture)null);
            device.Textures[0] = null;
            WaveTexture.SetData(WaveHeightColor);
            EffectWad.FindByName("Lava").effect.Parameters["t"].SetValue((float)step);
            EffectWad.FindByName("Lava").effect.Parameters["xHeight"].SetValue(WaveTexture);

            graphics.GraphicsDevice.Textures[1] = WaveTexture;
        }

        bool LavaInitialized;
        void LavaPhsx()
        {
            int step = 0;
            if (Tools.CurLevel != null)
                step = Tools.CurLevel.CurPhsxStep;

            for (int q = 0; q < 1; q++)
            {
                for (int i = 0; i < WaveN; i++)
                {
                    OldWaveHeight[i] = WaveHeight[i];
                    OldWaveHeightv[i] = WaveHeightv[i];
                }

                float h = .5f * 1.05f * 1f / WaveN;
                float dt = .05f;

                for (int i = 0; i < WaveN; i++)
                {
                    float z = (OldWaveHeightv[(i + 1) % WaveN] + OldWaveHeightv[(i - 1 + WaveN) % WaveN] - 2 * OldWaveHeightv[i]) / (h);
                    float w = (OldWaveHeight[(i + 1) % WaveN] + OldWaveHeight[(i - 1 + WaveN) % WaveN] - 2 * OldWaveHeight[i]) / (h);
                    WaveHeightv[i] += dt * (
                        .12f * ((float)Math.Abs(OldWaveHeight[i] * 15 + .033f) + .006f) * w
                        + .00075f * (float)Math.Sin((2 * i * h + (float)step * .3) * 3.14159f * 6)
                        - .0046f * (.04f - Math.Abs(z)) * Math.Sign(z)
                        - .0046f * (.02f - Math.Abs(w)) * Math.Sign(w)
                        - .06f * OldWaveHeightv[i]
                        - 38 * OldWaveHeight[i] * OldWaveHeight[i] * OldWaveHeight[i]
                        - 2 * Math.Abs(OldWaveHeight[i] * OldWaveHeightv[i]) * Math.Sign(OldWaveHeightv[(i + WaveN / 2) % WaveN])
                        - 4 * Math.Abs(OldWaveHeight[i] * OldWaveHeightv[i]) * Math.Sign(OldWaveHeightv[(i + (int)(WaveN * 1.3f)) % WaveN])
                        + .0043f * (.35f - Math.Abs(OldWaveHeight[i])) * Math.Sign(OldWaveHeight[i])
                                     );

                    if (Math.Abs(WaveHeightv[i]) > .047f)
                        WaveHeightv[i] = .047f * Math.Sign(WaveHeightv[i]);
                }

                for (int i = 0; i < WaveN; i++)
                {
                    WaveHeight[i] += dt * 3.6f * WaveHeightv[i];

                    if (Math.Abs(WaveHeight[i]) > .1125f)
                    {
                        WaveHeight[i] = .1125f * Math.Sign(WaveHeight[i]);
                    }
                }
            }
        }

        #if WINDOWS
        BmpWriter bmpwriter;
        #endif
    }

    #if WINDOWS
    public class BmpWriter
    {
        byte[] textureData;
        System.Drawing.Bitmap bmp;
        System.Drawing.Imaging.BitmapData bitmapData;
        IntPtr safePtr;
        System.Drawing.Rectangle rect;
        public System.Drawing.Imaging.ImageFormat imageFormat;

        public int Width, Height;

        public BmpWriter(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            textureData = new byte[4 * width * height];

            bmp = new System.Drawing.Bitmap(
                           width, height,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb
                         );

            rect = new System.Drawing.Rectangle(0, 0, width, height);

            imageFormat = System.Drawing.Imaging.ImageFormat.Png;
        }

        public void TextureToBmp(Texture2D texture, String filename, bool DelaySave = false)
        {
            texture.GetData<byte>(textureData);
            byte blue;
            for (int i = 0; i < textureData.Length; i += 4)
            {
                blue = textureData[i];
                textureData[i] = textureData[i + 2];
                textureData[i + 2] = blue;
            }

            //bitmapData = bmp.LockBits(
            //               rect,
            //               System.Drawing.Imaging.ImageLockMode.WriteOnly,
            //               System.Drawing.Imaging.PixelFormat.Format32bppArgb
            //             );

            safePtr = bitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
            //bmp.UnlockBits(bitmapData);

            if (DelaySave)
                ;
            else
                bmp.Save(filename, imageFormat);
        }
    }
    #endif
}
