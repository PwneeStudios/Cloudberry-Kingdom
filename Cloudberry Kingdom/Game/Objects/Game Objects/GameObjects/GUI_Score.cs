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
            int Score = PlayerManager.PlayerSum(new RunningCampaignScoreLambda());
            return Score;
        }

        class RunningCampaignScoreLambda : PlayerIntLambda
        {
            public override int Apply(PlayerData p)
            {
                return p.RunningCampaignScore();
            }
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
            }
        }

        EzText ScoreText;
        void UpdateScoreText()
        {
            ScoreText.SubstituteText(ToString());
        }

        bool DoSlideIn = true;
        public GUI_Score() { DoInit(false); }
        public GUI_Score(bool SlideIn) { DoInit(SlideIn); }
        
        void DoInit(bool SlideIn)
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
                ScoreText.Scale = scale;
                ScoreText.Pos = new Vector2(381.4434f, 85.55492f);
                ScoreText.MyFloatColor = c.ToVector4();
                ScoreText.OutlineColor = o.ToVector4();

            ScoreText.RightJustify = true;

            MyPile.Add(ScoreText);
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