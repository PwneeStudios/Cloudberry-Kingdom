using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class OhNo : GameObject
    {
        public override string[] GetViewables()
        {
            return new string[] { };
        }

        DrawPile MyPile = new DrawPile();

        public QuadClass Berry;
        public EzText Text;
        public OhNo()
        {
            MyPile.FancyPos.UpdateWithGame = true;

            Berry = new QuadClass();
            Berry.SetToDefault();
            //Berry.TextureName = "cb_enthusiastic";
            Berry.TextureName = "cb_surprised";
            Berry.Scale(500);
            Berry.ScaleYToMatchRatio();

            MyPile.Add(Berry);

            Text = new EzText("Oh no!", Tools.Font_DylanThin42, 1000, true, true);
            Text.Scale *= 1.85f;
            CampaignMenu.EasyColor(Text);
            MyPile.Add(Text);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            //CollectWhenDone = true;
            //Berry.Pos = new Vector2(0, -1550);
            //Text.Pos = new Vector2(0, -1100);
            //Berry.FancyPos.ToAndBack(Berry.Pos + new Vector2(0, 950), 100);
            //Text.FancyPos.ToAndBack(Text.Pos + new Vector2(0, 950), 100);

            //Berry.Pos = new Vector2(1800, -700);
            //Text.Pos = new Vector2(1700, -200);
            //Berry.FancyPos.ToAndBack(Berry.Pos + new Vector2(-900, 0), 130);
            //Text.FancyPos.ToAndBack(Text.Pos + new Vector2(-900, 0), 130);

            //Berry.Pos = new Vector2(0, -1550);
            //Text.Pos = new Vector2(-160, -1000);
            //Berry.FancyPos.LerpTo(Berry.Pos + new Vector2(0, 950), 25);
            //Text.FancyPos.LerpTo(Text.Pos + new Vector2(0, 950), 25);
            //MyGame.WaitThenDo(80, () =>
            //{
            //    Berry.FancyPos.LerpTo(Berry.Pos + new Vector2(0, -950), 25, LerpStyle.Linear);
            //    Text.FancyPos.LerpTo(Text.Pos + new Vector2(0, -950), 25, LerpStyle.Linear);
            //    CollectWhenDone = true;
            //});

            Vector2 BerryEnd = new Vector2(977.7775f, -633.3335f);
            Vector2 TextEnd = new Vector2(94.44446f, -766.6666f);
            Vector2 BerryStart = Berry.Pos = BerryEnd + new Vector2(0, -850);
            Vector2 TextStart = Text.Pos = TextEnd + new Vector2(0, -850);
            Berry.FancyPos.LerpTo(BerryEnd, 25);
            Text.FancyPos.LerpTo(TextEnd, 25);
            MyGame.WaitThenDo(105, () =>
            {
                Berry.FancyPos.LerpTo(BerryStart, 25, LerpStyle.Linear);
                Text.FancyPos.LerpTo(TextStart, 25, LerpStyle.Linear);
                CollectWhenDone = true;
            });
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            Level level = Core.MyLevel;
            MyPile.FancyPos.SetCenter(level.MainCamera, true);
            MyPile.FancyPos.Update();

            MyPile.Draw();
        }

        bool CollectWhenDone = false;
        protected override void MyPhsxStep()
        {
            if (!Berry.FancyPos.Playing && CollectWhenDone)
                MyGame.Recycle.CollectObject(this);
        }
    }
}