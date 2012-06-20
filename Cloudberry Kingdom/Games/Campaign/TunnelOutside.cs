using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class Campaign_TunnelOutside : WorldMap
    {
        public static bool WatchedIntro;

        public Campaign_TunnelOutside()
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "DoubleJumpIntro";

            Init("Doom\\SimpleInside.lvl");

            MakeCenteredCamZone(.8f);
            MakeBackground(BackgroundType.Outside);

            // Players
            SetHeroType(BobPhsxDouble.Instance);
            MakePlayers();

            if (WatchedIntro)
                EnterFrom(FindDoor("Enter"));
            else
            {
                WatchedIntro = true;

                EnterFrom(FindDoor("Enter"), 90);

                // Title
                WaitThenDo_Pausable(33, () => AddTitle(this, "Double\n   Jump", 175));
            }

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        protected override void AssignDoors()
        {
            MakeDoorAction(FindDoor("Enter"), () =>
                Load(new Campaign_IntroWorld()));

            MakeDoorAction(FindDoor("Exit"), () =>
                Load(new Campaign_TunnelOutside()));
        }
    }
}