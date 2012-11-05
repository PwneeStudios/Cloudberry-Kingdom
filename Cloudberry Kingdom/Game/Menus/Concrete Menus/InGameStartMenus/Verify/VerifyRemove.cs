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
            EzText HeaderText = new EzText(Localization.Words.RemovePlayerQuestion, ItemFont, true);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;

            string PlayerName = PlayerManager.Get(Control).GetName();
            var PlayerText = new EzText(PlayerName, ItemFont, true);
            SetHeaderProperties(PlayerText);
            MyPile.Add(PlayerText);

            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont));
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
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont));
            item.Go = menuitem => ReturnToCaller();
            AddItem(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }
}