using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GUI_NextLife : GameObject, IViewable
    {
        public string[] GetViewables()
        {
            return new string[] { };
        }

        public DrawPile MyPile = new DrawPile();

        GUI_LivesLeft GUI_Lives;

        int Max = 25;

        int _Coins = 0;
        // The time in number of frames
        public int Coins
        {
            get { return _Coins; }
            set
            {
                _Coins = value;
                if (_Coins >= Max)
                {
                    _Coins = 0;
                    GUI_Lives.NumLives++;

                    MyGame.AddGameObject(new SuperCheer(2));
                }

                UpdateCoinsText();
            }
        }

        /// <summary>
        /// Return a string representation of the coins grabbed.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //string str = string.Format("x{0}/{1}", Coins, Max);
            string str = string.Format("{0}/{1}", Coins, Max);

            return str;
        }

        public void OnCoinGrab(IObject obj)
        {
            Coins++;
        }

        EzText CoinsText;
        void UpdateCoinsText()
        {
            CoinsText.SubstituteText(ToString());
        }
        
        public GUI_NextLife(int CoinsToNextLife, GUI_LivesLeft GUI_Lives)
        {
            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            this.GUI_Lives = GUI_Lives;

            Max = CoinsToNextLife;

            MyPile.FancyPos.UpdateWithGame = true;

            CoinsText = new EzText(ToString(), Tools.Font_DylanThin42, 450, false, true);
            CoinsText.Scale = .625f;
            CoinsText.Pos = new Vector2(189.7776f, 111.7778f);
            CoinsText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            CoinsText.OutlineColor = new Color(0, 0, 0).ToVector4();

            QuadClass Coin = new QuadClass();
            Coin.SetToDefault();
            Coin.TextureName = "CoinBlue";
            Coin.Scale(61.5f);
            Coin.ScaleYToMatchRatio();
            Coin.PointxAxisTo(20 / Tools.c);

            //Coin.Pos = new Vector2(182.4f, 92.8f);
            Coin.Pos = new Vector2(149.0665f, 98.35567f);
            MyPile.Add(Coin);

            CoinsText.ShadowOffset = new Vector2(-11f, 11f);
            CoinsText.ShadowColor = new Color(65, 65, 65, 160);
            CoinsText.PicShadow = false;
            MyPile.Add(CoinsText);

            MyPile.Pos =
                //new Vector2(868.8f, 776.1f);
                new Vector2(1002.133f, 670.5443f);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            MyGame.OnCoinGrab += OnCoinGrab;
        }

        protected override void ReleaseBody()
        {
            if (MyGame != null)
                MyGame.OnCoinGrab -= OnCoinGrab;

            base.ReleaseBody();
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            MyPile.Draw();
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;
            MyPile.FancyPos.SetCenter(level.MainCamera, true);
            MyPile.FancyPos.Update();
        }
    }
}