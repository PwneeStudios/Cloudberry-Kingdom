using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public static class Cast
    {
        class ToMenuHelper : LambdaFunc_1<Menu, bool>
        {
            Lambda a;

            public ToMenuHelper(Lambda a)
            {
                this.a = a;
            }

            public bool Apply(Menu dummy)
            {
                a.Apply();
                return true;
            }
        }

        public static LambdaFunc_1<Menu, bool> ToMenu(Lambda a)
        {
            return new ToMenuHelper(a);
        }

        class ToMenuHelper1 : LambdaFunc_1<Menu, bool>
        {
            Lambda_1<MenuItem> a;

            public ToMenuHelper1(Lambda_1<MenuItem> a)
            {
                this.a = a;
            }

            public bool Apply(Menu dummy)
            {
                a.Apply(null);
                return true;
            }
        }

        public static LambdaFunc_1<Menu, bool> ToMenu(Lambda_1<MenuItem> a)
        {
            return new ToMenuHelper1(a);
        }

        class LambdaWrapper : Lambda_1<MenuItem>
        {
            Lambda a;

            public LambdaWrapper(Lambda a)
            {
                this.a = a;
            }

            public void Apply(MenuItem dummy)
            {
                a.Apply();
            }
        }

        public static Lambda_1<MenuItem> ToItem(Lambda a)
        {
            return new LambdaWrapper(a);
        }

        class Lambda_1Wrapper : Lambda
        {
            Lambda_1<Menu> a;

            public Lambda_1Wrapper(Lambda_1<Menu> a)
            {
                this.a = a;
            }

            public void Apply()
            {
                a.Apply(null);
            }
        }

        public static Lambda ToAction(Lambda_1<Menu> a)
        {
            return new Lambda_1Wrapper(a);
        }
    }

    public class Menu : ViewReadWrite, IViewableList
    {
        public static class DefaultMenuInfo
        {
            public static Vector4 SelectedNextColor = new Color(100, 250, 100, 255).ToVector4();
            public static Vector4 SelectedBackColor = new Color(250, 100, 100, 255).ToVector4();
            public static Vector4 UnselectedNextColor = new Color(40, 180, 40, 255).ToVector4();
            public static Vector4 UnselectedBackColor = new Color(180, 40, 40, 255).ToVector4();

            public static EzSound Menu_UpDown_Sound = Tools.NewSound("Menu_Hover", .7f);
            public static EzSound Menu_Select_Sound = Tools.NewSound("Menu_Select", .6f);
            public static EzSound Menu_Slide_Sound = Tools.NewSound("Menu_Tick", .3f);
            public static EzSound Menu_ListScroll_Sound = Tools.NewSound("Menu_Hover", .5f);
            public static EzSound Menu_Back_Sound = Tools.NewSound("Menu_Back", .8f);
            public static int Menu_Slide_SoundDelay = 8;

            public static EzTexture MenuRightArrow_Texture = Tools.Texture("ListRightArrow");
            public static EzTexture MenuLeftArrow_Texture = Tools.Texture("ListLeftArrow");
            public static Vector2 MenuRightArrow_Offset = new Vector2(20, -14);
            public static Vector2 MenuLeftArrow_Offset = new Vector2(-20, -14);
            public static Vector2 MenuArrow_Size = new Vector2(45, 45);
            public static Vector4 MenuArrow_Color = new Color(255, 255, 255, 255).ToVector4();

            public static EzTexture MenuRightArrow_Selected_Texture = Tools.Texture("ListRightArrow");
            public static EzTexture MenuLeftArrow_Selected_Texture = Tools.Texture("ListLeftArrow");
            public static Vector2 MenuRightArrow_Selected_Offset = new Vector2(20, -14);
            public static Vector2 MenuLeftArrow_Selected_Offset = new Vector2(-20, -14);
            public static Vector2 MenuArrow_Selected_Size = new Vector2(45, 45);
            public static Vector4 MenuArrow_Selected_Color = new Color(255, 255, 255, 0).ToVector4();

            public static EzTexture SliderBack_Texture = Tools.Texture("menuslider_bar");
            public static Vector2 SliderBack_Size = new Vector2(250, 35);
            public static EzTexture Slider_Texture = Tools.Texture("menuslider_slider");
            public static Vector2 Slider_StartPos = new Vector2(-210, 0);
            public static Vector2 Slider_EndPos = new Vector2(210, 0);
            public static Vector2 Slider_Size = new Vector2(28, 55);
        }

        public override string[] GetViewables()
        {
            return new string[] { "BackdropShift", "Items" };
        }

        public override string CopyToClipboard(string suffix)
        {
            string s = "";

            if (Items.Count > 0) s += "MenuItem _item;\n";
            foreach (MenuItem item in Items)
            {
                s += item.ToCode(suffix) + "\n";
            }

            if (Items.Count > 0) s += "\n";
            s += string.Format("{0}Pos = {1};\n", suffix, Tools.ToCode(Pos));

            return s;
        }

        public override void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
            Pos += shift;
        }

        class FindItemByNameLambda : LambdaFunc_1<MenuItem, bool>
        {
            string name;
            public FindItemByNameLambda(string name)
            {
                this.name = name;
            }

            public bool Apply(MenuItem item)
            {
                return item.Name == name;
            }
        }

        public MenuItem FindItemByName(string name)
        {
            return Tools.Find(Items, new FindItemByNameLambda(name));
        }

        public void GetChildren(List<InstancePlusName> ViewableChildren)
        {
            if (Items != null)
                foreach (MenuItem item in Items)
                {
                    string name = item.MyText.MyString;
                    ViewableChildren.Add(new InstancePlusName(item, name));
                }
        }

        /// <summary>
        /// Layer of the menu, used in DrawPiles
        /// </summary>
        public int Layer = 0;

        public QuadClass SelectIcon;

        public Vector2 PosOffset;
        public FancyVector2 FancyPos;
        public Vector2 Pos
        {
            get { return FancyPos.RelVal; }
            set { FancyPos.RelVal = value; }
        }

        public bool FixedToCamera = true;

        public EzSound UpDownSound, SelectSound, BackSound, SlideSound, ListScrollSound;
        public bool ReadyToPlaySound;

        public bool SkipPhsx;

        public Menu ParentMenu;

        int _Control;
        public int Control { get { return _Control; } set { _Control = value; } }

        public List<MenuItem> Items;
        public int CurIndex;

        public MenuItem CurItem
        {
            get
            {
                if (Items == null)
                    return null;

                return Items[CurIndex];
            }
        }

        public int SelectDelay = 11;
        int DelayCount;
        int MotionCount;

        public PieceQuad MyPieceQuad, MyPieceQuadTemplate;
        public PieceQuad MyPieceQuad2, MyPieceQuadTemplate2;
        public Vector2 BackdropShift;


        /// <summary>
        /// Called when the player presses (B) while the menu is active.
        /// Should return true if the menu phsx step should be ended immediately after executing the delegate.
        /// </summary>
        public LambdaFunc_1<Menu, bool> OnA, OnB, OnX, OnStart;
        public Lambda OnSelect, OnY;

        public Vector2 TR, BL;

        public int CurDrawLayer;

        public int OnA_AutoTimerLength = -1, OnA_AutoTimerCount;

        public bool Released;
        public void Release() { Release(true); }
        public void Release(bool ReleaseParents)
        {
            if (Released)
                return;
            Released = true;

            if (SelectIcon != null) SelectIcon.Release(); SelectIcon = null;
            if (FancyPos != null) FancyPos.Release(); FancyPos = null;

            if (ReleaseParents && ParentMenu != null) ParentMenu.Release(true);
            ParentMenu = null;

            if (Items != null)
                foreach (MenuItem item in Items)
                    item.Release();
            Items = null;

            OnStart = OnX = OnA = OnB = null;

            AdditionalCheckForOutsideClick = null;
        }

        public void ClearList()
        {
            if (Items != null)
                foreach (MenuItem item in Items)
                    item.Release();
            Items.Clear();
        }

        public Menu() { Init(); }
        public Menu(bool FixedToCamera)
        {
            Init();
            this.FixedToCamera = FixedToCamera;
        }

        protected void Init()
        {
            UpDownSound = DefaultMenuInfo.Menu_UpDown_Sound;
            SelectSound = DefaultMenuInfo.Menu_Select_Sound;
            BackSound = DefaultMenuInfo.Menu_Back_Sound;
            SlideSound = DefaultMenuInfo.Menu_Slide_Sound;
            ListScrollSound = DefaultMenuInfo.Menu_ListScroll_Sound;

            SkipPhsx = true;

            Control = -1;

            Items = new List<MenuItem>();
            CurIndex = 0;

            OnB = new DefaultOnBProxy();

            MyPieceQuadTemplate = null;
            MyPieceQuadTemplate2 = null;
        }

        class DefaultOnBProxy : LambdaFunc_1<Menu, bool>
        {
            public bool Apply(Menu menu)
            {
                return Menu.DefaultOnB(menu);
            }
        }

        public static bool DefaultOnB(Menu menu)
        {
            menu.Release(false);

            return true;
        }

        public bool HasSelectedThisStep = false;

        /// <summary>
        /// If true the menu will wrap from top to bottom or vis a versa when selecting.
        /// </summary>
        public bool WrapSelect = true;

        public void SelectItem(MenuItem item)
        {
            HasSelectedThisStep = true;

            int Index = Items.IndexOf(item);
            if (CurIndex != Index)
            {
                SelectItem(Index);
            }
        }

        public virtual void SelectItem(int Index)
        {
            HasSelectedThisStep = true;

            // If no items are selectable, return
            bool All = true;
            foreach (MenuItem item in Items)
                if (item.Selectable)
                    All = false;
            if (All)
                return;

            // Play a selection sound
            if (UpDownSound != null && ReadyToPlaySound)
                UpDownSound.Play();

            int Sign = Math.Sign(Index - CurIndex);
            if (Sign == 0) Sign = 1;

            // Ensure a valid index
            if (WrapSelect)
            {
                if (Index < 0) { CurIndex = Index = Items.Count - 1; }
                if (Index >= Items.Count) { CurIndex = Index = 0; }
            }
            else
                Index = Math.Max(0, Math.Min(Items.Count - 1, Index));


            if (Items[Index].Selectable && Items[Index].Show)
                CurIndex = Index;
            else
            {
                // Find the next selectable index
                while (!Items[Index].Selectable || !Items[Index].Show)
                {
                    //if (Index > CurIndex) Index++;
                    //else if (Index < CurIndex) Index--;
                    //else Index++;

                    Index += Sign;

                    if (WrapSelect)
                    {
                        if (Index < 0) Index = Items.Count - 1;
                        if (Index >= Items.Count) Index = 0;
                    }
                    else
                    {
                        if (Index < 0) { Index = CurIndex; break; }// Index = 0; CurIndex = 0; }
                        if (Index >= Items.Count) { Index = CurIndex; break; }// Index = Items.Count; CurIndex = Items.Count + 1; }
                    }
                }

                CurIndex = Index;
            }

            if (OnSelect != null)
                OnSelect.Apply();

            SkipPhsx = true;
        }

        /// <summary>
        /// If true then items can only be interacted with via the mouse,
        /// including both selection and activation of items
        /// </summary>
        public bool MouseOnly = false;

        /// <summary>
        /// If true then while the mouse button is down a new menu item can be selected.
        /// </summary>
        public bool SlipSelect = false;

        /// <summary>
        /// When true the current selected item is not highlighted or treated differently than unselected items.
        /// </summary>
        public bool NoneSelected = false;
        public bool AlwaysSelected = false;

        /// <summary>
        /// When true and no item is currently selected, the last activated item will be selected.
        /// This supplements the behavior of NoneSelected
        /// </summary>
        public bool ShowLastActivated = false;

        /// <summary>
        /// The index of the last item to be activated.
        /// </summary>
        public int LastActivatedItem = -1;
        int ActiveTimeStamp;

        /// <summary>
        /// When false all Phsx associated with the menu is paused.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// When true the menu's back command can be activated by clicking outside the menu's active area.
        /// </summary>
        //public bool CheckForOutsideClick = true;
        public bool CheckForOutsideClick = false;

        /// <summary>
        /// When true the menu can modify the mouse cursor show the back icon (when outside the menu's box).
        /// </summary>
        public bool AffectsOutsideMouse = true;

        /// <summary>
        /// Add a delegate that should return true if this menu should NOT go back when the user clicks
        /// </summary>
        public LambdaFunc<bool> AdditionalCheckForOutsideClick;

        bool CheckForBackFromOutsideClick()
        {
#if PC_VERSION
            bool Hit = HitTest();

            if (!Hit && AdditionalCheckForOutsideClick != null)
                Hit |= AdditionalCheckForOutsideClick.Apply();

            // Update the mouse icon to reflect whether clicking will go back or not
            Tools.TheGame.DrawMouseBackIcon = !Hit;

            return !Hit;
#else
            return false;
#endif
        }

