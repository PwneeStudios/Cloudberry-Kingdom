using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace CloudberryKingdom
{
    public enum Side { Right, Left, Top, Bottom };

    public class AABox
    {
        public bool Invalidated;

        public Vector2 BL, TR;
        public FloatRectangle Current, Target;
        public bool TopOnly;

        public Vector2 RealTR() { return Vector2.Max(TR, BL); }
        public Vector2 RealBL() { return Vector2.Min(TR, BL); }

        public void MakeNew()
        {
            TopOnly = false;
            Invalidated = true;
        }

        public AABox()
        {
            MakeNew();
        }

        public AABox(Vector2 center, Vector2 size)
        {
            TopOnly = false;
            Initialize(center, size);
        }

        public void Move(Vector2 shift)
        {
            Current.Center += shift;
            Target.Center += shift;

            Current.CalcBounds();
            Target.CalcBounds();

            CalcBounds();
        }

        public void Clone(AABox A)
        {
            Current.Clone(A.Current);
            Target.Clone(A.Target);
            TopOnly = A.TopOnly;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(TopOnly);
            Current.Write(writer);
            Target.Write(writer);

        }
        public void Read(BinaryReader reader)
        {
            TopOnly = reader.ReadBoolean();
            if (Current == null) Current = new FloatRectangle();
            Current.Read(reader);
            if (Target == null) Target = new FloatRectangle();
            Target.Read(reader);
        }

        public void Extend(Side side, float pos)
        {
            switch (side)
            {
                case Side.Left:
                    Target.BL.X = pos;
                    break;
                case Side.Right:
                    Target.TR.X = pos;
                    break;
                case Side.Top:
                    Target.TR.Y = pos;
                    break;
                case Side.Bottom:
                    Target.BL.Y = pos;
                    break;
            }

            Target.FromBounds();
            SwapToCurrent();
        }

        public void DrawFilled(QuadDrawer QDrawer, Color color)
        {
            QDrawer.DrawFilledBox(Current.BL, Current.TR, color);
            QDrawer.Flush();
        }

        public void Draw(QuadDrawer QDrawer, Color color, float Width)
        {
            Draw(QDrawer, color, Width, false);
        }
        public void Draw(QuadDrawer QDrawer, Color color, float Width, bool DisregardTopOnly)
        {
            Vector2 BR, TL;
            BR = new Vector2(Current.TR.X, Current.BL.Y);
            TL = new Vector2(Current.BL.X, Current.TR.Y);
            if (!TopOnly || DisregardTopOnly)
            {
                QDrawer.DrawLine(Current.BL, BR, color, Width);
                QDrawer.DrawLine(Current.TR, BR, color, Width);
                QDrawer.DrawLine(Current.BL, TL, color, Width);
            }
            QDrawer.DrawLine(Current.TR, TL, color, Width);
            QDrawer.Flush();
        }

        public void DrawT(QuadDrawer QDrawer, Color color, float Width)
        {
            Vector2 BR, TL;
            BR = new Vector2(Target.TR.X, Target.BL.Y);
            TL = new Vector2(Target.BL.X, Target.TR.Y);
            QDrawer.DrawLine(Target.BL, BR, color, Width);
            QDrawer.DrawLine(Target.TR, BR, color, Width);
            QDrawer.DrawLine(Target.BL, TL, color, Width);
            QDrawer.DrawLine(Target.TR, TL, color, Width);
            QDrawer.Flush();
        }

        public void Initialize(Vector2 center, Vector2 size)
        {
            if (Current == null) Current = new FloatRectangle(center, size);
            else Current.Set(center, size);
            if (Target == null) Target = new FloatRectangle(center, size);
            else Target.Set(center, size);

            Invalidated = true;
        }

        public Vector2 GetTR()
        {
            Validate();
            return TR;
        }

        public Vector2 GetBL()
        {
            Validate();
            return BL;
        }

        public float BoxSize()
        {
            return Tools.BoxSize(TR, BL);
        }

        public void CalcBounds()
        {
            TR = Vector2.Max(Current.TR, Target.TR);
            BL = Vector2.Min(Current.BL, Target.BL);

            if (TopOnly)
                BL.Y = TR.Y;
        }

        public void CalcBounds_Full()
        {
            Current.CalcBounds();
            Target.CalcBounds();

            TR = Vector2.Max(Current.TR, Target.TR);
            BL = Vector2.Min(Current.BL, Target.BL);

            if (TopOnly)
                BL.Y = TR.Y;
        }

        public void Validate()
        {
            if (Invalidated)
            {
                CalcBounds();

                Invalidated = false;
            }
        }

        public void SetCurrent(Vector2 center, Vector2 size)
        {
            Current.Set(center, size);

            Invalidated = true;
        }

        public void SetTarget(Vector2 center, Vector2 size)
        {
            Target.Set(center, size);

            Invalidated = true;
        }

        public void SwapToCurrent()
        {
            Current.Set(Target.Center, Target.Size);

            Invalidated = true;
        }
    }
}