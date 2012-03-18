using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public enum MovingBlockMoveType { Line, Circle, FigureEight }
    public class MovingBlock : BlockBase, IBound
    {
        public override void TextDraw() { }

        public MovingBlockMoveType MoveType;
        public int Period, Offset;
        public Vector2 Displacement;

        public NormalBlockDraw MyDraw;

        public override void MakeNew()
        {
            BlockCore.Init();
            BlockCore.MyType = ObjectType.MovingBlock;
            Core.DrawLayer = 3;

            Displacement = new Vector2(200, 0);
            Period = 400;
            Offset = 0;

            Active = false;

            BlockCore.Layer = .7f;

            Core.RemoveOnReset = false;
            BlockCore.HitHead = true;
            BlockCore.NoComputerTouch = true;

            Core.EditHoldable = Core.Holdable = true;
        }

        public override void Release()
        {
            Core.MyLevel = null;

            MyDraw.Release();
            MyDraw = null;

            MyBox = null;
        }

        public MovingBlock(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public Vector2 TR_Bound()
        {
            Vector2 max =
                Vector2.Max(
                Vector2.Max(CalcPosition(0), CalcPosition(.5f)),
                Vector2.Max(CalcPosition(0.25f), CalcPosition(.75f)));
            return max;
        }

        public Vector2 BL_Bound()
        {
            Vector2 min =
                Vector2.Min(
                Vector2.Min(CalcPosition(0), CalcPosition(.5f)),
                Vector2.Min(CalcPosition(0.25f), CalcPosition(.75f)));

            return min;
        }
               
        public void ResetPieces()
        {
            MyDraw.Init(this, PieceQuad.MovingBlock);

            if (Box.Current.Size.X < 100)
                MyDraw.MyPieces.Center.MyTexture = Tools.TextureWad.FindByName("Blue_Small");
            else if (Box.Current.Size.X < 175)
                MyDraw.MyPieces.Center.MyTexture = Tools.TextureWad.FindByName("Blue_Medium");
            else
                MyDraw.MyPieces.Center.MyTexture = Tools.TextureWad.FindByName("Blue_Large");

            float UV_Repeats = 1.25f * MyBox.Current.Size.Y / MyBox.Current.Size.X;
            if (UV_Repeats > 2)
            {
                MyDraw.MyPieces.Center.v2.Vertex.uv.Y = UV_Repeats;
                MyDraw.MyPieces.Center.v3.Vertex.uv.Y = UV_Repeats;
            }
            if (UV_Repeats < .75f)
                MyDraw.MyPieces.Center.MyTexture = Tools.TextureWad.FindByName("Blue_Thin");
        }

        public void Init(Vector2 center, Vector2 size)
        {
            MyBox.Initialize(center, size);
            Core.Data.Position = BlockCore.Data.Position = BlockCore.StartData.Position = center;

            if (!Core.BoxesOnly)
                MyDraw.Init(this, PieceQuad.MovingBlock);

            Update();
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public override void Move(Vector2 shift)
        {
            BlockCore.Data.Position += shift;
            BlockCore.StartData.Position += shift;

            Box.Move(shift);

            Update();
        }

        public override void Hit(Bob bob) { }
        public override void LandedOn(Bob bob) { }
        public override void HitHeadOn(Bob bob) { } public override void SideHit(Bob bob) { } 

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

            if (!Core.BoxesOnly)
                ResetPieces();           

            Core.Data = BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

            Update();

            Active = false;
        }

        
        Vector2 CalcPosition(float t)
        {
            switch (MoveType)
            {
                case MovingBlockMoveType.Line:
                    return BlockCore.StartData.Position + Displacement * (float)Math.Cos(2 * Math.PI * t);

                case MovingBlockMoveType.Circle:
                    return BlockCore.StartData.Position +
                        new Vector2(Displacement.X * (float)Math.Cos(2 * Math.PI * t),
                                    Displacement.Y * (float)Math.Sin(2 * Math.PI * t));
            }

            return BlockCore.StartData.Position;
        }

        public override void PhsxStep()
        {
            if (!Core.Held)
            {
                //int Step = Tools.Modulo(Core.GetPhsxStep() + Offset, Period);
                float Step = Tools.Modulo(Core.GetIndependentPhsxStep() + Offset, (float)Period);
                Core.Data.Position = CalcPosition((float)Step / Period);
            }

            Vector2 PhsxCutoff = new Vector2(900);
            if (Core.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(500, 500);
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, PhsxCutoff))
            {
                Active = false;
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;
                        
            MyBox.Target.Center = Core.Data.Position;

            Update();

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);
            if (!Active)
                MyBox.SwapToCurrent();

            Active = true;
        }

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }


        public void Update()
        {
            if (BlockCore.BoxesOnly) return;

            MyDraw.Update();
        }

        public override void Draw()
        {
            bool DrawSelf = true;
            if (!Core.Held)
            {
                if (!Active) return;
                                
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                    DrawSelf = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                    DrawSelf = false;
            }

            if (DrawSelf)
            {
                Update();

                if (Tools.DrawBoxes)
                    MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
            }

            if (Tools.DrawGraphics)
            {
                if (DrawSelf && !BlockCore.BoxesOnly)
                {
                    MyDraw.Draw();
                    Tools.QDrawer.Flush();
                }

                BlockCore.Draw();
            }
        }

        public override void Extend(Side side, float pos)
        {
            MyBox.Invalidated = true;

            MyBox.Extend(side, pos);

            Update();

            if (!Core.BoxesOnly)
                ResetPieces();

            BlockCore.StartData.Position = MyBox.Current.Center;

            ResetPieces();
        }

        public override void Interact(Bob bob) { }
        
        public override void Clone(ObjectBase A)
        {
            MovingBlock BlockA = A as MovingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            Core.Clone(A.Core);

            MoveType = BlockA.MoveType;
            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Displacement = BlockA.Displacement;
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
