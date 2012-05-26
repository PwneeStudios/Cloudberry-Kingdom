using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class CameraZone : ZoneTrigger
    {
        public bool Activated;
        public bool SnapNext;

        public int Priority;

        public float Zoom;
        public Vector2 Start, End;

        public bool FreeY;
        public float FreeY_MaxY, FreeY_MinY;

        //public float CameraSpeedVel = .5f;
        //public float CameraTargetSpeed = 30f;
        public float CameraSpeed; //30 for normal // 45 for the wheel
        public bool SetCameraSpeed;
        public Camera.PhsxType CameraType;

        public override void MakeNew()
        {
            base.MakeNew();

            Zoom = 1;
            FreeY_MaxY = 1100;  FreeY_MinY = -1100;
            
            CameraSpeed = 15;
            SetCameraSpeed = true;

            CameraType = Camera.PhsxType.SideLevel_Right;

            Core.MyType = ObjectType.CameraZone;
            Core.GenData.Used = true;

            Core.ResetOnlyOnReset = true;

            Activated = false;

            Priority = 0;
        }

        public CameraZone()
        {
            MakeNew();
        }

        new public void Init(Vector2 center, Vector2 size)
        {
            base.Init(center, size);

            MyContainsEvent = delegate(ZoneTrigger trig)
            {
                CameraZone CamZone = trig as CameraZone;

                Camera cam = CamZone.Core.MyLevel.MainCamera;
                if (cam.ZoneLocked ||
                    cam.MyPhsxType == Camera.PhsxType.Fixed) return;

                if (cam.MyZone == null ||
                    !cam.MyZone.Activated ||
                    cam.MyZone.Priority < CamZone.Priority)
                {
                    switch (CameraType)
                    {
                        case Camera.PhsxType.Center:
                            if (cam.MyPhsxType != CameraType)
                                cam.Target = Core.Data.Position;
                            break;
                    }

                    CamZone.Activated = true;

                    if (cam.MyZone == null)
                        CamZone.SnapNext = true;

                    //cam.TargetSpeed = CameraTargetSpeed;
                    //cam.SpeedVel = CameraSpeedVel;
                    //cam.Speed = cam.CurVel().Length();
                    if (SetCameraSpeed)
                        cam.Speed = CameraSpeed;
                        
                    cam.MyPhsxType = CameraType;

                    cam.MyZone = CamZone;
                }
            };
        }

        public override void PhsxStep()
        {
 	         base.PhsxStep();             
        }

        public override void PhsxStep2()
        {
            base.PhsxStep2();

            Activated = false;
        }

        public Vector2 GetProjectedPos(Vector2 x)
        {
            Vector2 pos = x;

            Vector2 Tangent = End - Start;
            float Length = Tangent.Length();

            float d;
            if (Length < 1)
            {
                d = 0;
                Length = 1;
            }
            else
            {
                d = Vector2.Dot(pos - Start, Tangent) / Length;
            }

            return new Vector2(d, 0);
        }

        public Vector2 FromProjected(Vector2 x)
        {
            Vector2 Tangent = End - Start;
            float Length = Tangent.Length();

            return Start + x.X * Tangent / Length;
        }

        public void SetZoom(Camera cam)
        {
            cam.Zoom = Zoom * new Vector2(.001f, .001f);
        }

        public void Enforce(Camera cam)
        {
            SetZoom(cam);

            Vector2 pos = cam.Target;

            Vector2 Tangent = End - Start;
            float Length = Tangent.Length();
            
            float d;
            if (Length < 1)
            {
                d = 0;
                Length = 1;
            }
            else
            {
                d = Vector2.Dot(pos - Start, Tangent) / Length;
                d = Math.Max(0, Math.Min(Length, d));
            }

            pos = Start + d * (End - Start) / Length;

            if (!FreeY)
                cam.Target = pos;
            else
            {
                cam.Target.X = pos.X;
                cam.Target.Y = Math.Max(pos.Y + FreeY_MinY + cam.GetHeight() / 2, cam.Target.Y);
                cam.Target.Y = Math.Min(pos.Y + FreeY_MaxY - cam.GetHeight() / 2, cam.Target.Y);
                //if (cam.Target.Y - cam.GetHeight() / 2 < pos.Y - 1000)
                  //  cam.Target.Y = pos.Y - 1000 + cam.GetHeight() / 2;
            }

            if (SnapNext)
            {
                cam.Data.Position = cam.Target;
                SnapNext = false;
            }
        }


        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            Start += shift;
            End += shift;
        }

        public override void Clone(ObjectBase A)
        {
            base.Clone(A);

            CameraZone ZoneA = A as CameraZone;

            Start = ZoneA.Start;
            End = ZoneA.End;

            Activated = ZoneA.Activated;

            Priority = ZoneA.Priority;

            FreeY = ZoneA.FreeY;
        }
    }
}