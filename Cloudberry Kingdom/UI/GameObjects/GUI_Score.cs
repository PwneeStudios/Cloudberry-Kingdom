using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_CampaignScore : GUI_Score
    {
        public GUI_CampaignScore() : base(false)
        {
            PreventRelease = false;
            UpdateAfterLevelFinish = true;
        }

        protected override int GetScore()
        {
            int Score = PlayerManager.PlayerSum(p => p.RunningCampaignScore());
            return Score;
        }
    }

    public class GUI_Score : GUI_Panel
    {
        /// <summary>
        /// Return a string representation of the score
        /// </summary>
        public override string ToString()
        {
            Tools.Warning();
            return Localization.WordString(Localization.Words.Score) + " " +
                GetScore().ToString();
        }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();
            
            if (!AddedOnce)
            {
                if (DoSlideIn)
                {
                    SlideOut(PresetPos.Top, 0);
                    SlideIn();
                    Show();
                }
                else
                {
                    SlideIn(0);
                    Show();
                }
            }

            AddedOnce = true;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
        }

        protected virtual int GetScore() { return Score; }

        int Score;
        void SetScore(int Score)
        {
            if (this.Score != Score)
            {
                this.Score = Score;
                UpdateScoreText();

                Challenge.CurrentScore = Score;
            }
        }

        EzText ScoreText;
        void UpdateScoreText()
        {
            ScoreText.SubstituteText(ToString());
        }

        bool DoSlideIn = true;
        public GUI_Score(bool TimeCrisis) { DoInit(false, TimeCrisis); }
        
        void DoInit(bool SlideIn, bool TimeCrisis)
        {
            DoSlideIn = SlideIn;

            MyPile = new DrawPile();
            EnsureFancy();

            MyPile.Pos = new Vector2(1235, 820);

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            EzFont font;
            float scale;
            Color c, o;

                font = Resources.Font_Grobold42;
                scale = .5f;
                c = new Color(228, 0, 69);
                o = Color.White;

                ScoreText = new EzText(ToString(), font, 950, false, true);
				ScoreText.Name = "Score";
                ScoreText.Scale = scale;
                ScoreText.Pos = new Vector2(381.4434f, 85.55492f);
                ScoreText.MyFloatColor = c.ToVector4();
                ScoreText.OutlineColor = o.ToVector4();

            ScoreText.RightJustify = true;

            MyPile.Add(ScoreText);


			if (TimeCrisis)
			{
				EzText _t;
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(396.5f, 85.55492f); _t.Scale = 0.5f; }
				MyPile.Pos = new Vector2(1240.555f, 756.1112f);
			}
			else
			{
				EzText _t;
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(381.4434f, 85.55492f); _t.Scale = 0.5f; }
				MyPile.Pos = new Vector2(865.5554f, 781.1111f);
			}

			// Extra squeeze
			Vector2 squeeze = new Vector2(-15, -15) * CloudberryKingdomGame.GuiSqueeze;

			MyPile.Pos += squeeze;
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected bool UpdateAfterLevelFinish = false;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Hid || !Active) return;

            if (Core.MyLevel.Watching || (Core.MyLevel.Finished && !UpdateAfterLevelFinish)) return;

            if (MyGame.AlwaysGiveCoinScore)
                SetScore(PlayerManager.GetGameScore());
            else
                SetScore(PlayerManager.GetGameScore_WithTemporary());
        }
    }
}