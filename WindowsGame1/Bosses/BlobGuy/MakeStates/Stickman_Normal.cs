using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Goombas;
using Drawing;

namespace CloudberryKingdom
{
    public partial class BlobGuy
    {
        void Flash(int step)
        {
            foreach (Goomba blob in Blobs)
            {
                if (step % 10 == 0)
                    blob.SetColor(Goomba.BlobColor.Blue);
                if (step % 10 == 5)
                    blob.SetColor(Goomba.BlobColor.Green);
            }
        }

        public ScenePart Make_Stickman_Die()
        {
            Vector2 ShakeOffset = Vector2.Zero;

            var State = new ScenePart();
            State.MyBegin = () =>
            {
                MyObject.AnimQueue.Clear();
                
                if (!IsPrincess)
                    MyObject.EnqueueAnimation("Normal", 0, false, false, 1, 1.35f, true);

                MyObject.Play = true;

                ShakeOffset = Vector2.Zero;
            };

            State.MyPhsxStep = step =>
            {
                Vector2 BallPos = .3f * Pos + .7f * Tools.CurCamera.Pos;


                // Flash colors
                Flash(step);

                int PreExplode = 84 + 10;
                int Ball = PreExplode + 50;
                int Explode = Ball + 8;

                // Launch the princess
                if (princess != null)
                {
                    if (step < Explode)
                        Core.Data.Velocity *= .8f;
                    else
                        Core.Data.Acceleration = new Vector2(0, 12.5f);

                    // Move blobs
                    Core.Data.Integrate();
                    //Core.Data.Position += Core.Data.Velocity;
                    Core.StartData.Position = Core.Data.Position;
                    Tools.MoveTo(princess, Pos);
                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived)
                            blob.Core.Data.Position += Core.Data.Velocity;
                }

                // Growl
                if (step == 70)
                    Tools.Sound("MonsterGrowl_Short").Play();

                // Kill the music
                if (step == Ball)
                    Tools.SongWad.FadeOut();

                // Pre-explode
                if (step < PreExplode)
                {
                    if (!IsPrincess)
                        StickmanToTargets(MyTargets);
                    else
                        BallToTargets(Pos, 600, Blobs.Count, MyTargets);
                        //BallToTargets(Pos, 200, MyTargets.Count, MyTargets);

                    // Shake
                    if (step % 2 == 0)
                    {
                        Vector2 OldShake = ShakeOffset;

                        float ShakingIntensity = .3f;
                        ShakeOffset.X = ShakingIntensity * MyLevel.Rnd.Rnd.Next(-70, 70);
                        ShakeOffset.Y = ShakingIntensity * MyLevel.Rnd.Rnd.Next(-70, 70);

                        foreach (Goomba blob in Blobs)
                        {
                            if (blob.HasArrived)
                            {
                                blob.Move(ShakeOffset - OldShake);
                            }
                        }
                    }
                }
                else if (step < Ball)
                {
                    if (IsPrincess)
                        BallPos = Pos;
                    BallToTargets(BallPos, 300, Blobs.Count, MyTargets);
                    Blobs.ForEach(blob => {
                        blob.MaxVel = 135;
                        blob.MaxAcc = 135;
                        blob.FinalizedParams = true;
                        blob.DistAccMod = 3.25f;
                        blob.Damp = .95f;
                        blob.DampRange = 0;

                        if ((blob.Target - blob.Pos).Length() < 50)
                            blob.Core.Data.Velocity *= .95f;
                    });
                }
                else if (step == Explode)
                {
                    if (OnExplode != null) OnExplode(); OnExplode = null;
                    Exploded = true;

                    // Launch the blobs
                    foreach (Goomba blob in Blobs)
                    {
                        blob.Target = (blob.Pos - BallPos);
                        blob.Target.Normalize();
                        blob.Target *= 4000 * MyLevel.Rnd.RndFloat(2.5f, 5f); ;
                        blob.Target += Pos;

                        blob.MaxVel *= MyLevel.Rnd.RndFloat(28, 68) / 2;
                        blob.MaxAcc *= MyLevel.Rnd.RndFloat(24, 58) / 2;
                        blob.FinalizedParams = true;
                    }
                }
                else if (step > Explode + 1 && step < Explode + 18)
                {
                    foreach (Goomba blob in Blobs)
                    {
                        if (MyLevel.Rnd.RndBool(.0375f))
                        {
                            blob.Squish(blob.Core.Data.Velocity * .4f);

                            //blob.Target = new Vector2(0, 1000000);
                        }
                    }
                }
                // Push away any straggler blobs
                else if (step > Explode + 24)
                {
                    foreach (Goomba blob in Blobs)
                    {
                        blob.Core.Show = false;
                        blob.Move(new Vector2(10000, 10000));
                        blob.Target = new Vector2(0, 1000000);
                    }
                }
            };

