using System;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_Timer_Base : GUI_Panel
    {
        int _Time = 5000;
        
        /// <summary>
        /// The time in number of frames
        /// </summary> 
        public int Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                UpdateTimerText();
            }
        }

        public int Minutes
        {
            get
            {
                return (int)(Time / (60 * 62));
            }
        }

        public int Seconds
        {
            get
            {
                return (int)((Time - 60 * 62 * Minutes) / 62);
            }
        }

        public int Milliseconds
        {
            get
            {
                float Remainder = Time - 60 * 62 * Minutes - 62 * Seconds;

                return (int)(100 * Remainder / 62f);
            }
        }

        StringBuilder MyString = new StringBuilder(50, 50);

        /// <summary>
        /// Return a string representation of the time
        /// </summary>
        /// <returns></returns>
        public StringBuilder BuildString()
        {
            MyString.Length = 0;

            if (Minutes > 0)
            {
                MyString.Add(Minutes, 1);
                MyString.Append(':');
                MyString.Add(Seconds, 2);
            }
            else
            {
                MyString.Add(Seconds, 1);
                MyString.Append(':');
                MyString.Add(Milliseconds, 2);
            }

            return MyString;
        }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();

            if (!AddedOnce)
            {
                Hide();
                
                Show();
            }

            AddedOnce = true;
        }

        public override void Hide()
        {
            base.Hide();
            SlideOut(PresetPos.Top, 0);
        }

        public float Intensity = 1;
        public override void Show()
        {
            base.Show();
            SlideIn();
            MyPile.BubbleUp(false, 5, Intensity);
        }

        public void ShowInstant()
        {
            base.Show();
            SlideIn(0);
            MyPile.BubbleUp(false, 0, Intensity);
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();            
        }

        public Vector2 ApparentPos
        {
            get { return TimerText.FancyPos.AbsVal + TimerText.GetWorldSize() / 2; }
        }

        EzText TimerText;
        void UpdateTimerText()
        {
            TimerText.SubstituteText(BuildString());
        }
        
        public GUI_Timer_Base()
        {
            MyPile = new DrawPile();
            EnsureFancy();

            MyPile.Pos = new Vector2(-90, 865);
            SlideInLength = 0;

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            EzFont font;
            float scale;
            Color c, o;

            if (false)
            {
                font = Resources.Font_Grobold42_2;
                scale = .55f;
                c = Color.White;
                o = Color.Black;
            }
            else
            {
                font = Resources.Font_Grobold42;
                scale = .75f;
                c = new Color(228, 0, 69);
                o = Color.White;
            }

            TimerText = new EzText(BuildString().ToString(), font, 450, true, true);
            TimerText.Scale = scale;
            TimerText.MyFloatColor = c.ToVector4();
            TimerText.OutlineColor = o.ToVector4();

            MyPile.Add(TimerText);
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        public Multicaster_1<GUI_Timer_Base> OnTimeExpired = new Multicaster_1<GUI_Timer_Base>();

        /// <summary>
        /// When true the timer will continue to count down even when every player is dead.
        /// </summary>
        public bool CountDownWhileDead = false;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
            if (Core.Released || MyGame == null) return;

            if (MyGame.HasBeenCompleted) return;

            if (MyGame.SoftPause) return;
            if (Hid || !Active) return;

            if (Time == 0)
            {
                if (OnTimeExpired != null)
                    OnTimeExpired.Apply(this);

                return;
            }

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            if (CountDownWhileDead || !PlayerManager.AllDead())
                Time--;
        }
    }
}