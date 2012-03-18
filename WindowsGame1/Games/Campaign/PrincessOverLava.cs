using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;
using CloudberryKingdom.Coins;
using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public class Campaign_PrincessOverLava : WorldMap
    {
        PrincessBubble princess;
        Challenge_SavePrincessRush MyRush;

        public Campaign_PrincessOverLava(Challenge_SavePrincessRush rush)
            : base(false)
        {
            MyRush = rush;

            Data = Campaign.Data;
            WorldName = "PrincessOverLava";

            Init("Doom\\PrincessOverLava.lvl", false);
            MyLevel.PreventReset = true;
            PhsxStepsToDo += 2;

            MyLevel.EraseNonDoodadCoins();

            MakeCenteredCamZone(1f, "Center_Cinematic");
            //MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Dungeon);

            // Princess
            princess = new PrincessBubble(MyLevel.FindBlock("Princess").Pos);
            MyLevel.AddObject(princess);
            princess.MyState = PrincessBubble.State.None;

            // Chain
            var chain = new Chain();
            chain.p1 = MyLevel.FindBlock("p1").Pos;
            chain.p2 = princess.Pos + new Vector2(0, 380);//new Vector2(0, 100);
            MyLevel.AddObject(chain);

            // Attach princess to chain
            AddToDo(() => { princess.Pos = chain.GetEndPoint(); return false; });


            // Lava
            LavaBlock lblock = (LavaBlock)Recycle.GetObject(ObjectType.LavaBlock, false);
            Vector2 LavaHeight = MyLevel.FindBlock("LavaHeight").Pos;
            lblock.Init(LavaHeight.Y, LavaHeight.X - 3000, LavaHeight.X + 3000, 5000);
            lblock.Core.EditHoldable = false;
            MyLevel.AddBlock(lblock);


            // Players
            //SetHeroType(BobPhsxNormal.Instance);
            //MakePlayers();

            //// Exit sign
            //Sign sign = new Sign(false);
            //sign.PlaceAt(Doors["Exit"].GetTop());
            //MyLevel.AddObject(sign);

            //// Exit door
            //Doors["Exit"].SetLock(false, true, false);
            //Doors["Exit"].NoNote = true;
            //Doors["Exit"].OnOpen += ReturnToWorldMap;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        public static int TimeOnTimer;
        GUI_Timer_Simple Timer;

        int Step = 0;
        public override void PhsxStep()
        {
            if (Timer != null)
                TimeOnTimer = Timer.Time;

            base.PhsxStep();

            Step++;
            if (Step % 2 == 0)
                MakeBlob();
            RemoveBlobs();

            // Music
            if (Step == 10)
            {
                Tools.DoNotKillMusicOnNextLoadingscreen = true;
                Tools.SongWad.SuppressNextInfoDisplay = true;
                Tools.SongWad.SetPlayList(Tools.Song_140mph);
                Tools.SongWad.Restart(true);
            };

            // Timer
            if (Step == 95 + 60)
            {
                Timer = new GUI_Timer_Simple(62 * 10 - 1);
                AddGameObject(Timer);
                Timer.Show();

                Tools.SoundWad.FindByName("Pop 2").Play();
            }

            // Floor #
            if (Step == 40)
            {
                int FloorNum = Challenge_SavePrincessRush.NumLevels[Campaign.Index];
                Tools.CurGameData.AddGameObject(new LevelTitle(string.Format("Floor {0}", FloorNum + 1)));
            }

            // Oh no!
            if (Step == 185 + 16)
            {
                //AddGameObject(new OhNo());
                GUI_Text text = new GUI_Text("Oh no!",
                    princess.Pos + new Vector2(230, 230));
                text.MyText.Scale *= 1.3f;
                text.Oscillate = true;
                text.NoPosMod = false;
                AddGameObject(text);
            }

            // Start game
            if (Step == 330 + 10)
            {
                //ReturnToWorldMap_Immediate();
                MyRush.SwitchBackFromCinematic();
            }
        }

        void RemoveBlobs()
        {
            // Remove blobs that are above the screen
            MyLevel.GetObjectList(ObjectType.FlyingBlob).ForEach(blob =>
            {
                if (blob.Pos.Y > Cam.TR.Y + 200)
                    blob.CollectSelf();
            });
        }

        void MakeBlob()
        {
            // Choose side
            BlockBase block;
            if (MyLevel.Rnd.RndBool())
                block = MyLevel.FindBlock("BlobSpawn_Left");
            else
                block = MyLevel.FindBlock("BlobSpawn_Right");

            // Make blob
            Goomba blob = (Goomba)Recycle.GetObject(ObjectType.FlyingBlob, false);
            block.Box.CalcBounds_Full();
            blob.Pos = new Vector2(MyLevel.Rnd.RndFloat(block.Box.TR.X, block.Box.BL.X), block.Pos.Y);

            blob.NeverSkip = true;

            blob.DeleteOnDeath = true;
            blob.Core.RemoveOnReset = true;

            blob.MaxVel = 20;
            blob.MaxAcc = 1;

            blob.MyPhsxType = Goomba.PhsxType.ToTarget;
            blob.Target = blob.Core.Data.Position + new Vector2(0, 4000);

            MyLevel.AddObject(blob);
        }
    }
}