using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
		public static void _code_Terrace(Background b)
		{
			b.GuidCounter = 0;
			b.MyGlobalIllumination = 1f;
			b.AllowLava = true;
			CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
			__1.Name = "Layer";
			__1.Foreground = false;
			__1.Fixed = false;
			CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
			__2.Name = "sea_backdrop_p1_0";
			__2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__2.MyQuad.Quad.ExtraTexture1 = null;
			__2.MyQuad.Quad.ExtraTexture2 = null;
			__2.MyQuad.Quad._MyTexture = Tools.Texture("sea_backdrop");
			__2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__2.MyQuad.Quad.BlendAddRatio = 0f;

			__2.MyQuad.Base = new CoreEngine.BasePoint(40199f, 0f, 0f, 17007.27f, -3333.765f, 0f);

			__2.uv_speed = new Vector2(0f, 0f);
			__2.uv_offset = new Vector2(0f, 0f);
			__2.Data = new PhsxData(-3333.765f, 0f, 0f, 0f, 0f, 0f);
			__2.StartData = new PhsxData(-3333.765f, 0f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__2);

			CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
			__3.Name = "sea_backdrop_p2_0";
			__3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__3.MyQuad.Quad.ExtraTexture1 = null;
			__3.MyQuad.Quad.ExtraTexture2 = null;
			__3.MyQuad.Quad._MyTexture = Tools.Texture("sea_backdrop_p2");
			__3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__3.MyQuad.Quad.BlendAddRatio = 0f;

			__3.MyQuad.Base = new CoreEngine.BasePoint(40199f, 0f, 0f, 17007.27f, 77064.23f, 0f);

			__3.uv_speed = new Vector2(0f, 0f);
			__3.uv_offset = new Vector2(0f, 0f);
			__3.Data = new PhsxData(77064.23f, 0f, 0f, 0f, 0f, 0f);
			__3.StartData = new PhsxData(77064.23f, 0f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__3);

			__1.Parallax = 0.06f;
			__1.DoPreDraw = false;
			b.MyCollection.Lists.Add(__1);

			CloudberryKingdom.BackgroundFloaterList __4 = new CloudberryKingdom.BackgroundFloaterList();
			__4.Name = "Layer 8";
			__4.Foreground = false;
			__4.Fixed = false;
			CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
			__5.Name = "Terrace_Cloud2";
			__5.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-14144.96f, -603.5261f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__5.MyQuad.Quad.v0.Pos = new Vector2(-1.003611f, 1.020833f);

			__5.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(-883.8804f, -603.5261f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__5.MyQuad.Quad.v1.Pos = new Vector2(0.9963889f, 1.020833f);

			__5.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-14144.96f, -6029.465f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__5.MyQuad.Quad.v2.Pos = new Vector2(-1.003611f, -0.9791667f);

			__5.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(-883.8804f, -6029.465f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__5.MyQuad.Quad.v3.Pos = new Vector2(0.9963889f, -0.9791667f);

			__5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__5.MyQuad.Quad.ExtraTexture1 = null;
			__5.MyQuad.Quad.ExtraTexture2 = null;
			__5.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__5.MyQuad.Quad.BlendAddRatio = 0f;

			__5.MyQuad.Base = new CoreEngine.BasePoint(6630.539f, 0f, 0f, 2712.969f, -7490.476f, -3373.016f);

			__5.uv_speed = new Vector2(0f, 0f);
			__5.uv_offset = new Vector2(0f, 0f);
			__5.Data = new PhsxData(-7490.476f, -3373.016f, 0f, 0f, 0f, 0f);
			__5.StartData = new PhsxData(-7490.476f, -3373.016f, 0f, 0f, 0f, 0f);
			__4.Floaters.Add(__5);

			CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
			__6.Name = "Terrace_Cloud2";
			__6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__6.MyQuad.Quad.ExtraTexture1 = null;
			__6.MyQuad.Quad.ExtraTexture2 = null;
			__6.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__6.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__6.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__6.MyQuad.Quad.BlendAddRatio = 0f;

			__6.MyQuad.Base = new CoreEngine.BasePoint(9083.824f, 0f, 0f, 3716.762f, 21575.96f, -4403.265f);

			__6.uv_speed = new Vector2(0f, 0f);
			__6.uv_offset = new Vector2(0f, 0f);
			__6.Data = new PhsxData(21575.96f, -4403.265f, 0f, 0f, 0f, 0f);
			__6.StartData = new PhsxData(21575.96f, -4403.265f, 0f, 0f, 0f, 0f);
			__4.Floaters.Add(__6);

			__4.Parallax = 0.07f;
			__4.DoPreDraw = false;
			b.MyCollection.Lists.Add(__4);

			CloudberryKingdom.BackgroundFloaterList __7 = new CloudberryKingdom.BackgroundFloaterList();
			__7.Name = "Layer 6";
			__7.Foreground = false;
			__7.Fixed = false;
			CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
			__8.Name = "Terrace_Hill_Large";
			__8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__8.MyQuad.Quad.ExtraTexture1 = null;
			__8.MyQuad.Quad.ExtraTexture2 = null;
			__8.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Large");
			__8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__8.MyQuad.Quad.BlendAddRatio = 0f;

			__8.MyQuad.Base = new CoreEngine.BasePoint(13702.45f, 0f, 0f, 7744.309f, -15177.16f, -4969.754f);

			__8.uv_speed = new Vector2(0f, 0f);
			__8.uv_offset = new Vector2(0f, 0f);
			__8.Data = new PhsxData(-15177.16f, -4969.754f, 0f, 0f, 0f, 0f);
			__8.StartData = new PhsxData(-15177.16f, -4969.754f, 0f, 0f, 0f, 0f);
			__7.Floaters.Add(__8);

			CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
			__9.Name = "Terrace_Hill_Large";
			__9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__9.MyQuad.Quad.ExtraTexture1 = null;
			__9.MyQuad.Quad.ExtraTexture2 = null;
			__9.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Large");
			__9.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__9.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__9.MyQuad.Quad.BlendAddRatio = 0f;

			__9.MyQuad.Base = new CoreEngine.BasePoint(13702.45f, 0f, 0f, 7744.309f, 5953.091f, -5123.456f);

			__9.uv_speed = new Vector2(0f, 0f);
			__9.uv_offset = new Vector2(0f, 0f);
			__9.Data = new PhsxData(5953.091f, -5123.456f, 0f, 0f, 0f, 0f);
			__9.StartData = new PhsxData(5953.091f, -5123.456f, 0f, 0f, 0f, 0f);
			__7.Floaters.Add(__9);

			CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
			__10.Name = "Terrace_Hill_Large";
			__10.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(15846.03f, 3711.286f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__10.MyQuad.Quad.v0.Pos = new Vector2(-1.009166f, 1f);

			__10.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(43250.93f, 3711.286f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__10.MyQuad.Quad.v1.Pos = new Vector2(0.9908335f, 1f);

			__10.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(15846.03f, -11777.33f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__10.MyQuad.Quad.v2.Pos = new Vector2(-1.009166f, -1f);

			__10.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(43250.93f, -11777.33f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__10.MyQuad.Quad.v3.Pos = new Vector2(0.9908335f, -1f);

			__10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__10.MyQuad.Quad.ExtraTexture1 = null;
			__10.MyQuad.Quad.ExtraTexture2 = null;
			__10.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Large");
			__10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__10.MyQuad.Quad.BlendAddRatio = 0f;

			__10.MyQuad.Base = new CoreEngine.BasePoint(13702.45f, 0f, 0f, 7744.309f, 29674.08f, -4033.023f);

			__10.uv_speed = new Vector2(0f, 0f);
			__10.uv_offset = new Vector2(0f, 0f);
			__10.Data = new PhsxData(29674.08f, -4033.023f, 0f, 0f, 0f, 0f);
			__10.StartData = new PhsxData(29674.08f, -4033.023f, 0f, 0f, 0f, 0f);
			__7.Floaters.Add(__10);

			CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
			__11.Name = "Terrace_Hill_Large";
			__11.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__11.MyQuad.Quad.ExtraTexture1 = null;
			__11.MyQuad.Quad.ExtraTexture2 = null;
			__11.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Large");
			__11.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__11.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__11.MyQuad.Quad.BlendAddRatio = 0f;

			__11.MyQuad.Base = new CoreEngine.BasePoint(13702.45f, 0f, 0f, 7744.309f, 54490.69f, -3658.024f);

			__11.uv_speed = new Vector2(0f, 0f);
			__11.uv_offset = new Vector2(0f, 0f);
			__11.Data = new PhsxData(54490.69f, -3658.024f, 0f, 0f, 0f, 0f);
			__11.StartData = new PhsxData(54490.69f, -3658.024f, 0f, 0f, 0f, 0f);
			__7.Floaters.Add(__11);

			__7.Parallax = 0.09f;
			__7.DoPreDraw = false;
			b.MyCollection.Lists.Add(__7);

			CloudberryKingdom.BackgroundFloaterList __12 = new CloudberryKingdom.BackgroundFloaterList();
			__12.Name = "Layer 7";
			__12.Foreground = false;
			__12.Fixed = false;
			CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
			__13.Name = "Terrace_Cloud2";
			__13.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-8956.773f, -2845.825f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__13.MyQuad.Quad.v0.Pos = new Vector2(-1f, 0.9997222f);

			__13.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(-775.0391f, -2845.825f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__13.MyQuad.Quad.v1.Pos = new Vector2(1f, 0.9997222f);

			__13.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-8956.773f, -6193.486f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__13.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1.000278f);

			__13.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(-775.0391f, -6193.486f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__13.MyQuad.Quad.v3.Pos = new Vector2(1f, -1.000278f);

			__13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__13.MyQuad.Quad.ExtraTexture1 = null;
			__13.MyQuad.Quad.ExtraTexture2 = null;
			__13.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__13.MyQuad.Quad.BlendAddRatio = 0f;

			__13.MyQuad.Base = new CoreEngine.BasePoint(4090.867f, 0f, 0f, 1673.83f, -4865.906f, -4519.19f);

			__13.uv_speed = new Vector2(0f, 0f);
			__13.uv_offset = new Vector2(0f, 0f);
			__13.Data = new PhsxData(-4865.906f, -4519.19f, 0f, 0f, 0f, 0f);
			__13.StartData = new PhsxData(-4865.906f, -4519.19f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__13);

			CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
			__14.Name = "Terrace_Cloud2";
			__14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__14.MyQuad.Quad.ExtraTexture1 = null;
			__14.MyQuad.Quad.ExtraTexture2 = null;
			__14.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__14.MyQuad.Quad.BlendAddRatio = 0f;

			__14.MyQuad.Base = new CoreEngine.BasePoint(3671.715f, 0f, 0f, 1502.329f, 13591.17f, -3923.232f);

			__14.uv_speed = new Vector2(0f, 0f);
			__14.uv_offset = new Vector2(0f, 0f);
			__14.Data = new PhsxData(13591.17f, -3923.232f, 0f, 0f, 0f, 0f);
			__14.StartData = new PhsxData(13591.17f, -3923.232f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__14);

			CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
			__15.Name = "Terrace_Cloud1";
			__15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__15.MyQuad.Quad.ExtraTexture1 = null;
			__15.MyQuad.Quad.ExtraTexture2 = null;
			__15.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__15.MyQuad.Quad.BlendAddRatio = 0f;

			__15.MyQuad.Base = new CoreEngine.BasePoint(4520.197f, 0f, 0f, 1849.496f, 35845.96f, -4088.887f);

			__15.uv_speed = new Vector2(0f, 0f);
			__15.uv_offset = new Vector2(0f, 0f);
			__15.Data = new PhsxData(35845.96f, -4088.887f, 0f, 0f, 0f, 0f);
			__15.StartData = new PhsxData(35845.96f, -4088.887f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__15);

			CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
			__16.Name = "Terrace_Cloud6";
			__16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__16.MyQuad.Quad.ExtraTexture1 = null;
			__16.MyQuad.Quad.ExtraTexture2 = null;
			__16.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud6");
			__16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__16.MyQuad.Quad.BlendAddRatio = 0f;

			__16.MyQuad.Base = new CoreEngine.BasePoint(3949.491f, 0f, 0f, 1986.073f, 19775.26f, -4709.342f);

			__16.uv_speed = new Vector2(0f, 0f);
			__16.uv_offset = new Vector2(0f, 0f);
			__16.Data = new PhsxData(19775.26f, -4709.342f, 0f, 0f, 0f, 0f);
			__16.StartData = new PhsxData(19775.26f, -4709.342f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__16);

			__12.Parallax = 0.11f;
			__12.DoPreDraw = false;
			b.MyCollection.Lists.Add(__12);

			CloudberryKingdom.BackgroundFloaterList __17 = new CloudberryKingdom.BackgroundFloaterList();
			__17.Name = "Layer";
			__17.Foreground = false;
			__17.Fixed = false;
			CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
			__18.Name = "Terrace_Hill_Medium";
			__18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__18.MyQuad.Quad.ExtraTexture1 = null;
			__18.MyQuad.Quad.ExtraTexture2 = null;
			__18.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__18.MyQuad.Quad.BlendAddRatio = 0f;

			__18.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 6237.038f, -4055.554f);

			__18.uv_speed = new Vector2(0f, 0f);
			__18.uv_offset = new Vector2(0f, 0f);
			__18.Data = new PhsxData(6237.038f, -4055.554f, 0f, 0f, 0f, 0f);
			__18.StartData = new PhsxData(6237.038f, -4055.554f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__18);

			CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
			__19.Name = "Terrace_Hill_Medium";
			__19.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(17555.48f, -2271.431f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__19.MyQuad.Quad.v0.Pos = new Vector2(-1.049167f, 1.011944f);

			__19.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(27577.7f, -2271.431f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__19.MyQuad.Quad.v1.Pos = new Vector2(0.9508333f, 1.011944f);

			__19.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(17555.48f, -8112.502f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__19.MyQuad.Quad.v2.Pos = new Vector2(-1.049167f, -0.9880555f);

			__19.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(27577.7f, -8112.502f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__19.MyQuad.Quad.v3.Pos = new Vector2(0.9508333f, -0.9880555f);

			__19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__19.MyQuad.Quad.ExtraTexture1 = null;
			__19.MyQuad.Quad.ExtraTexture2 = null;
			__19.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__19.MyQuad.Quad.BlendAddRatio = 0f;

			__19.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 22812.97f, -5226.851f);

			__19.uv_speed = new Vector2(0f, 0f);
			__19.uv_offset = new Vector2(0f, 0f);
			__19.Data = new PhsxData(22812.97f, -5226.851f, 0f, 0f, 0f, 0f);
			__19.StartData = new PhsxData(22812.97f, -5226.851f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__19);

			CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
			__20.Name = "Terrace_Hill_Medium";
			__20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__20.MyQuad.Quad.ExtraTexture1 = null;
			__20.MyQuad.Quad.ExtraTexture2 = null;
			__20.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__20.MyQuad.Quad.BlendAddRatio = 0f;

			__20.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, -7924.074f, -5605.555f);

			__20.uv_speed = new Vector2(0f, 0f);
			__20.uv_offset = new Vector2(0f, 0f);
			__20.Data = new PhsxData(-7924.074f, -5605.555f, 0f, 0f, 0f, 0f);
			__20.StartData = new PhsxData(-7924.074f, -5605.555f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__20);

			CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
			__21.Name = "Terrace_Hill_Medium";
			__21.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(9069.896f, -1704.52f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__21.MyQuad.Quad.v0.Pos = new Vector2(-1.001944f, 1.015833f);

			__21.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(19092.11f, -1704.52f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__21.MyQuad.Quad.v1.Pos = new Vector2(0.9980555f, 1.015833f);

			__21.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(9069.896f, -7545.592f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__21.MyQuad.Quad.v2.Pos = new Vector2(-1.001944f, -0.9841667f);

			__21.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(19092.11f, -7545.592f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__21.MyQuad.Quad.v3.Pos = new Vector2(0.9980555f, -0.9841667f);

			__21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__21.MyQuad.Quad.ExtraTexture1 = null;
			__21.MyQuad.Quad.ExtraTexture2 = null;
			__21.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__21.MyQuad.Quad.BlendAddRatio = 0f;

			__21.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 14090.75f, -4671.298f);

			__21.uv_speed = new Vector2(0f, 0f);
			__21.uv_offset = new Vector2(0f, 0f);
			__21.Data = new PhsxData(14090.75f, -4671.298f, 0f, 0f, 0f, 0f);
			__21.StartData = new PhsxData(14090.75f, -4671.298f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__21);

			CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
			__22.Name = "Terrace_Hill_Medium";
			__22.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(34736.84f, -1105.319f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v0.Pos = new Vector2(-1.018889f, 1.017778f);

			__22.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(44759.05f, -1105.319f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v1.Pos = new Vector2(0.9811109f, 1.017778f);

			__22.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(34736.84f, -6946.391f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v2.Pos = new Vector2(-1.018889f, -0.9822223f);

			__22.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(44759.05f, -6946.391f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__22.MyQuad.Quad.v3.Pos = new Vector2(0.9811109f, -0.9822223f);

			__22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__22.MyQuad.Quad.ExtraTexture1 = null;
			__22.MyQuad.Quad.ExtraTexture2 = null;
			__22.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__22.MyQuad.Quad.BlendAddRatio = 0f;

			__22.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 39842.61f, -4077.775f);

			__22.uv_speed = new Vector2(0f, 0f);
			__22.uv_offset = new Vector2(0f, 0f);
			__22.Data = new PhsxData(39842.61f, -4077.775f, 0f, 0f, 0f, 0f);
			__22.StartData = new PhsxData(39842.61f, -4077.775f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__22);

			CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
			__23.Name = "Terrace_Hill_Medium";
			__23.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(26034.23f, -1522.747f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__23.MyQuad.Quad.v0.Pos = new Vector2(-1.003889f, 1.001667f);

			__23.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(36056.45f, -1522.747f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__23.MyQuad.Quad.v1.Pos = new Vector2(0.9961109f, 1.001667f);

			__23.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(26034.23f, -7363.817f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__23.MyQuad.Quad.v2.Pos = new Vector2(-1.003889f, -0.9983333f);

			__23.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(36056.45f, -7363.817f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__23.MyQuad.Quad.v3.Pos = new Vector2(0.9961109f, -0.9983333f);

			__23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__23.MyQuad.Quad.ExtraTexture1 = null;
			__23.MyQuad.Quad.ExtraTexture2 = null;
			__23.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__23.MyQuad.Quad.BlendAddRatio = 0f;

			__23.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 31064.83f, -4448.149f);

			__23.uv_speed = new Vector2(0f, 0f);
			__23.uv_offset = new Vector2(0f, 0f);
			__23.Data = new PhsxData(31064.83f, -4448.149f, 0f, 0f, 0f, 0f);
			__23.StartData = new PhsxData(31064.83f, -4448.149f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__23);

			CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
			__24.Name = "Terrace_Hill_Medium";
			__24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__24.MyQuad.Quad.ExtraTexture1 = null;
			__24.MyQuad.Quad.ExtraTexture2 = null;
			__24.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__24.MyQuad.Quad.BlendAddRatio = 0f;

			__24.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 48916.65f, -4059.259f);

			__24.uv_speed = new Vector2(0f, 0f);
			__24.uv_offset = new Vector2(0f, 0f);
			__24.Data = new PhsxData(48916.65f, -4059.259f, 0f, 0f, 0f, 0f);
			__24.StartData = new PhsxData(48916.65f, -4059.259f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__24);

			CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
			__25.Name = "Terrace_Hill_Medium";
			__25.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(52536.59f, -762.7981f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__25.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__25.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(62558.8f, -762.7981f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__25.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__25.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(52536.59f, -6603.869f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__25.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__25.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(62558.8f, -6603.869f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__25.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__25.MyQuad.Quad.ExtraTexture1 = null;
			__25.MyQuad.Quad.ExtraTexture2 = null;
			__25.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Medium");
			__25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__25.MyQuad.Quad.BlendAddRatio = 0f;

			__25.MyQuad.Base = new CoreEngine.BasePoint(5011.106f, 0f, 0f, 2920.535f, 57546.3f, -3683.333f);

			__25.uv_speed = new Vector2(0f, 0f);
			__25.uv_offset = new Vector2(0f, 0f);
			__25.Data = new PhsxData(57546.3f, -3683.333f, 0f, 0f, 0f, 0f);
			__25.StartData = new PhsxData(57546.3f, -3683.333f, 0f, 0f, 0f, 0f);
			__17.Floaters.Add(__25);

			__17.Parallax = 0.15f;
			__17.DoPreDraw = false;
			b.MyCollection.Lists.Add(__17);

			CloudberryKingdom.BackgroundFloaterList __26 = new CloudberryKingdom.BackgroundFloaterList();
			__26.Name = "Layer";
			__26.Foreground = false;
			__26.Fixed = false;
			CloudberryKingdom.BackgroundFloater __27 = new CloudberryKingdom.BackgroundFloater();
			__27.Name = "Terrace_Cloud2";
			__27.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-488.0991f, -1031.782f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v0.Pos = new Vector2(-1.005556f, 0.9791667f);

			__27.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(3459.263f, -1031.782f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v1.Pos = new Vector2(0.9944444f, 0.9791667f);

			__27.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-488.0991f, -2646.896f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v2.Pos = new Vector2(-1.005556f, -1.020833f);

			__27.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(3459.263f, -2646.896f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__27.MyQuad.Quad.v3.Pos = new Vector2(0.9944444f, -1.020833f);

			__27.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__27.MyQuad.Quad.ExtraTexture1 = null;
			__27.MyQuad.Quad.ExtraTexture2 = null;
			__27.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__27.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__27.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__27.MyQuad.Quad.BlendAddRatio = 0f;

			__27.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 807.5567f, 1496.547f, -1822.515f);

			__27.uv_speed = new Vector2(0f, 0f);
			__27.uv_offset = new Vector2(0f, 0f);
			__27.Data = new PhsxData(1496.547f, -1822.515f, 0f, 0f, 0f, 0f);
			__27.StartData = new PhsxData(1496.547f, -1822.515f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__27);

			CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
			__28.Name = "Terrace_Cloud3";
			__28.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(6829.15f, -2735.527f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__28.MyQuad.Quad.v0.Pos = new Vector2(-0.9277778f, 0.91f);

			__28.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(10776.51f, -2735.527f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__28.MyQuad.Quad.v1.Pos = new Vector2(1.072222f, 0.91f);

			__28.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(6829.15f, -4624.863f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__28.MyQuad.Quad.v2.Pos = new Vector2(-0.9277778f, -1.09f);

			__28.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(10776.51f, -4624.863f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__28.MyQuad.Quad.v3.Pos = new Vector2(1.072222f, -1.09f);

			__28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__28.MyQuad.Quad.ExtraTexture1 = null;
			__28.MyQuad.Quad.ExtraTexture2 = null;
			__28.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__28.MyQuad.Quad.BlendAddRatio = 0f;

			__28.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 944.6678f, 8660.287f, -3595.175f);

			__28.uv_speed = new Vector2(0f, 0f);
			__28.uv_offset = new Vector2(0f, 0f);
			__28.Data = new PhsxData(8660.287f, -3595.175f, 0f, 0f, 0f, 0f);
			__28.StartData = new PhsxData(8660.287f, -3595.175f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__28);

			CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
			__29.Name = "Terrace_Cloud4";
			__29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__29.MyQuad.Quad.ExtraTexture1 = null;
			__29.MyQuad.Quad.ExtraTexture2 = null;
			__29.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud4");
			__29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__29.MyQuad.Quad.BlendAddRatio = 0f;

			__29.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 1278.518f, -3360.912f, -3434.356f);

			__29.uv_speed = new Vector2(0f, 0f);
			__29.uv_offset = new Vector2(0f, 0f);
			__29.Data = new PhsxData(-3360.912f, -3434.356f, 0f, 0f, 0f, 0f);
			__29.StartData = new PhsxData(-3360.912f, -3434.356f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__29);

			CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
			__30.Name = "Terrace_Cloud4";
			__30.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(13445.21f, -2113.135f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v0.Pos = new Vector2(-0.9497222f, 0.9133334f);

			__30.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(17392.57f, -2113.135f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v1.Pos = new Vector2(1.050278f, 0.9133334f);

			__30.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(13445.21f, -4670.169f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v2.Pos = new Vector2(-0.9497222f, -1.086667f);

			__30.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(17392.57f, -4670.169f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__30.MyQuad.Quad.v3.Pos = new Vector2(1.050278f, -1.086667f);

			__30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__30.MyQuad.Quad.ExtraTexture1 = null;
			__30.MyQuad.Quad.ExtraTexture2 = null;
			__30.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud4");
			__30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__30.MyQuad.Quad.BlendAddRatio = 0f;

			__30.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 1278.517f, 15319.65f, -3280.847f);

			__30.uv_speed = new Vector2(0f, 0f);
			__30.uv_offset = new Vector2(0f, 0f);
			__30.Data = new PhsxData(15319.65f, -3280.847f, 0f, 0f, 0f, 0f);
			__30.StartData = new PhsxData(15319.65f, -3280.847f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__30);

			CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
			__31.Name = "Terrace_Cloud2";
			__31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__31.MyQuad.Quad.ExtraTexture1 = null;
			__31.MyQuad.Quad.ExtraTexture2 = null;
			__31.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__31.MyQuad.Quad.BlendAddRatio = 0f;

			__31.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 807.5567f, 24833.54f, -3383.186f);

			__31.uv_speed = new Vector2(0f, 0f);
			__31.uv_offset = new Vector2(0f, 0f);
			__31.Data = new PhsxData(24833.54f, -3383.186f, 0f, 0f, 0f, 0f);
			__31.StartData = new PhsxData(24833.54f, -3383.186f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__31);

			CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
			__32.Name = "Terrace_Cloud6";
			__32.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(32170.71f, -2079.23f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__32.MyQuad.Quad.v0.Pos = new Vector2(-0.9547223f, 0.9602777f);

			__32.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(36118.08f, -2079.23f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__32.MyQuad.Quad.v1.Pos = new Vector2(1.045278f, 0.9602777f);

			__32.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(32170.71f, -4064.232f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__32.MyQuad.Quad.v2.Pos = new Vector2(-0.9547223f, -1.039722f);

			__32.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(36118.08f, -4064.232f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__32.MyQuad.Quad.v3.Pos = new Vector2(1.045278f, -1.039722f);

			__32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__32.MyQuad.Quad.ExtraTexture1 = null;
			__32.MyQuad.Quad.ExtraTexture2 = null;
			__32.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud6");
			__32.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__32.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__32.MyQuad.Quad.BlendAddRatio = 0f;

			__32.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 992.501f, 34055.03f, -3032.307f);

			__32.uv_speed = new Vector2(0f, 0f);
			__32.uv_offset = new Vector2(0f, 0f);
			__32.Data = new PhsxData(34055.03f, -3032.307f, 0f, 0f, 0f, 0f);
			__32.StartData = new PhsxData(34055.03f, -3032.307f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__32);

			CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
			__33.Name = "Terrace_Cloud2";
			__33.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(41532.55f, -2126.003f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v0.Pos = new Vector2(-1.000278f, 0.9955556f);

			__33.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(45479.91f, -2126.003f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v1.Pos = new Vector2(0.9997222f, 0.9955556f);

			__33.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(41532.55f, -3741.116f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v2.Pos = new Vector2(-1.000278f, -1.004444f);

			__33.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(45479.91f, -3741.116f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__33.MyQuad.Quad.v3.Pos = new Vector2(0.9997222f, -1.004444f);

			__33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__33.MyQuad.Quad.ExtraTexture1 = null;
			__33.MyQuad.Quad.ExtraTexture2 = null;
			__33.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__33.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__33.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__33.MyQuad.Quad.BlendAddRatio = 0f;

			__33.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 807.5567f, 43506.78f, -2929.971f);

			__33.uv_speed = new Vector2(0f, 0f);
			__33.uv_offset = new Vector2(0f, 0f);
			__33.Data = new PhsxData(43506.78f, -2929.971f, 0f, 0f, 0f, 0f);
			__33.StartData = new PhsxData(43506.78f, -2929.971f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__33);

			CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
			__34.Name = "Terrace_Cloud2";
			__34.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(47505.14f, -2405.657f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v0.Pos = new Vector2(-0.9519442f, 0.8755557f);

			__34.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(51452.5f, -2405.657f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v1.Pos = new Vector2(1.048056f, 0.8755557f);

			__34.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(47505.14f, -4020.771f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v2.Pos = new Vector2(-0.9519442f, -1.124444f);

			__34.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(51452.5f, -4020.771f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__34.MyQuad.Quad.v3.Pos = new Vector2(1.048056f, -1.124444f);

			__34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__34.MyQuad.Quad.ExtraTexture1 = null;
			__34.MyQuad.Quad.ExtraTexture2 = null;
			__34.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__34.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__34.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__34.MyQuad.Quad.BlendAddRatio = 0f;

			__34.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 807.5567f, 49383.97f, -3112.718f);

			__34.uv_speed = new Vector2(0f, 0f);
			__34.uv_offset = new Vector2(0f, 0f);
			__34.Data = new PhsxData(49383.97f, -3112.718f, 0f, 0f, 0f, 0f);
			__34.StartData = new PhsxData(49383.97f, -3112.718f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__34);

			CloudberryKingdom.BackgroundFloater __35 = new CloudberryKingdom.BackgroundFloater();
			__35.Name = "Terrace_Cloud1";
			__35.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(58123.03f, -2496.834f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__35.MyQuad.Quad.v0.Pos = new Vector2(-0.8722222f, 0.8622223f);

			__35.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(62070.39f, -2496.834f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__35.MyQuad.Quad.v1.Pos = new Vector2(1.127778f, 0.8622223f);

			__35.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(58123.03f, -4111.947f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__35.MyQuad.Quad.v2.Pos = new Vector2(-0.8722222f, -1.137778f);

			__35.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(62070.39f, -4111.947f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__35.MyQuad.Quad.v3.Pos = new Vector2(1.127778f, -1.137778f);

			__35.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__35.MyQuad.Quad.ExtraTexture1 = null;
			__35.MyQuad.Quad.ExtraTexture2 = null;
			__35.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__35.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__35.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__35.MyQuad.Quad.BlendAddRatio = 0f;

			__35.MyQuad.Base = new CoreEngine.BasePoint(1973.681f, 0f, 0f, 807.5567f, 59844.52f, -3193.127f);

			__35.uv_speed = new Vector2(0f, 0f);
			__35.uv_offset = new Vector2(0f, 0f);
			__35.Data = new PhsxData(59844.52f, -3193.127f, 0f, 0f, 0f, 0f);
			__35.StartData = new PhsxData(59844.52f, -3193.127f, 0f, 0f, 0f, 0f);
			__26.Floaters.Add(__35);

			__26.Parallax = 0.19f;
			__26.DoPreDraw = false;
			b.MyCollection.Lists.Add(__26);

			CloudberryKingdom.BackgroundFloaterList __36 = new CloudberryKingdom.BackgroundFloaterList();
			__36.Name = "Layer";
			__36.Foreground = false;
			__36.Fixed = false;
			CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
			__37.Name = "Terrace_Hill_Small";
			__37.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1525.995f, -1584.432f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__37.MyQuad.Quad.v0.Pos = new Vector2(-1.164167f, 1.069444f);

			__37.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(3589.949f, -1584.432f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__37.MyQuad.Quad.v1.Pos = new Vector2(0.8358335f, 1.069444f);

			__37.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1525.995f, -4554.031f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__37.MyQuad.Quad.v2.Pos = new Vector2(-1.164167f, -0.9305556f);

			__37.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(3589.949f, -4554.031f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__37.MyQuad.Quad.v3.Pos = new Vector2(0.8358335f, -0.9305556f);

			__37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__37.MyQuad.Quad.ExtraTexture1 = null;
			__37.MyQuad.Quad.ExtraTexture2 = null;
			__37.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__37.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__37.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__37.MyQuad.Quad.BlendAddRatio = 0f;

			__37.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 1451.91f, -3172.342f);

			__37.uv_speed = new Vector2(0f, 0f);
			__37.uv_offset = new Vector2(0f, 0f);
			__37.Data = new PhsxData(1451.91f, -3172.342f, 0f, 0f, 0f, 0f);
			__37.StartData = new PhsxData(1451.91f, -3172.342f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__37);

			CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
			__38.Name = "Terrace_Hill_Small";
			__38.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(6739.627f, -2531.771f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v0.Pos = new Vector2(-1.010278f, 1.021944f);

			__38.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(11855.57f, -2531.771f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v1.Pos = new Vector2(0.9897221f, 1.021944f);

			__38.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(6739.627f, -5501.37f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v2.Pos = new Vector2(-1.010278f, -0.9780556f);

			__38.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(11855.57f, -5501.37f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__38.MyQuad.Quad.v3.Pos = new Vector2(0.9897221f, -0.9780556f);

			__38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__38.MyQuad.Quad.ExtraTexture1 = null;
			__38.MyQuad.Quad.ExtraTexture2 = null;
			__38.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__38.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__38.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__38.MyQuad.Quad.BlendAddRatio = 0f;

			__38.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 9323.889f, -4049.154f);

			__38.uv_speed = new Vector2(0f, 0f);
			__38.uv_offset = new Vector2(0f, 0f);
			__38.Data = new PhsxData(9323.889f, -4049.154f, 0f, 0f, 0f, 0f);
			__38.StartData = new PhsxData(9323.889f, -4049.154f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__38);

			CloudberryKingdom.BackgroundFloater __39 = new CloudberryKingdom.BackgroundFloater();
			__39.Name = "Terrace_Hill_Small";
			__39.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(9739.074f, -2814.911f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__39.MyQuad.Quad.v0.Pos = new Vector2(-0.9897225f, 1.018333f);

			__39.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(14855.02f, -2814.911f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__39.MyQuad.Quad.v1.Pos = new Vector2(1.010277f, 1.018333f);

			__39.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(9739.074f, -5784.51f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__39.MyQuad.Quad.v2.Pos = new Vector2(-0.9897225f, -0.9816667f);

			__39.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(14855.02f, -5784.51f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__39.MyQuad.Quad.v3.Pos = new Vector2(1.010277f, -0.9816667f);

			__39.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__39.MyQuad.Quad.ExtraTexture1 = null;
			__39.MyQuad.Quad.ExtraTexture2 = null;
			__39.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__39.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__39.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__39.MyQuad.Quad.BlendAddRatio = 0f;

			__39.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 12270.76f, -4326.932f);

			__39.uv_speed = new Vector2(0f, 0f);
			__39.uv_offset = new Vector2(0f, 0f);
			__39.Data = new PhsxData(12270.76f, -4326.932f, 0f, 0f, 0f, 0f);
			__39.StartData = new PhsxData(12270.76f, -4326.932f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__39);

			CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
			__40.Name = "Terrace_Hill_Small";
			__40.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(13220.88f, -2508.622f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v0.Pos = new Vector2(-0.9977778f, 1.005f);

			__40.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(18336.83f, -2508.622f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v1.Pos = new Vector2(1.002222f, 1.005f);

			__40.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(13220.88f, -5478.22f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v2.Pos = new Vector2(-0.9977778f, -0.995f);

			__40.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(18336.83f, -5478.22f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__40.MyQuad.Quad.v3.Pos = new Vector2(1.002222f, -0.995f);

			__40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__40.MyQuad.Quad.ExtraTexture1 = null;
			__40.MyQuad.Quad.ExtraTexture2 = null;
			__40.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__40.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__40.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__40.MyQuad.Quad.BlendAddRatio = 0f;

			__40.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 15773.17f, -4000.845f);

			__40.uv_speed = new Vector2(0f, 0f);
			__40.uv_offset = new Vector2(0f, 0f);
			__40.Data = new PhsxData(15773.17f, -4000.845f, 0f, 0f, 0f, 0f);
			__40.StartData = new PhsxData(15773.17f, -4000.845f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__40);

			CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
			__41.Name = "Terrace_Hill_Small";
			__41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__41.MyQuad.Quad.ExtraTexture1 = null;
			__41.MyQuad.Quad.ExtraTexture2 = null;
			__41.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__41.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__41.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__41.MyQuad.Quad.BlendAddRatio = 0f;

			__41.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 22488.14f, -3529.831f);

			__41.uv_speed = new Vector2(0f, 0f);
			__41.uv_offset = new Vector2(0f, 0f);
			__41.Data = new PhsxData(22488.14f, -3529.831f, 0f, 0f, 0f, 0f);
			__41.StartData = new PhsxData(22488.14f, -3529.831f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__41);

			CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
			__42.Name = "Terrace_Hill_Small";
			__42.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(28926.04f, -2087.566f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__42.MyQuad.Quad.v0.Pos = new Vector2(-1.004444f, 1.003889f);

			__42.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(34041.99f, -2087.566f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__42.MyQuad.Quad.v1.Pos = new Vector2(0.9955555f, 1.003889f);

			__42.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(28926.04f, -5057.165f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__42.MyQuad.Quad.v2.Pos = new Vector2(-1.004444f, -0.9961111f);

			__42.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(34041.99f, -5057.165f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__42.MyQuad.Quad.v3.Pos = new Vector2(0.9955555f, -0.9961111f);

			__42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__42.MyQuad.Quad.ExtraTexture1 = null;
			__42.MyQuad.Quad.ExtraTexture2 = null;
			__42.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__42.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__42.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__42.MyQuad.Quad.BlendAddRatio = 0f;

			__42.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 31495.38f, -3578.14f);

			__42.uv_speed = new Vector2(0f, 0f);
			__42.uv_offset = new Vector2(0f, 0f);
			__42.Data = new PhsxData(31495.38f, -3578.14f, 0f, 0f, 0f, 0f);
			__42.StartData = new PhsxData(31495.38f, -3578.14f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__42);

			CloudberryKingdom.BackgroundFloater __43 = new CloudberryKingdom.BackgroundFloater();
			__43.Name = "Terrace_Hill_Small";
			__43.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(23861.39f, -1657.85f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__43.MyQuad.Quad.v0.Pos = new Vector2(-1.001389f, 1.008611f);

			__43.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(28977.33f, -1657.85f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__43.MyQuad.Quad.v1.Pos = new Vector2(0.9986113f, 1.008611f);

			__43.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(23861.39f, -4627.448f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__43.MyQuad.Quad.v2.Pos = new Vector2(-1.001389f, -0.9913889f);

			__43.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(28977.33f, -4627.448f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__43.MyQuad.Quad.v3.Pos = new Vector2(0.9986113f, -0.9913889f);

			__43.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__43.MyQuad.Quad.ExtraTexture1 = null;
			__43.MyQuad.Quad.ExtraTexture2 = null;
			__43.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__43.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__43.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__43.MyQuad.Quad.BlendAddRatio = 0f;

			__43.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 26422.91f, -3155.435f);

			__43.uv_speed = new Vector2(0f, 0f);
			__43.uv_offset = new Vector2(0f, 0f);
			__43.Data = new PhsxData(26422.91f, -3155.435f, 0f, 0f, 0f, 0f);
			__43.StartData = new PhsxData(26422.91f, -3155.435f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__43);

			CloudberryKingdom.BackgroundFloater __44 = new CloudberryKingdom.BackgroundFloater();
			__44.Name = "Terrace_Hill_Small";
			__44.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(35798.73f, -1946.763f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v0.Pos = new Vector2(-0.9994445f, 1.001111f);

			__44.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(40914.67f, -1946.763f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v1.Pos = new Vector2(1.000556f, 1.001111f);

			__44.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(35798.73f, -4916.361f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v2.Pos = new Vector2(-0.9994445f, -0.9988889f);

			__44.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(40914.67f, -4916.361f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__44.MyQuad.Quad.v3.Pos = new Vector2(1.000556f, -0.9988889f);

			__44.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__44.MyQuad.Quad.ExtraTexture1 = null;
			__44.MyQuad.Quad.ExtraTexture2 = null;
			__44.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__44.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__44.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__44.MyQuad.Quad.BlendAddRatio = 0f;

			__44.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 38355.28f, -3433.212f);

			__44.uv_speed = new Vector2(0f, 0f);
			__44.uv_offset = new Vector2(0f, 0f);
			__44.Data = new PhsxData(38355.28f, -3433.212f, 0f, 0f, 0f, 0f);
			__44.StartData = new PhsxData(38355.28f, -3433.212f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__44);

			CloudberryKingdom.BackgroundFloater __45 = new CloudberryKingdom.BackgroundFloater();
			__45.Name = "Terrace_Hill_Small";
			__45.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(42966.29f, -1271.314f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__45.MyQuad.Quad.v0.Pos = new Vector2(-0.9641665f, 1.021667f);

			__45.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(48082.23f, -1271.314f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__45.MyQuad.Quad.v1.Pos = new Vector2(1.035833f, 1.021667f);

			__45.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(42966.29f, -4240.912f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__45.MyQuad.Quad.v2.Pos = new Vector2(-0.9641665f, -0.9783334f);

			__45.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(48082.23f, -4240.912f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__45.MyQuad.Quad.v3.Pos = new Vector2(1.035833f, -0.9783334f);

			__45.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__45.MyQuad.Quad.ExtraTexture1 = null;
			__45.MyQuad.Quad.ExtraTexture2 = null;
			__45.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__45.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__45.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__45.MyQuad.Quad.BlendAddRatio = 0f;

			__45.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 45432.59f, -2788.284f);

			__45.uv_speed = new Vector2(0f, 0f);
			__45.uv_offset = new Vector2(0f, 0f);
			__45.Data = new PhsxData(45432.59f, -2788.284f, 0f, 0f, 0f, 0f);
			__45.StartData = new PhsxData(45432.59f, -2788.284f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__45);

			CloudberryKingdom.BackgroundFloater __46 = new CloudberryKingdom.BackgroundFloater();
			__46.Name = "Terrace_Hill_Small";
			__46.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__46.MyQuad.Quad.ExtraTexture1 = null;
			__46.MyQuad.Quad.ExtraTexture2 = null;
			__46.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Hill_Small");
			__46.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.BlendAddRatio = 0f;

			__46.MyQuad.Base = new CoreEngine.BasePoint(2557.971f, 0f, 0f, 1484.799f, 51978.5f, -2655.434f);

			__46.uv_speed = new Vector2(0f, 0f);
			__46.uv_offset = new Vector2(0f, 0f);
			__46.Data = new PhsxData(51978.5f, -2655.434f, 0f, 0f, 0f, 0f);
			__46.StartData = new PhsxData(51978.5f, -2655.434f, 0f, 0f, 0f, 0f);
			__36.Floaters.Add(__46);

			__36.Parallax = 0.23f;
			__36.DoPreDraw = false;
			b.MyCollection.Lists.Add(__36);

			CloudberryKingdom.BackgroundFloaterList __47 = new CloudberryKingdom.BackgroundFloaterList();
			__47.Name = "Layer";
			__47.Foreground = false;
			__47.Fixed = false;
			CloudberryKingdom.BackgroundFloater __48 = new CloudberryKingdom.BackgroundFloater();
			__48.Name = "Terrace_Cloud1";
			__48.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(38972.57f, -2085.065f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v0.Pos = new Vector2(-1.093611f, 1.0125f);

			__48.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(41972.56f, -2085.065f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v1.Pos = new Vector2(0.906389f, 1.0125f);

			__48.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(38972.57f, -3312.551f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v2.Pos = new Vector2(-1.093611f, -0.9875f);

			__48.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(41972.56f, -3312.551f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__48.MyQuad.Quad.v3.Pos = new Vector2(0.906389f, -0.9875f);

			__48.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__48.MyQuad.Quad.ExtraTexture1 = null;
			__48.MyQuad.Quad.ExtraTexture2 = null;
			__48.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__48.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__48.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__48.MyQuad.Quad.BlendAddRatio = 0f;

			__48.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 40612.98f, -2706.479f);

			__48.uv_speed = new Vector2(0f, 0f);
			__48.uv_offset = new Vector2(0f, 0f);
			__48.Data = new PhsxData(40612.98f, -2706.479f, 0f, 0f, 0f, 0f);
			__48.StartData = new PhsxData(40612.98f, -2706.479f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__48);

			CloudberryKingdom.BackgroundFloater __49 = new CloudberryKingdom.BackgroundFloater();
			__49.Name = "Terrace_Cloud1";
			__49.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(41939.61f, -2275.077f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v0.Pos = new Vector2(-1.06f, 1.019722f);

			__49.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(44939.6f, -2275.077f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v1.Pos = new Vector2(0.9400001f, 1.019722f);

			__49.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(41939.61f, -3502.563f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v2.Pos = new Vector2(-1.06f, -0.9802777f);

			__49.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(44939.6f, -3502.563f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__49.MyQuad.Quad.v3.Pos = new Vector2(0.9400001f, -0.9802777f);

			__49.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__49.MyQuad.Quad.ExtraTexture1 = null;
			__49.MyQuad.Quad.ExtraTexture2 = null;
			__49.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__49.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__49.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__49.MyQuad.Quad.BlendAddRatio = 0f;

			__49.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 43529.61f, -2900.925f);

			__49.uv_speed = new Vector2(0f, 0f);
			__49.uv_offset = new Vector2(0f, 0f);
			__49.Data = new PhsxData(43529.61f, -2900.925f, 0f, 0f, 0f, 0f);
			__49.StartData = new PhsxData(43529.61f, -2900.925f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__49);

			CloudberryKingdom.BackgroundFloater __50 = new CloudberryKingdom.BackgroundFloater();
			__50.Name = "Terrace_Cloud1";
			__50.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(49414.34f, -1988.328f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__50.MyQuad.Quad.v0.Pos = new Vector2(-1.002778f, 1.004167f);

			__50.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(52414.34f, -1988.328f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__50.MyQuad.Quad.v1.Pos = new Vector2(0.9972223f, 1.004167f);

			__50.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(49414.34f, -3215.814f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__50.MyQuad.Quad.v2.Pos = new Vector2(-1.002778f, -0.9958333f);

			__50.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(52414.34f, -3215.814f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__50.MyQuad.Quad.v3.Pos = new Vector2(0.9972223f, -0.9958333f);

			__50.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__50.MyQuad.Quad.ExtraTexture1 = null;
			__50.MyQuad.Quad.ExtraTexture2 = null;
			__50.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__50.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__50.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__50.MyQuad.Quad.BlendAddRatio = 0f;

			__50.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 50918.51f, -2604.628f);

			__50.uv_speed = new Vector2(0f, 0f);
			__50.uv_offset = new Vector2(0f, 0f);
			__50.Data = new PhsxData(50918.51f, -2604.628f, 0f, 0f, 0f, 0f);
			__50.StartData = new PhsxData(50918.51f, -2604.628f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__50);

			CloudberryKingdom.BackgroundFloater __51 = new CloudberryKingdom.BackgroundFloater();
			__51.Name = "Terrace_Cloud6";
			__51.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__51.MyQuad.Quad.ExtraTexture1 = null;
			__51.MyQuad.Quad.ExtraTexture2 = null;
			__51.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud6");
			__51.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__51.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__51.MyQuad.Quad.BlendAddRatio = 0f;

			__51.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 754.3007f, 46937.06f, -2734.258f);

			__51.uv_speed = new Vector2(0f, 0f);
			__51.uv_offset = new Vector2(0f, 0f);
			__51.Data = new PhsxData(46937.06f, -2734.258f, 0f, 0f, 0f, 0f);
			__51.StartData = new PhsxData(46937.06f, -2734.258f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__51);

			CloudberryKingdom.BackgroundFloater __52 = new CloudberryKingdom.BackgroundFloater();
			__52.Name = "Terrace_Cloud6";
			__52.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__52.MyQuad.Quad.ExtraTexture1 = null;
			__52.MyQuad.Quad.ExtraTexture2 = null;
			__52.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud6");
			__52.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__52.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__52.MyQuad.Quad.BlendAddRatio = 0f;

			__52.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 754.3007f, 34653.72f, -3015.74f);

			__52.uv_speed = new Vector2(0f, 0f);
			__52.uv_offset = new Vector2(0f, 0f);
			__52.Data = new PhsxData(34653.72f, -3015.74f, 0f, 0f, 0f, 0f);
			__52.StartData = new PhsxData(34653.72f, -3015.74f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__52);

			CloudberryKingdom.BackgroundFloater __53 = new CloudberryKingdom.BackgroundFloater();
			__53.Name = "Terrace_Cloud3";
			__53.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__53.MyQuad.Quad.ExtraTexture1 = null;
			__53.MyQuad.Quad.ExtraTexture2 = null;
			__53.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__53.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__53.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__53.MyQuad.Quad.BlendAddRatio = 0f;

			__53.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, 36987.04f, -3080.554f);

			__53.uv_speed = new Vector2(0f, 0f);
			__53.uv_offset = new Vector2(0f, 0f);
			__53.Data = new PhsxData(36987.04f, -3080.554f, 0f, 0f, 0f, 0f);
			__53.StartData = new PhsxData(36987.04f, -3080.554f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__53);

			CloudberryKingdom.BackgroundFloater __54 = new CloudberryKingdom.BackgroundFloater();
			__54.Name = "Terrace_Cloud3";
			__54.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__54.MyQuad.Quad.ExtraTexture1 = null;
			__54.MyQuad.Quad.ExtraTexture2 = null;
			__54.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__54.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__54.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__54.MyQuad.Quad.BlendAddRatio = 0f;

			__54.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, 30533.34f, -2950.925f);

			__54.uv_speed = new Vector2(0f, 0f);
			__54.uv_offset = new Vector2(0f, 0f);
			__54.Data = new PhsxData(30533.34f, -2950.925f, 0f, 0f, 0f, 0f);
			__54.StartData = new PhsxData(30533.34f, -2950.925f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__54);

			CloudberryKingdom.BackgroundFloater __55 = new CloudberryKingdom.BackgroundFloater();
			__55.Name = "Terrace_Cloud3";
			__55.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__55.MyQuad.Quad.ExtraTexture1 = null;
			__55.MyQuad.Quad.ExtraTexture2 = null;
			__55.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__55.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__55.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__55.MyQuad.Quad.BlendAddRatio = 0f;

			__55.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, 22030.55f, -3023.148f);

			__55.uv_speed = new Vector2(0f, 0f);
			__55.uv_offset = new Vector2(0f, 0f);
			__55.Data = new PhsxData(22030.55f, -3023.148f, 0f, 0f, 0f, 0f);
			__55.StartData = new PhsxData(22030.55f, -3023.148f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__55);

			CloudberryKingdom.BackgroundFloater __56 = new CloudberryKingdom.BackgroundFloater();
			__56.Name = "Terrace_Cloud4";
			__56.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__56.MyQuad.Quad.ExtraTexture1 = null;
			__56.MyQuad.Quad.ExtraTexture2 = null;
			__56.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud4");
			__56.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__56.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__56.MyQuad.Quad.BlendAddRatio = 0f;

			__56.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 971.673f, 24725.02f, -3097.221f);

			__56.uv_speed = new Vector2(0f, 0f);
			__56.uv_offset = new Vector2(0f, 0f);
			__56.Data = new PhsxData(24725.02f, -3097.221f, 0f, 0f, 0f, 0f);
			__56.StartData = new PhsxData(24725.02f, -3097.221f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__56);

			CloudberryKingdom.BackgroundFloater __57 = new CloudberryKingdom.BackgroundFloater();
			__57.Name = "Terrace_Cloud3";
			__57.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__57.MyQuad.Quad.ExtraTexture1 = null;
			__57.MyQuad.Quad.ExtraTexture2 = null;
			__57.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__57.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__57.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__57.MyQuad.Quad.BlendAddRatio = 0f;

			__57.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, 18703.71f, -2940.74f);

			__57.uv_speed = new Vector2(0f, 0f);
			__57.uv_offset = new Vector2(0f, 0f);
			__57.Data = new PhsxData(18703.71f, -2940.74f, 0f, 0f, 0f, 0f);
			__57.StartData = new PhsxData(18703.71f, -2940.74f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__57);

			CloudberryKingdom.BackgroundFloater __58 = new CloudberryKingdom.BackgroundFloater();
			__58.Name = "Terrace_Cloud1";
			__58.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(12561.07f, -2345.686f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
			__58.MyQuad.Quad.v0.Pos = new Vector2(-1.0025f, 0.9997222f);

			__58.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(15561.06f, -2345.686f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
			__58.MyQuad.Quad.v1.Pos = new Vector2(0.9975f, 0.9997222f);

			__58.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(12561.07f, -3573.172f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
			__58.MyQuad.Quad.v2.Pos = new Vector2(-1.0025f, -1.000278f);

			__58.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(15561.06f, -3573.172f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
			__58.MyQuad.Quad.v3.Pos = new Vector2(0.9975f, -1.000278f);

			__58.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__58.MyQuad.Quad.ExtraTexture1 = null;
			__58.MyQuad.Quad.ExtraTexture2 = null;
			__58.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__58.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__58.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__58.MyQuad.Quad.BlendAddRatio = 0f;

			__58.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 14064.82f, -2959.258f);

			__58.uv_speed = new Vector2(0f, 0f);
			__58.uv_offset = new Vector2(0f, 0f);
			__58.Data = new PhsxData(14064.82f, -2959.258f, 0f, 0f, 0f, 0f);
			__58.StartData = new PhsxData(14064.82f, -2959.258f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__58);

			CloudberryKingdom.BackgroundFloater __59 = new CloudberryKingdom.BackgroundFloater();
			__59.Name = "Terrace_Cloud1";
			__59.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__59.MyQuad.Quad.ExtraTexture1 = null;
			__59.MyQuad.Quad.ExtraTexture2 = null;
			__59.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__59.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__59.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__59.MyQuad.Quad.BlendAddRatio = 0f;

			__59.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 11435.19f, -2838.888f);

			__59.uv_speed = new Vector2(0f, 0f);
			__59.uv_offset = new Vector2(0f, 0f);
			__59.Data = new PhsxData(11435.19f, -2838.888f, 0f, 0f, 0f, 0f);
			__59.StartData = new PhsxData(11435.19f, -2838.888f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__59);

			CloudberryKingdom.BackgroundFloater __60 = new CloudberryKingdom.BackgroundFloater();
			__60.Name = "Terrace_Cloud3";
			__60.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__60.MyQuad.Quad.ExtraTexture1 = null;
			__60.MyQuad.Quad.ExtraTexture2 = null;
			__60.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__60.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__60.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__60.MyQuad.Quad.BlendAddRatio = 0f;

			__60.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, 7681.488f, -2902.776f);

			__60.uv_speed = new Vector2(0f, 0f);
			__60.uv_offset = new Vector2(0f, 0f);
			__60.Data = new PhsxData(7681.488f, -2902.776f, 0f, 0f, 0f, 0f);
			__60.StartData = new PhsxData(7681.488f, -2902.776f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__60);

			CloudberryKingdom.BackgroundFloater __61 = new CloudberryKingdom.BackgroundFloater();
			__61.Name = "Terrace_Cloud2";
			__61.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__61.MyQuad.Quad.ExtraTexture1 = null;
			__61.MyQuad.Quad.ExtraTexture2 = null;
			__61.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud2");
			__61.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__61.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__61.MyQuad.Quad.BlendAddRatio = 0f;

			__61.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 5329.639f, -2856.48f);

			__61.uv_speed = new Vector2(0f, 0f);
			__61.uv_offset = new Vector2(0f, 0f);
			__61.Data = new PhsxData(5329.639f, -2856.48f, 0f, 0f, 0f, 0f);
			__61.StartData = new PhsxData(5329.639f, -2856.48f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__61);

			CloudberryKingdom.BackgroundFloater __62 = new CloudberryKingdom.BackgroundFloater();
			__62.Name = "Terrace_Cloud1";
			__62.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__62.MyQuad.Quad.ExtraTexture1 = null;
			__62.MyQuad.Quad.ExtraTexture2 = null;
			__62.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud1");
			__62.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__62.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__62.MyQuad.Quad.BlendAddRatio = 0f;

			__62.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 613.743f, 1644.453f, -2902.776f);

			__62.uv_speed = new Vector2(0f, 0f);
			__62.uv_offset = new Vector2(0f, 0f);
			__62.Data = new PhsxData(1644.453f, -2902.776f, 0f, 0f, 0f, 0f);
			__62.StartData = new PhsxData(1644.453f, -2902.776f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__62);

			CloudberryKingdom.BackgroundFloater __63 = new CloudberryKingdom.BackgroundFloater();
			__63.Name = "Terrace_Cloud3";
			__63.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__63.MyQuad.Quad.ExtraTexture1 = null;
			__63.MyQuad.Quad.ExtraTexture2 = null;
			__63.MyQuad.Quad._MyTexture = Tools.Texture("Terrace_Cloud3");
			__63.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__63.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__63.MyQuad.Quad.BlendAddRatio = 0f;

			__63.MyQuad.Base = new CoreEngine.BasePoint(1499.997f, 0f, 0f, 717.9474f, -2212.955f, -3062.962f);

			__63.uv_speed = new Vector2(0f, 0f);
			__63.uv_offset = new Vector2(0f, 0f);
			__63.Data = new PhsxData(-2212.955f, -3062.962f, 0f, 0f, 0f, 0f);
			__63.StartData = new PhsxData(-2212.955f, -3062.962f, 0f, 0f, 0f, 0f);
			__47.Floaters.Add(__63);

			__47.Parallax = 0.3f;
			__47.DoPreDraw = false;
			b.MyCollection.Lists.Add(__47);

			b.Light = 1f;
			b.BL = new Vector2(-100000f, -10000f);
			b.TR = new Vector2(100000f, 10000f);
		}
    }
}
