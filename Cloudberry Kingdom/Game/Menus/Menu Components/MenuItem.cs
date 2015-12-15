using System;

using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public delegate void MenuItemGo(MenuItem item);
    public class MenuItem : ViewReadWrite
    {
        public int Code = 0;
        public string Name = "";

        public bool UnaffectedByScroll = false;

        /// <summary>
        /// An associated object to store extra info.
        /// </summary>
        public object MyObject = null;
        public int MyInt = 0;

        public override string[] GetViewables()
        {
            return new string[] { "Pos", "SelectedPos", "MyText", "MySelectedText", "!MyMenu", "SelectIconOffset" };
        }

        public override void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
#if WINDOWS && DEBUG
			if (Tools.CntrlDown() && ShiftDown)
				return;

            if (ShiftDown)
            {
                var scale = (shift.X + shift.Y) * .00003f;
                MyText.Scale += scale;
                MySelectedText.Scale += scale;
            }
            else
                SetPos += shift;
#endif
        }

        public virtual string ToCode(string suffix)
        {
#if WINDOWS && DEBUG
            string SelectedPosStr = "";
            if (SelectedPos != Pos)
            {
                var spos = SelectedPos;
                if (spos.Y == Pos.Y) spos.Y = -1;

                SelectedPosStr = string.Format("_item.SetSelectedPos({0});", Tools.ToCode(spos));
            }

            return string.Format("_item = {0}FindItemByName(\"{1}\"); if (_item != null) {{ _item.SetPos = {2}; _item.MyText.Scale = {3}f; _item.MySelectedText.Scale = {4}f; _item.SelectIconOffset = {5}; {6} }}", suffix, Name, Tools.ToCode(Pos), MyText.Scale, MySelectedText.Scale, Tools.ToCode(SelectIconOffset), SelectedPosStr);
#else
            return "";
#endif
        }

        public MenuItem Clone()
        {
            MenuItem clone = new MenuItem(MyText.Clone(), MySelectedText.Clone());
            if (Icon != null) { clone.Icon = Icon.Clone(); clone.Icon.FancyPos.SetCenter(clone.FancyPos); }
            return clone;
        }

		public MenuItem Clone(string Text)
		{
			MenuItem clone = new MenuItem(MyText.Clone(Text), MySelectedText.Clone(Text));
			if (Icon != null) { clone.Icon = Icon.Clone(); clone.Icon.FancyPos.SetCenter(clone.FancyPos); }
			return clone;
		}

        public Vector2 PosOffset, SelectIconOffset;

        public EzSound SelectSound, SlideSound, ListScrollSound;

        public Menu MyMenu;

        public FancyVector2 FancyPos = new FancyVector2();
        public Vector2 Pos, SelectedPos;
        public bool CustomSelectedPos;

        public void SetSelectedPos(Vector2 selectedpos)
        {
            SelectedPos = selectedpos;

            if (SelectedPos.Y == -1)
                SelectedPos.Y = Pos.Y;
        }

        public Vector2 SetPos { get { return Pos; } set { Pos = SelectedPos = value; } }

        /// <summary>
        /// Whether the item oscillates when selected.
        /// </summary>
        public bool SelectionOscillate = true;
        
        public bool AlwaysDrawAsSelected = false;
        public OscillateParams MyOscillateParams;

        public void Shift(Vector2 shift)
        {
            Pos += shift;
            SelectedPos += shift;
        }

        /// <summary>
        /// If true collision detection is performed against the item's icon
        /// </summary>
        public bool ColWithIcon = true;

        public ObjectIcon Icon;
        public void SetIcon(ObjectIcon Icon)
        {
            this.Icon = Icon;
            Icon.FancyPos.SetCenter(FancyPos);
        }

        public void SubstituteText(Localization.Words text)
        {
            MyText.SubstituteText(text);
            MySelectedText.SubstituteText(text);
        }

        public void SubstituteText(string text)
        {
            MyText.SubstituteText(text);
            MySelectedText.SubstituteText(text);
        }

        public int TextWidth;
        public EzText MyText, MySelectedText;

        public MenuItemGo OnClick;

        MenuItemGo _Go;
        public MenuItemGo Go
        {
            set
            {
                _Go = value;
                if (_Go != null)
                    OverrideA = true;
            }
            get { return _Go; }
        }

        public Action AdditionalOnSelect;

        public int Control;

        public bool Selectable, FadedOut;
        protected bool Selected;

        /// <summary>
        /// The previous value of Selected
        /// </summary>
        bool PrevSelected;

        bool _OverrideA;
        public bool OverrideA
        {
            get { return _OverrideA; }
            set
            {
                _OverrideA = value;
                //if (value && this is MenuSliderBase)
                //    Tools.Write("");
            }
        }

        public Vector4 MySelectedColor, MyColor;
        public EzFont MySelectedFont, MyFont;

        public string MyString
        {
            get
            {
                if (MyText == null)
                    return "";
                else
                    return MyText.MyString;
            }
        }

        public int MyDrawLayer;

        //public bool FixedToCamera;

        public virtual void Release()
        {
            MyObject = null;
            MyMenu = null;

            if (MyText != null) MyText.Release(); MyText = null;
            if (MySelectedText != null) MySelectedText.Release(); MySelectedText = null;

			SelectSound = null; SlideSound = null; ListScrollSound = null;
			if (FancyPos != null) FancyPos.Release(); FancyPos = null;
			Icon = null;
			OnClick = null;
			AdditionalOnSelect = null;
			MySelectedFont = null; MyFont = null;

            Go = null;
        }

        public void SetFade(bool Fade)
        {
            if (FadedOut != Fade)
            {
                if (Fade)
                    FadeOut();
                else
                    FadeIn();

                FadedOut = Fade;
            }            
        }

        public virtual void FadeOut()
        {
            MyText.MyFloatColor.W = .3f;
            MyText.OutlineColor.W = .3f;
        }

        public virtual void FadeIn()
        {
            MyText.MyFloatColor.W = 1f;
            MyText.OutlineColor.W = 1f;
        }

        public MenuItem()
        {
            SetToDefaultColors();

            OverrideA = false;
        }

        void SetToDefaultColors()
        {
            MySelectedColor = new Color(255, 255, 255, 255).ToVector4();
            MyColor = new Color(220, 220, 220, 200).ToVector4();

            MySelectedFont = Resources.Font_Grobold42;
            MyFont = Resources.Font_Grobold42;
        }

        public virtual Vector2 Size()
        {
            return new Vector2(Width(), Height());
        }

        public virtual float Height()
        {
            if (MyText == null) return 50;
            else return MyText.GetWorldHeight();
        }

        public virtual float Width()
        {
            if (MyText == null) return 50;
            else return
                Math.Max(
                MyText.GetWorldWidth(),
                MySelectedText.GetWorldWidth());
        }


		public string ExpandString = null;

        /// <summary>
        /// Initialize a new MenuItem.
        /// </summary>
        public MenuItem(EzText Text)
        {
            Init(Text, Text.Clone());
        }
        public MenuItem(EzText Text, string Name)
        {
            Init(Text, Text.Clone());
            this.Name = Name;
        }        
        public MenuItem(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
        }

        public void ScaleText(float scale)
        {
            MyText.Scale *= scale;
            MySelectedText.Scale *= scale;
        }

        /// <summary>
        /// The new style initialization function. User provides the EzText for both selected and unselected.
        /// </summary>
        protected virtual void Init(EzText Text, EzText SelectedText)
        {
            MyOscillateParams.Set(2.05f, 1.0295f, .045f);

            Selectable = true;

            MyText = Text;
            MySelectedText = SelectedText;
        }

        public bool FixedToCamera
        {
            set
            {
                if (MyText != null)
                    MyText.FixedToCamera = value;
                if (MySelectedText != null)
                    MySelectedText.FixedToCamera = value;
            }
            get
            {
                if (MyText != null)
                    return MyText.FixedToCamera;
                else
                    return false;
            }
        }

        public void SetTextSelection(bool Selected)
        {
            if (!Selectable)
                Selected = false;
        }

        public void UpdatePos()
        {
            MyText._Pos = Pos + PosOffset;
            MySelectedText._Pos = SelectedPos + PosOffset;
        }

        public void DrawBackdrop(bool Selected)
        {
            if (Selected)
            {
                if (MySelectedText != null)
                    MySelectedText._Pos = SelectedPos + PosOffset;
            }
            else
            {
                if (MyText != null)
                    MyText._Pos = Pos + PosOffset;
            }
        }

        /// <summary>
        /// True when the item is on the screen.
        /// </summary>
        protected bool OnScreen
        {
            get
            {
                if (FixedToCamera) return true;

                if (MyText.Pos.Y > Tools.CurLevel.MainCamera.TR.Y + 500)
                    return false;
                if (MyText.Pos.Y < Tools.CurLevel.MainCamera.BL.Y - 500)
                    return false;
                return true;
            }
        }

        public void DrawText(Camera cam, bool Selected)
        {
            if (!OnScreen) return;

            if (MyMenu.CurDrawLayer != MyDrawLayer || !Show)
                return;

            GrayOut();

            if (Selected
#if PC
                && (ButtonCheck.MouseInUse || !MyMenu.MouseOnly)
#endif
                || AlwaysDrawAsSelected && MyOscillateParams.MyType != OscillateParams.Type.Oscillate)
            {
                // The selected text of the current menu item may not ever have been drawn,
                // so update its CameraZoom manually
                MySelectedText.MyCameraZoom = MyCameraZoom;

                // Oscillate
                float HoldScale = MySelectedText.Scale;
                Vector2 HoldPos = MySelectedText.Pos;
                Vector2 HoldShadowOffset = MySelectedText.ShadowOffset;
                if (SelectionOscillate)
                {
                    float scale = MyOscillateParams.GetScale();

                    Vector2 PosShift = Vector2.Zero;
                    if (!MySelectedText.Centered)
                    {
                        PosShift = new Vector2(.5f * (scale - 1f) * MySelectedText.GetWorldWidth(), 0);
                        MySelectedText.Pos -= PosShift;
                    }

                    MySelectedText.Scale *= scale;
                    MySelectedText.ShadowScale = 1f / (.9f * (scale - 1) + 1);
                    MySelectedText.ShadowOffset *= 15f * (scale - 1) + 1;
                    MySelectedText.ShadowOffset -= .7f * PosShift;
                    MySelectedText.ShadowOffset.X = .5f * (MySelectedText.ShadowOffset.X + HoldShadowOffset.X - PosShift.X);
                    //MySelectedText.ShadowOffset.Y *= 6f * (scale - 1) + 1;
                }

                // Draw the selected text
                MySelectedText.Draw(cam, false);

                if (SelectionOscillate)
                {
                    MySelectedText.Scale = HoldScale;
                    MySelectedText.Pos = HoldPos;
                    MySelectedText.ShadowOffset = HoldShadowOffset;
                }

                PrevSelected = true;
            }
            else
            {
                MyOscillateParams.Reset();

                // Draw the unselected text
                MyText.Draw(cam, false);

                PrevSelected = false;
            }

            DeGrayOut();
        }

        public bool GrayOutOnUnselectable = false;
        public void GrayOut()
        {
            if (!Selectable && GrayOutOnUnselectable) DoGrayOut();
        }

        public void DeGrayOut()
        {
            if (!Selectable && GrayOutOnUnselectable) DoDeGrayOut();
        }

        public virtual void DoGrayOut()
        {
            MyText.MyFloatColor.W = .5f;
            if (Icon != null && Icon is PictureIcon) ((PictureIcon)Icon).IconQuad.Alpha = .5f;
        }

        public virtual void DoDeGrayOut()
        {
            MyText.MyFloatColor.W = 1f;
            if (Icon != null && Icon is PictureIcon) ((PictureIcon)Icon).IconQuad.Alpha = 1f;
        }

        public bool Show = true;

        public bool Include
        {
            set { Show = Selectable = value; }
        }

        /// <summary>
        /// If true the MenuItem jiggles when it is activated -- when the user presses (A)
        /// </summary>
        public bool JiggleOnGo = true;

        public void OnSelect()
        {
            if (AdditionalOnSelect != null)
                AdditionalOnSelect();

            // Stop jiggling
            if (JiggleOnGo)
            {
                MyOscillateParams.SetType(OscillateParams.Type.Oscillate);
                if (Icon != null) Icon.MyOscillateParams.SetType(OscillateParams.Type.Oscillate);
            }
        }

        Vector2 _MyCameraZoom = Vector2.One;
        /// <summary>
        /// The value of the camera zoom the last time this EzText was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        public void Draw()
        {
            this.Draw(false, Tools.CurCamera, false);
            this.Draw(true, Tools.CurCamera, false);
        }

        public bool DrawBase(bool Text, Camera cam, bool Selected)
        {
            MyCameraZoom = cam.Zoom;

            if (MyDrawLayer != MyMenu.CurDrawLayer || !Show)
                return true;

            if (!Selectable)
                Selected = false;

            if (FancyPos != null)
            {
                FancyPos.RelVal = Pos + PosOffset;
                FancyPos.Update();
            }

            // If just selected perfrom the OnSelect callback
            if (Selected != this.Selected && Selected)
            {
                OnSelect();
            }
            this.Selected = Selected;

            return false;
        }

        public virtual void Draw(bool Text, Camera cam, bool Selected)
        {
            if (DrawBase(Text, cam, Selected)) return;

            GrayOut();

            SetTextSelection(Selected);

            if (Text)
            {
                if (Selected || !CustomSelectedPos)
                {
                    MyText._Pos = Pos + PosOffset;
                    MySelectedText._Pos = SelectedPos + PosOffset;
                }
                else
                {
                    MyText._Pos = SelectedPos + PosOffset;
                    MySelectedText._Pos = SelectedPos + PosOffset;
                }

                DrawText(cam, Selected);
            }
            else
            {
                if (Icon != null)
                    Icon.Draw(Selected);
            }

            DeGrayOut();
        }

