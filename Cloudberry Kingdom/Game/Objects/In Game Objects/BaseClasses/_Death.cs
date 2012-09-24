using System.Text;
using Microsoft.Xna.Framework;

using Drawing;

using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public abstract class _Death : _Obstacle
    {
        protected Bob.BobDeathType DeathType = Bob.BobDeathType.None;
    }
}
