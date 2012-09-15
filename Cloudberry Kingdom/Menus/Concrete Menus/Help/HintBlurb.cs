using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HintBlurb : StartMenuBase
    {
        public HintBlurb()
        {
            PauseOnPause = false;
            PauseLevel = true;
            Core.RemoveOnReset = false;

            MyPile = new DrawPile();

            MyPile.Backdrop = new PieceQuad();
            MyPile.Backdrop.Clone(PieceQuad.SpeechBubble);
            MyPile.Backdrop.CalcQuads(new Vector2(855, 400));
            MyPile.BackdropShift = new Vector2(113.8889f, -180f);
            MyPile.Backdrop.SetColor(Color.Pink);
            MyPile.Backdrop.SetAlpha(.8f);
            
            QuadClass Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_surprised";
            Berry.Scale(625);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(1422, -468);
            MyPile.Add(Berry);

            SetText("Hold {pXbox_A,85,?} to jump higher!");
        }

        public override void Init()
        {
            base.Init();

            SlideInFrom = SlideOutTo = PresetPos.Right;
        }

        public void SetText(string text)
        {
            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            EzText Text = new EzText(text, ItemFont, 800, false, false, .575f);
            Text.Pos = new Vector2(-600.5554f, 55.55573f);
            Text.Scale *= .74f;
            MyPile.Add(Text);
        }

        int Step = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Step++;
            if (Step < 40) return;

            if (ButtonCheck.AllState(-1).Down)
            {
                PauseLevel = false;
                ReturnToCaller();
            }
        }
    }
}