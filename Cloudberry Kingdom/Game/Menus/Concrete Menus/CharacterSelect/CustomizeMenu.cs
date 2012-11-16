using Microsoft.Xna.Framework;
using System;

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

            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            MakeItems();

            EnsureFancy();

            CharacterSelect.Shift(this);

            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Color"); if (_item != null) { _item.SetPos = new Vector2(-208.3333f, 5.555542f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }
            _item = MyMenu.FindItemByName("Beard"); if (_item != null) { _item.SetPos = new Vector2(-205.5554f, -123.8889f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }
            _item = MyMenu.FindItemByName("Hat"); if (_item != null) { _item.SetPos = new Vector2(-141.6667f, -250.5555f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }
            _item = MyMenu.FindItemByName("Cape"); if (_item != null) { _item.SetPos = new Vector2(-188.8889f, -377.2222f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }
            _item = MyMenu.FindItemByName("Lining"); if (_item != null) { _item.SetPos = new Vector2(-191.6665f, -501.1112f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }
            _item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-177.7778f, -672.2223f); _item.MyText.Scale = 0.5835f; _item.MySelectedText.Scale = 0.5835f; _item.SelectIconOffset = new Vector2(0f, -160f); }

            MyMenu.Pos = new Vector2(-1320f, -22.22222f);

            MyPile.Pos = new Vector2(-1320f, 0f);
        }

        class GoProxy : Lambda_1<MenuItem>
        {
            CustomizeMenu cm;

            public GoProxy(CustomizeMenu cm)
            {
                this.cm = cm;
            }

            public void Apply(MenuItem item)
            {
                cm.Go(item);
            }
        }

        void Go(MenuItem item)
        {
            Call(new Waiting(Control, MyCharacterSelect));
            Hide();
        }

        //void AddMenuItem(string Text, string Name)
        void AddMenuItem(Localization.Words Word, string Name)
        {
            MenuItem item = new MenuItem(new EzText(Word, ItemFont));
            item.Name = Name;
            item.Go = Cast.ToItem(new CreateColorSelectProxy(this));
            
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
            back.Go = new GoProxy(this);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        class CreateColorSelectProxy : Lambda
        {
            CustomizeMenu cm;

            public CreateColorSelectProxy(CustomizeMenu cm)
            {
                this.cm = cm;
            }

            public void Apply()
            {
                cm.CreateColorSelect();
            }
        }

        public void CreateColorSelect()
        {
            ListSelectPanel ClrSelect;

            Vector2 ShiftSelect = Vector2.Zero;            

            // Make the hat select
            if (MyMenu.CurIndex == 2)
            {
                var list = new ListSelectPanel(Control, Localization.Words.Hat, MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = list;

                foreach (Hat hat in CharacterSelectManager.AvailableHats)
                {
                    int hat_index = ColorSchemeManager.HatInfo.IndexOf(hat);
                    var item = new MenuItem(new EzText(hat.Name, Resources.Font_Grobold42, false, true));
                    item.ScaleText(.375f);
                    item.MyObject = hat_index;

                    list.MyList.AddItem(item, hat_index);
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
                    item.ScaleText(.375f);
                    item.MyObject = beard_index;
                    
                    list.MyList.AddItem(item, beard_index);
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
                    if (!CloudberryKingdomGame.UnlockAll)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    int clr_index = list.IndexOf(item);
                    var _item = new MenuItem(new EzText(data.Name, Resources.Font_Grobold42, false, true));
                    _item.ScaleText(.375f);
                    _item.MyObject = clr_index;

                    select.MyList.AddItem(_item, clr_index);
                }
            }

            // Set the index of the list
            ClrSelect.SetIndexViaAssociated(MyCharacterSelect.ItemIndex[MyMenu.CurIndex]);

            Call(ClrSelect);
            Hide();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;
            MyCharacterSelect.Player.Exists = true;
        }
   }
}