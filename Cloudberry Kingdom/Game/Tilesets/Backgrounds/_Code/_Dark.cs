using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void AddDarkLayer(Background b)
        {
            CloudberryKingdom.BackgroundFloaterList __46 = new CloudberryKingdom.BackgroundFloaterList();
            __46.Name = "Dark";
            __46.Foreground = false;
            __46.Fixed = false;
            
            CloudberryKingdom.BackgroundFloater __47 = new CloudberryKingdom.BackgroundFloater();
            __47.Name = "Dark";
            __47.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __47.MyQuad.Quad._MyTexture = Tools.TextureWad.DefaultTexture;
            __47.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 158);
            __47.MyQuad.Quad.PremultipliedColor = new Color(157, 157, 157, 191);
            __47.MyQuad.Quad.BlendAddRatio = 0f;
            __47.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-2697.719f, 1436.633f), new Vector2(0f, 0f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __47.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(46939.48f, 1436.633f), new Vector2(18.00215f, 0f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __47.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-2697.719f, -1321.049f), new Vector2(0f, 0.9999274f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __47.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(46939.48f, -1321.049f), new Vector2(18.00215f, 0.9999274f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __47.MyQuad.Quad.SetColor(new Color(0, 0, 0, 125));
            __47.MyQuad.Alpha = .5f;
            __47.MyQuad.Update();

            __47.MyQuad.Quad.ExtraTexture1 = null;
            __47.MyQuad.Quad.ExtraTexture2 = null;

            __47.MyQuad.Base = new CoreEngine.BasePoint(24818.6f, 0f, 0f, 1378.841f, 22120.88f, 57.79195f);

            __47.uv_offset = new Vector2(0f, 0f);
            __47.Data = new PhsxData(22120.88f, 57.79195f, 0f, 0f, 0f, 0f);
            __47.StartData = new PhsxData(22120.88f, 57.79195f, 0f, 0f, 0f, 0f);
            __46.Floaters.Add(__47);

            __46.Parallax = 1.05f;
            __46.DoPreDraw = false;

            b.MyCollection.Lists.Add(__46);

            b.SetLevel(b.MyLevel);

            __47.MyQuad.Quad.SetColor(new Color(0, 0, 0, 100));
            __47.MyQuad.Alpha = .4f;
            __47.InitialUpdate();
        }
    }
}

