using Microsoft.Xna.Framework;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

using Drawing;

namespace CloudberryKingdom
{
    public class BackgroundType
    { 
        public static BackgroundTemplate
            None = new BackgroundTemplate(),
            Random = new BackgroundTemplate(),
            Castle = new BackgroundTemplate(),
            Dungeon = new BackgroundTemplate(),
            Outside = new BackgroundTemplate(),
            Space = new BackgroundTemplate(),
            Gray = new BackgroundTemplate(),
            Construct = new BackgroundTemplate(),
            Rain = new BackgroundTemplate(),
            Dark = new BackgroundTemplate(),
            Sky = new BackgroundTemplate(),
            Night = new BackgroundTemplate(),
            NightSky = new BackgroundTemplate(),
            Chaos = new BackgroundTemplate();

        public static Dictionary<string, BackgroundTemplate> NameLookup = new Dictionary<string, BackgroundTemplate>();
        public static void AddTemplate(BackgroundTemplate template)
        {
            NameLookup.Add(template.Name, template);
        }
    }

    public class BackgroundTemplate
    {
        public string Name;
        public bool MadeOfCode = true;

        public Background MakeInstanceOf()
        {
            var b = new Background();
            return b;
        }
    }

    public class Background
    {
        public float MyGlobalIllumination = 1f;
        public bool AllowLava = true;

        public Vector2 Wind = Vector2.Zero;

        //public static string[] BackgroundName = { "None", "Castle", "Dungeon", "Terrace", "Space", "Seed world" };
        //public static bool[] AllowInCustomGame = { false, true    , true     , true     , false  , false };

        public Level MyLevel;
        public Rand Rnd { get { return MyLevel.Rnd; } }

        public BackgroundTemplate MyType;

        public BackgroundCollection MyCollection;
        public Vector2 OffsetOffset = Vector2.Zero; // How much the BackgroundQuad offset is offset by (from calls to Move)

        public float Light = 1;

        public Vector2 BL, TR;

        public TileSet MyTileSet;

        public void Release()
        {
            MyLevel = null;
        }

        public static Background Get(BackgroundTemplate Type)
        {
            if (Type == BackgroundType.Castle) return new CastleBackground();

            if (Type == BackgroundType.Dungeon) return new DungeonBackground();
            if (Type == BackgroundType.Dark) return new DarkBackground();

            if (Type == BackgroundType.Sky) return new SkyBackground();
            if (Type == BackgroundType.Outside) return new OutsideBackground();
            if (Type == BackgroundType.Night) return new NightTimeBackground();
            if (Type == BackgroundType.NightSky) return new NightSkyBackground();
            if (Type == BackgroundType.Chaos) return new ChaosBackground();

            if (Type == BackgroundType.Space) return new SpaceBackground();

            if (Type == BackgroundType.Gray) return new GrayBackground();

            if (Type == BackgroundType.Construct) return new ConstructBackground();

            if (Type == BackgroundType.Rain) return new RainBackground();

            return null;
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
                //TestQuad.TextureName = "tigar_inside_castle";
                
                if (TestTexture == null)
                    TestTexture = Tools.Texture("BGPlain");
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
