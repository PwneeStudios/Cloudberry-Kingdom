using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class Escalation_Tutorial : GameObject
    {
        /// <summary>
        /// Whether the Escalation introduction has been watched before.
        /// </summary>
        public static bool WatchedOnce { get { return _WatchedOnce; } set { _WatchedOnce = value; PlayerManager.SavePlayerData.Changed = true; } }
        static bool _WatchedOnce = false;

        static public bool ShowTitle = true;

        /// <summary>
        /// Whether text makes a popping sound when we kill it
        /// </summary>
        protected bool SoundOnKill = false;

        Challenge_Escalation Escalation;
        Campaign.PrincessPos PosTo;
        public Escalation_Tutorial(Challenge_Escalation Escalation, Campaign.PrincessPos PosTo)
        {
            this.PosTo = PosTo;
            this.Escalation = Escalation;
        }

        PrincessBubble princess;
        public override void OnAdd()
        {
            base.OnAdd();

            // Add the princess
            if (!WatchedOnce)
            {
                Level lvl = MyGame.MyLevel;
                princess = new PrincessBubble(lvl.LeftMostCameraZone().Start + new Vector2(360, 25));
                lvl.AddObject(princess);

                lvl.PreventReset = true;
            }
            else
                PauseGame = true;

            MyGame.HideBobs();
            MyGame.PhsxStepsToDo += 2;
            
            // Start the music
            MyGame.WaitThenDo(20, () =>
                {
                    Tools.SongWad.SuppressNextInfoDisplay = true;
                    Tools.SongWad.SetPlayList(Tools.SongList_Standard);
                    //Tools.SongWad.SetPlayList(Tools.Song_140mph);
                    Tools.SongWad.Restart(true);
                });

            if (HeroRush_Tutorial.ShowTitle || !WatchedOnce)
            //if (ShowTitle || !WatchedOnce)
                MyGame.WaitThenDo(27, () => Title());
            else
                MyGame.WaitThenDo(20, () => Ready());
        }

        protected void TutorialOrSkip()
        {
            if (!WatchedOnce)
            {
                MyGame.MyLevel.PreventReset = MyGame.MyLevel.PreventHelp = true;

                Campaign.GrabPrincess(MyGame, princess, PosTo);
                int wait = MyGame.DramaticEntry(MyGame.MyLevel.StartDoor, 90);
                MyGame.MyLevel.SetBack(wait + 90);
                MyGame.WaitThenDo(wait, () =>
                {
                    MyGame.MyLevel.PreventHelp = false;
                    MyGame.MyLevel.PreventReset = false;
                    MyGame.MyLevel.Finished = false;
                });
            }
            else
                Ready();

            WatchedOnce = true;
        }

        protected virtual void Title()
        {
            ShowTitle = false;

            if (WatchedOnce)
            {
                GUI_Text text = GUI_Text.SimpleTitle(Escalation.Name);
                CampaignMenu.HappyBlueColor(text.MyText);

                MyGame.AddGameObject(text);

                // On (A) go to next part of the tutorial
                MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
                {
                    //MyGame.WaitThenDo(18, () => PointAtDoor());
                    MyGame.WaitThenDo(12, () => TutorialOrSkip());
                    text.Kill(SoundOnKill);
                }));
            }
            else
            {
                GUI_Text text = GUI_Text.SimpleTitle(Escalation.Name);
                CampaignMenu.HappyBlueColor(text.MyText);
                text.Pos.RelVal.Y -= 800;

                MyGame.AddGameObject(text);

                MyGame.WaitThenDo(120, () => text.Kill(SoundOnKill));
                MyGame.WaitThenDo(40, () => TutorialOrSkip());
            }
        }

        void Ready()
        {
            int Wait = 5 + 22;

            MyGame.WaitThenDo(Wait, () =>
                TutorialHelper.ReadyGo(MyGame, End));
        }

        void End()
        {
            PauseGame = false;

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}