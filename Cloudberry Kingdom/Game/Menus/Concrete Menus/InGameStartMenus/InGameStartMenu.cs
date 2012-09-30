using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
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

            text.Scale = FontScale * 1.45f;
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

            FontScale = .775f;

            MyPile = new DrawPile();

            this.CallDelay = 5;
            this.SlideLength = 14;
            this.SelectedItemShift = new Vector2(0, 0);

            // Make the backdrop
            QuadClass backdrop = new QuadClass("Backplate_1080x840", 1500, true);
            
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(-975.6945f, 54.86111f);

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.OnB = null;

            MenuItem item;

            MakeDarkBack();

            // Header
            EzText HeaderText = new EzText("Menu", ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = new Vector2(-1663.889f, 971.8889f);

            ItemPos = new Vector2(-1560.333f, 600f);
            PosAdd = new Vector2(0, -270);

            bool RemoveMeOption = false;
            if (PlayerManager.GetNumPlayers() > 1 && Control >= 0)
            {
                RemoveMeOption = true;
                PosAdd.Y += 44;
            }

            // Resume
            item = new MenuItem(new EzText("Resume", ItemFont));
            item.Name = "Resume";
            item.Go = Cast.ToItem(ReturnToCaller);
            item.MyText.Scale *= 1.1f;
            item.MySelectedText.Scale *= 1.1f;
            AddItem(item);
            item.SelectSound = null;

            
            // Statistics
            item = new MenuItem(new EzText("Stats", ItemFont));
            item.Name = "Stats";
            item.Go = Cast.ToItem(GoStats);
            AddItem(item);

            // SaveLoadSeed
            string text = Tools.CurLevel.CanLoadLevels ? "Save/Load" : "Save Seed";
            item = new MenuItem(new EzText(text, ItemFont));
            item.Name = "SaveLoadSeed";
            item.Go = Cast.ToItem(GoSaveLoad);
            if (!Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel)
            {
                item.Selectable = false;
                item.GrayOut();
            }
            AddItem(item);

            // Options
            item = new MenuItem(new EzText("Options", ItemFont));
            item.Name = "Options";
            item.Go = Cast.ToItem(GoOptions);
            AddItem(item);

            // Controls
            item = new MenuItem(new EzText("Controls", ItemFont));
            item.Name = "Controls";
            item.Go = Cast.ToItem(GoControls);
            AddItem(item);

            // Remove player
            if (RemoveMeOption)
            {
                item = new MenuItem(new EzText("remove me", ItemFont));
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

            SetPos();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        private void GoRemove()
        {
            VerifyRemoveMenu verify = new VerifyRemoveMenu(Control);
            Call(verify);
            Hide(PresetPos.Left);
            PauseGame = true;
        }

        private void GoControls()
        {
            MyGame.WaitThenDo(4, () =>
            {
                Hide(PresetPos.Left, 40);
                PauseGame = true;
            });
            ControlScreen screen = new ControlScreen(Control);
            Call(screen, 22);
        }

        private void GoOptions()
        {
#if PC_VERSION
            Call(new SoundMenu(Control, true), 0);
#else
            Call(new SoundMenu(Control, false), 0);
#endif
            Hide(PresetPos.Left);
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
            Hide(PresetPos.Left);
            PauseGame = true;

        }

        private void GoStats()
        {
            Call(new StatsMenu(StatGroup.Lifetime), 19);
            Hide(PresetPos.Left);
            PauseGame = true;
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Resume"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, 708.3334f); }
            _item = MyMenu.FindItemByName("Stats"); if (_item != null) { _item.SetPos = new Vector2(-1504.778f, 469.9999f); }
            _item = MyMenu.FindItemByName("SaveLoadSeed"); if (_item != null) { _item.SetPos = new Vector2(-1504.777f, 231.6667f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-1496.443f, -3.88887f); }
            _item = MyMenu.FindItemByName("Controls"); if (_item != null) { _item.SetPos = new Vector2(-1501.999f, -258.8889f); }
            _item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(-1499.222f, -502.7777f); }

            MyMenu.Pos = new Vector2(1109.028f, 20.13885f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1463.89f, 1474.667f); }

            QuadClass _q;
            _q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(-989.5837f, -0.6944637f); _q.Size = new Vector2(1198.763f, 932.3713f); }

            MyPile.Pos = new Vector2(995.1394f, -13.19449f);
        }

        protected virtual void MakeExitItem()
        {
            MenuItem item = new MenuItem(new EzText("Exit level", ItemFont));
            item.Go = Cast.ToItem(VerifyExit);

            AddItem(item);
        }

        private void VerifyExit()
        {
            Call(new VerifyQuitLevelMenu(Control), 0);
            Hide(PresetPos.Left);
            PauseGame = true;
        }

        protected override void MyDraw()
        {
            Pos.Update();

            base.MyDraw();
        }
    }
}