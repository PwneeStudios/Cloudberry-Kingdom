using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    public class SavedSeedsGUI : CkBaseMenu
    {
        public SavedSeedsGUI()
        {
			//EnableBounce();
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

		public static int LastSeedSave_TimeStamp = 0;

        void StartLevel(string seedstr)
        {
            LoadSeed(seedstr, this);
        }

        static CustomLevel_GUI FreeplayMenu;
        public static void LoadSeed(string seedstr, GUI_Panel panel)
        {
            // If the current panel or parent panel is the Freeplay menu,
            // then directly start the level.
            FreeplayMenu = panel.Caller as CustomLevel_GUI;
            if (FreeplayMenu == null) FreeplayMenu = panel as CustomLevel_GUI;
            if (null != FreeplayMenu)
            {
                LoadFromFreeplayMenu(seedstr, FreeplayMenu);
            }
            else
            {
                // Otherwise, if the parent game is Freeplay, then queue Freeplay to load the level.
                if (Tools.CurGameData.ParentGame is TitleGameData)
                {
                    Tools.CurrentAftermath = new AftermathData();
                    Tools.CurrentAftermath.Success = false;
                    Tools.CurrentAftermath.EarlyExit = true;

                    CustomLevel_GUI.SeedStringToLoad = seedstr;
                    Tools.CurGameData.EndGame(false);
                }
                else
                    CustomLevel_GUI.SeedStringToLoad = seedstr;
                    
//#if DEBUG
//                // otherwise, hard load the game, and forget about how it connects to anything else.
//                // This will cause crashes if you try to exit the game afterwards, but is fine for testing purposes.
//                else
//                {
//                    GameData.LockLevelStart = false;
//                    LevelSeedData.ForcedReturnEarly = 0;

//                    LevelSeedData data = new LevelSeedData();
//                    data.ReadString(seedstr);
//                    GameData.StartLevel(data);
//                }
//#endif
            }
        }

        private static void LoadFromFreeplayMenu(string seedstr, CustomLevel_GUI simple)
        {
            var seed = new LevelSeedData();
            seed.ReadString(seedstr);
            seed.PostMake += seed.PostMake_StandardLoad;

            simple.MyGame.PlayGame(() =>
                {
                    simple.StartLevel(seed);

                    // Randomize the seed for the next level, if the player chooses to continue using this LevelSeedData.
                    seed = new LevelSeedData();
                    seed.ReadString(seedstr);
                    seed.PostMake += seed.PostMake_StandardLoad;
                    seed.Seed = Tools.GlobalRnd.Rnd.Next();
                });
        }

        /// <summary>
        /// Returns true if any item in the menu has been marked for deletion.
        /// </summary>
        int NumSeedsToDelete()
        {
            int count = 0;
            foreach (MenuItem item in MyMenu.Items)
            {
                var seeditem = item as SeedItem;
                if (null != seeditem && seeditem.MarkedForDeletion) count++;
            }

            return count;
        }

        /// <summary>
        /// Mark the current item to be deleted
        /// </summary>
        bool Delete(Menu _menu)
        {
            if (!Active) return true;

            var seeditem = MyMenu.CurItem as SeedItem;
            if (null != seeditem) seeditem.ToggleDeletion();
            
            return true;
        }

        /// <summary>
        /// Delete the items marked for deletion, if the user selected "Yes"
        /// </summary>
        void DoDeletion(bool choice)
        {
            Active = false;

            // If "No", do not delete any seeds.
            if (!choice)
            {
				//MyGame.WaitThenDo(10, ReturnToCaller);
                return;
            }

            // Delete all saved seeds.
            player.MySavedSeeds.SeedStrings.Clear();

            // Save seeds not marked for deletion.
            foreach (MenuItem item in MyMenu.Items)
            {
                var seeditem = item as SeedItem;
                if (null == seeditem) continue;
                if (seeditem.MarkedForDeletion) continue;

                player.MySavedSeeds.SeedStrings.Add(seeditem.Seed);
            }

            SaveGroup.SaveAll();

			LastSeedSave_TimeStamp = Tools.DrawCount;
			ReInit();

			SlideOut(PresetPos.Left, 0);
			SlideInFrom = SlideOutTo = PresetPos.Left;
        }

		void ReInit()
		{
			MyInit_TimeStamp = Tools.DrawCount;

			CoreEngine.FancyVector2 HoldPos = Pos;
			Init();
			Pos = HoldPos;

			if (bar != null)
			{
				bar.Release();

				bar = new ScrollBar((LongMenu)MyMenu, this);
				bar.BarPos = new Vector2(-1860f, 102.7778f);
				MyGame.AddGameObject(bar);
#if PC_VERSION
				MyMenu.AdditionalCheckForOutsideClick += bar.MyMenu.HitTest;
#endif
			}
		}

        void Sort()
        {
            if (!Active) return;
        }

        PlayerData player;
		int MyInit_TimeStamp = 0;
        public override void Init()
        {
			if (Tools.CurGameData is NormalGameData)
			{
				EnableBounce();
			}

			MyInit_TimeStamp = Tools.DrawCount;

			base.Init();

            Control = -1;
            MyPile = new DrawPile();

            // Get the activating player
            player = MenuItem.GetActivatingPlayerData();

            // Set slide in and out parameters
            ReturnToCallerDelay = 6;
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;
            SlideInLength = 25;
            SlideOutLength = 20;

            // Make the menu
            //MyMenu = new Menu(false);
            MyMenu = new LongMenu(); MyMenu.FixedToCamera = false; MyMenu.WrapSelect = false; ((LongMenu)MyMenu).OffsetStep = 30;
            EnsureFancy();
            MyMenu.OnA = null;
            MyMenu.OnB = Back;
            MyMenu.OnX = Delete;
            MyMenu.OnY = Sort;
            MyMenu.SelectDelay = 11;

            ItemPos = new Vector2(80.5547f, 756.1112f);
            PosAdd = new Vector2(0, -120);

            FontScale = .666f;

			// Backdrop
			QuadClass backdrop;
			if (UseSimpleBackdrop)
				backdrop = new QuadClass("Arcade_BoxLeft", 1500, true);
			else
				backdrop = new QuadClass("Backplate_1500x900", 1500, true);

			backdrop.Name = "Backdrop";
			MyPile.Add(backdrop);

			if (!UseSimpleBackdrop)
			{
				EpilepsySafe(.9f);
			}

            // Header
            MenuItem item = new MenuItem(new EzText(Localization.Words.SavedSeeds, ItemFont));
            item.Name = "Header";
            item.Selectable = false;
            SetHeaderProperties(item.MySelectedText);
            AddItem(item);

            Vector2 SavePos = ItemPos;
            MakeOptions();
			ItemPos = SavePos;

            MakeList();

#if !PC_VERSION
            OptionalBackButton();
#endif

            SetPos();

if (ButtonCheck.ControllerInUse)
{
            MyMenu.SelectItem(4);
}
else
{
			MyMenu.SelectItem(3);
}
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

			if (Core.Released || !Active) return;

			if (MyInit_TimeStamp + 10 < LastSeedSave_TimeStamp)
			{
				ReInit();
			}

			// Update "Confirm"
			//if (ButtonCheck.State(ControllerButtons.A, -2).Pressed)
			{
				int n = NumSeedsToDelete();
				//MyPile.FindQuad("Confirm").Show = n > 0;
				//MyPile.FindEzText("Confirm").Show = n > 0;

				string GoString;
				if (n == 0)      GoString = Localization.WordString(Localization.Words.LoadSeed);
				else if (n == 1) GoString = string.Format(Localization.WordString(Localization.Words.DeleteSeeds), n);
				else		     GoString = string.Format(Localization.WordString(Localization.Words.DeleteSeedsPlural), n);

				MenuItem _item = MyMenu.FindItemByName("Load");
				if (_item != null)
				{
					_item.MyText.SubstituteText(GoString);
					_item.MySelectedText.SubstituteText(GoString);
				}
			}

#if WINDOWS
            if (ButtonCheck.State(Microsoft.Xna.Framework.Input.Keys.Delete).Pressed)
                Delete(MyMenu);
#endif
        }

        bool Back(Menu menu)
        {
            if (!Active) return true;

			SlideOutTo = SlideInFrom = PresetPos.Right;

            int num = NumSeedsToDelete();
            if (num > 0)
            {
                var verify = new VerifyDeleteSeeds(Control, num, UseBounce);
                verify.OnSelect += DoDeletion;

				SlideOutTo = PresetPos.Left;
				SlideInFrom = PresetPos.Left;
                Call(verify, 0);

				if (UseBounce)
				{
					Hid = true;
					RegularSlideOut(PresetPos.Right, 0);
				}
				else
				{
					Hide(PresetPos.Left);
				}
            }
            else
                ReturnToCaller();

            return true;
        }

        public override void OnReturnTo()
        {
			// Clear the pre-deleted items
			foreach (MenuItem item in MyMenu.Items)
			{
				SeedItem sitem = item as SeedItem;
				if (null != sitem && sitem.MarkedForDeletion)
					sitem.ToggleDeletion();
			}

			//Hid = false;
			//CallToLeft = false;
			//UseBounce = false;
			//SlideOutLength = 0;
			//ReturnToCallerDelay = 0;
            base.OnReturnTo();
        }

        void OptionalBackButton()
        {
            // Make a back button if there is no saved seeds.
            if (player.MySavedSeeds.SeedStrings.Count == 0)
                MakeBackButton();
        }

        void SetPos()
        {
			MenuItem _item;
			QuadClass _q;

if (ButtonCheck.ControllerInUse)
{
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(69.44482f, 955.5558f); _item.MyText.Scale = 0.666f; _item.MySelectedText.Scale = 0.666f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(736.1101f, 983.3332f); _item.MyText.Scale = 0.4186335f; _item.MySelectedText.Scale = 0.4186335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Delete"); if (_item != null) { _item.SetPos = new Vector2(741.6659f, 860.5553f); _item.MyText.Scale = 0.3892168f; _item.MySelectedText.Scale = 0.3892168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(741.6665f, 756.6665f); _item.MyText.Scale = 0.4212168f; _item.MySelectedText.Scale = 0.4212168f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-1016.667f, 0f);

			_q = MyPile.FindQuad("Button_A"); if (_q != null) { _q.Pos = new Vector2(658.3345f, 911.1108f); _q.Size = new Vector2(56.16665f, 56.16665f); }
			_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(661.1113f, 794.4444f); _q.Size = new Vector2(57.66665f, 57.66665f); }
			_q = MyPile.FindQuad("Button_B"); if (_q != null) { _q.Pos = new Vector2(663.8892f, 677.7778f); _q.Size = new Vector2(56.41664f, 56.41664f); }
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(2.77771f, 22.22221f); _q.Size = new Vector2(1572.44f, 1572.44f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else
{	
			_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(88.88916f, 941.6669f); _item.MyText.Scale = 0.666f; _item.MySelectedText.Scale = 0.666f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(591.6653f, 988.889f); _item.MyText.Scale = 0.3920501f; _item.MySelectedText.Scale = 0.3920501f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(591.6661f, 893.8889f); _item.MyText.Scale = 0.3935499f; _item.MySelectedText.Scale = 0.3935499f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-1016.667f, 0f);

			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(2.77771f, 22.22221f); _q.Size = new Vector2(1572.44f, 1572.44f); }

			MyPile.Pos = new Vector2(0f, 0f);
}

			// Extra squeeze
			Vector2 squeeze = new Vector2(-15, -18) * CloudberryKingdomGame.GuiSqueeze;

			_item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = _item.Pos + squeeze; }
			_item = MyMenu.FindItemByName("Delete"); if (_item != null) { _item.SetPos = _item.Pos + squeeze; }
			_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = _item.Pos + squeeze; }

			_q = MyPile.FindQuad("Button_A"); if (_q != null) { _q.Pos += squeeze; }
			_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos += squeeze; }
			_q = MyPile.FindQuad("Button_B"); if (_q != null) { _q.Pos += squeeze; }
        }

        class SeedItem : MenuItem
        {
            public string Seed;
            public bool MarkedForDeletion = false;

            public SeedItem(string name, string seed, EzFont font) : base(new EzText(name, font))
            {
                this.Name = name;
                this.Seed = seed;
            }

            public void ToggleDeletion()
            {
                MarkedForDeletion = !MarkedForDeletion;

                // Make items marked for deletion semi-transparent.
                if (MarkedForDeletion)
                    MyText.Alpha = MySelectedText.Alpha = .3f;
                else
                    MyText.Alpha = MySelectedText.Alpha = 1f;
            }
        }

        private void MakeList()
        {
            foreach (string seed in player.MySavedSeeds.SeedStrings)
            {
                string _seed = seed;
                string name = LevelSeedData.GetNameFromSeedStr(seed);

                // Get name of seed
                MenuItem seeditem = new SeedItem(name, seed, ItemFont);
				seeditem.Go = _item =>
				{
					int n = NumSeedsToDelete();

					if (n > 0)
						Back(MyMenu);
					else
						StartLevel(_seed);
				};
                AddItem(seeditem);
            }
        }

        ScrollBar bar;
        public override void OnAdd()
        {
            base.OnAdd();

            // Scroll bar
#if WINDOWS
//#if PC_VERSION
            //if (false)
            {
                bar = new ScrollBar((LongMenu)MyMenu, this);
                bar.BarPos = new Vector2(-1860f, 102.7778f);
                MyGame.AddGameObject(bar);
#if PC_VERSION
                MyMenu.AdditionalCheckForOutsideClick += bar.MyMenu.HitTest;
#endif
            }
#endif

			if (Caller is CustomLevel_GUI)
			{
				RegularSlideOut(PresetPos.Right, 0);
				SlideIn(30);
			}
        }

        private void MakeOptions()
        {
            FontScale *= .8f;

			//MakeBackButton(Localization.Words.Back, false).UnaffectedByScroll = true;

            MenuItem item;
if (ButtonCheck.ControllerInUse)
{
			// Load
			item = new MenuItem(new EzText(Localization.Words.LoadSeed, ItemFont));
			item.Name = "Load";
			AddItem(item);
			item.SelectSound = null;
			item.Go = ItemReturnToCaller;
			item.UnaffectedByScroll = true;
			MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_A"));
			item.Selectable = false;
#if XBOX
			Menu.DefaultMenuInfo.SetNext(item);
#endif
}

if (ButtonCheck.ControllerInUse)
{
			// Delete
			item = new MenuItem(new EzText(Localization.Words.Delete, ItemFont));
			item.Name = "Delete";
			AddItem(item);
			item.SelectSound = null;
			item.Go = ItemReturnToCaller;
			item.UnaffectedByScroll = true;

			MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));
			item.Selectable = false;
#if XBOX
			Menu.DefaultMenuInfo.SetX(item);
#endif
}

			// Back
			item = new MenuItem(new EzText(Localization.Words.Back, ItemFont));
			item.Name = "Back";
			AddItem(item);
			item.SelectSound = null;
			item.Go = ItemReturnToCaller;
			item.UnaffectedByScroll = true;
if (ButtonCheck.ControllerInUse)
{
			MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_B"));
			item.Selectable = false;
}
#if XBOX
			item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
			item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#endif
        }
    }
}