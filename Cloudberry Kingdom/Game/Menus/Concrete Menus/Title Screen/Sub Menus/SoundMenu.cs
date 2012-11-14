using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CloudberryKingdom
{
    public class SoundMenu : VerifyBaseMenu
    {
        public SoundMenu(int Control) : base(false)
        {
            this.Control = Control;
            FixedToCamera = true;

            Constructor();
        }

        class InitOnToggleHelper : Lambda_1<bool>
        {
            public void Apply(bool state)
            {
                PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
                Tools.Fullscreen = state;
                SaveGroup.SaveAll();
                PlayerManager.SaveRezAndKeys();
            }
        }

        EzText HeaderText;
        public override void Init()
        {
            base.Init();
            this.CallToLeft = true;

            this.FontScale *= .9f;

            // Header
            HeaderText = new EzText(Localization.Words.Options, ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);

            MenuSlider FxSlider = new MenuSlider(new EzText(Localization.Words.SoundVolume, ItemFont));
            FxSlider.MyFloat = Tools.SoundVolume;
            FxSlider.Name = "Sound";
            AddItem(FxSlider);

            MenuSlider MusicSlider = new MenuSlider(new EzText(Localization.Words.MusicVolume, ItemFont));
            MusicSlider.MyFloat = Tools.MusicVolume;
            MusicSlider.Name = "Music";
            AddItem(MusicSlider);

            MenuItem item = new MenuItem(new EzText(Localization.Words.Controls, ItemFont));
            item.Go = _item =>
                {
                    Hide();
                    Call(new ControlScreen(Control), 10);
                };
            item.Name = "Controls";
            AddItem(item);

#if PC_VERSION
            // Custom controls
            var mitem = new MenuItem(new EzText(Localization.Words.EditControls, ItemFont));
            mitem.Go = menuitem => Call(new CustomControlsMenu(), 10);
            mitem.Name = "Custom";
            AddItem(mitem);

            // Full screen resolutions
            var RezText = new EzText(Localization.Words.Resolution, ItemFont);
            SetHeaderProperties(RezText);
            RezText.Name = "RezText";
            MyPile.Add(RezText);

            MenuList FsRezList = new MenuList();
            FsRezList.Name = "RezList";
            FsRezList.Center = false;
            FsRezList.MyExpandPos = new Vector2(-498.1506f, 713.873f);
            int i = 0;
            int CurRez = 0;

            // Get viable resolutions
            List<DisplayMode> modes = new List<DisplayMode>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (modes.Any(existing => existing.Width == mode.Width && existing.Height == mode.Height))
                    continue;
                else
                    modes.Add(mode);
            }

            // Add resolutions to the current list
            bool found = false;
            foreach (var mode in modes)
            {
                string str = mode.Width + " x " + mode.Height;
                Tools.Write(str);
                item = new MenuItem(new EzText(str, ItemFont, false, true));
                SetItemProperties(item);
                FsRezList.AddItem(item, mode);

                if (mode.Width == Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth &&
                    mode.Height == Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight)
                {
                    CurRez = i;
                    found = true;
                }
                else if (!found && mode.Width == Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth)
                    CurRez = i;

                i++;
            }
            AddItem(FsRezList);
            FsRezList.SetIndex(CurRez);
            FsRezList.OnConfirmedIndexSelect = () =>
            {
                PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
                ResolutionGroup.Use(FsRezList.CurObj as DisplayMode);
                SaveGroup.SaveAll();
                PlayerManager.SaveRezAndKeys();
            };

            // Full screen toggle
            var FullScreenText = new EzText(Localization.Words.FullScreen, ItemFont);
            SetHeaderProperties(FullScreenText);
            FullScreenText.Name = "Fullscreen";
            MyPile.Add(FullScreenText);

            var toggle = new MenuToggle(ItemFont);
            toggle.OnToggle = new InitOnToggleHelper();
            toggle.Name = "FullscreenToggle";
            toggle.Toggle(Tools.Fullscreen);

            AddItem(toggle);

            //AddToggle_FixedTimestep();
            AddToggle_Borderless();
#endif

            MakeBackButton();
            SetPosition();

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }

#if PC_VERSION
        //private void AddToggle_FixedTimestep()
        //{
        //    // Header
        //    var Text = new EzText(Localization.Words.FixedTimeStep, ItemFont);
        //    SetHeaderProperties(Text);
        //    MyPile.Add(Text);
        //    Text.Pos = new Vector2(-1232.142f, -499.9359f);
        //    Text.Scale *= .9f;

        //    // Menu item
        //    var Toggle = new MenuToggle(ItemFont);
        //    Toggle.OnToggle = Toggle_FixedTimestep;

        //    Toggle.Toggle(Tools.FixedTimeStep);
        //    Toggle.PrefixText = "";
        //    AddItem(Toggle);
        //    Toggle.SetPos = new Vector2(1315.078f, -451.4125f);
        //}

        //private void Toggle_FixedTimestep(bool state)
        //{
        //    PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
        //    Tools.FixedTimeStep = state;
        //    SaveGroup.SaveAll();
        //    PlayerManager.SaveRezAndKeys();
        //}

        private void AddToggle_Borderless()
        {
            // Text
            var Text = new EzText(Localization.Words.WindowBorder, ItemFont);
            SetHeaderProperties(Text);
            Text.Name = "WindowBorder";
            MyPile.Add(Text);
            Text.Pos = new Vector2(-1232.142f, -499.9359f);
            Text.Scale *= .9f;

            // Toggle
            var Toggle = new MenuToggle(ItemFont);
            Toggle.OnToggle = new Toggle_BorderlessProxy(this);
            Toggle.Toggle(Tools.WindowBorder);
            Toggle.Name = "WindowBorderToggle";
            AddItem(Toggle);
            Toggle.SetPos = new Vector2(1315.078f, -451.4125f);
        }

        class Toggle_BorderlessProxy : Lambda_1<bool>
        {
            SoundMenu sm;

            public Toggle_BorderlessProxy(SoundMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(bool state)
            {
                sm.Toggle_Borderless(state);
            }
        }

        private void Toggle_Borderless(bool state)
        {
            PlayerManager.SavePlayerData.ResolutionPreferenceSet = true;
            Tools.WindowBorder = state;
            SaveGroup.SaveAll();
            PlayerManager.SaveRezAndKeys();

            Tools.GameClass.SetBorder(Tools.WindowBorder);
        }
#endif

        public override bool MenuReturnToCaller(Menu menu)
        {
#if PC_VERSION
            PlayerManager.SaveRezAndKeys();
#endif

            return base.MenuReturnToCaller(menu);
        }

        private void SetPosition()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Sound"); if (_item != null) { _item.SetPos = new Vector2(3.173767f, 751.4761f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); ((MenuSlider)_item).SliderShift = new Vector2(1611.11f, -152.7778f); }
            _item = MyMenu.FindItemByName("Music"); if (_item != null) { _item.SetPos = new Vector2(64.28528f, 534.286f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); ((MenuSlider)_item).SliderShift = new Vector2(1552.777f, -150.0001f); }
            _item = MyMenu.FindItemByName("Controls"); if (_item != null) { _item.SetPos = new Vector2(596.8245f, 325.825f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Custom"); if (_item != null) { _item.SetPos = new Vector2(591.6658f, 133.6347f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("RezList"); if (_item != null) { _item.SetPos = new Vector2(1019.047f, -256.5245f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("FullscreenToggle"); if (_item != null) { _item.SetPos = new Vector2(1245.634f, -281.9681f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("WindowBorderToggle"); if (_item != null) { _item.SetPos = new Vector2(1315.078f, -451.4125f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(1603.173f, -621.111f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-1007.934f, -43.651f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-967.064f, 951.6506f); _t.Scale = 0.864f; }
            _t = MyPile.FindEzText("RezText"); if (_t != null) { _t.Pos = new Vector2(-1173.81f, -174.9373f); _t.Scale = 0.7776f; }
            _t = MyPile.FindEzText("Fullscreen"); if (_t != null) { _t.Pos = new Vector2(-1190.475f, -338.825f); _t.Scale = 0.7776f; }
            _t = MyPile.FindEzText("WindowBorder"); if (_t != null) { _t.Pos = new Vector2(-1232.142f, -499.9359f); _t.Scale = 0.7776f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-18.6521f, -10.31725f); _q.Size = new Vector2(1376.984f, 1077.035f); }

            MyPile.Pos = new Vector2(29.76172f, 21.82541f);
        }

        public override void OnAdd()
        {
 	        base.OnAdd();

            SetPosition();
        }
    }
}