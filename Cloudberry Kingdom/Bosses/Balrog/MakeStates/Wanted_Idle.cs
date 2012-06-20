using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class Balrog
    {
        public ScenePart Make_Wanted_Idle()
        {
            ScenePart State = new ScenePart();
            State.MyBegin = delegate()
            {
                MyObject.EnqueueAnimation("Laugh", 0, true, true, 10, 1.35f, true);
                MyObject.Play = true;

                Core.Data.Velocity = Vector2.Zero;
            };

            State.MyPhsxStep = delegate(int Step)
            {
            };

            return State;
        }
    }
}