using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GrayBackground : Background
    {
        public QuadClass BackgroundQuad, BackgroundQuad2;
        
        public GrayBackground()
        {
            MyType = BackgroundType.Gray;
            //MyTileSet = TileSets.Get(TileSet.Cement);
            MyTileSet = TileSets.Get(TileSet.DarkTerrace);
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();
            BackgroundQuad.SetToDefault();
            BackgroundQuad.SetTexture("GrayBackground");
            BackgroundQuad.Quad.U_Wrap = BackgroundQuad.Quad.V_Wrap = true;
            //BackgroundQuad.Quad.SetColor(new Color(new Vector3(.5f, .5f, .5f) * .4f));

            BackgroundQuad2 = new QuadClass();
            BackgroundQuad2.SetToDefault();
            BackgroundQuad2.SetTexture("GrayBackground2");
            BackgroundQuad2.Quad.U_Wrap = BackgroundQuad2.Quad.V_Wrap = true;
            BackgroundQuad2.Quad.SetColor(new Color(new Vector3(1f, 1f, 1f) * .75f));

            Light = .2f;
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            TR = new Vector2(2400, 1000);
            BL = new Vector2(2400, 1000);
            //TR += new Vector2(2400, 1000);
            //BL -= new Vector2(2400, 1000);

            base.AddSpan(BL, TR);

            // Layers
            for (int i = 2; i >= 0; i--)
            {
                BackgroundFloaterList NewList = new BackgroundFloaterList();
                NewList.MyLevel = MyLevel;
                NewList.Parralax = .83f - .17f * i;

                Vector2 Pos = BL;
                Pos.X -= 650;
                while (Pos.X < TR.X + 650)
                {
                    Pos.Y = BL.Y;
                    while (Pos.Y < TR.Y)
                    {
                        BackgroundFloater floater = new BackgroundFloater(MyLevel, BL.X, TR.X);
                        floater.Data.Position = Pos + new Vector2(MyLevel.Rnd.RndFloat(-300, 300), MyLevel.Rnd.RndFloat(-300, 300));
                        floater.MyQuad.Scale(330);
                        floater.MyQuad.Base.e2.Y *= .7f;
                        //floater.MyQuad.SetTexture("Cloud" + (MyLevel.Rnd.RndInt(1, 6)).ToString());
                        floater.MyQuad.SetTexture("BlackCloud");
                        //floater.MyQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * .4f));
                        //floater.MyQuad.ScaleXToMatchRatio();                    

                        floater.Data.Velocity.X = -19;// MyLevel.Rnd.RndFloat(-10, 10);

                        NewList.Floaters.Add(floater);

                        Pos.Y += 930;
                    }
                    Pos.X += 930;
                }
                MyCollection.Lists.Add(NewList);
            }
        }

        public override void Draw()
        {
            MyCollection.PhsxStep();

            base.Draw();            

            Camera Cam = MyLevel.MainCamera;

            //BackgroundQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Light));
            BackgroundQuad.FullScreen(Cam);
            float Parralax = .37f;
            Vector2 repeat = new Vector2(1.6f, 1.5f);
            Vector2 shift = new Vector2(17 * MyLevel.CurPhsxStep, 0);
            Vector2 offset = Parralax * repeat * (Cam.Data.Position + shift + OffsetOffset) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            BackgroundQuad.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            BackgroundQuad.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            BackgroundQuad.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            BackgroundQuad.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;

            BackgroundQuad2.FullScreen(Cam);
            Parralax = .44f;
            repeat = new Vector2(1.6f, 1.5f);
            shift = new Vector2(17 * MyLevel.CurPhsxStep, 0);
            offset = Parralax * repeat * (Cam.Data.Position + shift) / (Cam.TR - Cam.BL);
            offset.Y *= -1;
            BackgroundQuad2.Quad.v0.Vertex.uv = new Vector2(0, 0) + offset;
            BackgroundQuad2.Quad.v1.Vertex.uv = new Vector2(repeat.X, 0) + offset;
            BackgroundQuad2.Quad.v2.Vertex.uv = new Vector2(0, repeat.Y) + offset;
            BackgroundQuad2.Quad.v3.Vertex.uv = new Vector2(repeat.X, repeat.Y) + offset;


            Cam.SetVertexCamera();
            //Vector4 HoldCameraPos = Tools.EffectWad.CameraPosition;
            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);

            BackgroundQuad.Base.Origin = Cam.Data.Position;



            BackgroundQuad.Draw();
            BackgroundQuad2.Draw();

            MyCollection.Draw();

            Tools.QDrawer.Flush();
            Cam.SetVertexCamera();
            //Tools.EffectWad.SetCameraPosition(HoldCameraPos);
        }
    }
}
