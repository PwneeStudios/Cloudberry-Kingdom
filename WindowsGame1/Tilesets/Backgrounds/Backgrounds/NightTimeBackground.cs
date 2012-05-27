using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class NightTimeBackground : Background
    {
        public QuadClass BackgroundQuad;
        public QuadClass Stars;
        
        public NightTimeBackground()
        {
            //Tools.QDrawer.GlobalIllumination = .5f;
            MyGlobalIllumination = .5f;
            Light = .4f;
            MyType = BackgroundType.Night;
            MyTileSet = TileSets.Terrace;
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

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            if (MyLevel.Geometry == LevelGeometry.Up || MyLevel.Geometry == LevelGeometry.Down)
            {
                BL += new Vector2(-7000, 0);
                TR += new Vector2(7000, 0);
            }

            TR.X += 900;
            BL.X -= 450;

            base.AddSpan(BL, TR);

            MyCollection.FromInfoWad("Outside", BL, TR, MyLevel);

            // Dim everything
            DimAll(Light);
        }

        public override void Draw()
        {
            MyCollection.PhsxStep();

            base.Draw();            

            Camera Cam = MyLevel.MainCamera;

            BackgroundQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Light * .4f));
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
