using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

namespace CloudberryKingdom
{
    public class SaveLoadSeedMenu : VerifyBaseMenu
    {
        public SaveLoadSeedMenu(int Control, bool CanLoad, bool CanSave)
            : base(false)
        {
            this.CallToLeft = true;
            this.Control = Control;
            FixedToCamera = true;

            this.CanLoad = CanLoad;
            this.CanSave = CanSave;

            Constructor();
        }

        bool CanLoad, CanSave;

        PlayerData player;

        EzText HeaderText;
        public override void Init()
        {
            base.Init();

            this.FontScale *= .9f;

            // Get the activating player
            player = MenuItem.GetActivatingPlayerData();

            // Header
            HeaderText = new EzText(Localization.Words.RandomSeed, ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;

            MenuItem item;

            if (CanSave)
            {
                // Save seed
                item = new MenuItem(new EzText(Localization.Words.SaveSeed, ItemFont));
                item.Name = "Save";
                item.Go = MakeSave(this, player);
                AddItem(item);
            }

            if (CanLoad)
            {
                // Load seed
                item = new MenuItem(new EzText(Localization.Words.LoadSeed, ItemFont));
                item.Name = "Load";
                item.Go = new LoadProxy(this);
                AddItem(item);
            }

#if WINDOWS
            if (CanSave)
            {
                // Copy seed
                item = new MenuItem(new EzText(Localization.Words.CopyToClipboard, ItemFont));
                item.Name = "Copy";
                item.Go = new CopyProxy(this);
                AddItem(item);
            }

            if (CanLoad)
            {
                // Load seed from string
                item = new MenuItem(new EzText(Localization.Words.LoadFromClipboard, ItemFont));
                item.Name = "LoadString";
                item.Go = new LoadStringProxy(this);
                AddItem(item);
            }
#endif
            MakeBackButton();

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);
        }

        private void SetPosition()
        {
#if PC_VERSION
            if (CanLoad && CanSave)
            {
                MenuItem _item;
                _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(686.1115f, 633.2222f); }
                _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(694.4447f, 441.5555f); }
                _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(672.2223f, 222.1111f); }
                _item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(672.2225f, 27.66663f); }
                _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(727.7777f, -163.9999f); }

                MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

                MyPile.FindEzText("Header").Pos = new Vector2(402.7776f, 871.8887f);

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1500f, 803.2258f); }

                MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
            }
            else if (CanLoad && !CanSave)
            {
                MenuItem _item;
                _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(686.1115f, 633.2222f); }
                _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(694.4447f, 441.5555f); }
                _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(672.2223f, 222.1111f); }
                _item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(672.2225f, 27.66663f); }
                _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(727.7777f, -163.9999f); }

                MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

                MyPile.FindEzText("Header").Pos = new Vector2(402.7776f, 871.8887f);

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1500f, 803.2258f); }

                MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
            }
            else
            {
                MenuItem _item;
                _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(686.1115f, 499.889f); }
                _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(674.9999f, 269.3333f); }
                _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(719.4445f, 47.11124f); }

                MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

                EzText _t;
                _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(425f, 807.9997f); }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1224.558f, 736.7258f); }

                MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
            }
#else
            MenuItem _item;
            _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(686.1115f, 633.2222f); }
            _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(694.4447f, 441.5555f); }
            _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(672.2223f, 222.1111f); }
            _item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(672.2225f, 27.66663f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(727.7777f, -163.9999f); }

            MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

            MyPile.FindEzText("Header").Pos = new Vector2(402.7776f, 871.8887f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1500f, 803.2258f); }

            MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
#endif
        }

#if WINDOWS
        class MakeSaveHelper : Lambda_1<MenuItem>
        {
            GUI_Panel panel;
            PlayerData player;

            public MakeSaveHelper(GUI_Panel panel, PlayerData player)
            {
                this.panel = panel;
                this.player = player;
            }

            public void Apply(MenuItem _item)
            {
                SaveLoadSeedMenu.Save(_item, panel, player);
            }
        }

        public static Lambda_1<MenuItem> MakeSave(GUI_Panel panel, PlayerData player)
        {
            return new MakeSaveHelper(panel, player);
        }

        IAsyncResult kyar;
        static void Save(MenuItem _item, GUI_Panel panel, PlayerData player)
        {
            SaveSeedAs SaveAs = new SaveSeedAs(panel.Control, player);
            panel.Call(SaveAs, 0);
        }
#else
        public static MenuItemGo MakeSave(GUI_Panel panel, PlayerData player)
        {
            return _item => Save(_item, player);
        }

        static IAsyncResult kyar;
        static PlayerData _player;
        static void Save(MenuItem _item, PlayerData activeplayer)
        {
            _player = activeplayer;
            kyar = Guide.BeginShowKeyboardInput(_player.MyPlayerIndex, "Save random seed as...", "Choose a name to save this level as.",
                    Tools.CurLevel.MyLevelSeed.SuggestedName(), OnKeyboardComplete, null);
        }

        static void OnKeyboardComplete(IAsyncResult ar)
        {
            // Get the input from the virtual keyboard
            string input = Guide.EndShowKeyboardInput(kyar);

            if (input == null) return;

            // Strip anything after a semicolon (because this will confuse the seed parser)
            if (input.Contains(";"))
            {
                input = input.Substring(0, input.IndexOf(";"));
            }

            // Save the seed
            _player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), input);
        }
#endif

        class LoadProxy : Lambda_1<MenuItem>
        {
            SaveLoadSeedMenu slsm;

            public LoadProxy(SaveLoadSeedMenu slsm)
            {
                this.slsm = slsm;
            }

            public void Apply(MenuItem _item)
            {
                slsm.Load(_item);
            }
        }

        void Load(MenuItem _item)
        {
            SavedSeedsGUI LoadMenu = new SavedSeedsGUI();
            Call(LoadMenu, 0);
        }

#if WINDOWS

        class CopyProxy : Lambda_1<MenuItem>
        {
            SaveLoadSeedMenu slsm;

            public CopyProxy(SaveLoadSeedMenu slsm)
            {
                this.slsm = slsm;
            }

            public void Apply(MenuItem _item)
            {
                slsm.Copy(_item);
            }
        }

        void Copy(MenuItem _item)
        {
            string seed = Tools.CurLevel.MyLevelSeed.ToString();
            System.Windows.Forms.Clipboard.SetText(seed);
        }
#endif

        class LoadStringProxy : Lambda_1<MenuItem>
        {
            SaveLoadSeedMenu slsm;

            public LoadStringProxy(SaveLoadSeedMenu slsm)
            {
                this.slsm = slsm;
            }

            public void Apply(MenuItem _item)
            {
                slsm.LoadString(_item);
            }
        }

        void LoadString(MenuItem _item)
        {
            LoadSeedAs LoadAs = new LoadSeedAs(Control, player);
            Call(LoadAs, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SetPosition();
        }
    }
}