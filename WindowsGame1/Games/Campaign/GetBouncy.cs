using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using Drawing;

namespace CloudberryKingdom
{
    public class Campaign_GetBouncy : WorldMap
    {
        public Campaign_GetBouncy(EzSong song)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "GetBouncy";

            Init("Doom\\GetBouncy.lvl");

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(.8f, "Center");
            MakeBackground(BackgroundType.Night);

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            // Players
            SetHeroType(BobPhsxNormal.Instance);

            MakePlayers();

            Vector2 buttonpos = MyLevel.FindBlock("Powerup").Pos;
            AddPowerups(BobPhsxBouncy.Instance, "Bouncy", buttonpos, new Vector2(-200, 0), Vector2.Zero, Vector2.Zero, () =>
            {
                CinematicToDo(75, () =>
                {
                    Arrow arrow = new Arrow();
                    arrow.SetOrientation(Arrow.Orientation.Right);
                    arrow.Move(Cam.Pos + new Vector2(30, -40));
                    arrow.PointTo(Cam.Pos + new Vector2(0, 1));
                    AddGameObject(arrow);

                    GUI_Text text = new GUI_Text("Hold " + ButtonString.Jump(85) + " to bounce higher", Cam.Pos + new Vector2(-170, 430));
                    AddGameObject(text);

                    CinematicToDo(165, () =>
                    {
                        arrow.Release();
                        text.Kill();
                    });
                });
            }, null);

            // Position players
            foreach (Bob bob in MyLevel.Bobs)
                bob.Immortal = false;
            SetSpawnPoint(MyLevel.FindIObject("Enter").Pos, new Vector2(30, 0));

            MyLevel.PreventReset = true;
            EnterFrom(FindDoor("Enter"), 100);
            CinematicToDo(150, () => MyLevel.PreventReset = false);

            // Exit sign
            //Sign sign = new Sign(false);
            //sign.PlaceAt(Doors["Exit"].GetTop());
            //MyLevel.AddObject(sign);

            // Exit door
            Doors["Exit"].SetLock(false, true, false);
            Doors["Exit"].NoNote = true;
            Doors["Exit"].OnOpen += ReturnToWorldMap;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }
    }
}