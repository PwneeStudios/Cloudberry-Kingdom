using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_DarkTower : WorldMap
    {
        Door TopDoor = null;

        public Campaign_DarkTower()
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "DarkTower";

            Init("Doom\\DarkTower.lvl");

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(.85f, "Center");
            MakeBackground(BackgroundType.Night);

            // Players
            SetHeroType(BobPhsxNormal.Instance);

            MakePlayers();

            // Position players
            foreach (Bob bob in MyLevel.Bobs)
                bob.Immortal = false;
            SetSpawnPoint(MyLevel.FindIObject("Enter").Pos, new Vector2(30, 0));

            MyLevel.PreventReset = true;
            EnterFrom(FindDoor("Enter"));

            // Floors
            int NumFloors = Challenge_SavePrincessRush.NumLevels[Campaign.Index] + 1;
            Vector2 FloorPos = MyLevel.FindBlock("Floor1").Pos;
            float Left = FloorPos.X;
            float Right = MyLevel.FindBlock("CastleBack").Pos.X;
                Right = 2 * Right - Left;
            for (int i = 1; i <= NumFloors; i++)
            {
                // Catwalk for door
                var block = (NormalBlock)Recycle.GetObject(ObjectType.NormalBlock, false);
                float pos = i % 2 == 0 ? Right : Left;
                block.Init(new Vector2(pos, FloorPos.Y), new Vector2(500, 100), MyLevel.MyTileSetInfo);
                block.Core.MyTileSet = TileSets.Catwalk;
                if (i % 2 == 0)
                    block.Stretch(Side.Left, 50);
                else
                    block.Stretch(Side.Right, -50);
                block.Core.EditHoldable = false;
                block.Core.DrawLayer = 2;
                MyLevel.AddBlock(block);

                // Door
                var door = MyLevel.PlaceDoorOnBlock(new Vector2(pos, FloorPos.Y), block, false);
                TopDoor = door;
                door.SetDoorType(Door.Types.Grass);
                door.Core.EditHoldable = false;
                door.Core.DrawLayer = 1;
                door.SetLock(true, true, false);
                MyLevel.AddObject(door);

                // Floor title
                //var title = new LevelTitle(string.Format("Floor {0}", i));
                Vector2 titlepos = i % 2 == 0 ? FloorPos + new Vector2(-300, 400) + new Vector2(398.6929f, 130.7188f) : FloorPos + new Vector2(870, 400);
                var title = new GUI_Text(string.Format("Floor {0}", i), titlepos, true);
                title.MyText.Scale *= 1.23f;
                title.MyText.ZoomWithCam = true;
                title.Core.DrawLayer = Level.LastInLevelDrawLayer - 1;
                title.MyText.Shadow = true;
                title.MyText.ShadowOffset = new Vector2(20, 16) * .823f;
                title.MyText.ShadowColor = new Color(.13f, .13f, .13f, .7f);
                CampaignMenu.EasyColor(title.MyText);
                Tools.Swap(ref title.Pos.RelVal, ref title.MyPile.FancyPos.RelVal);
                Tools.CurGameData.AddGameObject(title);

                FloorPos += new Vector2(0, 1850);
            }

            MyLevel.FindBlock("CastleBack").Extend(Side.Top, FloorPos.Y - 600);

            // Exit sign
            //Sign sign = new Sign(false);
            //sign.PlaceAt(Doors["Exit"].GetTop());
            //MyLevel.AddObject(sign);

            // Exit door
            Doors["Exit"].SetLock(false, true, false);
            Doors["Exit"].NoNote = true;
            //Doors["Exit"].OnOpen += ReturnToWorldMap;
            Doors["Exit"].OnOpen += d =>
            {
                d.SetLock(true, true, true);
                WaitThenDo(13, () =>
                    DoingPan = true);
            };

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                MyLevel.SetToReset = true;
                //WaitThenDo(3, );
                ShowedHelp = false;
            }

            if (DoingPan)
                PanUp();
        }

        bool DoingPan = false;
        bool ShowedHelp = false;
        void PanUp()
        {
            Cam.SetPhsxType(Camera.PhsxType.Fixed);
            
            Vector2 Dest = new Vector2(Cam.Pos.X, TopDoor.Pos.Y + 850);

            if (Cam.Pos.Y < Dest.Y - 4700)//4850)
                Cam.Data.Velocity.Y = Tools.Restrict(0, 110, Cam.Data.Velocity.Y + .45f);
            else
                Cam.Data.Velocity.Y *= .9735f;
            Cam.Data.Integrate();

            if (!ShowedHelp &&
                Cam.Data.Velocity.Y < 4 && Cam.Pos.Y > Dest.Y - 4000)
            {
                HelpBubble bubble = new HelpBubble(TopDoor.Pos + new Vector2(120, 80) + new Vector2(601.3071f, -385.6211f));
                bubble.TimesToShow = 1;
                bubble.ArrowToLeft();
                Tools.Swap(ref bubble.Pos.RelVal, ref bubble.MyPile.FancyPos.RelVal);
                AddGameObject(bubble);

                ShowedHelp = true;

                CinematicToDo(96, (Action)Done);
            }
        }

        void Done()
        {
            ReturnToWorldMap_Immediate();
            //MyLevel.SetToReset = true;
            //ShowedHelp = false;
        }
    }
}