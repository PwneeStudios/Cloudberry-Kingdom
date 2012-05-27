using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

using Drawing;

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
        public Rand Rnd { get { return MyLevel.Rnd; } }

        public BackgroundType MyType;

        public BackgroundCollection MyCollection;
        public Vector2 OffsetOffset = Vector2.Zero; // How much the BackgroundQuad offset is offset by (from calls to Move)

        public float Light = 1;

        public Vector2 BL, TR;

        public TileSet MyTileSet;

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
        

        public static bool Test = false;
        public static bool GreenScreen = false;
        static QuadClass TestQuad = new QuadClass();
        public static EzTexture TestTexture = null;

//#if DEBUG
        public static void DrawTest()
        {
            Camera Cam = Tools.CurCamera;

            if (GreenScreen)
            {
                TestQuad.Quad.SetColor(new Color(new Vector3(0, 1, 0) * 1));
                TestQuad.TextureName = "White";
                TestQuad.FullScreen(Cam);
            }
            else
            {
                TestQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * 1));
                //TestQuad.TextureName = "BGPlain";
                TestQuad.Quad.MyTexture = TestTexture;
                TestQuad.Quad.SetColor(Tools.GrayColor(.825f));
                TestQuad.FullScreen(Cam);
                TestQuad.ScaleXToMatchRatio();
            }
            
            Cam.SetVertexCamera();

            TestQuad.Draw();
        }
//#endif

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
