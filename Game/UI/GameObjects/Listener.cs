using System;

using CoreEngine;

using CloudberryKingdom.Levels;

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

        public Listener(ControllerButtons button, Action action)
        {
#if PC
            if (button == ControllerButtons.A || button == ControllerButtons.Any)
            {
                if (MyButton2 == null) MyButton2 = new ButtonClass();
                MyButton2.Set(ControllerButtons.Enter);
            }
#endif

            Active = true;
            PauseOnPause = false;
            RemoveAfterActivation = true;

            MyButton = button;
            MyAction = action;

            Control = -1;
        }

        public enum Type { OnDown, OnPressed };
        public Type MyType = Type.OnPressed;

        public Action MyAction;
        public int TriggeringPlayerIndex;

        /// <summary>
        /// If true the listener is removed immediately after it activates
        /// </summary>
        public bool RemoveAfterActivation = false;

		protected override void ReleaseBody()
		{
			base.ReleaseBody();

			MyAction = null;
		}

        public virtual void Activate()
        {
            if (MyAction != null)
                MyAction();

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
             //WARNING
            //if (true)
            if (MyButton == ControllerButtons.Any && MyType == Type.OnPressed && ButtonCheck.AnyKey()
				||
				MyType == Type.OnDown &&
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
                                if (PlayerManager.Get(i).Exists && ButtonCheck.GetState(MyButton, i, false, true, true).Pressed)
                                    TriggeringPlayerIndex = i;
                                break;

                            case Type.OnDown:
                                if (PlayerManager.Get(i).Exists && ButtonCheck.GetState(MyButton, i, false, true, true).Down)
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