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
                    return 999;

                case GameData.BankType.Escalation:
                    return Challenge.Coins;

                case GameData.BankType.Campaign:
                    return PlayerManager.PlayerMax(p => p.CampaignCoins);
            }

            return 0;

            //return
            //    PlayerManager.PlayerSum(p => p.CampaignStats.Coins) +
            //    PlayerManager.PlayerSum(p => p.GameStats.Coins) +
            //    PlayerManager.PlayerSum(p => p.LevelStats.Coins)
            //    - PlayerManager.CoinsSpent;
        }

        void Buy(int Cost)
        {
            switch (MyGame.MyBankType)
            {
                case GameData.BankType.Escalation:
                    Challenge.Coins -= Cost;
                    break;

                case GameData.BankType.Campaign:
                    for (int i = 0; i < 4; i++)
                    {
                        var player = PlayerManager.Players[i];
                        if (player == null || !player.Exists) continue;

                        player.CampaignCoins = System.Math.Max(player.CampaignCoins - Cost, 0);
                    }
                    break;

                case GameData.BankType.Infinite:
                    break;
            }

            Awardments.CheckForAward_Buy();
            //PlayerManager.CoinsSpent += Cost;

            SetCoins(Bank());
        }

        void SetCoins(int Coins)
        {
            //if (Coins > 99) Coins = 99;
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

        public static GameObject MakeListener()
        {
            Listener listener = new Listener();
            listener.Control = -1;
            listener.MyButton = ControllerButtons.Y;
            listener.MyButton2 = ButtonCheck.Help_KeyboardKey;
            listener.MyAction = () =>
                {
                    if (Tools.StepControl) return;

                    Level level = Tools.CurLevel;
                    if (!level.Replay && !level.Watching && !level.Finished && !level.PreventHelp)
                        listener.Call(new HelpMenu());
                };

            return listener;
        }

        int DelayExit = 29;
        public override void ReturnToCaller()
        {
            InGameStartMenu.PreventMenu = false;

            if (Active)
            {
                Active = false;
                MyGame.WaitThenDo(DelayExit, () => ReturnToCaller());
            }
            else
                base.ReturnToCaller();    
        }

        public override bool MenuReturnToCaller(Menu menu)
        {
            DelayExit = 0;

            return base.MenuReturnToCaller(menu);
        }

		public enum CostGrowthTypes { None, DoublePerBuy };
		static CostGrowthTypes CostGrowthType = CostGrowthTypes.None;
		public static int Cost_Multiplier_Watch = 1, Cost_Multiplier_Path = 1, Cost_Multiplier_Slow = 1;

		public static void SetCostGrowthType(CostGrowthTypes type)
		{
			CostGrowthType = type;

			Cost_Multiplier_Watch = 1;
			Cost_Multiplier_Path = 1;
			Cost_Multiplier_Slow = 1;
		}

		public static int CurrentCostTo_Watch
		{
			get
			{
				return Cost_Watch * CostMultiplier * Cost_Multiplier_Watch;
			}
		}

		public static int CurrentCostTo_Slow
		{
			get
			{
				return Cost_Slow * CostMultiplier * Cost_Multiplier_Slow;
			}
		}

		public static int CurrentCostTo_Path
		{
			get
			{
				return Cost_Path * CostMultiplier * Cost_Multiplier_Path;
			}
		}

		public static int CostMultiplier = 1;
        static int Cost_Watch = 5, Cost_Path = 40, Cost_Slow = 20;
        bool Allowed_WatchComputer()
        {
            return MyGame.MyLevel.WatchComputerEnabled() && Bank() >= CurrentCostTo_Watch;
        }
        void WatchComputer()
        {
            if (!Allowed_WatchComputer())
                return;

			Buy(CurrentCostTo_Watch);
			
			if (CostGrowthType == CostGrowthTypes.DoublePerBuy)
				Cost_Multiplier_Watch *= 2;

            ReturnToCaller();
            MyGame.WaitThenDo(DelayExit - 10, () => MyGame.MyLevel.WatchComputer());
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
			return MyGame.MyLevel.CanWatchComputer && Bank() >= CurrentCostTo_Path;
#endif
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
                MyGame.AddToDo(() => MyGame.RemoveAllGameObjects(match => match is ShowGuide));
            }
        }
        void ShowPath()
        {
            if (!Allowed_ShowPath())
                return;

			Buy(CurrentCostTo_Path);

			if (CostGrowthType == CostGrowthTypes.DoublePerBuy)
				Cost_Multiplier_Path *= 2;

            ReturnToCaller();
            MyGame.WaitThenDo(DelayExit - 10, () =>
                {
                    //MyGame.MyLevel.SetToReset = true;
                    Toggle_ShowPath(true);
                });
        }

        bool On_SlowMo()
        {
            return Tools.CurGameData.MyGameObjects.Any(obj => obj is SlowMo);
        }
        bool Allowed_SlowMo()
        {
			return true && Bank() >= CurrentCostTo_Slow;
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
                MyGame.AddToDo(() => MyGame.MyGameObjects.RemoveAll(match => match is SlowMo));
            }
        }
        void SlowMo()
        {
            if (!Allowed_SlowMo())
                return;

			Buy(CurrentCostTo_Slow);

			if (CostGrowthType == CostGrowthTypes.DoublePerBuy)
				Cost_Multiplier_Slow *= 2;

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

            string CoinPrefix = "{pCoin_Blue,100,?}";

            // Watch the computer
			MenuItem WatchItem = item = new MenuItem(new EzText(CoinPrefix + "x" + (CurrentCostTo_Watch).ToString(), ItemFont));
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
            MenuItem PathItem = null;
            if (On_ShowPath())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = Toggle_ShowPath;
                toggle.Toggle(true);
            }
            else
            {
				PathItem = item = new MenuItem(new EzText(CoinPrefix + "x" + (CurrentCostTo_Path).ToString(), ItemFont));
				if (Bank() >= CurrentCostTo_Path)
                    item.Go = Cast.ToItem(ShowPath);
                else
                    item.Go = null;
            }
            item.Name = "ShowPath";
            item.SetIcon(ObjectIcon.PathIcon.Clone());
            item.Icon.Pos = IconOffset + new Vector2(-20, -75);
            AddItem(item);
            item.AdditionalOnSelect = Blurb.SetText_Action(Localization.Words.ShowPath);
            Item_ShowPath = item;

            // Slow mo
            MenuItem SlowItem = null;
            if (On_SlowMo())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = Toggle_SlowMo;
                toggle.Toggle(true);
            }
            else
            {
				SlowItem = item = new MenuItem(new EzText(CoinPrefix + "x" + (CurrentCostTo_Slow).ToString(), ItemFont));
				if (Bank() >= (CurrentCostTo_Slow))
                    item.Go = Cast.ToItem(SlowMo);
                else
                    item.Go = null;
            }
            item.Name = "SlowMo";
            item.SetIcon(ObjectIcon.SlowMoIcon.Clone());
            item.Icon.Pos = IconOffset + new Vector2(-20, -55);
            AddItem(item);
            item.AdditionalOnSelect = Blurb.SetText_Action(Localization.Words.ActivateSlowMo);
            Item_SlowMo = item;

            // Fade if not usable
			if (WatchItem != null && Bank() < CurrentCostTo_Watch)
			{
				WatchItem.Go = null;
				WatchItem.MyText.Alpha = .6f;
				WatchItem.MySelectedText.Alpha = .6f;
			}

            if (PathItem != null && PathItem.Go == null)
            {
                PathItem.MyText.Alpha = .6f;
                PathItem.MySelectedText.Alpha = .6f;
            }

            if (SlowItem != null && SlowItem.Go == null)
            {
                SlowItem.MyText.Alpha = .6f;
                SlowItem.MySelectedText.Alpha = .6f;
            }


            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnY = Cast.ToAction(MenuReturnToCaller);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            EnsureFancy();
            SetPos();

			EpilepsySafe(.75f);
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("WatchComputer"); if (_item != null) { _item.SetPos = new Vector2(-1050f, 285.0002f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("ShowPath"); if (_item != null) { _item.SetPos = new Vector2(-1047.222f, -98.8887f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SlowMo"); if (_item != null) { _item.SetPos = new Vector2(-1052.777f, -499.4443f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(0f, 0f);

            EzText _t;
			_t = MyPile.FindEzText("Coins"); if (_t != null) { _t.Pos = new Vector2(-1497.222f, 615.889f); _t.Scale = 0.6593335f; }
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1497.222f, 816.3335f); _t.Scale = 0.9640832f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(22.22229f, -33.33333f); _q.Size = new Vector2(1740.553f, 1044.332f); }
            _q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(-798.1558f, 634.4669f); _q.Size = new Vector2(110.5714f, 110.5714f); }

            MyPile.Pos = new Vector2(0f, 0f);


			// Position coins
			float x = 0;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.CalcBounds(); x = CoinsText.Pos.X + _t.GetWorldWidth(); }
			CoinsText.Pos = new Vector2(x, CoinsText.Pos.Y);
			_q = MyPile.FindQuad("Coin"); if (_q != null) { _q.Pos = new Vector2(x + 10, _q.Pos.Y); }
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