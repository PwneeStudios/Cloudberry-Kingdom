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
            //item.ScaleText(.9332f);
            item.ScaleText(.9135f);
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
            NoBackIfNoCaller = true;

            SetParams();

            RightPanel = new CategoryPics();

            MyPile = new DrawPile();

            // Header
            //EzText Header = new EzText("Games!", Tools.Font_Grobold42_2);// ItemFont);
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
            string header = "Menu";
            MenuItem Header = new MenuItem(new EzText(header, Tools.Font_Grobold42_2));
            Header.Name = "Header";
            MyMenu.Add(Header);
            Header.Pos =
                new Vector2(-1834.998f, 999.1272f);
            SetHeaderProperties(Header.MyText);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;



            MenuItem item;
            ItemPos = new Vector2(-1689.523f, 520.4127f);

            // Campaign
            string campaign = "Campaign";
            item = new MenuItem(new EzText(campaign, ItemFont));
            item.Name = "Campaign";
            AddItem(item);
            item.Go = null;// MenuGo_Career;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_classic", "", new Vector2(388.8888f, -222.222f), true, new Vector2(484.1266f, 654.762f), new Vector2(685.5175f, 265.7117f), 0.349205f);
            item.MyText.MyFloatColor = new Color(255, 100, 100).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(255, 160, 160).ToVector4();
            GrayItem(item);


            //Arcade
            item = new MenuItem(new EzText("Arcade", ItemFont));
            item.Name = "Arcade";
            item.Go = menuitem => Call(new ArcadeMenu());
            AddItem(item);
            item.AdditionalOnSelect = () => pics.Set("menupic_arcade", "", Vector2.Zero, false);
            
            // Free Play
            item = new MenuItem(new EzText("Free Play", ItemFont));
            item.Name = "Freeplay";
            item.Go = menuitem => Call(new CustomLevel_GUI());
            AddItem(item);
            item.AdditionalOnSelect = () => pics.Set("menupic_freeplay", "", Vector2.Zero,
                false);//, new Vector2(611.1113f, -726.1903f));


            // Awardments
            item = new MenuItem(new EzText("Awardments", ItemFont));
            item.Name = "Awardments";
            AddItem(item);
            item.Go = MenuGo_Awardments;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_awardments", "", new Vector2(388.8888f, -222.222f), false, Vector2.Zero);

            // Hat shop
            item = new MenuItem(new EzText("Shop", ItemFont));
            item.Name = "Shop";
            AddItem(item);
            item.Go = MenuGo_Shop;
            item.AdditionalOnSelect = () =>
                    pics.Set("menupic_shop", "", new Vector2(388.8888f, -222.222f), false);


            // Back button
            item = MakeBackButton();
            item.AdditionalOnSelect = () => pics.Set(null, "", new Vector2(634.922f, -349.2057f), false, Vector2.Zero);

            // Backdrop
            QuadClass backdrop;
            
            backdrop = new QuadClass("Backplate_1500x900", 1500);
            if (Long)
                backdrop.SizeY *= 1.02f;
            MyPile.Add(backdrop, "Backdrop");
            backdrop.Pos = new Vector2(9.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            // Position
            EnsureFancy();
            MyMenu.Pos = new Vector2(332, -40f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            MyMenu.SelectItem(1);

            SetPos();
        }

        void SetPos()
        {
            RightPanel.AutoDraw = false;


            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-1901.664f, 1932.46f); }
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-1756.189f, 687.0794f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-1681.191f, 452.2554f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-1631.189f, 209.0979f); }
            _item = MyMenu.FindItemByName("Awardments"); if (_item != null) { _item.SetPos = new Vector2(-1658.967f, -14.6149f); }
            _item = MyMenu.FindItemByName("Shop"); if (_item != null) { _item.SetPos = new Vector2(-1653.411f, -243.8834f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(-1620.082f, -764.8185f); }

            MyMenu.Pos = new Vector2(2368.111f, -203.8889f);

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(1606.747f, -214.6829f); _q.Size = new Vector2(1465.694f, 879.4163f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        private static void GrayItem(MenuItem item)
        {
            item.MyText.MyFloatColor = Tools.Gray(.535f);
            item.MySelectedText.MyFloatColor = Tools.Gray(.55f);
        }

        private void SetParams()
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
            PosAdd = new Vector2(0, -151) * 1.2f * 1.2f;
            PosAdd *= .85f;
            FontScale *= .925f;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}