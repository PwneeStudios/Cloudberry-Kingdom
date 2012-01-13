using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
namespace CloudberryKingdom
{
#if WINDOWS
    [Serializable]
#endif
    

    public class DifficultySelection : GameData
    {
        QuadClass quad = new QuadClass("White", 2100);

        TextList QuickGames, NumPlayers;

        public static GameData Factory(LevelSeedData data, bool MakeInBackground)
        {
            return new DifficultySelection();
        }

        public DifficultySelection()
        {
            Init();
        }

        Seed seed;
        public Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();
            level.MainCamera.Zoom *= .7f;

            level.CurPiece = level.StartNewPiece(0, null, 4);
            level.CurPiece.StartData[0].Position = new Vector2(-26000, 600);
            level.MainCamera.Data.Position = level.MainCamera.Target = Vector2.Zero;
            level.CurPiece.CamStartPos = new Vector2(level.CurPiece.StartData[0].Position.X, 0);
            level.MainCamera.BLCamBound = new Vector2(-100000, 0);
            level.MainCamera.TRCamBound = new Vector2(100000, 0);
            level.MainCamera.Update();

            level.MyGame = this;
            //level.Load("Doom\\SimpleCastle.lvl");
            //level.MainCamera.Pos = level.FindBlock("Center").Pos;



            //level.Load("CampaignCastles\\NormalCastle.lvl");
            //level.Load("CampaignCastles\\grass.lvl");
            //level.Load("MainCastle.lvl");
            //level.Load("MainWorld.lvl");
            level.Load("BlobGuy_Level.lvl");

            //level.UseLighting = true;
            
            /* Camera zone
            CameraZone CamZone = (CameraZone)Recycle.GetObject(ObjectType.CameraZone, false);
            CamZone.Init(Vector2.Zero,
                         new Vector2(10000, 10000));
            CamZone.Start = new Vector2(-100000, 0);
            CamZone.End = new Vector2(1000000, 0);
            CamZone.FreeY = true;
            level.AddObject(CamZone);
            */

            /*
            Floater_Spin floater = (Floater_Spin)Recycle.GetObject(ObjectType.Floater_Spin, false);

            floater.Period = 600;
            floater.Length = 400;
            floater.Scale(5);
            level.AddObject(floater);
            */

            /*
            Checkpoint checkpoint = (Checkpoint)Recycle.GetObject(ObjectType.Checkpoint, false);
            checkpoint.Core.Data.Position = new Vector2(-6000, -300);
            level.AddObject(checkpoint);
            */

            
            /*
            seed = (Seed)Recycle.GetObject(ObjectType.Seed, false);
            seed.Core.Data.Position = new Vector2(-7000, 100);
            seed.Intensity = 1f;
            level.AddObject(seed);

            seed.SetState(Seed.State.Off);
            */



/*            Spike spike = (Spike)Recycle.GetObject(ObjectType.Spike, false);
            spike.SetPeriod(50);
            Tools.MoveTo(spike, new Vector2(-7000, 0));
            level.AddObject(spike);
            */

            /* Fireball emitter
            FireballEmitter emitter = (FireballEmitter)Recycle.GetObject(ObjectType.FireballEmitter, false);

            emitter.Core.Data.Position = new Vector2(-6000, -300);
            emitter.Delay = 70;
            emitter.Offset = Tools.Rnd.Next(emitter.Delay);
            emitter.FireOnScreen = true;
            emitter.DrawEmitter = true;
            double Angle = 0;
            int Speed = 15;
                        Angle += Math.PI / 2;
                        emitter.EmitData.Velocity = Tools.AngleToDir(Angle) * Speed;
            level.AddObject(emitter);
            */
             
            /*
            for (int i = 0; i < 60; i++)
            {
                FallingBlock fblock = (FallingBlock)Recycle.GetObject(ObjectType.FallingBlock, false);
                fblock.Init(new Vector2(-7000 - 120 * i, -350), new Vector2(60, 60), 1500);
                fblock.Thwomp = true;
                fblock.AngryMaxSpeed = 20;
                fblock.AngryAccel = new Vector2(0, 3);

                level.AddBlock(fblock);
            }*/


