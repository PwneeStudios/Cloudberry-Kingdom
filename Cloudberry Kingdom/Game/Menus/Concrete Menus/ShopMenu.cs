using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Stats;
using CloudberryKingdom;
using CoreEngine;

namespace CloudberryKingdom.Awards
{
    public class VerifyPurchaseMenu : VerifyBaseMenu
    {
        public Buyable buyable;
        public int Cost;
        public VerifyPurchaseMenu(int Control, Buyable buyable) : base(false)
        {
            this.buyable = buyable;
            this.Cost = buyable.GetPrice();
            this.Control = Control;

            Constructor();
        }

        class CoinSoundPlayer : Lambda
        {
            float pitch;
            EzSound sound;

            public CoinSoundPlayer(float pitch)
            {
                this.pitch = pitch;
                sound = Tools.Sound("Coin");
            }

            public void Apply()
            {
                sound.Play(1, pitch, 0);
            }
        }

        class YesProxy : Lambda_1<MenuItem>
        {
            VerifyPurchaseMenu vpm;

            public YesProxy(VerifyPurchaseMenu vpm)
            {
                this.vpm = vpm;
            }

            public void Apply(MenuItem item)
            {
                vpm.Yes(item);
            }
        }

        void Yes(MenuItem item)
        {
            var sound = Tools.Sound("Coin");
            int wait = 0;
            float pitch = 0;
            for (int i = 0; i < Cost; i += 150)
            {
                MyGame.WaitThenDo(wait, new CoinSoundPlayer(pitch));
                wait += 5;
                pitch += .05f;
            }

            PlayerManager.DeductCost(Cost);
            PlayerManager.GiveBoughtItem(buyable);

            ShopMenu.ActiveShop.UpdateAll();

            ReturnToCaller();
        }

        class NoProxy : Lambda_1<MenuItem>
        {
            VerifyPurchaseMenu vpm;

            public NoProxy(VerifyPurchaseMenu vpm)
            {
                this.vpm = vpm;
            }

            public void Apply(MenuItem item)
            {
                vpm.No(item);
            }
        }

        void No(MenuItem item)
        {
            ReturnToCaller();
        }

