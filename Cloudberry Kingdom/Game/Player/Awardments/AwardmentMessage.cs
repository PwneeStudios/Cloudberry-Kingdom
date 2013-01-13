using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Awards
{
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

            EzText Title, Description;

            Title = new EzText(award.TitleType, Resources.Font_Grobold42_2, 1800, false, false, .575f);
            Title.Pos = new Vector2(-1726.192f, 300);
            Title.Scale *= .6f;
            MyPile.Add(Title);

            Description = new EzText(award.Description, Resources.Font_Grobold42_2, 1800, true, true, .575f);
            Description.Pos = new Vector2(0, 100);
            Description.Scale *= .6f;
            MyPile.Add(Description);
        }

        protected virtual void MakeBackdrop()
        {
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "WidePlaque";
            Backdrop.Size = new Vector2(1750f, 284.8255f);
            Backdrop.Pos = new Vector2(-11.9043f, 59.52365f);

            MyPile.Add(Backdrop);
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