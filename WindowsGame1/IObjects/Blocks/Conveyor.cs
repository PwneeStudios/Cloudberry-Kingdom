using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class ConveyorBlock : BlockBase
    {
        public override void TextDraw() { }

        public QuadClass MyQuad, LeftEnd, RightEnd;
        
        public override void Interact(Bob bob) { }

        public override void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 3;
            BlockCore.MyType = ObjectType.ConveyorBlock;
        }

        public override void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            MyQuad = LeftEnd = RightEnd = null;
            MyBox = null;
        }

        public ConveyorBlock(bool BoxesOnly)
        {
            if (!BoxesOnly)
            {
                MyQuad = new QuadClass();
                MyQuad.Quad.U_Wrap = true;
                MyQuad.TextureName = "Conveyor";

                LeftEnd = new QuadClass();
                LeftEnd.TextureName = "ConveyorEnd";

                RightEnd = new QuadClass();
                RightEnd.TextureName = "ConveyorEnd";
            }

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public float Speed = -.05f;
        float u_offset;
        Vector2 texture_size = new Vector2(100, 100);
        void SetUV()
        {
            float repeats = Size.X / texture_size.X;
            MyQuad.Quad.UVFromBounds(new Vector2(repeats + u_offset, 1), new Vector2(0 + u_offset, 0));
        }

        Vector2 Size;
        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;

            Size = size;
            Size.Y = texture_size.Y;

            BlockCore.Layer = .35f;
            MyBox = new AABox(center, Size);
            MyQuad.Base.Origin = BlockCore.Data.Position = BlockCore.StartData.Position = center;

            MyBox.Initialize(center, Size);

            MyQuad.Base.e1.X = Size.X;
            MyQuad.Base.e2.Y = Size.Y;

            LeftEnd.Scale(Size.Y);
            LeftEnd.ScaleXToMatchRatio();
            LeftEnd.Base.e1.X *= -1;

            RightEnd.Scale(Size.Y);
            RightEnd.ScaleXToMatchRatio();

            Update();
        }

        public override void Hit(Bob bob) { }
        public override void LandedOn(Bob bob)
        {
        }
        public override void HitHeadOn(Bob bob) { } public override void SideHit(Bob bob) { } 

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            BlockCore.StoodOn = false;

            MyBox.Current.Center = BlockCore.StartData.Position;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public override void PhsxStep()
        {
            Active = Core.Active = true;
            if (!Core.Held)
            {
                if (!Core.MyLevel.MainCamera.OnScreen(MyBox.Current.BL, MyBox.Current.TR, 10))
                    Active = Core.Active = false;
            }

            // Update the block's apparent center according to attached objects
            BlockCore.UseCustomCenterAsParent = true;
            BlockCore.CustomCenterAsParent = Box.Target.Center;

            Update();

            BlockCore.GroundSpeed = 2 * Speed * texture_size.X;
            u_offset += Speed;
            SetUV();

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            BlockCore.StoodOn = false;
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public void Update()
        {
            if (BlockCore.BoxesOnly) return;
        }

        public override void Extend(Side side, float pos)
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

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }
        public override void Draw()
        {
            Update();

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (Tools.DrawGraphics)
            {
                if (!BlockCore.BoxesOnly)
                {
                    MyQuad.Base.Origin = MyBox.Current.Center;
                    MyQuad.Draw();

                    LeftEnd.Pos = Core.Data.Position - new Vector2(Size.X, 0);
                    LeftEnd.Draw();

                    RightEnd.Pos = Core.Data.Position + new Vector2(Size.X, 0);
                    RightEnd.Draw();
                }

                BlockCore.Draw();
            }
        }

        public override void Clone(ObjectBase A)
        {
            Core.Clone(A.Core);

            ConveyorBlock BlockA = A as ConveyorBlock;

            Speed = BlockA.Speed;
            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
        }

        public override void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
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
