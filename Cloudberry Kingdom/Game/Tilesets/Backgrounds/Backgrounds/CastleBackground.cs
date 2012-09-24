using System;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class CastleBackground : Background
    {
        public QuadClass BackgroundQuad;

        public CastleBackground()
        {
            //MyGlobalIllumination = .65f;
            //Light = .3f;

            MyType = BackgroundType.Castle;
            MyTileSet = TileSets.Castle;
        }

        public override void Init(Level level)
        {
            MyLevel = level;
            MyCollection = new BackgroundCollection(MyLevel);

            BackgroundQuad = new QuadClass();

            BackgroundQuad.Quad.SetColor(InfoWad.GetColor("Inside1_BackgroundColor"));
            BackgroundQuad.Quad.Illumination = Light;
            BackgroundQuad.Quad.MyTexture = Tools.TextureWad.FindByName(InfoWad.GetStr("Inside1_BackgroundTexture"));
            BackgroundQuad.Quad.MyEffect = Tools.BasicEffect;
            BackgroundQuad.Quad.U_Wrap = BackgroundQuad.Quad.V_Wrap = true;
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            base.AddSpan(BL, TR);

            if (MyLevel.Geometry == LevelGeometry.Up || MyLevel.Geometry == LevelGeometry.Down)
                AddVerticalSpan(BL, TR);
            else
                AddHorizontalSpan(BL, TR);

            SwapOutBalrogPics();
        }

        public void AddVerticalSpan(Vector2 BL, Vector2 TR)
        {
            TR.X += 900;
            BL.X -= 450;

            BackgroundFloaterList NewList;
            float Pos;

            NewList = new BackgroundFloaterList();
            NewList.Parallax = .5f;
            Pos = BL.Y;
            while (Pos < TR.Y)
            {
                if (MyLevel.Rnd.RndFloat() > .63f)
                //if (MyLevel.Rnd.RndBool())
                {
                    Vector2 pos = new Vector2(MyLevel.Rnd.RndFloat(-1500, 1500) * 1.5f, Pos);
                    RndFloater(ref BL, ref TR, NewList, ref pos);
                    Pos += MyLevel.Rnd.RndFloat(1400, 1550) * 2f;
                }
                else
                {
                    Vector2 pos = new Vector2(MyLevel.Rnd.RndFloat(-1500, -500) * 1.5f, Pos);
                    RndFloater(ref BL, ref TR, NewList, ref pos);
                    Pos += MyLevel.Rnd.RndFloat(0, 450) * 2f;

                    pos = new Vector2(MyLevel.Rnd.RndFloat(500, 1500) * 1.5f, Pos);
                    RndFloater(ref BL, ref TR, NewList, ref pos);
                    Pos += MyLevel.Rnd.RndFloat(1400, 1550) * 2f;
                }
            }
            MyCollection.Lists.Add(NewList);

            MyCollection.SetLevel(MyLevel);
        }

        private void RndFloater(ref Vector2 BL, ref Vector2 TR, BackgroundFloaterList NewList, ref Vector2 pos)
        {
            BackgroundFloater window = new BackgroundFloater_Stationary(MyLevel);//, BL.X - 1000, TR.X + 400);
            window.StartData.Position = pos;

            if (MyLevel.Rnd.RndFloat() > .6855f)
            {
                window.MyQuad.TextureName = "balrog portrait";
                window.MyQuad.Size = new Vector2(1200, 1380);
            }
            else
            {
                window.MyQuad.TextureName = "CastleInterior_Window";
                window.MyQuad.Size = new Vector2(1100, 810);
            }

            window.MyQuad.Quad.SetColor(new Color(120, 120, 140, 255));

            window.InitialUpdate();
            NewList.Floaters.Add(window);
        }

        void AddHorizontalSpan(Vector2 BL, Vector2 TR)
        {
            base.AddSpan(BL, TR);

            MyCollection.FromInfoWad("Inside1", BL, TR, MyLevel);
        }

        void SwapOutBalrogPics()
        {
            // Swap out one Balrog pic for a random one
            string BalrogPortraitName = "Balrog Portrait";
            bool FoundOne = false;
            foreach (BackgroundFloaterList list in MyCollection.Lists)
            {
                foreach (BackgroundFloater floater in list.Floaters)
                {
                    if (!FoundOne && string.Compare(BalrogPortraitName, floater.MyQuad.Quad.MyTexture.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    //if (!FoundOne && string.Compare(BalrogPortraitName, floater.MyQuad.Quad.MyTexture.Path, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        int i = MyLevel.Rnd.Rnd.Next(1, 6);
                        floater.MyQuad.Quad.MyTexture = Tools.TextureWad.FindByName("portrait" + i.ToString());
                        //FoundOne = true;
                    }
                }
            }

            DimAll(Light);
        }

        public override void Draw()
        {
            Tools.QDrawer.Flush();

            MyCollection.PhsxStep();

            base.Draw();

            Camera Cam = MyLevel.MainCamera;

            Cam.Update();
            BackgroundQuad.FullScreen(Cam);
            
            BackgroundQuad.Base.Origin = Cam.Data.Position;

            Vector2 repeat = InfoWad.GetVec("Inside1_BackgroundRepeat");
            float Parralax = InfoWad.GetFloat("Inside1_BackgroundParralax");
            BackgroundQuad.TextureParralax(Parralax, repeat, OffsetOffset, Cam);

            BackgroundQuad.Quad.U_Wrap = true;
            BackgroundQuad.Quad.V_Wrap = true;


            //BackgroundQuad.Draw();
            //Tools.QDrawer.Flush();


            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            //BackgroundQuad.Base.Origin = Cam.Data.Position;

            Cam.SetVertexCamera();
            BackgroundQuad.Base.Origin = Cam.EffectivePos;

            BackgroundQuad.Draw();

            MyCollection.Draw();

            Tools.QDrawer.Flush();
        }
    }
}
