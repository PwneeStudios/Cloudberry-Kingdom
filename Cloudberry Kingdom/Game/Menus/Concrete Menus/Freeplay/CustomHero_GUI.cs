using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Menus;

namespace CloudberryKingdom
{
    public class CustomHero_GUI : CkBaseMenu
    {
        public static BobPhsx Hero;
        BobPhsxNormal NormalHero { get { return Hero as BobPhsxNormal; } }

        protected CustomLevel_GUI CustomLevel;
        public CustomHero_GUI(CustomLevel_GUI CustomLevel)
        {
            this.CustomLevel = CustomLevel;

            CustomLevel.CallingPanel = this;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            MyGame.ForceTargetZoom = .001f;
            MyGame.DoForceZoom = true;
            MyGame.ForceLevelZoomBeforeDraw = .0007f;
        }

        protected override void ReleaseBody()
        {
            MyGame.ForceTargetZoom = .0007f;
            base.ReleaseBody();
        }

        PhsxSlider GravitySlider, MaxSpeedSlider, AccelSlider;
        PhsxSlider MaxFallSpeedSlider, JumpLengthSlider, JumpAccelSlider, DoubleJumpLengthSlider, DoubleJumpAccelSlider, FrictionSlider, JetPackSlider, JetPackFuelSlider, NumJumpsSlider;
        PhsxSlider SizeSlider, PhasedSizeSlider, PhasedGravitySlider, PhasePeriodSlider;
        private void MakeSliders()
        {
            ItemPos = new Vector2(-583.4305f, 698.1083f);
            PosAdd = new Vector2(0, -93.45f);

            PhsxSlider.Font = ItemFont;
            PhsxSlider.Process = AddItem;

            AccelSlider = new PhsxSlider("Accel", BobPhsx.CustomData.accel);
            MaxSpeedSlider = new PhsxSlider("Max vel", BobPhsx.CustomData.maxspeed);
            SizeSlider = new PhsxSlider("Size", BobPhsx.CustomData.size);
            GravitySlider = new PhsxSlider("Gravity", BobPhsx.CustomData.gravity);
            MaxFallSpeedSlider = new PhsxSlider("Max fall", BobPhsx.CustomData.maxfall);
            FrictionSlider = new PhsxSlider("Friction", BobPhsx.CustomData.friction);
            JumpLengthSlider = new PhsxSlider("Jump length", BobPhsx.CustomData.jumplength);
            JumpAccelSlider = new PhsxSlider("Jump accel", BobPhsx.CustomData.jumpaccel);

            NumJumpsSlider = new PhsxSlider("Num jump", BobPhsx.CustomData.numjumps);
            DoubleJumpLengthSlider = new PhsxSlider("Double jump length", BobPhsx.CustomData.jumplength2);
            DoubleJumpAccelSlider = new PhsxSlider("Doule jump accel", BobPhsx.CustomData.jumpaccel2);

            JetPackSlider = new PhsxSlider("Jetpack accel", BobPhsx.CustomData.jetpackaccel);
            JetPackFuelSlider = new PhsxSlider("Jetpack fuel", BobPhsx.CustomData.jetpackfuel);

            PhasedSizeSlider = new PhsxSlider("Phased size", BobPhsx.CustomData.size2);
            PhasedGravitySlider = new PhsxSlider("Phased gravity", BobPhsx.CustomData.gravity2);
            PhasePeriodSlider = new PhsxSlider("Phase period", BobPhsx.CustomData.phaseperiod);
        }

        public void StartTest()
        {
            Hide();

            MakeBobPhsx();

            CreateHeroes();
            
            RemovePreviousGround();
            CreateGround();

            Testing = true;

            var custom = CustomLevel as StartMenu_MW_CustomLevel;
            custom.Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.None);
        }

        bool Testing = false;
        void TestingPhsx()
        {
            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
                EndTest();
        }

        void EndTest()
        {
            var custom = CustomLevel as StartMenu_MW_CustomLevel;
            custom.Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);

            Testing = false;
            Show();

            foreach (Bob bob in MyGame.MyLevel.Bobs)
            {
                MyGame.MyLevel.AddPop(bob.Pos);
                bob.CollectSelf();
            }

