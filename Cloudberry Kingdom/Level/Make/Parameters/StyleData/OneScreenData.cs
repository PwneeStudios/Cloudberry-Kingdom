using Microsoft.Xna.Framework;

using CoreEngine.Random;

namespace CloudberryKingdom.Levels
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