using System;
using Microsoft.Xna.Framework;






namespace CloudberryKingdom
{
    public class CustomHero_GUI : CkBaseMenu
    {
        public static BobPhsx.CustomPhsxData HeroPhsxData;

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
            PhsxSlider.Process = new AddItemProxy(this);

            AccelSlider = new PhsxSlider(Localization.Words.Acceleration, BobPhsx.CustomData.accel);
            MaxSpeedSlider = new PhsxSlider(Localization.Words.MaxVelocity, BobPhsx.CustomData.maxspeed);
            SizeSlider = new PhsxSlider(Localization.Words.Size, BobPhsx.CustomData.size);
            GravitySlider = new PhsxSlider(Localization.Words.Gravity, BobPhsx.CustomData.gravity);
            MaxFallSpeedSlider = new PhsxSlider(Localization.Words.MaxFallSpeed, BobPhsx.CustomData.maxfall);
            FrictionSlider = new PhsxSlider(Localization.Words.Friction, BobPhsx.CustomData.friction);
            JumpLengthSlider = new PhsxSlider(Localization.Words.JumpLength, BobPhsx.CustomData.jumplength);
            JumpAccelSlider = new PhsxSlider(Localization.Words.JumpAcc, BobPhsx.CustomData.jumpaccel);

            NumJumpsSlider = new PhsxSlider(Localization.Words.NumJumps, BobPhsx.CustomData.numjumps); NumJumpsSlider.Discrete = true;
            DoubleJumpLengthSlider = new PhsxSlider(Localization.Words.DoubleJumpLength, BobPhsx.CustomData.jumplength2);
            DoubleJumpAccelSlider = new PhsxSlider(Localization.Words.DoubleJumpAccel, BobPhsx.CustomData.jumpaccel2);

            JetPackSlider = new PhsxSlider(Localization.Words.JetpackAcc, BobPhsx.CustomData.jetpackaccel);
            JetPackFuelSlider = new PhsxSlider(Localization.Words.JetpackFuel, BobPhsx.CustomData.jetpackfuel);

            PhasedSizeSlider = new PhsxSlider(Localization.Words.PhasedSize, BobPhsx.CustomData.size2);
            PhasedGravitySlider = new PhsxSlider(Localization.Words.PhasedGravity, BobPhsx.CustomData.gravity2);
            PhasePeriodSlider = new PhsxSlider(Localization.Words.PhasePeriod, BobPhsx.CustomData.phaseperiod);
        }

        class StartTestProxy : Lambda
        {
            CustomHero_GUI chGui;

