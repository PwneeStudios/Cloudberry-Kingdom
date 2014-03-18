using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
		public static void _code_Palace(Background b)
		{
			b.GuidCounter = 0;
			b.MyGlobalIllumination = 1f;
			b.AllowLava = true;
			CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
			__1.Name = "BackgroundWall";
			__1.Foreground = false;
			__1.Fixed = false;
			CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
			__2.Name = "Palace_Background";
			__2.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-29396.38f, 4045.997f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__2.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__2.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(39484.3f, 4045.997f), new Vector2(20f, 0f), new Color(255, 255, 255, 255));
			__2.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__2.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-29396.38f, -4068.219f), new Vector2(0f, 5f), new Color(255, 255, 255, 255));
			__2.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__2.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(39484.3f, -4068.219f), new Vector2(20f, 5f), new Color(255, 255, 255, 255));
			__2.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__2.MyQuad.Quad.ExtraTexture1 = null;
			__2.MyQuad.Quad.ExtraTexture2 = null;
			__2.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Background");
			__2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__2.MyQuad.Quad.BlendAddRatio = 0f;

			__2.MyQuad.Base = new CoreEngine.BasePoint(34440.34f, 0f, 0f, 4057.108f, 5043.959f, -11.11111f);

			__2.uv_speed = new Vector2(0f, 0f);
			__2.uv_offset = new Vector2(0f, 0f);
			__2.Data = new PhsxData(5043.959f, -11.11111f, 0f, 0f, 0f, 0f);
			__2.StartData = new PhsxData(5043.959f, -11.11111f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__2);

			CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
			__3.Name = "Palace_Window";
			__3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__3.MyQuad.Quad.ExtraTexture1 = null;
			__3.MyQuad.Quad.ExtraTexture2 = null;
			__3.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__3.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__3.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__3.MyQuad.Quad.BlendAddRatio = 0f;

			__3.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, -471.3809f, 1777.778f);

			__3.uv_speed = new Vector2(0f, 0f);
			__3.uv_offset = new Vector2(0f, 0f);
			__3.Data = new PhsxData(-471.3809f, 1777.778f, 0f, 0f, 0f, 0f);
			__3.StartData = new PhsxData(-471.3809f, 1777.778f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__3);

			CloudberryKingdom.BackgroundFloater __4 = new CloudberryKingdom.BackgroundFloater();
			__4.Name = "Palace_Portrait1";
			__4.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(1590.076f, 1542.368f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__4.MyQuad.Quad.v0.Pos = new Vector2(-0.9991667f, 1f);

			__4.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(4434.517f, 1542.368f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__4.MyQuad.Quad.v1.Pos = new Vector2(1.000833f, 1f);

			__4.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(1590.076f, -2231.256f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__4.MyQuad.Quad.v2.Pos = new Vector2(-0.9991667f, -1f);

			__4.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(4434.517f, -2231.256f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__4.MyQuad.Quad.v3.Pos = new Vector2(1.000833f, -1f);

			__4.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__4.MyQuad.Quad.ExtraTexture1 = null;
			__4.MyQuad.Quad.ExtraTexture2 = null;
			__4.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Portrait1");
			__4.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__4.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__4.MyQuad.Quad.BlendAddRatio = 0f;

			__4.MyQuad.Base = new CoreEngine.BasePoint(1422.22f, 0f, 0f, 1886.812f, 3011.111f, -344.444f);

			__4.uv_speed = new Vector2(0f, 0f);
			__4.uv_offset = new Vector2(0f, 0f);
			__4.Data = new PhsxData(3011.111f, -344.444f, 0f, 0f, 0f, 0f);
			__4.StartData = new PhsxData(3011.111f, -344.444f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__4);

			CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
			__5.Name = "Palace_Window";
			__5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__5.MyQuad.Quad.ExtraTexture1 = null;
			__5.MyQuad.Quad.ExtraTexture2 = null;
			__5.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__5.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__5.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__5.MyQuad.Quad.BlendAddRatio = 0f;

			__5.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 7122.223f, 1733.333f);

			__5.uv_speed = new Vector2(0f, 0f);
			__5.uv_offset = new Vector2(0f, 0f);
			__5.Data = new PhsxData(7122.223f, 1733.333f, 0f, 0f, 0f, 0f);
			__5.StartData = new PhsxData(7122.223f, 1733.333f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__5);

			CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
			__6.Name = "Palace_Window";
			__6.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(9331.162f, 1475.328f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__6.MyQuad.Quad.v0.Pos = new Vector2(-0.9974999f, 0.9994444f);

			__6.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(12042.27f, 1475.328f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__6.MyQuad.Quad.v1.Pos = new Vector2(1.0025f, 0.9994444f);

			__6.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(9331.162f, -771.0206f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__6.MyQuad.Quad.v2.Pos = new Vector2(-0.9974999f, -1.000556f);

			__6.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(12042.27f, -771.0206f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__6.MyQuad.Quad.v3.Pos = new Vector2(1.0025f, -1.000556f);

			__6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__6.MyQuad.Quad.ExtraTexture1 = null;
			__6.MyQuad.Quad.ExtraTexture2 = null;
			__6.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__6.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__6.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__6.MyQuad.Quad.BlendAddRatio = 0f;

			__6.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 10683.33f, 352.7776f);

			__6.uv_speed = new Vector2(0f, 0f);
			__6.uv_offset = new Vector2(0f, 0f);
			__6.Data = new PhsxData(10683.33f, 352.7776f, 0f, 0f, 0f, 0f);
			__6.StartData = new PhsxData(10683.33f, 352.7776f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__6);

			CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
			__7.Name = "Palace_Window";
			__7.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__7.MyQuad.Quad.ExtraTexture1 = null;
			__7.MyQuad.Quad.ExtraTexture2 = null;
			__7.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__7.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__7.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__7.MyQuad.Quad.BlendAddRatio = 0f;

			__7.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 7683.332f, -1836.111f);

			__7.uv_speed = new Vector2(0f, 0f);
			__7.uv_offset = new Vector2(0f, 0f);
			__7.Data = new PhsxData(7683.332f, -1836.111f, 0f, 0f, 0f, 0f);
			__7.StartData = new PhsxData(7683.332f, -1836.111f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__7);

			CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
			__8.Name = "Palace_Portrait1";
			__8.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(13217.06f, 1572.923f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__8.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__8.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(16061.5f, 1572.923f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__8.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__8.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(13217.06f, -2200.701f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__8.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__8.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(16061.5f, -2200.701f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__8.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__8.MyQuad.Quad.ExtraTexture1 = null;
			__8.MyQuad.Quad.ExtraTexture2 = null;
			__8.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Portrait1");
			__8.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__8.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__8.MyQuad.Quad.BlendAddRatio = 0f;

			__8.MyQuad.Base = new CoreEngine.BasePoint(1422.22f, 0f, 0f, 1886.812f, 14638.89f, -313.8889f);

			__8.uv_speed = new Vector2(0f, 0f);
			__8.uv_offset = new Vector2(0f, 0f);
			__8.Data = new PhsxData(14638.89f, -313.8889f, 0f, 0f, 0f, 0f);
			__8.StartData = new PhsxData(14638.89f, -313.8889f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__8);

			CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
			__9.Name = "Palace_Portrait1";
			__9.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(16452.27f, 2617.105f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__9.MyQuad.Quad.v0.Pos = new Vector2(-0.9827778f, 0.9925f);

			__9.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(19296.71f, 2617.105f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__9.MyQuad.Quad.v1.Pos = new Vector2(1.017222f, 0.9925f);

			__9.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(16452.27f, -1156.519f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__9.MyQuad.Quad.v2.Pos = new Vector2(-0.9827778f, -1.0075f);

			__9.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(19296.71f, -1156.519f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__9.MyQuad.Quad.v3.Pos = new Vector2(1.017222f, -1.0075f);

			__9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__9.MyQuad.Quad.ExtraTexture1 = null;
			__9.MyQuad.Quad.ExtraTexture2 = null;
			__9.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Portrait1");
			__9.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__9.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__9.MyQuad.Quad.BlendAddRatio = 0f;

			__9.MyQuad.Base = new CoreEngine.BasePoint(1422.22f, 0f, 0f, 1886.812f, 17849.99f, 744.4443f);

			__9.uv_speed = new Vector2(0f, 0f);
			__9.uv_offset = new Vector2(0f, 0f);
			__9.Data = new PhsxData(17849.99f, 744.4443f, 0f, 0f, 0f, 0f);
			__9.StartData = new PhsxData(17849.99f, 744.4443f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__9);

			CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
			__10.Name = "Palace_Window";
			__10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__10.MyQuad.Quad.ExtraTexture1 = null;
			__10.MyQuad.Quad.ExtraTexture2 = null;
			__10.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__10.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__10.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__10.MyQuad.Quad.BlendAddRatio = 0f;

			__10.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 22005.55f, 922.222f);

			__10.uv_speed = new Vector2(0f, 0f);
			__10.uv_offset = new Vector2(0f, 0f);
			__10.Data = new PhsxData(22005.55f, 922.222f, 0f, 0f, 0f, 0f);
			__10.StartData = new PhsxData(22005.55f, 922.222f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__10);

			CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
			__11.Name = "Palace_Portrait1";
			__11.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__11.MyQuad.Quad.ExtraTexture1 = null;
			__11.MyQuad.Quad.ExtraTexture2 = null;
			__11.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Portrait1");
			__11.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__11.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__11.MyQuad.Quad.BlendAddRatio = 0f;

			__11.MyQuad.Base = new CoreEngine.BasePoint(1422.22f, 0f, 0f, 1886.812f, 25716.66f, -975.0001f);

			__11.uv_speed = new Vector2(0f, 0f);
			__11.uv_offset = new Vector2(0f, 0f);
			__11.Data = new PhsxData(25716.66f, -975.0001f, 0f, 0f, 0f, 0f);
			__11.StartData = new PhsxData(25716.66f, -975.0001f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__11);

			CloudberryKingdom.BackgroundFloater __12 = new CloudberryKingdom.BackgroundFloater();
			__12.Name = "Palace_Portrait1";
			__12.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__12.MyQuad.Quad.ExtraTexture1 = null;
			__12.MyQuad.Quad.ExtraTexture2 = null;
			__12.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Portrait1");
			__12.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__12.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__12.MyQuad.Quad.BlendAddRatio = 0f;

			__12.MyQuad.Base = new CoreEngine.BasePoint(1422.22f, 0f, 0f, 1886.812f, 29105.55f, 1247.222f);

			__12.uv_speed = new Vector2(0f, 0f);
			__12.uv_offset = new Vector2(0f, 0f);
			__12.Data = new PhsxData(29105.55f, 1247.222f, 0f, 0f, 0f, 0f);
			__12.StartData = new PhsxData(29105.55f, 1247.222f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__12);

			CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
			__13.Name = "Palace_Window";
			__13.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(20561.48f, -940.7148f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__13.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__13.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(23272.59f, -940.7148f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__13.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__13.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(20561.48f, -3187.063f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__13.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__13.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(23272.59f, -3187.063f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__13.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__13.MyQuad.Quad.ExtraTexture1 = null;
			__13.MyQuad.Quad.ExtraTexture2 = null;
			__13.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__13.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__13.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__13.MyQuad.Quad.BlendAddRatio = 0f;

			__13.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 21916.66f, -2063.889f);

			__13.uv_speed = new Vector2(0f, 0f);
			__13.uv_offset = new Vector2(0f, 0f);
			__13.Data = new PhsxData(21916.66f, -2063.889f, 0f, 0f, 0f, 0f);
			__13.StartData = new PhsxData(21916.66f, -2063.889f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__13);

			CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
			__14.Name = "Palace_Window";
			__14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__14.MyQuad.Quad.ExtraTexture1 = null;
			__14.MyQuad.Quad.ExtraTexture2 = null;
			__14.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__14.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__14.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__14.MyQuad.Quad.BlendAddRatio = 0f;

			__14.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 33161.1f, 1036.111f);

			__14.uv_speed = new Vector2(0f, 0f);
			__14.uv_offset = new Vector2(0f, 0f);
			__14.Data = new PhsxData(33161.1f, 1036.111f, 0f, 0f, 0f, 0f);
			__14.StartData = new PhsxData(33161.1f, 1036.111f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__14);

			CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
			__15.Name = "Palace_Window";
			__15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__15.MyQuad.Quad.ExtraTexture1 = null;
			__15.MyQuad.Quad.ExtraTexture2 = null;
			__15.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__15.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__15.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__15.MyQuad.Quad.BlendAddRatio = 0f;

			__15.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 34977.77f, -1444.444f);

			__15.uv_speed = new Vector2(0f, 0f);
			__15.uv_offset = new Vector2(0f, 0f);
			__15.Data = new PhsxData(34977.77f, -1444.444f, 0f, 0f, 0f, 0f);
			__15.StartData = new PhsxData(34977.77f, -1444.444f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__15);

			CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
			__16.Name = "Palace_Window";
			__16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__16.MyQuad.Quad.ExtraTexture1 = null;
			__16.MyQuad.Quad.ExtraTexture2 = null;
			__16.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Window");
			__16.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__16.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__16.MyQuad.Quad.BlendAddRatio = 0f;

			__16.MyQuad.Base = new CoreEngine.BasePoint(1355.555f, 0f, 0f, 1123.174f, 36766.66f, 1744.444f);

			__16.uv_speed = new Vector2(0f, 0f);
			__16.uv_offset = new Vector2(0f, 0f);
			__16.Data = new PhsxData(36766.66f, 1744.444f, 0f, 0f, 0f, 0f);
			__16.StartData = new PhsxData(36766.66f, 1744.444f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__16);

			__1.Parallax = 0.25f;
			__1.DoPreDraw = false;
			b.MyCollection.Lists.Add(__1);

			CloudberryKingdom.BackgroundFloaterList __17 = new CloudberryKingdom.BackgroundFloaterList();
			__17.Name = "Chandaliers_1";
			__17.Foreground = false;
			__17.Fixed = false;
			CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
			__18.Name = "Palace_Chandelier_Blurred";
			__18.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(3408.542f, 3491.198f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__18.MyQuad.Quad.v0.Pos = new Vector2(-1.06f, 1.017778f);

			__18.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(5081.558f, 3491.198f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__18.MyQuad.Quad.v1.Pos = new Vector2(0.9400001f, 1.017778f);

			__18.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(3408.542f, 149.9459f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__18.MyQuad.Quad.v2.Pos = new Vector2(-1.06f, -0.9822222f);

			__18.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(5081.558f, 149.9459f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__18.MyQuad.Quad.v3.Pos = new Vector2(0.9400001f, -0.9822222f);

			__18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__18.MyQuad.Quad.ExtraTexture1 = null;
			__18.MyQuad.Quad.ExtraTexture2 = null;
			__18.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__18.MyQuad.Quad.BlendAddRatio = 0f;

			__18.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 4295.24f, 1790.872f);

			__18.uv_speed = new Vector2(0f, 0f);
			__18.uv_offset = new Vector2(0f, 0f);
			__18.Data = new PhsxData(4295.24f, 1790.872f, 0f, 0f, 0f, 0f);
			__18.StartData = new PhsxData(4295.24f, 1790.872f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__18);

			CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
			__19.Name = "Palace_Chandelier_Blurred";
			__19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__19.MyQuad.Quad.ExtraTexture1 = null;
			__19.MyQuad.Quad.ExtraTexture2 = null;
			__19.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__19.MyQuad.Quad.BlendAddRatio = 0f;

			__19.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 351.1973f, 2193.65f);

			__19.uv_speed = new Vector2(0f, 0f);
			__19.uv_offset = new Vector2(0f, 0f);
			__19.Data = new PhsxData(351.1973f, 2193.65f, 0f, 0f, 0f, 0f);
			__19.StartData = new PhsxData(351.1973f, 2193.65f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__19);

			CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
			__20.Name = "Palace_Chandelier_Blurred";
			__20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__20.MyQuad.Quad.ExtraTexture1 = null;
			__20.MyQuad.Quad.ExtraTexture2 = null;
			__20.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__20.MyQuad.Quad.BlendAddRatio = 0f;

			__20.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 30158.33f, 1553.571f);

			__20.uv_speed = new Vector2(0f, 0f);
			__20.uv_offset = new Vector2(0f, 0f);
			__20.Data = new PhsxData(30158.33f, 1553.571f, 0f, 0f, 0f, 0f);
			__20.StartData = new PhsxData(30158.33f, 1553.571f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__20);

			CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
			__21.Name = "Palace_Chandelier_Blurred";
			__21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__21.MyQuad.Quad.ExtraTexture1 = null;
			__21.MyQuad.Quad.ExtraTexture2 = null;
			__21.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__21.MyQuad.Quad.BlendAddRatio = 0f;

			__21.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 26666.28f, 1363.095f);

			__21.uv_speed = new Vector2(0f, 0f);
			__21.uv_offset = new Vector2(0f, 0f);
			__21.Data = new PhsxData(26666.28f, 1363.095f, 0f, 0f, 0f, 0f);
			__21.StartData = new PhsxData(26666.28f, 1363.095f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__21);

			CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
			__22.Name = "Palace_Chandelier_Blurred";
			__22.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(20531.14f, 2893.798f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v0.Pos = new Vector2(-1.005f, 0.9727778f);

			__22.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(22204.16f, 2893.798f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v1.Pos = new Vector2(0.995f, 0.9727778f);

			__22.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(20531.14f, -447.4539f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v2.Pos = new Vector2(-1.005f, -1.027222f);

			__22.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(22204.16f, -447.4539f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v3.Pos = new Vector2(0.995f, -1.027222f);

			__22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__22.MyQuad.Quad.ExtraTexture1 = null;
			__22.MyQuad.Quad.ExtraTexture2 = null;
			__22.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__22.MyQuad.Quad.BlendAddRatio = 0f;

			__22.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 21371.83f, 1268.65f);

			__22.uv_speed = new Vector2(0f, 0f);
			__22.uv_offset = new Vector2(0f, 0f);
			__22.Data = new PhsxData(21371.83f, 1268.65f, 0f, 0f, 0f, 0f);
			__22.StartData = new PhsxData(21371.83f, 1268.65f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__22);

			CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
			__23.Name = "Palace_Chandelier_Blurred";
			__23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__23.MyQuad.Quad.ExtraTexture1 = null;
			__23.MyQuad.Quad.ExtraTexture2 = null;
			__23.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__23.MyQuad.Quad.BlendAddRatio = 0f;

			__23.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 18114.68f, 1566.27f);

			__23.uv_speed = new Vector2(0f, 0f);
			__23.uv_offset = new Vector2(0f, 0f);
			__23.Data = new PhsxData(18114.68f, 1566.27f, 0f, 0f, 0f, 0f);
			__23.StartData = new PhsxData(18114.68f, 1566.27f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__23);

			CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
			__24.Name = "Palace_Chandelier_Blurred";
			__24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__24.MyQuad.Quad.ExtraTexture1 = null;
			__24.MyQuad.Quad.ExtraTexture2 = null;
			__24.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__24.MyQuad.Quad.BlendAddRatio = 0f;

			__24.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 13900.4f, 1971.031f);

			__24.uv_speed = new Vector2(0f, 0f);
			__24.uv_offset = new Vector2(0f, 0f);
			__24.Data = new PhsxData(13900.4f, 1971.031f, 0f, 0f, 0f, 0f);
			__24.StartData = new PhsxData(13900.4f, 1971.031f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__24);

			CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
			__25.Name = "Palace_Chandelier_Blurred";
			__25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__25.MyQuad.Quad.ExtraTexture1 = null;
			__25.MyQuad.Quad.ExtraTexture2 = null;
			__25.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__25.MyQuad.Quad.BlendAddRatio = 0f;

			__25.MyQuad.Base = new CoreEngine.BasePoint(836.5081f, 0f, 0f, 1670.626f, 9837.699f, 2404.365f);

			__25.uv_speed = new Vector2(0f, 0f);
			__25.uv_offset = new Vector2(0f, 0f);
			__25.Data = new PhsxData(9837.699f, 2404.365f, 0f, 0f, 0f, 0f);
			__25.StartData = new PhsxData(9837.699f, 2404.365f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__25);

			__17.Parallax = 0.35f;
			__17.DoPreDraw = false;
			b.MyCollection.Lists.Add(__17);

			CloudberryKingdom.BackgroundFloaterList __26 = new CloudberryKingdom.BackgroundFloaterList();
			__26.Name = "Pillars";
			__26.Foreground = false;
			__26.Fixed = false;
			CloudberryKingdom.BackgroundFloater __27 = new CloudberryKingdom.BackgroundFloater();
			__27.Name = "Palace_BPillar";
			__27.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-453.0871f, 2463.888f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__27.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(186.4192f, 2463.888f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__27.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-453.0871f, -2599.69f), new Vector2(0f, 8f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__27.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(186.4192f, -2599.69f), new Vector2(1f, 8f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__27.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__27.MyQuad.Quad.ExtraTexture1 = null;
			__27.MyQuad.Quad.ExtraTexture2 = null;
			__27.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__27.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__27.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__27.MyQuad.Quad.BlendAddRatio = 0f;

			__27.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, -133.334f, -67.90121f);

			__27.uv_speed = new Vector2(0f, 0f);
			__27.uv_offset = new Vector2(0f, 0f);
			__27.Data = new PhsxData(-133.334f, -67.90121f, 0f, 0f, 0f, 0f);
			__27.StartData = new PhsxData(-133.334f, -67.90121f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__27);

			CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
			__28.Name = "Palace_BPillar";
			__28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__28.MyQuad.Quad.ExtraTexture1 = null;
			__28.MyQuad.Quad.ExtraTexture2 = null;
			__28.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__28.MyQuad.Quad.BlendAddRatio = 0f;

			__28.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 1379.012f, -30.86419f);

			__28.uv_speed = new Vector2(0f, 0f);
			__28.uv_offset = new Vector2(0f, 0f);
			__28.Data = new PhsxData(1379.012f, -30.86419f, 0f, 0f, 0f, 0f);
			__28.StartData = new PhsxData(1379.012f, -30.86419f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__28);

			CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
			__29.Name = "Palace_BPillar";
			__29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__29.MyQuad.Quad.ExtraTexture1 = null;
			__29.MyQuad.Quad.ExtraTexture2 = null;
			__29.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__29.MyQuad.Quad.BlendAddRatio = 0f;

			__29.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 4169.135f, -43.20992f);

			__29.uv_speed = new Vector2(0f, 0f);
			__29.uv_offset = new Vector2(0f, 0f);
			__29.Data = new PhsxData(4169.135f, -43.20992f, 0f, 0f, 0f, 0f);
			__29.StartData = new PhsxData(4169.135f, -43.20992f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__29);

			CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
			__30.Name = "Palace_BPillar";
			__30.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(5315.899f, 2398.643f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v0.Pos = new Vector2(-1.0275f, 0.9986111f);

			__30.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(5955.405f, 2398.643f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v1.Pos = new Vector2(0.9725f, 0.9986111f);

			__30.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(5315.899f, -2664.935f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v2.Pos = new Vector2(-1.0275f, -1.001389f);

			__30.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(5955.405f, -2664.935f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v3.Pos = new Vector2(0.9725f, -1.001389f);

			__30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__30.MyQuad.Quad.ExtraTexture1 = null;
			__30.MyQuad.Quad.ExtraTexture2 = null;
			__30.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__30.MyQuad.Quad.BlendAddRatio = 0f;

			__30.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 5644.445f, -129.6297f);

			__30.uv_speed = new Vector2(0f, 0f);
			__30.uv_offset = new Vector2(0f, 0f);
			__30.Data = new PhsxData(5644.445f, -129.6297f, 0f, 0f, 0f, 0f);
			__30.StartData = new PhsxData(5644.445f, -129.6297f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__30);

			CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
			__31.Name = "Palace_BPillar";
			__31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__31.MyQuad.Quad.ExtraTexture1 = null;
			__31.MyQuad.Quad.ExtraTexture2 = null;
			__31.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__31.MyQuad.Quad.BlendAddRatio = 0f;

			__31.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 8602.77f, 22.22206f);

			__31.uv_speed = new Vector2(0f, 0f);
			__31.uv_offset = new Vector2(0f, 0f);
			__31.Data = new PhsxData(8602.77f, 22.22206f, 0f, 0f, 0f, 0f);
			__31.StartData = new PhsxData(8602.77f, 22.22206f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__31);

			CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
			__32.Name = "Palace_BPillar";
			__32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__32.MyQuad.Quad.ExtraTexture1 = null;
			__32.MyQuad.Quad.ExtraTexture2 = null;
			__32.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__32.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__32.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__32.MyQuad.Quad.BlendAddRatio = 0f;

			__32.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 9627.455f, -51.85201f);

			__32.uv_speed = new Vector2(0f, 0f);
			__32.uv_offset = new Vector2(0f, 0f);
			__32.Data = new PhsxData(9627.455f, -51.85201f, 0f, 0f, 0f, 0f);
			__32.StartData = new PhsxData(9627.455f, -51.85201f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__32);

			CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
			__33.Name = "Palace_BPillar";
			__33.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(13001.28f, 2634.415f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v0.Pos = new Vector2(-0.9930555f, 1.0025f);

			__33.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(13640.79f, 2634.415f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v1.Pos = new Vector2(1.006944f, 1.0025f);

			__33.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(13001.28f, -2429.163f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v2.Pos = new Vector2(-0.9930555f, -0.9975f);

			__33.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(13640.79f, -2429.163f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v3.Pos = new Vector2(1.006944f, -0.9975f);

			__33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__33.MyQuad.Quad.ExtraTexture1 = null;
			__33.MyQuad.Quad.ExtraTexture2 = null;
			__33.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__33.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__33.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__33.MyQuad.Quad.BlendAddRatio = 0f;

			__33.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 13318.82f, 96.29616f);

			__33.uv_speed = new Vector2(0f, 0f);
			__33.uv_offset = new Vector2(0f, 0f);
			__33.Data = new PhsxData(13318.82f, 96.29616f, 0f, 0f, 0f, 0f);
			__33.StartData = new PhsxData(13318.82f, 96.29616f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__33);

			CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
			__34.Name = "Palace_BPillar";
			__34.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(15170.06f, 2485.356f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1.000556f);

			__34.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(15809.56f, 2485.356f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v1.Pos = new Vector2(1f, 1.000556f);

			__34.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(15170.06f, -2578.222f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v2.Pos = new Vector2(-1f, -0.9994444f);

			__34.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(15809.56f, -2578.222f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v3.Pos = new Vector2(1f, -0.9994444f);

			__34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__34.MyQuad.Quad.ExtraTexture1 = null;
			__34.MyQuad.Quad.ExtraTexture2 = null;
			__34.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__34.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__34.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__34.MyQuad.Quad.BlendAddRatio = 0f;

			__34.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 15489.81f, -47.83963f);

			__34.uv_speed = new Vector2(0f, 0f);
			__34.uv_offset = new Vector2(0f, 0f);
			__34.Data = new PhsxData(15489.81f, -47.83963f, 0f, 0f, 0f, 0f);
			__34.StartData = new PhsxData(15489.81f, -47.83963f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__34);

			CloudberryKingdom.BackgroundFloater __35 = new CloudberryKingdom.BackgroundFloater();
			__35.Name = "Palace_BPillar";
			__35.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__35.MyQuad.Quad.ExtraTexture1 = null;
			__35.MyQuad.Quad.ExtraTexture2 = null;
			__35.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__35.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__35.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__35.MyQuad.Quad.BlendAddRatio = 0f;

			__35.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 14452.77f, -41.66681f);

			__35.uv_speed = new Vector2(0f, 0f);
			__35.uv_offset = new Vector2(0f, 0f);
			__35.Data = new PhsxData(14452.77f, -41.66681f, 0f, 0f, 0f, 0f);
			__35.StartData = new PhsxData(14452.77f, -41.66681f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__35);

			CloudberryKingdom.BackgroundFloater __36 = new CloudberryKingdom.BackgroundFloater();
			__36.Name = "Palace_BPillar";
			__36.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__36.MyQuad.Quad.ExtraTexture1 = null;
			__36.MyQuad.Quad.ExtraTexture2 = null;
			__36.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__36.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__36.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__36.MyQuad.Quad.BlendAddRatio = 0f;

			__36.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 19672.53f, -36.41989f);

			__36.uv_speed = new Vector2(0f, 0f);
			__36.uv_offset = new Vector2(0f, 0f);
			__36.Data = new PhsxData(19672.53f, -36.41989f, 0f, 0f, 0f, 0f);
			__36.StartData = new PhsxData(19672.53f, -36.41989f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__36);

			CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
			__37.Name = "Palace_BPillar";
			__37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__37.MyQuad.Quad.ExtraTexture1 = null;
			__37.MyQuad.Quad.ExtraTexture2 = null;
			__37.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__37.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__37.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__37.MyQuad.Quad.BlendAddRatio = 0f;

			__37.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 22944.43f, -30.55569f);

			__37.uv_speed = new Vector2(0f, 0f);
			__37.uv_offset = new Vector2(0f, 0f);
			__37.Data = new PhsxData(22944.43f, -30.55569f, 0f, 0f, 0f, 0f);
			__37.StartData = new PhsxData(22944.43f, -30.55569f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__37);

			CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
			__38.Name = "Palace_BPillar";
			__38.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(26560.61f, 2417.9f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v0.Pos = new Vector2(-1.000556f, 1f);

			__38.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(27200.12f, 2417.9f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v1.Pos = new Vector2(0.9994444f, 1f);

			__38.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(26560.61f, -2645.678f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v2.Pos = new Vector2(-1.000556f, -1f);

			__38.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(27200.12f, -2645.678f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v3.Pos = new Vector2(0.9994444f, -1f);

			__38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__38.MyQuad.Quad.ExtraTexture1 = null;
			__38.MyQuad.Quad.ExtraTexture2 = null;
			__38.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__38.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__38.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__38.MyQuad.Quad.BlendAddRatio = 0f;

			__38.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 26880.54f, -113.889f);

			__38.uv_speed = new Vector2(0f, 0f);
			__38.uv_offset = new Vector2(0f, 0f);
			__38.Data = new PhsxData(26880.54f, -113.889f, 0f, 0f, 0f, 0f);
			__38.StartData = new PhsxData(26880.54f, -113.889f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__38);

			CloudberryKingdom.BackgroundFloater __39 = new CloudberryKingdom.BackgroundFloater();
			__39.Name = "Palace_BPillar";
			__39.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__39.MyQuad.Quad.ExtraTexture1 = null;
			__39.MyQuad.Quad.ExtraTexture2 = null;
			__39.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__39.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__39.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__39.MyQuad.Quad.BlendAddRatio = 0f;

			__39.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 28090.43f, -45.98783f);

			__39.uv_speed = new Vector2(0f, 0f);
			__39.uv_offset = new Vector2(0f, 0f);
			__39.Data = new PhsxData(28090.43f, -45.98783f, 0f, 0f, 0f, 0f);
			__39.StartData = new PhsxData(28090.43f, -45.98783f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__39);

			CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
			__40.Name = "Palace_BPillar";
			__40.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(30111.81f, 2529.011f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v0.Pos = new Vector2(-1.014167f, 1f);

			__40.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(30751.31f, 2529.011f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v1.Pos = new Vector2(0.9858334f, 1f);

			__40.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(30111.81f, -2534.567f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v2.Pos = new Vector2(-1.014167f, -1f);

			__40.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(30751.31f, -2534.567f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v3.Pos = new Vector2(0.9858334f, -1f);

			__40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__40.MyQuad.Quad.ExtraTexture1 = null;
			__40.MyQuad.Quad.ExtraTexture2 = null;
			__40.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__40.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__40.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__40.MyQuad.Quad.BlendAddRatio = 0f;

			__40.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 30436.09f, -2.777854f);

			__40.uv_speed = new Vector2(0f, 0f);
			__40.uv_offset = new Vector2(0f, 0f);
			__40.Data = new PhsxData(30436.09f, -2.777854f, 0f, 0f, 0f, 0f);
			__40.StartData = new PhsxData(30436.09f, -2.777854f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__40);

			CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
			__41.Name = "Palace_BPillar";
			__41.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(31417.48f, 2448.764f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__41.MyQuad.Quad.v0.Pos = new Vector2(-1.004167f, 1f);

			__41.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(32056.98f, 2448.764f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__41.MyQuad.Quad.v1.Pos = new Vector2(0.9958334f, 1f);

			__41.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(31417.48f, -2614.813f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__41.MyQuad.Quad.v2.Pos = new Vector2(-1.004167f, -1f);

			__41.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(32056.98f, -2614.813f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__41.MyQuad.Quad.v3.Pos = new Vector2(0.9958334f, -1f);

			__41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__41.MyQuad.Quad.ExtraTexture1 = null;
			__41.MyQuad.Quad.ExtraTexture2 = null;
			__41.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__41.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__41.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__41.MyQuad.Quad.BlendAddRatio = 0f;

			__41.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 31738.56f, -83.0247f);

			__41.uv_speed = new Vector2(0f, 0f);
			__41.uv_offset = new Vector2(0f, 0f);
			__41.Data = new PhsxData(31738.56f, -83.0247f, 0f, 0f, 0f, 0f);
			__41.StartData = new PhsxData(31738.56f, -83.0247f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__41);

			CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
			__42.Name = "Palace_BPillar";
			__42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__42.MyQuad.Quad.ExtraTexture1 = null;
			__42.MyQuad.Quad.ExtraTexture2 = null;
			__42.MyQuad.Quad._MyTexture = Tools.Texture("Palace_BPillar");
			__42.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__42.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__42.MyQuad.Quad.BlendAddRatio = 0f;

			__42.MyQuad.Base = new CoreEngine.BasePoint(319.7531f, 0f, 0f, 2531.789f, 34964.8f, -124.3828f);

			__42.uv_speed = new Vector2(0f, 0f);
			__42.uv_offset = new Vector2(0f, 0f);
			__42.Data = new PhsxData(34964.8f, -124.3828f, 0f, 0f, 0f, 0f);
			__42.StartData = new PhsxData(34964.8f, -124.3828f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__42);

			__26.Parallax = 0.45f;
			__26.DoPreDraw = false;
			b.MyCollection.Lists.Add(__26);

			CloudberryKingdom.BackgroundFloaterList __43 = new CloudberryKingdom.BackgroundFloaterList();
			__43.Name = "Chandaliers_2";
			__43.Foreground = false;
			__43.Fixed = false;
			CloudberryKingdom.BackgroundFloater __44 = new CloudberryKingdom.BackgroundFloater();
			__44.Name = "Palace_Chandelier_Blurred";
			__44.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1075.752f, 2534.881f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v0.Pos = new Vector2(-1f, 0.9752778f);

			__44.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(334.3383f, 2534.881f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v1.Pos = new Vector2(1f, 0.9752778f);

			__44.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1075.752f, -281.2706f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1.024722f);

			__44.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(334.3383f, -281.2706f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v3.Pos = new Vector2(1f, -1.024722f);

			__44.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__44.MyQuad.Quad.ExtraTexture1 = null;
			__44.MyQuad.Quad.ExtraTexture2 = null;
			__44.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__44.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__44.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__44.MyQuad.Quad.BlendAddRatio = 0f;

			__44.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, -370.707f, 1161.616f);

			__44.uv_speed = new Vector2(0f, 0f);
			__44.uv_offset = new Vector2(0f, 0f);
			__44.Data = new PhsxData(-370.707f, 1161.616f, 0f, 0f, 0f, 0f);
			__44.StartData = new PhsxData(-370.707f, 1161.616f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__44);

			CloudberryKingdom.BackgroundFloater __45 = new CloudberryKingdom.BackgroundFloater();
			__45.Name = "Palace_Chandelier_Blurred";
			__45.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__45.MyQuad.Quad.ExtraTexture1 = null;
			__45.MyQuad.Quad.ExtraTexture2 = null;
			__45.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__45.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__45.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__45.MyQuad.Quad.BlendAddRatio = 0f;

			__45.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 1932.324f, 873.7375f);

			__45.uv_speed = new Vector2(0f, 0f);
			__45.uv_offset = new Vector2(0f, 0f);
			__45.Data = new PhsxData(1932.324f, 873.7375f, 0f, 0f, 0f, 0f);
			__45.StartData = new PhsxData(1932.324f, 873.7375f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__45);

			CloudberryKingdom.BackgroundFloater __46 = new CloudberryKingdom.BackgroundFloater();
			__46.Name = "Palace_Chandelier_Blurred";
			__46.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__46.MyQuad.Quad.ExtraTexture1 = null;
			__46.MyQuad.Quad.ExtraTexture2 = null;
			__46.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__46.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.BlendAddRatio = 0f;

			__46.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 5381.819f, 1085.859f);

			__46.uv_speed = new Vector2(0f, 0f);
			__46.uv_offset = new Vector2(0f, 0f);
			__46.Data = new PhsxData(5381.819f, 1085.859f, 0f, 0f, 0f, 0f);
			__46.StartData = new PhsxData(5381.819f, 1085.859f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__46);

			CloudberryKingdom.BackgroundFloater __47 = new CloudberryKingdom.BackgroundFloater();
			__47.Name = "Palace_Chandelier_Blurred";
			__47.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__47.MyQuad.Quad.ExtraTexture1 = null;
			__47.MyQuad.Quad.ExtraTexture2 = null;
			__47.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__47.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__47.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__47.MyQuad.Quad.BlendAddRatio = 0f;

			__47.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 8069.449f, 1465.404f);

			__47.uv_speed = new Vector2(0f, 0f);
			__47.uv_offset = new Vector2(0f, 0f);
			__47.Data = new PhsxData(8069.449f, 1465.404f, 0f, 0f, 0f, 0f);
			__47.StartData = new PhsxData(8069.449f, 1465.404f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__47);

			CloudberryKingdom.BackgroundFloater __48 = new CloudberryKingdom.BackgroundFloater();
			__48.Name = "Palace_Chandelier_Blurred";
			__48.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(11314.25f, 2587.749f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v0.Pos = new Vector2(-0.8952778f, 0.9744445f);

			__48.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(12724.34f, 2587.749f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v1.Pos = new Vector2(1.104722f, 0.9744445f);

			__48.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(11314.25f, -228.4033f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v2.Pos = new Vector2(-0.8952778f, -1.025555f);

			__48.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(12724.34f, -228.4033f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v3.Pos = new Vector2(1.104722f, -1.025555f);

			__48.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__48.MyQuad.Quad.ExtraTexture1 = null;
			__48.MyQuad.Quad.ExtraTexture2 = null;
			__48.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__48.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__48.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__48.MyQuad.Quad.BlendAddRatio = 0f;

			__48.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 11945.46f, 1215.657f);

			__48.uv_speed = new Vector2(0f, 0f);
			__48.uv_offset = new Vector2(0f, 0f);
			__48.Data = new PhsxData(11945.46f, 1215.657f, 0f, 0f, 0f, 0f);
			__48.StartData = new PhsxData(11945.46f, 1215.657f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__48);

			CloudberryKingdom.BackgroundFloater __49 = new CloudberryKingdom.BackgroundFloater();
			__49.Name = "Palace_Chandelier_Blurred";
			__49.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(14726.77f, 2357.041f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v0.Pos = new Vector2(-0.9975001f, 0.9994444f);

			__49.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(16136.86f, 2357.041f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v1.Pos = new Vector2(1.0025f, 0.9994444f);

			__49.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(14726.77f, -459.1115f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v2.Pos = new Vector2(-0.9975001f, -1.000556f);

			__49.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(16136.86f, -459.1115f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v3.Pos = new Vector2(1.0025f, -1.000556f);

			__49.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__49.MyQuad.Quad.ExtraTexture1 = null;
			__49.MyQuad.Quad.ExtraTexture2 = null;
			__49.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__49.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__49.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__49.MyQuad.Quad.BlendAddRatio = 0f;

			__49.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 15430.05f, 949.7471f);

			__49.uv_speed = new Vector2(0f, 0f);
			__49.uv_offset = new Vector2(0f, 0f);
			__49.Data = new PhsxData(15430.05f, 949.7471f, 0f, 0f, 0f, 0f);
			__49.StartData = new PhsxData(15430.05f, 949.7471f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__49);

			CloudberryKingdom.BackgroundFloater __50 = new CloudberryKingdom.BackgroundFloater();
			__50.Name = "Palace_Chandelier_Blurred";
			__50.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__50.MyQuad.Quad.ExtraTexture1 = null;
			__50.MyQuad.Quad.ExtraTexture2 = null;
			__50.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__50.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__50.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__50.MyQuad.Quad.BlendAddRatio = 0f;

			__50.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 17159.85f, 738.8887f);

			__50.uv_speed = new Vector2(0f, 0f);
			__50.uv_offset = new Vector2(0f, 0f);
			__50.Data = new PhsxData(17159.85f, 738.8887f, 0f, 0f, 0f, 0f);
			__50.StartData = new PhsxData(17159.85f, 738.8887f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__50);

			CloudberryKingdom.BackgroundFloater __51 = new CloudberryKingdom.BackgroundFloater();
			__51.Name = "Palace_Chandelier_Blurred";
			__51.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__51.MyQuad.Quad.ExtraTexture1 = null;
			__51.MyQuad.Quad.ExtraTexture2 = null;
			__51.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__51.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__51.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__51.MyQuad.Quad.BlendAddRatio = 0f;

			__51.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 20351.77f, 774.2423f);

			__51.uv_speed = new Vector2(0f, 0f);
			__51.uv_offset = new Vector2(0f, 0f);
			__51.Data = new PhsxData(20351.77f, 774.2423f, 0f, 0f, 0f, 0f);
			__51.StartData = new PhsxData(20351.77f, 774.2423f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__51);

			CloudberryKingdom.BackgroundFloater __52 = new CloudberryKingdom.BackgroundFloater();
			__52.Name = "Palace_Chandelier_Blurred";
			__52.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__52.MyQuad.Quad.ExtraTexture1 = null;
			__52.MyQuad.Quad.ExtraTexture2 = null;
			__52.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__52.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__52.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__52.MyQuad.Quad.BlendAddRatio = 0f;

			__52.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 22953.54f, 708.0806f);

			__52.uv_speed = new Vector2(0f, 0f);
			__52.uv_offset = new Vector2(0f, 0f);
			__52.Data = new PhsxData(22953.54f, 708.0806f, 0f, 0f, 0f, 0f);
			__52.StartData = new PhsxData(22953.54f, 708.0806f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__52);

			CloudberryKingdom.BackgroundFloater __53 = new CloudberryKingdom.BackgroundFloater();
			__53.Name = "Palace_Chandelier_Blurred";
			__53.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(25007.05f, 1964.642f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__53.MyQuad.Quad.v0.Pos = new Vector2(-0.9986112f, 1f);

			__53.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(26417.14f, 1964.642f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__53.MyQuad.Quad.v1.Pos = new Vector2(1.001389f, 1f);

			__53.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(25007.05f, -851.511f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__53.MyQuad.Quad.v2.Pos = new Vector2(-0.9986112f, -1f);

			__53.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(26417.14f, -851.511f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__53.MyQuad.Quad.v3.Pos = new Vector2(1.001389f, -1f);

			__53.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__53.MyQuad.Quad.ExtraTexture1 = null;
			__53.MyQuad.Quad.ExtraTexture2 = null;
			__53.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__53.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__53.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__53.MyQuad.Quad.BlendAddRatio = 0f;

			__53.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 25711.12f, 556.5653f);

			__53.uv_speed = new Vector2(0f, 0f);
			__53.uv_offset = new Vector2(0f, 0f);
			__53.Data = new PhsxData(25711.12f, 556.5653f, 0f, 0f, 0f, 0f);
			__53.StartData = new PhsxData(25711.12f, 556.5653f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__53);

			CloudberryKingdom.BackgroundFloater __54 = new CloudberryKingdom.BackgroundFloater();
			__54.Name = "Palace_Chandelier_Blurred";
			__54.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__54.MyQuad.Quad.ExtraTexture1 = null;
			__54.MyQuad.Quad.ExtraTexture2 = null;
			__54.MyQuad.Quad._MyTexture = Tools.Texture("Palace_Chandelier_Blurred");
			__54.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__54.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__54.MyQuad.Quad.BlendAddRatio = 0f;

			__54.MyQuad.Base = new CoreEngine.BasePoint(705.0453f, 0f, 0f, 1408.076f, 30284.59f, 635.1008f);

			__54.uv_speed = new Vector2(0f, 0f);
			__54.uv_offset = new Vector2(0f, 0f);
			__54.Data = new PhsxData(30284.59f, 635.1008f, 0f, 0f, 0f, 0f);
			__54.StartData = new PhsxData(30284.59f, 635.1008f, 0f, 0f, 0f, 0f);
			__43.Floaters.Add(__54);

			__43.Parallax = 0.55f;
			__43.DoPreDraw = false;
			b.MyCollection.Lists.Add(__43);

			b.Light = 1f;
			b.BL = new Vector2(-100000f, -10000f);
			b.TR = new Vector2(100000f, 10000f);


			foreach (var l in b.MyCollection.Lists)
				foreach (var f in l.Floaters)
				{
					if (f.MyQuad.Quad.MyTexture.Name == "Palace_Portrait1")
						f.MyQuad.TextureName = "Palace_Portrait" + Tools.GlobalRnd.RndInt(1, 6).ToString();
					f.MyQuad.Quad.SetColor(f.MyQuad.Quad.PremultipliedColor, true);
				}
		}
    }
}
