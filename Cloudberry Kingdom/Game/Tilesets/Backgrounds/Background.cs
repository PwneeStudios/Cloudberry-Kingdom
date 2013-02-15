using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;
using CoreEngine.Random;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class BackgroundType
    {
        public static Dictionary<string, BackgroundTemplate>
            NameLookup = new Dictionary<string, BackgroundTemplate>(),
            PathLookup = new Dictionary<string, BackgroundTemplate>();

        public static BackgroundTemplate
            None = new BackgroundTemplate(),
            Random = new BackgroundTemplate(),
            
            _Sea = new BackgroundTemplate("sea", Background._code_Sea),
            _Sea_Rain = new BackgroundTemplate("sea_rain", Background._code_Sea, Background.AddRainLayer),
            _Hills = new BackgroundTemplate("hills", Background._code_Hills),
            _Hills_Rain = new BackgroundTemplate("hills_rain", Background._code_Hills, Background.AddRainLayer),
            _Forest = new BackgroundTemplate("forest", Background._code_Forest, Background.TurnOffSnow),
            _Forest_Snow = new BackgroundTemplate("forest_snow", Background._code_Forest, Background.TurnOnSnow),
            _Cloud = new BackgroundTemplate("cloud", Background._code_Cloud),
            _Cloud_Rain = new BackgroundTemplate("cloud_rain", Background._code_Cloud, Background.AddRainLayer),
            _Cave = new BackgroundTemplate("cave", Background._code_Cave),
            _Castle = new BackgroundTemplate("castle", Background._code_Castle);
        
        public static void AddTemplate(BackgroundTemplate template)
        {
            NameLookup.AddOrOverwrite(template.Name, template);
            PathLookup.AddOrOverwrite(template.File, template);
        }

        public static void Load(string path)
        {
            BackgroundTemplate template;
            if (PathLookup.ContainsKey(path))
            {
                template = PathLookup[path];
            }
            else
            {
                template = new BackgroundTemplate();

                var name = Tools.GetFileName(path);
                template.Name = name;
                template.File = path;

                AddTemplate(template);
            }

            template.MadeOfCode = false;
            template.MadeOfText = true;
        }
    }

    public class BackgroundTemplate
    {
        public string Name;
        public bool MadeOfCode = true;
        public bool MadeOfText = false;
        public string File = null;

        public Action<Background> Code;

        public BackgroundTemplate()
        {
        }

        public BackgroundTemplate(string Name, params Action<Background>[] Codes)
        {
            this.Name = this.File = Name;
            this.MadeOfCode = true;
            this.MadeOfText = false;
            
            foreach (var code in Codes)
                this.Code += code;

            BackgroundType.AddTemplate(this);
        }

        public Background MakeInstanceOf()
        {
            var b = new RegularBackground();
            b.MyTemplate = this;

            if (MadeOfCode || File == null) return b;

            return b;
        }
    }

    public class RegularBackground : Background
    {
        public BackgroundTemplate MyTemplate = null;

        public RegularBackground()
        {
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);
            TR = new Vector2(5000, 2000);
            BL = new Vector2(-2000, -2000);

            if (MyTemplate != null)
            {
                if (MyTemplate.MadeOfCode)
                    UseCode(MyTemplate, this);
                else if (MyTemplate.MadeOfText)
                    Load(MyTemplate.File);
            }
        }

        public override void Draw()
        {
            Tools.QDrawer.Flush();
            Camera Cam = MyLevel.MainCamera;
            Cam.SetVertexCamera();

#if DEBUG && INCLUDE_EDITOR
            if (Tools.background_viewer != null)
            {
                Tools.background_viewer.PreDraw();
                Tools.QDrawer.Flush();
            }
#endif

            if (MyLevel.IndependentDeltaT > 0)
                MyCollection.PhsxStep();

            MyCollection.Draw();

            Tools.QDrawer.Flush();
        }

        public override void DrawForeground()
        {
            Tools.QDrawer.Flush();
            Camera Cam = MyLevel.MainCamera;
            Cam.SetVertexCamera();

            MyCollection.Draw(1f, true);

            Tools.QDrawer.Flush();
        }
    }

    public partial class Background : ViewReadWrite
    {
        public int GuidCounter = 0;
        public int GetGuid()
        {
            if (GuidCounter <= 0) GuidCounter = 1;

            return GuidCounter++;
        }

        public float MyGlobalIllumination = 1f;
        public bool AllowLava = true;

        public Vector2 Wind = Vector2.Zero;

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
			MyCollection.Release();
			MyCollection = null;

			MyType = null;
			MyTileSet = null;

            MyLevel = null;
        }

        /// <summary>
        /// Reset the list to its start position.
        /// </summary>
        public void Reset()
        {
            if (MyCollection == null) return;

            MyCollection.Reset();
        }

        public static Background UseCode(BackgroundTemplate template, Background b)
        {
            b.MyCollection.Lists.Clear();

            template.Code(b);
            
            b.SetLevel(b.MyLevel);
            b.SetBackground(b);
            b.Reset();

            return b;
        }

        public static Background Get(string name)
        {
            return BackgroundType.NameLookup[name].MakeInstanceOf();
        }

        public static Background Get(BackgroundTemplate Type)
        {
            return Type.MakeInstanceOf();
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

        public virtual void SetBackground(Background b)
        {
            if (MyCollection != null)
                MyCollection.SetBackground(b);
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

        public static bool GreenScreen = false;
        static QuadClass TestQuad = new QuadClass();
        public static EzTexture TestTexture = null;

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

                if (TestTexture == null)
                {
                    //TestTexture = Tools.Texture("BGPlain");
                    TestTexture = Tools.Texture("11 hill_4");
                }
                TestQuad.Quad.MyTexture = TestTexture;

                TestQuad.Quad.SetColor(ColorHelper.GrayColor(.825f));
                TestQuad.FullScreen(Cam);
                TestQuad.ScaleXToMatchRatio();
            }
            
            Cam.SetVertexCamera();

            TestQuad.Draw();
        }

        public virtual void Draw()
        {
            Tools.QDrawer.GlobalIllumination = MyGlobalIllumination;
        }

        public virtual void DrawForeground() { }

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

        public override string[] GetViewables()
        {
            return new string[] { "MyGlobalIllumination", "AllowLava", "Light", "BL", "TR", "MyCollection", "GuidCounter" };
        }
        public override void Read(StreamReader reader)
        {
            MyCollection.Lists.Clear();

            base.Read(reader);

            SetLevel(MyLevel);
            SetBackground(this);
            Reset();
        }

        public void Save(string path)
        {
            var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);

            Write(writer);

            writer.Close();
            stream.Close();
        }

        public void Load(string path)
        {
            Tools.UseInvariantCulture();
            var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            var reader = new StreamReader(stream);

            Read(reader);

            reader.Close();
            stream.Close();
        }

        public void SetWeatherIntensity(float Intensity)
        {
            // Mod snow
            foreach (var l in MyCollection.Lists)
                if (l.Name.Contains("Snow"))
                {
                    l.Show = true;
                    foreach (var f in l.Floaters)
                    {
                        f.MyQuad.Alpha *= Intensity;
                        f.uv_speed *= 1;
                    }
                }

            // Mod rain
            foreach (var l in MyCollection.Lists)
                if (l.Name.Contains("Rain"))
                {
                    l.Show = true;
                    foreach (var f in l.Floaters)
                    {
                        f.MyQuad.Alpha *= Intensity;
                        f.uv_speed *= 1;
                    }
                }
        }
    }
}
