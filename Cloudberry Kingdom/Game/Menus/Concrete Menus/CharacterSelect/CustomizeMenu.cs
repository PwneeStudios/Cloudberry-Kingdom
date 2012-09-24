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
            //base.SetItemProperties(item);

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
            if (CurClrSelect != null)
                CurClrSelect.ReturnToCaller();

            Call(new Waiting(Control, MyCharacterSelect));
            Hide();
        }

        private void MakeItems()
        {
            ItemPos = new Vector2(-200, 100);
            PosAdd = new Vector2(0, -160);
            SelectedItemShift = new Vector2(0, 0);
            FontScale = .635f;
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

        /// <summary>
        /// The current color selection GameObject, if there is one.
        /// Set to null once the selection object starts sliding out.
        /// </summary>
        public BaseColorSelect CurClrSelect;

        /// <summary>
        /// The custom menu index associated with the current color selection box
        /// </summary>
        int ClrSelectIndex;

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        public void CreateColorSelect()
        {
            return;

            BaseColorSelect ClrSelect;

            Vector2 ShiftSelect = Vector2.Zero;            

            // Make the hat select
            if (MyMenu.CurIndex == 2)
            {
                var list = new ListSelectPanel(Control, "Hat");
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
                var list = new ListSelectPanel(Control, "Beard");
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
                var select = new ListSelectPanel(Control, "Color");
                ClrSelect = select;

                foreach (MenuListItem item in list)
                {
                    ClrTextFx data = (ClrTextFx)item.obj;

                    // Check if color is available
                    if (!CloudberryKingdom.UnlockAll)
                        if (data.Price > 0 && !PlayerManager.Bought(data)) continue;

                    int clr_index = list.IndexOf(item);
                    var _item = new MenuItem(new EzText(data.Name, Tools.Font_Grobold42, false, true));
                    _item.ScaleText(.375f);
                    _item.MyObject = clr_index;

                    select.MyList.AddItem(_item, clr_index);
                }
            }

            // Set the position
            if (ClrSelect is ColorSelectPanel)
            {
                ((ColorSelectPanel)ClrSelect).Grid.Pos = AmountShifted + new Vector2(-20f, -480f);
            }

            // Set the index of the list
            if (ClrSelect is BaseColorSelect)
            {
                ClrSelect.SetIndexViaAssociated(MyCharacterSelect.ItemIndex[MyMenu.CurIndex]);
            }

            Call(ClrSelect);
            Hide();

            CurClrSelect = ClrSelect;
            ClrSelectIndex = MyMenu.CurIndex;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
            return;

            if (CurClrSelect == null)
                base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;
            MyCharacterSelect.Player.Exists = true;

            // When set to true the menu phsx will be skipped
#if PC_VERSION
            //bool SkipMenuPhsx = false;
            //if (CurClrSelect != null && (!Tools.TheGame.MouseInUse || CurClrSelect.Grid.MouseInBox))
            //    SkipMenuPhsx = true;
            bool SkipMenuPhsx = (CurClrSelect != null);
#else
            bool SkipMenuPhsx = (CurClrSelect != null);
#endif


            // When the color select is up
            if (CurClrSelect != null)
            {
                // Check if the user is done selecting a color
                if (CurClrSelect.Done())
                {
                    // If the user canceled
                    if (CurClrSelect.ExitState() == GuiExitState.Cancel)
                    {
                        MyCharacterSelect.ItemIndex[ClrSelectIndex] = MyCharacterSelect.HoldIndex;
                        MyMenu.BackSound.Play();
                    }
                    // If the user selected something
                    else
                    {
                        MyMenu.SelectSound.Play();

                        // Save new custom color scheme
                        MyCharacterSelect.Player.CustomColorScheme = MyCharacterSelect.Player.ColorScheme;
                        MyCharacterSelect.Player.ColorSchemeIndex = -1;
                    }

                    // Slide out the color select
                    CurClrSelect.ReturnToCaller();
                    CurClrSelect = null;

                    MyMenu.Show = true;
                    SkipMenuPhsx = true;
                    SkipPhsx = 2;

                    // Burn one phsx step of the menu
                    bool Hold = MyMenu.CheckForOutsideClick;
                    MyMenu.CheckForOutsideClick = false;
                    MyMenu.PhsxStep();
                    MyMenu.PhsxStep();
                    MyMenu.PhsxStep();
                    MyMenu.CheckForOutsideClick = Hold;
                    MyMenu.SkipPhsx = true;
                }
                else
                {
#if PC_VERSION
                    //// If the mouse is in the box update the color/texture index
                    //if (CurClrSelect.Grid.MouseInBox)                                            
                    //    Parent.ItemIndex[ClrSelectIndex] = CurClrSelect.Grid.GetAssociatedIndex();
                    //// Otherwise revert to last selected index
                    //else
                    //    Parent.ItemIndex[ClrSelectIndex] = Parent.HoldIndex;
                    MyCharacterSelect.ItemIndex[ClrSelectIndex] = CurClrSelect.GetAssociatedIndex();
#else
                    // Update the color/texture index
                    MyCharacterSelect.ItemIndex[ClrSelectIndex] = CurClrSelect.GetAssociatedIndex();
#endif
                }

                MyCharacterSelect.Customize_UpdateColors();
            }

            if (!SkipMenuPhsx)
            {
                // Check to see if we should bring a new color select up
                if (CurClrSelect == null)
                if (ButtonCheck.State(ControllerButtons.X, Control).Pressed ||
                    ButtonCheck.State(ControllerButtons.A, Control).Pressed)
                {
                    if (MyMenu.CurIndex < MyCharacterSelect.ItemIndex.Length && !MyMenu.NoneSelected
#if PC_VERSION
                        // Skip if the color select is up but the mouse isn't in use
                        && !(!Tools.TheGame.MouseInUse && CurClrSelect != null))
#else
                        )
#endif
                    {
                        MyCharacterSelect.HoldIndex = MyCharacterSelect.ItemIndex[MyMenu.CurIndex];

                        MyMenu.LastActivatedItem = MyMenu.CurIndex;

                        if (CurClrSelect != null)
                            CurClrSelect.ReturnToCaller();
                        CreateColorSelect();
                        MyMenu.SelectSound.Play();
                    }
                }
            }
        }
   }
}