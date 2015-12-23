using CoreEngine;

namespace CloudberryKingdom
{
    public class CoreFont
    {
        public HackSpriteFont HFont, HOutlineFont;

        public float CharacterSpacing;
        public int LineSpacing;

        public CoreFont(string FontName, float CharacterSpacing, int LineSpacing)
        {
            Initialize(FontName, "", CharacterSpacing, LineSpacing);
        }

        public CoreFont(string FontName, string OutlineFontName, float CharacterSpacing, int LineSpacing)
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