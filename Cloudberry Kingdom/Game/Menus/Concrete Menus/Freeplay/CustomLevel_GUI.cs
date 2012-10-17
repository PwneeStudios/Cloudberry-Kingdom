using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class CustomLevel_GUI : CkBaseMenu
    {
        public static List<TileSet> FreeplayTilesets = new List<TileSet>();
        public static List<BobPhsx> FreeplayHeroes = new List<BobPhsx>();

        public static bool IsMaxLength = false;
        public static int Difficulty = 0;

        /// <summary>
        /// This is the level seed being edited.
        /// </summary>
        public LevelSeedData LevelSeed;
        public PieceSeedData PieceSeed;

        public ObjectIcon HeroIcon, MiniCheckpoint;

        //static string CustomHeroString = "Custom";
        static string CustomHeroString = "Factory";
        
        public CustomLevel_GUI()
        {
            NoBackIfNoCaller = true;

            SeedStringToLoad = null;
            ExitFreeplay = false;
        }

        public void StartLevelFromMenuData()
        {
            LevelSeedData data = new LevelSeedData(LevelSeed);
            data.Seed = Tools.GlobalRnd.Rnd.Next();

            // Debug: set hero type
            //data.DefaultHeroType = BobPhsx.MakeCustom(Hero_BaseType.Spaceship, Hero_Shape.Small, Hero_MoveMod.Classic);

            // Vertical levels have no checkpoints
            if (LevelSeed.MyGeometry != LevelGeometry.Right)
                LevelSeed.NumPieces = 1;

            // Copy Upgrade1 to Upgrade2
            PieceSeed.MyUpgrades1.UpgradeLevels.CopyTo(PieceSeed.MyUpgrades2.UpgradeLevels, 0);

            // Custom difficulty
            if (IsCustomDifficulty())
            {
                data.Initialize(piece =>
                {
                    piece.CopyUpgrades(PieceSeed);

                    piece.StandardClose();
                });
            }
            // Preset difficulty
            else
            {
                int diff = new int[] { 0, 2, 4, 6, 9 }[DiffList.ListIndex];
                LevelSeedData.CustomDifficulty custom;
                
                if (data.DefaultHeroType is BobPhsxSpaceship)
                    custom = SpaceshipLevel.StandardPieceMod(diff, data);
                else
                    custom = RegularLevel.FixedPieceMod(DiffList.ListIndex - 1, data);

                LevelSeedData.CustomDifficulty modcustom = p =>
                {
                    custom(p);

                    p.StandardClose();
                };

                data.Initialize(custom);
            }

            // Wall
            if (HasWall)
            {
                //p.Style.ComputerWaitLengthRange = new Vector2(4, 23);
                var p = data.PieceSeeds[0];
                p.Style.MyModParams =
                    (level, piece) =>
                    {
                        var Params = (NormalBlock_Parameters)piece.Style.FindParams(NormalBlock_AutoGen.Instance);
                        Wall wall = Params.SetWall(LevelGeometry.Right);
                        wall.Space = 20; wall.MyBufferType = Wall.BufferType.Space;
                        p.CamZoneStartAdd.X = -2000;
                        wall.StartOffset = -600;
                        wall.Speed = 17.5f;
                        wall.InitialDelay = 72;
                    };
            }

            // Dark bottom
            if (data.MyGeometry == LevelGeometry.Down)
                data.PieceSeeds[0].Style.MyFinalPlatsType = StyleData.FinalPlatsType.DarkBottom;

            data.PostMake = data.PostMake_StandardLoad;

            StartLevel(data);
        }

        public void StartLevel(LevelSeedData data)
        {
            data.PostMake += data.PostMake_EnableLoad;

            PlayerManager.CoinsSpent = -999;

            GameData game = data.Create();
            game.ParentGame = MyGame;
            game.Freeplay = true;
            game.DefaultHeroType = data.DefaultHeroType;
            Difficulty = DiffList.ListIndex - 1;
            IsMaxLength = length.IsMaxed;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            MyPile.Pos += RightPanelCenter;
            MyMenu.FancyPos.RelVal += RightPanelCenter;

            // Register for when the created level ends
            MyGame.OnReturnTo += OnReturnFromLevel;

            SetPos();
        }

        protected override void ReleaseBody()
        {
            // Unregister 
            MyGame.OnReturnTo -= OnReturnFromLevel;

            base.ReleaseBody();
        }

        void OnReturnFromLevel()
        {
            if (ExitFreeplay)
            {
                if (CallingPanel != null)
                {
                    SlideOut(PresetPos.Left);
                    CallingPanel.SlideOut(PresetPos.Left);
                    CallingPanel.ReleaseWhenDone = true;
                }

                MenuReturnToCaller(MyMenu);
                MyGame.PhsxStepsToDo += 20;
                ExitFreeplay = false;
                return;
            }

            // If we started the level from the basic menu
            if (CallingPanel == null)
            {
                if (Active)
                {
                    SlideOut(PresetPos.Right, 0);
                    SlideIn();
                }
            }
            // If we started the level from the obstacle upgrade menu
            else
            {
                SlideOut(PresetPos.Right, 0);
                CallingPanel.SlideOut(PresetPos.Right, 0);
                CallingPanel.SlideIn();
                Active = false;
            }
        }

        void AnyHero()
        {
            foreach (var item in HeroList.MyList)
                item.Selectable = true;
        }

        void UpHero_ModShown()
        {
            foreach (var item in HeroList.MyList)
            {
                if (item.MyObject is BobPhsxSpaceship || item.MyObject is BobPhsxRocketbox)
                    item.Selectable = false;
                else
                    item.Selectable = true;
            }
        }

        void UpHero_NoSpaceship()
        {
            if (LevelSeed.DefaultHeroType == BobPhsxSpaceship.Instance ||
                LevelSeed.DefaultHeroType == BobPhsxRocketbox.Instance)
            {
                int HoldIndex = DesiredHeroIndex;
                HeroList.SetIndex(1);
                if (HeroList.Show)
                    DesiredHeroIndex = HoldIndex;
            }
        }

        void ShowHeros(bool Show)
        {
            HeroList.Include = Show;
            HeroText.Show = Show;

            if (Show)
                HeroList.SetIndex(DesiredHeroIndex);
            else
            {
                int HoldIndex = DesiredHeroIndex;
                HeroList.SetIndex(0);
                DesiredHeroIndex = HoldIndex;
            }
        }

        float HoldNumCheckpoints = 1;
        int HoldDesiredNumCheckpoints;
        void ShowCheckpoints(bool Show)
        {
            if (Show)
            {
                if (!checkpoints.Show)
                {
                    checkpoints.Val = HoldNumCheckpoints;
                    DesiredNumCheckpoints = HoldDesiredNumCheckpoints;
                }
            }
            else
            {
                HoldNumCheckpoints = checkpoints.Val;
                HoldDesiredNumCheckpoints = DesiredNumCheckpoints;
                checkpoints.Val = 0;
                DesiredNumCheckpoints = 0;
            }

            checkpoints.Show = Show;
            CheckpointsText.Show = Show;
        }

        bool HasWall = false;
        void SelectNormal()
        {
            HasWall = false;

            AnyHero();
            ShowHeros(true);
            
            ShowCheckpoints(true);
        }

        void SelectBuild()
        {
            HasWall = false;

            ShowHeros(false);
            ShowCheckpoints(true);
        }

        void SelectBungee()
        {
            SelectNormal();
        }

        void SelectSurvival()
        {
            SelectNormal();
            ShowCheckpoints(false);
        }

        bool LeftJustify = true;
        float LeftJustifyAddX = -400;

        //static Vector2 RightPanelCenter = new Vector2(-410, 75);
        static Vector2 RightPanelCenter = new Vector2(-285, 0);
        LengthSlider length;
        MenuSliderBase checkpoints;
        MenuItem Start;
        MenuList HeroList, DiffList;
        EzText HeroText, CheckpointsText;
        public override void Init()
        {
            ItemShadows = false;

            LevelSeed = new LevelSeedData();
            PieceSeed = new PieceSeedData(null);

            SlideInFrom = SlideOutTo = PresetPos.Right;

            FontScale = .73f;

            MyPile = new DrawPile();

            base.Init();

            ReturnToCallerDelay = 18;
            SlideInLength = 25;
            SlideOutLength = 24;

            CallDelay = 5;


            SelectedItemShift = new Vector2(0, 0);

            // Backdrop
            QuadClass backdrop;

            backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop);
            backdrop.Size =
                new Vector2(1690.477f, 1115.617f);
            backdrop.Pos =
                new Vector2(287.6977f, 51.58758f);

            // Make the menu
            MyMenu = new Menu(false);

            Control = -1;

            MyMenu.OnB = null;

            MenuItem item;

            // Location screenshot
            QuadClass Screenshot = new QuadClass();
            Screenshot.Name = "Screenshot";
            Screenshot.SetToDefault();
            Screenshot.TextureName = "Screenshot_Terrace";
            Screenshot.ScaleYToMatchRatio(500);
            MyPile.Add(Screenshot);
            Screenshot.Pos = new Vector2(1340.002f, 497.2222f);
            Screenshot.SetDefaultShadow(15);

            // Location
            EzText LocationText = new EzText("Location:", ItemFont);
            LocationText.Name = "Location";
            SetHeaderProperties(LocationText);
            MyPile.Add(LocationText);
            LocationText.Pos = new Vector2(-1050.111f, 933f);
            
            MenuList LocationList = new MenuList();
            LocationList.Name = "Location";
            LocationList.Center = !LeftJustify;
            LocationList.MyExpandPos = new Vector2(-498.1506f, 713.873f);
            foreach (TileSet tileset in FreeplayTilesets)
            {
                item = new MenuItem(new EzText(tileset.Name, ItemFont, false, true));
                SetItemProperties(item);
                LocationList.AddItem(item, tileset);
            }
            AddItem(LocationList);
            if (LeftJustify)
                LocationList.Pos = new Vector2(200f + LeftJustifyAddX, 828f);
            else
                LocationList.Pos = new Vector2(200f, 828f);
            LocationList.OnIndexSelect = () =>
                {
                    TileSet tileset = LocationList.CurObj as TileSet;

                    Vector2 HoldRelativeSize = Screenshot.GetTextureScaling();
                    Screenshot.TextureName = tileset.ScreenshotString;
                    Screenshot.ScaleYToMatchRatio(HoldRelativeSize.X * Screenshot.Quad.MyTexture.Width);

                    LevelSeed.SetTileSet(tileset);
                };
            LocationList.SetIndex(0);

            // Game type
            EzText GameText = new EzText("Game:", ItemFont);
            GameText.Name = "Game";
            SetHeaderProperties(GameText);
            MyPile.Add(GameText);
            GameText.Pos = new Vector2(-1061.11f, 933f - 222f);
            
            MenuList GameList = new MenuList();
            GameList.Name = "Game";
            GameList.MyExpandPos = new Vector2(-580, 500.873f);
            GameList.Center = !LeftJustify;
            string[] GameNames;
            if (PlayerManager.NumPlayers <= 1)
                //GameNames = new string[] { "Classic", "Build", "Up Level", "Down Level", "Wall Level" };
                GameNames = new string[] { "Classic", "Wall Level" };
            else
                //GameNames = new string[] { "Classic", "Build", "Bungee", "Up Level", "Down Level", "Wall Level" };
                GameNames = new string[] { "Classic", "Bungee", "Wall Level" };
            foreach (string name in GameNames)
            {
                item = new MenuItem(new EzText(name, ItemFont, false, true));
                SetItemProperties(item);
                GameList.AddItem(item, name);
            }
            AddItem(GameList);
            if (LeftJustify)
                GameList.Pos = new Vector2(117 + LeftJustifyAddX, 828f - 222f);
            else
                GameList.Pos = new Vector2(117, 828f - 222f);
            GameList.OnIndexSelect = () =>
                {
                    LevelSeed.MyGameFlags.SetToDefault();

                    string gamename = GameList.CurObj as string;
                    if (gamename.CompareTo("Classic") == 0)
                    {
                        LevelSeed.MyGameType = NormalGameData.Factory;
                        LevelSeed.MyGeometry = LevelGeometry.Right;
                        SelectNormal();
                    }
                    else if (gamename.CompareTo("Bungee") == 0)
                    {
                        LevelSeed.MyGameType = NormalGameData.Factory;
                        LevelSeed.MyGeometry = LevelGeometry.Right;
                        LevelSeed.MyGameFlags.IsTethered = true;
                        SelectBungee();
                    }
                    else if (gamename.CompareTo("Up Level") == 0)
                    {
                        SelectUpLevel();
                    }
                    else if (gamename.CompareTo("Down Level") == 0)
                    {
                        SelectDownLevel();
                    }
                    else if (gamename.CompareTo("Wall Level") == 0)
                    {
                        LevelSeed.MyGameType = NormalGameData.Factory;
                        LevelSeed.MyGeometry = LevelGeometry.Right;
                        SelectNormal();
                        ShowHeros(true);
                        ShowCheckpoints(false);
                        HasWall = true;
                    }
                };
            
            // Hero
            HeroText = new EzText("Hero:", ItemFont);
            HeroText.Name = "Hero";
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(-1044.443f, 933 - 2 * 222f);

            HeroList = new MenuList();
            HeroList.Name = "Hero";
            HeroList.Center = !LeftJustify;
            HeroList.MyExpandPos = new Vector2(-782.1666f, 293.6826f);
            foreach (BobPhsx hero in FreeplayHeroes)
            {
                item = new MenuItem(new EzText(hero.Name, ItemFont, false, true));
                item.MyObject = hero;
                SetItemProperties(item);
                HeroList.AddItem(item, hero);
            }
            // Random
            item = new MenuItem(new EzText("Random", ItemFont, false, true));
            SetItemProperties(item);
            HeroList.AddItem(item, BobPhsxRandom.Instance);
            // Custom
            item = new MenuItem(new EzText(CustomHeroString, ItemFont, false, true));
            SetItemProperties(item);
            HeroList.AddItem(item, null);

            AddItem(HeroList);
            if (LeftJustify)
                HeroList.Pos = new Vector2(117.2227f + LeftJustifyAddX - 150, 828f - 2 * 222f);
            else
                HeroList.Pos = new Vector2(117.2227f, 828f - 2 * 222f);
            HeroList.OnIndexSelect = HeroList_OnIndex;
            HeroList.SetIndex(0);

            // Difficulty
            EzText DiffText = new EzText("Difficulty:", ItemFont);
            DiffText.Name = "Diff";
            SetHeaderProperties(DiffText);
            MyPile.Add(DiffText);
            DiffText.Pos = new Vector2(-1233.889f, 40.55557f);

            string[] Names = CampaignHelper.DifficultyNames;
            DiffList = new MenuList();
            DiffList.Name = "Diff";
            DiffList.Center = !LeftJustify;
            DiffList.MyExpandPos = new Vector2(-519.6807f, -151.5238f);
            DiffList.DoIndexWrapping = false;
            for (int i = 0; i < 5; i++)
            {
                item = new MenuItem(new EzText(Names[i], ItemFont, false, true));
                SetItemProperties(item);
                DiffList.AddItem(item, Names[i]);
            }
            AddItem(DiffList);
            if (LeftJustify)
                DiffList.Pos = new Vector2(242.2246f + LeftJustifyAddX, -73.11105f);
            else
                DiffList.Pos = new Vector2(242.2246f, -73.11105f);
            DiffList.OnIndexSelect = () => { };

            DiffList.OnIndexSelect = DiffList_OnIndex;


            // Length
            EzText LengthText = new EzText("Length:", ItemFont);
            LengthText.Name = "Length";
            SetHeaderProperties(LengthText);
            MyPile.Add(LengthText);
            LengthText.Pos = new Vector2(-1224.999f, -191.6667f);

            length = new LengthSlider();
            length.Name = "Length";
            length.Go = null;
            AddItem(length);
            length.Pos = new Vector2(-283f, -556.1017f);

            //length.OnSetValue = () => LevelSeed.PieceLength = (int)(length.MyFloat.Val / LevelSeed.NumPieces);
            length.OnSetValue = () =>
                {
                    LevelSeed.Length = (int)(length.MyFloat.Val);
                    switch (LevelSeed.NumPieces)
                    {
                        case 1: LevelSeed.PieceLength = (int)(length.MyFloat.Val); break;
                        case 2: LevelSeed.PieceLength = (int)(length.MyFloat.Val * .7f); break;
                        case 3: LevelSeed.PieceLength = (int)(length.MyFloat.Val * .5f); break;
                        case 4: LevelSeed.PieceLength = (int)(length.MyFloat.Val * .4f); break;

                        default:
                            LevelSeed.PieceLength = (int)(length.MyFloat.Val / LevelSeed.NumPieces); break;
                    }
                };
            length.OnSlide = () =>
                {
                    DesiredLength = length.MyFloat.Val;

                    int MaxCheckpoints = Math.Min(4, (int)(length.MyFloat.Percent / 20));

                    float currentcheckpoints = checkpoints.MyFloat.Val;
                    currentcheckpoints = Math.Max(currentcheckpoints, DesiredNumCheckpoints);
                    currentcheckpoints = Math.Min(currentcheckpoints, MaxCheckpoints);
                    checkpoints.MyFloat.Val = currentcheckpoints;
                };

            // Mini checkpoints
            MiniCheckpoint = ObjectIcon.CheckpointIcon.Clone(ObjectIcon.IconScale.Widget);
            MiniCheckpoint.SetShadow(false);

            // Checkpoints
            CheckpointsText = new EzText("Checkpoints:", ItemFont);
            CheckpointsText.Name = "Checkpoints";
            SetHeaderProperties(CheckpointsText);
            MyPile.Add(CheckpointsText);
            CheckpointsText.Pos = new Vector2(-1008.33f, -661.1111f);

            checkpoints = new MenuSliderNoSlide(new EzText("x ", ItemFont));
            checkpoints.Name = "Checkpoints";
            checkpoints.MyFloat = new WrappedFloat(1, 0, 4);
            checkpoints.InitialSlideSpeed = 1;
            checkpoints.MaxSlideSpeed = 1;
            checkpoints.Discrete = true;
            checkpoints.SetToShowText();
            checkpoints.Go = null;
            checkpoints.SetIcon(ObjectIcon.CheckpointIcon.Clone());
            checkpoints.Icon.SetShadow(false);
            checkpoints.MyDrawLayer = 1;
            AddItem(checkpoints);
            checkpoints.Pos = checkpoints.SelectedPos = new Vector2(267f, -680.549f);
            checkpoints.Icon.Pos = new Vector2(-22.22266f, -41.66666f);
            checkpoints.OnSlide = () =>
                {
                    DesiredNumCheckpoints = (int)checkpoints.MyFloat.Val;

                    float MinLength = (.11f + .2f * (checkpoints.MyFloat.Val + 1)) * length.MyFloat.Spread;
                    length.MyFloat.Val = Math.Min(length.MyFloat.Val, DesiredLength);
                    length.MyFloat.Val = Math.Max(length.MyFloat.Val, MinLength);
                };

            checkpoints.OnSetValue = () =>
                {
                    int PrevNumPieces = LevelSeed.NumPieces;

                    LevelSeed.NumPieces = 1 + (int)checkpoints.MyFloat.Val;
                    length.Val = LevelSeed.Length;

                    if (PrevNumPieces != LevelSeed.NumPieces)
                        MiniCheckpoint.SetScale(.01f);
                };

            checkpoints.Val = DesiredNumCheckpoints = 1;
            length.Val = 8000;


            FontScale = 1f;

            // Start
            Start = item = new MenuItem(new EzText(ButtonString.Go(90) + " Start Level", ItemFont));
            Start.Name = "Start";
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(682.1445f, -238.8095f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
#if NOT_PC
            item.Selectable = false;
            item.Pos = new Vector2(721.8262f, -226.9048f);
#endif
            item.Go = Cast.ToItem(BringNext);
            item.ScaleText(.92f);

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = menu => { Start.Go(null); return true; };

            // Load
            MenuItem Load;
#if PC_VERSION
            Load = item = new MenuItem(new EzText("Load Level", ItemFont));
#else
            Load = item = new MenuItem(new EzText(ButtonString.Y(90) + " Load", ItemFont));
#endif
            Load.Name = "Load";
            Load.Go = me => BringLoad();
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(682.1445f, -238.8095f);
            //item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            //item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
#if NOT_PC
            item.Selectable = false;
            item.Pos = new Vector2(721.8262f, -226.9048f);
#endif
            item.ScaleText(.92f);

            // Back
            item = new MenuItem(new EzText(ButtonString.Back(90) + " Back", ItemFont));
            item.Name = "Back";
            AddItem(item);
            item.SelectSound = null;
            item.Go = me => ReturnToCaller();
            item.Pos = item.SelectedPos = new Vector2(922.9375f, -523.8096f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#if NOT_PC
            item.Selectable = false;
            item.Pos = new Vector2(958.6523f, -468.254f);
#endif
            item.ScaleText(.92f);

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
            GameList.SetIndex(0);
            DiffList.SetIndex(0);
            MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnY = BringLoad;
        }

        private void DiffList_OnIndex()
        {
            if (DiffList.ListIndex == 0)
                Start.SubstituteText(" Continue");
            else
                Start.SubstituteText(" Start");
        }

        void StartLevel()
        {
            CallingPanel = null;
            StartLevelFromMenuData();
        }

        private void HeroList_OnIndex()
        {
            DesiredHeroIndex = HeroList.ListIndex;

            BobPhsx Hero = HeroList.CurObj as BobPhsx;

            if (null != Hero)
            {
                LevelSeed.DefaultHeroType = Hero;
                HeroIcon = Hero.Icon.Clone();
                HeroIcon.SetScale(1.25f);

                if (LevelSeed.MyGeometry == LevelGeometry.Up || LevelSeed.MyGeometry == LevelGeometry.Down)
                    if (Hero == BobPhsxSpaceship.Instance || Hero == BobPhsxRocketbox.Instance)
                        HeroList.SetIndex(HeroList.ListIndex + 1);
            }
            else
                if (IsCustomHero())
                    HeroIcon = ObjectIcon.CustomIcon;

            HeroIcon.FancyPos.SetCenter(Pos);
            
            HeroIcon.Pos = new Vector2(1050.003f, 383.3334f);
            //HeroIcon.Pos = new Vector2(525.002f, 238.8889f);
        }

        private bool IsCustomHero()
        {
            return (string.Compare(HeroList.CurMenuItem.MyString, CustomHeroString) == 0);
        }

        public bool IsCustomDifficulty()
        {
            return DiffList.ListIndex == 0;
        }

        public void SetPos()
        {
#if PC_VERSION
            MenuItem _item;
            _item = MyMenu.FindItemByName("Location"); if (_item != null) { _item.SetPos = new Vector2(-66.66797f, 677.9999f); }
            _item = MyMenu.FindItemByName("Game"); if (_item != null) { _item.SetPos = new Vector2(-24.66602f, 475.4443f); }
            _item = MyMenu.FindItemByName("Hero"); if (_item != null) { _item.SetPos = new Vector2(-4.999969f, 284f); }
            _item = MyMenu.FindItemByName("Diff"); if (_item != null) { _item.SetPos = new Vector2(-41.10744f, 79.66678f); }
            _item = MyMenu.FindItemByName("Length"); if (_item != null) { _item.SetPos = new Vector2(-221.8892f, -342.2129f); }
            _item = MyMenu.FindItemByName("Checkpoints"); if (_item != null) { _item.SetPos = new Vector2(150.3331f, -602.7712f); }
            _item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(580.5577f, 4.047569f); }
            _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(773.0172f, -224.9205f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(850.7158f, -461.508f); }

            MyMenu.Pos = new Vector2(-285f, 0f);

            EzText _t;
            _t = MyPile.FindEzText("Location"); if (_t != null) { _t.Pos = new Vector2(-1002.89f, 802.4446f); }
            _t = MyPile.FindEzText("Game"); if (_t != null) { _t.Pos = new Vector2(-844.444f, 619.3334f); }
            _t = MyPile.FindEzText("Hero"); if (_t != null) { _t.Pos = new Vector2(-780.5543f, 419.5557f); }
            _t = MyPile.FindEzText("Diff"); if (_t != null) { _t.Pos = new Vector2(-1033.889f, 212.7778f); }
            _t = MyPile.FindEzText("Length"); if (_t != null) { _t.Pos = new Vector2(-1166.665f, 36.11107f); }
            _t = MyPile.FindEzText("Checkpoints"); if (_t != null) { _t.Pos = new Vector2(-1177.774f, -463.8889f); }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }
            _q = MyPile.FindQuad("Screenshot"); if (_q != null) { _q.Pos = new Vector2(1340.002f, 497.2222f); _q.Size = new Vector2(10.81731f, 13.22116f); }

            MyPile.Pos = new Vector2(-285f, 0f);

#else
            MenuItem _item;
            _item = MyMenu.FindItemByName("Location"); if (_item != null) { _item.SetPos = new Vector2(-66.66797f, 677.9999f); }
            _item = MyMenu.FindItemByName("Game"); if (_item != null) { _item.SetPos = new Vector2(-24.66602f, 475.4443f); }
            _item = MyMenu.FindItemByName("Hero"); if (_item != null) { _item.SetPos = new Vector2(-4.999969f, 284f); }
            _item = MyMenu.FindItemByName("Diff"); if (_item != null) { _item.SetPos = new Vector2(-41.10744f, 79.66678f); }
            _item = MyMenu.FindItemByName("Length"); if (_item != null) { _item.SetPos = new Vector2(-221.8892f, -342.2129f); }
            _item = MyMenu.FindItemByName("Checkpoints"); if (_item != null) { _item.SetPos = new Vector2(150.3331f, -602.7712f); }
            _item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(708.3351f, 4.047569f); }
            _item = MyMenu.FindItemByName("Load"); if (_item != null) { _item.SetPos = new Vector2(839.6832f, -241.5872f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(975.7158f, -483.7302f); }

            MyMenu.Pos = new Vector2(-285f, 0f);

            EzText _t;
            _t = MyPile.FindEzText("Location"); if (_t != null) { _t.Pos = new Vector2(-1002.89f, 802.4446f); }
            _t = MyPile.FindEzText("Game"); if (_t != null) { _t.Pos = new Vector2(-844.444f, 619.3334f); }
            _t = MyPile.FindEzText("Hero"); if (_t != null) { _t.Pos = new Vector2(-780.5543f, 419.5557f); }
            _t = MyPile.FindEzText("Diff"); if (_t != null) { _t.Pos = new Vector2(-1033.889f, 212.7778f); }
            _t = MyPile.FindEzText("Length"); if (_t != null) { _t.Pos = new Vector2(-1166.665f, 36.11107f); }
            _t = MyPile.FindEzText("Checkpoints"); if (_t != null) { _t.Pos = new Vector2(-1177.774f, -463.8889f); }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }
            _q = MyPile.FindQuad("Screenshot"); if (_q != null) { _q.Pos = new Vector2(1340.002f, 497.2222f); _q.Size = new Vector2(10.81731f, 13.22116f); }

            MyPile.Pos = new Vector2(-285f, 0f);
