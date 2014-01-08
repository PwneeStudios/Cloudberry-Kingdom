using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GUI_CampaignLevel : GUI_Level
    {
        public GUI_CampaignLevel() : base(false) { }

        public override void OnAdd()
        {
            base.OnAdd();

            UpdateLevelText();

			// SetPos()
			EzText _t;
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(0f, 0f); _t.Scale = 0.55f; }
			MyPile.Pos = new Vector2(1590.556f, 803.2224f);

			// Extra squeeze
			Vector2 squeeze = new Vector2(-15, -15) * CloudberryKingdomGame.GuiSqueeze;

			MyPile.Pos += squeeze;
        }

        public override string ToString()
        {
			return base.ToString();
			//if (MyGame == null) return "   ";

			//return MyGame.MyLevel.Name;
        }
    }

    public class GUI_Level : GUI_Panel
    {
        public Localization.Words Prefix = Localization.Words.Level;

        /// <summary>
        /// Return a string representation of the Level
        /// </summary>
        public override string ToString()
        {
            Tools.Warning();
            return Localization.WordString(Prefix) + " " + Level.ToString();
        }

        bool DoSlideIn = true;

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

        int Level;
        public void SetLevel(int Level)
        {
            if (this.Level != Level)
            {
                this.Level = Level;
                UpdateLevelText();
            }
        }

        public EzText LevelText;
        protected void UpdateLevelText()
        {
            LevelText.SubstituteText(ToString());
        }

        public GUI_Level(bool TimeCrisis) { DoInit(false, TimeCrisis); }
        public GUI_Level(int LevelNum)
        {
            DoInit(false, false);
            PreventRelease = false;

            SetLevel(LevelNum);
        }

        void DoInit(bool SlideIn, bool TimeCrisis)
        {
            DoSlideIn = SlideIn;

            MyPile = new DrawPile();
            EnsureFancy();

			if (TimeCrisis)
				MyPile.Pos = new Vector2(1621.112f, 711.5558f);
			else
				MyPile.Pos = new Vector2(1590.556f, 803.2224f);

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;


            EzFont font;
            float scale;
            Color c, o;

            if (false)
            {
                font = Resources.Font_Grobold42;
                scale = .5f;
                c = Color.White;
                o = Color.Black;
            }
            else
            {
                font = Resources.Font_Grobold42;
                scale = .55f;
                c = new Color(228, 0, 69);
                o = Color.White;
            }

            LevelText = new EzText(ToString(), font, 950, false, true);
            LevelText.Scale = scale;
            LevelText.MyFloatColor = c.ToVector4();
            LevelText.OutlineColor = o.ToVector4();


            LevelText.RightJustify = true;

            MyPile.Add(LevelText, "Level");
        }

        protected override void MyDraw()
        {
            if (!CoreData.Show || CoreData.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Hid || !Active) return;

            if (CoreData.MyLevel.Watching || CoreData.MyLevel.Finished) return;
        }
    }
}