            /* Big Bouncy
            BouncyBlock bouncy;
            bouncy = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            bouncy.Init(new Vector2(-6800, -350), new Vector2(220, 220), 70);
            bouncy.Core.DrawLayer = 9;
            level.AddBlock(bouncy);
            */


            /*
            BouncyBlock fblock;
            for (int i = -30; i < 25; i++)
            {
                fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
                fblock.Init(new Vector2(-6800 + 180 * i, -350), new Vector2(90, 90), 23);//42);
                fblock.Core.DrawLayer = 9;
                level.AddBlock(fblock);

                if (i > -25)
                {
                    fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
                    //fblock.Init(new Vector2(-6800 + 180 * i, 150), new Vector2(90, 90), 23);//42);
                    fblock.Init(new Vector2(-6800 + 180 * i, 150), new Vector2(50, 50), 35);
                    fblock.Core.DrawLayer = 9;
                    level.AddBlock(fblock);
                }
            }
             */

            /*
            MovingBlock mblock = (MovingBlock)Recycle.GetObject(ObjectType.MovingBlock, false);
            mblock.Init(new Vector2(500, -400), new Vector2(100, 100));

            mblock.Period = 80;
            mblock.Displacement = new Vector2(700, 0);
            level.AddBlock(mblock);
            */

            /*
            ConveyorBlock cblock = (ConveyorBlock)Recycle.GetObject(ObjectType.ConveyorBlock, false);
            cblock.Init(new Vector2(-300, -600), new Vector2(1800, 100));
            //cblock.Speed = -.08f;
            cblock.Speed = -.01f;
            level.AddBlock(cblock);
            */
            
            /*
            LavaBlock lblock = (LavaBlock)Recycle.GetObject(ObjectType.LavaBlock, false);
            lblock.Init(-590, -7000f, 7000f, 5000f);
            level.AddBlock(lblock);
             */



            /*
            for (int i = 0; i < 25; i++)
            {
                fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
                fblock.Init(new Vector2(-6800 + 180 * i, -350), new Vector2(90, 90), .15f, .4f);
                fblock.SnapForce = 15;
                fblock.DownForce = 35;
                fblock.Core.DrawLayer = 9;
                level.AddBlock(fblock);
            }
             * */
            /*
            fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            fblock.Init(new Vector2(-6800, -350), new Vector2(90, 90), .16f, .40f);
            fblock.SnapForce = -1;
            level.AddBlock(fblock);

            fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            fblock.Init(new Vector2(-6600, -350), new Vector2(90, 90), .16f, .37f);
            fblock.SnapForce = 15;
            fblock.DownForce = 40;
            level.AddBlock(fblock);

            fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            fblock.Init(new Vector2(-6400, -350), new Vector2(90, 90), .26f, .27f);
            fblock.SnapForce = 15;
            fblock.DownForce = 40;
            level.AddBlock(fblock);

            fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            fblock.Init(new Vector2(-6200, -350), new Vector2(90, 90), .12f, .27f);
            fblock.SnapForce = 15;
            fblock.DownForce = 40;
            level.AddBlock(fblock);

            fblock = (BouncyBlock)Recycle.GetObject(ObjectType.BouncyBlock, false);
            fblock.Init(new Vector2(-6000, -350), new Vector2(90, 90), .16f, .27f);
            fblock.SnapForce = 15;
            fblock.DownForce = 40;
            level.AddBlock(fblock);
            */

            level.MyBackground = Background.Get(BackgroundType.Outside);
            //level.MyBackground = Background.Get(BackgroundType.Dungeon);
            level.MyBackground.Init(level);
            level.BL.X -= 12000;
            level.TR.X += 12000;
            level.MyBackground.AddSpan(level.BL, level.TR);

            return level;
        }

