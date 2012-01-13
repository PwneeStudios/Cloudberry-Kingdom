using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class InGameStartMenu_CampaignLevel : InGameStartMenu
    {
        public InGameStartMenu_CampaignLevel(int Control) : base(Control) { }

        protected override void MakeExitItem()
        {
            MenuItem item = new MenuItem(new EzText("Exit to menu", ItemFont));
            item.Go = menuitem => Call(new VerifyQuitLevelMenu(Control, "Exit to menu?"), 0);

            AddItem(item);
        }

        public static new GameObject MakeListener()
        {
            return MakeListener_Base(listener =>
                new InGameStartMenu_Campaign(listener.TriggeringPlayerIndex));
        }
    }

    public class InGameStartMenu_Campaign : InGameStartMenu
    {
        public InGameStartMenu_Campaign(int Control) : base(Control) { }

        protected override void MakeExitItem()
        {
            //MenuItem item = new MenuItem(new EzText("Exit campaign", ItemFont));
            //item.Go = menuitem => Call(new VerifyQuitLevelMenu(Control, "Exit campaign?"), 0);

            MenuItem item = new MenuItem(new EzText("Exit to menu", ItemFont));
            item.Go = menuitem => Call(new VerifyQuitLevelMenu(Control, "Exit to menu?"), 0);

            AddItem(item);
        }

        public static new GameObject MakeListener()
        {
            return MakeListener_Base(listener =>
                new InGameStartMenu_Campaign(listener.TriggeringPlayerIndex));
        }
    }

    public class InGameStartMenu : StartMenuBase
    {
        public static bool PreventMenu = false;

        public InGameStartMenu(int Control) : base(false)
        {
            this.Control = Control;

            Constructor();
        }

        public static GameObject MakeListener()
        {
            return MakeListener_Base(listener => 
                new InGameStartMenu(listener.TriggeringPlayerIndex));
        }

        public static GameObject MakeListener_Base(Func<Listener, GUI_Panel> Make)
        {
            Listener listener = new Listener();
            listener.MyButton = ControllerButtons.Start;

            listener.Tags += GameObject.Tag.RemoveOnLevelFinish;

            listener.MyAction = () =>
            {
                if (!PreventMenu)
                    listener.Call(Make(listener));
            };

            return listener;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

            if (MyMenu.CurItem == RemoveMe)
                ReturnToCaller(false);
        }

        MenuItem RemoveMe;
        public override void Init()
        {
            base.Init();

            PauseGame = true;

            CallDelay = 15;

            //FontScale = .73f;
            FontScale = .8f;

            MyPile = new DrawPile();

            //RightPanel = Blurb = new BlurbBerry();

            this.CallDelay = 5;
            this.SlideLength = 20;
            this.SelectedItemShift = new Vector2(0, 0);
            //this.SlideInFrom = PresetPos.Right;

            // Make the backdrop
            QuadClass backdrop = new QuadClass("WoodMenu_1", 1500, true);
            MyPile.Add(backdrop);
            backdrop.Pos =
                //new Vector2(-1777.778f, 30.55557f);
                new Vector2(-975.6945f, 54.86111f);

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.OnB = null;

            MenuItem item;

            // Header
            EzText HeaderText = new EzText("Menu", ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            //HeaderText.Pos = new Vector2(616.5557f, 941.3334f);
            HeaderText.Pos = new Vector2(-1663.889f, 971.8889f);



            ItemPos = new Vector2(-1560.333f, 600f);
            PosAdd = new Vector2(0, -280);

            bool RemoveMeOption = false;
            if (PlayerManager.GetNumPlayers() > 1 && Control >= 0)
            {
                RemoveMeOption = true;
                PosAdd.Y += 44;
            }

            // Resume
            item = new MenuItem(new EzText("Resume", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = menuitem => ReturnToCaller();
            item.MyText.Scale *= 1.1f;
            item.MySelectedText.Scale *= 1.1f;
            AddItem(item);
            item.SelectSound = null;

            
            // Statistics
            item = new MenuItem(new EzText("Stats", ItemFont));
            //item.Go = menuitem => Call(new ResumeScreen(), 12);
            item.Go = menuitem =>
                {
                    Call(new StatsMenu(StatGroup.Lifetime), 19);//22);
                    Hide(PresetPos.Left);//, 40);
                    PauseGame = true;
                };
            AddItem(item);

            // Options
            item = new MenuItem(new EzText("Options", ItemFont));
            item.Go = menuitem =>
                {
#if PC_VERSION
                    Call(new SoundMenu(Control, true), 0);
#else
                    Call(new SoundMenu(Control, false), 0);
#endif
                };
            AddItem(item);

            // Controls
            item = new MenuItem(new EzText("Controls", ItemFont));
            item.Go = menuitem =>
                {
                    MyGame.WaitThenDo(4, () =>
                        {
                            Hide(PresetPos.Left, 40);
                            PauseGame = true;
                        });
                    ControlScreen screen = new ControlScreen(Control);
                    Call(screen, 22);
                };
            AddItem(item);

            // Remove player
            if (RemoveMeOption)
            {
                item = new MenuItem(new EzText("remove me", ItemFont));
                item.Go = _item =>
                {
                    VerifyRemoveMenu verify = new VerifyRemoveMenu(Control);
                    Call(verify);
                };
                AddItem(item);
                RemoveMe = item;
            }

            // Exit level
            MakeExitItem();

            // Button interactions
            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            // Shift everything
            EnsureFancy();
            Shift(new Vector2(1045.139f, -10.41669f));
        }

        protected virtual void MakeExitItem()
        {
            MenuItem item = new MenuItem(new EzText("Exit level", ItemFont));
            item.Go = menuitem => Call(new VerifyQuitLevelMenu(Control), 0);

            AddItem(item);
        }

        protected override void MyDraw()
        {
            Pos.Update();

            base.MyDraw();
        }
    }
}