            MyGame.AddToDo((Func<bool>)MoveBlockAndKill, "MoveOut", true, true);
        }

        bool MoveBlockAndKill()
        {
            if (MyGame == null || MyGame.MyLevel == null) return true;

            foreach (BlockBase block in MyGame.MyLevel.Blocks)
                block.Move(new Vector2(0, -20));
            return false;
        }

        void KillBobs()
        {
            MyGame.WaitThenDo(20, () =>
            {
                foreach (Bob bob in MyGame.MyLevel.Bobs)
                    bob.Die(Bob.BobDeathType.None);
            }, "RemoveBobs", false, true);
        }

        void MakeBobPhsx()
        {
            // Error check
            if (Base is BobPhsxSpaceship)
            {
                Jump = BobPhsxNormal.Instance;
            }

            // Make the hero
            Hero = MyGame.MyLevel.DefaultHeroType = BobPhsx.MakeCustom(Base, Size, Jump, BobPhsxNormal.Instance);

            var data = new BobPhsx.CustomPhsxData();
            data.Init(GravitySlider.Val, AccelSlider.Val, MaxSpeedSlider.Val, MaxFallSpeedSlider.Val, JumpLengthSlider.Val, DoubleJumpLengthSlider.Val, JumpAccelSlider.Val, DoubleJumpAccelSlider.Val, JetPackSlider.Val, JetPackFuelSlider.Val, NumJumpsSlider.Val, SizeSlider.Val, PhasedSizeSlider.Val, PhasedGravitySlider.Val, PhasePeriodSlider.Val, FrictionSlider.Val);
            Hero.SetCustomPhsx(data);
        }


        void CreateHeroes()
        {
            // Remove any previous bobs
            MyGame.KillToDo("RemoveBobs");
            foreach (Bob bob in MyGame.MyLevel.Bobs)
                bob.CollectSelf();

            // Make new bobs
            MyGame.MakeBobs(MyGame.MyLevel);
            
            // Position bobs
            Vector2 shift = new Vector2(-700, 100);
            foreach (Bob bob in MyGame.MyLevel.Bobs)
            {
                Tools.MoveTo(bob, MyGame.CamPos + shift);
                shift += new Vector2(100, 20);

                bob.ScreenWrap = true;
            }
        }

        void RemovePreviousGround()
        {
            MyGame.KillToDo("MoveOut");

            foreach (BlockBase block in MyGame.MyLevel.Blocks)
                block.CollectSelf();
        }

        void CreateGround()
        {
            MyGame.MyLevel.MyTileSet = "castle";

            NormalBlock block;

            foreach (NormalBlock _block in MyGame.MyLevel.Blocks)
                if (_block is NormalBlock)
                    _block.CollectSelf();

            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(new Vector2(-1000, -3100), new Vector2(1100, 2500), MyGame.MyLevel.MyTileSetInfo);
            block.MyBox.TopOnly = false;
            MyGame.MyLevel.AddBlock(block);

            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(new Vector2(1050, -2950), new Vector2(1000, 2500), MyGame.MyLevel.MyTileSetInfo);
            block.MyBox.TopOnly = false;
            MyGame.MyLevel.AddBlock(block);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.ShadowColor = new Color(.2f, .2f, .2f, .6f);
            text.Shadow = true;

            //text.Angle = CoreMath.Radians(23);
        }

        void SetSuperHeader(EzText text)
        {
            base.SetHeaderProperties(text);
            text.MyFloatColor = new Vector4(1, 1, 1, 1);
            text.Scale = FontScale * 1.42f;
            
            text.Shadow = false;

            CkColorHelper._x_x_HappyBlueColor(text); 
            text.Scale *= 1.25f;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);
            StartMenu.SetItemProperties_Red(item);
        }

        MenuList MakeList()
        {
            var list = new MenuList();

            return list;
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            MenuList list = item as MenuList;
            if (null != list)
            {
                list.SetIndex(0);
            }
        }

        BobPhsx Base, Jump, Size;

        MenuItem Start, Back;
        EzText HeroText;
        MenuList BaseList, JumpList, SizeList;
        EzText BaseHeader, JumpHeader, SizeHeader;

        void SetListItemProperties(MenuItem item)
        {
            SetItemProperties(item);
        }

        public override void Init()
        {
            ItemShadows = false;

            SlideInFrom = SlideOutTo = PresetPos.Right;

            FontScale = .5f;

            MyPile = new DrawPile();
            MyMenu = new Menu(false);
            EnsureFancy();

            base.Init();

            Control = -1;
            MyMenu.OnB = null;

            ReturnToCallerDelay = 18;
            SlideInLength = 25;
            SlideOutLength = 24;

            CallDelay = 5;

            SelectedItemShift = new Vector2(0, 0);

            // Header
            EzText HeaderText = new EzText("Hero Factory!", Tools.Font_Grobold42);
            HeaderText.Name = "Header";
            SetSuperHeader(HeaderText);
            HeaderText.Pos = new Vector2(-1169.842f, 985.7144f);
            MyPile.Add(HeaderText);

            // Backdrop
            var backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop);
            backdrop.Pos =
                new Vector2(0, 0);

            MenuItem item;

            // Hero lists
            BaseHeader = HeroText = new EzText("base:", ItemFont);
            HeroText.Name = "base";
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);

            BaseList = MakeList();
            BaseList.Name = "base";
            foreach (Hero_BaseType _hero in Tools.GetValues<Hero_BaseType>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = new MenuItem(new EzText(hero.Name, ItemFont, false, true));
                SetListItemProperties(item);
                BaseList.AddItem(item, hero);
            }
            AddItem(BaseList);

            // Hero jump
            JumpHeader = HeroText = new EzText("jump:", ItemFont);
            HeroText.Name = "jump";
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);

            JumpList = MakeList();
            JumpList.Name = "jump";
            foreach (Hero_MoveMod _hero in Tools.GetValues<Hero_MoveMod>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = new MenuItem(new EzText(hero.Name, ItemFont, false, true));
                SetListItemProperties(item);
                JumpList.AddItem(item, hero);
            }
            AddItem(JumpList);

            // Hero shape
            SizeHeader = HeroText = new EzText("size:", ItemFont);
            HeroText.Name = "size";
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            
            SizeList = MakeList();
            SizeList.Name = "size";
            foreach (Hero_Shape _hero in Tools.GetValues<Hero_Shape>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = new MenuItem(new EzText(hero.Name, ItemFont, false, true));
                SetListItemProperties(item);
                SizeList.AddItem(item, hero);
            }
            
            AddItem(SizeList);

            SetListActions();
            BaseList.SetIndex(0);
            SizeList.SetIndex(0);
            JumpList.SetIndex(0);

            FontScale = 1f;

            MakeSliders();

            // Start/test/back/advanced
            MakeOptionItems();

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
            MyMenu.OnB = MenuReturnToCaller;

            SetPos();
        }

        private void SetListActions()
        {
            BaseList.OnIndexSelect = () => Base = BaseList.CurObj as BobPhsx;
            SizeList.OnIndexSelect = () => Size = SizeList.CurObj as BobPhsx;
            JumpList.OnIndexSelect = () => Jump = JumpList.CurObj as BobPhsx;
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("base"); if (_item != null) { _item.SetPos = new Vector2(-1655.38f, 642.6317f); _item.MyText.Scale = 0.5f; _item.MySelectedText.Scale = 0.5f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("jump"); if (_item != null) { _item.SetPos = new Vector2(-1653.446f, 451.6321f); _item.MyText.Scale = 0.5f; _item.MySelectedText.Scale = 0.5f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("size"); if (_item != null) { _item.SetPos = new Vector2(-1656.223f, 256.2355f); _item.MyText.Scale = 0.5f; _item.MySelectedText.Scale = 0.5f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("test"); if (_item != null) { _item.SetPos = new Vector2(-1914.311f, 134.3449f); _item.MyText.Scale = 0.7685415f; _item.MySelectedText.Scale = 0.7685415f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("back"); if (_item != null) { _item.SetPos = new Vector2(-1909.15f, -90.52583f); _item.MyText.Scale = 0.6955291f; _item.MySelectedText.Scale = 0.6955291f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("continue"); if (_item != null) { _item.SetPos = new Vector2(-1915.104f, -288.0107f); _item.MyText.Scale = 0.7520385f; _item.MySelectedText.Scale = 0.7520385f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("reset"); if (_item != null) { _item.SetPos = new Vector2(-1916.694f, -502.1649f); _item.MyText.Scale = 0.7f; _item.MySelectedText.Scale = 0.7f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(1166.862f, -69.45605f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-664.2021f, 960.7101f); _t.Scale = 0.8010691f; }
            _t = MyPile.FindEzText("base"); if (_t != null) { _t.Pos = new Vector2(-1269.655f, 708.4517f); _t.Scale = 0.6189448f; }
            _t = MyPile.FindEzText("jump"); if (_t != null) { _t.Pos = new Vector2(-1270.534f, 507.2669f); _t.Scale = 0.5981081f; }
            _t = MyPile.FindEzText("size"); if (_t != null) { _t.Pos = new Vector2(-1234.811f, 317.9383f); _t.Scale = 0.6634525f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(0f, 0f); _q.Size = new Vector2(1500f, 1083.871f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }

        MenuItem A, B, X, Y;
        private void MakeOptionItems()
        {
            MenuItem item;

            float bigscale = .52f;
            float scale = .52f;

#if PC_VERSION
            bool WithButtonPics = false;
#else
            bool WithButtonPics = true;
#endif

            // Start
            if (WithButtonPics)
                A = Start = item = new MenuItem(new EzText(ButtonString.Go(90) + " test", ItemFont));
            else
                A = Start = item = new MenuItem(new EzText("Test", ItemFont));
            item.Name = "test";
            item.JiggleOnGo = false;
            AddItem(item);
            item.Go = Cast.ToItem(StartTest);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
#if NOT_PC
            item.Selectable = false;
#endif

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(Start.Go);


            // Back
            if (WithButtonPics)
                B = Back = item = new MenuItem(new EzText(ButtonString.Back(90) + " back", ItemFont));
            else
                B = Back = item = new MenuItem(new EzText("Back", ItemFont));
            item.Name = "back";
            AddItem(item);
            item.SelectSound = null;
            item.Go = Cast.ToItem(ReturnToCaller);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#if NOT_PC
            item.Selectable = false;
#endif

            // Continue
#if NOT_PC
            X = item = new MenuItem(new EzText(ButtonString.X(90) + " Continue", ItemFont));
#else
            if (WithButtonPics)
                X = item = new MenuItem(new EzText(ButtonString.X(90) + " play", ItemFont));
            else
                X = item = new MenuItem(new EzText("Play", ItemFont));
#endif
            item.Name = "continue";
            AddItem(item);
            item.SelectSound = null;
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
#if NOT_PC
            item.Selectable = false;
            MyMenu.OnX = Cast.ToMenu(X.Go);
#endif
            item.Go = Cast.ToItem(Next);

            // Reset
            item = ResetButton = new MenuItem(new EzText("Reset", ItemFont));
            item.Name = "reset";
            AddItem(item);
            item.Go = Cast.ToItem(ResetSliders);
        }

        bool AdvancedAvailable()
        {
            if (Base is BobPhsxSpaceship && !(Size is BobPhsxScale))
                return false;
            else
                return true;
        }

        void ResetAdvancedSliders()
        {
            MaxFallSpeedSlider.Show = JumpLengthSlider.Show = JumpAccelSlider.Show = FrictionSlider.Show = true;

            DoubleJumpAccelSlider.Show = DoubleJumpLengthSlider.Show = NumJumpsSlider.Show = Jump is BobPhsxDouble && Base is BobPhsxNormal;

            JetPackFuelSlider.Show = JetPackSlider.Show = Jump is BobPhsxJetman && Base is BobPhsxNormal;

            SizeSlider.Show = PhasedSizeSlider.Show = PhasedGravitySlider.Show = PhasePeriodSlider.Show = Size is BobPhsxScale;

            GravitySlider.Show = Base is BobPhsxNormal;
        }

        MenuItem ResetButton;

        void Next()
        {
            if (CustomLevel.IsCustomDifficulty())
            {
                MakeBobPhsx();
                CustomLevel.LevelSeed.DefaultHeroType = Hero;

                CustomLevel.CallingPanel = new PassiveUpgrades_GUI(CustomLevel.PieceSeed, CustomLevel);
                Call(CustomLevel.CallingPanel, 0);
                Hide(PresetPos.Left);
                this.SlideInFrom = PresetPos.Left;
            }
            else
            {
                MyGame.PlayGame(StartLevel);
            }
        }

        void StartLevel()
        {
            MakeBobPhsx();
            CustomLevel.LevelSeed.DefaultHeroType = Hero;
            CustomLevel.StartLevelFromMenuData();
        }

        void ResetSliders()
        {
            foreach (var item in MyMenu.Items)
            {
                PhsxSlider slider = item as PhsxSlider;
                if (null != slider)
                    slider.Reset();
            }
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }

        protected override void MyPhsxStep()
        {
            if (Testing)
            {
                TestingPhsx();
                return;
            }

            base.MyPhsxStep();
        }
    }
}