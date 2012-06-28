#if PC_VERSION
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System;

using KeyboardHandler;

namespace CloudberryKingdom
{
    public class GUI_EnterName : GUI_TextBox
    {
        public GUI_EnterName() : base(PlayerManager.DefaultName, Vector2.Zero)
        {
            EzText Text = new EzText("New high score!", Tools.MonoFont);

            Text.Pos =
                new Vector2(-579.365f, 253.9681f);
            Text.Scale *= 1.1f;
            MyPile.Add(Text);
        }

        protected override void Enter()
        {
            if (Length > 0)
                PlayerManager.DefaultName = Text;
            else
                Text = PlayerManager.DefaultName;

            base.Enter();
        }
    }

    public class GUI_TextBox : GUI_Text
    {
        /// <summary>
        /// Event handler. Activated when the the user presses Enter while the textbox has focus.
        /// </summary>
        public event Action OnEnter;

        public override void OnAdd()
        {
            base.OnAdd();
            
            GetFocus();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            OnEnter = null;
            ReleaseFocus();
        }

        bool HasFocus = false;
        public void GetFocus()
        {
            HasFocus = true;

            KeyboardExtension.FreezeInput();

            EventInput.CharEntered += CharEntered;
            EventInput.KeyDown += KeyDown;
        }

        public void ReleaseFocus()
        {
            if (!HasFocus) return;
            HasFocus = false;

            KeyboardExtension.UnfreezeInput();

            EventInput.CharEntered -= CharEntered;
            EventInput.KeyDown -= KeyDown;
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

                GamepadInteract();
            }
            else
                // Don't draw the caret when we don't have focus
                Caret.Show = false;
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
            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed) { Backspace(); return; }
            if (ButtonCheck.State(ControllerButtons.X, -1).Pressed) { Enter(); return; }

            var dir = ButtonCheck.GetDir(-1);

            if (Tools.TheGame.DrawCount % 7 == 0 && Math.Abs(dir.Y) > .5)
            {
                if (dir.Y > 0) Text = Text.Substring(0, Length - 1) + IncrChar(c);
                if (dir.Y < 0) Text = Text.Substring(0, Length - 1) + DecrChar(c);

                Recenter();
            }
        }

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
        public GUI_TextBox(string InitialText, Vector2 pos) : base(InitialText, pos, false)
        {
            FixedToCamera = true;
            NoPosMod = true;

            // Backdrop
            Backdrop = new QuadClass(null, true, false);
            Backdrop.TextureName = "score screen";
            Backdrop.Size = new Vector2(640.4763f, 138.0953f);

            MyText.Pos = new Vector2(-522.2222f, 23.80954f);
            MyPile.Insert(0, Backdrop);

            // Caret
            Caret = new EzText("_", Tools.MonoFont, 1000, false, true, .575f);
            Caret.MyFloatColor = Color.Black.ToVector4();
            Caret.Pos = MyText.Pos;

            MyPile.Add(Caret);

            // Select quad
            SelectQuad = new QuadClass(null, true, false);
            SelectQuad.TextureName = "White";
            SelectQuad.Quad.SetColor(new Color(255, 255, 255, 125));
            SelectQuad.Size = new Vector2(100f, 100f);
            SelectQuad.Layer = 1;

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

        protected override EzText MakeText(string text, bool centered)
        {
            EzText eztext = new EzText(text, Tools.MonoFont, 1000, centered, true, .575f);
            eztext.MyFloatColor = Color.Black.ToVector4();
            
            return eztext;
        }

        public int MaxLength = 18;
        public bool LimitLength = true;
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

        void KeyDown(object o, KeyEventArgs e)
        {
            if (!Active) return;

            if (e.KeyCode == Keys.Back) Backspace();
            if (e.KeyCode == Keys.Enter) Enter();
        }

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

        protected virtual void Enter()
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
#endif