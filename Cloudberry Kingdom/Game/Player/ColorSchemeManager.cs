using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public struct ColorSchemeManager
    {
        public static List<ColorScheme> ColorSchemes, ComputerColorSchemes;

        static void AddScheme(ColorScheme scheme, bool ValidComputerScheme)
        {
            ColorSchemes.Add(scheme);
            if (ValidComputerScheme)
                ComputerColorSchemes.Add(scheme);
        }

        public static List<MenuListItem> HatList, ColorList, CapeColorList, CapeOutlineColorList, TextureList, OutlineList;

        public static List<Hat> HatInfo;
        public static List<Hat> BeardInfo;

        public static List<MenuListItem> ClrList;

        public static ClrTextFx None;

        static MenuListItem _i(int Guid, int Price, Color Clr, Matrix M, string Name)
        {
            return new MenuListItem
                (
                    new ClrTextFx(Guid, Price, Clr, M, Name),
                    Name
                );
        }

        static MenuListItem _i(int Guid, int Price, Color Clr, Color HighlightClr, Matrix M, string Name)
        {
            return new MenuListItem
                (
                    new ClrTextFx(Guid, Price, Clr, HighlightClr, M, Name),
                    Name
                );
        }

        static MenuListItem _i(int Guid, int Price, Color Clr, Matrix M, EzEffect Effect, string Name)
        {
            var ctf = new ClrTextFx(Guid, Price, Clr, M, Name);
            ctf.Effect = Effect;

            return new MenuListItem
                (
                    ctf,
                    Name
                );
        }

        public static void InitColorSchemes()
        {
            ColorList = new List<MenuListItem>();
            CapeColorList = new List<MenuListItem>();
            CapeOutlineColorList = new List<MenuListItem>();
            HatList = new List<MenuListItem>();
            TextureList = new List<MenuListItem>();
            OutlineList = new List<MenuListItem>();

            ColorSchemes = new List<ColorScheme>();
            ComputerColorSchemes = new List<ColorScheme>();

            HatInfo = new List<Hat>();
            BeardInfo = new List<Hat>();

            // Mod cape functions
            Action<Bob> Reset = bob =>
            {
                bob.ShowCape = false;

                ObjectClass obj = bob.PlayerObject;
                obj.FindQuad("Wing1").Show = false;
                obj.FindQuad("Wing2").Show = false;
                obj.FindQuad("DWing1").Show = false;
                obj.FindQuad("DWing2").Show = false;
            };
            Action<Bob> CapeOn = bob =>
            {
                Reset(bob);
                bob.ShowCape = true;
            };   

            // Fill the beard list
            Hat beard;

            Hat.Vandyke =
            beard = new Hat("None");
            beard.Name = "Vandyke";
            beard.Price = Hat.Expensive;
            beard.Guid = 5259001;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Beard = 
            beard = new Hat("Facial_Beard");
            beard.Name = "Rugged";
            beard.Price = Hat.Expensive;
            beard.Guid = 5259002;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Mustache = 
            beard = new Hat("Facial_Moustache");
            beard.Name = "Manhattan";
            beard.Price = Hat.Expensive;
            beard.Guid = 5259003;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.BigBeard = 
            beard = new Hat("Facial_BigBeard");
            beard.Name = "Lumberjack";
            beard.Price = Hat.Expensive;
            beard.Guid = 5259004;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Goatee = 
            beard = new Hat("Facial_Goatee");
            beard.Name = "Goatee";
            beard.Price = Hat.Expensive;
            beard.Guid = 5259005;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);


            // Fill the hat list
            Hat hat;
            hat = new Hat("None");
            hat.Name = "None";
            hat.Guid = 19;
                HatInfo.Add(hat);
                Hat.None = hat;


            hat = new Hat("Hat_Viking");
            hat.Name = "Viking";
            hat.Price = Hat.Expensive;
            hat.Guid = 20;
                hat.HatPicShift = new Vector2(-.02f, .075f);
				HatInfo.Add(hat);
				Hat.Viking = hat;
            hat = new Hat("Hat_Fedora");
            hat.Name = "Fedora";
            hat.Price = Hat.Cheap;
            hat.Guid = 21;
                hat.HatPicScale *= 1.075f;
				HatInfo.Add(hat);                
				Hat.Fedora = hat;
            hat = new Hat("Hat_Afro");
            hat.Name = "Afro";
            hat.Price = Hat.Mid;
            hat.Guid = 22;
                hat.HatPicScale *= 1.065f;
                HatInfo.Add(hat);
                Hat.Afro = hat;
            hat = new Hat("Hat_Halo");
            hat.Name = "Halo";
            hat.Price = Hat.Mid;
            hat.Guid = 23;
                hat.HatPicScale *= 1.07f;
                hat.HatPicShift = new Vector2(.15f, .08f);
                HatInfo.Add(hat);
                Hat.Halo = hat;
            hat = new Hat("Hat_FireHead", false);
            hat.Name = "Firehead";
            hat.Price = Hat.Expensive;
            hat.Guid = 24;
                hat.HatPicTexture = Fireball.FlameTexture;
                hat.HatPicScale *= 1.8f;
                HatInfo.Add(hat);
                Hat.FireHead = hat;
            hat = new Hat("Hat_Ghost");
            hat.Name = "Ghost";
            hat.Price = Hat.Cheap;
            hat.Guid = 25;
                hat.HatPicScale *= .8f;
                HatInfo.Add(hat);
                Hat.Ghost = hat;
            //hat = new Hat("Hat_CheckpointHead", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 26;
            //    HatInfo.Add(hat);
            //    Hat.CheckpointHead = hat;
            hat = new Hat("Hat_Horns", true, false);
            hat.Name = "Horns";
            hat.Price = Hat.Mid;
            hat.Guid = 27;
                hat.HatPicTexture = Tools.TextureWad.FindByName("HatPic_Horns");
                hat.HatPicScale *= 1.1f;
                HatInfo.Add(hat);
                Hat.Horns = hat;
            //hat = new Hat("Hat_Cloud");
            //hat.Price = Hat.Mid;
            //hat.Guid = 28;
            //    hat.HatPicScale *= new Vector2(1.45f, 1.85f) * .83f;
            //    HatInfo.Add(hat);
            //    Hat.Cloud = hat;
            //hat = new Hat("", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 29;
            //    hat.HatPicTexture = Tools.TextureWad.FindByName("QM_Happy");
            //    hat.HatPicScale *= .85f;
            //    HatInfo.Add(hat);
            //    Hat.NoHead = hat;
            //hat = new Hat("Hat_FallingBlockHead", false);
            //hat.Price = Hat.Cheap;
            //hat.Guid = 30;
            //    hat.HatPicScale *= .82f;
            //    HatInfo.Add(hat);
            //    Hat.FallingBlockHead = hat;
            //hat = new Hat("Hat_BlobHead", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 31;
            //    hat.HatPicScale *= .89f;
            //    HatInfo.Add(hat);
            //    Hat.BlobHead = hat;
            //hat = new Hat("Hat_MovingBlockHead", false);
            //hat.Price = Hat.Cheap;
            //hat.Guid = 32;
            //    hat.HatPicScale *= .89f;
            //    HatInfo.Add(hat);
            //    Hat.MovingBlockHead = hat;
            //hat = new Hat("Hat_SpikeyHead", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 33;
            //    hat.HatPicScale *= 1.16f;
            //    hat.HatPicShift = new Vector2(-.015f, .2f);
            //    HatInfo.Add(hat);
            //    Hat.SpikeyHead = hat;
            //hat = new Hat("Hat_FallingBlock3Head", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 34;
            //    hat.HatPicScale *= .82f;
            //    HatInfo.Add(hat);
            //    Hat.FallingBlock3Head = hat;
            //hat = new Hat("Hat_Pink", false);
            //hat.Price = Hat.Cheap;
            //hat.Guid = 35;
            //    hat.HatPicScale *= .95f;
            //    HatInfo.Add(hat);
            //    Hat.Pink = hat;
            hat = new Hat("Hat_Bubble", true);
            hat.Name = "Bubble";
            hat.DrawHead = false;
            hat.Price = Hat.Mid;
            hat.Guid = 36;
				HatInfo.Add(hat);
				Hat.Bubble = hat;


float ScaleNew = 1.35f;
float DefaultShiftX = -.35f;

            hat = new Hat("Hat_TopHat");
            hat.Name = "Mmm, yes, indeed.";
            hat.Price = Hat.Expensive;
            hat.Guid = 37;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.35f;
				HatInfo.Add(hat);
				Hat.TopHat = hat;
            hat = new Hat("Hat_Knight", false);
            hat.Name = "Sir Arthur";
            hat.Price = Hat.Expensive;
            hat.Guid = 38;
                hat.HatPicShift = new Vector2(DefaultShiftX, .105f);
                hat.HatPicScale *= ScaleNew * 1.145f;
				HatInfo.Add(hat);
				Hat.Knight = hat;
            hat = new Hat("Hat_Toad");
            hat.Name = "Trippy";
            hat.Price = Hat.Expensive;
            hat.Guid = 39;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.115f;
				HatInfo.Add(hat);
				Hat.Toad = hat;
            hat = new Hat("Hat_BubbleBobble");
            hat.Name = "Om Nom Nom";
            hat.Price = Hat.Expensive;
            hat.Guid = 40;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.105f;
				HatInfo.Add(hat);
				Hat.BubbleBobble = hat;
            hat = new Hat("Hat_Brain");
            hat.DrawHead = false;
            hat.Name = "BRAAAINS";
            hat.Price = Hat.Expensive;
            hat.Guid = 41;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.095f;
                hat.AllowsFacialHair = false;
				HatInfo.Add(hat);
				Hat.Brain = hat;
            hat = new Hat("Hat_Gosu");
            hat.Name = "Gosu";
            hat.Price = Hat.Expensive;
            hat.Guid = 42;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				Hat.Gosu = hat;



            // Buyables
            Hat.RobinHood = 
            hat = new Hat("Hat_RobinHood");
            hat.Name = "Bob in Tights";
            hat.Price = Hat.Mid;
            hat.Guid = 1;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);

            Hat.Rasta = 
            hat = new Hat("Hat_Rasta");
            hat.Name = "Pastafarian";
            hat.Price = Hat.Mid;
            hat.Guid = 2;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Pumpkin = 
            hat = new Hat("Hat_Pumpkin");
            hat.DrawHead = false;
            hat.Name = "Pumpkin";
            hat.Price = Hat.Mid;
            hat.Guid = 3;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                hat.AllowsFacialHair = false;
				HatInfo.Add(hat);
				
            Hat.Pirate = 
            hat = new Hat("Hat_Pirate");
            hat.Name = "ARRRGGG";
            hat.Price = Hat.Expensive;
            hat.Guid = 4;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Miner = 
            hat = new Hat("Hat_Miner");
            hat.Name = "Men at Work";
            hat.Price = Hat.Cheap;
            hat.Guid = 5;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Glasses = 
            hat = new Hat("Hat_Glasses");
            hat.Name = "Four Eyes";
            hat.Price = Hat.Mid;
            hat.Guid = 6;
                hat.HatPicShift = new Vector2(DefaultShiftX, .1f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
			
            Hat.BunnyEars = 
            hat = new Hat("Hat_BunnyEars");
            hat.Name = "Bunny Ears";
            hat.Price = Hat.Mid;
            hat.Guid = 7;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.135f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            //hat = new Hat("Hat_Beard");
            //hat.Price = Hat.Mid;
            //hat.Guid = 8;
            //    hat.HatPicShift = new Vector2(DefaultShiftX, .435f);
            //    hat.HatPicScale *= ScaleNew * 1.165f;
            //    HatInfo.Add(hat);
				
            Hat.Antlers = 
            hat = new Hat("Hat_Antlers");
            hat.Name = "The Great Stag";
            hat.Price = Hat.Mid;
            hat.Guid = 9;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.135f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Arrow = 
            hat = new Hat("Hat_Arrow");
            hat.Name = "Custard";
            hat.Price = Hat.Expensive;
            hat.Guid = 10;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Bag = 
            hat = new Hat("Hat_Bag");
            hat.Name = "Brown Bag";
            hat.DrawHead = false;
            hat.Price = Hat.Cheap;
            hat.Guid = 11;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                hat.AllowsFacialHair = false;
				HatInfo.Add(hat);
				
            Hat.Cone = 
            hat = new Hat("Hat_Cone");
            hat.Name = "Traffic Cone";
            hat.Price = Hat.Cheap;
            hat.Guid = 12;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Pope = 
            hat = new Hat("Hat_Pope");
            hat.Name = "Pope Hat";
            hat.Price = Hat.Expensive;
            hat.Guid = 13;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Rice = 
            hat = new Hat("Hat_Rice");
            hat.Name = "Rice Hat";
            hat.Price = Hat.Cheap;
            hat.Guid = 14;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Santa = 
            hat = new Hat("Hat_Santa");
            hat.Name = "Santa Claus";
            hat.Price = Hat.Expensive;
            hat.Guid = 15;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Sombrero = 
            hat = new Hat("Hat_Sombrero");
            hat.Name = "Sombrero";
            hat.Price = Hat.Cheap;
            hat.Guid = 16;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Tiki = 
            hat = new Hat("Hat_Tiki");
            hat.DrawHead = false;
            hat.Name = "Tiki Mask";
            hat.Price = Hat.Mid;
            hat.Guid = 17;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            Hat.Wizard = 
            hat = new Hat("Hat_Wizard");
            hat.Name = "Wizard";
            hat.Price = Hat.Mid;
            hat.Guid = 18;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.015f);
                hat.HatPicScale *= ScaleNew * 1.145f;
				HatInfo.Add(hat);
				
