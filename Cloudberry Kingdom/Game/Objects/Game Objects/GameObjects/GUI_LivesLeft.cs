using Microsoft.Xna.Framework;
using System;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    /// <summary>
    /// A GUI element that tracks how many lives are left.
    /// Element also shows how many lives are left, displayed after all players die and the level resets.
    /// </summary>
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

            MyPile.Add(new QuadClass("Bob_Stand_0001", 130, true), "Bob");

            LivesLeftText = new EzText(ToString(), Resources.Font_Grobold42, 450, false, true);
            LivesLeftText.Name = "Text";
            LivesLeftText.Scale = .53f;
            LivesLeftText.Pos = new Vector2(187, -16);
            LivesLeftText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            LivesLeftText.OutlineColor = new Color(0, 0, 0).ToVector4();

            LivesLeftText.ShadowOffset = new Vector2(-11f, 11f);
            LivesLeftText.ShadowColor = new Color(65, 65, 65, 160);
            LivesLeftText.PicShadow = false;
            MyPile.Add(LivesLeftText);

            SetPos();

            // Hide initially
            MyPile.Alpha = 0;
        }

        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("Text"); if (_t != null) { _t.Pos = new Vector2(195.3334f, -43.77777f); }

            QuadClass _q;
            _q = MyPile.FindQuad("Bob"); if (_q != null) { _q.Pos = new Vector2(133.3333f, -27.77774f); _q.ScaleYToMatchRatio(82.5f); }

            MyPile.Pos = new Vector2(-222.2232f, 897.222f);
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
		float FadeInMult;
        public void Bring(bool PlusOne)
        {
			if (PlusOne)
			{
				FadeInMult = 2;

				LivesLeftText.MyFloatColor = new Color(84, 232, 79).ToVector4();
				LivesLeftText.OutlineColor = new Color(0, 0, 0).ToVector4();
				CkColorHelper._x_x_HappyBlueColor(LivesLeftText);
			}
			else
			{
				FadeInMult = 1;

				LivesLeftText.MyFloatColor = new Color(255, 255, 255).ToVector4();
				LivesLeftText.OutlineColor = new Color(0, 0, 0).ToVector4();
			}

            if (PauseOnShow)
            {
                PauseLevel = true;
                MyGame.MyLevel.PreventReset = true;
            }

            // Fade in and out
            MyPile.Alpha = 0;
            MyGame.WaitThenDo(InitialDelay, () => {
                MyPile.AlphaVel = FadeInVel * FadeInMult;
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
                Bring(false);
        }

        public override void OnAdd()
        {
            base.OnAdd();

			MyPile.AlphaVel = FadeOutVel;
			MyPile.Alpha = 0;

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