using System.Collections.Generic;

using CloudberryKingdom.Levels;

using System.Threading;

namespace CloudberryKingdom
{
    public class CoinMod
    {
        GUI_Timer MyTimer;
        public CoinMod(GUI_Timer Timer)
        {
            this.MyTimer = Timer;
        }

        /// <summary>
        /// The manner in which coins are controlled to control difficulty.
        /// </summary>
        public CoinControlTypes CoinControlType = CoinControlTypes.ModNumber;
        public enum CoinControlTypes { ModNumber, ModValue };

        // The level after which difficulty doesn't increase (with respect to coins)
        public int LevelMax = 50;

        /// <summary>
        /// Restricts the number of coins in the level so that 
        /// the total amount of seconds possible to gain equals the level's par
        /// </summary>
        public void CoinControl(Level level, int Index)
        {
            // Calculate difficulty based on level
            // t ranges from 0 to 1
            float t = Tools.Restrict(0, 1, Index / (float)LevelMax);

            switch (CoinControlType)
            {
                case CoinControlTypes.ModNumber: CoinControl_ModNumber(level, t); break;
                case CoinControlTypes.ModValue: CoinControl_ModValue(level, t); break;
            }
        }

        void CoinControl_ModValue(Level level, float t)
        {
            int NumCoins = ModCoinNumber(level, 6);

            // Par starts a 1.75 times the actual par, and decreases down to the actual par as t -> 1
            float par = level.Par * (2f * (1 - t) + 1f * t);

            MyTimer.CoinTimeValue = (int)(par / NumCoins);
        }

        //public float ParMultiplier_Start = 1.85f, ParMultiplier_End = 1f;
        public float ParMultiplier_Start = 1.5f, ParMultiplier_End = 1.05f;
        void CoinControl_ModNumber(Level level, float t)
        {
            // Par starts a 1.75 times the actual par, and decreases down to the actual par as t -> 1
            float par = level.Par * (ParMultiplier_Start * (1 - t) + ParMultiplier_End * t);

            int NumCoinsNeeded = (int)((float)par / MyTimer.CoinTimeValue + .65f);

            ModCoinNumber(level, NumCoinsNeeded);
        }

        int ModCoinNumber(Level level, int N)
        {
            // Get all coins
            List<ObjectBase> coins = level.GetObjectList(ObjectType.Coin);

            foreach (ObjectBase coin in coins) coin.Core.MarkedForDeletion = true;

            List<ObjectBase> keep = level.Rnd.Choose(coins, N);

            foreach (ObjectBase coin in keep) coin.Core.MarkedForDeletion = false;

            int NumCoins = 0;
            foreach (ObjectBase coin in coins)
            {
                if (coin.Core.MarkedForDeletion)
                    coin.CollectSelf();
                else
                    // Count the number of coins we keep.
                    // May be different than N if there were few coins to start with
                    NumCoins++;
            }

            level.NumCoins = NumCoins;

            return NumCoins;
        }
    }
}