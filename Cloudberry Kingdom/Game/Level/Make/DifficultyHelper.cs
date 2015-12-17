using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public static class DifficultyHelper
    {
        public static float Interp(float Level1Val, float Level10Val, float level)
        {
            return CoreMath.Lerp(new Vector2(1, Level1Val), new Vector2(10, Level10Val), level);
        }
        public static float InterpRestrict19(float Level1Val, float Level9Val, float level)
        {
            level = CoreMath.Restrict(1, 9, level);
            return Interp19(Level1Val, Level9Val, level);
        }
        public static float Interp19(float Level1Val, float Level9Val, float level)
        {
            return CoreMath.Lerp(new Vector2(1, Level1Val), new Vector2(9, Level9Val), level);
        }
        public static Vector2 InterpRestrict19(Vector2 Level1Val, Vector2 Level9Val, float level)
        {
            level = CoreMath.Restrict(1, 9, level);
            return Interp19(Level1Val, Level9Val, level);
        }
        public static Vector2 Interp19(Vector2 Level1Val, Vector2 Level9Val, float level)
        {
            float t = (level - 1f) / (9f - 1f);
            return Vector2.Lerp(Level1Val, Level9Val, t);
        }
        public static float InterpRestrict159(float Level1Val, float Level5Val, float Level9Val, float level)
        {
            level = CoreMath.Restrict(1, 9, level);
            return Interp159(Level1Val, Level5Val, Level9Val, level);
        }
        public static float Interp159(float Level1Val, float Level5Val, float Level9Val, float level)
        {
            return Level1Val * (level - 5f) * (level - 9f) / ((1f - 5f) * (1f - 9f)) +
                   Level5Val * (level - 1f) * (level - 9f) / ((5f - 1f) * (5f - 9f)) +
                   Level9Val * (level - 1f) * (level - 5f) / ((9f - 1f) * (9f - 5f));
        }
    }
}