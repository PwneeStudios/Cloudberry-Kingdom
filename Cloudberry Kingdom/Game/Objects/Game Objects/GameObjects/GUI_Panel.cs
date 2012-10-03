using Microsoft.Xna.Framework;
using System;
using CoreEngine;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GUI_Panel : GameObject
    {
        public override string[] GetViewables()
        {
            return new string[] { "MyMenu", "MyPile" };
        }

        int _Control = -2; // MUST be initialized to something negative
        public int Control
        {
            set
            {
                _Control = value;
                if (MyMenu != null) MyMenu.Control = value;
            }
            get
            {
                return _Control;
            }
        }

        public Menu MyMenu;
        public DrawPile MyPile;

        public override string CopyToClipboard(string suffix)
        {
            string s = "";

            if (MyMenu != null) s += MyMenu.CopyToClipboard("MyMenu.") + "\n";
            if (MyPile != null) s += MyPile.CopyToClipboard("MyPile.");

            return s;
        }

        public override void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
            if (MyMenu != null) MyMenu.ProcessMouseInput(shift, ShiftDown);
            if (MyPile != null) MyPile.ProcessMouseInput(shift, ShiftDown);
        }

        /// <summary>
        /// The accumulation of all shifts
        /// </summary>
        public Vector2 AmountShifted = Vector2.Zero;

        /// <summary>
        /// Shift the DrawPile and Menu
        /// </summary>
        public void Shift(Vector2 shift)
        {
            AmountShifted += shift;

            if (MyPile != null) MyPile.Pos += shift;
            if (MyMenu != null) MyMenu.Pos += shift;
        }

        /// <summary>
        /// Whether the Panel's physics is running. Set to false when slid out.
        /// </summary>
        public bool Active;

        /// <summary>
        /// The GUI_Panel that called up this GUI_Panel, if it exists.
        /// </summary>
        public GUI_Panel Caller;

        /// <summary>
        /// When ReturnToCaller is called, this is the number of frames to wait before showing the parent.
        /// </summary>
        public int ReturnToCallerDelay = 0;

        /// <summary>
        /// When calling a child panel, this is the number of frames to wait before showing the child.
        /// </summary>
        public int CallDelay = 0;

        /// <summary>
        /// Don't allow the menu to back out if there is no parent menu that called it.
        /// </summary>
        public bool NoBackIfNoCaller = false;

        /// <summary>
        /// Hide the panel and return to its parent.
        /// </summary>
        public virtual void ReturnToCaller()
        {
            if (NoBackIfNoCaller && Caller == null) return;

            ReleaseWhenDone = true;

            Hide();

            if (Caller != null)
                MyGame.WaitThenDo(ReturnToCallerDelay, Caller.OnReturnTo);
        }

        /// <summary>
        /// When true the Panel will release itself once all animations are finished.
        /// </summary>
        public bool ReleaseWhenDone = false;

        /// <summary>
        /// When true the Panel will release itself once all scaling animations are finished.
        /// </summary>
        protected bool ReleaseWhenDoneScaling = false;

        public virtual void ItemReturnToCaller(MenuItem item)
        {
            ReturnToCaller();
        }

        public virtual bool MenuReturnToCaller(Menu menu)
        {
            ReturnToCaller();

            return true;
        }

        /// <summary>
        /// Called when a child panel is exiting to this parent panel.
        /// </summary>
        public virtual void OnReturnTo()
        {
            // Reset the menu's selected item's oscillate
            if (MyMenu != null) MyMenu.CurItem.OnSelect();

            // Activate and show the panel
            Active = true;
            Show();
        }

        /// <summary>
        /// Called to deactivate this panel and bring a child panel.
        /// </summary>
        public void Call(GUI_Panel child) { Call(child, CallDelay); }
        public virtual void Call(GUI_Panel child, int Delay)
        {
            child.Caller = this;

            SetChildControl(child);

            MyGame.WaitThenDo(Delay, () => MyGame.AddGameObject(child));

            Active = false;
        }

        protected virtual void SetChildControl(GUI_Panel child)
        {
            // Copy control index to the child panel if the child hasn't initialized it's control already
            if (child.Control < 0)
                child.Control = Control;
        }

        /// <summary>
        /// Called to show the Panel. By default, called by OnReturnTo.
        /// </summary>
        public virtual void Show()
        {
            Hid = false;
        }

        /// <summary>
        /// Called to hide the Panel. By default, called by ReturnToCaller.
        /// </summary>
        public virtual void Hide()
        {
            PauseGame = false;
            Hid = true;
        }

        public bool Hid = true;

        public FancyVector2 Pos;

        public GUI_Panel()
        {
            Constructor();
        }

        public GUI_Panel(bool CallBaseConstructor)
        {
            if (CallBaseConstructor)
                Constructor();
        }

        public virtual void Constructor()
        {
            Tools.StartGUIDraw();
            Init();
            Tools.EndGUIDraw();
        }

        public override void Init()
        {
            base.Init();

            Pos = new FancyVector2();
        }

        public bool FixedToCamera = true;
        public override void OnCameraChange()
        {
            base.OnCameraChange();

            if (FixedToCamera && Core.MyLevel != null)
            {
                Pos.Center = Core.MyLevel.MainCamera;
                //Pos.SetCenter(Core.MyLevel.MainCamera, false);
            }
        }
        public override void OnAdd()
        {
            EnsureFancy();

            base.OnAdd();

            if (FixedToCamera)
                Pos.SetCenter(Core.MyLevel.MainCamera, true);
            Pos.Update();
        }

        public void SlideIn() { SlideIn(SlideInLength); }
        public virtual void SlideIn(int Frames)
        {
            Active = true;
            //Pos.LerpTo(Vector2.Zero, Frames, LerpStyle.DecayNoOvershoot);
            Pos.LerpTo(Vector2.Zero, Frames);
        }

        /// <summary>
        /// Copy the slide in and out lengths from a source panel.
        /// </summary>
        public void CopySlideLengths(GUI_Panel source)
        {
            SlideInLength = source.SlideInLength;
            SlideOutLength = source.SlideOutLength;
        }

        /// <summary>
        /// The number of frames a slide in/out lasts.
        /// </summary>
        public int SlideOutLength = 30, SlideInLength = 30;

        /// <summary>
        /// The number of frames a slide lasts.
        /// </summary>
        public virtual int SlideLength { set { SlideInLength = SlideOutLength = value; } }

        public enum PresetPos { Left, Right, Top, Bottom };
        public void SlideOut(PresetPos Preset) { SlideOut(Preset, SlideOutLength); }
        public virtual void SlideOut(PresetPos Preset, int Frames)
        {
            // Don't slide out if we've already slid out
            //if (!Active && !Tools.CurCamera.OnScreen(Pos.AbsVal)) return;

            Active = false;

            Vector2 Destination = SlideOutDestination(Preset);

            if (Frames == 0)
            {
                Pos.RelVal = Destination;
                Pos.Playing = false;
            }
            else
                Pos.LerpTo(Destination, Frames);
        }

        /// <summary>
        /// When sliding out this is the number of camera widths/heights to move the panel.
        /// </summary>
        public Vector2 DestinationScale = Vector2.One;

        /// <summary>
        /// Where a panel slides out to (in relative screen coordinates)
        /// </summary>
        public virtual Vector2 SlideOutDestination(PresetPos Preset)
        {
            Vector2 Destination = Vector2.Zero;
            switch (Preset)
            {
                case PresetPos.Left:
                    Destination = new Vector2(-Core.MyLevel.MainCamera.GetWidth(), 0);
                    break;

                case PresetPos.Right:
                    Destination = new Vector2(Core.MyLevel.MainCamera.GetWidth(), 0);
                    break;

                case PresetPos.Top:
                    Destination = new Vector2(0, Core.MyLevel.MainCamera.GetHeight());
                    break;

                case PresetPos.Bottom:
                    Destination = new Vector2(0, -Core.MyLevel.MainCamera.GetHeight());
                    break;
            }

            return Destination * DestinationScale;
        }

        /// <summary>
        /// Make sure that DrawPiles and Menus have FancyPos positions
        /// </summary>
        protected void EnsureFancy()
        {
            if (MyPile != null && MyPile.FancyPos == null)
                MyPile.FancyPos = new FancyVector2(Pos);
            if (MyMenu != null && MyMenu.FancyPos == null)
                MyMenu.FancyPos = new FancyVector2(Pos);
        }

        public virtual bool OnScreen()
        {
            if (FixedToCamera)
            {
                return Pos.Playing ||
                    // Pos.RelVal.Length() < .5f * Tools.CurLevel.MainCamera.GetWidth();
                    Math.Abs(Pos.RelVal.X) < .85f * Tools.CurLevel.MainCamera.GetWidth() &&
                    Math.Abs(Pos.RelVal.Y) < .85f * Tools.CurLevel.MainCamera.GetHeight();
            }
            else
                return MyGame.Cam.OnScreen(Pos.Update(), 800);
        }

        Vector2 _MyCameraZoom = Vector2.One;
        /// <summary>
        /// The value of the camera zoom the last time this panel was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        protected bool IsOnScreen = false;
        protected override void MyDraw()
        {
            MyCameraZoom = Tools.CurCamera.Zoom;

            if (CloudberryKingdomGame.HideGui) return;
            if (!Core.Show || Core.Released) return;

            // Skip if offscreen
            IsOnScreen = OnScreen();
            if (!IsOnScreen)
                return;

            EnsureFancy();

            EnsurePileFancyPos();
            EnsureMenuFancyPos();

            if (MyPile != null)
                MyPile.Update();

            for (int i = 0; i < 3; i++)
            {
                // Draw the DrawPile
                if (MyPile != null)
                    MyPile.Draw(i);

                // Draw the menu
                if (MyMenu != null && MyMenu.Layer == i)
                    MyMenu.Draw();
            }
        }

        /// <summary>
        /// Ensures that the DrawPile's position is attached to the GUI_Panels's center
        /// </summary>
        void EnsurePileFancyPos()
        {
            if (MyPile != null)
            {
                MyPile.FancyPos.SetCenter(Pos, true);
                MyPile.FancyPos.Update();
            }
        }

        /// <summary>
        /// Ensures that the menu's position is attached to the GUI_Panels's center
        /// </summary>
        void EnsureMenuFancyPos()
        {
            if (MyMenu != null)
            {
                MyMenu.FancyPos.SetCenter(Pos, true);
                MyMenu.FancyPos.Update();
            }
        }



        public virtual void DrawNonText()
        {
            EnsurePileFancyPos();
            EnsureMenuFancyPos();

            if (MyPile != null)
                MyPile.DrawNonText(0);

            if (MyMenu != null)
                MyMenu.DrawNonText(0);
        }
        public void DrawNonText2()
        {
            EnsurePileFancyPos();
            EnsureMenuFancyPos();

            if (MyMenu != null)
                MyMenu.DrawNonText2();
        }
        public void DrawText()
        {
            EnsurePileFancyPos();
            EnsureMenuFancyPos();

            if (MyPile != null)
                MyPile.DrawText(0);

            if (MyMenu != null)
                MyMenu.DrawText(0);
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Caller = null;
        }

        public void PauseAnimation() { Pos.Playing = false; }
        public void UnpauseAnimation() { Pos.Playing = true; }

        protected override void MyPhsxStep()
        {
            CheckForBackFromOutsideClick();

            Level level = Core.MyLevel;

            Pos.Update();

            // Release if we are set to release and are done with any animations.
            if (ReleaseWhenDone && !Pos.Playing) Release();
            // Release if we are set to release after scaling and scaling is done
            if (MyPile != null && MyPile.FancyScale != null)
                if (ReleaseWhenDoneScaling && !MyPile.FancyScale.Playing) Release();

            if (!Active) return;

            if (MyMenu != null && !MyMenu.Released)
                MyMenu.PhsxStep();

            // Right shoulder action
            if (ButtonCheck.State(ControllerButtons.RS, Control).Pressed)
            {
                OnRightShoulder();
            }
        }

        public virtual void OnRightShoulder()
        {
        }

        public virtual bool HitTest(Vector2 pos)
        {
            return false;
        }

        public bool CheckForOutsideClick = false;
        bool outside = false;
        public Action OnOutsideClick;
        void CheckForBackFromOutsideClick()
        {
            if (!CheckForOutsideClick) return;
            if (OnOutsideClick == null) return;

#if PC_VERSION
            // Show the mouse so we can see when we are outside the panel
            Tools.TheGame.ShowMouse = true;

            // check if the player clicks back outside the panel
            bool ClickBack = false;
            if (CheckForOutsideClick && ButtonCheck.MouseInUse && Tools.MouseReleased())
                if (outside)
                    ClickBack = true;
            if (!Tools.CurMouseDown())
                if (CheckForOutsideClick) outside = IsOutside();

            if (Tools.RightMouseReleased())
                ClickBack = true;

            // If so then perfom the designated action
            if (ClickBack)
                OnOutsideClick();
#endif
        }

        bool IsOutside()
        {
#if PC_VERSION
            if (Tools.ViewerIsUp) return false;

            Vector2 MousePos = Tools.MouseGUIPos(MyCameraZoom);
            bool Hit = HitTest(MousePos);

            // Update the mouse icon to reflect whether clicking will go back or not
            Tools.TheGame.DrawMouseBackIcon = !Hit;

            return !Hit;
#else
            return false;
#endif
        }
    }
}