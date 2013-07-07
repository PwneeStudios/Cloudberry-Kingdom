using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CoreEngine;

#if WINDOWS
using KeyboardHandler;
#endif

namespace CloudberryKingdom
{
#if PC_VERSION
    public class GUI_EnterName : GUI_TextBox
    {
        public GUI_EnterName() : base(PlayerManager.DefaultName, Vector2.Zero)
        {
            EzText Text = new EzText(Localization.Words.NewHighScore, Resources.Font_Grobold42);

            Text.Pos = new Vector2(-579.365f, 253.9681f);
            Text.Scale *= .7f;
            MyPile.Add(Text);
        }

        public override void Enter()
        {
            if (Length > 0)
                PlayerManager.DefaultName = Text;
            else
                Text = PlayerManager.DefaultName;

            base.Enter();
        }
    }
#endif

    public class GUI_TextBox : GUI_Text
    {
        /// <summary>
        /// Event handler. Activated when the the user presses Enter while the textbox has focus.
        /// </summary>
        public event Action OnEnter, OnEscape;

        public override void OnAdd()
        {
            base.OnAdd();
            
            GetFocus();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            OnEnter = null;
            OnEscape = null;
            ReleaseFocus();
        }

        bool HasFocus = false;
        public void GetFocus()
        {
            HasFocus = true;

#if WINDOWS && !XBOX
			//KeyboardExtension.FreezeInput();

			//EventInput.CharEntered += CharEntered;
			//EventInput.KeyDown += KeyDown;
#endif
        }

        public void ReleaseFocus()
        {
            if (!HasFocus) return;
            HasFocus = false;

#if WINDOWS
			//KeyboardExtension.UnfreezeInput();

			//EventInput.CharEntered -= CharEntered;
			//EventInput.KeyDown -= KeyDown;
#endif
        }

		void PosCaret()
		{
			// Position the caret at the end of the text
			string hold = Text;
			string _s = Text.Substring(0, Math.Max(0, Text.Length - 1));
			//string _s = Text;
			MyText.SubstituteText(_s);
			float width = MyText.GetWorldWidth();
			MyText.SubstituteText(hold);

			Caret.Scale = MyText.Scale * 1.105f;
			if (_s.Length == 0)
				Caret.X = MyText.Pos.X + 15.5f;
			else
				Caret.X = MyText.Pos.X + width - 130 * Caret.Scale;
			Caret.Y = MyText.Pos.Y + 7 * Caret.Scale;

			// If we're selecting move the caret one character to the left
			//if (SelectIndex_End - SelectIndex_Start > 0)
			//    Caret.X -= MyText.GetWorldWidth(" ");
		}

		void ScaleTextToFit()
		{
			MyText.Scale = .8f;
			float w = MyText.GetWorldWidth();
			float MaxWidth = 2400.0f;
			if (w > MaxWidth)
			{
				MyText.Scale *= MaxWidth / w;
			}
		}

		protected override void MyDraw()
		{
			// Draw first part of string
			//string hold = Text;
			//Vector4 HoldColor = MyText.MyFloatColor;
			//MyText.MyFloatColor = Color.Green.ToVector4();
			//string _s = Text.Substring(0, Math.Max(0, Text.Length - 1));
			//MyText.SubstituteText(_s);
			//MyText.Draw(Tools.CurCamera);
			//MyText.SubstituteText(hold);
			//MyText.MyFloatColor = HoldColor;

			// Draw normally
			base.MyDraw();
		}

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

			//Tools.TheGame.ShowMouse = true;

			ScaleTextToFit();

			PosCaret();

            // Decide if we should draw the caret
            if (HasFocus)
            {
                // Don't draw the caret if there isn't room for another character
                if (LimitLength && Length == MaxLength)
                    Caret.Show = false;
                else
                {
                    // Draw the caret every other half second
					//Caret.Show = Tools.TheGame.DrawCount / 30 % 2 == 0;
					Caret.Show = Tools.TheGame.DrawCount % 65 > 23;
                }

#if PC_VERSION
				ButtonCheck.DisableSecondaryInput = true;

				KeyboardExtension.Freeze = false;
                if (ButtonCheck.State(Keys.Escape).Down) { Cancel(); return; }
                if (ButtonCheck.State(Keys.V).Pressed && Tools.CntrlDown()) { Paste(); return; }
                //if (ButtonCheck.State(Keys.V).Pressed && Tools.CntrlDown()) { Paste(); return; }
                if (ButtonCheck.State(Keys.C).Down && Tools.CntrlDown()) { Copy(); return; }
                if (ButtonCheck.State(Keys.X).Down && Tools.CntrlDown()) { Copy(); Clear(); return; }

				GamepadInteract();

				ButtonCheck.DisableSecondaryInput = false;
#else
				GamepadInteract();
#endif


			}
            else
                // Don't draw the caret when we don't have focus
                Caret.Show = false;
        }

