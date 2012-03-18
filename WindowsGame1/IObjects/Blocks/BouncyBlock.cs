using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public enum BouncyBlockState { Regular, SuperStiff };
    public class BouncyBlock : BlockBase, Block
    {
        public void TextDraw() { }

        public EzSound BounceSound = Tools.SoundWad.FindByName("BouncyBlock_Bounce");

        public QuadClass MyQuad;
        BouncyBlockState State;
        public Vector2 Offset, SizeOffset;
        public float speed;

        int TouchedCountdown = 0;

        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            Core.Init();
            //BlockCore.GivesVelocity = false;
            Core.DrawLayer = 3;
            BlockCore.MyType = ObjectType.BouncyBlock;

            SideDampening = 1f;

            SetState(BouncyBlockState.Regular);
        }

        public void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            MyQuad = null;
            MyBox = null;
        }

        public void SetState(BouncyBlockState NewState) { SetState(NewState, false); }
        public void SetState(BouncyBlockState NewState, bool ForceSet)
        {
            if (State != NewState || ForceSet)
            {
                switch (NewState)
                {
                    case BouncyBlockState.Regular:
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("BouncyBlock1");
                        break;
                    case BouncyBlockState.SuperStiff:
                        MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("BouncyBlock2");
                        break;
                }
            }

            State = NewState;
        }

        public BouncyBlock(bool BoxesOnly)
        {
            MyQuad = new QuadClass();

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size, float speed)
        {
            Active = true;
            
            this.speed = speed;

            BlockCore.Layer = .35f;
            MyBox = new AABox(center, size);
            MyQuad.Base.Origin = BlockCore.Data.Position = BlockCore.StartData.Position = center;

            MyBox.Initialize(center, size);

            SetState(BouncyBlockState.Regular, true);
            MyQuad.Base.e1.X = size.X;
            MyQuad.Base.e2.Y = size.Y;

            Update();
        }

        public float SideDampening;
        void Snap(Bob bob)
        {
            TouchedCountdown = 3;

            if (Offset.X != 0)
                bob.Core.Data.Velocity.X = 1.1f * speed * Offset.X * SideDampening;
            if (Offset.Y != 0)
            {
                bob.NewVel = speed * Offset.Y;
                bob.MyPhsx.OnGround = false;
                bob.MyPhsx.DisableJump(5);
                bob.MyPhsx.DampForcedJump();
            }
            //bob.Core.Data.Velocity += new Vector2(speed, 2.3f * speed) * Offset;
            Offset *= 42;
            SizeOffset = new Vector2(14, 14);
            SetState(BouncyBlockState.SuperStiff);
        }
        public void Hit(Bob bob) { }
        public void SideHit(Bob bob)
        {            
            Offset = new Vector2(Math.Sign(bob.Core.Data.Position.X - Core.Data.Position.X), 0);
            Snap(bob);
        }
        public void LandedOn(Bob bob)
        {
            bob.MyPhsx.OverrideSticky = true;

            Offset = new Vector2(0, 1);
            bob.MyPhsx.MaxJumpAccelMultiple = 2.5f;
            bob.MyPhsx.JumpLengthModifier = .85f;
            Snap(bob);
        }
        public void HitHeadOn(Bob bob)
        {
            bob.MyPhsx.KillJump();
            Offset = new Vector2(0, -1);            
            Snap(bob);
        }

        public void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            Offset = Vector2.Zero;
            SizeOffset = Vector2.Zero;

            SetState(BouncyBlockState.Regular, true);

            BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 50)
                    Active = Core.Active = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 150)
                    Active = Core.Active = false;
            }

            Update();

            if (TouchedCountdown == 0)
            {
                Offset *= .5f;
                SizeOffset *= .5f;
            }
            else
                TouchedCountdown--;

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center + Offset;
            BlockCore.OffsetMultAsParent = Vector2.One + Vector2.Divide(SizeOffset, Box.Current.Size);

            if (SizeOffset.X < .1f && State == BouncyBlockState.SuperStiff)
                SetState(BouncyBlockState.Regular);

            MyBox.SetTarget(Core.Data.Position, MyBox.Current.Size);

            BlockCore.StoodOn = false;
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (BlockCore.BoxesOnly) return;
        }

        public void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    MyBox.Target.BL.X = pos;
                    break;
                case Side.Right:
                    MyBox.Target.TR.X = pos;
                    break;
                case Side.Top:
                    MyBox.Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    MyBox.Target.BL.Y = pos;
                    break;
            }

            MyBox.Target.FromBounds();
            MyBox.SwapToCurrent();

            Update();

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        public void Draw()
        {
            Update();

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (Tools.DrawGraphics)
            {
                if (!BlockCore.BoxesOnly)
                {
                    MyQuad.Base.Origin = MyBox.Current.Center + Offset;
                    MyQuad.Base.e1.X = MyBox.Current.Size.X + SizeOffset.X;
                    MyQuad.Base.e2.Y = MyBox.Current.Size.Y + SizeOffset.Y;

                    MyQuad.Draw();
                    Tools.QDrawer.Flush();
                }

                BlockCore.Draw();
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            BouncyBlock BlockA = A as BouncyBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.speed);

            SideDampening = BlockA.SideDampening;

            State = BlockA.State;
        }

        public void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