#if DEBUG
            // Check GUID uniqueness
            if (!Tools.AllUnique(HatInfo, h => h.GetGuid()))
                Tools.Write("Duplicate hat guid!");
#endif

            for (int i = 0; i < HatInfo.Count; i++)
                HatList.Add(new MenuListItem(i, ""));

            
            //hat = new Hat("Hat_Cattails", true, false);
            //hat.HatPicTexture = Tools.TextureWad.FindByName("HatPic_Horns");
            //HatInfo.Add(hat);


            //colorm = LinearColorTransform(0); // Green
            //colorm = HsvTransform(1.3f, 1.2f, 100); // Gold
            //colorm = HsvTransform(1.5f, 1.5f, 100); // Gold 2
            //colorm = HsvTransform(1.3f, 1.2f, 200); // Hot pink
            //colorm = HsvTransform(1.3f, 1.2f, 250); // ?
            //colorm = HsvTransform(.5f, 0f, 0); // Black
            //colorm = HsvTransform(.15f, 0f, 0); // Dark Black
            //colorm = HsvTransform(.75f, 0f, 0); // Gray
            //colorm = HsvTransform(.8f, 1.3f, 225); // Purple
            //colorm = HsvTransform(.9f, 1.3f, 110); // Orange
            //colorm = LinearColorTransform(45); // Teal
            //colorm = LinearColorTransform(120); // Blue
            //colorm = HsvTransform(.95f, 1.3f, 0) * LinearColorTransform(240); // Red
            //colorm = HsvTransform(1.25f, 1.3f, 0) * LinearColorTransform(305); // Yellow

            // Fill the skin color list
            ColorList.Add(_i(3500, 0, new Color(1f, 1f, 1f), ColorHelper.HsvTransform(1.25f, 0f, 0),  "White"));    // 0
            ColorList.Add(_i(3501, 0, Color.Silver,          ColorHelper.HsvTransform(.85f, 0f, 0),   "Silver"));   // 1
            ColorList.Add(_i(3502, 0, Color.Gray,            ColorHelper.HsvTransform(.525f, 0f, 0),   "Gray"));     // 2
            ColorList.Add(_i(3503, 0, new Color(0f, 0f, 0f),
                                      new Color(50, 50, 50), ColorHelper.HsvTransform(.3f, 0f, 0),    "Black"));    // 3

            ColorList.Add(_i(3504, 0, Color.Cyan,            ColorHelper.LinearColorTransform(45),    "Cyan"));     // 4  
            ColorList.Add(_i(3505, 0, new Color(0f, 0f, 1f), ColorHelper.LinearColorTransform(120),   "Blue"));     // 5
            ColorList.Add(_i(3506, 0, Color.DarkBlue,        ColorHelper.LinearColorTransform(80),    "Teal"));     // 6
            ColorList.Add(_i(3507, 0, Color.Indigo,          ColorHelper.HsvTransform(.8f, 1.3f, 225),"Indigo"));   // 7
            ColorList.Add(_i(3508, 0, Color.Purple,          ColorHelper.HsvTransform(.85f, 1.1f, 205),"Purple"));  // 8
            ColorList.Add(_i(3509, 0, Color.Brown,           ColorHelper.HsvTransform(1f, 1f, 80),"Brown"));        // 9
            ColorList.Add(_i(3510, 0, new Color(1f, 0, 0f),
                                      ColorHelper.HsvTransform(.95f, 1.3f, 0) * ColorHelper.LinearColorTransform(240), "Red")); // 10
            ColorList.Add(_i(3511, 0, Color.HotPink,         ColorHelper.HsvTransform(1.3f, 1.2f, 200), "Hot Pink"));     // 11
            ColorList.Add(_i(3512, 0, new Color(1f, .6f, 0f),ColorHelper.HsvTransform(.9f, 1.3f, 110), "Orange"));        // 12
            ColorList.Add(_i(3513, 0, Color.Gold,            ColorHelper.HsvTransform(1.3f, 1.2f, 100), "Gold"));         // 13
            ColorList.Add(_i(3514, 0, Color.Yellow,          ColorHelper.HsvTransform(1.5f, 1.5f, 100), "Yellow"));       // 14
            ColorList.Add(_i(3515, 0, new Color(0f, 1f, 0f), ColorHelper.LinearColorTransform(0), "Green"));              // 15
            ColorList.Add(_i(3516, 0, Color.LimeGreen,       ColorHelper.HsvTransform(1.25f, 1.35f, 0), "LimeGreen"));    // 16
            ColorList.Add(_i(3517, 0, Color.ForestGreen,     ColorHelper.HsvTransform(.75f, .8f, 0), "ForestGreen"));     // 17
            

            // Fill the textured skin list
            /*
            TextureList.Add(new MenuListItem(new ClrTextFx(3518, 1500, Color.White, "pillar_xlarge", Color.Gray), "Stone Skin"));     // 0
            TextureList.Add(new MenuListItem(new ClrTextFx(3519, 1000, Color.White, "Skins\\Rainbow", Color.Red), "Rainbow Skin"));  // 1
            TextureList.Add(new MenuListItem(new ClrTextFx(3520, 1000, Color.White, "Skins\\Tiger", Color.Orange), "Tiger Skin"));      // 2
            TextureList.Add(new MenuListItem(new ClrTextFx(3521, 1000, new Color(1, 1, 1, .66f),
                                            "Skins\\Water", Color.LightBlue, "WaterBob"), "Aqua Skin"));                     // 3
            TextureList.Add(new MenuListItem(new ClrTextFx(3522, 1000, Color.White, "Skins\\Stars2", Color.DarkBlue), "Star Skin"));      // 4
            TextureList.Add(new MenuListItem(new ClrTextFx(3523, 1000, Color.White, "Skins\\Fractal1", Color.Cyan, "WaterBob"),
                                            "Psychadelic Skin"));                                           // 5

             */
            var m = new Matrix(); m.M11 = m.M12 = m.M13 = m.M14 = m.M21 = m.M22 = m.M23 = m.M24 = m.M31 = m.M32 = m.M33 = m.M34 = m.M41 = m.M42 = m.M43 = m.M44 = 0;
            MenuListItem NoTexture = _i(3524, 0, Color.Transparent, m, "Clear"); // 6
            TextureList.Add(NoTexture);


            // Fill the cape color list
            ClrTextFx cape;
            None = new ClrTextFx(3525, 0, new Color(1f, 1f, 1f, 0f), Matrix.Identity);
            None.Name = "None";
            CapeColorList.Add(new MenuListItem(None, "None"));
            CapeColorList.AddRange(ColorList);

            // Fill the outline color list
            OutlineList.AddRange(CapeColorList);
            OutlineList.Add(NoTexture);

            // Fill the cape outline list
            foreach (MenuListItem item in CapeColorList)
            {
                Color clr = ((ClrTextFx)item.obj).Clr;
                Color color = new Color(clr.ToVector3() * .8f);
                color.A = clr.A;
                ClrTextFx capeoutline = (ClrTextFx)item.obj;
                capeoutline.Clr = color;
                CapeOutlineColorList.Add(new MenuListItem(capeoutline, item.str));
            }

            // Add textures to skin color list and cape color list
            ColorList.AddRange(TextureList);
            CapeColorList.AddRange(TextureList);
            CapeColorList.Remove(NoTexture);

            ClrTextFx fx;
            Vector2 fx_scale = new Vector2(.875f, 1.195f) * .98f;

            fx = new ClrTextFx(3526, 500, Color.White, "FallingBlock2", false, "FallingBlock2");
            fx.Name = "Concern";
            fx.PicScale = fx_scale;
            CapeColorList.Add(new MenuListItem(fx, "ConcernedCape"));

            fx = new ClrTextFx(3527, 500, Color.White, "FallingBlock3", false, "FallingBlock3");
            fx.Name = "AAAaahhh!";
            fx.PicScale = fx_scale;
            CapeColorList.Add(new MenuListItem(fx, "ScreamingCape"));

            fx = new ClrTextFx(3528, 500, Color.White, "Capes\\FallingBlock4Cape", false, "FallingBlock4");
            fx.Name = "Anger";
            fx.PicScale = fx_scale * new Vector2(1.022f, 1.028f);
            CapeColorList.Add(new MenuListItem(fx, "AngryCape"));

            // Wings and cape mod functions
            List<MenuListItem> NewCapeList = new List<MenuListItem>();
            foreach (MenuListItem item in CapeColorList)
            {
                cape = (ClrTextFx)item.obj;
                cape.ModObject = CapeOn;
                NewCapeList.Add(new MenuListItem(cape, item.str));
            }
            CapeColorList = NewCapeList;

            //cape = (ClrTextFx)CapeColorList[1].obj;
            //cape.ModObject = bob =>
            //{
            //    Reset(bob);
            //    bob.PlayerObject.FindQuad("Wing1").Show = true;
            //    bob.PlayerObject.FindQuad("Wing2").Show = true;
            //};
            //cape.PicTexture = Tools.TextureWad.FindByName("CapePic_Wings");
            //cape.Guid = 5000;
            //cape.Price = 5000;
            //CapeColorList.Add(new MenuListItem(cape, "Wings"));
            
            //cape = (ClrTextFx)CapeColorList[1].obj;
            //cape.ModObject = bob =>
            //{
            //    Reset(bob);
            //    bob.PlayerObject.FindQuad("DWing1").Show = true;
            //    bob.PlayerObject.FindQuad("DWing2").Show = true;
            //};
            //cape.PicTexture = Tools.TextureWad.FindByName("CapePic_DWings");
            //cape.Guid = 5001;
            //cape.Price = 5000;
            //CapeColorList.Add(new MenuListItem(cape, "DWings"));


            //// Add solid colors
            //ColorList.Add(_i(77001, 0, Color.Red, ColorHelper.PureColor(Color.Red), Tools.HslEffect, "Solid Red"));
            //ColorList.Add(_i(77002, 0, Color.Green, ColorHelper.PureColor(Color.Green), Tools.HslEffect, "Solid Green"));
            //ColorList.Add(_i(77003, 0, Color.Blue, ColorHelper.PureColor(Color.Blue), Tools.HslEffect, "Solid Blue"));
            //ColorList.Add(_i(77004, 0, Color.Purple, ColorHelper.PureColor(Color.Purple), Tools.HslEffect, "Solid Purple"));
            //ColorList.Add(_i(77005, 0, Color.Orange, ColorHelper.PureColor(Color.Orange), Tools.HslEffect, "Solid Orange"));

            // Combine all colors
            List<MenuListItem> temp;
            temp = new List<MenuListItem>();
            temp.AddRange(ColorList);
            temp.AddRange(CapeColorList);
            temp.AddRange(CapeOutlineColorList);
            temp.AddRange(OutlineList);
            ClrList = Tools.MakeUnique(temp, item => ((ClrTextFx)item.obj).Guid);


            // Create the default color schemes
            ColorScheme scheme;

            AddScheme(new ColorScheme("Green", "Red", "Black", "", ""), true);
            AddScheme(new ColorScheme("Gray", "Red", "Black", "", Hat.Mustache.Name), false);
            AddScheme(new ColorScheme("Hot Pink", "Hot Pink", "Hot Pink", Hat.BunnyEars.Name, Hat.Beard.Name), false);
            AddScheme(new ColorScheme("Gold", "Gold", "Gold", "", ""), false);
            AddScheme(new ColorScheme("Purple", "Indigo", "Hot Pink", "None", "Vandyke"), false);
            AddScheme(new ColorScheme("ForestGreen", "Yellow", "Gold", "Rice Hat", "Rugged"), false);
            AddScheme(new ColorScheme("Red", "None", "None", "The Great Stag", "Vandyke"), false);
        }
    }
}