using System;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    /// <summary>
    /// A simple timer. The timer is removed on level resets, and removes itself when the timer expires.
    /// </summary>
    public class GUI_Timer_Simple : GUI_Timer_Base
    {
        public GUI_Timer_Simple(int Time)
        {
            PreventRelease = false;
            Core.RemoveOnReset = true;

            this.Time = Time;

            OnTimeExpired.Add(new SimpleOnExpireLambda(this));
        }

        class SimpleOnExpireLambda : Lambda_1<GUI_Timer_Base>
        {
            GUI_Timer_Simple timer;
            public SimpleOnExpireLambda(GUI_Timer_Simple timer)
            {
                this.timer = timer;
            }

            public void Apply(GUI_Timer_Base timer)
            {
                timer.Hide();
                timer.ReleaseWhenDone = true;
            }
        }

        //void OnExpire(GUI_Timer_Base timer)
        //{
        //    Hide();
        //    ReleaseWhenDone = true;
        //}
    }
}