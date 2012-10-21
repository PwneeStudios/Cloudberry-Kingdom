using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;

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
            EzText Text = new EzText("New high score!", Tools.Font_Grobold42);

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

#if WINDOWS
            KeyboardExtension.FreezeInput();

            EventInput.CharEntered += CharEntered;
            EventInput.KeyDown += KeyDown;
#endif
        }

        public void ReleaseFocus()
        {
            if (!HasFocus) return;
            HasFocus = false;

#if WINDOWS
            KeyboardExtension.UnfreezeInput();

            EventInput.CharEntered -= CharEntered;
            EventInput.KeyDown -= KeyDown;
#endif
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            // Decide if we should draw the caret
            if (HasFocus)
            {
                // Don't draw the caret if there isn't room for another character
                if (LimitLength && Length == MaxLength)
                    Caret.Show = false;
                else
                {
                    // Draw the caret every other half second
                    Caret.Show = Tools.TheGame.DrawCount / 30 % 2 == 0;
                }

#if WINDOWS
                KeyboardExtension.Freeze = false;
                if (ButtonCheck.State(Keys.Escape).Down) { Cancel(); return; }
                if (ButtonCheck.State(Keys.V).Pressed && Tools.CntrlDown()) { Paste(); return; }
                //if (ButtonCheck.State(Keys.V).Pressed && Tools.CntrlDown()) { Paste(); return; }
                if (ButtonCheck.State(Keys.C).Down && Tools.CntrlDown()) { Copy(); return; }
                if (ButtonCheck.State(Keys.X).Down && Tools.CntrlDown()) { Copy(); Clear(); return; }
                KeyboardExtension.Freeze = true;
#endif

                GamepadInteract();
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

        void GamepadInteract()
        {
            if (Length == 0)
            {
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed) if (Length < MaxLength) Text += 'A';
                return;
            }

            char c = Text[Length - 1];
            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed) if (Length < MaxLength) { Text += c; Recenter(); }
            if (ButtonCheck.State(ControllerButtons.X, -1).Pressed) { Backspace(); return; }
            if (ButtonCheck.State(ControllerButtons.Y, -1).Pressed) { Cancel(); return; }
            if (ButtonCheck.State(ControllerButtons.Start, -1).Pressed) { Enter(); return; }

            var dir = ButtonCheck.GetDir(-1);

            if (Tools.TheGame.DrawCount % 7 == 0 && Math.Abs(dir.Y) > .5)
            {
                if (dir.Y > 0) Text = Text.Substring(0, Length - 1) + IncrChar(c);
                if (dir.Y < 0) Text = Text.Substring(0, Length - 1) + DecrChar(c);

                Recenter();
            }
        }

#if WINDOWS
        void Paste()
        {
            Clear();

            string clipboard = System.Windows.Forms.Clipboard.GetText();

            clipboard = Tools.SantitizeOneLineString(clipboard, Tools.LilFont);

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
            var _c = (int)c + 1;
            if (_c > (int)'z') return 'A';
            return (char)_c;
        }
        char DecrChar(char c)
        {
            var _c = (int)c - 1;
            if (_c < (int)'A') return 'z';
            return (char)_c;
        }

        QuadClass Backdrop, SelectQuad;
        EzText Caret;

        public GUI_TextBox(string InitialText, Vector2 pos)
            : base(Tools.SantitizeOneLineString(InitialText, Tools.Font_Grobold42), pos, false)
        {
            Init(InitialText, pos, Vector2.One, 1f);
        }

        public GUI_TextBox(string InitialText, Vector2 pos, Vector2 scale, float fontscale)
            : base(InitialText, pos, false, Tools.LilFont)
        {
            Init(InitialText, pos, scale, fontscale);
        }

        void Init(string InitialText, Vector2 pos, Vector2 scale, float fontscale)
        {
            InitialText = Tools.SantitizeOneLineString(InitialText, Tools.LilFont);

            FixedToCamera = true;
            NoPosMod = true;

            MyText.Scale *= fontscale;

            // Backdrop
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "score screen";
            Backdrop.Size = new Vector2(640.4763f, 138.0953f) * scale;

            MyText.Pos = new Vector2(-522.2222f, 23.80954f) * scale;
            //MyPile.Insert(0, Backdrop);

            // Caret
            //var font = Tools.Font_Grobold42;
            var font = Tools.LilFont;
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
            Text = Text.Remove(SelectIndex_Start, SelectIndex_End - SelectIndex_Start);
            Unselect();
        }

        void UpdateSelectQuad()
        {
            float width = MyText.GetWorldWidth(Text.Substring(SelectIndex_Start, SelectIndex_End - SelectIndex_Start));
            float pos = MyText.GetWorldWidth(Text.Substring(0, SelectIndex_Start));

            SelectQuad.Size = new Vector2(width / 2, SelectQuad.Size.Y);
            SelectQuad.Left = MyText.Pos.X + pos;
        }

        protected override EzText MakeText(string text, bool centered, EzFont font)
        {
            EzText eztext = new EzText(text, font, 1000, centered, true, .575f);
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
            // Position the caret at the end of the text
            Caret.X = MyText.Pos.X + MyText.GetWorldWidth();

            // If we're selecting move the caret one character to the left
            if (SelectIndex_End - SelectIndex_Start > 0)
                Caret.X -= MyText.GetWorldWidth(" ");

            if (DoRecenter)
                MyText.Pos = new Vector2(-MyText.GetWorldWidth() / 2, 0);
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
            if (SelectIndex_End - SelectIndex_Start > 0)
                DeleteSelected();
            else
            {
                // Otherwise delete one character
                if (Length == 0) return;

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
            Backdrop.TextureName = "Score\\Score Screen_grey";

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
