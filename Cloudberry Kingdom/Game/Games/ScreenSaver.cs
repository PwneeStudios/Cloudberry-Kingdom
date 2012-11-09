using System.Collections.Generic;
using CloudberryKingdom.Levels;
using System.Threading;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Blocks;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ScreenSaver : StringWorldGameData
    {
        public bool ForTrailer = false;

        bool Bungee = false;
        bool AllHeroes = false;
        int Difficulty = 4;
        int Paths = 1;
        BobPhsx FixedHero = BobPhsxNormal.Instance;
        TileSet FixedTileSet = null;

        float InitialZoom = 0f;
        void ForTrailerParams()
        {
            ForTrailer = true;

            Bungee = false;
            AllHeroes = true;
            Difficulty = 4;
            Paths = 1;
            FixedHero = BobPhsxNormal.Instance;
            FixedTileSet = "sea";

            InitialFadeInSpeed = 1;
            InitialDarkness = 0;
            PartialZoomOut += 260;
            FullZoomOut += 260;
            KillCapeDelay += 260;
            InitialDelay += 0;
            
            //InitialZoom = .45f;
            InitialZoom = 1f;
        }

        public override void Init()
        {
            base.Init();

            Tools.WorldMap = Tools.CurGameData = this;
            Tools.CurLevel = this.MyLevel;
        }

        GUI_Text PressA;

        public static int MandatoryWatchLength
        {
            get 
            {
                if (UserPowers.CanSkipScreensaver)
                    return 0;
                else
                    return MandatoryWatchLength_Initial;
            }
        }
        const int MandatoryWatchLength_Initial = 400;

        float InitialFadeInSpeed = .01f;
        static int InitialDarkness = 30;
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

        class OnSwapLambda : Lambda_1<LevelSeedData>
        {
            ScreenSaver ch;
            public OnSwapLambda(ScreenSaver ch)
            {
                this.ch = ch;
            }

            public void Apply(LevelSeedData data)
            {
                Tools.ShowLoadingScreen = false;
                Tools.TheGame.LogoScreenPropUp = false;
                Tools.Write("+++++++++++++++++++ Ending screensave load...");
            }
        }

        void Constructor()
        {
            WaitLengthToOpenDoor_FirstLevel = 10 + InitialDarkness - 3;

            Tools.TheGame.LogoScreenPropUp = true;
            Tools.Write("+++++++++++++++++++ Beginning screensave load...");

            this.GetSeedFunc = Make;
            
            //OnSwapToFirstLevel += (data) =>
            //    {
            //        //Tools.TextureWad.LoadThread.Join();
            //        Tools.ShowLoadingScreen = false;
            //        Tools.TheGame.LogoScreenPropUp = false;
            //        Tools.Write("+++++++++++++++++++ Ending screensave load...");
            //    };

            OnSwapToFirstLevel.Add(new OnSwapLambda(this));

            OnSwapToLevel += index =>
                {
                    // Hide the 'Press A to start' text after the first level
                    if (index > 0)
                        PressA.Hid = true;

                    Tools.CurLevel.SuppressSounds = true;

                    Tools.CurLevel.WatchComputer(false);  // Watch the computer
                    Tools.CurGameData.PhsxStepsToDo += 1; // Skip beginning 
                    Tools.CurGameData.SuppressSoundForExtraSteps = true;
                    Duration = 10000;

                    bool First = index == 0;
                    //bool First = true;

                    if (ForTrailer)
                    {
                        var Bobs = Tools.CurLevel.Bobs;
                        for (int i = 0; i < 4; i++)
                            if (Bobs.Count > i) Bobs[i].SetColorScheme(ColorSchemeManager.ColorSchemes[i]);
                    }

                    if (First)
                    {
                        Tools.CurGameData.SuppressSongInfo = true;

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
                            UserPowers.Set(ref UserPowers.CanSkipScreensaver, true);

#if PC_VERSION
                            PressA = new GUI_Text(Localization.Words.PressAnyKey,
                                                           new Vector2(0, -865), true);
#else
                            PressA = new GUI_Text(Localization.Words.PressAnyKey,
                                                           new Vector2(0, -865), true);
#endif
                            PressA.MyText.Scale *= .68f;
                            PressA.PreventRelease = true;
                            PressA.FixedToCamera = true;
                            PressA.Oscillate = true;
                            if (!ForTrailer)
                                Tools.CurGameData.AddGameObject(PressA);
                        }, true);

                        Tools.CurGameData.WaitThenDo(MandatoryWatchLength + InitialDarkness - 3, () =>
                        {
                            Listener PressA_Listener = null;
                            PressA_Listener = new Listener(ControllerButtons.A, () =>
                                 {
                                     Tools.CurGameData.FadeToBlack(.0275f);
                                     Tools.SongWad.FadeOut();
                                     DoBackgroundPhsx = false;

                                     Tools.CurGameData.WaitThenDo(55, () =>
                                         {
                                             Tools.CurGameData = CloudberryKingdomGame.TitleGameFactory();
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

                    lvl.MyGame.WaitThenDo(InitialDarkness, ()
                        => lvl.MyGame.FadeIn(InitialFadeInSpeed));

                    //if (ForTrailer)
                    //{
                    //    lvl.MyGame.WaitThenDo(240, () => zoom_t.LerpTo(InitialZoom, 1f, 100, LerpStyle.Linear));
                    //}
                    //else
                    {
                        lvl.MyGame.WaitThenDo(PartialZoomOut, () => zoom_t.LerpTo(.6f, 90, LerpStyle.Sigmoid));
                        int zoomout_length = 21;
                        int zoomout_start = FullZoomOut + InitialDarkness - 3;
                        LerpStyle style = LerpStyle.Sigmoid;
                        lvl.MyGame.WaitThenDo(zoomout_start, () => zoom_t.LerpTo(1f, zoomout_length, style));
                        lvl.MyGame.WaitThenDo(zoomout_start, () => pos_t.LerpTo(1f, zoomout_length + 6, style));

                        lvl.MyGame.WaitThenDo(KillCapeDelay + InitialDarkness, () => wind_t.LerpTo(1f, 40));

                        lvl.MyGame.WaitThenDo(zoomout_start - 3 - 3, () =>
                            Tools.SoundWad.FindByName("Record_Scratch").Play());
                        Tools.SongWad.SetPlayList("Ripcurl^Blind_Digital");
                        Tools.SongWad.Restart(true, false);
                        Tools.SongWad.Pause();
                        lvl.MyGame.WaitThenDo(zoomout_start + zoomout_length + 28, () =>
                        {
                            // Start the music
                            Tools.SongWad.Unpause();
                        });
                    }
                }

                lvl.Bobs[0].CapeWind = CoreMath.LerpRestrict(2.7f, 0, wind_t.Val) *
                    Cape.SineWind(new Vector2(-1, .15f), .75f + .3f, 4.5f, lvl.CurPhsxStep);

                Camera cam = lvl.MainCamera;
                cam.UseEffective = true;
                cam.EffectivePos = lvl.Bobs[0].Pos;
                cam.EffectiveZoom = new Vector2(.0025f);

                cam.EffectivePos = CoreMath.LerpRestrict(lvl.Bobs[0].Pos, cam.Data.Position, pos_t.Val);
                cam.EffectivePos.Y = cam.Data.Position.Y;
                cam.EffectiveZoom = new Vector2(CoreMath.LerpRestrict(.0025f, .001f, zoom_t.Val));
            }
            

            if (DoBackgroundPhsx &&
                NextIsReady() &&
                (Tools.CurLevel.CurPhsxStep > Duration || Tools.CurLevel.CurPhsxStep > Tools.CurLevel.CurPiece.PieceLength - 50))
            {
                SetLevel();
                Recycler.DumpMetaBin();
            }
        }

        LevelSeedData Make(int index)
        {
            bool First = index == 0;

            BobPhsx hero = FixedHero;
            if (hero == null)
            {
                var l = new BobPhsx[] {
                        BobPhsxSmall.Instance,
                        BobPhsxBig.Instance,
                        BobPhsxBouncy.Instance,
                        BobPhsxInvert.Instance,
                        BobPhsxSpaceship.Instance,
                        BobPhsxScale.Instance,
                        BobPhsxWheel.Instance,
                        BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack),
                        BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Classic, Hero_MoveMod.Double) };
                hero = l[index % l.Length];
            }

            int Length = 6700;

            // Create the LevelSeedData
            LevelSeedData data;
            if (Difficulty >= 0)
                data = RegularLevel.HeroLevel(Difficulty, hero, Length);
            else
                data = RegularLevel.HeroLevel(index % 5, hero, Length);

            if (Bungee)
            {
                data.MyGameFlags.IsTethered = true;
            }

            if (FixedTileSet == null)
                data.SetTileSet(Tools.GlobalRnd.ChooseOne("forest", "cave", "castle", "cloud", "hills", "sea"));
            else
                data.SetTileSet(FixedTileSet);
            
            //data.SetTileSet(Tools.GlobalRnd.ChooseOne("sea", "forest", "cave", "castle", "cloud", "hills",
            //                                          "sea_rain", "forest_snow", "hills_rain"));

            // Adjust the piece seed data
            foreach (PieceSeedData piece in data.PieceSeeds)
            {
                if (First)
                {
                    FirstLevel(index, piece);
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

                // Only one path
                piece.Paths = Paths; piece.LockNumOfPaths = true;
            }

            return data;
        }

        private void FirstLevel(int index, PieceSeedData piece)
        {
            piece.MyUpgrades1.Zero();
            piece.MyUpgrades2.Zero();

            float Difficulty = 8.25f;
            piece.MyUpgrades1[Upgrade.Pinky] = Difficulty;
            piece.MyUpgrades1[Upgrade.Spike] = Difficulty;
            piece.MyUpgrades1[Upgrade.SpikeyGuy] = Difficulty;
            piece.MyUpgrades1[Upgrade.Jump] = 0;
            piece.MyUpgrades1[Upgrade.Speed] = 9;
            piece.MyUpgrades1[Upgrade.Ceiling] = 7;

            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);
        }

        private void MultiplayerBlobs(int index, PieceSeedData piece)
        {
            //// Easy/Masochistic
            //piece.MyUpgrades1.Zero();
            //piece.MyUpgrades2.Zero();

            if (index % 2 == 1)
            {
                //piece.MyUpgrades1[Upgrade.Pinky] = 9;
                //piece.MyUpgrades1[Upgrade.Spike] = 9;
                //piece.MyUpgrades1[Upgrade.SpikeyGuy] = 9;
                //piece.MyUpgrades1[Upgrade.Laser] = 9;
                //piece.MyUpgrades1[Upgrade.SpikeyLine] = 9;
                //piece.MyUpgrades1[Upgrade.Firesnake] = 4;
                //piece.MyUpgrades1[Upgrade.FireSpinner] = 9;
                //piece.MyUpgrades1[Upgrade.MovingBlock] = 7;
                //piece.MyUpgrades1[Upgrade.GhostBlock] = 8;
                //piece.MyUpgrades1[Upgrade.Jump] = 2;
                //piece.MyUpgrades1[Upgrade.Speed] = 9;
                //piece.MyUpgrades1[Upgrade.Ceiling] = 7;
            }

            piece.MyUpgrades1[Upgrade.Spike] = 9;
            piece.MyUpgrades1[Upgrade.FlyBlob] = 9;
            if (index % 4 == 1)
                piece.MyUpgrades1[Upgrade.Fireball] = 9;
            piece.MyUpgrades1[Upgrade.Jump] = 1;
            piece.MyUpgrades1[Upgrade.Speed] = 7;
            piece.MyUpgrades1[Upgrade.Ceiling] = 7;

            piece.Style.FunRun = false;
            piece.Style.PauseType = StyleData._PauseType.Normal;
            piece.Style.MoveTypePeriod = StyleData._MoveTypePeriod.Normal1;
            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);
            piece.Paths = Paths; piece.LockNumOfPaths = true; LevelSeedData.NoDoublePaths = false;

            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

            piece.Style.MyModParams = (level, p) =>
            {
                FlyingBlob_Parameters GParams = (FlyingBlob_Parameters)p.Style.FindParams(FlyingBlob_AutoGen.Instance);
                GParams.KeepUnused.SetVal(MyLevel.Rnd.RndBool(.5f) ? 0f : MyLevel.Rnd.RndFloat(0, .06f));
                GParams.FillWeight.SetVal(100);
                GParams.Period.SetVal(115);
                GParams.Range.SetVal(600);
            };

            /*
            bool Custom = MyLevel.Rnd.RndBool();

            if (Custom)
            {
                piece.MyUpgrades1.Zero();
                piece.MyUpgrades2.Zero();

                switch (index % 4)
                //switch (0)
                {
                    case 0:
                        piece.MyUpgrades1[Upgrade.FlyBlob] = 10;
                        piece.MyUpgrades1[Upgrade.Speed] = 10;
                        piece.MyUpgrades1[Upgrade.Jump] = 1;
                        piece.MyUpgrades1[Upgrade.Speed] = 11;
                        piece.MyUpgrades1[Upgrade.Ceiling] = 5;

                        piece.MyUpgrades1[Upgrade.SpikeyLine] = 5.5f;
                        piece.MyUpgrades1[Upgrade.Firesnake] = 5.5f;
                        piece.MyUpgrades1[Upgrade.Elevator] = 7f;
                        break;

                    case 1:
                        piece.MyUpgrades1[Upgrade.MovingBlock] = 7.5f;
                        piece.MyUpgrades1[Upgrade.FlyBlob] = 7;
                        piece.MyUpgrades1[Upgrade.FireSpinner] = 5;
                        piece.MyUpgrades1[Upgrade.Speed] = 12;
                        piece.MyUpgrades1[Upgrade.Jump] = 5;
                        piece.MyUpgrades1[Upgrade.Speed] = 8;
                        piece.MyUpgrades1[Upgrade.Ceiling] = 8;
                        break;

                    case 2:
                        piece.MyUpgrades1[Upgrade.MovingBlock] = 7.5f;
                        piece.MyUpgrades1[Upgrade.Firesnake] = 3.5f;
                        piece.MyUpgrades1[Upgrade.Elevator] = 3.5f;
                        piece.MyUpgrades1[Upgrade.Speed] = 10;
                        piece.MyUpgrades1[Upgrade.Jump] = 5;
                        piece.MyUpgrades1[Upgrade.Speed] = 11;
                        piece.MyUpgrades1[Upgrade.Ceiling] = 8;
                        break;

                    case 3:
                        piece.MyUpgrades1[Upgrade.MovingBlock] = 7.5f;
                        piece.MyUpgrades1[Upgrade.FlyBlob] = 7;
                        piece.MyUpgrades1[Upgrade.Firesnake] = 3.5f;
                        piece.MyUpgrades1[Upgrade.Spike] = 3.5f;
                        piece.MyUpgrades1[Upgrade.SpikeyLine] = 3.5f;
                        piece.MyUpgrades1[Upgrade.Elevator] = 6f;
                        piece.MyUpgrades1[Upgrade.Speed] = 10;
                        piece.MyUpgrades1[Upgrade.Jump] = 5;
                        piece.MyUpgrades1[Upgrade.Speed] = 11;
                        piece.MyUpgrades1[Upgrade.Ceiling] = 8;
                        break;
                }
            }


            piece.Style.FunRun = false;
            piece.Style.PauseType = StyleData._PauseType.Normal;
            piece.Style.MoveTypePeriod = StyleData._MoveTypePeriod.Normal1;
            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

            //piece.Paths = 4; piece.LockNumOfPaths = true; LevelSeedData.NoDoublePaths = false;
            if (Custom)
                piece.Style.MyModParams = (level, p) =>
                {
                    Goomba_Parameters GParams = (Goomba_Parameters)p.Style.FindParams(Goomba_AutoGen.Instance);
                    GParams.KeepUnused = MyLevel.Rnd.RndBool(.5f) ? 0f : MyLevel.Rnd.RndFloat(0, .06f);
                    GParams.FillWeight = 100;
                    GParams.Period = 115;
                    GParams.Range = 600;

                    //MovingBlock_Parameters MParams = (MovingBlock_Parameters)p.Style.FindParams(MovingBlock_AutoGen.Instance);
                    //MParams.KeepUnused = 0;
                    //MParams.Aspect = MovingBlock_Parameters.AspectType.Square;
                    //MParams.FillWeight = 100;
                    //MParams.Period = 116;
                    //MParams.Range = 720;
                };
                * */
        }
    }
}