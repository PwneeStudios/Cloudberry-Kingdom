using System.Collections.Generic;
using CloudberryKingdom.Levels;
using System.Threading;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Blocks;
using Drawing;

namespace CloudberryKingdom
{
    public class ScreenSaver : StringWorldGameData
    {
        public bool ForTrailer = false;

        /// <summary>
        /// Whether the game introduction has been watched before or not
        /// </summary>
        public static bool WatchedIntro { get { return _WatchedIntro; } set { _WatchedIntro = value; PlayerManager.SavePlayerData.Changed = true; } }
        static bool _WatchedIntro = false;

        public void InitForTrailer()
        {
            InitToRecord();
        }

        float InitialZoom = 0f;
        void ForTrailerParams()
        {
            ForTrailer = true;

            InitialFadeInSpeed = 1;
            InitialDarkness = 0;
            PartialZoomOut += 260;
            FullZoomOut += 260;
            KillCapeDelay += 260;
            InitialDelay += 0;
            
            //InitialZoom = .45f;
            InitialZoom = 1f;
        }

        bool ForRecording = false;
        public void InitToRecord()
        {
            Init();
            ForRecording = true;
        }

        public override void Init()
        {
            base.Init();

            Tools.WorldMap = Tools.CurGameData = this;
            Tools.CurLevel = this.MyLevel;
        }

        StartMenu menu;
        GUI_Text PressA;

        public static int MandatoryWatchLength
        {
            get 
            {
                if (WatchedIntro)
                    return 0;
                else
                    return MandatoryWatchLength_Initial;
            }
        }
        const int MandatoryWatchLength_Initial = 400;

        float InitialFadeInSpeed = .01f;
        static int InitialDarkness = 30;//3;
        int PartialZoomOut = 60 + InitialDarkness - 3, FullZoomOut = 180, KillCapeDelay = 200;
        int InitialDelay = 210 + InitialDarkness - 3;
        public ScreenSaver()
        {
            Constructor();
        }
        public ScreenSaver(bool ForTrailer)
        {
            this.ForTrailer = ForTrailer;
            if (ForTrailer) ForTrailerParams();
            Constructor();
        }
        void Constructor()
        {
            if (ForTrailer)
                WaitLengthToOpenDoor_FirstLevel = 135 + InitialDarkness - 3;
            else
                WaitLengthToOpenDoor_FirstLevel = 10 + InitialDarkness - 3;

            Tools.TheGame.LogoScreenPropUp = true;
            Tools.Write("+++++++++++++++++++ Beginning screensave load...");

            LevelSeeds = new List<LevelSeedData>();
            for (int i = 0; i < 5; i++)
                LevelSeeds.Add(Make(i));
            
            OnSwapToFirstLevel += (data) =>
                {
                    //Tools.TextureWad.LoadThread.Join();
                    Tools.ShowLoadingScreen = false;
                    Tools.TheGame.LogoScreenPropUp = false;
                    Tools.Write("+++++++++++++++++++ Ending screensave load...");

                    // Bring dialog to save video
                    if (ForRecording)
                        data.MyGame.WaitThenDo(3, () =>
                            Tools.TheGame.SetToBringSaveVideoDialog = true);
                };

            OnSwapToLevel += index =>
                {
                    if (!ForRecording)
                    {
                        // Hid the 'Press A to start' text after the first level
                        if (index > 0)
                            PressA.Hid = true;
                    }

                    if (ForTrailer)
                    {
                        Camera.DisableOscillate = true;
                        foreach (Block block in Tools.CurLevel.Blocks)
                            if (block.Pos.X > Tools.CurLevel.Bobs[0].Pos.X + 200)
                                block.CollectSelf();
                    }

                    Tools.CurLevel.SuppressSounds = true;

                    LevelSeeds.Add(Make(LevelSeeds.Count));                     // Replenish pool of level seeds
                    Tools.CurLevel.WatchComputer(false);                        // Watch the computer
                    Tools.CurGameData.PhsxStepsToDo += 1;// Tools.RndInt(150, 190);   // Skip beginning 
                    Tools.CurGameData.SuppressSoundForExtraSteps = true;
                    //Duration = Tools.RndInt(100, 330) + Tools.CurGameData.PhsxStepsToDo;
                    Duration = 10000;

                    bool First = index == 0;
                    //bool First = true;

                    if (First)
                    {
                        Tools.SongWad.FadeOut();

                        Tools.CurLevel.Bobs[0].SetColorScheme(ColorSchemeManager.ComputerColorSchemes[0]);

                        Tools.CurLevel.Bobs[0].PlayerObject.EnqueueAnimation(0, 0, true, true, false, 100);

                        pos_t = zoom_t = null;
                        Tools.CurGameData.FadeIn(0);
                    }

                    // Add 'Press (A) to start' text
                    if (index == 0)
                    {
                        Tools.CurGameData.WaitThenDo(MandatoryWatchLength_Initial + InitialDarkness - 3, () =>
                        {
                            WatchedIntro = true;
                            if (ForRecording) return;

                            PressA = new GUI_Text("Press " + ButtonString.Go(97) + " to start",
                                                           new Vector2(0, -865), true);
                            PressA.MyText.Scale *= .68f;
                            PressA.PreventRelease = true;
                            PressA.FixedToCamera = true;
                            PressA.Oscillate = true;
                            Tools.CurGameData.AddGameObject(PressA);
                        }, true);

                        Tools.CurGameData.WaitThenDo(MandatoryWatchLength + InitialDarkness - 3, () =>
                        {
                            if (ForRecording) return;

                            Listener PressA_Listener = null;
                            PressA_Listener = new Listener(ControllerButtons.A, () =>
                                 {
                                     Tools.CurGameData.FadeToBlack(.0275f);
                                     Tools.SongWad.FadeOut();
                                     DoBackgroundPhsx = false;

                                     Tools.CurGameData.WaitThenDo(55, () =>
                                         {
                                             Tools.CurGameData = new TitleGameData();
                                             Tools.CurGameData.FadeIn(.0275f);
                                             Tools.AddToDo(() => this.Release());
                                         });

                                     PressA_Listener.Release();
                                     if (PressA != null) PressA.Kill(true);
                                 });
                            PressA_Listener.PreventRelease = true;
                            PressA_Listener.Control = -2;
                            Tools.CurGameData.AddGameObject(PressA_Listener);
                        }, true);
                    }
                };
        }

