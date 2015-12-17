using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public struct IntVector2
    {
        public int X, Y;
        public IntVector2(int X, int Y) { this.X = X; this.Y = Y; }
        public IntVector2(float X, float Y) { this.X = (int)X; this.Y = (int)Y; }
        public IntVector2(Vector2 v) { X = (int)v.X; Y = (int)v.Y; }

        public static IntVector2 operator +(IntVector2 A, IntVector2 B)
        {
            return new IntVector2(A.X + B.X, A.Y + B.Y);
        }
        public static IntVector2 operator *(IntVector2 A, IntVector2 B)
        {
            return new IntVector2(A.X * B.X, A.Y * B.Y);
        }
        public static IntVector2 operator *(IntVector2 A, Vector2 B)
        {
            return new IntVector2(A.X * B.X, A.Y * B.Y);
        }

        public static IntVector2 operator *(IntVector2 A, float a)
        {
            return new IntVector2(A.X * a, A.Y * a);
        }

        public static implicit operator Vector2(IntVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}