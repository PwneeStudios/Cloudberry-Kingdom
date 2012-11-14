using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class HelpMenu : CkBaseMenu
    {
        int Bank()
        {
            switch (MyGame.MyBankType)
            {
                case GameData.BankType.Infinite:
                    return 99;

                case GameData.BankType.Campaign:
                    return PlayerManager.PlayerMax(p => p.CampaignCoins);
            }

            return 0;
        }

        void Buy(int Cost)
        {
            switch (MyGame.MyBankType)
            {
                case GameData.BankType.Campaign:
                    foreach (var p in PlayerManager.ExistingPlayers)
                        p.CampaignCoins = System.Math.Max(p.CampaignCoins - Cost, 0);
                    break;

                case GameData.BankType.Infinite:
                    break;
            }

            //PlayerManager.CoinsSpent += Cost;

            SetCoins(Bank());
        }

        void SetCoins(int Coins)
        {
            if (Coins > 99) Coins = 99;
            CoinsText.SubstituteText("x" + Coins.ToString());
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
            item.MyText.PicShadow = item.MySelectedText.PicShadow = false;
        }

        public HelpMenu()
        {
            // Note that help was used, so that no hint is given about it
            Hints.SetYForHelpNum(999);
        }

        class MakeListenerHelper : Lambda
        {
            Listener listener;

            public MakeListenerHelper(Listener listener)
            {
                this.listener = listener;
            }

            public void Apply()
            {
                if (Tools.StepControl) return;

                Level level = Tools.CurLevel;
                if (!level.Replay && !level.Watching && !level.Finished && !level.PreventHelp)
                    listener.Call(new HelpMenu());
            }
        }

        public static GameObject MakeListener()
        {
            Listener listener = new Listener();
            //listener.MyType = Listener.Type.OnDown;
            listener.MyButton = ControllerButtons.Y;
            listener.MyButton2 = ButtonCheck.Help_KeyboardKey;
            listener.MyAction = new MakeListenerHelper(listener);

            return listener;
        }

        class ReturnToCallerProxy : Lambda
        {
            HelpMenu hm;

            public ReturnToCallerProxy(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply()
            {
                hm.ReturnToCaller();
            }
        }

        int DelayExit = 29;
        public override void ReturnToCaller()
        {
            InGameStartMenu.PreventMenu = false;

            if (Active)
            {
                Active = false;
                MyGame.WaitThenDo(DelayExit, new ReturnToCallerProxy(this));
            }
            else
                base.ReturnToCaller();    
        }

        public override bool MenuReturnToCaller(Menu menu)
        {
            DelayExit = 0;

            return base.MenuReturnToCaller(menu);
        }

        int Cost_Watch = 0, Cost_Path = 30, Cost_Slow = 10;
        bool Allowed_WatchComputer()
        {
            return MyGame.MyLevel.WatchComputerEnabled() && Bank() >= Cost_Watch;
        }

        class WatchComputerHelper : Lambda
        {
            HelpMenu hm;

            public WatchComputerHelper(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply()
            {
                hm.MyGame.MyLevel.WatchComputer();
            }
        }

        void WatchComputer()
        {
            if (!Allowed_WatchComputer())
                return;

            Buy(Cost_Watch);

            ReturnToCaller();
            MyGame.WaitThenDo(DelayExit - 10, new WatchComputerHelper(this));
        }

        bool On_ShowPath()
        {
            return Tools.CurGameData.MyGameObjects.Any(obj => obj is ShowGuide);
        }
        bool Allowed_ShowPath()
        {
#if DEBUG
            return MyGame.MyLevel.WatchComputerEnabled();
#else
            return MyGame.MyLevel.CanWatchComputer && Bank() >= Cost_Path;
#endif
        }

        class Toggle_ShowPathHelper : Lambda
        {
            HelpMenu hm;

            public Toggle_ShowPathHelper(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply()
            {
                foreach(GameObject go in hm.MyGame.MyGameObjects)
                {
                    if(go is ShowGuide)
                        go.Release();
                }
            }
        }

        class Toggle_ShowPathSetter : Lambda
        {
            HelpMenu hm;
            bool state;

            public Toggle_ShowPathSetter(HelpMenu hm, bool state)
            {
                this.hm = hm;
                this.state = state;
            }

            public void Apply()
            {
                hm.Toggle_ShowPath(state);
            }
        }

        class Toggle_ShowPathProxy : Lambda_1<bool>
        {
            HelpMenu hm;

            public Toggle_ShowPathProxy(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply(bool state)
            {
                hm.Toggle_ShowPath(state);
            }
        }

        void Toggle_ShowPath(bool state)
        {
            if (state)
            {
                ShowGuide guide = new ShowGuide();

                MyGame.AddGameObject(guide);
            }
            else
            {
                foreach (var obj in MyGame.MyGameObjects)
                    if (obj is ShowGuide)
                        obj.Release();
                MyGame.AddToDo(new Toggle_ShowPathHelper(this));
            }
        }
        void ShowPath()
        {
            if (!Allowed_ShowPath())
                return;

            Buy(Cost_Path);

            ReturnToCaller();
            MyGame.WaitThenDo(DelayExit - 10, new Toggle_ShowPathSetter(this, true));
        }

        bool On_SlowMo()
        {
            return Tools.CurGameData.MyGameObjects.Any(obj => obj is SlowMo);
        }
        bool Allowed_SlowMo()
        {
            return true && Bank() >= Cost_Slow;
        }

        class Toggle_SloMoHelper : Lambda
        {
            HelpMenu hm;

            public Toggle_SloMoHelper(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply()
            {
                hm.MyGame.MyGameObjects.RemoveAll(match => match is SlowMo);
            }
        }

        class Toggle_SlowMoProxy : Lambda_1<bool>
        {
            HelpMenu hm;

            public Toggle_SlowMoProxy(HelpMenu hm)
            {
                this.hm = hm;
            }

            public void Apply(bool state)
            {
                hm.Toggle_SlowMo(state);
            }
        }

        void Toggle_SlowMo(bool state)
        {
            if (state)
            {
                SlowMo slowmo = new SlowMo();
                slowmo.Control = Control;

                MyGame.AddGameObject(slowmo);
            }
            else
            {
                MyGame.AddToDo(new Toggle_SloMoHelper(this));
            }
        }

        void SlowMo()
        {
            if (!Allowed_SlowMo())
                return;

            Buy(Cost_Slow);

            Toggle_SlowMo(true);
            ReturnToCaller();
        }

        public override void OnAdd()
        {
            Initialization();

            base.OnAdd();

            InGameStartMenu.PreventMenu = true;

            Item_WatchComputer.Icon.Fade(!Allowed_WatchComputer());
            Item_SlowMo.Icon.Fade(!Allowed_SlowMo());
            Item_ShowPath.Icon.Fade(!Allowed_ShowPath());

            ReturnToCallerDelay = 30;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        MenuItem Item_ShowPath, Item_WatchComputer, Item_SlowMo;

        EzText CoinsText;

        HelpBlurb Blurb;

        void Initialization()
        {
            GameData game = Tools.CurGameData;

            PauseGame = true;

            //FontScale = .73f;
            FontScale = .8f;

            MyPile = new DrawPile();

            RightPanel = Blurb = new HelpBlurb();

            this.CallDelay = 3;
            this.SlideLength = 14;
            this.SelectedItemShift = new Vector2(0, 0);
            //this.SlideInFrom = PresetPos.Right;

            MakeDarkBack();

            // Make the left backdrop
            QuadClass backdrop = new QuadClass("Backplate_1500x900", 1500);
            MyPile.Add(backdrop, "Backdrop");
            backdrop.Pos = new Vector2(-1777.778f, 30.55557f);

            // Coin
            QuadClass Coin = new QuadClass("Coin_Blue", 90, true);
            Coin.Pos = new Vector2(-873.1558f, 770.5778f);
            MyPile.Add(Coin, "Coin");

            CoinsText = new EzText("x", Resources.Font_Grobold42, 450, false, true);
            CoinsText.Name = "Coins";
            CoinsText.Scale = .8f;
            CoinsText.Pos = new Vector2(-910.2224f, 717.3333f);
            CoinsText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            CoinsText.OutlineColor = new Color(0, 0, 0).ToVector4();

            CoinsText.ShadowOffset = new Vector2(-11f, 11f);
            CoinsText.ShadowColor = new Color(65, 65, 65, 160);
            CoinsText.PicShadow = false;
            MyPile.Add(CoinsText);
            SetCoins(Bank());


            // Make the menu
            MyMenu = new Menu(false);

            Control = -1;

            MyMenu.OnB = null;

            MenuItem item;
            MenuToggle toggle;

            // Header
            EzText HeaderText = new EzText(Localization.Words.Coins, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText, "Header");
            HeaderText.Pos = new Vector2(-1663.889f, 971.8889f);


            Vector2 IconOffset = new Vector2(-150, 0);

            string CoinPrefix = "{pCoin_Blue,68,?}";

            // Watch the computer
            item = new MenuItem(new EzText(CoinPrefix + "x" + Cost_Watch.ToString(), ItemFont));
            item.Name = "WatchComputer";
            Item_WatchComputer = item;
            item.SetIcon(ObjectIcon.RobotIcon.Clone());
            item.Icon.Pos = IconOffset + new Vector2(-10, 0);
            item.Go = Cast.ToItem(WatchComputer);
            ItemPos = new Vector2(-1033.333f, 429.4446f);
            PosAdd = new Vector2(0, -520);
            AddItem(item);
            item.AdditionalOnSelect = Blurb.SetText_Action(Localization.Words.WatchComputer);

            // Show path
            if (On_ShowPath())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = new Toggle_ShowPathProxy(this);
                toggle.Toggle(true);
            }
            else
            {
                item = new MenuItem(new EzText(CoinPrefix + "x" + Cost_Path.ToString(), ItemFont));
                item.Go = Cast.ToItem(ShowPath);
            }
            item.Name = "ShowPath";
            item.SetIcon(ObjectIcon.PathIcon.Clone());
            item.Icon.Pos = IconOffset + new Vector2(-20, -75);
            AddItem(item);
            item.AdditionalOnSelect = Blurb.SetText_Action(Localization.Words.ShowPath);
            Item_ShowPath = item;

            // Slow mo
            if (On_SlowMo())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = new Toggle_SlowMoProxy(this);
                toggle.Toggle(true);
            }
            else
            {
                item = new MenuItem(new EzText(CoinPrefix + "x" + Cost_Slow.ToString(), ItemFont));
                item.Go = Cast.ToItem(SlowMo);
            }
            item.Name = "SlowMo";
            item.SetIcon(ObjectIcon.SlowMoIcon.Clone());
            item.Icon.Pos = IconOffset + new Vector2(-20, -55);
            AddItem(item);
            item.AdditionalOnSelect = Blurb.SetText_Action(Localization.Words.ActivateSlowMo);
            Item_SlowMo = item;

            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnY = Cast.ToAction(MenuReturnToCaller);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            EnsureFancy();
            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("WatchComputer"); if (_item != null) { _item.SetPos = new Vector2(-1050f, 285.0002f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("ShowPath"); if (_item != null) { _item.SetPos = new Vector2(-1047.222f, -98.8887f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SlowMo"); if (_item != null) { _item.SetPos = new Vector2(-1052.777f, -499.4443f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(0f, 0f);

            EzText _t;
            _t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-771.3337f, 622.889f); _t.Scale = 0.6593335f; }
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1497.222f, 816.3335f); _t.Scale = 0.9640832f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(22.22229f, -33.33333f); _q.Size = new Vector2(1740.553f, 1044.332f); }
            _q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-798.1558f, 634.4669f); _q.Size = new Vector2(110.5714f, 110.5714f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

#if PC_VERSION
            item.Padding += new Vector2(20, 40);
#endif
        }

        public override void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller(PlaySound);
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
        }

        protected override void SlideOut_RightPanel(PresetPos Preset, int Frames)
        {
            base.SlideOut_RightPanel(Preset, Frames);
        }
    }
}