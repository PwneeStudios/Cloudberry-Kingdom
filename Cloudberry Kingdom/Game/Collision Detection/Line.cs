using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public struct Line { public Vector2 p1, p2; }
    public struct MovingLine
    {
        public Line Current, Target;
        public Vector2 TR, BL;
        public bool Invalidated;

        public bool SkipOverlap, SkipEdge;

        public void SetCurrent(Vector2 _p1, Vector2 _p2)
        {
            Current.p1 = _p1;
            Current.p2 = _p2;

            Invalidated = true;
        }

        public void SetTarget(Vector2 _p1, Vector2 _p2)
        {
            Target.p1 = _p1;
            Target.p2 = _p2;

            Invalidated = true;
        }

        public void SwapToCurrent()
        {
            Current = Target;

            Invalidated = true;
        }

        public void CalcBounds()
        {
            // Expensive
            //TR = Vector2Extension.Max(Vector2Extension.Max(Current.p1, Current.p2), Vector2Extension.Max(Target.p1, Target.p2));
            //BL = Vector2Extension.Min(Vector2Extension.Min(Current.p1, Current.p2), Vector2Extension.Min(Target.p1, Target.p2));
            
            // Cheaper
            //TR = Vector2Extension.Max(Target.p1, Target.p2);
            //BL = Vector2Extension.Min(Target.p1, Target.p2);

            // Cheapest
            CoreMath.MaxAndMin(ref Target.p1, ref Target.p2, ref TR, ref BL);
        }

        public void Validate()
        {
            if (Invalidated)
            {
                CalcBounds();

                Invalidated = false;
            }
        }
    }
}