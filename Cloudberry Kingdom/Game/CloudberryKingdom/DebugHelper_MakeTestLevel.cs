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
            PlayerManager.Players[0].Exists = true;
            PlayerManager.Players[1].Exists = false;
            PlayerManager.Players[2].Exists = true;
            PlayerManager.Players[3].Exists = false;

            LevelSeedData data = new LevelSeedData();


            //data.ReadString("0;s:2130347488;geo:1;h:5,0,0,0;t:castle;n:1;l:6000;u:0,0,0,0,8.03125,0,4.81875,8.03125,0,0,0,0,0,0,10,0,0,0,0,0,0,7.425,7.60625,0;m:1;");
            //GameData.StartLevel(data);
            //return;


            data.Seed = Tools.GlobalRnd.Rnd.Next();
            //data.Seed = 110040853;
            Tools.Write("Seed chosen = {0}", data.Seed);


            //TileSetToTest = "sea";
            //TileSetToTest = "hills";
            //TileSetToTest = "forest";
            //TileSetToTest = "cloud";
            //TileSetToTest = "cave";
            TileSetToTest = "castle";
            //TileSetToTest = "anders__terrace";
			//TileSetToTest = "anders__dungeon";
			//TileSetToTest = "anders__palace";

            if (TileSetToTest == null)
                data.SetTileSet("castle");
            else
                data.SetTileSet(TileSetToTest);


            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Bouncy, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Box, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Small, Hero_MoveMod.Double);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Small, Hero_MoveMod.Double);

            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Small, Hero_MoveMod.Double, Hero_Special.Classic, true);
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Classic, Hero_Special.Classic, true);

            //data.DefaultHeroType2 = null;
            data.DefaultHeroType2 = BobPhsxUpsideDown.Instance;
            //data.DefaultHeroType2 = BobPhsxBlobby.Instance;

            data.DefaultHeroType = BobPhsxNormal.Instance;
            //data.DefaultHeroType = BobPhsxBlobby.Instance;
            //data.DefaultHeroType = BobPhsxMeat.Instance;
			//data.DefaultHeroType = BobPhsxFourWay.Instance;
            //data.DefaultHeroType = BobPhsxBouncy.Instance;
            //data.DefaultHeroType = BobPhsxWheel.Instance;
            //data.DefaultHeroType = BobPhsxTime.Instance;
            //data.DefaultHeroType = BobPhsxInvert.Instance;
            //data.DefaultHeroType = BobPhsxDouble.Instance;
            //data.DefaultHeroType = BobPhsxSpaceship.Instance;
            //data.DefaultHeroType = BobPhsxTimeship.Instance;
            //data.DefaultHeroType = BobPhsxRocketbox.Instance;
            //data.DefaultHeroType = BobPhsxSmall.Instance;
            //data.DefaultHeroType = BobPhsxBig.Instance;
            //data.DefaultHeroType = BobPhsxScale.Instance;
            //data.DefaultHeroType = BobPhsxJetman.Instance;
            //data.DefaultHeroType = BobPhsxBox.Instance;


			data.MyGeometry = LevelGeometry.Right;
			data.PieceLength = 6000;
            data.NumPieces = 1;

            data.MyGameType = NormalGameData.Factory;

            //data.MyGameFlags.IsTethered = true;
            //data.MyGameFlags.IsDoppleganger = true;
            //data.MyGameFlags.IsDopplegangerInvert = true;

            data.Initialize(TestLevelInit);

            data.PostMake = TestLevelPostMake;

            data.LavaMake = LevelSeedData.LavaMakeTypes.NeverMake;

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
            RndDifficulty.ZeroUpgrades(piece.MyUpgrades1);

            piece.MyUpgrades1[Upgrade.FallingBlock] = 5;

			//__Roughly_Abusive(piece);
			//__Roughly_Maso(piece);
            //piece.Style.Masochistic = true;

            piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

			piece.Paths = 1; piece.LockNumOfPaths = true;

            piece.Style.MyModParams = TestLevelModParams;

            RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
            piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
            piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);

            piece.Style.MyInitialPlatsType = StyleData.InitialPlatsType.Door;
            piece.Style.MyFinalPlatsType = StyleData.FinalPlatsType.Door;
        }

        private static void __Roughly_Maso(PieceSeedData piece)
        {
            piece.MyUpgrades1[Upgrade.Jump] = 1;
            piece.MyUpgrades1[Upgrade.Speed] = 10;
            piece.MyUpgrades1[Upgrade.Serpent] = 7;
            piece.MyUpgrades1[Upgrade.Laser] = 5;
            piece.MyUpgrades1[Upgrade.FlyBlob] = 3;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 4;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 4;
            piece.MyUpgrades1[Upgrade.SpikeyGuy] = 10;
            piece.MyUpgrades1[Upgrade.Spike] = 9;
            piece.MyUpgrades1[Upgrade.FireSpinner] = 10;
            piece.MyUpgrades1[Upgrade.Pinky] = 4;
        }

        private static void __Roughly_Abusive(PieceSeedData piece)
        {
            piece.MyUpgrades1[Upgrade.Jump] = 1;
            piece.MyUpgrades1[Upgrade.Speed] = 3;
            piece.MyUpgrades1[Upgrade.FlyBlob] = 3;
            piece.MyUpgrades1[Upgrade.GhostBlock] = 4;
            piece.MyUpgrades1[Upgrade.FallingBlock] = 4;
            piece.MyUpgrades1[Upgrade.MovingBlock] = 4;
            piece.MyUpgrades1[Upgrade.SpikeyGuy] = 2;
            piece.MyUpgrades1[Upgrade.Spike] = 2;
            piece.MyUpgrades1[Upgrade.FireSpinner] = 2;
            piece.MyUpgrades1[Upgrade.Pinky] = 1;
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


			//p.Style.BlockFillType = StyleData._BlockFillType.Sideways;
			//level.CurMakeData.BlocksAsIs = true;
        }
    }
}
#endif