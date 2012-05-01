using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class RainBackground : OutsideBackground
    {
        public RainBackground()
        {
            MyType = BackgroundType.Rain;
            MyTileSet = TileSets.Get(TileSet.Terrace);
        }

        bool RainAdded = false;
        public override void Draw()
        {
            base.Draw();

            if (!RainAdded && MyLevel.MyGame != null)
            {
                MyLevel.MyGame.AddGameObject(new Rain());
                RainAdded = true;
            }
        }
    }
}
