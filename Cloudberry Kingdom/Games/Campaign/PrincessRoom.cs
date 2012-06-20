using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class Campaign_PrincessRoom : WorldMap
    {
        public static bool WatchedIntro;

        PrincessBubble princess;

        bool ForTrailer = false;

        public Campaign_PrincessRoom()
            : base(false)
        {
            int WaitToEnter = 63;
            if (ForTrailer) WaitToEnter = 180;

            Data = Campaign.Data;
            WorldName = "PrincessRoom";

            Init("Doom\\PrincessRoom.lvl");
            MyLevel.PreventReset = true;

            MakeCenteredCamZone(.8f);
            //MakeBackground(BackgroundType.Night);
            MakeBackground(BackgroundType.Rain);
            if (ForTrailer)
                MakeBackground(BackgroundType.Outside);

            // Players
            SetHeroType(BobPhsxNormal.Instance);
            MakeMvp();
            // Player control via loaded recording
            if (ForTrailer)
            {
                Door.AllowCompControl = true;
                LoadRecording("PrincessTrailer.rec");
            }


            if (WatchedIntro)
                EnterFrom(FindDoor("Enter"));
            else
            {
                WatchedIntro = true;

                EnterFrom(FindDoor("Enter"), WaitToEnter);
            }

            // Princess
            princess = new PrincessBubble(Pos("Princess"));
            if (Campaign.Index >= 3) princess.MyAnimation = PrincessBubble.DeadAnim;
            princess.OnPickup = () =>
            {
                WaitThenDo(44, () =>
                    Doors["Exit"].SetLock(false, true, true, false));
            };
            MyLevel.AddObject(princess);

            // Exit sign
            Sign sign = new Sign(false);
            sign.PlaceAt(Doors["Exit"].GetTop());
            MyLevel.AddObject(sign);

            // Exit door
            Doors["Exit"].SetLock(true, true, false);
            Doors["Exit"].NoNote = true;

            // Menu
            AddGameObject(InGameStartMenu_Campaign.MakeListener());

            // Start recording
            if (ForTrailer)
                WaitThenDo(1, () =>
                    Tools.TheGame.SetToBringSaveVideoDialog = true);
                    //Tools.TheGame.SetToRecordInput = true);
        }


        protected override void AssignDoors()
        {
            // Entrance
            if (!ForTrailer)
            MakeDoorAction(FindDoor("Enter"), () =>
                  Load(new Campaign_Boss(false)));

            // Exit
            Door exit = Doors["Exit"];

            if (ForTrailer)
            {
                MakeDoorAction(exit, () => { });
                exit.OnOpen += d => princess.ShowWithMyBob = true;
            }
            else
            MakeDoorAction(exit, () =>
            {
                MakeDoorAction(exit, () => { });
                Bob bob = exit.InteractingBob;
                exit.MoveFeet = true;

                princess.MyState = PrincessBubble.State.FloatUp;
                princess.Core.Data.Velocity = Vector2.Zero;
                bob.HeldObject = null;
                bob.UnsetPlaceAnimData();
                bob.Core.Data.Velocity = Vector2.Zero;
                bob.SetCodeControl();

                WaitThenDo(0, () => {
                    EnterFrom(exit, 76);
                    WaitThenDo(105, () => SetBobControl(bob));
                    WaitThenDo(66 + 250, OnLand);
                });
            });
        }

        void SetBobControl(Bob bob)
        {
            // Control the player:
            // Bob jumps up and grabs the princess, then enters door
            bob.CodeControl = true;
            bob.ControlFunc = step =>
            {
                int Jump = 23;// 38;
                bob.CurInput.A_Button = false;

                if (step < 20)
                    return;
                else if (step < Jump)
                    bob.MyPhsx.LandOnSomething(true, null);
                else if (step < Jump + 52)
                {
                    if (step == Jump + 2)
                    {
                        bob.OnLand = OnLand;
                        bob.OnApexReached = () => princess.PickUp(bob);
                    }
                    bob.CurInput.A_Button = true;
                }
            };
        }

        bool Landed = false;
        void OnLand()
        {
            if (Landed) return;
            Landed = true;

            Door exit = Doors["Exit"];

            princess.MyBob.ControlFunc = step => { };

            WaitThenDo(18, () =>
            {
                // Exit with the princess
                princess.ShowWithMyBob = true;
                exit.SetLock(true, true, true);

                // End the campaign
                WaitThenDo(96, () => {
                    CampaignEnd();
                    EndGame = AfterScore;
                });

                //// Fade to black
                //WaitThenDo(100, () => {
                //    FadeToBlack(.008f);
                //});
            });
        }

        void AfterScore(bool Replay)
        {
            // Fade to black
            WaitThenDo(35, () =>
            {
                FadeToBlack(.009f);

                WaitThenDo(165, () => {
                    // Credits level
                    Load(new Campaign_CreditsLevel());
                });
            });
        }
    }
}