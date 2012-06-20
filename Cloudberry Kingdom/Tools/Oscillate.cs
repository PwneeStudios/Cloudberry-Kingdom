using System;

namespace CloudberryKingdom
{
    public struct OscillateParams
    {
        public enum Type { None, Oscillate, GetBig, GetBigSlow, Jiggle };

        public Type MyType;
        public void SetType(Type type) { SetType(type, true); }
        public void SetType(Type type, bool DoReset)
        {
            MyType = type;
            Done = false;

            if (DoReset)
                Reset();
        }

        public float w, base_value, max_addition;

        public int Count;

        public bool UseGlobalCount;

        /// <summary>
        /// Dummy variable for internal use.
        /// </summary>
        float scale;

        public void Set(float w, float base_value, float max_addition)
        {
            MyType = Type.Oscillate;
            scale = base_value;

            this.w = w;
            this.base_value = base_value;
            this.max_addition = max_addition;
        }

        float TargetJiggleScale;
        public void Reset()
        {
            switch (MyType)
            {
                case Type.Jiggle:
                    TargetJiggleScale = scale;
                    Count = 0; break;

                default:
                    scale = base_value;
                    Count = 0;
                    break;
            }
        }

        /// <summary>
        /// If the current animation is not infinite (looped) then this variable is true
        /// when the animation finishes.
        /// </summary>
        public bool Done;

        //static float[] JiggleScale = { 1.25f, .9f, 1.08f, 1f };
        static float[] JiggleScale = { 1.15f, .94f, 1.05f, 1f };
        float JigglePhsx()
        {
            Count++;

            float target = base_value + max_addition * 0f;

            if (Count >= 7 * JiggleScale.Length)
            {
                scale = TargetJiggleScale;
                Done = true;
            }
            else
            {
                scale = .5f * (scale + JiggleScale[Count / 7] * TargetJiggleScale);
                Done = false;
            }

            return scale;
        }

        float GetScale_GetBig(float speed)
        {
            scale = (1 - speed) * scale + speed * (base_value + max_addition);

            return scale;
        }

        public float GetScale()
        {
            switch (MyType)
            {
                case OscillateParams.Type.Oscillate:
                    Count++;
                    if (UseGlobalCount) Count = Tools.TheGame.PhsxCount;
                    scale = Oscillate.GetScale_Oscillate(Count, w, base_value, max_addition);
                    break;

                case OscillateParams.Type.GetBig:
                    Count = 0;
                    //scale = GetScale_GetBig(.3f);
                    //scale = GetScale_GetBig(.835f);
                    break;

                case OscillateParams.Type.GetBigSlow:
                    Count = 0;
                    scale = GetScale_GetBig(.15f);
                    break;

                case OscillateParams.Type.Jiggle:
                    scale = JigglePhsx();
                    break;

                case Type.None:
                    break;

                default:
                    scale = 1;
                    break;
            }

            return scale;
        }
    }

    public class Oscillate
    {
        public static float GetAngle(float Step, float w)
        {
            return (float)Math.Sin(.06f * w * Step);
        }

        public static float GetScale_Oscillate(float Step, float w, float base_value, float max_addition)
        {
            return base_value + .5f * (max_addition + max_addition * Oscillate.GetAngle(Step, w));
        }
    }
}