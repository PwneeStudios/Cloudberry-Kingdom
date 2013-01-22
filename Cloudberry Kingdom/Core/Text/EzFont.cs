using Microsoft.Xna.Framework.Graphics;
using CoreEngine;

namespace CloudberryKingdom
{
    public class EzFont
    {
        public HackSpriteFont HFont, HOutlineFont;

        public float CharacterSpacing;
        public int LineSpacing;

        public EzFont(string FontName, float CharacterSpacing, int LineSpacing)
        {
            Initialize(FontName, "", CharacterSpacing, LineSpacing);
        }

        public EzFont(string FontName, string OutlineFontName, float CharacterSpacing, int LineSpacing)
        {
            Initialize(FontName, OutlineFontName, CharacterSpacing, LineSpacing);
        }

        void Initialize(string FontName, string OutlineFontName, float CharacterSpacing, int LineSpacing)
        {
            this.CharacterSpacing = CharacterSpacing;
            this.LineSpacing = LineSpacing;

            this.LineSpacing = 133;
        }
    }
}