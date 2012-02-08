using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;

namespace CloudberryKingdom
{
    public class HeroFactoryMenu : StartMenuBase
    {
        public HeroFactoryMenu() { }

        public void StartLevel()
        {
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.ShadowColor = new Color(.2f, .2f, .2f, .6f);
            text.Shadow = false;

            //text.Angle = Tools.Radians(30);
            text.Angle = Tools.Radians(23);
        }

        private MenuItem MakeListItem(BobPhsx hero)
        {
            MenuItem item;
            item = new MenuItem(new EzText("    ", ItemFont, false, true));

            item.Icon = hero.Icon.Clone(ObjectIcon.IconScale.NearlyFull);
            item.Icon.SetScale(.805f);
            item.Icon.FancyPos.SetCenter(HeroList.FancyPos);
            item.Icon.SetShadow(false);
            item.Icon.SetShadow(new Color(.2f, .2f, .2f, .6f));

            item.MyOscillateParams.MyType = OscillateParams.Type.GetBig;
            item.Icon.MyOscillateParams.MyType = OscillateParams.Type.GetBig;
            item.JiggleOnGo = false;

            item.MyObject = hero;

            SetItemProperties(item);
            return item;
        }

        MenuListAll MakeList()
        {
            var list = new MenuListAll();
            //list.ShiftAmount = 325;
            list.ShiftAmount = 275;

            return list;
        }

        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);

            MenuListAll list = item as MenuListAll;
            if (null != list)
            {
                list.SetIndex(0);
            }
        }

        MenuItem Start;
        MenuListAll HeroList;
        EzText HeroText;
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
            HeaderText.Scale = 1.05f;
            HeaderText.OutlineColor = new Color(255, 255, 255).ToVector4();
            HeaderText.MyFloatColor = new Color(0, 13, 44).ToVector4();
            //CharacterSelectManager.ChooseHeroTextStyle(HeaderText);
            HeaderText.Pos = new Vector2(-1169.842f, 953.9683f);
            MyPile.Add(HeaderText);


            // Backdrop
            var backdrop = new QuadClass("BigPlaque", 1295, true);
            MyPile.Add(backdrop);
            //backdrop.Size =
            //    new Vector2(1230.477f, 1115.617f);
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
            float y = 435; float y_add = 395;
            Vector2 TextShift = new Vector2(-550, 200);

            // Hero base
            HeroText = new EzText("base", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;

            HeroList = MakeList();
            foreach (Hero_BaseType _hero in Tools.GetValues<Hero_BaseType>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero);
                HeroList.AddItem(item, hero);
            }

            HeroList.OnIndexSelect = () =>
                {
                    BobPhsx Hero = HeroList.CurObj as BobPhsx;
                };
            AddItem(HeroList);
            HeroList.Pos = new Vector2(x, y); y -= y_add;

            // Hero jump
            HeroText = new EzText("jump", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;

            HeroList = MakeList();
            foreach (Hero_MoveMod _hero in Tools.GetValues<Hero_MoveMod>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero);
                HeroList.AddItem(item, hero);
            }

            HeroList.OnIndexSelect = () =>
            {
                BobPhsx Hero = HeroList.CurObj as BobPhsx;
            };
            AddItem(HeroList);
            HeroList.Pos = new Vector2(x, y); y -= y_add;

            // Hero shape
            HeroText = new EzText("size", ItemFont);
            SetHeaderProperties(HeroText);
            MyPile.Add(HeroText);
            HeroText.Pos = new Vector2(x, y) + TextShift;
            
            HeroList = MakeList();
            foreach (Hero_Shape _hero in Tools.GetValues<Hero_Shape>())
            {
                BobPhsx hero = BobPhsx.GetPhsx(_hero);

                item = MakeListItem(hero);
                HeroList.AddItem(item, hero);
            }

            HeroList.OnIndexSelect = () =>
            {
                BobPhsx Hero = HeroList.CurObj as BobPhsx;
            };
            AddItem(HeroList);
            HeroList.Pos = new Vector2(x, y); y -= y_add;

            FontScale = 1f;

            // Start
            Start = item = new MenuItem(new EzText(ButtonString.Go(90) + " Test Hero", ItemFont));
            item.JiggleOnGo = false;
            AddItem(item);
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
                //menuitem =>
                //{
                //    if (MyMenu.CurItem.Go != null) return false;

                //    MyMenu.SelectItem(Start);
                //    return true;
                //};

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
            HeroList.SetIndex(0);
            MyMenu.OnB = MenuReturnToCaller;
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
            base.MyPhsxStep();

            if (MyMenu.CurItem is MenuListAll)
            {
                Band.PosY = MyMenu.CurItem.Pos.Y;
                Band.Show = true;
            }
            else
                Band.Show = false;
        }
    }
}