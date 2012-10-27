using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudberryKingdom
{
    class StartMenu_MW_Options : SoundMenu
    {
        public StartMenu_MW_Options(int Control, bool Centered)
            : base(Control, Centered)
        {
            CallDelay = ReturnToCallerDelay = 0;
        }

        public override void SlideIn(int Frames)
        {
            base.SlideIn(0);
            //MyPile.FadeIn(.1f);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }
    }
}