using System.IO;

using Microsoft.Xna.Framework;

using WindowsGame1.Blocks;
using WindowsGame1.Bobs;

namespace WindowsGame1
{
    public class ConveyorBlock : Block
    {
        public void TextDraw() { }

        public AABox MyBox;

        public QuadClass MyQuad;
        
        public AABox Box { get { return MyBox; } }

        public bool Active;
        public bool IsActive { get { return Active; } set { Active = value; } }

        public BlockData CoreData;
        public BlockData BlockCore { get { return CoreData; } }
        public ObjectData Core { get { return CoreData as BlockData; } }
        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            Core.Init();
            Core.DrawLayer = 3;
            CoreData.MyType = ObjectType.ConveyorBlock;

            MyQuad.TextureName = "Conveyor";
        }

        public void Release()
        {
            BlockCore.Release();
            Core.MyLevel = null;
            MyQuad = null;
            MyBox = null;
        }

        public ConveyorBlock(bool BoxesOnly)
        {
            CoreData = new BlockData();

            MyQuad = new QuadClass();

            MyBox = new AABox();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Init(Vector2 center, Vector2 size)
        {
            Active = true;

            CoreData.Layer = .35f;
            MyBox = new AABox(center, size);
            MyQuad.Base.Origin = CoreData.Data.Position = CoreData.StartData.Position = center;

            MyBox.Initialize(center, size);

            MyQuad.Base.e1.X = size.X;
            MyQuad.Base.e2.Y = size.Y;

            Update();
        }

        public void Hit(Bob bob) { }
        public void LandedOn(Bob bob)
        {
        }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { } 

        public void Reset(bool BoxesOnly)
        {
            CoreData.BoxesOnly = BoxesOnly;

            Active = true;

            CoreData.Data = CoreData.StartData;

            BlockCore.StoodOn = false;

            MyBox.Current.Center = CoreData.StartData.Position;

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

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);

            CoreData.StoodOn = false;
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public void Update()
        {
            if (CoreData.BoxesOnly) return;
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

            CoreData.StartData.Position = MyBox.Current.Center;
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
                if (!CoreData.BoxesOnly)
                {
                    MyQuad.Base.Origin = MyBox.Current.Center;
                    MyQuad.Draw();
                    Tools.QDrawer.Flush();
                }

                BlockCore.Draw();
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            ConveyorBlock BlockA = A as ConveyorBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);
        }

        public void Write(BinaryWriter writer)
        {
            BlockCore.Write(writer);
        }
        public void Read(BinaryReader reader)
        {
            BlockCore.Read(reader);
        }
    }
}
