using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using System.Linq;
using System.Collections.Generic;
using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public class Campaign_IntroWorld : WorldMap
    {
        public static bool WatchedIntro;

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                foreach (ObjectBase obj in MyLevel.Objects)
                    if (obj is PrincessBubble || obj is Goombas.Goomba)
                        obj.CollectSelf();

                MyLevel.SetToReset = true;
                WaitThenDo(3, IntroCinematic);
            }
        }

        public Campaign_IntroWorld()
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "World1";

            //Init("Doom\\SimpleCastle.lvl");
            Init("Doom\\SimpleCastle_Simple.lvl");

            MakeCenteredCamZone(.72f);
            MakeBackground(BackgroundType.Outside);
            
            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakePlayers();
            if (WatchedIntro)
                EnterFrom(Doors[4]);

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Intro cutscene
            if (!WatchedIntro)
            {
                WatchedIntro = true;

                // Hide players
                PhsxStepsToDo += 2;
                WaitThenDo(1, () => HideBobs());

                // Fade from black
                Black();
                WaitThenDo(16, () =>
                    FadeIn(.02f));

                // Do intro
                //WaitThenDo(45, () =>
                //    IntroCinematic());
                IntroCinematic();
            }
        }


        int InitialDelay;
        bool ShowWorldTitle, TitleBubble;
        float TitleFadeSpeed;
        int TitleLength;
        Vector2 TitlePos;
        int WaitToDispatch, NumBlobs, WaitToTitle;
        float CastleVel, CastleDecell, BrakeRegion, BlobExpand;
        int NumBlobs_Bob, BringBobDelay, BringBobDelayAdd;
        Vector2 BobVel, BobPos, BobAccel, BobSize;
        float BobDropHeight, VelMod;
        void SetIntroParams()
        {
            InitialDelay = 50;

            // Title
            TitleBubble = false;
            ShowWorldTitle = true;
            TitleFadeSpeed = .0125f;
            TitleLength = 173;
            WaitToTitle = 160;// 222; //67;
            TitlePos = new Vector2(150, 0) + new Vector2(-246.9133f, -763.8888f);

            // Bring in castle
            NumBlobs = 300;
            CastleVel = 35.5f;
            BrakeRegion = -1350;// -2250;// -960;
            CastleDecell = .493f;
            WaitToDispatch = 8;// 18;// 27;
            BlobExpand = 4.51f;

            // Bring in bob
            NumBlobs_Bob = 9;// 16;
            BobSize = new Vector2(340);
            VelMod = 1.3f;
            BringBobDelay = 420;// 460;
            BringBobDelayAdd = 25;

            BobVel = new Vector2(34.5f, -9.75f);
            BobAccel = new Vector2(0, 1.1f);
            BobDropHeight = -430;
            BobPos = new Vector2(-2700 - 100, -600);

            // VIDEO
            //ShowWorldTitle = false;
            //BringBobDelay = 360;
        }

        Vector2 EndPos = Vector2.Zero;
        public void IntroCinematic()
        {
            // Hide bob
            MyLevel.PreventReset = true;
            foreach (Bob bob in MyLevel.Bobs)
            {
                bob.Immortal = true;
                bob.Core.Show = false;
            }
            PhsxStepsToDo += 2;

            // Set parameters and make sure camera is fixed
            SetIntroParams();
            if (Cam.MyZone != null)
                Cam.MyZone.CameraType = Camera.PhsxType.Fixed;
            Cam.MyPhsxType = Camera.PhsxType.Fixed;

            // Bring in the castle
            BringCastle();

            // Bring in the bobs
            int WaitLength = BringBobDelay + InitialDelay;
            foreach (Bob bob in MyLevel.Bobs)
            {
                WaitThenDo(WaitLength, () => BringBob(bob));
                WaitLength += BringBobDelayAdd;
            }
        }

        void BringBob(Bob bob)
        {
            bob.Core.Show = true;
            bob.SetHeroPhsx(BobPhsxCinematic.Instance);
            bob.DoObjectInteractions = false;
            Tools.MoveTo(bob, Cam.Pos + BobPos);

            // Make blobs
            int n = Campaign.RectangleTarget(NumBlobs_Bob);
            List<Goomba> bob_blobs = Campaign.MakeTargetBlobs(n, this);
            Campaign.RectangleTarget(bob_blobs, bob.Pos, BobSize);
            Campaign.MoveBlobsToTarget(bob_blobs);

            // Track bob, move bob in
            Vector2 vel = BobVel;
            CinematicToDo(() =>
            {
                vel += BobAccel;

                // Move the bob and blobs
                bob.Move(vel);
                bob.Core.Data.Velocity = Vector2.Zero;

                Campaign.RectangleTarget(bob_blobs, bob.Pos, BobSize);
                foreach (Goomba blob in bob_blobs) blob.Move(vel);

                // Once the bob has reached a certain height, release
                if (bob.Pos.Y > BobDropHeight + Cam.Pos.Y)
                {
                    bob.SetHeroPhsx(BobPhsxNormal.Instance);
                    bob.Core.Data.Velocity = VelMod * vel;

                    CinematicToDo(80, () => bob.DoObjectInteractions = true);

                    CinematicToDo(() =>
                    {
                        vel += BobAccel;
                        foreach (Goomba blob in bob_blobs) blob.Move(vel);
                        return false;
                    });

                    // End this callback
                    return true;
                }

                return false;
            });
        }

        void BringCastle()
        {
            BringCastle_Start();
        }

        BlockBase castleblock = null;
        Vector2 vel;
        List<Goomba> blobs;
        ObjectGroup castle;
        void BringCastle_Start()
        {
            // Get castle
            BlockBase grass = null;
            castle = new ObjectGroup();
            foreach (ObjectBase obj in MyLevel.Objects)
            {
                // Don't grab camzones
                if (obj is ZoneTrigger) continue;

                if (obj.Core.Show && !(obj is Bob))
                    castle.Add(obj);
            }
            foreach (BlockBase block in MyLevel.Blocks)
            {
                // Don't grab invisible things, unless it's icon positions
                if (!block.Core.Show && block.Core.EditorCode3.CompareTo("icon") != 0) continue;

                // Don't grab the grass
                if (block.Core.MyTileSetType == TileSet.OutsideGrass)
                    grass = block;
                else if (block.Core.MyTileSetType == TileSet.CastlePiece2 ||
                         block.Core.MyTileSetType == TileSet.CastlePiece)
                {
                    castleblock = block;
                    castle.Add(block);
                }
                else
                    castle.Add(block);
            }
            castleblock.Extend(Side.Bottom, grass.Box.Current.TR.Y - 100);

            // Save original position
            if (EndPos == Vector2.Zero)
                EndPos = castleblock.Pos;

            // Move out
            //castle.Shift(new Vector2(-4500, 0));
            castle.Shift(new Vector2(-4300, 0));

            // Wait for scene to fade in
            CinematicToDo(InitialDelay, (Action)BringCastle_MakeBlobs);
        }

        Vector2 StartPos;
        void BringCastle_MakeBlobs()
        {
            // Make blobs
            int n = Campaign.RectangleTarget(NumBlobs);
            blobs = Campaign.MakeTargetBlobs(n, this);
            Campaign.RectangleTarget(blobs, castleblock);
            //Campaign.RectangleTarget(blobs,
            //    castleblock.Box.Current.Center + new Vector2(0, 200),
            //    2 * castleblock.Box.Current.Size + new Vector2(0, -200));
            Campaign.MoveBlobsToTarget(blobs);

            // Track castle, move castle in
            vel = new Vector2(CastleVel, 0);
            StartPos = castleblock.Pos;
            CinematicToDo(BringCastle_MoveCastle);
        }

        enum State { Up, Down, Slide };
        State state = State.Up;
        bool BringCastle_MoveCastle()
        {
            Vector2 AmountToShift = vel;
            
            if (state == State.Up)
            {
                vel.X += .35f;
                Tools.Restrict(0, 23.5f, ref vel.X);

                vel.Y += .25f;
                if (castleblock.Pos.Y > Cam.Pos.Y + 50)
                    state = State.Down;
            }
            else if (state == State.Down)
            {
                vel.Y -= .4525f;
                if (castleblock.Pos.Y < StartPos.Y)
                {
                    Tools.CurCamera.StartShake(.5f, 36);

                    //castle.Shift(new Vector2(0, StartPos.Y - castleblock.Pos.Y));
                    AmountToShift = new Vector2(0, StartPos.Y - castleblock.Pos.Y);
                    state = State.Slide;
                    vel.Y = 0;
                }
            }
            else if (state == State.Slide)
            {
                vel.Y = 0;

                if (vel.X > 0.1f)
                    vel.X *= .965f;
                else
                {
                    if (castleblock.Pos.X > EndPos.X - 500 && vel.X > 0)
                    {
                        // Wait a moment, then dispatch the blobs
                        CinematicToDo(WaitToDispatch, () =>
                        {
                            Campaign.RectangleTarget(blobs,
                                castleblock.Box.Current.Center + new Vector2(50, 200),
                                BlobExpand * castleblock.Box.Current.Size + new Vector2(0, -200));

                            CinematicToDo(() =>
                            {
                                vel.X += .25f;
                                foreach (Goomba blob in blobs) blob.Move(vel);
                                return false;
                            });
                        });

                        // Add world title
                        if (ShowWorldTitle)
                            CinematicToDo(WaitToTitle, () => MakeWorldTitle("World 1", 0, TitleLength, TitleFadeSpeed, TitlePos, TitleBubble));

                        // End this callback
                        return true;
                    }
                    else
                        state = State.Up;
                }
            }

            //if (castleblock.Pos.X > EndPos.X + BrakeRegion && vel.X > 0)
            //    vel.X -= CastleDecell;

            //// Once the castle has come to a stop..
            //if (vel.X < 1)
            //{
            //    // Wait a moment, then dispatch the blobs
            //    CinematicToDo(WaitToDispatch, () =>
            //    {
            //        Campaign.RectangleTarget(blobs,
            //            castleblock.Box.Current.Center + new Vector2(50, 200),
            //            BlobExpand * castleblock.Box.Current.Size + new Vector2(0, -200));

            //        CinematicToDo(() =>
            //        {
            //            vel.X += .25f;
            //            foreach (Goomba blob in blobs) blob.Move(vel);
            //            return false;
            //        });
            //    });

            //    // Add world title
            //    if (ShowWorldTitle)
            //        CinematicToDo(WaitToTitle, () => MakeWorldTitle("World 1", 0, TitleLength, TitleFadeSpeed, TitlePos, TitleBubble));

            //    // End this callback
            //    return true;
            //}

            //Vector2 AmountToShift = vel;


            castle.Shift(AmountToShift);
            //Campaign.RectangleTarget(blobs, castleblock);
            Campaign.RectangleTarget(blobs,
                castleblock.Box.Current.Center + new Vector2(0, 100),
                2 * castleblock.Box.Current.Size + new Vector2(0, -50));
            foreach (Goomba blob in blobs) blob.Move(AmountToShift);
            return false;
        }

        /*
        public void IntroCinematic()
        {
            //IntroShown = true;

            // Hard fall
            CinematicToDo(10, () =>
            {
                // Pop in all the bobs
                Vector2 pos = Cam.Pos + new Vector2(-880, 500);
                PopIn.PopInAll(MyLevel, pos, _bob =>
                {
                    _bob.Core.Data.Velocity = new Vector2(0, 40);
                    _bob.Core.Data.Acceleration = new Vector2(0, -2.25f);
                });

                // Fall to the ground
                Block ground = MyLevel.FindBlock("ground");
                foreach (Bob bob in MyLevel.Bobs)
                {
                    // Set falling animation
                    bob.SetToCinematic();
                    bob.SetAnimation("Falling", MyLevel.Rnd.RndFloat(0, .3f), true, 3.3f);

                    // Check for hitting the ground
                    bob.CinematicFunc = step =>
                    {
                        bob.Box.CalcBounds(); ground.Box.CalcBounds();
                        float dif = bob.Box.BL.Y - ground.Box.TR.Y;
                        //float dif = bob.Box.BL.Y + bob.Core.Data.Velocity.Y - ground.Box.TR.Y;
                        if (dif < 0)
                        {
                            // Slam into the ground
                            bob.SetAnimation("HitGround", 2.3f);
                            bob.Box.CalcBounds();
                            bob.Move(new Vector2(0, ground.Box.TR.Y - bob.Box.BL.Y));
                            bob.UpdateObject();
                            bob.Core.Data.Velocity = bob.Core.Data.Acceleration = Vector2.Zero;

                            bob.CinematicFunc = _step =>
                            {
                                bob.Box.CalcBounds();
                                bob.Move(new Vector2(0, ground.Box.TR.Y - bob.Box.BL.Y));
                                bob.UpdateObject();
                            };

                            // Get back up
                            CinematicToDo(60, () => {
                                bob.TransferToAnimation("GetUp", 2f);
                                bob.OnAnimFinish = () => AddToDo(() => bob.EndCinematic());
                            });
                        }
                        else
                            bob.Core.Data.Integrate();
                    };
                }

                // Help bubble
                HelpBubble bubble = new HelpBubble(new Vector2(1017.527f, 1365f));
                bubble.TimesToShow = 1;
                AddGameObject(bubble);
            });
        }
        */
