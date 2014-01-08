using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom.Blocks
{
    public enum PendulumMoveType { Line, Square }
    public class Pendulum : BlockBase, IBound
    {
        public class PendulumTileInfo : TileInfoBase
        {
            public BlockGroup Group = PieceQuad.ElevatorGroup;
        }

        public PendulumMoveType MoveType;

        public float Angle, MaxAngle, Length;
        public int Period, Offset;
        public Vector2 PivotPoint;

        public override void LandedOn(Bob bob)
        {
            base.LandedOn(bob);
        }

        public override void MakeNew()
        {
            BlockCore.Init();
            BlockCore.MyType = ObjectType.Pendulum;
            CoreData.DrawLayer = 3;

            MyBox.TopOnly = true;

            Angle = 0;
            MaxAngle = 0;
            Period = 150;
            Offset = 0;
            PivotPoint = Vector2.Zero;

            Active = false;

            BlockCore.Layer = .7f;

            CoreData.RemoveOnReset = false;
            BlockCore.HitHead = true;

            CoreData.EditHoldable = CoreData.Holdable = true;
        }

        public Pendulum(bool BoxesOnly)
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

        BlockEmitter_Parameters.BoxStyle MyBoxStyle;
        public void Init(Vector2 center, Vector2 size, Level level, BlockEmitter_Parameters.BoxStyle MyBoxStyle)
        {
            this.MyBoxStyle = MyBoxStyle;

            if (MyBoxStyle == BlockEmitter_Parameters.BoxStyle.NoSides)
            {
                Box.TopOnly = false;
                Box.NoSides = true;
            }
            else if (MyBoxStyle == BlockEmitter_Parameters.BoxStyle.FullBox)
            {
                Box.TopOnly = false;
            }

            base.Init(ref center, ref size, level, level.Info.Pendulums.Group);

            CoreData.Data.Position = CoreData.StartData.Position = PivotPoint = center;
        }

        public void MoveToBounded(Vector2 shift)
        {
            Move(shift);
        }

        public void CalculateLength()
        {
            Length = (CoreData.StartData.Position - PivotPoint).Length();
        }

        public override void Move(Vector2 shift)
        {
            base.Move(shift);

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

        public float AddAngle;
        float CorrespondingAngle;
        Vector2 CalcPosition(float t)
        {
            switch (MoveType)
            {
                default:
                    CorrespondingAngle = MaxAngle * (float)Math.Cos(2 * Math.PI * t);

                    // Horizontal
                    //Vector2 Dir = new Vector2((float)Math.Cos(AddAngle + CorrespondingAngle - Math.PI / 2),
                    //                          (float)-1);

                    // Normal
                    Vector2 Dir = new Vector2((float)Math.Cos(AddAngle + CorrespondingAngle - Math.PI / 2),
                                              (float)Math.Sin(AddAngle + CorrespondingAngle - Math.PI / 2));

                    // Upside-down
                    //Vector2 Dir = new Vector2((float)Math.Cos(AddAngle + CorrespondingAngle - Math.PI / 2),
                    //                          (float)Math.Sin(AddAngle + CorrespondingAngle + Math.PI / 2));

                    // Vertical
                    //Vector2 Dir = new Vector2((float)0,
                    //                          (float)Math.Sin(AddAngle + CorrespondingAngle - Math.PI / 2));

                    return PivotPoint + Length * Dir;
            }

            //return BlockCore.StartData.Position;
        }

        public float MyTime = 0;
        public override void PhsxStep()
        {
            if (!CoreData.Held)
            {
                float Step = CoreMath.Modulo(CoreData.GetIndependentPhsxStep() + Offset, (float)Period);
                CoreData.Data.Position = CalcPosition((float)Step / Period);
            }

            Vector2 PhsxCutoff = new Vector2(900 + Length);
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

        public override void PhsxStep2()
        {
            if (!Active) return;

            MyBox.SwapToCurrent();
        }

        public override void Draw()
        {
            bool DrawSelf = true;

            if (!CoreData.Held)
            {
                if (!Active) return;
                                
                if (MyBox.Current.BL.X > BlockCore.MyLevel.MainCamera.TR.X || MyBox.Current.BL.Y > BlockCore.MyLevel.MainCamera.TR.Y)
                    DrawSelf = false;
                if (MyBox.Current.TR.X < BlockCore.MyLevel.MainCamera.BL.X || MyBox.Current.TR.Y < BlockCore.MyLevel.MainCamera.BL.Y)
                    DrawSelf = false;

                if (CoreData.MyLevel.CurrentDrawLayer == CoreData.DrawLayer)
                {
                    //if (PivotLocationType == PivotLocationTypes.TopBottom)
                    {
                        if (PivotPoint.X < CoreData.MyLevel.MainCamera.TR.X && PivotPoint.X > CoreData.MyLevel.MainCamera.BL.X)
                            DrawSelf = true;
                    }
                    //else
                    //{
                    //    if (PivotPoint.Y < Core.MyLevel.MainCamera.TR.Y && PivotPoint.Y > Core.MyLevel.MainCamera.BL.Y)
                    //        DrawSelf = true;
                    //}
                }
            }

            if (DrawSelf)
            {
                if (Tools.DrawBoxes)
                    MyBox.Draw(Tools.QDrawer, Color.Olive, 15);
            }

            if (Tools.DrawGraphics)
            {
                if (DrawSelf && !BlockCore.BoxesOnly)
                {
                    //Vector2 add = new Vector2(Box.Current.Size.X, 0);
                    //Tools.QDrawer.DrawLine(Core.Data.Position + add, PivotPoint + add, Info.SpikeyGuys.Chain);
                    //Tools.QDrawer.DrawLine(Core.Data.Position - add, PivotPoint - add, Info.SpikeyGuys.Chain);

					int RepeatWidth = 1900;
					if (CoreData.Data.Position.Y < -1200) RepeatWidth = 2150;
					Tools.QDrawer.DrawLine(CoreData.Data.Position, PivotPoint + (PivotPoint - CoreData.Data.Position), Info.Boulders.Chain, RepeatWidth);

                    MyDraw.Update();
                    MyDraw.Draw();
                }

                BlockCore.Draw();
            }
        }

        public override void ResetPieces()
        {
            if (Info.Pendulums.Group != null)
                if (MyDraw.MyTemplate != null)
                {
                    MyDraw.MyTemplate = CoreData.MyTileSet.GetPieceTemplate(this, Rnd, Info.Pendulums.Group);
                    MyDraw.Init(this, MyDraw.MyTemplate, false);
                }
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
            Pendulum BlockA = A as Pendulum;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size, BlockA.MyLevel, MyBoxStyle);

            CoreData.Clone(A.CoreData);

            MoveType = BlockA.MoveType;

            Angle = BlockA.Angle;
            MaxAngle = BlockA.MaxAngle;
            Period = BlockA.Period;
            Offset = BlockA.Offset;
            PivotPoint = BlockA.PivotPoint;
            Length = BlockA.Length;

            AddAngle = BlockA.AddAngle;
            //PivotLocationType = BlockA.PivotLocationType;
        }
    }
}
