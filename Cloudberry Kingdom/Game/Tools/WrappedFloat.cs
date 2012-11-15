using System;

namespace CloudberryKingdom
{
    public class WrappedBool
    {
        public bool MyBool;

        public WrappedBool(bool val) { MyBool = val; }
    }

    public class WrappedInt
    {
        public int MyInt;

        public WrappedInt(int val) { MyInt = val; }
    }

    public class WrappedFloat
    {
        public float MyFloat;
        public float MinVal = float.MinValue, MaxVal = float.MaxValue;

        public float Val
        {
            get
            {
                if (GetCallback != null)
                    MyFloat = GetCallback.Apply();

                return MyFloat;
            }
            set
            {
                Set(value);
            }
        }

        public Lambda SetCallback;
        public LambdaFunc<float> GetCallback;

        public WrappedFloat() { }

        public float DefaultValue;
        public WrappedFloat(float Val, float MinVal, float MaxVal)
        {
            this.MinVal = MinVal;
            this.MaxVal = MaxVal;
            DefaultValue = this.Val = Val;
        }

        public void Set(float val)
        {
            MyFloat = Math.Min(MaxVal, Math.Max(MinVal, val));
            if (SetCallback != null)
                SetCallback.Apply();
        }

        public float Spread { get { return MaxVal - MinVal; } }
        public float Ratio { get { return (Val - MinVal) / Spread; } }
        public float Percent { get { return 100f * Ratio; } }

        public void Release()
        {
            SetCallback = null;
        }
    }
}