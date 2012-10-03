using System;

using CoreEngine;

namespace CloudberryKingdom
{
    public class BobPhsxCinematic : BobPhsxNormal
    {
        public bool OverrideAnimBehavior = true;

        public FancyVector2 FancyPos;
        public bool UseFancy = false;

        public BobPhsxCinematic()
        {
            FancyPos = new FancyVector2();
        }

        public override void PhsxStep2()
        {
            base.PhsxStep2();
        }

        public override void PhsxStep()
        {
            if (UseFancy)
            {
                FancyPos.Update();
                MyBob.Move(FancyPos.AbsVal - MyBob.Core.Data.Position);
            }

            MyBob.Box.Current.Size = MyBob.PlayerObject.BoxList[1].Size() / 2;
            MyBob.Box.SetTarget(MyBob.Core.Data.Position, MyBob.Box.Current.Size);
            MyBob.Box.SwapToCurrent();
        }

        public override void AnimStep()
        {
            if (!OverrideAnimBehavior)
                base.AnimStep();
            else
            {
                CheckForAnimDone();

                float AnimSpeed = 1f;
                if (MyBob.PlayerObject.DestinationAnim() == 1 && MyBob.PlayerObject.Loop)
                    AnimSpeed = Math.Max(.35f, .1f * Math.Abs(MyBob.Core.Data.Velocity.X));
                //MyBob.PlayerObject.PlayUpdate(AnimSpeed * 1000f / 60f / 150f);
                MyBob.PlayerObject.PlayUpdate(1000f * AnimSpeed * Tools.dt / 150f);
            }
        }
    }
}