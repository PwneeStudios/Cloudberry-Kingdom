using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    public class EzFont
    {
        public SpriteFont Font, OutlineFont;
        public float CharacterSpacing;
        public int LineSpacing;

        public EzFont(string FontName)
        {
            Font = Tools.GameClass.Content.Load<SpriteFont>(FontName);
            CharacterSpacing = Font.Spacing;
            
            //LineSpacing = Font.LineSpacing;
            LineSpacing = (int)Font.MeasureString("abc").Y;
        }

        public EzFont(string FontName, float CharacterSpacing, int LineSpacing)
        {
            Initialize(FontName, "", CharacterSpacing, LineSpacing);
        }

        public EzFont(string FontName, string OutlineFontName, float CharacterSpacing, int LineSpacing)
        {
            Initialize(FontName, OutlineFontName, CharacterSpacing, LineSpacing);
        }

        public EzFont(string FontName, string OutlineFontName, float CharacterSpacing, float LineSpacingMod)
        {
            Initialize(FontName, OutlineFontName, CharacterSpacing, LineSpacing);
            LineSpacing = (int)(LineSpacing * LineSpacingMod);
            FixFont();
        }

        public void FixFont()
        {
            Font.Spacing = CharacterSpacing;
            Font.LineSpacing = LineSpacing;
        }

        void Initialize(string FontName, string OutlineFontName, float CharacterSpacing, int LineSpacing)
        {
            this.CharacterSpacing = CharacterSpacing;
            this.LineSpacing = LineSpacing;

            Font = Tools.GameClass.Content.Load<SpriteFont>(FontName);
            FixFont();

            if (OutlineFontName.Length > 1)
            {
                OutlineFont = Tools.GameClass.Content.Load<SpriteFont>(OutlineFontName);
                OutlineFont.Spacing = CharacterSpacing;
                OutlineFont.LineSpacing = LineSpacing;
            }
            else
                OutlineFont = null;

            this.LineSpacing = (int)Font.MeasureString("abc").Y;
        }
    }
}