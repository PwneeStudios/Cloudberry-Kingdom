using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ChapterTitle : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            // Add the text
            MyPile.Add(text);

            // Slide out
			//this.SlideOut(PresetPos.Bottom, 0);
			SlideIn(0);
			MyPile.Alpha = 0;
        }

        public EzText text;
        public ChapterTitle(Localization.Words word) { Init(Localization.WordString(word), Vector2.Zero, 1f); }

        void Init(string str, Vector2 shift, float scale)
        {
			//SlideInLength = 84;

            PauseOnPause = true;

            MyPile = new DrawPile();
            EnsureFancy();
            MyPile.Pos += shift;

            Tools.Warning(); // May be text, rather than Localization.Words
            text = new EzText(str, Resources.Font_Grobold42, true, true);
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

            // Otherwise show and hide
			if (Count == 50)
			{
				//SlideIn();
				MyPile.FadeIn(.02f);
			}

            if (Count == 235)
            {
                //SlideOut(PresetPos.Bottom, 160);
				MyPile.FadeOut(.025f);
				//ReleaseWhenDone = true;
            }

			if (Count == 400)
				Release();
        }
    }
}