using System;



namespace CloudberryKingdom
{
    public class Listener : GUI_Panel
    {
        public Listener()
        {
            Active = true;
            PauseOnPause = true;
            Core.Show = false;
        }

        public Listener(ControllerButtons button, Lambda action)
        {
            if (button == ControllerButtons.A)
            {
                if (MyButton2 == null) MyButton2 = new ButtonClass();
                MyButton2.Set(ControllerButtons.Enter);
            }

            Active = true;
            PauseOnPause = false;
            RemoveAfterActivation = true;

            MyButton = button;
            MyAction = action;

            Control = -1;
        }

        public enum Type { OnDown, OnPressed };
        public Type MyType = Type.OnPressed;

        public Lambda MyAction;
        public int TriggeringPlayerIndex;

        /// <summary>
        /// If true the listener is removed immediately after it activates
        /// </summary>
        public bool RemoveAfterActivation = false;

        public virtual void Activate()
        {
            if (MyAction != null)
                MyAction.Apply();

            if (RemoveAfterActivation)
                Release();
        }

        public ControllerButtons MyButton = ControllerButtons.A;
        public ButtonClass MyButton2 = null;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Level level = Core.MyLevel;

            // Listen
            if (MyType == Type.OnDown &&
                    (ButtonCheck.State(MyButton, Control).Down || ButtonCheck.State(MyButton2, Control).Down)
                ||
               (MyType == Type.OnPressed &&
                    (ButtonCheck.State(MyButton, Control).Pressed || ButtonCheck.State(MyButton2, Control).Pressed)))
            {
                // If any player could trigger the event, check to see which did
                TriggeringPlayerIndex = Control;
                if (Control < 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        switch (MyType)
                        {
                            case Type.OnPressed:
                                if (PlayerManager.Get(i).Exists && ButtonCheck.State(MyButton, i).Pressed)
                                    TriggeringPlayerIndex = i;
                                break;

                            case Type.OnDown:
                                if (PlayerManager.Get(i).Exists && ButtonCheck.State(MyButton, i).Down)
                                    TriggeringPlayerIndex = i;
                                break;
                        }
                    }
                }

                Activate();
            }
        }
    }
}