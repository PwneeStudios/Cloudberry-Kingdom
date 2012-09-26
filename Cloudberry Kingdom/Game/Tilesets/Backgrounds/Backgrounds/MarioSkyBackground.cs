using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class MarioSkyBackground : Background
    {
        public QuadClass BackgroundQuad;

        public MarioSkyBackground()
        {
            AllowLava = false;

            MyType = BackgroundType.MarioSky;
            MyTileSet = TileSets.Island;
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();

            BackgroundQuad.Quad.SetColor(new Color(255, 255, 255));
            BackgroundQuad.Quad.TextureName = "background_mario3_outside";
            BackgroundQuad.Quad.MyEffect = Tools.BasicEffect;
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            TR.X += 900;
            BL.X -= 450;

            base.AddSpan(BL, TR);

            BackgroundFloaterList NewList;
            float Pos;

            NewList = new BackgroundFloaterList();
            NewList.Parallax = 1f;
            Pos = BL.X;
            while (Pos < TR.X)
            {
                BackgroundFloater cloud = new BackgroundFloater(MyLevel);
                cloud.StartData.Position = new Vector2(Pos, MyLevel.Rnd.RndFloat(-900, 900));
                cloud.MyQuad.TextureName = "cloud_" + Tools.GlobalRnd.RndInt(1, 2).ToString();

                //cloud.MyQuad.Quad.SetColor(ColorHelper.Gray(.945f));
                cloud.MyQuad.ScaleXToMatchRatio(130);
                //cloud.Data.Velocity = new Vector2(-55, 0);
                cloud.InitialUpdate();

                Pos += MyLevel.Rnd.RndFloat(800, 1400);

                NewList.Floaters.Add(cloud);
            }
            MyCollection.Lists.Add(NewList);

            MyCollection.SetLevel(MyLevel);
        }

        public override void Draw()
        {
            MyCollection.PhsxStep();

            base.Draw();            

            Camera Cam = MyLevel.MainCamera;

            BackgroundQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Light));
            BackgroundQuad.FullScreen(Cam);

            Cam.SetVertexCamera();

            BackgroundQuad.Draw();

            MyCollection.Draw();

            Tools.QDrawer.Flush();
        }
    }
}
