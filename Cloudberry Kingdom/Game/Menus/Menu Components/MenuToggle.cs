using System;

namespace CloudberryKingdom
{
    public class MenuToggle : MenuItem
    {
        public MenuToggle(EzFont Font)
        {
            EzText text = new EzText("xxxxx", Font);
            base.Init(text, text.Clone());
        }

        bool MyState = false;
        public void Toggle(bool state)
        {
            MyState = state;

            if (state)
            {
                MyText.SubstituteText(Localization.Words.On);
                MySelectedText.SubstituteText(Localization.Words.On);
            }
            else
            {
                MyText.SubstituteText(Localization.Words.Off);
                MySelectedText.SubstituteText(Localization.Words.Off);
            }
        }

        public Lambda_1<bool> OnToggle;
        public override void PhsxStep(bool Selected)
        {
            base.PhsxStep(Selected);

            if (!Selected) return;

            if (ButtonCheck.State(ControllerButtons.A, Control).Pressed)
            {
                Toggle(!MyState);

                if (OnToggle != null)
                    OnToggle.Apply(MyState);
            }
        }
    }
}