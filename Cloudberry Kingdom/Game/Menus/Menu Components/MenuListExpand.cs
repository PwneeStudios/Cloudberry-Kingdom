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
            base.ReleaseBody();

            MyMenuList.MyMenu.Active = true;
            MyMenuList.MyMenu.NoneSelected = true;
            MyMenuList.MyMenu.PhsxStep();
            MyMenuList.MyMenu.PhsxStep();

            if (MyMenuList.OnConfirmedIndexSelect != null)
                MyMenuList.OnConfirmedIndexSelect.Apply();
        }

        MenuList MyMenuList;
        public MenuListExpand(int Control, MenuList MyMenuList) : base(false)
        {
            this.MyMenuList = MyMenuList;
            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            //base.SetItemProperties(item);

            item.MyText.Scale = item.MySelectedText.Scale = FontScale;

            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();

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

        class OnSelectProxy : Lambda
        {
            MenuListExpand mle;

            public OnSelectProxy(MenuListExpand mle)
            {
                this.mle = mle;
            }

            public void Apply()
            {
                mle.OnSelect();
            }
        }

        void OnSelect()
        {
            //MyMenuList.SetIndex(MyMenu.CurIndex);
            MyMenuList.SetIndex(MyMenu.CurItem.MyInt);
        }

        class InitOnBMenuHelper : LambdaFunc_1<Menu, bool>
        {
            MenuListExpand mle;

            public InitOnBMenuHelper(MenuListExpand mle)
            {
                this.mle = mle;
            }

            public bool Apply(Menu menu)
            {
                mle.Back();
                return true;
            }
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

            MyMenu.OnB = new InitOnBMenuHelper(this);

            ItemPos = new Vector2(0, 0);
            PosAdd = new Vector2(0, -168 - 3);
            SelectedItemShift = new Vector2(0, 0);
            FontScale = .78f;

            float Width = 0;
            int index = 0;
            foreach (MenuItem item in MyMenuList.MyList)
            {
                index++;

                if (!item.Selectable) continue;

                MenuItem clone = item.Clone();
                clone.MyInt = index - 1;
                clone.AdditionalOnSelect = new OnSelectProxy(this);
                AddItem(clone);
                clone.ScaleText(.85f);
                Vector2 size = clone.MyText.GetWorldSize();
                //clone.Shift(-size / 2);
                Width = Math.Max(size.X, Width);

                if (MyMenuList.AdditionalExpandProcessing != null)
                    MyMenuList.AdditionalExpandProcessing.Apply(this, clone);
            }

            // Backdrop
            backdrop = new QuadClass("score_screen_grey", 482);
            //backdrop.Size = backdrop.Size * new Vector2(1f, 2.03f);
            MyMenu.CalcBounds();
            float Height = (MyMenu.TR.Y - MyMenu.BL.Y) / 2;
            backdrop.Size = new Vector2(Width / 2 + 88, Height + 70);
            DefaultBackdropSize = backdrop.Size;
            backdrop.Quad.RotateUV();
            MyPile.Add(backdrop);
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(Width/2, (MyMenu.TR.Y + MyMenu.BL.Y) / 2);

            EnsureFancy();

            MyPile.Pos = new Vector2(0, 0);
            MyMenu.FancyPos.RelVal = new Vector2(0, 0);
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