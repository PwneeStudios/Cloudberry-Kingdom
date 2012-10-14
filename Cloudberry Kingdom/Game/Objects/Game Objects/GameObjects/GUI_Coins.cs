using System;
using System.Text;

using Microsoft.Xna.Framework;

using CloudberryKingdom.InGameObjects;

namespace CloudberryKingdom
{
    public class GUI_Quota : GUI_Coins
    {
        public int Quota;

        public GUI_Quota(int Quota)
        {
            this.Quota = Quota;
        }

        protected override int OutOf()
        {
            return Quota;
        }

        public Action<GUI_Coins> OnQuotaMet;
        public bool QuotaMet = false;

        Door FinalDoor;
        public override void OnAdd()
        {
            base.OnAdd();

            FinalDoor = MyGame.MyLevel.FinalDoor;
            FinalDoor.SetLock(true, true, false);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Coins >= Quota)
            {
                if (!QuotaMet)
                {
                    if (OnQuotaMet != null) OnQuotaMet(this);
                    MyPile.BubbleUp(true);

                    FinalDoor.SetLock(false);

                    QuotaMet = true;
                }
            }
            else
            {
                FinalDoor.SetLock(true, true, false);

                QuotaMet = false;
            }
        }
    }

    public class GUI_Coins : GUI_Panel
    {
        StringBuilder MyString = new StringBuilder(50, 50);

        public int Coins, TotalCoins = 0;

        /// <summary>
        /// Return a string representation of the number of coins
        /// </summary>
        /// <returns></returns>
        public StringBuilder BuildString()
        {
            MyString.Length = 0;

            MyString.Add(Coins, 1);
            MyString.Append('/');
            MyString.Add(OutOf(), 1);

            return MyString;
        }

        protected virtual int OutOf() { return TotalCoins; }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();

            if (!AddedOnce)
            {
                Hide();
                
                Show();
            }

            AddedOnce = true;

            if (TotalCoins == 0)
                TotalCoins = MyGame.MyLevel.NumCoins;
            Coins = 0;
        }

        public override void Hide()
        {
            base.Hide();
            SlideOut(PresetPos.Top, 0);
        }

        public override void Show()
        {
            base.Show();
            SlideIn();
            MyPile.BubbleUp(false);
        }

        public Vector2 ApparentPos
        {
            get { return CoinText.FancyPos.AbsVal + CoinText.GetWorldSize() / 2; }
        }

        QuadClass CoinQuad;
        EzText CoinText;
        void UpdateCoinText()
        {
            CoinText.SubstituteText(BuildString());
        }
        
        public GUI_Coins() { }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            EnsureFancy();

            Vector2 shift = new Vector2(-320, 0);
            MyPile.Pos = new Vector2(836.1112f, 806.6667f) - shift;
            SlideInLength = 0;

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            EzFont font;
            string coin;
            float scale;

            if (false)
            {
                font = Tools.Font_Grobold42_2;
                coin = "CoinRed";
                scale = .55f;
            }
            else
            {
                font = Tools.Font_Grobold42;
                coin = "coin_blue";
                scale = .55f;
            }

            CoinText = new EzText(BuildString().ToString(), font, 450, false, false);
            CoinText.Name = "Coin";
            CoinText.Scale = scale;
            CoinText.Pos = new Vector2(297.2223f, 88.82223f) + shift;
            CoinText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            CoinText.OutlineColor = new Color(0, 0, 0).ToVector4();
            MyPile.Add(CoinText);

            CoinQuad = new QuadClass(coin, 150, true);
            CoinQuad.Name = "Coin";
            CoinQuad.Pos = new Vector2(200.5664f, -42.03058f) + shift;
            CoinQuad.Size = new Vector2(122.2223f, 193.6508f);
            CoinQuad.Angle = 0.1055556f;
            MyPile.Add(CoinQuad);
        }

        protected override void MyDraw()
        {
            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (MyGame.SoftPause) return;
            if (Hid || !Active) return;

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            Coins = PlayerManager.GetLevelCoins();
            //CoinText.Pos = new Vector2(CoinText.GetWorldWidth(), CoinText.Pos.Y);
            UpdateCoinText();
        }
    }
}