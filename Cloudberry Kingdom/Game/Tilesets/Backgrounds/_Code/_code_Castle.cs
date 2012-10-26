using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void _code_Castle(Background b)
        {
            b.GuidCounter = 0;
            b.MyGlobalIllumination = 1f;
            b.AllowLava = true;
            CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
            __1.Name = "Wall";
            __1.Foreground = false;
            __1.Fixed = false;
            CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
            __2.Name = "castle_wall";
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad._MyTexture = Tools.Texture("castle_wall");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;

            __2.MyQuad.Base = new CoreEngine.BasePoint(8090.417f, 0f, 0f, 3422.704f, 0f, 0f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(0f, 0f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(0f, 0f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
            __3.Name = "castle_wall_p2";
            __3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __3.MyQuad.Quad._MyTexture = Tools.Texture("castle_wall_p2");
            __3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.BlendAddRatio = 0f;
            __3.MyQuad.Quad.ExtraTexture1 = null;
            __3.MyQuad.Quad.ExtraTexture2 = null;

            __3.MyQuad.Base = new CoreEngine.BasePoint(8090.417f, 0f, 0f, 3422.704f, 16180.76f, 0.06445313f);

            __3.uv_speed = new Vector2(0f, 0f);
            __3.uv_offset = new Vector2(0f, 0f);
            __3.Data = new PhsxData(16180.76f, 0.06445313f, 0f, 0f, 0f, 0f);
            __3.StartData = new PhsxData(16180.76f, 0.06445313f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__3);

            CloudberryKingdom.BackgroundFloater __4 = new CloudberryKingdom.BackgroundFloater();
            __4.Name = "castle_wall";
            __4.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __4.MyQuad.Quad._MyTexture = Tools.Texture("castle_wall");
            __4.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.BlendAddRatio = 0f;
            __4.MyQuad.Quad.ExtraTexture1 = null;
            __4.MyQuad.Quad.ExtraTexture2 = null;

            __4.MyQuad.Base = new CoreEngine.BasePoint(8090.417f, 0f, 0f, 3422.704f, 32361.51f, -0.03735352f);

            __4.uv_speed = new Vector2(0f, 0f);
            __4.uv_offset = new Vector2(0f, 0f);
            __4.Data = new PhsxData(32361.51f, -0.03735352f, 0f, 0f, 0f, 0f);
            __4.StartData = new PhsxData(32361.51f, -0.03735352f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__4);

            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "castle_wall_p2";
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad._MyTexture = Tools.Texture("castle_wall_p2");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;

            __5.MyQuad.Base = new CoreEngine.BasePoint(8090.417f, 0f, 0f, 3422.704f, 48542.02f, 0.08365631f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(48542.02f, 0.08365631f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(48542.02f, 0.08365631f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__5);

            CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
            __6.Name = "Castle_Window_Left_Frame";
            __6.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __6.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Left_Frame");
            __6.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.BlendAddRatio = 0f;
            __6.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __6.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Left_Mask");

            __6.MyQuad.Base = new CoreEngine.BasePoint(821.9902f, 0f, 0f, 1827.779f, 1229.637f, -119.3635f);

            __6.uv_speed = new Vector2(0f, 0f);
            __6.uv_offset = new Vector2(0f, 0f);
            __6.Data = new PhsxData(1229.637f, -119.3635f, 0f, 0f, 0f, 0f);
            __6.StartData = new PhsxData(1229.637f, -119.3635f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__6);

            CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
            __7.Name = "Castle_Window_Center_Frame";
            __7.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __7.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Center_Frame");
            __7.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.BlendAddRatio = 0f;
            __7.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __7.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Center_Mask");

            __7.MyQuad.Base = new CoreEngine.BasePoint(1069.899f, 0f, 0f, 2655.173f, 3022.709f, -70.24944f);

            __7.uv_speed = new Vector2(0f, 0f);
            __7.uv_offset = new Vector2(0f, 0f);
            __7.Data = new PhsxData(3022.709f, -70.24944f, 0f, 0f, 0f, 0f);
            __7.StartData = new PhsxData(3022.709f, -70.24944f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__7);

            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "Castle_Window_Right_Frame";
            __8.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __8.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Right_Frame");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;
            __8.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __8.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Right_Mask");

            __8.MyQuad.Base = new CoreEngine.BasePoint(868.2858f, 0f, 0f, 1843.033f, 4746.968f, -156.1684f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(4746.968f, -156.1684f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(4746.968f, -156.1684f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__8);

            CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
            __9.Name = "Castle_Window_Left_Frame";
            __9.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __9.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Left_Frame");
            __9.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.BlendAddRatio = 0f;
            __9.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __9.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Left_Mask");

            __9.MyQuad.Base = new CoreEngine.BasePoint(821.9902f, 0f, 0f, 1827.779f, 10648.15f, -214.8148f);

            __9.uv_speed = new Vector2(0f, 0f);
            __9.uv_offset = new Vector2(0f, 0f);
            __9.Data = new PhsxData(10648.15f, -214.8148f, 0f, 0f, 0f, 0f);
            __9.StartData = new PhsxData(10648.15f, -214.8148f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__9);

            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "Castle_Window_Center_Frame";
            __10.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __10.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Center_Frame");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;
            __10.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __10.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Center_Mask");

            __10.MyQuad.Base = new CoreEngine.BasePoint(1069.899f, 0f, 0f, 2655.173f, 12472.22f, -112.9629f);

            __10.uv_speed = new Vector2(0f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(12472.22f, -112.9629f, 0f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(12472.22f, -112.9629f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__10);

            CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
            __11.Name = "Castle_Window_Right_Frame";
            __11.MyQuad.Quad.MyEffect = Tools.WindowEffect;
            __11.MyQuad.Quad._MyTexture = Tools.Texture("Castle_Window_Right_Frame");
            __11.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.BlendAddRatio = 0f;
            __11.MyQuad.Quad.ExtraTexture1 = Tools.Texture("Castle_Backdrop_2");
            __11.MyQuad.Quad.ExtraTexture2 = Tools.Texture("Castle_Window_Right_Mask");

            __11.MyQuad.Base = new CoreEngine.BasePoint(868.2858f, 0f, 0f, 1843.033f, 14259.26f, -279.6296f);

            __11.uv_speed = new Vector2(0f, 0f);
            __11.uv_offset = new Vector2(0f, 0f);
            __11.Data = new PhsxData(14259.26f, -279.6296f, 0f, 0f, 0f, 0f);
            __11.StartData = new PhsxData(14259.26f, -279.6296f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__11);

            __1.Parallax = 0.3f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __12 = new CloudberryKingdom.BackgroundFloaterList();
            __12.Name = "Pillars";
            __12.Foreground = false;
            __12.Fixed = false;
            CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
            __13.Name = "castle_pillar";
            __13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __13.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.BlendAddRatio = 0f;
            __13.MyQuad.Quad.ExtraTexture1 = null;
            __13.MyQuad.Quad.ExtraTexture2 = null;

            __13.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, -432.9412f, -40.13337f);

            __13.uv_speed = new Vector2(0f, 0f);
            __13.uv_offset = new Vector2(0f, 0f);
            __13.Data = new PhsxData(-432.9412f, -40.13337f, 0f, 0f, 0f, 0f);
            __13.StartData = new PhsxData(-432.9412f, -40.13337f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__13);

            CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
            __14.Name = "castle_pillar";
            __14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __14.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.BlendAddRatio = 0f;
            __14.MyQuad.Quad.ExtraTexture1 = null;
            __14.MyQuad.Quad.ExtraTexture2 = null;

            __14.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 6537.952f, -69.89888f);

            __14.uv_speed = new Vector2(0f, 0f);
            __14.uv_offset = new Vector2(0f, 0f);
            __14.Data = new PhsxData(6537.952f, -69.89888f, 0f, 0f, 0f, 0f);
            __14.StartData = new PhsxData(6537.952f, -69.89888f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__14);

            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "castle_pillar";
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;

            __15.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 8990.559f, -131.1371f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(8990.559f, -131.1371f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(8990.559f, -131.1371f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__15);

            CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
            __16.Name = "castle_pillar";
            __16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __16.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.BlendAddRatio = 0f;
            __16.MyQuad.Quad.ExtraTexture1 = null;
            __16.MyQuad.Quad.ExtraTexture2 = null;

            __16.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, -2508.168f, -225.2005f);

            __16.uv_speed = new Vector2(0f, 0f);
            __16.uv_offset = new Vector2(0f, 0f);
            __16.Data = new PhsxData(-2508.168f, -225.2005f, 0f, 0f, 0f, 0f);
            __16.StartData = new PhsxData(-2508.168f, -225.2005f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__16);

            CloudberryKingdom.BackgroundFloater __17 = new CloudberryKingdom.BackgroundFloater();
            __17.Name = "castle_pillar";
            __17.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __17.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __17.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __17.MyQuad.Quad.BlendAddRatio = 0f;
            __17.MyQuad.Quad.ExtraTexture1 = null;
            __17.MyQuad.Quad.ExtraTexture2 = null;

            __17.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, -7470.558f, -152.276f);

            __17.uv_speed = new Vector2(0f, 0f);
            __17.uv_offset = new Vector2(0f, 0f);
            __17.Data = new PhsxData(-7470.558f, -152.276f, 0f, 0f, 0f, 0f);
            __17.StartData = new PhsxData(-7470.558f, -152.276f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__17);

            CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
            __18.Name = "castle_pillar";
            __18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __18.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.BlendAddRatio = 0f;
            __18.MyQuad.Quad.ExtraTexture1 = null;
            __18.MyQuad.Quad.ExtraTexture2 = null;

            __18.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 15948.25f, 6.882294f);

            __18.uv_speed = new Vector2(0f, 0f);
            __18.uv_offset = new Vector2(0f, 0f);
            __18.Data = new PhsxData(15948.25f, 6.882294f, 0f, 0f, 0f, 0f);
            __18.StartData = new PhsxData(15948.25f, 6.882294f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__18);

            CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
            __19.Name = "castle_pillar";
            __19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __19.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.BlendAddRatio = 0f;
            __19.MyQuad.Quad.ExtraTexture1 = null;
            __19.MyQuad.Quad.ExtraTexture2 = null;

            __19.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 18275.31f, -203.903f);

            __19.uv_speed = new Vector2(0f, 0f);
            __19.uv_offset = new Vector2(0f, 0f);
            __19.Data = new PhsxData(18275.31f, -203.903f, 0f, 0f, 0f, 0f);
            __19.StartData = new PhsxData(18275.31f, -203.903f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__19);

            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "castle_pillar";
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;

            __20.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 25510.57f, -232.6846f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(25510.57f, -232.6846f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(25510.57f, -232.6846f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__20);

            CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
            __21.Name = "castle_pillar";
            __21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __21.MyQuad.Quad._MyTexture = Tools.Texture("castle_pillar");
            __21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.BlendAddRatio = 0f;
            __21.MyQuad.Quad.ExtraTexture1 = null;
            __21.MyQuad.Quad.ExtraTexture2 = null;

            __21.MyQuad.Base = new CoreEngine.BasePoint(851.4517f, 0f, 0f, 3055.162f, 28404.66f, -225.4594f);

            __21.uv_speed = new Vector2(0f, 0f);
            __21.uv_offset = new Vector2(0f, 0f);
            __21.Data = new PhsxData(28404.66f, -225.4594f, 0f, 0f, 0f, 0f);
            __21.StartData = new PhsxData(28404.66f, -225.4594f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__21);

            __12.Parallax = 0.36f;
            __12.DoPreDraw = false;
            b.MyCollection.Lists.Add(__12);

            CloudberryKingdom.BackgroundFloaterList __22 = new CloudberryKingdom.BackgroundFloaterList();
            __22.Name = "Chandeliers_Far";
            __22.Foreground = false;
            __22.Fixed = false;
            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "castle_chandelier_far";
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;

            __23.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, -12552.62f, 2073.532f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(-12552.62f, 2073.532f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(-12552.62f, 2073.532f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__23);

            CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
            __24.Name = "castle_chandelier_far";
            __24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __24.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.BlendAddRatio = 0f;
            __24.MyQuad.Quad.ExtraTexture1 = null;
            __24.MyQuad.Quad.ExtraTexture2 = null;

            __24.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, -9512.358f, 2305.82f);

            __24.uv_speed = new Vector2(0f, 0f);
            __24.uv_offset = new Vector2(0f, 0f);
            __24.Data = new PhsxData(-9512.358f, 2305.82f, 0f, 0f, 0f, 0f);
            __24.StartData = new PhsxData(-9512.358f, 2305.82f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__24);

            CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
            __25.Name = "castle_chandelier_far";
            __25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __25.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.BlendAddRatio = 0f;
            __25.MyQuad.Quad.ExtraTexture1 = null;
            __25.MyQuad.Quad.ExtraTexture2 = null;

            __25.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, -6887.27f, 2162.044f);

            __25.uv_speed = new Vector2(0f, 0f);
            __25.uv_offset = new Vector2(0f, 0f);
            __25.Data = new PhsxData(-6887.27f, 2162.044f, 0f, 0f, 0f, 0f);
            __25.StartData = new PhsxData(-6887.27f, 2162.044f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__25);

            CloudberryKingdom.BackgroundFloater __26 = new CloudberryKingdom.BackgroundFloater();
            __26.Name = "castle_chandelier_far";
            __26.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __26.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __26.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.BlendAddRatio = 0f;
            __26.MyQuad.Quad.ExtraTexture1 = null;
            __26.MyQuad.Quad.ExtraTexture2 = null;

            __26.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, -3205.039f, 2042.446f);

            __26.uv_speed = new Vector2(0f, 0f);
            __26.uv_offset = new Vector2(0f, 0f);
            __26.Data = new PhsxData(-3205.039f, 2042.446f, 0f, 0f, 0f, 0f);
            __26.StartData = new PhsxData(-3205.039f, 2042.446f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__26);

            CloudberryKingdom.BackgroundFloater __27 = new CloudberryKingdom.BackgroundFloater();
            __27.Name = "castle_chandelier_far";
            __27.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __27.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __27.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __27.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __27.MyQuad.Quad.BlendAddRatio = 0f;
            __27.MyQuad.Quad.ExtraTexture1 = null;
            __27.MyQuad.Quad.ExtraTexture2 = null;

            __27.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, -849.4259f, 1762.513f);

            __27.uv_speed = new Vector2(0f, 0f);
            __27.uv_offset = new Vector2(0f, 0f);
            __27.Data = new PhsxData(-849.4259f, 1762.513f, 0f, 0f, 0f, 0f);
            __27.StartData = new PhsxData(-849.4259f, 1762.513f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__27);

            CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
            __28.Name = "castle_chandelier_far";
            __28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __28.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.BlendAddRatio = 0f;
            __28.MyQuad.Quad.ExtraTexture1 = null;
            __28.MyQuad.Quad.ExtraTexture2 = null;

            __28.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 1569.904f, 1935.758f);

            __28.uv_speed = new Vector2(0f, 0f);
            __28.uv_offset = new Vector2(0f, 0f);
            __28.Data = new PhsxData(1569.904f, 1935.758f, 0f, 0f, 0f, 0f);
            __28.StartData = new PhsxData(1569.904f, 1935.758f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__28);

            CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
            __29.Name = "castle_chandelier_far";
            __29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __29.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.BlendAddRatio = 0f;
            __29.MyQuad.Quad.ExtraTexture1 = null;
            __29.MyQuad.Quad.ExtraTexture2 = null;

            __29.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 3734.492f, 1787.61f);

            __29.uv_speed = new Vector2(0f, 0f);
            __29.uv_offset = new Vector2(0f, 0f);
            __29.Data = new PhsxData(3734.492f, 1787.61f, 0f, 0f, 0f, 0f);
            __29.StartData = new PhsxData(3734.492f, 1787.61f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__29);

            CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
            __30.Name = "castle_chandelier_far";
            __30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __30.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.BlendAddRatio = 0f;
            __30.MyQuad.Quad.ExtraTexture1 = null;
            __30.MyQuad.Quad.ExtraTexture2 = null;

            __30.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 6327.063f, 1954.761f);

            __30.uv_speed = new Vector2(0f, 0f);
            __30.uv_offset = new Vector2(0f, 0f);
            __30.Data = new PhsxData(6327.063f, 1954.761f, 0f, 0f, 0f, 0f);
            __30.StartData = new PhsxData(6327.063f, 1954.761f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__30);

            CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
            __31.Name = "castle_chandelier_far";
            __31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __31.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.BlendAddRatio = 0f;
            __31.MyQuad.Quad.ExtraTexture1 = null;
            __31.MyQuad.Quad.ExtraTexture2 = null;

            __31.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 8253.275f, 1765.66f);

            __31.uv_speed = new Vector2(0f, 0f);
            __31.uv_offset = new Vector2(0f, 0f);
            __31.Data = new PhsxData(8253.275f, 1765.66f, 0f, 0f, 0f, 0f);
            __31.StartData = new PhsxData(8253.275f, 1765.66f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__31);

            CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
            __32.Name = "castle_chandelier_far";
            __32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __32.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __32.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.BlendAddRatio = 0f;
            __32.MyQuad.Quad.ExtraTexture1 = null;
            __32.MyQuad.Quad.ExtraTexture2 = null;

            __32.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 10659.29f, 2202.28f);

            __32.uv_speed = new Vector2(0f, 0f);
            __32.uv_offset = new Vector2(0f, 0f);
            __32.Data = new PhsxData(10659.29f, 2202.28f, 0f, 0f, 0f, 0f);
            __32.StartData = new PhsxData(10659.29f, 2202.28f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__32);

            CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
            __33.Name = "castle_chandelier_far";
            __33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __33.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __33.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.BlendAddRatio = 0f;
            __33.MyQuad.Quad.ExtraTexture1 = null;
            __33.MyQuad.Quad.ExtraTexture2 = null;

            __33.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 12939.41f, 1861.483f);

            __33.uv_speed = new Vector2(0f, 0f);
            __33.uv_offset = new Vector2(0f, 0f);
            __33.Data = new PhsxData(12939.41f, 1861.483f, 0f, 0f, 0f, 0f);
            __33.StartData = new PhsxData(12939.41f, 1861.483f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__33);

            CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
            __34.Name = "castle_chandelier_far";
            __34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __34.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __34.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.BlendAddRatio = 0f;
            __34.MyQuad.Quad.ExtraTexture1 = null;
            __34.MyQuad.Quad.ExtraTexture2 = null;

            __34.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 14887.88f, 2277.576f);

            __34.uv_speed = new Vector2(0f, 0f);
            __34.uv_offset = new Vector2(0f, 0f);
            __34.Data = new PhsxData(14887.88f, 2277.576f, 0f, 0f, 0f, 0f);
            __34.StartData = new PhsxData(14887.88f, 2277.576f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__34);

            CloudberryKingdom.BackgroundFloater __35 = new CloudberryKingdom.BackgroundFloater();
            __35.Name = "castle_chandelier_far";
            __35.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __35.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __35.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __35.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __35.MyQuad.Quad.BlendAddRatio = 0f;
            __35.MyQuad.Quad.ExtraTexture1 = null;
            __35.MyQuad.Quad.ExtraTexture2 = null;

            __35.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 16950.35f, 1941.349f);

            __35.uv_speed = new Vector2(0f, 0f);
            __35.uv_offset = new Vector2(0f, 0f);
            __35.Data = new PhsxData(16950.35f, 1941.349f, 0f, 0f, 0f, 0f);
            __35.StartData = new PhsxData(16950.35f, 1941.349f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__35);

            CloudberryKingdom.BackgroundFloater __36 = new CloudberryKingdom.BackgroundFloater();
            __36.Name = "castle_chandelier_far";
            __36.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __36.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __36.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.BlendAddRatio = 0f;
            __36.MyQuad.Quad.ExtraTexture1 = null;
            __36.MyQuad.Quad.ExtraTexture2 = null;

            __36.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 19168.29f, 2669.889f);

            __36.uv_speed = new Vector2(0f, 0f);
            __36.uv_offset = new Vector2(0f, 0f);
            __36.Data = new PhsxData(19168.29f, 2669.889f, 0f, 0f, 0f, 0f);
            __36.StartData = new PhsxData(19168.29f, 2669.889f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__36);

            CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
            __37.Name = "castle_chandelier_far";
            __37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __37.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __37.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.BlendAddRatio = 0f;
            __37.MyQuad.Quad.ExtraTexture1 = null;
            __37.MyQuad.Quad.ExtraTexture2 = null;

            __37.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 21178.94f, 1858.435f);

            __37.uv_speed = new Vector2(0f, 0f);
            __37.uv_offset = new Vector2(0f, 0f);
            __37.Data = new PhsxData(21178.94f, 1858.435f, 0f, 0f, 0f, 0f);
            __37.StartData = new PhsxData(21178.94f, 1858.435f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__37);

            CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
            __38.Name = "castle_chandelier_far";
            __38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __38.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_far");
            __38.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.BlendAddRatio = 0f;
            __38.MyQuad.Quad.ExtraTexture1 = null;
            __38.MyQuad.Quad.ExtraTexture2 = null;

            __38.MyQuad.Base = new CoreEngine.BasePoint(619.2842f, 0f, 0f, 2095.021f, 24296.85f, 1994.823f);

            __38.uv_speed = new Vector2(0f, 0f);
            __38.uv_offset = new Vector2(0f, 0f);
            __38.Data = new PhsxData(24296.85f, 1994.823f, 0f, 0f, 0f, 0f);
            __38.StartData = new PhsxData(24296.85f, 1994.823f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__38);

            __22.Parallax = 0.42f;
            __22.DoPreDraw = false;
            b.MyCollection.Lists.Add(__22);

            CloudberryKingdom.BackgroundFloaterList __39 = new CloudberryKingdom.BackgroundFloaterList();
            __39.Name = "Chandeliers_Close";
            __39.Foreground = false;
            __39.Fixed = false;
            CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
            __40.Name = "castle_chandelier_close";
            __40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __40.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __40.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.BlendAddRatio = 0f;
            __40.MyQuad.Quad.ExtraTexture1 = null;
            __40.MyQuad.Quad.ExtraTexture2 = null;

            __40.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 3612.716f, 1209.252f);

            __40.uv_speed = new Vector2(0f, 0f);
            __40.uv_offset = new Vector2(0f, 0f);
            __40.Data = new PhsxData(3612.716f, 1209.252f, 0f, 0f, 0f, 0f);
            __40.StartData = new PhsxData(3612.716f, 1209.252f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__40);

            CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
            __41.Name = "castle_chandelier_close";
            __41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __41.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __41.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.BlendAddRatio = 0f;
            __41.MyQuad.Quad.ExtraTexture1 = null;
            __41.MyQuad.Quad.ExtraTexture2 = null;

            __41.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, -652.3344f, 1221.809f);

            __41.uv_speed = new Vector2(0f, 0f);
            __41.uv_offset = new Vector2(0f, 0f);
            __41.Data = new PhsxData(-652.3344f, 1221.809f, 0f, 0f, 0f, 0f);
            __41.StartData = new PhsxData(-652.3344f, 1221.809f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__41);

            CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
            __42.Name = "castle_chandelier_close";
            __42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __42.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __42.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.BlendAddRatio = 0f;
            __42.MyQuad.Quad.ExtraTexture1 = null;
            __42.MyQuad.Quad.ExtraTexture2 = null;

            __42.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 7568.055f, 1272.035f);

            __42.uv_speed = new Vector2(0f, 0f);
            __42.uv_offset = new Vector2(0f, 0f);
            __42.Data = new PhsxData(7568.055f, 1272.035f, 0f, 0f, 0f, 0f);
            __42.StartData = new PhsxData(7568.055f, 1272.035f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__42);

            CloudberryKingdom.BackgroundFloater __43 = new CloudberryKingdom.BackgroundFloater();
            __43.Name = "castle_chandelier_close";
            __43.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __43.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __43.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.BlendAddRatio = 0f;
            __43.MyQuad.Quad.ExtraTexture1 = null;
            __43.MyQuad.Quad.ExtraTexture2 = null;

            __43.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 12490.26f, 1238.551f);

            __43.uv_speed = new Vector2(0f, 0f);
            __43.uv_offset = new Vector2(0f, 0f);
            __43.Data = new PhsxData(12490.26f, 1238.551f, 0f, 0f, 0f, 0f);
            __43.StartData = new PhsxData(12490.26f, 1238.551f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__43);

            CloudberryKingdom.BackgroundFloater __44 = new CloudberryKingdom.BackgroundFloater();
            __44.Name = "castle_chandelier_close";
            __44.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __44.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __44.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __44.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __44.MyQuad.Quad.BlendAddRatio = 0f;
            __44.MyQuad.Quad.ExtraTexture1 = null;
            __44.MyQuad.Quad.ExtraTexture2 = null;

            __44.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, -4636.972f, 1288.777f);

            __44.uv_speed = new Vector2(0f, 0f);
            __44.uv_offset = new Vector2(0f, 0f);
            __44.Data = new PhsxData(-4636.972f, 1288.777f, 0f, 0f, 0f, 0f);
            __44.StartData = new PhsxData(-4636.972f, 1288.777f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__44);

            CloudberryKingdom.BackgroundFloater __45 = new CloudberryKingdom.BackgroundFloater();
            __45.Name = "castle_chandelier_close";
            __45.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __45.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __45.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.BlendAddRatio = 0f;
            __45.MyQuad.Quad.ExtraTexture1 = null;
            __45.MyQuad.Quad.ExtraTexture2 = null;

            __45.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, -9182.47f, 1196.696f);

            __45.uv_speed = new Vector2(0f, 0f);
            __45.uv_offset = new Vector2(0f, 0f);
            __45.Data = new PhsxData(-9182.47f, 1196.696f, 0f, 0f, 0f, 0f);
            __45.StartData = new PhsxData(-9182.47f, 1196.696f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__45);

            CloudberryKingdom.BackgroundFloater __46 = new CloudberryKingdom.BackgroundFloater();
            __46.Name = "castle_chandelier_close";
            __46.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __46.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __46.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __46.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __46.MyQuad.Quad.BlendAddRatio = 0f;
            __46.MyQuad.Quad.ExtraTexture1 = null;
            __46.MyQuad.Quad.ExtraTexture2 = null;

            __46.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 16752.85f, 1337.078f);

            __46.uv_speed = new Vector2(0f, 0f);
            __46.uv_offset = new Vector2(0f, 0f);
            __46.Data = new PhsxData(16752.85f, 1337.078f, 0f, 0f, 0f, 0f);
            __46.StartData = new PhsxData(16752.85f, 1337.078f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__46);

            CloudberryKingdom.BackgroundFloater __47 = new CloudberryKingdom.BackgroundFloater();
            __47.Name = "castle_chandelier_close";
            __47.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __47.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __47.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __47.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __47.MyQuad.Quad.BlendAddRatio = 0f;
            __47.MyQuad.Quad.ExtraTexture1 = null;
            __47.MyQuad.Quad.ExtraTexture2 = null;

            __47.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 20212.56f, 1004.161f);

            __47.uv_speed = new Vector2(0f, 0f);
            __47.uv_offset = new Vector2(0f, 0f);
            __47.Data = new PhsxData(20212.56f, 1004.161f, 0f, 0f, 0f, 0f);
            __47.StartData = new PhsxData(20212.56f, 1004.161f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__47);

            CloudberryKingdom.BackgroundFloater __48 = new CloudberryKingdom.BackgroundFloater();
            __48.Name = "castle_chandelier_close";
            __48.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __48.MyQuad.Quad._MyTexture = Tools.Texture("castle_chandelier_close");
            __48.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __48.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __48.MyQuad.Quad.BlendAddRatio = 0f;
            __48.MyQuad.Quad.ExtraTexture1 = null;
            __48.MyQuad.Quad.ExtraTexture2 = null;

            __48.MyQuad.Base = new CoreEngine.BasePoint(928.205f, 0f, 0f, 790.1899f, 23463.48f, 1307.482f);

            __48.uv_speed = new Vector2(0f, 0f);
            __48.uv_offset = new Vector2(0f, 0f);
            __48.Data = new PhsxData(23463.48f, 1307.482f, 0f, 0f, 0f, 0f);
            __48.StartData = new PhsxData(23463.48f, 1307.482f, 0f, 0f, 0f, 0f);
            __39.Floaters.Add(__48);

            __39.Parallax = 0.52f;
            __39.DoPreDraw = false;
            b.MyCollection.Lists.Add(__39);

            b.Light = 1f;
            //b.BL = new Vector2(-20555.55f, -6177.398f);
            //b.TR = new Vector2(31313.13f, 2446.58f);
            b.BL = new Vector2(-100000, -10000);
            b.TR = new Vector2(100000, 10000);
        }
    }
}
