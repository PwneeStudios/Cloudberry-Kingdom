namespace CloudberryKingdom
{
    class StartMenu_MW_Exit : VerifyQuitGameMenu2
    {
        public StartMenu_MW_Exit(int Control)
            : base(Control)
        {
            CallDelay = ReturnToCallerDelay = 0;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(Frames);
            //base.SlideIn(0);
            //MyPile.FadeIn(.1f);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, Frames);
            //base.SlideOut(Preset, 0);
        }
    }
}