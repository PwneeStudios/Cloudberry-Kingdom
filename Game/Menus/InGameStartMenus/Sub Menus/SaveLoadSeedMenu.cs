using Microsoft.Xna.Framework;
using System;

#if PC
using SteamManager;
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
			if (Tools.CurGameData is NormalGameData)
			{
				EnableBounce();
			}

            if (UseBounce)
                CallToLeft = false;
            else
                CallToLeft = true;

            this.Control = Control;
            FixedToCamera = true;

            this.CanLoad = CanLoad;
            this.CanSave = CanSave;

            Constructor();
        }

        bool CanLoad, CanSave;

        PlayerData player;

        Text HeaderText;
        public override void Init()
        {
            base.Init();

            this.FontScale *= .9f;

            // Get the activating player
            player = MenuItem.GetActivatingPlayerData();

            // Header
            HeaderText = new Text(Localization.Words.RandomSeed, ItemFont);
            HeaderText.Name = "Header";
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;

            MenuItem item;

            if (CanSave)
            {
                // Save seed
                item = new MenuItem(new Text(Localization.Words.SaveSeed, ItemFont));
                item.Name = "Save";
                item.Go = MakeSave(this, player);
                AddItem(item);
            }

            if (CanLoad)
            {
                // Load seed
                item = new MenuItem(new Text(Localization.Words.LoadSeed, ItemFont));
                item.Name = "Load";
                item.Go = Load;
                AddItem(item);
            }

#if WINDOWS && !MONO && !SDL2 // FIXME: SDL2 can do this... -flibit
            if (CanSave)
            {
                // Copy seed
                item = new MenuItem(new Text(Localization.Words.CopyToClipboard, ItemFont));
                item.Name = "Copy";
                item.Go = Copy;
                AddItem(item);
            }

            if (CanLoad)
            {
                // Load seed from string
                item = new MenuItem(new Text(Localization.Words.LoadFromClipboard, ItemFont));
                item.Name = "LoadString";
                item.Go = LoadString;
                AddItem(item);
            }
#endif

#if PC
            MakeBackButton();
#else
            MakeBackButton();
            //MakeStaticBackButton();
#endif

            MyMenu.OnB = MenuReturnToCaller;

            SetPosition();
            MyMenu.SortByHeight();
            MyMenu.SelectItem(0);

			//if (!(Tools.CurGameData is NormalGameData))
			//{
			//    zoom = null;
			//    UseBounce = false;
			//}
        }

		public override void ReturnToCaller()
		{
			//if (!UseBounce) EnableBounce();

			base.ReturnToCaller();
		}

		public override void Call(GUI_Panel child, int Delay)
		{
			//if (!UseBounce) EnableBounce();

			base.Call(child, Delay);
		}

        private void SetPosition()
        {
#if PC
            if (CanLoad && CanSave)
            {
				MenuItem _item;
                _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(661.1118f, 616.5555f); _item.MyText.Scale = 0.5616f; _item.MySelectedText.Scale = 0.5616f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
                _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(661.1118f, 430.4444f); _item.MyText.Scale = 0.5616f; _item.MySelectedText.Scale = 0.5616f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
                _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(661.1118f, 244.3332f); _item.MyText.Scale = 0.5616f; _item.MySelectedText.Scale = 0.5616f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
                _item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(661.1118f, 58.22205f); _item.MyText.Scale = 0.5616f; _item.MySelectedText.Scale = 0.5616f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
                _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(661.1118f, -127.8891f); _item.MyText.Scale = 0.5616f; _item.MySelectedText.Scale = 0.5616f; _item.SelectIconOffset = new Vector2(0f, 0f);  }

				MyMenu.Pos = new Vector2(-1177.779f, -222.2221f);

				Text _t;
				_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(402.7776f, 871.8887f); _t.Scale = 0.864f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1500f, 803.2258f); }

				MyPile.Pos = new Vector2(-1177.779f, -222.2221f);
			}
            else if (CanLoad && !CanSave)
            {
				MenuItem _item;
				_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(602.7778f, 472.1111f); _item.MyText.Scale = 0.6090844f; _item.MySelectedText.Scale = 0.6090844f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(602.7778f, 274.8889f); _item.MyText.Scale = 0.6090844f; _item.MySelectedText.Scale = 0.6090844f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(602.7778f, 77.66666f); _item.MyText.Scale = 0.6090844f; _item.MySelectedText.Scale = 0.6090844f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-1216.668f, -288.8889f);

				Text _t;
				_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(402.7776f, 871.8887f); _t.Scale = 0.864f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1167.362f, 330.5555f); _q.Size = new Vector2(1236.444f, 743.7259f); }

				MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
            }
            else
            {
                MenuItem _item;
                _item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(674.9999f, 499.889f); }
                _item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(674.9999f, 269.3333f); }
                _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(674.9999f, 47.11124f); }

                MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

                Text _t;
                _t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(425f, 807.9997f); }

                QuadClass _q;
                _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1224.558f, 736.7258f); }

                MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
            }

            // Shrink and shift MenuItems down for some languages.
            float scale = 1;
            var shift = Vector2.Zero;
            switch (Localization.CurrentLanguage.MyLanguage)
            {
                case Localization.Language.German:      scale = .88f; shift.X = -200; break;
                case Localization.Language.Portuguese:  scale = .82f; shift.X = -370; break;
                case Localization.Language.French:      scale = .88f; shift.X = -100; break;
				case Localization.Language.Russian:
					if (!CanLoad && CanSave)
					{
						scale = .85f;
						shift.X = -150;
					}
					break;
                default: break;
            }

            foreach (var _item in MyMenu.Items)
                _item.ScaleText(scale);

            MyMenu.Pos += shift;
            foreach (var text in MyPile.MyTextList)
                text.Pos += shift;
