using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class ChaosBackground : Background
    {
        public QuadClass BackgroundQuad;
        public QuadClass Stars;

        public ChaosBackground()
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
            TR.X += 900;
            BL.X -= 450;

            base.AddSpan(BL, TR);

            float ModCloudSpeed = .235f, ModSpinVel = .033f;

            TR.X += 900;
            BL.X -= 450;

            //base.AddSpan(BL, TR);

            string[] textures = { InfoWad.GetStr("FallingBlock_Touched_Texture"), InfoWad.GetStr("FallingBlock_Falling_Texture"), "blue_small", "fading block", "SpikeyGuy", "checkpoint3", "CoinBlue" };

            BackgroundFloaterList NewList;
            float Pos;

            NewList = new BackgroundFloaterList();
            NewList.Parralax = .6f;
            Pos = BL.X;
            while (Pos < TR.X)
            {
                BackgroundFloater cloud = new BackgroundFloater(MyLevel, BL.X - 1700, TR.X + 400);
                cloud.Data.Position = new Vector2(Pos, MyLevel.Rnd.RndFloat(-2900, 2900));
                cloud.MyQuad.TextureName = Rnd.RandomItem(textures);

                cloud.MyQuad.Quad.SetColor(Tools.Gray(.945f));
                cloud.MyQuad.Size = new Vector2(300, 200) * MyLevel.Rnd.RndFloat(1, 2.3f);
                cloud.MyQuad.ScaleYToMatchRatio();
                cloud.Data.Velocity = new Vector2(MyLevel.Rnd.RndFloat(-55, -40), 0) * ModCloudSpeed;
                cloud.SpinVelocity = MyLevel.Rnd.RndFloat(.55f, .9f) * ModSpinVel;
                cloud.InitialUpdate();

                Pos += MyLevel.Rnd.RndFloat(800, 1400) * .2f;

                NewList.Floaters.Add(cloud);
            }
            NewList.Floaters.Sort((f1, f2) => f1.MyQuad.Size.X.CompareTo(f2.MyQuad.Size.X));
            MyCollection.Lists.Add(NewList);


            //NewList = new BackgroundFloaterList();
            //NewList.Parralax = .8f;
            //Pos = BL.X;
            //while (Pos < TR.X)
            //{
            //    BackgroundFloater cloud = new BackgroundFloater(MyLevel, BL.Y - 1700, TR.Y + 400);
            //    cloud.Data.Position = new Vector2(MyLevel.Rnd.RndFloat(-1800, 1800)*2.3f, Pos);
            //    cloud.MyQuad.TextureName = Tools.RandomItem(textures);

            //    cloud.MyQuad.Size = new Vector2(300, 200) * MyLevel.Rnd.RndFloat(1, 2);
            //    cloud.Data.Velocity = new Vector2(-55, 0) * ModCloudSpeed;
            //    cloud.SpinVelocity = MyLevel.Rnd.RndFloat(-2, 2) * ModSpinVel;
            //    cloud.InitialUpdate();

            //    Pos += MyLevel.Rnd.RndFloat(800, 1400) * .4f;

            //    NewList.Floaters.Add(cloud);
            //}
            //MyCollection.Lists.Add(NewList);

            MyCollection.SetLevel(MyLevel);


            // Dim everything
            DimAll(Light);
        }

        public bool StarsOnly = false;
        public float CamMod = 1f;
        public bool AllowShake = false;
        public override void Draw()
        {
            MyCollection.PhsxStep();

            base.Draw();

            Camera Cam = MyLevel.MainCamera;

            BackgroundQuad.Quad.SetColor(new Color(new Vector3(1, 1, 1) * Light * .4f));
            BackgroundQuad.FullScreen(Cam);
            Stars.Quad.SetColor(Color.White);
            Stars.FullScreen(Cam);
            if (AllowShake) Stars.Pos += Cam.ShakeOffset;

            Cam.SetVertexCamera();

            BackgroundQuad.Draw();

            Stars.Quad.UseGlobalIllumination = false;
            Stars.TextureName = "Stars";
            Stars.Draw();

            if (!StarsOnly)
            {
                Tools.QDrawer.Flush();
                MyCollection.Draw(CamMod);
                Tools.QDrawer.Flush();
            }

            Tools.QDrawer.Flush();
        }
    }
}
