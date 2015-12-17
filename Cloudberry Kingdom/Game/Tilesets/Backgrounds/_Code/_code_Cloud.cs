using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void _code_Cloud(Background b)
        {
            b.GuidCounter = 0;
            b.MyGlobalIllumination = 1f;
            b.AllowLava = true;
            CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
            __1.Name = "Layer";
            __1.Foreground = false;
            __1.Fixed = false;
            CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
            __2.Name = "cloud_castle_layer5";
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;
            __2.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer5");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;

            __2.MyQuad.Base = new CoreEngine.BasePoint(23579.31f, 0f, 0f, 10479.69f, 5485.83f, -9.382813f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(5485.83f, -9.382813f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(5485.83f, -9.382813f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
            __3.Name = "cloud_castle_layer5";
            __3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __3.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer5_p2");
            __3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.BlendAddRatio = 0f;
            __3.MyQuad.Quad.ExtraTexture1 = null;
            __3.MyQuad.Quad.ExtraTexture2 = null;

            __3.MyQuad.Base = new CoreEngine.BasePoint(23579.31f, 0f, 0f, 10479.69f, 52644.45f, -9.382813f);

            __3.uv_speed = new Vector2(0f, 0f);
            __3.uv_offset = new Vector2(0f, 0f);
            __3.Data = new PhsxData(52644.45f, -9.382813f, 0f, 0f, 0f, 0f);
            __3.StartData = new PhsxData(52644.45f, -9.382813f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__3);

            __1.Parallax = 0.1f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __4 = new CloudberryKingdom.BackgroundFloaterList();
            __4.Name = "Layer";
            __4.Foreground = false;
            __4.Fixed = false;
            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "cloud_castle_layer4";
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer4");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;

            __5.MyQuad.Base = new CoreEngine.BasePoint(13583.82f, 0f, 0f, 6025.19f, 3683.155f, -164.6855f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(3683.155f, -164.6855f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(3683.155f, -164.6855f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__5);

            CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
            __6.Name = "cloud_castle_layer4";
            __6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __6.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer4_p2");
            __6.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.BlendAddRatio = 0f;
            __6.MyQuad.Quad.ExtraTexture1 = null;
            __6.MyQuad.Quad.ExtraTexture2 = null;

            __6.MyQuad.Base = new CoreEngine.BasePoint(13583.82f, 0f, 0f, 6025.19f, 30850.79f, -164.6855f);

            __6.uv_speed = new Vector2(0f, 0f);
            __6.uv_offset = new Vector2(0f, 0f);
            __6.Data = new PhsxData(30850.79f, -164.6855f, 0f, 0f, 0f, 0f);
            __6.StartData = new PhsxData(30850.79f, -164.6855f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__6);

            __4.Parallax = 0.17f;
            __4.DoPreDraw = false;
            b.MyCollection.Lists.Add(__4);

            CloudberryKingdom.BackgroundFloaterList __7 = new CloudberryKingdom.BackgroundFloaterList();
            __7.Name = "cloud_3";
            __7.Foreground = false;
            __7.Fixed = false;
            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "cloud_cloud_layer3_1";
            __8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __8.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_1");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;
            __8.MyQuad.Quad.ExtraTexture1 = null;
            __8.MyQuad.Quad.ExtraTexture2 = null;

            __8.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 813.913f, -7449.813f, 713.1904f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(-7449.813f, 713.1904f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(-7449.813f, 713.1904f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__8);

            CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
            __9.Name = "cloud_cloud_layer3_2";
            __9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __9.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_2");
            __9.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.BlendAddRatio = 0f;
            __9.MyQuad.Quad.ExtraTexture1 = null;
            __9.MyQuad.Quad.ExtraTexture2 = null;

            __9.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 872.7272f, -2406.215f, 935.2354f);

            __9.uv_speed = new Vector2(0f, 0f);
            __9.uv_offset = new Vector2(0f, 0f);
            __9.Data = new PhsxData(-2406.215f, 935.2354f, 0f, 0f, 0f, 0f);
            __9.StartData = new PhsxData(-2406.215f, 935.2354f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__9);

            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "cloud_cloud_layer3_3";
            __10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __10.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_3");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;
            __10.MyQuad.Quad.ExtraTexture1 = null;
            __10.MyQuad.Quad.ExtraTexture2 = null;

            __10.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 804.7058f, 2447.061f, 205.6582f);

            __10.uv_speed = new Vector2(0f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(2447.061f, 205.6582f, 0f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(2447.061f, 205.6582f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__10);

            CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
            __11.Name = "cloud_cloud_layer3_4";
            __11.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __11.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_4");
            __11.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.BlendAddRatio = 0f;
            __11.MyQuad.Quad.ExtraTexture1 = null;
            __11.MyQuad.Quad.ExtraTexture2 = null;

            __11.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 854.9999f, 5904.623f, 78.77539f);

            __11.uv_speed = new Vector2(0f, 0f);
            __11.uv_offset = new Vector2(0f, 0f);
            __11.Data = new PhsxData(5904.623f, 78.77539f, 0f, 0f, 0f, 0f);
            __11.StartData = new PhsxData(5904.623f, 78.77539f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__11);

            CloudberryKingdom.BackgroundFloater __12 = new CloudberryKingdom.BackgroundFloater();
            __12.Name = "cloud_cloud_layer3_5";
            __12.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __12.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_5");
            __12.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.BlendAddRatio = 0f;
            __12.MyQuad.Quad.ExtraTexture1 = null;
            __12.MyQuad.Quad.ExtraTexture2 = null;

            __12.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 606.8827f, 9330.461f, 522.8662f);

            __12.uv_speed = new Vector2(0f, 0f);
            __12.uv_offset = new Vector2(0f, 0f);
            __12.Data = new PhsxData(9330.461f, 522.8662f, 0f, 0f, 0f, 0f);
            __12.StartData = new PhsxData(9330.461f, 522.8662f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__12);

            CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
            __13.Name = "cloud_cloud_layer3_6";
            __13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __13.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer3_6");
            __13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.BlendAddRatio = 0f;
            __13.MyQuad.Quad.ExtraTexture1 = null;
            __13.MyQuad.Quad.ExtraTexture2 = null;

            __13.MyQuad.Base = new CoreEngine.BasePoint(1440f, 0f, 0f, 1008f, 12312.21f, 15.33398f);

            __13.uv_speed = new Vector2(0f, 0f);
            __13.uv_offset = new Vector2(0f, 0f);
            __13.Data = new PhsxData(12312.21f, 15.33398f, 0f, 0f, 0f, 0f);
            __13.StartData = new PhsxData(12312.21f, 15.33398f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__13);

            __7.Parallax = 0.25f;
            __7.DoPreDraw = false;
            b.MyCollection.Lists.Add(__7);

            CloudberryKingdom.BackgroundFloaterList __14 = new CloudberryKingdom.BackgroundFloaterList();
            __14.Name = "castle_3";
            __14.Foreground = false;
            __14.Fixed = false;
            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "cloud_castle_layer3_1";
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_1");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;

            __15.MyQuad.Base = new CoreEngine.BasePoint(1619.587f, 0f, 0f, 1574.057f, -3419.336f, -1515.219f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(-3419.336f, -1515.219f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(-3419.336f, -1515.219f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__15);

            CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
            __16.Name = "cloud_castle_layer3_2";
            __16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __16.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_2");
            __16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.BlendAddRatio = 0f;
            __16.MyQuad.Quad.ExtraTexture1 = null;
            __16.MyQuad.Quad.ExtraTexture2 = null;

            __16.MyQuad.Base = new CoreEngine.BasePoint(1090.909f, 0f, 0f, 1556.735f, 6745.723f, -1731.496f);

            __16.uv_speed = new Vector2(0f, 0f);
            __16.uv_offset = new Vector2(0f, 0f);
            __16.Data = new PhsxData(6745.723f, -1731.496f, 0f, 0f, 0f, 0f);
            __16.StartData = new PhsxData(6745.723f, -1731.496f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__16);

            CloudberryKingdom.BackgroundFloater __17 = new CloudberryKingdom.BackgroundFloater();
            __17.Name = "cloud_castle_layer3_3";
            __17.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __17.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_3");
            __17.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.BlendAddRatio = 0f;
            __17.MyQuad.Quad.ExtraTexture1 = null;
            __17.MyQuad.Quad.ExtraTexture2 = null;

            __17.MyQuad.Base = new CoreEngine.BasePoint(1114.938f, 0f, 0f, 2341.099f, 10662.76f, -1642.704f);

            __17.uv_speed = new Vector2(0f, 0f);
            __17.uv_offset = new Vector2(0f, 0f);
            __17.Data = new PhsxData(10662.76f, -1642.704f, 0f, 0f, 0f, 0f);
            __17.StartData = new PhsxData(10662.76f, -1642.704f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__17);

            CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
            __18.Name = "cloud_castle_layer3_1";
            __18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __18.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_1");
            __18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.BlendAddRatio = 0f;
            __18.MyQuad.Quad.ExtraTexture1 = null;
            __18.MyQuad.Quad.ExtraTexture2 = null;

            __18.MyQuad.Base = new CoreEngine.BasePoint(1619.587f, 0f, 0f, 1574.057f, 18973.14f, -1646.107f);

            __18.uv_speed = new Vector2(0f, 0f);
            __18.uv_offset = new Vector2(0f, 0f);
            __18.Data = new PhsxData(18973.14f, -1646.107f, 0f, 0f, 0f, 0f);
            __18.StartData = new PhsxData(18973.14f, -1646.107f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__18);

            CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
            __19.Name = "cloud_castle_layer3_2";
            __19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __19.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_2");
            __19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.BlendAddRatio = 0f;
            __19.MyQuad.Quad.ExtraTexture1 = null;
            __19.MyQuad.Quad.ExtraTexture2 = null;

            __19.MyQuad.Base = new CoreEngine.BasePoint(1090.909f, 0f, 0f, 1556.735f, 24460.43f, -1646.107f);

            __19.uv_speed = new Vector2(0f, 0f);
            __19.uv_offset = new Vector2(0f, 0f);
            __19.Data = new PhsxData(24460.43f, -1646.107f, 0f, 0f, 0f, 0f);
            __19.StartData = new PhsxData(24460.43f, -1646.107f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__19);

            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "cloud_castle_layer3_3";
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer3_3");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;

            __20.MyQuad.Base = new CoreEngine.BasePoint(1114.938f, 0f, 0f, 2341.099f, 27066.12f, -1891.348f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(27066.12f, -1891.348f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(27066.12f, -1891.348f, 0f, 0f, 0f, 0f);
            __14.Floaters.Add(__20);

            __14.Parallax = 0.33f;
            __14.DoPreDraw = false;
            b.MyCollection.Lists.Add(__14);

            CloudberryKingdom.BackgroundFloaterList __21 = new CloudberryKingdom.BackgroundFloaterList();
            __21.Name = "cloud_2";
            __21.Foreground = false;
            __21.Fixed = false;
            CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
            __22.Name = "cloud_cloud_layer2_1";
            __22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __22.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer2_1");
            __22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.BlendAddRatio = 0f;
            __22.MyQuad.Quad.ExtraTexture1 = null;
            __22.MyQuad.Quad.ExtraTexture2 = null;

            __22.MyQuad.Base = new CoreEngine.BasePoint(4429.467f, 0f, 0f, 1100.83f, 1758.984f, 54.01685f);

            __22.uv_speed = new Vector2(0f, 0f);
            __22.uv_offset = new Vector2(0f, 0f);
            __22.Data = new PhsxData(1758.984f, 54.01685f, 0f, 0f, 0f, 0f);
            __22.StartData = new PhsxData(1758.984f, 54.01685f, 0f, 0f, 0f, 0f);
            __21.Floaters.Add(__22);

            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "cloud_cloud_layer2_1";
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer2_1");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;

            __23.MyQuad.Base = new CoreEngine.BasePoint(4429.467f, 0f, 0f, 1100.83f, 15294.52f, 3.80072f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(15294.52f, 3.80072f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(15294.52f, 3.80072f, 0f, 0f, 0f, 0f);
            __21.Floaters.Add(__23);

            CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
            __24.Name = "cloud_cloud_layer2_1";
            __24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __24.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer2_1");
            __24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.BlendAddRatio = 0f;
            __24.MyQuad.Quad.ExtraTexture1 = null;
            __24.MyQuad.Quad.ExtraTexture2 = null;

            __24.MyQuad.Base = new CoreEngine.BasePoint(4429.467f, 0f, 0f, 1100.83f, 26639.14f, -429.751f);

            __24.uv_speed = new Vector2(0f, 0f);
            __24.uv_offset = new Vector2(0f, 0f);
            __24.Data = new PhsxData(26639.14f, -429.751f, 0f, 0f, 0f, 0f);
            __24.StartData = new PhsxData(26639.14f, -429.751f, 0f, 0f, 0f, 0f);
            __21.Floaters.Add(__24);

            CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
            __25.Name = "cloud_cloud_layer2_1";
            __25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __25.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer2_1");
            __25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.BlendAddRatio = 0f;
            __25.MyQuad.Quad.ExtraTexture1 = null;
            __25.MyQuad.Quad.ExtraTexture2 = null;

            __25.MyQuad.Base = new CoreEngine.BasePoint(4429.467f, 0f, 0f, 1100.83f, 38561.82f, -502.0099f);

            __25.uv_speed = new Vector2(0f, 0f);
            __25.uv_offset = new Vector2(0f, 0f);
            __25.Data = new PhsxData(38561.82f, -502.0099f, 0f, 0f, 0f, 0f);
            __25.StartData = new PhsxData(38561.82f, -502.0099f, 0f, 0f, 0f, 0f);
            __21.Floaters.Add(__25);

            __21.Parallax = 0.28f;
            __21.DoPreDraw = false;
            b.MyCollection.Lists.Add(__21);

            CloudberryKingdom.BackgroundFloaterList __26 = new CloudberryKingdom.BackgroundFloaterList();
            __26.Name = "castle_2";
            __26.Foreground = false;
            __26.Fixed = false;
            CloudberryKingdom.BackgroundFloater __27 = new CloudberryKingdom.BackgroundFloater();
            __27.Name = "cloud_castle_layer2_1";
            __27.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __27.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_1");
            __27.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __27.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __27.MyQuad.Quad.BlendAddRatio = 0f;
            __27.MyQuad.Quad.ExtraTexture1 = null;
            __27.MyQuad.Quad.ExtraTexture2 = null;

            __27.MyQuad.Base = new CoreEngine.BasePoint(1350.048f, 0f, 0f, 2798.974f, -2574.793f, -1026.896f);

            __27.uv_speed = new Vector2(0f, 0f);
            __27.uv_offset = new Vector2(0f, 0f);
            __27.Data = new PhsxData(-2574.793f, -1026.896f, 0f, 0f, 0f, 0f);
            __27.StartData = new PhsxData(-2574.793f, -1026.896f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__27);

            CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
            __28.Name = "cloud_castle_layer2_2";
            __28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __28.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_2");
            __28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.BlendAddRatio = 0f;
            __28.MyQuad.Quad.ExtraTexture1 = null;
            __28.MyQuad.Quad.ExtraTexture2 = null;

            __28.MyQuad.Base = new CoreEngine.BasePoint(1415.46f, 0f, 0f, 2348.253f, 4617.543f, -726.7338f);

            __28.uv_speed = new Vector2(0f, 0f);
            __28.uv_offset = new Vector2(0f, 0f);
            __28.Data = new PhsxData(4617.543f, -726.7338f, 0f, 0f, 0f, 0f);
            __28.StartData = new PhsxData(4617.543f, -726.7338f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__28);

            CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
            __29.Name = "cloud_castle_layer2_3";
            __29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __29.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_3");
            __29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.BlendAddRatio = 0f;
            __29.MyQuad.Quad.ExtraTexture1 = null;
            __29.MyQuad.Quad.ExtraTexture2 = null;

            __29.MyQuad.Base = new CoreEngine.BasePoint(1256.856f, 0f, 0f, 2094.533f, 13222.73f, -543.1658f);

            __29.uv_speed = new Vector2(0f, 0f);
            __29.uv_offset = new Vector2(0f, 0f);
            __29.Data = new PhsxData(13222.73f, -543.1658f, 0f, 0f, 0f, 0f);
            __29.StartData = new PhsxData(13222.73f, -543.1658f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__29);

            CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
            __30.Name = "cloud_castle_layer2_1";
            __30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __30.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_1");
            __30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.BlendAddRatio = 0f;
            __30.MyQuad.Quad.ExtraTexture1 = null;
            __30.MyQuad.Quad.ExtraTexture2 = null;

            __30.MyQuad.Base = new CoreEngine.BasePoint(1350.048f, 0f, 0f, 2798.974f, 17024.4f, -1760.757f);

            __30.uv_speed = new Vector2(0f, 0f);
            __30.uv_offset = new Vector2(0f, 0f);
            __30.Data = new PhsxData(17024.4f, -1760.757f, 0f, 0f, 0f, 0f);
            __30.StartData = new PhsxData(17024.4f, -1760.757f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__30);

            CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
            __31.Name = "cloud_castle_layer2_2";
            __31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __31.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_2");
            __31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.BlendAddRatio = 0f;
            __31.MyQuad.Quad.ExtraTexture1 = null;
            __31.MyQuad.Quad.ExtraTexture2 = null;

            __31.MyQuad.Base = new CoreEngine.BasePoint(1415.46f, 0f, 0f, 2348.253f, 21905.47f, -1027.331f);

            __31.uv_speed = new Vector2(0f, 0f);
            __31.uv_offset = new Vector2(0f, 0f);
            __31.Data = new PhsxData(21905.47f, -1027.331f, 0f, 0f, 0f, 0f);
            __31.StartData = new PhsxData(21905.47f, -1027.331f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__31);

            CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
            __32.Name = "cloud_castle_layer2_3";
            __32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __32.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_3");
            __32.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.BlendAddRatio = 0f;
            __32.MyQuad.Quad.ExtraTexture1 = null;
            __32.MyQuad.Quad.ExtraTexture2 = null;

            __32.MyQuad.Base = new CoreEngine.BasePoint(1590.616f, 0f, 0f, 2662.089f, 29651.78f, -1127.46f);

            __32.uv_speed = new Vector2(0f, 0f);
            __32.uv_offset = new Vector2(0f, 0f);
            __32.Data = new PhsxData(29651.78f, -1127.46f, 0f, 0f, 0f, 0f);
            __32.StartData = new PhsxData(29651.78f, -1127.46f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__32);

            CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
            __33.Name = "cloud_castle_layer2_1";
            __33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __33.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_1");
            __33.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.BlendAddRatio = 0f;
            __33.MyQuad.Quad.ExtraTexture1 = null;
            __33.MyQuad.Quad.ExtraTexture2 = null;

            __33.MyQuad.Base = new CoreEngine.BasePoint(882.7829f, 0f, 0f, 1830.22f, 33324.57f, -2302.971f);

            __33.uv_speed = new Vector2(0f, 0f);
            __33.uv_offset = new Vector2(0f, 0f);
            __33.Data = new PhsxData(33324.57f, -2302.971f, 0f, 0f, 0f, 0f);
            __33.StartData = new PhsxData(33324.57f, -2302.971f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__33);

            CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
            __34.Name = "cloud_castle_layer2_2";
            __34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __34.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer2_2");
            __34.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.BlendAddRatio = 0f;
            __34.MyQuad.Quad.ExtraTexture1 = null;
            __34.MyQuad.Quad.ExtraTexture2 = null;

            __34.MyQuad.Base = new CoreEngine.BasePoint(1415.46f, 0f, 0f, 2348.253f, 37953.88f, -2136.087f);

            __34.uv_speed = new Vector2(0f, 0f);
            __34.uv_offset = new Vector2(0f, 0f);
            __34.Data = new PhsxData(37953.88f, -2136.087f, 0f, 0f, 0f, 0f);
            __34.StartData = new PhsxData(37953.88f, -2136.087f, 0f, 0f, 0f, 0f);
            __26.Floaters.Add(__34);

            __26.Parallax = 0.4f;
            __26.DoPreDraw = false;
            b.MyCollection.Lists.Add(__26);

            CloudberryKingdom.BackgroundFloaterList __35 = new CloudberryKingdom.BackgroundFloaterList();
            __35.Name = "cloud_1";
            __35.Foreground = false;
            __35.Fixed = false;
            CloudberryKingdom.BackgroundFloater __36 = new CloudberryKingdom.BackgroundFloater();
            __36.Name = "cloud_cloud_layer1_1";
            __36.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __36.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_1");
            __36.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.BlendAddRatio = 0f;
            __36.MyQuad.Quad.ExtraTexture1 = null;
            __36.MyQuad.Quad.ExtraTexture2 = null;

            __36.MyQuad.Base = new CoreEngine.BasePoint(1286.25f, 0f, 0f, 706.0881f, -106.0938f, -291.884f);

            __36.uv_speed = new Vector2(0f, 0f);
            __36.uv_offset = new Vector2(0f, 0f);
            __36.Data = new PhsxData(-106.0938f, -291.884f, 0f, 0f, 0f, 0f);
            __36.StartData = new PhsxData(-106.0938f, -291.884f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__36);

            CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
            __37.Name = "cloud_cloud_layer1_2";
            __37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __37.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_2");
            __37.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.BlendAddRatio = 0f;
            __37.MyQuad.Quad.ExtraTexture1 = null;
            __37.MyQuad.Quad.ExtraTexture2 = null;

            __37.MyQuad.Base = new CoreEngine.BasePoint(1153.097f, 0f, 0f, 600.9785f, 4506.916f, -181.2072f);

            __37.uv_speed = new Vector2(0f, 0f);
            __37.uv_offset = new Vector2(0f, 0f);
            __37.Data = new PhsxData(4506.916f, -181.2072f, 0f, 0f, 0f, 0f);
            __37.StartData = new PhsxData(4506.916f, -181.2072f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__37);

            CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
            __38.Name = "cloud_cloud_layer1_3";
            __38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __38.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_3");
            __38.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.BlendAddRatio = 0f;
            __38.MyQuad.Quad.ExtraTexture1 = null;
            __38.MyQuad.Quad.ExtraTexture2 = null;

            __38.MyQuad.Base = new CoreEngine.BasePoint(1163.339f, 0f, 0f, 451.3339f, 7103.76f, -501.3638f);

            __38.uv_speed = new Vector2(0f, 0f);
            __38.uv_offset = new Vector2(0f, 0f);
            __38.Data = new PhsxData(7103.76f, -501.3638f, 0f, 0f, 0f, 0f);
            __38.StartData = new PhsxData(7103.76f, -501.3638f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__38);

            CloudberryKingdom.BackgroundFloater __39 = new CloudberryKingdom.BackgroundFloater();
            __39.Name = "cloud_cloud_layer1_4";
            __39.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __39.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_4");
            __39.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __39.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __39.MyQuad.Quad.BlendAddRatio = 0f;
            __39.MyQuad.Quad.ExtraTexture1 = null;
            __39.MyQuad.Quad.ExtraTexture2 = null;

            __39.MyQuad.Base = new CoreEngine.BasePoint(978.9733f, 0f, 0f, 644.0235f, 10113.27f, -203.6833f);

            __39.uv_speed = new Vector2(0f, 0f);
            __39.uv_offset = new Vector2(0f, 0f);
            __39.Data = new PhsxData(10113.27f, -203.6833f, 0f, 0f, 0f, 0f);
            __39.StartData = new PhsxData(10113.27f, -203.6833f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__39);

            CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
            __40.Name = "cloud_cloud_layer1_1";
            __40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __40.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_1");
            __40.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.BlendAddRatio = 0f;
            __40.MyQuad.Quad.ExtraTexture1 = null;
            __40.MyQuad.Quad.ExtraTexture2 = null;

            __40.MyQuad.Base = new CoreEngine.BasePoint(1286.25f, 0f, 0f, 706.0881f, 14141.27f, -125.3017f);

            __40.uv_speed = new Vector2(0f, 0f);
            __40.uv_offset = new Vector2(0f, 0f);
            __40.Data = new PhsxData(14141.27f, -125.3017f, 0f, 0f, 0f, 0f);
            __40.StartData = new PhsxData(14141.27f, -125.3017f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__40);

            CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
            __41.Name = "cloud_cloud_layer1_2";
            __41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __41.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_2");
            __41.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.BlendAddRatio = 0f;
            __41.MyQuad.Quad.ExtraTexture1 = null;
            __41.MyQuad.Quad.ExtraTexture2 = null;

            __41.MyQuad.Base = new CoreEngine.BasePoint(1153.097f, 0f, 0f, 600.9785f, 17429.04f, -327.6263f);

            __41.uv_speed = new Vector2(0f, 0f);
            __41.uv_offset = new Vector2(0f, 0f);
            __41.Data = new PhsxData(17429.04f, -327.6263f, 0f, 0f, 0f, 0f);
            __41.StartData = new PhsxData(17429.04f, -327.6263f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__41);

            CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
            __42.Name = "cloud_cloud_layer1_3";
            __42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __42.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_3");
            __42.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.BlendAddRatio = 0f;
            __42.MyQuad.Quad.ExtraTexture1 = null;
            __42.MyQuad.Quad.ExtraTexture2 = null;

            __42.MyQuad.Base = new CoreEngine.BasePoint(1163.339f, 0f, 0f, 451.3339f, 20784.25f, -445.6487f);

            __42.uv_speed = new Vector2(0f, 0f);
            __42.uv_offset = new Vector2(0f, 0f);
            __42.Data = new PhsxData(20784.25f, -445.6487f, 0f, 0f, 0f, 0f);
            __42.StartData = new PhsxData(20784.25f, -445.6487f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__42);

            CloudberryKingdom.BackgroundFloater __43 = new CloudberryKingdom.BackgroundFloater();
            __43.Name = "cloud_cloud_layer1_4";
            __43.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __43.MyQuad.Quad._MyTexture = Tools.Texture("cloud_cloud_layer1_4");
            __43.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.BlendAddRatio = 0f;
            __43.MyQuad.Quad.ExtraTexture1 = null;
            __43.MyQuad.Quad.ExtraTexture2 = null;

            __43.MyQuad.Base = new CoreEngine.BasePoint(978.9733f, 0f, 0f, 644.0235f, 24527.25f, 110.7433f);

            __43.uv_speed = new Vector2(0f, 0f);
            __43.uv_offset = new Vector2(0f, 0f);
            __43.Data = new PhsxData(24527.25f, 110.7433f, 0f, 0f, 0f, 0f);
            __43.StartData = new PhsxData(24527.25f, 110.7433f, 0f, 0f, 0f, 0f);
            __35.Floaters.Add(__43);

            __35.Parallax = 0.6f;
            __35.DoPreDraw = false;
            b.MyCollection.Lists.Add(__35);

            CloudberryKingdom.BackgroundFloaterList __44 = new CloudberryKingdom.BackgroundFloaterList();
            __44.Name = "castle_1";
            __44.Foreground = false;
            __44.Fixed = false;
            CloudberryKingdom.BackgroundFloater __45 = new CloudberryKingdom.BackgroundFloater();
            __45.Name = "cloud_castle_layer1";
            __45.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __45.MyQuad.Quad._MyTexture = Tools.Texture("cloud_castle_layer1");
            __45.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.BlendAddRatio = 0f;
            __45.MyQuad.Quad.ExtraTexture1 = null;
            __45.MyQuad.Quad.ExtraTexture2 = null;

            __45.MyQuad.Base = new CoreEngine.BasePoint(1231.156f, 0f, 0f, 2231.931f, 761.7891f, 296.3109f);

            __45.uv_speed = new Vector2(0f, 0f);
            __45.uv_offset = new Vector2(0f, 0f);
            __45.Data = new PhsxData(761.7891f, 296.3109f, 0f, 0f, 0f, 0f);
            __45.StartData = new PhsxData(761.7891f, 296.3109f, 0f, 0f, 0f, 0f);
            __44.Floaters.Add(__45);

            __44.Parallax = 0.666f;
            __44.DoPreDraw = false;
            b.MyCollection.Lists.Add(__44);

            b.Light = 1f;
            //b.BL = new Vector2(-4848.254f, -4500f);
            //b.TR = new Vector2(42470f, 3600f);
            b.BL = new Vector2(-100000, -10000);
            b.TR = new Vector2(100000, 10000);
        }
    }
}

