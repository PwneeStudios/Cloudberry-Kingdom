using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class HeroFactoryMenu2 : StartMenuBase
    {
        public HeroFactoryMenu2() { }

        MenuSlider GravitySlider, MaxSpeedSlider, AccelSlider;

        public void StartGame()
        {
            Challenge_Escalation.Hero = Hero;

            // Start the game
            MyGame.PlayGame(() =>
            {
                // Show title again if we're selecting from the menu
                if (!MyGame.ExecutingPreviousLoadFunction)
                    HeroRush_Tutorial.ShowTitle = true;

                Challenge_Escalation.Instance.Start(0);
            });
        }

        public void StartTest()
        {
            Hide();

            MakeBobPhsx();
            
            //CreateHeros();
            //CreateGround();

            StartGame();

            Testing = true;
        }

        bool Testing = false;
        void TestingPhsx()
        {
            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
            {
                Testing = false;
                Show();
            }
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
            Hero = MyGame.MyLevel.DefaultHeroType = BobPhsx.MakeCustom(Base, Jump, Size);
            Hero.ModPhsxValues = _vals =>
                {
                    _vals.Gravity *= GravitySlider.Val;
                    _vals.XAccel *= AccelSlider.Val;
                    _vals.MaxSpeed *= MaxSpeedSlider.Val;
                };
        }

        BobPhsx Hero;
        void CreateHeros()
        {
            // Remove any previous bobs
            MyGame.KillToDo("RemoveBobs");
            foreach (Bob bob in MyGame.MyLevel.Bobs)
                bob.CollectSelf();

            // Make new bobs
            MyGame.MakeBobs(MyGame.MyLevel);
            
            // Position bobs
            Vector2 shift = new Vector2(-700, 0);
            foreach (Bob bob in MyGame.MyLevel.Bobs)
            {
                Tools.MoveTo(bob, MyGame.CamPos + shift);
                shift += new Vector2(100, 20);
            }
        }

        void CreateGround()
        {
            NormalBlock block;

            foreach (NormalBlock _block in MyGame.MyLevel.Blocks)
                if (_block is NormalBlock)
                    _block.CollectSelf();


            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(MyGame.CamPos + new Vector2(-1000, -3100), new Vector2(1000, 2000));
            block.BlockCore.MyTileSetType = TileSet.OutsideGrass;
            MyGame.MyLevel.AddBlock(block);

            block = (NormalBlock)MyGame.Recycle.GetObject(ObjectType.NormalBlock, false);
            block.Init(MyGame.CamPos + new Vector2(1150, -2950), new Vector2(1000, 2000));
            block.BlockCore.MyTileSetType = TileSet.OutsideGrass;
            MyGame.MyLevel.AddBlock(block);
        }



        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.ShadowColor = new Color(.2f, .2f, .2f, .6f);
            text.Shadow = false;

            //text.Angle = Tools.Radians(30);
            text.Angle = Tools.Radians(23);
        }

        void SetSuperHeader(EzText text)
        {
            base.SetHeaderProperties(text);
            text.MyFloatColor = new Vector4(1, 1, 1, 1);
            text.Scale = FontScale * 1.42f;
            text.ShadowOffset = new Vector2(17);

            CampaignMenu.HappyBlueColor(text); text.ShadowColor = Tools.GrayColor(.3f); text.Scale *= 1.25f;
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

        MenuItem Start;
        EzText HeroText;
        MenuList BaseList, JumpList, SizeList;
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
            EzText HeaderText = new EzText("Hero Factory!", Tools.Font_DylanThin42);
            SetSuperHeader(HeaderText);
            HeaderText.Pos = new Vector2(-1169.842f, 953.9683f);
            MyPile.Add(HeaderText);


            // Backdrop
            //var backdrop = new QuadClass("BigPlaque", 1295, true);
            var backdrop = new QuadClass("WoodMenu_1", 1500, true);
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
            float y = 455; float y_add = 395;
            Vector2 TextShift = new Vector2(-572, 200);

            // Hero base
            HeroText = new EzText("base", ItemFont);
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
            HeroText = new EzText("jump", ItemFont);
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
            HeroText = new EzText("size", ItemFont);
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
            MakeSliders();

            // Start
            Start = item = new MenuItem(new EzText(ButtonString.Go(90) + " Test Hero", ItemFont));
            item.JiggleOnGo = false;
            AddItem(item);
            item.Go = _item => StartTest();
            item.Pos = item.SelectedPos = new Vector2(682.1445f, -238.8095f);
            item.MyText.MyFloatColor = InfoWad.GetColor("Menu_UnselectedNextColor").ToVector4();
            item.MySelectedText.MyFloatColor = InfoWad.GetColor("Menu_SelectedNextColor").ToVector4();
#if NOT_PC
            item.Selectable = false;
            item.Pos = new Vector2(721.8262f, -226.9048f);
#endif
            item.ScaleText(.68f);

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = menu => { Start.Go(null); return true; };


            item = new MenuItem(new EzText(ButtonString.Back(90) + " Back", ItemFont));
            AddItem(item);
            item.SelectSound = null;
            item.Go = me => ReturnToCaller();
            item.Pos = item.SelectedPos = new Vector2(922.9375f, -523.8096f);
            item.MyText.MyFloatColor = InfoWad.GetColor("Menu_UnselectedBackColor").ToVector4();
            item.MySelectedText.MyFloatColor = InfoWad.GetColor("Menu_SelectedBackColor").ToVector4();
#if NOT_PC
            item.Selectable = false;
            item.Pos = new Vector2(958.6523f, -468.254f);
#endif
            item.ScaleText(.65f);

            /*
            item = new MenuItem(new EzText(ButtonString.Y(90) + " Advanced", ItemFont));
            item.Go = menuitem => BringAdvanced();
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(905f, -653.3308f);
            item.MyText.Scale *= .9f;
            item.MySelectedText.Scale *= .9f;
            */

            // Select the first item in the menu to start
            MyMenu.SelectItem(0);
            MyMenu.OnB = MenuReturnToCaller;
        }

        private void MakeSliders()
        {
            GravitySlider = new MenuSlider(new EzText("gravity", ItemFont));
            GravitySlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            AddItem(GravitySlider);

            AccelSlider = new MenuSlider(new EzText("accel", ItemFont));
            AccelSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            AddItem(AccelSlider);

            MaxSpeedSlider = new MenuSlider(new EzText("max vel", ItemFont));
            MaxSpeedSlider.MyFloat = new WrappedFloat(1f, .5f, 2f);
            AddItem(MaxSpeedSlider);
        }

        QuadClass Band;
        private void MakeBand()
        {
            Band = new QuadClass("Band", 900, true);
            Band.Size = new Vector2(1100f, 245f);
            Band.Alpha = .36f;
            MyPile.Add(Band);
        }

        AdvancedCustomGUI AdvancedGUI;
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

        protected override void MyPhsxStep()
        {
            if (Testing)
            {
                TestingPhsx();
                return;
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