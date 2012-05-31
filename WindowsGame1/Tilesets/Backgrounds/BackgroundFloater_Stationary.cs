using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundFloater_Stationary : BackgroundFloater
    {
        public BackgroundFloater_Stationary(Level level)
            : base(level, 0, 0)
        {
        }

        public BackgroundFloater_Stationary(Level level, string Root)
            : base(level, Root, 0, 0)
        {
        }

        public override void PhsxStep()
        {
        }

        public override void Draw()
        {
            Tools.QDrawer.DrawQuad(MyQuad.Quad);

#if DEBUG
            Draw_DebugExtra();
#endif
        }
    }
}