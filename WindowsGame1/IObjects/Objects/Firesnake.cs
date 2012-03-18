using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Firesnake : ObjectBase
    {
        public override void TextDraw() { }
        public override void Release()
        {
            Core.Release();
        }

        public CircleBox Box;

        public SimpleQuad MyQuad;
        public BasePoint Base;

        Vector2 Size = new Vector2(220);

        public Vector2 Orbit;
        public Vector2 Radii;
        public int Period, Offset;

        public override void MakeNew()
        {
            if (!Core.BoxesOnly)
            {
                MyQuad.Init();
                MyQuad.MyEffect = Tools.BasicEffect;
                MyQuad.MyTexture = Fireball.EmitterTexture;

                Base.e1 = new Vector2(Size.X, 0);
                Base.e2 = new Vector2(0, Size.Y);
            }

            Core.Init();
            Core.MyType = ObjectType.Firesnake;
            Core.DrawLayer = 7;

            Orbit = Vector2.Zero;

            Box.Initialize(Core.Data.Position, .5f * Size.X);
        }

        public Firesnake(bool BoxesOnly)
        {
            Box = new CircleBox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }


        public override void PhsxStep()
        {
            //double t = 2 * Math.PI * (Core.GetPhsxStep() + Offset) / (float)Period;
            double t = 2 * Math.PI * (Core.GetIndependentPhsxStep() + Offset) / (float)Period;
            Core.Data.Position = Tools.AngleToDir(t) * Radii + Orbit;

            Vector2 PhsxCutoff = new Vector2(400);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(-100, 400);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Core.SkippedPhsx = true;
                return;
            }
            Core.SkippedPhsx = false;

            Box.Center = Core.Data.Position;
        }

        public override void PhsxStep2() { }
        
        public override void Draw()
        {
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 150))
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
            {
                Base.Origin = Core.Data.Position;
                MyQuad.Update(ref Base);

                // Draw the Firesnake
                Tools.QDrawer.DrawQuad(MyQuad);
            }

            if (Tools.DrawBoxes)
                Box.Draw(new Color(50, 50, 255, 220));
        }

        public override void Move(Vector2 shift)
        {
            Base.Origin += shift;
            Core.Data.Position += shift;

            Orbit += shift;

            Box.Move(shift);
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;
        }

        public override void Interact(Bob bob)
        {
            if (Core.SkippedPhsx) return;

            bool hold = Box.BoxOverlap(bob.Box2);
            if (hold)
            {
                if (Core.MyLevel.PlayMode == 1)
                {
                    bool col = Box.BoxOverlap_Tiered(Core, bob, Firesnake_AutoGen.Instance);

                    if (col)
                        Core.Recycle.CollectObject(this);
                }

                if (Core.MyLevel.PlayMode == 0)
                    bob.Die(Bob.BobDeathType.Firesnake, this);
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            Firesnake FiresnakeA = A as Firesnake;

            Size = FiresnakeA.Size;

            Period = FiresnakeA.Period;
            Offset = FiresnakeA.Offset;
            Radii = FiresnakeA.Radii;
            Orbit = FiresnakeA.Orbit;
        }

        public override void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public override void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public override void OnUsed() { }
public override void OnMarkedForDeletion() { }
public override void OnAttachedToBlock() { }
public override bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public override void Smash(Bob bob) { }
public override bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
