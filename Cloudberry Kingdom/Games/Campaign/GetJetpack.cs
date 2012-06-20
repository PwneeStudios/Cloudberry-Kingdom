using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_GetJetpack : WorldMap
    {
        static int DelayToDrop, DelayToEnter;
        //static Vector2 DropPos, DropVel;
        static void SetParams()
        {
            DelayToEnter = 350;
            DelayToDrop = 95;

            DelayToDrop = 130;
        }

        public Campaign_GetJetpack(BackgroundTemplate type, EzSong song)
            : base(false)
        {
            SetParams();

            Data = Campaign.Data;
            WorldName = "GetJetpack";

            Init("Doom\\GetJetpack.lvl");
            MyLevel.PreventReset = true;

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            //Cam.MyZone.CameraType = Camera.PhsxType.SideLevel_Up_Relaxed;
            Cam.MyZone.CameraType = Camera.PhsxType.SideLevel_Up;
            MakeBackground(type);

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            // Players
            SetHeroType(BobPhsxNormal.Instance);

            MakePlayers();

            // Position players
            EnterFrom(FindDoor("Enter"), DelayToEnter);
            CinematicToDo(360, () => MyLevel.PreventReset = false);

            // Princess
            //Campaign.SpecifiedPrincessPos = MyLevel.FindBlock("Princess").Pos;
            //Campaign.SpecifiedBlobPos = Campaign.SpecifiedPrincessPos + new Vector2(20, -1450);
            //Campaign.SpecifiedVel = new Vector2(0, 23);
            //Campaign.LevelWithPrincess(MyLevel, false, Campaign.PrincessPos.Specified);

            CinematicToDo(40, () =>
            {
                PhsxData data;
                Vector2 point = MyLevel.FindBlock("Princess").Pos + new Vector2(332, 0);
                data.Position = point + new Vector2(200, -1400);
                data.Velocity = (point - data.Position); data.Velocity.Normalize(); data.Velocity *= 23;
                data.Acceleration = new Vector2(.02f, 0);
                Campaign.LinearCarry(MyLevel, data);

                CinematicToDo(DelayToDrop, () => AddJetpack());
            });

            PhsxStepsToDo += 2;

            // Exit sign
            Sign sign = new Sign(false);
            sign.PlaceAt(Doors["Exit"].GetTop());
            MyLevel.AddObject(sign);

            // Exit door
            Doors["Exit"].SetLock(false, true, false);
            Doors["Exit"].NoNote = true;
            Doors["Exit"].OnOpen += ReturnToWorldMap;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        void AddJetpack()
        {
            var princess = MyLevel.Objects.Find(obj => obj is PrincessBubble);
            if (princess == null) return;

            Vector2 Pos = princess.Pos;
            AddPowerups(BobPhsxJetman.Instance, "Jetpack", Pos, new Vector2(0, 0), new Vector2(-8.5f, 0), new Vector2(1.2f, 0), Tutorial, p => p.Friction = .99f);
        }

        void Tutorial()
        {
            //CinematicToDo(115, () =>
            //{
            //    Arrow arrow = new Arrow();
            //    arrow.SetOrientation(Arrow.Orientation.Right);
            //    arrow.Move(Cam.Pos + new Vector2(30, -40));
            //    arrow.PointTo(Cam.Pos + new Vector2(0, 1));
            //    AddGameObject(arrow);

            //    GUI_Text text = new GUI_Text("Hold " + ButtonString.Jump(85) + " to thrust", Cam.Pos + new Vector2(0, 500));
            //    AddGameObject(text);

            //    CinematicToDo(165, () =>
            //    {
            //        arrow.Release();
            //        text.Kill();
            //    });
            //});
        }
    }
}