using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    class StartMenu_Clouds_Exit : VerifyQuitGameMenu2
    {
        public StartMenu_Clouds_Exit(int Control)
            : base(Control)
        {
            CallDelay = ReturnToCallerDelay = 0;

            var _q = MyPile.FindQuad("Backdrop"); if (_q != null)
            {
                _q.TextureName = "MediumBox";
            }
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
        }

        protected override void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Yes"); if (_item != null) { _item.SetPos = new Vector2(677.7778f, 430.4445f); _item.MyText.Scale = 0.7628335f; _item.MySelectedText.Scale = 0.7628335f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("No"); if (_item != null) { _item.SetPos = new Vector2(702.7778f, 197.1111f); _item.MyText.Scale = 0.73975f; _item.MySelectedText.Scale = 0.73975f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-510.7157f, -268.6508f);

            Text _t;
            _t = MyPile.FindText(""); if (_t != null) { _t.Pos = new Vector2(-26.18762f, 394.5072f); _t.Scale = 0.7095835f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(44.44434f, 30.5556f); _q.Size = new Vector2(1687.076f, 848.1268f); }
            _q = MyPile.FindQuad("Berry"); if (_q != null) { _q.Pos = new Vector2(-416.6666f, -13.8889f); _q.Size = new Vector2(398.1559f, 537.0001f); }

            MyPile.Pos = new Vector2(-100.0012f, -4.761917f);
        }
    }
}