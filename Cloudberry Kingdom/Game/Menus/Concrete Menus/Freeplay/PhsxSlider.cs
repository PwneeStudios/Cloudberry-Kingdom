using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Menus
{
    public class PhsxSlider : MenuSlider
    {
        public override string[] GetViewables()
        {
            return new string[] { "Pos", "SelectedPos", "!MyMenu", "SliderShift", "!MenuToAddTo" };
        }

        public static EzFont Font;
        public static Action<MenuItem> Process;

        public PhsxSlider(string text, BobPhsx.CustomData type)
            : base(new EzText(text, Font))
        {
            MyFloat = new WrappedFloat(BobPhsx.CustomPhsxData.Bounds(type).DefaultValue,
                                       BobPhsx.CustomPhsxData.Bounds(type).MinValue,
                                       BobPhsx.CustomPhsxData.Bounds(type).MaxValue);
            Process(this);
            ScaleText(.5f);

            SliderShift = new Vector2(-266.6946f, -57.77405f);

            Slider.Scale(.8f);
            SliderBack.Scale(.8f);
            Start *= .8f;
            End *= .8f;
        }

        bool _State = true;
        public bool State
        {
            set
            {
                _State = value;
                if (State)
                {
                    MyText.Alpha = MySelectedText.Alpha = 1f;
                }
                else
                {
                    MyText.Alpha = MySelectedText.Alpha = .3f;
                }
            }

            get
            {
                return _State;
            }
        }
    }
}
