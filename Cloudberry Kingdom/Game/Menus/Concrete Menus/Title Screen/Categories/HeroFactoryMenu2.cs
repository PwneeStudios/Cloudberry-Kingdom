using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class HeroFactoryMenu2 : CkBaseMenu
    {
        public HeroFactoryMenu2() { }

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

        MenuSlider GravitySlider, MaxSpeedSlider, AccelSlider;

        public void StartGame()
        {
            Hide();

            MakeBobPhsx();

            Challenge_HeroFactoryEscalation.FactoryHero = Hero;

            // Start the game
            MyGame.PlayGame(() =>
            {
                // Show title again if we're selecting from the menu
                if (!MyGame.ExecutingPreviousLoadFunction)
                    HeroRush_Tutorial.ShowTitle = true;

                Challenge_HeroFactoryEscalation.Instance.Start(0);
            });

            MyGame.OnReturnTo_OneOff += () => Show();
        }

        public void StartTest()
        {
            Hide();

            MakeBobPhsx();

            CreateHeros();
            
            RemovePreviousGround();
            CreateGround();

            Testing = true;
        }

        bool Testing = false;
        void TestingPhsx()
        {
            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
                EndTest();
        }

        void EndTest()
        {
            Testing = false;
            Show();

            foreach (Bob bob in MyGame.MyLevel.Bobs)
            {
                //bob.CodeControl = true;
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

        BobPhsx Hero;
        BobPhsxNormal NormalHero { get { return (BobPhsxNormal)Hero; } }

        void MakeBobPhsx()
        {
            // Error check
            if (Base is BobPhsxSpaceship)
            {
                Jump = BobPhsxNormal.Instance;
            }

            // Make the hero
            Hero = MyGame.MyLevel.DefaultHeroType = BobPhsx.MakeCustom(Base, Size, Jump, BobPhsxNormal.Instance);

            Hero.Gravity *= GravitySlider.Val;
            Hero.XAccel *= AccelSlider.Val;
            Hero.MaxSpeed *= MaxSpeedSlider.Val;

            BobPhsxWheel wheel = Hero as BobPhsxWheel;
            if (null != wheel)
            {
                wheel.AngleAcc *= (float)Math.Pow(AccelSlider.Val, 1.5f);
                wheel.MaxAngleSpeed *= MaxSpeedSlider.Val;
            }

            if (Hero is BobPhsxNormal)
            {
                Hero.BobMaxFallSpeed *= MaxFallSpeedSlider.Val;

                NormalHero.BobJumpLength = (int)(NormalHero.BobJumpLength * JumpLengthSlider.Val);
                NormalHero.BobJumpLength2 = (int)(NormalHero.BobJumpLength2 * JumpLengthSlider2.Val);
                NormalHero.SetAccels(NormalHero.BobJumpLength);
                NormalHero.BobJumpAccel *= JumpAccelSlider.Val;
                NormalHero.BobJumpAccel2 *= JumpAccelSlider2.Val;

                NormalHero.XFriction *= FrictionSlider.Val;

                if (Jump is BobPhsxJetman)
                {
                    NormalHero.JetPackAccel *= JetPackSlider.Val;
                    NormalHero.JetPackLength = (int)(NormalHero.JetPackLength * JetPackFuelSlider.Val);
                }

                if (Jump is BobPhsxDouble)
                {
                    NormalHero.NumJumps = (int)(NumJumpsSlider.Val);
                }
            }

            if (Size is BobPhsxScale)
            {
                Hero.OscillateSize1 = Size1Slider.Val;
                Hero.OscillateSize2 = Size2Slider.Val;
                Hero.OscillateGravity1 *= Gravity1Slider.Val;
                Hero.OscillateGravity2 *= Gravity2Slider.Val;
                Hero.OscillatePeriod *= PhasePeriodSlider.Val;
            }
        }


        void CreateHeros()
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
            NormalBlock block;

            foreach (NormalBlock _block in MyGame.MyLevel.Blocks)
                if (_block is NormalBlock)
                    _block.CollectSelf();

            Vector2 shift = new Vector2(30, 430);

            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(MyGame.CamPos + new Vector2(-1000, -3100) + shift, new Vector2(1000, 2000), MyGame.MyLevel.MyTileSetInfo);
            MyGame.MyLevel.AddBlock(block);

            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(MyGame.CamPos + new Vector2(1150, -2950) + shift, new Vector2(1000, 2000), MyGame.MyLevel.MyTileSetInfo);
            MyGame.MyLevel.AddBlock(block);
        }



        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.ShadowColor = new Color(.2f, .2f, .2f, .6f);
            text.Shadow = false;

            //text.Angle = CoreMath.Radians(30);
            text.Angle = CoreMath.Radians(23);
        }

        void SetSuperHeader(EzText text)
        {
            base.SetHeaderProperties(text);
            text.MyFloatColor = new Vector4(1, 1, 1, 1);
            text.Scale = FontScale * 1.42f;
            text.ShadowOffset = new Vector2(17);

            CkColorHelper._x_x_HappyBlueColor(text); text.ShadowColor = ColorHelper.GrayColor(.3f); text.Scale *= 1.25f;
        }

        private MenuItem MakeListItem(BobPhsx hero, MenuList list)
        {
            MenuItem item;
            item = new MenuItem(new EzText("    ", ItemFont, false, true));

            item.Icon = hero.Icon.Clone(ObjectIcon.IconScale.NearlyFull);
            item.Icon.SetScale(.805f);
            item.Icon.FancyPos.SetCenter(list.FancyPos);
            item.Icon.SetShadow(false);
            item.Icon.SetShadow(new Color(.2f, .2f, .2f, .6f));

            item.MyOscillateParams.MyType = OscillateParams.Type.GetBig;
            item.Icon.MyOscillateParams.MyType = OscillateParams.Type.GetBig;
            item.JiggleOnGo = false;

            item.MyObject = hero;

            SetItemProperties(item);
            return item;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            if (item is MenuSlider || item is MenuList)
                item.GrayOutOnUnselectable = true;

            var slider = item as MenuSlider;
            if (null != slider)
            {
                if (slider.Code == 23)
                    slider.ScaleText(.485f);
                else
                    slider.ScaleText(.600f);

                slider.FancyPos.RelVal += new Vector2(-300, 0);

                if (slider.Code == 23)
                    slider.SliderShift += new Vector2(660, 60);
                else
                    slider.SliderShift += new Vector2(275, 60);
                //slider.SliderShift += new Vector2(275, 60);
            }
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
        public override void Init()
        {
            ItemShadows = false;

            SlideInFrom = SlideOutTo = PresetPos.Right;

            FontScale = .73f;

            MyPile = new DrawPile();

            base.Init();

            ReturnToCallerDelay = 18;
            SlideInLength = 25;
            SlideOutLength = 24;

            CallDelay = 5;

            SelectedItemShift = new Vector2(0, 0);

            // Header
            EzText HeaderText = new EzText("Hero Factory!", Resources.Font_Grobold42);
            SetSuperHeader(HeaderText);
            //HeaderText.Pos = new Vector2(-1169.842f, 953.9683f);
            HeaderText.Pos = new Vector2(-1169.842f, 985.7144f);
            MyPile.Add(HeaderText);


            // Backdrop
            //var backdrop = new QuadClass("BigPlaque", 1295, true);
            var backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            MyPile.Add(backdrop);
            backdrop.Pos =
                new Vector2(0, 0);

            // Band
            MakeBand();


            // Make the menu
            MyMenu = new Menu(false);

            Control = -1;

            MyMenu.OnB = null;

            MenuItem item;

            // Hero lists
            float x = -585;
            //float y = 455;
            //float y_add = 395;
            float y = 447.5f;
            float y_add = 370;
            Vector2 TextShift = new Vector2(-572, 200);

            // Hero base
            BaseHeader = HeroText = new EzText("base", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;

            BaseList = MakeList();
            foreach (Hero_BaseType _hero in Tools.GetValues<Hero_BaseType>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero, BaseList);
                BaseList.AddItem(item, hero);
            }

            BaseList.OnIndexSelect = () =>
                {
                    Base = BaseList.CurObj as BobPhsx;
                };
            AddItem(BaseList);
            BaseList.Pos = new Vector2(x, y); y -= y_add;

            // Hero jump
            JumpHeader = HeroText = new EzText("jump", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;

            JumpList = MakeList();
            foreach (Hero_MoveMod _hero in Tools.GetValues<Hero_MoveMod>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero, JumpList);
                JumpList.AddItem(item, hero);
            }

            JumpList.OnIndexSelect = () =>
            {
                Jump = JumpList.CurObj as BobPhsx;
            };
            AddItem(JumpList);
            JumpList.Pos = new Vector2(x, y); y -= y_add;

            // Hero shape
            SizeHeader = HeroText = new EzText("size", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;
            
            SizeList = MakeList();
            foreach (Hero_Shape _hero in Tools.GetValues<Hero_Shape>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero, SizeList);
                SizeList.AddItem(item, hero);
            }

            SizeList.OnIndexSelect = () =>
            {
                Size = SizeList.CurObj as BobPhsx;
            };
            AddItem(SizeList);
            SizeList.Pos = new Vector2(x, y); y -= y_add;

            FontScale = 1f;

            // Basic phsx sliders
            MakeBasicSliders();

            // Start/test/back/advanced
            MakeOptionItems();

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
            MyMenu.OnB = MenuReturnToCaller;
        }

        MenuItem A, B, X, Y;
        private void MakeOptionItems()
        {
            MenuItem item;

            //float bigscale = .68f;
            //float scale = .65f;

            float bigscale = .52f;
            float scale = .52f;

            //Vector2 pos = new Vector2(527.7778f, -492.0635f);
            //Vector2 add = new Vector2(0, -34);

            Vector2 pos = new Vector2(527.7778f, -492.0635f);
            Vector2 add = new Vector2(0, -60);


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
            item.JiggleOnGo = false;
            AddItem(item);
            item.Go = Cast.ToItem(StartTest);
            item.Pos = item.SelectedPos = pos; pos += add;
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
#if NOT_PC
            item.Selectable = false;
            item.Pos = pos; pos += add;
#endif
            item.ScaleText(bigscale);

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(Start.Go);


            // Back
            if (WithButtonPics)
                B = Back = item = new MenuItem(new EzText(ButtonString.Back(90) + " back", ItemFont));
            else
                B = Back = item = new MenuItem(new EzText("Back", ItemFont));
            AddItem(item);
            item.SelectSound = null;
            item.Go = Cast.ToItem(ReturnToCaller);
            item.Pos = item.SelectedPos = pos; pos += add;
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
#if NOT_PC
            item.Selectable = false;
            item.Pos = pos; pos += add;
#endif
            item.ScaleText(scale);



            // Advanced
#if NOT_PC
            Y = item = new MenuItem(new EzText(ButtonString.Y(90) + " more", ItemFont));
#else
            if (WithButtonPics)
                Y = item = new MenuItem(new EzText(ButtonString.Y(90) + " more", ItemFont));
            else
                Y = item = new MenuItem(new EzText("More", ItemFont));
#endif
            AddItem(item);
            item.SelectSound = null;
            item.Go = Cast.ToItem(ToAdvanced);
            item.Pos = item.SelectedPos = pos; pos += add;
            item.MyText.MyFloatColor = new Color(235, 255, 80).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(235, 255, 80).ToVector4();
#if NOT_PC
            item.Selectable = false;
            item.Pos = pos; pos += add;
            MyMenu.OnY = Cast.ToAction(Y.Go);
#endif
            item.ScaleText(scale);

            // Play
#if NOT_PC
            X = item = new MenuItem(new EzText(ButtonString.X(90) + " play", ItemFont));
#else
            if (WithButtonPics)
                X = item = new MenuItem(new EzText(ButtonString.X(90) + " play", ItemFont));
            else
                X = item = new MenuItem(new EzText("Play", ItemFont));
#endif
            AddItem(item);
            item.SelectSound = null;
            item.Go = Cast.ToItem(StartGame);
            item.Pos = item.SelectedPos = pos; pos += add;
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
#if NOT_PC
            item.Selectable = false;
            item.Pos = pos; pos += add;
            MyMenu.OnX = Cast.ToMenu(X.Go);
#endif
            item.ScaleText(scale);



            Diamond();

            MakeAdvancedSliders();
            ShowSimple();
        }

        void RightHand()
        {
            A.Show = A.Selectable = B.Show = B.Selectable = true;
            X.Show = X.Selectable = Y.Show = Y.Selectable = false;

            Vector2 shift = new Vector2(-106, -10);
            A.SetPos = new Vector2(920.635f, -578.7302f) + shift;
            B.SetPos = new Vector2(920.635f, -770.3174f) + shift;
        }

        void Diamond()
        {
            A.Show = A.Selectable = B.Show = B.Selectable = X.Show = X.Selectable = Y.Show = Y.Selectable = true;

            Vector2 shift = new Vector2(-106, -10);
#if PC_VERSION
            A.SetPos = new Vector2(920, -250) + shift;
            B.SetPos = new Vector2(920, -400) + shift;
            Y.SetPos = new Vector2(920, -550) + shift;
            X.SetPos = new Vector2(920, -700) + shift;
#else
            A.SetPos = new Vector2(718.2542f, -770.3174f) + shift;
            Y.SetPos = new Vector2(702.3809f, -390.3174f) + shift;
            X.SetPos = new Vector2(503.9681f, -581.746f) + shift;
            B.SetPos = new Vector2(920.635f, -578.7302f) + shift;
#endif

        }

        bool AdvancedAvailable()
        {
            if (Base is BobPhsxSpaceship && !(Size is BobPhsxScale))
                return false;
            else
                return true;
        }

        void ToAdvanced()
        {
            // No advanced options for spaceship, unless there is scaling
            if (!AdvancedAvailable()) return;

            MyGame.AddToDo(() =>
            {
                Basic = false;

                BaseHeader.Show = JumpHeader.Show = SizeHeader.Show = false;

                ShowAdvanced();
                RightHand();
                ResetAdvancedSliders();

                Back.Go = me => ToSimple();
                MyMenu.OnB = _menu => { ToSimple(); return true; };

                MyMenu.SelectItem(0);
            });
        }

        void ToSimple()
        {
            MyGame.AddToDo(() =>
            {
                Basic = true;

                BaseHeader.Show = JumpHeader.Show = SizeHeader.Show = true;

                ShowSimple();
                Diamond();

                Back.Go = me => ReturnToCaller();
                MyMenu.OnB = MenuReturnToCaller;

                MyMenu.SelectItem(0);
            });
        }

        MenuSlider MaxFallSpeedSlider, JumpLengthSlider, JumpAccelSlider, JumpLengthSlider2, JumpAccelSlider2, FrictionSlider, JetPackSlider, JetPackFuelSlider, NumJumpsSlider;
        MenuSlider Size1Slider, Size2Slider, Gravity1Slider, Gravity2Slider, PhasePeriodSlider;
        private void MakeAdvancedSliders()
        {
            ItemPos = new Vector2(-1069.841f, 600);

            MaxFallSpeedSlider = new MenuSlider(new EzText("Max fall", ItemFont));
            MaxFallSpeedSlider.MyFloat = new WrappedFloat(1f, .33f, 3f);
            MaxFallSpeedSlider.Code = 23;
            AddItem(MaxFallSpeedSlider);

            FrictionSlider = new MenuSlider(new EzText("Friction", ItemFont));
            FrictionSlider.MyFloat = new WrappedFloat(1f, 0f, 3f);
            FrictionSlider.Code = 23;
            AddItem(FrictionSlider);

            JumpLengthSlider = new MenuSlider(new EzText("Jump Length", ItemFont));
            JumpLengthSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            JumpLengthSlider.Code = 23;
            AddItem(JumpLengthSlider);

            JumpAccelSlider = new MenuSlider(new EzText("Jump Accel", ItemFont));
            JumpAccelSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            JumpAccelSlider.Code = 23;
            AddItem(JumpAccelSlider);

            //if (Jump is BobPhsxDouble)
            {
                JumpLengthSlider2 = new MenuSlider(new EzText("Jump Length 2", ItemFont));
                JumpLengthSlider2.MyFloat = new WrappedFloat(1f, .5f, 2f);
                JumpLengthSlider2.Code = 23;
                AddItem(JumpLengthSlider2);

                JumpAccelSlider2 = new MenuSlider(new EzText("Jump Accel 2", ItemFont));
                JumpAccelSlider2.MyFloat = new WrappedFloat(1f, .5f, 2f);
                JumpAccelSlider2.Code = 23;
                AddItem(JumpAccelSlider2);

                NumJumpsSlider = new MenuSlider(new EzText("Num jump", ItemFont));
                NumJumpsSlider.MyFloat = new WrappedFloat(2f, 2f, 4f);
                if (Jump is BobPhsxDouble)
                {
                    NumJumpsSlider.Val = 2;
                    NumJumpsSlider.Selectable = true;
                }
                else
                {
                    NumJumpsSlider.Val = 1;
                    NumJumpsSlider.Selectable = true;
                }
                NumJumpsSlider.InitialSlideSpeed = 1;
                NumJumpsSlider.MaxSlideSpeed = 1;
                NumJumpsSlider.Discrete = true;
                NumJumpsSlider.Code = 23;
                AddItem(NumJumpsSlider);
            }

            //if (Jump is BobPhsxJetman)
            {
                JetPackSlider = new MenuSlider(new EzText("Jetpack str", ItemFont));
                JetPackSlider.MyFloat = new WrappedFloat(1f, .75f, 2f);
                JetPackSlider.Code = 23;
                AddItem(JetPackSlider);

                JetPackFuelSlider = new MenuSlider(new EzText("Jetpack fuel", ItemFont));
                JetPackFuelSlider.MyFloat = new WrappedFloat(1f, .5f, 3f);
                JetPackFuelSlider.Code = 23;
                AddItem(JetPackFuelSlider);
            }

            //if (Size is BobPhsxScale)
            {
                Size1Slider = new MenuSlider(new EzText("Size 1", ItemFont));
                Size1Slider.MyFloat = new WrappedFloat(.32f, .2f, 2.1f);
                Size1Slider.Code = 23;
                AddItem(Size1Slider);

                Size2Slider = new MenuSlider(new EzText("Size 2", ItemFont));
                Size2Slider.MyFloat = new WrappedFloat(2.08f, .2f, 2.1f);
                Size2Slider.Code = 23;
                AddItem(Size2Slider);

                Gravity1Slider = new MenuSlider(new EzText("Gravity 1", ItemFont));
                Gravity1Slider.MyFloat = new WrappedFloat(1f, .5f, 2f);
                Gravity1Slider.Code = 23;
                AddItem(Gravity1Slider);

                Gravity2Slider = new MenuSlider(new EzText("Gravity 2", ItemFont));
                Gravity2Slider.MyFloat = new WrappedFloat(1f, .5f, 2f);
                Gravity2Slider.Code = 23;
                AddItem(Gravity2Slider);

                PhasePeriodSlider = new MenuSlider(new EzText("Phase period", ItemFont));
                PhasePeriodSlider.MyFloat = new WrappedFloat(1f, .35f, 2f);
                PhasePeriodSlider.Code = 23;
                AddItem(PhasePeriodSlider);
            }

            // Not spaceship or scaling
            GravitySlider = new MenuSlider(new EzText("gravity", ItemFont));
            GravitySlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            GravitySlider.Code = 23;
            AddItem(GravitySlider);

        }

        void ResetItemPos(MenuItem item)
        {
            item.SetPos = ItemPos;
            //ItemPos += new Vector2(0, -143);
            ItemPos += new Vector2(0, -128.5f);

            item.Show = item.Selectable = true;
        }
        void ResetAdvancedSliders()
        {
            ItemPos = new Vector2(-1069.841f, 600);
            
            //ResetItemPos(JumpLengthSlider);
            //ResetItemPos(JumpAccelSlider);
            //ResetItemPos(FrictionSlider);

            MaxFallSpeedSlider.Show = JumpLengthSlider.Show = JumpAccelSlider.Show = FrictionSlider.Show = true;

            if (Jump is BobPhsxDouble)
                JumpAccelSlider2.Show = JumpLengthSlider2.Show = NumJumpsSlider.Show = true;

            if (Jump is BobPhsxJetman)
                JetPackFuelSlider.Show = JetPackSlider.Show = true;

            if (Size is BobPhsxScale)
                Size1Slider.Show = Size2Slider.Show = Gravity1Slider.Show = Gravity2Slider.Show = PhasePeriodSlider.Show = true;
            else
                if (Base is BobPhsxNormal)
                    GravitySlider.Show = true;

            foreach (MenuItem item in MyMenu.Items)
                if (item.Code == 23 && item.Show)
                    ResetItemPos(item);
        }

        void ShowAdvanced()
        {
            ResetButton.Show = ResetButton.Selectable = false;
            foreach (var item in MyMenu.Items)
                if (item is MenuSlider || item is MenuList)
                    item.Show = item.Selectable = false;
        }

        void ShowSimple()
        {
            ResetButton.Show = ResetButton.Selectable = true;
            foreach (var item in MyMenu.Items)
                if (item is MenuSlider || item is MenuList)
                    item.Show = item.Selectable = item.Code != 23;
        }

        MenuItem ResetButton;
        private void MakeBasicSliders()
        {
            ItemPos = new Vector2(-1069.841f, -460.2539f);
            PosAdd = new Vector2(0, -145);

            AccelSlider = new MenuSlider(new EzText("accel", ItemFont));
            AccelSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            AddItem(AccelSlider);

            MaxSpeedSlider = new MenuSlider(new EzText("max vel", ItemFont));
            MaxSpeedSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            AddItem(MaxSpeedSlider);

            // Reset
            var item = ResetButton = new MenuItem(new EzText("Reset", ItemFont));
            AddItem(item);
            item.Go = Cast.ToItem(ResetSliders);
            item.ScaleText(.7f);
            item.SetPos -= new Vector2(0, -20);
        }

        void ResetSliders()
        {
            foreach (var item in MyMenu.Items)
            {
                MenuSlider slider = item as MenuSlider;
                if (null != slider)
                    slider.Reset();
            }
        }

        QuadClass Band;
        private void MakeBand()
        {
            Band = new QuadClass("Band", 900, true);
            Band.Size = new Vector2(1100f, 245f);
            Band.Alpha = .36f;
            MyPile.Add(Band);
        }

        CustomUpgrades_GUI AdvancedGUI;
        void BringAdvanced()
        {
            //// Create the advanced menu
            //AdvancedGUI = new AdvancedCustomGUI(PieceSeed);
            //Call(AdvancedGUI, 0);
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }

        /// <summary>
        /// Whether we are in basic editing mode.
        /// </summary>
        bool Basic = true;

        protected override void MyPhsxStep()
        {
            if (Basic)
            {
                if (AdvancedAvailable())
                    Y.Show = Y.Selectable = true;
                else
                    Y.Show = Y.Selectable = false;
            }

            if (Testing)
            {
                TestingPhsx();
                return;
            }

            if (Size == BobPhsxScale.Instance)
            {
                GravitySlider.Selectable = false;
            }
            else
            {
                GravitySlider.Selectable = true;
            }

            if (Base == BobPhsxSpaceship.Instance)
            {
                JumpList.Selectable = false;
            }
            else
            {
                JumpList.Selectable = true;
            }


            base.MyPhsxStep();

            if (MyMenu.CurItem is MenuList)
            {
                Band.PosY = MyMenu.CurItem.Pos.Y;
                Band.Show = true;
            }
            else
                Band.Show = false;
        }
    }
}