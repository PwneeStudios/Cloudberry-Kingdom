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

            backdrop.Quad.RotateUV();
            backdrop.Size = new Vector2(488 + 21 - 1.5f, 1405f);
            MyPile.Pos = new Vector2(-10, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            DestinationScale = new Vector2(1.215f);
            SlideOut(PresetPos.Bottom, 0);
        }

        public override void ReturnToCaller(bool PlaySound)
        {
            base.ReturnToCaller(PlaySound);
        }
    }
}