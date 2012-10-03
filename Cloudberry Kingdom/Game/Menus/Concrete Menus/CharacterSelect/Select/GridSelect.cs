using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public class GridSelect : ViewReadWrite
    {
        public override string[] GetViewables() { return new string[] { }; }

        protected List<object> AssociatedObjects;

        /// <summary>
        /// The index of the player controlling the grid selection.
        /// </summary>
        public int PlayerIndex;

        protected EzSound MoveSound;

        public FancyVector2 FancyPos;

        public Vector2 Pos
        {
            get
            {
                return FancyPos.RelVal;
            }
            set
            {
                FancyPos.RelVal = value;
            }
        }

        /// <summary>
        /// The number of items added to the grid.
        /// </summary>
        int Count;

        int J, K, HeightOffset, Width, Height;
        protected Vector2 Size;
        Vector2 Spacing;

        int FullHeight
        {
            get 
            {
                if (Count % Width == 0)
                    return Count / Width;
                else
                    return Count / Width + 1;
            }
        }

        Vector2 PrevDir;
        int NoMoveCountX, NoMoveCountY;
        protected int NoMoveDuration = 9, NoMoveDuration_Fast = 6;//8;

        enum DirMoved { Horizontal, Vertical };
        DirMoved LastDirMoved;

        /// <summary>
        /// Whether the grid wraps from left to right and vis a versa.
        /// </summary>
        protected bool LeftRightWrap = false;

        /// <summary>
        /// Whether the grid wraps from top to bottm and vis a versa.
        /// </summary>
        protected bool UpDownWrap = true;

        public virtual void Release()
        {
            AssociatedObjects = null;
        }

        public void Add()
        {
            Count++;
        }

        public GridSelect() { }
        public GridSelect(Vector2 Size, int Width, int Height)
        {
            Init(Size, Width, Height);
        }

        public virtual void Init(Vector2 Size, int Width, int Height)
        {
            AssociatedObjects = new List<object>();

            FancyPos = new FancyVector2();

            this.Size = Size;
            this.Width = Width;
            this.Height = Height;

            Spacing = 2 * new Vector2((Size.X - 20) / Width, (Size.Y - 20) / Height);
        }

#if PC_VERSION
        public virtual void MouseInteract()
        {
            int i;

            Vector2 mouse = Tools.MouseGUIPos(MyCameraZoom);

            for (int j = 0; j < Width; j++)
            {
                for (int k = 0; k < Height; k++)
                {
                    i = (k + HeightOffset) * Width + j;
                    if (i >= 0 && i < Count)
                    {
                        // Whether the mouse is trying to click on the item
                        bool Interact = (Tools.MouseNotDown() && ItemHitTest(i, mouse));
                        
                        if (Interact)
                        {
#if PC_VERSION
                            MouseInBox = true;
#endif
                        }

                        // If this item wasn't previously selected
                        if (GetIndex() != i || !DrawSelectedLast)
                        {
                            if (Interact)
                                SetIndex(i);

                            if (i == GetIndex())
                                ItemMouseInteract(i);
                        }
                    }
                }
            }
        }

        public virtual bool ItemHitTest(int i, Vector2 MousePos)
        {
            return false;
        }

        public virtual void ItemMouseInteract(int i)
        {
        }
#endif

        /// <summary>
        /// When true the selected item is drawn last, above all other items
        /// </summary>
        public bool DrawSelectedLast = true;

        Vector2 _MyCameraZoom = Vector2.One;
        /// <summary>
        /// The value of the camera zoom the last time this EzText was drawn
        /// </summary>
        public Vector2 MyCameraZoom { get { return _MyCameraZoom; } set { _MyCameraZoom = value; } }

        public int NumLayers = 1;
        public virtual void Draw()
        {
            MyCameraZoom = Tools.CurCamera.Zoom;

            int i;
            Vector2 pos;

            // Draw the unselected items
            for (int Layer = 0; Layer < NumLayers; Layer++)
            for (int j = 0; j < Width; j++)
            {
                for (int k = 0; k < Height; k++)
                {
                    i = (k + HeightOffset) * Width + j;
                    if (i >= 0 && i < Count && (GetIndex() != i || !DrawSelectedLast))
                    {
                        if (Inverted)
                        {
                            pos = FancyPos.Pos + Spacing * new Vector2(j - Width / 2f + .5f, (Height - k) - Height / 2f + .5f);
                            pos.Y -= Spacing.Y / 2;
                        }
                        else
                            pos = FancyPos.Pos + Spacing * new Vector2(j - Width / 2f + .5f, k - Height / 2f + .5f);
                        DrawItem(i, pos, GetIndex() == i, Layer);
                    }
                }
            }
            
            // Draw the selected quad
            if (DrawSelectedLast)
            {
                i = GetIndex();

                if (i >= 0 && i < Count)
                {
                    if (Inverted)
                    {
                        pos = FancyPos.Pos + Spacing * new Vector2(J - Width / 2f + .5f, (Height - K) + HeightOffset - Height / 2f + .5f);
                        //pos = FancyPos.Pos + Spacing * new Vector2(J - Width / 2f + .5f, (Height - K) - Height / 2f + .5f);
                        pos.Y -= Spacing.Y / 2;
                    }
                    else
                        pos = FancyPos.Pos + Spacing * new Vector2(J - Width / 2f + .5f, K - HeightOffset - Height / 2f + .5f);
                        //pos = FancyPos.Pos + Spacing * new Vector2(J - Width / 2f + .5f, K - Height / 2f + .5f);
                    DrawItem(i, pos, true);
                }
            }
        }

        public bool Inverted = true;

        public virtual void DrawItem(int i, Vector2 pos, bool selected)
        {
#if PC_VERSION
            // Don't draw as selected if mouse isn't over the grid
            if (selected && !MouseInBox)
                selected = false;
#endif

            for (int Layer = 0; Layer < NumLayers; Layer++)
                DrawItem(i, pos, selected, Layer);
        }

        public virtual void DrawItem(int i, Vector2 pos, bool selected, int Layer)
        {
        }

        public void SetIndexViaAssociated(int index)
        {
            int CorrespondingIndex = AssociatedObjects.FindIndex(obj => (int)obj == index);
            if (CorrespondingIndex < 0) CorrespondingIndex = 0;
            SetIndex(CorrespondingIndex);
        }

        public int GetAssociatedIndex()
        {
            if (GetIndex() >= AssociatedObjects.Count)
                SetIndex(0);

            return (int)AssociatedObjects[GetIndex()];
        }

        public int GetIndex()
        {
            return ToIndex(J, K);
        }

        public void SetIndex(int i)
        {
            FromIndex(i, ref J, ref K);
            Scroll();
        }

        int ToIndex(int j, int k)
        {
            return k * Width + j;
        }
        void FromIndex(int Index, ref int j, ref int k)
        {
            k = (int)(Index / Width);
            j = Index - k * Width;
        }

        void Scroll()
        {
            if (K > HeightOffset + Height - 1)
                HeightOffset = K - Height + 1;
            if (K < HeightOffset)
                HeightOffset = K;
        }

        /// <summary>
        /// The length of time to delay phsx after the GridSelect has been created.
        /// </summary>
        public int DelayPhsxLength = 18;
        protected int PhsxCount = 0;

        /// <summary>
        /// Tracks how long the user has been continuously pushing the joystick.
        /// </summary>
        int MotionCount = 0;

#if PC_VERSION
        /// <summary>
        /// Whether the mouse is over the grid
        /// </summary>
        public bool MouseInBox = true;

        void UpdateMouseInBox()
        {
            // Always consider the mouse in the box if the mouse isn't in use
            if (!ButtonCheck.MouseInUse)
                MouseInBox = true;
            // Otherwise do an actual distance check
            else if (!CoreMath.Close(FancyPos.AbsVal, Tools.MouseGUIPos(MyCameraZoom), Size + new Vector2(150, 150)))
                MouseInBox = false;
        }
#endif

        public virtual void PhsxStep()
        {
#if PC_VERSION
            UpdateMouseInBox();

            if (Tools.TheGame.ShowMouse && ButtonCheck.MouseInUse)
                MouseInteract();
#endif

            float Sensitivity = ButtonCheck.ThresholdSensitivity;

            Vector2 Dir = ButtonCheck.GetDir(PlayerIndex);
            if (Inverted)
                Dir.Y *= -1;

            if (NoMoveCountX > 0) NoMoveCountX--;
            if (Math.Abs(Dir.X - PrevDir.X) > Sensitivity)
                //NoMoveCountX = 0;
                NoMoveCountX = NoMoveCountY = 0;
            if (NoMoveCountY > 0) NoMoveCountY--;
            if (Math.Abs(Dir.Y - PrevDir.Y) > Sensitivity)
                //NoMoveCountY = 0;
                NoMoveCountX = NoMoveCountY = 0;
            //if ((Dir - PrevDir).Length() > .5f) NoMoveCountX = NoMoveCountY = 0;

            if (Dir.Length() > Sensitivity)
                MotionCount++;
            else
                MotionCount = 0;

            bool Moved = false;

            // X motion
            if (NoMoveCountX == 0 || LastDirMoved == DirMoved.Vertical && NoMoveCountX < 3)
            {
                if (Dir.X > Sensitivity)
                {
                    Moved = true;
                    J++;

                    // Enforce boundary conditions
                    if (J >= Width)
                    {
                        if (LeftRightWrap) J = 0;
                        else { J = Width - 1; Moved = false; }
                    }
                }
                if (Dir.X < -Sensitivity)
                {
                    Moved = true;
                    J--;

                    // Enforce boundary conditions
                    if (J < 0)
                    {
                        if (LeftRightWrap) J = Width - 1;
                        else { J = 0; Moved = false; }
                    }
                }
                if (Moved)
                {
                    // If the user has been moving for a while, speed up motion
                    if (MotionCount > 1.75f * NoMoveDuration && LastDirMoved != DirMoved.Horizontal)
                        NoMoveCountY = NoMoveCountX = NoMoveDuration_Fast;
                    else
                        NoMoveCountY = NoMoveCountX = NoMoveDuration;

                    LastDirMoved = DirMoved.Horizontal;
                }
            }

            // Y motion
            if (NoMoveCountY == 0 || LastDirMoved == DirMoved.Horizontal && NoMoveCountY < 3)
            {
                if (!Moved)
                {
                    if (Dir.Y > Sensitivity)
                    {
                        K++;
                        Moved = true;

                        // Enforce boundary conditions
                        if (K >= FullHeight)
                        {
                            if (UpDownWrap) K = 0;
                            else { K = FullHeight - 1; Moved = false; }
                        }
                    }

                    if (Dir.Y < -Sensitivity)
                    {
                        K--;
                        Moved = true;

                        // Enforce boundary conditions
                        if (K < 0)
                        {
                            if (UpDownWrap) K = FullHeight - 1;
                            else { K = 0; Moved = false; }
                        }
                    }

                    if (Moved)
                    {
                        // If the user has been moving for a while, speed up motion
                        if (MotionCount > 1.75f * NoMoveDuration && LastDirMoved != DirMoved.Vertical)
                            NoMoveCountY = NoMoveCountX = NoMoveDuration_Fast;
                        else
                            NoMoveCountY = NoMoveCountX = NoMoveDuration;

                        LastDirMoved = DirMoved.Vertical;
                    }
                }
            }

            // Update indices and scroll if need
            if (Moved)
            {
                if (MoveSound != null)
                    MoveSound.Play();

                int i = ToIndex(J, K);
                if (i < 0) i = 0;
                if (i >= Count) i = Count - 1;
                FromIndex(i, ref J, ref K);

                Scroll();

                OnSelect();
            }
            PrevDir = Dir;

            AdditionalPhsx();

            PhsxCount++;
        }

        protected virtual void OnSelect()
        {
        }

        protected virtual void AdditionalPhsx()
        {
        }
    }
}