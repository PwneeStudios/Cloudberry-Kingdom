using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class TimeCrisis_Tutorial : HeroRush_Tutorial
    {
        Challenge_TimeCrisis TimeCrisis;
        public TimeCrisis_Tutorial(Challenge_TimeCrisis TimeCrisis)
            : base(TimeCrisis)
        {
            this.TimeCrisis = TimeCrisis;
        }

        protected override void Title()
        {
            ShowTitle = false;

            GUI_Text text = GUI_Text.SimpleTitle(Challenge_TimeCrisis.Instance.Name);
            //GUI_Text text2 = GUI_Text.SimpleTitle("Revenge of the Double Jump");
            text.MyText.Pos += new Vector2(0, -110);
            //text2.MyText.Pos += new Vector2(0, 300);
            //CampaignHelper.AbusiveColor(text2.MyText);

            MyGame.AddGameObject(text);
            MyGame.WaitThenDo(0, new AddGameObjectHelper(this, text));
        }
    }
}