        LevelSeedData data;
        public override void Init()
        {
            base.Init();

            Tools.CurLevel = MyLevel = MakeLevel();
            Tools.CurGameData = MyLevel.MyGame = this;
            Tools.CurGameType = DifficultySelection.Factory;
            Tools.WorldMap = this;


            HelpBubble bubble = new HelpBubble(new Vector2(242.0636f, 1224.009f));
            AddGameObject(bubble);

            int count = 1;
            EzTexture levelicon = Tools.TextureWad.FindByName("levelicon");
            foreach (Block block in MyLevel.Blocks)
            {
                Doodad doodad = block as Doodad;
                if (null != doodad && doodad.MyQuad.Quad.MyTexture == levelicon)
                {
                    DoorIcon icon = new DoorIcon(count);
                    icon.Core.DrawLayer = doodad.Core.DrawLayer;
                    icon.Pos.RelVal = doodad.Pos;
                    AddGameObject(icon);

                    count += 6;

                    doodad.Core.Show = false;
                    //doodad.CollectSelf();
                }
            }




            PhsxData StartData = new PhsxData();
            StartData.Position = MyLevel.CurPiece.CamStartPos;
            for (int i = 0; i < 4; i++)
                MyLevel.CurPiece.StartData[i].Position = StartData.Position + new Vector2(i * 100, 0);

            //DefaultHeroType = MyLevel.DefaultHeroType = BobPhsxWheel.Instance;
            DefaultHeroType = MyLevel.DefaultHeroType = BobPhsxNormal.Instance;
           
            // Bob Player1 = new Bob(Prototypes.bob[(int)MyLevel.DefaultHeroType], false);

            MyLevel.Bobs.Clear();
           // MyLevel.AddBob(Player1);

           // Player1.Init(false, StartData, this);
           // Player1.Immortal = true; 

           // Player1.ScreenWrap = true;

            MyLevel.PlayMode = 0;
            //MyLevel.ResetAll(false);

            //PlayerManager.Get(0).IsAlive = PlayerManager.Get(0).Exists = true;
            //PlayerManager.Get(1).IsAlive = PlayerManager.Get(1).Exists = true;
            //PlayerManager.Get(2).IsAlive = PlayerManager.Get(2).Exists = true;
            //PlayerManager.Get(3).IsAlive = PlayerManager.Get(3).Exists = true;

            MakeBobs(MyLevel);

            for (int i = 0; i < 4; i++)
                if (PlayerManager.Get(i).Exists)
                {
                    MyLevel.Bobs[i].Move(new Vector2(i * 100, 0) + StartData.Position - MyLevel.Bobs[i].Core.Data.Position);
                }



            
                data = new LevelSeedData();
                data.Seed = Tools.Rnd.Next();
                data.SetBackground(BackgroundType.Outside);
                data.DefaultHeroType = BobPhsxNormal.Instance;
                data.MyGameFlags.IsTethered = false;

                data.Initialize(NormalGameData.Factory, LevelGeometry.Right, (int)2, (int)6000, delegate(PieceSeedData piece)
                {
                    piece.Paths = RndDifficulty.ChoosePaths(piece);

                    RndDifficulty.ZeroUpgrades(piece.MyUpgrades1);
                    piece.MyUpgrades1[Upgrade.Jump] = 1;
                    piece.MyUpgrades1[Upgrade.Speed] = 7;
                    piece.MyUpgrades1[Upgrade.FireSpinner] = 7;
                    piece.MyUpgrades1[Upgrade.General] = 1;
                    piece.MyUpgrades1.CalcGenData(piece.MyGenData.gen1, piece.Style);

                    RndDifficulty.ZeroUpgrades(piece.MyUpgrades2);
                    piece.MyUpgrades1.UpgradeLevels.CopyTo(piece.MyUpgrades2.UpgradeLevels, 0);
                    //piece.MyUpgrades2[Upgrade.Cloud] = 10;
                    piece.MyUpgrades2.CalcGenData(piece.MyGenData.gen2, piece.Style);
                });

                // Add Landing Zone
                data.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.LandingZone;

            //GameData.StartLevel(data, true);
            //seed.LandingSeedData = data;
        }

        public override void ReturnTo(int code)
        {
            base.ReturnTo(code);

            Tools.CurGameType = DifficultySelection.Factory;
            Tools.CurGameData = this;
            Tools.CurLevel = MyLevel;

            //Recycle.Empty();
        }

