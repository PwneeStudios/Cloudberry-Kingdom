using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class Doom : WorldMap
    {
        public static DoorDict<DoorData> DoomData = new DoorDict<DoorData>();

        public Doom()
            : base(false)
        {
            Data = Doom.DoomData;
            Campaign.Data = Doom.DoomData;
            WorldName = "Doom";

            Init("Doom\\Doom.lvl");

            MakeCenteredCamZone(.72f);
            //MakeBackground(BackgroundType.Outside);
            MakeBackground(BackgroundType.Chaos);
            
            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakePlayers();

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Intro cutscene
            {
                // Fade from black
                Black();
                WaitThenDo(6, () =>
                    FadeIn(.02f));

                // Hide
                EnterFrom(Doors["Enter"], 70);
            }
        }

        protected override void AssignDoors()
        {
            MakeDoorAction_Level(Doors[1], Challenge_ObstacleTraining.StartLevel);
            Icons[1].SetIcon(BobPhsxBox.Instance);

            //MakeDoorAction_Level(Doors[2], Campaign.WallLevel());
            AddDependence(2, 1);
            Icons[2].SetIcon(Upgrade.FlyBlob);

            MakeDoorAction_Level(Doors[3], () => Challenge_Survival.Instance.Start(3));
            AddDependence(3, 2);
            Icons[3].SetIcon(Upgrade.Fireball);

            //MakeDoorAction_Level(Doors[4], Campaign.WallLevel());
            AddDependence(4, 3);
            Icons[4].SetIcon(BobPhsxDouble.Instance);
        }
    }
}