using Microsoft.Xna.Framework;

using CloudberryKingdom.Goombas;

namespace CloudberryKingdom
{
    public partial class BlobGuy
    {
        public ScenePart Make_Wanted_Idle()
        {
            ScenePart State = new ScenePart();
            State.MyBegin = delegate()
            {
                MyObject.EnqueueAnimation("StandUp", 0, true, true, 10, 1.35f, true);
                MyObject.Play = true;

                Core.Data.Velocity = Vector2.Zero;
            };

            State.MyPhsxStep = delegate(int Step)
            {
                StickmanToTargets(MyTargets);

                if (Step < 2)
                {
                    foreach (Goomba blob in Blobs)
                        blob.Core.Data.Position = blob.Target;
                }
            };

            return State;
        }
    }
}