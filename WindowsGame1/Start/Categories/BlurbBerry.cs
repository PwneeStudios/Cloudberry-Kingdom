using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BlurbBerry : GUI_Panel
    {
        public override void OnAdd()
        {
            base.OnAdd();

            this.SlideOut(PresetPos.Right, 0);
        }

        public BlurbBerry()
        {
            MyPile = new DrawPile();

            MyPile.Backdrop = new PieceQuad();
            MyPile.Backdrop.Clone(PieceQuad.SpeechBubble);
            //MyPile.Backdrop.CalcQuads(new Vector2(815, 860));
            MyPile.Backdrop.CalcQuads(new Vector2(850, 370));
            MyPile.BackdropShift = new Vector2(565, 250);
            MyPile.Backdrop.SetColor(Color.Pink);
            MyPile.Backdrop.SetAlpha(.8f);
            
            QuadClass Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_surprised";
            Berry.Scale(625);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(1412, -418);
            MyPile.Add(Berry);
        }

        public void SetText(string text)
        {
            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            EzText Text = new EzText(text, Tools.Font_DylanThin42, 900, false, false, .575f);
            Text.Pos = new Vector2(-162.7773f, 580.7778f);
            Text.Scale *= .65f;
            MyPile.Add(Text);
        }
    }
}