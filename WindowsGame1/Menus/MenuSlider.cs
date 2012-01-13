using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class MenuSlider : MenuSliderBase
    {
        public override string[] GetViewables()
        {
            return new string[] { "Pos", "SelectedPos", "!MyMenu", "SliderShift" };
        }

        public Vector2 SliderShift = new Vector2(716.666f, -172.2223f),
                       TabOffset = new Vector2(0, 0);
        
        public QuadClass SliderBack, Slider, StartQuad, EndQuad;

        public MenuSlider(EzText Text)
        {
            Init(Text, Text.Clone());
            InitializeSlider();
        }
        public MenuSlider(EzText Text, EzText SelectedText)
        {
            Init(Text, SelectedText);
            InitializeSlider();
        }

        protected override void  InitializeSlider()
        {
            base.InitializeSlider();

            SelectionOscillate = true;

#if PC_VERSION
            BL_HitPadding.X += 40;
            TR_HitPadding.X += 40;
#endif

            SliderBack = new QuadClass();
            SliderBack.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("SliderBack_Texture"));
            Vector2 Size = new Vector2(250, 35) * 1.35f;
            SliderBack.Base.e1 *= Size.X;
            SliderBack.Base.e2 *= Size.Y;

            Slider = new QuadClass();
            Slider.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Slider_Texture"));
            Size = new Vector2(28, 55) * 1.35f;
            Slider.Base.e1 *= Size.X;
            Slider.Base.e2 *= Size.Y;
        }

        public Vector2 SliderBackSize
        {
            get { return SliderBack.Base.GetScale(); }
            set { SliderBack.Base.SetScale(value); }
        }

        public Vector2 SliderSize
        {
            get { return Slider.Base.GetScale(); }
            set { Slider.Base.SetScale(value); }
        }

        public override float Height()
        {
            return .65f * 2 * SliderBack.Base.e2.Y;
        }

        public override float Width()
        {
            return MyText.GetWorldWidth() + 2 * SliderBack.Base.e1.X;
        }

        protected override void CalcEndPoints()
        {
            CalcRelEndPoints();

            Start = RelStart + Pos + PosOffset + SliderShift;
            End = RelEnd + Pos + PosOffset + SliderShift;
        }

        public bool CustomEndPoints = false;
        public Vector2 CustomStart, CustomEnd;

        Vector2 RelStart, RelEnd;
        void CalcRelEndPoints()
        {
            if (CustomEndPoints)
            {
                RelStart = CustomStart;
                RelEnd = CustomEnd;
            }
            else
            {
                float scale = SliderBackSize.X / (250 * 1.35f);
                RelStart = new Vector2(-210, 0) * 1.35f * scale;
                RelEnd = new Vector2(210, 0) * 1.35f * scale;
            }
        }

        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            MyCameraZoom = cam.Zoom;

            if (MyMenu.CurDrawLayer != 0 || !Show) return;

            // If just selected perfrom the OnSelect callback
            if (Selected != this.Selected && Selected)
            {
                OnSelect();
            }
            this.Selected = Selected;

            SetTextSelection(Selected);

            SliderBack.Base.Origin = Pos + PosOffset + SliderShift;

            CalcEndPoints();

            Slider.Base.Origin = Start + (MyFloat.Val - MyFloat.MinVal) / (MyFloat.MaxVal - MyFloat.MinVal) * (End - Start)
                                    + TabOffset;

            if (Text)
                DrawText(cam, Selected);

            if (!Text)
            {
                if (FancyPos != null)
                {
                    FancyPos.RelVal = Pos + PosOffset;
                    FancyPos.Update();
                }

                if (Icon != null)
                    Icon.Draw(Selected);

                SliderBack.Draw();
                Slider.Draw();
                Tools.QDrawer.Flush();

                if (Tools.DrawBoxes)
                {
                    Tools.QDrawer.DrawCircle(End, 5, Color.Red);
                    Tools.QDrawer.DrawCircle(Start, 5, Color.Red);
                }
            }
        }
    }
}