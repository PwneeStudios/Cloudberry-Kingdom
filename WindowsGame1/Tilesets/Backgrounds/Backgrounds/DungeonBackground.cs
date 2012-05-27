using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class DungeonBackground : Background
    {
        public QuadClass BackgroundQuad, SmokeQuad, SmokeQuad2;
        public Vector2 SmokeOffset, SmokeOffset2;

        public DungeonBackground()
        {
            MyType = BackgroundType.Dungeon;
            MyTileSet = TileSets.Dungeon;
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();

            BackgroundQuad.Quad.SetColor(InfoWad.GetColor("Inside2_BackgroundColor"));
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

            if (MyLevel.Geometry == LevelGeometry.Up || MyLevel.Geometry == LevelGeometry.Down)
                AddVerticalSpan(BL, TR);
            else
                MyCollection.FromInfoWad("Inside2", BL, TR, MyLevel);
        }

        public void AddVerticalSpan(Vector2 BL, Vector2 TR)
        {
            TR.X += 900;
            BL.X -= 450;

            BackgroundFloaterList NewList;
            float Pos;

            NewList = new BackgroundFloaterList();
            NewList.Parralax = .5f;
            Pos = BL.Y;
            while (Pos < TR.Y)
            {
                BackgroundFloater window = new BackgroundFloater_Stationary(MyLevel);//, BL.X - 1000, TR.X + 400);
                window.Data.Position = new Vector2(MyLevel.Rnd.RndFloat(-1500, 1500) * 1.5f, Pos);

                if (MyLevel.Rnd.RndBool())
                {
                    window.MyQuad.TextureName = "Inside2_window1";
                    window.MyQuad.Size = new Vector2(1500, 1000);
                }
                else
                {
                    window.MyQuad.TextureName = "Inside2_window2";
                    window.MyQuad.Size = new Vector2(1500, 1500);
                }

                window.MyQuad.Quad.SetColor(new Color(85, 85, 85, 255));

                Pos += MyLevel.Rnd.RndFloat(800, 1400) * 2.15f;

                NewList.Floaters.Add(window);
            }
            MyCollection.Lists.Add(NewList);

            MyCollection.SetLevel(MyLevel);
        }

        public override void Draw()
        {
            MyCollection.PhsxStep();

            base.Draw();

            Camera Cam = MyLevel.MainCamera;

            // Background
            BackgroundQuad.FullScreen(Cam);
            //BackgroundQuad.Base.Origin = Cam.Data.Position;

            Vector2 offset;
            Vector2 repeat = InfoWad.GetVec("Inside2_BackgroundRepeat");
            float Parralax = InfoWad.GetFloat("Inside2_BackgroundParralax");
            /*
            Vector2 offset = Parralax * repeat * (Cam.Data.Position + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            
            BackgroundQuad.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            BackgroundQuad.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            BackgroundQuad.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            BackgroundQuad.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;
            */
            BackgroundQuad.TextureParralax(Parralax, repeat, OffsetOffset, Cam);

            BackgroundQuad.Quad.U_Wrap = true;
            BackgroundQuad.Quad.V_Wrap = true;


            // Smoke
            //SmokeQuad.FullScreen(Cam);
            float repeatScale = .38f;
            float scale = SmokeQuad.WidthToScreenWidthRatio(Cam);
            SmokeQuad.Scale(repeatScale / scale);
            SmokeQuad.Base.Origin.X = Cam.EffectivePos.X;//.Data.Position.X;
            SmokeQuad.Base.Origin.Y = (Cam.EffectiveBL.Y + SmokeQuad.Base.e2.Y);

            repeat = InfoWad.GetVec("Inside2_SmokeRepeat");
            Parralax = InfoWad.GetFloat("Inside2_SmokeParralax");
            SmokeOffset.X += .7f * InfoWad.GetFloat("Inside2_SmokeSpeed");
            /*
            offset = Parralax * repeat * (Cam.Data.Position + SmokeOffset + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            SmokeQuad.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            SmokeQuad.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            SmokeQuad.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            SmokeQuad.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;
            */
            SmokeQuad.TextureParralax(Parralax, repeat, SmokeOffset + OffsetOffset - new Vector2(0, 1000f / Parralax), Cam);

            SmokeQuad.Quad.U_Wrap = true;
            SmokeQuad.Quad.V_Wrap = false;

            // Smoke2
            //SmokeQuad2.FullScreen(Cam);
            scale = SmokeQuad.WidthToScreenWidthRatio(Cam);
            SmokeQuad.Scale(repeatScale / scale);

            SmokeQuad2.Base.Origin.X = Cam.EffectivePos.X;//.Data.Position.X;
            SmokeQuad2.Base.Origin.Y = (Cam.EffectiveBL.Y + SmokeQuad2.Base.e2.Y);

            repeat = InfoWad.GetVec("Inside2_SmokeRepeat");
            Parralax = InfoWad.GetFloat("Inside2_SmokeParralax");
            SmokeOffset2.X += InfoWad.GetFloat("Inside2_SmokeSpeed") * .8f;
            /*
            offset = Parralax * repeat * (Cam.Data.Position + SmokeOffset2 + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            SmokeQuad2.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            SmokeQuad2.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            SmokeQuad2.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y - .2f) + offset;
            SmokeQuad2.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y - .2f) + offset;
            */
            SmokeQuad2.TextureParralax(Parralax, repeat, SmokeOffset2 + OffsetOffset - new Vector2(0, 1000f / Parralax - .4f * 1000f / Parralax / repeat.Y), Cam);

            SmokeQuad2.Quad.U_Wrap = true;
            SmokeQuad2.Quad.V_Wrap = false;

            Cam.SetVertexCamera();
            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);

            BackgroundQuad.Base.Origin = Cam.EffectivePos;// Cam.Data.Position;
            BackgroundQuad.Draw();
                        
            Tools.QDrawer.Flush();

            //MyCollection.Draw();
            MyCollection.DrawLayer(0);

            Tools.QDrawer.Flush();
            Cam.SetVertexCamera();
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            SmokeQuad2.Draw();
            Tools.QDrawer.Flush();

            MyCollection.DrawLayer(1);

            Tools.QDrawer.Flush();
            Cam.SetVertexCamera();
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            SmokeQuad.Draw();
            Tools.QDrawer.Flush();

            MyCollection.FinishDraw();

            Tools.QDrawer.Flush();
        }
    }
}
