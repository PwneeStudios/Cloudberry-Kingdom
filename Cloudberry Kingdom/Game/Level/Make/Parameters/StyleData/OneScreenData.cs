using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class OneScreenData : StyleData
    {
        public Vector2 CamShift = Vector2.Zero;

        public OneScreenData(Rand Rnd)
            : base(Rnd)
        {
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}