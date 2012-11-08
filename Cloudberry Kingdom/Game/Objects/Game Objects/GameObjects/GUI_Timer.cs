using System;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_Timer : GUI_Timer_Base
    {
        class OnCoinGrabProxy : Lambda_1<ObjectBase>
        {
            GUI_Timer timer;

            public OnCoinGrabProxy(GUI_Timer timer)
            {
                this.timer = timer;
            }

            public void Apply(ObjectBase obj)
            {
                timer.OnCoinGrab(obj);
            }
        }

        class OnCompleteLevelProxy : Lambda_1<Levels.Level>
        {
            GUI_Timer timer;

            public OnCompleteLevelProxy(GUI_Timer timer)
            {
                this.timer = timer;
            }

            public void Apply(Levels.Level level)
            {
                timer.OnCompleteLevel(level);
            }
        }


        public override void OnAdd()
        {
            base.OnAdd();
            
            MyGame.OnCoinGrab.Add(new OnCoinGrabProxy(this));
            MyGame.OnCompleteLevel.Add(new OnCompleteLevelProxy(this));
        }

        protected override void ReleaseBody()
        {
            //MyGame.OnCoinGrab -= OnCoinGrab; 
            
            base.ReleaseBody();            
        }

        public int CoinTimeValue = 60, MinLevelStartTimeValue = 62 + 31;
        public int MaxTime = 60;
        public void OnCoinGrab(ObjectBase obj)
        {
            Time += CoinTimeValue;

            if (Time > MaxTime)
                Time = MaxTime;
        }

        public void OnCompleteLevel(Levels.Level level)
        {
            MinLevelStartTimeValue = 124;
            Time = CoreMath.Restrict(MinLevelStartTimeValue, MaxTime, Time);
        }
    }
}