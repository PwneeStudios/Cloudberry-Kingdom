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
            Core.DrawLayer = 3;

            Displacement = new Vector2(200, 0);
            Period = 400;
            Offset = 0;

            Active = false;

            BlockCore.Layer = .7f;

            Core.RemoveOnReset = false;
            BlockCore.HitHead = true;

            Core.EditHoldable = Core.Holdable = true;
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
               
        public override void ResetPieces()
        {
            if (Info.MovingBlocks.Group != null)
            {
                if (MyDraw.MyTemplate != null)
                {
                    MyDraw.MyTemplate = Core.MyTileSet.GetPieceTemplate(this, Rnd, Info.MovingBlocks.Group);
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

            Core.Data = BlockCore.Data = BlockCore.StartData;

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
            if (!Core.Held)
            {
                float Step = CoreMath.Modulo(Core.GetIndependentPhsxStep() + Offset, (float)Period);
                Core.Data.Position = CalcPosition((float)Step / Period);
            }

            Vector2 PhsxCutoff = new Vector2(1250);
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

            MyBox.SetTarget(MyBox.Target.Center, MyBox.Current.Size);
            if (!Active)
                MyBox.SwapToCurrent();

            Active = true;
        }

        public override void Draw()
        {
            bool DrawSelf = true;
            if (!Core.Held)
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

            if (!Core.BoxesOnly)
                ResetPieces();

            BlockCore.StartData.Position = MyBox.Current.Center;

            ResetPieces();
        }

        public override void Clone(ObjectBase A)
        {
            MovingBlock BlockA = A as MovingBlock;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.MyLevel);

            Core.Clone(A.Core);

            MoveType = BlockA.MoveType;
            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Displacement = BlockA.Displacement;
        }
    }
}
