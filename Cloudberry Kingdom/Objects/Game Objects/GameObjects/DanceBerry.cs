using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class DanceBerry : GameObject
    {
        DrawPile MyPile = new DrawPile();

        public QuadClass Berry;
        public DanceBerry()
        {
            MyPile.FancyPos.UpdateWithGame = true;

            Berry = new QuadClass("cb_enthusiastic", 300, true);
            Berry.Quad.Shift(new Vector2(0, .55f));

            Berry.Pos = new Vector2(0, 0);
            MyPile.Add(Berry);
        }

        protected override void MyDraw()
        {
            if (!Core.Show) return;

            Level level = Core.MyLevel;
            MyPile.FancyPos.SetCenter(level.MainCamera, true);
            MyPile.FancyPos.Update();

            MyPile.Draw();
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;

            // Alternate between leaning left and right
            float LeanAngle = 16;
            int Wait = 34;
            if (level.CurPhsxStep % Wait == 0)
            {
                if ((level.CurPhsxStep / Wait) % 2 == 0)
                    Berry.Degrees = LeanAngle;
                else
                    Berry.Degrees = -LeanAngle;
            }
        }
    }
}