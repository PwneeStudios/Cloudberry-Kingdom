using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class _TileInfo
    {
        public TextureOrAnim Sprite = null;
    }

    public class _Obstacle : ObjectBase
    {
        public AutoGen AutoGenSingleton;

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data = Core.StartData;
        }
    }

    public class _Death : _Obstacle
    {
        protected Bob.BobDeathType DeathType = Bob.BobDeathType.None;
    }

    public class _LineDeath : _Death
    {
        protected MovingLine MyLine;

        public override void Interact(Bob bob)
        {
            if (Phsx.AABoxAndLineCollisionTest(bob.Box2, ref MyLine))
            {
                if (Core.MyLevel.PlayMode == 0)
                    bob.Die(Bob.BobDeathType.Laser, this);
                else
                {
                    bool col = Phsx.AABoxAndLineCollisionTest_Tiered(ref MyLine, Core, bob, AutoGenSingleton);

                    if (col)
                        Core.Recycle.CollectObject(this);
                }
            }
        }

        public override void PhsxStep2()
        {
            if (!Core.SkippedPhsx)
                MyLine.SwapToCurrent();
        }
    }

    public class _CircleDeath : _Death
    {
        protected CircleBox Circle;
        protected float Radius;

        public override void MakeNew()
        {
            Circle.Initialize(Vector2.Zero, 1);

            Init();

            Core.SkippedPhsx = true;
            Core.ContinuousEnabled = true;
        }

        public _CircleDeath() { }
        public _CircleDeath(bool BoxesOnly) { Construct(BoxesOnly); }

        public void Construct(bool BoxesOnly)
        {
            Core.BoxesOnly = BoxesOnly;

            Circle = new CircleBox();

            MakeNew();
        }

        public virtual void Init()
        {
            Core.Init();

            Core.GenData.OverlapWidth = 60;
        }

        public virtual void Scale(float scale)
        {
            Circle.Scale(scale);
        }

        public override void PhsxStep()
        {
            Circle.Center = Pos;
            Circle.Radius = Radius;

            if (Core.WakeUpRequirements)
            {
                Circle.Invalidate();

                Core.WakeUpRequirements = false;
            }
        }

        public override void Draw()
        {
            if (Tools.DrawBoxes)
            {
                Circle.Draw(new Color(50, 50, 255, 120));
            }
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
                        bob.Die(DeathType, this);

                    if (Core.MyLevel.PlayMode != 0)
                    {
                        bool col = Circle.BoxOverlap_Tiered(Core, bob, AutoGenSingleton);

                        if (col)
                            Core.Recycle.CollectObject(this);
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
