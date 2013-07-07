using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public delegate void MenuListSelect();
    public class MenuList : MenuItem
    {
        public override string[] GetViewables()
        {
            return new string[] { "RightArrowOffset", "LeftArrowOffset", "Pos", "SelectedPos", "!MyMenu", "MyExpandPos" };
        }

        public bool ExpandOnGo
        {
            get { return _ExpandOnGo; }
            set
            {
                _ExpandOnGo = value;
                if (_ExpandOnGo)
                    //Go = item => Expand();
                    OnClick = item => Expand();
                else
                    OnClick = null;
            }
        }
        bool _ExpandOnGo = false;

		public struct ExpandParams
		{
			public float ScaleItems;
			public Vector2 ShiftTopLeftItem;
			public Vector2 SizePadding;

			public void Initialize()
			{
				ScaleItems = 1f;
				ShiftTopLeftItem = Vector2.Zero;
				SizePadding = Vector2.Zero;
			}
		}
		public ExpandParams MyExpandParams;


        public MenuListExpand MyMenuListExpand;
        public Vector2 MyExpandPos = Vector2.Zero;
        public Action<MenuListExpand,MenuItem> AdditionalExpandProcessing;
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




				//MyMenuListExpand.Pos.RelVal = Tools.Num_Vec;
            }
        }

        public override void Release()
        {
            base.Release();

            MyMenuListExpand = null;

			foreach (var _item in MyList)
			{
				_item.Release();
			}
			MyList = null;

			AdditionalExpandProcessing = null;

			RightArrow = null; LeftArrow = null;
			RightArrow_Selected = null; LeftArrow_Selected = null;
			OnIndexSelect = null; OnConfirmedIndexSelect = null;
			ObjDict = null;
			CurMenuItem = null;
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

        public MenuListSelect OnIndexSelect, OnConfirmedIndexSelect;

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

			MyExpandParams.Initialize();

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

		/// <summary>
		/// Set this to true when you are setting the list index programmatically as a user.
		/// Check this boolean when you are responding to a SetIndex event, so you don't update something that causes another SetIndex to happen.
		/// </summary>
		public static bool ProgrammaticalyCalled = false;

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
                OnIndexSelect();

            if (MyMenuListExpand == null && OnConfirmedIndexSelect != null)
                OnConfirmedIndexSelect();

            CurMenuItem.OnSelect();
        }

		public void IncrementIndex()
		{
			if (Tools.MousePressed())
				IncrementIndex(1);
		}

        int LastIncrDir = 0;
        public void IncrementIndex(int Increment)
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

		public bool LeftRightControlOn = true; 

        bool HoldSelected;
        public override void PhsxStep(bool Selected)
        {
            base.PhsxStep(Selected);

            if (MyMenuListExpand != null && MyMenuListExpand.Core.Released)
                MyMenuListExpand = null;

            HoldSelected = Selected;
            int CurIndex = ListIndex;

			if (LeftRightControlOn)
			{
				Vector2 Dir = Vector2.Zero;
				if (Selected)
				{
					Dir = Vector2.Zero;
					if (Control < 0)
						Dir = ButtonCheck.GetMaxDir(Control);
					else
					{
						if (MyMenu.UseMouseAndKeyboard)
						{
							Dir = ButtonCheck.GetDir_WithMouseAndKeyboard(Control);
						}
						else
						{
							Dir = ButtonCheck.GetDir(Control);
						}
					}
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
						
						// Scrolling
//#if PC_VERSION
//                        int ScrollDir = (int)Math.Sign(Tools.DeltaScroll);
//                        if (ScrollDir != 0)
//                        {
//                            IncrementIndex(ScrollDir);
//                        }
//#endif
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
			// Draw a selected nob if the menu is expanded
			//if (MyMenuListExpand != null)
			//{
			//    LeftArrow_Selected.Size = new Vector2(31, 26);
			//    LeftArrow_Selected.Quad.SetColor(new Vector4(.75f, .75f, .85f, .7f));
			//    LeftArrow_Selected.Draw();
			//}

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
						var Size = Menu.DefaultMenuInfo.MenuArrow_Size * .6f;
						LeftArrow_Selected.Base.e1 = new Vector2(Size.X, 0);
						LeftArrow_Selected.Base.e2 = new Vector2(0, Size.Y);
						RightArrow_Selected.Base.e1 = new Vector2(Size.X, 0);
						RightArrow_Selected.Base.e2 = new Vector2(0, Size.Y);

                        RightArrow_Selected.Base.Origin = ItemPos + new Vector2(2*Width + RightArrow_Selected.Base.e1.X, 0) + RightArrowOffset;
                        LeftArrow_Selected.Base.Origin = ItemPos - new Vector2(LeftArrow_Selected.Base.e1.X, 0) + LeftArrowOffset;
                    }

					// Draw a nob to show a drop down menu is available
					//LeftArrow_Selected.Size = new Vector2(30, 30);
					//LeftArrow_Selected.Quad.MyEffect = Tools.CircleEffect;
					//LeftArrow_Selected.Quad.SetColor(new Vector4(.75f, .75f, .8f, .6f));
					//LeftArrow_Selected.Draw();

#if PC_VERSION
					if (!ButtonCheck.MouseInUse && KeyboardSelectable)
					{
						// Highlight selected arrow
						QuadClass arrow = null;
						if (ButtonCheck.MouseInUse)
						{
							Vector2 mouse = Tools.MouseGUIPos(MyCameraZoom);
							arrow = GetSelectedArrow();

							if (arrow != null) { arrow.Scale(1.25f); }
						}

						if (!OnLastIndex) RightArrow_Selected.Draw();
						if (!OnFirstIndex) LeftArrow_Selected.Draw();

						if (arrow != null) { arrow.Scale(1f / 1.25f); }
					}
#endif
                }
            }
        }
    }
}