using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class HelpMenu : StartMenuBase
    {
        int Bank()
        {
            return
                PlayerManager.PlayerSum(p => p.CampaignStats.Coins) +
                PlayerManager.PlayerSum(p => p.GameStats.Coins) +
                PlayerManager.PlayerSum(p => p.LevelStats.Coins)
                - PlayerManager.CoinsSpent;
        }
        void Buy(int Cost)
        {
            PlayerManager.CoinsSpent += Cost;
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
            Hints.YForHelp = 999;
        }

        public static GameObject MakeListener()
        {
            Listener listener = new Listener();
            //listener.MyType = Listener.Type.OnDown;
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

        int Cost_Watch = 0, Cost_Path = 30, Cost_Slow = 10;
        bool Allowed_WatchComputer()
        {
            return MyGame.MyLevel.WatchComputerEnabled() && Bank() >= Cost_Watch;
        }
        void WatchComputer()
        {
            if (!Allowed_WatchComputer())
                return;

            Buy(Cost_Watch);

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
            return MyGame.MyLevel.CanWatchComputer && Bank() >= Cost_Path;
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

            Buy(Cost_Path);

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
            return true && Bank() >= Cost_Slow;
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

            Buy(Cost_Slow);

            Toggle_SlowMo(true);
            ReturnToCaller();
        }

        public override void OnAdd()
        {
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
        public override void Init()
        {
            base.Init();

            GameData game = Tools.CurGameData;

            PauseGame = true;

            //FontScale = .73f;
            FontScale = .8f;

            MyPile = new DrawPile();

            RightPanel = Blurb = new HelpBlurb();

            this.CallDelay = 3;
            this.SlideLength = 30;
            this.SelectedItemShift = new Vector2(0, 0);
            //this.SlideInFrom = PresetPos.Right;

            // Make the backdrop
            QuadClass backdrop = new QuadClass("WoodMenu_1", 1500);
            MyPile.Add(backdrop);
            //backdrop.Pos = new Vector2(1777.778f, 30.55557f);
            backdrop.Pos = new Vector2(-1777.778f, 30.55557f);


            // Coin
            QuadClass Coin = new QuadClass("CoinBlue", 90, true);
            //Coin.PointxAxisTo(20 / Tools.c);
            Coin.Pos = new Vector2(-873.1558f, 770.5778f);
            MyPile.Add(Coin);

            CoinsText = new EzText("x", Tools.Font_DylanThin42, 450, false, true);
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
            //EzText HeaderText = new EzText("Power Ups!", ItemFont);
            EzText HeaderText = new EzText("Coins", ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            //HeaderText.Pos = new Vector2(616.5557f, 941.3334f);
            HeaderText.Pos = new Vector2(-1663.889f, 971.8889f);


            //Vector2 IconOffset = new Vector2(-200, 0);
            Vector2 IconOffset = new Vector2(-150, 0);

            string CoinPrefix = "{pCoinBlue,68,?} ";

            // Watch the computer
            //item = new MenuItem(new EzText("x10", ItemFont));
            item = new MenuItem(new EzText(CoinPrefix + "x " + Cost_Watch.ToString(), ItemFont));
            Item_WatchComputer = item;
            item.SetIcon(ObjectIcon.RobotIcon.Clone());
            item.Icon.Pos = IconOffset;
            item.Go = menuitem => WatchComputer();
            //ItemPos = new Vector2(1072.222f, 510f);
            ItemPos = new Vector2(-1033.333f, 429.4446f);
            PosAdd = new Vector2(0, -520);
            AddItem(item);
            //item.Pos = item.SelectedPos = new Vector2(769.4453f, -270.5556f);
            item.AdditionalOnSelect = () => Blurb.SetText("Watch the computer to see how it's done!");

            // Show path
            if (On_ShowPath())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = Toggle_ShowPath;
                toggle.Toggle(true);
            }
            else
            {
                item = new MenuItem(new EzText(CoinPrefix + "x " + Cost_Path.ToString(), ItemFont));
                item.Go = menuitem => ShowPath();
            }
            item.SetIcon(ObjectIcon.PathIcon.Clone());
            item.Icon.Pos = IconOffset;            
            AddItem(item);
            item.AdditionalOnSelect = () => Blurb.SetText("Show a path through the level while you play.");
            Item_ShowPath = item;

            // Slow mo
            if (On_SlowMo())
            {
                item = toggle = new MenuToggle(ItemFont);
                toggle.OnToggle = Toggle_SlowMo;
                toggle.Toggle(true);
            }
            else
            {
                item = new MenuItem(new EzText(CoinPrefix + "x " + Cost_Slow.ToString(), ItemFont));
                item.Go = menuitem => { SlowMo(); }; //menuitem.MyOscillateParams.SetType(OscillateParams.Type.Jiggle); };
            }
            item.SetIcon(ObjectIcon.SlowMoIcon.Clone());
            item.Icon.Pos = IconOffset;            
            AddItem(item);
            item.AdditionalOnSelect = () => Blurb.SetText(Text_SlowMo());
            Item_SlowMo = item;

            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnY = () => MenuReturnToCaller(MyMenu);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }

        private static string Text_SlowMo()
        {
            return string.Format("Activate {0}Slow\nMotion{1}! Toggle\n with {2}.", EzText.ColorToMarkup(205, 10, 10, -35, null), EzText.ColorToMarkup(Color.White, -35, null), ButtonString.X(86));
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

#if PC_VERSION
            item.Padding += new Vector2(20, 40);
#endif
        }
    }
}