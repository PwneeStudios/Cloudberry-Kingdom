using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Obstacles;

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

        static MenuListItem _i(int Guid, int Price, Color Clr, Matrix M, Localization.Words Name)
        {
            return new MenuListItem
                (
                    new ClrTextFx(Guid, Price, Clr, M, Name),
                    Name
                );
        }

        static MenuListItem _i(int Guid, int Price, Color Clr, Color HighlightClr, Matrix M, Localization.Words Name)
        {
            return new MenuListItem
                (
                    new ClrTextFx(Guid, Price, Clr, HighlightClr, M, Name),
                    Name
                );
        }

        static MenuListItem _i(int Guid, int Price, Color Clr, Matrix M, CoreEffect Effect, Localization.Words Name)
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
            beard.Name = Localization.Words.Vandyke;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259001;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Beard = 
            beard = new Hat("Facial_Beard");
            beard.Name = Localization.Words.Rugged;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259002;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Mustache = 
            beard = new Hat("Facial_Moustache");
            beard.Name = Localization.Words.Manhattan;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259003;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.BigBeard = 
            beard = new Hat("Facial_BigBeard");
            beard.Name = Localization.Words.Lumberjack;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259004;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.Goatee = 
            beard = new Hat("Facial_Goatee");
            beard.Name = Localization.Words.Goatee;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259005;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);

            Hat.AhBeard =
            beard = new Hat("Facial_Ah_9");
            beard.Name = Localization.Words.Hat_Ah_9;
            beard.Price = Hat.Expensive;
            beard.Guid = 5259006;
            beard.HatPicShift = Vector2.Zero;
            BeardInfo.Add(beard);


            // Fill the hat list
            Hat hat;
            hat = new Hat("None");
            hat.Name = Localization.Words.None;
            hat.Guid = 19;
                HatInfo.Add(hat);
                Hat.None = hat;


            hat = new Hat("Hat_Viking");
            hat.Name = Localization.Words.Viking;
            hat.Price = Hat.Expensive;
            hat.Guid = 20;
                hat.HatPicShift = new Vector2(-.02f, .075f);
                HatInfo.Add(hat);
                Hat.Viking = hat;
            hat = new Hat("Hat_Fedora");
            hat.Name = Localization.Words.Fedora;
            hat.Price = Hat.Cheap;
            hat.Guid = 21;
                hat.HatPicScale *= 1.075f;
                HatInfo.Add(hat);                
                Hat.Fedora = hat;
            hat = new Hat("Hat_Afro");
            hat.Name = Localization.Words.Afro;
            hat.Price = Hat.Mid;
            hat.Guid = 22;
                hat.HatPicScale *= 1.065f;
                HatInfo.Add(hat);
                Hat.Afro = hat;
            hat = new Hat("Hat_Halo");
            hat.Name = Localization.Words.Halo;
            hat.Price = Hat.Mid;
            hat.Guid = 23;
                hat.HatPicScale *= 1.07f;
                hat.HatPicShift = new Vector2(.15f, .08f);
                HatInfo.Add(hat);
                Hat.Halo = hat;
            hat = new Hat("Hat_FireHead", false);
            hat.Name = Localization.Words.Firehead;
            hat.Price = Hat.Expensive;
            hat.Guid = 24;
                hat.HatPicTexture = Fireball.FlameTexture;
                hat.HatPicScale *= 1.8f;
                HatInfo.Add(hat);
                Hat.FireHead = hat;
            //hat = new Hat("Hat_Ghost");
            //hat.Name = Localization.Words.Ghost;
            //hat.Price = Hat.Cheap;
            //hat.Guid = 25;
            //    hat.HatPicScale *= .8f;
            //    HatInfo.Add(hat);
            //    Hat.Ghost = hat;
            //hat = new Hat("Hat_CheckpointHead", false);
            //hat.Price = Hat.Mid;
            //hat.Guid = 26;
            //    HatInfo.Add(hat);
            //    Hat.CheckpointHead = hat;
            hat = new Hat("Hat_Horns", true, false);
            hat.Name = Localization.Words.Horns;
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
            hat.Name = Localization.Words.Bubble;
            hat.DrawHead = false;
            hat.Price = Hat.Mid;
            hat.Guid = 36;
                HatInfo.Add(hat);
                Hat.Bubble = hat;


