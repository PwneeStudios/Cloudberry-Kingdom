using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public static class ColorHelper
    {
        public static Color GrayColor(float val) { return new Color(Gray(val)); }
        public static Vector4 Gray(float val)
        {
            return new Vector4(val, val, val, 1);
        }

        static Matrix transform_red = new Matrix(0, 1, 0, 0,
                                                 1, 0, 0, 0,
                                                 0, 0, 1, 0,
                                                 0, 0, 0, 1);

        static Matrix transform_green = new Matrix(1, 0, 0, 0,
                                                   0, 1, 0, 0,
                                                   0, 0, 1, 0,
                                                   0, 0, 0, 1);

        static Matrix transform_blue = new Matrix(0, 0, 1, 0,
                                                  1, 0, 0, 0,
                                                  0, 1, 0, 0,
                                                  0, 0, 0, 1);

        public static Matrix LinearColorTransform(float angle)
        {
            float s = ((angle % 360 + 360) % 360) / 120;

            if (s < 1)
            {
                return (1 - s) * transform_green + s * transform_blue;
            }
            else if (s < 2)
            {
                s = s - 1;
                return (1 - s) * transform_blue + s * transform_red;
            }
            else
            {
                s = s - 2;
                return (1 - s) * transform_red + s * transform_green;
            }
        }

        public static Matrix PureColor(Color color)
        {
            return new Matrix(0, 0, 0, color.R,
                              0, 0, 0, color.G,
                              0, 0, 0, color.B,
                              0, 0, 0, 1);
        }

        public static Matrix HsvTransform(float V, float S, float H)
        {
            float a = CoreMath.Radians(H);
            float U = (float)Math.Cos(a), W = (float)Math.Sin(a);
            var hsv = new Matrix(
                .299f * V + .701f * V * S * U + .168f * V * S * W, .587f * V - .587f * V * S * U + .330f * V * S * W, .114f * V - .114f * V * S * U - .497f * V * S * W, 0,
                .299f * V - .299f * V * S * U - .328f * V * S * W, .587f * V + .413f * V * S * U + .035f * V * S * W, .114f * V - .114f * V * S * U + .292f * V * S * W, 0,
                .299f * V - .300f * V * S * U + 1.25f * V * S * W, .587f * V - .588f * V * S * U - 1.05f * V * S * W, .114f * V + .886f * V * S * U - .203f * V * S * W, 0,
                0, 0, 0, 1);
            return hsv;
        }

        /// <summary>
        /// Gets a non-unique number associated with a matrix.
        /// Used to quickly determine if two matrices are probably the same.
        /// </summary>
        public static float MatrixSignature(Matrix m)
        {
            return m.M11 + m.M22 + m.M33 + m.M44;
        }

        /// <summary>
        /// Premultiply a color's alpha against its RGB components.
        /// </summary>
        /// <param name="color">The normal, non-premultiplied color.</param>
        public static Color PremultiplyAlpha(Color color)
        {
            return new Color(color.R, color.G, color.B) * (color.A / 255f);
        }

        /// <summary>
        /// Premultiply a color's alpha against its RGB components.
        /// </summary>
        /// <param name="color">The normal, non-premultiplied color.</param>
        /// <param name="BlendAddRatio">When 0 blending is normal, when 1 blending is additive.</param>
        public static Color PremultiplyAlpha(Color color, float BlendAddRatio)
        {
            Color NewColor = PremultiplyAlpha(color);
            NewColor.A = (byte)(NewColor.A * (1 - BlendAddRatio));

            return NewColor;
        }
    }
}