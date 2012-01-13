using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Rain : GameObject
    {
        public QuadClass Full;
        public Rain()
        {
            Full = new QuadClass();
            Full.TextureName = "Rain";
            Full.EffectName = "Rain";
        }

        protected override void MyDraw()
        {
            if (!Core.Show) return;

            Level level = Core.MyLevel;
            Camera cam = level.MainCamera;

            Full.FullScreen(cam);
            Vector2 Repeat =
                             //new Vector2(10);
                             new Vector2(3.5f);
            float Angle = -10;

            float ModVel = .615f;
            Tools.RainEffect.effect.Parameters["Vel1"].SetValue(new Vector2(-.5f, -10f) * ModVel);
            Tools.RainEffect.effect.Parameters["Vel2"].SetValue(new Vector2(-1f, -12f) * ModVel);
            Tools.RainEffect.effect.Parameters["Vel3"].SetValue(new Vector2(-.75f, -13f) * ModVel);
            Tools.RainEffect.effect.Parameters["SkyColor"].SetValue(new Vector4(.3f, .3f, .3f, .7f));
            Tools.RainEffect.effect.Parameters["RainColor"].SetValue(new Vector4(.6f, .5f, .925f, .95f));

            Full.TextureParralax(1, Repeat, Vector2.Zero, cam);
            Tools.RotatedBasis(Angle, ref Full.Quad.v0.Vertex.uv);
            Tools.RotatedBasis(Angle, ref Full.Quad.v1.Vertex.uv);
            Tools.RotatedBasis(Angle, ref Full.Quad.v2.Vertex.uv);
            Tools.RotatedBasis(Angle, ref Full.Quad.v3.Vertex.uv);
            

            Full.Quad.U_Wrap = true;
            Full.Quad.V_Wrap = true;

            Full.Quad.SetColor(new Color(100,100,150,100));

            Full.Draw();
        }
    }
}