        string teststring = "boo";
        IAsyncResult result;
        public override void PhsxStep()
        {
            base.PhsxStep();
            /*
            lock (data.Loaded)
            {
                if (data.Loaded.val)
                {
                    Tools.CurGameData = data.MyGame;
                    Tools.CurLevel = data.MyGame.MyLevel;
                }
            }*/

            /*
            if (MyLevel.CurPhsxStep < 300)
            ;//    ParticleEffects.PieceOrb(MyLevel, new Vector2(-7500,0), MyLevel.CurPhsxStep, 1);
            else
                ParticleEffects.PieceExplosion(MyLevel, new Vector2(-7500, 0), MyLevel.CurPhsxStep, 1);
            */

            /* Test seed On/Off
            for (int i = 0; i < 50; i++)
            {
                if (Tools.CurLevel.CurPhsxStep == 40 + i * 400)
                    seed.SetState(Seed.State.TurningOn);

                if (Tools.CurLevel.CurPhsxStep == 240 + i * 400)
                    seed.SetState(Seed.State.TurningOff);
            }*/

            /* Test seed launch 
            if (Tools.CurLevel.CurPhsxStep == 40)
                seed.SetState(Seed.State.TurningOn);
            if (Tools.CurLevel.CurPhsxStep == 350)
                seed.SetState(Seed.State.Launch);
            */

            // TEST THE SCORE SCREEN
            /*
            if (Tools.CurLevel.CurPhsxStep == 10)
            {
                NewScoreObject score = new NewScoreObject();
                AddGameObject(score);
            }*/


            /* Text box
#if PC_VERSION
            if (Tools.CurLevel.CurPhsxStep == 10)
            {
                GUI_TextBox textbox = new GUI_TextBox(PlayerManager.RandomNames.Choose(), Vector2.Zero);
                AddGameObject(textbox);
            }
#endif
             */

            /*
            // Test closing circle
            if (Tools.CurLevel.CurPhsxStep == 30)
                Tools.CurLevel.MakeClosingCircle(200);
            */

            /*
            if (Tools.CurLevel.CurPhsxStep == 40)
            {
                //Tools.CurGameData.AddGameObject(new GameOverPanel(SaveGroup.HeroRushHighScore, SaveGroup.HeroRushHighLevel, ));
                Tools.CurGameData.AddGameObject(new HighScorePanel(SaveGroup.HeroRushHighScore, SaveGroup.HeroRushHighLevel));
            }*/

            // Test save game
            /*
            if (Tools.CurLevel.CurPhsxStep > 30000)
            {
                if (Tools.saveDevice.IsReady)
                {
                    Tools.saveDevice.SaveAsync(
                        "HighScores",
                        "Test.txt",
                        stream =>
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                                writer.WriteLine("Hello!!!!!!!");
                        });
                }
            }*/

            /*
            if (Tools.CurLevel.CurPhsxStep == 30)
            {
                ScoreList list = new ScoreList(10);
                list.Add(new ScoreEntry(102030, "Ez Zeal"));
                list.Add(new ScoreEntry(1435143, "TheEzEzz"));
                list.Add(new ScoreEntry(12030, "Ez Zeal"));
                list.Add(new ScoreEntry(1020300000, "Ez Zeal FOREVER"));

                //Tools.Write("11111");
                //EzStorage.Load("HighScores", "TestTest.txt", reader => list = ScoreList.Deserializse(reader),
                //    () =>
                //    {
                //        list = new ScoreList();
                //        Tools.Write("boooob!!");
                //    });
                //Tools.Write("222222");

                EzStorage.Save("HighScores", "NewText.txt", writer => list.Serialize(writer));
            }
            */
            
            //if (Tools.CurLevel.CurPhsxStep > 120 &&
            //    ButtonCheck.State(ControllerButtons.A, -1).Pressed)
            /*
            if (Tools.CurLevel.CurPhsxStep == 20 || ButtonCheck.State( Microsoft.Xna.Framework.Input.Keys.Q).Pressed)
            {
                SaveGroup.SaveAll();
                Tools.CurGameData.RemoveAllGameObjects(obj => obj is GameOverPanel || obj is HighScorePanel);

                //SaveGroup.HeroRushHighScore.Add(new ScoreEntry(602350, "TACO MONSTER"));
                //SaveGroup.SaveAll();

                Tools.CurGameData.AddGameObject(new GameOverPanel(SaveGroup.HeroRushHighScore, null));
                //Tools.CurGameData.AddGameObject(new HighScorePanel(SaveGroup.HeroRushHighScore));

                //ScoreList list = null;

                //EzStorage.Load("HighScores", "NewText.txt", reader =>
                //    {
                //        list = ScoreList.Deserializse(reader);
                //        Tools.Write("");
                //    }, () => list = new ScoreList());
            }
            */

            // Test hints
            /*
            if (Tools.CurLevel.CurPhsxStep == 20 || ButtonCheck.State(Microsoft.Xna.Framework.Input.Keys.Q).Pressed)
            {
                Tools.CurGameData.RemoveAllGameObjects(obj => obj is HintBlurb2);
                HintBlurb2 hint = new HintBlurb2();
                //teststring += "!";
                //hint.SetText(teststring);
                hint.SetText("Check out the insane replay!");
                Tools.CurGameData.AddGameObject(hint);
            }*/

            // TEST Blob Guy
            if (Tools.CurLevel.CurPhsxStep == 10)
            {
                MyLevel.MainCamera.SetPhsxType(Camera.PhsxType.Fixed);
                MyLevel.MainCamera.Target = MyLevel.MainCamera.Data.Position = MyLevel.FindBlock("Camera").Core.Data.Position;

                MyLevel.CurPiece.StartData[0].Position = MyLevel.MainCamera.Target + new Vector2(-1000, 0);
                MyLevel.CurPiece.CamStartPos = MyLevel.MainCamera.Target;

                MyLevel.HaveTimeLimit = false;
                MyLevel.AllowRecording = false;
                MyLevel.StopRecording();

                Vector2 BossCenter = MyLevel.FindBlock("Boss_Center").Core.Data.Position;

                //BlobGuy boss = new BlobGuy();
                //MyLevel.AddObject(boss);
                //boss.Init(BossCenter, BlobGuy.Mode.Boss);
            }


            // TEST THE CHAR SELECT
            if (Tools.CurLevel.CurPhsxStep == 5)
            {
                MyLevel.MyGame.AllowQuickJoin = true;
             //   CharacterSelectManager.Start(0, false);
             //   CharacterSelectManager.Start(1, false);
             //   CharacterSelectManager.Start(2, false);
             //   CharacterSelectManager.Start(3, false);
            }
        }

