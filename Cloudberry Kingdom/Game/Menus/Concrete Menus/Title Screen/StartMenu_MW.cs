using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW : StartMenu
    {
        protected override void MenuGo_Options(MenuItem item)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            Call(new StartMenu_MW_Options(Control, true), 0);
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
        public StartMenu_MW(TitleGameData_MW Title) : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            //Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur);
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Title);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void BringCampaign()
        {
            base.BringCampaign();

            Call(new StartMenu_MW_Campaign(Title));
        }

        protected override void BringArcade()
        {
            base.BringArcade();

            Call(new StartMenu_MW_Arcade(Title));
        }

        protected override void BringFreeplay()
        {
            base.BringFreeplay();

            PlayerManager.Players[0].Exists = true;
            PlayerManager.Players[1].Exists = true;
            PlayerManager.Players[2].Exists = true;
            PlayerManager.Players[3].Exists = true;
            PlayerManager.GetNumPlayers();

            SkipCallSound = true;
            Call(new StartMenu_MW_CustomLevel(Title));
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

            var Header = new MenuItem(new EzText(Localization.Words.Menu, ItemFont));
            Header.ScaleText(1.3f);
            SetItemProperties(Header);
            Header.Selectable = false;
            MyMenu.Add(Header, 0);
            MyMenu.SelectItem(1);

            BackBox = new QuadClass("Title_Strip");
            BackBox.Alpha = .9f;
            MyPile.Add(BackBox, "Back");

            MyPile.FadeIn(.33f);

            //BlackBox();
            SmallBlackBox();
        }

        QuadClass BackBox;

        void BlackBox()
        {
            EnsureFancy();
        }

        void SmallBlackBox()
        {
            BackBox.TextureName = "White";
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;

            MenuItem _item;
            _item = MyMenu.FindItemByName(""); if (_item != null) { _item.SetPos = new Vector2(255.5566f, -8.333374f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2232.778f, 337.7501f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2233.943f, 149.1946f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2090.221f, -213.25f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -413.5834f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1709.92f, -246.1907f);

            QuadClass _q;
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-61.11133f, -336.1111f); _q.Size = new Vector2(524.4158f, 524.4158f); }

            MyPile.Pos = new Vector2(-27.77734f, -33.33337f);
        }
    }
}