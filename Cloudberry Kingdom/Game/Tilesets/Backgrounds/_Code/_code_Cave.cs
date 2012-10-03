using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public partial class Background : ViewReadWrite
    {
        public static void _code_Cave(Background b)
        {
            b.GuidCounter = 0;
            b.MyGlobalIllumination = 1f;
            b.AllowLava = true;
            CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
            __1.Name = "Backdrop";
            __1.Foreground = false;
            __1.Fixed = false;
            CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
            __2.Name = "cave_backdrop_p1_0";
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;
            __2.MyQuad.Quad._MyTexture = Tools.Texture("cave_backdrop");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;

            __2.MyQuad.Base = new CoreEngine.BasePoint(10952.22f, 0f, 0f, 4632.95f, -14739.01f, 157.4063f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(-14739.01f, 157.4063f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(-14739.01f, 157.4063f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
            __3.Name = "cave_backdrop_p2_0";
            __3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __3.MyQuad.Quad.ExtraTexture1 = null;
            __3.MyQuad.Quad.ExtraTexture2 = null;
            __3.MyQuad.Quad._MyTexture = Tools.Texture("cave_backdrop_p2");
            __3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.BlendAddRatio = 0f;

            __3.MyQuad.Base = new CoreEngine.BasePoint(10952.22f, 0f, 0f, 4632.95f, 7165.438f, 157.4063f);

            __3.uv_speed = new Vector2(0f, 0f);
            __3.uv_offset = new Vector2(0f, 0f);
            __3.Data = new PhsxData(7165.438f, 157.4063f, 0f, 0f, 0f, 0f);
            __3.StartData = new PhsxData(7165.438f, 157.4063f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__3);

            CloudberryKingdom.BackgroundFloater __4 = new CloudberryKingdom.BackgroundFloater();
            __4.Name = "cave_backdrop_p1_1";
            __4.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __4.MyQuad.Quad.ExtraTexture1 = null;
            __4.MyQuad.Quad.ExtraTexture2 = null;
            __4.MyQuad.Quad._MyTexture = Tools.Texture("cave_backdrop");
            __4.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __4.MyQuad.Quad.BlendAddRatio = 0f;

            __4.MyQuad.Base = new CoreEngine.BasePoint(10952.22f, 0f, 0f, 4632.95f, 29069.88f, 157.4063f);

            __4.uv_speed = new Vector2(0f, 0f);
            __4.uv_offset = new Vector2(0f, 0f);
            __4.Data = new PhsxData(29069.88f, 157.4063f, 0f, 0f, 0f, 0f);
            __4.StartData = new PhsxData(29069.88f, 157.4063f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__4);

            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "cave_backdrop_p2_1";
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;
            __5.MyQuad.Quad._MyTexture = Tools.Texture("cave_backdrop_p2");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;

            __5.MyQuad.Base = new CoreEngine.BasePoint(10952.22f, 0f, 0f, 4632.95f, 50974.33f, 157.4063f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(50974.33f, 157.4063f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(50974.33f, 157.4063f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__5);

            __1.Parallax = 0.235f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __6 = new CloudberryKingdom.BackgroundFloaterList();
            __6.Name = "Top_2";
            __6.Foreground = false;
            __6.Fixed = false;
            CloudberryKingdom.BackgroundFloater __7 = new CloudberryKingdom.BackgroundFloater();
            __7.Name = "cave_top_2_p1_0";
            __7.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __7.MyQuad.Quad.ExtraTexture1 = null;
            __7.MyQuad.Quad.ExtraTexture2 = null;
            __7.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_2");
            __7.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __7.MyQuad.Quad.BlendAddRatio = 0f;

            __7.MyQuad.Base = new CoreEngine.BasePoint(7364.382f, 0f, 0f, 3116.508f, -9290.906f, 81.43799f);

            __7.uv_speed = new Vector2(0f, 0f);
            __7.uv_offset = new Vector2(0f, 0f);
            __7.Data = new PhsxData(-9290.906f, 81.43799f, 0f, 0f, 0f, 0f);
            __7.StartData = new PhsxData(-9290.906f, 81.43799f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__7);

            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "cave_top_2_p2_0";
            __8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __8.MyQuad.Quad.ExtraTexture1 = null;
            __8.MyQuad.Quad.ExtraTexture2 = null;
            __8.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_2_p2");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;

            __8.MyQuad.Base = new CoreEngine.BasePoint(7364.382f, 0f, 0f, 3116.508f, 5437.859f, 81.43799f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(5437.859f, 81.43799f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(5437.859f, 81.43799f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__8);

            CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
            __9.Name = "cave_top_2_p1_1";
            __9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __9.MyQuad.Quad.ExtraTexture1 = null;
            __9.MyQuad.Quad.ExtraTexture2 = null;
            __9.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_2");
            __9.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.BlendAddRatio = 0f;

            __9.MyQuad.Base = new CoreEngine.BasePoint(7364.382f, 0f, 0f, 3116.508f, 20166.62f, 81.43799f);

            __9.uv_speed = new Vector2(0f, 0f);
            __9.uv_offset = new Vector2(0f, 0f);
            __9.Data = new PhsxData(20166.62f, 81.43799f, 0f, 0f, 0f, 0f);
            __9.StartData = new PhsxData(20166.62f, 81.43799f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__9);

            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "cave_top_2_p2_1";
            __10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __10.MyQuad.Quad.ExtraTexture1 = null;
            __10.MyQuad.Quad.ExtraTexture2 = null;
            __10.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_2_p2");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;

            __10.MyQuad.Base = new CoreEngine.BasePoint(7364.382f, 0f, 0f, 3116.508f, 34895.39f, 81.43799f);

            __10.uv_speed = new Vector2(0f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(34895.39f, 81.43799f, 0f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(34895.39f, 81.43799f, 0f, 0f, 0f, 0f);
            __6.Floaters.Add(__10);

            __6.Parallax = 0.34f;
            __6.DoPreDraw = false;
            b.MyCollection.Lists.Add(__6);

            CloudberryKingdom.BackgroundFloaterList __11 = new CloudberryKingdom.BackgroundFloaterList();
            __11.Name = "Bottom_2";
            __11.Foreground = false;
            __11.Fixed = false;
            CloudberryKingdom.BackgroundFloater __12 = new CloudberryKingdom.BackgroundFloater();
            __12.Name = "cave_bottom_2_p1";
            __12.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __12.MyQuad.Quad.ExtraTexture1 = null;
            __12.MyQuad.Quad.ExtraTexture2 = null;
            __12.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_2_p1");
            __12.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __12.MyQuad.Quad.BlendAddRatio = 0f;

            __12.MyQuad.Base = new CoreEngine.BasePoint(4499.655f, 0f, 0f, 1242.685f, -2566.051f, -1868.793f);

            __12.uv_speed = new Vector2(0f, 0f);
            __12.uv_offset = new Vector2(0f, 0f);
            __12.Data = new PhsxData(-2566.051f, -1868.793f, 0f, 0f, 0f, 0f);
            __12.StartData = new PhsxData(-2566.051f, -1868.793f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__12);

            CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
            __13.Name = "cave_bottom_2_p2";
            __13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __13.MyQuad.Quad.ExtraTexture1 = null;
            __13.MyQuad.Quad.ExtraTexture2 = null;
            __13.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_2_p2");
            __13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.BlendAddRatio = 0f;

            __13.MyQuad.Base = new CoreEngine.BasePoint(4745.988f, 0f, 0f, 2350.854f, 8166.965f, -813.0869f);

            __13.uv_speed = new Vector2(0f, 0f);
            __13.uv_offset = new Vector2(0f, 0f);
            __13.Data = new PhsxData(8166.965f, -813.0869f, 0f, 0f, 0f, 0f);
            __13.StartData = new PhsxData(8166.965f, -813.0869f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__13);

            CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
            __14.Name = "cave_bottom_2_p1";
            __14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __14.MyQuad.Quad.ExtraTexture1 = null;
            __14.MyQuad.Quad.ExtraTexture2 = null;
            __14.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_2_p1");
            __14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.BlendAddRatio = 0f;

            __14.MyQuad.Base = new CoreEngine.BasePoint(4499.655f, 0f, 0f, 1242.685f, 19126.95f, -1801.052f);

            __14.uv_speed = new Vector2(0f, 0f);
            __14.uv_offset = new Vector2(0f, 0f);
            __14.Data = new PhsxData(19126.95f, -1801.052f, 0f, 0f, 0f, 0f);
            __14.StartData = new PhsxData(19126.95f, -1801.052f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__14);

            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "cave_bottom_2_p2";
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;
            __15.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_2_p2");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;

            __15.MyQuad.Base = new CoreEngine.BasePoint(4745.988f, 0f, 0f, 2350.854f, 29756.15f, -843.8784f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(29756.15f, -843.8784f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(29756.15f, -843.8784f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__15);

            CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
            __16.Name = "cave_bottom_2_p1";
            __16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __16.MyQuad.Quad.ExtraTexture1 = null;
            __16.MyQuad.Quad.ExtraTexture2 = null;
            __16.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_2_p1");
            __16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.BlendAddRatio = 0f;

            __16.MyQuad.Base = new CoreEngine.BasePoint(4499.655f, 0f, 0f, 1242.685f, 40610.07f, -1603.306f);

            __16.uv_speed = new Vector2(0f, 0f);
            __16.uv_offset = new Vector2(0f, 0f);
            __16.Data = new PhsxData(40610.07f, -1603.306f, 0f, 0f, 0f, 0f);
            __16.StartData = new PhsxData(40610.07f, -1603.306f, 0f, 0f, 0f, 0f);
            __11.Floaters.Add(__16);

            __11.Parallax = 0.35f;
            __11.DoPreDraw = false;
            b.MyCollection.Lists.Add(__11);

            CloudberryKingdom.BackgroundFloaterList __17 = new CloudberryKingdom.BackgroundFloaterList();
            __17.Name = "Bottom_1";
            __17.Foreground = false;
            __17.Fixed = false;
            CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
            __18.Name = "cave_bottom_1_p1_0";
            __18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __18.MyQuad.Quad.ExtraTexture1 = null;
            __18.MyQuad.Quad.ExtraTexture2 = null;
            __18.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_1");
            __18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.BlendAddRatio = 0f;

            __18.MyQuad.Base = new CoreEngine.BasePoint(5069.857f, 0f, 0f, 2145.972f, -3929.473f, 73.79541f);

            __18.uv_speed = new Vector2(0f, 0f);
            __18.uv_offset = new Vector2(0f, 0f);
            __18.Data = new PhsxData(-3929.473f, 73.79541f, 0f, 0f, 0f, 0f);
            __18.StartData = new PhsxData(-3929.473f, 73.79541f, 0f, 0f, 0f, 0f);
            __17.Floaters.Add(__18);

            CloudberryKingdom.BackgroundFloater __19 = new CloudberryKingdom.BackgroundFloater();
            __19.Name = "cave_bottom_1_p2_0";
            __19.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __19.MyQuad.Quad.ExtraTexture1 = null;
            __19.MyQuad.Quad.ExtraTexture2 = null;
            __19.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_1_p2");
            __19.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __19.MyQuad.Quad.BlendAddRatio = 0f;

            __19.MyQuad.Base = new CoreEngine.BasePoint(5069.857f, 0f, 0f, 2145.972f, 6210.242f, 73.79541f);

            __19.uv_speed = new Vector2(0f, 0f);
            __19.uv_offset = new Vector2(0f, 0f);
            __19.Data = new PhsxData(6210.242f, 73.79541f, 0f, 0f, 0f, 0f);
            __19.StartData = new PhsxData(6210.242f, 73.79541f, 0f, 0f, 0f, 0f);
            __17.Floaters.Add(__19);

            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "cave_bottom_1_p1_1";
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;
            __20.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_1");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;

            __20.MyQuad.Base = new CoreEngine.BasePoint(5069.857f, 0f, 0f, 2145.972f, 16349.96f, 73.79541f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(16349.96f, 73.79541f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(16349.96f, 73.79541f, 0f, 0f, 0f, 0f);
            __17.Floaters.Add(__20);

            CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
            __21.Name = "cave_bottom_1_p2_1";
            __21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __21.MyQuad.Quad.ExtraTexture1 = null;
            __21.MyQuad.Quad.ExtraTexture2 = null;
            __21.MyQuad.Quad._MyTexture = Tools.Texture("cave_bottom_1_p2");
            __21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.BlendAddRatio = 0f;

            __21.MyQuad.Base = new CoreEngine.BasePoint(5069.857f, 0f, 0f, 2145.972f, 26489.67f, 73.79541f);

            __21.uv_speed = new Vector2(0f, 0f);
            __21.uv_offset = new Vector2(0f, 0f);
            __21.Data = new PhsxData(26489.67f, 73.79541f, 0f, 0f, 0f, 0f);
            __21.StartData = new PhsxData(26489.67f, 73.79541f, 0f, 0f, 0f, 0f);
            __17.Floaters.Add(__21);

            __17.Parallax = 0.5f;
            __17.DoPreDraw = false;
            b.MyCollection.Lists.Add(__17);

            CloudberryKingdom.BackgroundFloaterList __22 = new CloudberryKingdom.BackgroundFloaterList();
            __22.Name = "Top_1";
            __22.Foreground = false;
            __22.Fixed = false;
            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "cave_top_1_p1_0";
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;
            __23.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_1");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;

            __23.MyQuad.Base = new CoreEngine.BasePoint(4329.43f, 0f, 0f, 1832.563f, -2258.242f, 30.32861f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(-2258.242f, 30.32861f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(-2258.242f, 30.32861f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__23);

            CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
            __24.Name = "cave_top_1_p2_0";
            __24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __24.MyQuad.Quad.ExtraTexture1 = null;
            __24.MyQuad.Quad.ExtraTexture2 = null;
            __24.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_1_p2");
            __24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.BlendAddRatio = 0f;

            __24.MyQuad.Base = new CoreEngine.BasePoint(4329.43f, 0f, 0f, 1832.563f, 6400.621f, 30.32861f);

            __24.uv_speed = new Vector2(0f, 0f);
            __24.uv_offset = new Vector2(0f, 0f);
            __24.Data = new PhsxData(6400.621f, 30.32861f, 0f, 0f, 0f, 0f);
            __24.StartData = new PhsxData(6400.621f, 30.32861f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__24);

            CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
            __25.Name = "cave_top_1_p1_1";
            __25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __25.MyQuad.Quad.ExtraTexture1 = null;
            __25.MyQuad.Quad.ExtraTexture2 = null;
            __25.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_1");
            __25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.BlendAddRatio = 0f;

            __25.MyQuad.Base = new CoreEngine.BasePoint(4329.43f, 0f, 0f, 1832.563f, 15059.48f, 30.32861f);

            __25.uv_speed = new Vector2(0f, 0f);
            __25.uv_offset = new Vector2(0f, 0f);
            __25.Data = new PhsxData(15059.48f, 30.32861f, 0f, 0f, 0f, 0f);
            __25.StartData = new PhsxData(15059.48f, 30.32861f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__25);

            CloudberryKingdom.BackgroundFloater __26 = new CloudberryKingdom.BackgroundFloater();
            __26.Name = "cave_top_1_p2_1";
            __26.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __26.MyQuad.Quad.ExtraTexture1 = null;
            __26.MyQuad.Quad.ExtraTexture2 = null;
            __26.MyQuad.Quad._MyTexture = Tools.Texture("cave_top_1_p2");
            __26.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.BlendAddRatio = 0f;

            __26.MyQuad.Base = new CoreEngine.BasePoint(4329.43f, 0f, 0f, 1832.563f, 23718.34f, 30.32861f);

            __26.uv_speed = new Vector2(0f, 0f);
            __26.uv_offset = new Vector2(0f, 0f);
            __26.Data = new PhsxData(23718.34f, 30.32861f, 0f, 0f, 0f, 0f);
            __26.StartData = new PhsxData(23718.34f, 30.32861f, 0f, 0f, 0f, 0f);
            __22.Floaters.Add(__26);

            __22.Parallax = 0.6f;
            __22.DoPreDraw = false;
            b.MyCollection.Lists.Add(__22);

            CloudberryKingdom.BackgroundFloaterList __27 = new CloudberryKingdom.BackgroundFloaterList();
            __27.Name = "Lights";
            __27.Foreground = false;
            __27.Fixed = false;
            CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
            __28.Name = "cave_lightshafts_p1_0";
            __28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __28.MyQuad.Quad.ExtraTexture1 = null;
            __28.MyQuad.Quad.ExtraTexture2 = null;
            __28.MyQuad.Quad._MyTexture = Tools.Texture("cave_lightshafts");
            __28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.BlendAddRatio = 0f;

            __28.MyQuad.Base = new CoreEngine.BasePoint(3582.637f, 0f, 0f, 1516.46f, 2142.867f, -1.561279f);

            __28.uv_speed = new Vector2(0f, 0f);
            __28.uv_offset = new Vector2(0f, 0f);
            __28.Data = new PhsxData(2142.867f, -1.561279f, 0f, 0f, 0f, 0f);
            __28.StartData = new PhsxData(2142.867f, -1.561279f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__28);

            CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
            __29.Name = "cave_lightshafts_p2_0";
            __29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __29.MyQuad.Quad.ExtraTexture1 = null;
            __29.MyQuad.Quad.ExtraTexture2 = null;
            __29.MyQuad.Quad._MyTexture = Tools.Texture("cave_lightshafts_p2");
            __29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.BlendAddRatio = 0f;

            __29.MyQuad.Base = new CoreEngine.BasePoint(3582.637f, 0f, 0f, 1516.46f, 9308.145f, -1.561279f);

            __29.uv_speed = new Vector2(0f, 0f);
            __29.uv_offset = new Vector2(0f, 0f);
            __29.Data = new PhsxData(9308.145f, -1.561279f, 0f, 0f, 0f, 0f);
            __29.StartData = new PhsxData(9308.145f, -1.561279f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__29);

            CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
            __30.Name = "cave_lightshafts_p1_1";
            __30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __30.MyQuad.Quad.ExtraTexture1 = null;
            __30.MyQuad.Quad.ExtraTexture2 = null;
            __30.MyQuad.Quad._MyTexture = Tools.Texture("cave_lightshafts");
            __30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.BlendAddRatio = 0f;

            __30.MyQuad.Base = new CoreEngine.BasePoint(3582.637f, 0f, 0f, 1516.46f, 16473.42f, -1.561279f);

            __30.uv_speed = new Vector2(0f, 0f);
            __30.uv_offset = new Vector2(0f, 0f);
            __30.Data = new PhsxData(16473.42f, -1.561279f, 0f, 0f, 0f, 0f);
            __30.StartData = new PhsxData(16473.42f, -1.561279f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__30);

            CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
            __31.Name = "cave_lightshafts_p2_1";
            __31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __31.MyQuad.Quad.ExtraTexture1 = null;
            __31.MyQuad.Quad.ExtraTexture2 = null;
            __31.MyQuad.Quad._MyTexture = Tools.Texture("cave_lightshafts_p2");
            __31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.BlendAddRatio = 0f;

            __31.MyQuad.Base = new CoreEngine.BasePoint(3582.637f, 0f, 0f, 1516.46f, 23638.69f, -1.561279f);

            __31.uv_speed = new Vector2(0f, 0f);
            __31.uv_offset = new Vector2(0f, 0f);
            __31.Data = new PhsxData(23638.69f, -1.561279f, 0f, 0f, 0f, 0f);
            __31.StartData = new PhsxData(23638.69f, -1.561279f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__31);

            __27.Parallax = 0.68f;
            __27.DoPreDraw = false;
            b.MyCollection.Lists.Add(__27);

            b.Light = 1f;
            b.BL = new Vector2(-4670f, -4500f);
            b.TR = new Vector2(42470f, 2071.545f);
        }
    }
}
