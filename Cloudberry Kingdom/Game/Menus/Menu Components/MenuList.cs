using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class MenuList : MenuItem
    {
        public override string[] GetViewables()
        {
            return new string[] { "RightArrowOffset", "LeftArrowOffset", "Pos", "SelectedPos", "!MyMenu" };
        }

        public bool ExpandOnGo
        {
            get { return _ExpandOnGo; }
            set
            {
                _ExpandOnGo = value;
                if (_ExpandOnGo)
                    OnClick = new ExpandProxy1(this);
                else
                    OnClick = null;
            }
        }
        bool _ExpandOnGo = false;

        class ExpandProxy1 : Lambda_1<MenuItem>
        {
            MenuList ml;

            public ExpandProxy1(MenuList ml)
            {
                this.ml = ml;
            }

            public void Apply(MenuItem dummy)
            {
                ml.Expand();
            }
        }

        public MenuListExpand MyMenuListExpand;
        public Vector2 MyExpandPos = Vector2.Zero;
        public Lambda_2<MenuListExpand,MenuItem> AdditionalExpandProcessing;
        public void Expand()
        {
            if (MyMenuListExpand == null || MyMenuListExpand.Core.Released)
            {
                MyMenuListExpand = new MenuListExpand(this.Control, this);
                Tools.CurGameData.AddGameObject(MyMenuListExpand);
                MyMenu.Active = false;

                float Width = MyText.GetWorldWidth() / 2;
                if (MyExpandPos == Vector2.Zero)
                    MyMenuListExpand.Pos.RelVal = Pos + PosOffset - MyMenu.FancyPos.AbsVal + new Vector2(-Width, 120);
                else
                    MyMenuListExpand.Pos.RelVal = MyExpandPos;
            }
        }

        public List<MenuItem> MyList;
        public int ListIndex;

        const int SelectDelay = 12;
        int DelayCount;

        public QuadClass RightArrow, LeftArrow;
        public QuadClass RightArrow_Selected, LeftArrow_Selected;

        public bool CustomArrow;
        public Vector2 LeftArrowOffset, RightArrowOffset;
        //public Vector2 LeftArrow_SelectedOffset, RightArrow_SelectedOffset;

        public Lambda OnIndexSelect, OnConfirmedIndexSelect;

        /// <summary>
        /// Whether to draw the list's arrows when the list isn't currently selected
        /// </summary>
        public bool DrawArrowsWhenUnselected = false;

#if WINDOWS
        //Vector2 ListPadding = new Vector2(65, 0);
        Vector2 ListPadding = new Vector2(65, 0);
        Vector2 TotalPadding = Vector2.Zero;
        public override bool HitTest(Vector2 pos, Vector2 padding)
        {
            //TotalPadding = padding + Padding + ListPadding;
            TotalPadding = Padding + ListPadding;

            //return base.HitTest(pos, TotalPadding) || LeftArrow.HitTest(pos, TotalPadding) || RightArrow.HitTest(pos, TotalPadding);
            return base.HitTest(pos, padding) || 
                (HoldSelected &&
                 (LeftArrow_Selected.HitTest(pos, TotalPadding) || RightArrow_Selected.HitTest(pos, TotalPadding)));

            /*
            if (HoldSelected)
                return base.HitTest(pos, TotalPadding) || LeftArrow.HitTest(pos, TotalPadding) || RightArrow.HitTest(pos, TotalPadding);
            else
                return base.HitTest(pos, TotalPadding) || LeftArrow_Selected.HitTest(pos, TotalPadding) || RightArrow_Selected.HitTest(pos, TotalPadding);
             */
        }
#endif

        public override void FadeIn()
        {
            base.FadeIn();

            LeftArrow.Quad.SetColor(new Color(1, 1, 1, 1f));
            RightArrow.Quad.SetColor(new Color(1, 1, 1, 1f));
        }

        public override void FadeOut()
        {
            base.FadeOut();

            LeftArrow.Quad.SetColor(new Color(1, 1, 1, .3f));
            RightArrow.Quad.SetColor(new Color(1, 1, 1, .3f));
        }

        public MenuList()
        {
            MyList = new List<MenuItem>();

            base.Init(null, null);

            OverrideA = false;

#if PC_VERSION
            Padding.Y = 7;
            ExpandOnGo = true;
#endif

            InitializeArrows();
        }

        void InitializeArrows()
        {
            Vector2 Size;

            RightArrow = new QuadClass();
            RightArrow.Quad.MyTexture = Menu.DefaultMenuInfo.MenuRightArrow_Texture;
            Size = Menu.DefaultMenuInfo.MenuArrow_Size;
            RightArrow.Base.e1 *= Size.X;
            RightArrow.Base.e2 *= Size.Y;

            LeftArrow = new QuadClass();
            LeftArrow.Quad.MyTexture = Menu.DefaultMenuInfo.MenuLeftArrow_Texture;
            Size = Menu.DefaultMenuInfo.MenuArrow_Size;
            LeftArrow.Base.e1 *= Size.X;
            LeftArrow.Base.e2 *= Size.Y;

            RightArrow_Selected = new QuadClass();
            RightArrow_Selected.Quad.MyTexture = Menu.DefaultMenuInfo.MenuRightArrow_Selected_Texture;
            Size = Menu.DefaultMenuInfo.MenuArrow_Selected_Size;
            RightArrow_Selected.Base.e1 *= Size.X;
            RightArrow_Selected.Base.e2 *= Size.Y;

            LeftArrow_Selected = new QuadClass();
            LeftArrow_Selected.Quad.MyTexture = Menu.DefaultMenuInfo.MenuLeftArrow_Selected_Texture;
            Size = Menu.DefaultMenuInfo.MenuArrow_Selected_Size;
            LeftArrow_Selected.Base.e1 *= Size.X;
            LeftArrow_Selected.Base.e2 *= Size.Y;
        }

        public override float Height()
        {
            return MyText.GetWorldHeight();  
        }

        public override float Width()
        {
            return 0;
        }

        Dictionary<MenuItem, object> ObjDict = new Dictionary<MenuItem, object>();
        public void AddItem(MenuItem item, object obj)
        {
            MyList.Add(item);
            ObjDict.Add(item, obj);
        }

        public MenuItem GetListItem()
        {
            return MyList[ListIndex];
        }

        /// <summary>
        /// The object associated with the currently selected MenuItem
        /// </summary>
        public object CurObj { get { return ObjDict[CurMenuItem]; } }

        /// <summary>
        /// When true the list's index will wrap if too large or too small.
        /// Otherwise it will be restricted.
        /// </summary>
        public bool DoIndexWrapping = true;

        public bool OnFirstIndex { get { return ListIndex == 0 && !DoIndexWrapping; } }
        public bool OnLastIndex { get { return ListIndex == MyList.Count - 1 && !DoIndexWrapping; } }

        public bool ValidIndex(int index)
        {
            return MyList[index].Selectable;
        }

        public MenuItem CurMenuItem;
        public void SetSelectedItem(MenuItem item)
        {
            SetIndex(MyList.IndexOf(item));
        }

        public virtual void SetIndex(int NewIndex)
        {
            if (DoIndexWrapping)
            {
                if (NewIndex < 0) NewIndex = MyList.Count - 1;
                if (NewIndex >= MyList.Count) NewIndex = 0;
            }
            else
            {
                CoreMath.Restrict(0, MyList.Count - 1, ref NewIndex);
            }

            int HoldIndex = ListIndex;
            ListIndex = NewIndex;

            // Check new index is valid
            if (!ValidIndex(ListIndex))
            {
                //if (HoldIndex == ListIndex) return;
                if (LastIncrDir == 0) return;
                if (OnLastIndex || OnFirstIndex)
                    ListIndex = HoldIndex;
                else if (LastIncrDir != 0)
                    SetIndex(LastIncrDir + ListIndex);
                return;
            }
            LastIncrDir = 0;

            CurMenuItem = MyList[ListIndex];
            CurMenuItem.MyOscillateParams.Reset();

            MyText = CurMenuItem.MyText;
            MySelectedText = CurMenuItem.MySelectedText;

            if (OnIndexSelect != null)
                OnIndexSelect.Apply();

            if (MyMenuListExpand == null && OnConfirmedIndexSelect != null)
                OnConfirmedIndexSelect.Apply();

            CurMenuItem.OnSelect();
        }

        int LastIncrDir = 0;
        void IncrementIndex(int Increment)
        {
            if (Increment > 0)
            {
                LastIncrDir = 1;
                ListIndex++;
            }
            else
            {
                LastIncrDir = -1;
                ListIndex--;
            }

            SetIndex(ListIndex);

            DelayCount = SelectDelay;
        }

#if WINDOWS
        public QuadClass GetSelectedArrow()
        {
            if (!HoldSelected) return null;

            Vector2 MousePos = Tools.MouseGUIPos(MyCameraZoom);

            float Center = (RightArrow_Selected.TR.X + LeftArrow_Selected.BL.X) / 2;
            if (RightArrow_Selected.HitTest(MousePos, TotalPadding))
            //if (MousePos.X > RightArrow_Selected.BL.X - 200)
                return RightArrow_Selected;
            if (LeftArrow_Selected.HitTest(MousePos, TotalPadding))
            //else if (MousePos.X < LeftArrow_Selected.TR.X + 200)
                return LeftArrow_Selected;
            else
                return null;
        }
#endif

        /// <summary>
        /// When true clicking on the menu list will selected the next item in the list.
        /// </summary>
        public bool ClickForNextItem = true;

        bool HoldSelected;
        public override void PhsxStep(bool Selected)
        {
            base.PhsxStep(Selected);

            HoldSelected = Selected;
            int CurIndex = ListIndex;

            Vector2 Dir = Vector2.Zero;
            if (Selected)
            {
                Dir = Vector2.Zero;
                if (Control < 0)
                    Dir = ButtonCheck.GetMaxDir(Control);
                else
                    Dir = ButtonCheck.GetDir(Control);
            }

            if (DelayCount > 0)
            {
                DelayCount--;

                float Sensitivity = ButtonCheck.ThresholdSensitivity;
                if (Math.Abs(Dir.X) < Sensitivity / 2)
                    DelayCount -= 3;
            }
            else
            {
                if (Selected)
                {
#if WINDOWS
                    if (ButtonCheck.MouseInUse && ClickForNextItem)
                        if (ButtonCheck.State(ControllerButtons.A, Control).Pressed &&
                            !ButtonCheck.KeyboardGo())
                        {
                            QuadClass SelectedArrow = GetSelectedArrow();
                            if (SelectedArrow == RightArrow_Selected)
                                IncrementIndex(1);
                            else if (SelectedArrow == LeftArrow_Selected)
                                IncrementIndex(-1);
                            else
                                IncrementIndex(1);
                        }
#endif
                    
                    float Sensitivity = ButtonCheck.ThresholdSensitivity;
                    if (Math.Abs(Dir.X) > Sensitivity)
                    {
                        IncrementIndex(Math.Sign(Dir.X));
                    }
                }
            }

            if (CurIndex != ListIndex)
                if (ListScrollSound != null)
                    ListScrollSound.Play();
        }

        public bool Center = true;
        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            if (MyMenu.CurDrawLayer != MyDrawLayer || !Show || (MyMenuListExpand != null && !MyMenuListExpand.Core.Released))
                return;

            base.Draw(false, cam, Selected);

            // The unselected text of the current menu item may not ever have been drawn,
            // so update its CameraZoom manually
            CurMenuItem.MyText.MyCameraZoom = MyCameraZoom;

            float Width = MyText.GetWorldWidth() / 2;
            Vector2 ItemPos = Pos + PosOffset;
            if (Center)
                ItemPos.X -= Width;
            CurMenuItem.PosOffset = ItemPos;

            CurMenuItem.MyMenu = MyMenu;
            if (!Selectable && GrayOutOnUnselectable) CurMenuItem.DoGrayOut();
            CurMenuItem.Draw(Text, cam, Selected);
            if (!Selectable && GrayOutOnUnselectable) CurMenuItem.DoDeGrayOut();
            if (MyMenuListExpand == null)
                CurMenuItem.MyMenu = null;
            else
                CurMenuItem.MyMenu = MyMenuListExpand.MyMenu;


            if (!Text)
            {
                if (!Selected)
                {
                    if (!CustomArrow)
                    {
                        RightArrow.Base.Origin = ItemPos + new Vector2(2*Width + RightArrow.Base.e1.X, 0) + Menu.DefaultMenuInfo.MenuRightArrow_Offset + cam.Data.Position;
                        LeftArrow.Base.Origin = ItemPos - new Vector2(LeftArrow.Base.e1.X, 0) + Menu.DefaultMenuInfo.MenuLeftArrow_Offset + cam.Data.Position;
                    }
                    else
                    {
                        RightArrow.Base.Origin = ItemPos + new Vector2(2*Width + RightArrow.Base.e1.X, 0) + RightArrowOffset + cam.Data.Position;
                        LeftArrow.Base.Origin = ItemPos - new Vector2(LeftArrow.Base.e1.X, 0) + LeftArrowOffset + cam.Data.Position;
                    }

                    if (DrawArrowsWhenUnselected)
                    {
                        if (!OnLastIndex) RightArrow.Draw();
                        if (!OnFirstIndex) LeftArrow.Draw();
                    }
                }
                else
                {
                    if (!CustomArrow)
                    {
                        RightArrow_Selected.Base.Origin = ItemPos + new Vector2(2 * Width + RightArrow_Selected.Base.e1.X, 0) + Menu.DefaultMenuInfo.MenuRightArrow_Selected_Offset;
                        LeftArrow_Selected.Base.Origin = ItemPos - new Vector2(LeftArrow_Selected.Base.e1.X, 0) + Menu.DefaultMenuInfo.MenuLeftArrow_Selected_Offset;
                    }
                    else
                    {
                        RightArrow_Selected.Base.Origin = ItemPos + new Vector2(2*Width + RightArrow_Selected.Base.e1.X, 0) + RightArrowOffset;
                        LeftArrow_Selected.Base.Origin = ItemPos - new Vector2(LeftArrow_Selected.Base.e1.X, 0) + LeftArrowOffset;
                    }

#if WINDOWS
                    // Highlight selected arrow
                    QuadClass arrow = null;
                    if (ButtonCheck.MouseInUse)
                    {
                        Vector2 mouse = Tools.MouseGUIPos(MyCameraZoom);
                        arrow = GetSelectedArrow();

                        if (arrow != null) { arrow.Scale(1.25f); }
                    }
#endif

                    if (!OnLastIndex) RightArrow_Selected.Draw();
                    if (!OnFirstIndex) LeftArrow_Selected.Draw();

#if PC_VERSION
                    if (arrow != null) { arrow.Scale(1f / 1.25f); }
#endif
                }
            }
        }
    }
}