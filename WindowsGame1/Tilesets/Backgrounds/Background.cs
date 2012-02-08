using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public enum BackgroundType { None, Random, Castle, Dungeon, Outside, Space, Gray, Construct, Rain, Dark, Sky, Night, NightSky, Chaos };
    public class Background
    {
        public float MyGlobalIllumination = 1f;
        public bool AllowLava = true;

        public Vector2 Wind = Vector2.Zero;

        //public static string[] BackgroundName = { "None", "Castle", "Dungeon", "Terrace", "Space", "Seed world" };
        //public static bool[] AllowInCustomGame = { false, true    , true     , true     , false  , false };

        public Level MyLevel;

        public BackgroundType MyType;

        public BackgroundCollection MyCollection;
        public Vector2 OffsetOffset = Vector2.Zero; // How much the BackgroundQuad offset is offset by (from calls to Move)

        public float Light = 1;

        public Vector2 BL, TR;

        public TileSetInfo MyTileSet;

        public void Release()
        {
            MyLevel = null;
        }

        public static Background Get(BackgroundType Type)
        {
            Background NewBackground = null;

            switch (Type)
            {
                case BackgroundType.Castle: NewBackground = new CastleBackground(); break;

                case BackgroundType.Dungeon: NewBackground = new DungeonBackground(); break;
                case BackgroundType.Dark: NewBackground = new DarkBackground(); break;

                case BackgroundType.Sky: NewBackground = new SkyBackground(); break;
                case BackgroundType.Outside: NewBackground = new OutsideBackground(); break;
                case BackgroundType.Night: NewBackground = new NightTimeBackground(); break;
                case BackgroundType.NightSky: NewBackground = new NightSkyBackground(); break;
                case BackgroundType.Chaos: NewBackground = new ChaosBackground(); break;

                case BackgroundType.Space: NewBackground = new SpaceBackground(); break;

                case BackgroundType.Gray: NewBackground = new GrayBackground(); break;

                case BackgroundType.Construct: NewBackground = new ConstructBackground(); break;

                case BackgroundType.Rain: NewBackground = new RainBackground(); break;

                default: return null;
            }

            return NewBackground;
        }

        /// <summary>
        /// Get the tileset associated with a given background type
        /// </summary>
        public static TileSet GetTileSet(BackgroundType Type)
        {
            switch (Type)
            {
                case BackgroundType.Random: return TileSet.Random;

                case BackgroundType.Castle: return TileSet.Castle;

                case BackgroundType.Dungeon: return TileSet.Dungeon;
                case BackgroundType.Dark: return TileSet.Dark;

                case BackgroundType.NightSky:
                case BackgroundType.Sky: return TileSet.Island;

                case BackgroundType.Night:
                case BackgroundType.Rain:
                case BackgroundType.Outside: return TileSet.Terrace;

                case BackgroundType.Space: return TileSet.Castle;

                case BackgroundType.Gray:
                    return TileSet.Terrace;
                //return TileSet.Cement;
            }

            return TileSet.None;
        }

        public Background()
        {
            Tools.QDrawer.GlobalIllumination = 1f;
        }

        public virtual void Init(Level level)
        {
        }

        public virtual void Move(Vector2 shift)
        {
            OffsetOffset -= shift;

            if (MyCollection != null)
                MyCollection.Move(shift);

            TR += shift;
            BL += shift;
        }

        public virtual void SetLevel(Level level)
        {
            MyLevel = level;

            if (MyCollection != null)
                MyCollection.SetLevel(level);
        }

        public virtual void Absorb(Background background)
        {
            if (MyCollection != null && background.MyCollection != null)
                MyCollection.Absorb(background.MyCollection);
        }

        public void Clear()
        {
            FloatRectangle rect = new FloatRectangle();
            rect.TR = TR + new Vector2(10000, 10000);
            rect.BL = BL - new Vector2(10000, 10000);
            Clear(rect);
        }

        public virtual void Clear(FloatRectangle Area)
        {
            if (MyCollection != null)
                MyCollection.Clear(Area);
        }

        public virtual void ExtendRight(float RightBound)
        {
            if (RightBound > TR.X + 3500)
                AddSpan(new Vector2(TR.X + 1000, BL.X), new Vector2(TR.X + RightBound, TR.Y));
        }

        public virtual void ExtendLeft(float LeftBound)
        {
            if (LeftBound < BL.X - 3500)
                AddSpan(new Vector2(LeftBound, BL.X), new Vector2(BL.X - 1000, TR.Y));
        }

        public virtual void AddSpan(Vector2 BL, Vector2 TR)
        {
            if (TR == BL)
            {
                this.TR = TR;
                this.BL = BL;
            }
            else
            {
                this.TR = Vector2.Max(this.TR, TR);
                this.BL = Vector2.Min(this.BL, BL);
            }

            if (MyCollection != null)
                MyCollection.UpdateBounds(BL, TR);
        }

        public virtual void Draw()
        {
            Tools.QDrawer.GlobalIllumination = MyGlobalIllumination;
        }

        public void DimAll(float dim)
        {
            foreach (var c in MyCollection.Lists)
                foreach (var fl in c.Floaters)
                {
                    Vector4 clr = fl.MyQuad.Quad.MySetColor.ToVector4();
                    clr *= dim;
                    clr.W = fl.MyQuad.Quad.MySetColor.ToVector4().W;
                    fl.MyQuad.Quad.SetColor(clr);
                }
        }
    }
}
