using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class CircleBox
    {
        public bool Invalidated;

        public Vector2 BL, TR;
        Vector2 _Center;
        public Vector2 Center { get { return _Center; } set { _Center = value; Invalidate(); } }
        float _Radius;
        public float Radius { get { return _Radius; } set { _Radius = value; Invalidate(); } }

        public void Invalidate()
        {
            Invalidated = true;
        }

        public void MakeNew()
        {
            Invalidated = true;
        }

        public CircleBox()
        {
            MakeNew();
        }

        public CircleBox(Vector2 center, float radius)
        {
            Initialize(center, radius);
        }

        public void Move(Vector2 shift)
        {
            Center += shift;

            CalcBounds();
        }

        public void Clone(CircleBox A)
        {
            Center = A.Center;
            Radius = A.Radius;

            TR = A.TR;
            BL = A.BL;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Center);
            writer.Write(Radius);
        }

        public void Read(BinaryReader reader)
        {
            Center = reader.ReadVector2();
            Radius = reader.ReadSingle();
        }

        public void Draw(Color color)
        {
            Tools.QDrawer.DrawCircle(Center, Radius, color);
            Tools.QDrawer.Flush();
        }

        public void Initialize(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;

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

        public void CalcBounds()
        {
            TR = Center + new Vector2(Radius, Radius);
            BL = Center - new Vector2(Radius, Radius);
        }

        public void Validate()
        {
            if (Invalidated)
            {
                CalcBounds();

                Invalidated = false;
            }
        }

        public void Scale(float scale)
        {
            Radius *= scale;
        }

        /// <summary>
        /// Performs a collision detection between the circle and a list of tiered boxes associated with the Bob.
        /// </summary>
        public bool BoxOverlap_Tiered(ObjectData Core, Bob bob, AutoGen singleton)
        {
            AutoGen_Parameters Params = Core.GetParams(singleton);
            int WidthLevel = (int)(Params.BobWidthLevel.GetVal(Core.Data.Position));

            bool col = BoxOverlap(bob.GetBox(WidthLevel));

            return col;
        }

        /// <summary>
        /// Returns true if the box overlaps the circle
        /// </summary>
        public bool BoxOverlap(AABox box)
        {
            Validate();

            if (box.Target.BL.X >= TR.X) return false;
            if (box.Target.TR.X <= BL.X) return false;

            if (box.Target.BL.Y >= TR.Y) return false;
            if (box.Target.TR.Y <= BL.Y) return false;

            FloatRectangle rect = box.Target;

            return
                VerticalSegmentOverlap(rect.TR.X, rect.BL.Y, rect.TR.Y)   || // Right segment
                VerticalSegmentOverlap(rect.BL.X, rect.BL.Y, rect.TR.Y)   || // Left segment 
                HorizontalSegmentOverlap(rect.TR.Y, rect.BL.X, rect.TR.X) || // Top segment
                HorizontalSegmentOverlap(rect.BL.Y, rect.BL.X, rect.TR.X) || // Bottom segment
                ContainedIn(box);                                            // Inside
        }

        /// <summary>
        /// Whether the circle is contained in the FloatRectangle
        /// </summary>
        public bool ContainedIn(FloatRectangle rect)
        {
            return TR.LE(rect.TR) && BL.GE(rect.BL);
        }

        /// <summary>
        /// Whether the circle is contained in the AABox
        /// </summary>
        public bool ContainedIn(AABox box)
        {
            box.CalcBounds();
            return TR.LE(box.TR) && BL.GE(box.BL);
        }

        /// <summary>
        /// Checks if a vertical line segment from (x,y1) to (x,y2) intersects the circle.
        /// Requires that y1 is less than y2
        /// </summary>
        public bool VerticalSegmentOverlap(float x, float y1, float y2)
        {
            // Get the height of the circle at x (relative to the center)
            float h = Height(x - Center.X);

            if (h == 0) return false;

            // Get the heights of the line segment (relative to the center)
            float h1 = y1 - Center.Y;
            float h2 = y2 - Center.Y;

            if (h1 > h) return false;
            if (h2 < -h) return false;

            return true;
        }

        /// <summary>
        /// Checks if a horizontal line segment from (x1,y) to (x2,y) intersects the circle.
        /// Requires that x1 is less than x2
        /// </summary>
        public bool HorizontalSegmentOverlap(float y, float x1, float x2)
        {
            // Get the height of the circle at y (relative to the center)
            float h = Height(y - Center.Y);

            if (h == 0) return false;

            // Get the heights of the line segment (relative to the center)
            float h1 = x1 - Center.X;
            float h2 = x2 - Center.X;

            if (h1 > h) return false;
            if (h2 < -h) return false;

            return true;
        }

        /// <summary>
        /// Returns the height of the circle at a distance x from the center.
        /// </summary>
        public float Height(float x)
        {
            if (x > Radius || x < -Radius) return 0;
            return (float)Math.Sqrt(Radius * Radius - x * x);
        }
    }
}