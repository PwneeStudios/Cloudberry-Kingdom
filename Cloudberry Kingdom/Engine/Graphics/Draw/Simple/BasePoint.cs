using Microsoft.Xna.Framework;

namespace CoreEngine
{
    public struct BasePoint
    {
        public Vector2 Origin, e1, e2;

        public BasePoint(float e1x, float e1y, float e2x, float e2y, float ox, float oy)
        {
            e1 = new Vector2(e1x, e1y);
            e2 = new Vector2(e2x, e2y);
            Origin = new Vector2(ox, oy);
        }

        public void Init()
        {
            Origin = Vector2.Zero;
            e1 = new Vector2(1, 0);
            e2 = new Vector2(0, 1);
        }

        public Vector2 GetScale()
        {
            return new Vector2(e1.X, e2.Y);
        }

        public void SetScale(Vector2 scale)
        {
            e1.X = scale.X;
            e2.Y = scale.Y;
        }
    }
}