using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Pre : StartMenu
    {
        bool CallingOptionsMenu;
        protected override void MenuGo_Options(MenuItem item)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            Call(new StartMenu_MW_Options(Control, true), 0);
            CallingOptionsMenu = true;
        }

        protected override void Exit()
        {
            SelectSound = null;
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            Call(new StartMenu_MW_Exit(Control), 0);
        }

        protected override void BringNextMenu()
        {
            base.BringNextMenu();

            Hide();
        }

        public TitleGameData_MW Title;
        public StartMenu_MW_Pre(TitleGameData_MW Title) : base()
        {
            this.Title = Title;
            CallingOptionsMenu = false;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Title);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void OnReturnTo()
        {
            if (CallingOptionsMenu)
            {
                MyMenu.SelectItem(4);
                CallingOptionsMenu = false;
            }

            base.OnReturnTo();
        }

        public override bool MenuReturnToCaller(Menu menu)
        {
            if (NoBack) return false;

            return base.MenuReturnToCaller(menu);
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            BackBox = new QuadClass("Title_Strip");
            BackBox.Alpha = .9f;
            MyPile.Add(BackBox, "Back");

            MyPile.FadeIn(.33f);

            SmallBlackBox();
        }

        void MenuGo_Play(MenuItem item)
        {
            Call(new StartMenu_MW(Title), 0);
        }

        void MenuGo_Leaderboards(MenuItem item)
        {
            Call(new LeaderboardGUI(Title, 0), 0);
        }

        void MenuGo_Achievements(MenuItem item)
        {
        }

        void MenuGo_BuyGame(MenuItem item)
        {
        }

        protected override void MakeMenu()
        {
            MenuItem item;

            // Play
            item = new MenuItem(new EzText("Play Game", ItemFont));
            item.Name = "Play";
            item.Go = MenuGo_Play;
            AddItem(item);

            // Leaderboard
            item = new MenuItem(new EzText(Localization.Words.Leaderboard, ItemFont));
            item.Name = "Leaderboard";
            item.Go = MenuGo_Leaderboards;
            AddItem(item);

            // Achievements
            item = new MenuItem(new EzText(Localization.Words.Achievement, ItemFont));
            item.Name = "Achievements";
            item.Go = MenuGo_Achievements;
            AddItem(item);

            //// Options
            //item = new MenuItem(new EzText(Localization.Words.Options, ItemFont));
            //item.Name = "Options";
            //item.Go = MenuGo_Options;
            //AddItem(item);

            // Buy Game
            item = new MenuItem(new EzText(Localization.Words.UnlockFullGame, ItemFont));
            item.Name = "Buy";
            item.Go = MenuGo_BuyGame;
            AddItem(item);

            // Exit
            item = new MenuItem(new EzText(Localization.Words.Exit, ItemFont));
            item.Name = "Exit";
            item.Go = MenuGo_Exit;
            AddItem(item);

            EnsureFancy();

            this.CallToLeft = true;
        }

        QuadClass BackBox;

        void SmallBlackBox()
        {
            BackBox.TextureName = "White";
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;

            MenuItem _item;
            _item = MyMenu.FindItemByName("Play"); if (_item != null) { _item.SetPos = new Vector2(-435.7779f, 168.3334f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-521.889f, -29.22222f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Achievements"); if (_item != null) { _item.SetPos = new Vector2(-538.555f, -229.5555f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Buy"); if (_item != null) { _item.SetPos = new Vector2(-607.9997f, -435.7777f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-402.4451f, -624.9999f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-38.88916f, -30.55554f);

            QuadClass _q;
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-61.11133f, -336.1111f); _q.Size = new Vector2(608.4988f, 520.8323f); }

            MyPile.Pos = new Vector2(-27.77734f, -33.33337f);
        }
    }
}