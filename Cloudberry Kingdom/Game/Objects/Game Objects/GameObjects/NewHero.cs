using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class NewHero_GUI : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            Vector2 shift = new Vector2(0, -.5f * 2000 + 380 + 500);

            // Add the text
            text.Pos = shift;
            MyPile.Add(text);

            // Scale to fit
            Vector2 size = text.GetWorldSize();
            float max = MyGame.Cam.GetWidth() - 400;
            if (size.X > max)
                text.Scale *= max / size.X;

            // Slide out
			//this.SlideOut(PresetPos.Left, 0);
			SlideIn(0);
			MyPile.Alpha = 0;

            // Sound
            Tools.SoundWad.FindByName("HeroUnlockedSound").Play();

            if (Perma)
                this.SlideIn(0);
        }

        public EzText text;
        public NewHero_GUI(string str) { Init(str, Vector2.Zero, 1f, false); }
        public NewHero_GUI(string str, Vector2 shift, float scale, bool perma) { Init(str, shift, scale, perma); }

        public static NewHero_GUI HeroTitle(string str)
        {
            var title = new NewHero_GUI(str, new Vector2(150, -130), 1f, false);
            title.SlideInLength = 55;

            return title;
        }

        bool Perma;
        void Init(string str, Vector2 shift, float scale, bool perma)
        {
			shift += new Vector2(0, -350);

            SlideInLength = 84;

            this.Perma = perma;

            //Core.DrawLayer++; // Draw above cheering berries
            PauseOnPause = true;

            MyPile = new DrawPile();
            EnsureFancy();
            MyPile.Pos += shift;

            Tools.Warning(); // May be text, rather than Localization.Words
            text = new EzText(str, Resources.Font_Grobold42, 3000, true, true, .55f);
            text.Scale *= scale;

            text.MyFloatColor = new Color(26, 188, 241).ToVector4();
            text.OutlineColor = new Color(255, 255, 255).ToVector4();

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
			if (MyLevel.MyLevelSeed.ShowChapterName  && Count == 120 ||
				!MyLevel.MyLevelSeed.ShowChapterName && Count == 40)
            {
                //SlideIn();
				//SlideIn(0);
				//MyPile.BubbleUp(false);
				MyPile.FadeIn(.02f);
            }

			if (MyLevel.MyLevelSeed.ShowChapterName && Count == 180 ||
				!MyLevel.MyLevelSeed.ShowChapterName && Count == 150)
            {
                //SlideOut(PresetPos.Right, 160);
                //ReleaseWhenDone = true;

                //MyPile.BubbleDownAndFade(true);
                //ReleaseWhenDoneScaling = true;
				MyPile.FadeOut(.025f);
            }

			if (Count == 400)
				Release();
        }
    }
}