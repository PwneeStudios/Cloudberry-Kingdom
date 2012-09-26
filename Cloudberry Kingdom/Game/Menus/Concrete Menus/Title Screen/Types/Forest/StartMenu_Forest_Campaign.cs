using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Forest_Campaign : StartMenu
    {
        public TitleGameData_Forest Title;
        public StartMenu_Forest_Campaign(TitleGameData_Forest Title)
            : base()
        {
            this.Title = Title;
        }

        protected override void MyPhsxStep()
        {
            if (MyGame.IsFading()) return;
            base.MyPhsxStep();
        }

        public override void SlideIn(int Frames)
        {
            Title.Title.SetPos_Campaign();
            //Title.SetBackground("KobblerPie");

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

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            //var BackBox = new QuadClass("Title_Strip");
            //BackBox.Alpha = .9f;
            //MyPile.Add(BackBox, "BackBox");

            var BackBox = new QuadClass("White"); BackBox.Quad.SetColor(Color.Black);
            BackBox.Quad.SetColor(CoreMath.Gray(.1f));
            BackBox.Alpha = .73f;
            MyPile.Add(BackBox, "BackBox");

            MyMenu.ClearList();

            var Header = new MenuItem(new EzText("Career", ItemFont));
            Header.Name = "Header";
            Header.ScaleText(1.3f);
            SetItemProperties(Header);
            Header.Selectable = false;
            MyMenu.Add(Header);

            MenuItem item;

            // Story of the Orb
            item = new MenuItem(new EzText("Story of the Orb", ItemFont));
            item.Name = "MainCampaign";
            item.Go = null;
            AddItem(item);

            // Easy
            item = new MenuItem(new EzText("Mini-Quests", ItemFont));
            item.Name = "Easy";
            item.Go = null;
            AddItem(item);

            // Hard
            item = new MenuItem(new EzText("Abusive Quests", ItemFont));
            item.Name = "Hard";
            item.Go = null;
            AddItem(item);

            // Hardcore
            item = new MenuItem(new EzText("Hardcore Quests", ItemFont));
            item.Name = "Hardcore";
            item.Go = null;
            AddItem(item);

            // Masochistic
            item = new MenuItem(new EzText("Masochist", ItemFont));
            item.Name = "Maso";
            item.Go = null;
            AddItem(item);

            MyMenu.SelectItem(1);
            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-58.33594f, 363.8889f); _item.MyText.Scale = 1.100083f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("MainCampaign"); if (_item != null) { _item.SetPos = new Vector2(36.44531f, 24.99997f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Easy"); if (_item != null) { _item.SetPos = new Vector2(3.110352f, -175.3334f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Hard"); if (_item != null) { _item.SetPos = new Vector2(3.111328f, -367.3335f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Hardcore"); if (_item != null) { _item.SetPos = new Vector2(8.666016f, -553.7778f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }
            _item = MyMenu.FindItemByName("Maso"); if (_item != null) { _item.SetPos = new Vector2(8.667969f, -737.4445f); _item.MyText.Scale = 0.66f; _item.MySelectedText.Scale = 0.66f; }

            MyMenu.Pos = new Vector2(305.5553f, 358.3334f);

            QuadClass _q;
            _q = MyPile.FindQuad("BackBox"); if (_q != null) { _q.Pos = new Vector2(-119.448f, 5.722046E-05f); _q.Size = new Vector2(788.996f, 788.996f); }

            MyPile.Pos = new Vector2(3333.333f, -8.333237f);
        }
    }
}