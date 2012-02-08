using System;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Camera : IPos, IViewable
    {
        public virtual string[] GetViewables()
        {
            return new string[] { "!MyLevel" };
        }

        /// <summary>
        /// When true the camera uses its EffectivePos and EffectiveZoom when drawing.
        /// The camera's TR, BL, width, and height are unaffected and are updated normally.
        /// This allows the camera to be moved without affecting what is drawn.
        /// </summary>
        public bool UseEffective = false;
        public Vector2 EffectivePos;
        public Vector2 EffectiveZoom;

        public Vector4 VertexCam;

        public bool RocketManCamera = false;

        public FancyVector2 FancyPos, FancyZoom;

        public bool Shaking, Oscillating = false;
        public float ShakingIntensity;
        public Vector2 ShakingSaveZoom, ShakingSavePos;

        public CameraZone MyZone;
        
        /// <summary>
        /// If true the camera will not interact with other CameraZones
        /// </summary>
        public bool ZoneLocked = false;

        public Level MyLevel;
        public PhsxData Data;
        public Vector2 PrevPos, PrevPrevPos;

        public Vector2 Pos
        {
            get { return Data.Position; }
            set { Data.Position = value; }
        }
        

        /// <summary>
        /// The current maximum speed amonst all the alive players.
        /// </summary>
        public Vector2 MaxPlayerSpeed;

        public float Speed, SpeedVel, TargetSpeed;
        public Vector2 Target;
        Vector2 _Zoom;
        public Vector2 Zoom
        {
            get { return _Zoom; }
            set { _Zoom = value; }
        }

        public Vector2 ScreenSize { get { return 2 * new Vector2(ScreenWidth, ScreenHeight); } }

        public Vector2 Offset;
        public int ScreenWidth, ScreenHeight;
        public float AspectRatio;

        public Vector2 TR, BL;
        public Vector2 EffectiveTR, EffectiveBL;

        public Vector2 BLCamBound, TRCamBound;

        public bool FollowCenter;

        public void Release()
        {
            MyLevel = null;
            MyZone = null;

            FancyPos = FancyZoom = null;
        }


        public void Move(Vector2 shift)
        {
            Data.Position += shift;
            Target += shift;
            
            TR += shift;
            BL += shift;
            TRCamBound += shift;
            BLCamBound += shift;
            
            PrevPos += shift;
            PrevPrevPos += shift;
        }

        public void Clone(Camera cam) { Clone(cam, false); }
        public void Clone(Camera cam, bool DataOnly)
        {
            MyPhsxType = cam.MyPhsxType;

            UseEffective = cam.UseEffective;
            EffectivePos = cam.EffectivePos;
            EffectiveZoom = cam.EffectiveZoom;

            Data = cam.Data;
            PrevPos = cam.PrevPos;
            Speed = cam.Speed;
            Target = cam.Target;
            Zoom = cam.Zoom;
            ScreenWidth = cam.ScreenWidth;
            ScreenHeight = cam.ScreenHeight;
            AspectRatio = cam.AspectRatio;
            TR = cam.TR;
            BL = cam.BL;
            TRCamBound = cam.TRCamBound;
            BLCamBound = cam.BLCamBound;
            Shaking = cam.Shaking;
            ShakingIntensity = cam.ShakingIntensity;
            ShakingSaveZoom = cam.ShakingSaveZoom;
            ShakingSavePos = cam.ShakingSavePos;
            VertexCam = cam.VertexCam;

            // Clone the zone if the cameras are in the same level
            if (!DataOnly)
            {
                if (MyLevel == null)
                    MyLevel = cam.MyLevel;
                if (MyLevel == cam.MyLevel)
                    MyZone = cam.MyZone;
            }
        }

        public void MakeFancyPos()
        {
            if (FancyPos != null) FancyPos.Release();
            FancyPos = new FancyVector2();
            FancyPos.RelVal = Data.Position;

            if (FancyZoom != null) FancyZoom.Release();
            FancyZoom = new FancyVector2();
            FancyZoom.RelVal = Zoom;
        }

        public Vector2 GetSize()
        {
            return new Vector2(GetWidth(), GetHeight());
        }

        public float GetHeight()
        {
            if (LastUpdate != Tools.PhsxCount) Update();
            return TR.Y - BL.Y;
        }

        public float GetWidth()
        {
            if (LastUpdate != Tools.PhsxCount) Update();
            return TR.X - BL.X;
        }

        int LastUpdate = -1;
        public void Update()
        {
            LastUpdate = Tools.PhsxCount;

            TR.X = Data.Position.X + AspectRatio / Zoom.X;
            TR.Y = Data.Position.Y + 1f / Zoom.Y;

            BL.X = Data.Position.X - AspectRatio / Zoom.X;
            BL.Y = Data.Position.Y - 1f / Zoom.Y;

            if (UseEffective)
            {
                EffectiveTR.X = EffectivePos.X + AspectRatio / EffectiveZoom.X;
                EffectiveTR.Y = EffectivePos.Y + 1f / EffectiveZoom.Y;

                EffectiveBL.X = EffectivePos.X - AspectRatio / EffectiveZoom.X;
                EffectiveBL.Y = EffectivePos.Y - 1f / EffectiveZoom.Y;
            }
            else
            {
                EffectivePos = Data.Position;
                EffectiveZoom = Zoom;
                EffectiveTR = TR;
                EffectiveBL = BL;
            }
        }

        public Camera()
        {
            Init(Tools.Device.PresentationParameters.BackBufferWidth,
                 Tools.Device.PresentationParameters.BackBufferHeight);
        }

        public Camera(int width, int height)
        {
            Init(width, height);
        }

        public void Init(int width, int height)
        {
            // FIXED DIMENSIONS!!
            width = 1280;
            height = 720;

            Speed = 30;

            BLCamBound = new Vector2(-1000000, -1000000);
            TRCamBound = new Vector2(1000000, 1000000);

            ScreenWidth = width;
            ScreenHeight = height;
            Data.Position = new Vector2(0, 0);
            ShakingSaveZoom = Zoom = new Vector2(.001f, .001f);
            Offset = new Vector2(width / 2, height / 2);
            AspectRatio = (float)ScreenWidth / (float)ScreenHeight;
        }

        public Camera(Camera cam)
        {
            Clone(cam);
            Speed = cam.Speed;

            ScreenHeight = cam.ScreenHeight;
            ScreenWidth = cam.ScreenWidth;
            Data.Position = cam.Data.Position;
            Zoom = cam.Zoom;
            Offset = cam.Offset;
            AspectRatio = cam.AspectRatio;
            TR = cam.TR;
            BL = cam.BL;

            BLCamBound = cam.BLCamBound;
            TRCamBound = cam.TRCamBound;
        }

        public Vector2 ShakeOffset, OscillateOffset;
        int ShakeCount, ShakeLength;
        public void StartShake() { StartShake(1, -1); }
        public void StartShake(float Intensity) { StartShake(Intensity, -1); }
        public void StartShake(float Intensity, int Length) { StartShake(Intensity, Length, true); }
        public void StartShake(float Intensity, int Length, bool Sound)
        {
            if (Sound)
                Tools.Sound("Rumble_Short").Play();

            ShakeCount = 0;
            ShakeLength = Length;

            if (!Shaking)
            {
                Shaking = true;
                ShakingSaveZoom = Zoom;
                ShakingSavePos = Data.Position;
            }

            ShakingIntensity = Intensity;
        }

        public void EndShake()
        {
            Shaking = false;
            Zoom = ShakingSaveZoom;
            Data.Position = ShakingSavePos;
            ShakeOffset = Vector2.Zero;
        }

        public bool OnScreen(Vector2 pos) { return OnScreen(pos, new Vector2(200, 600)); }
        public bool OnScreen(Vector2 pos, Vector2 GraceSize)
        {
            if (pos.X > TR.X + GraceSize.X) return false;
            if (pos.X < BL.X - GraceSize.X) return false;
            if (pos.Y > TR.Y + GraceSize.Y) return false;
            if (pos.Y < BL.Y - GraceSize.Y) return false;
            return true;
        }
        public bool OnScreen(Vector2 pos, float GraceSize)
        {
            if (pos.X > TR.X + GraceSize) return false;
            if (pos.X < BL.X - GraceSize) return false;
            if (pos.Y > TR.Y + GraceSize) return false;
            if (pos.Y < BL.Y - GraceSize) return false;
            return true;
        }
        public bool OnScreen(Vector2 bl, Vector2 tr, float GraceSize)
        {
            if (bl.X > TR.X + GraceSize) return false;
            if (tr.X < BL.X - GraceSize) return false;
            if (bl.Y > TR.Y + GraceSize) return false;
            if (tr.Y < BL.Y - GraceSize) return false;
            return true;
        }


        Vector4 GetVertex()
        {
            if (UseEffective)
                return new Vector4(EffectivePos.X, EffectivePos.Y, EffectiveZoom.X, EffectiveZoom.Y);
            else
                return new Vector4(Data.Position.X, Data.Position.Y, Zoom.X, Zoom.Y);
        }

        public void SetVertexCamera()
        {
            VertexCam = GetVertex();
            Tools.EffectWad.SetCameraPosition(VertexCam);
        }

        public void SetVertexZoom(float factor) { SetVertexZoom(new Vector2(factor)); }
        public void SetVertexZoom(Vector2 factor)
        {
            VertexCam = GetVertex();
            VertexCam.Z *= factor.X;
            VertexCam.W *= factor.Y;
            
            Tools.EffectWad.SetCameraPosition(VertexCam);
        }

        public void SetVertexCamera(Vector2 shift, Vector2 factor)
        {
            VertexCam = GetVertex();
            VertexCam.X += shift.X;
            VertexCam.Y += shift.Y;
            VertexCam.Z *= Zoom.X;
            VertexCam.W *= Zoom.Y;

            Tools.EffectWad.SetCameraPosition(VertexCam);
        }

        public Vector2 HoldZoom;
        public void SetToDefaultZoom()
        {
            HoldZoom = Zoom;
            Zoom = new Vector2(.001f, .001f);
            SetVertexCamera();
            Update();
        }

        public void RevertZoom()
        {
            Zoom = HoldZoom;
            SetVertexCamera();
            Update();
        }

        public Vector2 CurVel()
        {
            //return Data.Position - PrevPos;
            return PrevPos - PrevPrevPos;
        }

        public static bool DisableOscillate = false;

        public enum PhsxType { Fixed, SideLevel_Right, SideLevel_Up, SideLevel_Down, Center, SideLevel_Up_Relaxed };
        public PhsxType MyPhsxType = PhsxType.SideLevel_Right;
        float t;
        public void PhsxStep()
        {
            Vector2 CurPos = Data.Position;
            Vector2 CurZoom = Zoom;
            if (Shaking)
            {
                Data.Position -= ShakeOffset;
                //Zoom = ShakingSaveZoom;
            }
            if (Oscillating && !DisableOscillate)
            {
                //OscillateOffset = new Vector2(0, 40 * (float)Math.Sin(.95f * Tools.t));
                OscillateOffset = new Vector2(0, 40 * (float)Math.Sin(.95f * t)); t += Tools.dt;
                Data.Position -= OscillateOffset;
            }

            if (SpeedVel != 0)
            {
                float dif = TargetSpeed - Speed;
                Speed += Math.Min(SpeedVel, Math.Abs(dif)) * Math.Sign(dif);
            }

            switch (MyPhsxType)
            {
                case PhsxType.Fixed:
                    Fixed_PhsxStep();
                    break;

                case PhsxType.SideLevel_Up:
                case PhsxType.SideLevel_Up_Relaxed:
                case PhsxType.SideLevel_Down:
                case PhsxType.SideLevel_Right:
                    SideLevel_PhsxStep();
                    break;

                case PhsxType.Center:
                    Center_PhsxStep();
                    break;
            }

            if (Shaking)
            {
                ShakeCount++;
                Vector2 change = Data.Position + OscillateOffset + ShakeOffset - CurPos;
                ShakingSavePos += change;
                Data.Position = Data.Position + OscillateOffset + ShakeOffset;
                Update();
                
                // Fade out the intensity
                if (ShakeCount > ShakeLength - 20)
                    ShakingIntensity *= .95f;

                // Check for shake end
                if (ShakeLength > 0 && ShakeCount >= ShakeLength)
                {                    
                    EndShake();
                    Update();
                }
                else
                {
                    if (Tools.TheGame.DrawCount % 2 == 0)
                    {
                        Data.Position = ShakingSavePos;
                        ShakeOffset.X = ShakingIntensity * Tools.Rnd.Next(-70, 70);
                        ShakeOffset.Y = ShakingIntensity * Tools.Rnd.Next(-70, 70);
                        Data.Position += ShakeOffset;
                        Update();
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (PlayerManager.Get(i).Exists && PlayerManager.Get(i).IsAlive)
                        {
                            Tools.SetVibration((PlayerIndex)i, 1f, 1f, 10);
                        }
                    }
                }
            }
            else
                if (Oscillating && !DisableOscillate)
                {
                    Data.Position = Data.Position + OscillateOffset;
                    Update();
                }


            //PrevPos = HoldPrevPos;
            PrevPrevPos = PrevPos;
            PrevPos = Data.Position;
        }

        public void SetPhsxType(PhsxType NewType)
        {
            if (NewType != MyPhsxType)
            {
                switch (NewType)
                {
                }

                MyPhsxType = NewType;
            }
        }

        public void Fixed_PhsxStep()
        {
            if (MyZone != null)
                MyZone.SetZoom(this);

            if (FancyPos != null)
                Data.Position = FancyPos.Update();                

            if (FancyZoom != null)
                Zoom = FancyZoom.Update();

            Update();
        }

        public void Center_PhsxStep()
        {
            //if (MyZone != null)
              //  Target = MyZone.Core.Data.Position;

            Vector2 vel = CurVel();
            Vector2 dif = Target - Data.Position;
            Vector2 TargetVel = dif; TargetVel.Normalize();
            
            float EffectiveSpeed = Vector2.Dot(vel, TargetVel);
            if (15 * EffectiveSpeed > dif.Length() || dif.Length() < 2 * Speed)
                vel = 1f / 16f * dif;
            else
            {
                TargetVel *= Speed;
                vel += .08f * (TargetVel - vel);
            }

            Data.Position += vel;

            Update();
        }

        /// <summary>
        /// Forces the camera's Target.X to be within a given box.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="BoxSize"></param>
        /// <param name="BoxShift"></param>
        Vector2 BoxLimit_X(Vector2 Pos, Vector2 BoxCenter, Vector2 BoxSize, Vector2 BoxShift)
        {
            if (Pos.X < BoxCenter.X - BoxSize.X + BoxShift.X)
                Pos.X = BoxCenter.X - BoxSize.X + BoxShift.X;
            if (Pos.X > BoxCenter.X + BoxSize.X + BoxShift.X)
                Pos.X = BoxCenter.X + BoxSize.X + BoxShift.X;

            return Pos;
        }

        /// <summary>
        /// Forces the camera's Target.Y to be within a given box.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="BoxSize"></param>
        /// <param name="BoxShift"></param>
        Vector2 BoxLimit_Y(Vector2 Pos, Vector2 BoxCenter, Vector2 BoxSize, Vector2 BoxShift)
        {
            if (Pos.Y < BoxCenter.Y - BoxSize.Y + BoxShift.Y)
                Pos.Y = BoxCenter.Y - BoxSize.Y + BoxShift.Y;
            if (Pos.Y > BoxCenter.Y + BoxSize.Y + BoxShift.Y)
                Pos.Y = BoxCenter.Y + BoxSize.Y + BoxShift.Y;

            return Pos;
        }

        Vector2 BoxLimitLeft(Vector2 Pos, Vector2 BoxCenter, Vector2 BoxSize, Vector2 BoxShift)
        {
            if (Pos.X < BoxCenter.X - BoxSize.X + BoxShift.X)
                Pos.X = BoxCenter.X - BoxSize.X + BoxShift.X;

            return Pos;
        }

        Vector2 BoxLimitDown(Vector2 Pos, Vector2 BoxCenter, Vector2 BoxSize, Vector2 BoxShift)
        {
            if (Pos.Y < BoxCenter.Y - BoxSize.Y + BoxShift.Y)
                Pos.Y = BoxCenter.Y - BoxSize.Y + BoxShift.Y;

            return Pos;
        }
        Vector2 BoxLimitUp(Vector2 Pos, Vector2 BoxCenter, Vector2 BoxSize, Vector2 BoxShift)
        {
            if (Pos.Y > BoxCenter.Y + BoxSize.Y + BoxShift.Y)
                Pos.Y = BoxCenter.Y + BoxSize.Y + BoxShift.Y;

            return Pos;
        }


        public void SideLevel_PhsxStep()
        {
            Vector2 TR, BL;
            TR = new Vector2(-10000000, -10000000);
            BL = new Vector2(10000000, 10000000);

            MaxPlayerSpeed = Vector2.Zero;

            int Count = 0;
            float TotalWeight = 0;
            Vector2 BobsCenter = Vector2.Zero;
            foreach (Bob bob in MyLevel.Bobs)
            {
                if (PlayerManager.IsAlive(bob.MyPlayerIndex) && bob.AffectsCamera && (!bob.DoNotTrackOffScreen || OnScreen(bob.Core.Data.Position)) || MyLevel.PlayMode != 0)
                {
                    //MaxPlayerSpeed = Math.Max(MaxPlayerSpeed, bob.Core.Data.Velocity.Length());
                    MaxPlayerSpeed = Vector2.Max(MaxPlayerSpeed, Tools.Abs(bob.Core.Data.Velocity));

                    BobsCenter += bob.Core.Data.Position;
                    
                    //TR.X = Math.Max(TR.X, bob.Core.Data.Position.X);
                    //TR.Y = Math.Max(TR.Y, bob.Core.Data.Position.Y);

                    TR = Vector2.Max(TR, bob.Core.Data.Position);
                    BL = Vector2.Min(BL, bob.Core.Data.Position);

                    Count++;
                    TotalWeight += bob.CameraWeight;
                    bob.CameraWeight = Tools.Restrict(0, 1, bob.CameraWeight + bob.CameraWeightSpeed);
                }
            }

            
            if (FollowCenter)
            {
                if (Count > 0)
                    //BobsCenter /= Count;
                    BobsCenter /= TotalWeight;
                else
                    BobsCenter = Data.Position;
            }
            

            Vector2 BoxSize, BoxShift = Vector2.Zero;
            Vector2 Pos = TR;
            if (FollowCenter) Pos = BobsCenter;

            if (Count > 0)
            {
                if (RocketManCamera)
                {
                    BoxSize = new Vector2(350, 1000);
                    if (FollowCenter && Count > 1) BoxSize.X = 50;
                    float px = MyZone.GetProjectedPos(Data.Position).X;
                    float cx = MyZone.GetProjectedPos(Pos).X;
                    float tx;

                    if (px < cx - BoxSize.X)
                    {
                        tx = cx - BoxSize.X;
                        Target = MyZone.FromProjected(new Vector2(tx, 0));
                    }
                    if (px > cx + BoxSize.X)
                    {
                        tx = cx + BoxSize.X;
                        Target = MyZone.FromProjected(new Vector2(tx, 0));
                    }
                }
                else
                {
                    // Old
                    //Box = new Vector2(650, 1000);

                    // Good for long rocketman
                    // BoxSize = new Vector2(325, 1000); 
                    // BoxShift = new Vector2(800, 0);


                    if (MyPhsxType == PhsxType.SideLevel_Up)
                    {
                        //BoxSize = new Vector2(450, 200);
                        //BoxShift = new Vector2(250, 275);
                        BoxSize = new Vector2(450, 330);
                        BoxShift = new Vector2(250, 400);
                    }
                    else if (MyPhsxType == PhsxType.SideLevel_Up_Relaxed)
                    {
                        BoxSize = new Vector2(450, 350);
                        BoxShift = new Vector2(250, 160);
                    }
                    else if (MyPhsxType == PhsxType.SideLevel_Down)
                    {
                        BoxSize = new Vector2(450, 250);
                        BoxShift = new Vector2(250, -800);
                    }
                    else //if (MyPhsxType == PhsxType.SideLevel_Right)
                    {
                        BoxSize = new Vector2(450, 250);//1000);
                        BoxShift = new Vector2(250, 250);
                    }

                    if (FollowCenter && Count > 1) BoxSize.X = 50;

                    // Single player: keep player nearly centered
                    if (Count <= 1)
                    {
                        if (MyPhsxType == PhsxType.SideLevel_Right)
                            Target = BoxLimit_X(Data.Position, Pos, BoxSize, BoxShift);
                        else
                            Target = BoxLimit_Y(Data.Position, Pos, BoxSize, BoxShift);
                    }
                    // Multiplayer: if all together, stay centered. Otherwise allow leading player to push ahead.
                    else
                    {
                        if (MyPhsxType == PhsxType.SideLevel_Right)
                            Target = BoxLimitLeft(Data.Position, BL, BoxSize, BoxShift);
                        else if (MyPhsxType == PhsxType.SideLevel_Up)
                            Target = BoxLimitDown(Data.Position, BL, BoxSize, BoxShift);
                        else if (MyPhsxType == PhsxType.SideLevel_Down)
                            Target = BoxLimitUp(Data.Position, BL, BoxSize, BoxShift);

                        Vector2 Lead = Target;
                        if (MyPhsxType == PhsxType.SideLevel_Down)
                        {
                            Lead = BL;

                            BoxSize = new Vector2(0, 500);
                            BoxShift = new Vector2(0, -250);
                        }
                        else if (MyPhsxType == PhsxType.SideLevel_Up)
                        {
                            Lead = TR;

                            BoxSize = new Vector2(0, 500);
                            BoxShift = new Vector2(0, 100);
                        }
                        else if (MyPhsxType == PhsxType.SideLevel_Up_Relaxed)
                        {
                            Lead = TR;

                            BoxSize = new Vector2(0, 600);
                            BoxShift = new Vector2(0, 0);
                        }
                        else if (MyPhsxType == PhsxType.SideLevel_Right)
                        {
                            Lead = TR;

                            BoxSize = new Vector2(650, 0);
                            BoxShift = new Vector2(0, 0);
                        }

                        if (MyPhsxType == PhsxType.SideLevel_Right)
                            Target = BoxLimit_X(Target, Lead, BoxSize, BoxShift);
                        else
                            Target = BoxLimit_Y(Target, Lead, BoxSize, BoxShift);
                    }
                }
            }

            if (Count > 0)
            {
                if (MyZone != null)
                {
                    if (MyZone.FreeY)
                    {
                        //BoxSize = new Vector2(650, 300);
                        BoxSize = new Vector2(650, 100);
                        if (FollowCenter && Count > 1) BoxSize.Y = 50;
                        if (Data.Position.Y < Pos.Y - BoxSize.Y)
                            Target.Y = Pos.Y - BoxSize.Y;
                        if (Data.Position.Y > Pos.Y + BoxSize.Y)
                            Target.Y = Pos.Y + BoxSize.Y;
                    }

                    MyZone.Enforce(this);
                }
            }

            //float CurMaxSpeed = Math.Max(Speed, 1.05f * MaxPlayerSpeed);
            //CurMaxSpeed = Math.Max(CurMaxSpeed, CurVel().X);

            Vector2 CurMaxSpeed = Vector2.Max(new Vector2(Speed), 1.05f * MaxPlayerSpeed);
            //CurMaxSpeed.Y = Math.Max(CurMaxSpeed.Y, 
            CurMaxSpeed = Vector2.Max(CurMaxSpeed, CurVel());
            Data.Position.X += Math.Sign(Target.X - Data.Position.X) * Math.Min(.15f * Math.Abs(Target.X - Data.Position.X), CurMaxSpeed.X);
            Data.Position.Y += Math.Sign(Target.Y - Data.Position.Y) * Math.Min(.15f * Math.Abs(Target.Y - Data.Position.Y), CurMaxSpeed.Y);


            Update();
        }
    }
}

/* REMNANT: autozoom
if (MyZone != null && MyZone.FreeY)
if (MyLevel.Bobs.Count > 1)
{
    float zoom = Math.Max(TR.X - BL.X, (TR.Y - BL.Y) * AspectRatio) + 450;
    zoom /= 2850;
    zoom = (float)Math.Pow(zoom, 1.7f);
    zoom = Math.Max(1, Math.Min(1.5f, zoom));
    Zoom = new Vector2(.001f, .001f) / zoom;
}
*/