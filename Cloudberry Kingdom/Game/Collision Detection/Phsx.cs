using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public struct colData
    {
        public bool Collision;
        public float Location;
    }

    public enum ColType { NoCol, Left, Right, Top, Bottom };

    public class Phsx
    {
        /// <summary>
        /// Return true if a point is in a box specified by BL and TR.
        /// </summary>
        public static bool Inside(Vector2 p, Vector2 BL, Vector2 TR)
        {
            if (p.X > TR.X) return false;
            if (p.X < BL.X) return false;
            if (p.Y > TR.Y) return false;
            if (p.Y < BL.Y) return false;

            return true;
        }

        public static bool Inside(Vector2 p, Vector2 BL, Vector2 TR, Vector2 padding)
        {
            return Inside(p, BL - padding, TR + padding);
        }

        /// <summary>
        /// Performs a collision detection between Box and a list of tiered boxes associated with the Bob.
        /// </summary>
        public static bool BoxBoxOverlap_Tiered(AABox Box, ObjectData Core, Bob bob, AutoGen singleton)
        {
            AutoGen_Parameters Params = Core.GetParams(singleton);
            int WidthLevel = (int)(Params.BobWidthLevel.GetVal(Core.Data.Position));

            bool col = Phsx.BoxBoxOverlap(bob.GetBox(WidthLevel), Box);

            return col;
        }

        /// <summary>
        /// Performs a collision detection between Box and a list of tiered boxes associated with the Bob.
        /// </summary>
        public static bool AABoxAndLineCollisionTest_Tiered(ref MovingLine Line, ObjectData Core, Bob bob, AutoGen singleton)
        {
            AutoGen_Parameters Params = Core.GetParams(singleton);
            int WidthLevel = (int)(Params.BobWidthLevel.GetVal(Core.Data.Position));

            bool col = Phsx.AABoxAndLineCollisionTest(bob.GetBox(WidthLevel), ref Line);

            return col;
        }

        /// <summary>
        /// Returns true if A.Current and B.Current overlap
        /// </summary>
        public static bool BoxBoxOverlap(AABox A, AABox B)
        {
            // Do not need to validate if we are using *.Target directly
            //A.Validate();
            //B.Validate();
            
            //A.Current.CalcReal();
            //B.Current.CalcReal();

            /*
            Vector2 A_BL = A.Current.RealBL;
            Vector2 B_TR = B.Current.RealTR;
            if (A_BL.X >= B_TR.X || A_BL.Y >= B_TR.Y) return false;

            Vector2 A_TR = A.Current.RealTR;
            Vector2 B_BL = B.Current.RealBL;
            if (A_TR.X <= B_BL.X || A_TR.Y <= B_BL.Y) return false;
             * */

            if (A.Target.BL.X >= B.Target.TR.X) return false;            
            if (A.Target.TR.X <= B.Target.BL.X) return false;

            if (A.Target.BL.Y >= B.Target.TR.Y) return false;

            if (B.TopOnly)
            {
                if (A.Target.TR.Y <= B.Target.TR.Y) return false;
            }
            else
            {
                if (A.Target.TR.Y <= B.Target.BL.Y) return false;
            }
            
            //if (A.BL.X >= B.TR.X || A.BL.Y >= B.TR.Y) return false;
            //if (A.TR.X <= B.BL.X || A.TR.Y <= B.BL.Y) return false;

            return true;
        }

        public static bool PointAndAABoxCollisionTest(ref Vector2 p, AABox Box)
        {
            return PointAndAABoxCollisionTest(ref p, Box, 0);
        }
        public static bool PointAndAABoxCollisionTest(ref Vector2 p, AABox Box, float Padding)
        {
            Box.Validate();
            Vector2 A_BL = Vector2.Min(Box.Current.BL, Box.Target.BL);
            if (A_BL.X > p.X + Padding || A_BL.Y > p.Y + Padding) return false;

            Vector2 A_TR = Vector2.Max(Box.Current.TR, Box.Target.TR);
            if (A_TR.X < p.X - Padding || A_TR.Y < p.Y - Padding) return false;

            return true;
        }

        public static bool PointAndLineCollisionTest(ref Vector2 t1, ref Vector2 t2, ref Vector2 n1, ref Vector2 n2, ref Vector2 LC1, ref Vector2 LT1, float Length, ref Vector2 p1, ref Vector2 p2)
        {
            float d1 = Vector2.Dot(p1 - LC1, n1);
            float d2 = Vector2.Dot(p2 - LT1, n2);

            if (Math.Sign(d1) != Math.Sign(d2))
            {
                float n = Vector2.Dot(p1 - LC1, t1);
                if (n > 0 && n < Length)
                    //Console.WriteLine("!!!!!!!!!!!!!!!");
                    return true;
            }

            return false;
        }

        public static bool LineAndHorizontalLineCollisionTest(float y, float x1, float x2, ref Line L)
        {
            if (Math.Sign(L.p1.Y - y) == Math.Sign(L.p2.Y - y)) return false;

            float t = (y - L.p2.Y) / (L.p1.Y - L.p2.Y);
            if (t < 0 || t > 1) return false;
            float x = t * L.p1.X + (1 - t) * L.p2.X;
            if (Math.Sign(x - x1) == Math.Sign(x - x2)) return false;

            return true;
        }

        public static bool LineAndVerticalLineCollisionTest(float x, float y1, float y2, ref Line L)
        {
            if (Math.Sign(L.p1.X - x) == Math.Sign(L.p2.X - x)) return false;

            float t = (x - L.p2.X) / (L.p1.X - L.p2.X);
            if (t < 0 || t > 1) return false;
            float y = t * L.p1.Y + (1 - t) * L.p2.Y;
            if (Math.Sign(y - y1) == Math.Sign(y - y2)) return false;

            return true;
        }

        public static bool AABoxAndLineCollisionTest(AABox Box, ref MovingLine L)
        {
            Box.Validate();
            L.Validate();
            if (L.BL.X > Box.TR.X || L.BL.Y > Box.TR.Y) return false;
            if (L.TR.X < Box.BL.X || L.TR.Y < Box.BL.Y) return false;

            if (false)//L.SkipEdge)
            {
                Vector2 p1, p2;

                Vector2 t1 = L.Current.p2 - L.Current.p1;
                Vector2 t2 = L.Target.p2 - L.Target.p1;
                float Length = t1.Length();
                t1.Normalize(); t2.Normalize();
                Vector2 n1 = new Vector2(); n1.X = t1.Y; n1.Y = -t1.X;
                Vector2 n2 = new Vector2(); n2.X = t2.Y; n2.Y = -t2.X;

                p1 = Box.Current.TR;
                p2 = Box.Target.TR;
                if (PointAndLineCollisionTest(ref t1, ref t2, ref n1, ref n2, ref L.Current.p1, ref L.Target.p1, Length, ref p1, ref p2))
                    return true;

                p1 = Box.Current.BL;
                p2 = Box.Target.BL;
                if (PointAndLineCollisionTest(ref t1, ref t2, ref n1, ref n2, ref L.Current.p1, ref L.Target.p1, Length, ref p1, ref p2))
                    return true;

                p1 = new Vector2(Box.Current.TR.X, Box.Current.BL.Y);
                p2 = new Vector2(Box.Target.TR.X, Box.Target.BL.Y);
                if (PointAndLineCollisionTest(ref t1, ref t2, ref n1, ref n2, ref L.Current.p1, ref L.Target.p1, Length, ref p1, ref p2))
                    return true;

                p1 = new Vector2(Box.Current.BL.X, Box.Current.TR.Y);
                p2 = new Vector2(Box.Target.BL.X, Box.Target.TR.Y);
                if (PointAndLineCollisionTest(ref t1, ref t2, ref n1, ref n2, ref L.Current.p1, ref L.Target.p1, Length, ref p1, ref p2))
                    return true;

                if (PointAndAABoxCollisionTest(ref L.Current.p1, Box)) return true;
                if (PointAndAABoxCollisionTest(ref L.Current.p2, Box)) return true;
            }

            if (!L.SkipOverlap)
            {
                if (LineAndHorizontalLineCollisionTest(Box.Target.TR.Y, Box.Target.TR.X, Box.Target.BL.X, ref L.Target)) return true;
                if (LineAndHorizontalLineCollisionTest(Box.Target.BL.Y, Box.Target.TR.X, Box.Target.BL.X, ref L.Target)) return true;
                if (LineAndVerticalLineCollisionTest(Box.Target.TR.X, Box.Target.TR.Y, Box.Target.BL.Y, ref L.Target)) return true;
                if (LineAndVerticalLineCollisionTest(Box.Target.BL.X, Box.Target.TR.Y, Box.Target.BL.Y, ref L.Target)) return true;
            }

            return false;
        }



        public static bool AALineCollisionTest(float ACy, float ATy, float BCy, float BTy, float ACx1, float ACx2, float ATx1, float ATx2, float BCx1, float BCx2, float BTx1, float BTx2)
        {
            int CurrentSign, TargetSign;

            CurrentSign = Math.Sign(ACy - BCy);
            TargetSign = Math.Sign(ATy - BTy);
            if (CurrentSign != TargetSign || CurrentSign == 0 && TargetSign == 0)
            {
                if (Math.Max(ACx2, ATx2) < Math.Max(BCx1, BTx1))
                    return false;
                if (Math.Min(ACx1, ATx1) > Math.Min(BCx2, BTx2))
                    return false;

                return true;
            }

            return false;
        }

        public static ColType CollisionTest(AABox A, AABox B)
        {
            /*
            A.Current.CalcReal();
            A.Target.CalcReal();
            B.Current.CalcReal();
            B.Target.CalcReal();

            Vector2 A_BL = Vector2.Min(A.Current.RealBL, A.Target.RealBL);
            Vector2 B_TR = Vector2.Max(B.Current.RealTR, B.Target.RealTR);
            if (A_BL.X > B_TR.X || A_BL.Y > B_TR.Y) return ColType.NoCol;

            Vector2 A_TR = Vector2.Max(A.Current.RealTR, A.Target.RealTR);
            Vector2 B_BL = Vector2.Min(B.Current.RealBL, B.Target.RealBL);
            if (A_TR.X < B_BL.X || A_TR.Y < B_BL.Y) return ColType.NoCol;
            */

            A.Validate();
            B.Validate();
            if (A.BL.X > B.TR.X || A.BL.Y > B.TR.Y) return ColType.NoCol;
            if (A.TR.X < B.BL.X || A.TR.Y < B.BL.Y) return ColType.NoCol;

            ColType type = ColType.NoCol;

            // A bottom to B top
            if (!A.TopOnly)
            {
                if (A.Current.BL.Y >= B.Current.TR.Y)
                    if (AALineCollisionTest(A.Current.BL.Y, A.Target.BL.Y, B.Current.TR.Y, B.Target.TR.Y, A.Current.BL.X, A.Current.TR.X, A.Target.BL.X, A.Target.TR.X, B.Current.BL.X, B.Current.TR.X, B.Target.BL.X, B.Target.TR.X))
                        type = ColType.Top;
            }

            // A top to B bottom
            if (!B.TopOnly)
            {
                if (A.Current.TR.Y <= B.Current.BL.Y)
                    if (AALineCollisionTest(A.Current.TR.Y, A.Target.TR.Y, B.Current.BL.Y, B.Target.BL.Y, A.Current.BL.X, A.Current.TR.X, A.Target.BL.X, A.Target.TR.X, B.Current.BL.X, B.Current.TR.X, B.Target.BL.X, B.Target.TR.X))
                        type = ColType.Bottom;
            }

            if (!A.TopOnly && !B.TopOnly)
            {
                // A right to B left
                if (A.Current.TR.X <= B.Current.BL.X && A.Target.TR.X - A.Current.TR.X >= B.Target.BL.X - B.Current.BL.X)
                    if (AALineCollisionTest(A.Current.TR.X, A.Target.TR.X, B.Current.BL.X, B.Target.BL.X, A.Current.BL.Y, A.Current.TR.Y, A.Target.BL.Y, A.Target.TR.Y, B.Current.BL.Y, B.Current.TR.Y, B.Target.BL.Y, B.Target.TR.Y))
                        type = ColType.Left;

                // A left to B right
                if (A.Current.BL.X >= B.Current.TR.X && A.Target.BL.X - A.Current.BL.X <= B.Target.TR.X - B.Current.TR.X)
                    if (AALineCollisionTest(A.Current.BL.X, A.Target.BL.X, B.Current.TR.X, B.Target.TR.X, A.Current.BL.Y, A.Current.TR.Y, A.Target.BL.Y, A.Target.TR.Y, B.Current.BL.Y, B.Current.TR.Y, B.Target.BL.Y, B.Target.TR.Y))
                        type = ColType.Right;
            }

            return type;
        }
    }
}