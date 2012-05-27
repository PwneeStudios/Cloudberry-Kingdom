using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class Campaign_World2 : WorldMap
    {
        public override void PhsxStep()
        {
            base.PhsxStep();

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                AddGameObject(new ScoreScreen(StatGroup.Level, this));

                //foreach (IObject obj in MyLevel.Objects)
                //    if (obj is PrincessBubble || obj is Goombas.Goomba)
                //        obj.CollectSelf();

                //PrincessBubble princess = new PrincessBubble(Cam.Pos);
                //MyLevel.AddObject(princess);

                //Campaign.GrabPrincess(this, princess);
            }
        }



        public Campaign_World2(bool FromEntrance)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "World2";

            Init("Doom\\World2.lvl");
            PhsxStepsToDo += 2;

            MakeCenteredCamZone(.72f);
            Cam.MyZone.CameraType = Camera.PhsxType.SideLevel_Up_Relaxed;
            Cam.MyZone.End.Y += 6000;

            

            //MakeBackground(BackgroundType.Outside);
            MakeBackground(BackgroundType.Rain);
            //MyLevel.UseLighting = true;
            //MyLevel.StickmanLighting = true;
            
            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakePlayers();

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Enter
            if (FromEntrance)
                EnterFrom(Doors["Enter"]);
            else
            {
                EnterFrom(Doors[201]);
                Cam.Pos = new Vector2(Cam.Pos.X, Cam.Pos.Y + 3330);
            }

            // Elevator
            BlockEmitter bm = (BlockEmitter)Recycle.GetObject(ObjectType.BlockEmitter, false);
            bm.EmitData.Position = MyLevel.FindBlock("Elevator").Pos + new Vector2(0, -1000);
            bm.EmitData.Velocity = new Vector2(0, 10);
            bm.Range = new Vector2(20000);
            bm.GiveCustomRange = true;
            bm.AlwaysOn = true;
            bm.Delay = 65;
            bm.Size = new Vector2(120, 40);
            bm.Core.DrawLayer = 9;
            bm.GiveLayer = true;
            MyLevel.AddObject(bm);

            // Rain
            //AddGameObject(new Rain());

            // Dancing Berry
            //AddGameObject(new DanceBerry());
        }

        LevelSeedData DarkLevel()
        {
            var d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxDouble.Instance, 1.2f, TileSets.Dark);
            d.PostMake += Campaign.UseBobLighting;

            return d;
        }

        LevelSeedData World1_To_World2_UpLevel()
        {
            var d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxJetman.Instance, 8000, 1, LevelGeometry.Up);
            d.MyGeometry = LevelGeometry.Up;
            d.SetTileSet(TileSets.Terrace);

            return d;
        }

        LevelSeedData World2_To_World3_DownLevel()
        {
            var d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxBox.Instance, 12000, 1, LevelGeometry.Down);
            //var d = Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxBox.Instance, 3000, 1, LevelGeometry.Down);
            d.MyGeometry = LevelGeometry.Down;
            d.PieceSeeds[0].Style.MyFinalPlatsType = StyleData.FinalPlatsType.DarkBottom;
            //d.SetTileSet(TileSets.Terrace);
            d.SetTileSet(TileSets.Rain);

            return d;
        }

        protected override void AssignDoors()
        {
            MakeDoorAction(FindDoor("Enter"), () =>
                 Load(new Campaign_IntroWorld()));

            //MakeDoorAction_Level(Doors[1], Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxDouble.Instance, 1.2f, TileSets.Dark));
            //MakeDoorAction_Level(Doors[1], World1_To_World2_UpLevel());
            //MakeDoorAction_Level(Doors[1], DarkLevel());
            LevelSeedData d = Campaign.HeroLevel(Campaign.Difficulty, BobPhsxNormal.Instance, 4000, 1);
            d.SetTileSet(TileSets.Terrace);
            d.PieceSeeds[0].Style.MyInitialPlatsType = StyleData.InitialPlatsType.CastleToTerrace;
            d.PieceSeeds[0].Style.MyFinalDoorStyle = StyleData.FinalDoorStyle.TerraceToCastle;
            d.PostMake += lvl => Campaign.LevelWithPrincess(lvl, true, Campaign.PrincessPos.CenterToRight, true);
            MakeDoorAction_Level(Doors[1], d);
            Icons[1].SetIcon(BobPhsxDouble.Instance);

            //MakeDoorAction_Level(Doors[2], Campaign.HeroLevel(Campaign.Difficulty+.2f, BobPhsxJetman.Instance, 1.2f));
            MakeDoorAction_Level(Doors[2], World2_To_World3_DownLevel());
            Icons[2].SetIcon(BobPhsxJetman.Instance);

            if (PlayerManager.GetNumPlayers() > 1)
            {
                MakeDoorAction_Level(Doors[3], Campaign.Bungee(Campaign.Difficulty - .3f, BobPhsxJetman.Instance, .9f));
                Icons[3].SetIcon(IconType.Bungee);
            }
            else
            {
                MakeDoorAction_Level(Doors[3], Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxRocketbox.Instance, Campaign.Length(2.65f), 1));
                Icons[3].SetIcon(BobPhsxRocketbox.Instance);
                //Icons[3].SetIcon(IconType.Place);
            }
            AddDependence(3, 1, 2);

            MakeDoorAction_Level(Doors[4], Campaign.HeroLevel(Campaign.Difficulty + .2f, BobPhsxRocketbox.Instance, Campaign.Length(2.65f), 1));
            AddDependence(4, 1, 2);
            Icons[4].SetIcon(BobPhsxRocketbox.Instance);

            // Blob pure
            MakeDoorAction_Level(Doors[5], Campaign.HeroLevel(Campaign.Difficulty + .4f, BobPhsxWheel.Instance, 1.3f));
            AddDependence(5, 3, 4);
            Icons[5].SetIcon(BobPhsxWheel.Instance);

            // Spikey guy pure
            MakeDoorAction_Level(Doors[6], Campaign.HeroLevel(Campaign.Difficulty + .4f, BobPhsxBouncy.Instance, 1.3f));
            AddDependence(6, 3, 4);
            Icons[6].SetIcon(BobPhsxBouncy.Instance);

            // Boss
            Doors[201].MyObject.Base.SetScale(new Vector2(608, 600) * 1.56f);
            Icons[201].MyPile.Scale(1.4f);
            MakeDoorAction(Doors[201], () => Load(new Campaign_Boss(true)));
            //AddDependence(201, 5, 6);
            Icons[201].SetIcon(IconType.Boss);

            // Bonus
            MakeDoorAction_Level(Doors[101], Campaign.BonusLevel_1());
            Icons[101].SetIcon(IconType.Boss);
            AddDependence(101, 1, 2);
        }
    }
}