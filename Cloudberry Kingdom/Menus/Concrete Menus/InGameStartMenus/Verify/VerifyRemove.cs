namespace CloudberryKingdom
{
    public class VerifyRemoveMenu : VerifyBaseMenu
    {
        public VerifyRemoveMenu(int Control) : base(Control) { }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            string Text = "Remove " + PlayerManager.Get(Control).GetName() + "?";
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
                if (PlayerManager.GetNumPlayers() > 1)
                {
                    if (Control >= 0)
                        Tools.CurGameData.RemovePlayer(Control);
                }

                ReturnToCaller();
            };
            AddItem(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new EzText("No", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = menuitem => ReturnToCaller();
            AddItem(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }
}