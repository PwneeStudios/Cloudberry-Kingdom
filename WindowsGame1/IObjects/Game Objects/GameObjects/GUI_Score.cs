using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_CampaignScore : GUI_Score
    {
        public GUI_CampaignScore() : base(1, SlideIn:false) { }

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
            return "Score " +
                GetScore().ToString();
                //Score.ToString();
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
        public GUI_Score(int Style, bool SlideIn = false)//true)
        {
            DoSlideIn = SlideIn;

            MyPile = new DrawPile();
            EnsureFancy();

            //MyPile.Pos = new Vector2(1115, 820);
            MyPile.Pos = new Vector2(1235, 820);

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            //ScoreText = new EzText(ToString(), Tools.Font_DylanThin42, 950, false, true);
            //ScoreText.Scale = .6f;
            //ScoreText.Pos = new Vector2(187, 130);
            //ScoreText.MyFloatColor = new Color(50, 170, 250).ToVector4();
            //ScoreText.OutlineColor = new Color(0, 0, 0).ToVector4();

            if (Style == 0)
            {
                ScoreText = new EzText(ToString(), Tools.MonoFont, 950, false, true);
                ScoreText.Scale = .95f;
                ScoreText.Pos = new Vector2(187, 130);
                ScoreText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            }
            else if (Style == 1)
            {
                ScoreText = new EzText(ToString(), Tools.Font_DylanThin42, 950, false, true);
                ScoreText.Scale = .5f;// .625f;
                ScoreText.Pos = new Vector2(381.4434f, 85.55492f);
                ScoreText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            }

            ScoreText.RightJustify = true;

            MyPile.Add(ScoreText);
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Hid || !Active) return;

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            if (MyGame.AlwaysGiveCoinScore)
                SetScore(PlayerManager.GetGameScore());
            else
                SetScore(PlayerManager.GetGameScore_WithTemporary());
        }
    }
}