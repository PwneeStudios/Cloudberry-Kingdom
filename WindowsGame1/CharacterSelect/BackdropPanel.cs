using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BackdropPanel : StartMenuBase
    {
        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public BackdropPanel(int Control, CharacterSelect Parent)
            : base(false)
        {
            this.Control = Control;
            this.Parent = Parent;

//#if NOT_PC
            this.AutoDraw = false;
//#endif
            Constructor();
        }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Backdrop
            QuadClass backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "score screen";
            MyPile.Add(backdrop);

#if PC_VERSION
            //backdrop.Size = new Vector2(1000.873f, 715.8236f);
            //backdrop.Pos = new Vector2(789.6825f, 3.051758E-05f);

            backdrop.Size = new Vector2(496.9023f, 1771.379f);
            backdrop.Pos = new Vector2(126.9872f, -59.52365f);

            // A second backdrop
            backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "score screen";
            MyPile.Add(backdrop);

            backdrop.Size = new Vector2(496.9023f, 1771.379f);
            backdrop.Pos = new Vector2(1063.495f, -115.0792f);
#else
            backdrop.Quad.RotateUV();
            backdrop.Size = new Vector2(488 + 21 - 1.5f, 1405f);
#endif
            MyPile.Pos = new Vector2(-10, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();

#if PC_VERSION
            //DestinationScale = new Vector2(1.575f);
            SlideOut(PresetPos.Bottom, 0);
            //SlideOut(PresetPos.Right, 0);
#else
            DestinationScale = new Vector2(1.215f);
            SlideOut(PresetPos.Bottom, 0);
#endif

        }

        public override void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller(PlaySound);
        }
    }
}