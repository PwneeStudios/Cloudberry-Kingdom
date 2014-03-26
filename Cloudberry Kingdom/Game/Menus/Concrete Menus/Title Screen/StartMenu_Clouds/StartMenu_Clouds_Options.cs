using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    class StartMenu_Clouds_Options : SoundMenu
    {
        public StartMenu_Clouds_Options(int Control, bool Centered)
            : base(Control, true)
        {
            CallDelay = ReturnToCallerDelay = 0;

            var _q = MyPile.FindQuad("Backdrop"); if (_q != null)
            {
                _q.TextureName = "MediumBox";
            }
        }

        public override void SlideIn(int Frames)
        {
            if (UseBounce)
                base.SlideIn(Frames);
            else
                base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (UseBounce)
                base.SlideOut(Preset, Frames);
            else
                base.SlideOut(Preset, 0);
        }

        protected override void SetPosition_PC()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Sound"); if (_item != null) { _item.SetPos = new Vector2(-169.048f, 729.2538f); _item.MyText.Scale = 0.6389169f; _item.MySelectedText.Scale = 0.6389169f; _item.SelectIconOffset = new Vector2(0f, 0f); ((MenuSlider)_item).SliderShift = new Vector2(1869.443f, -152.7778f); }
            _item = MyMenu.FindItemByName("Music"); if (_item != null) { _item.SetPos = new Vector2(-169.048f, 537.0638f); _item.MyText.Scale = 0.6699168f; _item.MySelectedText.Scale = 0.6389169f; _item.SelectIconOffset = new Vector2(0f, 0f); ((MenuSlider)_item).SliderShift = new Vector2(1869.443f, -136.1112f); }
            _item = MyMenu.FindItemByName("RezList"); if (_item != null) { _item.SetPos = new Vector2(1077.38f, 151.8088f); _item.MyText.Scale = 0.5901669f; _item.MySelectedText.Scale = 0.5901669f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("FullscreenToggle"); if (_item != null) { _item.SetPos = new Vector2(1092.856f, 93.03196f); _item.MyText.Scale = 0.5877503f; _item.MySelectedText.Scale = 0.5877503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("LanguageList"); if (_item != null) { _item.SetPos = new Vector2(1088.888f, -191.778f); _item.MyText.Scale = 0.594917f; _item.MySelectedText.Scale = 0.594917f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Controls"); if (_item != null) { _item.SetPos = new Vector2(-117.0643f, -385.2859f); _item.MyText.Scale = 0.5910001f; _item.MySelectedText.Scale = 0.5910001f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Custom"); if (_item != null) { _item.SetPos = new Vector2(-117.0643f, -530.254f); _item.MyText.Scale = 0.5785837f; _item.MySelectedText.Scale = 0.5785837f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("ResetStory"); if (_item != null) { _item.SetPos = new Vector2(-119.8421f, -670.1907f); _item.MyText.Scale = 0.6284168f; _item.MySelectedText.Scale = 0.6284168f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("AndersToggle"); if (_item != null) { _item.SetPos = new Vector2(1083.333f, -239.0005f); _item.MyText.Scale = 0.5811669f; _item.MySelectedText.Scale = 0.5811669f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-1010.712f, -63.09545f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-1072.62f, 968.3172f); _t.Scale = 0.864f; }
            _t = MyPile.FindEzText("Language"); if (_t != null) { _t.Pos = new Vector2(-1041.665f, -119.4448f); _t.Scale = 0.6748332f; }
            _t = MyPile.FindEzText("Anders"); if (_t != null) { _t.Pos = new Vector2(-1055.556f, -288.8889f); _t.Scale = 0.6584173f; }
            _t = MyPile.FindEzText("RezText"); if (_t != null) { _t.Pos = new Vector2(-1040.476f, 238.9516f); _t.Scale = 0.6868508f; }
            _t = MyPile.FindEzText("Fullscreen"); if (_t != null) { _t.Pos = new Vector2(-1040.475f, 63.95281f); _t.Scale = 0.7051834f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-110.3188f, 0.7938643f); _q.Size = new Vector2(2034.146f, 1349.452f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(1055.558f, -889.9993f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(29.76184f, 7.936525f);
        }
    }
}