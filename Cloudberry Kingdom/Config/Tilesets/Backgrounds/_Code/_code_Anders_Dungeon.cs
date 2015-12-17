using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
		public static void _code_Dungeon(Background b)
		{
			b.GuidCounter = 0;
			b.MyGlobalIllumination = 1f;
			b.AllowLava = true;
			CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
			__1.Name = "BackgroundWall";
			__1.Foreground = false;
			__1.Fixed = false;
			CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
			__2.Name = "Dungeon_Background";
			__2.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-5578.24f, 3599.392f), new Vector2(0f, 0f), new Color(128, 128, 128, 255));
			__2.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__2.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(59389.1f, 3599.392f), new Vector2(3f, 0f), new Color(128, 128, 128, 255));
			__2.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__2.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-5578.24f, -3580.426f), new Vector2(0f, 1f), new Color(128, 128, 128, 255));
			__2.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__2.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(59389.1f, -3580.426f), new Vector2(3f, 1f), new Color(128, 128, 128, 255));
			__2.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__2.MyQuad.Quad.ExtraTexture1 = null;
			__2.MyQuad.Quad.ExtraTexture2 = null;
			__2.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Background");
			__2.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__2.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__2.MyQuad.Quad.BlendAddRatio = 0f;

			__2.MyQuad.Base = new CoreEngine.BasePoint(32483.67f, 0f, 0f, 3589.909f, 26905.43f, 9.482803f);

			__2.uv_speed = new Vector2(0f, 0f);
			__2.uv_offset = new Vector2(0f, 0f);
			__2.Data = new PhsxData(26905.43f, 9.482803f, 0f, 0f, 0f, 0f);
			__2.StartData = new PhsxData(26905.43f, 9.482803f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__2);

			CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
			__3.Name = "Dungeon_Window1";
			__3.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1752.454f, 2186.185f), new Vector2(0f, 0f), new Color(128, 128, 128, 255));
			__3.MyQuad.Quad.v0.Pos = new Vector2(-1.002778f, 0.9986111f);

			__3.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(2965.552f, 2186.185f), new Vector2(1f, 0f), new Color(128, 128, 128, 255));
			__3.MyQuad.Quad.v1.Pos = new Vector2(0.9972224f, 0.9986111f);

			__3.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1752.454f, -1271.637f), new Vector2(0f, 1f), new Color(128, 128, 128, 255));
			__3.MyQuad.Quad.v2.Pos = new Vector2(-1.002778f, -1.001389f);

			__3.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(2965.552f, -1271.637f), new Vector2(1f, 1f), new Color(128, 128, 128, 255));
			__3.MyQuad.Quad.v3.Pos = new Vector2(0.9972224f, -1.001389f);

			__3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__3.MyQuad.Quad.ExtraTexture1 = null;
			__3.MyQuad.Quad.ExtraTexture2 = null;
			__3.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window1");
			__3.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__3.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__3.MyQuad.Quad.BlendAddRatio = 0f;

			__3.MyQuad.Base = new CoreEngine.BasePoint(2359.003f, 0f, 0f, 1728.911f, 613.1016f, 459.6747f);

			__3.uv_speed = new Vector2(0f, 0f);
			__3.uv_offset = new Vector2(0f, 0f);
			__3.Data = new PhsxData(613.1016f, 459.6747f, 0f, 0f, 0f, 0f);
			__3.StartData = new PhsxData(613.1016f, 459.6747f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__3);

			CloudberryKingdom.BackgroundFloater __4 = new CloudberryKingdom.BackgroundFloater();
			__4.Name = "Dungeon_Window2";
			__4.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__4.MyQuad.Quad.ExtraTexture1 = null;
			__4.MyQuad.Quad.ExtraTexture2 = null;
			__4.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window2");
			__4.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__4.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__4.MyQuad.Quad.BlendAddRatio = 0f;

			__4.MyQuad.Base = new CoreEngine.BasePoint(2828.352f, 0f, 0f, 2826.532f, 6714.635f, 1752.778f);

			__4.uv_speed = new Vector2(0f, 0f);
			__4.uv_offset = new Vector2(0f, 0f);
			__4.Data = new PhsxData(6714.635f, 1752.778f, 0f, 0f, 0f, 0f);
			__4.StartData = new PhsxData(6714.635f, 1752.778f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__4);

			CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
			__5.Name = "Dungeon_Halberd";
			__5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__5.MyQuad.Quad.ExtraTexture1 = null;
			__5.MyQuad.Quad.ExtraTexture2 = null;
			__5.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Halberd");
			__5.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__5.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__5.MyQuad.Quad.BlendAddRatio = 0f;

			__5.MyQuad.Base = new CoreEngine.BasePoint(1123.369f, 0f, 0f, 984.7476f, 6408.118f, -1542.242f);

			__5.uv_speed = new Vector2(0f, 0f);
			__5.uv_offset = new Vector2(0f, 0f);
			__5.Data = new PhsxData(6408.118f, -1542.242f, 0f, 0f, 0f, 0f);
			__5.StartData = new PhsxData(6408.118f, -1542.242f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__5);

			CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
			__6.Name = "Dungeon_Window2";
			__6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__6.MyQuad.Quad.ExtraTexture1 = null;
			__6.MyQuad.Quad.ExtraTexture2 = null;
			__6.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window2");
			__6.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__6.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__6.MyQuad.Quad.BlendAddRatio = 0f;

			__6.MyQuad.Base = new CoreEngine.BasePoint(2828.352f, 0f, 0f, 2826.532f, 13669.83f, 1670.784f);

			__6.uv_speed = new Vector2(0f, 0f);
			__6.uv_offset = new Vector2(0f, 0f);
			__6.Data = new PhsxData(13669.83f, 1670.784f, 0f, 0f, 0f, 0f);
			__6.StartData = new PhsxData(13669.83f, 1670.784f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__6);

			CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
			__7.Name = "Dungeon_Window1";
			__7.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__7.MyQuad.Quad.ExtraTexture1 = null;
			__7.MyQuad.Quad.ExtraTexture2 = null;
			__7.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window1");
			__7.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__7.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__7.MyQuad.Quad.BlendAddRatio = 0f;

			__7.MyQuad.Base = new CoreEngine.BasePoint(2359.003f, 0f, 0f, 1728.911f, 21250.57f, 134.8655f);

			__7.uv_speed = new Vector2(0f, 0f);
			__7.uv_offset = new Vector2(0f, 0f);
			__7.Data = new PhsxData(21250.57f, 134.8655f, 0f, 0f, 0f, 0f);
			__7.StartData = new PhsxData(21250.57f, 134.8655f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__7);

			CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
			__8.Name = "Dungeon_Window1";
			__8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__8.MyQuad.Quad.ExtraTexture1 = null;
			__8.MyQuad.Quad.ExtraTexture2 = null;
			__8.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window1");
			__8.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__8.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__8.MyQuad.Quad.BlendAddRatio = 0f;

			__8.MyQuad.Base = new CoreEngine.BasePoint(2359.003f, 0f, 0f, 1728.911f, 26218.02f, 128.6395f);

			__8.uv_speed = new Vector2(0f, 0f);
			__8.uv_offset = new Vector2(0f, 0f);
			__8.Data = new PhsxData(26218.02f, 128.6395f, 0f, 0f, 0f, 0f);
			__8.StartData = new PhsxData(26218.02f, 128.6395f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__8);

			CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
			__9.Name = "Dungeon_Window2";
			__9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__9.MyQuad.Quad.ExtraTexture1 = null;
			__9.MyQuad.Quad.ExtraTexture2 = null;
			__9.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Window2");
			__9.MyQuad.Quad.MySetColor = new Color(128, 128, 128, 255);
			__9.MyQuad.Quad.PremultipliedColor = new Color(128, 128, 128, 255);
			__9.MyQuad.Quad.BlendAddRatio = 0f;

			__9.MyQuad.Base = new CoreEngine.BasePoint(2828.352f, 0f, 0f, 2826.532f, 33217.61f, 1439.847f);

			__9.uv_speed = new Vector2(0f, 0f);
			__9.uv_offset = new Vector2(0f, 0f);
			__9.Data = new PhsxData(33217.61f, 1439.847f, 0f, 0f, 0f, 0f);
			__9.StartData = new PhsxData(33217.61f, 1439.847f, 0f, 0f, 0f, 0f);
			__1.Floaters.Add(__9);

			__1.Parallax = 0.29f;
			__1.DoPreDraw = false;
			b.MyCollection.Lists.Add(__1);

			CloudberryKingdom.BackgroundFloaterList __10 = new CloudberryKingdom.BackgroundFloaterList();
			__10.Name = "Fog";
			__10.Foreground = false;
			__10.Fixed = false;
			CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
			__11.Name = "Dungeon_Smoke_Thick";
			__11.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-4026.328f, 2461.61f), new Vector2(1.503548f, 0f), new Color(255, 255, 255, 255));
			__11.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__11.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(42424.75f, 2461.61f), new Vector2(17.50663f, 0f), new Color(255, 255, 255, 255));
			__11.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__11.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-4026.328f, -2144.788f), new Vector2(1.503548f, 1f), new Color(255, 255, 255, 255));
			__11.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__11.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(42424.75f, -2144.788f), new Vector2(17.50663f, 1f), new Color(255, 255, 255, 255));
			__11.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__11.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__11.MyQuad.Quad.ExtraTexture1 = null;
			__11.MyQuad.Quad.ExtraTexture2 = null;
			__11.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Smoke_Thick");
			__11.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__11.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__11.MyQuad.Quad.BlendAddRatio = 0f;

			__11.MyQuad.Base = new CoreEngine.BasePoint(23225.54f, 0f, 0f, 2303.199f, 19199.21f, 158.4108f);

			__11.uv_speed = new Vector2(0.0004f, 0f);
			__11.uv_offset = new Vector2(0f, 0f);
			__11.Data = new PhsxData(19199.21f, 158.4108f, 0f, 0f, 0f, 0f);
			__11.StartData = new PhsxData(19199.21f, 158.4108f, 0f, 0f, 0f, 0f);
			__10.Floaters.Add(__11);

			__10.Parallax = 0.35f;
			__10.DoPreDraw = false;
			b.MyCollection.Lists.Add(__10);

			CloudberryKingdom.BackgroundFloaterList __12 = new CloudberryKingdom.BackgroundFloaterList();
			__12.Name = "Pillars";
			__12.Foreground = false;
			__12.Fixed = false;
			CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
			__13.Name = "pillar_dungeon_150";
			__13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__13.MyQuad.Quad.ExtraTexture1 = null;
			__13.MyQuad.Quad.ExtraTexture2 = null;
			__13.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__13.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__13.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__13.MyQuad.Quad.BlendAddRatio = 0f;

			__13.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 7971.375f, -153.5616f);

			__13.uv_speed = new Vector2(0f, 0f);
			__13.uv_offset = new Vector2(0f, 0f);
			__13.Data = new PhsxData(7971.375f, -153.5616f, 0f, 0f, 0f, 0f);
			__13.StartData = new PhsxData(7971.375f, -153.5616f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__13);

			CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
			__14.Name = "pillar_dungeon_150";
			__14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__14.MyQuad.Quad.ExtraTexture1 = null;
			__14.MyQuad.Quad.ExtraTexture2 = null;
			__14.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__14.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__14.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__14.MyQuad.Quad.BlendAddRatio = 0f;

			__14.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 5193.608f, -68.09125f);

			__14.uv_speed = new Vector2(0f, 0f);
			__14.uv_offset = new Vector2(0f, 0f);
			__14.Data = new PhsxData(5193.608f, -68.09125f, 0f, 0f, 0f, 0f);
			__14.StartData = new PhsxData(5193.608f, -68.09125f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__14);

			CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
			__15.Name = "pillar_dungeon_150";
			__15.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(2146.576f, 2345.861f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__15.MyQuad.Quad.v0.Pos = new Vector2(-1.000556f, 0.9925f);

			__15.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(2978.487f, 2345.861f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__15.MyQuad.Quad.v1.Pos = new Vector2(0.9994444f, 0.9925f);

			__15.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(2146.576f, -2884.088f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__15.MyQuad.Quad.v2.Pos = new Vector2(-1.000556f, -1.0075f);

			__15.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(2978.487f, -2884.088f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__15.MyQuad.Quad.v3.Pos = new Vector2(0.9994444f, -1.0075f);

			__15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__15.MyQuad.Quad.ExtraTexture1 = null;
			__15.MyQuad.Quad.ExtraTexture2 = null;
			__15.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__15.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__15.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__15.MyQuad.Quad.BlendAddRatio = 0f;

			__15.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 2562.763f, -249.5013f);

			__15.uv_speed = new Vector2(0f, 0f);
			__15.uv_offset = new Vector2(0f, 0f);
			__15.Data = new PhsxData(2562.763f, -249.5013f, 0f, 0f, 0f, 0f);
			__15.StartData = new PhsxData(2562.763f, -249.5013f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__15);

			CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
			__16.Name = "pillar_dungeon_150";
			__16.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(1527.496f, 2657.496f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__16.MyQuad.Quad.v0.Pos = new Vector2(-0.9991667f, 1f);

			__16.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(2359.406f, 2657.496f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__16.MyQuad.Quad.v1.Pos = new Vector2(1.000833f, 1f);

			__16.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(1527.496f, -2572.453f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__16.MyQuad.Quad.v2.Pos = new Vector2(-0.9991667f, -1f);

			__16.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(2359.406f, -2572.453f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__16.MyQuad.Quad.v3.Pos = new Vector2(1.000833f, -1f);

			__16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__16.MyQuad.Quad.ExtraTexture1 = null;
			__16.MyQuad.Quad.ExtraTexture2 = null;
			__16.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__16.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__16.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__16.MyQuad.Quad.BlendAddRatio = 0f;

			__16.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 1943.104f, 42.52148f);

			__16.uv_speed = new Vector2(0f, 0f);
			__16.uv_offset = new Vector2(0f, 0f);
			__16.Data = new PhsxData(1943.104f, 42.52148f, 0f, 0f, 0f, 0f);
			__16.StartData = new PhsxData(1943.104f, 42.52148f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__16);

			CloudberryKingdom.BackgroundFloater __17 = new CloudberryKingdom.BackgroundFloater();
			__17.Name = "pillar_dungeon_150";
			__17.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1107.022f, 2456.612f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__17.MyQuad.Quad.v0.Pos = new Vector2(-0.9972222f, 0.9994444f);

			__17.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(-275.1107f, 2456.612f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__17.MyQuad.Quad.v1.Pos = new Vector2(1.002778f, 0.9994444f);

			__17.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1107.022f, -2773.337f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__17.MyQuad.Quad.v2.Pos = new Vector2(-0.9972222f, -1.000556f);

			__17.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(-275.1107f, -2773.337f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__17.MyQuad.Quad.v3.Pos = new Vector2(1.002778f, -1.000556f);

			__17.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__17.MyQuad.Quad.ExtraTexture1 = null;
			__17.MyQuad.Quad.ExtraTexture2 = null;
			__17.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__17.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__17.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__17.MyQuad.Quad.BlendAddRatio = 0f;

			__17.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, -692.2217f, -156.9089f);

			__17.uv_speed = new Vector2(0f, 0f);
			__17.uv_offset = new Vector2(0f, 0f);
			__17.Data = new PhsxData(-692.2217f, -156.9089f, 0f, 0f, 0f, 0f);
			__17.StartData = new PhsxData(-692.2217f, -156.9089f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__17);

			CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
			__18.Name = "pillar_dungeon_150";
			__18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__18.MyQuad.Quad.ExtraTexture1 = null;
			__18.MyQuad.Quad.ExtraTexture2 = null;
			__18.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__18.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__18.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__18.MyQuad.Quad.BlendAddRatio = 0f;

			__18.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 13141.93f, -422.436f);

			__18.uv_speed = new Vector2(0f, 0f);
			__18.uv_offset = new Vector2(0f, 0f);
			__18.Data = new PhsxData(13141.93f, -422.436f, 0f, 0f, 0f, 0f);
			__18.StartData = new PhsxData(13141.93f, -422.436f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__18);

			CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
			__19.Name = "pillar_dungeon_150";
			__19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__19.MyQuad.Quad.ExtraTexture1 = null;
			__19.MyQuad.Quad.ExtraTexture2 = null;
			__19.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__19.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__19.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__19.MyQuad.Quad.BlendAddRatio = 0f;

			__19.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 16461.52f, 32.90593f);

			__19.uv_speed = new Vector2(0f, 0f);
			__19.uv_offset = new Vector2(0f, 0f);
			__19.Data = new PhsxData(16461.52f, 32.90593f, 0f, 0f, 0f, 0f);
			__19.StartData = new PhsxData(16461.52f, 32.90593f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__19);

			CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
			__20.Name = "pillar_dungeon_150";
			__20.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(18640.27f, 1830.715f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__20.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__20.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(19472.18f, 1830.715f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__20.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__20.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(18640.27f, -3399.233f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__20.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__20.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(19472.18f, -3399.233f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__20.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__20.MyQuad.Quad.ExtraTexture1 = null;
			__20.MyQuad.Quad.ExtraTexture2 = null;
			__20.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__20.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__20.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__20.MyQuad.Quad.BlendAddRatio = 0f;

			__20.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 19056.11f, -784.2593f);

			__20.uv_speed = new Vector2(0f, 0f);
			__20.uv_offset = new Vector2(0f, 0f);
			__20.Data = new PhsxData(19056.11f, -784.2593f, 0f, 0f, 0f, 0f);
			__20.StartData = new PhsxData(19056.11f, -784.2593f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__20);

			CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
			__21.Name = "pillar_dungeon_150";
			__21.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(19879.7f, 2550.088f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__21.MyQuad.Quad.v0.Pos = new Vector2(-0.9994444f, 1f);

			__21.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(20711.61f, 2550.088f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__21.MyQuad.Quad.v1.Pos = new Vector2(1.000556f, 1f);

			__21.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(19879.7f, -2679.86f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__21.MyQuad.Quad.v2.Pos = new Vector2(-0.9994444f, -1f);

			__21.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(20711.61f, -2679.86f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__21.MyQuad.Quad.v3.Pos = new Vector2(1.000556f, -1f);

			__21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__21.MyQuad.Quad.ExtraTexture1 = null;
			__21.MyQuad.Quad.ExtraTexture2 = null;
			__21.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__21.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__21.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__21.MyQuad.Quad.BlendAddRatio = 0f;

			__21.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 20295.42f, -64.88607f);

			__21.uv_speed = new Vector2(0f, 0f);
			__21.uv_offset = new Vector2(0f, 0f);
			__21.Data = new PhsxData(20295.42f, -64.88607f, 0f, 0f, 0f, 0f);
			__21.StartData = new PhsxData(20295.42f, -64.88607f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__21);

			CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
			__22.Name = "pillar_dungeon_150";
			__22.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(23546.39f, 2538.051f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__22.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__22.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(24378.3f, 2538.051f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__22.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__22.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(23546.39f, -2691.897f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__22.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__22.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(24378.3f, -2691.897f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__22.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__22.MyQuad.Quad.ExtraTexture1 = null;
			__22.MyQuad.Quad.ExtraTexture2 = null;
			__22.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__22.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__22.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__22.MyQuad.Quad.BlendAddRatio = 0f;

			__22.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 23962.23f, -76.92314f);

			__22.uv_speed = new Vector2(0f, 0f);
			__22.uv_offset = new Vector2(0f, 0f);
			__22.Data = new PhsxData(23962.23f, -76.92314f, 0f, 0f, 0f, 0f);
			__22.StartData = new PhsxData(23962.23f, -76.92314f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__22);

			CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
			__23.Name = "pillar_dungeon_150";
			__23.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(24664.52f, 2657.681f), new Vector2(0f, 0f), new Color(189, 189, 189, 255));
			__23.MyQuad.Quad.v0.Pos = new Vector2(-1f, 0.9994444f);

			__23.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(25496.43f, 2657.681f), new Vector2(1f, 0f), new Color(189, 189, 189, 255));
			__23.MyQuad.Quad.v1.Pos = new Vector2(1f, 0.9994444f);

			__23.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(24664.52f, -2572.267f), new Vector2(0f, 1f), new Color(189, 189, 189, 255));
			__23.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1.000556f);

			__23.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(25496.43f, -2572.267f), new Vector2(1f, 1f), new Color(189, 189, 189, 255));
			__23.MyQuad.Quad.v3.Pos = new Vector2(1f, -1.000556f);

			__23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__23.MyQuad.Quad.ExtraTexture1 = null;
			__23.MyQuad.Quad.ExtraTexture2 = null;
			__23.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__23.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__23.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__23.MyQuad.Quad.BlendAddRatio = 0f;

			__23.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 25080.47f, 44.15975f);

			__23.uv_speed = new Vector2(0f, 0f);
			__23.uv_offset = new Vector2(0f, 0f);
			__23.Data = new PhsxData(25080.47f, 44.15975f, 0f, 0f, 0f, 0f);
			__23.StartData = new PhsxData(25080.47f, 44.15975f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__23);

			CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
			__24.Name = "pillar_dungeon_150";
			__24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__24.MyQuad.Quad.ExtraTexture1 = null;
			__24.MyQuad.Quad.ExtraTexture2 = null;
			__24.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_150");
			__24.MyQuad.Quad.MySetColor = new Color(189, 189, 189, 255);
			__24.MyQuad.Quad.PremultipliedColor = new Color(189, 189, 189, 255);
			__24.MyQuad.Quad.BlendAddRatio = 0f;

			__24.MyQuad.Base = new CoreEngine.BasePoint(415.9554f, 0f, 0f, 2614.974f, 28876.76f, -568.376f);

			__24.uv_speed = new Vector2(0f, 0f);
			__24.uv_offset = new Vector2(0f, 0f);
			__24.Data = new PhsxData(28876.76f, -568.376f, 0f, 0f, 0f, 0f);
			__24.StartData = new PhsxData(28876.76f, -568.376f, 0f, 0f, 0f, 0f);
			__12.Floaters.Add(__24);

			__12.Parallax = 0.39f;
			__12.DoPreDraw = false;
			b.MyCollection.Lists.Add(__12);

			CloudberryKingdom.BackgroundFloaterList __25 = new CloudberryKingdom.BackgroundFloaterList();
			__25.Name = "Fog2";
			__25.Foreground = false;
			__25.Fixed = false;
			CloudberryKingdom.BackgroundFloater __26 = new CloudberryKingdom.BackgroundFloater();
			__26.Name = "Dungeon_Smoke_Thick";
			__26.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1761.551f, 888.4113f), new Vector2(4.698712f, 0f), new Color(255, 255, 255, 255));
			__26.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__26.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(33503.21f, 888.4113f), new Vector2(20.69623f, 0f), new Color(255, 255, 255, 255));
			__26.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__26.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1761.551f, -2608.677f), new Vector2(4.698712f, 1f), new Color(255, 255, 255, 255));
			__26.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__26.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(33503.21f, -2608.677f), new Vector2(20.69623f, 1f), new Color(255, 255, 255, 255));
			__26.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__26.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__26.MyQuad.Quad.ExtraTexture1 = null;
			__26.MyQuad.Quad.ExtraTexture2 = null;
			__26.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Smoke_Thick");
			__26.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__26.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__26.MyQuad.Quad.BlendAddRatio = 0f;

			__26.MyQuad.Base = new CoreEngine.BasePoint(17632.38f, 0f, 0f, 1748.544f, 15870.83f, -860.1327f);

			__26.uv_speed = new Vector2(0.00125f, 0f);
			__26.uv_offset = new Vector2(0f, 0f);
			__26.Data = new PhsxData(15870.83f, -860.1327f, 0f, 0f, 0f, 0f);
			__26.StartData = new PhsxData(15870.83f, -860.1327f, 0f, 0f, 0f, 0f);
			__25.Floaters.Add(__26);

			__25.Parallax = 0.48f;
			__25.DoPreDraw = false;
			b.MyCollection.Lists.Add(__25);

			CloudberryKingdom.BackgroundFloaterList __27 = new CloudberryKingdom.BackgroundFloaterList();
			__27.Name = "Pillars";
			__27.Foreground = false;
			__27.Fixed = false;
			CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
			__28.Name = "pillar_dungeon_60";
			__28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__28.MyQuad.Quad.ExtraTexture1 = null;
			__28.MyQuad.Quad.ExtraTexture2 = null;
			__28.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__28.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__28.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__28.MyQuad.Quad.BlendAddRatio = 0f;

			__28.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 6045.938f, -509.6002f);

			__28.uv_speed = new Vector2(0f, 0f);
			__28.uv_offset = new Vector2(0f, 0f);
			__28.Data = new PhsxData(6045.938f, -509.6002f, 0f, 0f, 0f, 0f);
			__28.StartData = new PhsxData(6045.938f, -509.6002f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__28);

			CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
			__29.Name = "pillar_dungeon_60";
			__29.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(23527.91f, 1499.915f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__29.MyQuad.Quad.v0.Pos = new Vector2(-1f, 0.9994444f);

			__29.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(23754.45f, 1499.915f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__29.MyQuad.Quad.v1.Pos = new Vector2(1f, 0.9994444f);

			__29.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(23527.91f, -1795.118f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__29.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1.000556f);

			__29.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(23754.45f, -1795.118f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__29.MyQuad.Quad.v3.Pos = new Vector2(1f, -1.000556f);

			__29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__29.MyQuad.Quad.ExtraTexture1 = null;
			__29.MyQuad.Quad.ExtraTexture2 = null;
			__29.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__29.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__29.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__29.MyQuad.Quad.BlendAddRatio = 0f;

			__29.MyQuad.Base = new CoreEngine.BasePoint(113.2668f, 0f, 0f, 1647.517f, 23641.18f, -146.6862f);

			__29.uv_speed = new Vector2(0f, 0f);
			__29.uv_offset = new Vector2(0f, 0f);
			__29.Data = new PhsxData(23641.18f, -146.6862f, 0f, 0f, 0f, 0f);
			__29.StartData = new PhsxData(23641.18f, -146.6862f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__29);

			CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
			__30.Name = "pillar_dungeon_60";
			__30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__30.MyQuad.Quad.ExtraTexture1 = null;
			__30.MyQuad.Quad.ExtraTexture2 = null;
			__30.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__30.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__30.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__30.MyQuad.Quad.BlendAddRatio = 0f;

			__30.MyQuad.Base = new CoreEngine.BasePoint(113.2668f, 0f, 0f, 1647.517f, 29115.45f, -736.5498f);

			__30.uv_speed = new Vector2(0f, 0f);
			__30.uv_offset = new Vector2(0f, 0f);
			__30.Data = new PhsxData(29115.45f, -736.5498f, 0f, 0f, 0f, 0f);
			__30.StartData = new PhsxData(29115.45f, -736.5498f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__30);

			CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
			__31.Name = "pillar_dungeon_60";
			__31.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(24098.52f, 1518.493f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__31.MyQuad.Quad.v0.Pos = new Vector2(-0.9961112f, 0.9988889f);

			__31.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(24325.05f, 1518.493f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__31.MyQuad.Quad.v1.Pos = new Vector2(1.003889f, 0.9988889f);

			__31.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(24098.52f, -1776.54f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__31.MyQuad.Quad.v2.Pos = new Vector2(-0.9961112f, -1.001111f);

			__31.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(24325.05f, -1776.54f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__31.MyQuad.Quad.v3.Pos = new Vector2(1.003889f, -1.001111f);

			__31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__31.MyQuad.Quad.ExtraTexture1 = null;
			__31.MyQuad.Quad.ExtraTexture2 = null;
			__31.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__31.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__31.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__31.MyQuad.Quad.BlendAddRatio = 0f;

			__31.MyQuad.Base = new CoreEngine.BasePoint(113.2668f, 0f, 0f, 1647.517f, 24211.35f, -127.1929f);

			__31.uv_speed = new Vector2(0f, 0f);
			__31.uv_offset = new Vector2(0f, 0f);
			__31.Data = new PhsxData(24211.35f, -127.1929f, 0f, 0f, 0f, 0f);
			__31.StartData = new PhsxData(24211.35f, -127.1929f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__31);

			CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
			__32.Name = "pillar_dungeon_60";
			__32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__32.MyQuad.Quad.ExtraTexture1 = null;
			__32.MyQuad.Quad.ExtraTexture2 = null;
			__32.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__32.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__32.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__32.MyQuad.Quad.BlendAddRatio = 0f;

			__32.MyQuad.Base = new CoreEngine.BasePoint(113.2668f, 0f, 0f, 1647.517f, 25970.63f, -1447.856f);

			__32.uv_speed = new Vector2(0f, 0f);
			__32.uv_offset = new Vector2(0f, 0f);
			__32.Data = new PhsxData(25970.63f, -1447.856f, 0f, 0f, 0f, 0f);
			__32.StartData = new PhsxData(25970.63f, -1447.856f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__32);

			CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
			__33.Name = "pillar_dungeon_60";
			__33.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(7110.562f, 1402.948f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__33.MyQuad.Quad.v0.Pos = new Vector2(-0.9977778f, 0.9975f);

			__33.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(7379.571f, 1402.948f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__33.MyQuad.Quad.v1.Pos = new Vector2(1.002222f, 0.9975f);

			__33.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(7110.562f, -2509.904f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__33.MyQuad.Quad.v2.Pos = new Vector2(-0.9977778f, -1.0025f);

			__33.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(7379.571f, -2509.904f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__33.MyQuad.Quad.v3.Pos = new Vector2(1.002222f, -1.0025f);

			__33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__33.MyQuad.Quad.ExtraTexture1 = null;
			__33.MyQuad.Quad.ExtraTexture2 = null;
			__33.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__33.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__33.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__33.MyQuad.Quad.BlendAddRatio = 0f;

			__33.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 7244.768f, -548.5868f);

			__33.uv_speed = new Vector2(0f, 0f);
			__33.uv_offset = new Vector2(0f, 0f);
			__33.Data = new PhsxData(7244.768f, -548.5868f, 0f, 0f, 0f, 0f);
			__33.StartData = new PhsxData(7244.768f, -548.5868f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__33);

			CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
			__34.Name = "pillar_dungeon_60";
			__34.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(9231.023f, 1577.79f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__34.MyQuad.Quad.v0.Pos = new Vector2(-1.029722f, 0.9897223f);

			__34.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(9500.032f, 1577.79f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__34.MyQuad.Quad.v1.Pos = new Vector2(0.970278f, 0.9897223f);

			__34.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(9231.023f, -2335.062f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__34.MyQuad.Quad.v2.Pos = new Vector2(-1.029722f, -1.010278f);

			__34.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(9500.032f, -2335.062f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__34.MyQuad.Quad.v3.Pos = new Vector2(0.970278f, -1.010278f);

			__34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__34.MyQuad.Quad.ExtraTexture1 = null;
			__34.MyQuad.Quad.ExtraTexture2 = null;
			__34.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__34.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__34.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__34.MyQuad.Quad.BlendAddRatio = 0f;

			__34.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 9369.525f, -358.5281f);

			__34.uv_speed = new Vector2(0f, 0f);
			__34.uv_offset = new Vector2(0f, 0f);
			__34.Data = new PhsxData(9369.525f, -358.5281f, 0f, 0f, 0f, 0f);
			__34.StartData = new PhsxData(9369.525f, -358.5281f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__34);

			CloudberryKingdom.BackgroundFloater __35 = new CloudberryKingdom.BackgroundFloater();
			__35.Name = "pillar_dungeon_60";
			__35.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(3630.655f, 1183.668f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__35.MyQuad.Quad.v0.Pos = new Vector2(-1.000556f, 1f);

			__35.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(3899.664f, 1183.668f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__35.MyQuad.Quad.v1.Pos = new Vector2(0.9994444f, 1f);

			__35.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(3630.655f, -2729.185f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__35.MyQuad.Quad.v2.Pos = new Vector2(-1.000556f, -1f);

			__35.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(3899.664f, -2729.185f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__35.MyQuad.Quad.v3.Pos = new Vector2(0.9994444f, -1f);

			__35.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__35.MyQuad.Quad.ExtraTexture1 = null;
			__35.MyQuad.Quad.ExtraTexture2 = null;
			__35.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__35.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__35.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__35.MyQuad.Quad.BlendAddRatio = 0f;

			__35.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 3765.234f, -772.7584f);

			__35.uv_speed = new Vector2(0f, 0f);
			__35.uv_offset = new Vector2(0f, 0f);
			__35.Data = new PhsxData(3765.234f, -772.7584f, 0f, 0f, 0f, 0f);
			__35.StartData = new PhsxData(3765.234f, -772.7584f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__35);

			CloudberryKingdom.BackgroundFloater __36 = new CloudberryKingdom.BackgroundFloater();
			__36.Name = "pillar_dungeon_60";
			__36.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__36.MyQuad.Quad.ExtraTexture1 = null;
			__36.MyQuad.Quad.ExtraTexture2 = null;
			__36.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__36.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__36.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__36.MyQuad.Quad.BlendAddRatio = 0f;

			__36.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 1618.256f, -615.3508f);

			__36.uv_speed = new Vector2(0f, 0f);
			__36.uv_offset = new Vector2(0f, 0f);
			__36.Data = new PhsxData(1618.256f, -615.3508f, 0f, 0f, 0f, 0f);
			__36.StartData = new PhsxData(1618.256f, -615.3508f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__36);

			CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
			__37.Name = "pillar_dungeon_60";
			__37.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(22.6993f, 1220.89f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__37.MyQuad.Quad.v0.Pos = new Vector2(-0.9930555f, 1.003333f);

			__37.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(291.7079f, 1220.89f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__37.MyQuad.Quad.v1.Pos = new Vector2(1.006945f, 1.003333f);

			__37.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(22.6993f, -2691.961f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__37.MyQuad.Quad.v2.Pos = new Vector2(-0.9930555f, -0.9966667f);

			__37.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(291.7079f, -2691.961f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__37.MyQuad.Quad.v3.Pos = new Vector2(1.006945f, -0.9966667f);

			__37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__37.MyQuad.Quad.ExtraTexture1 = null;
			__37.MyQuad.Quad.ExtraTexture2 = null;
			__37.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__37.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__37.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__37.MyQuad.Quad.BlendAddRatio = 0f;

			__37.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 156.2695f, -742.0564f);

			__37.uv_speed = new Vector2(0f, 0f);
			__37.uv_offset = new Vector2(0f, 0f);
			__37.Data = new PhsxData(156.2695f, -742.0564f, 0f, 0f, 0f, 0f);
			__37.StartData = new PhsxData(156.2695f, -742.0564f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__37);

			CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
			__38.Name = "pillar_dungeon_60";
			__38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__38.MyQuad.Quad.ExtraTexture1 = null;
			__38.MyQuad.Quad.ExtraTexture2 = null;
			__38.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__38.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__38.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__38.MyQuad.Quad.BlendAddRatio = 0f;

			__38.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 5747.25f, -685.4286f);

			__38.uv_speed = new Vector2(0f, 0f);
			__38.uv_offset = new Vector2(0f, 0f);
			__38.Data = new PhsxData(5747.25f, -685.4286f, 0f, 0f, 0f, 0f);
			__38.StartData = new PhsxData(5747.25f, -685.4286f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__38);

			CloudberryKingdom.BackgroundFloater __39 = new CloudberryKingdom.BackgroundFloater();
			__39.Name = "pillar_dungeon_60";
			__39.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__39.MyQuad.Quad.ExtraTexture1 = null;
			__39.MyQuad.Quad.ExtraTexture2 = null;
			__39.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__39.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__39.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__39.MyQuad.Quad.BlendAddRatio = 0f;

			__39.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 11093.81f, -629.6783f);

			__39.uv_speed = new Vector2(0f, 0f);
			__39.uv_offset = new Vector2(0f, 0f);
			__39.Data = new PhsxData(11093.81f, -629.6783f, 0f, 0f, 0f, 0f);
			__39.StartData = new PhsxData(11093.81f, -629.6783f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__39);

			CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
			__40.Name = "pillar_dungeon_60";
			__40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__40.MyQuad.Quad.ExtraTexture1 = null;
			__40.MyQuad.Quad.ExtraTexture2 = null;
			__40.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__40.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__40.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__40.MyQuad.Quad.BlendAddRatio = 0f;

			__40.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 11829.67f, -785.6237f);

			__40.uv_speed = new Vector2(0f, 0f);
			__40.uv_offset = new Vector2(0f, 0f);
			__40.Data = new PhsxData(11829.67f, -785.6237f, 0f, 0f, 0f, 0f);
			__40.StartData = new PhsxData(11829.67f, -785.6237f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__40);

			CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
			__41.Name = "pillar_dungeon_60";
			__41.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(12790.1f, 1521.875f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__41.MyQuad.Quad.v0.Pos = new Vector2(-0.9997222f, 1f);

			__41.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(13059.11f, 1521.875f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__41.MyQuad.Quad.v1.Pos = new Vector2(1.000278f, 1f);

			__41.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(12790.1f, -2390.978f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__41.MyQuad.Quad.v2.Pos = new Vector2(-0.9997222f, -1f);

			__41.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(13059.11f, -2390.978f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__41.MyQuad.Quad.v3.Pos = new Vector2(1.000278f, -1f);

			__41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__41.MyQuad.Quad.ExtraTexture1 = null;
			__41.MyQuad.Quad.ExtraTexture2 = null;
			__41.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__41.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__41.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__41.MyQuad.Quad.BlendAddRatio = 0f;

			__41.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 12924.57f, -434.5516f);

			__41.uv_speed = new Vector2(0f, 0f);
			__41.uv_offset = new Vector2(0f, 0f);
			__41.Data = new PhsxData(12924.57f, -434.5516f, 0f, 0f, 0f, 0f);
			__41.StartData = new PhsxData(12924.57f, -434.5516f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__41);

			CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
			__42.Name = "pillar_dungeon_60";
			__42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__42.MyQuad.Quad.ExtraTexture1 = null;
			__42.MyQuad.Quad.ExtraTexture2 = null;
			__42.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__42.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__42.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__42.MyQuad.Quad.BlendAddRatio = 0f;

			__42.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 15195.52f, -1263.012f);

			__42.uv_speed = new Vector2(0f, 0f);
			__42.uv_offset = new Vector2(0f, 0f);
			__42.Data = new PhsxData(15195.52f, -1263.012f, 0f, 0f, 0f, 0f);
			__42.StartData = new PhsxData(15195.52f, -1263.012f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__42);

			CloudberryKingdom.BackgroundFloater __43 = new CloudberryKingdom.BackgroundFloater();
			__43.Name = "pillar_dungeon_60";
			__43.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(17174.49f, 1240.86f), new Vector2(0f, 0f), new Color(192, 192, 192, 255));
			__43.MyQuad.Quad.v0.Pos = new Vector2(-1.001944f, 1.004722f);

			__43.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(17443.5f, 1240.86f), new Vector2(1f, 0f), new Color(192, 192, 192, 255));
			__43.MyQuad.Quad.v1.Pos = new Vector2(0.9980557f, 1.004722f);

			__43.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(17174.49f, -2671.992f), new Vector2(0f, 1f), new Color(192, 192, 192, 255));
			__43.MyQuad.Quad.v2.Pos = new Vector2(-1.001944f, -0.9952778f);

			__43.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(17443.5f, -2671.992f), new Vector2(1f, 1f), new Color(192, 192, 192, 255));
			__43.MyQuad.Quad.v3.Pos = new Vector2(0.9980557f, -0.9952778f);

			__43.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__43.MyQuad.Quad.ExtraTexture1 = null;
			__43.MyQuad.Quad.ExtraTexture2 = null;
			__43.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__43.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__43.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__43.MyQuad.Quad.BlendAddRatio = 0f;

			__43.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 17309.26f, -724.8051f);

			__43.uv_speed = new Vector2(0f, 0f);
			__43.uv_offset = new Vector2(0f, 0f);
			__43.Data = new PhsxData(17309.26f, -724.8051f, 0f, 0f, 0f, 0f);
			__43.StartData = new PhsxData(17309.26f, -724.8051f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__43);

			CloudberryKingdom.BackgroundFloater __44 = new CloudberryKingdom.BackgroundFloater();
			__44.Name = "pillar_dungeon_60";
			__44.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__44.MyQuad.Quad.ExtraTexture1 = null;
			__44.MyQuad.Quad.ExtraTexture2 = null;
			__44.MyQuad.Quad._MyTexture = Tools.Texture("pillar_dungeon_60");
			__44.MyQuad.Quad.MySetColor = new Color(192, 192, 192, 255);
			__44.MyQuad.Quad.PremultipliedColor = new Color(192, 192, 192, 255);
			__44.MyQuad.Quad.BlendAddRatio = 0f;

			__44.MyQuad.Base = new CoreEngine.BasePoint(134.5043f, 0f, 0f, 1956.426f, 20311.2f, -1100.049f);

			__44.uv_speed = new Vector2(0f, 0f);
			__44.uv_offset = new Vector2(0f, 0f);
			__44.Data = new PhsxData(20311.2f, -1100.049f, 0f, 0f, 0f, 0f);
			__44.StartData = new PhsxData(20311.2f, -1100.049f, 0f, 0f, 0f, 0f);
			__27.Floaters.Add(__44);

			__27.Parallax = 0.57f;
			__27.DoPreDraw = false;
			b.MyCollection.Lists.Add(__27);

			CloudberryKingdom.BackgroundFloaterList __45 = new CloudberryKingdom.BackgroundFloaterList();
			__45.Name = "Fog3";
			__45.Foreground = true;
			__45.Fixed = false;
			CloudberryKingdom.BackgroundFloater __46 = new CloudberryKingdom.BackgroundFloater();
			__46.Name = "Dungeon_Smoke";
			__46.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-2295.65f, -180.1815f), new Vector2(15.78773f, 0f), new Color(255, 255, 255, 255));
			__46.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__46.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(29704.35f, -180.1815f), new Vector2(79.79308f, 0f), new Color(255, 255, 255, 255));
			__46.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__46.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-2295.65f, -967.9669f), new Vector2(15.78773f, 1f), new Color(255, 255, 255, 255));
			__46.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__46.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(29704.35f, -967.9669f), new Vector2(79.79308f, 1f), new Color(255, 255, 255, 255));
			__46.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__46.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__46.MyQuad.Quad.ExtraTexture1 = null;
			__46.MyQuad.Quad.ExtraTexture2 = null;
			__46.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Smoke");
			__46.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
			__46.MyQuad.Quad.BlendAddRatio = 0f;

			__46.MyQuad.Base = new CoreEngine.BasePoint(16000f, 0f, 0f, 393.8927f, 13704.35f, -574.0742f);

			__46.uv_speed = new Vector2(0.0042f, 0f);
			__46.uv_offset = new Vector2(9.90776f, 0f);
			__46.Data = new PhsxData(13704.35f, -574.0742f, 0f, 0f, 0f, 0f);
			__46.StartData = new PhsxData(13704.35f, -574.0742f, 0f, 0f, 0f, 0f);
			__45.Floaters.Add(__46);

			CloudberryKingdom.BackgroundFloater __47 = new CloudberryKingdom.BackgroundFloater();
			__47.Name = "Dungeon_Smoke_Thick";
			__47.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-1388.24f, -203.5316f), new Vector2(18.41902f, 0f), new Color(154, 154, 154, 144));
			__47.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

			__47.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(54611.76f, -203.5316f), new Vector2(82.41635f, 0f), new Color(154, 154, 154, 144));
			__47.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

			__47.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-1388.24f, -1579.537f), new Vector2(18.41902f, 1f), new Color(154, 154, 154, 144));
			__47.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

			__47.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(54611.76f, -1579.537f), new Vector2(82.41635f, 1f), new Color(154, 154, 154, 144));
			__47.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

			__47.MyQuad.Quad.MyEffect = Tools.BasicEffect;
			__47.MyQuad.Quad.ExtraTexture1 = null;
			__47.MyQuad.Quad.ExtraTexture2 = null;
			__47.MyQuad.Quad._MyTexture = Tools.Texture("Dungeon_Smoke_Thick");
			__47.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 155);
			__47.MyQuad.Quad.PremultipliedColor = new Color(154, 154, 154, 144);
			__47.MyQuad.Quad.BlendAddRatio = 0.06f;

			__47.MyQuad.Base = new CoreEngine.BasePoint(28000f, 0f, 0f, 688.0027f, 26611.76f, -891.5343f);

			__47.uv_speed = new Vector2(0.0049f, 0f);
			__47.uv_offset = new Vector2(11.55905f, 0f);
			__47.Data = new PhsxData(26611.76f, -891.5343f, 0f, 0f, 0f, 0f);
			__47.StartData = new PhsxData(26611.76f, -891.5343f, 0f, 0f, 0f, 0f);
			__45.Floaters.Add(__47);

			__45.Parallax = 1.05f;
			__45.DoPreDraw = false;
			b.MyCollection.Lists.Add(__45);

			b.Light = 1f;
			b.BL = new Vector2(-100000f, -10000f);
			b.TR = new Vector2(100000f, 10000f);

			foreach (var l in b.MyCollection.Lists)
				foreach (var f in l.Floaters)
					f.MyQuad.Quad.SetColor(f.MyQuad.Quad.PremultipliedColor, true);
					//f.MyQuad.Quad.MySetColor = f.MyQuad.Quad.PremultipliedColor;
		}
    }
}
