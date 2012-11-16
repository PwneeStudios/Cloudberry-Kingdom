using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu : CkBaseMenu
    {
        public override void Hide(PresetPos pos, int frames)
        {
            base.Hide(pos, frames);
            ButtonCheck.PreLogIn = false;
        }

        public enum Next { Campaign, Arcade, Freeplay };
        public Next MyNextMenu;

        class StartMenuLambda_Campaign : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Campaign(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Campaign(item);
            }
        }

        protected virtual void MenuGo_Campaign(MenuItem item)
        {
            MyNextMenu = Next.Campaign;
            BringCharacterSelect();
        }

        class StartMenuLambda_Arcade : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Arcade(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Arcade(item);
            }
        }

        protected virtual void MenuGo_Arcade(MenuItem item)
        {
            MyNextMenu = Next.Arcade;
            BringCharacterSelect();
            //DoneWithCharSelect();StartMenu_MW_HeroSelect
        }

        class StartMenuLambda_Freeplay : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Freeplay(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Freeplay(item);
            }
        }

        protected virtual void MenuGo_Freeplay(MenuItem item)
        {
            MyNextMenu = Next.Freeplay;
            BringCharacterSelect();
            //DoneWithCharSelect();
        }

        class CharacterSelectProxy : Lambda
        {
            StartMenu startMenu;

            public CharacterSelectProxy(StartMenu startMenu)
            {
                this.startMenu = startMenu;
            }

            public void Apply()
            {
                startMenu.CharacterSelect();
            }
        }

        /// <summary>
        /// When true the user can not selected back.
        /// </summary>
        protected bool NoBack = false;

        protected virtual void BringCharacterSelect()
        {
            NoBack = true;
            MyGame.SlideOut_FadeIn(20, new CharacterSelectProxy(this));
        }

        public override void Show()
        {
            NoBack = false;
            base.Show();
        }

        public override void ReturnToCaller()
        {
            base.ReturnToCaller();
        }

        protected virtual void CharacterSelect()
        {
            Hide();
            CharacterSelectManager.Start(this);
            CharacterSelectManager.OnDone = new DoneWithCharSelectProxy(this);
            CharacterSelectManager.OnBack = null;
        }

        class MenuGo_ScreenSaverHelper : Lambda
        {
            StartMenu sm;

            public MenuGo_ScreenSaverHelper(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply()
            {
                sm.MyGame.Release();
            }
        }

        protected virtual void MenuGo_ScreenSaver(MenuItem item)
        {
            ScreenSaver Intro = new ScreenSaver(); Intro.Init();
            Tools.TheGame.LogoScreenPropUp = false;
            Tools.AddToDo(new MenuGo_ScreenSaverHelper(this));
        }

        class StartMenuLambda_Controls : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Controls(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Controls(item);
            }
        }

        protected virtual void MenuGo_Controls(MenuItem item)
        {
            Call(new ControlScreen(Control), 0);
        }

        protected virtual void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(StatGroup.Lifetime), 0);
        }

        class StartMenuLambda_Options : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Options(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Options(item);
            }
        }

        protected virtual void MenuGo_Options(MenuItem item)
        {
            Call(new SoundMenu(Control), 0);
        }

        class StartMenuLambda_Exit : Lambda_1<MenuItem>
        {
            StartMenu sm;
            public StartMenuLambda_Exit(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply(MenuItem item)
            {
                sm.MenuGo_Exit(item);
            }
        }

        protected virtual void MenuGo_Exit(MenuItem item)
        {
            Exit();
        }

        protected virtual void Exit()
        {
            SelectSound = null;
            Call(new VerifyQuitGameMenu2(Control), 0);
        }

        public StartMenu()
        {
            if (Tools.TheGame.LoadingScreen != null)
                Tools.TheGame.LoadingScreen.IsDone = true;
        }

        ///// <summary>
        ///// When true the menu slides in, rather than bubbles in, when a child menu returns control to it
        ///// </summary>
        //bool SlideOnReturn = false;

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
            //base.SlideIn(0);
            //return;

            //if (SlideOnReturn)
            //    base.SlideIn(Frames);
            //else
            //{
            //    base.SlideIn(0);

            //    MyPile.BubbleUp(true, 8, 1);
            //    MyMenu.FancyPos.LerpTo(MenuPos_Down, MenuPos, 40);
            //}

            //SlideOnReturn = false;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);
            SetItemProperties_Red(item);
        }

        public static void SetText_Green(EzText text, bool outline)
        {
            if (text == null) return;

            text.OutlineColor = new Color(0, 0, 0, outline ? 255 : 0).ToVector4();
            text.MyFloatColor = new Color(34, 214, 47).ToVector4();
        }

        public static void SetSelectedText_Green(EzText text, bool outline)
        {
            if (text == null) return;

            text.OutlineColor = new Color(0, 0, 0, outline ? 255 : 0).ToVector4();
            text.MyFloatColor = new Color(65, 255, 100).ToVector4();
        }

        public static void SetItemProperties_Green(MenuItem item, bool outline)
        {
            SetText_Green(item.MyText, outline);
            SetText_Green(item.MySelectedText, outline);
        }

        public static void SetItemProperties_Red(MenuItem item)
        {
            if (item.MyText == null) return;
            item.MyText.OutlineColor = new Color(191, 191, 191).ToVector4();
            item.MyText.MyFloatColor = new Color(175, 8, 64).ToVector4();
            item.MySelectedText.OutlineColor = new Color(205, 205, 205).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(242, 12, 85).ToVector4(); 
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;

            ReturnToCallerDelay = 4;
            CallDelay = 33;
            SlideLength = 36;
            DestinationScale *= 1.015f;

            MyPile = new DrawPile();

            MyMenu = new Menu(false);

            MyMenu.Control = -2;

            MyMenu.CheckForOutsideClick = false;
            MyMenu.OnB = new StartMenuExitLambda(this);

            FontScale *= .88f;
            PosAdd = new Vector2(0, -117);

            // Make the start menu
            MakeMenu();
        }

        class StartMenuExitLambda : LambdaFunc_1<Menu, bool>
        {
            StartMenu sm;
            public StartMenuExitLambda(StartMenu sm)
            {
                this.sm = sm;
            }

            public bool Apply(Menu menu)
            {
                sm.Exit(); return false;
            }
        }

        private static void GrayItem(MenuItem item)
        {
            item.MyText.MyFloatColor = ColorHelper.Gray(.535f);
            item.MySelectedText.MyFloatColor = ColorHelper.Gray(.55f);
        }

        void MakeMenu()
        {
            MenuItem item;

            // Arcade
            item = new MenuItem(new EzText(Localization.Words.TheArcade, ItemFont));
            item.Name = "Arcade";
            item.Go = new StartMenuLambda_Arcade(this);
            AddItem(item);

            // Campaign
            item = new MenuItem(new EzText(Localization.Words.StoryMode, ItemFont));
            item.Name = "Campaign";
            AddItem(item);
            item.Go = new StartMenuLambda_Campaign(this);

            //// Extra
            //item = new MenuItem(new EzText("Extras", ItemFont));
            //item.Name = "Freeplay";
            //item.Go = null;
            //AddItem(item);

            // Free Play
            item = new MenuItem(new EzText(Localization.Words.FreePlay, ItemFont));
            item.Name = "Freeplay";
            item.Go = new StartMenuLambda_Freeplay(this);
            AddItem(item);

            //// Jukebox
            //item = new MenuItem(new EzText("Jukebox", ItemFont));
            //item.Name = "Jukebox";
            //item.Go = MenuGo_ScreenSaver;
            //AddItem(item);

            // Options
            item = new MenuItem(new EzText(Localization.Words.Options, ItemFont));
            item.Name = "Options";
            item.Go = new StartMenuLambda_Options(this);
            AddItem(item);

            // Stats
            //item = new MenuItem(new EzText("Stats", ItemFont));
            //item.Go = new StartMenuLambda_Stats(this);
            //AddItem(item);

            // Exit
            item = new MenuItem(new EzText(Localization.Words.Exit, ItemFont));
            item.Name = "Exit";
            item.Go = new StartMenuLambda_Exit(this);
            AddItem(item);

            EnsureFancy();

            this.CallToLeft = true;
        }

        void BlackBox()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2206.164f, 346.4168f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2118.89f, 148.8611f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); }
            _item = MyMenu.FindItemByName("Jukebox"); if (_item != null) { _item.SetPos = new Vector2(2.666809f, -843.4722f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2090.221f, -213.25f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -413.5834f); }

            MyMenu.Pos = new Vector2(1737.697f, -212.8573f);

            QuadClass _q;
            _q = MyPile.FindQuad("TitleScreen"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1799.546f, 1012.247f); }
            _q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-94.44531f, -355.5555f); _q.Size = new Vector2(458.5224f, 560.4163f); }
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-25f, 88.88889f); _q.Size = new Vector2(1690.654f, 950.9934f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        void Centered()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2206.164f, 346.4168f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2116.112f, 148.8611f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); }
            _item = MyMenu.FindItemByName("Jukebox"); if (_item != null) { _item.SetPos = new Vector2(2.666809f, -843.4722f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2087.443f, -213.25f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -410.8056f); }

            MyMenu.Pos = new Vector2(1715.474f, -154.5238f);

            QuadClass _q;
            _q = MyPile.FindQuad("TitleScreen"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1799.547f, 1012.247f); }
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-25f, 88.88889f); _q.Size = new Vector2(1690.655f, 950.9938f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        void Forest()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-45.05542f, 907.5278f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(89.4444f, 690.5278f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-17.33331f, 509.6389f); }
            _item = MyMenu.FindItemByName("Jukebox"); if (_item != null) { _item.SetPos = new Vector2(49.88898f, 312.0833f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(112.5558f, 114.5278f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(357.5555f, -83.02777f); }

            MyMenu.Pos = new Vector2(746.03f, -312.8573f);

            MyPile.Pos = new Vector2(0f, 0f);
        }

        void Title3()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-45.05542f, 907.5278f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(89.4444f, 690.5278f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-17.33331f, 509.6389f); }
            _item = MyMenu.FindItemByName("Jukebox"); if (_item != null) { _item.SetPos = new Vector2(49.88898f, 312.0833f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(112.5558f, 114.5278f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(357.5555f, -83.02777f); }

            MyMenu.Pos = new Vector2(-228.97f, -551.7461f);

            QuadClass _q;
            _q = MyPile.FindQuad("TitleScreen"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1799.551f, 1012.248f); }
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-25f, 88.88889f); _q.Size = new Vector2(1690.66f, 950.9964f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        protected override void SetChildControl(GUI_Panel child)
        {
        }

        protected override void MyPhsxStep()
        {
            MyMenu.Active = !NoBack;

            base.MyPhsxStep();

            if (Active)
                ButtonCheck.PreLogIn = true;
        }

        class DoneWithCharSelectProxy : Lambda
        {
            StartMenu sm;

            public DoneWithCharSelectProxy(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply()
            {
                sm.DoneWithCharSelect();
            }
        }

        private void DoneWithCharSelect()
        {
            MyGame.WaitThenDo(0, new BringNextMenuLambda(this));
        }

        class BringNextMenuLambda : Lambda
        {
            StartMenu sm;

            public BringNextMenuLambda(StartMenu sm)
            {
                this.sm = sm;
            }

            public void Apply()
            {
                sm.BringNextMenu();
            }
        }

        public virtual void BringNextMenu()
        {
            switch (MyNextMenu)
            {
                case Next.Campaign: BringCampaign(); break;
                case Next.Arcade: BringArcade(); break;
                case Next.Freeplay: BringFreeplay(); break;
            }
        }

        protected virtual void BringCampaign()
        {
        }

        protected virtual void BringArcade()
        {
        }

        protected virtual void BringFreeplay()
        {
            //Call(new CustomLevel_GUI());
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
            NoBack = false;
        }
    }
}