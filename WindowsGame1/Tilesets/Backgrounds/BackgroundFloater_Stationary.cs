using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundFloater_Stationary : BackgroundFloater
    {
        public BackgroundFloater_Stationary(Level level)
            : base(level)
        {
        }

        public BackgroundFloater_Stationary(Level level, string Root)
            : base(level, Root)
        {
        }

        public override void PhsxStep(BackgroundFloaterList list)
        {
        }

        public override void Draw()
        {
            Tools.QDrawer.DrawQuad(ref MyQuad.Quad);

#if DEBUG && INCLUDE_EDITOR
            Draw_DebugExtra();
#endif
        }
    }
}