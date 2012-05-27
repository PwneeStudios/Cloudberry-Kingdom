using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class ConstructBackground : Background
    {
        public QuadClass BackgroundQuad;

        public ConstructBackground()
        {
            MyType = BackgroundType.Construct;
            MyTileSet = TileSets.None;
        }

        public override void Init(Level level)
        {
            MyLevel = level;

            BackgroundQuad = new QuadClass();
            BackgroundQuad.SetToDefault();
            BackgroundQuad.Quad.SetColor(Color.Black);
        }

        public override void AddSpan(Vector2 BL, Vector2 TR)
        {
            base.AddSpan(BL, TR);
        }

        public override void Draw()
        {
            base.Draw();

            //Camera Cam = MyLevel.MainCamera;

            //// Background
            //BackgroundQuad.FullScreen(Cam);
            //Vector4 cameraPos = new Vector4(Cam.Data.Position.X, Cam.Data.Position.Y, Cam.Zoom.X, Cam.Zoom.Y);
            //Tools.EffectWad.SetCameraPosition(cameraPos);
            //BackgroundQuad.Base.Origin = Cam.Data.Position;
            //BackgroundQuad.Draw();
                        
            //Tools.QDrawer.Flush();
        }
    }
}
