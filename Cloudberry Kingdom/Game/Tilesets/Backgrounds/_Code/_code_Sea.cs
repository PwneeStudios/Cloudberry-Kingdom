using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void _code_Sea(Background b)
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
            __2.MyQuad.Quad._MyTexture = Tools.Texture("sea_backdrop");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;

            __2.MyQuad.Base = new CoreEngine.BasePoint(40199f, 0f, 0f, 17007.27f, -3333.766f, 0f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(-3333.766f, 0f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(-3333.766f, 0f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
            __3.Name = "sea_backdrop_p2_0";
            __3.MyQuad.Quad._MyTexture = Tools.Texture("sea_backdrop_p2");
            __3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.BlendAddRatio = 0f;
            __3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __3.MyQuad.Quad.ExtraTexture1 = null;
            __3.MyQuad.Quad.ExtraTexture2 = null;

            __3.MyQuad.Base = new CoreEngine.BasePoint(40199f, 0f, 0f, 17007.27f, 77064.25f, 0f);

            __3.uv_speed = new Vector2(0f, 0f);
            __3.uv_offset = new Vector2(0f, 0f);
            __3.Data = new PhsxData(77064.25f, 0f, 0f, 0f, 0f, 0f);
            __3.StartData = new PhsxData(77064.25f, 0f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__3);

            __1.Parallax = 0.06f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __4 = new CloudberryKingdom.BackgroundFloaterList();
            __4.Name = "Layer";
            __4.Foreground = false;
            __4.Fixed = false;
            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "sea_behind_water_1";
            __5.MyQuad.Quad._MyTexture = Tools.Texture("sea_behind_water_1");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;

            __5.MyQuad.Base = new CoreEngine.BasePoint(2439.164f, 0f, 0f, 5446.352f, -13367.37f, -754.4899f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(-13367.37f, -754.4899f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(-13367.37f, -754.4899f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__5);

            CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
            __6.Name = "sea_behind_water_3";
            __6.MyQuad.Quad._MyTexture = Tools.Texture("sea_behind_water_3");
            __6.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.BlendAddRatio = 0f;
            __6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __6.MyQuad.Quad.ExtraTexture1 = null;
            __6.MyQuad.Quad.ExtraTexture2 = null;

            __6.MyQuad.Base = new CoreEngine.BasePoint(8032.311f, 0f, 0f, 2956.627f, 1839.004f, -3958.902f);

            __6.uv_speed = new Vector2(0f, 0f);
            __6.uv_offset = new Vector2(0f, 0f);
            __6.Data = new PhsxData(1839.004f, -3958.902f, 0f, 0f, 0f, 0f);
            __6.StartData = new PhsxData(1839.004f, -3958.902f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__6);

            CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
            __7.Name = "sea_behind_water_2";
            __7.MyQuad.Quad._MyTexture = Tools.Texture("sea_behind_water_2");
            __7.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.BlendAddRatio = 0f;
            __7.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __7.MyQuad.Quad.ExtraTexture1 = null;
            __7.MyQuad.Quad.ExtraTexture2 = null;

            __7.MyQuad.Base = new CoreEngine.BasePoint(3021.783f, 0f, 0f, 4421.52f, 14889.68f, -2327.567f);

            __7.uv_speed = new Vector2(0f, 0f);
            __7.uv_offset = new Vector2(0f, 0f);
            __7.Data = new PhsxData(14889.68f, -2327.567f, 0f, 0f, 0f, 0f);
            __7.StartData = new PhsxData(14889.68f, -2327.567f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__7);

            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "sea_behind_water_3";
            __8.MyQuad.Quad._MyTexture = Tools.Texture("sea_behind_water_3");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;
            __8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __8.MyQuad.Quad.ExtraTexture1 = null;
            __8.MyQuad.Quad.ExtraTexture2 = null;

            __8.MyQuad.Base = new CoreEngine.BasePoint(8032.311f, 0f, 0f, 2956.627f, 33263.91f, -4019.146f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(33263.91f, -4019.146f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(33263.91f, -4019.146f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__8);

            __4.Parallax = 0.15f;
            __4.DoPreDraw = false;
            b.MyCollection.Lists.Add(__4);

            CloudberryKingdom.BackgroundFloaterList __9 = new CloudberryKingdom.BackgroundFloaterList();
            __9.Name = "Layer";
            __9.Foreground = false;
            __9.Fixed = false;
            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "sea_seamonster";
            __10.MyQuad.Quad._MyTexture = Tools.Texture("sea_seamonster");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;
            __10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __10.MyQuad.Quad.ExtraTexture1 = null;
            __10.MyQuad.Quad.ExtraTexture2 = null;

            __10.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 817.7778f, 4668.237f, -2436.043f);

            __10.uv_speed = new Vector2(0f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(4668.237f, -2436.043f, 10f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(4668.237f, -2436.043f, 10f, 0f, 0f, 0f);
            __9.Floaters.Add(__10);

            __9.Parallax = 0.25f;
            __9.DoPreDraw = false;
            b.MyCollection.Lists.Add(__9);

            CloudberryKingdom.BackgroundFloaterList __11 = new CloudberryKingdom.BackgroundFloaterList();
            __11.Name = "Layer";
            __11.Foreground = false;
            __11.Fixed = false;
            CloudberryKingdom.BackgroundFloater __12 = new CloudberryKingdom.BackgroundFloater();
            __12.Name = "sea_clouds";
            __12.MyQuad.Quad._MyTexture = Tools.Texture("sea_clouds");
            __12.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.BlendAddRatio = 0f;
            __12.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-9694.891f, 3457.797f), new Vector2(1.241851f, 0f), new Color(255, 255, 255, 255));
            __12.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __12.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(47179.37f, 3457.797f), new Vector2(3.242014f, 0f), new Color(255, 255, 255, 255));
            __12.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __12.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-9694.891f, -3651.485f), new Vector2(1.241851f, 1f), new Color(255, 255, 255, 255));
            __12.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __12.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(47179.37f, -3651.485f), new Vector2(3.242014f, 1f), new Color(255, 255, 255, 255));
            __12.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __12.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __12.MyQuad.Quad.ExtraTexture1 = null;
            __12.MyQuad.Quad.ExtraTexture2 = null;

            __12.MyQuad.Base = new CoreEngine.BasePoint(28437.13f, 0f, 0f, 3554.641f, 18742.24f, -96.84418f);

            __12.uv_speed = new Vector2(0.00025f, 0f);
            __12.uv_offset = new Vector2(1.241851f, 0f);
            __12.Data = new PhsxData(18742.24f, -96.84418f, 0f, 0f, 0f, 0f);
            __12.StartData = new PhsxData(18742.24f, -96.84418f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__12);

            __11.Parallax = 0.2f;
            __11.DoPreDraw = false;
            b.MyCollection.Lists.Add(__11);

            CloudberryKingdom.BackgroundFloaterList __13 = new CloudberryKingdom.BackgroundFloaterList();
            __13.Name = "Layer";
            __13.Foreground = false;
            __13.Fixed = false;
            CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
            __14.Name = "sea_water_1";
            __14.MyQuad.Quad._MyTexture = Tools.Texture("sea_water_1");
            __14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.BlendAddRatio = 0f;
            __14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __14.MyQuad.Quad.ExtraTexture1 = null;
            __14.MyQuad.Quad.ExtraTexture2 = null;

            __14.MyQuad.Base = new CoreEngine.BasePoint(7761.776f, 0f, 0f, 557.1197f, 1360.16f, -2833.725f);

            __14.uv_speed = new Vector2(0f, 0f);
            __14.uv_offset = new Vector2(0f, 0f);
            __14.Data = new PhsxData(1360.16f, -2833.725f, 0f, 0f, 0f, 0f);
            __14.StartData = new PhsxData(1360.16f, -2833.725f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__14);

            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "sea_water_2";
            __15.MyQuad.Quad._MyTexture = Tools.Texture("sea_water_2");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;

            __15.MyQuad.Base = new CoreEngine.BasePoint(7761.776f, 0f, 0f, 557.1197f, 16882.17f, -2834.259f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(16882.17f, -2834.259f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(16882.17f, -2834.259f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__15);

            CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
            __16.Name = "sea_rock_1";
            __16.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_1");
            __16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.BlendAddRatio = 0f;
            __16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __16.MyQuad.Quad.ExtraTexture1 = null;
            __16.MyQuad.Quad.ExtraTexture2 = null;

            __16.MyQuad.Base = new CoreEngine.BasePoint(1200f, 0f, 0f, 1316.289f, 644.2549f, -1623.055f);

            __16.uv_speed = new Vector2(0f, 0f);
            __16.uv_offset = new Vector2(0f, 0f);
            __16.Data = new PhsxData(644.2549f, -1623.055f, 0f, 0f, 0f, 0f);
            __16.StartData = new PhsxData(644.2549f, -1623.055f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__16);

            CloudberryKingdom.BackgroundFloater __17 = new CloudberryKingdom.BackgroundFloater();
            __17.Name = "sea_rock_2";
            __17.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_2");
            __17.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.BlendAddRatio = 0f;
            __17.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __17.MyQuad.Quad.ExtraTexture1 = null;
            __17.MyQuad.Quad.ExtraTexture2 = null;

            __17.MyQuad.Base = new CoreEngine.BasePoint(1200f, 0f, 0f, 1008.589f, 10703.01f, -2166.667f);

            __17.uv_speed = new Vector2(0f, 0f);
            __17.uv_offset = new Vector2(0f, 0f);
            __17.Data = new PhsxData(10703.01f, -2166.667f, 0f, 0f, 0f, 0f);
            __17.StartData = new PhsxData(10703.01f, -2166.667f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__17);

            CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
            __18.Name = "sea_rock_3";
            __18.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_3");
            __18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.BlendAddRatio = 0f;
            __18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __18.MyQuad.Quad.ExtraTexture1 = null;
            __18.MyQuad.Quad.ExtraTexture2 = null;

            __18.MyQuad.Base = new CoreEngine.BasePoint(3412.958f, 0f, 0f, 3211.681f, -3843.269f, -342.5923f);

            __18.uv_speed = new Vector2(0f, 0f);
            __18.uv_offset = new Vector2(0f, 0f);
            __18.Data = new PhsxData(-3843.269f, -342.5923f, 0f, 0f, 0f, 0f);
            __18.StartData = new PhsxData(-3843.269f, -342.5923f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__18);

            CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
            __19.Name = "sea_rock_4";
            __19.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_4");
            __19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.BlendAddRatio = 0f;
            __19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __19.MyQuad.Quad.ExtraTexture1 = null;
            __19.MyQuad.Quad.ExtraTexture2 = null;

            __19.MyQuad.Base = new CoreEngine.BasePoint(220.506f, 0f, 0f, 162.2992f, 9069.232f, -2839.639f);

            __19.uv_speed = new Vector2(0f, 0f);
            __19.uv_offset = new Vector2(0f, 0f);
            __19.Data = new PhsxData(9069.232f, -2839.639f, 0f, 0f, 0f, 0f);
            __19.StartData = new PhsxData(9069.232f, -2839.639f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__19);

            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "sea_rock_5";
            __20.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_5");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;

            __20.MyQuad.Base = new CoreEngine.BasePoint(2536.874f, 0f, 0f, 1736.723f, 14098.87f, -1460.87f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(14098.87f, -1460.87f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(14098.87f, -1460.87f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__20);

            CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
            __21.Name = "sea_rock_1";
            __21.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_1");
            __21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.BlendAddRatio = 0f;
            __21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __21.MyQuad.Quad.ExtraTexture1 = null;
            __21.MyQuad.Quad.ExtraTexture2 = null;

            __21.MyQuad.Base = new CoreEngine.BasePoint(1200f, 0f, 0f, 1316.289f, 6594.551f, -1877.636f);

            __21.uv_speed = new Vector2(0f, 0f);
            __21.uv_offset = new Vector2(0f, 0f);
            __21.Data = new PhsxData(6594.551f, -1877.636f, 0f, 0f, 0f, 0f);
            __21.StartData = new PhsxData(6594.551f, -1877.636f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__21);

            CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
            __22.Name = "sea_rock_2";
            __22.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_2");
            __22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.BlendAddRatio = 0f;
            __22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __22.MyQuad.Quad.ExtraTexture1 = null;
            __22.MyQuad.Quad.ExtraTexture2 = null;

            __22.MyQuad.Base = new CoreEngine.BasePoint(1200f, 0f, 0f, 1008.589f, 18768.2f, -2215.924f);

            __22.uv_speed = new Vector2(0f, 0f);
            __22.uv_offset = new Vector2(0f, 0f);
            __22.Data = new PhsxData(18768.2f, -2215.924f, 0f, 0f, 0f, 0f);
            __22.StartData = new PhsxData(18768.2f, -2215.924f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__22);

            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "sea_rock_4";
            __23.MyQuad.Quad._MyTexture = Tools.Texture("sea_rock_4");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;

            __23.MyQuad.Base = new CoreEngine.BasePoint(220.506f, 0f, 0f, 162.2992f, 4206.418f, -2972.248f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(4206.418f, -2972.248f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(4206.418f, -2972.248f, 0f, 0f, 0f, 0f);
            __13.Floaters.Add(__23);

            __13.Parallax = 0.3f;
            __13.DoPreDraw = false;
            b.MyCollection.Lists.Add(__13);

            b.Light = 1f;
            //b.BL = new Vector2(-4630f, -4500f);
            //b.TR = new Vector2(28986.07f, 1452.433f);
            b.BL = new Vector2(-100000, -10000);
            b.TR = new Vector2(100000, 10000);
        }
    }
}
