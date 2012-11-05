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
        }

        public override string ToString()
        {
            if (MyGame == null) return "   ";

            return MyGame.MyLevel.Name;
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

        public GUI_Level() { DoInit(false); }
        public GUI_Level(bool SlideIn) { DoInit(SlideIn); }

        public GUI_Level(int LevelNum)
        {
            DoInit(false);
            PreventRelease = false;

            SetLevel(LevelNum);
        }

        void DoInit(bool SlideIn)
        {
            DoSlideIn = SlideIn;

            MyPile = new DrawPile();
            EnsureFancy();

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

            MyPile.Add(LevelText);
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
        }
    }
}