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
        public static Lambda_1<MenuItem> Process;

        BobPhsx.CustomData MyType;

        class PhsxDataSetter : Lambda
        {
            BobPhsx.CustomData MyType;
            WrappedFloat MyFloat;

            public PhsxDataSetter(BobPhsx.CustomData MyType, WrappedFloat MyFloat)
            {
                this.MyType = MyType;
                this.MyFloat = MyFloat;
            }

            public void Apply()
            {
                CustomHero_GUI.HeroPhsxData[MyType] = MyFloat.Val;
            }
        }

        public PhsxSlider(Localization.Words word, BobPhsx.CustomData type)
            : base(new EzText(word, Font))
        {
            MyType = type;

            MyFloat = new WrappedFloat(
                                       //CustomHero_GUI.Hero.MyCustomPhsxData[type],
                                       //BobPhsx.CustomPhsxData.Bounds(type).DefaultValue,
                                       CustomHero_GUI.HeroPhsxData[MyType],

                                       BobPhsx.CustomPhsxData.Bounds(type).MinValue,
                                       BobPhsx.CustomPhsxData.Bounds(type).MaxValue);
            Process.Apply(this);
            ScaleText(.33f);

            MyFloat.SetCallback = new PhsxDataSetter(MyType, MyFloat);

            SliderShift = new Vector2(-296.6946f, -57.77405f);

            Slider.Scale(.66f);
            SliderBack.Scale(.73f);
            Start *= .8f;
            End *= .8f;

            GrayOutOnUnselectable = true;
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
