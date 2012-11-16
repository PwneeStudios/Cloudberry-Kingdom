using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class Background_Hills : BackgroundTemplate
    {
        public Background_Hills(string Name) : base(Name) { }

        public override void Code(Background b)
        {
            base.Code(b);

            Background._code_Hills(b);
        }
    }

    public class Background_HillsRain : BackgroundTemplate
    {
        public Background_HillsRain(string Name) : base(Name) { }

        public override void Code(Background b)
        {
            base.Code(b);

            Background._code_Hills(b);
            Background.AddRainLayer(b);
        }
    }

    public partial class Background : ViewReadWrite
    {
        public static void _code_Hills(Background b)
        {
            b.GuidCounter = 0;
            b.MyGlobalIllumination = 1f;
            b.AllowLava = true;
            CloudberryKingdom.BackgroundFloaterList __1 = new CloudberryKingdom.BackgroundFloaterList();
            __1.Name = "Backdrop";
            __1.Foreground = false;
            __1.Fixed = false;
            CloudberryKingdom.BackgroundFloater __2 = new CloudberryKingdom.BackgroundFloater();
            __2.Name = "hills_backdrop_p1_0";
            __2.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __2.MyQuad.Quad.ExtraTexture1 = null;
            __2.MyQuad.Quad.ExtraTexture2 = null;
            __2.MyQuad.Quad._MyTexture = Tools.Texture("hills_backdrop");
            __2.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __2.MyQuad.Quad.BlendAddRatio = 0f;

            __2.MyQuad.Base = new BasePoint(81277.7f, 0f, 0f, 34106.41f, -4469.054f, 784.0586f);

            __2.uv_speed = new Vector2(0f, 0f);
            __2.uv_offset = new Vector2(0f, 0f);
            __2.Data = new PhsxData(-4469.054f, 784.0586f, 0f, 0f, 0f, 0f);
            __2.StartData = new PhsxData(-4469.054f, 784.0586f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__2);

            CloudberryKingdom.BackgroundFloater __3 = new CloudberryKingdom.BackgroundFloater();
            __3.Name = "hills_backdrop_p2_0";
            __3.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __3.MyQuad.Quad.ExtraTexture1 = null;
            __3.MyQuad.Quad.ExtraTexture2 = null;
            __3.MyQuad.Quad._MyTexture = Tools.Texture("hills_backdrop_p2");
            __3.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __3.MyQuad.Quad.BlendAddRatio = 0f;

            __3.MyQuad.Base = new BasePoint(81277.7f, 0f, 0f, 34106.41f, 158086.3f, 784.0586f);

            __3.uv_speed = new Vector2(0f, 0f);
            __3.uv_offset = new Vector2(0f, 0f);
            __3.Data = new PhsxData(158086.3f, 784.0586f, 0f, 0f, 0f, 0f);
            __3.StartData = new PhsxData(158086.3f, 784.0586f, 0f, 0f, 0f, 0f);
            __1.Floaters.Add(__3);

            __1.Parallax = 0.03f;
            __1.DoPreDraw = false;
            b.MyCollection.Lists.Add(__1);

            CloudberryKingdom.BackgroundFloaterList __4 = new CloudberryKingdom.BackgroundFloaterList();
            __4.Name = "Back_Castles";
            __4.Foreground = false;
            __4.Fixed = false;
            CloudberryKingdom.BackgroundFloater __5 = new CloudberryKingdom.BackgroundFloater();
            __5.Name = "hills_backcastles_trim";
            __5.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __5.MyQuad.Quad.ExtraTexture1 = null;
            __5.MyQuad.Quad.ExtraTexture2 = null;
            __5.MyQuad.Quad._MyTexture = Tools.Texture("hills_backcastles_trim");
            __5.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __5.MyQuad.Quad.BlendAddRatio = 0f;

            __5.MyQuad.Base = new BasePoint(19649.94f, 0f, 0f, 6075.512f, 3557.881f, 4444.444f);

            __5.uv_speed = new Vector2(0f, 0f);
            __5.uv_offset = new Vector2(0f, 0f);
            __5.Data = new PhsxData(3557.881f, 4444.444f, 0f, 0f, 0f, 0f);
            __5.StartData = new PhsxData(3557.881f, 4444.444f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__5);

            CloudberryKingdom.BackgroundFloater __6 = new CloudberryKingdom.BackgroundFloater();
            __6.Name = "hills_backcastles_p2_trim";
            __6.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __6.MyQuad.Quad.ExtraTexture1 = null;
            __6.MyQuad.Quad.ExtraTexture2 = null;
            __6.MyQuad.Quad._MyTexture = Tools.Texture("hills_backcastles_p2_trim");
            __6.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __6.MyQuad.Quad.BlendAddRatio = 0f;

            __6.MyQuad.Base = new BasePoint(21693.85f, 0f, 0f, 6280.27f, 44884.28f, 4316.639f);

            __6.uv_speed = new Vector2(0f, 0f);
            __6.uv_offset = new Vector2(0f, 0f);
            __6.Data = new PhsxData(44884.28f, 4316.639f, 0f, 0f, 0f, 0f);
            __6.StartData = new PhsxData(44884.28f, 4316.639f, 0f, 0f, 0f, 0f);
            __4.Floaters.Add(__6);

            __4.Parallax = 0.1f;
            __4.DoPreDraw = false;
            b.MyCollection.Lists.Add(__4);

            CloudberryKingdom.BackgroundFloaterList __7 = new CloudberryKingdom.BackgroundFloaterList();
            __7.Name = "Hill";
            __7.Foreground = false;
            __7.Fixed = false;
            CloudberryKingdom.BackgroundFloater __8 = new CloudberryKingdom.BackgroundFloater();
            __8.Name = "hills_backhills_p1_0";
            __8.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __8.MyQuad.Quad.ExtraTexture1 = null;
            __8.MyQuad.Quad.ExtraTexture2 = null;
            __8.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills");
            __8.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __8.MyQuad.Quad.BlendAddRatio = 0f;

            __8.MyQuad.Base = new BasePoint(10593.01f, 0f, 0f, 4445.094f, 2926.976f, 102.2695f);

            __8.uv_speed = new Vector2(0f, 0f);
            __8.uv_offset = new Vector2(0f, 0f);
            __8.Data = new PhsxData(2926.976f, 102.2695f, 0f, 0f, 0f, 0f);
            __8.StartData = new PhsxData(2926.976f, 102.2695f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__8);

            CloudberryKingdom.BackgroundFloater __9 = new CloudberryKingdom.BackgroundFloater();
            __9.Name = "hills_backhills_p2_0";
            __9.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __9.MyQuad.Quad.ExtraTexture1 = null;
            __9.MyQuad.Quad.ExtraTexture2 = null;
            __9.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills_p2");
            __9.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __9.MyQuad.Quad.BlendAddRatio = 0f;

            __9.MyQuad.Base = new BasePoint(10593.01f, 0f, 0f, 4445.094f, 24113f, 102.2695f);

            __9.uv_speed = new Vector2(0f, 0f);
            __9.uv_offset = new Vector2(0f, 0f);
            __9.Data = new PhsxData(24113f, 102.2695f, 0f, 0f, 0f, 0f);
            __9.StartData = new PhsxData(24113f, 102.2695f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__9);

            CloudberryKingdom.BackgroundFloater __10 = new CloudberryKingdom.BackgroundFloater();
            __10.Name = "hills_backhills_p1_0";
            __10.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __10.MyQuad.Quad.ExtraTexture1 = null;
            __10.MyQuad.Quad.ExtraTexture2 = null;
            __10.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills");
            __10.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __10.MyQuad.Quad.BlendAddRatio = 0f;

            __10.MyQuad.Base = new BasePoint(10593.01f, 0f, 0f, 4445.094f, 39921.59f, 451.0566f);

            __10.uv_speed = new Vector2(0f, 0f);
            __10.uv_offset = new Vector2(0f, 0f);
            __10.Data = new PhsxData(39921.59f, 451.0566f, 0f, 0f, 0f, 0f);
            __10.StartData = new PhsxData(39921.59f, 451.0566f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__10);

            CloudberryKingdom.BackgroundFloater __11 = new CloudberryKingdom.BackgroundFloater();
            __11.Name = "hills_backhills_p2_0";
            __11.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __11.MyQuad.Quad.ExtraTexture1 = null;
            __11.MyQuad.Quad.ExtraTexture2 = null;
            __11.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills_p2");
            __11.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __11.MyQuad.Quad.BlendAddRatio = 0f;

            __11.MyQuad.Base = new BasePoint(10593.01f, 0f, 0f, 4445.094f, 61107.63f, 451.0566f);

            __11.uv_speed = new Vector2(0f, 0f);
            __11.uv_offset = new Vector2(0f, 0f);
            __11.Data = new PhsxData(61107.63f, 451.0566f, 0f, 0f, 0f, 0f);
            __11.StartData = new PhsxData(61107.63f, 451.0566f, 0f, 0f, 0f, 0f);
            __7.Floaters.Add(__11);

            __7.Parallax = 0.23f;
            __7.DoPreDraw = false;
            b.MyCollection.Lists.Add(__7);

            CloudberryKingdom.BackgroundFloaterList __12 = new CloudberryKingdom.BackgroundFloaterList();
            __12.Name = "Hill";
            __12.Foreground = false;
            __12.Fixed = false;
            CloudberryKingdom.BackgroundFloater __13 = new CloudberryKingdom.BackgroundFloater();
            __13.Name = "hills_backhills2_trim";
            __13.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __13.MyQuad.Quad.ExtraTexture1 = null;
            __13.MyQuad.Quad.ExtraTexture2 = null;
            __13.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills2_trim");
            __13.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __13.MyQuad.Quad.BlendAddRatio = 0f;

            __13.MyQuad.Base = new BasePoint(5887f, 0f, 0f, 1970.189f, 4186.129f, -1231.915f);

            __13.uv_speed = new Vector2(0f, 0f);
            __13.uv_offset = new Vector2(0f, 0f);
            __13.Data = new PhsxData(4186.129f, -1231.915f, 0f, 0f, 0f, 0f);
            __13.StartData = new PhsxData(4186.129f, -1231.915f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__13);

            CloudberryKingdom.BackgroundFloater __14 = new CloudberryKingdom.BackgroundFloater();
            __14.Name = "hills_backhills2_p2_trim";
            __14.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(10065.5f, -192.9017f), new Vector2(0f, 0f), new Color(255, 255, 255, 255));
            __14.MyQuad.Quad.v0.Pos = new Vector2(-1.000975f, 0.9990253f);

            __14.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(25706.37f, -192.9017f), new Vector2(1f, 0f), new Color(255, 255, 255, 255));
            __14.MyQuad.Quad.v1.Pos = new Vector2(0.999025f, 0.9990253f);

            __14.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(10065.5f, -3209.26f), new Vector2(0f, 1f), new Color(255, 255, 255, 255));
            __14.MyQuad.Quad.v2.Pos = new Vector2(-1.000975f, -1.000975f);

            __14.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(25706.37f, -3209.26f), new Vector2(1f, 1f), new Color(255, 255, 255, 255));
            __14.MyQuad.Quad.v3.Pos = new Vector2(0.999025f, -1.000975f);

            __14.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __14.MyQuad.Quad.ExtraTexture1 = null;
            __14.MyQuad.Quad.ExtraTexture2 = null;
            __14.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills2_p2_trim");
            __14.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __14.MyQuad.Quad.BlendAddRatio = 0f;

            __14.MyQuad.Base = new BasePoint(7820.432f, 0f, 0f, 1508.179f, 17893.56f, -1699.611f);

            __14.uv_speed = new Vector2(0f, 0f);
            __14.uv_offset = new Vector2(0f, 0f);
            __14.Data = new PhsxData(17893.56f, -1699.611f, 0f, 0f, 0f, 0f);
            __14.StartData = new PhsxData(17893.56f, -1699.611f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__14);

            CloudberryKingdom.BackgroundFloater __15 = new CloudberryKingdom.BackgroundFloater();
            __15.Name = "hills_backhills2_trim";
            __15.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __15.MyQuad.Quad.ExtraTexture1 = null;
            __15.MyQuad.Quad.ExtraTexture2 = null;
            __15.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills2_trim");
            __15.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __15.MyQuad.Quad.BlendAddRatio = 0f;

            __15.MyQuad.Base = new BasePoint(5887f, 0f, 0f, 1970.189f, 32150.93f, -1535.743f);

            __15.uv_speed = new Vector2(0f, 0f);
            __15.uv_offset = new Vector2(0f, 0f);
            __15.Data = new PhsxData(32150.93f, -1535.743f, 0f, 0f, 0f, 0f);
            __15.StartData = new PhsxData(32150.93f, -1535.743f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__15);

            CloudberryKingdom.BackgroundFloater __16 = new CloudberryKingdom.BackgroundFloater();
            __16.Name = "hills_backhills2_p2_trim";
            __16.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __16.MyQuad.Quad.ExtraTexture1 = null;
            __16.MyQuad.Quad.ExtraTexture2 = null;
            __16.MyQuad.Quad._MyTexture = Tools.Texture("hills_backhills2_p2_trim");
            __16.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __16.MyQuad.Quad.BlendAddRatio = 0f;

            __16.MyQuad.Base = new BasePoint(7820.432f, 0f, 0f, 1508.179f, 45858.36f, -1998.595f);

            __16.uv_speed = new Vector2(0f, 0f);
            __16.uv_offset = new Vector2(0f, 0f);
            __16.Data = new PhsxData(45858.36f, -1998.595f, 0f, 0f, 0f, 0f);
            __16.StartData = new PhsxData(45858.36f, -1998.595f, 0f, 0f, 0f, 0f);
            __12.Floaters.Add(__16);

            __12.Parallax = 0.31f;
            __12.DoPreDraw = false;
            b.MyCollection.Lists.Add(__12);

            CloudberryKingdom.BackgroundFloaterList __17 = new CloudberryKingdom.BackgroundFloaterList();
            __17.Name = "Clouds";
            __17.Foreground = false;
            __17.Fixed = false;
            CloudberryKingdom.BackgroundFloater __18 = new CloudberryKingdom.BackgroundFloater();
            __18.Name = "hills_clouds";
            __18.MyQuad.Quad.v0.Vertex = new MyOwnVertexFormat(new Vector2(-4171.699f, 2314.271f), new Vector2(0.7512081f, 0f), new Color(255, 255, 255, 255));
            __18.MyQuad.Quad.v0.Pos = new Vector2(-1f, 1f);

            __18.MyQuad.Quad.v1.Vertex = new MyOwnVertexFormat(new Vector2(40964.18f, 2314.271f), new Vector2(3.749899f, 0f), new Color(255, 255, 255, 255));
            __18.MyQuad.Quad.v1.Pos = new Vector2(1f, 1f);

            __18.MyQuad.Quad.v2.Vertex = new MyOwnVertexFormat(new Vector2(-4171.699f, -1292.736f), new Vector2(0.7512081f, 1f), new Color(255, 255, 255, 255));
            __18.MyQuad.Quad.v2.Pos = new Vector2(-1f, -1f);

            __18.MyQuad.Quad.v3.Vertex = new MyOwnVertexFormat(new Vector2(40964.18f, -1292.736f), new Vector2(3.749899f, 1f), new Color(255, 255, 255, 255));
            __18.MyQuad.Quad.v3.Pos = new Vector2(1f, -1f);

            __18.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __18.MyQuad.Quad.ExtraTexture1 = null;
            __18.MyQuad.Quad.ExtraTexture2 = null;
            __18.MyQuad.Quad._MyTexture = Tools.Texture("hills_clouds");
            __18.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __18.MyQuad.Quad.BlendAddRatio = 0f;

            __18.MyQuad.Base = new BasePoint(22567.94f, 0f, 0f, 1803.504f, 18396.24f, 510.7676f);

            __18.uv_speed = new Vector2(0.0008f, 0f);
            __18.uv_offset = new Vector2(0f, 0f);
            __18.Data = new PhsxData(18396.24f, 510.7676f, 0f, 0f, 0f, 0f);
            __18.StartData = new PhsxData(18396.24f, 510.7676f, 0f, 0f, 0f, 0f);
            __17.Floaters.Add(__18);

            __17.Parallax = 0.4f;
            __17.DoPreDraw = false;
            b.MyCollection.Lists.Add(__17);

            CloudberryKingdom.BackgroundFloaterList __19 = new CloudberryKingdom.BackgroundFloaterList();
            __19.Name = "Hills";
            __19.Foreground = false;
            __19.Fixed = false;
            CloudberryKingdom.BackgroundFloater __20 = new CloudberryKingdom.BackgroundFloater();
            __20.Name = "hills_hill1";
            __20.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __20.MyQuad.Quad.ExtraTexture1 = null;
            __20.MyQuad.Quad.ExtraTexture2 = null;
            __20.MyQuad.Quad._MyTexture = Tools.Texture("hills_hill1");
            __20.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __20.MyQuad.Quad.BlendAddRatio = 0f;

            __20.MyQuad.Base = new BasePoint(5711.815f, 0f, 0f, 1410.282f, 8635.98f, -926.418f);

            __20.uv_speed = new Vector2(0f, 0f);
            __20.uv_offset = new Vector2(0f, 0f);
            __20.Data = new PhsxData(8635.98f, -926.418f, 0f, 0f, 0f, 0f);
            __20.StartData = new PhsxData(8635.98f, -926.418f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__20);

            CloudberryKingdom.BackgroundFloater __21 = new CloudberryKingdom.BackgroundFloater();
            __21.Name = "hills_hillandtree";
            __21.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __21.MyQuad.Quad.ExtraTexture1 = null;
            __21.MyQuad.Quad.ExtraTexture2 = null;
            __21.MyQuad.Quad._MyTexture = Tools.Texture("hills_hillandtree");
            __21.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __21.MyQuad.Quad.BlendAddRatio = 0f;

            __21.MyQuad.Base = new BasePoint(5250.666f, 0f, 0f, 1960.848f, 34.27734f, -34.0918f);

            __21.uv_speed = new Vector2(0f, 0f);
            __21.uv_offset = new Vector2(0f, 0f);
            __21.Data = new PhsxData(34.27734f, -34.0918f, 0f, 0f, 0f, 0f);
            __21.StartData = new PhsxData(34.27734f, -34.0918f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__21);

            CloudberryKingdom.BackgroundFloater __22 = new CloudberryKingdom.BackgroundFloater();
            __22.Name = "hills_hillandtree";
            __22.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __22.MyQuad.Quad.ExtraTexture1 = null;
            __22.MyQuad.Quad.ExtraTexture2 = null;
            __22.MyQuad.Quad._MyTexture = Tools.Texture("hills_hillandtree");
            __22.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __22.MyQuad.Quad.BlendAddRatio = 0f;

            __22.MyQuad.Base = new BasePoint(5250.666f, 0f, 0f, 1960.848f, 17604.08f, -34.0918f);

            __22.uv_speed = new Vector2(0f, 0f);
            __22.uv_offset = new Vector2(0f, 0f);
            __22.Data = new PhsxData(17604.08f, -34.0918f, 0f, 0f, 0f, 0f);
            __22.StartData = new PhsxData(17604.08f, -34.0918f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__22);

            CloudberryKingdom.BackgroundFloater __23 = new CloudberryKingdom.BackgroundFloater();
            __23.Name = "hills_hill2";
            __23.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __23.MyQuad.Quad.ExtraTexture1 = null;
            __23.MyQuad.Quad.ExtraTexture2 = null;
            __23.MyQuad.Quad._MyTexture = Tools.Texture("hills_hill2");
            __23.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __23.MyQuad.Quad.BlendAddRatio = 0f;

            __23.MyQuad.Base = new BasePoint(4082.421f, 0f, 0f, 1469.273f, 10419.8f, -1173.33f);

            __23.uv_speed = new Vector2(0f, 0f);
            __23.uv_offset = new Vector2(0f, 0f);
            __23.Data = new PhsxData(10419.8f, -1173.33f, 0f, 0f, 0f, 0f);
            __23.StartData = new PhsxData(10419.8f, -1173.33f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__23);

            CloudberryKingdom.BackgroundFloater __24 = new CloudberryKingdom.BackgroundFloater();
            __24.Name = "hills_hill1";
            __24.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __24.MyQuad.Quad.ExtraTexture1 = null;
            __24.MyQuad.Quad.ExtraTexture2 = null;
            __24.MyQuad.Quad._MyTexture = Tools.Texture("hills_hill1");
            __24.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __24.MyQuad.Quad.BlendAddRatio = 0f;

            __24.MyQuad.Base = new BasePoint(5711.815f, 0f, 0f, 1410.282f, 27956.91f, -438.7754f);

            __24.uv_speed = new Vector2(0f, 0f);
            __24.uv_offset = new Vector2(0f, 0f);
            __24.Data = new PhsxData(27956.91f, -438.7754f, 0f, 0f, 0f, 0f);
            __24.StartData = new PhsxData(27956.91f, -438.7754f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__24);

            CloudberryKingdom.BackgroundFloater __25 = new CloudberryKingdom.BackgroundFloater();
            __25.Name = "hills_hillandtree";
            __25.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __25.MyQuad.Quad.ExtraTexture1 = null;
            __25.MyQuad.Quad.ExtraTexture2 = null;
            __25.MyQuad.Quad._MyTexture = Tools.Texture("hills_hillandtree");
            __25.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __25.MyQuad.Quad.BlendAddRatio = 0f;

            __25.MyQuad.Base = new BasePoint(5250.666f, 0f, 0f, 1960.848f, 34432.56f, -336.752f);

            __25.uv_speed = new Vector2(0f, 0f);
            __25.uv_offset = new Vector2(0f, 0f);
            __25.Data = new PhsxData(34432.56f, -336.752f, 0f, 0f, 0f, 0f);
            __25.StartData = new PhsxData(34432.56f, -336.752f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__25);

            CloudberryKingdom.BackgroundFloater __26 = new CloudberryKingdom.BackgroundFloater();
            __26.Name = "hills_hill1";
            __26.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __26.MyQuad.Quad.ExtraTexture1 = null;
            __26.MyQuad.Quad.ExtraTexture2 = null;
            __26.MyQuad.Quad._MyTexture = Tools.Texture("hills_hill1");
            __26.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __26.MyQuad.Quad.BlendAddRatio = 0f;

            __26.MyQuad.Base = new BasePoint(5711.815f, 0f, 0f, 1410.282f, 43620.78f, -565.0293f);

            __26.uv_speed = new Vector2(0f, 0f);
            __26.uv_offset = new Vector2(0f, 0f);
            __26.Data = new PhsxData(43620.78f, -565.0293f, 0f, 0f, 0f, 0f);
            __26.StartData = new PhsxData(43620.78f, -565.0293f, 0f, 0f, 0f, 0f);
            __19.Floaters.Add(__26);

            __19.Parallax = 0.55f;
            __19.DoPreDraw = false;
            b.MyCollection.Lists.Add(__19);

            CloudberryKingdom.BackgroundFloaterList __27 = new CloudberryKingdom.BackgroundFloaterList();
            __27.Name = "Plants";
            __27.Foreground = false;
            __27.Fixed = false;
            CloudberryKingdom.BackgroundFloater __28 = new CloudberryKingdom.BackgroundFloater();
            __28.Name = "hills_plants_1";
            __28.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __28.MyQuad.Quad.ExtraTexture1 = null;
            __28.MyQuad.Quad.ExtraTexture2 = null;
            __28.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_1");
            __28.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __28.MyQuad.Quad.BlendAddRatio = 0f;

            __28.MyQuad.Base = new BasePoint(646.1251f, 0f, 0f, 486.9349f, -8483.381f, -959.5684f);

            __28.uv_speed = new Vector2(0f, 0f);
            __28.uv_offset = new Vector2(0f, 0f);
            __28.Data = new PhsxData(-8483.381f, -959.5684f, 0f, 0f, 0f, 0f);
            __28.StartData = new PhsxData(-8483.381f, -959.5684f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__28);

            CloudberryKingdom.BackgroundFloater __29 = new CloudberryKingdom.BackgroundFloater();
            __29.Name = "hills_plants_2";
            __29.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __29.MyQuad.Quad.ExtraTexture1 = null;
            __29.MyQuad.Quad.ExtraTexture2 = null;
            __29.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_2");
            __29.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __29.MyQuad.Quad.BlendAddRatio = 0f;

            __29.MyQuad.Base = new BasePoint(974.2943f, 0f, 0f, 843.0762f, -6522.574f, -951.364f);

            __29.uv_speed = new Vector2(0f, 0f);
            __29.uv_offset = new Vector2(0f, 0f);
            __29.Data = new PhsxData(-6522.574f, -951.364f, 0f, 0f, 0f, 0f);
            __29.StartData = new PhsxData(-6522.574f, -951.364f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__29);

            CloudberryKingdom.BackgroundFloater __30 = new CloudberryKingdom.BackgroundFloater();
            __30.Name = "hills_plants_3";
            __30.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __30.MyQuad.Quad.ExtraTexture1 = null;
            __30.MyQuad.Quad.ExtraTexture2 = null;
            __30.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_3");
            __30.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __30.MyQuad.Quad.BlendAddRatio = 0f;

            __30.MyQuad.Base = new BasePoint(892.2518f, 0f, 0f, 804.9318f, -4241.8f, -1058.019f);

            __30.uv_speed = new Vector2(0f, 0f);
            __30.uv_offset = new Vector2(0f, 0f);
            __30.Data = new PhsxData(-4241.8f, -1058.019f, 0f, 0f, 0f, 0f);
            __30.StartData = new PhsxData(-4241.8f, -1058.019f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__30);

            CloudberryKingdom.BackgroundFloater __31 = new CloudberryKingdom.BackgroundFloater();
            __31.Name = "hills_plants_4";
            __31.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __31.MyQuad.Quad.ExtraTexture1 = null;
            __31.MyQuad.Quad.ExtraTexture2 = null;
            __31.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_4");
            __31.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __31.MyQuad.Quad.BlendAddRatio = 0f;

            __31.MyQuad.Base = new BasePoint(957.8854f, 0f, 0f, 524.0001f, -2026.664f, -1213.899f);

            __31.uv_speed = new Vector2(0f, 0f);
            __31.uv_offset = new Vector2(0f, 0f);
            __31.Data = new PhsxData(-2026.664f, -1213.899f, 0f, 0f, 0f, 0f);
            __31.StartData = new PhsxData(-2026.664f, -1213.899f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__31);

            CloudberryKingdom.BackgroundFloater __32 = new CloudberryKingdom.BackgroundFloater();
            __32.Name = "hills_plants_5";
            __32.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __32.MyQuad.Quad.ExtraTexture1 = null;
            __32.MyQuad.Quad.ExtraTexture2 = null;
            __32.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_5");
            __32.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __32.MyQuad.Quad.BlendAddRatio = 0f;

            __32.MyQuad.Base = new BasePoint(695.9648f, 0f, 0f, 565.1686f, -4.105469f, -1058.24f);

            __32.uv_speed = new Vector2(0f, 0f);
            __32.uv_offset = new Vector2(0f, 0f);
            __32.Data = new PhsxData(-4.105469f, -1058.24f, 0f, 0f, 0f, 0f);
            __32.StartData = new PhsxData(-4.105469f, -1058.24f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__32);

            CloudberryKingdom.BackgroundFloater __33 = new CloudberryKingdom.BackgroundFloater();
            __33.Name = "hills_plants_6";
            __33.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __33.MyQuad.Quad.ExtraTexture1 = null;
            __33.MyQuad.Quad.ExtraTexture2 = null;
            __33.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_6");
            __33.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __33.MyQuad.Quad.BlendAddRatio = 0f;

            __33.MyQuad.Base = new BasePoint(1266.809f, 0f, 0f, 1009.208f, 2359.654f, -917.8555f);

            __33.uv_speed = new Vector2(0f, 0f);
            __33.uv_offset = new Vector2(0f, 0f);
            __33.Data = new PhsxData(2359.654f, -917.8555f, 0f, 0f, 0f, 0f);
            __33.StartData = new PhsxData(2359.654f, -917.8555f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__33);

            CloudberryKingdom.BackgroundFloater __34 = new CloudberryKingdom.BackgroundFloater();
            __34.Name = "hills_plants_1";
            __34.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __34.MyQuad.Quad.ExtraTexture1 = null;
            __34.MyQuad.Quad.ExtraTexture2 = null;
            __34.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_1");
            __34.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __34.MyQuad.Quad.BlendAddRatio = 0f;

            __34.MyQuad.Base = new BasePoint(646.1251f, 0f, 0f, 486.9349f, 4476.271f, -1029.628f);

            __34.uv_speed = new Vector2(0f, 0f);
            __34.uv_offset = new Vector2(0f, 0f);
            __34.Data = new PhsxData(4476.271f, -1029.628f, 0f, 0f, 0f, 0f);
            __34.StartData = new PhsxData(4476.271f, -1029.628f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__34);

            CloudberryKingdom.BackgroundFloater __35 = new CloudberryKingdom.BackgroundFloater();
            __35.Name = "hills_plants_2";
            __35.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __35.MyQuad.Quad.ExtraTexture1 = null;
            __35.MyQuad.Quad.ExtraTexture2 = null;
            __35.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_2");
            __35.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __35.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __35.MyQuad.Quad.BlendAddRatio = 0f;

            __35.MyQuad.Base = new BasePoint(974.2943f, 0f, 0f, 843.0762f, 6329.44f, -981.4927f);

            __35.uv_speed = new Vector2(0f, 0f);
            __35.uv_offset = new Vector2(0f, 0f);
            __35.Data = new PhsxData(6329.44f, -981.4927f, 0f, 0f, 0f, 0f);
            __35.StartData = new PhsxData(6329.44f, -981.4927f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__35);

            CloudberryKingdom.BackgroundFloater __36 = new CloudberryKingdom.BackgroundFloater();
            __36.Name = "hills_plants_3";
            __36.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __36.MyQuad.Quad.ExtraTexture1 = null;
            __36.MyQuad.Quad.ExtraTexture2 = null;
            __36.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_3");
            __36.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __36.MyQuad.Quad.BlendAddRatio = 0f;

            __36.MyQuad.Base = new BasePoint(892.2518f, 0f, 0f, 804.9318f, 8591.752f, -933.3588f);

            __36.uv_speed = new Vector2(0f, 0f);
            __36.uv_offset = new Vector2(0f, 0f);
            __36.Data = new PhsxData(8591.752f, -933.3588f, 0f, 0f, 0f, 0f);
            __36.StartData = new PhsxData(8591.752f, -933.3588f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__36);

            CloudberryKingdom.BackgroundFloater __37 = new CloudberryKingdom.BackgroundFloater();
            __37.Name = "hills_plants_4";
            __37.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __37.MyQuad.Quad.ExtraTexture1 = null;
            __37.MyQuad.Quad.ExtraTexture2 = null;
            __37.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_4");
            __37.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __37.MyQuad.Quad.BlendAddRatio = 0f;

            __37.MyQuad.Base = new BasePoint(957.8854f, 0f, 0f, 524.0001f, 10830f, -1125.896f);

            __37.uv_speed = new Vector2(0f, 0f);
            __37.uv_offset = new Vector2(0f, 0f);
            __37.Data = new PhsxData(10830f, -1125.896f, 0f, 0f, 0f, 0f);
            __37.StartData = new PhsxData(10830f, -1125.896f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__37);

            CloudberryKingdom.BackgroundFloater __38 = new CloudberryKingdom.BackgroundFloater();
            __38.Name = "hills_plants_5";
            __38.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __38.MyQuad.Quad.ExtraTexture1 = null;
            __38.MyQuad.Quad.ExtraTexture2 = null;
            __38.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_5");
            __38.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __38.MyQuad.Quad.BlendAddRatio = 0f;

            __38.MyQuad.Base = new BasePoint(695.9648f, 0f, 0f, 565.1686f, 13020.1f, -1053.695f);

            __38.uv_speed = new Vector2(0f, 0f);
            __38.uv_offset = new Vector2(0f, 0f);
            __38.Data = new PhsxData(13020.1f, -1053.695f, 0f, 0f, 0f, 0f);
            __38.StartData = new PhsxData(13020.1f, -1053.695f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__38);

            CloudberryKingdom.BackgroundFloater __39 = new CloudberryKingdom.BackgroundFloater();
            __39.Name = "hills_plants_6";
            __39.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __39.MyQuad.Quad.ExtraTexture1 = null;
            __39.MyQuad.Quad.ExtraTexture2 = null;
            __39.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_6");
            __39.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __39.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __39.MyQuad.Quad.BlendAddRatio = 0f;

            __39.MyQuad.Base = new BasePoint(1266.809f, 0f, 0f, 1009.208f, 15234.28f, -933.358f);

            __39.uv_speed = new Vector2(0f, 0f);
            __39.uv_offset = new Vector2(0f, 0f);
            __39.Data = new PhsxData(15234.28f, -933.358f, 0f, 0f, 0f, 0f);
            __39.StartData = new PhsxData(15234.28f, -933.358f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__39);

            CloudberryKingdom.BackgroundFloater __40 = new CloudberryKingdom.BackgroundFloater();
            __40.Name = "hills_plants_1";
            __40.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __40.MyQuad.Quad.ExtraTexture1 = null;
            __40.MyQuad.Quad.ExtraTexture2 = null;
            __40.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_1");
            __40.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __40.MyQuad.Quad.BlendAddRatio = 0f;

            __40.MyQuad.Base = new BasePoint(646.1251f, 0f, 0f, 486.9349f, 17219.33f, -986.3066f);

            __40.uv_speed = new Vector2(0f, 0f);
            __40.uv_offset = new Vector2(0f, 0f);
            __40.Data = new PhsxData(17219.33f, -986.3066f, 0f, 0f, 0f, 0f);
            __40.StartData = new PhsxData(17219.33f, -986.3066f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__40);

            CloudberryKingdom.BackgroundFloater __41 = new CloudberryKingdom.BackgroundFloater();
            __41.Name = "hills_plants_2";
            __41.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __41.MyQuad.Quad.ExtraTexture1 = null;
            __41.MyQuad.Quad.ExtraTexture2 = null;
            __41.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_2");
            __41.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __41.MyQuad.Quad.BlendAddRatio = 0f;

            __41.MyQuad.Base = new BasePoint(974.2943f, 0f, 0f, 843.0762f, 19203.91f, -1006.042f);

            __41.uv_speed = new Vector2(0f, 0f);
            __41.uv_offset = new Vector2(0f, 0f);
            __41.Data = new PhsxData(19203.91f, -1006.042f, 0f, 0f, 0f, 0f);
            __41.StartData = new PhsxData(19203.91f, -1006.042f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__41);

            CloudberryKingdom.BackgroundFloater __42 = new CloudberryKingdom.BackgroundFloater();
            __42.Name = "hills_plants_3";
            __42.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __42.MyQuad.Quad.ExtraTexture1 = null;
            __42.MyQuad.Quad.ExtraTexture2 = null;
            __42.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_3");
            __42.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __42.MyQuad.Quad.BlendAddRatio = 0f;

            __42.MyQuad.Base = new BasePoint(892.2518f, 0f, 0f, 804.9318f, 21566.81f, -1132.153f);

            __42.uv_speed = new Vector2(0f, 0f);
            __42.uv_offset = new Vector2(0f, 0f);
            __42.Data = new PhsxData(21566.81f, -1132.153f, 0f, 0f, 0f, 0f);
            __42.StartData = new PhsxData(21566.81f, -1132.153f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__42);

            CloudberryKingdom.BackgroundFloater __43 = new CloudberryKingdom.BackgroundFloater();
            __43.Name = "hills_plants_4";
            __43.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __43.MyQuad.Quad.ExtraTexture1 = null;
            __43.MyQuad.Quad.ExtraTexture2 = null;
            __43.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_4");
            __43.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __43.MyQuad.Quad.BlendAddRatio = 0f;

            __43.MyQuad.Base = new BasePoint(957.8854f, 0f, 0f, 524.0001f, 23854.63f, -1173.068f);

            __43.uv_speed = new Vector2(0f, 0f);
            __43.uv_offset = new Vector2(0f, 0f);
            __43.Data = new PhsxData(23854.63f, -1173.068f, 0f, 0f, 0f, 0f);
            __43.StartData = new PhsxData(23854.63f, -1173.068f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__43);

            CloudberryKingdom.BackgroundFloater __44 = new CloudberryKingdom.BackgroundFloater();
            __44.Name = "hills_plants_5";
            __44.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __44.MyQuad.Quad.ExtraTexture1 = null;
            __44.MyQuad.Quad.ExtraTexture2 = null;
            __44.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_5");
            __44.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __44.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __44.MyQuad.Quad.BlendAddRatio = 0f;

            __44.MyQuad.Base = new BasePoint(695.9648f, 0f, 0f, 565.1686f, 25828.14f, -1124.934f);

            __44.uv_speed = new Vector2(0f, 0f);
            __44.uv_offset = new Vector2(0f, 0f);
            __44.Data = new PhsxData(25828.14f, -1124.934f, 0f, 0f, 0f, 0f);
            __44.StartData = new PhsxData(25828.14f, -1124.934f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__44);

            CloudberryKingdom.BackgroundFloater __45 = new CloudberryKingdom.BackgroundFloater();
            __45.Name = "hills_plants_6";
            __45.MyQuad.Quad.MyEffect = Tools.BasicEffect;
            __45.MyQuad.Quad.ExtraTexture1 = null;
            __45.MyQuad.Quad.ExtraTexture2 = null;
            __45.MyQuad.Quad._MyTexture = Tools.Texture("hills_plants_6");
            __45.MyQuad.Quad.MySetColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.PremultipliedColor = new Color(255, 255, 255, 255);
            __45.MyQuad.Quad.BlendAddRatio = 0f;

            __45.MyQuad.Base = new BasePoint(1266.809f, 0f, 0f, 1009.208f, 28090.46f, -860.1955f);

            __45.uv_speed = new Vector2(0f, 0f);
            __45.uv_offset = new Vector2(0f, 0f);
            __45.Data = new PhsxData(28090.46f, -860.1955f, 0f, 0f, 0f, 0f);
            __45.StartData = new PhsxData(28090.46f, -860.1955f, 0f, 0f, 0f, 0f);
            __27.Floaters.Add(__45);

            __27.Parallax = 0.78f;
            __27.DoPreDraw = false;
            b.MyCollection.Lists.Add(__27);

            b.Light = 1f;
            //b.BL = new Vector2(-4750f, -4500f);
            //b.TR = new Vector2(42470f, 2000f);
            b.BL = new Vector2(-100000, -10000);
            b.TR = new Vector2(100000, 10000);
        }
    }
}
