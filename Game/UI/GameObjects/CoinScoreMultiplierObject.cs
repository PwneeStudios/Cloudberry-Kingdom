namespace CloudberryKingdom
{
    public class CoinScoreMultiplierObject : GameObject
    {
        public override void OnAdd()
        {
            base.OnAdd();

            ResetMultiplier();
            
            MyGame.OnCoinGrab += OnCoinGrab;
            MyGame.OnLevelRetry += OnLevelRetry;

            MyGame.OnCalculateCoinScoreMultiplier += game => game.CoinScoreMultiplier *= _CoinScoreMultiplier;
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyGame.OnCoinGrab -= OnCoinGrab;
            MyGame.OnLevelRetry -= OnLevelRetry;
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