#if PC_VERSION
        public bool HitTest() { return HitTest(new Vector2(100, 100)); }
        public bool HitTest(Vector2 HitPadding)
        {
            Vector2 MousePos = Tools.MouseGUIPos(MyCameraZoom);

            bool Hit = false;
            foreach (MenuItem item in Items)
                Hit |= item.HitTest(MousePos, HitPadding);

            return Hit;
        }
#endif
        bool outside = false;
        public virtual void PhsxStep()
        {
            if (!Active || !Show) return;

            if (SkipPhsx)
            {
                SkipPhsx = false;
                return;
            }

            if (Tools.TheGame.DrawCount - ActiveTimeStamp > 5)
            {
                SkipPhsx = true;
                ActiveTimeStamp = Tools.TheGame.DrawCount;
                return;
            }
            ActiveTimeStamp = Tools.TheGame.DrawCount;

#if PC_VERSION
            // Show the mouse 
            Tools.TheGame.ShowMouse = true;

            // If mouse is in use check to see if anything should be selected
            if (ButtonCheck.MouseInUse)
            {
                /*
                if (Tools.MouseDown())
                    NoneSelected = false;
                else*/
                {
                    // MUST BE POSITIVE
                    Vector2 HitPadding = new Vector2(100, 0);

                    // If we are checking whether to start showing selections again,
                    // only do so if we actually hit a MenuItem
                    if (NoneSelected)
                        HitPadding = Vector2.Zero;

                    if (!HitTest(HitPadding))
                        NoneSelected = true;
                    else
                        NoneSelected = false;
                }
            }
            else
            {
                // If the mouse isn't in use and the menu is capable of using the keyboard,
                // then start behaving as if the keyboard is the main input device and no mouse exists
                if (!MouseOnly)
                    NoneSelected = false;
            }
#endif 

            // Start button action
            if (OnStart != null && ButtonCheck.State(ControllerButtons.Start, Control).Pressed)
            {
                ButtonCheck.PreventInput();
                OnStart.Apply(this);
            }

            // X button action
            if (OnX != null && ButtonCheck.State(ControllerButtons.X, Control).Pressed)
            {
                ButtonCheck.PreventInput();
                OnX.Apply(this);
            }

            // Y button action
            if (OnY != null && ButtonCheck.State(ControllerButtons.Y, Control).Pressed)
            {
                ButtonCheck.PreventInput();
                OnY.Apply();
            }

            // Allow for a new item to be selected if the user has stopped holding down A (or LeftMouseButton)
            if (!ButtonCheck.State(ControllerButtons.A, Control).Pressed)
                HasSelectedThisStep = false;

            if (OnA_AutoTimerLength > 0 && OnA_AutoTimerCount > 0)
            {
                OnA_AutoTimerCount--;
                if (OnA_AutoTimerCount == 0)
                    if (OnA != null)
                        if (OnA.Apply(this))
                            return;
            }

            bool ActivateOnA = false;
            if (ButtonCheck.State(ControllerButtons.A, Control).Pressed)
                ActivateOnA = true;

            // Don't activate the item if it isn't being drawn as selected
            //if (NoneSelected)
            //    ActivateOnA = false;

#if WINDOWS
            if (!Tools.MouseNotDown())
                ActivateOnA = false;
#endif
            // A button
            if (ActivateOnA)
            {
                bool CheckForOverride = true;
#if PC_VERSION
                if (ButtonCheck.State(ButtonCheck.Go_Secondary).Pressed)
                    CheckForOverride = false;
#endif
                if (OnA != null && !(CheckForOverride && Items[CurIndex].OverrideA))
                {
                    //ButtonCheck.PreventInput();
                    if (OnA.Apply(this))
                        return;
                }
            }

            if (SkipPhsx)
            {
                return;
            }

            // Click outside the menu to go back
            bool ClickBack = false;
            
#if PC_VERSION
            if (CheckForOutsideClick && NoneSelected && ButtonCheck.MouseInUse && Tools.MouseReleased())
                if (outside)
                    ClickBack = true;
            if (!Tools.CurMouseDown())
            {
                if (CheckForOutsideClick && OnB != null)
                    outside = CheckForBackFromOutsideClick();
                else
                {
                    outside = false;
                    if (AffectsOutsideMouse)
                        Tools.TheGame.DrawMouseBackIcon = false;
                }
            }

            if (Tools.RightMouseReleased())
                ClickBack = true;
#endif
            // B button
            if (ButtonCheck.State(ControllerButtons.B, Control).Pressed ||
                ClickBack)
            {
                if (OnB != null)
                {
                    if (BackSound != null)
                        BackSound.Play();

                    if (OnB.Apply(this))
                        return;

                    ButtonCheck.PreventInput();
                }
            }

            if (SkipPhsx)
            {
                return;
            }

            foreach (MenuItem item in Items)
            {
                item.PhsxStep(item == Items[CurIndex] && (!NoneSelected || AlwaysSelected));
                if (Released)
                    return;
            }

#if PC_VERSION
            // If the mouse is in use and nothing is selected, then hitting a key on the keyboard
            // should only hide the mouse, it shouldn't immediately change what is selected
            if (ButtonCheck.MouseInUse)
            {
                if (NoneSelected) DelayCount = SelectDelay;
                else DelayCount = 0;
            }
#endif

            // If the menu can use the keyboard then check for index changes by the arrow keys
#if PC_VERSION
            if (!MouseOnly && !ButtonCheck.PrevMouseInUse)
#else
            if (!MouseOnly)
#endif
            {
                if (DelayCount > 0)
                    DelayCount--;

                Vector2 Dir = Vector2.Zero;
                if (Control < 0)
                {
                    Dir = ButtonCheck.GetMaxDir(Control == -1);
                }
                else
                    Dir = ButtonCheck.GetDir(Control);

                if (Dir.Length() < .2f)
                    DelayCount = 0;

                if (Math.Abs(Dir.Y) > ButtonCheck.ThresholdSensitivity)
                {
                    MotionCount++;
                    if (DelayCount <= 0)
                    {
                        if (Dir.Y > 0) SelectItem(CurIndex - 1);
                        else SelectItem(CurIndex + 1);

                        DelayCount = SelectDelay;
                        if (MotionCount > SelectDelay * 2)
                            DelayCount /= 2;
                    }
                }
                else
                    MotionCount = 0;
            }

            ReadyToPlaySound = true;
        }

        public void ArrangeItems(float Spacing, Vector2 Center)
        {
            Vector2 Pos = Center;
            for (int i = 0; i < Items.Count; i++)
            {
                Pos.Y -= Spacing + Items[i].Height() / 2;

                Items[i].Pos = Pos;

                Pos.Y -= Spacing + Items[i].Height() / 2;
            }

            float Height = Center.Y - Pos.Y;
            foreach (MenuItem item in Items)
                item.Pos.Y += Height / 2;
        }

        

        public void SetBoundary()
        {
            SetBoundary(new Vector2(110, 84), new Vector2(110, 84));
        }
        public void SetBoundary(Vector2 Padding)
        {
            SetBoundary(Padding, Padding);
        }
        public void SetBoundary(Vector2 TR_Padding, Vector2 BL_Padding)
        {
            CalcBounds();

            TR += TR_Padding;
            BL -= BL_Padding;

            ResetPieces();
        }

        public void CalcBounds()
        {
            TR = new Vector2(-100000, -100000);
            BL = new Vector2(100000, 100000);

            foreach (MenuItem item in Items)
            {
                Vector2 Size = item.Size();
                TR = Vector2.Max(TR, item.Pos + Size / 2);
                BL = Vector2.Min(BL, item.Pos - Size / 2);
            }
        }

        static int SortByHeightMethod(MenuItem item1, MenuItem item2)
        {
            return -item1.Pos.Y.CompareTo(item2.Pos.Y);
        }

        public void SortByHeight()
        {
            Items.Sort(SortByHeightMethod);
        }

        public void ResetPieces()
        {
            MyPieceQuad = new PieceQuad();
            MyPieceQuad.Clone(MyPieceQuadTemplate);
            MyPieceQuad.CalcQuads((TR - BL) / 2);
            BackdropShift = (TR + BL) / 2;

            if (MyPieceQuadTemplate2 != null)
            {
                MyPieceQuad2 = new PieceQuad();
                MyPieceQuad2.Clone(MyPieceQuadTemplate2);
                MyPieceQuad2.CalcQuads((TR - BL) / 2);
            }
        }

        public void DrawNonText(int Layer)
        {
            MyCameraZoom = Tools.CurCamera.Zoom;
            if (!Show) return;

            if (FancyPos != null)
                PosOffset = FancyPos.Update();

            if (MyPieceQuad != null && Layer == 0)
            {
                if (FixedToCamera)
                    MyPieceQuad.Base.Origin = Tools.CurGameData.MyLevel.MainCamera.Data.Position + BackdropShift + PosOffset;
                else
                    MyPieceQuad.Base.Origin = BackdropShift + PosOffset;
                MyPieceQuad.Draw();
            }

            CurDrawLayer = Layer;

            int SelectedIndex = ApparentCurIndex;

            // Draw item text backdrops
            foreach (MenuItem item in Items)
            {
                item.PosOffset = PosOffset;
                if (SelectedIndex < 0 || item != Items[SelectedIndex])
                    item.DrawBackdrop(false);
            }
            if (SelectedIndex >= 0 && Items.Count > 0)
                Items[SelectedIndex].DrawBackdrop(true);

            // Draw item non-text
            foreach (MenuItem item in Items)
            {
                item.PosOffset = PosOffset;
                item.Draw(false, Tools.CurLevel.MainCamera, DrawItemAsSelected(item));
            }

            if (MyPieceQuadTemplate2 != null)
            {
                MyPieceQuad2.Base.Origin = Tools.CurLevel.MainCamera.Data.Position + BackdropShift + PosOffset;
                MyPieceQuad2.Draw();
            }
        }

        public void DrawNonText2()
        {
            MyCameraZoom = Tools.CurCamera.Zoom;
            if (!Show) return;

            CurDrawLayer = 1;
            foreach (MenuItem item in Items)
            {
                item.Draw(false, Tools.CurLevel.MainCamera, DrawItemAsSelected(item));
            }

            // Draw select icon
            if (SelectIcon != null && ApparentCurIndex >= 0)
            {
                SelectIcon.Pos = Items[ApparentCurIndex].Pos + Items[ApparentCurIndex].PosOffset + Items[ApparentCurIndex].SelectIconOffset;
                if (FixedToCamera)
                    SelectIcon.Pos += Tools.CurLevel.MainCamera.Data.Position; 
                SelectIcon.Draw();
            }
        }

        public virtual void DrawText(int Layer)
        {
            MyCameraZoom = Tools.CurCamera.Zoom;
            if (!Show) return;

            CurDrawLayer = Layer;

            // Draw item text
            foreach (MenuItem item in Items)
            {
                item.PosOffset = PosOffset;
                if (item.UnaffectedByScroll)
                    item.PosOffset -= FancyPos.RelVal;

                item.Draw(true, Tools.CurLevel.MainCamera, DrawItemAsSelected(item));
            }
        }

        /// <summary>
        /// Whether an item should be drawn as selected or not.
        /// </summary>
        protected bool DrawItemAsSelected(MenuItem item)
        {
#if PC_VERSION
#else
            // Never draw anything as selected if the menu is mouse only and there is no mouse
            if (MouseOnly) return false;
#endif

            if (!NoneSelected || AlwaysSelected)
                return item == Items[CurIndex];
            else if (ShowLastActivated && LastActivatedItem >= 0)
                return item == Items[LastActivatedItem];
            else
                return false;
        }

        /// <summary>
        /// The index of the item to be drawn as selected
        /// </summary>
        int ApparentCurIndex
        {
            get
            {
                if (!NoneSelected)
                    return CurIndex;
                else if (ShowLastActivated && LastActivatedItem >= 0)
                    return LastActivatedItem;
                else
                    return -1;
            }
        }

        Vector2 _MyCameraZoom = Vector2.One;
        /// <summary>
        /// The value of the camera zoom the last time this menu was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        //bool _Show = true;
        //public bool Show { get { return _Show; } set { _Show = value; } }
        public bool Show = true;

        public virtual void Draw()
        {
            MyCameraZoom = Tools.CurCamera.Zoom;

            if (!Show) return;

            DrawNonText(0);
            DrawText(0);
            Tools.Render.EndSpriteBatch();
            DrawNonText2();
            DrawText(1);
        }

        public void Add(MenuItem item) { Add(item, Items.Count); }
        public void Add(MenuItem item, int index)
        {
            item.SelectSound = SelectSound;
            item.SlideSound = SlideSound;
            item.ListScrollSound = ListScrollSound;

            item.MyMenu = this;
            item.FixedToCamera = FixedToCamera;
            item.Control = Control;
            Items.Insert(index, item);
        }
    }
}
