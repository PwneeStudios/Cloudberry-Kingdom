using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class SavedSeedsGUI : CkBaseMenu
    {
        public SavedSeedsGUI()
        {
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
                    Tools.CurGameData.EndGame.Apply(false);
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

        class PostMakeStandardLoadHelper : Lambda_1<Level>
        {
            LevelSeedData seed;

            public PostMakeStandardLoadHelper(LevelSeedData seed)
            {
                this.seed = seed;
            }

            public void Apply(Level level)
            {
                seed.PostMake_StandardLoad(level);
            }
        }

        class LoadFromFreeplayMenuHelper : Lambda
        {
            LevelSeedData seed;
            string seedstr;
            CustomLevel_GUI simple;

            public LoadFromFreeplayMenuHelper(LevelSeedData seed, string seedstr, CustomLevel_GUI simple)
            {
                this.seed = seed;
                this.seedstr = seedstr;
                this.simple = simple;
            }

            public void Apply()
            {
                simple.StartLevel(seed);

                // Randomize the seed for the next level, if the player chooses to continue using this LevelSeedData.
                seed = new LevelSeedData();
                seed.ReadString(seedstr);
                seed.PostMake.Add(new PostMakeStandardLoadHelper(seed));
                seed.Seed = Tools.GlobalRnd.Rnd.Next();
            }
        }

        private static void LoadFromFreeplayMenu(string seedstr, CustomLevel_GUI simple)
        {
            var seed = new LevelSeedData();
            seed.ReadString(seedstr);
            seed.PostMake.Add(new PostMakeStandardLoadHelper(seed));

            simple.MyGame.PlayGame(new LoadFromFreeplayMenuHelper(seed, seedstr, simple));

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

        class ReturnToCallerProxy : Lambda
        {
            SavedSeedsGUI ssGui;

            public ReturnToCallerProxy(SavedSeedsGUI ssGui)
            {
                this.ssGui = ssGui;
            }

            public void Apply()
            {
                ssGui.ReturnToCaller();
            }
        }

        class DoDeletionProxy : Lambda_1<bool>
        {
            SavedSeedsGUI ssGui;

            public DoDeletionProxy(SavedSeedsGUI ssGui)
            {
                this.ssGui = ssGui;
            }

            public void Apply(bool choice)
            {
                ssGui.DoDeletion(choice);
            }
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
                MyGame.WaitThenDo(10, new ReturnToCallerProxy(this));
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

            MyGame.WaitThenDo(10, new ReturnToCallerProxy(this));
        }

        void Sort()
        {
            if (!Active) return;
        }

        PlayerData player;
        public override void Init()
        {
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

            // Header
            MenuItem item = new MenuItem(new EzText(Localization.Words.SaveSeed, ItemFont));
            item.Name = "Header";
            item.Selectable = false;
            SetHeaderProperties(item.MySelectedText);
            AddItem(item);

#if PC_VERSION
            Vector2 SavePos = ItemPos;
            MakeBackButton().UnaffectedByScroll = true;
            ItemPos = SavePos;
#endif
            
            MakeOptions();
            MakeList();

#if !PC_VERSION
            OptionalBackButton();
#endif

            // Backdrop
            QuadClass backdrop;

            backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop);

            SetPos();
#if PC_VERSION
            MyMenu.SelectItem(2);
#else
            MyMenu.SelectItem(0);
#endif
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
#if WINDOWS
            if (ButtonCheck.State(Microsoft.Xna.Framework.Input.Keys.Delete).Pressed)
                Delete(MyMenu);
#endif
        }

        bool Back(Menu menu)
        {
            if (!Active) return true;

            int num = NumSeedsToDelete();
            if (num > 0)
            {
                var verify = new VerifyDeleteSeeds(Control, num);
                verify.OnSelect.Add(new DoDeletionProxy(this));

                Call(verify, 0);
            }
            else
                ReturnToCaller();

            return true;
        }

        public override void OnReturnTo()
        {
            Hid = false;
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
#if PC_VERSION
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(27.77808f, 925.0002f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(278.9676f, 937.6984f); }
            _item = MyMenu.FindItemByName("castle_testsave"); if (_item != null) { _item.SetPos = new Vector2(80.5547f, 516.1112f); }

            MyMenu.Pos = new Vector2(-1016.667f, 0f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(2.77771f, 22.22221f); _q.Size = new Vector2(1572.44f, 1572.44f); }

            MyPile.Pos = new Vector2(0f, 0f);
#else
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(27.77808f, 925.0002f); }

            MyMenu.Pos = new Vector2(-1016.667f, 0f);

            EzText _t;
            _t = MyPile.FindEzText("Load"); if (_t != null) _t.Pos = new Vector2(564.6826f, -51.11127f);
            _t = MyPile.FindEzText("Delete"); if (_t != null) _t.Pos = new Vector2(570.572f, -309.3967f);
            _t = MyPile.FindEzText("Back"); if (_t != null) _t.Pos = new Vector2(579.6982f, -569.7302f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(2.77771f, 22.22221f); _q.Size = new Vector2(1572.44f, 1572.44f); }

            MyPile.Pos = new Vector2(0f, 0f);
#endif
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
                seeditem.Go = _menu => StartLevel(_seed);
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
        }

        private void MakeOptions()
        {
            FontScale *= .8f;

#if PC_VERSION
            MenuItem item;
            //// Load
            //item = new MenuItem(new EzText("Load", ItemFont));
            //item.Name = "Load";
            //AddItem(item);
            //item.SelectSound = null;
            //item.Go = ItemReturnToCaller;
            //item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            //item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;

            //// Delete
            //item = new MenuItem(new EzText("Delete", ItemFont));
            //item.Name = "Delete";
            //AddItem(item);
            //item.SelectSound = null;
            //item.Go = ItemReturnToCaller;
            //item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4();
            //item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();

            //// Back
            //item = new MenuItem(new EzText("Back", ItemFont));
            //item.Name = "Back";
            //AddItem(item);
            //item.SelectSound = null;
            //item.Go = ItemReturnToCaller;
            //item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            //item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#else
            float scale = .75f;

            EzText text;
            text = new EzText(ButtonString.Go(90) + " Load", ItemFont);
            text.Name = "Load";
            text.Scale *= scale;
            text.Pos = new Vector2(417.4604f, -159.4446f);
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            MyPile.Add(text);

            text = new EzText(ButtonString.X(90) + " Delete", ItemFont);
            text.Name = "Delete";
            text.Scale *= scale;
            text.Pos = new Vector2(531.6831f, -389.9523f);
            text.MyFloatColor = new Color(204, 220, 255).ToVector4();
            MyPile.Add(text);

            text = new EzText(ButtonString.Back(90) + " Back", ItemFont);
            text.Name = "Back";
            text.Scale *= scale;
            text.Pos = new Vector2(682.4761f, -622.5079f);
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            MyPile.Add(text);
#endif
        }
    }
}