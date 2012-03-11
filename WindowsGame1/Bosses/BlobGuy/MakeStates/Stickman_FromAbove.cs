using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Goombas;
using CloudberryKingdom;

namespace CloudberryKingdom
{
    public partial class BlobGuy
    {
        public Action OnLand, OnExplode;
        public bool Exploded;

        public ScenePart Make_Stickman_FromAbove(ScenePart Next)
        {
            ScenePart State = new ScenePart();
            State.MyBegin = delegate()
            {
                SnapForm = true;

                //// Horizontal laser (Abusive Campaign only)
                //Laser Laser_1, Laser_2, Laser_3;
                //if (false)//Campaign.Index == 2)
                //{
                //    Laser_1 = new Laser();
                //    Core.MyLevel.AddObject(Laser_1);
                //    Game.CinematicToDo(() =>
                //    {
                //        float Height = Tools.Periodic(1450, -500, 980, GameData.CurItemStep);
                //        Laser_1.SetLine(cam.Pos + new Vector2(0, Height), 0);
                //        return false;
                //    });
                //}

                // Vertical lasers (Hardcore Campaign and up)
                if (Campaign.Index == 2 || Campaign.Index == 3)
                    MakeLasers();


                //// Vertical lasers (Hardcore Campaign and up)
                //if (Campaign.Index == 4)
                //{
                //    Laser.DeleteOnTouch = true;
                //    for (int i = 0; i < 2000; i++)
                //    {
                //        var laser = new Laser(true);
                //        laser.Core.RemoveOnReset = true;
                //        Core.MyLevel.AddObject(laser);
                //        laser.SetLine(MyLevel.Rnd.RndPos(Tools.CurCamera.BL + new Vector2(400), Tools.CurCamera.TR - new Vector2(500)), MyLevel.Rnd.RndFloat(0, 360));

                //        laser.Period = 360;
                //        laser.Offset = MyLevel.Rnd.Rnd.Next(laser.Period);
                //        laser.WarnDuration = 40;
                //        laser.Duration = 60;
                //    }
                //}


                //MyObject.EnqueueAnimation("Normal", 0, true, true);
                MyObject.Read(3, 0);
                MyObject.Play = false;

                Core.Data.Position = cam.Data.Position + new Vector2(0, cam.GetHeight());
                Core.Data.Velocity = Vector2.Zero;
            };

            State.MyPhsxStep = delegate(int Step)
            {
                StickmanToTargets(MyTargets);

                if (Core.Data.Position.Y > Core.StartData.Position.Y)
                {
                    Core.Data.Position.Y -= 65;
                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived || SnapForm)
                            blob.Core.Data.Position.Y -= 65;
                }
                else
                {
                    if (!cam.Shaking)
                        Nova(Core.Data.Position, Vector2.Zero, 10);

                    Shake();
                    if (OnLand != null) OnLand(); OnLand = null;
                    Core.Data.Position.Y = Core.StartData.Position.Y;
                }

                if (Step > 69)
                //if (Step > 99)
                {
                    if (Next != null)
                    {
                        CurState = Next;//Stickman_Normal;
                        CurState.Begin();
                    }
                    else
                        CurState = null;
                }
                //{
                //    CurState = Ball_Bounce;
                //    CurState.Begin();
                //}


                if (Step < 2)
                {
                    foreach (Goomba blob in Blobs)
                        if (blob.HasArrived || SnapForm)
                            blob.Core.Data.Position = blob.Target;
                }
            };

            return State;
        }

        private void MakeLasers()
        {
            foreach (IObject obj in Core.MyLevel.Objects)
                if (obj is Laser)
                    return;
                    //obj.CollectSelf();

            Laser Laser_1, Laser_2, Laser_3;

            //if (IsPrincess)
            //{
            //    Laser_1 = new Laser();
            //    Core.MyLevel.AddObject(Laser_1);
            //    Game.CinematicToDo(() =>
            //    {
            //        float Height = Tools.Periodic(1450, -500, 980, GameData.CurItemStep);
            //        Laser_1.SetLine(cam.Pos + new Vector2(0, Height), 0);
            //        return false;
            //    });
            //}


            Laser_2 = new Laser();
            Core.MyLevel.AddObject(Laser_2);
            Laser_3 = new Laser();
            Core.MyLevel.AddObject(Laser_3);

            Game.CinematicToDo(() =>
            {
                float Loc;

                //float Left = -2800, Right = 1100;
                //int Period = 980;
                float Left = -3000, Right = 1350;
                int Period = 780;
                if (Campaign.Index == 2) Period = 960;

                Loc = Tools.Periodic(Left, Right, Period, GameData.CurItemStep);
                Laser_2.SetLine(cam.Pos + new Vector2(Loc, 0), 90);

                if (GameData.CurItemStep > Period / 2)
                {
                    Loc = Tools.Periodic(Right, Left, Period, GameData.CurItemStep);
                    Laser_3.SetLine(cam.Pos - new Vector2(Loc, 0), 90);
                }
                else
                    Laser_3.SetLine(cam.Pos + new Vector2(10000, 0), 90);

                if (GameData.CurItemStep % Period < Period / 2 - 85)
                {
                    Laser_2.AlwaysOn = true; Laser_2.AlwaysOff = false;
                }
                else
                {
                    Laser_2.AlwaysOn = false; Laser_2.AlwaysOff = true;
                }

                if ((GameData.CurItemStep + Period / 2) % Period < Period / 2 - 85)
                {
                    Laser_3.AlwaysOn = true; Laser_3.AlwaysOff = false;
                }
                else
                {
                    Laser_3.AlwaysOn = false; Laser_3.AlwaysOff = true;
                }

                return false;
            });
        }
    }
}