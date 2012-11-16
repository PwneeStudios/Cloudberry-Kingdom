using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;




namespace CloudberryKingdom
{
    public class HeroRush2_Tutorial : HeroRush_Tutorial
    {
        Challenge_HeroRush2 HeroRush2;
        public HeroRush2_Tutorial(Challenge_HeroRush2 HeroRush2)
            : base(HeroRush2)
        {
            this.HeroRush2 = HeroRush2;
        }

        protected override void Title()
        {
            ShowTitle = false;

            GUI_Text text = GUI_Text.SimpleTitle(Localization.Words.HybridRush);
            text.MyText.Pos += new Vector2(0, -110);

            MyGame.AddGameObject(text);
            MyGame.WaitThenDo(0, new AddGameObjectHelper(this, text));
        }
    }
}