using System;

using CoreEngine;

namespace CloudberryKingdom
{
    public class MenuToggle : MenuItem
    {
        public MenuToggle(CoreFont Font)
        {
            Text text = new Text("xxxxx", Font);
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

        public Action<bool> OnToggle;
        public override void PhsxStep(bool Selected)
        {
            base.PhsxStep(Selected);

            if (!Selected) return;

            if (ButtonCheck.State(ControllerButtons.A, Control).Pressed)
            {
                Toggle(!MyState);

                if (OnToggle != null)
                    OnToggle(MyState);
            }
        }
    }
}