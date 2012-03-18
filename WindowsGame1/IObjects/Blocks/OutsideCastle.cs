using Microsoft.Xna.Framework;
using System.IO;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class OutsideCastleBlock : BlockBase, Block
    {
        public void TextDraw() { }

        PieceQuad MyPieces;

        public void Interact(Bob bob) { }

        public void MakeNew()
        {
            Active = true;

            Box.MakeNew();
            Box.TopOnly = true;

            BlockCore.Init();
            BlockCore.Layer = .3f;
            Core.MyType = ObjectType.OutsideCastleBlock;

            MyPieces.Init(Tools.TextureWad.FindByName("Castle"), Tools.BasicEffect);
        }

        public void Release()
        {
            Core.MyLevel = null;
            MyBox = null;
            MyPieces = null;
        }

        public OutsideCastleBlock(bool BoxesOnly)
        {
            MyBox = new AABox();

            Core.BoxesOnly = BoxesOnly;

            MyPieces = new PieceQuad();

            MakeNew();
        }

        public void Init(Vector2 center, Vector2 size)
        {
            BlockCore.Data.Position = BlockCore.StartData.Position = center;

            MyBox.Initialize(center, size);

            MyPieces.Data.RepeatWidth = 180;
            MyPieces.Data.RepeatHeight = 300;
            MyPieces.Right.MyTexture = Tools.TextureWad.FindByName("Castle_Right");
            MyPieces.Left.MyTexture = Tools.TextureWad.FindByName("Castle_Left");
            MyPieces.Center.MyTexture = Tools.TextureWad.FindByName("Castle_Center");
            MyPieces.TR.MyTexture = Tools.TextureWad.FindByName("Castle_TR");
            MyPieces.TL.MyTexture = Tools.TextureWad.FindByName("Castle_TL");
            MyPieces.Top.MyTexture = Tools.TextureWad.FindByName("Castle_Top");

            MyPieces.Data.LeftWidth = 90;
            MyPieces.Data.RightWidth = 90;
            MyPieces.Data.TopWidth = 90;
            MyPieces.TL_UV = new Vector2(63, 94);
            MyPieces.BR_UV = new Vector2(225, 323);
            MyPieces.CalcQuads(size);

            Update();
        }


        public void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public void LandedOn(Bob bob) { }
        public void HitHeadOn(Bob bob) { } public void SideHit(Bob bob) { }
        public void Hit(Bob bob) { }

        public void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            Active = true;

            BlockCore.Data = BlockCore.StartData;

            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();
        }

        public void PhsxStep()
        {
            if (!Active) return;
        }

        public void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (BlockCore.BoxesOnly) return;

            MyPieces.Base.Origin = Box.Current.Center;
            MyPieces.Update();
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

        public void Draw()
        {
            if (!Active) return;

            Vector2 BL = MyBox.Current.BL;//MyQuad.BL();
            if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                return;
            Vector2 TR = MyBox.Current.TR;// MyQuad.TR();
            if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                return;

            Update();

            if (Tools.DrawBoxes)
                MyBox.Draw(Tools.QDrawer, Color.Olive, 15);

            if (BlockCore.BoxesOnly) return;

            if (Tools.DrawGraphics)
            {
                MyPieces.Draw();
                Tools.QDrawer.Flush();
            }
        }

        public void Clone(IObject A)
        {
            OutsideCastleBlock BlockA = A as OutsideCastleBlock;
            BlockCore.Clone(A.Core);

            if (BlockA == null) return;

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
