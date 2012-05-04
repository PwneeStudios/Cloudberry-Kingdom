using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class NightSkyBackground : Background
    {
        public QuadClass BackgroundQuad;
        public QuadClass Stars;
        
        public NightSkyBackground()
        {
            AllowLava = false;

            //Tools.QDrawer.GlobalIllumination = .5f;
            MyGlobalIllumination = .5f;
            Light = .4f;
            MyType = BackgroundType.NightSky;
            MyTileSet = TileSets.Get(TileSet.Island);
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();

            BackgroundQuad.Quad.SetColor(new Color(255, 255, 255));
            BackgroundQuad.Quad.TextureName = "bg";
            BackgroundQuad.Quad.MyEffect = Tools.BasicEffect;


            Stars = new QuadClass();

            Stars.Quad.SetColor(new Color(255, 255, 255));
            Stars.Quad.TextureName = "Stars";
            Stars.Quad.MyEffect = Tools.BasicEffect;
        }

        public float ModCloudSpeed = .735f;
        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            if (MyLevel.Geometry == LevelGeometry.Up || MyLevel.Geometry == LevelGeometry.Down)
            {
                OutsideBackground.AddVerticalSpan(BL, TR, this);

                foreach (var c in MyCollection.Lists)
                    foreach (var fl in c.Floaters)
                        fl.MyQuad.Quad.MySetColor.A /= 2;
            }
            else
                AddHorizontalSpan(BL, TR);

            // Dim everything
            foreach (var c in MyCollection.Lists)
                foreach (var fl in c.Floaters)
                {
                    Vector4 clr = fl.MyQuad.Quad.MySetColor.ToVector4();
                    clr *= Light;
                    clr.W = fl.MyQuad.Quad.MySetColor.ToVector4().W;
                    fl.MyQuad.Quad.SetColor(clr);
                }
        }

        void AddHorizontalSpan(Vector2 BL, Vector2 TR)
        {
            TR.X += 900;
            BL.X -= 450;

            base.AddSpan(BL, TR);

            BackgroundFloaterList NewList;
            float Pos;

            NewList = new BackgroundFloaterList();
            NewList.Parralax = .6f;
            Pos = BL.X;
            while (Pos < TR.X)
            {
                BackgroundFloater cloud = new BackgroundFloater(MyLevel, BL.X, TR.X);
                cloud.Data.Position = new Vector2(Pos, MyLevel.Rnd.RndFloat(-1600, 1400));
                cloud.MyQuad.TextureName = "cloud1";

                cloud.MyQuad.Quad.SetColor(Tools.Gray(.945f));
                cloud.MyQuad.Size = new Vector2(300, 200);
                cloud.Data.Velocity = new Vector2(-55, 0) * ModCloudSpeed;
                cloud.InitialUpdate();

                Pos += MyLevel.Rnd.RndFloat(800, 1400);

                NewList.Floaters.Add(cloud);
            }
            MyCollection.Lists.Add(NewList);

            NewList = new BackgroundFloaterList();
            NewList.Parralax = .8f;
            Pos = BL.X;
            while (Pos < TR.X)
            {
                BackgroundFloater cloud = new BackgroundFloater(MyLevel, BL.X, TR.X);
                cloud.Data.Position = new Vector2(Pos, MyLevel.Rnd.RndFloat(-1250, 1000));
                cloud.MyQuad.TextureName = "cloud1";
                cloud.MyQuad.Size = new Vector2(300, 200);
                cloud.Data.Velocity = new Vector2(-55, 0) * ModCloudSpeed;
                cloud.InitialUpdate();

                Pos += MyLevel.Rnd.RndFloat(800, 1400);

                NewList.Floaters.Add(cloud);
            }
            MyCollection.Lists.Add(NewList);

            MyCollection.SetLevel(MyLevel);
        }

        public override void Draw()
        {
            MyLevel.MainCamera.Oscillating = true;
            MyCollection.PhsxStep();

            base.Draw();

            // Wind
            Wind = SkyBackground.SkyWind(MyLevel.CurPhsxStep);

            Camera Cam = MyLevel.MainCamera;

            BackgroundQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Light));
            BackgroundQuad.FullScreen(Cam);
            Stars.Quad.SetColor(Color.White);
            Stars.FullScreen(Cam);

            Cam.SetVertexCamera();

            BackgroundQuad.Draw();

            Stars.Quad.UseGlobalIllumination = false;
            Stars.TextureName = "Stars";
            Stars.Draw();

            MyCollection.Draw();

            Tools.QDrawer.Flush();
        }
    }
}