        public bool Canceled = false;
        void Cancel()
        {
            if (Canceled) return;
            if (OnEscape != null) OnEscape();
        }

        bool EnterPressed = false;
        bool UsingGamepad = true;
        bool BackspacePressed = false;
        void GamepadInteract()
        {
            int control = Control;
            if (control < 0 || control > 3)
            {
                control = -1;
            }
            else
            {
                if (!PlayerManager.Players[control].Exists)
                {
                    control = -1;
                }
            }

            if (Length == 0)
            {
                if (ButtonCheck.State(ControllerButtons.A, control).Pressed) if (Length < MaxLength) Text += 'A';
                return;
            }

            char c = Text[Length - 1];
            if (ButtonCheck.State(ControllerButtons.A, control).Pressed) if (Length < MaxLength)
			{
				//UsingGamepad = true;
				if (ButtonCheck.ControllerInUse ||
					(!ButtonCheck.State(Keys.Space).Down && !ButtonCheck.State(Keys.Enter).Down))
				{
					Text += c;
					Recenter();
				}
			}
            if (ButtonCheck.State(ControllerButtons.X, control).Pressed || BackspacePressed) { UsingGamepad = true; Backspace(); BackspacePressed = false; return; }
            //if (ButtonCheck.State(ControllerButtons.Y, control).Pressed) { Cancel(); return; }
            if (ButtonCheck.State(ControllerButtons.Start, control).Pressed || EnterPressed)
			{
				if (ButtonCheck.ControllerInUse ||
					!ButtonCheck.State(Keys.Back).Down)
				{
					Enter(); EnterPressed = false;
					return;
				}
			}
            if (ButtonCheck.State(ControllerButtons.B, control).Pressed) 
			{
				if (ButtonCheck.ControllerInUse ||
					!ButtonCheck.State(Keys.Back).Down)
				{
					Cancel();
					return;
				}
			}
            BackspacePressed = false;

#if XBOX
			bool PlayerKeyboardUsed = false;

            if (Control >= 0 && Tools.PlayerKeyboard[Control] != null)
            {
                ProcessKeyboard(Tools.PlayerKeyboard[Control], Tools.PrevPlayerKeyboard[Control]);
                PlayerKeyboardUsed = true;
            }

            if (Tools.Keyboard != null && !(PlayerKeyboardUsed && Tools.Keyboard == Tools.PlayerKeyboard[Control]))
			{
				ProcessKeyboard(Tools.Keyboard, Tools.PrevKeyboard);
			}
#else
			ProcessKeyboard(Tools.Keyboard, Tools.PrevKeyboard);
#endif

			// Gamepad control
			if (ButtonCheck.ControllerInUse)
			{
				var dir = ButtonCheck.GetDir(control);

				if (Tools.TheGame.DrawCount % 7 == 0 && Math.Abs(dir.Y) > .5 && !ButtonCheck.State(ControllerButtons.A, -1).Down)
				{
					if (dir.Y > 0) Text = Text.Substring(0, Length - 1) + IncrChar(c);
					if (dir.Y < 0) Text = Text.Substring(0, Length - 1) + DecrChar(c);

					Recenter();
				}
			}
        }

        static Keys[] ValidKeys = new Keys[] {
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
            Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemQuotes, Keys.OemBackslash, Keys.OemTilde,
            Keys.Space, Keys.Enter, Keys.Back, Keys.Delete, Keys.CapsLock,
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
        };

        static List<char> ValidChars = KeysToList();

