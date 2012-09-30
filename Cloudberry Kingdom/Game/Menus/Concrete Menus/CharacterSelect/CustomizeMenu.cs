using Microsoft.Xna.Framework;
using System;
using Drawing;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class CustomizeMenu : StartMenuBase
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

            item.Go = null;
            
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

            MyMenu.OnB = MenuReturnToCaller;

            MakeItems();

            EnsureFancy();

            CharacterSelect.Shift(this);

            SetPos();
        }

        void SetPos()
        {
        }

        void Go(MenuItem item)
        {
            Call(new Waiting(Control, MyCharacterSelect));
            Hide();
        }

        private void MakeItems()
        {
            ItemPos = new Vector2(-200, 100);
            PosAdd = new Vector2(0, -160);
            SelectedItemShift = new Vector2(0, 0);
            FontScale = .5835f;
            ItemFont = Tools.Font_Grobold42;

            AddItem(new MenuItem(new EzText("Color", ItemFont)));
            //AddItem(new MenuItem(new EzText("Border", ItemFont)));

            AddItem(new MenuItem(new EzText("Beard", ItemFont)));
            AddItem(new MenuItem(new EzText("Hat", ItemFont)));

            AddItem(new MenuItem(new EzText("Cape", ItemFont)));
            AddItem(new MenuItem(new EzText("Lining", ItemFont)));

            MenuItem back = new MenuItem(new EzText("Done", ItemFont));
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

            // Make the hat select
            if (MyMenu.CurIndex == 2)
            {
                var list = new ListSelectPanel(Control, "Hat", MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = list;

                foreach (Hat hat in CharacterSelectManager.AvailableHats)
                {
                    int hat_index = ColorSchemeManager.HatInfo.IndexOf(hat);
                    var item = new MenuItem(new EzText(hat.Name, Tools.Font_Grobold42, false, true));
                    item.ScaleText(.375f);
                    item.MyObject = hat_index;

                    list.MyList.AddItem(item, hat_index);
                }
                ClrSelect.MyMenu.Pos = ClrSelect.MyPile.Pos = AmountShifted;
            }
            // Make the beard select
            else if (MyMenu.CurIndex == 1)
            {
                var list = new ListSelectPanel(Control, "Beard", MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = list;

                foreach (Hat beard in CharacterSelectManager.AvailableBeards)
                {
                    int beard_index = ColorSchemeManager.BeardInfo.IndexOf(beard);
                    var item = new MenuItem(new EzText(beard.Name, Tools.Font_Grobold42, false, true));
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
                var select = new ListSelectPanel(Control, "Color", MyCharacterSelect, MyMenu.CurIndex);
                ClrSelect = select;

                foreach (MenuListItem item in list)
                {
                    ClrTextFx data = (ClrTextFx)item.obj;

                    // Check if color is available
                    if (!CloudberryKingdomGame.UnlockAll)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    int clr_index = list.IndexOf(item);
                    var _item = new MenuItem(new EzText(data.Name, Tools.Font_Grobold42, false, true));
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