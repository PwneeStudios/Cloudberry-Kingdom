using Microsoft.Xna.Framework;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class CategoryMenu : StartMenuBase
    {
        bool Long = false;

        protected CategoryPics pics { get { return RightPanel as CategoryPics; } }
        protected override void SlideIn_RightPanel(int Frames)
        {
            RightPanel.SlideOut(SlideInFrom, 0);
            RightPanel.SlideIn(Frames);
        }
        protected override void SlideOut_RightPanel(PresetPos Preset, int Frames)
        {
            RightPanel.SlideOut(Preset, Frames);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.MyFloatColor = new Vector4(1, 1, 1, 1);
            text.Scale = FontScale * 1.42f;
            text.ShadowOffset = new Vector2(17);
        }

        public override void Call(GUI_Panel child, int Delay)
        {
            base.Call(child, Delay);

            Hide();
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            item.MyText.Scale = item.MySelectedText.Scale *= 1.06f;
        }

        void MenuGo_Career(MenuItem item)
        {
            Call(new CampaignMenu());
        }

        void MenuGo_Doom(MenuItem item)
        {
            Hide();

            MyGame.WaitThenDo(18, () => MyGame.FadeToBlack(.033f));

            MyGame.WaitThenDo(55, () =>
            {
                // Todo for when doom exits
                MyGame.AddToDo(() =>
                {
                    GameData game = MyGame;

                    MyGame.Black();
                    MyGame.WaitThenDo(10, () =>
                        game.FadeIn(.033f));
                    MyGame.MyLevel.UseLighting = false;
                    //ReturnToCaller();
                    //SlideOut(PresetPos.Left, 0);

                    MyGame.WaitThenDo(23, () =>
                        Show());
                });
                MyGame.PhsxStepsToDo = 3;

                Campaign.InitCampaign(0);
                Tools.CurGameData = new Doom();
            });
        }

        void MenuGo_Awardments(MenuItem item)
        {
            Call(new AwardmentMenu());
        }

        void MenuGo_Shop(MenuItem item)
        {
            Call(new ShopMenu());
        }

        void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(StatGroup.Lifetime));
        }

        protected PresetPos ReturnSlideOutTo = PresetPos.Top;
        protected PresetPos OnAddSlideOutTo = PresetPos.Top;
        protected bool UseAdditionalSlideOutPos = false;

#if PC_VERSION
        public override void OnAdd()
        {
            base.OnAdd();
            AdditionalOnAdd();

            if (Long)
            {
                ScrollBar bar = new ScrollBar((LongMenu)MyMenu, this);
                bar.BarPos = new Vector2(-2384.921f, 135);
                MyGame.AddGameObject(bar);
            }
        }
        public override void ReturnToCaller()
        {
            if (UseAdditionalSlideOutPos)
                SlideOutTo = ReturnSlideOutTo;
            base.ReturnToCaller();
        }
#else
        public override void OnAdd()
        {
            base.OnAdd();
            AdditionalOnAdd();
        }
        public override void ReturnToCaller()
        {
            DestinationScale *= 1.03f;
            SlideLength = 20;
            SlideOutTo = ReturnSlideOutTo;
            base.ReturnToCaller();
        }