            return State;
        }

        PrincessBubble princess;
        public ScenePart Make_Stickman_Princess()
        {
            ScenePart State = new ScenePart();

            Laser Laser_1, Laser_2, Laser_3;
            princess = null;

            Vector2[] Targets = { Vector2.Zero, new Vector2(-1200, 0), new Vector2(1200, 0), Vector2.Zero,
                                new Vector2(0, 1000), new Vector2(0, -700),
                                new Vector2(-1200, -700), new Vector2(1200, -700),
                                Vector2.Zero,
                                new Vector2(-400, 0), new Vector2(0,400), new Vector2(400,0), new Vector2(0,-400)};
        
            int TargetIndex = 0;

            // Princess
            float BallRadius = 560;
            State = new ScenePart();
            State.MyBegin = delegate()
            {
                SnapForm = true;
                //MakeLasers();

                Core.Data.Position = cam.Data.Position + new Vector2(0, cam.GetHeight() + 500);

                princess = new PrincessBubble(Core.Data.Position);
                princess.Core.RemoveOnReset = true;
                princess.CanBePickedUp = false;
                princess.Core.DrawLayer = 4;
                Core.MyLevel.AddObject(princess);
            };

            State.MyPhsxStep = delegate(int Step)
            {
                if (Step == 1)
                    foreach (Goomba blob in Blobs)
                        blob.Core.Data.Position = blob.Target;

                // Target destination
                Vector2 Target = cam.Data.Position + Targets[TargetIndex];

                Vector2 dif = (Target - Pos);
                if (dif.Length() < 400) Core.Data.Velocity *= .85f;
                if (dif.Length() < 50 && Core.Data.Velocity.Length() < 5)
                {
                    TargetIndex = (TargetIndex + 1) % Targets.Length;
                    
                    Nova(Core.Data.Position, Core.Data.Velocity, 10);
                }

                dif.Normalize(); dif *= 60;
                Core.Data.Velocity += (dif - Core.Data.Velocity) * .035f;

                // Target blobs
                Vector2 pos = Pos;
                princess.MyAnimation = PrincessBubble.BossAnim;
                float extra = Tools.PeriodicCentered(0, 600, 60, Core.MyLevel.CurPhsxStep);
                int n1 = (int)(.7f * Blobs.Count);
                int n2 = Blobs.Count - n1;
                BallToTargets(pos, BallRadius + extra, n1, MyTargets);
                RingToTargets(pos, BallRadius + extra, BallRadius + 200 + extra, n2, MyTargets);

                // Layer blobs
                int Count = 0;
                foreach (Goomba blob in Blobs)
                {
                    int DesiredLayer;

                    if (Count < n1)
                        DesiredLayer = 3;
                    else
                        DesiredLayer = 5;

                    Core.MyLevel.ChangeObjectDrawLayer(blob, DesiredLayer);

                    Count++;
                }
                //foreach (Goomba blob in CrossBlobs)
                foreach (ObjectBase obj in Core.MyLevel.Objects)
                {
                    Goomba blob = obj as Goomba;
                    if (null != blob && blob.MyColor == Goomba.BlobColor.Pink)
                        Core.MyLevel.ChangeObjectDrawLayer(blob, 3);
                }

                // Move blobs
                Core.Data.Position += Core.Data.Velocity;
                Core.StartData.Position = Core.Data.Position;
                Tools.MoveTo(princess, Pos);
                foreach (Goomba blob in Blobs)
                    if (blob.HasArrived)
                        blob.Core.Data.Position += Core.Data.Velocity;
            };

            return State;
        }

