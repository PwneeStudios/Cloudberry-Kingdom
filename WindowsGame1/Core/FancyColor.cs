using Microsoft.Xna.Framework;

namespace Drawing
{
    public class FancyColor
    {
        FancyVector2 clr1, clr2;
        public float A { get { return clr2.Pos.Y; } }
        public Color Color
        {
            get
            {
                return ToColor(clr1.AbsVal, clr2.AbsVal);
            }
            set
            {
                clr1.Playing = false;
                clr2.Playing = false;
                clr1.RelVal = Pair1(value.ToVector4());
                clr2.RelVal = Pair2(value.ToVector4());
            }
        }

        public void Release()
        {
            clr1.Release();
            clr2.Release();
        }

        public FancyColor()
        {
            Init(Color.White);
        }

        public FancyColor(Color color)
        {
            Init(color);
        }

        void Init(Color color)
        {
            CreateVectors();
            Color = color;
        }

        void CreateVectors()
        {
            clr1 = new FancyVector2();
            clr2 = new FancyVector2();
        }

        static Color ToColor(Vector2 v1, Vector2 v2)
        {
            return new Color(new Vector4(v1.X, v1.Y, v2.X, v2.Y));
        }
        static Vector2 Pair1(Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }
        static Vector2 Pair2(Vector4 v)
        {
            return new Vector2(v.Z, v.W);
        }

        public Color GetDest()
        {
            return ToColor(clr1.GetDest(), clr2.GetDest());
        }

        public void ToAndBack(Vector4 End, int Frames)
        {
            clr1.ToAndBack(Pair1(End), Frames);
            clr2.ToAndBack(Pair2(End), Frames);
        }
        public void ToAndBack(Vector4 Start, Vector4 End, int Frames)
        {
            clr1.ToAndBack(Pair1(Start), Pair1(End), Frames);
            clr2.ToAndBack(Pair2(Start), Pair2(End), Frames);
        }

        LerpStyle DefaultLerpStyle = LerpStyle.Linear;
        public void LerpTo(Vector4 End, int Frames) { LerpTo(End, Frames, DefaultLerpStyle); }
        public void LerpTo(Vector4 End, int Frames, LerpStyle Style)
        {
            clr1.LerpTo(Pair1(End), Frames, Style);
            clr2.LerpTo(Pair2(End), Frames, Style);
        }
        public void LerpTo(Vector4 Start, Vector4 End, int Frames) { LerpTo(Start, End, Frames, DefaultLerpStyle); }
        public void LerpTo(Vector4 Start, Vector4 End, int Frames, LerpStyle Style)
        {
            clr1.LerpTo(Pair1(Start), Pair1(End), Frames, Style);
            clr2.LerpTo(Pair2(Start), Pair2(End), Frames, Style);
        }

        public Color Update()
        {
            return ToColor(clr1.Update(), clr2.Update());
        }
    }
}