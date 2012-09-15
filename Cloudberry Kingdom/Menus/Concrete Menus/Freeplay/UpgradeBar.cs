using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class UpgradeBar : ViewReadWrite
    {
        public override string[] GetViewables()
        {
            return new string[] { "IconOffset" };
        }

        QuadClass Bar, NullBar;
        public ObjectIcon Icon;

        int _NumBars = 0;
        int MaxBars = 4;
        public int NumBars
        {
            get { return _NumBars; }
            set
            {
                _NumBars = value;
                if (_NumBars > MaxBars) _NumBars = MaxBars;
                if (_NumBars < 0) _NumBars = 0;

                if (_NumBars == value)
                    StartJiggle();
            }
        }

        FancyVector2 FancyPos;
        public Vector2 Pos
        {
            get
            {
                return FancyPos.RelVal;
            }
            set
            {
                FancyPos.RelVal = value;
            }
        }

        bool Jiggle;
        int JiggleCount;
        void StartJiggle()
        {
            Jiggle = true;
            JiggleCount = 0;
        }

        float[] JiggleScale = { 1.25f, .93f, 1.04f };
        void JigglePhsx()
        {
            // Treat negative counts as a delay
            if (JiggleCount < 0)
            {
                JiggleCount++;
                return;
            }

            if (JiggleCount == 6 * JiggleScale.Length)
            {
                Bar.Size = BarSize;
                Jiggle = false;

                return;
            }

            Bar.Size = .5f * (Bar.Size + JiggleScale[JiggleCount / 6] * BarSize);

            JiggleCount++;
        }

        Vector2 BarSize;
        public Upgrade MyUpgradeType;
        public UpgradeBar(Upgrade upgrade)
        {
            MyUpgradeType = upgrade;

            FancyPos = new FancyVector2();

            Icon = ObjectIcon.CreateIcon(upgrade);
            Icon.FancyPos.SetCenter(FancyPos, true);

            //NumBars = MyLevel.Rnd.RndInt(1, 4);
            NumBars = 0;

            Bar = new QuadClass();
            Bar.SetToDefault();
            Bar.TextureName = "box4";
            Bar.ScaleYToMatchRatio(180);
            BarSize = Bar.Size;
            Bar.Quad.SetColor(Icon.BarColor);

            Bar.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            Bar.ShadowOffset = new Vector2(12, 12);

            NullBar = new QuadClass();
            NullBar.SetToDefault();
            NullBar.TextureName = "BoxZero2";
            NullBar.ScaleYToMatchRatio(180);
            NullBar.Quad.SetColor(BarColor[0]);

            NullBar.Shadow = true;
            NullBar.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            NullBar.ShadowOffset = new Vector2(12, 12);

            Height = 106;
            Icon.Pos = new Vector2(126, -94);
        }

#if PC_VERSION
        public void MouseInteract()
        {
            if (Tools.TheGame.MouseInUse)
                if (ButtonCheck.State(ControllerButtons.A, -1).Down &&
                    !ButtonCheck.KeyboardGo())
                {
                    Vector2 mouse = Tools.MouseGUIPos(MyCameraZoom);

                    NumBars = (int)(mouse.Y + 1.55f * Height - Pos.Y) / (int)Height;
                    JiggleCount = -4;
                }
        }
#endif

        public static Color[] BarColor = { Color.LightBlue, Color.Lime, Color.Orange, Color.OrangeRed, Color.Red };

        Vector2 _MyCameraZoom = Vector2.One;
        /// <summary>
        /// The value of the camera zoom the last time this EzText was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        public float Height;
        public void Draw(bool Selected, QuadClass Circle, int Layer)
        {
            MyCameraZoom = Tools.CurCamera.Zoom;

            if (Jiggle && Layer == 0)
                JigglePhsx();

            Bar.Quad.SetColor(BarColor[NumBars]);

            if (NumBars == 0)
            {
                NullBar.Pos = Pos;
                if (Layer == 0) NullBar.DrawShadow();
                if (Layer == 1) NullBar.Draw();
            }
            else
            {
                if (Layer == 0)
                for (int i = 0; i < NumBars; i++)
                {
                    Bar.Pos = Pos + new Vector2(0, Height * i);
                    Bar.DrawShadow();
                }

                if (Layer == 1)
                for (int i = 0; i < NumBars; i++)
                {
                    Bar.Pos = Pos + new Vector2(0, Height * i);
                    Bar.Draw();
                }
            }

            if (Layer == 2)
            {
                Icon.Draw(Selected);

                if (Selected)
                {
                    Circle.Pos = Pos + new Vector2(-115, -82);
                    Circle.Draw();
                }
            }
        }

        int DelayIncr;
        public void PhsxStep(int control)
        {
            // Increase/Decrease
#if WINDOWS
            if (ButtonCheck.State(Keys.Z).Pressed) NumBars++;
            if (ButtonCheck.State(Keys.X).Pressed) NumBars--;
#endif

            float Dir = Vector2.Dot(new Vector2(1, 1), ButtonCheck.State(ControllerButtons.RJ, control, false).Dir);
            if (Math.Abs(Dir) > .5f && DelayIncr <= 0)
            {
                NumBars += Math.Sign(Dir);
                DelayIncr = 7;
            }
            else
                DelayIncr--;

            if (ButtonCheck.State(ControllerButtons.A, control, false).Pressed)
                NumBars++;
            if (ButtonCheck.State(ControllerButtons.X, control, false).Pressed)
                NumBars--;

            if (ButtonCheck.State(ControllerButtons.LS, control).Pressed)
                NumBars = 0;
            if (ButtonCheck.State(ControllerButtons.RS, control).Pressed)
                NumBars = MaxBars;
        }
    }
}