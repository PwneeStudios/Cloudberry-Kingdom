using Microsoft.Xna.Framework;
using System;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GUI_LivesLeft : GUI_Panel
    {
        public Action<GUI_LivesLeft> OnOutOfLives;

        int _NumLives = 2;
        public int NumLives
        {
            get { return _NumLives; }
            set
            {
                _NumLives = value;
                UpdateLivesLeftText();
            }
        }
        
        /// <summary>
        /// Returns a string representation of the number of lives left
        /// </summary>
        public override string ToString()
        {
            return "x " + NumLives.ToString();
        }

        EzText LivesLeftText;
        void UpdateLivesLeftText()
        {
            LivesLeftText.SubstituteText(ToString());
        }

        public GUI_LivesLeft(int Lives)
        {
            SetParams();

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            _NumLives = Lives;

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            QuadClass Backdrop = new QuadClass("cloud1", 190, true);
            Backdrop.Pos = new Vector2(144.4448f, 50f);
            Backdrop.Size = new Vector2(634.4441f, 494.4075f);
            MyPile.Add(Backdrop);

            QuadClass Stickman = new QuadClass("Score\\Stickman", 206, true);
            Stickman.Pos = new Vector2(10, 2);
            MyPile.Add(Stickman);

            LivesLeftText = new EzText(ToString(), Tools.Font_Dylan60, 450, false, true);
            LivesLeftText.Scale = .74f;
            LivesLeftText.Pos = new Vector2(187, -16);
            LivesLeftText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            LivesLeftText.OutlineColor = new Color(0, 0, 0).ToVector4();

            //LivesLeftText.Shadow = true;
            LivesLeftText.ShadowOffset = new Vector2(-11f, 11f);
            LivesLeftText.ShadowColor = new Color(65, 65, 65, 160);
            LivesLeftText.PicShadow = false;
            MyPile.Add(LivesLeftText);

            MyPile.Pos = new Vector2(972.2213f, -594.4446f);

            // Hide initially
            MyPile.Alpha = 0;
        }

        bool UseBlackBack = false;
        float FadeInVel, FadeOutVel;
        int InitialDelay, ShowLength, StartDelay;
        void SetParams()
        {
            FadeInVel = .04325f;
            FadeOutVel = -FadeInVel;

            //UseBlackBack = true;
            //FadeInVel = 1f;
            //FadeOutVel = -.07f;

            InitialDelay = 20;
            ShowLength = 90;
            StartDelay = 30;
        }

        bool PauseOnShow = false;
        public void Bring()
        {
            if (PauseOnShow)
            {
                PauseLevel = true;
                MyGame.MyLevel.PreventReset = true;
            }

            // Fade in and out
            MyPile.Alpha = 0;
            MyGame.WaitThenDo(InitialDelay, () => {
                MyPile.AlphaVel = FadeInVel;
                MyGame.CinematicToDo(ShowLength, () => {
                    MyPile.AlphaVel = FadeOutVel;
                    MyGame.CinematicToDo(StartDelay, () => {
                        if (PauseOnShow)
                        {
                            PauseLevel = false;
                            MyGame.MyLevel.PreventReset = false;
                        }
                    });
                });
            }, "Start lives left bring", true, false);
        }


        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            if (!Core.MyLevel.Watching && Core.MyLevel.PlayMode == 0)
                Bring();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            // Black background
            if (UseBlackBack)
            {
                QuadClass Black = new QuadClass("White", 1);
                Black.FullScreen(MyGame.Cam);
                Black.Quad.SetColor(Color.Black);
                Black.Scale(2);
                MyPile.Insert(0, Black);
            }

            MyGame.ToDoOnReset.Add(OnReset);
            //MyGame.ToDoOnDoneDying.Add(OnDoneDying);
            MyGame.ToDoOnDeath.Add(OnDeath);

            if (MyGame.MyLevel != null)
                PreventResetOnLastLife(MyGame.MyLevel);
        }

        int LastLife = 0;
        void OnReset()
        {
            MyGame.ToDoOnReset.Add(OnReset);

            Level level = Core.MyLevel;

            if (!MyGame.FreeReset)
                NumLives--;

            PreventResetOnLastLife(level);
        }

        private void PreventResetOnLastLife(Level level)
        {
            // If only 1 life remains.
            if (NumLives == LastLife)
            {
                // Prevent quickspawn and watching the computer from afar
                level.MyGame.SuppressQuickSpawn_External = true;
                level.CanWatchComputerFromAfar_External = false;
            }
            else if (NumLives > LastLife)
            {
                level.MyGame.SuppressQuickSpawn_External = false;
                level.CanWatchComputerFromAfar_External = true;
            }
        }

        void OnDoneDying()
        {
            MyGame.ToDoOnDoneDying.Add(OnDoneDying);

            if (NumLives == LastLife)
            {
                //Core.MyLevel.MyGame.AddGameObject(new GameOverPanel());
                if (OnOutOfLives != null)
                    OnOutOfLives(this);

                Release();
                return;
            }
        }
        void OnDeath()
        {
            MyGame.ToDoOnDeath.Add(OnDeath);

            if (NumLives == LastLife)
            {
                //Core.MyLevel.MyGame.AddGameObject(new GameOverPanel());
                if (OnOutOfLives != null)
                    OnOutOfLives(this);

                Release();
                return;
            }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (MyGame.MyLevel != null)
                PreventResetOnLastLife(MyGame.MyLevel);
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }
    }
}