#endif
        }

        private void SelectUpLevel()
        {
            LevelSeed.MyGameType = NormalGameData.Factory;
        
            if (LevelSeed.MyGeometry != LevelGeometry.Up && LevelSeed.MyGeometry != LevelGeometry.Down)
                SelectNormal();

            LevelSeed.MyGeometry = LevelGeometry.Up;
            UpHero_NoSpaceship();
            UpHero_ModShown();

            ShowCheckpoints(false);
        }

        private void SelectDownLevel()
        {
            LevelSeed.MyGameType = NormalGameData.Factory;

            if (LevelSeed.MyGeometry != LevelGeometry.Up && LevelSeed.MyGeometry != LevelGeometry.Down)
                SelectNormal();

            LevelSeed.MyGeometry = LevelGeometry.Down;
            UpHero_NoSpaceship();
            UpHero_ModShown();

            ShowCheckpoints(false);
        }

        /// <summary>
        /// The panel that actually starts the level, when it is started.
        /// </summary>
        public GUI_Panel CallingPanel;

        void BringNext()
        {
            if (IsCustomHero())
                BringHero();
            else if (IsCustomDifficulty())
                BringUpgrades();
            else
                StartLevel();
        }

        public override void Show()
        {
            base.Show();
        }

        CustomHero_GUI HeroGui;
        void BringHero()
        {
            HeroGui = new CustomHero_GUI(this);
            Call(HeroGui, 0);
            Hide(PresetPos.Left);
            this.SlideInFrom = PresetPos.Left;
        }

        void BringUpgrades()
        {
            CallingPanel = new PassiveUpgrades_GUI(PieceSeed, this);
            Call(CallingPanel, 0);
            Hide(PresetPos.Left);
            this.SlideInFrom = PresetPos.Left;
        }

        void BringLoad()
        {
            // Create the load menu
            //SavedSeedsGUI LoadMenu = new SavedSeedsGUI();
            //Call(LoadMenu, 0);
            SaveLoadSeedMenu menu = new SaveLoadSeedMenu(MenuItem.ActivatingPlayer, true, false);
            Call(menu, 0);

            Hide(PresetPos.Left);
            this.SlideInFrom = PresetPos.Left;
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

            MyMenu.Show = true;
            CallingPanel = null;
        }

        int DesiredNumCheckpoints = 0;
        float DesiredLength = 0;
        int DesiredHeroIndex = 0;
        protected override void MyDraw()
        {
            if (!Active && CallingPanel != null && !CallingPanel.Pos.Playing)
                MyMenu.Show = false;
            else
                MyMenu.Show = true;
 
            base.MyDraw();

            if (!MyMenu.Show)
            {
                MyMenu.CurDrawLayer = 0;
                MyMenu.PosOffset = MyMenu.FancyPos.Update();
                length.PosOffset = MyMenu.PosOffset;
                length.Draw();
            }

            if (HeroIcon != null)
                HeroIcon.Draw(false);
 
            int NumCheckpoints = (int)checkpoints.MyFloat.Val;

            MiniCheckpoint.SetScale(.8f * MiniCheckpoint.PrevSetRatio + .2f);
            float step = (length.Slider_TR.X - length.BL.X) / (NumCheckpoints + 1);
            for (int i = 1; i <= NumCheckpoints; i++)
            {
                MiniCheckpoint.Pos = new Vector2(length.BL.X + i * step + 15, length.TR.Y);
                MiniCheckpoint.Draw(false);
            }

        }

        public static string SeedStringToLoad = null;
        public static bool ExitFreeplay = false;
        protected override void MyPhsxStep()
        {
            if (ExitFreeplay) return;

            if (SeedStringToLoad != null)
            {
                string seed = SeedStringToLoad;
                SeedStringToLoad = null;
                
                SavedSeedsGUI.LoadSeed(seed, this);
                return;
            }

            base.MyPhsxStep();
        }
    }
}