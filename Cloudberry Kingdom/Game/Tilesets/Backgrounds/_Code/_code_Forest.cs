using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void TurnOnSnow(Background b)
        {
            foreach (var l in b.MyCollection.Lists)
                if (l.Name.Contains("Snow"))
                    l.Show = true;
        }

        public static void TurnOffSnow(Background b)
        {
            foreach (var l in b.MyCollection.Lists)
                if (l.Name.Contains("Snow"))
                    l.Show = false;
        }

        public static void _code_Forest(Background b)
        {
            b.GuidCounter = 0;
            b.MyGlobalIllumination = 1f;
            b.AllowLava = true;
            CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
            __1.Name = "Layer";
            __1.Foreground = false;
            __1.Fixed = false;
            CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
            __2.Name = "forest_sky";
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;
            __2.MyQuad.Quad._MyTexture = Tools.Texture("forest_sky");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;

            __2.MyQuad.Base = new Drawing.BasePoint(492237.9f, 0f, 0f, 104122.2f, 135500.6f, 165.7959f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(135500.6f, 165.7959f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(135500.6f, 165.7959f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            __1.Parallax = 0.01f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __3 = new CloudberryKingdom.BackgroundFloaterList();
            __3.Name = "Layer";
            __3.Foreground = false;
            __3.Fixed = false;
            CloudberryKingdom.BackgroundFloater __4 = new CloudberryKingdom.BackgroundFloater();
            __4.Name = "forest_backhills_p1_0";
            __4.MyQuad.Quad._MyTexture = Tools.Texture("forest_backhills");
            __4.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.BlendAddRatio = 0f;
            __4.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __4.MyQuad.Quad.ExtraTexture1 = null;
            __4.MyQuad.Quad.ExtraTexture2 = null;

            __4.MyQuad.Base = new Drawing.BasePoint(32845.93f, 0f, 0f, 13895.69f, 3240.223f, 995.9708f);

            __4.uv_speed = new Vector2(0f, 0f);
            __4.uv_offset = new Vector2(0f, 0f);
            __4.Data = new PhsxData(3240.223f, 995.9708f, 0f, 0f, 0f, 0f);
            __4.StartData = new PhsxData(3240.223f, 995.9708f, 0f, 0f, 0f, 0f);
            __3.Floaters.Add(__4);

            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "forest_backhills_p2_0";
            __5.MyQuad.Quad._MyTexture = Tools.Texture("forest_backhills_p2");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;

            __5.MyQuad.Base = new Drawing.BasePoint(32845.93f, 0f, 0f, 13895.69f, 68932.08f, 995.9708f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(68932.08f, 995.9708f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(68932.08f, 995.9708f, 0f, 0f, 0f, 0f);
            __3.Floaters.Add(__5);

            __3.Parallax = 0.075f;
            __3.DoPreDraw = false;
            b.MyCollection.Lists.Add(__3);

            CloudberryKingdom.BackgroundFloaterList __6 = new CloudberryKingdom.BackgroundFloaterList();
            __6.Name = "Layer";
            __6.Foreground = false;
            __6.Fixed = false;
            CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
            __7.Name = "forest_mid_p1_0";
            __7.MyQuad.Quad._MyTexture = Tools.Texture("forest_mid");
            __7.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.BlendAddRatio = 0f;
            __7.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __7.MyQuad.Quad.ExtraTexture1 = null;
            __7.MyQuad.Quad.ExtraTexture2 = null;

            __7.MyQuad.Base = new Drawing.BasePoint(16237.1f, 0f, 0f, 6852.254f, 4230.581f, 884.8433f);

            __7.uv_speed = new Vector2(0f, 0f);
            __7.uv_offset = new Vector2(0f, 0f);
            __7.Data = new PhsxData(4230.581f, 884.8433f, 0f, 0f, 0f, 0f);
            __7.StartData = new PhsxData(4230.581f, 884.8433f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__7);

            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "forest_mid_p2_0";
            __8.MyQuad.Quad._MyTexture = Tools.Texture("forest_mid_p2");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;
            __8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __8.MyQuad.Quad.ExtraTexture1 = null;
            __8.MyQuad.Quad.ExtraTexture2 = null;

            __8.MyQuad.Base = new Drawing.BasePoint(16237.1f, 0f, 0f, 6852.254f, 36704.78f, 884.8433f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(36704.78f, 884.8433f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(36704.78f, 884.8433f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__8);

            __6.Parallax = 0.15f;
            __6.DoPreDraw = false;
            b.MyCollection.Lists.Add(__6);

            CloudberryKingdom.BackgroundFloaterList __9 = new CloudberryKingdom.BackgroundFloaterList();
            __9.Name = "Layer";
            __9.Foreground = false;
            __9.Fixed = false;
            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "forest_clouds";
            __10.MyQuad.Quad._MyTexture = Tools.Texture("forest_clouds");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;
            __10.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-26948.33f, 5153.887f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
            __10.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __10.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(162881.6f, 5153.887f), new Vector2(2.999816f, 0f), new Color(255, 255, 255, 255));
            __10.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __10.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-26948.33f, -2755.695f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
            __10.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __10.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(162881.6f, -2755.695f), new Vector2(2.999816f, 1f), new Color(255, 255, 255, 255));
            __10.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __10.MyQuad.Quad.ExtraTexture1 = null;
            __10.MyQuad.Quad.ExtraTexture2 = null;

            __10.MyQuad.Base = new Drawing.BasePoint(94914.98f, 0f, 0f, 3954.791f, 67966.65f, 1199.096f);

            __10.uv_speed = new Vector2(0.0003f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(67966.65f, 1199.096f, 0f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(67966.65f, 1199.096f, 0f, 0f, 0f, 0f);
            __9.Floaters.Add(__10);

            __9.Parallax = 0.19f;
            __9.DoPreDraw = false;
            b.MyCollection.Lists.Add(__9);

            CloudberryKingdom.BackgroundFloaterList __11 = new CloudberryKingdom.BackgroundFloaterList();
            __11.Name = "Layer";
            __11.Foreground = false;
            __11.Fixed = false;
            CloudberryKingdom.BackgroundFloater __12 = new CloudberryKingdom.BackgroundFloater();
            __12.Name = "forest_backtrees_p1_0";
            __12.MyQuad.Quad._MyTexture = Tools.Texture("forest_backtrees");
            __12.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.BlendAddRatio = 0f;
            __12.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __12.MyQuad.Quad.ExtraTexture1 = null;
            __12.MyQuad.Quad.ExtraTexture2 = null;

            __12.MyQuad.Base = new Drawing.BasePoint(10096.42f, 0f, 0f, 3263.351f, -1529.426f, -1261.67f);

            __12.uv_speed = new Vector2(0f, 0f);
            __12.uv_offset = new Vector2(0f, 0f);
            __12.Data = new PhsxData(-1529.426f, -1261.67f, 0f, 0f, 0f, 0f);
            __12.StartData = new PhsxData(-1529.426f, -1261.67f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__12);

            CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
            __13.Name = "forest_backtrees_p2_0";
            __13.MyQuad.Quad._MyTexture = Tools.Texture("forest_backtrees_p2");
            __13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.BlendAddRatio = 0f;
            __13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __13.MyQuad.Quad.ExtraTexture1 = null;
            __13.MyQuad.Quad.ExtraTexture2 = null;

            __13.MyQuad.Base = new Drawing.BasePoint(10576.94f, 0f, 0f, 3263.351f, 19143.93f, -1261.67f);

            __13.uv_speed = new Vector2(0f, 0f);
            __13.uv_offset = new Vector2(0f, 0f);
            __13.Data = new PhsxData(19143.93f, -1261.67f, 0f, 0f, 0f, 0f);
            __13.StartData = new PhsxData(19143.93f, -1261.67f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__13);

            CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
            __14.Name = "forest_backtrees_p1_0";
            __14.MyQuad.Quad._MyTexture = Tools.Texture("forest_backtrees");
            __14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.BlendAddRatio = 0f;
            __14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __14.MyQuad.Quad.ExtraTexture1 = null;
            __14.MyQuad.Quad.ExtraTexture2 = null;

            __14.MyQuad.Base = new Drawing.BasePoint(10072.41f, 0f, 0f, 3255.594f, 36512.66f, -1059.907f);

            __14.uv_speed = new Vector2(0f, 0f);
            __14.uv_offset = new Vector2(0f, 0f);
            __14.Data = new PhsxData(36512.66f, -1059.907f, 0f, 0f, 0f, 0f);
            __14.StartData = new PhsxData(36512.66f, -1059.907f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__14);

            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "forest_backtrees_p2_0";
            __15.MyQuad.Quad._MyTexture = Tools.Texture("forest_backtrees_p2");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;

            __15.MyQuad.Base = new Drawing.BasePoint(10551.8f, 0f, 0f, 3255.594f, 57136.88f, -1059.907f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(57136.88f, -1059.907f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(57136.88f, -1059.907f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__15);

            __11.Parallax = 0.25f;
            __11.DoPreDraw = false;
            b.MyCollection.Lists.Add(__11);

            CloudberryKingdom.BackgroundFloaterList __16 = new CloudberryKingdom.BackgroundFloaterList();
            __16.Name = "Snow1";
            __16.Foreground = false;
            __16.Fixed = false;
            CloudberryKingdom.BackgroundFloater __17 = new CloudberryKingdom.BackgroundFloater();
            __17.Name = "Snow";
            __17.MyQuad.Quad._MyTexture = Tools.Texture("Snow");
            __17.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 89);
            __17.MyQuad.Quad.PremultipliedColor = new Color(88, 88, 88, 88);
            __17.MyQuad.Quad.BlendAddRatio = 0f;
            __17.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-16817f, 4586.932f), new Vector2(18.35001f, -7.339994f), new Color(88, 88, 88, 88));
            __17.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __17.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(310331.8f, 4586.932f), new Vector2(38.34973f, -7.339994f), new Color(88, 88, 88, 88));
            __17.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __17.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-16817f, -4614.128f), new Vector2(18.35001f, -6.337544f), new Color(88, 88, 88, 88));
            __17.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __17.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(310331.8f, -4614.128f), new Vector2(38.34973f, -6.337544f), new Color(88, 88, 88, 88));
            __17.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __17.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __17.MyQuad.Quad.ExtraTexture1 = null;
            __17.MyQuad.Quad.ExtraTexture2 = null;

            __17.MyQuad.Base = new Drawing.BasePoint(163574.4f, 0f, 0f, 4600.53f, 146757.4f, -13.5979f);

            __17.uv_speed = new Vector2(0.001f, -0.0009f);
            __17.uv_offset = new Vector2(18.35001f, -7.339994f);
            __17.Data = new PhsxData(146757.4f, -13.5979f, 0f, 0f, 0f, 0f);
            __17.StartData = new PhsxData(146757.4f, -13.5979f, 0f, 0f, 0f, 0f);
            __16.Floaters.Add(__17);

            __16.Parallax = 0.25f;
            __16.DoPreDraw = false;
            b.MyCollection.Lists.Add(__16);

            CloudberryKingdom.BackgroundFloaterList __18 = new CloudberryKingdom.BackgroundFloaterList();
            __18.Name = "Layer";
            __18.Foreground = false;
            __18.Fixed = false;
            CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
            __19.Name = "forest_foretrees_p1_0";
            __19.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees");
            __19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.BlendAddRatio = 0f;
            __19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __19.MyQuad.Quad.ExtraTexture1 = null;
            __19.MyQuad.Quad.ExtraTexture2 = null;

            __19.MyQuad.Base = new Drawing.BasePoint(4487.996f, 0f, 0f, 1898.676f, 2154.271f, -47.44482f);

            __19.uv_speed = new Vector2(0f, 0f);
            __19.uv_offset = new Vector2(0f, 0f);
            __19.Data = new PhsxData(2154.271f, -47.44482f, 0f, 0f, 0f, 0f);
            __19.StartData = new PhsxData(2154.271f, -47.44482f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__19);

            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "forest_foretrees_p2_0";
            __20.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees_p2");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;

            __20.MyQuad.Base = new Drawing.BasePoint(4487.996f, 0f, 0f, 1898.676f, 11130.26f, -47.44482f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(11130.26f, -47.44482f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(11130.26f, -47.44482f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__20);

            CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
            __21.Name = "forest_foretrees_p1_0";
            __21.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees");
            __21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.BlendAddRatio = 0f;
            __21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __21.MyQuad.Quad.ExtraTexture1 = null;
            __21.MyQuad.Quad.ExtraTexture2 = null;

            __21.MyQuad.Base = new Drawing.BasePoint(4441.479f, 0f, 0f, 1878.997f, 19630.17f, -49.11255f);

            __21.uv_speed = new Vector2(0f, 0f);
            __21.uv_offset = new Vector2(0f, 0f);
            __21.Data = new PhsxData(19630.17f, -49.11255f, 0f, 0f, 0f, 0f);
            __21.StartData = new PhsxData(19630.17f, -49.11255f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__21);

            CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
            __22.Name = "forest_foretrees_p2_0";
            __22.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees_p2");
            __22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.BlendAddRatio = 0f;
            __22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __22.MyQuad.Quad.ExtraTexture1 = null;
            __22.MyQuad.Quad.ExtraTexture2 = null;

            __22.MyQuad.Base = new Drawing.BasePoint(4441.479f, 0f, 0f, 1878.997f, 28513.13f, -49.11255f);

            __22.uv_speed = new Vector2(0f, 0f);
            __22.uv_offset = new Vector2(0f, 0f);
            __22.Data = new PhsxData(28513.13f, -49.11255f, 0f, 0f, 0f, 0f);
            __22.StartData = new PhsxData(28513.13f, -49.11255f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__22);

            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "forest_foretrees_p1_0";
            __23.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;

            __23.MyQuad.Base = new Drawing.BasePoint(4441.479f, 0f, 0f, 1878.997f, 37097.56f, -92.27881f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(37097.56f, -92.27881f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(37097.56f, -92.27881f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__23);

            CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
            __24.Name = "forest_foretrees_p2_0";
            __24.MyQuad.Quad._MyTexture = Tools.Texture("forest_foretrees_p2");
            __24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.BlendAddRatio = 0f;
            __24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __24.MyQuad.Quad.ExtraTexture1 = null;
            __24.MyQuad.Quad.ExtraTexture2 = null;

            __24.MyQuad.Base = new Drawing.BasePoint(4441.479f, 0f, 0f, 1878.997f, 45980.52f, -92.27881f);

            __24.uv_speed = new Vector2(0f, 0f);
            __24.uv_offset = new Vector2(0f, 0f);
            __24.Data = new PhsxData(45980.52f, -92.27881f, 0f, 0f, 0f, 0f);
            __24.StartData = new PhsxData(45980.52f, -92.27881f, 0f, 0f, 0f, 0f);
            __18.Floaters.Add(__24);

            __18.Parallax = 0.55f;
            __18.DoPreDraw = false;
            b.MyCollection.Lists.Add(__18);

            CloudberryKingdom.BackgroundFloaterList __25 = new CloudberryKingdom.BackgroundFloaterList();
            __25.Name = "Snow2";
            __25.Foreground = true;
            __25.Fixed = false;
            CloudberryKingdom.BackgroundFloater __26 = new CloudberryKingdom.BackgroundFloater();
            __26.Name = "Snow";
            __26.MyQuad.Quad._MyTexture = Tools.Texture("Snow");
            __26.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 196);
            __26.MyQuad.Quad.PremultipliedColor = new Color(195, 195, 195, 195);
            __26.MyQuad.Quad.BlendAddRatio = 0f;
            __26.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-37182.87f, 1486.698f), new Vector2(0f, 0f), new Color(195, 195, 195, 195));
            __26.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __26.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(68532.89f, 1486.698f), new Vector2(19.98709f, 0f), new Color(195, 195, 195, 195));
            __26.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __26.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-37182.87f, -1486.698f), new Vector2(0f, 1.000068f), new Color(195, 195, 195, 195));
            __26.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __26.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(68532.89f, -1486.698f), new Vector2(19.98709f, 1.000068f), new Color(195, 195, 195, 195));
            __26.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __26.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __26.MyQuad.Quad.ExtraTexture1 = null;
            __26.MyQuad.Quad.ExtraTexture2 = null;

            __26.MyQuad.Base = new Drawing.BasePoint(52857.88f, 0f, 0f, 1486.698f, 15675.01f, 0f);

            __26.uv_speed = new Vector2(0.0015f, -0.0015f);
            __26.uv_offset = new Vector2(0f, 0f);
            __26.Data = new PhsxData(15675.01f, 0f, 0f, 0f, 0f, 0f);
            __26.StartData = new PhsxData(15675.01f, 0f, 0f, 0f, 0f, 0f);
            __25.Floaters.Add(__26);

            CloudberryKingdom.BackgroundFloater __27 = new CloudberryKingdom.BackgroundFloater();
            __27.Name = "Snow";
            __27.MyQuad.Quad._MyTexture = Tools.Texture("Snow");
            __27.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 224);
            __27.MyQuad.Quad.PremultipliedColor = new Color(223, 223, 223, 223);
            __27.MyQuad.Quad.BlendAddRatio = 0f;
            __27.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-4499.879f, 1865.506f), new Vector2(0f, 0f), new Color(223, 223, 223, 223));
            __27.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __27.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(118981.7f, 1865.506f), new Vector2(20.0145f, 0f), new Color(223, 223, 223, 223));
            __27.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __27.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-4499.879f, -1605.93f), new Vector2(0f, 0.999887f), new Color(223, 223, 223, 223));
            __27.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __27.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(118981.7f, -1605.93f), new Vector2(20.0145f, 0.999887f), new Color(223, 223, 223, 223));
            __27.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __27.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __27.MyQuad.Quad.ExtraTexture1 = null;
            __27.MyQuad.Quad.ExtraTexture2 = null;

            __27.MyQuad.Base = new Drawing.BasePoint(61740.77f, 0f, 0f, 1735.718f, 57240.89f, 129.7882f);

            __27.uv_speed = new Vector2(0.002f, -0.0023f);
            __27.uv_offset = new Vector2(0f, 0f);
            __27.Data = new PhsxData(57240.89f, 129.7882f, 0f, 0f, 0f, 0f);
            __27.StartData = new PhsxData(57240.89f, 129.7882f, 0f, 0f, 0f, 0f);
            __25.Floaters.Add(__27);

            __25.Parallax = 0.9f;
            __25.DoPreDraw = false;
            b.MyCollection.Lists.Add(__25);

            b.Light = 1f;
            b.BL = new Vector2(-4900f, -4500f);
            b.TR = new Vector2(28677.74f, 2055.556f);
        }
    }
}
