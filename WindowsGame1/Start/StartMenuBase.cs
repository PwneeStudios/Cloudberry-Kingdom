using Microsoft.Xna.Framework;

using Drawing;

namespace CloudberryKingdom
{
    public class StartMenuBase : GUI_Panel
    {
        public static void GreenItem(MenuItem item)
        {
            item.MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        protected EzSound SelectSound, BackSound;

        public Vector2 ItemPos = new Vector2(-808, 110);
        protected Vector2 PosAdd = new Vector2(0, -151) * 1.181f;

        protected EzFont ItemFont = Tools.Font_DylanThin42;
        protected float FontScale = .72f;

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
            SelectSound = InfoWad.GetSound("Menu_Select_Sound");
            BackSound = InfoWad.GetSound("Menu_Back_Sound");

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

        public override void Call(GUI_Panel child, int Delay)
        {
            base.Call(child, Delay);

            if (SelectSound != null)
                SelectSound.Play();
        }

        public override void ReturnToCaller() { ReturnToCaller(true); }
        public virtual void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller();

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

        protected PresetPos SlideInFrom = PresetPos.Left;
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

        protected MenuItem MakeBackButton() { return MakeBackButton("Back"); }
        protected MenuItem MakeBackButton(string text)
        {
            MenuItem item;

            item = new MenuItem(new EzText(ButtonString.Back(86) + " " + text, ItemFont));
            item.Go = menuitem => MyMenu.OnB(MyMenu);
            AddItem(item);
            item.SelectSound = null;
            item.MySelectedText.MyFloatColor = InfoWad.GetColor("Menu_SelectedBackColor").ToVector4();
            item.MyText.MyFloatColor = InfoWad.GetColor("Menu_UnselectedBackColor").ToVector4();

            item.AdditionalOnSelect = () =>
                {
                    if (RightPanel == null) return;

                    BlurbBerry blurb = RightPanel as BlurbBerry;
                    if (null != blurb) blurb.SetText("=(");

                    DiffPics diffpics = RightPanel as DiffPics;
                    if (null != diffpics) diffpics.SetDiffPic(0);
                };

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

        public static PieceQuad MenuTemplate = PieceQuad.Get("DullMenu");
        protected void MakeBackdrop(Vector2 TR, Vector2 BL)
        {
            MakeBackdrop(MyMenu, TR, BL);
        }

        public static void SetBackdropProperties(PieceQuad piecequad)
        {
            piecequad.SetAlpha(.7f);
        }

        //public static int DefaultMenuLayer = Levels.Level.LastInLevelDrawLayer + 1;
        public static int DefaultMenuLayer = Levels.Level.LastInLevelDrawLayer;

        public StartMenuBase() { Core.DrawLayer = DefaultMenuLayer; }
        public StartMenuBase(bool CallBaseConstructor) : base(CallBaseConstructor) { Core.DrawLayer = DefaultMenuLayer; }
    }
}