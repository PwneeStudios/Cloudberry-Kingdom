using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Forest : StartMenu
    {
        protected override void BringNextMenu()
        {
            base.BringNextMenu();

            Hide();
        }

        public TitleGameData_Forest Title;
        public StartMenu_Forest(TitleGameData_Forest Title) : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
            Title.Title.SetPos_Menu();
            Title.SetBackground("forest_snow");
            Title.Title.MyPile.FadeIn(100);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void BringCampaign()
        {
            base.BringCampaign();

            Call(new StartMenu_Forest_Campaign(Title));
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

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            //BackBox = new QuadClass("Title_Strip");
            //BackBox.Alpha = .9f;
            //MyPile.Add(BackBox, "BackBox");

            BackBox = new QuadClass("White"); BackBox.Quad.SetColor(Color.Black);
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;
            MyPile.Add(BackBox, "BackBox");

            BlackBox();
            //SetPos();
        }

        QuadClass BackBox;

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2100.609f, 365.8612f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2118.89f, 148.8611f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2090.221f, -213.25f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -413.5834f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }

            MyMenu.Pos = new Vector2(1229.364f, -218.4129f);

            QuadClass _q;
            _q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-61.11206f, 25.00008f); _q.Size = new Vector2(774.0089f, 1013.248f); }

            MyPile.Pos = new Vector2(-488.8894f, -11.11102f);
        }

        void BlackBox()
        {
            //MenuItem _item;
            //_item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2100.609f, 365.8612f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            //_item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2118.89f, 148.8611f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            //_item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            //_item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2090.221f, -213.25f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            //_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -413.5834f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }

            //MyMenu.Pos = new Vector2(1229.364f, -218.4129f);

            //QuadClass _q;
            //_q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-94.44531f, -355.5555f); _q.Size = new Vector2(458.5224f, 560.4163f); }

            //MyPile.Pos = new Vector2(-488.8894f, -11.11102f);

            MenuItem _item;
            _item = MyMenu.FindItemByName("Campaign"); if (_item != null) { _item.SetPos = new Vector2(-2086.72f, 343.639f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Arcade"); if (_item != null) { _item.SetPos = new Vector2(-2118.89f, 148.8611f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-2156.22f, -34.80548f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Options"); if (_item != null) { _item.SetPos = new Vector2(-2090.221f, -213.25f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(-1950.778f, -413.5834f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }

            MyMenu.Pos = new Vector2(1715.475f, -193.4129f);

            QuadClass _q;
            _q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-94.44531f, -355.5555f); _q.Size = new Vector2(458.5224f, 560.4163f); }

            MyPile.Pos = new Vector2(-13.88965f, 11.11115f);
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

        float t = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            t += .01f;
        }
    }
}