using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class Campaign_DungeonFromAbove : WorldMap
    {
        public Campaign_DungeonFromAbove(EzSong song)
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "DungeonFromAbove";

            Init("Doom\\Dungeon_FromAbove.lvl");
            //MyLevel.PreventReset = true;

            MyLevel.EraseNonDoodadCoins();

            //MakeCenteredCamZone(1f, "Center");
            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Dungeon);

            if (song != null)
                WaitThenDo(45, () => Tools.SongWad.LoopSong(song));

            // Thwomp
            Thwomp thwomp = new Thwomp(false);
            thwomp.KillsOnSmash = false;
            thwomp.BlockCore.DoNotPushHard = true;
            thwomp.Init(Cam.Pos + new Vector2(-650, 720), new Vector2(260));
            bool SmashedOnce = false;
            thwomp.OnSmash = b =>
            {
                // Hero title
                if (!SmashedOnce)
                    CinematicToDo(55, () => AddGameObject(LevelTitle.HeroTitle("Tiny bob")));
                SmashedOnce = true;

                // Switch hero
                MyLevel.SwitchHeroType(b, BobPhsxSmall.Instance);
                ParticleEffects.HeroExplosion(MyLevel, b.Pos);

                // Prevent player from moving
                b.MyPhsx.Vel = Vector2.Zero;
                b.Immobile = true;
                ToDoOnReset.Add(() => b.Immobile = false);
                CinematicToDo(78, () => b.Immobile = false);
            };
            MyLevel.AddBlock(thwomp);

            // Action block
            Vector2 buttonpos = MyLevel.FindBlock("Powerup").Pos;
            var button = new SwitchBlock(buttonpos);
            button.OnActivate = () => thwomp.SetState(ThwompState.Vibrate);
            MyLevel.AddBlock(button);

            // Players
            //SetHeroType(BobPhsxInvert.Instance);
            //SetHeroType(BobPhsxMeat.Instance);

            SetHeroType(BobPhsxNormal.Instance);
            //SetHeroType(BobPhsxSmall.Instance);
            
            //BobPhsx SmallerBob = BobPhsxNormal.Instance.Clone();
            //BobPhsxSmall.Instance.Set(SmallerBob, new Vector2(.9f));
            //SetHeroType(SmallerBob);

            MakePlayers();

            //AddPowerups(BobPhsxWheel.Instance, "Wheelie", Cam.Pos + new Vector2(-700, 0), new Vector2(300, 0));

            // Position players
            Vector2 pos = Cam.Pos + new Vector2(-280, 1500);
            foreach (Bob bob in MyLevel.Bobs)
            {
                //Tools.MoveTo(bob, pos);
                bob.Immortal = false;
            }
            SetSpawnPoint(pos, new Vector2(100, 0));
            MoveBobsToSpawnPoint();

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
    }
}