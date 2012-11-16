using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Black : CkBaseMenu
    {
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

        public StartMenu_MW_Black() : base()
        {
            Core.DrawLayer++;
        }

        QuadClass Black;
        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            EnsureFancy();

            //Black = new QuadClass("BlackSwipe");
            Black = new QuadClass("BlackSwipe_Vertical");
            Black.ScaleXToMatchRatio(1000);
            Black.SizeX *= 1.35f;
            MyPile.Add(Black, "Black");

            MyPile.Alpha = 0;

            BlackBox();
        }

        void BlackBox()
        {

        }

        public void SlideFromRight()
        {
            MyPile.Alpha = 1f;

            float scale = 1.3f;
            Black.SizeX = 4700 * scale;
            Black.Pos = new Vector2(9500 * scale, 0);
            Black.FancyPos.LerpTo(new Vector2(-6500 * scale, 0), 50);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}