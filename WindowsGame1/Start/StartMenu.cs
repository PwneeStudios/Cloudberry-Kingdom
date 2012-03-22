using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu : StartMenuBase
    {
        public override void Hide(PresetPos pos, int frames)
        {
            base.Hide(pos, frames);
            ButtonCheck.PreLogIn = false;
        }

        void MenuGo_Start(MenuItem item)
        {
#if PC_VERSION
            SlideInFrom = PresetPos.Right;
            Hide(PresetPos.Right);

            // PC version goes straight to category menu
            Call(new CategoryMenu());
#else
            Hide(PresetPos.Top);

            // XBox goes to sign in / char select, unless we are debugging
            //if (Tools.TheGame.SimpleLoad)
            //    Call(new CategoryMenu());
            //else
                CharSelect();
#endif
        }

#if PC_VERSION
        void MenuGo_Customize(MenuItem item)
        {
            SlideInFrom = PresetPos.Left;
            Hide(PresetPos.Left);
            CharSelect();
        }
#endif

        void CharSelect()
        {
            CharacterSelectManager.ParentPanel = null;
            MyGame.WaitThenDo(CallDelay, () =>
            {
#if PC_VERSION
                CharacterSelectManager.Start(0, false);
#else
                for (int i = 0; i < 4; i++)
                    CharacterSelectManager.Start(i, false);
#endif
            }, "StartCharSelect");
        }

        void MenuGo_ScreenSaver(MenuItem item)
        {
            ScreenSaver Intro = new ScreenSaver(); Intro.Init();
            Tools.TheGame.LogoScreenPropUp = false;
            Tools.AddToDo(() => MyGame.Release());
        }

        void MenuGo_Controls(MenuItem item)
        {
            //Hide(PresetPos.Left);
            //SlideOnReturn = true;
            Call(new ControlScreen(Control), 0);
        }

        void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(StatGroup.Lifetime), 0);
        }

        void MenuGo_Options(MenuItem item)
        {
            //Hide(PresetPos.Left);
            //SlideOnReturn = true;
            Call(new SoundMenu(Control, true), 0);
        }

        void MenuGo_Exit(MenuItem item)
        {
            Exit();
        }

        void Exit()
        {
            SelectSound = null;
            Call(new VerifyQuitGameMenu2(Control), 0);
        }

        public StartMenu()
        {
            if (Tools.TheGame.LoadingScreen != null)
                Tools.TheGame.LoadingScreen.IsDone = true;
        }

#if PC_VERSION
        bool BubbledIn = false;
        public override void SlideIn(int Frames)
        {
            if (BubbledIn)
                base.SlideIn(SlideInLength);
            else
            {
                MyPile.BubbleUp(true, 8, 1f);

                //SlideInLength = 0;
                base.SlideIn(0);//SlideInLength);

                if (MyMenu != null)
                    MyMenu.FancyPos.LerpTo(MenuPos_Down, MenuPos, 40);

                BubbledIn = true;
            }
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
        }
