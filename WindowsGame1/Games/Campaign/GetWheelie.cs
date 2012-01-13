using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_GetWheelie : WorldMap
    {
        public Campaign_GetWheelie(EzSong song)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "GetWheelie";

            if (Campaign.Index <= 1)
                Init("Doom\\GetWheelie_0.lvl");
            else
                Init("Doom\\GetWheelie.lvl");

            MyLevel.EraseNonDoodadCoins();

            //MakeCenteredCamZone(1f, "Center");
            //MakeBackground(BackgroundType.Dungeon);

            MakeCenteredCamZone(.85f, "Center");
            MakeBackground(BackgroundType.Night);

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            // Players
            SetHeroType(BobPhsxNormal.Instance);

            MakePlayers();

            Vector2 buttonpos = MyLevel.FindBlock("Powerup").Pos;
            AddPowerups(BobPhsxWheel.Instance, "Wheelie", buttonpos, new Vector2(200, 0), Vector2.Zero, Vector2.Zero, null, null);

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