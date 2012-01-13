using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Goombas;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public partial class BlobGuy : Boss, IObject
    {
        public void TextDraw() { }

        public void Release()
        {
            Core.Release();
        }

        ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        ObjectClass MyObject;

        ScenePart Ball_Bounce, Ball_ToCenter, Ball_Up, Stickman_FromAbove, Stickman_Normal, Stickman_Leap,
            Stickman_Ball2,
            Stickman_Princess,
            Stickman_Die;

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 5;

            //Core.ResetOnlyOnReset = true;
            Core.RemoveOnReset = true;

            Core.Data.Velocity = new Vector2(20, 20);
        }

        public static Rand Rnd = new Rand(25);
        public BlobGuy()
        {
            Rnd = new Rand(25);

            CoreData = new ObjectData();

            EzReader reader = new EzReader(Path.Combine(Globals.ContentDirectory, "Objects\\BlobGuy.smo"));
            MyObject = new ObjectClass();
            MyObject.ReadFile(reader);
            MyObject.FinishLoading();
            MyObject.Scale(new Vector2(4250, 2700));
            reader.Dispose();

            MakeNew();
        }

        int LastShakeSound = 0;
        void Shake()
        {
            //Core.MyLevel.MainCamera.StartShake(1, 79);
            Core.MyLevel.MainCamera.StartShake(.95f, 52);//40);
            //if (Tools.PhsxCount - LastShakeSound > 60)
            //{
            //    LastShakeSound = Tools.PhsxCount;
            //    Tools.Sound("Bash").Play();
            //}
        }
        int DelayedShakeCount, DelayedShakeLength = -1;
        void DelayedShake(int Delay)
        {
            DelayedShakeCount = 0;
            DelayedShakeLength = 40;
        }
        void DelayedShakePhsx()
        {
            if (DelayedShakeLength <= 0)
                return;

            DelayedShakeCount++;
            if (DelayedShakeCount >= DelayedShakeLength)
            {
                Shake();
                DelayedShakeLength = -1;
            }
        }

        public void Die()
        {
            CurState = Stickman_Die;
            CurState.Begin();
        }


        bool IsPrincess;
        bool SnapForm = true;
        public void Init(Vector2 center, bool IsPrincess)
        {
            this.IsPrincess = IsPrincess;

        //}
        //public enum StartState { Boss, 
        //public void Init(Vector2 center, )
        //{
            Tools.MoveTo(this, center);
            Core.StartData.Position = center;

            cam = Core.MyLevel.MainCamera;

            //MakeBlobs(180);
            MakeBlobs(300);
            if (!DrawInLayers)
                Core.MyLevel.ShuffleLayer(4);

            #region States
            // Stickman, die
            Stickman_Die = Make_Stickman_Die();

            // Stickman, normal
            Stickman_Normal = Make_Stickman_Normal();

            Stickman_Princess = Make_Stickman_Princess();

            Stickman_Ball2 = Make_Stickman_Ball2();

            // Stickman, from above
            Stickman_FromAbove = Make_Stickman_FromAbove(Stickman_Normal);

            // Stickman, ball 2

            // Stickman, leaping
            Stickman_Leap = new ScenePart();
            Stickman_Leap.MyBegin = delegate()
            {
                Core.Data.Velocity.Y = 0;
                SnapForm = false;
            };

            Stickman_Leap.MyPhsxStep = delegate(int Step)
            {
                StickmanToTargets(MyTargets);

                if (Core.Data.Position.Y < Core.StartData.Position.Y + 4000)
                {
                    if (Core.Data.Velocity.Y < 75)
                        Core.Data.Velocity.Y += 3.3f;

                    Core.Data.Position.Y += Core.Data.Velocity.Y;
                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived || SnapForm)
                            blob.Core.Data.Position.Y += Core.Data.Velocity.Y;
                }
                else
                {
                    CurState = Ball_Bounce;
                    CurState.Begin();
                }
            };
            
            // Ball bounce
            float BallRadius = 500;
            float BallColRadius = 500 * .8f;
            Ball_Bounce = new ScenePart();
            Ball_Bounce.MyBegin = delegate()
            {
                Core.Data.Velocity = new Vector2(35, 30);
            };

            Ball_Bounce.MyPhsxStep = delegate(int Step)
            {                
                Vector2 pos = Core.Data.Position;
                //BallToTargets(pos, BallRadius, Blobs.Count, MyTargets);
                RingToTargets(pos, 500, 1050, Blobs.Count, MyTargets);


                Core.Data.Position += Core.Data.Velocity;
                foreach (Goomba blob in Blobs)
                    if (blob.HasArrived)
                        blob.Core.Data.Position += Core.Data.Velocity;
                if (pos.X > cam.TR.X - BallColRadius) Core.Data.Velocity.X = -Math.Abs(Core.Data.Velocity.X);
                if (pos.X < cam.BL.X + BallColRadius) Core.Data.Velocity.X = Math.Abs(Core.Data.Velocity.X);
                if (pos.Y > cam.TR.Y - BallColRadius) Core.Data.Velocity.Y = -Math.Abs(Core.Data.Velocity.Y);
                if (pos.Y < cam.BL.Y + BallColRadius) Core.Data.Velocity.Y = Math.Abs(Core.Data.Velocity.Y);

                //if (Step > 2000)
                if (Step > 750)
                {
                    CurState = Ball_ToCenter;
                    CurState.Begin();
                }

                // Extra blobs
                RandomBlobsVertical(Core.MyLevel, 10, .8f, Goomba.BlobColor.Gold);
                return;
                //ElevatorBlobs();

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

            // Ball Up
            Ball_Up = new ScenePart();
            Ball_Up.MyBegin = delegate()
            {
            };

            Ball_Up.MyPhsxStep = delegate(int Step)
            {
                Vector2 pos = Core.Data.Position;
                BallToTargets(pos, BallRadius, Blobs.Count, MyTargets);

                Vector2 DesiredVel = new Vector2(0, 100);
                Core.Data.Velocity += .16f * (DesiredVel - Core.Data.Velocity);

                Core.Data.Position += Core.Data.Velocity;
                foreach (Goomba blob in Blobs)
                    if (blob.HasArrived)
                        blob.Core.Data.Position += Core.Data.Velocity;

                if (Step > 80)
                {
                    CurState = Stickman_FromAbove;
                    CurState.Begin();
                    SnapForm = false;
                }
            };

            // Ball bounce, center
            int CenteredCount = 0;
            Ball_ToCenter = new ScenePart();
            Ball_ToCenter.MyBegin = delegate()
            {
                CenteredCount = 0;

                //Core.Data.Position -= ToBallShift;
                //Core.StartData.Position -= ToBallShift;
            };

            Ball_ToCenter.MyPhsxStep = delegate(int Step)
            {
                Vector2 pos = Core.Data.Position;
                BallToTargets(pos, BallRadius, Blobs.Count, MyTargets);

                Vector2 dif = cam.Data.Position - Core.Data.Position;
                float dist = dif.Length();

                if (dist < 100)
                {
                    CenteredCount++;
                    if (CenteredCount > 90)
                    {
                        CurState = Ball_Up;
                        CurState.Begin();
                    }
                }
                /*
                if (dist < Core.Data.Velocity.Length() + 5)
                {                    
                    Core.Data.Position = cam.Data.Position;
                }
                else*/
                {
                    Core.Data.Position += Core.Data.Velocity;

                    if (dist < 1) dist = 1;
                    Vector2 DesiredVel = 35 * dif / dist;

                    Core.Data.Velocity += .1f * (DesiredVel - Core.Data.Velocity);

                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived)
                            blob.Core.Data.Position += Core.Data.Velocity;
                }
            };
            #endregion

            //CurState = Ball_Bounce;
            if (IsPrincess)
                CurState = Stickman_Princess;
            else
                CurState = Stickman_FromAbove;
            CurState.Begin();
            PhsxStep();
        }

        void DrawBlobList(List<Goomba> list, int Part) { DrawBlobList(list, Part, false); }
        void DrawBlobList(List<Goomba> list, int Part, bool OnlyArrived)
        {
            int i = Part;
            //for (int i = 0; i < 3; i++)
                foreach (Goomba blob in list)
                    if (!OnlyArrived || blob.HasArrived)
                        Tools.QDrawer.DrawQuad(blob.MyObject.Quads[i]);
        }

        public void Draw()
        {
            if (!Core.Active) return;

            //MyObject.MoveTo(Tools.CurLevel.MainCamera.Data.Position + new Vector2(0, -450));

            MyObject.PlayUpdate(.03f);
            //MyObject.Draw(true);

            //if (Blobs != null)
            //{
            //    if (DrawInLayers)
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            DrawBlobList(Blobs, i);
            //            Tools.QDrawer.Flush();
            //        }
            //    }
            //}

            //if (CrossBlobs != null)
            //{
            //    for (int i = 0; i < 3; i++)
            //        DrawBlobList(CrossBlobs, i);
            //}
        }

        void SetBlobParams(Goomba blob, float ArrivedMaxVel, float ArrivedMaxAcc, float NotArrivedMaxVel, float NotArrivedMaxAcc)
        {
            blob.ArrivedRadius = 550;
            if (blob.HasArrived)
            {
                blob.MaxVel = ArrivedMaxVel;
                blob.MaxAcc = ArrivedMaxAcc;
            }
            else
            {
                blob.MaxVel = NotArrivedMaxVel;
                blob.MaxAcc = NotArrivedMaxAcc;
            }
        }

        int RandomOffset = 0;
        Vector2 RandomPos_ScreenSide(Camera cam)
        {
            Vector2 pos = Vector2.Zero;
            //if (Tools.Rnd.NextDouble() > .5f)
            if (Math.Cos(2 * RandomOffset + Core.MyLevel.CurPhsxStep) > 0)
                pos.X = cam.TR.X + 500;
            else
                pos.X = cam.BL.X - 500;
            
            //pos.Y = Tools.RndFloat(cam.BL.Y, cam.TR.Y);
            pos.Y = (float)Math.Cos(RandomOffset + Core.MyLevel.CurPhsxStep) * cam.GetHeight() / 2 + cam.BL.Y;
            RandomOffset++;

            return pos;
        }

        Goomba MakeRedBlob()
        {
            Goomba blob = MakeBlob();

            blob.SetColor(Goomba.BlobColor.Green);

            return blob;
        }

        int SourceAssignmentCount = 0;
        Goomba MakeBlob() { return MakeBlob(true); }
        Goomba MakeBlob(bool CopySource)
        {
            Goomba blob = (Goomba)Core.Recycle.GetObject(ObjectType.FlyingBlob, false);
            blob.Core.Data.Position = RandomPos_ScreenSide(Core.MyLevel.MainCamera); //Core.MyLevel.MainCamera.Data.Position +2000 * Tools.RndDir();

            if (CopySource)
            {
                blob.CopySource = SourceBlobs[SourceAssignmentCount % NumSourceBlobs];
                SourceAssignmentCount += (int)(15 + 10 * Math.Cos(SourceAssignmentCount));
            }

            blob.NeverSkip = true;

            blob.DeleteOnDeath = true;
            blob.Core.RemoveOnReset = true;

            blob.SetAnimation();

            blob.GiveVelocity = true;

            blob.MaxVel = 10;
            blob.MaxAcc = 1;

            blob.MaxVel = 20;
            blob.MaxAcc = 1.5f;


            blob.MyPhsxType = Goomba.PhsxType.ToTarget;
            blob.Target = Core.MyLevel.MainCamera.Data.Position;

            if (DrawInLayers)
            {
                if (CopySource)
                    blob.Core.DrawLayer = -1;
            }

            return blob;
        }

        static bool DrawInLayers = false;

        List<Goomba> Blobs, TempBlobs, SourceBlobs, CrossBlobs;
        int NumSourceBlobs = 3;
        void MakeBlobs(int Num)
        {
            SourceBlobs = new List<Goomba>();
            for (int i = 0; i < NumSourceBlobs; i++)
            {
                Goomba blob = MakeBlob(false);

                blob.MyObject.Linear = false;
                SourceBlobs.Add(blob);

                //Tools.MoveTo(blob, new Vector2(Core.MyLevel.MainCamera.Data.Position.X, Core.MyLevel.MainCamera.BL.Y - 500));
                //blob.Target = blob.Core.Data.Position;
            }


            Blobs = new List<Goomba>();
            TempBlobs = new List<Goomba>();
            MyTargets = new List<Vector2>();

            for (int i = 0; i < Num; i++)
            {
                Goomba blob = MakeRedBlob();

                Core.MyLevel.AddObject(blob);

                Blobs.Add(blob);
            }

            CrossBlobs = new List<Goomba>();
        }

        float OscillateSpeed = .05f;
        
        //float Thickness = .04f;
        float Thickness = .0575f;

        List<Vector2> MyTargets;
        void BendableQuadToTargets(BendableQuad quad, int N, List<Vector2> Targets, float h)
        {
            float step = (float)(quad.MySpline.Nodes - 1) / (N - 1);
            for (int i = 0; i < N; i++)
            {
                float n = h * Thickness * (float)Math.Cos(5 * i + OscillateSpeed * Core.MyLevel.CurPhsxStep);
                Vector2 Target = quad.GetVector(i * step, n);
                Targets.Add(Target);
            }
        }

        Vector2 QuadCenter(Quad quad)
        {
            return (quad.Vertices[0].xy + quad.Vertices[1].xy + quad.Vertices[2].xy + quad.Vertices[3].xy) / 4;
        }

        void QuadToTargets(Quad quad, int N, List<Vector2> Targets, float Radius, Vector2 shift)
        {
            Vector2 center = QuadCenter(quad) + shift;

            float radius = Radius * (quad.xAxis.Pos - center).Length();

            BallToTargets(center, radius, N, Targets);
        }

        void LineToTargets(Vector2 p1, Vector2 p2, int N, List<Vector2> Targets)
        {
            if (N <= 1) return;

            Vector2 step = (p2 - p1) / (N - 1);
            Vector2 pos = p1;
            for (int i = 0; i < N; i++)
            {
                Targets.Add(pos);
                pos += step;
            }
        }

        void BallToTargets(Vector2 Center, float Radius, int N, List<Vector2> Targets)
        {
            for (int i = 0; i < N; i++)
            {
                float r = (float)Math.Cos(5 * i + .75f * OscillateSpeed * Core.MyLevel.CurPhsxStep);
                r = 1.5f * Math.Sign(r) * (1 - (float)Math.Pow(Math.Abs(r), 1.6f));
                
                r *= Radius;
                
                Vector2 Target = Center + r * Tools.AngleToDir(2 * Math.PI * i / N);
                
                Targets.Add(Target);
            }
        }

        void RingToTargets(Vector2 Center, float Radius1, float Radius2, int N, List<Vector2> Targets)
        {
            for (int i = 0; i < N; i++)
            {
                float r = (float)Math.Cos(5 * i + .75f * OscillateSpeed * Core.MyLevel.CurPhsxStep);
                r = 1.5f * Math.Sign(r) * (1 - (float)Math.Pow(Math.Abs(r), 1.6f));

                r *= Radius2 - Radius1;
                r += Radius1;

                Vector2 Target = Center + r * Tools.AngleToDir(2 * Math.PI * i / N + Tools.CurLevel.CurPhsxStep * .045f);

                Targets.Add(Target);
            }
        }

        int gattling_count = 0;
        void Gattling()
        {
            //Vector2 hand = QuadCenter((Quad)MyObject.FindQuad("Hips"));
            Vector2 hand = ((BendableQuad)MyObject.FindQuad("Body")).GeoCenter();
            if (gattling_count > 0)
            _RandomBlobs(Core.MyLevel, 1, 2, Goomba.BlobColor.Blue, blob =>
            {
                Camera Cam = Core.MyLevel.MainCamera;

                Vector2 dir = Tools.AngleToDir(Core.MyLevel.CurPhsxStep * .035f);
                blob.Pos = hand + dir * 30 + Tools.RndDir(90);
                blob.Core.DrawLayer--;

                blob.Target = blob.Pos + dir * 3000;
            });

            gattling_count++;
            if (gattling_count > 40) gattling_count = -85;
        }

        List<Vector2> TempTargets = new List<Vector2>();
        void StickmanToTargets(List<Vector2> Targets)
        {
            float Total = HeadRatio + 2 * HandRatio + 2 * LegRatio + 2 * ArmRatio + BodyRatio;

            Head_N = (int)(HeadRatio * Blobs.Count / Total);
            Hand_N = (int)(HandRatio * Blobs.Count / Total);
            Arm_N = (int)(ArmRatio * Blobs.Count / Total);
            Leg_N = (int)(LegRatio * Blobs.Count / Total);
            Body_N = (int)(BodyRatio * Blobs.Count / Total);

            MyObject.MoveTo(Core.Data.Position);
            MyObject.Update(null);

            if (MyObject.xFlip)
                MyObject.ParentQuad.xAxis.RelPos.X = -Math.Abs(MyObject.ParentQuad.xAxis.RelPos.X);
            else
                MyObject.ParentQuad.xAxis.RelPos.X = Math.Abs(MyObject.ParentQuad.xAxis.RelPos.X);


            foreach (BaseQuad quad in MyObject.QuadList)
            {
                int n = 0;

                BendableQuad bquad = quad as BendableQuad;
                if (null != bquad)
                {
                    float h = 1;

                    if (bquad.Is("Arm_Left") || bquad.Is("Arm_Right"))
                        n = Arm_N;
                    if (bquad.Is("Leg_Left") || bquad.Is("Leg_Right"))
                        n = Leg_N;
                    if (bquad.Is("Body"))
                    {
                        n = Body_N;
                        h = .85f;
                    }

                    BendableQuadToTargets(bquad, n, Targets, h);
                }

                Quad qquad = quad as Quad;
                if (null != qquad && qquad.Show)
                {
                    float radius = 1;
                    Vector2 shift = Vector2.Zero;

                    if (qquad.Is("Hand_Left") || qquad.Is("Hand_Right"))
                    {
                        n = Hand_N;
                        radius = .85f;
                    }
                    if (qquad.Is("Head"))
                    {
                        n = Blobs.Count - 2 * Hand_N - 2 * Leg_N - 2 * Arm_N - Body_N;
                        radius = 1.00f;
                        shift = new Vector2(0, -110);
                    }
                    if (qquad.Is("Hips"))
                        n = 0;

                    if (n > 0)
                        QuadToTargets(qquad, n, Targets, radius, shift);
                }
            }
        }

        Goomba FindClosest(List<Goomba> list, Vector2 target)
        {
            Goomba Closest = null;
            float ClosestDist = 0;
            foreach (Goomba blob in list)
            {
                float dist = (blob.Core.Data.Position - target).LengthSquared();
                if (Closest == null || dist < ClosestDist)
                {
                    Closest = blob;
                    ClosestDist = dist;
                }
            }

            return Closest;
        }

        

        void AssignTargetsToClosestBlobs()
        {
            TempBlobs.Clear();
            TempBlobs.AddRange(Blobs);

            
            foreach (Vector2 target in MyTargets)
            {
                Goomba blob = FindClosest(TempBlobs, target);
                blob.Target = target;

                TempBlobs.Remove(blob);
                if (TempBlobs.Count == 0)
                    break;
            }
        }


        float AssignmentOffset = 0;
        void AssignTargetsToBlobs()
        {
            for (int i = 0; i < MyTargets.Count; i++)
            {
                //if (i >= Blobs.Count) break;

                Goomba blob = Blobs[(i + (int)AssignmentOffset) % Blobs.Count];
                blob.Target = MyTargets[i];
            }
        }

        float HeadRatio = 63, HandRatio = 20, LegRatio = 45, ArmRatio = 40, BodyRatio = 65;
        int Head_N, Hand_N, Leg_N, Arm_N, Body_N;

        void RemoveDeadBlobs()
        {
            Camera cam = Core.MyLevel.MainCamera;

            for (int i = 0; i < Blobs.Count; i++)
                if (Blobs[i].Core.MarkedForDeletion)
                    Blobs[i] = null;
            for (int i = 0; i < Blobs.Count; i++)
            {
                Goomba blob = Blobs[i];
                if (blob == null)
                {
                    Goomba newblob = MakeRedBlob();
                    Core.MyLevel.AddObject(newblob);

                    Vector2 pos = RandomPos_ScreenSide(Core.MyLevel.MainCamera);
                    newblob.Core.Data.Position = pos;

                    Blobs[i] = newblob;
                }
            }
        }


        void UpdateSourceBlobs()
        {
            foreach (Goomba blob in SourceBlobs)
            {
                blob.Core.WakeUpRequirements = false;
                blob.CopySource = null;

                blob.AnimStep();
                blob.UpdateObject();
            }
        }

        void LineSweep(int Dir, float h1, float h2, int N)
        {
            Camera cam = Core.MyLevel.MainCamera;

            float y1 = cam.BL.Y + h1 * cam.GetHeight();

            Vector2 p1 = new Vector2(cam.BL.X - 200, y1);
            Vector2 p2 = new Vector2(cam.TR.X + 200, y1);
            if (Dir == -1)
            {
                Vector2 hold = p1;
                p1 = p2;
                p2 = hold;
            }

            Vector2 step = new Vector2(0, cam.GetHeight() * (h2 - h1)) / N;
            for (int i = 0; i < N; i++)
            {
                Goomba blob = MakeBlob();

                blob.SetColor(Goomba.BlobColor.Pink);

                blob.Core.Data.Position = p1 + i * step;
                blob.Target = p2 + i * step;

                blob.MaxVel = 35;

                Core.MyLevel.AddObject(blob);
                CrossBlobs.Add(blob);
            }
        }

        void Nova(Vector2 pos, Vector2 vel, int N)
        {
            float step = 2f * (float)Math.PI / N;
            for (int i = 0; i < N; i++)
            {
                Goomba blob = MakeBlob();

                blob.SetColor(Goomba.BlobColor.Pink);

                blob.Core.Data.Position = pos;
                blob.Core.Data.Velocity = vel;
                blob.Target = pos + Core.MyLevel.MainCamera.GetWidth() * Tools.AngleToDir(i * step);

                blob.MaxVel = 35;

                blob.Core.DrawLayer = 5;
                Core.MyLevel.AddObject(blob);
                CrossBlobs.Add(blob);
            }
        }

        Vector2[] CrossPos1 = { new Vector2(.25f, -.1f), new Vector2(.5f, -.1f), new Vector2(.75f, -.1f) };
        Vector2[] CrossPos2 = { new Vector2(.25f, 1.1f), new Vector2(.5f, 1.1f), new Vector2(.75f, 1.1f) };
        void ElevatorBlobs()
        {
            int Step = Core.MyLevel.CurPhsxStep;
            if (Step % 30 == 0)
            {
                Camera cam = Core.MyLevel.MainCamera;
                Vector2 CamSize = new Vector2(cam.GetWidth(), cam.GetHeight());
                int i = Step / 30 % 3;
                Vector2 pos1 = CrossPos1[i] * CamSize + cam.BL;
                Vector2 pos2 = CrossPos2[i] * CamSize + cam.BL;

                Goomba blob = MakeBlob();

                blob.Core.Data.Position = pos1;
                blob.Target = pos2;

                Core.MyLevel.AddObject(blob);
                CrossBlobs.Add(blob);
            }
        }
        
        void CrossBlobsPhsxStep()
        {
            foreach (Goomba blob in CrossBlobs)
            {
                if ((blob.Target - blob.Core.Data.Position).LengthSquared() < 10000)
                {
                    Core.Recycle.CollectObject(blob);
                }
            }
            CrossBlobs.RemoveAll(delegate(Goomba blob) { return blob.Core.MarkedForDeletion; });
        }

        //public static Rand Rnd;
        public static void RandomBlobs(Level level, int Delay, float SpeedMod, Goomba.BlobColor Color)
        {
            _RandomBlobs(level, Delay, SpeedMod, Color, blob =>
            {
                Camera Cam = level.MainCamera;

                Vector2 pos = new Vector2(0, Rnd.RndFloat(Cam.BL.Y + 550, Cam.TR.Y));
                if (Rnd.RndBool())
                    pos.X = Cam.BL.X - 300;
                else
                    pos.X = Cam.TR.X + 300;
                blob.Pos = pos;

                Tools.Write("p " + level.CurPhsxStep.ToString() + " -> " + pos.X);

                blob.Target = 2.2f * (Cam.Pos - blob.Pos) + blob.Pos;
                blob.Target.Y = blob.Pos.Y + Rnd.RndFloat(-200, 420);
            });
        }

        public static void RandomBlobsVertical(Level level, int Delay, float SpeedMod, Goomba.BlobColor Color)
        {
            _RandomBlobs(level, Delay, SpeedMod, Color, blob =>
            {
                Camera Cam = level.MainCamera;

                Vector2 pos = new Vector2(Rnd.RndFloat(Cam.BL.X + 150, Cam.TR.X - 150), 0);
                if (Rnd.RndBool())
                    pos.Y = Cam.BL.Y - 300;
                else
                    pos.Y = Cam.TR.Y + 300;
                blob.Pos = pos;

                blob.Target = 2.2f * (Cam.Pos - blob.Pos) + blob.Pos;
                blob.Target.X = blob.Pos.X + Rnd.RndFloat(-200, 420);
            });
        }

        static void _RandomBlobs(Level level, int Delay, float SpeedMod, Goomba.BlobColor Color, Action<Goomba> ModBlob)
        {
            if (level.CurPhsxStep % Delay != 0)
                return;

            //Tools.Write("! " + level.CurPhsxStep.ToString() + " -> " + Rnd.RndInt(0, 1000).ToString());

            // Make blob
            Goomba blob = (Goomba)level.Recycle.GetObject(ObjectType.FlyingBlob, false);

            ModBlob(blob);

            blob.NeverSkip = true;

            blob.DeleteOnDeath = true;
            blob.RemoveOnArrival = true;
            blob.Core.RemoveOnReset = true;

            blob.MaxVel = 35 * SpeedMod;
            blob.MaxAcc = 1.5f;

            blob.MyPhsxType = Goomba.PhsxType.ToTarget;

            blob.SetColor(Color);

            level.AddObject(blob);
        }

        public override void PhsxStep()
        {
            if (Core.Released) return;

            if (Game != null)
            {
                Game.BlobGraceY = 336;
                Game.ModdedBlobGrace = true;
            }

            if (!Core.Active) return;

            MyWaitAndToDoList.PhsxStep();

            DelayedShakePhsx();

            foreach (Goomba blob in Blobs)
                if (!blob.FinalizedParams)
                    SetBlobParams(blob, 20, 2, 20, 1.2f);

            RemoveDeadBlobs();

            CrossBlobsPhsxStep();

            UpdateSourceBlobs();

            MyTargets.Clear();

            if (CurState != null)
                CurState.PhsxStep();
                                
            AssignTargetsToBlobs();
        }

        public void PhsxStep2() { }
        public void Reset(bool BoxesOnly)
        {

        }

        public void Clone(IObject A) { }
        public void Interact(Bob bob) { }
        public void Move(Vector2 shift)
        {
            Core.Data.Position += shift;            
            MyObject.MoveTo(Core.Data.Position);

            if (Title != null) Title.Pos += shift;

            Core.StartData.Position += shift;
            if (Blobs != null)
                foreach (Goomba blob in Blobs)
                {
                    blob.Core.Data.Position += shift;
                    blob.Target += shift;
                    //blob.Move(shift);
                }
        }
        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}