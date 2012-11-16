using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;





namespace CloudberryKingdom
{
    public class HeroRush_Tutorial : GameObject
    {
        /// <summary>
        /// Whether the Hero Rush introduction has been watched before.
        /// </summary>
        public static bool HasWatchedOnce = false;
        public static void WatchedOnce() { HasWatchedOnce = true; PlayerManager.SavePlayerData.Changed = true; }

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

        class StartMusicHelper : Lambda
        {
            public void Apply()
            {
                Tools.SongWad.SuppressNextInfoDisplay = true;
                Tools.SongWad.SetPlayList(Tools.Song_140mph);
                Tools.SongWad.Restart(true);
            }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            PauseGame = true;

            // Find the initial door
            Door door = MyGame.MyLevel.FindIObject(LevelConnector.StartOfLevelCode) as Door;
            if (null != door)
            {
                foreach (Bob bob in MyGame.MyLevel.Bobs)
                    bob.Core.Show = false;
            }

            // Start the music
            MyGame.WaitThenDo(20, new StartMusicHelper());

            if (ShowTitle || !HasWatchedOnce || CloudberryKingdomGame.AlwaysGiveTutorials)
                MyGame.WaitThenDo(27, new TitleProxy(this));
            else
                MyGame.WaitThenDo(20, new ReadyProxy(this));
        }

        class TutorialOrSkipProxy : Lambda
        {
            HeroRush_Tutorial tutorial;

            public TutorialOrSkipProxy(HeroRush_Tutorial tutorial)
            {
                this.tutorial = tutorial;
            }

            public void Apply()
            {
                tutorial.TutorialOrSkip();
            }
        }

        class ListenerHelper : Lambda
        {
            HeroRush_Tutorial tutorial;
            GUI_Text text;

            public ListenerHelper(HeroRush_Tutorial tutorial, GUI_Text text)
            {
                this.tutorial = tutorial;
                this.text = text;
            }

            public void Apply()
            {
                tutorial.MyGame.WaitThenDo(12, new TutorialOrSkipProxy(tutorial));
                text.Kill(tutorial.SoundOnKill);
            }
        }

        public class AddGameObjectHelper : Lambda
        {
            HeroRush_Tutorial tutorial;
            GUI_Text text;

            public AddGameObjectHelper(HeroRush_Tutorial tutorial, GUI_Text text)
            {
                this.tutorial = tutorial;
                this.text = text;
            }

            public void Apply()
            {
                // On (A) go to next part of the tutorial
                tutorial.MyGame.AddGameObject(new Listener(ControllerButtons.A,
                    new ListenerHelper(tutorial, text)));
            }
        }

        protected void TutorialOrSkip()
        {
            if (!CloudberryKingdomGame.AlwaysGiveTutorials && (HasWatchedOnce || TemporarySkip))
                Ready();
            else
            {
                StartTutorial();

                WatchedOnce();
            }

            TemporarySkip = false;
        }

        void StartTutorial()
        {
            PointAtDoor();
        }

        class TitleProxy : Lambda
        {
            HeroRush_Tutorial hrt;

            public TitleProxy(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.Title();
            }
        }

        class TitleNextTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;
            GUI_Text text;

            public TitleNextTutorialHelper(HeroRush_Tutorial hrt, GUI_Text text)
            {
                this.hrt = hrt;
                this.text = text;
            }

            public void Apply()
            {
                hrt.MyGame.WaitThenDo(12, new TutorialOrSkipProxy(hrt));
                text.Kill(hrt.SoundOnKill);
            }
        }

