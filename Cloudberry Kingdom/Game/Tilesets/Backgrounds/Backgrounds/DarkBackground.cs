using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class DarkBackground : DungeonBackground
    {
        const int Darken = 40;
        Color DarkenColor = new Color(Darken, Darken, Darken);

        public DarkBackground()
            : base()
        {
            MyType = BackgroundType.Dark;
            MyTileSet = TileSets.Dark;
        }

        public override void Init(Level level)
        {
            base.Init(level);

            BackgroundQuad.Quad.SetColor(DarkenColor);

            SmokeQuad.Quad.SetColor(DarkenColor);
            SmokeQuad2.Quad.SetColor(DarkenColor);
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            base.AddSpan(BL, TR);

            foreach (BackgroundFloaterList list in MyCollection.Lists)
                foreach (BackgroundFloater floater in list.Floaters)
                    floater.MyQuad.Quad.SetColor(DarkenColor);
        }
    }
}