#else
        /// <summary>
        /// When true the menu slides in, rather than bubbles in, when a child menu returns control to it
        /// </summary>
        bool SlideOnReturn = false;

        public override void SlideIn(int Frames)
        {
            if (SlideOnReturn)
                base.SlideIn(Frames);
            else
            {
                //SlideInLength = 0;
                //base.SlideIn(SlideInLength);

                base.SlideIn(0);

                MyPile.BubbleUp(true, 8, 1);

                MyMenu.FancyPos.LerpTo(MenuPos_Down, MenuPos, 40);
            }

            SlideOnReturn = false;
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        //{
        //    base.SlideOut(Preset, Frames);

        //    bool sound = true;
        //    if (Frames == 0) sound = false; // Don't make a popping sound if this is an instaneous slideout

        //    sound = false; // Never make a popping noise when we are popping out

        //    MyPile.BubbleDown(sound, 130);
        //}
        {
            base.SlideOut(Preset, Frames);
            //base.SlideOut(PresetPos.Top, Frames);
        }
#endif
        static Vector2 MenuPos = new Vector2(746.03f, -312.8573f);
        static Vector2 MenuPos_Down = MenuPos - new Vector2(0, 1000);

        void AddItem(MenuItem item, float scale_mult)
        {
            base.AddItem(item);

            item.MyText.Scale *= scale_mult;
            item.MySelectedText.Scale *= scale_mult;

            ModItem(item);
        }

        void ModItem(MenuItem item)
        {
            item.MyText.Shadow = item.MySelectedText.Shadow = false;
            
            Vector2 add = -item.MyText.GetWorldSize() / 2;
            item.Pos += add;
            item.SelectedPos += add;
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            ModItem(item);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        public override void  Init()
        {
 	        base.Init();

#if PC_VERSION
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;

            ReturnToCallerDelay = 0;
            CallDelay = 12;
#else
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;

            ReturnToCallerDelay = 4;
            CallDelay = 33;
            SlideLength = 36;
            DestinationScale *= 1.015f;
#endif



            MyPile = new DrawPile();

            QuadClass backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "logo3";
            //backdrop.TextureName = "CK logo new";
            backdrop.ScaleYToMatchRatio(1050);

            if (!TitleGameData.PromoTitle)
                MyPile.Add(backdrop);



            //TopPanel = new StartMenuCK();
            //RightPanel = new StartMenuBerry();

            if (TitleGameData.PromoTitle)
                return;

            MyMenu = new Menu(false);

            MyMenu.Control = -2;

            MyMenu.CheckForOutsideClick = false;
            MyMenu.OnB = menu =>
                //{ Exit(); return false; };
                { MenuGo_ScreenSaver(null); return false; };

            FontScale *= .88f;
            PosAdd = new Vector2(0, -117);

            MenuItem item;

            bool Center = false, yCenter = false;
            item = new MenuItem(new EzText("Start", ItemFont, Center, yCenter));
            item.Go = MenuGo_Start;
            AddItem(item, 1.3f);
            ItemPos.Y -= 95;

            //item = new MenuItem(new EzText("Watch", ItemFont, Center, yCenter));
            //item.Go = MenuGo_ScreenSaver;
            //AddItem(item);

#if PC_VERSION
            item = new MenuItem(new EzText("Customize", ItemFont, Center, yCenter));
            //item = new MenuItem(new EzText("Tailor", ItemFont, Center, yCenter));
            item.Go = MenuGo_Customize;
            AddItem(item);
#endif

#if NOT_PC
            item = new MenuItem(new EzText("Controls", ItemFont, Center, yCenter));
            item.Go = MenuGo_Controls;
            AddItem(item);
#endif
            item = new MenuItem(new EzText("Options", ItemFont, Center, yCenter));
            item.Go = MenuGo_Options;
            AddItem(item);

            bool IncludeStats = false;
            if (IncludeStats)
            {
            item = new MenuItem(new EzText("Stats", ItemFont, Center, yCenter));
            item.Go = MenuGo_Stats;
            AddItem(item);
            }

            item = new MenuItem(new EzText("Exit", ItemFont, Center, yCenter));
            item.Go = MenuGo_Exit;
            //item.Pos = item.SelectedPos = new Vector2(-1024.223f, 30.63501f);
            AddItem(item);

            EnsureFancy();
            MyMenu.Pos = MenuPos;
            if (IncludeStats) MyMenu.Pos += new Vector2(0, 80);
        }


        /// <summary>
        /// Once the players have finished choosing their hero start counting
        /// </summary>
        int FinishedCount = 0;

        /// <summary>
        /// Once the players have exited out of the character select start counting
        /// </summary>
        int ExitCount = 0;

        protected override void SetChildControl(GUI_Panel child)
        {
        //    if (child.Control < 0)
          //      child.Control = -1;
        }

        int Count = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
            Count++;

            if (Active)
                ButtonCheck.PreLogIn = true;

            // Start music
            if (Count == 20)
            {
                Tools.PlayHappyMusic();
                Tools.SongWad.Unpause();

                //Tools.StartPlaylist();
                //Tools.SongWad.SetSong(0);
                ////Tools.SongWad.SetSong("The Heavens Opened^Peacemaker");
                //Tools.SongWad.DisplayInfo = false;
            }

            // Check for finishing from character selection
            if (!this.Active && CharacterSelectManager.AllFinished()
                && CharacterSelectManager.IsShowing)
            {
                FinishedCount++;
                
#if PC_VERSION
                int Delay = 120;
#else
                int Delay = 126;
#endif
                if (FinishedCount == Delay)
                {
                    CharacterSelectManager.SetToLeave();

                    MyGame.WaitThenDo(10, () =>
                        {
                            CategoryMenu menu = new CategoryMenu();
                            Call(menu);
                            menu.Control = -1;
                        });
                }
            }
            else
                FinishedCount = 0;

#if PC_VERSION
            int DelayOnReturn = 12;
            int DelayKillCharSelect = 30;
#else
            int DelayOnReturn = 40;
            int DelayKillCharSelect = 40;
#endif
            // Check for ready to exit from character selection
            if (!this.Active && CharacterSelectManager.AllExited()
                && CharacterSelectManager.IsShowing)
            {
                ExitCount++;
#if PC_VERSION
                if (ButtonCheck.State(ControllerButtons.B, -2).Pressed
                    //|| ExitCount > 45)
                    || ExitCount > 2)
#else
                if (ButtonCheck.State(ControllerButtons.B, -2).Pressed)
#endif
                {
                    CharacterSelectManager.SlideOutAll();

                    // End the character selection
                    MyGame.WaitThenDo(DelayKillCharSelect, () => CharacterSelectManager.FinishAll());

                    MyGame.WaitThenDo(DelayOnReturn, () =>
                        {
                            // Kill any ToDo functions that might restart the character selection
                            MyGame.KillToDo("StartCharSelect");

                            if (CharacterSelectManager.ParentPanel != null)
                                MyGame.WaitThenDo(5, () => CharacterSelectManager.ParentPanel.Show());
                            else
                                MyGame.WaitThenDo(ReturnToCallerDelay, () => this.Show());
                        });
                }
            }
            else
                ExitCount = 0;
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}