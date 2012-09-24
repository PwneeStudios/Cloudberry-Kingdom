using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_GetCart : WorldMap
    {
        static int DelayToDrop, DelayToEnter;
        static Vector2 DropPos, DropVel;
        static void SetParams()
        {
            //DelayToEnter = 215;
            //DelayToDrop = 50;
            //DropPos = Vector2.Zero;
            //DropVel = new Vector2(8, 0)

            //DelayToDrop = 125;
            //DropPos = new Vector2(-65, 0);
            //DropVel = new Vector2(-22, 5);

            DelayToEnter = 222;// 200;// 126;
            DelayToDrop = 120;// 120;// 150;
            DropPos = new Vector2(-405, 0);
            DropVel = new Vector2(-22.6f, 5);
        }

        public Campaign_GetCart(EzSong song)
            : base(false)
        {
            SetParams();

            Data = Campaign.Data;
            WorldName = "GetCart";

            Init("Doom\\GetCart.lvl");
            //Init("Doom\\GetCart_Sky.lvl");
            //Init("Doom\\GetCart_Castle.lvl");

            this.PhsxStepsToDo += 1;
            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Outside);
            //MakeBackground(BackgroundType.Castle);

            //MakeBackground(BackgroundType.Sky);
            //SkyBackground sky = MyLevel.MyBackground as SkyBackground;
            //sky.AffectCamera = false;

            // Players
            SetHeroType(BobPhsxNormal.Instance);
            //SetHeroType(BobPhsxSmall.Instance);

            MakePlayers();

            // Swoop down and drop cart
            MyLevel.PreventReset = true;
            CinematicToDo(45, () =>
            {
                if (song != null)
                    Tools.SongWad.LoopSong(song);
                Campaign.SwoopDown(MyLevel);
                CinematicToDo(DelayToDrop, (Action)AddCart);
            });

            // Position players
            EnterFrom(FindDoor("Enter"), DelayToEnter);
            CinematicToDo(280, () => MyLevel.PreventReset = false);

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

        //public override void AdditionalReset()
        //{
        //    base.AdditionalReset();
        //    // Swoop down and drop cart
        //    Campaign.SwoopDown(MyLevel);
        //    CinematicToDo(60, AddCart);
        //}

        void AddCart()
        {
            var princess = MyLevel.Objects.Find(obj => obj is PrincessBubble);
            if (princess == null) return;

            //Vector2 CartPos = MyLevel.FindBlock("Powerup").Pos;
            Vector2 CartPos = princess.Pos;
            //AddPowerups(BobPhsxRocketbox.Instance, "Hero in a Cart", CartPos, new Vector2(300, 0), null);
            AddPowerups(BobPhsxRocketbox.Instance, "Hero in a Cart", CartPos + DropPos, new Vector2(30, 0), DropVel, new Vector2(8, 0), null, null);
        }
    }
}