using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class ConveyorBlock : BlockBase, Block
    {
        public void TextDraw() { }

        public QuadClass MyQuad, LeftEnd, RightEnd;
        
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 3;
            BlockCore.MyType = ObjectType.ConveyorBlock;
        }

        public void Release()
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

        public void Hit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
        }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { } 

        public void Reset(bool BoxesOnly)
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

        public void PhsxStep()
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

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            ConveyorBlock BlockA = A as ConveyorBlock;

            Speed = BlockA.Speed;
            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
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
