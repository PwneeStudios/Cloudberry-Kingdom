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

        public static List<MenuListItem> ClrList;

        public static ClrTextFx None;

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

            // Fill the hat list
            Hat hat;
            hat = new Hat("None");
            hat.Guid = 19;
                HatInfo.Add(hat);
                Hat.None = hat;


            hat = new Hat("Hat_Viking");
            hat.Price = Hat.Expensive;
            hat.Guid = 20;
                hat.HatPicShift = new Vector2(-.02f, .075f);
				HatInfo.Add(hat);
				Hat.Viking = hat;
            hat = new Hat("Hat_Fedora");
            hat.Price = Hat.Cheap;
            hat.Guid = 21;
                hat.HatPicScale *= 1.075f;
				HatInfo.Add(hat);                
				Hat.Fedora = hat;
            hat = new Hat("Hat_Afro");
            hat.Price = Hat.Mid;
            hat.Guid = 22;
                hat.HatPicScale *= 1.065f;
                HatInfo.Add(hat);
                Hat.Afro = hat;
            hat = new Hat("Hat_Halo");
            hat.Price = Hat.Mid;
            hat.Guid = 23;
                hat.HatPicScale *= 1.07f;
                hat.HatPicShift = new Vector2(.15f, .08f);
                HatInfo.Add(hat);
                Hat.Halo = hat;
            hat = new Hat("Hat_FireHead", false);
            hat.Price = Hat.Expensive;
            hat.Guid = 24;
                hat.HatPicTexture = Fireball.FlameTexture;
                hat.HatPicScale *= 1.8f;
                HatInfo.Add(hat);
                Hat.FireHead = hat;
            hat = new Hat("Hat_Ghost");
            hat.Price = Hat.Cheap;
            hat.Guid = 25;
                hat.HatPicScale *= .8f;
                HatInfo.Add(hat);
                Hat.Ghost = hat;
            hat = new Hat("Hat_CheckpointHead", false);
            hat.Price = Hat.Mid;
            hat.Guid = 26;
				HatInfo.Add(hat);
				Hat.CheckpointHead = hat;
            hat = new Hat("Hat_Horns", true, false);
            hat.Price = Hat.Mid;
            hat.Guid = 27;
                hat.HatPicTexture = Tools.TextureWad.FindByName("HatPic_Horns");
                hat.HatPicScale *= 1.1f;
                HatInfo.Add(hat);
                Hat.Horns = hat;
            hat = new Hat("Hat_Cloud");
            hat.Price = Hat.Mid;
            hat.Guid = 28;
                hat.HatPicScale *= new Vector2(1.45f, 1.85f) * .83f;
                HatInfo.Add(hat);
                Hat.Cloud = hat;
            hat = new Hat("", false);
            hat.Price = Hat.Mid;
            hat.Guid = 29;
                //hat.HatPicTexture = Tools.TextureWad.FindByName("Pop");
                hat.HatPicTexture = Tools.TextureWad.FindByName("QM_Happy");
                hat.HatPicScale *= .85f;
                HatInfo.Add(hat);
                Hat.NoHead = hat;
            hat = new Hat("Hat_FallingBlockHead", false);
            hat.Price = Hat.Cheap;
            hat.Guid = 30;
                hat.HatPicScale *= .82f;
                HatInfo.Add(hat);
                Hat.FallingBlockHead = hat;
            hat = new Hat("Hat_BlobHead", false);
            hat.Price = Hat.Mid;
            hat.Guid = 31;
                hat.HatPicScale *= .89f;
				HatInfo.Add(hat);
				Hat.BlobHead = hat;
            hat = new Hat("Hat_MovingBlockHead", false);
            hat.Price = Hat.Cheap;
            hat.Guid = 32;
                hat.HatPicScale *= .89f;
				HatInfo.Add(hat);
				Hat.MovingBlockHead = hat;
            hat = new Hat("Hat_SpikeyHead", false);
            hat.Price = Hat.Mid;
            hat.Guid = 33;
                hat.HatPicScale *= 1.16f;
                hat.HatPicShift = new Vector2(-.015f, .2f);
                HatInfo.Add(hat);
                Hat.SpikeyHead = hat;
            hat = new Hat("Hat_FallingBlock3Head", false);
            hat.Price = Hat.Mid;
            hat.Guid = 34;
                hat.HatPicScale *= .82f;
                HatInfo.Add(hat);
                Hat.FallingBlock3Head = hat;
            hat = new Hat("Hat_Pink", false);
            hat.Price = Hat.Cheap;
            hat.Guid = 35;
                hat.HatPicScale *= .95f;
				HatInfo.Add(hat);
				Hat.Pink = hat;
            hat = new Hat("Hat_Bubble", true);
            hat.Price = Hat.Mid;
            hat.Guid = 36;
				HatInfo.Add(hat);
				Hat.Bubble = hat;


