using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BobPhsxCharSelect : BobPhsxNormal
    {
        Vector2 Offset;
        public bool OverrideAnimBehavior = false;

        public override void PhsxStep2()
        {
            base.PhsxStep2();

            OnGround = true;

            //Offset.Y += MyBob.Core.Data.Velocity.Y;
        }
        
        public override void AnimStep()
        {
            if (!OverrideAnimBehavior)
                base.AnimStep();
            else
            {
                float AnimSpeed = 1f;
                if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
                    AnimSpeed = Math.Max(.35f, .1f * Math.Abs(MyBob.Core.Data.Velocity.X));
                MyBob.PlayerObject.PlayUpdate(AnimSpeed * 1000f / 60f / 150f);
                //MyBob.PlayerObject.PlayUpdate(1000f * AnimSpeed * Tools.dt / 150f);
                //Console.WriteLine("{0},  {1}", 1000f / 60f, Tools.dt);
            }
        }
    }
}