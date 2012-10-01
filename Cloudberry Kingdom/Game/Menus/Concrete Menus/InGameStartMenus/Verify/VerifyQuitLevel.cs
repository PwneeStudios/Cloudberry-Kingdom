namespace CloudberryKingdom
{
    public class VerifyQuitLevelMenu : VerifyBaseMenu
    {
        public VerifyQuitLevelMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { }
        public VerifyQuitLevelMenu(int Control) : base(Control) { }

        public string VerifyString = "Exit level?";
        public VerifyQuitLevelMenu(int Control, string VerifyString)
            : base(false)
        {
            this.VerifyString = VerifyString;
            this.Control = Control;

            Constructor();
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            EzText HeaderText = new EzText(VerifyString, ItemFont);
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;


            // k
            item = new MenuItem(new EzText("Yes", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = _item =>
                {
                    Tools.CurrentAftermath = new AftermathData();
                    Tools.CurrentAftermath.Success = false;
                    Tools.CurrentAftermath.EarlyExit = true;

                    Tools.CurGameData.EndGame(false);
                };
            AddItem(item);

            // No
            item = new MenuItem(new EzText("No", ItemFont));
            //item.SetIcon(ObjectIcon.RobotIcon.Clone());
            //item.Icon.Pos = IconOffset;
            item.Go = menuitem => ReturnToCaller();
            AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }
}