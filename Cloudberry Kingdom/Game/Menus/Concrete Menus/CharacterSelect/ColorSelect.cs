using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ListSelectPanel : CkBaseMenu
    {
        public MenuList MyList;
        CharacterSelect MyCharacterSelect;
        int ClrSelectIndex;

		protected override void SetItemProperties(MenuItem item)
		{
			item.MyText.Scale = item.MySelectedText.Scale = FontScale;
			item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

			item.SelectIconOffset = new Vector2(0, -160);
		}

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public void SetPos(Vector2 pos)
        {
            MyPile.Pos += pos;
            MyMenu.Pos += pos;
        }

        public void SetIndexViaAssociated(int index)
        {
            int CorrespondingIndex = MyList.MyList.FindIndex(item => (int)(item.MyObject) == index);
            if (CorrespondingIndex < 0) CorrespondingIndex = 0;
            MyList.SetIndex(CorrespondingIndex);
        }

        public int GetAssociatedIndex()
        {
            return (int)MyList.CurObj;
        }

        Localization.Words Header;
        int HoldIndex;

        public ListSelectPanel(int Control, Localization.Words Header, CharacterSelect Parent, int ClrSelectIndex)
            : base(false)
        {
            this.MyCharacterSelect = Parent;
            this.ClrSelectIndex = ClrSelectIndex;
            this.Control = Control;

            this.Header = Header;

            HoldIndex = MyCharacterSelect.ItemIndex[ClrSelectIndex];

            Constructor();
        }

        void OnSelect()
        {
            MyCharacterSelect.ItemIndex[ClrSelectIndex] = GetAssociatedIndex();
            MyCharacterSelect.Customize_UpdateColors();
        }

        public override void Constructor()
        {
            base.Constructor();

            SlideLength = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            MyMenu = new Menu(false);
            EnsureFancy();

            MyMenu.OnB = Cast.ToMenu(Back);
            MyMenu.OnA = Cast.ToMenu(Select);

            MyList = new MenuList();
            MyList.Name = "list";
            MyList.Center = true;
            MyList.OnIndexSelect = OnSelect;
            MyList.Go = Cast.ToItem(Select);
            MyMenu.Add(MyList);

            //var Done = new MenuItem(new EzText("Use", ItemFont));
            var Done = new MenuItem(new EzText(Localization.Words.Use, ItemFont));
            Done.Name = "Done";
            Done.Go = Cast.ToItem(Select);
            AddItem(Done);

            //var BackButton = new MenuItem(new EzText("{pBackArrow2,80,?}", ItemFont));
            //var BackButton = new MenuItem(new EzText("Cancel", ItemFont));
            var BackButton = new MenuItem(new EzText(Localization.Words.Cancel, ItemFont));
            BackButton.Name = "Cancel";
            BackButton.Go = Cast.ToItem(Back);
            AddItem(BackButton);

            var header = new EzText(Header, Resources.Font_Grobold42, true);
            MyPile.Add(header, "Header");

            CharacterSelect.Shift(this);
        }

        public override void OnAdd()
		{
			base.OnAdd();

			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-299.6664f, 48.88882f); _item.MyText.Scale = 0.5535835f; _item.MySelectedText.Scale = 0.5535835f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-294.1117f, -96.10889f); _item.MyText.Scale = 0.5352504f; _item.MySelectedText.Scale = 0.5352504f; _item.SelectIconOffset = new Vector2(0f, 0f);  }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = 1f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-324.6669f, 51.66659f); _item.MyText.Scale = 0.5425833f; _item.MySelectedText.Scale = 0.5425833f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-330.2227f, -90.55334f); _item.MyText.Scale = 0.4997506f; _item.MySelectedText.Scale = 0.4997506f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(7.142685f, 531.3489f); _t.Scale = 0.7875f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-324.6669f, 51.66659f); _item.MyText.Scale = 0.5425833f; _item.MySelectedText.Scale = 0.5425833f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-330.2227f, -90.55334f); _item.MyText.Scale = 0.4997506f; _item.MySelectedText.Scale = 0.4997506f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(7.142685f, 531.3489f); _t.Scale = 0.7875f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-324.6669f, 51.66659f); _item.MyText.Scale = 0.5425833f; _item.MySelectedText.Scale = 0.5425833f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-330.2227f, -90.55334f); _item.MyText.Scale = 0.4997506f; _item.MySelectedText.Scale = 0.4997506f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = .785f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-332.9999f, 46.11105f); _item.MyText.Scale = 0.5192502f; _item.MySelectedText.Scale = 0.5192502f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-330.2227f, -90.55334f); _item.MyText.Scale = 0.4997506f; _item.MySelectedText.Scale = 0.4997506f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = 1f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-246.8888f, 62.77774f); _item.MyText.Scale = 0.5192502f; _item.MySelectedText.Scale = 0.5192502f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-238.5559f, -84.9978f); _item.MyText.Scale = 0.5033337f; _item.MySelectedText.Scale = 0.5033337f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = 1f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-257.4442f, 51.66659f); _item.MyText.Scale = 0.5535835f; _item.MySelectedText.Scale = 0.5535835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-260.2225f, -82.21997f); _item.MyText.Scale = 0.5352504f; _item.MySelectedText.Scale = 0.5352504f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = .785f; }
				MyPile.Pos = new Vector2(MyPile.Pos.X, -492.0635f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-232.9996f, 73.88882f); _item.MyText.Scale = 0.5535835f; _item.MySelectedText.Scale = 0.5535835f; _item.SelectIconOffset = new Vector2(0f, 0f);  }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-224.6671f, -62.77551f); _item.MyText.Scale = 0.5352504f; _item.MySelectedText.Scale = 0.5352504f; _item.SelectIconOffset = new Vector2(0f, 0f);  }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = 1f; }
				MyPile.Pos = new Vector2(MyMenu.Pos.X, -492.0635f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(-2.77784f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-227.4442f, 51.66659f); _item.MyText.Scale = 0.5535835f; _item.MySelectedText.Scale = 0.5535835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-230.2225f, -82.21997f); _item.MyText.Scale = 0.5352504f; _item.MySelectedText.Scale = 0.5352504f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -484.127f);

				EzText _t;
				_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(9.920441f, 536.9045f); _t.Scale = 1f; }
				MyPile.Pos = new Vector2(MyPile.Pos.X, -492.0635f);
			}

			EzText __t;
			__t = MyPile.FindEzText("Header"); if (__t != null) { __t.Scale *= .875f; }
		}

        void Back()
        {
            MyCharacterSelect.ItemIndex[ClrSelectIndex] = HoldIndex;
            MyCharacterSelect.Customize_UpdateColors();

            //MyMenu.BackSound.Play();
            ReturnToCaller();
        }

        void Select()
        {
            //MyMenu.SelectSound.Play();

            // Save new custom color scheme
            MyCharacterSelect.Player.CustomColorScheme = MyCharacterSelect.Player.ColorScheme;
            MyCharacterSelect.Player.ColorSchemeIndex = -1;

            //MyMenu.SelectSound.Play();
            ReturnToCaller();
        }

        protected override void MyPhsxStep()
        {
 	        base.MyPhsxStep();

            if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }
        }
    }
}