using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Drawing;

namespace CloudberryKingdom
{
    public class ButtonTexture
    {
#if PC_VERSION
        //public static EzTexture Go { get { return Tools.TextureWad.FindByName("Z_Key"); } }
        public static EzTexture Go { get { return Tools.TextureWad.FindByName("EnterKey"); } }
#else
        public static EzTexture Go { get { return Tools.TextureWad.FindByName("Controller_A_BigThin"); } }
#endif
    }

    public class ButtonString
    {
#if PC_VERSION
        public static Dictionary<Keys, string> KeyToString;
        public static void Init()
        {
            KeyToString = new Dictionary<Keys, string>();
            KeyToString.Add(Keys.A, "A");
            KeyToString.Add(Keys.B, "B");
            KeyToString.Add(Keys.C, "C");
            KeyToString.Add(Keys.D, "D");
            KeyToString.Add(Keys.E, "E");
            KeyToString.Add(Keys.F, "F");
            KeyToString.Add(Keys.G, "G");
            KeyToString.Add(Keys.H, "H");
            KeyToString.Add(Keys.I, "I");
            KeyToString.Add(Keys.J, "J");
            KeyToString.Add(Keys.K, "K");
            KeyToString.Add(Keys.L, "L");
            KeyToString.Add(Keys.M, "M");
            KeyToString.Add(Keys.N, "N");
            KeyToString.Add(Keys.O, "O");
            KeyToString.Add(Keys.P, "P");
            KeyToString.Add(Keys.Q, "Q");
            KeyToString.Add(Keys.R, "R");
            KeyToString.Add(Keys.S, "S");
            KeyToString.Add(Keys.T, "T");
            KeyToString.Add(Keys.U, "U");
            KeyToString.Add(Keys.V, "V");
            KeyToString.Add(Keys.W, "W");
            KeyToString.Add(Keys.X, "X");
            KeyToString.Add(Keys.Y, "Y");
            KeyToString.Add(Keys.Z, "Z");
        }

        public static void SetKeyFromString(ref Keys key, string str)
        {
            foreach (var pair in KeyToString)
            {
                if (string.Compare(str, pair.Value, true) == 0)
                {
                    key = pair.Key;
                    return;
                }
            }
        }

        public static string KeyToTexture(Keys key)
        {
            string str;
            try
            {
                str = KeyToString[key] + "_Key";
            }
            catch
            {
                str = "White";
            }

            return str;
        }

        // Secondary
        public static string Back(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.Back_Secondary)); }
        public static string Go(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.Go_Secondary)); }
        public static string X(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.Toggle_Secondary)); }
        public static string Y(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.Help_Secondary)); }
        public static string LeftRight(int size) { return string.Format("{{pLeftRight_Key,{0},?}}{{s15,0}}", size); }
        public static string LeftBumper(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.ReplayPrev_Secondary)); }
        public static string RightBumper(int size) { return string.Format("{{p{1},{0},?}}{{s15,0}}", size, KeyToTexture(ButtonCheck.ReplayNext_Secondary)); }
        public static string Up(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }
        public static string Jump(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }

        // Regular keys
        public static string KeyStr(Keys key, int size)
        {
            float BackScale = 2.0083f;

            switch (key)
            {
                case Keys.Space:
                    return string.Format("{{pSpaceKey,{0},?}}{{s15,0}}", size * BackScale);
                case Keys.Enter:
                    return string.Format("{{pEnter_Key,{0},?}}{{s15,0}}", size * BackScale);
                default:
                    return string.Format("{{p{0},{1},?}}{{s15,0}}", ButtonString.KeyToTexture(key), size);
            }
        }

        // Sane
        //public static float BackScale = 1.3f;
        //public static string Back(int size) { return string.Format("{{pBackspaceKey,{0},?}}{{s15,0}}", size * BackScale); }
        //public static string Go(int size) { return string.Format("{{pEnterKey,{0},?}}{{s15,0}}", size * BackScale); }
        //public static string X(int size) { return string.Format("{{pC_Key,{0},?}}{{s15,0}}", size); }
        //public static string Y(int size) { return string.Format("{{pV_Key,{0},?}}{{s15,0}}", size); }
        //public static string LeftRight(int size) { return string.Format("{{pLeftRight_Key,{0},?}}{{s15,0}}", size); }
        //public static string LeftBumper(int size) { return string.Format("{{pSpaceKey,{0},?}}{{s15,0}}", size * BackScale); }
        //public static string RightBumper(int size) { return string.Format("{{pD_Key,{0},?}}{{s15,0}}", size); }
        //public static string Up(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }
        //public static string Jump(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }

        // Old school
        //public static string Back(int size) { return string.Format("{{pX_Key,{0},?}}{{s15,0}}", size); }
        //public static string Go(int size) { return string.Format("{{pZ_Key,{0},?}}{{s15,0}}", size); }
        //public static string X(int size) { return string.Format("{{pC_Key,{0},?}}{{s15,0}}", size); }
        //public static string Y(int size) { return string.Format("{{pV_Key,{0},?}}{{s15,0}}", size); }
        //public static string LeftRight(int size) { return string.Format("{{pLeftRight_Key,{0},?}}{{s15,0}}", size); }
        //public static string LeftBumper(int size) { return string.Format("{{pA_Key,{0},?}}{{s15,0}}", size); }
        //public static string RightBumper(int size) { return string.Format("{{pD_Key,{0},?}}{{s15,0}}", size); }
        //public static string Up(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }
        //public static string Jump(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }
#else
        public static void Init() { }
        public static string Back(int size) { return string.Format("{{pController_B_BigThin,{0},?}}", size); }
        public static string Go(int size) { return string.Format("{{pController_A_BigThin,{0},?}}", size); }
        public static string X(int size) { return string.Format("{{pController_X_Big,{0},?}}", size); }
        public static string Y(int size) { return string.Format("{{pController_Y_Big,{0},?}}", size); }
        public static string LeftRight(int size) { return string.Format("{{pLeft_Thumbstick,{0},?}}", size); }
        public static string LeftBumper(int size) { return string.Format("{{pLeft_Bumper,{0},?}}", size); }
        public static string RightBumper(int size) { return string.Format("{{pRight_Bumper,{0},?}}", size); }
        public static string Jump(int size) { return string.Format("{{pController_A_BigThin,{0},?}}", size); }
#endif
    }

    public class EzText : IViewable
    {
        public static string ColorToMarkup(int r, int g, int b)
        {
            return ColorToMarkup(r, g, b, 0, null);
        }
        public static string ColorToMarkup(int r, int g, int b, int shift)
        {
            return ColorToMarkup(r, g, b, shift, null);
        }
        public static string ColorToMarkup(int r, int g, int b, int shift, string bit)
        {
            return ColorToMarkup(new Color(r, g, b), shift, bit);
        }
        public static string ColorToMarkup(Color clr)
        {
            return ColorToMarkup(clr, 0, null);
        }
        public static string ColorToMarkup(Color clr, int shift)
        {
            return ColorToMarkup(clr, shift, null);
        }
        public static string ColorToMarkup(Color clr, int shift, string bit)
        {
            if (bit != null)
            {
                return ColorToMarkup(clr, shift) + bit + ColorToMarkup(Color.White, shift);
            }

            string str = string.Format("{{c{0},{1},{2},{3}}}", clr.R, clr.G, clr.B, clr.A);
            if (shift != 0)
            {
                string shiftstr = string.Format("{{s{0},0}}", shift);
                str += shiftstr;
            }

            return str;
        }

        public static EzText FindText(List<EzText> list, string Name)
        {
            return list.Find(text => string.Compare(text.Name, Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Name of the text, used in DrawPiles
        /// </summary>
        public string Name = "";


        public string[] GetViewables()
        {
            return new string[] { };
        }

        /// <summary>
        /// Layer of the text, used in DrawPiles
        /// </summary>
        public int Layer = 0;

        public bool HitTest(Vector2 pos) { return HitTest(pos, Vector2.Zero); }
        public bool HitTest(Vector2 pos, Vector2 padding)
        {
            CalcBounds();
            if (pos.X > TR.X + padding.X) return false;
            if (pos.X < BL.X - padding.X) return false;
            if (pos.Y > TR.Y + padding.Y) return false;
            if (pos.Y < BL.Y - padding.Y) return false;

            return true;
        }

        class EzTextBit
        {
            public int LineNumber;
            public String str;
            public StringBuilder builder_str; 
            public Vector2 loc, size;
            public Color clr;
        }
        class EzTextPic
        {
            public int LineNumber;
            public EzTexture tex;
            public Rectangle rect;
            public Vector2 size;
            public bool AsPaint;
        }
        List<EzTextBit> Bits;
        List<EzTextPic> Pics;

        /// <summary>
        /// Replaces the first bit of text, with no reformatting
        /// </summary>
        public void SubstituteText(string text)
        {
            Bits[0].str = text;
            Bits[0].builder_str = null;
            TextWidth -= Bits[0].size.X;
            Bits[0].size = MyFont.Font.MeasureString(text);
            TextWidth += Bits[0].size.X;
        }
        public void SubstituteText(StringBuilder text)
        {
            Bits[0].builder_str = text;
            TextWidth -= Bits[0].size.X;
            Bits[0].size = MyFont.Font.MeasureString(text);
            TextWidth += Bits[0].size.X;
        }

        /// <summary>
        /// Appends the given text to the first bit of text, with no reformatting
        /// </summary>
        public void AppendText(string text)
        {
            SubstituteText(Bits[0].str + text);
        }

        /// <summary>
        /// Appends the given character to the first bit of text, with no reformatting
        /// </summary>
        public void AppendText(char character)
        {
            SubstituteText(Bits[0].str + character);
        }

        /// <summary>
        /// Returns the string from the first bit of text
        /// </summary>
        /// <returns></returns>
        public string FirstString()
        {
            return Bits[0].str;
        }

        public PieceQuad Backdrop;

        public bool FixedToCamera;

        public bool ColorizePics;
        Color PicColor;

        public EzFont MyFont;
        Color MyColor;
        public Vector4 MyFloatColor;

        public Vector2 TR, BL;

        public void MakeFancyPos()
        {
            if (FancyPos != null) FancyPos.Release();
            FancyPos = new FancyVector2();
            FancyPos.RelVal = _Pos; ;
        }

        /// <summary>
        /// If this quad is an element in a bigger structure (such as a DrawPile),
        /// this vector represents the scaling of the parent structure.
        /// </summary>
        public Vector2 ParentScaling = Vector2.One;
        public float ParentAlpha = 1f;

        public Vector2 _Pos;
        public Vector2 Pos
        {
            get
            {
                if (FancyPos == null)
                    return _Pos;
                else
                    return FancyPos.RelVal;
            }
            set
            {
                if (FancyPos == null)
                    _Pos = value;
                else
                    FancyPos.RelVal = value;
            }
        }
        public float X
        {
            get
            {
                if (FancyPos == null)
                    return _Pos.X;
                else
                    return FancyPos.RelVal.X;
            }
            set
            {
                if (FancyPos == null)
                    _Pos.X = value;
                else
                    FancyPos.RelValX = value;
            }
        }
        public float Y
        {
            get
            {
                if (FancyPos == null)
                    return _Pos.Y;
                else
                    return FancyPos.RelVal.Y;
            }
            set
            {
                if (FancyPos == null)
                    _Pos.Y = value;
                else
                    FancyPos.RelValY = value;
            }
        }

        public FancyVector2 FancyPos;
        public float TextBoxWidth;
        public float Height, TextWidth;

        public String MyString;

        public int Code;

        public Vector4 OutlineColor = Color.Black.ToVector4();

        public bool Shadow = false;
        public bool PicShadow = true;
        public Vector2 ShadowOffset = Vector2.Zero;
        public Color ShadowColor = Color.Black;
        public float ShadowScale = 1f;

        public float Alpha = 1, AlphaVel = 0;

        public float _Scale = 1;
        public float Scale
        {
            set { _Scale = value; } //if (Centered) Center(); }
            get { return _Scale; }
        }

        public void Release()
        {
            if (FancyPos != null) FancyPos.Release(); FancyPos = null;
        }

        public EzText Clone()
        {
            EzText clone = new EzText(MyString, MyFont,TextBoxWidth, Centered, YCentered, LineHeightMod);
            clone.MyFloatColor = MyFloatColor;
            clone.OutlineColor = OutlineColor;
            return clone;
        }

        public enum Style { Normal, FadingOff };
        public bool RightJustify = false;
        public bool Centered, YCentered;
        public EzText() { }
        public EzText(String str, EzFont font) { MyFont = font; Init(str); }
        public EzText(String str, EzFont font, Style style)
        {
            MyFont = font;
            if (style == Style.FadingOff)
            {
                BitByBit = true;
                Alpha = 0;
                AlphaVel = .025f * 1.3f * 2;
            }
            Init(str, 10000, false, false, .65f);
        }
        public EzText(String str, EzFont font, bool Centered) { MyFont = font; Init(str, 10000, Centered, false, 1); }
        public EzText(String str, EzFont font, float Width, bool Centered) { MyFont = font; Init(str, Width, Centered, false, 1); }
        public EzText(String str, EzFont font, bool Centered, bool YCentered) { MyFont = font; Init(str, 10000, Centered, YCentered, 1); }
        public EzText(String str, EzFont font, float Width, bool Centered, bool YCentered) { MyFont = font; Init(str, Width, Centered, YCentered, 1); }
        public EzText(String str, EzFont font, float Width, bool Centered, bool YCentered, float LineHeightMod) { MyFont = font; Init(str, Width, Centered, YCentered, LineHeightMod); }

        public Vector2 BackdropShift = Vector2.Zero;
        public void AddBackdrop() { AddBackdrop(new Vector2(.65f, .65f), Vector2.Zero, Vector2.Zero, new Vector2(10000,10000)); }
        public void AddBackdrop(Vector2 mult_padding) { AddBackdrop(mult_padding, Vector2.Zero, Vector2.Zero, new Vector2(10000, 10000)); }
        public void AddBackdrop(Vector2 mult_padding, Vector2 add_padding) { AddBackdrop(mult_padding, add_padding, Vector2.Zero, new Vector2(10000, 10000)); }
        public void AddBackdrop(Vector2 mult_padding, Vector2 add_padding, Vector2 min_dim, Vector2 max_dim)
        {
            if (Backdrop == null)
                Backdrop = new PieceQuad();
            Backdrop.Clone(PieceQuad.Get("DullMenu"));

            Vector2 dim;
            dim = mult_padding * GetWorldSize() + add_padding;

            Backdrop.CalcQuads(Vector2.Min(Vector2.Max(dim, min_dim), max_dim));
        }

        /// <summary>
        /// The backdrop alpha is multiplied by this before being drawn.
        /// </summary>
        public float BackdropModAlpha = 1;

        public void DrawBackdrop()
        {
            if (Backdrop == null) return;

            if (FancyPos == null)
            {
                Backdrop.Base.Origin = _Pos;
                if (FixedToCamera)
                    Backdrop.Base.Origin += Tools.CurLevel.MainCamera.Data.Position;
            }
            else
                Backdrop.Base.Origin = FancyPos.Update();

            Backdrop.Base.Origin += BackdropShift;

            Backdrop.SetAlpha(Alpha * BackdropModAlpha);
            Backdrop.Draw();
        }

        Vector2 loc;
        float LineHeight;
        void CheckForLineEnd(Vector2 TextSize)
        {            
            LineHeight = Math.Max(LineHeight, TextSize.Y);
            if (TextSize.X + loc.X > TextBoxWidth && loc.X != 0)
            {
                loc.X = 0;
                loc.Y += LineHeight;
                LineHeight = 0;
            }
        }

        /// <summary>
        /// Returns the text code to generate the given color, of the form "{c r,g,b,a}"
        /// </summary>
        public static string ColorToCode(Color clr)
        {
            return string.Format("{{c{0}, {1}, {2}, {3}}}", clr.R, clr.G, clr.B, clr.A);
        }

        String Parse_PicName;
        Vector2 Parse_PicSize;
        Vector2 Parse_PicShift;
        bool AsPaint;
        Color Parse_Color;
        enum ParseData { Pic, Color };
        ParseData Parse_Type;
        void Parse(String str)
        {
            char c = str[1];

            int Comma1, Comma2, Comma3, Comma4;
            String WidthString, HeightString;
            switch (c)
            {
                case 'p':
                    Parse_Type = ParseData.Pic;

                    string[] string_bits = str.Split(',');

                    Comma1 = str.IndexOf(",");
                    Comma2 = str.IndexOf(",", Comma1 + 1);

                    Parse_PicName = str.Substring(2, Comma1 - 2);
                    //WidthString = str.Substring(Comma1 + 1, Comma2 - 1 - Comma1);
                    //HeightString = str.Substring(Comma2 + 1);
                    //Parse_PicName = string_bits[0];
                    WidthString = string_bits[1];
                    HeightString = string_bits[2];

                    AsPaint = false;
                    if (string_bits.Length > 3)
                    {
                        Parse_PicShift = new Vector2(float.Parse(string_bits[3]),
                                                     float.Parse(string_bits[4])) * Tools.TheGame.Resolution.LineHeightMod;
                        if (string_bits.Length > 5)
                            AsPaint = int.Parse(string_bits[5]) > 0;
                    }
                    else
                        Parse_PicShift = Vector2.Zero;

                    Vector2 size;
                    EzTexture texture = Tools.TextureWad.FindByName(Parse_PicName);
                    float ratio = (float)texture.Tex.Width / (float)texture.Tex.Height;

                    // 's' scale the texture
                    if (WidthString.Contains('s'))
                    {
                        size = new Vector2(texture.Tex.Width, texture.Tex.Height);
                        size *= float.Parse(HeightString);
                    }
                    // '?' calculates that number from the texture height/width ratio
                    else if (WidthString.Contains('?'))
                    {
                        size = new Vector2(0, float.Parse(HeightString));
                        size.X = size.Y * ratio;
                    }
                    else if (HeightString.Contains('?'))
                    {
                        size = new Vector2(float.Parse(WidthString), 0);
                        size.Y = size.X / ratio;
                    }
                    else
                        size = new Vector2(float.Parse(WidthString), float.Parse(HeightString));
                    Parse_PicSize = size * Tools.TheGame.Resolution.LineHeightMod;
                    break;

                // Blank space
                case 's':
                    Parse_Type = ParseData.Pic;
                    Parse_PicName = "Transparent";

                    Comma1 = str.IndexOf(",");

                    WidthString = str.Substring(2, Comma1 - 2);
                    HeightString = str.Substring(Comma1 + 1);
                    Parse_PicSize = new Vector2(float.Parse(WidthString), float.Parse(HeightString));
                    Parse_PicSize *= Tools.TheGame.Resolution.LineHeightMod;
                    break;

                case 'c':
                    Parse_Type = ParseData.Color;

                    Comma1 = str.IndexOf(",");
                    Comma2 = str.IndexOf(",", Comma1 + 1);
                    Comma3 = str.IndexOf(",", Comma2 + 1);

                    String RString = str.Substring(2, Comma1 - 2);
                    String GString = str.Substring(Comma1 + 1, Comma2 - 1 - Comma1);
                    String BString = str.Substring(Comma2 + 1, Comma3 - 1 - Comma2);
                    String AString = str.Substring(Comma3 + 1);

                    Parse_Color = new Color(byte.Parse(RString), byte.Parse(GString), byte.Parse(BString), byte.Parse(AString));
                    break;
            }
        }

        int GetLineEnd(String str)
        {
            int EndIndex = 0;
            int NewEndIndex;
            int BracketIndex, SpaceIndex, DelimiterIndex;

            bool ReachedEnd = false;
            while (!ReachedEnd)
            {
                BracketIndex = str.IndexOf("}", EndIndex);
                SpaceIndex = str.IndexOf(" ", EndIndex);
                DelimiterIndex = str.IndexOf('\n', EndIndex);
                if (BracketIndex == -1 && SpaceIndex == -1) { NewEndIndex = str.Length; ReachedEnd = true; }
                else if (BracketIndex == -1) NewEndIndex = SpaceIndex + 1;
                else if (SpaceIndex == -1) NewEndIndex = BracketIndex + 1;
                //else NewEndIndex = Math.Min(BracketIndex, SpaceIndex) + 1;
                else NewEndIndex = BracketIndex + 1;

                if (DelimiterIndex >= 0 && DelimiterIndex < NewEndIndex) { NewEndIndex = DelimiterIndex; ReachedEnd = true; }

                float width = StringSize(str.Substring(0, NewEndIndex)).X;
                if (width > TextBoxWidth && EndIndex > 0) return EndIndex;
                else EndIndex = NewEndIndex;
            }

            return EndIndex;
        }

        Vector2 StringSize(String str)
        {
            MyFont.FixFont();

            Vector2 Size = Vector2.Zero;
            int BeginBracketIndex, EndBracketIndex;
            bool flag = false;

            while (!flag)
            {
                BeginBracketIndex = str.IndexOf("{", 0);
                if (BeginBracketIndex >= 0)
                {
                    EndBracketIndex = str.IndexOf("}", 0);
                    String PicStr = str.Substring(BeginBracketIndex, EndBracketIndex - BeginBracketIndex);
                    Parse(PicStr);
                    str = str.Remove(BeginBracketIndex, EndBracketIndex - BeginBracketIndex + 1);

                    if (Parse_Type == ParseData.Pic)
                    {
                        Size.X += Parse_PicSize.X;
                        Size.Y = Math.Max(Size.Y, Parse_PicSize.Y);
                    }
                }
                else
                    flag = true;
            }

            Vector2 TextSize = MyFont.Font.MeasureString(str);
            //TextSize.Y = MyFont.LineSpacing;
            TextSize.Y *= Tools.TheGame.Resolution.LineHeightMod;

            Size.X += TextSize.X;
            Size.Y = Math.Max(Size.Y, TextSize.Y);

            return Size;
        }

        bool BitByBit = false;
        Color CurColor = Color.White;
        float AddLine(String str, float StartX, float StartY, int LineNumber)
        {
            MyFont.FixFont();

            Vector2 loc = new Vector2(StartX, 0);
            float LineHeight = MyFont.Font.MeasureString(" ").Y;
            int BeginBracketIndex, EndBracketIndex;

            //Color CurColor = Color.White;

            bool FirstElement = true;

            //BitByBit = true;
            while (str.Length > 0)
            {
                BeginBracketIndex = str.IndexOf("{", 0);
                if (BeginBracketIndex == 0)
                {
                    EndBracketIndex = str.IndexOf("}", 0);
                    String PicStr = str.Substring(BeginBracketIndex, EndBracketIndex - BeginBracketIndex);
                    Parse(PicStr);
                    str = str.Remove(BeginBracketIndex, EndBracketIndex - BeginBracketIndex + 1);

                    // Parse picture info
                    if (Parse_Type == ParseData.Pic)
                    {
                        EzTextPic pic = new EzTextPic();
                        pic.LineNumber = LineNumber;

                        if (!FirstElement) Parse_PicShift.X -= .25f * Parse_PicSize.X;

                        pic.tex = Tools.TextureWad.FindByName(Parse_PicName);
                        //float y = Tools.TheGame.Resolution.LineHeightMod * MyFont.LineSpacing / 2 - Parse_PicSize.Y / 2 + StartY;
                        //float y = Tools.TheGame.Resolution.LineHeightMod * MyFont.LineSpacing / 1 + StartY;

                        float y = .5f * Tools.TheGame.Resolution.LineHeightMod * MyFont.LineSpacing - .5f * Parse_PicSize.Y + StartY;
                        //float y = .5f * Tools.TheGame.Resolution.LineHeightMod * MyFont.Font.MeasureString("abc").Y - .5f * Parse_PicSize.Y + StartY;
                        pic.rect = new Rectangle((int)(loc.X + Parse_PicShift.X), (int)(y + loc.Y + Parse_PicShift.Y), (int)Parse_PicSize.X, (int)Parse_PicSize.Y);
                        pic.size = Parse_PicSize;
                        pic.AsPaint = AsPaint;

                        Pics.Add(pic);

                        loc.X += Parse_PicSize.X / 2;
                        LineHeight = Math.Max(LineHeight, Parse_PicSize.Y * .9f);
                    }

                    // Parse color info
                    if (Parse_Type == ParseData.Color)
                    {
                        CurColor = Parse_Color;
                        if (BitByBit) loc.X += MyFont.Font.MeasureString(" ").X;// -MyFont.Font.Spacing;
                        loc.X += .35f * MyFont.Font.Spacing;
                    }
                }
                // Parse text info
                else
                {
                    int i;
                    if (BeginBracketIndex < 0) i = str.Length; else i = BeginBracketIndex;

                    if (BitByBit) i = 1;

                    EzTextBit bit = new EzTextBit();
                    bit.LineNumber = LineNumber;
                    bit.clr = CurColor;
                    bit.str = str.Substring(0, i);
                    str = str.Remove(0, i);

                    bit.size = MyFont.Font.MeasureString(bit.str);
                    bit.size.Y *= Tools.TheGame.Resolution.LineHeightMod;
                    //if (BitByBit && (bit.str.Length == 0 || bit.str[0] != ' ')) bit.size.X += MyFont.Font.Spacing;
                    if (BitByBit) bit.size.X += MyFont.Font.Spacing;

                    bit.loc = loc + new Vector2(0, StartY);
                    Bits.Add(bit);

                    loc.X += bit.size.X;
                    LineHeight = Math.Max(LineHeight, bit.size.Y * .9f);
                }

                FirstElement = false;
            }

            return LineHeight;
        }

        /// <summary>
        /// Gets the real world size of the text, accounting for scaling
        /// </summary>
        public Vector2 GetWorldSize()
        {
            return new Vector2(GetWorldWidth(), GetWorldHeight());
        }

        /// <summary>
        /// Get the real world height of the text, accounting for scaling
        /// </summary>
        public float GetWorldHeight()
        {
            return Scale * GetWorldFloat(Height);
        }

        /// <summary>
        /// Get the real world width of the text, accounting for scaling
        /// </summary>
        public float GetWorldWidth()
        {
            return Scale * GetWorldFloat(TextWidth);
        }

        public float GetWorldWidth(string str)
        {
            return Scale * GetWorldFloat(MyFont.Font.MeasureString(str).X);
        }

        Vector2 _MyCameraZoom = new Vector2(.001f, .001f);
        /// <summary>
        /// The value of the camera zoom the last time this EzText was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        Vector2 GetWorldVector(Vector2 v) { return GetWorldVector(v, MyCameraZoom); }
        float GetWorldFloat(float interval) { return GetWorldFloat(interval, MyCameraZoom); }


        /// <summary>
        /// Converts a length from screen units to world units
        /// </summary>
        public static float GetWorldFloat(float interval, Vector2 zoom)
        {
            return GetWorldVector(new Vector2(interval, 0), zoom).X;
        }

        /// <summary>
        /// Converts a length from screen units to world units
        /// </summary>
        public static Vector2 GetWorldVector(Vector2 v, Vector2 zoom)
        {
            Vector2 vec2 = v;
            Vector2 vec1 = new Vector2(0, 0);
            vec1 = Tools.ToWorldCoordinates(vec1, Tools.CurLevel.MainCamera, zoom);
            vec2 = Tools.ToWorldCoordinates(vec2, Tools.CurLevel.MainCamera, zoom);

            return (vec2 - vec1) * Tools.TheGame.Resolution.LineHeightMod;
        }

        /// <summary>
        /// Converts a length from screen units to GUI units
        /// </summary>
        public static Vector2 GetGUIVector(Vector2 v)
        {
            Vector2 vec2 = v;
            Vector2 vec1 = new Vector2(0, 0);
            vec1 = Tools.ToGUICoordinates(vec1, Tools.CurLevel.MainCamera);
            vec2 = Tools.ToGUICoordinates(vec2, Tools.CurLevel.MainCamera);

            return (vec2 - vec1) * Tools.TheGame.Resolution.LineHeightMod;
        }

        float LineHeightMod = 1f;
        Vector2[] LineSizes = new Vector2[20];
        void Init(String str) { Init(str, 10000, false, false, 1f); }
        void Init(String str, float Width, bool Centered, bool YCentered, float LineHeightMod)
        {
            this.LineHeightMod = LineHeightMod;
            this.Centered = Centered;
            this.YCentered = YCentered;

            TextBoxWidth = Width;

            MyString = str;

            MyColor = Color.White;
            MyFloatColor = new Vector4(1, 1, 1, 1);

            Bits = new List<EzTextBit>();
            Pics = new List<EzTextPic>();

            loc = Vector2.Zero;
            LineHeight = 0;

            int i = 0;

            TextWidth = 0;
            float y = 0;
            int LineNumber = 0;
            CurColor = Color.White;
            while (str.Length > 0)
            {
                i = GetLineEnd(str);
                String Line = str.Substring(0, i);
                if (Line.Length > 0 && Line[Line.Length-1] == ' ') Line = Line.Remove(Line.Length-1, 1);
                Vector2 Size = StringSize(Line);
                LineSizes[LineNumber] = Size;
                
                float x = -Size.X / 2;
                TextWidth = Math.Max(TextWidth, Size.X);
                y += LineHeightMod * AddLine(Line, x, y, LineNumber);
                //AddLine(Line, x, y, LineNumber);
                //y += MyFont.LineSpacing;
                str = str.Remove(0, i);
                if (str.Length > 0 && str[0] == ' ') str = str.Remove(0, 1);
                if (str.Length > 0 && str[0] == '\n') str = str.Remove(0, 1);

                LineNumber++;
            }

            Height = y;

            
            if (YCentered)
            {
                foreach (EzTextBit bit in Bits) bit.loc.Y -= Height / 2;
                foreach (EzTextPic pic in Pics) pic.rect.Y -= (int)(Height / 2);
            }

            if (!Centered)
            {
                foreach (EzTextBit bit in Bits) bit.loc.X += LineSizes[bit.LineNumber].X / 2 * Tools.TheGame.Resolution.LineHeightMod;
                foreach (EzTextPic pic in Pics) pic.rect.X += (int)(LineSizes[pic.LineNumber].X / 2);// * Tools.TheGame.Resolution.LineHeightMod);
            }
            else
            {
                foreach (EzTextPic pic in Pics) pic.rect.X = (int)((Scale * pic.rect.X - Pos.X + pic.rect.Width / 2f) * Tools.TheGame.Resolution.LineHeightMod + Pos.X - pic.rect.Width / 2f);
            }
        }

        //public void RightJustify()
        //{
        //    Pos = new Vector2(Pos.X - GetWorldWidth(), Pos.Y
        //}
        public void Center()
        {
            Pos = new Vector2(Pos.X - .5f * GetWorldWidth(), Pos.Y);
        }

        Vector2 JustificationShift;

        public bool Show = true;
        public void Draw(Camera cam) { Draw(cam, true); }
        public void Draw(Camera cam, bool EndBatch)
        {
            Alpha += AlphaVel;

            MyCameraZoom = cam.Zoom;
            //MyCameraZoom = cam.Zoom * Tools.EffectWad.ModZoom;
            //MyCameraZoom = new Vector2(-cam.Zoom.X, cam.Zoom.Y);

            if (!Show) return;

            // Scale if the parent this text is attached to is scaled
            float HoldScale = Scale;
            float HoldAlpha = Alpha;
            if (ParentScaling != Vector2.One)
            {
                Scale *= ParentScaling.X;
            }
            if (ParentAlpha != 1)
                Alpha *= ParentAlpha;

            if (RightJustify)
                JustificationShift = new Vector2(-GetWorldWidth(), 0);

            if (FancyPos != null)
                _Pos = FancyPos.Update(ParentScaling);

            PicColor = Color.Black;//MyColor;
            if (Shadow)
            {
                // Note: never end the SpriteBatch for drawing a shadow,
                // since we will always be drawing the non-shadow part afterward

                float _HoldScale = Scale;
                Scale *= ShadowScale;

                _Pos -= ShadowOffset;
                if (MyFont.OutlineFont != null || OutlineColor.W == 0)
                    _Draw(cam, false, PicShadow, MyFont.OutlineFont, ShadowColor.ToVector4());
                _Draw(cam, false, false, MyFont.Font, ShadowColor.ToVector4());
                _Pos += ShadowOffset;

                Scale = _HoldScale;
            }
            PicColor = Color.White;

            //PicColor = new Color(MyFloatColor);
            if (!ColorizePics)
            {
                PicColor = Color.White;
                //PicColor.A = MyColor.A;
            }

            if (MyFont.OutlineFont != null || OutlineColor.W == 0)
                _Draw(cam, false, true, MyFont.OutlineFont, OutlineColor); 
            _Draw(cam, EndBatch, true, MyFont.Font, MyFloatColor);

            // Draw box outline
            if (Tools.DrawBoxes)
            {
                CalcBounds();
                Tools.QDrawer.DrawBox(BL, TR, Color.Black, 5);
            }

            // Revert the scaling if this text is attached to something
            if (ParentScaling != Vector2.One)
            {
                Scale = HoldScale;
            }
            if (ParentAlpha != 1)
                Alpha = HoldAlpha;
        }

        public void KillBitByBit() { KillBitByBit(float.NaN); }
        public void KillBitByBit(float SetAlpha)
        {
            if (SetAlpha != float.NaN)
                Alpha = SetAlpha;

            BitByBit = false;
            foreach (EzTextBit bit in Bits)
                bit.clr.A = (byte)255;
        }

        public float Angle = 0;

        public static bool ZoomWithCamera_Override = false;
        public bool ZoomWithCam = false;
        public void _Draw(Camera cam, bool EndBatch, bool DrawPics, SpriteFont font, Vector4 color)
        {
            if (MyFloatColor.W <= 0) return;

            MyFont.FixFont();

            MyColor.R = Tools.FloatToByte(color.X);
            MyColor.G = Tools.FloatToByte(color.Y);
            MyColor.B = Tools.FloatToByte(color.Z);
            MyColor.A = Tools.FloatToByte(color.W * Alpha);

            float ZoomMod = 1;
            if (ZoomWithCam || ZoomWithCamera_Override)
                ZoomMod = MyCameraZoom.X / .001f;

            Vector2 Position = _Pos + JustificationShift;
            if (FixedToCamera) Position += cam.Data.Position;
            Vector2 Loc = Tools.ToScreenCoordinates(Position, cam, Tools.EffectWad.ModZoom);
            //Vector2 Loc2 = Tools.ToScreenCoordinates(Position + new Vector2(.5f, .5f), cam);

            //Tools.QDrawer.DrawLine(Position, Position + new Vector2(1000, 0), Color.YellowGreen, 10);

            float BitAlphaMod = 0;
            Tools.StartSpriteBatch();
            foreach (EzTextBit bit in Bits)
            {
                Color textcolor;
                if (BitByBit)
                {
                    var c = MyColor.ToVector4() * bit.clr.ToVector4();
                    c.W = color.W * Alpha * bit.clr.ToVector4().W + BitAlphaMod;
                    textcolor = Tools.PremultiplyAlpha(new Color(c));
                }
                else
                    textcolor = Tools.PremultiplyAlpha(new Color(MyColor.ToVector4() * bit.clr.ToVector4()));

                if (bit.builder_str != null)
                    Tools.spriteBatch.DrawString(font, bit.builder_str, Scale * bit.loc*ZoomMod + Loc, textcolor,
                        0, bit.size * Tools.TheGame.Resolution.TextOrigin, new Vector2(Tools.TheGame.Resolution.LineHeightMod, Tools.TheGame.Resolution.LineHeightMod) * Scale*ZoomMod, SpriteEffects.None, 1);
                else
                    Tools.spriteBatch.DrawString(font, bit.str, Scale * bit.loc*ZoomMod + Loc, textcolor,
                        Angle, bit.size * Tools.TheGame.Resolution.TextOrigin, new Vector2(Tools.TheGame.Resolution.LineHeightMod, Tools.TheGame.Resolution.LineHeightMod) * Scale*ZoomMod, SpriteEffects.None, 1);

                if (BitByBit)
                {
                    if (bit.str != null && bit.str[0] != ' ')
                        BitAlphaMod -= .165f * 1.35f;
                    else
                        BitAlphaMod -= .165f * .5f;// 2f;
                }
            }
            if (DrawPics)
                foreach (EzTextPic pic in Pics)
                {
                    Color piccolor = PicColor;
                    piccolor.A = Tools.FloatToByte(Alpha * piccolor.A / 255f);

                    piccolor = Tools.PremultiplyAlpha(piccolor);

                    Vector2 pos = Loc + Scale * ZoomMod * new Vector2(pic.rect.X, pic.rect.Y);
                    Vector2 scale = Scale * ZoomMod * new Vector2(pic.rect.Width / (float)pic.tex.Tex.Width, pic.rect.Height / (float)pic.tex.Tex.Height);

                    if (pic.AsPaint)
                    {
                        Tools.EndSpriteBatch();
                        Tools.StartSpriteBatch(true);
                    }
                    Tools.spriteBatch.Draw(pic.tex.Tex, pos, null, piccolor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    if (pic.AsPaint)
                    {
                        Tools.EndSpriteBatch();
                        Tools.StartSpriteBatch();
                    }
                }

            if (EndBatch)
                Tools.EndSpriteBatch();
        }

        public void CalcBounds()
        {
            TR = new Vector2(-100000000, -100000000);
            BL = new Vector2(100000000, 100000000);

            Vector2 Loc = _Pos;

            foreach (EzTextBit bit in Bits)
            {
                Vector2 loc, size;

                {
                    loc = GetWorldVector(bit.loc);
                    size = GetWorldVector(bit.size);
                }
                //else
                //{
                //    loc = GetGUIVector(bit.loc);
                //    size = GetGUIVector(bit.size);
                //}
                Vector2 bit_TR = Scale * loc + Loc; bit_TR.X += 1 * Scale * size.X; bit_TR.Y += .25f * Scale * size.Y;
                Vector2 bit_BL = Scale * loc + Loc; bit_BL.Y += .75f * Scale * size.Y;

                TR = Vector2.Max(TR, bit_TR);
                BL = Vector2.Min(BL, bit_BL);
            }

            foreach (EzTextPic pic in Pics)
            {
                Vector2 loc = GetWorldVector(new Vector2(pic.rect.X, pic.rect.Y));
                Vector2 size = new Vector2(pic.rect.Width, pic.rect.Height);
                size = GetWorldVector(size);

                Vector2 bit_TR = Scale * loc + Loc; bit_TR.X += 1 * Scale * size.X; bit_TR.Y += .25f * Scale * size.Y;
                Vector2 bit_BL = Scale * loc + Loc; bit_BL.Y += .75f * Scale * size.Y;

                TR = Vector2.Max(TR, bit_TR);
                BL = Vector2.Min(BL, bit_BL);
            }
        }
    }
}