float ScaleNew = 1.35f;
float DefaultShiftX = -.35f;

            hat = new Hat("Hat_TopHat");
            hat.Price = Hat.Expensive;
            hat.Guid = 37;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.35f;
				HatInfo.Add(hat);
				Hat.TopHat = hat;
            hat = new Hat("Hat_Knight", false);
            hat.Price = Hat.Expensive;
            hat.Guid = 38;
                hat.HatPicShift = new Vector2(DefaultShiftX, .105f);
                hat.HatPicScale *= ScaleNew * 1.145f;
				HatInfo.Add(hat);
				Hat.Knight = hat;
            hat = new Hat("Hat_Toad");
            hat.Price = Hat.Expensive;
            hat.Guid = 39;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.115f;
				HatInfo.Add(hat);
				Hat.Toad = hat;
            hat = new Hat("Hat_BubbleBobble");
            hat.Price = Hat.Expensive;
            hat.Guid = 40;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.105f;
				HatInfo.Add(hat);
				Hat.BubbleBobble = hat;
            hat = new Hat("Hat_Brain");
            hat.Price = Hat.Expensive;
            hat.Guid = 41;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.095f;
				HatInfo.Add(hat);
				Hat.Brain = hat;
            hat = new Hat("Hat_Gosu");
            hat.Price = Hat.Expensive;
            hat.Guid = 42;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				Hat.Gosu = hat;



            // Buyables
            hat = new Hat("Hat_RobinHood");
            hat.Price = Hat.Mid;
            hat.Guid = 1;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);

            hat = new Hat("Hat_Rasta");
            hat.Price = Hat.Mid;
            hat.Guid = 2;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Pumpkin");
            hat.Price = Hat.Mid;
            hat.Guid = 3;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Pirate");
            hat.Price = Hat.Expensive;
            hat.Guid = 4;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Miner");
            hat.Price = Hat.Cheap;
            hat.Guid = 5;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Glasses");
            hat.Price = Hat.Mid;
            hat.Guid = 6;
                hat.HatPicShift = new Vector2(DefaultShiftX, .1f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_BunnyEars");
            hat.Price = Hat.Mid;
            hat.Guid = 7;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.135f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Beard");
            hat.Price = Hat.Mid;
            hat.Guid = 8;
                //hat.HatPicShift = new Vector2(DefaultShiftX, .295f);
                hat.HatPicShift = new Vector2(DefaultShiftX, .435f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Antlers");
            hat.Price = Hat.Mid;
            hat.Guid = 9;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.135f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Arrow");
            hat.Price = Hat.Expensive;
            hat.Guid = 10;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Bag");
            hat.Price = Hat.Cheap;
            hat.Guid = 11;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Cone");
            hat.Price = Hat.Cheap;
            hat.Guid = 12;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Pope");
            hat.Price = Hat.Expensive;
            hat.Guid = 13;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Rice");
            hat.Price = Hat.Cheap;
            hat.Guid = 14;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Santa");
            hat.Price = Hat.Expensive;
            hat.Guid = 15;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Sombrero");
            hat.Price = Hat.Cheap;
            hat.Guid = 16;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Tiki");
            hat.Price = Hat.Mid;
            hat.Guid = 17;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
				HatInfo.Add(hat);
				
            hat = new Hat("Hat_Wizard");
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


            // Fill the skin color list
            ColorList.Add(new MenuListItem(new ClrTextFx(3500, 0, new Color(1f, 1f, 1f)), "White"));    // 0
            ColorList.Add(new MenuListItem(new ClrTextFx(3501, 0, Color.Silver), "Silver"));            // 1
            ColorList.Add(new MenuListItem(new ClrTextFx(3502, 0, Color.Gray), "Gray"));                // 2
            ColorList.Add(new MenuListItem(new ClrTextFx(3503, 0, new Color(0f, 0f, 0f), new Color(50, 50, 50)), "Black"));    // 3

            ColorList.Add(new MenuListItem(new ClrTextFx(3504, 0, Color.Cyan), "Cyan"));                // 4  
            ColorList.Add(new MenuListItem(new ClrTextFx(3505, 0, new Color(0f, 0f, 1f)), "Blue"));     // 5
            ColorList.Add(new MenuListItem(new ClrTextFx(3506, 0, Color.DarkBlue), "Teal"));            // 6
            ColorList.Add(new MenuListItem(new ClrTextFx(3507, 0, Color.Indigo), "Indigo"));            // 7
            ColorList.Add(new MenuListItem(new ClrTextFx(3508, 0, Color.Purple), "Purple"));            // 8
            ColorList.Add(new MenuListItem(new ClrTextFx(3509, 0, Color.Brown), "Brown"));              // 9
            ColorList.Add(new MenuListItem(new ClrTextFx(3510, 0, new Color(1f, 0, 0f)), "Red"));       // 10
            ColorList.Add(new MenuListItem(new ClrTextFx(3511, 0, Color.HotPink), "Hot Pink"));         // 11
            ColorList.Add(new MenuListItem(new ClrTextFx(3512, 0, new Color(1f, .6f, 0f)), "Orange"));  // 12
            ColorList.Add(new MenuListItem(new ClrTextFx(3513, 0, Color.Gold), "Gold"));                // 13
            ColorList.Add(new MenuListItem(new ClrTextFx(3514, 0, Color.Yellow), "Yellow"));            // 14
            ColorList.Add(new MenuListItem(new ClrTextFx(3515, 0, new Color(0f, 1f, 0f)), "Green"));    // 15
            ColorList.Add(new MenuListItem(new ClrTextFx(3516, 0, Color.LimeGreen), "LimeGreen"));      // 16
            ColorList.Add(new MenuListItem(new ClrTextFx(3517, 0, Color.ForestGreen), "ForestGreen"));  // 17
            

            // Fill the textured skin list
            TextureList.Add(new MenuListItem(new ClrTextFx(3518, 1500, Color.White, "pillar_xlarge", Color.Gray), "Stone Skin"));     // 0
            TextureList.Add(new MenuListItem(new ClrTextFx(3519, 1000, Color.White, "Skins\\Rainbow", Color.Red), "Rainbow Skin"));  // 1
            TextureList.Add(new MenuListItem(new ClrTextFx(3520, 1000, Color.White, "Skins\\Tiger", Color.Orange), "Tiger Skin"));      // 2
            TextureList.Add(new MenuListItem(new ClrTextFx(3521, 1000, new Color(1, 1, 1, .66f),
                                            "Skins\\Water", Color.LightBlue, "WaterBob"), "Aqua Skin"));                     // 3
            TextureList.Add(new MenuListItem(new ClrTextFx(3522, 1000, Color.White, "Skins\\Stars2", Color.DarkBlue), "Star Skin"));      // 4
            TextureList.Add(new MenuListItem(new ClrTextFx(3523, 1000, Color.White, "Skins\\Fractal1", Color.Cyan, "WaterBob"),
                                            "Psychadelic Skin"));                                           // 5
            //TextureList.Add(new MenuListItem(new ClrTextFx(3520, Color.White, "skin", Color.Teal), "Bubble"));      // 6
            MenuListItem NoTexture = new MenuListItem(new ClrTextFx(3524, 0, Color.Transparent), "Clear"); // 6
            TextureList.Add(NoTexture);
            
            // Fill the cape color list
            ClrTextFx cape;
            None = new ClrTextFx(3525, 0, new Color(1f, 1f, 1f, 0f));
            None.Name = "None";
            CapeColorList.Add(new MenuListItem(None, "None"));
            CapeColorList.AddRange(ColorList);

            // Fill the outline color list
            OutlineList.AddRange(CapeColorList);

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
            //CapeColorList.Add(new MenuListItem(new ClrTextFx(3520, Color.White, "FallingBlock1", false), ""));
            //CapeColorList.Add(new MenuListItem(new ClrTextFx(3520, Color.White, "FallingBlock2", false), ""));
            //CapeColorList.Add(new MenuListItem(new ClrTextFx(3520, Color.White, "FallingBlock3", false), ""));
            ClrTextFx fx;
            Vector2 fx_scale = new Vector2(.875f, 1.195f) * .98f;
            
            //fx = new ClrTextFx(3520, Color.White, "FallingBlock1", false, "FallingBlock1");
            //fx.PicScale = fx_scale;
            //CapeColorList.Add(new MenuListItem(fx, "HappyCape"));

            fx = new ClrTextFx(3526, 500, Color.White, "FallingBlock2", false, "FallingBlock2");
            fx.PicScale = fx_scale;
            CapeColorList.Add(new MenuListItem(fx, "ConcernedCape"));

            fx = new ClrTextFx(3527, 500, Color.White, "FallingBlock3", false, "FallingBlock3");
            fx.PicScale = fx_scale;
            CapeColorList.Add(new MenuListItem(fx, "ScreamingCape"));

            fx = new ClrTextFx(3528, 500, Color.White, "Capes\\FallingBlock4Cape", false, "FallingBlock4");
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

            cape = (ClrTextFx)CapeColorList[1].obj;
            cape.ModObject = bob =>
            {
                Reset(bob);
                bob.PlayerObject.FindQuad("Wing1").Show = true;
                bob.PlayerObject.FindQuad("Wing2").Show = true;
            };
            cape.PicTexture = Tools.TextureWad.FindByName("CapePic_Wings");
            cape.Guid = 5000;
            cape.Price = 5000;
            CapeColorList.Add(new MenuListItem(cape, "Wings"));
            
            cape = (ClrTextFx)CapeColorList[1].obj;
            cape.ModObject = bob =>
            {
                Reset(bob);
                bob.PlayerObject.FindQuad("DWing1").Show = true;
                bob.PlayerObject.FindQuad("DWing2").Show = true;
            };
            cape.PicTexture = Tools.TextureWad.FindByName("CapePic_DWings");
            cape.Guid = 5001;
            cape.Price = 5000;
            CapeColorList.Add(new MenuListItem(cape, "DWings"));


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

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[11].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[4].obj;
            scheme.SkinColor = (ClrTextFx)ColorList[0].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[3].obj;
            AddScheme(scheme, true);
            

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[11].obj;//3].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[11].obj;//4].obj;
            scheme.SkinColor = (ClrTextFx)ColorList[11].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[10].obj;
            AddScheme(scheme, true);

            AddScheme(new ColorScheme("White", "Black", "None", "None", "Hat_SpikeyHead"), true);

            AddScheme(new ColorScheme("Hot Pink", "Stone Skin", "Wings", "White", "Hat_Horns"), false);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[18].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[16].obj;
            scheme.SkinColor = (ClrTextFx)ColorList[15].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[17].obj;
            AddScheme(scheme, true);

            AddScheme(new ColorScheme("Brown", "Red", "AngryCape", "Brown", "Hat_FallingBlockHead"), true);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[7].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[6].obj;
            scheme.SkinColor = (ClrTextFx)ColorList[5].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[6].obj;
            AddScheme(scheme, true);

            AddScheme(new ColorScheme("White", "Black", "None", "None", "Hat_Bubble"), true);

            AddScheme(new ColorScheme("White", "Rainbow Skin", "None", "None", "Hat_Cloud"), false);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)ColorList[7].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[8].obj;
            scheme.SkinColor = (ClrTextFx)ColorList[8].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[7].obj;
            AddScheme(scheme, true);

            //AddScheme(new ColorScheme("Black", "Gold", "Yellow", "Black", "None"));

            AddScheme(new ColorScheme("Black", "Stone Skin", "Tiger Skin", "Black", "Hat_Viking"), true);
            AddScheme(new ColorScheme("Black", "White", "Red", "Black", ""), false);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[4].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[13].obj;
            scheme.SkinColor = (ClrTextFx)TextureList[2].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[3].obj;
            AddScheme(scheme, true);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[13].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[13].obj;
            scheme.SkinColor = (ClrTextFx)TextureList[3].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[3].obj;
            scheme.HatData = HatInfo[3];
            AddScheme(scheme, true);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[0].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[0].obj;
            scheme.SkinColor = (ClrTextFx)TextureList[4].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[3].obj;
            scheme.HatData = HatInfo[4];
            AddScheme(scheme, true);

            AddScheme(new ColorScheme("Red", "Yellow", "Red", "Yellow", "Hat_CheckpointHead"), true);

            scheme = new ColorScheme();
            scheme.Init();
            scheme.CapeColor = (ClrTextFx)CapeColorList[0].obj;
            scheme.CapeOutlineColor = (ClrTextFx)CapeOutlineColorList[0].obj;
            scheme.SkinColor = (ClrTextFx)TextureList[5].obj;
            scheme.OutlineColor = (ClrTextFx)ColorList[3].obj;
            scheme.HatData = HatInfo[3];
            AddScheme(scheme, true);

            AddScheme(new ColorScheme("Black", "Clear", "None", "None", "None"), false);

            AddScheme(new ColorScheme("Black", "Stone Skin", "DWings", "White", "Hat_Horns"), false);

            AddScheme(new ColorScheme("None", "Clear", "Red", "Red", "None"), false);
        }
    }
}