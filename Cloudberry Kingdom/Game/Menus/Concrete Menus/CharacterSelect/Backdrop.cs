using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class CharSelectBackdrop : CkBaseMenu
    {
        public CharSelectBackdrop()
            : base()
        {
            //Core.DrawLayer += 1;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            if (Frames != 0) Frames = 20;
            base.SlideOut(PresetPos.Right, Frames);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            EnsureFancy();

            var Backdrop = new QuadClass("CharSelect_Backdrop", 1778);
            MyPile.Add(Backdrop, "Backdrop");

            var Frame = new QuadClass("CharSelect_Frame", 1778);
            MyPile.Add(Frame, "Frame");

            SetPos();
        }

        void SetPos()
        {
        }
    }
}