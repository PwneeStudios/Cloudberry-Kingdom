using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyQuitGameMenu2 : VerifyBaseMenu
    {
        public VerifyQuitGameMenu2(int Control) : base(Control) { }

        //public static int Version = 0;
        public static int Version = 1;

        QuadClass Berry;
        public override void MakeBackdrop()
        {
            if (Version == 0)
            {
                QuadClass backdrop = new QuadClass(null, true, false);
                backdrop.TextureName = "WoodMenu_5";
                backdrop.ScaleYToMatchRatio(1000);
                MyPile.Add(backdrop);
                backdrop.Pos =
                    new Vector2(99.20645f, 19.84137f);
                backdrop.Size =
                    new Vector2(1222.22f, 1134.69f);

                Berry = new QuadClass(null, true, false);
                Berry.TextureName = "cb_crying";
                Berry.ScaleYToMatchRatio(1000);
                MyPile.Add(Berry);
                Berry.Pos =
                    new Vector2(-202.3781f, -123.0159f);
                Berry.Size =
                    new Vector2(694.4494f, 892.3699f);
            }
            else
            {
                QuadClass Cloudback = new QuadClass("menupic_bg_cloud", 1200, true);
                Cloudback.Pos = new Vector2(99.20645f, 19.84137f);
                Cloudback.Size = new Vector2(1303.173f, 1019.564f);
                MyPile.Add(Cloudback);

                Berry = new QuadClass(null, true, false);
                Berry.TextureName = "cb_crying";
                Berry.ScaleYToMatchRatio(1000);
                MyPile.Add(Berry);
                Berry.Pos =
                    new Vector2(-107.14f, -99.20639f);
                Berry.Size =
                    new Vector2(519.8459f, 678.0841f);
            }
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.Scale *= 1.15f;
            item.MySelectedText.Scale *= 1.3f;

            GreenItem(item);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
            text.Scale *= 1.15f;
        }

        public override void Init()
        {
            base.Init();
            
            SlideInFrom = SlideOutTo = PresetPos.Bottom;
            DestinationScale = new Vector2(1.223f);

            // Make the menu
            MenuItem item;

            // Header
            string Text = "Exit Game?";
            EzText HeaderText = new EzText(Text, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            if (Version == 0)
                HeaderText.Pos = new Vector2(-915.4741f, 967.5232f);
            if (Version == 1)
                HeaderText.Pos = new Vector2(-701.1883f, 816.7295f);

            // Yes
            item = new MenuItem(new EzText("Yes", ItemFont));
            item.Go = _item =>
            {
                Tools.TheGame.Exit();
            };
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_crying";
            AddItem(item);

            // No
            item = new MenuItem(new EzText("No", ItemFont));
            item.Go = menuitem => ReturnToCaller();
            item.AdditionalOnSelect = () => Berry.TextureName = "cb_enthusiastic";
            item.SelectSound = null;
            BackSound = null;
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            SetPosition();
        }

        void SetPosition()
        {
            MyPile.Pos = new Vector2(13.8877f, -1.984146f);
            EnsureFancy();

            if (Version == 0)
                MyMenu.Pos = new Vector2(-301.5887f, -83.3335f);
            else
                MyMenu.Pos = new Vector2(-396.8268f, -265.873f);
        }
    }

    public class VerifyQuitGameMenu : VerifyBaseMenu
    {
        public VerifyQuitGameMenu(int Control) : base(Control) { }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            string Text = "Exit Game?";
            EzText HeaderText = new EzText(Text, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;


            // Yes
            item = new MenuItem(new EzText("Yes", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = _item =>
                {
                    Tools.TheGame.Exit();
                };
            AddItem(item);

            // No
            item = new MenuItem(new EzText("No", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = menuitem => ReturnToCaller();
            item.SelectSound = null;
            BackSound = null;
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }
}