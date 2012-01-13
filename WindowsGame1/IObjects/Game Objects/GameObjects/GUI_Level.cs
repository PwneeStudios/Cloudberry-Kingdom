using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GUI_CampaignLevel : GUI_Level
    {
        public GUI_CampaignLevel() : base(SlideIn: false) { }

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
        public string Prefix = "Level ";

        /// <summary>
        /// Return a string representation of the Level
        /// </summary>
        public override string ToString()
        {
            return Prefix + Level.ToString();
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
        
        public GUI_Level(bool SlideIn = false)//true)
        {
            DoSlideIn = SlideIn;

            MyPile = new DrawPile();
            EnsureFancy();

            //MyPile.Pos = new Vector2(1235, 731);
            MyPile.Pos = new Vector2(1590.556f, 803.2224f);

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            //LevelText = new EzText(ToString(), Tools.Font_DylanThin42, 950, false, true);
            //LevelText.Scale = .6f;
            //LevelText.Pos = new Vector2(187, 130);
            //LevelText.MyFloatColor = new Color(50, 170, 250).ToVector4();
            //LevelText.OutlineColor = new Color(0, 0, 0).ToVector4();


            LevelText = new EzText(ToString(), Tools.Font_DylanThin42, 950, false, true);
            LevelText.Scale = .5f;
            //LevelText.Pos = new Vector2(187, 130);
            //LevelText.Pos = new Vector2(55.55542f, -55.55557f);
            LevelText.MyFloatColor = new Color(255, 255, 255).ToVector4();


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