using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class InGameStartMenu : CkBaseMenu
    {
        public static bool PreventMenu = false;

        public InGameStartMenu(int Control) : base(false)
        {
            EnableBounce();

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
                PlayerData Player = null;
                if (listener.TriggeringPlayerIndex >= 0)
                    Player = PlayerManager.Get(listener.TriggeringPlayerIndex);

                bool PlayerExists = false;
                if (Player != null && Player.Exists) PlayerExists = true;

                if (!PlayerExists && PlayerManager.NumExistingPlayers() == 0)
                    PlayerExists = true;

                //if (!PreventMenu)
                if (!PreventMenu && PlayerExists)
                    listener.Call(Make(listener));
            };

            return listener;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            MyMenu.Control = Control;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.45f;
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

            if (MyMenu.CurItem == RemoveMe && VerifyRemoveMenu.YesChosen)
                ReturnToCaller(false);
        }

        MenuItem RemoveMe;
        public override void Init()
        {
            base.Init();

            PauseGame = true;

            CallDelay = 15;

            FontScale = .775f;

            MyPile = new DrawPile();

            this.CallDelay = 5;
            this.SlideLength = 14;
            this.SelectedItemShift = new Vector2(0, 0);

            MakeDarkBack();

            // Make the backdrop
            QuadClass backdrop; 
            if (UseBounce)
                backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
            else
                backdrop = new QuadClass("Backplate_1080x840", 1500, true);

            backdrop.Name = "Backdrop";

            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(-975.6945f, 54.86111f);

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.OnB = null;

            MenuItem item;

            ItemPos = new Vector2(-1560.333f, 600f);
            PosAdd = new Vector2(0, -270);

            bool RemoveMeOption = false;
            if (PlayerManager.GetNumPlayers() > 1 && Control >= 0)
            {
                RemoveMeOption = true;
                PosAdd.Y += 44;
            }

            // Resume
            item = new MenuItem(new EzText(Localization.Words.Resume, ItemFont));
            item.Name = "Resume";
            item.Go = Cast.ToItem(ReturnToCaller);
            item.MyText.Scale *= 1.1f;
            item.MySelectedText.Scale *= 1.1f;
            AddItem(item);
            item.SelectSound = null;
            
            // Statistics
            item = new MenuItem(new EzText(Localization.Words.Statistics, ItemFont));
            item.Name = "Stats";
            item.Go = Cast.ToItem(GoStats);
            AddItem(item);

            // SaveLoadSeed
            Localization.Words word = Tools.CurLevel.CanLoadLevels ? Localization.Words.SaveLoad : Localization.Words.SaveSeed;
            item = new MenuItem(new EzText(word, ItemFont));
            item.Name = "SaveLoadSeed";
            item.Go = Cast.ToItem(GoSaveLoad);
            if (!Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel)
            {
                item.Selectable = false;
                item.GrayOut();
            }
            AddItem(item);

            // Options
            item = new MenuItem(new EzText(Localization.Words.Options, ItemFont));
            item.Name = "Options";
            item.Go = Cast.ToItem(GoOptions);
            AddItem(item);

            //// Controls
            //item = new MenuItem(new EzText(Localization.Words.Controls, ItemFont));
            //item.Name = "Controls";
            //item.Go = Cast.ToItem(GoControls);
            //AddItem(item);

            // Remove player
            if (RemoveMeOption)
            {
                item = new MenuItem(new EzText(Localization.Words.RemoveMe, ItemFont));
                item.Name = "Remove";
                item.Go = Cast.ToItem(GoRemove);
                AddItem(item);
                RemoveMe = item;
            }

            // Exit level
            MakeExitItem();

            // Button interactions
            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Shift everything
            EnsureFancy();
            Shift(new Vector2(1045.139f, -10.41669f));

            if (RemoveMeOption)
                SetPos_WithRemoveMe();
            else
                SetPos();

            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        private void GoRemove()
        {
            VerifyRemoveMenu verify = new VerifyRemoveMenu(Control);
            Call(verify);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        private void GoControls()
        {
            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                MyGame.WaitThenDo(4, () =>
                {
                    Hide(PresetPos.Left, 40);
                    PauseGame = true;
                });
            }

            ControlScreen screen = new ControlScreen(Control);
            Call(screen, 22);
        }

        private void GoOptions()
        {
            Call(new SoundMenu(Control, false), 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        private void GoSaveLoad()
        {
            // If this isn't a PC, and we can't load seeds right now, then go directly to the SaveAs menu.
#if !PC_VERSION
            if (!MyLevel.CanLoadLevels)
            {
                Call(new SaveSeedAs(Control, MenuItem.GetActivatingPlayerData()), 0);
            }
            else
#endif
            {
#if PC_VERSION
                Call(new SaveLoadSeedMenu(Control, MyLevel.CanLoadLevels, MyLevel.CanSaveLevel), 0);
#else
                Call(new SaveLoadSeedMenu(Control, MyLevel.CanLoadLevels, MyLevel.CanSaveLevel), 0);
#endif
            }

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;

        }

        private void GoStats()
        {
            Call(new StatsMenu(StatGroup.Lifetime), 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1496.444f, -252.7777f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1109.028f, -40.97224f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-972.9169f, 29.86109f); _q.Size = new Vector2(1132.148f, 880.288f); }

            MyPile.Pos = new Vector2(995.1394f, -13.19449f);
        }

        void SetPos_WithRemoveMe()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -488.8888f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Remove"); if (_item != null) { _item.SetPos = new Vector2(-1488.11f, -249.4445f); _item.MyText.Scale = 0.775f; _item.MySelectedText.Scale = 0.775f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1106.25f, 50.69439f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1463.89f, 1474.667f); _t.Scale = 1.12375f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Dark"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(8888.889f, 5000f); }
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-942.361f, 27.08332f); _q.Size = new Vector2(1167.945f, 908.121f); }

            MyPile.Pos = new Vector2(995.1394f, -13.19449f);
        }

        protected virtual void MakeExitItem()
        {
            MenuItem item = new MenuItem(new EzText(Localization.Words.ExitLevel, ItemFont));
            item.Go = Cast.ToItem(VerifyExit);
            item.Name = "Exit";
            AddItem(item);
        }

        private void VerifyExit()
        {
            Call(new VerifyQuitLevelMenu(Control), 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
            else
            {
                Hide(PresetPos.Left);
            }

            PauseGame = true;
        }

        protected override void MyDraw()
        {
            Pos.Update();

            base.MyDraw();
        }
    }
}