#if WINDOWS
        public virtual bool HitTest()
        {
            return HitTest(Tools.MouseGUIPos(MyCameraZoom));
        }

        public Vector2 Padding = new Vector2(-22, -7); //new Vector2(150, 0);
        public virtual bool HitTest(Vector2 pos)
        {
            return HitTest(pos, Vector2.Zero);
        }

		public bool MouseSelectable = true;
		public bool KeyboardSelectable = true;
        public virtual bool HitTest(Vector2 pos, Vector2 padding)
        {
			if (!MouseSelectable || !Selectable) return false;

            return ColWithIcon && Icon != null && Icon.HitTest(pos) ||
                MyText.HitTest(pos, Padding + padding);
        }
#endif

        public static int ActivatingPlayer = -1;
        public static PlayerIndex ActivatingPlayerIndex()
        {
            int p = MenuItem.ActivatingPlayer;
            if (p < 0) return PlayerIndex.One;

            return (PlayerIndex)p;
        }
        public static PlayerData GetActivatingPlayerData()
        {
            int p = MenuItem.ActivatingPlayer;
            if (p < 0)
                p = PlayerManager.FirstPlayer;
            return PlayerManager.Get(p);
        }


        /// <summary>
        /// Called once per frame to perform any behavior.
        /// </summary>
        /// <param name="Selected">Whether this item is selected in the menu.</param>
        public virtual void PhsxStep(bool Selected)
        {
            // When a select jiggle animation is done, go back to oscillating
            if (MyOscillateParams.MyType == OscillateParams.Type.Jiggle &&
                MyOscillateParams.Done)
                MyOscillateParams.SetType(OscillateParams.Type.Oscillate);

            FancyPos.RelVal = Pos + PosOffset;
            FancyPos.Update();

            if (MyMenu != null) { Control = MyMenu.Control; FixedToCamera = MyMenu.FixedToCamera; }

#if PC
            // Mouse interact
            bool SelectThis = false;
			if (ButtonCheck.MouseInUse && (Tools.MouseNotDown() || MyMenu.SlipSelect)
				&& !MyMenu.HasSelectedThisStep && Selectable
				&& (MyMenu.UseMouseAndKeyboard || Control == CoreKeyboard.KeyboardPlayerNumber))
			{
				if (HitTest())
				{
					SelectThis = true;
					MyMenu.HasSelectedThisStep = true;
				}
			}
#endif

            if (!Selected)
            {
#if PC
                // Mouse interact
                if (SelectThis)
                    MyMenu.SelectItem(this);
#endif
                return;
            }

            bool Activate = false;

			ButtonData data = ButtonCheck.GetState(ControllerButtons.A, Control, false, true, MyMenu.UseMouseAndKeyboard);
            if (data.Pressed)
            {
                ActivatingPlayer = data.PressingPlayer;
                Activate = true;
            }

            // Don't activate the item if it isn't being drawn as selected
            if (MyMenu.NoneSelected)
                Activate = false;

#if PC
            // Don't activate the item if the menu is mouse only and the mouse isn't in use
            if (MyMenu.MouseOnly && !ButtonCheck.MouseInUse)
                Activate = false;

            // Don't activate the itme if the mouse is in use and isn't over the item
            if (ButtonCheck.MouseInUse && !HitTest())
                Activate = false;
#else
            if (MyMenu.MouseOnly)
                Activate = false;
#endif

            // Mouse down over the item
#if PC
            if (OnClick != null && ButtonCheck.MouseInUse && Tools.CurMouseDown() && HitTest())
                OnClick(this);
#endif
            // Go function
            if (Activate && Go != null)
            {
                DoActivationAnimation();

                if (SelectSound != null)
                    SelectSound.Play();
                Go(this);
                MyMenu.LastActivatedItem = MyMenu.Items.IndexOf(this);
                ButtonCheck.PreventInput();
            }
        }

        /// <summary>
        /// Jiggle the item. To be called when it is activated
        /// </summary>
        public void DoActivationAnimation()
        {
            if (JiggleOnGo)
            {
                MyOscillateParams.Reset();
                MyOscillateParams.SetType(OscillateParams.Type.Jiggle);
                if (Icon != null) Icon.MyOscillateParams.SetType(OscillateParams.Type.Jiggle);
            }
        }
    }
}