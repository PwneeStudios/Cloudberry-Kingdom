using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public enum MovingBlockMoveType { Line, Circle, FigureEight }
    public class MovingBlock : BlockBase, IBound
    {
        public class MovingBlockTileInfo : TileInfoBase
        {
            public BlockGroup Group = PieceQuad.MovingGroup;

            public MovingBlockTileInfo()
            {
            }
        }

        public MovingBlockMoveType MoveType;
        public int Period, Offset;
        public Vector2 Displacement;

        public override void MakeNew()
        {
            BlockCore.Init();
            BlockCore.MyType = ObjectType.MovingBlock;
            CoreData.DrawLayer = 3;

            Displacement = new Vector2(200, 0);
            Period = 400;
            Offset = 0;

            Active = false;

            BlockCore.Layer = .7f;

            CoreData.RemoveOnReset = false;
            BlockCore.HitHead = true;

            CoreData.EditHoldable = CoreData.Holdable = true;
        }

        public MovingBlock(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyDraw = new NormalBlockDraw();

            MakeNew();

            CoreData.BoxesOnly = BoxesOnly;
        }

        public Vector2 TR_Bound()
        {
            Vector2 max =
                Vector2Extension.Max(
                Vector2Extension.Max(CalcPosition(0), CalcPosition(.5f)),
                Vector2Extension.Max(CalcPosition(0.25f), CalcPosition(.75f)));
            return max;
        }

        public Vector2 BL_Bound()
        {
            Vector2 min =
                Vector2Extension.Min(
                Vector2Extension.Min(CalcPosition(0), CalcPosition(.5f)),
                Vector2Extension.Min(CalcPosition(0.25f), CalcPosition(.75f)));

            return min;
        }
               
        public override void ResetPieces()
        {
            if (Info.MovingBlocks.Group != null)
            {
                if (MyDraw.MyTemplate != null)
                {
                    MyDraw.MyTemplate = CoreData.MyTileSet.GetPieceTemplate(this, Rnd, Info.MovingBlocks.Group);
                    MyDraw.Init(this, MyDraw.MyTemplate, false);
                }
            }
        }

        public void Init(Vector2 center, Vector2 size, Level level)
        {
            base.Init(ref center, ref size, level, level.Info.MovingBlocks.Group);            
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
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            CoreData.Data = BlockCore.Data = BlockCore.StartData;

            MyBox.Current.Center = BlockCore.StartData.Position;
            MyBox.SetTarget(MyBox.Current.Center, MyBox.Current.Size);
            MyBox.SwapToCurrent();

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
            if (!CoreData.Held)
            {
                float Step = CoreMath.Modulo(CoreData.GetIndependentPhsxStep() + Offset, (float)Period);
                CoreData.Data.Position = CalcPosition((float)Step / Period);
            }

            Vector2 PhsxCutoff = new Vector2(1250);
            if (CoreData.MyLevel.BoxesOnly) PhsxCutoff = new Vector2(500, 500);
            if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, PhsxCutoff))
            {
                Active = false;
                CoreData.SkippedPhsx = true;
                CoreData.WakeUpRequirements = true;
                return;
            }
            CoreData.SkippedPhsx = false;
                        
            MyBox.Target.Center = CoreData.Data.Position;

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);
            if (!Active)
                MyBox.SwapToCurrent();

            Active = true;
        }

        public override void Draw()
        {
            bool DrawSelf = true;
            if (!CoreData.Held)
            {
                if (!Active) return;
                                
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X + 50 || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y + 100)
                    DrawSelf = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X - 50 || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y - 100)
                    DrawSelf = false;
            }

            if (DrawSelf)
            {
				if (Tools.DrawBoxes)
				{
					MyBox.DrawFilled(Tools.QDrawer, new Color(35, 35, 255));

					//MyBox.DrawFilled(Tools.QDrawer, Color.DarkBlue);
				}
            }

            if (Tools.DrawGraphics)
            {
                if (DrawSelf && !BlockCore.BoxesOnly)
                {
                    MyDraw.Update();
                    MyDraw.Draw();
                }
            }

            BlockCore.Draw();
        }

        public override void Extend(Side side, float pos)
        {
            MyBox.Invalidated = true;

            MyBox.Extend(side, pos);

            if (!CoreData.BoxesOnly)
                ResetPieces();

            BlockCore.StartData.Position = MyBox.Current.Center;

            ResetPieces();
        }

        public override void Clone(ObjectBase A)
        {
            MovingBlock BlockA = A as MovingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.MyLevel);

            CoreData.Clone(A.CoreData);

            MoveType = BlockA.MoveType;
            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Displacement = BlockA.Displacement;
        }
    }
}
