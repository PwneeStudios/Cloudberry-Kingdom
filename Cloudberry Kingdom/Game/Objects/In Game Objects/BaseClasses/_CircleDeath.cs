using System.Text;
using Microsoft.Xna.Framework;



using System.IO;


namespace CloudberryKingdom
{
    public abstract class _CircleDeath : _Death
    {
        protected CircleBox Circle;
        protected float Radius;

        public override void MakeNew()
        {
            Circle.Initialize(Vector2.Zero, 1);

            Core.Init();
            Core.GenData.OverlapWidth = 60;

            Core.SkippedPhsx = true;
            Core.ContinuousEnabled = true;
        }

        public _CircleDeath() { }
        public _CircleDeath(bool BoxesOnly) { Construct(BoxesOnly); }

        public override void Construct(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            Circle = new CircleBox();

            MakeNew();
        }

        public virtual void Scale(float scale)
        {
            Circle.Scale(scale);
        }

        protected override void ActivePhsxStep()
        {
            Circle.Center = Pos;
            Circle.Radius = Radius;

            if (Core.WakeUpRequirements)
            {
                Circle.Invalidate();

                Core.WakeUpRequirements = false;
            }
        }

        protected override void DrawBoxes()
        {
            Circle.Draw(new Color(50, 50, 255, 120));
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

            Circle.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!Core.SkippedPhsx)
            {
                bool Col = Circle.BoxOverlap(bob.Box2);
                
                if (Col)
                {
                    if (Core.MyLevel.PlayMode == 0)
                    {
                        bob.Die(DeathType, this);
                        Die();
                    }

                    if (Core.MyLevel.PlayMode != 0)
                    {
                        bool col = Circle.BoxOverlap_Tiered(Core, bob, AutoGenSingleton);

                        if (col) CollectSelf();
                    }
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Core.WakeUpRequirements = true;
        }
    }
}