float ScaleNew = 1.35f;
float DefaultShiftX = -.35f;

            hat = new Hat("Hat_TopHat");
            hat.Name = Localization.Words.TopHat;
            hat.Price = Hat.Expensive;
            hat.Guid = 37;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.35f;
                HatInfo.Add(hat);
                Hat.TopHat = hat;
            hat = new Hat("Hat_Knight", false);
            hat.Name = Localization.Words.KnightHelmet;
            hat.Price = Hat.Expensive;
            hat.Guid = 38;
                hat.HatPicShift = new Vector2(DefaultShiftX, .105f);
                hat.HatPicScale *= ScaleNew * 1.145f;
                HatInfo.Add(hat);
                Hat.Knight = hat;
            
            // Removed due to infringement issues.
            /*
            hat = new Hat("Hat_Toad");
            hat.Name = Localization.Words.MushroomHat;
            hat.Price = Hat.Expensive;
            hat.Guid = 39;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.115f;
                HatInfo.Add(hat);
                Hat.Toad = hat;

            hat = new Hat("Hat_BubbleBobble");
            hat.Name = Localization.Words.OmNomNom;
            hat.Price = Hat.Expensive;
            hat.Guid = 40;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.105f;
                HatInfo.Add(hat);
                Hat.BubbleBobble = hat;
            */

            hat = new Hat("Hat_Brain");
            hat.DrawHead = false;
            hat.Name = Localization.Words.BrainHat;
            hat.Price = Hat.Expensive;
            hat.Guid = 41;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.095f;
                hat.AllowsFacialHair = false;
                HatInfo.Add(hat);
                Hat.Brain = hat;
            hat = new Hat("Hat_Gosu");
            hat.Name = Localization.Words.Gosu;
            hat.Price = Hat.Expensive;
            hat.Guid = 42;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                Hat.Gosu = hat;



            // Buyables
            Hat.RobinHood = 
            hat = new Hat("Hat_RobinHood");
            hat.Name = Localization.Words.RobinHood;
            hat.Price = Hat.Mid;
            hat.Guid = 1;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);

            Hat.Rasta = 
            hat = new Hat("Hat_Rasta");
            hat.Name = Localization.Words.Reggae;
            hat.Price = Hat.Mid;
            hat.Guid = 2;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Pumpkin = 
            hat = new Hat("Hat_Pumpkin");
            hat.DrawHead = false;
            hat.Name = Localization.Words.Pumpkin;
            hat.Price = Hat.Mid;
            hat.Guid = 3;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                hat.AllowsFacialHair = false;
                HatInfo.Add(hat);
                
            Hat.Pirate = 
            hat = new Hat("Hat_Pirate");
            hat.Name = Localization.Words.PirateHat;
            hat.Price = Hat.Expensive;
            hat.Guid = 4;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Miner = 
            hat = new Hat("Hat_Miner");
            hat.Name = Localization.Words.HardHat;
            hat.Price = Hat.Cheap;
            hat.Guid = 5;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Glasses = 
            hat = new Hat("Hat_Glasses");
            hat.Name = Localization.Words.FourEyes;
            hat.Price = Hat.Mid;
            hat.Guid = 6;
                hat.HatPicShift = new Vector2(DefaultShiftX, .1f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            
            Hat.BunnyEars = 
            hat = new Hat("Hat_BunnyEars");
            hat.Name = Localization.Words.BunnyEars;
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
            hat.Name = Localization.Words.Antlers;
            hat.Price = Hat.Mid;
            hat.Guid = 9;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.135f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Arrow = 
            hat = new Hat("Hat_Arrow");
            hat.Name = Localization.Words.ArrowThroughHead;
            hat.Price = Hat.Expensive;
            hat.Guid = 10;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Bag = 
            hat = new Hat("Hat_Bag");
            hat.Name = Localization.Words.BrownBag;
            hat.DrawHead = false;
            hat.Price = Hat.Cheap;
            hat.Guid = 11;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                hat.AllowsFacialHair = false;
                HatInfo.Add(hat);
                
            Hat.Cone = 
            hat = new Hat("Hat_Cone");
            hat.Name = Localization.Words.TrafficCone;
            hat.Price = Hat.Cheap;
            hat.Guid = 12;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Pope = 
            hat = new Hat("Hat_Pope");
            hat.Name = Localization.Words.PopeHat;
            hat.Price = Hat.Expensive;
            hat.Guid = 13;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Rice = 
            hat = new Hat("Hat_Rice");
            hat.Name = Localization.Words.RiceHat;
            hat.Price = Hat.Cheap;
            hat.Guid = 14;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Santa = 
            hat = new Hat("Hat_Santa");
            hat.Name = Localization.Words.SantaClaus;
            hat.Price = Hat.Expensive;
            hat.Guid = 15;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Sombrero = 
            hat = new Hat("Hat_Sombrero");
            hat.Name = Localization.Words.Sombrero;
            hat.Price = Hat.Cheap;
            hat.Guid = 16;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Tiki = 
            hat = new Hat("Hat_Tiki");
            hat.DrawHead = false;
            hat.Name = Localization.Words.TikiMask;
            hat.Price = Hat.Mid;
            hat.Guid = 17;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
                
            Hat.Wizard = 
            hat = new Hat("Hat_Wizard");
            hat.Name = Localization.Words.Wizard;
            hat.Price = Hat.Mid;
            hat.Guid = 18;
                hat.HatPicShift = new Vector2(DefaultShiftX, -.015f);
                hat.HatPicScale *= ScaleNew * 1.145f;
                HatInfo.Add(hat);

            Hat.Ah_1 = 
            hat = new Hat("Hat_Ah_1");
            hat.Name = Localization.Words.Hat_Ah_1;
            hat.Price = Hat.Expensive;
            hat.Guid = 12301;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_2 = 
            hat = new Hat("Hat_Ah_2");
            hat.Name = Localization.Words.Hat_Ah_2;
            hat.Price = Hat.Expensive;
            hat.Guid = 12302;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_3 = 
            hat = new Hat("Hat_Ah_3");
            hat.Name = Localization.Words.Hat_Ah_3;
            hat.Price = Hat.Expensive;
            hat.Guid = 12303;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_4 = 
            hat = new Hat("Hat_Ah_4");
            hat.Name = Localization.Words.Hat_Ah_4;
            hat.Price = Hat.Expensive;
            hat.Guid = 12304;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_5 = 
            hat = new Hat("Hat_Ah_5");
            hat.Name = Localization.Words.Hat_Ah_5;
            hat.Price = Hat.Expensive;
            hat.Guid = 12305;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_6 = 
            hat = new Hat("Hat_Ah_6");
            hat.Name = Localization.Words.Hat_Ah_6;
            hat.Price = Hat.Expensive;
            hat.Guid = 12306;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_7 = 
            hat = new Hat("Hat_Ah_7");
            hat.DrawHead = false;
            hat.Name = Localization.Words.Hat_Ah_7;
            hat.Price = Hat.Expensive;
            hat.Guid = 12307;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            Hat.Ah_8 = 
            hat = new Hat("Hat_Ah_8");
            hat.DrawHead = false;
            hat.Name = Localization.Words.Hat_Ah_8;
            hat.Price = Hat.Expensive;
            hat.Guid = 12308;
                hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
                hat.HatPicScale *= ScaleNew * 1.165f;
                HatInfo.Add(hat);
            //Hat.Ah_9 = 
            //hat = new Hat("Hat_Ah_9");
            //hat.AllowsFacialHair = false;
            //hat.Name = Localization.Words.ArrowThroughHead;
            //hat.Price = Hat.Expensive;
            //hat.Guid = 12301;
            //    hat.HatPicShift = new Vector2(DefaultShiftX, .075f);
            //    hat.HatPicScale *= ScaleNew * 1.165f;
            //    HatInfo.Add(hat);

                
#if DEBUG
            // Check GUID uniqueness
            if (!Tools.AllUnique(HatInfo, h => h.GetGuid()))
                Tools.Write("Duplicate hat guid!");
#endif

            for (int i = 0; i < HatInfo.Count; i++)
                HatList.Add(new MenuListItem(i, Localization.Words.None));

            
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
            ColorList.Add(_i(3500, 0, new Color(1f, 1f, 1f), ColorHelper.HsvTransform(1.25f, 0f, 0),    Localization.Words.White));    // 0
            ColorList.Add(_i(3501, 0, Color.Silver,          ColorHelper.HsvTransform(.85f, 0f, 0),     Localization.Words.Silver));   // 1
            ColorList.Add(_i(3502, 0, Color.Gray,            ColorHelper.HsvTransform(.525f, 0f, 0),    Localization.Words.Gray));     // 2
            ColorList.Add(_i(3503, 0, new Color(0f, 0f, 0f),
                                      new Color(50, 50, 50), ColorHelper.HsvTransform(.3f, 0f, 0),      Localization.Words.Black));    // 3

            ColorList.Add(_i(3504, 0, Color.Cyan,            ColorHelper.LinearColorTransform(45),      Localization.Words.Cyan));     // 4  
            ColorList.Add(_i(3505, 0, new Color(0f, 0f, 1f), ColorHelper.LinearColorTransform(120),     Localization.Words.Blue));     // 5
            ColorList.Add(_i(3506, 0, Color.DarkBlue,        ColorHelper.LinearColorTransform(80),      Localization.Words.Teal));     // 6
            ColorList.Add(_i(3507, 0, Color.Indigo,          ColorHelper.HsvTransform(.8f, 1.3f, 225),  Localization.Words.Indigo));   // 7
            ColorList.Add(_i(3508, 0, Color.Purple,          ColorHelper.HsvTransform(.85f, 1.1f, 205), Localization.Words.Purple));  // 8
            ColorList.Add(_i(3509, 0, Color.Brown,           ColorHelper.HsvTransform(1f, 1f, 80),      Localization.Words.Brown));        // 9
            ColorList.Add(_i(3510, 0, new Color(1f, 0, 0f),
                                      ColorHelper.HsvTransform(.95f, 1.3f, 0) * ColorHelper.LinearColorTransform(240), Localization.Words.Red)); // 10
            ColorList.Add(_i(3511, 0, Color.HotPink,         ColorHelper.HsvTransform(1.3f, 1.2f, 200), Localization.Words.HotPink));     // 11
            ColorList.Add(_i(3512, 0, new Color(1f, .6f, 0f),ColorHelper.HsvTransform(.9f, 1.3f, 110),  Localization.Words.Orange));        // 12
            ColorList.Add(_i(3513, 0, Color.Gold,            ColorHelper.HsvTransform(1.3f, 1.2f, 100), Localization.Words.Gold));         // 13
            ColorList.Add(_i(3514, 0, Color.Yellow,          ColorHelper.HsvTransform(1.5f, 1.5f, 100), Localization.Words.Yellow));       // 14
            ColorList.Add(_i(3515, 0, new Color(0f, 1f, 0f), ColorHelper.LinearColorTransform(0),       Localization.Words.Green));              // 15
            ColorList.Add(_i(3516, 0, Color.LimeGreen,       ColorHelper.HsvTransform(1.25f, 1.35f, 0), Localization.Words.LimeGreen));    // 16
            ColorList.Add(_i(3517, 0, Color.ForestGreen,     ColorHelper.HsvTransform(.75f, .8f, 0),    Localization.Words.ForestGreen));     // 17
            
            ColorList.Add(_i(3518, 0, ColorHelper.GrayColor(.2f), ColorHelper.HsvTransform(0, 0, 1),     Localization.Words.Ninja));     // 18
            ColorList.Add(_i(3519, 0, Color.White,                ColorHelper.HsvTransform(1.75f, 0, 1), Localization.Words.BrightWhite));     // 19

            

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
            MenuListItem NoTexture = _i(3524, 0, Color.Transparent, m, Localization.Words.Clear); // 6
            TextureList.Add(NoTexture);


            // Fill the cape color list
            ClrTextFx cape;
            None = new ClrTextFx(3525, 0, new Color(1f, 1f, 1f, 0f), Matrix.Identity);
            None.Name = Localization.Words.None;
            CapeColorList.Add(new MenuListItem(None, Localization.Words.None));
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
                CapeOutlineColorList.Add(new MenuListItem(capeoutline, item.word));
            }

            // Add textures to skin color list and cape color list
            ColorList.AddRange(TextureList);
            CapeColorList.AddRange(TextureList);
            CapeColorList.Remove(NoTexture);

            ClrTextFx fx;
            Vector2 fx_scale = new Vector2(.875f, 1.195f) * .98f;

            //fx = new ClrTextFx(3526, 500, Color.White, "FallingBlock2", false, "FallingBlock2");
            //fx.Name = "Concern;
            //fx.PicScale = fx_scale;
            //CapeColorList.Add(new MenuListItem(fx, "ConcernedCape"));

            //fx = new ClrTextFx(3527, 500, Color.White, "FallingBlock3", false, "FallingBlock3");
            //fx.Name = "AAAaahhh!;
            //fx.PicScale = fx_scale;
            //CapeColorList.Add(new MenuListItem(fx, "ScreamingCape"));

            //fx = new ClrTextFx(3528, 500, Color.White, "Capes\\FallingBlock4Cape", false, "FallingBlock4");
            //fx.Name = "Anger;
            //fx.PicScale = fx_scale * new Vector2(1.022f, 1.028f);
            //CapeColorList.Add(new MenuListItem(fx, "AngryCape"));

            // Wings and cape mod functions
            List<MenuListItem> NewCapeList = new List<MenuListItem>();
            foreach (MenuListItem item in CapeColorList)
            {
                cape = (ClrTextFx)item.obj;
                cape.ModObject = CapeOn;
                NewCapeList.Add(new MenuListItem(cape, item.word));
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
            AddScheme(new ColorScheme(Localization.Words.Black, Localization.Words.Red, Localization.Words.Red, Localization.Words.Hat_Ah_6, Localization.Words.Hat_Ah_9), true);
            AddScheme(new ColorScheme(Localization.Words.White, Localization.Words.BrightWhite, Localization.Words.BrightWhite, Localization.Words.Hat_Ah_4, Localization.Words.Hat_Ah_9), true);
            AddScheme(new ColorScheme(Localization.Words.Green, Localization.Words.LimeGreen, Localization.Words.Green, Localization.Words.Hat_Ah_3, Localization.Words.Vandyke), true);
            AddScheme(new ColorScheme(Localization.Words.Blue, Localization.Words.Red, Localization.Words.Brown, Localization.Words.Hat_Ah_5, Localization.Words.Vandyke), true);
            AddScheme(new ColorScheme(Localization.Words.White, Localization.Words.BrightWhite, Localization.Words.BrightWhite, Localization.Words.Hat_Ah_8, Localization.Words.Vandyke), true);
            

            AddScheme(new ColorScheme(Localization.Words.Green, Localization.Words.Red, Localization.Words.Black, Localization.Words.None, Localization.Words.None), true);
            AddScheme(new ColorScheme(Localization.Words.HotPink, Localization.Words.HotPink, Localization.Words.HotPink, Hat.BunnyEars.Name, Hat.Beard.Name), false);
            AddScheme(new ColorScheme(Localization.Words.Gold, Localization.Words.Gold, Localization.Words.Gold, Localization.Words.None, Localization.Words.None), false);
            AddScheme(new ColorScheme(Localization.Words.Purple, Localization.Words.Indigo, Localization.Words.HotPink, Localization.Words.None, Localization.Words.Vandyke), false);
            AddScheme(new ColorScheme(Localization.Words.ForestGreen, Localization.Words.Yellow, Localization.Words.Gold, Localization.Words.RiceHat, Localization.Words.Rugged), false);
            AddScheme(new ColorScheme(Localization.Words.Red, Localization.Words.None, Localization.Words.None, Localization.Words.Antlers, Localization.Words.Vandyke), false);
            AddScheme(new ColorScheme(Localization.Words.Gray, Localization.Words.Red, Localization.Words.Black, Localization.Words.None, Hat.Mustache.Name), false);
            AddScheme(new ColorScheme(Localization.Words.Indigo, Localization.Words.Cyan, Localization.Words.Silver, Localization.Words.Wizard, Localization.Words.Vandyke), false);
            AddScheme(new ColorScheme(Localization.Words.Ninja, Localization.Words.White, Localization.Words.Black, Localization.Words.Pumpkin, Localization.Words.Manhattan), false);
            AddScheme(new ColorScheme(Localization.Words.ForestGreen, Localization.Words.ForestGreen, Localization.Words.ForestGreen, Localization.Words.RobinHood, Localization.Words.Vandyke), false);
            AddScheme(new ColorScheme(Localization.Words.Silver, Localization.Words.Gray, Localization.Words.Gray, Localization.Words.KnightHelmet, Localization.Words.Lumberjack), false);
        }
    }
}