using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class SingleData : StyleData
    {
        public Vector2 InitialDoorYRange = new Vector2(-600, 200);

        public SingleData(Rand Rnd)
            : base(Rnd)
        {
        }

        public override void Randomize()
        {
            base.Randomize();
        }
    }
}