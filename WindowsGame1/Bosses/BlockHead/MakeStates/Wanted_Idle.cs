using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class BlockHead : IObject
    {
        int Delay = 180;
        int Count = 0;

        void PlayAnims()
        {
            MyObject.AnimQueue.Clear();
            MyObject.EnqueueAnimation("SinglePunch", 0, false, true, 8, 1.2f, true);
            MyObject.EnqueueAnimation("Kick", 0, false, false, 2, .7f, true);
            MyObject.EnqueueAnimation("DoublePunch", 0, false, false, 10, 1.35f, true);
            MyObject.EnqueueAnimation("Flip", 0, false, false, 10, 3.75f, true);
            MyObject.EnqueueAnimation("Stand", 0, true, false, 10, 3.3f, true);
            MyObject.Play = true;
        }

        public ScenePart Make_Wanted_Idle()
        {
            ScenePart State = new ScenePart();
            State.MyBegin = delegate()
            {
                MyObject.EnqueueAnimation("Stand", 0, true, false, 10, 3.3f, true);
                MyObject.Play = true;

                //PlayAnims();

                Core.Data.Velocity = Vector2.Zero;
            };

            State.MyPhsxStep = delegate(int Step)
            {
                return;

                if (MyObject.AnimQueue.Count <= 1)
                {
                    Count++;
                    if (Count > Delay)
                    {
                        Count = 0;
                        PlayAnims();
                    }
                }
            };

            return State;
        }
    }
}