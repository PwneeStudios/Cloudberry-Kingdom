using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HelpBlurb : CkBaseMenu
    {
        public HelpBlurb()
        {
            MyPile = new DrawPile();
            
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

        class SetText_ActionHelper : Lambda
        {
            HelpBlurb hb;
            Localization.Words Word;

            public SetText_ActionHelper(HelpBlurb hb, Localization.Words Word)
            {
                this.hb = hb;
                this.Word = Word;
            }

            public void Apply()
            {
                hb.SetText(Word);
            }
        }

        public Lambda SetText_Action(Localization.Words Word)
        {
            return new SetText_ActionHelper(this, Word);
        }

        public void SetText(Localization.Words Word)
        {
            // Erase previous text
            MyPile.MyTextList.Clear();

            // Add the new text
            EzText Text = new EzText(Word, ItemFont, 800, false, false, .575f);
            Text.Pos = new Vector2(-139.4445f, 536.1113f);
            Text.Scale *= .74f;
            MyPile.Add(Text);
        }
    }
}