        static Vector2 ToBallShift = new Vector2(0, 1150);

        public ScenePart Make_Stickman_Ball2()
        {
            ScenePart State = new ScenePart();

            // Ball bounce
            float BallRadius = 500;
            float BallColRadius = 500 * .8f;
            State = new ScenePart();
            State.MyBegin = delegate()
            {
                //Core.Data.Position += ToBallShift;
                //Core.StartData.Position += ToBallShift;
            };

            State.MyPhsxStep = delegate(int Step)
            {
                Vector2 pos = Core.Data.Position;
                BallToTargets(pos, BallRadius, Blobs.Count, MyTargets);

                Vector2 DesiredVelocity = new Vector2(35, 30);

                // Wait, then start bouncing
                if (Step > 125)
                {
                    if (Core.Data.Velocity == Vector2.Zero)
                        Core.Data.Velocity = .01f * DesiredVelocity;
                    else
                        Core.Data.Velocity += .045f * 
                            Tools.Sign(Core.Data.Velocity) * (DesiredVelocity - Tools.Abs(Core.Data.Velocity));

                    Core.Data.Position += Core.Data.Velocity;
                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived)
                            blob.Core.Data.Position += Core.Data.Velocity;
                    if (pos.X > cam.TR.X - BallColRadius) Core.Data.Velocity.X = -Math.Abs(Core.Data.Velocity.X);
                    if (pos.X < cam.BL.X + BallColRadius) Core.Data.Velocity.X = Math.Abs(Core.Data.Velocity.X);
                    if (pos.Y > cam.TR.Y - BallColRadius) Core.Data.Velocity.Y = -Math.Abs(Core.Data.Velocity.Y);
                    if (pos.Y < cam.BL.Y + BallColRadius) Core.Data.Velocity.Y = Math.Abs(Core.Data.Velocity.Y);
                }

                if (Step > 800)
                {
                    CurState = Ball_ToCenter;
                    CurState.Begin();
                }

                // Extra blobs
                RandomBlobsVertical(Core.MyLevel, 12, .7f, Goomba.BlobColor.Gold);
                return;


                int LineSweepPeriod = 240;

                if (Core.MyLevel.CurPhsxStep % (3 * LineSweepPeriod / 2) == 25)
                    Nova(Core.Data.Position, Core.Data.Velocity, 10);

                if (Core.MyLevel.CurPhsxStep % LineSweepPeriod == 100)
                {
                    int i = Core.MyLevel.CurPhsxStep / LineSweepPeriod;
                    switch (i % 4)
                    {
                        case 0:
                            LineSweep(1, .66f, 1f, 5);
                            break;

                        case 1:
                            LineSweep(-1, .33f, .66f, 5);
                            break;

                        case 2:
                            LineSweep(1, .0f, .33f, 5);
                            break;

                        case 3:
                            LineSweep(-1, .33f, .66f, 5);
                            break;
                    }
                }
            };

