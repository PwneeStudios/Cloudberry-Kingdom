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
            this.Control = Control;

            Constructor();
        }

        class MakeListenerHelper : LambdaFunc_1<Listener, GUI_Panel>
        {
            public GUI_Panel Apply(Listener listener)
            {
                return new InGameStartMenu(listener.TriggeringPlayerIndex);
            }
        }

        public static GameObject MakeListener()
        {
            return MakeListener_Base(new MakeListenerHelper());
        }

        class PreventMenuHelper : Lambda
        {
            Listener listener;
            LambdaFunc_1<Listener, GUI_Panel> Make;

            public PreventMenuHelper(Listener listener, LambdaFunc_1<Listener, GUI_Panel> Make)
            {
                this.listener = listener;
                this.Make = Make;
            }

            public void Apply()
            {
                if (!InGameStartMenu.PreventMenu)
                    listener.Call(Make.Apply(listener));
            }
        }

        public static GameObject MakeListener_Base(LambdaFunc_1<Listener, GUI_Panel> Make)
        {
            Listener listener = new Listener();
            listener.MyButton = ControllerButtons.Start;

            listener.Tags += GameObject.Tag.RemoveOnLevelFinish;

            listener.MyAction = new PreventMenuHelper(listener, Make);

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

            MakeDarkBack();

            // Make the backdrop
            QuadClass backdrop = new QuadClass("Backplate_1080x840", 1500, true);
            
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(-975.6945f, 54.86111f);

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.OnB = null;

            MenuItem item;

            // Header
            EzText HeaderText = new EzText(Localization.Words.Menu, ItemFont);
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
            item = new MenuItem(new EzText(Localization.Words.Resume, ItemFont));
            item.Name = "Resume";
            item.Go = Cast.ToItem(new ReturnToCallerProxy(this));
            item.MyText.Scale *= 1.1f;
            item.MySelectedText.Scale *= 1.1f;
            AddItem(item);
            item.SelectSound = null;

            
            // Statistics
            item = new MenuItem(new EzText(Localization.Words.Statistics, ItemFont));
            item.Name = "Stats";
            item.Go = Cast.ToItem(new GoStatsProxy(this));
            AddItem(item);

            // SaveLoadSeed
            Localization.Words word = Tools.CurLevel.CanLoadLevels ? Localization.Words.SaveLoad : Localization.Words.SaveSeed;
            item = new MenuItem(new EzText(word, ItemFont));
            item.Name = "SaveLoadSeed";
            item.Go = Cast.ToItem(new GoSaveLoadProxy(this));
            if (!Tools.CurLevel.CanLoadLevels && !Tools.CurLevel.CanSaveLevel)
            {
                item.Selectable = false;
                item.GrayOut();
            }
            AddItem(item);

            // Options
            item = new MenuItem(new EzText(Localization.Words.Options, ItemFont));
            item.Name = "Options";
            item.Go = Cast.ToItem(new GoOptionsProxy(this));
            AddItem(item);

            // Controls
            item = new MenuItem(new EzText(Localization.Words.Controls, ItemFont));
            item.Name = "Controls";
            item.Go = Cast.ToItem(new GoControlsProxy(this));
            AddItem(item);

            // Remove player
            if (RemoveMeOption)
            {
                item = new MenuItem(new EzText(Localization.Words.RemoveMe, ItemFont));
                item.Name = "Remove";
                item.Go = Cast.ToItem(new GoRemoveProxy(this));
                AddItem(item);
                RemoveMe = item;
            }

            // Exit level
            MakeExitItem();

            // Button interactions
            MyMenu.OnStart = MyMenu.OnX = MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            // Shift everything
            EnsureFancy();
            Shift(new Vector2(1045.139f, -10.41669f));

            SetPos();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        class GoRemoveProxy : Lambda
        {
            InGameStartMenu igsm;

            public GoRemoveProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.GoRemove();
            }
        }

        private void GoRemove()
        {
            VerifyRemoveMenu verify = new VerifyRemoveMenu(Control);
            Call(verify);
            Hide(PresetPos.Left);
            PauseGame = true;
        }

        class GoControlsHelper : Lambda
        {
            InGameStartMenu igsm;

            public GoControlsHelper(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.Hide(PresetPos.Left, 40);
                igsm.PauseGame = true;
            }
        }

        class GoControlsProxy : Lambda
        {
            InGameStartMenu igsm;

            public GoControlsProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.GoControls();
            }
        }

        private void GoControls()
        {
            MyGame.WaitThenDo(4, new GoControlsHelper(this));
            ControlScreen screen = new ControlScreen(Control);
            Call(screen, 22);
        }

        class GoOptionsProxy : Lambda
        {
            InGameStartMenu igsm;

            public GoOptionsProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.GoOptions();
            }
        }

        private void GoOptions()
        {
            Call(new SoundMenu(Control), 0);

            Hide(PresetPos.Left);
            PauseGame = true;
        }

        class GoSaveLoadProxy : Lambda
        {
            InGameStartMenu igsm;

            public GoSaveLoadProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.GoSaveLoad();
            }
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

        class GoStatsProxy : Lambda
        {
            InGameStartMenu igsm;

            public GoStatsProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.GoStats();
            }
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
            MenuItem item = new MenuItem(new EzText(Localization.Words.ExitLevel, ItemFont));
            item.Go = Cast.ToItem(new VerifyExitProxy(this));

            AddItem(item);
        }

        class VerifyExitProxy : Lambda
        {
            InGameStartMenu igsm;

            public VerifyExitProxy(InGameStartMenu igsm)
            {
                this.igsm = igsm;
            }

            public void Apply()
            {
                igsm.VerifyExit();
            }
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