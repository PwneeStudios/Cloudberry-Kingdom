using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class SavePrincessRush_Intro : GameObject
    {
        /// <summary>
        /// Whether the introduction has been watched before.
        /// </summary>
        static int TimesWatched = 0;

        public static void CampaignReset()
        {
            Challenge_SavePrincessRush.CinematicShown = false;
            TimesWatched = 0;
        }

        /// <summary>
        /// Whether text makes a popping sound when we kill it
        /// </summary>
        protected bool SoundOnKill = false;

        Challenge_SavePrincessRush SavePrincessRush;
        public SavePrincessRush_Intro(Challenge_SavePrincessRush SavePrincessRush)
        {
            this.SavePrincessRush = SavePrincessRush;

            SavePrincessRush.Timer.Hide();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            PauseGame = true;

            foreach (Bob bob in MyGame.MyLevel.Bobs)
                bob.Core.Show = false;
            
            // Start the music
            if (TimesWatched > 0)
            MyGame.WaitThenDo(20, () =>
                {
                    Tools.SongWad.SuppressNextInfoDisplay = true;
                    Tools.SongWad.SetPlayList(Tools.Song_140mph);
                    Tools.SongWad.Restart(true);
                });

            if (TimesWatched == 0)
            {
                Tools.CurGameData.PhsxStepsToDo += 1;
                SavePrincessRush.Timer.Time = Campaign_PrincessOverLava.TimeOnTimer;
                SavePrincessRush.Timer.Show();
                SavePrincessRush.Timer.PauseOnPause = false; // Start the timer
                //End();
                Ready();
            }
            else if (TimesWatched == 1)
                MyGame.WaitThenDo(27, () => StartTutorial());
            else
                MyGame.WaitThenDo(20, () => Ready());

            TimesWatched++;
        }

        void StartTutorial()
        {
            //PointAtDoor();
            PointAtCoins();
        }

        void PointAtDoor()
        {
            //SavePrincessRush.Timer.Show();

            ObjectBase end_door = MyGame.MyLevel.FindIObject(LevelConnector.EndOfLevelCode);
            Vector2 endpos = end_door.Core.Data.Position;

            Arrow arrow = new Arrow();
            arrow.SetOrientation(Arrow.Orientation.Right);
            arrow.Move(endpos + new Vector2(-673, 0));
            arrow.PointTo(endpos);
            MyGame.AddGameObject(arrow);

            GUI_Text text = new GUI_Text("Get to the exit", arrow.Core.Data.Position + new Vector2(-200, 400));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
            {
                arrow.Release();
                text.Kill(SoundOnKill);

                MyGame.WaitThenDo(7, () => SavePrincessRush.Timer.Show());
                MyGame.WaitThenDo(0, () =>
                    {
                        PointAtTimer();
                        //arrow.Release();
                        //text.Kill();
                    });
            }));
        }

        void PointAtTimer()
        {
            //Vector2 timerpos = SavePrincessRush.Timer.ApparentPos;
            Vector2 timerpos = MyGame.CamPos + new Vector2(-60, 1000);

            Arrow arrow = new Arrow();
            arrow.SetOrientation(Arrow.Orientation.Right);
            arrow.Move(timerpos + new Vector2(30, -655));
            arrow.PointTo(timerpos);
            MyGame.AddGameObject(arrow);

            GUI_Text text = new GUI_Text(
                string.Format("{0} seconds\n   on the clock!", SavePrincessRush.Timer.Seconds),
                arrow.Core.Data.Position + new Vector2(830, -130));
            
            MyGame.AddGameObject(text);
            
            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
            {
                PointAtCoins();
                arrow.Release();
                text.Kill(SoundOnKill);
            }));
        }

        void PointAtCoins()
        {
            List<Arrow> arrows = new List<Arrow>();
            foreach (ObjectBase coin in MyGame.MyLevel.GetObjectList(ObjectType.Coin))
            {
                Vector2 coinpos = coin.Core.Data.Position;

                Arrow arrow = new Arrow();
                arrow.SetScale(300);
                arrow.SetOrientation(Arrow.Orientation.Left);
                arrow.Move(coinpos + new Vector2(120, 200) * 1.04f);
                arrow.PointTo(coinpos);
                MyGame.AddGameObject(arrow);
                arrows.Add(arrow);
            }

            GUI_Text text = new GUI_Text("Coins add seconds to the clock",
                Tools.CurLevel.MainCamera.Data.Position + new Vector2(0, -750));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
            {
                MyGame.WaitThenDo(0, () => Ready());
                arrows.ForEach(arrow => arrow.Release());
                text.Kill(SoundOnKill);
            }));
        }

        void Ready()
        {
            int Wait = 5 + 22;
            if (SavePrincessRush.Timer.Hid) Wait = 28 + 12;

            SavePrincessRush.Timer.Show();
            SavePrincessRush.Timer.PauseOnPause = false; // Start the timer

            MyGame.WaitThenDo(Wait, () =>
                TutorialHelper.ReadyGo(MyGame, End));
        }

        void End()
        {
            PauseGame = false;
            MyGame.WaitThenDo(25, () => SavePrincessRush.Timer.PauseOnPause = true);

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}