        static List<char> KeysToList()
        {
            var l = new List<char>();
            for (int i = 0; i < ValidKeys.Length; i++)
            {
                string s = KeyToChar(ValidKeys[i]);

                if (s.Length == 1)
                    l.Add(s[0]);
            }

            l.AddRange(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' });

            return l;
        }

        static string KeyToChar(Keys key)
        {
            switch (key)
            {
                case Keys.Space: return " ";
                case Keys.OemOpenBrackets: return "[";
                case Keys.OemCloseBrackets: return "]";
                case Keys.OemQuotes: return "\"";
                case Keys.OemBackslash: return "/";
                case Keys.OemTilde: return "`";

                case Keys.D0: return "0";
                case Keys.D1: return "1";
                case Keys.D2: return "2";
                case Keys.D3: return "3";
                case Keys.D4: return "4";
                case Keys.D5: return "5";
                case Keys.D6: return "6";
                case Keys.D7: return "7";
                case Keys.D8: return "8";
                case Keys.D9: return "9";
                default: return key.ToString();
            }
        }

        bool CapsOn;
		int BackspaceCount = 0;
        void ProcessKeyboard(KeyboardState cur, KeyboardState prev)
        {
			if (cur.IsKeyDown(Keys.Delete) || cur.IsKeyDown(Keys.Back))
			{
				BackspaceCount++;

				int Delay = 4;
				if (BackspaceCount > 14) Delay = 3;
				if (BackspaceCount > 18) Delay = 2;

				if (BackspaceCount > 10 && (BackspaceCount - 10) % Delay == 0)
				{
					BackspacePressed = true;
					return;
				}
			}
			else
			{
				BackspaceCount = 0;
			}
            
            foreach (Keys key in ValidKeys)
            {
                if (cur.IsKeyDown(key) && !prev.IsKeyDown(key))
                {
                    if (cur.IsKeyDown(Keys.Enter))
                    {
                        EnterPressed = true;
                        return;
                    }
                    else if (cur.IsKeyDown(Keys.Delete) && !prev.IsKeyDown(Keys.Delete) ||
                             cur.IsKeyDown(Keys.Back) && !prev.IsKeyDown(Keys.Back))
                    {
                        BackspacePressed = true;
                        return;
                    }
                    else if (cur.IsKeyDown(Keys.CapsLock) && !prev.IsKeyDown(Keys.CapsLock))
                    {
                        CapsOn = !CapsOn;
                        return;
                    }

                    string _c = KeyToChar(key);

                    bool MakeUppercase = CapsOn;
                    if (cur.IsKeyDown(Keys.CapsLock) || cur.IsKeyDown(Keys.LeftShift) || cur.IsKeyDown(Keys.RightShift))
                    {
                        MakeUppercase = !MakeUppercase;
                    }

                    if (MakeUppercase)
                    {
                        _c = _c.ToUpper();
                    }
                    else
                    {
                        _c = _c.ToLower();
                    }

                    if (_c.Length == 0) continue;
                    char c = _c[0];

                    if (Length < MaxLength)
                    {
                        if (UsingGamepad && Text.Length == 1)
                        {
                            Text = _c;
                        }
                        else
                        {
                            Text += _c;
                        }
                        Recenter();
                    }

                    UsingGamepad = false;
                }
            }
        }

#if WINDOWS
        void Paste()
        {
            Clear();

            string clipboard = System.Windows.Forms.Clipboard.GetText();

            clipboard = Tools.SantitizeOneLineString(clipboard, Resources.LilFont);

            Text += Tools.SantitizeOneLineString(clipboard, MyText.MyFont);
        }

        void Copy()
        {
            if (Text != null && Text.Length > 0)
                System.Windows.Forms.Clipboard.SetText(Text);
        }
#endif

        char IncrChar(char c)
        {
            int i = ValidChars.IndexOf(c);
            if (i < 0) return 'A';

            i++;
            if (i >= ValidChars.Count)
                return ValidChars[0];

            return ValidChars[i];
            //var _c = (int)c + 1;
            //if (_c > (int)'z') return 'A';
            //return (char)_c;
        }
        char DecrChar(char c)
        {
            int i = ValidChars.IndexOf(c);
            if (i < 0) return 'A';

            i--;
            if (i < 0)
                return ValidChars[ValidChars.Count - 1];

            return ValidChars[i];

            //var _c = (int)c - 1;
            //if (_c < (int)'A') return 'z';
            //return (char)_c;
        }

        QuadClass Backdrop, SelectQuad;
        EzText Caret;

        public GUI_TextBox(string InitialText, Vector2 pos)
            : base(Tools.SantitizeOneLineString(InitialText, Resources.Font_Grobold42), pos, false)
        {
            Init(InitialText, pos, Vector2.One, 1f);
        }

        public GUI_TextBox(string InitialText, Vector2 pos, Vector2 scale, float fontscale)
            : base(InitialText, pos, false, Resources.LilFont)
        {
            Init(InitialText, pos, scale, fontscale);
        }

        void Init(string InitialText, Vector2 pos, Vector2 scale, float fontscale)
        {
            InitialText = Tools.SantitizeOneLineString(InitialText, Resources.LilFont);

			if (InitialText.Length == 0)
				InitialText = "_";

            FixedToCamera = true;
            NoPosMod = true;

            MyText.Scale *= fontscale;

            // Backdrop
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "score_screen";
            Backdrop.Size = new Vector2(640.4763f, 138.0953f) * scale;

            MyText.Pos = new Vector2(-522.2222f + 40, 23.80954f) * scale;
            //MyPile.Insert(0, Backdrop);

            // Caret
            //var font = Resources.Font_Grobold42;
            //var font = Resources.LilFont;
            var font = Resources.Font_Grobold42;
            Caret = new EzText("_", font, 1000, false, true, .575f);
            Caret.MyFloatColor = Color.Black.ToVector4();
            Caret.Pos = MyText.Pos;
            Caret.Scale *= fontscale;

            MyPile.Add(Caret);

            // Select quad
            SelectQuad = new QuadClass(null, true, false);
            SelectQuad.TextureName = "White";
            SelectQuad.Quad.SetColor(new Color(255, 255, 255, 125));
            SelectQuad.Size = new Vector2(100f, 100f * scale.Y);
            SelectQuad.Layer = 0;

            MyPile.Add(SelectQuad);

            SelectAll();
            Recenter();
        }

        int SelectIndex_Start = 0, SelectIndex_End = 0;

        void SelectAll()
        {
            SelectIndex_Start = 0;
            SelectIndex_End = Length;

            UpdateSelectQuad();
        }

        void Unselect()
        {
            SelectIndex_Start = SelectIndex_End = 0;
            UpdateSelectQuad();
        }

        void Clear()
        {
            SelectAll();
            DeleteSelected();
        }

        void DeleteSelected()
        {
			//Text = Text.Remove(SelectIndex_Start, SelectIndex_End - SelectIndex_Start);
			Text = "A";
            Unselect();
        }

        void UpdateSelectQuad()
        {
            float shift = 690;
            float width = 1820;// MyText.GetWorldWidth(Text.Substring(SelectIndex_Start, SelectIndex_End - SelectIndex_Start));
            width += shift;
            float pos = MyText.GetWorldWidth(Text.Substring(0, SelectIndex_Start));

            SelectQuad.Size = new Vector2(width / 2 + 50, SelectQuad.Size.Y + 30);
            SelectQuad.Left = MyText.Pos.X + pos;
            SelectQuad.Pos = new Vector2(347, -MyText.GetWorldHeight() / 4);
        }

        protected override EzText MakeText(string text, bool centered, EzFont font)
        {
            EzText eztext = new EzText(text, font, 100000, centered, true, .575f);
            eztext.MyFloatColor = Color.Black.ToVector4();
            eztext.OutlineColor = Color.Transparent.ToVector4();
            
            return eztext;
        }

        public int MaxLength = 18;
        public bool LimitLength = true;

#if WINDOWS
        void CharEntered(object o, CharacterEventArgs e)
        {
            if (!Active) return;

            DeleteSelected();

            if (IsAcceptableChar(e.Character) && (!LimitLength || MyText.FirstString().Length < MaxLength))
            {
                MyText.AppendText(e.Character);
                Recenter();
            }
        }

        bool IsAcceptableChar(char c)
        {
            int ascii = (int)c;

            return ascii >= 32 && ascii <= 122;
        }
#endif

        bool IsLetter(char c)
        {
            int ascii = (int)c;

            if (ascii >= 65 && ascii <= 90 || ascii >= 97 && ascii <= 122)
                return true;
            else
                return false;
        }

        /// <summary>
        /// If true the text is recentered every time it is changed
        /// </summary>
        public bool DoRecenter = false;

        void Recenter()
        {
            if (DoRecenter)
                MyText.Pos = new Vector2(-MyText.GetWorldWidth() / 2, 0);

			ScaleTextToFit();
			PosCaret();
        }

#if WINDOWS
        void KeyDown(object o, KeyEventArgs e)
        {
            if (!Active) return;

            if (e.KeyCode == Keys.Back) Backspace();
            if (e.KeyCode == Keys.Enter) Enter();
        }
#endif

        void Backspace()
        {
            // If we're selecting delete the selection
			//if (SelectIndex_End - SelectIndex_Start > 0)
			//    DeleteSelected();
			//else
            {
                // Otherwise delete one character
                if (Length <= 1) return;

                Text = Text.Substring(0, Length - 1);
            }

            Recenter();
        }

        public virtual void Enter()
        {
            Unselect();

            ReleaseFocus();
            MyPile.Jiggle(true);

            // Change the backdrop color
            Backdrop.TextureName = "Score\\Score_Screen_grey";

            if (OnEnter != null) OnEnter();
        }

        protected int Length { get { return Text.Length; } }
        public string Text
        {
            get
            {
                return MyText.FirstString();
            }
            set
            {
                MyText.SubstituteText(value);
            }
        }
    }
}
