using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class HintBlurb : CkBaseMenu
    {
        protected QuadClass Backdrop;

        public HintBlurb()
        {
            PauseOnPause = false;
            PauseLevel = false;
            FixedToCamera = true;
            CoreData.RemoveOnReset = false;

            MyPile = new DrawPile();

            MakeBackdrop();

            SetText(string.Format(Localization.WordString(Localization.Words.JumpHigherNote), ButtonString.Go(85)));
        }

        protected virtual void MakeBackdrop()
        {
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "WidePlaque";
            Backdrop.Size = new Vector2(1250, 138);
            Backdrop.Pos = new Vector2(0, 0);

            MyPile.Add(Backdrop);
            MyPile.Pos = new Vector2(0, -800);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            // Remove all other hints
            foreach (GameObject obj in MyGame.MyGameObjects)
            {
                if (obj == this) continue;

                HintBlurb blurb = obj as HintBlurb;
                if (null != blurb)
                    blurb.Kill();
            }
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
        public void SetText(string text)
        {
            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            Text = new EzText(text, ItemFont, 1800, false, false, .575f);
            Text.Scale *= .74f;
            
            MyPile.Add(Text);
            SizeAndPosition();
        }

        protected virtual void SizeAndPosition()
        {
            Vector2 size = Text.GetWorldSize();
            float MaxSize = Backdrop.Size.X - 100;
            if (size.X / 2 > MaxSize)
                Backdrop.SizeX = size.X / 2 + 100;
                //Text.Scale *= MaxSize / (size.X / 2);

            Text.Pos = new Vector2(-size.X / 2, size.Y * .85f);
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

        protected virtual bool ShouldDie()
        {
            return ButtonCheck.AllState(-1).Down && Step > 95 ||
                   ButtonCheck.GetDir(-1).Length() > .5 && Step > 140 ||
                   ButtonCheck.State(ControllerButtons.B, -2).Pressed;
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