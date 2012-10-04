using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Intense_Backpanel : CkBaseMenu
    {
        public override void Hide(PresetPos pos, int frames)
        {
            base.Hide(pos, frames);
            ButtonCheck.PreLogIn = false;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            EnsureFancy();

            // Title
            //MyPile.Add(new QuadClass("TitleScreen_3"), "TitleScreen");
            //MyPile.Add(new QuadClass("Title_3"), "Title");

            MyPile.Add(new QuadClass("TitleScreen_4"), "TitleScreen");
            MyPile.Add(new QuadClass("Title_4"), "Title");

            BlackBox();
        }

        void BlackBox()
        {
            QuadClass _q;
            _q = MyPile.FindQuad("TitleScreen"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1799.546f, 1012.247f); }
            _q = MyPile.FindQuad("Title"); if (_q != null) { _q.Pos = new Vector2(-25f, 88.88889f); _q.Size = new Vector2(1690.654f, 950.9934f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}