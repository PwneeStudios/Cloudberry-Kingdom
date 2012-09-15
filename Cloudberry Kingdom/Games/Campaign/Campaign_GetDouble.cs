using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_GetDouble : WorldMap
    {
        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ButtonCheck.State(Microsoft.Xna.Framework.Input.Keys.C).Pressed)
            {
                //var d = new Doodad();
                var d = (Doodad)Recycle.GetObject(ObjectType.Doodad, false);
                d.MakeNew();
                d.Init(MyLevel.Bobs[0].Pos, new Vector2(80));
                d.Core.EditorCode1 = "Coin";
                d.IsActive = true;
                d.Core.Show = true;
                d.Core.DrawLayer = 5;
                MyLevel.AddBlock(d);
            }
        }

        public Campaign_GetDouble(EzSong song)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "GetDouble";

            if (Campaign.Index <= 1)
                Init("Doom\\GetDouble_0.lvl");
            else
                Init("Doom\\GetDouble.lvl");
            //MyLevel.PreventReset = true;

            MyLevel.EraseNonDoodadCoins();

            //MakeCenteredCamZone(1f, "Center");
            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Dungeon);

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            // Players
            SetHeroType(BobPhsxNormal.Instance);
            //SetHeroType(BobPhsxSmall.Instance);

            MakePlayers();

            Vector2 powerup_pos = MyLevel.FindBlock("Powerup").Pos;
            AddPowerups(BobPhsxDouble.Instance, "Double Jump", powerup_pos, new Vector2(200, 0), Vector2.Zero, Vector2.Zero, null, null);

            // Position players
            foreach (Bob bob in MyLevel.Bobs)
                bob.Immortal = false;
            SetSpawnPoint(Doors["Enter"].Pos, new Vector2(30, 0));

            MyLevel.PreventReset = true;
            EnterFrom(FindDoor("Enter"), 100);
            CinematicToDo(150, () => MyLevel.PreventReset = false);

            // Exit sign
            if (MyLevel.Info.Doors.ShowSign)
            {
                Sign sign = new Sign(false, MyLevel);
                sign.PlaceAt(Doors["Exit"].GetTop());
                MyLevel.AddObject(sign);
            }

            // Exit door
            Doors["Exit"].SetLock(false, true, false);
            Doors["Exit"].NoNote = true;
            Doors["Exit"].OnOpen += ReturnToWorldMap;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }
    }
}