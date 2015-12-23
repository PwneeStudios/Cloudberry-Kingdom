using Microsoft.Xna.Framework;

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
            Text HeaderText = new Text(Text, ItemFont, true, true);
            SetHeaderProperties(HeaderText);
			MyPile.Add(HeaderText, "Header");
            HeaderText.Pos = HeaderPos;


            // Yes
            item = new MenuItem(new Text(Localization.Words.Yes, ItemFont, "Yes"));
            item.Go = _item =>
            {
                DoSelect(true);
                ReturnToCaller();
            };
            AddItem(item);
            item.SelectSound = null;

            // No
            item = new MenuItem(new Text(Localization.Words.No, ItemFont, "No"));
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

			SetPos();
		}

		void SetPos()
		{
            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

			Text _t;
			_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(0, 350); _t.Scale = 0.96f; }

			QuadClass _q;
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1500f, 902.2556f); }

			MyPile.Pos = new Vector2(0f, 0f);
		}
    }
}