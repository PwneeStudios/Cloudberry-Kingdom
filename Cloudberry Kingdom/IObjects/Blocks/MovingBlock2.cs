using System;
using System.IO;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Blocks
{
    public enum MovingBlock2MoveType { Line, Square }
    public class MovingBlock2 : BlockBase, IBound
    {
        public MovingBlock2MoveType MoveType;
        public int Period, Offset;
        public Vector2 Displacement;

        public QuadClass MyQuad;

        public Vector2[] Points = new Vector2[5];
        public int NumPoints = 0;

        public override void MakeNew()
        {
            NumPoints = 0;

            BlockCore.Init();
            BlockCore.MyType = ObjectType.MovingBlock2;
            Core.DrawLayer = 3;

            MyBox.TopOnly = true;

            Displacement = new Vector2(200, 0);
            Period = 400;
            Offset = 0;

            Active = false;

            BlockCore.Layer = .7f;

            Core.RemoveOnReset = false;
            BlockCore.HitHead = true;

            Core.EditHoldable = Core.Holdable = true;
        }

        public override void Release()
        {
            base.Release();

            MyQuad = null;
        }

        public MovingBlock2(bool BoxesOnly)
        {
            MyBox = new AABox();
            MyQuad = new QuadClass();

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

        public void Init(Vector2 center, Vector2 size) { Init(center, size, true); }
        public void Init(Vector2 center, Vector2 size, bool TopOnly)
        {
            if (!TopOnly)
                Box.TopOnly = false;

            MyBox.Initialize(center, size);
            Core.Data.Position = BlockCore.Data.Position = BlockCore.StartData.Position = center;

            //if (!Core.BoxesOnly)
            {
                MyQuad.SetToDefault();
                MyQuad.TextureName = "Palette";
                MyQuad.Quad.SetColor(new Color(210, 210, 210));

                MyQuad.Base.e1.X = size.X;
                MyQuad.Base.e2.Y = size.Y;

                Update();
            }
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

        public override void Reset(bool BoxesOnly)
        {
            BlockCore.BoxesOnly = BoxesOnly;

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
                case MovingBlock2MoveType.Line:
                    //return BlockCore.StartData.Position + Displacement * (float)Math.Cos(2 * Math.PI * t);
                    //return Tools.MultiLerpRestrict(t, BlockCore.StartData.Position + Displacement,
                    //                                  BlockCore.StartData.Position - Displacement);
                    return Tools.MultiLerpRestrict(Tools.ZigZag(1, t) * (NumPoints - 1), Points);
            }

            return BlockCore.StartData.Position;
        }

        public override void PhsxStep()
        {
            if (!Core.Held)
            {
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

        void Update()
        {
            if (BlockCore.BoxesOnly) return;

            MyQuad.Pos = MyBox.Target.Center;

            MyQuad.Update();
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
                    for (int i = 0; i < NumPoints - 1; i++)
                        Tools.QDrawer.DrawLine(Points[i], Points[i + 1], Tools.GrayColor(.5f), 9f);
                    for (int i = 0; i < NumPoints; i++)
                        Tools.QDrawer.DrawCircle(Points[i], 12f, Tools.GrayColor(.35f));
                    MyQuad.Draw();
                }

                BlockCore.Draw();
            }
        }

        public override void Extend(Side side, float pos)
        {
            MyBox.Invalidated = true;

            MyBox.Extend(side, pos);

            Update();

            BlockCore.StartData.Position = MyBox.Current.Center;
        }

        public override void Clone(ObjectBase A)
        {
            MovingBlock2 BlockA = A as MovingBlock2;

            Init(BlockA.Box.Current.Center, BlockA.Box.Current.Size);

            Core.Clone(A.Core);

            MoveType = BlockA.MoveType;
            Period = BlockA.Period;
            Offset = BlockA.Offset;
            Displacement = BlockA.Displacement;
        }
    }
}