        public override void Draw()
        {
            base.Draw();

            return;

            quad.Pos = MyLevel.MainCamera.Pos + new Vector2(0, -3070);
            quad.EffectName = "Lava";
            quad.TextureName = "NewLava";
            quad.Base.e1 *= -1;
            /*
            Tools.EffectWad.FindByName("Lava").effect.Parameters["EdgeColor"].SetValue(new Color(0, 0, 155).ToVector4());
            Tools.EffectWad.FindByName("Lava").effect.Parameters["LavaColor"].SetValue(new Color(0, 0, 255).ToVector4());
            quad.Draw();
            Tools.QDrawer.Flush();
            */
            Tools.EffectWad.FindByName("Lava").effect.Parameters["EdgeColor"].SetValue(new Color(169, 18, 18).ToVector4());
            Tools.EffectWad.FindByName("Lava").effect.Parameters["LavaColor"].SetValue(new Color(255, 0, 0).ToVector4());
            quad.Pos = MyLevel.MainCamera.Pos + new Vector2(0, -3150);
            quad.Base.e1 *= -1;
            quad.Draw();
            Tools.QDrawer.Flush();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        public override void BobDie(Level level, Bob bob)
        {
            base.BobDie(level, bob);
        }

        public override void BobDoneDying(Level level, Bob bob)
        {
            base.BobDoneDying(level, bob);
        }
    }
}