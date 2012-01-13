using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class DiffSlider : StartMenuBase
    {
        public DiffSlider()
            : base(false)
        {
            Constructor();
        }

        public float Val
        {
            get { return slider.MyFloat.Val; }
            set { slider.MyFloat.Val = value;  }
        }

        MenuScrollBar slider;
        public override void Init()
        {
            base.Init();

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Active = false;
            MyMenu.CheckForOutsideClick = false;

            MyMenu.OnB = null;
            MyMenu.MouseOnly = true;
            Control = -1;

            EnsureFancy();

            slider = new MenuScrollBar();
            slider.Held = slider.Normal = Fireball.EmitterTexture;
            slider.Slider.Size = new Vector2(140);
            slider.EndPlus = slider.StartPlus = Vector2.Zero;
            slider.SliderBackSize *= 1.12f * new Vector2(1.15f, .72f);
            slider.CustomEndPoints = true;
            slider.CustomStart = new Vector2(-870, 0);
            slider.CustomEnd = new Vector2(870, 0);
#if PC_VERSION
            slider.BL_HitPadding.X += 50;
            slider.TR_HitPadding.X += 50;
#endif
            slider.SliderBack.TextureName = "Chain_Tile";
            slider.TabOffset = new Vector2(0, 0);//28);
            slider.MyFloat = new WrappedFloat(0, 0, 9);

            slider.MyFloat.MaxVal = 1;
            slider.MyFloat.MinVal = 0;

            MyMenu.Add(slider);

            MyMenu.Pos = new Vector2(-606.0605f, -702.0201f); 
            slider.Pos = slider.PosOffset = slider.SelectedPos = Vector2.Zero;
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

            SlideIn(0);
        }
    }
}