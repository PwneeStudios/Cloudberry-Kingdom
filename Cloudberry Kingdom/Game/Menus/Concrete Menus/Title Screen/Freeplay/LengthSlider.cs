
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class LengthSlider : MenuSliderBase
    {
        public LengthSlider() { }
        public LengthSlider(EzText Text)
        {
            Init(Text, Text.Clone());
            InitializeSlider();
        }
        public LengthSlider(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
            InitializeSlider();
        }

        public int PerceivedMin = 0;

        ProgressBar LengthBar;
        protected override void InitializeSlider()
        {
            base.InitializeSlider();

#if PC
            BL_HitPadding = new Vector2(200, 30);
#endif
            LengthBar = new ProgressBar("Length", "LengthBack", 850);
            LengthBar.MyType = ProgressBar.BarType.Reveal;

            LengthBar.Outline.Shadow = false;
			//LengthBar.MyPile.MyOscillateParams.Set(2f, .99f, .05f);
			LengthBar.MyPile.MyOscillateParams.Set(1.85f, .99f, .0205f);

            MyFloat = new WrappedFloat(8000, 3000, 15000);
            PerceivedMin = 500;
            InitialSlideSpeed = 120;
            MaxSlideSpeed = 550;
        }

        public override Vector2 BL { get { return LengthBar.Full_BL; } }
        public override Vector2 TR { get { return LengthBar.Full_TR; } }
        public override Vector2 Slider_TR { get { return LengthBar.Current_TR; } }

        public override void SetCallback()
        {
            base.SetCallback();

            LengthBar.SetPercent(100f * (MyFloat.Val - PerceivedMin) / (MyFloat.MaxVal - PerceivedMin));
        }

        protected override void CalcEndPoints()
        {
            float min_ratio = (MyFloat.MinVal - PerceivedMin) / (MyFloat.MaxVal - PerceivedMin);
            Start = LengthBar.Full_BL;
            End = LengthBar.Full_TR;

            Start = Start + min_ratio * (End - Start);
        }

        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            base.Draw(Text, cam, Selected);

            if (MyDrawLayer != MyMenu.CurDrawLayer)
                return;

            if (!Text)
            {
                LengthBar.Pos = Pos + PosOffset;
                LengthBar.Draw(Selected);
            }
        }
    }
}