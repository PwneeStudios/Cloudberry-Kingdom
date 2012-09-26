using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Intense : StartMenu
    {
        protected override void BringNextMenu()
        {
            base.BringNextMenu();

            Hide();
        }

        public StartMenu_Intense() : base()
        {
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
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

            BackBox = new QuadClass("White"); BackBox.Quad.SetColor(Color.Black);
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;
            MyPile.Add(BackBox, "BackBox");

            BlackBox();
        }

        QuadClass BackBox;

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
            _q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-94.44531f, -355.5555f); _q.Size = new Vector2(458.5224f, 560.4163f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}