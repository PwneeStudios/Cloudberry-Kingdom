using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ScrollBar : StartMenuBase
    {
        LongMenu AttachedMenu;
        GUI_Panel Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            AttachedMenu = null;
            Parent = null;
        }

        public ScrollBar(LongMenu AttachedMenu, GUI_Panel Parent)
            : base(false)
        {
            this.AttachedMenu = AttachedMenu;
            this.Parent = Parent;
            this.Parent.OnRelease += () => Release();

            Constructor();
        }

        MenuSlider slider;
        public override void Init()
        {
            base.Init();

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.CheckForOutsideClick = false;
            MyMenu.AffectsOutsideMouse = false;

            MyMenu.OnB = null;
            MyMenu.MouseOnly = true;
            Control = -1;

            EnsureFancy();

            slider = new MenuScrollBar();
            slider.SliderBackSize *= new Vector2(1.15f, .72f);
            slider.CustomEndPoints = true;
            slider.CustomStart = new Vector2(0, -800);
            slider.CustomEnd = new Vector2(0, 800);
#if PC_VERSION
            slider.BL_HitPadding.X += 50;
            slider.TR_HitPadding.X += 50;
#endif
            slider.Slider.TextureName = "BouncyBlock1";
            slider.Slider.ScaleYToMatchRatio(90);
            slider.SliderBack.TextureName = "Chain_Tile";
            slider.TabOffset = new Vector2(0, 28);
            slider.MyFloat = new WrappedFloat(0, 0, 9);

            float height = AttachedMenu.Height();
            slider.MyFloat.GetCallback = () => height - AttachedMenu.FancyPos.RelVal.Y;
            slider.MyFloat.SetCallback = () =>
                AttachedMenu.FancyPos.RelVal = new Vector2(AttachedMenu.FancyPos.RelVal.X, height - slider.MyFloat.MyFloat);
            slider.MyFloat.MaxVal = height;
            slider.MyFloat.MinVal = 0;

            MyMenu.Add(slider);

            slider.Pos = slider.PosOffset = slider.SelectedPos = Vector2.Zero;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

#if WINDOWS
            slider.MyFloat.Val += Tools.DeltaScroll * .9f;// .68f;
#endif
        }

        public Vector2 BarPos
        {
            get { return slider.Pos; }
            set { slider.Pos = value; }
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            Pos.SetCenter(Parent.Pos);
            SlideIn(0);
        }
    }
}