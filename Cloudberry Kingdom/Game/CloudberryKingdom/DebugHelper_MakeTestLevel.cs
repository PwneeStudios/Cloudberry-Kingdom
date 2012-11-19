#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        void MakeEmptyLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();
            level.CurPiece = level.StartNewPiece(0, null, 4);
            level.CurPiece.StartData[0].Position = new Vector2(0, 0);
            level.MainCamera.BLCamBound = new Vector2(-100000, 0);
            level.MainCamera.TRCamBound = new Vector2(100000, 0);
            level.MainCamera.Update();
            level.TimeLimit = -1;

            level.MyBackground = new RegularBackground();
            level.MyBackground.Init(level);

            Tools.CurGameData = level.MyGame = new GameData();
            Tools.CurGameData.MyLevel = Tools.CurLevel = level;
        }

        void MakeTestLevel()
        {
            //PlayerManager.Players[0].Exists = true;
            //PlayerManager.Players[1].Exists = true;
            //PlayerManager.Players[2].Exists = true;
            //PlayerManager.Players[3].Exists = true;

            LevelSeedData data = new LevelSeedData();


            //data.ReadString("0;s:230413531;h:2,0,2,0;t:castle;l:6000;n:2;u:2,0,0,0,0,0,0,0,0,0,0,0,1,5,0,0,0,0,0,0,0,3,8;");
            //data.ReadString("seed 0;s:305632318;h:0,0,0,0;t:hills_rain;n:2;l:5110;u:0,0,0,0,2.319968,0,0,1.375736,0,0,3.318264,0,0,0,0,0,0,0,0,0,0,1.659075,2.474871,0;u:0,0,0,0,2.319968,0,0,1.375736,0,0,3.318264,0,0,0,0,0,0,0,0,0,0,1.659075,2.474871,0;fadein;opendoor:30;opendoorsound;song:Writer's_Block^Peacemaker;weather:1;");
            //GameData.StartLevel(data);
            //return;

            data.Seed = Tools.GlobalRnd.Rnd.Next();
            //data.Seed = 110040853;
            Console.WriteLine("Seed chosen = {0}", data.Seed);

            //data.MyBackgroundType = BackgroundType.Dungeon;

            //TileSetToTest = "sea";
            TileSetToTest = "hills";
            //TileSetToTest = "forest_snow";
            //TileSetToTest = "cloud";
            //TileSetToTest = "cave";
            //TileSetToTest = "castle";

            if (TileSetToTest == null)
                data.SetTileSet("castle");
            else
                data.SetTileSet(TileSetToTest);

            //data.SetTileSet(TileSets.Dungeon);

            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Bouncy, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Box, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Small, Hero_MoveMod.Double);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Double);

            data.DefaultHeroType = BobPhsxNormal.Instance;
            //data.DefaultHeroType = BobPhsxBouncy.Instance;
            //data.DefaultHeroType = BobPhsxWheel.Instance;
            //data.DefaultHeroType = BobPhsxTime.Instance;
            //data.DefaultHeroType = BobPhsxInvert.Instance;
            //data.DefaultHeroType = BobPhsxMeat.Instance;
            //data.DefaultHeroType = BobPhsxDouble.Instance;
            //data.DefaultHeroType = BobPhsxSpaceship.Instance;
            //data.DefaultHeroType = BobPhsxRocketbox.Instance;
            //data.DefaultHeroType = BobPhsxSmall.Instance;
            //data.DefaultHeroType = BobPhsxBig.Instance;
            //data.DefaultHeroType = BobPhsxScale.Instance;
            //data.DefaultHeroType = BobPhsxJetman.Instance;
            //data.DefaultHeroType = BobPhsxBox.Instance;

            // 8-jumps
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Double);
            //var d = new BobPhsx.CustomPhsxData();
            //d.Init();
            //d[BobPhsx.CustomData.numjumps] = 2;
            //data.DefaultHeroType.SetCustomPhsx(d);

            // Long jetpack
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //var d = new BobPhsx.CustomPhsxData();
            //d.Init();
            //d[BobPhsx.CustomData.jetpackfuel] *= 2;
            //d[BobPhsx.CustomData.jetpackaccel] *= 2;
            //data.DefaultHeroType.SetCustomPhsx(d);


            data.MyGeometry = LevelGeometry.Right;
            //data.MyGeometry = LevelGeometry.Up;
            //data.PieceLength = 90000;
            data.PieceLength = 15000;
            //data.PieceLength = 37000;
            data.NumPieces = 1;

            data.MyGameType = NormalGameData.Factory;
            //data.MyGameType = PlaceGameData.Factory;

            //data.MyGameFlags.IsTethered = true;
            //data.MyGameFlags.IsDoppleganger = true;
            //data.MyGameFlags.IsDopplegangerInvert = true;

            data.Initialize(TestLevelInit);

            // Add Landing Zone
            //data.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.LandingZone;

            data.PostMake = TestLevelPostMake;

            //Campaign.CarryPrinces(data);

            // Rumble
            //level.MyGame.AddGameObject(new Rumble());
            //};

            //data.LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;
            data.LavaMake = LevelSeedData.LavaMakeTypes.AlwaysMake;

            GameData.StartLevel(data);
        }

        void TestLevelPostMake(Level level)
        {
            level.StartRecording();

            level.MyGame.AddGameObject(new HintGiver());
            level.MyGame.AddGameObject(HelpMenu.MakeListener());
            level.MyGame.AddGameObject(InGameStartMenu.MakeListener());

            //GameData.UseBobLighting(level, 0);
            //Background.AddDarkLayer(level.MyBackground);

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
            //writelist();
            //Tools.Write("!");

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


            //piece.MyUpgrades1[Upgrade.SpikeyLine] = 5f;
            //piece.MyUpgrades1[Upgrade.Ceiling] = 10;
            //piece.MyUpgrades1[Upgrade.SpikeyGuy] = 5;
            //piece.MyUpgrades1[Upgrade.FireSpinner] = 6;
            //piece.MyUpgrades1[Upgrade.Serpent] = 5;
            //piece.MyUpgrades1[Upgrade.Cloud] = 5;
            //piece.MyUpgrades1[Upgrade.Pinky] = 5;
            //piece.MyUpgrades1[Upgrade.Fireball] = 3;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 6;
            piece.MyUpgrades1[Upgrade.Pendulum] = 7;
            //piece.MyUpgrades1[Upgrade.Elevator] = ;
            //piece.MyUpgrades1[Upgrade.BouncyBlock] = ;
            piece.MyUpgrades1[Upgrade.FallingBlock] = 7;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 7;

            //__Roughly_Abusive(piece);
            //__Roughly_Maso(piece);
            //piece.Style.Masochistic = true;

            //piece.MyUpgrades1[Upgrade.MovingBlock] = 8;
            //piece.MyUpgrades1[Upgrade.LavaDrip] = 9;
            //piece.MyUpgrades1[Upgrade.Serpent] = 9;
            //piece.MyUpgrades1[Upgrade.Pendulum] = 9;
            //piece.MyUpgrades1[Upgrade.Fireball] = 9f;
            //piece.MyUpgrades1[Upgrade.Jump] = 8;
            //piece.MyUpgrades1[Upgrade.Speed] = 9;


            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

            piece.Paths = 1; piece.LockNumOfPaths = true;
            piece.Style.SinglePathType = StyleData._SinglePathType.Normal;

            /*
            piece.Paths = 2;

            SingleData style = piece.Style as SingleData;
            style.InitialDoorYRange = new Vector2(-800);
            style.DoublePathType = StyleData._DoublePathType.Gap;
            */

            //piece.Style.MyModParams = TestLevelModParams;

            piece.Style.ChanceToKeepUnused = 0;

            RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            //piece.MyUpgrades2[Upgrade.Cloud] = 10;
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

            piece.Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
            piece.Style.MyFinalPlatsType = StyleData.FinalPlatsType.Door;
        }

        private static void __Roughly_Maso(PieceSeedData piece)
        {
            piece.MyUpgrades1[Upgrade.Jump] = 1;
            piece.MyUpgrades1[Upgrade.Speed] = 10;

            piece.MyUpgrades1[Upgrade.Serpent] = 7;
            //piece.MyUpgrades1[Upgrade.LavaDrip] = 10;
            piece.MyUpgrades1[Upgrade.Laser] = 5;

            piece.MyUpgrades1[Upgrade.FlyBlob] = 3;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 4;
            ////piece.MyUpgrades1[Upgrade.FallingBlock] = 4;
            ////piece.MyUpgrades1[Upgrade.BouncyBlock] = 4;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 4;
            //piece.MyUpgrades1[Upgrade.Elevator] = 1;
            piece.MyUpgrades1[Upgrade.SpikeyGuy] = 10;
            ////piece.MyUpgrades1[Upgrade.Firesnake] = 6;
            piece.MyUpgrades1[Upgrade.Spike] = 9;
            piece.MyUpgrades1[Upgrade.FireSpinner] = 10;

            piece.MyUpgrades1[Upgrade.Pinky] = 4;
            //piece.MyUpgrades1[Upgrade.SpikeyLine] = 2;
            //piece.MyUpgrades1[Upgrade.Conveyor] = 8;
        }

        private static void __Roughly_Abusive(PieceSeedData piece)
        {
            piece.MyUpgrades1[Upgrade.Jump] = 1;
            piece.MyUpgrades1[Upgrade.Speed] = 3;
            ////piece.MyUpgrades1[Upgrade.Serpent] = 0;
            ////piece.MyUpgrades1[Upgrade.LavaDrip] = 4;
            piece.MyUpgrades1[Upgrade.FlyBlob] = 3;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 4;
            piece.MyUpgrades1[Upgrade.FallingBlock] = 4;
            ////piece.MyUpgrades1[Upgrade.BouncyBlock] = 4;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 4;
            //piece.MyUpgrades1[Upgrade.Elevator] = 1;
            piece.MyUpgrades1[Upgrade.SpikeyGuy] = 2;
            ////piece.MyUpgrades1[Upgrade.Firesnake] = 6;
            piece.MyUpgrades1[Upgrade.Spike] = 2;
            piece.MyUpgrades1[Upgrade.FireSpinner] = 2;
            //piece.MyUpgrades1[Upgrade.Laser] = 2;
            piece.MyUpgrades1[Upgrade.Pinky] = 1;
            //piece.MyUpgrades1[Upgrade.SpikeyLine] = 2;
            //piece.MyUpgrades1[Upgrade.Conveyor] = 8;
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
            var GParams = (FlyingBlob_Parameters)p.Style.FindParams(FlyingBlob_AutoGen.Instance);
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
    }
}
#endif