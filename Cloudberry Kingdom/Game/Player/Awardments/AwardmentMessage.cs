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

            SetText(award);

            EzText Title;
            if (award.Unlockable == null)
                Title = new EzText("", Resources.Font_Grobold42_2, 1800, false, false, .575f);
            else
                Title = new EzText(award.Name, Resources.Font_Grobold42_2, 1800, false, false, .575f);
            Title.Pos = new Vector2(-1726.192f, 369.0475f);
            Title.Scale *= .79f;
            MyPile.Add(Title);
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

        protected EzText Text;
        public void SetText(Awardment award)
        {
            string text = award.Name;

            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            Text = new EzText(text, ItemFont, 1800, false, false, .575f);

            if (award.Unlockable == null)
            {
                Text.Scale *= 1.15f;
                MyPile.Add(Text);

                Vector2 size = Text.GetWorldSize();
                Text.Pos = new Vector2(-size.X / 2 - 350, size.Y * .85f) + new Vector2(38, 32);
            }
            else
            {
                Text.Scale *= .74f;
                MyPile.Add(Text);
                SizeAndPosition();
            }
        }

        protected virtual void SizeAndPosition()
        {
            Vector2 size = Text.GetWorldSize();
            float MaxSize = 1200;
            if (size.X > MaxSize)
                Text.Scale *= MaxSize / size.X;

            size = Text.GetWorldSize();
            Text.Pos = new Vector2(-size.X / 2 - 350, size.Y * .85f);

            Text.Pos += new Vector2(0, -42);
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