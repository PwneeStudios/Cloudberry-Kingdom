using System.Text;
using Microsoft.Xna.Framework;

using System.IO;

namespace CloudberryKingdom
{
    public abstract class _Death : _Obstacle
    {
        protected Bob.BobDeathType DeathType = Bob.BobDeathType.None;
    }
}