#else
			MenuItem _item;
			_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(741.6669f, 547.111f); _item.MyText.Scale = 0.7334167f; _item.MySelectedText.Scale = 0.7334167f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
			_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(750.0004f, 330.4444f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
			_item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(3044.444f, -1480.667f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
			_item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(1527.778f, -1983.444f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
			_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(758.3339f, 8.222351f); _item.MyText.Scale = 0.72f; _item.MySelectedText.Scale = 0.72f; _item.SelectIconOffset = new Vector2(0f, 0f);  }

			MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

			Text _t;
			_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(658.333f, 905.222f); _t.Scale = 0.864f; }

			QuadClass _q;
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 313.889f); _q.Size = new Vector2(1089.917f, 752.6426f); }

			MyPile.Pos = new Vector2(-1127.779f, -322.2222f);

			//MenuItem _item;
			//_item = MyMenu.FindItemByName("Save"); if (_item != null) { _item.SetPos = new Vector2(686.1115f, 633.2222f); }
			//_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(694.4447f, 441.5555f); }
			//_item = MyMenu.FindItemByName("Copy"); if (_item != null) { _item.SetPos = new Vector2(672.2223f, 222.1111f); }
			//_item = MyMenu.FindItemByName("LoadString"); if (_item != null) { _item.SetPos = new Vector2(672.2225f, 27.66663f); }
			//_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(727.7777f, -163.9999f); }

			//MyMenu.Pos = new Vector2(-1125.001f, -319.4444f);

			//MyPile.FindText("Header").Pos = new Vector2(402.7776f, 871.8887f);

			//QuadClass _q;
			//_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1181.251f, 241.6668f); _q.Size = new Vector2(1500f, 803.2258f); }

			//MyPile.Pos = new Vector2(-1125.001f, -319.4444f);
#endif
		}

#if PC || XBOX || MONO
        // This shows our custom virtual keyboard, instead of the Xbox keyboard.
        public static MenuItemGo MakeSave(GUI_Panel panel, PlayerData player)
        {
            return _item => Save(_item, panel, player);
        }

#if PC
		static PlayerData _player;
		static GUI_Panel _SaveLoadSeedMenu;

		protected override void MyPhsxStep()
		{
			if (SteamTextInput.OverlayActive) return;

			base.MyPhsxStep();
		}

		static void OnOk()
		{
		}

		static void OnGamepadTextInputEnd_SaveSeed(bool result)
		{
			if (result && _player != null)
			{
				// Get the text
				string s = SteamManager.SteamTextInput.GetText();

				// Save the seed
				_player.MySavedSeeds.SaveSeed(Tools.CurLevel.MyLevelSeed.ToString(), s);

				// Success!
				var ok = new AlertBaseMenu(_player.MyIndex, Localization.Words.SeedSavedSuccessfully, Localization.Words.Hooray);
				ok.OnOk = OnOk;
				_SaveLoadSeedMenu.Call(ok);
				_SaveLoadSeedMenu.Hid = true;

				CkBaseMenu _ckbasemenu = _SaveLoadSeedMenu as CkBaseMenu;
				if (null != _ckbasemenu)
					_ckbasemenu.RegularSlideOut(PresetPos.Right, 0);
				else
					_SaveLoadSeedMenu.SlideOut(PresetPos.Right, 0);

				SavedSeedsGUI.LastSeedSave_TimeStamp = Tools.DrawCount;
			}
		}
