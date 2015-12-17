namespace CloudberryKingdom
{
    class StartMenu_MW_Options : SoundMenu
    {
        public StartMenu_MW_Options(int Control, bool Centered)
            : base(Control, true)
        {
            CallDelay = ReturnToCallerDelay = 0;
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
    }
}