using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyQuitGameMenu2 : VerifyBaseMenu
    {
        public VerifyQuitGameMenu2(int Control) : base(Control, true)
        {
            EnableBounce();
        }

        //public static int Version = 0;
        public static int Version = 1;

        QuadClass Berry;
        public override void MakeBackdrop()
        {
            QuadClass backdrop = new QuadClass(null, true, false);
            //backdrop.TextureName = "Backplate_1230x740";
            backdrop.TextureName = "Arcade_BoxLeft";
            backdrop.ScaleYToMatchRatio(1000);
            MyPile.Add(backdrop, "Backdrop");

            Berry = new QuadClass(null, true, false);
            Berry.TextureName = "cb_crying";
            MyPile.Add(Berry, "Berry");
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            //item.MyText.Scale *= 1.15f;
            //item.MySelectedText.Scale *= 1.3f;
            //CkColorHelper.GreenItem(item);

            StartMenu.SetItemProperties_Red(item);
            item.MyText.Shadow = item.MySelectedText.Shadow = false;
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
            text.Scale *= 1.15f;

            text.MyFloatColor = ColorHelper.Gray(.9f);
        }

        public override void Init()
        {
            base.Init();
            
            SlideInFrom = SlideOutTo = PresetPos.Bottom;
            DestinationScale = new Vector2(1.223f);

            // Make the menu
            MenuItem item;

            // Header
            EzText HeaderText = new EzText(Localization.Words.ExitGame, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            if (Version == 0)
                HeaderText.Pos = new Vector2(-915.4741f, 967.5232f);
            if (Version == 1)
                HeaderText.Pos = new Vector2(-701.1883f, 816.7295f);

            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont));
            item.Name = "Yes";
            item.Go = Cast.ToItem(Tools.TheGame.Exit);
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_crying";
            AddItem(item);

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont));
            item.Name = "No";
            item.Go = ItemReturnToCaller;
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_enthusiastic";
            item.SelectSound = null;
            BackSound = null;
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            EnsureFancy();
            SetPosition();
        }

        void SetPosition()
        {
            //MenuItem _item;
            //_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(800f, 361f); _item.MyText.Scale = 0.92f; _item.MySelectedText.Scale = 1.04f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            //_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(800f, 61f); _item.MyText.Scale = 0.92f; _item.MySelectedText.Scale = 1.04f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            //MyMenu.Pos = new Vector2(-396.8268f, -265.873f);

            //EzText _t;
            //_t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-442.855f, 605.6183f); _t.Scale = 0.7911667f; }

            //QuadClass _q;
            //_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(1065.378f, 640.9592f); }
            //_q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(16.6665f, -25f); _q.Size = new Vector2(398.1559f, 537.0001f); }

            //MyPile.Pos = new Vector2(13.8877f, -1.984146f);

            MenuItem _item;
            _item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-396.8268f, -265.873f);

            EzText _t;
            _t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-26.18762f, 394.5072f); _t.Scale = 0.7095835f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(978.2115f, 727.7092f); }
            _q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

            MyPile.Pos = new Vector2(13.8877f, -1.984146f);
        }
    }
}