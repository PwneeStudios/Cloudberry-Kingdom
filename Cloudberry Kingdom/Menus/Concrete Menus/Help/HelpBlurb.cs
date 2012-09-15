using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HelpBlurb : StartMenuBase
    {
        public HelpBlurb()
        {
            MyPile = new DrawPile();

            MyPile.Backdrop = new PieceQuad();
            MyPile.Backdrop.Clone(PieceQuad.SpeechBubble);
            MyPile.Backdrop.CalcQuads(new Vector2(855, 400));
            MyPile.BackdropShift = new Vector2(575, 220);
            MyPile.Backdrop.SetColor(Color.Pink);
            MyPile.Backdrop.SetAlpha(.8f);
            
            QuadClass Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_surprised";
            Berry.Scale(625);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(1422, -468);
            MyPile.Add(Berry);
        }

        public override void Init()
        {
            base.Init();

            SlideInFrom = SlideOutTo = PresetPos.Right;
        }

        public Action SetText_Action(string text)
        {
            return () => SetText(text);
        }

        public void SetText(string text)
        {
            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            EzText Text = new EzText(text, ItemFont, 800, false, false, .575f);
            Text.Pos = new Vector2(-139.4445f, 536.1113f);
            Text.Scale *= .74f;
            MyPile.Add(Text);
        }
    }
}