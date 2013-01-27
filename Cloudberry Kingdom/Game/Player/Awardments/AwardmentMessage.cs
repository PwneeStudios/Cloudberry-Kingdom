using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Awards
{
    public class HeroUnlockedMessage : AwardmentMessage
    {
        public HeroUnlockedMessage() : base(null)
        {
            MakeText("", Localization.WordString(Localization.Words.NewHeroUnlocked));


            EzText _t;
            _t = MyPile.FindEzText("Title"); if (_t != null) { _t.Pos = new Vector2(-1726.192f, 300f); _t.Scale = 0.6f; }
            _t = MyPile.FindEzText("Description"); if (_t != null) { _t.Pos = new Vector2(19.44458f, 36.11111f); _t.Scale = 0.6f; }

            QuadClass _q;
            _q = MyPile.FindQuad("ArcadeBox"); if (_q != null) { _q.Pos = new Vector2(4.763306f, 0f); _q.Size = new Vector2(919.4252f, 163.4914f); }

            MyPile.Pos = new Vector2(36.11108f, 827.7778f);
        }
    }

    public class AwardmentMessage : CkBaseMenu
    {
        protected QuadClass Backdrop;

        public AwardmentMessage(Awardment award)
        {
            Core.DrawLayer += 2;

            PauseOnPause = false;
            PauseLevel = false;
            FixedToCamera = true;
            Core.RemoveOnReset = false;

            MyPile = new DrawPile();

            MakeBackdrop();

            if (award != null)
            {
                MakeText(award.TitleType, Localization.WordString(award.Description));
            }
        }

        protected void MakeText(string TitleWord, string DescriptionWord)
        {
            EzText Title, Description;

            Title = new EzText(TitleWord, Resources.Font_Grobold42_2, 1800, false, false, .575f);
            Title.Pos = new Vector2(-1726.192f, 300);
            Title.Scale *= .6f;
            MyPile.Add(Title, "Title");

            Description = new EzText(DescriptionWord, Resources.Font_Grobold42_2, 1800, true, true, .575f);
            Description.Pos = new Vector2(0, 100);
            Description.Scale *= .6f;
            MyPile.Add(Description, "Description");
        }

        protected virtual void MakeBackdrop()
        {
            //// Backrop
            //QuadClass backdrop2 = new QuadClass("White", 1500);
            //backdrop2.Quad.SetColor(ColorHelper.GrayColor(.1f));
            //backdrop2.Alpha = .45f;
            //MyPile.Add(backdrop2, "Backdrop2");

            //QuadClass backdrop = new QuadClass("White", 1500);
            //backdrop.Quad.SetColor(ColorHelper.GrayColor(.25f));
            //backdrop.Alpha = .35f;
            //MyPile.Add(backdrop, "Backdrop");


            Backdrop = new QuadClass(null, true, true);
            //Backdrop.TextureName = "WidePlaque";
            Backdrop.TextureName = "MessageBoxThin";
            Backdrop.Size = new Vector2(1750f, 284.8255f);
            Backdrop.Pos = new Vector2(-11.9043f, 59.52365f);
            Backdrop.Degrees = 0;

            MyPile.Add(Backdrop, "ArcadeBox");
            MyPile.Pos = new Vector2(0, -800);
        }

        public override void SlideIn(int Frames)
        {
            Pos.RelVal = new Vector2(0, 0);
            Active = true;
            MyPile.BubbleUp(true);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (Frames == 0) return;

            Kill(true);
            Active = false;
        }

        protected int Step = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Step++;
            if (Step < 40) return;

            if (ShouldDie())
            {
                ReturnToCaller(false);
                PauseLevel = false;
            }
        }

        const int Duration = (int)(2.3 * 62);
        protected bool ShouldDie()
        {
            //return false;
            return Step > Duration;
        }

        public void Kill() { Kill(true); }
        public void Kill(bool sound)
        {
            MyPile.BubbleDownAndFade(sound);
            ReleaseWhenDone = false;
            ReleaseWhenDoneScaling = true;
        }
    }
}