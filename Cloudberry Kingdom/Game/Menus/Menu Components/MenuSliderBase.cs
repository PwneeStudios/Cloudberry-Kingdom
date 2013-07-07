using System;

using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    /// <summary>
    /// The base class for all derived MenuSlider classes
    /// </summary>
    public class MenuSliderBase : MenuItem
    {
        /// <summary>
        /// Called when the user explicitly manipulates the slider.
        /// </summary>
        public Action OnSlide;

        /// <summary>
        /// Called whenever the slider value is set.
        /// </summary>
        public Action OnSetValue;

        public bool IsMaxed
        {
            get { return MyFloat.Val == MyFloat.MaxVal; }
        }

        protected void Slide()
        {
            if (OnSlide != null)
                OnSlide();
        }

        public int DelayToSlideSound = Menu.DefaultMenuInfo.Menu_Slide_SoundDelay;
        public int DelayToSlideSoundCount = 0;

        public virtual Vector2 BL { get { return Vector2.Zero; } }
        public virtual Vector2 TR { get { return Vector2.Zero; } }
        public virtual Vector2 Slider_TR { get { return Vector2.Zero; } }

        public float Val
        {
            get { return MyFloat.Val; }
            set
            {
                MyFloat.Val = value;
                if (OnSetValue != null) OnSetValue();
            }
        }

        public WrappedFloat _MyFloat;
        public WrappedFloat MyFloat
        {
            get { return _MyFloat; }
            set
            {
                _MyFloat = value;
                _MyFloat.SetCallback = SetCallback;
                SetCallback();
            }
        }

        public float InitialSlideSpeed = 1f / 55f;
        public float MaxSlideSpeed = 3.5f / 55f;
        public float Acceleration = .03f;
        float Speed = 0;

        public bool Discrete;
        const int SelectDelay = 8;
        int DelayCount;

        public MenuSliderBase()
        {
            EzText NoText = new EzText("", Resources.Font_Grobold42);
            Init(NoText, NoText.Clone());
            InitializeSlider();
        }

        public MenuSliderBase(EzText Text)
        {
            Init(Text, Text.Clone());
            InitializeSlider();
        }
        public MenuSliderBase(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
            InitializeSlider();
        }

        string BaseString;
        protected virtual void InitializeSlider()
        {
            BaseString = MyText.MyString;
            SelectionOscillate = false;

            OverrideA = false;
        }

        public virtual void SetCallback()
        {
            if (ShowText)
                UpdateText();

            if (OnSetValue != null)
                OnSetValue();
        }

        public override void Release()
        {
            base.Release();

            MyFloat.Release();
        }

#if PC_VERSION
        public Vector2 BL_HitPadding = Vector2.Zero, TR_HitPadding = Vector2.Zero;
        public override bool HitTest(Vector2 pos, Vector2 padding)
        {
            CalcEndPoints();
            
            bool Hit = Phsx.Inside(pos, Start - new Vector2(0, 50) - BL_HitPadding,
                                        End + new Vector2(0, 50) + TR_HitPadding, padding);
            Hit |= base.HitTest(pos, padding);

            return Hit;
        }
#endif

        protected Vector2 Start, End;
        protected virtual void CalcEndPoints()
        {
            Start = End = Vector2.Zero;
        }

        public override string ToString()
        {
            return BaseString + ((int)MyFloat.Val).ToString();
        }

        bool ShowText = false;
        void UpdateText()
        {
            MyText.SubstituteText(ToString());
            MySelectedText.SubstituteText(ToString());
        }

        public void SetToShowText()
        {
            ShowText = true;
            UpdateText();
        }

        public bool Inverted = false;
#if PC_VERSION
        protected virtual void PC_OnLeftMouseDown()
        {
            if (!ButtonCheck.MouseInUse) return;

            MyMenu.HasSelectedThisStep = true;

            CalcEndPoints();
            Vector2 MousePos = Tools.MouseGUIPos(MyCameraZoom);

            Vector2 dif, tangent;

            if (Inverted)
            {
                dif = MousePos - End;
                tangent = Start - End;
            }
            else
            {
                dif = MousePos - Start;
                tangent = End - Start;
            }

            float length = tangent.Length();
            float t = Vector2.Dot(dif, tangent) / (length * length);

            t = CoreMath.Restrict(0, 1, t);

            MyFloat.Val = (1 - t) * MyFloat.MinVal + t * MyFloat.MaxVal;

            Slide();
        }
#endif

        Vector2 PrevDir;
        public override void PhsxStep(bool Selected)
        {
            base.PhsxStep(Selected);
            
            if (!Selected) return;

            if (Speed < InitialSlideSpeed)
                Speed = InitialSlideSpeed;

            float CurVal = MyFloat.Val;

#if PC_VERSION
            if (ButtonCheck.State(ControllerButtons.A, Control).Down &&
                !ButtonCheck.KeyboardGo())
            {
                PC_OnLeftMouseDown();
            }
#endif

            // Quick slide
            if (ButtonCheck.State(ControllerButtons.LS, Control).Pressed)
            {
                MyFloat.Val = MyFloat.MinVal;
                Slide();
            }
            if (ButtonCheck.State(ControllerButtons.RS, Control).Pressed)
            {
                MyFloat.Val = MyFloat.MaxVal;
                Slide();
            }

            Vector2 Dir = Vector2.Zero;
            if (Control < 0)
                Dir = ButtonCheck.GetMaxDir();
            else
                Dir = ButtonCheck.GetDir(Control);

            if (Discrete)
            {
                if (DelayCount > 0)
                    DelayCount--;
                else
                {
                    float Sensitivty = ButtonCheck.ThresholdSensitivity;
                    if (Math.Abs(Dir.X) > Sensitivty)
                    {
                        MyFloat.Val = (int)MyFloat.Val + Math.Sign(Dir.X);
                        Slide();

                        DelayCount = SelectDelay;
                    }
                }
            }
            else
            {
                if (Math.Abs(Dir.X) > .5f)
                {
                    MyFloat.Val += Dir.X * Speed;
                    Slide();

                    Speed = Acceleration * MaxSlideSpeed + (1 - Acceleration) * Speed;
                }

                if (Math.Abs(Dir.X) < .5f || Math.Sign(Dir.X) != Math.Sign(PrevDir.X))
                    Speed = InitialSlideSpeed;
            }

            if (CurVal != MyFloat.Val)
            {
                if (DelayToSlideSoundCount > DelayToSlideSound)
                {
                    DelayToSlideSoundCount = 0;
                    if (SlideSound != null)
                        SlideSound.Play();
                }
            }

            DelayToSlideSoundCount++;

            PrevDir = Dir;
        }
    }
}