using System.Text;
using Microsoft.Xna.Framework;

using CoreEngine;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class _CircleDeath : _Death
    {
        protected CircleBox Circle;
        protected float Radius;

        public override void MakeNew()
        {
            Circle.Initialize(Vector2.Zero, 1);

            CoreData.Init();
            CoreData.GenData.OverlapWidth = 60;

            CoreData.SkippedPhsx = true;
            CoreData.ContinuousEnabled = true;
        }

        public _CircleDeath() { }
        public _CircleDeath(bool BoxesOnly) { Construct(BoxesOnly); }

        public override void Construct(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Circle = new CircleBox();

            MakeNew();
        }

        public virtual void Scale(float scale)
        {
            Circle.Scale(scale);
        }

        protected override void ActivePhsxStep()
        {
            Circle.Center = CoreData.Data.Position;
            Circle.Radius = Radius;

            if (CoreData.WakeUpRequirements)
            {
                Circle.Invalidate();

                CoreData.WakeUpRequirements = false;
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
            if (!CoreData.SkippedPhsx)
            {
                bool Col = Circle.BoxOverlap(bob.Box2);
                
                if (Col)
                {
                    if (CoreData.MyLevel.PlayMode == 0)
                    {
                        bob.Die(DeathType, this);
                        Die();
                    }

                    if (CoreData.MyLevel.PlayMode != 0)
                    {
                        bool col = Circle.BoxOverlap_Tiered(CoreData, bob, AutoGenSingleton);

                        if (col) CollectSelf();
                    }
                }
            }
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

            CoreData.WakeUpRequirements = true;
        }
    }
}
