using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public partial class Seed : ObjectBase, IObject, ILevelConnector
    {
        public NormalBlock LandingBlock = null; // Block to land on
        List<Bob> HoldBobs;
        Vector2 HoldShift;

        public bool Launched; // Whether the stickmen have been launched from the seed yet
        public bool Panning; // Whether the camera is panning
        List<Block> Blocks;
        float LandingY; // Y coordinate of the landing block
        int PanDuration = 180;
        int[] PostSlideCount = { 0, 0, 0, 0 };

        public void Start_Launch()
        {
            Launched = Panning = false;
            PreLaunchCount = 0;
        }

        bool Wrapping = false;
        public void StartPan()
        {
            Launched = true;
            Panning = true;
            Wrapping = true;

            Camera cam = Core.MyLevel.MainCamera;

            // Calculate the height of the landing block
            //LandingY = LandingBlock.Box.TR.Y;
            //LandingY = cam.Data.Position.Y - cam.GetHeight() / 2 + 450;
            Camera cam2 = LandingBlock.Core.MyLevel.MainCamera;
            LandingY = LandingBlock.Box.TR.Y - cam2.BL.Y + cam.BL.Y; ;

            // Make random blocks to pan past
            Blocks = MakeRandomBlocks(Core.MyLevel, new Vector2(Core.Data.Position.X + 2500, cam.BL.Y), new Vector2(Core.Data.Position.X + 10000, cam.TR.Y));

            // Prepare the stickmen
            foreach (Bob bob in Core.MyLevel.Bobs)
            {
                // Revive if needed
                if (!PlayerManager.IsAlive(bob.MyPlayerIndex))
                {
                    PlayerManager.ReviveBob(bob);
                    //Generic.RevivePlayer(bob.MyPlayerIndex);                    
                }
                bob.Move(this.Core.Data.Position - bob.Core.Data.Position);

                int i = (int)bob.MyPlayerIndex;

                bob.EndSuckedIn();

                bob.SetToCinematic();
                bob.AffectsCamera = false;

                bob.Core.Data.Velocity = new Vector2(250 + 1.15f * i, 0);
                    //MyLevel.Rnd.RndFloat(-1.95f, 1.5f), 0);

                bob.PlayerObject.xFlip = false;

                // Calculate gravity
                bob.Core.Data.Velocity.Y = 12f + (float)Math.Pow(-1, i) * .4f * i; //MyLevel.Rnd.RndFloat(11, 12.75f);
                float dif = LandingY - bob.Core.Data.Position.Y;
                bob.Core.Data.Acceleration.Y = 2 * (dif - (PanDuration + (float)Math.Pow(-1, i)*2.0f) * bob.Core.Data.Velocity.Y) / (PanDuration * PanDuration);
                //MyLevel.Rnd.RndFloat(0,24)

                // Set animation
                bob.PlayerObject.AnimQueue.Clear();
                bob.PlayerObject.EnqueueAnimation("Launched", MyLevel.Rnd.RndFloat(0, 1), true, true, 1, 3.3f, false);
                bob.PlayerObject.DequeueTransfers();
            }

            cam.SetPhsxType(Camera.PhsxType.Fixed);
            cam.ZoneLocked = false;
        }

        int PreLaunchCount;
        //ScoreObject ScoreObj;
        public void PreLaunchPhsx()
        {
            WhispySound_Phsx();

            //if (ScoreObj == null)
            {
                //ScoreObj = new ScoreObject();
                //Core.MyLevel.AddObject(ScoreObj);
                //ScoreObj.Init();
            }

            // Wait for next level to finish loading
            if (LandingBlock == null && NextLevelSeedData != null)
            {
                lock (NextLevelSeedData.Loaded)
                {
                    if (NextLevelSeedData.Loaded.val)
                    {
                        LandingBlock = (NormalBlock)NextLevelSeedData.MyGame.MyLevel.FindBlock("Landing Platform");
                    }
                    else
                        return;
                }
            }

            // Wait for score screen to finish
            //if (ScoreObj != null && !ScoreObj.Done)
              //  return;

            // If no new level, return to WorldMap
            if (NextLevelSeedData == null && Tools.WorldMap != null)
            {
                Tools.WorldMap.SetToReturnTo(0);
                return;
            }

            PreLaunchCount++;

            // Suck in stickmen to center
            foreach (Bob bob in Core.MyLevel.Bobs)
            {
                if (PreLaunchCount > 40)
                if (bob.SuckedIn && bob.SuckedInSeed == this)
                {
                    bob.Core.Data.Velocity *= .875f;
                    bob.Core.Data.Position += .875f * (Core.Data.Position - bob.Core.Data.Position);
                }

                if (PreLaunchCount == 42)
                {
#if XBOX
                    Tools.SetVibration(bob.MyPlayerIndex, 1f, 1f, 60);
#endif
                }
            }        

            CoalescingRing(PreLaunchCount, 12);

            // Extra puff
            if (PreLaunchCount == 40)
                Tools.SoundWad.FindByName("Maelstrom_ReadyToLaunch").Play();
            /*
            if (PreLaunchCount > 48)
            {
                float ExtraIntensity = 1.0f - (float)Math.Pow(Math.Abs(PreLaunchCount - 60) / 10f, 2); //Math.Max(0.1f, 1f - .03f * (StartStep - 45));
                //ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, 3 * Intensity);
                for (int i = 0; i < 3; i++)
                {
                    ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position + (75 + ExtraIntensity * 80) * Tools.AngleToDir(i * 2 * Math.PI / 3), Core.MyLevel.CurPhsxStep, .7f * ExtraIntensity);
                }
            }*/

            // Launch
            if (PreLaunchCount == 70 || PreLaunchCount == 71)
                Tools.SoundWad.FindByName("Maelstrom_Launch").Play();
            if (PreLaunchCount == 74)
            {
                switch (MyState)
                {
                    case State.Launch:
                        StartPan();
                        break;

                    default:
                        DownLaunch();
                        break;
                }
            }
        }

        void AddNextLevel(LevelSeedData data)
        {
            if (Tools.CurLevel != data.MyGame.MyLevel)
            {
                Core.MyLevel.MyGame.AddToDo(delegate()
                {
                    Tools.CurGameData = data.MyGame;
                    //Level NewLevel = Tools.CurLevel = data.MyGame.MyLevel;
                    Level NewLevel = data.MyGame.MyLevel;
                    Level PrevLevel = Core.MyLevel;

                    
                    // Shift new level so that landing block is in correct position
                    Vector2 pos = Core.MyLevel.Bobs[0].Core.Data.Position;
                    Camera cam = PrevLevel.MainCamera;

                    Vector2 DesiredTR = new Vector2(pos.X + cam.GetWidth() + 3250,//7500,
                                                    LandingY);
                                                    //cam.Data.Position.Y - cam.GetHeight() / 2 + 450);

                    HoldShift = DesiredTR - LandingBlock.Box.TR;
                    NewLevel.Move(HoldShift);

                    LandingBlock.Extend(Side.Left, pos.X + cam.GetWidth() - 230);

                    NewLevel.MyGame.AdditionalReset();

                    // Make sure each player has a bob
                    for (int i = 0; i < 4; i++)
                    {
                        if (PlayerManager.Get(i).Exists && !NewLevel.Bobs.Exists(delegate (Bob bob){ return (int)bob.MyPlayerIndex==i; }))
                            Tools.CurGameData.CreateBob(i, false);
                    }

                    // Transfer bobs
                    HoldBobs = new List<Bob>(NewLevel.Bobs);
                    /*
                    foreach (Bob bob in NewLevel.Bobs)
                    {
                        Bob match = Core.MyLevel.Bobs.Find(delegate(Bob MatchingBob) { return MatchingBob.MyPlayerIndex == bob.MyPlayerIndex; });
                        match.MyPiece = bob.MyPiece;
                        match.MyPieceIndex = bob.MyPieceIndex;

                        bob.Release();
                    }
                     * */
                    NewLevel.Bobs.Clear();

                    foreach (Bob bob in PrevLevel.Bobs)
                    {
                        NewLevel.AddBob(bob);
                    }
                    PrevLevel.Bobs = null;

                    // Abosrb the current level into the new level
                    NewLevel.MainCamera.Clone(cam, true);
                    //NewLevel.AbsorbLevel(Core.MyLevel);
                    //NewLevel.AbsorbLevelVisuals(Core.MyLevel);

                    // Extend the background                    
                    FloatRectangle Area = new FloatRectangle();
                    Area.TR = new Vector2(NewLevel.MyBackground.BL.X - 10000, NewLevel.MyBackground.TR.Y);
                    Area.BL = new Vector2(PrevLevel.MyBackground.BL.X - 10000, PrevLevel.MyBackground.BL.Y);
                    PrevLevel.MyBackground.Clear(Area);
                    PrevLevel.MyBackground.ExtendRight(NewLevel.MyBackground.TR.X);
                    //NewLevel.MyBackground.Clear();
                    //NewLevel.MyBackground.Absorb(PrevLevel.MyBackground);
                    //NewLevel.MyBackground.Conform(PrevLevel.MyBackground);
                    NewLevel.SetBackground(PrevLevel.MyBackground);

                    PrevLevel.MyBackground = null;

                    // Add the seed to the new level
                    NewLevel.AddObject(this);

                    // Add the wrapping blocks to the new level
                    foreach (Block block in Blocks)
                        NewLevel.AddBlock(block);

                    // Remove seed and wrapping blocks from previous level
                    PrevLevel.RemoveForeignObjects();
                    //PrevLevel.Release();
                    ((StringWorldGameData)Tools.WorldMap).SetLevel(NextLevelSeedData, false);
                    //((StringWorldGameData)Tools.WorldMap).cur

                    // Start music
//                    Tools.StartPlaylist();

                    Console.WriteLine("TRANSFER COMPLETE!");
                    

                    return false;
                });
            }
        }

        int LaunchStep = 0;
        public void Launch_PhsxStep()
        {
            if (!SkipPhsx)
            {
                ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position);
                ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);
            }

            if (!Launched)
            {
                PreLaunchPhsx();
                return;
            }

            if (LandingBlock == null)
                return;

            //if (Core.MyLevel.CurPhsxStep >= PanDuration) return;

            Camera cam = Core.MyLevel.MainCamera;
            PhsxData data;

            LaunchStep++;

            if (LaunchStep == 45)
                Tools.SoundWad.FindByName("Launched_Flying").Play();

            // Move stickmen
            bool OneIsSliding = Core.MyLevel.Bobs.Exists(delegate(Bob bob) { return bob.Core.Data.Velocity.Y == 0; });
            foreach (Bob bob in Core.MyLevel.Bobs)
            {
                if (!bob.Cinematic) continue;

                // If the stickman has already hit the ground
                //if (OneIsSliding)
                if (bob.Core.Data.Acceleration.Y == 0)
                {
                    // Apply friction
                    if (bob.Core.Data.Velocity.X > 62)
                        bob.Core.Data.Velocity.X *= .76f;
                    else
                    {
                        float fric = .86f + .14f * (float)Math.Cos((int)bob.MyPlayerIndex);
                        bob.Core.Data.Velocity.X = Math.Max(0, bob.Core.Data.Velocity.X - fric);
                    }
                }
                else
                    if (OneIsSliding)
                        bob.Core.Data.Velocity.Y -= 7.5f;

                // Integrate
                bob.Core.Data.Velocity.Y += bob.Core.Data.Acceleration.Y;
                bob.Core.Data.Position += bob.Core.Data.Velocity;

                // Check to see if stickman is about to hit the ground
                if (Wrapping)
                {
                    data = bob.Core.Data;
                    if (data.Velocity.Y < 0 && (data.Position.Y - LandingY) / -data.Velocity.Y < 25)
                    {
                        SetupEndOfPan();
                    }
                }
            }

            // Move camera
            data = Core.MyLevel.Bobs[0].Core.Data;

            if (data.Velocity.X > 40)
            {
                cam.Data.Position.X += data.Velocity.X;
                cam.Data.Position.X += .01f * (data.Position.X - cam.Data.Position.X);
            }
            else
            {
                if (cam.MyPhsxType != Camera.PhsxType.Center)
                {
                    cam.SetPhsxType(Camera.PhsxType.Center);
                    cam.ZoneLocked = true;
                    cam.Target = NextLevelSeedData.MyGame.MyLevel.CurPiece.CamStartPos;
                    cam.Speed = 10;
                    //Tools.StepControl = true;
                }
                //cam.Data.Position += .66f * cam.CurVel();
            }

            /*
            // At first the camera tracks stickmen (to make them look like they are moving forward faster)
            if (Wrapping)
                ;//cam.Data.Position.X += .01f * (data.Position.X - cam.Data.Position.X);
            // After landing, the camera tracks left of stickmen
            else
            {
                float dif = data.Position.X - cam.GetWidth() / 3.4f - cam.Data.Position.X;
                if (Math.Abs(dif) > 440)
                    cam.Data.Position.X += .02f * dif;
            }*/

            cam.Update();

            // Wrap blocks that have gone off screen
            if (Wrapping)
            {
                float LowestY = 10000000; foreach (Bob bob in Core.MyLevel.Bobs) LowestY = Math.Min(LowestY, bob.Core.Data.Position.Y - 700);
                foreach (Block block in Blocks)
                    if (block.Box.Current.TR.X < cam.BL.X - 100)
                    {
                        block.Move(new Vector2(cam.GetWidth() + block.Box.Current.Size.X + MyLevel.Rnd.RndFloat(0, 10000), 0));

                        // Make sure block is below lowest bob
                        if (block.Box.TR.Y > LowestY)
                            block.Move(new Vector2(0, LowestY - block.Box.TR.Y));
                    }
            }

            // Extend the background
            float BackgroundEdge = Core.MyLevel.MyBackground.TR.X;
            if (cam.Data.Position.X > BackgroundEdge - 8000)
            {
                Core.MyLevel.MyBackground.ExtendRight(BackgroundEdge + 8000);
            }

            if (!Wrapping)
            {
                // Check for end of Launch cinematic
                if (Core.MyLevel.Bobs.All(delegate(Bob bob) { return !bob.Cinematic; })
                    && (cam.Data.Position - cam.Target).Length() < 50)
                    EndLaunchCinematic();


                foreach (Bob bob in Core.MyLevel.Bobs)
                {
                    int i = (int)bob.MyPlayerIndex;

                    if (!bob.Cinematic) continue;

                    float BL_y = bob.PlayerObject.BoxList[1].BL.Pos.Y;
                    // Check for collision with landing block
                    if (BL_y < LandingY + 20)
                    {
                        // Correct y coordinate
                        bob.Move(new Vector2(0, 1f + LandingY - BL_y));

                        if (bob.Core.Data.Velocity.Y != 0)
                        {
                            Tools.SoundWad.FindByName("Launched_Landing").Play();

                            bob.Core.Data.Velocity.Y = 0;
                            bob.Core.Data.Acceleration.Y = 0;

                            // Allow cape to interact with the ground
                            /*
                            bob.MyCape.GroundCollision = true;
                            bob.MyCape.GroundHeight = 38;
                            bob.MyCape.Gravity.X = 1f;
                            */
                            // Set animation
                            bob.PlayerObject.AnimQueue.Clear();
                            bob.PlayerObject.EnqueueAnimation("LaunchedLanding3", 0, false, true, 10f, 2.9f, false);
                            bob.PlayerObject.DequeueTransfers();
                        }
                        else
                        {
                            // Check for done sliding
                            if (bob.Core.Data.Velocity.X == 0)
                            {
                                PostSlideCount[i]++;

                                if (PostSlideCount[i] == 18 + (int)(4 * Math.Cos(3*i)))
                                {
                                    bob.PlayerObject.AnimQueue.Clear();
                                    bob.PlayerObject.EnqueueAnimation(0, 0, false, true, false, 15f);
                                }

                                if (PostSlideCount[i] == 45)
                                    bob.EndCinematic();
                            }
                        }
                    }
                }
            }
        }

        // Finish the launch cinematic, killing the seed in the end
        void EndLaunchCinematic()
        {
            ((StringWorldGameData)Tools.WorldMap).LevelBegin(Core.MyLevel);

            // Move the new level back to where it was
            Core.MyLevel.Move(-HoldShift);

            Camera cam = Core.MyLevel.MainCamera;
            if (cam.MyZone != null) cam.MyZone.SetCameraSpeed = false;
            cam.MyPhsxType = Camera.PhsxType.SideLevel_Right;
            cam.Speed = cam.CurVel().Length();
            cam.TargetSpeed = 30;
            cam.SpeedVel = .5f;
            cam.ZoneLocked = false;
            
            foreach (Bob bob in HoldBobs)
            {
                Bob match = Core.MyLevel.Bobs.Find(delegate(Bob MatchingBob) { return MatchingBob.MyPlayerIndex == bob.MyPlayerIndex; });
                if (match != null)
                    bob.Move(match.Core.Data.Position - bob.Core.Data.Position);
                else
                {
                    // The corresponding Bob has been removed, remove this one as well
                    bob.Core.MarkedForDeletion = true;
                }
            }
            HoldBobs.RemoveAll(delegate(Bob bob) { return bob.Core.MarkedForDeletion; });
            Core.MyLevel.Bobs = HoldBobs;

            /*
            // Correct the bob parameters
            foreach (Bob bob in Core.MyLevel.Bobs)
                Core.MyLevel.MyGame.SetCreatedBobParameters(bob);
            */
            // Get rid of this seed
            Core.MarkedForDeletion = true;
        }

        void SetupEndOfPan()
        {
            //Tools.StepControl = true;
            Camera cam = Core.MyLevel.MainCamera;

            // End the block wrapping
            Wrapping = false;

            // Remove blocks to the right of screen
            foreach (Block block in Blocks)
                if (block.Box.Current.BL.X > cam.TR.X)
                    block.Move(new Vector2(-50000, 0));

            AddNextLevel(NextLevelSeedData);
        }

        static public List<Block> MakeRandomBlocks(Level level, Vector2 BL, Vector2 TR)
        {
            List<Block> blocks = new List<Block>();

            Vector2 Pos = BL;

            float width = 400;
            float step = 2 * width;

            NormalBlock block = null;
            while (Pos.X < TR.X)
            {
                block = (NormalBlock)level.Recycle.GetObject(ObjectType.NormalBlock, false);
                block.Core.MyTileSetType = level.MyTileSet;
                block.Init(Pos + new Vector2(width, level.Rnd.RndFloat(200, 1000)), new Vector2(level.Rnd.RndFloat(width / 2, width), 200));
                block.Extend(Side.Bottom, BL.Y - 300);

                level.AddBlock(block);
                blocks.Add(block);

                Pos.Y = BL.Y;
                Pos.X += level.Rnd.RndFloat(step, 2 * step);
            }

            return blocks;
        }
    }
}