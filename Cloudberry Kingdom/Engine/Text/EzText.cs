using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CoreEngine;

namespace CloudberryKingdom
{
    public class ButtonTexture
    {
        public static CoreTexture Go { get { return CloudberryKingdomGame.AsianButtonSwitch ? _Back : _Go; } }
        public static CoreTexture Back { get { return CloudberryKingdomGame.AsianButtonSwitch ? _Go : _Back; } }

#if PC
        public static CoreTexture _Go { get { return Tools.Texture( ButtonCheck.ControllerInUse ? "Xbox_A" : "Enter_Key" ); } }
        public static CoreTexture _Back { get { return Tools.Texture( ButtonCheck.ControllerInUse ? "Xbox_B" : "Esc_Key" ); } }
        public static CoreTexture X { get { return ButtonCheck.ControllerInUse ? Tools.Texture( "Xbox_X" ) : ButtonString.KeyToTexture(ButtonCheck.SlowMoToggle_Secondary); } }
        public static CoreTexture Y { get { return ButtonCheck.ControllerInUse ? Tools.Texture( "Xbox_Y" ) : ButtonString.KeyToTexture(ButtonCheck.Help_KeyboardKey.KeyboardKey); } }
        public static CoreTexture LeftRight { get { return Tools.Texture( ButtonCheck.ControllerInUse ? "Xbox_Dir" : "LeftRight_Key"); } }
        public static CoreTexture LeftBumper { get { return ButtonCheck.ControllerInUse ? Tools.Texture("Xbox_LB") : ButtonString.KeyToTexture(ButtonCheck.ReplayPrev_Secondary); } }
        public static CoreTexture RightBumper { get { return ButtonCheck.ControllerInUse ? Tools.Texture("Xbox_RB") : ButtonString.KeyToTexture(ButtonCheck.ReplayNext_Secondary); } }
#elif PS3
        static CoreTexture _Go { get { return Tools.Texture("PS3_X"); } }
        static CoreTexture _Back { get { return Tools.Texture("PS3_Circle"); } }
        public static CoreTexture X { get { return Tools.Texture("PS3_Square"); } }
        public static CoreTexture Y { get { return Tools.Texture("PS3_Triangle"); } }
        public static CoreTexture LeftRight { get { return Tools.Texture("PS3_Dir"); } }
        public static CoreTexture LeftBumper { get { return Tools.Texture("PS3_1"); } }
        public static CoreTexture RightBumper { get { return Tools.Texture("PS3_1"); } }
#elif CAFE
        public static bool UseGamepad = true;
        public static CoreTexture _Go { get { return Tools.Texture( UseGamepad ? "WiiU_B" : "WiiU_2" ); } }
        public static CoreTexture _Back { get { return Tools.Texture( UseGamepad ? "WiiU_A" : "WiiU_1" ); } }
        public static CoreTexture X { get { return Tools.Texture( UseGamepad ? "WiiU_Y" : "WiiU_1" ); } }
        public static CoreTexture Y { get { return Tools.Texture( UseGamepad ? "WiiU_X" : "WiiU_Dash" ); } }
        public static CoreTexture LeftRight { get { return Tools.Texture( UseGamepad ? "WiiU_Dir" : "WiiU_Dir" ); } }
        public static CoreTexture LeftBumper { get { return Tools.Texture( UseGamepad ? "WiiU_R" : "WiiU_R" ); } }
        public static CoreTexture RightBumper { get { return Tools.Texture( UseGamepad ? "WiiU_L" : "WiiU_L" ); } }
#else
        static CoreTexture _Go { get { return Tools.Texture("Xbox_A"); } }
        static CoreTexture _Back { get { return Tools.Texture("Xbox_B"); } }
        public static CoreTexture X { get { return Tools.Texture("Xbox_X"); } }
        public static CoreTexture Y { get { return Tools.Texture("Xbox_Y"); } }
        public static CoreTexture LeftRight { get { return Tools.Texture("Xbox_Dir"); } }
        public static CoreTexture LeftBumper { get { return Tools.Texture("Xbox_LB"); } }
        public static CoreTexture RightBumper { get { return Tools.Texture("Xbox_RB"); } }
#endif
    }