        protected virtual void Title()
        {
            ShowTitle = false;

            GUI_Text text = GUI_Text.SimpleTitle(Localization.Words.HeroRush);

            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, new TitleNextTutorialHelper(this, text)));
        }

        class HeroRushTimerShowHelper : Lambda
        {
            HeroRush_Tutorial hrt;

            public HeroRushTimerShowHelper(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.HeroRush.Timer.Show();
            }
        }

        class PointAtDoorNextTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;
            Arrow arrow;
            GUI_Text text;

            public PointAtDoorNextTutorialHelper(HeroRush_Tutorial hrt, Arrow arrow, GUI_Text text)
            {
                this.hrt = hrt;
                this.arrow = arrow;
                this.text = text;
            }

            public void Apply()
            {
                arrow.Release();
                text.Kill(hrt.SoundOnKill);

                hrt.MyGame.WaitThenDo(7, new HeroRushTimerShowHelper(hrt));
                hrt.MyGame.WaitThenDo(0, new PointAtTimerProxy(hrt));
            }
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

            GUI_Text text = new GUI_Text(Localization.Words.GetToTheExit, arrow.Core.Data.Position + new Vector2(-200, 400));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, new PointAtDoorNextTutorialHelper(this, arrow, text)));
        }

        class PointAtTimerProxy : Lambda
        {
            HeroRush_Tutorial hrt;

            public PointAtTimerProxy(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.PointAtTimer();
            }
        }

        class PointAtTimerNextTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;
            Arrow arrow;
            GUI_Text text;
            GUI_Text text2;

            public PointAtTimerNextTutorialHelper(HeroRush_Tutorial hrt, Arrow arrow, GUI_Text text, GUI_Text text2)
            {
                this.hrt = hrt;
                this.arrow = arrow;
                this.text = text;
                this.text2 = text2;
            }

            public void Apply()
            {
                hrt.PointAtCoins();
                arrow.Release();
                text.Kill(hrt.SoundOnKill);
                text2.Kill(false);
            }
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
                Localization.Words.SecondsOnTheClock,
                arrow.Core.Data.Position + new Vector2(830, -130));

            GUI_Text text2 = new GUI_Text(
                HeroRush.Timer.Seconds.ToString(),
                arrow.Core.Data.Position + new Vector2(830, -130) + new Vector2(-150, 0));

            MyGame.AddGameObject(text);
            MyGame.AddGameObject(text2);
            
            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, new PointAtTimerNextTutorialHelper(this, arrow, text, text2)));
        }

        class PointAtCoinsNextTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;
            GUI_Text text;
            List<Arrow> arrows;

            public PointAtCoinsNextTutorialHelper(HeroRush_Tutorial hrt, GUI_Text text, List<Arrow> arrows)
            {
                this.hrt = hrt;
                this.text = text;
                this.arrows = arrows;
            }

            public void Apply()
            {
                hrt.PointAtScore();
                foreach (Arrow arrow in arrows)
                {
                    arrow.Release();
                }
                text.Kill(hrt.SoundOnKill);
            }
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

            GUI_Text text = new GUI_Text(Localization.Words.CoinsAddSeconds,
                Tools.CurLevel.MainCamera.Data.Position + new Vector2(0, -750));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, new PointAtCoinsNextTutorialHelper(this, text, arrows)));
        }

        class PointAtScoreNextTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;
            Arrow arrow;
            GUI_Text text;

            public PointAtScoreNextTutorialHelper(HeroRush_Tutorial hrt, Arrow arrow, GUI_Text text)
            {
                this.hrt = hrt;
                this.arrow = arrow;
                this.text = text;
            }

            public void Apply()
            {
                hrt.MyGame.WaitThenDo(0, new ReadyProxy(hrt));
                //Ready();
                arrow.Release();
                text.Kill(false);
            }
        }

        void PointAtScore()
        {
            GUI_Score score = null;
            foreach (object obj in MyGame.MyGameObjects)
            {
                if (obj is GUI_Score)
                {
                    score = obj as GUI_Score;
                    break;
                }
            }
            if (null == score) { End(); return; }

            Vector2 scorepos = score.MyPile.FancyPos.AbsVal + new Vector2(-60, 40);

            Arrow arrow = new Arrow();
            arrow.SetOrientation(Arrow.Orientation.Right);
            arrow.Move(scorepos + new Vector2(-510, -430));
            arrow.PointTo(scorepos);
            MyGame.AddGameObject(arrow);

            GUI_Text text = new GUI_Text(Localization.Words.GetAHighScore,
                arrow.Core.Data.Position + new Vector2(-500, -100) + new Vector2(-38.88892f, -150f));
            MyGame.AddGameObject(text);

            // On (A) go to next part of the tutorial
            MyGame.AddGameObject(new Listener(ControllerButtons.A, new PointAtScoreNextTutorialHelper(this, arrow, text)));
        }

        class ReadyProxy : Lambda
        {
            HeroRush_Tutorial hrt;

            public ReadyProxy(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.Ready();
            }
        }

        class ReadyTutorialHelper : Lambda
        {
            HeroRush_Tutorial hrt;

            public ReadyTutorialHelper(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                TutorialHelper.ReadyGo(hrt.MyGame, new EndProxy(hrt));
            }
        }

        void Ready()
        {
            int Wait = 5 + 22;
            if (HeroRush.Timer.Hid) Wait = 28 + 12;

            HeroRush.Timer.Show();
            HeroRush.Timer.PauseOnPause = false; // Start the timer

            MyGame.WaitThenDo(Wait, new ReadyTutorialHelper(this));
        }

        class PauseHeroRushTimerHelper : Lambda
        {
            HeroRush_Tutorial hrt;

            public PauseHeroRushTimerHelper(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.HeroRush.Timer.PauseOnPause = true;
            }
        }

        class EndProxy : Lambda
        {
            HeroRush_Tutorial hrt;

            public EndProxy(HeroRush_Tutorial hrt)
            {
                this.hrt = hrt;
            }

            public void Apply()
            {
                hrt.End();
            }
        }

        void End()
        {
            PauseGame = false;
            MyGame.WaitThenDo(25, new PauseHeroRushTimerHelper(this));

            Release();
        }

        protected override void MyPhsxStep()
        {
        }
    }
}