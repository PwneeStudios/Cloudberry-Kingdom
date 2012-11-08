namespace CloudberryKingdom
{
    public class CoinScoreMultiplierObject : GameObject
    {
        class OnCoinGrabProxy : Lambda_1<ObjectBase>
        {
            CoinScoreMultiplierObject csmo;
            public OnCoinGrabProxy(CoinScoreMultiplierObject csmo)
            {
                this.csmo = csmo;
            }

            public void Apply(ObjectBase obj)
            {
                csmo.OnCoinGrab(obj);
            }
        }

        class OnLevelRetryProxy : Lambda
        {
            CoinScoreMultiplierObject csmo;
            public OnLevelRetryProxy(CoinScoreMultiplierObject csmo)
            {
                this.csmo = csmo;
            }

            public void Apply()
            {
                csmo.OnLevelRetry();
            }
        }

        class OnCalculateCoinScoreMultiplierProxy : Lambda_1<GameData>
        {
            CoinScoreMultiplierObject csmo;
            public OnCalculateCoinScoreMultiplierProxy(CoinScoreMultiplierObject csmo)
            {
                this.csmo = csmo;
            }

            public void Apply(GameData obj)
            {
                obj.CoinScoreMultiplier *= csmo._CoinScoreMultiplier;
            }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            ResetMultiplier();

            MyGame.OnCoinGrab.Add(new OnCoinGrabProxy(this));
            MyGame.OnLevelRetry.Add(new OnLevelRetryProxy(this));

            MyGame.OnCalculateCoinScoreMultiplier.Add(new OnCalculateCoinScoreMultiplierProxy(this));
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
            
            //MyGame.OnCoinGrab -= OnCoinGrab;
            //MyGame.OnLevelRetry -= OnLevelRetry;
        }

        float _CoinScoreMultiplier = 1f;
        public float CoinScoreMultiplier
        {
            get { return _CoinScoreMultiplier; }
            set
            {
                _CoinScoreMultiplier = value;
                //UpdateGameMultiplier();                
            }
        }

        /*
        /// <summary>
        /// Make the GameData's multiplier match this multiplier
        /// </summary>
        void UpdateGameMultiplier()
        {
            MyGame.CoinScoreMultiplier = _CoinScoreMultiplier;
        }*/

        /// <summary>
        /// Every time a coin is grabbed increase the multiplier
        /// </summary>
        public void OnCoinGrab(ObjectBase obj)
        {
            CoinScoreMultiplier++;
        }

        /// <summary>
        /// When the players die reset the multiplier
        /// </summary>
        public void OnLevelRetry()
        {
            ResetMultiplier();   
        }

        void ResetMultiplier()
        {
            CoinScoreMultiplier = 1;
        }

        public CoinScoreMultiplierObject()
        {
            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;
        }
    }
}