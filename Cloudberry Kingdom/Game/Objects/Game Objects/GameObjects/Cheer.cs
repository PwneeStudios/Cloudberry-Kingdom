using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Cheer : GameObject
    {
        DrawPile MyPile = new DrawPile();

        public QuadClass Berry;
        public Cheer()
        {
            MyPile.FancyPos.UpdateWithGame = true;

            Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_enthusiastic";
            Berry.Scale(300);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(0, -1250);
            MyPile.Add(Berry);

            //MyPile.Pos = new Vector2(315, 775);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            Berry.FancyPos.ToAndBack(Berry.Pos + new Vector2(0, 550), 53);
            //Berry.FancyPos.ToAndBack(new Vector2(0, 700), 90);
        }

        protected override void MyDraw()
        {
            // No cheers =(
            return;

            if (!CoreData.Show || CoreData.MyLevel.SuppressCheckpoints) return;

            Level level = CoreData.MyLevel;
            MyPile.FancyPos.SetCenter(level.MainCamera, true);
            MyPile.FancyPos.Update();

            MyPile.Draw();
        }

        protected override void MyPhsxStep()
        {
            if (!Berry.FancyPos.Playing)
                MyGame.Recycle.CollectObject(this);
        }
    }
}