        int PhsxCount = 0;

        int Duration = 0;
        FancyVector2 pos_t, zoom_t, wind_t;

        public override void UpdateGamePause()
        {
            base.UpdateGamePause();
            if (Tools.TheGame.LoadingScreen != null && !Tools.TheGame.LoadingScreen.IsDone)
                PauseGame = true;
        }

        bool DoBackgroundPhsx = true;
        public override void BackgroundPhsx()
        {
 	        base.BackgroundPhsx();

            if (Tools.TheGame.LoadingScreen != null && !Tools.TheGame.LoadingScreen.IsDone)
            {
                Tools.TheGame.LoadingScreen.Accelerate = true;
                return;
            }

            PhsxCount++;

            Level lvl = Tools.CurLevel;
            if (lvl != null && lvl.Bobs.Count > 0)
            {
                if (pos_t == null)
                {
                    pos_t = new FancyVector2();
                    pos_t.Val = 0f;
                    zoom_t = new FancyVector2();
                    zoom_t.Val = InitialZoom;
                    wind_t = new FancyVector2();
                    wind_t.Val = 0f;

                    lvl.MyGame.WaitThenDo(InitialDarkness, () => lvl.MyGame.FadeIn(InitialFadeInSpeed));

                    if (ForTrailer)
                    {
                        //lvl.MyGame.WaitThenDo(240, () => zoom_t.LerpTo(InitialZoom, 1f, 100, LerpStyle.Linear));
                    }
                    else
                    {
                        lvl.MyGame.WaitThenDo(PartialZoomOut, () => zoom_t.LerpTo(.6f, 90, LerpStyle.Sigmoid));
                        int zoomout_length = 21;
                        int zoomout_start = FullZoomOut + InitialDarkness - 3;
                        LerpStyle style = LerpStyle.Sigmoid;
                        lvl.MyGame.WaitThenDo(zoomout_start, () => zoom_t.LerpTo(1f, zoomout_length, style));
                        lvl.MyGame.WaitThenDo(zoomout_start, () => pos_t.LerpTo(1f, zoomout_length + 6, style));

                        lvl.MyGame.WaitThenDo(KillCapeDelay + InitialDarkness, () => wind_t.LerpTo(1f, 40));

                        lvl.MyGame.WaitThenDo(zoomout_start - 3 - 3, () =>
                            Tools.SoundWad.FindByName("Record Scratch").Play());
                        Tools.SongWad.SetPlayList("Ripcurl^Blind Digital");
                        Tools.SongWad.Restart(true, false);
                        Tools.SongWad.Pause();
                        lvl.MyGame.WaitThenDo(zoomout_start + zoomout_length + 28, () =>
                        {
                            // Start the music
                            Tools.SongWad.Unpause();
                        });
                    }
                }

                lvl.Bobs[0].CapeWind = Tools.LerpRestrict(2.7f, 0, wind_t.Val) *
                    Cape.SineWind(new Vector2(-1, .15f), .75f + .3f, 4.5f, lvl.CurPhsxStep);

                Camera cam = lvl.MainCamera;
                cam.UseEffective = true;
                cam.EffectivePos = lvl.Bobs[0].Pos;
                cam.EffectiveZoom = new Vector2(.0025f);

                cam.EffectivePos = Tools.LerpRestrict(lvl.Bobs[0].Pos, cam.Data.Position, pos_t.Val);
                cam.EffectivePos.Y = cam.Data.Position.Y;
                cam.EffectiveZoom = new Vector2(Tools.LerpRestrict(.0025f, .001f, zoom_t.Val));
            }
            

            if (DoBackgroundPhsx &&
                IsLoaded(CurLevelIndex + 1) &&
                //true)
                (Tools.CurLevel.CurPhsxStep > Duration || Tools.CurLevel.CurPhsxStep > Tools.CurLevel.CurPiece.PieceLength - 50))
            {
                SetLevel(CurLevelIndex + 1);
                Recycler.DumpMetaBin();
            }
        }

