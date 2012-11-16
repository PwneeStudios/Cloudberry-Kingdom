using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;




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
        public Escalation_Tutorial(Challenge_Escalation Escalation)
        {
            this.Escalation = Escalation;
        }

        class ConfigureSongsHelper : Lambda
        {
            public void Apply()
            {
                Tools.SongWad.SuppressNextInfoDisplay = true;
                Tools.SongWad.SetPlayList(Tools.SongList_Standard);
                //Tools.SongWad.SetPlayList(Tools.Song_140mph);
                Tools.SongWad.Restart(true);
            }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            if (MyGame.MyLevel.StartDoor == null)
                WatchedOnce = true;

            // Add the princess
            if (!WatchedOnce)
            {
                Level lvl = MyGame.MyLevel;

                lvl.PreventReset = true;
            }
            else
                PauseGame = true;

            if (MyGame.MyLevel.StartDoor != null)
                MyGame.HideBobs();
            MyGame.PhsxStepsToDo += 2;
            
            // Start the music
            MyGame.WaitThenDo(20, new ConfigureSongsHelper());

            if (HeroRush_Tutorial.ShowTitle || !WatchedOnce)
            //if (ShowTitle || !WatchedOnce)
                MyGame.WaitThenDo(27, new TitleProxy(this));
            else
                MyGame.WaitThenDo(20, new ReadyProxy(this));
        }

        class PreventThingsHelper : Lambda
        {
            Escalation_Tutorial et;

            public PreventThingsHelper(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                et.MyGame.MyLevel.PreventHelp = false;
                et.MyGame.MyLevel.PreventReset = false;
                et.MyGame.MyLevel.Finished = false;
            }
        }

        protected void TutorialOrSkip()
        {
            if (!WatchedOnce && MyGame.MyLevel.StartDoor != null)
            {
                MyGame.MyLevel.PreventReset = MyGame.MyLevel.PreventHelp = true;

                int wait = MyGame.DramaticEntry(MyGame.MyLevel.StartDoor, 90);
                MyGame.MyLevel.SetBack(wait + 90);
                MyGame.WaitThenDo(wait, new PreventThingsHelper(this));
            }
            else
                Ready();

            WatchedOnce = true;
        }

        class TitleProxy : Lambda
        {
            Escalation_Tutorial et;

            public TitleProxy(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                et.Title();
            }
        }

        class TutorialOrSkipProxy : Lambda
        {
            Escalation_Tutorial et;

            public TutorialOrSkipProxy(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                et.TutorialOrSkip();
            }
        }

        class NextTutorialHelper : Lambda
        {
            Escalation_Tutorial et;
            GUI_Text text;

            public NextTutorialHelper(Escalation_Tutorial et, GUI_Text text)
            {
                this.et = et;
                this.text = text;
            }

            public void Apply()
            {
                et.MyGame.WaitThenDo(12, new TutorialOrSkipProxy(et));
                text.Kill(et.SoundOnKill);
            }
        }

        class TextKillHelper : Lambda
        {
            Escalation_Tutorial et;
            GUI_Text text;

            public TextKillHelper(Escalation_Tutorial et, GUI_Text text)
            {
                this.et = et;
                this.text = text;
            }

            public void Apply()
            {
                text.Kill(et.SoundOnKill);
            }
        }

        protected virtual void Title()
        {
            ShowTitle = false;

            if (WatchedOnce)
            {
                GUI_Text text = GUI_Text.SimpleTitle(Escalation.Name);
                CkColorHelper._x_x_HappyBlueColor(text.MyText);

                MyGame.AddGameObject(text);

                // On (A) go to next part of the tutorial
                MyGame.AddGameObject(new Listener(ControllerButtons.A, new NextTutorialHelper(this, text)));
            }
            else
            {
                GUI_Text text = GUI_Text.SimpleTitle(Escalation.Name);
                CkColorHelper._x_x_HappyBlueColor(text.MyText);
                text.Pos.RelVal.Y -= 800;

                MyGame.AddGameObject(text);

                MyGame.WaitThenDo(120, new TextKillHelper(this, text));
                MyGame.WaitThenDo(40, new TutorialOrSkipProxy(this));
            }
        }

        class ReadyProxy : Lambda
        {
            Escalation_Tutorial et;

            public ReadyProxy(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                et.Ready();
            }
        }

        class TutorialHelperReadyGo : Lambda
        {
            Escalation_Tutorial et;

            public TutorialHelperReadyGo(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                TutorialHelper.ReadyGo(et.MyGame, new EndProxy(et));
            }
        }

        void Ready()
        {
            int Wait = 5 + 22;

            MyGame.WaitThenDo(Wait, new TutorialHelperReadyGo(this));
        }

        class EndProxy : Lambda
        {
            Escalation_Tutorial et;

            public EndProxy(Escalation_Tutorial et)
            {
                this.et = et;
            }

            public void Apply()
            {
                et.End();
            }
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