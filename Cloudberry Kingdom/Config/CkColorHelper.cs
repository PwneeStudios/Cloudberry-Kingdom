using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class CkColorHelper
    {
        public static void RegularColor(Text name)
        {
            name.MyFloatColor = new Color(255, 255, 255).ToVector4();
            name.OutlineColor = new Color(0, 0, 0).ToVector4();
        }

        public static void _x_x_MasochisticColor(Text name)
        {
            name.MyFloatColor = new Color(0, 0, 0).ToVector4();
            name.OutlineColor = new Color(0, 255, 255).ToVector4();
        }

        public static void _x_x_HardcoreColor(Text name)
        {
            name.MyFloatColor = new Color(0, 0, 0).ToVector4();
            name.OutlineColor = new Color(255, 10, 10).ToVector4();
        }

        public static void AbusiveColor(Text name)
        {
            name.MyFloatColor = new Color(248, 136, 8).ToVector4();
            name.OutlineColor = new Color(248, 0, 8).ToVector4();
        }

        public static void UnpleasantColor(Text name)
        {
            name.MyFloatColor = new Color(44, 203, 48).ToVector4();
            name.OutlineColor = new Color(0, 71, 0).ToVector4();
        }

        public static void _x_x_EasyColor(Text name)
        {
            name.MyFloatColor = new Color(184, 240, 255).ToVector4();
            name.OutlineColor = new Color(37, 118, 158).ToVector4();
        }

        public static void _x_x_HappyBlueColor(Text name)
        {
            name.MyFloatColor = new Color(26, 188, 241).ToVector4();
            name.OutlineColor = new Color(255, 255, 255).ToVector4();
        }

        public static void _x_x_Red(Text text)
        {
            text.MyFloatColor = new Color(228, 0, 69).ToVector4();
            text.OutlineColor = Color.White.ToVector4();
        }

        public static void GreenItem(MenuItem item)
        {
            item.MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }
    }
}