#endif

        IAsyncResult kyar;
        static void Save(MenuItem _item, GUI_Panel panel, PlayerData player)
        {
#if PC
			if (CloudberryKingdomGame.UsingSteam)
			{
				_SaveLoadSeedMenu = panel;
				_player = player;

				bool GamepadInputUp = SteamTextInput.ShowGamepadTextInput(
					Localization.WordString(Localization.Words.SaveRandomSeedAs), 32, OnGamepadTextInputEnd_SaveSeed);

				if (GamepadInputUp) return;
			}
#endif

			CkBaseMenu ckpanel = panel as CkBaseMenu;

            SaveSeedAs SaveAs = new SaveSeedAs(panel.Control, player);
            
			if (null != ckpanel && ckpanel.UseBounce)
			{
				panel.Call(SaveAs, 0);

				ckpanel.Hid = true;
				ckpanel.RegularSlideOut(PresetPos.Right, 0);
			}
			else
			{
				panel.Call(SaveAs, 17);

				ckpanel.Hide(PresetPos.Left, 20);
			}
        }
#else
        public static MenuItemGo MakeSave(GUI_Panel panel, PlayerData player)
        {
            return _item => Save(_item, player);
        }

        public static void BeginShowKeyboard()
        {
            try
            {
                CloudberryKingdomGame.ShowKeyboard = false;
                CloudberryKingdomGame.KeyboardIsDone = false;

                kyar = Guide.BeginShowKeyboardInput(_player.MyPlayerIndex,
                        Localization.WordString(Localization.Words.SaveRandomSeedAs), "Choose a name to save this level as.",
                        Tools.CurLevel.MyLevelSeed.SuggestedName(), OnKeyboardComplete, null);
            }
            catch
            {
                CloudberryKingdomGame.ShowKeyboard = false;
                CloudberryKingdomGame.KeyboardIsDone = false;
                kyar = null;
            }
        }

        static IAsyncResult kyar;
        static PlayerData _player;
        static void Save(MenuItem _item, PlayerData activeplayer)
        {
			CloudberryKingdomGame.ShowKeyboard = true;

            _player = activeplayer;
        }

        static void OnKeyboardComplete(IAsyncResult ar)
        {
			CloudberryKingdomGame.KeyboardIsDone = true;

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

        protected override void MyPhsxStep()
        {
			if (CloudberryKingdomGame.KeyboardIsDone)
            {
				CloudberryKingdomGame.KeyboardIsDone = false;
				kyar = null;
            }

            if (kyar != null && kyar.IsCompleted)
            {
				CloudberryKingdomGame.KeyboardIsDone = false;
				kyar = null;
            }

            base.MyPhsxStep();
        }
#endif

        void Load(MenuItem _item)
        {
            SavedSeedsGUI LoadMenu = new SavedSeedsGUI();
            Call(LoadMenu, 0);

            if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
        }

#if WINDOWS && !MONO && !SDL2 // FIXME: SDL2 can do this... -flibit
        void Copy(MenuItem _item)
        {
            string seed = Tools.CurLevel.MyLevelSeed.ToString();
            System.Windows.Forms.Clipboard.SetText(seed);

			// Show a success window
			var ok = new AlertBaseMenu(Control, Localization.Words.SeedCopiedSuccessfully, Localization.Words.Hooray);
			ok.OnOk = ok.ReturnToCaller;
			Call(ok, 0);

			Hid = true;
			RegularSlideOut(PresetPos.Right, 0);
        }
#endif

        void LoadString(MenuItem _item)
        {
            LoadSeedAs LoadAs = new LoadSeedAs(Control, player);
            Call(LoadAs, 0);

            //if (UseBounce)
            {
                Hid = true;
                RegularSlideOut(PresetPos.Right, 0);
            }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SetPosition();
        }
    }
}