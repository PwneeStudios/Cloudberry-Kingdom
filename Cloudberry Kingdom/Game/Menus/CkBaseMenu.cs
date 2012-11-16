using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class CkBaseMenu : GUI_Panel
    {
        QuadClass DarkBack;
        protected void MakeDarkBack()
        {
            // Make the dark back
            DarkBack = new QuadClass("White");
            DarkBack.Quad.SetColor(ColorHelper.GrayColor(.25f));
            DarkBack.Alpha = 0f;
            DarkBack.Fade(.1f); DarkBack.MaxAlpha = .5f;
            DarkBack.FullScreen(Tools.CurCamera);
            DarkBack.Pos = Vector2.Zero;
            DarkBack.Scale(5);
            MyPile.Add(DarkBack, "Dark");
        }

        protected EzSound SelectSound, BackSound;

        public Vector2 ItemPos = new Vector2(-808, 110);
        protected Vector2 PosAdd = new Vector2(0, -151) * 1.181f;

        protected EzFont ItemFont = Resources.Font_Grobold42;
        protected float FontScale = .75f;

        protected virtual void SetItemProperties(MenuItem item)
        {
            if (item.MyText == null) return;

            SetTextProperties(item.MyText);
            SetSelectedTextProperties(item.MySelectedText);
        }
        
        protected virtual void SetHeaderProperties(EzText text)
        {
            text.MyFloatColor = new Vector4(.6f, .6f, .6f, 1f);
            text.OutlineColor = new Vector4(0f, 0f, 0f, 1f);

            text.Shadow = true;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);

            text.Scale = FontScale * .9f;
        }

        /// <summary>
        /// Whether menu items added to the menu are given shadows
        /// </summary>
        protected bool ItemShadows = true;

        protected virtual void SetTextProperties(EzText text)
        {
            text.MyFloatColor = new Color(184, 231, 231).ToVector4();

            text.Scale = FontScale;

            text.Shadow = ItemShadows;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);
        }

        protected void SetSelectedTextProperties(EzText text)
        {
            text.MyFloatColor = new Color(246, 214, 33).ToVector4();
            //text.MyFloatColor = new Color(50, 220, 50).ToVector4();

            text.Scale = FontScale;

            text.Shadow = ItemShadows;
            text.ShadowColor = new Color(.2f, .2f, .2f, 1f);
            text.ShadowOffset = new Vector2(12, 12);
        }

        /// <summary>
        /// Amount a menu item is shifted when selected.
        /// </summary>
        //protected Vector2 SelectedItemShift = new Vector2(-65, 0);
        protected Vector2 SelectedItemShift = new Vector2(0, 0);

        protected virtual void AddItem(MenuItem item)
        {
            SetItemProperties(item);

            item.Pos = ItemPos;
            item.SelectedPos = ItemPos + SelectedItemShift;

            ItemPos += PosAdd;

            MyMenu.Add(item);
        }

        public override int SlideLength
        {
            set
            {
                base.SlideLength = value;
                if (TopPanel != null) TopPanel.SlideLength = value;
                if (RightPanel != null) RightPanel.SlideLength = value;
            }
        }

        public override void Init()
        {
            base.Init();

            // Sounds
            SelectSound = Menu.DefaultMenuInfo.Menu_Select_Sound;
            BackSound = Menu.DefaultMenuInfo.Menu_Back_Sound;

            // Delays
            Defaults();
        }

        public void DefaultDelays()
        {
            ReturnToCallerDelay = 40;
            CallDelay = 25;
        }

        public void DefaultSlides()
        {
            SlideLength = 38;
        }

        public void Defaults()
        {
            DefaultDelays();
            DefaultSlides();
        }

        public void NoDelays()
        {
            SlideLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;
        }

        public void FastDelays()
        {
            CallDelay = 18;
            ReturnToCallerDelay = 26;
        }

        public void FastSlides()
        {
            SlideLength = 33;
        }

        public void Fast()
        {
            FastDelays();
            FastSlides();
        }

        public void CategoryDelays()
        {
            ReturnToCallerDelay = 16;
            SlideInLength = 25;
            SlideOutLength = 24;

            CallDelay = 18;
        }

        /// <summary>
        /// When true this panel follows the following convention:
        /// When it calls a subpanel, this panel slides out to the left.
        /// When that subpanel returns control to this panel, this panel slides in from the left.
        /// </summary>
        protected bool CallToLeft = false;
        public override void Call(GUI_Panel child, int Delay)
        {
            base.Call(child, Delay);

            if (SelectSound != null)
                SelectSound.Play();

            if (CallToLeft)
            {
                Hide(PresetPos.Left);
            }
        }

        public override void OnReturnTo()
        {
            if (CallToLeft)
            {
                // Reset the menu's selected item's oscillate
                if (MyMenu != null) MyMenu.CurItem.OnSelect();

                // Activate and show the panel
                Active = true;

                if (!Hid) return;
                base.Show();
                this.SlideOut(PresetPos.Left, 0);
                this.SlideIn();
            }
            else
                base.OnReturnTo();
        }

        protected class ReturnToCallerProxy : Lambda
        {
            CkBaseMenu cbm;

            public ReturnToCallerProxy(CkBaseMenu cbm)
            {
                this.cbm = cbm;
            }

            public void Apply()
            {
                cbm.ReturnToCaller();
            }
        }

        protected class ReturnToCallerProxy1 : Lambda_1<MenuItem>
        {
            CkBaseMenu cbm;

            public ReturnToCallerProxy1(CkBaseMenu cbm)
            {
                this.cbm = cbm;
            }

            public void Apply(MenuItem dummy)
            {
                cbm.ReturnToCaller();
            }
        }

        public override void ReturnToCaller() { ReturnToCaller(true); }
        public virtual void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller();

            if (DarkBack != null)
                DarkBack.Fade(-.1f);

            if (PlaySound && BackSound != null)
                BackSound.Play();
        }

        GUI_Panel _RightPanel, _TopPanel;
        protected GUI_Panel RightPanel
        {
            set { _RightPanel = value; _RightPanel.CopySlideLengths(this); }
            get { return _RightPanel; }
        }
        protected GUI_Panel TopPanel
        {
            set { _TopPanel = value; _TopPanel.CopySlideLengths(this); }
            get { return _TopPanel; }
        }

        public override void OnAdd()
        {
            base.OnAdd();

            if (TopPanel != null) MyGame.AddGameObject(TopPanel);
            if (RightPanel != null) MyGame.AddGameObject(RightPanel);

            //SlideLength = 38;

            Show();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (TopPanel != null) TopPanel.Release();
            if (RightPanel != null) RightPanel.Release();
        }

        public PresetPos SlideInFrom = PresetPos.Left;
        public override void Show()
        {
            if (!Hid) return;

 	        base.Show();

            this.SlideOut(SlideInFrom, 0);

            this.SlideIn();
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);

            if (RightPanel != null)
                SlideIn_RightPanel(Frames);

            if (TopPanel != null)
            {
                TopPanel.SlideOut(PresetPos.Right, 0);
                TopPanel.SlideIn();
            }
        }

        protected virtual void SlideIn_RightPanel(int Frames)
        {
            RightPanel.SlideOut(PresetPos.Right, 0);
            RightPanel.SlideIn();
        }

        public override void SlideOut(GUI_Panel.PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);

            if (RightPanel != null) SlideOut_RightPanel(Preset, Frames);
            if (TopPanel != null) TopPanel.SlideOut(PresetPos.Right, Frames);
        }

        protected virtual void SlideOut_RightPanel(GUI_Panel.PresetPos Preset, int Frames)
        {
            RightPanel.SlideOut(PresetPos.Right, Frames);
        }

        protected PresetPos SlideOutTo = PresetPos.Left;
        public override void Hide() { Hide(SlideOutTo); }
        public virtual void Hide(PresetPos pos) { Hide(pos, -1); }
        public virtual void Hide(PresetPos pos, int frames)
        {
            base.Hide();

            if (frames == -1)
                this.SlideOut(pos);
            else
                this.SlideOut(pos, frames);
        }

        class MakeBackButtonHelper : Lambda_1<MenuItem>
        {
            CkBaseMenu bm;

            public MakeBackButtonHelper(CkBaseMenu bm)
            {
                this.bm = bm;
            }

            public void Apply(MenuItem menuitem)
            {
                bm.MyMenu.OnB.Apply(bm.MyMenu);
            }
        }

        protected MenuItem MakeBackButton() { return MakeBackButton(Localization.Words.Back); }
        protected MenuItem MakeBackButton(Localization.Words Word)
        {
            MenuItem item;

#if PC_VERSION
            //item = new MenuItem(new EzText(ButtonString.Back(86) + text, ItemFont));
            item = new MenuItem(new EzText(ButtonString.Back(86) + Localization.WordString(Word), ItemFont));
#else
            //item = new MenuItem(new EzText(ButtonString.Back(86) + " " + text, ItemFont));
            item = new MenuItem(new EzText(ButtonString.Back(86) + " " + Localization.WordString(Word)));
#endif

            item.Go = new MakeBackButtonHelper(this);
            item.Name = "Back";
            AddItem(item);
            item.SelectSound = null;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;

            return item;
        }

        public static void MakeBackdrop(Menu menu, Vector2 TR, Vector2 BL)
        {
            menu.MyPieceQuad = new PieceQuad();
            menu.MyPieceQuadTemplate = MenuTemplate;
            menu.TR = TR;
            menu.BL = BL;
            menu.ResetPieces();

            SetBackdropProperties(menu.MyPieceQuad);
        }

        public static PieceQuad MenuTemplate = null;
        protected void MakeBackdrop(Vector2 TR, Vector2 BL)
        {
            MakeBackdrop(MyMenu, TR, BL);
        }

        public static void SetBackdropProperties(PieceQuad piecequad)
        {
            piecequad.SetAlpha(.7f);
        }

        public static int DefaultMenuLayer = Level.LastInLevelDrawLayer;

        public CkBaseMenu() { Core.DrawLayer = DefaultMenuLayer; }
        public CkBaseMenu(bool CallBaseConstructor) : base(CallBaseConstructor) { Core.DrawLayer = DefaultMenuLayer; }

        public override void Draw()
        {
            base.Draw();

            if (DarkBack != null && !IsOnScreen)
                DarkBack.Draw();
        }
    }
}