            public StartTestProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.StartTest();
            }
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
                ParticleEffects.AddPop(MyGame.MyLevel, bob.Pos);
                bob.CollectSelf();
            }

            MyGame.AddToDo(new MoveBlockAndKillProxy(this), "MoveOut", true, true);
        }

        class MoveBlockAndKillProxy : LambdaFunc<bool>
        {
            CustomHero_GUI chGui;

            public MoveBlockAndKillProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public bool Apply()
            {
                return chGui.MoveBlockAndKill();
            }
        }

        bool MoveBlockAndKill()
        {
            if (MyGame == null || MyGame.MyLevel == null) return true;

            foreach (BlockBase block in MyGame.MyLevel.Blocks)
                block.Move(new Vector2(0, -20));
            return false;
        }

        class KillBobsHelper : Lambda
        {
            CustomHero_GUI chGui;

            public KillBobsHelper(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                foreach (Bob bob in chGui.MyGame.MyLevel.Bobs)
                    bob.Die(Bob.BobDeathType.None);
            }
        }

        void KillBobs()
        {
            MyGame.WaitThenDo(20, new KillBobsHelper(this), "RemoveBobs", false, true);
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

            //var data = new BobPhsx.CustomPhsxData();
            //data.Init(GravitySlider.Val, AccelSlider.Val, MaxSpeedSlider.Val, MaxFallSpeedSlider.Val, JumpLengthSlider.Val, DoubleJumpLengthSlider.Val, JumpAccelSlider.Val, DoubleJumpAccelSlider.Val, JetPackSlider.Val, JetPackFuelSlider.Val, NumJumpsSlider.Val, SizeSlider.Val, PhasedSizeSlider.Val, PhasedGravitySlider.Val, PhasePeriodSlider.Val, FrictionSlider.Val);
            Hero.SetCustomPhsx(HeroPhsxData);
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

            list.GrayOutOnUnselectable = true;

            return list;
        }

        class AddItemProxy : Lambda_1<MenuItem>
        {
            CustomHero_GUI chGui;

            public AddItemProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply(MenuItem item)
            {
                chGui.AddItem(item);
            }
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

        static int BaseListIndex = 0;
        static int SizeListIndex = 0;
        static int JumpListIndex = 0;

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
            EzText HeaderText = new EzText(Localization.Words.HeroFactory, Resources.Font_Grobold42);
            HeaderText.Name = "Header";
            SetSuperHeader(HeaderText);
            HeaderText.Pos = new Vector2(-1169.842f, 985.7144f);
            MyPile.Add(HeaderText);

            // Backdrop
            var backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop);

            MenuItem item;

            // Hero lists
            BaseHeader = HeroText = new EzText(Localization.Words.Base, ItemFont);
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
            JumpHeader = HeroText = new EzText(Localization.Words.Jump, ItemFont);
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
            SizeHeader = HeroText = new EzText(Localization.Words.Shape, ItemFont);
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

            BaseList.SetIndex(BaseListIndex);
            SizeList.SetIndex(SizeListIndex);
            JumpList.SetIndex(JumpListIndex);

            FontScale = 1f;

            MakeSliders();

            // Start/test/back/advanced
            MakeOptionItems();

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            UpdateSliders();

            SetPos();
        }

        private void SetListActions()
        {
            BaseList.OnIndexSelect = new UpdateBaseHeroProxy(this);
            SizeList.OnIndexSelect = new UpdateSizeHeroProxy(this);
            JumpList.OnIndexSelect = new UpdateJumpHeroProxy(this);
        }

        void UpdateSliders()
        {
            if (GravitySlider == null) return;

            BaseList.Selectable = true;
            JumpList.Selectable = true;
            SizeList.Selectable = true;

            GravitySlider.Selectable = true;
            MaxSpeedSlider.Selectable = true;
            AccelSlider.Selectable = true;
            MaxFallSpeedSlider.Selectable = true;
            JumpLengthSlider.Selectable = true;
            JumpAccelSlider.Selectable = true;
            DoubleJumpLengthSlider.Selectable = true;
            DoubleJumpAccelSlider.Selectable = true;
            FrictionSlider.Selectable = true;
            JetPackSlider.Selectable = true;
            JetPackFuelSlider.Selectable = true;
            NumJumpsSlider.Selectable = true;
            SizeSlider.Selectable = true;
            PhasedSizeSlider.Selectable = true;
            PhasedGravitySlider.Selectable = true;
            PhasePeriodSlider.Selectable = true;

            // Base
            if (Base == BobPhsxNormal.Instance ||
                Base == BobPhsxWheel.Instance ||
                Base == BobPhsxBox.Instance ||
                Base == BobPhsxBouncy.Instance ||
                Base == BobPhsxMeat.Instance ||
                Base == BobPhsxRocketbox.Instance)
            {

            }
            else if (Base == BobPhsxSpaceship.Instance)
            {
                GravitySlider.Selectable &= false;
                MaxFallSpeedSlider.Selectable &= false;
                JumpLengthSlider.Selectable &= false;
                JumpAccelSlider.Selectable &= false;
                DoubleJumpLengthSlider.Selectable &= false;
                DoubleJumpAccelSlider.Selectable &= false;
                JetPackSlider.Selectable &= false;
                JetPackFuelSlider.Selectable &= false;
                NumJumpsSlider.Selectable &= false;

                JumpList.Selectable &= false;
            }
            else if (Base == BobPhsxMeat.Instance)
            {

            }

            // Jump 
            if (Jump == BobPhsxNormal.Instance)
            {
                DoubleJumpLengthSlider.Selectable &= false;
                DoubleJumpAccelSlider.Selectable &= false;
                NumJumpsSlider.Selectable &= false;
                JetPackSlider.Selectable &= false;
                JetPackFuelSlider.Selectable &= false;
            }
            else if (Jump == BobPhsxDouble.Instance)
            {
                JetPackSlider.Selectable &= false;
                JetPackFuelSlider.Selectable &= false;
            }
            else if (Jump == BobPhsxJetman.Instance)
            {
                DoubleJumpLengthSlider.Selectable &= false;
                DoubleJumpAccelSlider.Selectable &= false;
                NumJumpsSlider.Selectable &= false;
            }
            else if (Jump == BobPhsxInvert.Instance)
            {
                DoubleJumpLengthSlider.Selectable &= false;
                DoubleJumpAccelSlider.Selectable &= false;
                NumJumpsSlider.Selectable &= false;
                JetPackSlider.Selectable &= false;
                JetPackFuelSlider.Selectable &= false;
            }

            // Shape
            if (Size == BobPhsxNormal.Instance ||
                Size == BobPhsxBig.Instance ||
                Size == BobPhsxSmall.Instance)
            {
                PhasedSizeSlider.Selectable &= false;
                PhasedGravitySlider.Selectable &= false;
                PhasePeriodSlider.Selectable &= false;
            }
            else if (Size == BobPhsxScale.Instance)
            {

            }
        }

        //PhsxSlider GravitySlider, MaxSpeedSlider, AccelSlider;
        //PhsxSlider MaxFallSpeedSlider, JumpLengthSlider, JumpAccelSlider, DoubleJumpLengthSlider, DoubleJumpAccelSlider, FrictionSlider, JetPackSlider, JetPackFuelSlider, NumJumpsSlider;
        //PhsxSlider SizeSlider, PhasedSizeSlider, PhasedGravitySlider, PhasePeriodSlider;

        class UpdateBaseHeroProxy : Lambda
        {
            CustomHero_GUI chGui;

            public UpdateBaseHeroProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.UpdateBaseHero();
            }
        }

        void UpdateBaseHero()
        {
            Base = BaseList.CurObj as BobPhsx;
            BaseListIndex = BaseList.ListIndex;

            UpdateSliders();
        }

        class UpdateSizeHeroProxy : Lambda
        {
            CustomHero_GUI chGui;

            public UpdateSizeHeroProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.UpdateSizeHero();
            }
        }

        void UpdateSizeHero()
        {
            Size = SizeList.CurObj as BobPhsx;
            SizeListIndex = SizeList.ListIndex;

            UpdateSliders();
        }

        class UpdateJumpHeroProxy : Lambda
        {
            CustomHero_GUI chGui;

            public UpdateJumpHeroProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.UpdateJumpHero();
            }
        }

        void UpdateJumpHero()
        {
            Jump = JumpList.CurObj as BobPhsx;
            JumpListIndex = JumpList.ListIndex;

            UpdateSliders();
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
            {
                MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_A"));
                A = Start = item = new MenuItem(new EzText(Localization.Words.Test, ItemFont));
            }
            else
                A = Start = item = new MenuItem(new EzText(Localization.Words.Test, ItemFont));
            item.Name = "test";
            item.JiggleOnGo = false;
            AddItem(item);
            item.Go = Cast.ToItem(new StartTestProxy(this));
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
#if NOT_PC
            item.Selectable = false;
#endif

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(Start.Go);


            // Back
            if (WithButtonPics)
            {
                MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_B"));
                B = Back = item = new MenuItem(new EzText(Localization.Words.Back, ItemFont));
            }
            else
                B = Back = item = new MenuItem(new EzText(Localization.Words.Back, ItemFont));
            item.Name = "back";
            AddItem(item);
            item.SelectSound = null;
            item.Go = Cast.ToItem(new ReturnToCallerProxy(this));
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#if NOT_PC
            item.Selectable = false;
#endif

            // Continue
            if (WithButtonPics)
            {
                MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));
                X = item = new MenuItem(new EzText(Localization.Words.Play, ItemFont));
            }
            else
                X = item = new MenuItem(new EzText(Localization.Words.Play, ItemFont));

            item.Name = "continue";
            AddItem(item);
            item.SelectSound = null;
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
#if NOT_PC
            item.Selectable = false;
            MyMenu.OnX = Cast.ToMenu(X.Go);
#endif
            item.Go = Cast.ToItem(new NextProxy(this));

            // Reset
            item = ResetButton = new MenuItem(new EzText(Localization.Words.Reset, ItemFont));
            item.Name = "reset";
            AddItem(item);
            item.Go = Cast.ToItem(new ResetSlidersProxy(this));
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

        class NextProxy : Lambda
        {
            CustomHero_GUI chGui;

            public NextProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.Next();
            }
        }

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
                MyGame.PlayGame(new StartLevelProxy(this));
            }
        }

        class StartLevelProxy : Lambda
        {
            CustomHero_GUI chGui;

            public StartLevelProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.StartLevel();
            }
        }

        void StartLevel()
        {
            MakeBobPhsx();
            CustomLevel.LevelSeed.DefaultHeroType = Hero;
            CustomLevel.StartLevelFromMenuData();
        }

        class ResetSlidersProxy : Lambda
        {
            CustomHero_GUI chGui;

            public ResetSlidersProxy(CustomHero_GUI chGui)
            {
                this.chGui = chGui;
            }

            public void Apply()
            {
                chGui.ResetSliders();
            }
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