        LevelSeedData Make(int index)
        {
            //bool First = true;
            bool First = index == 0;

            BobPhsx hero = BobPhsxNormal.Instance;// Bob.HeroTypes[(Index - StartIndex) % Bob.HeroTypes.Count];

            int Length = 6700;
            //int Length = 4000;

            // Create the LevelSeedData
            LevelSeedData data = RegularLevel.HeroLevel(4, hero, Length);

            if (First)
                data.SetBackground(BackgroundType.Dungeon);

            if (ForTrailer)
            {
                //data.SetBackground(BackgroundType.NightSky);
                data.SetBackground(BackgroundType.Sky);
                data.SetTileSet(TileSet.Dungeon);
            }

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                if (First)
                {
                    piece.MyUpgrades1.Zero();
                    piece.MyUpgrades2.Zero();

                    /*
                    piece.MyUpgrades1[Upgrade.FireSpinner] = 9;
                    piece.MyUpgrades1[Upgrade.FlyBlob] = 2;
                    piece.MyUpgrades1[Upgrade.MovingBlock] = 2;
                    piece.MyUpgrades1[Upgrade.Pinky] = 9;
                    piece.MyUpgrades1[Upgrade.Spike] = 9;
                    piece.MyUpgrades1[Upgrade.SpikeyGuy] = 9;
                    piece.MyUpgrades1[Upgrade.Jump] = 4;
                    piece.MyUpgrades1[Upgrade.Speed] = 7;
                    piece.MyUpgrades1[Upgrade.Ceiling] = 7;
                    */

                    if (ForTrailer)
                    {
                        piece.MyUpgrades1[Upgrade.Jump] = 0;
                    }
                    else
                    {
                        piece.MyUpgrades1[Upgrade.Pinky] = 7.7f;
                        piece.MyUpgrades1[Upgrade.Spike] = 7.7f;
                        piece.MyUpgrades1[Upgrade.SpikeyGuy] = 7.7f;
                        piece.MyUpgrades1[Upgrade.Jump] = 0;
                        piece.MyUpgrades1[Upgrade.Speed] = 9;
                        piece.MyUpgrades1[Upgrade.Ceiling] = 7;
                    }

                    piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
                    piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
                    piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);
                }

                // Shorten the initial computer delay
                if (First)
                {
                    SingleData style = piece.Style as SingleData;
                    style.ComputerWaitLengthRange = new Vector2(InitialDelay);
                    style.InitialDoorYRange = new Vector2(-200);
                }
                else
                    piece.Style.ComputerWaitLengthRange = new Vector2(0);

                // No balls to the wall
                piece.Style.FunRun = false;

                if (ForTrailer)
                    piece.Style.AlwaysEdgeJump = true;

                // Only one path
                piece.Paths = 1; piece.LockNumOfPaths = true;

                piece.Style.MyModParams = (level, p) =>
                {
                    Coin_Parameters Params = (Coin_Parameters)p.Style.FindParams(Coin_AutoGen.Instance);
                    Params.FillType = Coin_Parameters.FillTypes.Regular;

                    if (ForTrailer)
                    {
                        Params.FillType = Coin_Parameters.FillTypes.None;
                    }
                };
            }

            return data;
        }
    }
}