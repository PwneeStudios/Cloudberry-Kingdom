using Microsoft.Xna.Framework;
using System;
using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CustomizeMenu : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public CustomizeMenu(int Control, CharacterSelect Parent) : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = Parent;

            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            item.MyText.Scale = item.MySelectedText.Scale = FontScale;
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

            item.SelectIconOffset = new Vector2(0, -160);
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Make the menu
            MyMenu = new Menu(false);
			MyMenu.UseMouseAndKeyboard = false;

            MyMenu.OnB = MenuReturnToCaller;

            MakeItems();

            EnsureFancy();
            MyMenu.Control = Control;

            CharacterSelect.Shift(this);

            SetPos();
        }

        void SetPos()
        {
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, 22.22222f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, 5.555542f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, -121.1112f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, -247.7779f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, -374.4446f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, -501.1113f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-211.1111f, -627.778f); _item.MyText.Scale = 0.48325f; _item.MySelectedText.Scale = 0.48325f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, 5.555559f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -2.777761f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -2.777761f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.4379163f; _item.MySelectedText.Scale = 0.4379163f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X - 65, -2.777761f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.4920831f; _item.MySelectedText.Scale = 0.4920831f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X - 80, 0);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-219.4443f, 8.333324f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -5.555534f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-219.4443f, 8.333324f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyPile.Pos = new Vector2(MyMenu.Pos.X, -5.555534f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-219.4443f, 8.333324f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -123.8889f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -250.5555f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -377.2222f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -501.1112f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, -672.2223f); _item.MyText.Scale = 0.5408335f; _item.MySelectedText.Scale = 0.5408335f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -5.555534f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, 22.22221f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, -115.5556f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, -253.3334f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, -391.1112f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, -528.889f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }
				_item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-213.889f, -666.6669f); _item.MyText.Scale = 0.5825001f; _item.MySelectedText.Scale = 0.5825001f; _item.SelectIconOffset = new Vector2(0f, -160f); }

				MyMenu.Pos = new Vector2(MyMenu.Pos.X, -5);
			}
        }

        void Go(MenuItem item)
        {
            Call(new Waiting(Control, MyCharacterSelect, true));
            Hide();
        }

        void AddMenuItem(Localization.Words Word, string Name)
        {
            MenuItem item = new MenuItem(new EzText(Word, ItemFont));
            item.Name = Name;
            item.Go = Cast.ToItem(CreateColorSelect);
            
            AddItem(item);
        }

        private void MakeItems()
        {
            ItemPos = new Vector2(-200, 100);
            PosAdd = new Vector2(0, -160);
            SelectedItemShift = new Vector2(0, 0);
            FontScale = .5835f;
            ItemFont = Resources.Font_Grobold42;

            AddMenuItem(Localization.Words.Color, "Color");
            AddMenuItem(Localization.Words.Beard, "Beard");
            AddMenuItem(Localization.Words.Hat, "Hat");
            AddMenuItem(Localization.Words.Cape, "Cape");
            AddMenuItem(Localization.Words.Lining, "Lining");

            MenuItem back = new MenuItem(new EzText(Localization.Words.Done, ItemFont), "Done");
            AddItem(back);
            back.Go = Go;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        public void CreateColorSelect()
        {
            ListSelectPanel ClrSelect;

            Vector2 ShiftSelect = Vector2.Zero;

			bool custom_arrows = false;
			float item_width = .375f;
            if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
            {
                item_width = .305f;
            }
            else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
            {
                item_width = .305f;
            }
            else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
            {
                if (MyMenu.CurIndex == 2)
                    item_width = .29f;
                else
                    item_width = .32f;
                custom_arrows = true;
            }
            else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
            {
                item_width = .34f;
            }

            // Make the hat select
            if (MyMenu.CurIndex == 2)
            {
                var list = new ListSelectPanel(Control, Localization.Words.Hat, MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = list;

                foreach (Hat hat in CharacterSelectManager.AvailableHats)
                {
                    int hat_index = ColorSchemeManager.HatInfo.IndexOf(hat);
                    var item = new MenuItem(new EzText(hat.Name, Resources.Font_Grobold42, false, true));
					item.ScaleText(item_width);
                    item.MyObject = hat_index;

                    list.AddItem(item, hat_index);
                }
                ClrSelect.MyMenu.Pos = ClrSelect.MyPile.Pos = AmountShifted;
            }
            // Make the beard select
            else if (MyMenu.CurIndex == 1)
            {
                var list = new ListSelectPanel(Control, Localization.Words.Beard, MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = list;

                foreach (Hat beard in CharacterSelectManager.AvailableBeards)
                {
                    int beard_index = ColorSchemeManager.BeardInfo.IndexOf(beard);
                    var item = new MenuItem(new EzText(beard.Name, Resources.Font_Grobold42, false, true));
					item.ScaleText(item_width);
                    item.MyObject = beard_index;
                    
                    list.AddItem(item, beard_index);
                }
                ClrSelect.MyMenu.Pos = ClrSelect.MyPile.Pos = AmountShifted;
            }
            // Make the color select
            else
            {
                var list = MyCharacterSelect.ItemList[MyMenu.CurIndex];
                var select = new ListSelectPanel(Control, Localization.Words.Color, MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = select;

                foreach (MenuListItem item in list)
                {
                    ClrTextFx data = (ClrTextFx)item.obj;

                    // Check if color is available
                    if (!CloudberryKingdomGame.Unlock_Customization)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    int clr_index = list.IndexOf(item);
                    var _item = new MenuItem(new EzText(data.Name, Resources.Font_Grobold42, false, true));
					_item.ScaleText(item_width);
                    _item.MyObject = clr_index;

                    select.AddItem(_item, clr_index);
                }
            }

			if (custom_arrows && ClrSelect != null)
			{
				ClrSelect.MyList.CustomArrow = custom_arrows;
				ClrSelect.MyList.RightArrowOffset = Menu.DefaultMenuInfo.MenuRightArrow_Selected_Offset * .7f;
				ClrSelect.MyList.RightArrowOffset.Y = 0;
				ClrSelect.MyList.LeftArrowOffset = Menu.DefaultMenuInfo.MenuLeftArrow_Selected_Offset * .7f;
				ClrSelect.MyList.LeftArrowOffset.Y = 0;
			}

            // Set the index of the list
            ClrSelect.SetIndexViaAssociated(MyCharacterSelect.ItemIndex[MyMenu.CurIndex]);

            Call(ClrSelect);
            ClrSelect.Control = ClrSelect.MyMenu.Control = Control;
            //CharacterSelect.Shift(ClrSelect);

            Hide();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            if (MyCharacterSelect.MyState != CharacterSelect.SelectState.Selecting)
            {
                MyCharacterSelect.Player.Exists = true;
            }

            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;


			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }
        }
   }
}