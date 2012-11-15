namespace CloudberryKingdom
{
    public class VerifyDeleteSeeds : VerifyBaseMenu
    {
        int NumSeeds;
        public VerifyDeleteSeeds(int Control, int NumSeeds) : base(false)
        {
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
            item.Go = new VerifyDeleteYesGoLambda(this);
            AddItem(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new EzText(Localization.Words.No, ItemFont, "No"));
            item.Go = new VerifyDeleteNoGoLambda(this);
            AddItem(item);
            item.SelectSound = null;

            MyMenu.OnX = MyMenu.OnB = new VerifyDeleteOnXLambda(this);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
        }

        class VerifyDeleteYesGoLambda : Lambda_1<MenuItem>
        {
            VerifyDeleteSeeds vds;
            public VerifyDeleteYesGoLambda(VerifyDeleteSeeds vds)
            {
                this.vds = vds;
            }

            public void Apply(MenuItem item)
            {
                vds.DoSelect(true);
                vds.ReturnToCaller();
            }
        }

        class VerifyDeleteOnXLambda : LambdaFunc_1<Menu, bool>
        {
            VerifyDeleteSeeds vds;
            public VerifyDeleteOnXLambda(VerifyDeleteSeeds vds)
            {
                this.vds = vds;
            }

            public bool Apply(Menu item)
            {
                vds.DoSelect(false);
                vds.ReturnToCaller();
                return true;
            }
        }

        class VerifyDeleteNoGoLambda : Lambda_1<MenuItem>
        {
            VerifyDeleteSeeds vds;
            public VerifyDeleteNoGoLambda(VerifyDeleteSeeds vds)
            {
                this.vds = vds;
            }

            public void Apply(MenuItem item)
            {
                vds.DoSelect(false);
                vds.ReturnToCaller();
            }
        }
    }
}