#endif
        void AdditionalOnAdd()
        {
            if (UseAdditionalSlideOutPos)
            {
                SlideOut(OnAddSlideOutTo, 0);
                SlideIn();
                RightPanel.SlideOut(OnAddSlideOutTo, 0);
                RightPanel.SlideIn();
            }
        }

        public CategoryMenu()
        {
            CallDelay = 20;
            
            SlideLength = 27;
#if PC_VERSION
            DestinationScale *= 1.015f;
            ReturnToCallerDelay = 8;
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;
#else
            UseAdditionalSlideOutPos = true;
            ReturnToCallerDelay = 37;
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;
#endif

            ItemPos = new Vector2(-1630, 572);
            //PosAdd = new Vector2(0, -151) * 1.2f * 1.4f;
            PosAdd = new Vector2(0, -151) * 1.2f * 1.2f;

            RightPanel = new CategoryPics();

            MyPile = new DrawPile();

            // Header
            //EzText Header = new EzText("Games!", Tools.Font_Dylan60);// ItemFont);
            //MyPile.Add(Header);
            //Header.Pos =
            //    new Vector2(-1295.316f, 725.3176f) + new Vector2(-297.6191f, 15.87299f);
            //SetHeaderProperties(Header);
            //Header.Scale *= 1.15f;



            // Menu
            if (Long)
            {
                MyMenu = new LongMenu();
                MyMenu.FixedToCamera = false;
                MyMenu.WrapSelect = false;
            }
            else
                MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;


            // Header
            //string header = "Games!";
            string header = "Menu";
            MenuItem Header = new MenuItem(new EzText(header, Tools.Font_Dylan60));
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1834.998f, 999.1272f);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;



            MenuItem item;
            ItemPos = new Vector2(-1689.523f, 520.4127f);

            // Campaign
            //string campaign = "Campaign";
            string campaign = "Classic";
            item = new MenuItem(new EzText(campaign, ItemFont));
            AddItem(item);
            item.Go = MenuGo_Career;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_classic", "", new Vector2(388.8888f, -222.222f), false, Vector2.Zero);

            //Arcade
            item = new MenuItem(new EzText("Arcade", ItemFont));
            item.Go = menuitem => Call(new ArcadeMenu());
            AddItem(item);
            item.AdditionalOnSelect = () => pics.Set("menupic_arcade", "", Vector2.Zero, false);
            
            //item = new MenuItem(new EzText("Hero Factory", ItemFont));
            ////item.Go = menuitem => Call(new HeroFactoryMenu());
            //item.Go = menuitem => Call(new HeroFactoryMenu2());
            //AddItem(item);
            //item.AdditionalOnSelect = () => pics.Set("menupic_arcade", "", Vector2.Zero, false);


            
            // Doom
            //item = new MenuItem(new EzText("Doom", ItemFont));
            //AddItem(item);
            //item.Go = MenuGo_Doom;
            //item.AdditionalOnSelect = () =>
            //        pics.Set("CategoryBack", "Doom", new Vector2(575.3966f, -444.4442f),
            //        true, new Vector2(924.6016f, -662.6984f));

            // Free Play
            item = new MenuItem(new EzText("Free Play", ItemFont));
            item.Go = menuitem => Call(new SimpleCustomGUI());
            AddItem(item);
            item.AdditionalOnSelect = () => pics.Set("menupic_freeplay", "", Vector2.Zero,
                false);//, new Vector2(611.1113f, -726.1903f));

            /*
            // Hero Rush
            item = new MenuItem(new EzText("Hero Rush", ItemFont));
            AddItem(item);
            item.AdditionalOnSelect = () => pics.Set("HeroRush", "", Vector2.Zero,
                false, new Vector2(742.0625f, -738.0952f));
            item.Go = menuitem =>
            {
                DifficultyMenu levelmenu = new LevelMenu();
                levelmenu.ShowHighScore(SaveGroup.HeroRushHighScore.Top);
                levelmenu.MyMenu.SelectItem(Challenge_HeroRush.PreviousMenuIndex);

                levelmenu.StartFunc = (level, menuindex) =>
                    {
                        // Save the menu item index
                        Challenge_HeroRush.PreviousMenuIndex = menuindex;

                        // Start the game
                        MyGame.PlayGame(() =>
                            {
                                // Show Hero Rush title again if we're selecting from the menu
                                if (!MyGame.ExecutingPreviousLoadFunction)
                                    HeroRushTutorial.ShowTitle = true;

                                Challenge_HeroRush.Instance.Start(level);
                            });
                    };

                levelmenu.ReturnFunc = () => { };

                Call(levelmenu);
            };
            */

            // Awardments
            item = new MenuItem(new EzText("Awardments", ItemFont));
            AddItem(item);
            item.Go = MenuGo_Awardments;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_awardments", "", new Vector2(388.8888f, -222.222f), false, Vector2.Zero);

            // Hat shop
            item = new MenuItem(new EzText("Shop", ItemFont));
            AddItem(item);
            item.Go = MenuGo_Shop;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_shop", "", new Vector2(388.8888f, -222.222f), false);

            // Back button
            item = MakeBackButton();
            //item.AdditionalOnSelect = () => pics.Set("menupic_back", "", new Vector2(634.922f, -349.2057f), false, Vector2.Zero);
            //item.AdditionalOnSelect = () => pics.Set("menupic_bg_cloud", "", new Vector2(634.922f, -349.2057f), false, Vector2.Zero);
            item.AdditionalOnSelect = () => pics.Set(null, "", new Vector2(634.922f, -349.2057f), false, Vector2.Zero);

            // Backdrop
            QuadClass backdrop;
            
            backdrop = new QuadClass("WoodMenu_1", 1500);
            if (Long)
                backdrop.SizeY *= 1.02f;
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(9.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            // Position
            EnsureFancy();
            MyMenu.Pos = new Vector2(332, -40f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            MyMenu.SelectItem(1);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}