using System;
using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class GUI_Timer : GUI_Timer_Base
    {
        public override void OnAdd()
        {
            base.OnAdd();
            
            MyGame.OnCoinGrab += OnCoinGrab;
        }

        protected override void ReleaseBody()
        {
            MyGame.OnCoinGrab -= OnCoinGrab; 
            
            base.ReleaseBody();            
        }

        public int CoinTimeValue = 60;
        public int MaxTime = 60;
        public void OnCoinGrab(ObjectBase obj)
        {
            Time += CoinTimeValue;

            if (Time > MaxTime)
                Time = MaxTime;
        }
    }
}