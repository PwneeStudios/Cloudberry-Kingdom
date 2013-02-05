using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CoreEngine;

namespace CloudberryKingdom
{
    public class MenuScrollBar : MenuSlider
    {
        public MenuScrollBar() : base(new EzText("", Resources.Font_Grobold42)) { }

        public EzTexture Normal, Held;
        protected override void InitializeSlider()
        {
            base.InitializeSlider();

			Normal = Tools.TextureWad.FindByName("Floater_Spikey_Forest");
			Held = Tools.TextureWad.FindByName("Floater_Spikey_Forest");

			
			//EndQuad = new QuadClass("Floater_Buzzsaw_Forest", 85, true);
			//StartQuad = new QuadClass("Floater_Buzzsaw_Forest", 85, true);
			EndQuad = new QuadClass("Firespinner_Base_Forest_1", 73, true);
			StartQuad = new QuadClass("Firespinner_Base_Forest_1", 73, true);
        }

        public Vector2 StartPlus = new Vector2(0, -600);
        public Vector2 EndPlus = new Vector2(0, 600);
        public bool DrawEnds = false;
        public override void Draw(bool Text, Camera cam, bool Selected)
        {
            Slider.Show = SliderBack.Show = false;
            base.Draw(Text, cam, Selected);

            if (MyMenu.CurDrawLayer != 0 || !Show) return;

            if (!Text)
            {
#if PC_VERSION
                if (Selected && Tools.Mouse.LeftButton == ButtonState.Pressed)
                    Slider.Quad.MyTexture = Held;
                else
                    Slider.Quad.MyTexture = Normal;
#else
                if (Selected)
                    Slider.Quad.MyTexture = Held;
                else
                    Slider.Quad.MyTexture = Normal;
#endif
				Slider.ScaleYToMatchRatio(100);

                Tools.QDrawer.DrawLine(Start + StartPlus, End + EndPlus,
                            new Color(255, 255, 255, 235),
                            85,
                            SliderBack.Quad.MyTexture, Tools.BasicEffect, 3000, 0, 0f);
                StartQuad.Pos = Start;
                EndQuad.Pos = End;

                StartQuad.Draw();
                EndQuad.Draw();
                Slider.Show = true; Slider.Draw();

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