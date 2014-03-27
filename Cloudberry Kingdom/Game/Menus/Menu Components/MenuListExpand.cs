using Microsoft.Xna.Framework;
using System;
using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class MenuListExpand : CkBaseMenu
    {
        protected override void ReleaseBody()
        {
			bool IndexSelected = MyMenu != null && !MyMenu.NoneSelected;

            base.ReleaseBody();

            MyMenuList.MyMenu.Active = true;
            MyMenuList.MyMenu.NoneSelected = true;
            MyMenuList.MyMenu.PhsxStep();
            MyMenuList.MyMenu.PhsxStep();

			// If an item was selected
			if (IndexSelected)
			{
				// and it was different than the item already selected
				if (MyMenuList.ListIndex != HoldIndex)
				{
					// then run the confirmation call back
					if (MyMenuList.OnConfirmedIndexSelect != null)
					{
						MyMenuList.OnConfirmedIndexSelect();
					}
				}
			}
			else
			{
				// otherwise make sure we re-select what was originally selected
				MyMenuList.SetIndex(HoldIndex);
			}
        }

        MenuList MyMenuList;
		
		/// <summary>
		/// The original index that was selected in the menu list before this drop down was created.
		/// </summary>
		int HoldIndex;
        
		public MenuListExpand(int Control, MenuList MyMenuList) : base(false)
        {
            this.MyMenuList = MyMenuList;
			
			HoldIndex = MyMenuList.ListIndex;

            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            //base.SetItemProperties(item);

            item.MyText.Scale = item.MySelectedText.Scale = FontScale;

            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
			StartMenu.SetItemProperties_Red(item);

			item.MyText.MyFloatColor = new Color(235, 235, 235).ToVector4();
			item.MyText.OutlineColor = new Color(0, 0, 0).ToVector4();
			item.MySelectedText.MyFloatColor = new Color(210, 210, 210).ToVector4();
			item.MySelectedText.OutlineColor = new Color(0, 0, 0).ToVector4();


            item.Go = null;
            
#if PC_VERSION
                item.Padding += new Vector2(0, 35);
#endif

                //item.SelectIconOffset = new Vector2(0, -160);
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

#if PC_VERSION
            item.Padding.Y -= 37;
#endif
        }

        void OnSelect()
        {
            //MyMenuList.SetIndex(MyMenu.CurIndex);
            MyMenuList.SetIndex(MyMenu.CurItem.MyInt);
        }

        QuadClass backdrop;
        Vector2 DefaultBackdropSize;
        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.SlipSelect = true;
            MyMenu.Active = false;
            MyMenu.NoneSelected = true;

            Control = this.Control;

            MyMenu.OnB = null;

			MyMenu.OnB = Cast.ToMenu(Back);

			ItemPos = new Vector2(0, 0);
			PosAdd = new Vector2(0, -80 * MyMenuList.MyExpandParams.ScaleItems);
            SelectedItemShift = new Vector2(0, 0);
            FontScale = .78f;

            float Width = 0, Height = 0;
            int index = 0;
            foreach (MenuItem item in MyMenuList.MyList)
            {
				if (Height > 0) Height += PosAdd.Y;

                index++;

                if (!item.Selectable) continue;

                MenuItem clone;
				if (item.ExpandString == null)
					clone = item.Clone();
				else
					clone = item.Clone(item.ExpandString);
                
				clone.MyInt = index - 1;
                clone.AdditionalOnSelect = OnSelect;
                AddItem(clone);
				//clone.ScaleText(.5f);
				clone.ScaleText(.5f * MyMenuList.MyExpandParams.ScaleItems);
				clone.Padding.Y += 8;
                Vector2 size = clone.MyText.GetWorldSize();

                Width = Math.Max(size.X, Width);
				Height += size.Y;

                if (MyMenuList.AdditionalExpandProcessing != null)
                    MyMenuList.AdditionalExpandProcessing(this, clone);
            }

            // Backdrop
            backdrop = new QuadClass("White", 50);
			backdrop.Quad.SetColor(ColorHelper.Gray(.2f));
            backdrop.Alpha = .8f;

            MyMenu.CalcBounds();
			backdrop.Size = new Vector2(Width / 2 + 21, Height / 2 + 7) + MyMenuList.MyExpandParams.SizePadding;
            DefaultBackdropSize = backdrop.Size + MyMenuList.MyExpandParams.SizePadding;
			MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(Width/2, (MyMenu.TR.Y + MyMenu.BL.Y) / 2);

            EnsureFancy();

            MyPile.Pos = new Vector2(0, 0);
            MyMenu.FancyPos.RelVal = new Vector2(0, 0) + MyMenuList.MyExpandParams.ShiftTopLeftItem;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideIn(0);
        }

        void Back()
        {
            if (Active)
            {
                SlideOutLength = 0;
                ReturnToCaller();
            }
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        int Count = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
            if (!Active) return;

            Count++;
            if (Count > 4) MyMenu.Active = true;
            else { MyMenu.NoneSelected = true; MyMenu.Active = false; }

#if PC_VERSION
            if (!Tools.CurMouseDown())
                Back();
#endif
        }
    }
}