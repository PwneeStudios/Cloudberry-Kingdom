using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using System.Linq;
using System.Collections.Generic;
using CloudberryKingdom.Goombas;
using Drawing;

namespace CloudberryKingdom
{
    public class Campaign_FlyIn : WorldMap
    {
        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                foreach (IObject obj in MyLevel.Objects)
                    if (obj is PrincessBubble || obj is Goombas.Goomba)
                        obj.CollectSelf();

                MyLevel.SetToReset = true;
                WaitThenDo(3, IntroCinematic);
            }
        }

        public static bool WatchedIntro;

        public Campaign_FlyIn()
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

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Intro cutscene
            if (!WatchedIntro)
            {
                WatchedIntro = true;

                // Fade from black
                Black();
                WaitThenDo(16, () =>
                    FadeIn(.02f));
                WaitThenDo(45, () =>
                    IntroCinematic());
            }
        }

        static bool IntroShown = false;

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
            BringBobDelay = 460;// 335;
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
            int WaitLength = BringBobDelay;
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
            List<Goomba> blobs = Campaign.MakeTargetBlobs(n, this);
            Campaign.RectangleTarget(blobs, bob.Pos, BobSize);
            Campaign.MoveBlobsToTarget(blobs);

            // Track bob, move bob in
            Vector2 vel = BobVel;
            CinematicToDo(() =>
            {
                vel += BobAccel;

                // Move the bob and blobs
                bob.Move(vel);
                bob.Core.Data.Velocity = Vector2.Zero;

                Campaign.RectangleTarget(blobs, bob.Pos, BobSize);
                foreach (Goomba blob in blobs) blob.Move(vel);

                // Once the bob has reached a certain height, release
                if (bob.Pos.Y > BobDropHeight + Cam.Pos.Y)
                {
                    bob.SetHeroPhsx(BobPhsxNormal.Instance);
                    bob.Core.Data.Velocity = VelMod * vel;

                    CinematicToDo(() =>
                    {
                        vel += BobAccel;
                        foreach (Goomba blob in blobs) blob.Move(vel);
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
            // Get castle
            Block castleblock = null;
            Block grass = null;
            ObjectGroup castle = new ObjectGroup();
            foreach (IObject obj in MyLevel.Objects)
            {
                // Don't grab camzones
                if (obj is ZoneTrigger) continue;

                if (obj.Core.Show && !(obj is Bob))
                    castle.Add(obj);
            }
            foreach (Block block in MyLevel.Blocks)
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
            castle.Shift(new Vector2(-4500, 0));

            // Make blobs
            int n = Campaign.RectangleTarget(NumBlobs);
            List<Goomba> blobs = Campaign.MakeTargetBlobs(n, this);
            Campaign.RectangleTarget(blobs, castleblock);
            //Campaign.RectangleTarget(blobs,
            //    castleblock.Box.Current.Center + new Vector2(0, 200),
            //    2 * castleblock.Box.Current.Size + new Vector2(0, -200));
            Campaign.MoveBlobsToTarget(blobs);

            // Track castle, move castle in
            Vector2 vel = new Vector2(CastleVel, 0);
            CinematicToDo(() =>
            {
                if (castleblock.Pos.X > EndPos.X + BrakeRegion && vel.X > 0)
                    vel.X -= CastleDecell;

                // Once the castle has come to a stop..
                if (vel.X < 1)
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

                castle.Shift(vel);
                //Campaign.RectangleTarget(blobs, castleblock);
                Campaign.RectangleTarget(blobs,
                    castleblock.Box.Current.Center + new Vector2(0, 100),
                    2 * castleblock.Box.Current.Size + new Vector2(0, -50));
                foreach (Goomba blob in blobs) blob.Move(vel);
                return false;
            });
        }
    }
}