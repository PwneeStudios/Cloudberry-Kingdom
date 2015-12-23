using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class VerifyStoryReset : VerifyBaseMenu
    {
        public VerifyStoryReset(bool CallBaseConstructor) : base(CallBaseConstructor)
        {
            EnableBounce();
        }
		public VerifyStoryReset(int Control)
			: base(Control, true)
        {
            //EnableBounce();
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MenuItem item;

            // Header
			string reset_story = string.Format("{2}\n{0} ({1})?", Localization.WordString(Localization.Words.Reset), Localization.WordString(Localization.Words.StoryMode), Localization.WordString(Localization.Words.ProgressWillBeLost));
			Text HeaderText = new Text(reset_story, ItemFont, 1600, true, false, .7f);
            SetHeaderProperties(HeaderText);
            HeaderText.Name = "Header";
            MyPile.Add(HeaderText);

            // No
            item = new MenuItem(new Text(Localization.Words.Cancel, ItemFont, true));
            item.Go = Cast.ToItem(ReturnToCaller);
            item.Name = "No";
            AddItem(item);

			// Yes
			item = new MenuItem(new Text(Localization.Words.Continue, ItemFont, true));
			item.Go = Reset;
			item.Name = "Yes";
			AddItem(item);

            MyMenu.OnX = MyMenu.OnB = MenuReturnToCaller;

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);

            SetPos();
        }

		void Reset(MenuItem item)
		{
			for (int i = 0; i < 4; i++)
			{
				PlayerManager.Players[i].CampaignLevel =
				PlayerManager.Players[i].CampaignIndex = 0;
			}

			// Show a success window
			var ok = new AlertBaseMenu(Control, Localization.Words.None, Localization.Words.Hooray);
			ok.OnOk = ok.ReturnToCaller;
			ok.PauseGame = false;
			Caller.Call(ok, 0);

			Hid = true;
			RegularSlideOut(PresetPos.Right, 0);
			CollectSelf();
		}

		public override void OnReturnTo()
		{
			base.OnReturnTo();

			ReturnToCaller(false);

			Hid = true;
			RegularSlideOut(PresetPos.Right, 0);
		}

        void SetPos()
        {
			MenuItem _item;
			_item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(0f, 349.8889f); _item.MyText.Scale = 0.5410002f; _item.MySelectedText.Scale = 0.5410002f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(0f, 127.6667f); _item.MyText.Scale = 0.5438334f; _item.MySelectedText.Scale = 0.5438334f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-2.777588f, -330.5555f);

			Text _t;
			_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(0f, 777.4443f); _t.Scale = 0.5221667f; }

			QuadClass _q;
			_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 316.6668f); _q.Size = new Vector2(1281.083f, 729.4391f); }

			MyPile.Pos = new Vector2(0f, -319.4444f);
		}
    }
}