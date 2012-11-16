using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BackgroundFloater_Stationary : BackgroundFloater
    {
        public BackgroundFloater_Stationary(Level level)
            : base(level)
        {
        }

        public override void PhsxStep(BackgroundFloaterList list)
        {
        }

        public override void Draw()
        {
            Tools.QDrawer.DrawQuad(ref MyQuad.Quad);
        }
    }
}