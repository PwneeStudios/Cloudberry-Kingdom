using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

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

        protected virtual void MenuGo_Campaign(MenuItem item)
        {
            MyNextMenu = Next.Campaign;
            BringCharacterSelect();
        }

        protected virtual void MenuGo_Arcade(MenuItem item)
        {
            MyNextMenu = Next.Arcade;
            BringCharacterSelect();
            //DoneWithCharSelect();StartMenu_MW_HeroSelect
        }

        protected virtual void MenuGo_Freeplay(MenuItem item)
        {
            MyNextMenu = Next.Freeplay;
            BringCharacterSelect();
            //DoneWithCharSelect();
        }

        /// <summary>
        /// When true the user can not selected back.
        /// </summary>
        protected bool NoBack = false;

        protected virtual void BringCharacterSelect()
        {
			UseBounce = false;
			ReturnToCallerDelay = 0;

            NoBack = true;
            MyGame.SlideOut_FadeIn(20, CharacterSelect);
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
            CharacterSelectManager.Start(this, false);
            CharacterSelectManager.OnDone = DoneWithCharSelect;
            CharacterSelectManager.OnBack = null;
        }

        protected virtual void MenuGo_ScreenSaver(MenuItem item)
        {
            ScreenSaver Intro = new ScreenSaver(); Intro.Init();
            Tools.TheGame.LogoScreenPropUp = false;
            Tools.AddToDo(() => MyGame.Release());
        }

        protected virtual void MenuGo_Controls(MenuItem item)
        {
            Call(new ControlScreen(Control), 0);
        }

        protected virtual void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(StatGroup.Lifetime), 0);
        }

        protected virtual void MenuGo_Options(MenuItem item)
        {
            Call(new SoundMenu(Control, false), 0);
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
			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.TitleScreen);

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
            MyMenu.OnB = menu =>
                { Exit(); return false; };

            FontScale *= .88f;
            PosAdd = new Vector2(0, -117);

            // Make the start menu
            MakeMenu();
        }

        private static void GrayItem(MenuItem item)
        {
            item.MyText.MyFloatColor = ColorHelper.Gray(.535f);
            item.MySelectedText.MyFloatColor = ColorHelper.Gray(.55f);
        }

        protected virtual void MakeMenu()
        {
            EnsureFancy();

            this.CallToLeft = true;
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

        private void DoneWithCharSelect()
        {
            MyGame.WaitThenDo(0, BringNextMenu);
        }

        protected virtual void BringNextMenu()
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
			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.TitleScreen);

            base.OnReturnTo();
            NoBack = false;
        }
    }
}