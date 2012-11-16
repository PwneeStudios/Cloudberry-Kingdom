using Microsoft.Xna.Framework;


namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void AddRainLayer(Background b)
        {
            CloudberryKingdom.BackgroundFloaterList __46 = new CloudberryKingdom.BackgroundFloaterList();
            __46.Name = "Rain";
            __46.Foreground = true;
            __46.Fixed = false;
            CloudberryKingdom.BackgroundFloater __47 = new CloudberryKingdom.BackgroundFloater();
            __47.Name = "Rain";
            __47.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __47.MyQuad.Quad._MyTexture = Tools.Texture("Rain");
            __47.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 158);
            __47.MyQuad.Quad.PremultipliedColor = new Color(157, 157, 157, 91);
            __47.MyQuad.Quad.BlendAddRatio = 0.42f;
            __47.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-2697.719f, 1436.633f), new Vector2(0f, 0f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __47.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(46939.48f, 1436.633f), new Vector2(18.00215f, 0f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __47.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-2697.719f, -1321.049f), new Vector2(0f, 0.9999274f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __47.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(46939.48f, -1321.049f), new Vector2(18.00215f, 0.9999274f), new Color(157, 157, 157, 91));
            __47.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __47.MyQuad.Quad.ExtraTexture1 = null;
            __47.MyQuad.Quad.ExtraTexture2 = null;

            __47.MyQuad.Base = new BasePoint(24818.6f, 0f, 0f, 1378.841f, 22120.88f, 57.79195f);

            __47.uv_speed = 1.05f * new Vector2(0.0037f, -0.01f);

            __47.uv_offset = new Vector2(0f, 0f);
            __47.Data = new PhsxData(22120.88f, 57.79195f, 0f, 0f, 0f, 0f);
            __47.StartData = new PhsxData(22120.88f, 57.79195f, 0f, 0f, 0f, 0f);
            __46.Floaters.Add(__47);

            __46.Parallax = 1.05f;
            __46.DoPreDraw = false;
            b.MyCollection.Lists.Add(__46);
        }
    }
}

