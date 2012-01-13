using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class SpaceBackground : Background
    {
        public QuadClass BackgroundQuad, SmokeQuad, SmokeQuad2;
        public Vector2 SmokeOffset, SmokeOffset2;

        public SpaceBackground()
        {
            MyType = BackgroundType.Castle;
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();

            BackgroundQuad.Quad.SetColor(Color.Black);
            BackgroundQuad.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Inside2_BackgroundTexture"));
            BackgroundQuad.Quad.MyEffect = Tools.BasicEffect;
            
            SmokeQuad = new QuadClass();
            SmokeQuad.Quad.V_Wrap = false;
            SmokeQuad.Quad.U_Wrap = true;
            SmokeQuad.Quad.SetColor(InfoWad.GetColor("Inside2_SmokeColor"));
            SmokeQuad.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Inside2_SmokeTexture"));
            SmokeQuad.Quad.MyEffect = Tools.BasicEffect;
            SmokeQuad.ScaleToTextureSize();
            SmokeQuad.Base.e1.X *= 4;
            SmokeQuad.Base.e2.Y *= 1;

            SmokeQuad2 = new QuadClass();
            SmokeQuad2.Quad.V_Wrap = false;
            SmokeQuad2.Quad.U_Wrap = true;
            SmokeQuad2.Quad.SetColor(InfoWad.GetColor("Inside2_SmokeColor"));
            SmokeQuad2.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Inside2_SmokeTexture"));
            SmokeQuad2.Quad.MyEffect = Tools.BasicEffect;
            SmokeQuad2.ScaleToTextureSize();
            SmokeQuad2.Base.e1.X *= 4;
            SmokeQuad2.Base.e2.Y *= 1;
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            base.AddSpan(BL, TR);

            //MyCollection.FromInfoWad("Inside2", BL, TR, MyLevel);
        }

        public override void Draw()
        {
            //MyCollection.PhsxStep();

            base.Draw();

            Camera Cam = MyLevel.MainCamera;

            // Background
            BackgroundQuad.FullScreen(Cam);
            BackgroundQuad.Base.Origin = Cam.Data.Position;

            Vector2 repeat = InfoWad.GetVec("Inside2_BackgroundRepeat");
            float Parralax = InfoWad.GetFloat("Inside2_BackgroundParralax");
            Vector2 offset = Parralax * repeat * (Cam.Data.Position + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            BackgroundQuad.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            BackgroundQuad.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            BackgroundQuad.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            BackgroundQuad.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;

            BackgroundQuad.Quad.U_Wrap = true;
            BackgroundQuad.Quad.V_Wrap = true;


            // Smoke
            //SmokeQuad.FullScreen(Cam);
            SmokeQuad.Base.Origin.X = Cam.Data.Position.X;
            SmokeQuad.Base.Origin.Y = (Cam.BL.Y + SmokeQuad.Base.e2.Y);

            repeat = InfoWad.GetVec("Inside2_SmokeRepeat");
            Parralax = InfoWad.GetFloat("Inside2_SmokeParralax");
            SmokeOffset.X += InfoWad.GetFloat("Inside2_SmokeSpeed");
            offset = Parralax * repeat * (Cam.Data.Position + SmokeOffset + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            SmokeQuad.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            SmokeQuad.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            SmokeQuad.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            SmokeQuad.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;

            SmokeQuad.Quad.U_Wrap = true;
            SmokeQuad.Quad.V_Wrap = false;

            // Smoke2
            //SmokeQuad2.FullScreen(Cam);
            SmokeQuad2.Base.Origin.X = Cam.Data.Position.X;
            SmokeQuad2.Base.Origin.Y = (Cam.BL.Y + SmokeQuad2.Base.e2.Y);

            repeat = InfoWad.GetVec("Inside2_SmokeRepeat");
            Parralax = InfoWad.GetFloat("Inside2_SmokeParralax");
            SmokeOffset2.X += InfoWad.GetFloat("Inside2_SmokeSpeed") * .8f;
            offset = Parralax * repeat * (Cam.Data.Position + SmokeOffset2 + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            SmokeQuad2.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            SmokeQuad2.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            SmokeQuad2.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y - .2f) + offset;
            SmokeQuad2.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y - .2f) + offset;

            SmokeQuad2.Quad.U_Wrap = true;
            SmokeQuad2.Quad.V_Wrap = false;

            Cam.SetVertexCamera();
            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);

            BackgroundQuad.Base.Origin = Cam.Data.Position;
            BackgroundQuad.Draw();
                        
            Tools.QDrawer.Flush();

            //MyCollection.Draw(0);

            Tools.QDrawer.Flush();
            Cam.SetVertexCamera();
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            SmokeQuad2.Draw();
            Tools.QDrawer.Flush();

            //MyCollection.Draw(1);

            Tools.QDrawer.Flush();
            Cam.SetVertexCamera();
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            SmokeQuad.Draw();
            Tools.QDrawer.Flush();

            //MyCollection.FinishDraw();

            Tools.QDrawer.Flush();
        }
    }
}