/*
        public void IntroCinematic()
        {
            foreach (Door door in Doors.Values)
                door.SetLock(true, true, false);

            Block ground = MyLevel.FindBlock("ground");
            Vector2 CamCenter = MyLevel.FindBlock("Center").Pos;
            Vector2 Land = new Vector2(CamCenter.X, ground.Box.TR.Y);

            foreach (Bob bob in MyLevel.Bobs)
            {
                bob.Core.Show = false;
                bob.Pos = new Vector2(0, 100000);
            }

            Vector2 Add = Vector2.Zero;
            int Wait = 0;
            WaitThenDo(45, () =>
            {
                foreach (Bob bob in MyLevel.Bobs)
                {
                    bob.Core.Show = true;

                    bob.SetToCinematic();
                    bob.AffectsCamera = false;
                    bob.PlayerObject.AnimQueue.Clear();
                    bob.PlayerObject.EnqueueAnimation("Intro_HoldOn", 0, true, true, 1, 3.3f, false);

                    BobPhsxCinematic cin = bob.MyPhsx as BobPhsxCinematic;
                    Vector2 _add = Add;
                    WaitThenDo(Wait, () =>
                        cin.Pos.LerpTo(Land + new Vector2(0, 3000) +_add, Land + _add, 40));
                    Add.X += 100; Wait += 10;
                    cin.UseFancy = true;
                }
            });
        }

        public void BossCinematic()
        {
            foreach (Door door in Doors.Values)
                door.SetLock(true, true, false);

            Block ground = MyLevel.FindBlock("ground");
            Vector2 CamCenter = MyLevel.FindBlock("BossCenter").Pos;
            Vector2 Land = new Vector2(CamCenter.X, ground.Box.TR.Y);

            Cam.SetPhsxType(Camera.PhsxType.Fixed);
            Cam.MakeFancyPos();

            Doors[4].HideBobs();
            WaitThenDo(45, () => {
                Doors[4].SetLock(false, true, true);
                Doors[4].MoveBobs();
                Doors[4].ShowBobs();

                foreach (Bob bob in MyLevel.Bobs)
                {
                    bob.SetToCinematic();
                    bob.AffectsCamera = false;
                    bob.PlayerObject.AnimQueue.Clear();
                    bob.PlayerObject.EnqueueAnimation("Intro_HoldOn", 0, true, true, 1, 3.3f, false);
                    
                    BobPhsxCinematic cin = bob.MyPhsx as BobPhsxCinematic;
                    cin.Pos.LerpTo(bob.Pos, Land, 20); Land.X += 100;
                    cin.UseFancy = true;
                }

                WaitThenDo(3, () =>
                    Cam.FancyPos.LerpTo(CamCenter, 80, LerpStyle.Sigmoid));
            });
        }
        */

        protected override void AssignDoors()
        {
            // DEBUG: make the first door the victory door
            if (DEBUG_FirstDoorVictory)
                MakeDoorAction_Level(Doors[1], CampaignEnd);
            else
            {
                LevelSeedData d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxNormal.Instance, .9f);
                d.SetBackground(TileSet.Terrace);
                d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
                MakeDoorAction_Level(Doors[1], d);//, Doors.Exit[1]);
            }

            MakeDoorAction_Level(Doors[2], Campaign.WallLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 1.15f));//, Doors.Exit[2]);
            AddDependence(2, 1);

            MakeDoorAction_Level(Doors[3], Challenge_PlaceTraining.StartLevel);//, Doors.Exit[3]);
            AddDependence(3, 2);
            Icons[3].SetIcon(IconType.Place);

            MakeDoorAction(Doors[4], () => Load(new Campaign_World2(true)));
            AddDependence(4, 3);
            Icons[4].SetIcon(IconType.Boss);
        }
    }
}