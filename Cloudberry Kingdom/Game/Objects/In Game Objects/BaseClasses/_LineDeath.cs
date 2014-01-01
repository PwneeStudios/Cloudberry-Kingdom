using System.Text;
using Microsoft.Xna.Framework;

using CoreEngine;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class _LineDeath : _Death
    {
        protected MovingLine MyLine;
        public Vector2 p1, p2;

        public _LineDeath() { }
        public _LineDeath(bool BoxesOnly) { Construct(BoxesOnly); }

        public override void Construct(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            MakeNew();
        }

        public override void Interact(Bob bob)
        {
            if (Phsx.AABoxAndLineCollisionTest(bob.Box2, ref MyLine))
            {
                if (Core.MyLevel.PlayMode == 0)
                    bob.Die(BobDeathType.Laser, this);
                else
                {
                    bool col = Phsx.AABoxAndLineCollisionTest_Tiered(ref MyLine, Core, bob, AutoGenSingleton);

                    if (col)
                        Core.Recycle.CollectObject(this);
                }
            }
        }

        protected override void DrawBoxes()
        {
			Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, new Color(255, 165, 35), 30);

			//Tools.QDrawer.DrawLine(MyLine.Target.p1, MyLine.Target.p2, Color.Orange, 20);
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            MyLine.Current.p1 += shift;
            MyLine.Current.p2 += shift;
            MyLine.Target.p1 += shift;
            MyLine.Target.p2 += shift;

            p1 += shift;
            p2 += shift;
        }

        public override void PhsxStep2()
        {
            if (!Core.SkippedPhsx)
                MyLine.SwapToCurrent();
        }
    }
}
