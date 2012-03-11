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
    public class Campaign_PrincessOverLava_Get : WorldMap
    {
        PrincessBubble princess;

        Chain chain;
        LavaBlock lblock;

        public Campaign_PrincessOverLava_Get()
            : base(false)
        {
            Data = Campaign.Data;
            WorldName = "PrincessOverLava_Get";

            Init("Doom\\PrincessOverLava.lvl");
            MyLevel.PreventReset = true;
            PhsxStepsToDo += 2;

            MyLevel.EraseNonDoodadCoins();

            //MakeCenteredCamZone(1f, "Center");
            MakeCenteredCamZone(1f, "Center", "CameraEnd");
            MakeBackground(BackgroundType.Dungeon);

            // Princess
            princess = new PrincessBubble(MyLevel.FindBlock("Princess").Pos);

            // Chain
            chain = new Chain();
            //Chain_p2_Start = princess.Pos + new Vector2(0, 380);//new Vector2(0, 100);
            float PrincessHeight = Tools.LerpRestrict(100, -380, Campaign.Difficulty / 4f);
            Chain_p2_Start = princess.Pos + new Vector2(0, PrincessHeight);//-380);
            SetChain();
            MyLevel.AddObject(chain);

            // Attach princess
            ResetPrincess();
            MyLevel.AddObject(princess);


            // Open exit when you get the princess
            princess.OnPickup += () => 
            {
                Doors["Exit"].SetLock(false, true, true);
                chain.DrawEnd = true;
            };

            // Lava
            lblock = (LavaBlock)Recycle.GetObject(ObjectType.LavaBlock, false);
            Vector2 LavaHeight = MyLevel.FindBlock("LavaHeight").Pos;
            lblock.Init(LavaHeight.Y, LavaHeight.X - 3000, LavaHeight.X + 3000, 5000);
            lblock.Core.EditHoldable = false;
            MyLevel.AddBlock(lblock);

            // Floor #
            CinematicToDo(25, () =>
            {
                int FloorNum = Challenge_SavePrincessRush.NumLevels[Campaign.Index];
                Tools.CurGameData.AddGameObject(new LevelTitle(string.Format("Floor {0}", FloorNum + 1)));
            });

            // Players
            SetHeroType(BobPhsxJetman.Instance);
            MakePlayers();

            // Position players
            EnterFrom(FindDoor("Enter"));
            CinematicToDo(50, () => MyLevel.PreventReset = false);

            // Exit sign
            Sign sign = new Sign(false);
            sign.PlaceAt(Doors["Exit"].GetTop());
            MyLevel.AddObject(sign);

            // Exit door
            Doors["Exit"].SetLock(true, true, false);
            Doors["Exit"].NoNote = true;
            //Doors["Exit"].OnOpen += ReturnToWorldMap;
            Doors["Exit"].OnOpen += ReturnToWorldMap_Slow;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());
        }

        private void ResetPrincess()
        {
            Tools.MoveTo(princess, MyLevel.FindBlock("Princess").Pos);
            princess.Box.Center = princess.Core.Data.Position;

            princess.MyBob = null;
            princess.MyState = PrincessBubble.State.Float;
            princess.ShowWithMyBob = true;
            princess.Oscillate = false;

            foreach (Bob bob in MyLevel.Bobs)
            {
                bob.MoveData.PlacePlatforms = false;
                bob.HeldObject = null;
                bob.UnsetPlaceAnimData();
            }

            // Attach princess to chain
            AddToDo(() =>
            {
                // Disconnect princess from chain when Bob grabs her
                if (princess.MyState != PrincessBubble.State.Float) return true;

                // Fix princess to end of chain
                princess.Pos = chain.GetEndPoint();
                return false;
            }, "AttachToChain", true, true);
        }

        Vector2 Chain_p2_Start;
        void SetChain()
        {
            chain.p1 = MyLevel.FindBlock("p1").Pos;
            chain.p2 = Chain_p2_Start;
        }

        int Step = 0;
        public override void PhsxStep()
        {
            base.PhsxStep();

            // Add blobs
            Step++;
            if (Step % 6 == 0)
                MakeBlob();
            RemoveBlobs();

            // Check for princess hits lava
            lblock.Box.CalcBounds_Full();
            if (princess.Core.Show && princess.Pos.Y < lblock.Box.TR.Y - 60)
            {
                princess.Core.Show = false;
                Fireball.Explosion(princess.Pos, MyLevel, Vector2.Zero, 3.95f, 2.3f);
                WaitThenDo(60, Reset, "Reset", true, true);
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

        public override void AdditionalReset()
        {
            base.AdditionalReset();

            princess.Core.Show = true;
            SetChain();
            ResetPrincess();
        }

        void MakeBlob()
        {
            // Make blob
            Goomba blob = (Goomba)Recycle.GetObject(ObjectType.FlyingBlob, false);
            blob.Pos = new Vector2(MyLevel.Rnd.RndFloat(Cam.BL.X - 200, Cam.TR.X + 500), Cam.BL.Y - 300);

            blob.NeverSkip = true;
            blob.Core.EditHoldable = false;

            blob.DeleteOnDeath = true;
            blob.Core.RemoveOnReset = true;

            blob.MaxVel = 20;
            blob.MaxAcc = 1;

            blob.MyPhsxType = Goomba.PhsxType.ToTarget;
            blob.Target = blob.Core.Data.Position + new Vector2(0, 4000);

            MyLevel.AddObject(blob);
            blob.Core.EditHoldable = false;
        }
    }
}