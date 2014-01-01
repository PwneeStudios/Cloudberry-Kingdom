using System.Text;
using Microsoft.Xna.Framework;

using CoreEngine;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class _Death : _Obstacle
    {
        protected BobDeathType DeathType = BobDeathType.None;
    }
}
