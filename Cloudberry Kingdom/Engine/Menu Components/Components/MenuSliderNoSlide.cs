using CoreEngine;

namespace CloudberryKingdom
{
    public class MenuSliderNoSlide : MenuSliderBase
    {
        public MenuSliderNoSlide(EzText Text)
        {
            Init(Text, Text.Clone());
            InitializeSlider();
        }
        
        public MenuSliderNoSlide(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
            InitializeSlider();
        }

#if PC
        protected override void PC_OnLeftMouseDown()
        {
            if (!ButtonCheck.State(ControllerButtons.A, Control).Pressed)
                return;

            if (MyFloat.Val == MyFloat.MaxVal)
                MyFloat.Val = MyFloat.MinVal;
            else
                MyFloat.Val += 1;

            Slide();
        }
#endif
    }
}