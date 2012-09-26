using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Coins;

namespace CloudberryKingdom
{
    public class GUI_NextLife : GameObject
    {
        public override string[] GetViewables()
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
                    GiveLife();
                }

                UpdateCoinsText();
            }
        }

        private void GiveLife()
        {
            // Reset coin counter
            _Coins = 0;

            // Give extra life
            GUI_Lives.NumLives++;

            //MyGame.AddGameObject(new SuperCheer(2));

            // Remove last coin score text
            MyGame.RemoveLastCoinText();

            // Add text
            TextFloat text = new TextFloat("+1 Life", Coin.PosOfLastCoinGrabbed + new Vector2(21, 22.5f));
            text.MyText.Scale *= 1.33f;
            text.Core.DrawLayer = 8;
            text.MyText.MyFloatColor = new Color(0, 195, 17).ToVector4();
            text.MyText.OutlineColor = new Color(0, 80, 8).ToVector4();
            Core.MyLevel.MyGame.AddGameObject(text);
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

        public void OnCoinGrab(ObjectBase obj)
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

            EzFont font;
            string coin;
            float scale;
            Color c, o;

            if (false)
            {
                font = Tools.Font_Grobold42;
                coin = "CoinBlue";
                scale = .625f;
                c = Color.White;
                o = Color.Black;
            }
            else
            {
                font = Tools.Font_Grobold42;
                coin = "coin_blue";
                scale = .55f;
                c = new Color(228, 0, 69);
                o = Color.White;
            }

            CoinsText = new EzText(ToString(), font, 450, false, true);
            CoinsText.Name = "coin";
            CoinsText.Scale = scale;
            CoinsText.MyFloatColor = c.ToVector4();
            CoinsText.OutlineColor = o.ToVector4();

            QuadClass Coin = new QuadClass();
            Coin.Name = "coin";
            Coin.SetToDefault();
            Coin.TextureName = coin;
            Coin.Scale(61.5f);
            Coin.ScaleYToMatchRatio();
            Coin.PointxAxisTo(20 / CoreMath.c);
            
            MyPile.Add(Coin);
            MyPile.Add(CoinsText);

            MyPile.Pos = new Vector2(1002.133f, 670.5443f);

            if (false)
            {
                CoinsText.Pos = new Vector2(189.7776f, 111.7778f);
                Coin.Pos = new Vector2(149.0665f, 98.35567f);
            }
            else
            {
                EzText _t;
                _t = MyPile.FindEzText("coin"); if (_t != null) { _t.Pos = new Vector2(189.7776f, 111.7778f); }

                QuadClass _q;
                _q = MyPile.FindQuad("coin"); if (_q != null) { _q.Pos = new Vector2(140.7331f, 117.8001f); _q.Size = new Vector2(51.00002f, 72.85717f); }

                Pos = new Vector2(1002.133f, 670.5443f);
            }
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