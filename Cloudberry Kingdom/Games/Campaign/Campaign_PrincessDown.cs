using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Goombas;
using Drawing;

namespace CloudberryKingdom
{
    public static class Campaign_PrincessDown
    {
        static Vector2 PrincessPos, ExplodePos, PrincessVel, PrincessAcc;
        static int DelayToPrincess, DoorOpenWait, CanResetWait;
        static float PrincessAngleSpeed;
        static bool Dramatic;
        static int ShakeStart, Explode;
        static int StartBlobs, EndBlobs;
        public static int SetBack = 200 + 172;

        static void SetParams()
        {
            Dramatic = true;

            CanResetWait = 200;
            DoorOpenWait = 200;

            ShakeStart = -1;
            Explode = -1;
            ExplodePos = new Vector2(0, -1000);

            DelayToPrincess = 49;
            PrincessPos = new Vector2(300, 1050);
            PrincessVel = new Vector2(7.3f, -30.5f);//-36.5f);
            PrincessAcc = new Vector2(-.15f, 0);//-.05f);
            PrincessAngleSpeed = 6.65f;

            StartBlobs = DelayToPrincess + 22;
            EndBlobs = 140;
        }
        public static void PrincessDown(Level lvl)
        {
            GameData game = lvl.MyGame;
            SetParams();
            Tools.SongWad.SuppressNextInfoDisplay = true;

            game.MyLevel.StartDoor.SetLock(true, true, false);

            lvl.PreventReset = true;
            lvl.PreventHelp = true;
            game.AddToDo(() =>
            {
                game.HideBobs();

                // Create the princess
                game.CinematicToDo(DelayToPrincess, () =>
                {
                    var princess = new PrincessBubble(lvl.LeftMostCameraZone().Start + PrincessPos);
                    princess.Core.RemoveOnReset = true;
                    princess.Core.Data.Velocity = PrincessVel;
                    princess.Core.Data.Acceleration = PrincessAcc;
                    princess.RotateSpeed = PrincessAngleSpeed;
                    princess.MyState = PrincessBubble.State.Integrate;
                    lvl.AddObject(princess);
                });
                

                // If there's a start door then enter through it
                if (lvl.StartDoor != null)
                {
                    if (Dramatic)
                    {
                        game.DramaticEntry(lvl.StartDoor, DoorOpenWait);
                        game.CinematicToDo(DoorOpenWait + CanResetWait,
                            () => lvl.PreventReset = lvl.PreventHelp = false);
                    }
                    else
                    {
                        game.EnterFrom(lvl.StartDoor, DoorOpenWait - 20);
                        game.CinematicToDo(DoorOpenWait + CanResetWait - 60,
                            () => lvl.PreventReset = lvl.PreventHelp = false);
                    }
                }

                // Rumble
                game.CinematicToDo(ShakeStart, () => Tools.CurCamera.StartShake(.5f, 36));

                // Piece explosion
                game.CinematicToDo(Explode, () =>
                    ParticleEffects.PiecePopFart(game.MyLevel, game.Cam.Pos + ExplodePos));


                // Cross blobs
                int count = 0;
                game.WaitThenAddToToDo(1, () =>
                {
                    count++;
                    if (count > StartBlobs && count < EndBlobs && count % 2 == 0)
                        MakeBlob(game.MyLevel);

                    return count >= 240;
                });
            });

            game.PhsxStepsToDo += 3;
        }

        public static void MakeBlob(Level level)
        {
            // Make blob
            Goomba blob = (Goomba)level.Recycle.GetObject(ObjectType.FlyingBlob, false);
            var pos = new Vector2(level.Rnd.RndFloat(level.MainCamera.BL.X, level.MainCamera.TR.X),
                                   level.MainCamera.TR.Y + 500);
            blob.Init(pos, level);

            blob.NeverSkip = true;

            blob.DeleteOnDeath = true;
            blob.Core.RemoveOnReset = true;
            blob.RemoveOnArrival = true;

            blob.MaxVel = 36;// 42.5f;
            blob.MaxAcc = 3.6f;

            blob.MyPhsxType = Goomba.PhsxType.ToTarget;
            blob.Target = blob.Core.Data.Position - new Vector2(0, 17000);
            float acc = Math.Sign(blob.Pos.X - level.MainCamera.Pos.X);
            if (acc == 0) acc = 1;
            blob.Core.Data.Acceleration.X = -acc * .95f;

            level.AddObject(blob);
        }
    }
}