    public class ButtonString
    {
#if PC
        public static Dictionary<Keys, string> KeyToString;
        public static void Init()
        {
            KeyToString = new Dictionary<Keys, string>();

            KeyToString.Add(Keys.None, "None");
            
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

            KeyToString.Add(Keys.Enter, "Enter");
            KeyToString.Add(Keys.Space, "Space");
            KeyToString.Add(Keys.Back, "Backspace");
            KeyToString.Add(Keys.Escape, "Esc");

            //KeyToString.Add(Keys.OemPeriod, ".");
            //KeyToString.Add(Keys.OemComma, ",");
            //KeyToString.Add(Keys.OemBackslash, "\\");
            //KeyToString.Add(Keys.OemQuestion, "?");
            //KeyToString.Add(Keys.OemOpenBrackets, "[");
            //KeyToString.Add(Keys.OemCloseBrackets, "]");

            //KeyToString.Add(Keys.RightControl, "R Cntrl");
            //KeyToString.Add(Keys.LeftControl, "L Cntrl");
            //KeyToString.Add(Keys.RightAlt, "R Alt");
            //KeyToString.Add(Keys.LeftAlt, "L Alt");
            //KeyToString.Add(Keys.RightShift, "R Shift");
            //KeyToString.Add(Keys.LeftShift, "L Shift");
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
        
        // Go/Back no keys
        //public static string Back(int size) { return ""; }
        //public static string Go(int size) { return ""; }
        public static string Up(int size) { return string.Format("{{pUp_Key,{0},?}}{{s15,0}}", size); }

        public static string Backspace(int size) { return string.Format("{{p{1},{0},?}}{{s70,0}}", size, KeyToTexture(Keys.Back)); }
        public static string Enter(int size) { return string.Format("{{p{1},{0},?}}{{s70,0}}", size, KeyToTexture(Keys.Enter)); }        

        // Regular keys
        public static string KeyStr(Keys key, int size)
        {
            if (key == Keys.Enter || key == Keys.Space)
                size = (int)(2.0083f * size);

            return string.Format("{{p{0},{1},?}}{{s15,0}}", ButtonString.KeyToTexture(key), size);
        }

		public static string LeftClick(int size)	 { return string.Format("{{pLeftClick_Icon,{0},?}}", size); }
        public static string Go_Controller(int size) { return string.Format("{{pXbox_A,{0},?}}", size); }
#else
        public static void Init() { }
#endif

        public static string Back(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.Back.Name); }
        public static string Go(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.Go.Name); }
        public static string X(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.X.Name); }
        public static string Y(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.Y.Name); }
        public static string LeftRight(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.LeftRight.Name); }
        public static string LeftBumper(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.LeftBumper.Name); }
        public static string RightBumper(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.RightBumper.Name); }
        public static string Jump(int size) { return string.Format("{{p{1},{0},?}}", size, ButtonTexture.Go.Name); }


    }

    public class Text : ViewReadWrite
    {
        public override string[] GetViewables()
        {
            return new string[] { "Name", "MyString", "Code", "FancyPos", "_Pos", "Shadow", "PicShadow", "ShadowOffset", "ShadowColor", "ShadowScale",
                "Alpha", "MyColor", "MyFloatColor" };
        }

        public override string GetConstructorString()
        {
            return string.Format("new Text(\"{0}\", ItemFont)", MyString);
        }

        public override void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
#if WINDOWS && DEBUG
			if (Tools.CntrlDown() && ShiftDown)
				return;

            if (ShiftDown)
                Scale += (shift.X + shift.Y) * .00003f;
            else
                Pos += shift;
#endif
        }

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

        /// <summary>
        /// Name of the text, used in DrawPiles
        /// </summary>
        public string Name = "";

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

        class TextBit
        {
            public int LineNumber;
            public String str;
            public StringBuilder builder_str; 
            public Vector2 loc, size;
            public Color clr;
        }
        class TextPic
        {
            public int LineNumber;
            public CoreTexture tex;
            public Rectangle rect;
            public Vector2 size;
            public bool AsPaint;
        }
        List<TextBit> Bits;
        List<TextPic> Pics;

        Vector2 MeasureString(string text)
        {
            Vector2 val;
            
            //val = MyFont.Font.MeasureString(text);
            //return val;

            val = Tools.QDrawer.MeasureString(MyFont.HFont, text);
            return val;
        }

        Vector2 MeasureString(StringBuilder text)
        {
            return Tools.QDrawer.MeasureString(MyFont.HFont, text);
            //return MyFont.Font.MeasureString(text);
        }

		/// <summary>
		/// Returns the string from the first bit of text
		/// </summary>
		/// <returns></returns>
		public string FirstString()
		{
			if (Bits == null || Bits.Count == 0)
				return "";

			return Bits[0].str;
		}

        /// <summary>
        /// Replaces the first bit of text, with no reformatting
        /// </summary>
        public void SubstituteText(string text)
        {
            Bits[0].str = text;
            Bits[0].builder_str = null;
            TextWidth -= Bits[0].size.X;
            Bits[0].size = MeasureString(text);
            TextWidth += Bits[0].size.X;
        }
        public void SubstituteText(StringBuilder text)
        {
            Bits[0].builder_str = text;
            TextWidth -= Bits[0].size.X;
            Bits[0].size = MeasureString(text);
            TextWidth += Bits[0].size.X;
        }
        public void SubstituteText(Localization.Words word)
        {
            SubstituteText(Localization.WordString(word));
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

        public bool FixedToCamera;

        public bool ColorizePics;
        Color PicColor;

        public CoreFont MyFont;
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
            set { _Scale = value; }
            get { return _Scale; }
        }

        public void Release()
        {
            if (FancyPos != null) FancyPos.Release(); FancyPos = null;
        }

        public virtual Text Clone()
        {
            Text clone = new Text(MyString, MyFont,TextBoxWidth, Centered, YCentered, LineHeightMod);
            clone.MyFloatColor = MyFloatColor;
            clone.OutlineColor = OutlineColor;
            return clone;
        }

		public virtual Text Clone(string Text)
		{
			Text clone = new Text(Text, MyFont, TextBoxWidth, Centered, YCentered, LineHeightMod);
			clone.MyFloatColor = MyFloatColor;
			clone.OutlineColor = OutlineColor;
			return clone;
		}

        public enum Style { Normal, FadingOff };
        public bool RightJustify = false;
        public bool Centered, YCentered;

        public Text(Localization.Words word) { MyFont = Resources.Font_Grobold42; Init(Localization.WordString(word)); }
        public Text(Localization.Words word, CoreFont font) { MyFont = font; Init(Localization.WordString(word)); }
        public Text(Localization.Words word, CoreFont font, string Name) { this.Name = Name; MyFont = font; Init(Localization.WordString(word)); }
        public Text(Localization.Words word, CoreFont font, bool Centered) { MyFont = font; Init(Localization.WordString(word), 10000, Centered, false, 1); }
        public Text(Localization.Words word, CoreFont font, bool Centered, bool YCentered) { MyFont = font; Init(Localization.WordString(word), 10000, Centered, YCentered, 1); }
        public Text(Localization.Words word, CoreFont font, float Width, bool Centered, bool YCentered) { MyFont = font; Init(Localization.WordString(word), Width, Centered, YCentered, 1); }
        public Text(Localization.Words word, CoreFont font, float Width, bool Centered, bool YCentered, float LineHeightMod) { MyFont = font; Init(Localization.WordString(word), Width, Centered, YCentered, LineHeightMod); }

        public Text(String str) { MyFont = Resources.Font_Grobold42; Init(str); }
        public Text(String str, CoreFont font) { MyFont = font; Init(str); }
        public Text(String str, CoreFont font, string Name) { this.Name = Name; MyFont = font; Init(str); }
        public Text(String str, CoreFont font, bool Centered) { MyFont = font; Init(str, 10000, Centered, false, 1); }
        public Text(String str, CoreFont font, bool Centered, bool YCentered) { MyFont = font; Init(str, 10000, Centered, YCentered, 1); }
        public Text(String str, CoreFont font, float Width, bool Centered, bool YCentered) { MyFont = font; Init(str, Width, Centered, YCentered, 1); }
        public Text(String str, CoreFont font, float Width, bool Centered, bool YCentered, float LineHeightMod) { MyFont = font; Init(str, Width, Centered, YCentered, LineHeightMod); }

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

            int Comma1, Comma2, Comma3;
            String WidthString, HeightString;
            switch (c)
            {
                case 'p':
                    Parse_Type = ParseData.Pic;

                    string[] string_bits = str.Split(',');

                    Comma1 = str.IndexOf(",");
                    Comma2 = str.IndexOf(",", Comma1 + 1);

                    Parse_PicName = str.Substring(2, Comma1 - 2);
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
                    CoreTexture texture = Tools.Texture(Parse_PicName);
                    float ratio = (float)texture.Width / (float)texture.Height;

                    // 's' scale the texture
                    if (WidthString.Contains('s'))
                    {
                        size = new Vector2(texture.Width, texture.Height);
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
                DelimiterIndex = Math.Max(str.IndexOf('\r', EndIndex), str.IndexOf('\n', EndIndex));
                if (BracketIndex == -1 && SpaceIndex == -1) { NewEndIndex = str.Length; ReachedEnd = true; }
                else if (BracketIndex == -1) NewEndIndex = SpaceIndex + 1;
                else if (SpaceIndex == -1) NewEndIndex = BracketIndex + 1;
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

            Vector2 TextSize = MeasureString(str);
            TextSize.Y *= Tools.TheGame.Resolution.LineHeightMod;

            Size.X += TextSize.X;
            Size.Y = Math.Max(Size.Y, TextSize.Y);

            return Size;
        }

        Color CurColor = Color.White;
        float AddLine(String str, float StartX, float StartY, int LineNumber)
        {
            Vector2 loc = new Vector2(StartX, 0);
            float LineHeight = MeasureString(" ").Y;
            int BeginBracketIndex, EndBracketIndex;

            bool FirstElement = true;

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
                        TextPic pic = new TextPic();
                        pic.LineNumber = LineNumber;

                        if (!FirstElement) Parse_PicShift.X -= .25f * Parse_PicSize.X;

                        pic.tex = Tools.Texture(Parse_PicName);

                        float y = .5f * Tools.TheGame.Resolution.LineHeightMod * MyFont.LineSpacing - .5f * Parse_PicSize.Y + StartY;
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
                        loc.X += .35f * MyFont.HFont.font.CharSpacing;// Font.Spacing;
                    }
                }
                // Parse text info
                else
                {
                    int i;
                    if (BeginBracketIndex < 0) i = str.Length; else i = BeginBracketIndex;

                    TextBit bit = new TextBit();
                    bit.LineNumber = LineNumber;
                    bit.clr = CurColor;
                    bit.str = str.Substring(0, i);
                    str = str.Remove(0, i);

                    bit.size = MeasureString(bit.str);
                    bit.size.Y *= Tools.TheGame.Resolution.LineHeightMod;
                    
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
            return Scale * GetWorldFloat(MeasureString(str).X);
        }

        Vector2 _MyCameraZoom = new Vector2(.001f, .001f);
        /// <summary>
        /// The value of the camera zoom the last time this Text was drawn
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

            Bits = new List<TextBit>();
            Pics = new List<TextPic>();

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

                str = str.Remove(0, i);
                if (str.Length > 0 && str[0] == ' ') str = str.Remove(0, 1);
                if (str.Length > 0 && (str[0] == '\n' || str[0] == '\r')) str = str.Remove(0, 1);

                LineNumber++;
            }

            Height = y;

            
            if (YCentered)
            {
                foreach (TextBit bit in Bits) bit.loc.Y -= Height / 2;
                foreach (TextPic pic in Pics) pic.rect.Y -= (int)(Height / 2);
            }

            if (!Centered)
            {
                foreach (TextBit bit in Bits) bit.loc.X += LineSizes[bit.LineNumber].X / 2 * Tools.TheGame.Resolution.LineHeightMod;
                foreach (TextPic pic in Pics) pic.rect.X += (int)(LineSizes[pic.LineNumber].X / 2);// * Tools.TheGame.Resolution.LineHeightMod);
            }
            else
            {
                foreach (TextPic pic in Pics) pic.rect.X = (int)((Scale * pic.rect.X - Pos.X + pic.rect.Width / 2f) * Tools.TheGame.Resolution.LineHeightMod + Pos.X - pic.rect.Width / 2f);
            }
        }

        public void Center()
        {
            Pos = new Vector2(Pos.X - .5f * GetWorldWidth(), Pos.Y);
        }

        Vector2 JustificationShift;

        public bool Show = true;
        public void Draw(Camera cam) { Draw(cam, true); }
        public virtual void Draw(Camera cam, bool EndBatch)
        {
            Alpha += AlphaVel;

            MyCameraZoom = cam.Zoom;

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

            PicColor = Color.Black;
            if (Shadow)
            {
                // Note: never end the SpriteBatch for drawing a shadow,
                // since we will always be drawing the non-shadow part afterward

                float _HoldScale = Scale;
                Scale *= ShadowScale;

                _Pos -= ShadowOffset;
                if (MyFont.HOutlineFont != null || OutlineColor.W == 0)
                    _Draw(cam, false, PicShadow, MyFont.HOutlineFont, ShadowColor.ToVector4());
                _Draw(cam, false, false, MyFont.HFont, ShadowColor.ToVector4());
                _Pos += ShadowOffset;

                Scale = _HoldScale;
            }
            PicColor = Color.White;

            if (!ColorizePics)
            {
                PicColor = Color.White;
            }

            if (MyFont.HOutlineFont != null && OutlineColor.W != 0)
                _Draw(cam, false, true, MyFont.HOutlineFont, OutlineColor); 
            _Draw(cam, EndBatch, true, MyFont.HFont, MyFloatColor);

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

        public float Angle = 0;

        public static bool ZoomWithCamera_Override = false;
        public bool ZoomWithCam = false;
        public void _Draw(Camera cam, bool EndBatch, bool DrawPics, HackSpriteFont font, Vector4 color)
        {
            if (MyFloatColor.W <= 0) return;

            MyColor.R = Tools.FloatToByte(color.X);
            MyColor.G = Tools.FloatToByte(color.Y);
            MyColor.B = Tools.FloatToByte(color.Z);
            MyColor.A = Tools.FloatToByte(color.W * Alpha);

            float ZoomMod = 1;
            if (ZoomWithCam || ZoomWithCamera_Override)
                ZoomMod = MyCameraZoom.X / .001f;

            //Tools.Write(new Vector2(Tools.EffectWad.CameraPosition.X, Tools.EffectWad.CameraPosition.Y));

            Vector2 Position = _Pos + JustificationShift;
            //if (FixedToCamera) Position += cam.Data.Position;
            if (FixedToCamera) Position += new Vector2(Tools.EffectWad.CameraPosition.X, Tools.EffectWad.CameraPosition.Y);
            Vector2 Loc = Tools.ToScreenCoordinates(Position, cam, Tools.EffectWad.ModZoom);

            foreach (TextBit bit in Bits)
            {
                Color textcolor = ColorHelper.PremultiplyAlpha(new Color(MyColor.ToVector4() * bit.clr.ToVector4()));

			    Vector2 _pos = Scale * bit.loc * ZoomMod + Loc;
			    _pos = Tools.ToWorldCoordinates( _pos, cam, MyCameraZoom * Tools.EffectWad.ModZoom.X );

                if (bit.builder_str != null)
                    Tools.QDrawer.DrawString(font, bit.builder_str, _pos, textcolor.ToVector4(), new Vector2(Tools.TheGame.Resolution.LineHeightMod) * Scale * ZoomMod * (1000f / 360f));
                else
                    Tools.QDrawer.DrawString(font, bit.str, _pos, textcolor.ToVector4(), new Vector2(Tools.TheGame.Resolution.LineHeightMod) * Scale * ZoomMod * (1000f / 360f));
            }
            if (DrawPics)
                foreach (TextPic pic in Pics)
                {
                    Color piccolor = PicColor;
                    piccolor.A = Tools.FloatToByte(Alpha * piccolor.A / 255f);

                    piccolor = ColorHelper.PremultiplyAlpha(piccolor);

                    Vector2 pos, scale;

                    pos = Loc + Scale * ZoomMod * new Vector2(pic.rect.X, pic.rect.Y);
                    scale = Scale * ZoomMod * new Vector2(pic.rect.Width / (float)pic.tex.Width, pic.rect.Height / (float)pic.tex.Height);

                    pos = Loc + Scale * ZoomMod * new Vector2(pic.rect.X, pic.rect.Y);
                    scale = Scale * ZoomMod * new Vector2(pic.rect.Width, pic.rect.Height);

				    Vector2 pos2 = pos + scale;
				    pos = Tools.ToWorldCoordinates( pos, cam, MyCameraZoom * Tools.EffectWad.ModZoom.X );
				    pos2 = Tools.ToWorldCoordinates( pos2, cam, MyCameraZoom * Tools.EffectWad.ModZoom.X );

                    Tools.QDrawer.DrawPic(pos, pos2, pic.tex, piccolor);
                }
        }

        public void CalcBounds()
        {
            TR = new Vector2(-100000000, -100000000);
            BL = new Vector2(100000000, 100000000);

            Vector2 Loc = _Pos;

            foreach (TextBit bit in Bits)
            {
                Vector2 loc, size;

                loc = GetWorldVector(bit.loc);
                size = GetWorldVector(bit.size);

                Vector2 bit_TR = Scale * loc + Loc; bit_TR.X += 1 * Scale * size.X; bit_TR.Y += .25f * Scale * size.Y;
                Vector2 bit_BL = Scale * loc + Loc; bit_BL.Y += .75f * Scale * size.Y;

                TR = Vector2.Max(TR, bit_TR);
                BL = Vector2.Min(BL, bit_BL);
            }

            foreach (TextPic pic in Pics)
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
