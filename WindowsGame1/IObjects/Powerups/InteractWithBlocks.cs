using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class InteractWithBlocks : ObjectBase
    {
        public override void TextDraw() { }

        protected EzSound MySound;

        public QuadClass MyQuad;

        public Vector2 Size;
        public AABox Box;

        protected float BounceLoss = .75f;

        public float Friction = .98f;
        public float SmallFriction = .905f;

        public override void Release()
        {
            Core.Release();

            OnTouched = null;
        }

        public Action OnTouched;

        public override void MakeNew()
        {
            Core.Init();

            Size = new Vector2(50);

            Core.ResetOnlyOnReset = true;
            Core.EditHoldable = false;
            Core.MyType = ObjectType.Undefined;
            Core.DrawLayer = 3;

            Init();
        }

        public InteractWithBlocks(bool BoxesOnly)
        {
            Box = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }
        public void Init()
        {
            Box.Initialize(Core.Data.Position, Size);

            Update();
        }

        public override void PhsxStep()
        {
            if (!Core.Active) return;

            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 1000))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            // Integrate
            Core.Data.Acceleration = new Vector2(0, -1);
            Core.Data.Integrate();
            if (Math.Abs(Core.Data.Velocity.X) < 2)
                Core.Data.Velocity.X *= SmallFriction;
            else
                Core.Data.Velocity.X *= Friction;
            Tools.Restrict(-30, 40, ref Core.Data.Velocity.Y);

            Box.SetTarget(Pos + new Vector2(0, -1), Size);
            if (MyQuad != null) MyQuad.Pos = Pos;

            // Interact with blocks
            foreach (BlockBase block in Core.MyLevel.Blocks)
                ColWithBlock(block);

            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            Box.SwapToCurrent();

            Update();
        }


        public void Update()
        {
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            //Core.Data.Position = Core.StartData.Position + new Vector2(0, 1000);
            //Box.SetCurrent(Core.Data.Position, Box.Current.Size);
            //Box.SetTarget(Core.Data.Position, Box.Current.Size);

            //Update();
        }

        public override void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public virtual void OnTouch(Bob bob)
        {
            if (OnTouched != null) OnTouched(); OnTouched = null;
        }

        public override void Interact(Bob bob)
        {
            if (!Core.Active) return;
            //if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.ShowCoinsInReplay) return;

            bool Col = Phsx.BoxBoxOverlap(bob.Box2, Box);
            if (Col)
            {
                OnTouch(bob);
            }
        }

        void ColWithBlock(BlockBase block)
        {
            if (block.Core.MarkedForDeletion || !block.IsActive || !block.Core.Real) return;

            ColType col = Phsx.CollisionTest(Box, block.Box);
            if (col == ColType.Top)
            {
                float VelFadeBegin = -15;
                if (MySound != null && Core.Data.Velocity.Y < -2)
                    MySound.Play(Tools.Restrict(0, 1, Core.Data.Velocity.Y / VelFadeBegin));

                //Core.Data.Position += new Vector2(0, block.Box.TR.Y - Box.Target.BL.Y + 1f);
                Core.Data.Position += new Vector2(0, block.Box.TR.Y - Box.Target.BL.Y);
                Box.Move(new Vector2(0, 1));
                if (Core.Data.Velocity.Y < 0) Core.Data.Velocity.Y *= -1 * BounceLoss;
            }
        }

        static Vector2 DrawGrace = new Vector2(50, 50);
        public override void Draw()
        {
            if (!Core.Active) return;

            if (Box.Current.BL.X > Core.MyLevel.MainCamera.TR.X + DrawGrace.X || Box.Current.BL.Y > Core.MyLevel.MainCamera.TR.Y + DrawGrace.Y)
                return;
            if (Box.Current.TR.X < Core.MyLevel.MainCamera.BL.X - DrawGrace.X || Box.Current.TR.Y < Core.MyLevel.MainCamera.BL.Y - DrawGrace.Y)
                return;

            if (MyQuad != null && Tools.DrawGraphics && !Core.BoxesOnly)
                MyDraw();
            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Bisque, 10);
        }

        public virtual void MyDraw()
        {
            MyQuad.Draw();
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            InteractWithBlocks InteractWithBlocksA = A as InteractWithBlocks;

            Box.SetTarget(InteractWithBlocksA.Box.Target.Center, InteractWithBlocksA.Box.Target.Size);
            Box.SetCurrent(InteractWithBlocksA.Box.Current.Center, InteractWithBlocksA.Box.Current.Size);
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