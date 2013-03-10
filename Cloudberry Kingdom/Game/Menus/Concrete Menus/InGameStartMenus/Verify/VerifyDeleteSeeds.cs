namespace CloudberryKingdom
{
    public class VerifyDeleteSeeds : VerifyBaseMenu
    {
        int NumSeeds;
        public VerifyDeleteSeeds(int Control, int NumSeeds, bool DoEnableBounce) : base(false)
        {
			if (DoEnableBounce)
			{
				EnableBounce();
			}

            this.NumSeeds = NumSeeds;
            this.Control = Control;

            Constructor();
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
            string Text;
            if (NumSeeds == 1)
                Text = string.Format(Localization.WordString(Localization.Words.DeleteSeeds), NumSeeds);
            else
                Text = string.Format(Localization.WordString(Localization.Words.DeleteSeedsPlural), NumSeeds);
            EzText HeaderText = new EzText(Text, ItemFont, "Header");
            SetHeaderProperties(HeaderText);
            MyPile.Add(HeaderText);
            HeaderText.Pos = HeaderPos;


            // Yes
            item = new MenuItem(new EzText(Localization.Words.Yes, ItemFont, "Yes"));
            item.Go = _item =>
            {
                DoSelect(true);
                ReturnToCaller();
            };
            AddItem(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont, "No"));
            item.Go = _item =>
            {
                DoSelect(false);
                ReturnToCaller();
            };
            AddItem(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = _menu =>
            {
                DoSelect(false);
                ReturnToCaller();
                return true;
            };

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }
    }
}