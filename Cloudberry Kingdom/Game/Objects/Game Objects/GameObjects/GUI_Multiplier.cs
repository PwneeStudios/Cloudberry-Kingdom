using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_Multiplier : GUI_Panel
    {
        /// <summary>
        /// Return a string representation of the Multiplier
        /// </summary>
        public override string ToString()
        {
            return "x" +
                GetMultiplier().ToString();
        }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();
            
            if (!AddedOnce)
            {
                if (DoSlideIn)
                {
                    SlideOut(PresetPos.Right, 0);
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

        protected virtual int GetMultiplier() { return Multiplier; }

        int Multiplier;
        void SetMultiplier(int Multiplier)
        {
            if (this.Multiplier != Multiplier)
            {
                this.Multiplier = Multiplier;
                UpdateMultiplierText();
            }
        }

        EzText MultiplierText;
        void UpdateMultiplierText()
        {
            MultiplierText.SubstituteText(ToString());
        }

        bool DoSlideIn = true;
        public GUI_Multiplier(int Style) { DoInit(Style, false); }
        public GUI_Multiplier(int Style, bool SlideIn) { DoInit(Style, SlideIn); }
        
        void DoInit(int Style, bool SlideIn)
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
                scale = .5f;
                c = new Color(228, 0, 69);
                o = Color.White;
            }

            if (Style == 0)
            {
                MultiplierText = new EzText(ToString(), Resources.Font_Grobold42, 950, false, true);
                MultiplierText.Scale = .95f;
                MultiplierText.Pos = new Vector2(187, 130);
                MultiplierText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            }
            else if (Style == 1)
            {
                MultiplierText = new EzText(ToString(), font, 950, false, true);
                MultiplierText.Scale = scale;
                MultiplierText.Pos = new Vector2(381.4434f, 85.55492f);
                MultiplierText.MyFloatColor = c.ToVector4();
                MultiplierText.OutlineColor = o.ToVector4();
            }

            MultiplierText.RightJustify = true;

            MyPile.Add(MultiplierText);
        }

        protected override void MyDraw()
        {
            // Skip completely
            //return;

            if (!CoreData.Show || CoreData.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            //this.MultiplierText.MyFloatColor = new Color(100, 100, 200).ToVector4();

            if (Hid || !Active) return;

            if (CoreData.MyLevel.Watching || CoreData.MyLevel.Finished) return;

            SetMultiplier((int)(MyGame.ScoreMultiplier + .1f));
        }
    }
}