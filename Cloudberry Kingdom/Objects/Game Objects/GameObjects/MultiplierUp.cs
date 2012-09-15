using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class MultiplierUp : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            Vector2 shift = new Vector2(0, -800);

            // Add the text
            //text.Pos = shift;
            MyPile.Pos = shift;
            MyPile.Add(text);

            // Scale to fit
            Vector2 size = text.GetWorldSize();
            float max = MyGame.Cam.GetWidth() - 400;
            if (size.X > max)
                text.Scale *= max / size.X;

            // Slide out
            this.SlideOut(PresetPos.Bottom, 0);

            if (Perma)
                this.SlideIn(0);
        }

        EzText text;
        public MultiplierUp(string str) { Init(str, Vector2.Zero, 1f, false); }
        public MultiplierUp(string str, Vector2 shift, float scale, bool perma) { Init(str, shift, scale, perma); }

        bool Perma;
        void Init(string str, Vector2 shift, float scale, bool perma)
        {
            SlideInLength = 84;

            this.Perma = perma;

            //Core.DrawLayer++; // Draw above cheering berries
            PauseOnPause = true;

            MyPile = new DrawPile();
            EnsureFancy();
            MyPile.Pos += shift;

            text = new EzText(str, Tools.Font_Grobold42, true, true);
            text.Scale *= scale;

            //// Happy Blue
            //text.MyFloatColor = new Color(26, 188, 241).ToVector4();
            //text.OutlineColor = new Color(255, 255, 255).ToVector4();

            // Red
            CampaignMenu.Red(text);

            text.Shadow = true;
            text.ShadowOffset = new Vector2(10.5f, 10.5f);
            text.ShadowColor = new Color(30, 30, 30);
        }

        int Count = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            Count++;

            // Make sure we're on top
            if (!Core.Released && Core.MyLevel != null)
                Core.MyLevel.MoveToTopOfDrawLayer(this);

            // Do nothing if this is permanent
            if (Perma) return;

            // Otherwise show and hide
            if (Count == 4)
            {
                SlideIn(0);
                MyPile.BubbleUp(false);
            }

            if (Count == 180)
            {
                SlideOut(PresetPos.Bottom, 160);
                ReleaseWhenDone = true;
            }
        }
    }
}