            return State;
        }

        void ToAndBack()
        {
            var pos = new FancyVector2();
            Vector2 shift = new Vector2(-310, 0);
            if (MyObject.xFlip) shift.X *= -1;

            pos.ToAndBack(Pos, Pos + shift, 53);
            Game.CinematicToDo(() =>
            {
                Core.StartData.Position = Pos = pos.Update();
                if (!pos.Playing)
                {
                    pos.Release();
                    return true;
                }
                else
                    return false;
            });
        }

        public ScenePart Make_Stickman_Normal()
        {
            int ReadyToLeapCount = -1;

            ScenePart State = new ScenePart();
            State.MyBegin = delegate()
            {
                ReadyToLeapCount = -1;

                float PlaySpeed = 1.165f;
                float TransferSpeed = 1.8f;
                MyObject.EnqueueAnimation("StandUp", 0, false, true, 10, 1.475f, true);
                //MyObject.EnqueueAnimation("Normal", 0, false, false, 1, 1.35f, true);
                MyObject.EnqueueAnimation("FootStomp", 0, false, false, TransferSpeed, 1.5f * PlaySpeed, true);
                MyObject.EnqueueAnimation("FootStomp", 0, false, false, .5f * TransferSpeed, 1.5f * PlaySpeed, true);
                MyObject.EnqueueAnimation("FootStomp", 0, false, false, .5f * TransferSpeed, 1.5f * PlaySpeed, true);
                MyObject.EnqueueAnimation("FootStomp", 0, false, false, .5f * TransferSpeed, 1.5f * PlaySpeed, true);
                //MyObject.EnqueueAnimation("Kick", 0, false, false, TransferSpeed, 1.5f * PlaySpeed, true);
                
                
                //MyObject.EnqueueAnimation("StandUp", 0, false, false, TransferSpeed, 1.35f, true);
                //MyObject.EnqueueAnimation("Normal", 0, false, false, 1, 1.35f, true);
                //MyObject.EnqueueAnimation("FootStomp", 0, false, false, TransferSpeed, 1.5f, true);
                //MyObject.EnqueueAnimation("StandUp", 0, false, false, TransferSpeed, 1.35f, true);
                //MyObject.EnqueueAnimation("Normal", 0, false, false, 1, 1.35f, true);

                MyObject.Play = true;
            };

            State.MyPhsxStep = delegate(int Step)
            {
                //if (Step > 140)
                //    Gattling();

                // Growl
                if (MyObject.anim == MyObject.FindAnim("StandUp"))
                    if (MyObject.AtTime(.85f))
                        Tools.Sound("MonsterGrowl_Short").Play();

                // Extra blobs (Normal Campaign and up)
                if (Campaign.Index == 1)
                    RandomBlobs(Core.MyLevel, 21, .525f, Goomba.BlobColor.Grey);
                else if (Campaign.Index == 2)
                    RandomBlobs(Core.MyLevel, 19, .6f, Goomba.BlobColor.Grey);
                else if (Campaign.Index == 3)
                    RandomBlobs(Core.MyLevel, 17, .65f, Goomba.BlobColor.Grey);
                else if (Campaign.Index == 4)
                    RandomBlobs(Core.MyLevel, 15, .73f, Goomba.BlobColor.Grey);


                if (MyObject.anim == MyObject.FindAnim("FootStomp") && MyObject.AtTime(1f))
                    ToAndBack();

                // Nova on foot stomp
                if (MyObject.anim == MyObject.FindAnim("FootStomp") && MyObject.AtTime(3.79f))
                {
                    MyWaitAndToDoList.Add(40, () =>
                    {
                        Shake();
                        Nova(Core.Data.Position, Vector2.Zero, 10);
                        MyObject.xFlip = !MyObject.xFlip;

                        return true;
                    });
                }

                if (ReadyToLeapCount < 40)
                {
                    //// Move faster
                    //foreach (Goomba blob in Blobs)
                    //{
                    //    if (blob.HasArrived)
                    //    {
                    //        blob.Pos += .05f * (blob.Target - blob.Pos);
                    //        blob.Core.Data.Velocity *= .2f;
                    //    }
                    //}

                    if (MyObject.AnimQueue.Count == 0)
                        ReadyToLeapCount++;

                    if (ReadyToLeapCount == 40)
                        MyObject.EnqueueAnimation("StandUp", 0, false, true, 4, 3.5f, true);
                }
                else
                {
                    BlobModOnArrival();

                    ReadyToLeapCount++;

                    if (ReadyToLeapCount > 143)
                    {
                        CurState = Stickman_Leap;
                        //CurState = Stickman_Ball2;
                        CurState.Begin();
                    }
                }

                Core.Data.Position = Core.StartData.Position;

                StickmanToTargets(MyTargets);
            };

            return State;
        }

        private void BlobModOnArrival()
        {
            foreach (Goomba blob in Blobs)
            {
                if (blob.HasArrived)
                {
                    blob.MaxVel = 40;
                    blob.MaxAcc = 4;
                }
                else
                {
                    blob.Target = 100 * (blob.Target - cam.Data.Position) + cam.Data.Position;
                }
            }
        }
    }
}