        public override void MakeBackdrop()
        {
            Backdrop = new QuadClass("score_screen", 1500, true);
            MyPile.Add(Backdrop);
            MyPile.Add(Backdrop);
            Backdrop.Size = new Vector2(1246.031f, 691.4683f);
            Backdrop.Pos = new Vector2(1086.013f, 352.7779f);
            Backdrop.Quad.SetColor(new Color(215, 215, 215, 255));
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            string pic = ShopMenu.GetString(buyable);
            /*
            float width = (int)(100f * buyable.HatPicScale.X);
            Vector2 Offset = buyable.HatPicShift * 100;
            string offset = string.Format("{0},{1}", Offset.X, -Offset.Y);
            string pic = "{p" + buyable.GetTexture().Name + "," + width.ToString() + ",?," + offset + "}";
            */

            string postfix = "{pCoinBlue,80,?}x " + Cost.ToString();
            string Text = "Buy  " + pic + "\n    for " + postfix + "?";
            EzText HeaderText = new EzText(Text, ItemFont, 1000, false, false, .8f);
            HeaderText.Scale *= .85f;
            //SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos + new Vector2(-200, 200);

            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont));
            item.Go = new YesProxy(this);
            AddItem(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont));
            item.Go = new NoProxy(this);
            AddItem(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }

    public class ShopMenu : CkBaseMenu
    {
        public static ShopMenu ActiveShop;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            ActiveShop = null;
            SaveGroup.SaveAll();
        }

        class VerifyPurchaseProxy : Lambda_1<MenuItem>
        {
            ShopMenu sm;

            public VerifyPurchaseProxy(ShopMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.VerifyPurchase(item);
            }
        }

        void VerifyPurchase(MenuItem item)
        {
            Buyable buyable = item.MyObject as Buyable;
            int Price = buyable.GetPrice();
            if (PlayerManager.CombinedBank() >= Price)
                Call(new VerifyPurchaseMenu(-1, buyable), 6);
            else
                Tools.Sound("Menu_Tick").Play();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected void SetHeaderProperties2(EzText text)
        {
            base.SetHeaderProperties(text);
            text.Shadow = false;
            text.MyFloatColor = ColorHelper.Gray(.923f);
        }

        public override void Init()
        {
            base.Init();

            CategoryDelays();
        }

        void SetBankAmount()
        {
            int coins = PlayerManager.CombinedBank();
            BankAmount.SubstituteText(coins.ToString());
        }

        public void UpdateAll()
        {
            SetItem(MyMenu.CurItem);
            SetBankAmount();
        }

        static string ClrString(ClrTextFx data)
        {
            EzTexture texture;
            int width = 100, height = 96;

            Vector2 Offset = Vector2.Zero;
            string offset = string.Format("{0},{1}", Offset.X, -Offset.Y);

            string pic;
            if (data.PicTexture != null)
            {
                texture = data.PicTexture;
            }
            else
            {
                texture = data.Texture;
                offset += ",1"; // Use paint effect
            }

            pic = "{p" + texture.Name + "," + width.ToString() + "," + height.ToString() + "," + offset + "}";

            return pic;
        }

        void SetItem(MenuItem item)
        {
            Buyable buyable = item.MyObject as Buyable;

            bool Sold = PlayerManager.Bought(buyable);



            string pic, postfix;

            pic = GetString(item);
            
            if (Sold)
                postfix = "  {c255,100,100,255}Sold!";
            else
                postfix = "  {pCoinBlue,80,?}x " + buyable.GetPrice().ToString();

            // Replace text and reset item properties
            var Text = new EzText(pic + postfix, ItemFont);
            var SelectedText = new EzText(pic + postfix, ItemFont);
            item.MyText.Release(); item.MySelectedText.Release();
            item.MyText = Text; item.MySelectedText = SelectedText;
            SetItemProperties(item);

            // Action
            if (Sold)
                item.Go = null;
            else
            {
                item.Go = new VerifyPurchaseProxy(this);
                item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
            }
        }


        public static string GetString(MenuItem item)
        {
            Buyable buyable = item.MyObject as Buyable;
            return GetString(buyable);
        }
        public static string GetString(Buyable buyable)
        {
            string pic;
            Hat hat = buyable as Hat;
            if (null != hat)
            {
                float width;
                Vector2 Offset;
                string offset;

                width = (int)(100f * hat.HatPicScale.X);
                Offset = hat.HatPicShift * 100;
                offset = string.Format("{0},{1}", Offset.X, -Offset.Y);
                pic = "{p" + hat.GetTexture().Name + "," + width.ToString() + ",?," + offset + "}";
            }
            else
            {
                ClrTextFx clr = (ClrTextFx)buyable;
                pic = ShopMenu.ClrString(clr);
            }
            return pic;
        }

        void MenuGo_Customize(MenuItem item)
        {
            SlideInFrom = PresetPos.Left;
            Hide(PresetPos.Left);
            CharSelect();
        }
        void CharSelect()
        {
            CharacterSelectManager.Start(this);
        }

        static int HatCompare(Hat h1, Hat h2)
        {
            return h1.Price.CompareTo(h2.Price);
        }

        EzText Bank, BankAmount;
        public ShopMenu()
        {
            ActiveShop = this;

            MyPile = new DrawPile();

            // Make the menu
            ItemPos = new Vector2(-1305, 620);
            PosAdd = new Vector2(0, -151) * 1.2f * 1.1f;

            MyMenu = new LongMenu();
            MyMenu.FixedToCamera = false;
            MyMenu.WrapSelect = false;

            MyMenu.Control = -1;

            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            // Header
            MenuItem Header = new MenuItem(new EzText(Localization.Words.HatsForSale, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1608.809f, 951.508f);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;


            MakeBack();
            ItemPos = new Vector2(-1305, 620);

            // Format the list of hats into a menu
            MenuItem item;
            List<Hat> hats = new List<Hat>(ColorSchemeManager.HatInfo);
            hats.Sort(HatCompare);
            
            //foreach (Hat hat in ColorSchemeManager.HatInfo)
            foreach (Hat hat in hats)
            {
                if (hat.AssociatedAward != null || hat.GetTexture() == null || hat == Hat.None) continue;

                //item = new MenuItem(new EzText(pic + postfix, ItemFont));
                item = new MenuItem(new EzText("xxx", ItemFont));
                item.MyObject = hat;

                AddItem(item);
                SetItem(item);                

                ItemPos.Y -= 60;
            }

            // Header
            MakeHeader(Header, "Skins");

            foreach (MenuListItem clr_item in ColorSchemeManager.ColorList)
            {
                ClrTextFx clr = (ClrTextFx)clr_item.obj;

                if (clr.Price <= 0) continue;

                item = new MenuItem(new EzText("xxx", ItemFont));
                item.MyObject = clr;

                AddItem(item);
                SetItem(item);

                // Space before header
                ItemPos.Y -= 60;
            }


            // Header
            MakeHeader(Header, "Capes");

            foreach (MenuListItem clr_item in ColorSchemeManager.CapeColorList)
            {
                ClrTextFx clr = (ClrTextFx)clr_item.obj;

                bool found = false;
                foreach (MenuListItem match in ColorSchemeManager.ColorList)
                {
                    if (clr.Guid == ((Buyable)match.obj).GetGuid())
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    continue;

                if (clr.Price <= 0) continue;

                item = new MenuItem(new EzText("xxx", ItemFont));
                item.MyObject = clr;

                AddItem(item);
                SetItem(item);

                // Space before header
                ItemPos.Y -= 60;
            }

            //MakeBack();


            // Select first selectable item
            //MyMenu.SelectItem(1);
            MyMenu.SelectItem(3);

            MakeRest();
        }

        void MakeHeader(MenuItem Header, string str)
        {
            Header = new MenuItem(new EzText(str, Resources.Font_Grobold42_2));
            MyMenu.Add(Header);
            ItemPos.Y -= 40;
            Header.Pos = ItemPos + new Vector2(-130, 40);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1f;// 1.15f;
            Header.Selectable = false;

            ItemPos.Y -= 213;
        }


        void MakeRest()
        {
            // Backdrop
            QuadClass backdrop;
            
            //backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            //MyPile.Add(backdrop);
            //backdrop.Pos = new Vector2(3009.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            backdrop = new QuadClass("score_screen", 1500, true);
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Size = new Vector2(853.1744f, 1973.215f);
            backdrop.Pos = new Vector2(869.0458f, -35.71438f);

            backdrop = new QuadClass("score_screen", 1500, true);
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Size = new Vector2(853.1744f, 1973.215f);
            backdrop.Pos = new Vector2(-825.3976f, -71.42863f);

            var shop = new QuadClass("menupic_shop", 965 * 1.042f * 1.15f, true);
            shop.Pos = new Vector2(800, -200);
            MyPile.Add(shop);

            Bank = new EzText(Localization.Words.Bank, Resources.Font_Grobold42);
            Bank.Scale *= 1.1f;
            CkColorHelper.UnpleasantColor(Bank);
            MyPile.Add(Bank);
            Bank.Pos = new Vector2(100f, 919.0476f);

            BankAmount = new EzText("xx", Resources.Font_Grobold42);
            BankAmount.Scale *= .935f;
            MyPile.Add(BankAmount);
            BankAmount.Pos = new Vector2(855f, 877.5f);
            SetBankAmount();

            EnsureFancy();
            MyMenu.Pos = new Vector2(67.45706f, 0f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

        }

        void MakeBack()
        {
            MenuItem item;

            ItemPos = new Vector2(-1257.38f, -5900f);

//#if PC_VERSION
//            // Customize
//            ItemPos.Y -= 5;
//            item = new MenuItem(new EzText(Localization.Words.Customize, ItemFont));
//            item.Go = MenuGo_Customize;
//            //AddItem(item);
//            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
//            //item.ScaleText(1.12f);
//            ItemPos.Y -= 60;

//            ItemPos = new Vector2(1175.161f, 773.0161f);
//            item.UnaffectedByScroll = true;
//            AddItem(item);
//            item.ScaleText(.5f);
//#endif

            this.DestinationScale *= 1.05f;

            // Back
            //item = MakeBackButton();
            //item.ScaleText(1.4f);

            ItemPos = new Vector2(1341.829f, 697.1431f);
            item = MakeBackButton();
            item.UnaffectedByScroll = true;
            item.ScaleText(.5f);
        }

        class OnAddHelper : LambdaFunc<bool>
        {
            ScrollBar bar;

            public OnAddHelper(ScrollBar bar)
            {
                this.bar = bar;
            }

            public bool Apply()
            {
                return bar.MyMenu.HitTest();
            }
        }

        public override void OnAdd()
        {
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;

            base.OnAdd();

            // Scroll bar
#if PC_VERSION
            {
                ScrollBar bar = new ScrollBar((LongMenu)MyMenu, this);
                bar.BarPos = new Vector2(-2384.921f, 135);
                MyGame.AddGameObject(bar);
                MyMenu.AdditionalCheckForOutsideClick = new OnAddHelper(bar);
            }
#endif
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}