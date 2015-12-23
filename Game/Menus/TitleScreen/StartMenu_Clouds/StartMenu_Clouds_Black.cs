namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Black : CkBaseMenu
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

        public StartMenu_Clouds_Black() : base()
        {
            Core.DrawLayer++;
        }

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            EnsureFancy();
            MyPile.Alpha = 0;

            BlackBox();
        }

        void BlackBox()
        {

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