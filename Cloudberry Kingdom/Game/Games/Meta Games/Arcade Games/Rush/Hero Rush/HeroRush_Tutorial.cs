using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class HeroRush_Tutorial : GameObject
    {
        /// <summary>
        /// Whether the Hero Rush introduction has been watched before.
        /// </summary>
        public static bool WatchedOnce { get { return _WatchedOnce; } set { _WatchedOnce = value; PlayerManager.SavePlayerData.Changed = true; } }
        static bool _WatchedOnce = false;

        /// <summary>
        /// When true the tutorial will skip the long version.
        /// The bool is then set to false.
        /// </summary>
        public static bool TemporarySkip = false;

        static public bool ShowTitle = true;

        /// <summary>
        /// Whether text makes a popping sound when we kill it
        /// </summary>
        protected bool SoundOnKill = false;

        Challenge_HeroRush HeroRush;
        public HeroRush_Tutorial(Challenge_HeroRush HeroRush)
        {
            this.HeroRush = HeroRush;

            HeroRush.Timer.Hide();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            PauseGame = true;

            foreach (Bob bob in MyGame.MyLevel.Bobs)
                bob.Core.Show = false;
            
            // Start the music
            MyGame.WaitThenDo(20, () =>
                {
                    Tools.SongWad.SuppressNextInfoDisplay = true;
                    Tools.SongWad.SetPlayList(Tools.Song_140mph);
                    Tools.SongWad.Restart(true);
                });

            if (ShowTitle || !WatchedOnce || CloudberryKingdom_XboxPC.AlwaysGiveTutorials)
                MyGame.WaitThenDo(27, () => Title());
            else
                MyGame.WaitThenDo(20, () => Ready());
        }

        protected void TutorialOrSkip()
        {
            if (!CloudberryKingdom_XboxPC.AlwaysGiveTutorials && (WatchedOnce || TemporarySkip))
                Ready();
            else
            {
                StartTutorial();

                WatchedOnce = true;
            }

            TemporarySkip = false;
        }

        void StartTutorial()
        {
            PointAtDoor();
        }

        protected virtual void Title()
        {
            ShowTitle = false;

            GUI_Text text = GUI_Text.SimpleTitle("Hero\n  Rush");

            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
            {
                //MyGame.WaitThenDo(18, () => PointAtDoor());
                MyGame.WaitThenDo(12, () => TutorialOrSkip());
                text.Kill(SoundOnKill);
            }));
        }

        void PointAtDoor()
        {
            //HeroRush.Timer.Show();

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

                MyGame.WaitThenDo(7, () => HeroRush.Timer.Show());
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
            //Vector2 timerpos = HeroRush.Timer.ApparentPos;
            Vector2 timerpos = MyGame.CamPos + new Vector2(-60, 1000);

            Arrow arrow = new Arrow();
            arrow.SetOrientation(Arrow.Orientation.Right);
            arrow.Move(timerpos + new Vector2(30, -655));
            arrow.PointTo(timerpos);
            MyGame.AddGameObject(arrow);

            GUI_Text text = new GUI_Text(
                string.Format("{0} seconds\n   on the clock!", HeroRush.Timer.Seconds),
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
                PointAtScore();
                arrows.ForEach(arrow => arrow.Release());
                text.Kill(SoundOnKill);
            }));
        }

        void PointAtScore()
        {
            GUI_Score score = MyGame.MyGameObjects.Find(obj => obj is GUI_Score) as GUI_Score;
            if (null == score) { End(); return; }

            Vector2 scorepos = score.MyPile.FancyPos.AbsVal + new Vector2(-60, 40);

            Arrow arrow = new Arrow();
            arrow.SetOrientation(Arrow.Orientation.Right);
            arrow.Move(scorepos + new Vector2(-510, -430));
            arrow.PointTo(scorepos);
            MyGame.AddGameObject(arrow);

            GUI_Text text = new GUI_Text("Get a high score!",
                arrow.Core.Data.Position + new Vector2(-500, -100) + new Vector2(-38.88892f, -150f));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, () =>
            {
                MyGame.WaitThenDo(0, () => Ready());
                //Ready();
                arrow.Release();
                text.Kill(false);
            }));
        }

        void Ready()
        {
            int Wait = 5 + 22;
            if (HeroRush.Timer.Hid) Wait = 28 + 12;

            HeroRush.Timer.Show();
            HeroRush.Timer.PauseOnPause = false; // Start the timer

            MyGame.WaitThenDo(Wait, () =>
                TutorialHelper.ReadyGo(MyGame, End));

            //MyGame.WaitThenDo(Wait, () =>
            //{
            //    GUI_Text text = new GUI_Text("Ready?", MyGame.CamPos + ReadyGoPos);
            //    MyGame.AddGameObject(text);

            //    MyGame.WaitThenDo(36, () => text.Kill(false));
            //    MyGame.WaitThenDo(40, () => LetsGo());
            //});
        }

        //void LetsGo()
        //{
        //    GUI_Text text = new GUI_Text("Go!", MyGame.CamPos + ReadyGoPos);
        //    MyGame.AddGameObject(text);

        //    MyGame.WaitThenDo(20, () => End());
        //    MyGame.WaitThenDo(30, () => text.Kill(false));
        //}

        void End()
        {
            PauseGame = false;
            MyGame.WaitThenDo(25, () => HeroRush.Timer.PauseOnPause = true);

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}