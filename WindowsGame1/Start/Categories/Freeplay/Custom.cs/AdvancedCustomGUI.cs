using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace CloudberryKingdom
{
    public class AdvancedCustomGUI : StartMenuBase
    {
        PieceSeedData PieceSeed;

        //PieceQuad Backdrop;

        EzText TopText;
        QuadClass Berry, Stickman;

        public AdvancedCustomGUI(PieceSeedData PieceSeed)
        {
            this.PieceSeed = PieceSeed;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
        }

        public ObjectIcon BigIcon;
        void AddUpgrade(Upgrade upgrade)
        {
            MenuSlider slider = new MenuSlider(new EzText("", ItemFont));
            slider.SliderBackSize *= new Vector2(1.15f, .72f) * .975f * ScaleList;
            slider.SetIcon(ObjectIcon.CreateIcon(upgrade));
            slider.Icon.SetScale(.6f * ScaleList);
            slider.MyFloat = new WrappedFloat(0, 0, 9);
            slider.InitialSlideSpeed *= 9;
            slider.MaxSlideSpeed *= 9;
            slider.Icon.Pos += new Vector2(110, -123.5f);// *ScaleList;
            slider.MyFloat.GetCallback = () => PieceSeed.MyUpgrades1[upgrade];
            
            // Keep the PieceSeed up to date when the slider changes
            if (upgrade == Upgrade.FallingBlock)
            {
                slider.MyFloat.SetCallback = () =>
                {
                    PieceSeed.MyUpgrades1[upgrade] = slider.MyFloat.MyFloat;

                    // Set the Fallingblock to an O-face when maxed out
                    if (slider == MyMenu.CurItem)
                    {
                        PictureIcon pic = BigIcon as PictureIcon;
                        if (slider.MyFloat.Val > 7.6f) pic.IconQuad.TextureName = "FallingBlock3";
                        else if (slider.MyFloat.Val > 3.2f) pic.IconQuad.TextureName = "FallingBlock2";
                        else pic.IconQuad.TextureName = "FallingBlock1";
                    }
                };
            }
            else
                slider.MyFloat.SetCallback = () => PieceSeed.MyUpgrades1[upgrade] = slider.MyFloat.MyFloat;

            slider.AdditionalOnSelect = () =>
                {
                    TopText.SubstituteText(slider.Icon.DisplayText);
                    TopText.Pos = new Vector2(761 + 280, -46 + 771);
                    TopText.Center();

                    BigIcon = ObjectIcon.CreateIcon(upgrade);
                    BigIcon.SetScale(2f);
                    BigIcon.FancyPos.SetCenter(Pos);
                    BigIcon.Pos = new Vector2(731f + 500 * (1 - ScaleList), 198f);
                    BigIcon.MyOscillateParams.max_addition *= .25f;

                    TopText.Show = true;
                    Berry.Show = false;
                };
            AddItem(slider);
        }

        void Clear()
        {
            BigIcon = null;

            TopText.Show = false;
            Berry.Show = false;
        }
        

        void StartLevel()
        {
            SimpleCustomGUI simple = Caller as SimpleCustomGUI;
            if (null != simple) simple.StartLevel();
        }

        void Zero(MenuItem _item)
        {
            foreach (MenuItem item in MyMenu.Items)
            {
                MenuSlider slider = item as MenuSlider;
                if (null != slider)
                    slider.MyFloat.Val = 0;
            }
        }

        void Randomize(MenuItem _item)
        {
            if (MyLevel.Rnd.RndFloat() < .25f)
            {
                foreach (MenuItem item in MyMenu.Items)
                {
                    MenuSlider slider = item as MenuSlider;
                    if (null != slider)
                        slider.MyFloat.Val = (float)Math.Pow(MyLevel.Rnd.RndFloat(0, 9), .9f);
                }
            }
            else
            {
                foreach (MenuItem item in MyMenu.Items)
                {
                    MenuSlider slider = item as MenuSlider;
                    if (null != slider)
                    {
                        float ChanceToZero = .1f;
                        if (slider.MyFloat.Val > 1) ChanceToZero = .6f;
                        if (MyLevel.Rnd.RndFloat() < ChanceToZero)
                            slider.MyFloat.Val = 0;
                        else
                            slider.MyFloat.Val = (float)Math.Pow(MyLevel.Rnd.RndFloat(0, 9), .93f);
                    }
                }
            }
        }

        float ScaleList = .7f;

        void SetBerry(string name)
        {
            return;

            //TopText.Show = false;
            //BigIcon = null; Berry.Show = true; Berry.TextureName = name;
        }

        static Vector2 RightPanelCenter = new Vector2(183, 75);
        public override void Init()
        {
            base.Init();

            Control = -1;

            ReturnToCallerDelay = 6;
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;
            SlideInLength = 25;
            SlideOutLength = 20;

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.OnA = _menu => { MyGame.PlayGame(() => StartLevel()); return true; };
            MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnX = _menu => { Randomize(null); return true; };
            MyMenu.OnY = () => Zero(null);
            MyMenu.SelectDelay = 11;
            ItemPos = new Vector2(-950, 928 + 300 * (1 - ScaleList));
            PosAdd.Y *= 1.015f * ScaleList;

            // Add obstacles to the grid
            AddUpgrade(Upgrade.Jump);
            AddUpgrade(Upgrade.Speed);
            AddUpgrade(Upgrade.Ceiling);

            foreach (Upgrade upgrade in RegularLevel.ObstacleUpgrades)
                AddUpgrade(upgrade);

            MyPile = new DrawPile();

            FontScale = 1f;
            MakeOptions();

            /*
            // Make the backdrop
            Backdrop = new PieceQuad();

            Backdrop.Clone(PieceQuad.FreePlayMenu);
            Vector2 MenuSize = 1.9f * new Vector2(943, 627);
            Backdrop.CalcQuads(MenuSize);
            Backdrop.Center.MyTexture = Tools.TextureWad.FindByName("WoodMenu_1");

            // Backdrop #2
            QuadClass backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "score screen";
            backdrop.Pos = new Vector2(924.6074f, -35.71436f);
            backdrop.Size = new Vector2(766.7461f, 1283.284f);
            */
            //MyPile.Add(backdrop);

            // Backdrop
            QuadClass backdrop;

            backdrop = new QuadClass("WoodMenu_1", 1500, true);
            MyPile.Add(backdrop);
            backdrop.Size =
                //new Vector2(1615.079f, 1111.649f);
                new Vector2(1682.54f, 1107.681f);
            backdrop.Pos =
                //new Vector2(248.018f, 55.55578f);
                new Vector2(347.2231f, 51.58749f);

            // Make the stickman
            Stickman = new QuadClass("Stickman", 700, true);
            //MyPile.Add(Stickman);
            Stickman.Size = new Vector2(414.2891f, 605.3662f);
            Stickman.Pos = new Vector2(901.5855f, 344.6031f);

            // Make the berry
            Berry = new QuadClass("cb_standing", 500, true);
            MyPile.Add(Berry);
            Berry.Pos = new Vector2(949.2065f, 329.3651f);

            // Make the top text
            MakeTopText();
        }

        void Go(MenuItem item)
        {
            MyGame.PlayGame(() => StartLevel());
        }

        bool Go(Menu menu) { Go((MenuItem)null); return true; }

        private void MakeOptions()
        {
            FontScale *= .8f;

#if PC_VERSION
            // Start
            MenuItem item;
            //MenuItem Start = item = new MenuItem(new EzText(ButtonString.Go(82) + " Start", ItemFont));
            MenuItem Start = item = new MenuItem(new EzText("Start", ItemFont));
            item.Go = Go;
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(425.3959f, -99.92095f);
            item.MyText.MyFloatColor = InfoWad.GetColor("Menu_UnselectedNextColor").ToVector4();
            item.MySelectedText.MyFloatColor = InfoWad.GetColor("Menu_SelectedNextColor").ToVector4();
            //item.AdditionalOnSelect = () => SetBerry("cb_enthusiastic");

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = MyMenu.OnA = Go;

            // Random
            //item = new MenuItem(new EzText(ButtonString.X(82) + " Random", ItemFont));
            item = new MenuItem(new EzText("Random", ItemFont));
            item.Go = Randomize;
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(511.8408f, -302.6506f);
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
            //item.AdditionalOnSelect = () => SetBerry("cb_surprised");

            // Zero
            //item = new MenuItem(new EzText(ButtonString.Y(82) + " Zero", ItemFont));
            item = new MenuItem(new EzText("Reset", ItemFont));
            item.Go = Zero;
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(599.1416f, -501.0634f);
            //item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            //item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
            item.MyText.MyFloatColor = new Color(235, 255, 80).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(235, 255, 80).ToVector4();
            //item.AdditionalOnSelect = () => SetBerry("cb_surprised");

            // Back
            //item = new MenuItem(new EzText(ButtonString.Back(82) + " Back", ItemFont));
            item = new MenuItem(new EzText("Back", ItemFont));
            item.Go = null;
            AddItem(item);
            item.SelectSound = null;
            item.Go = ItemReturnToCaller;
            item.Pos = item.SelectedPos = new Vector2(702.3179f, -689.9683f);
            item.MyText.MyFloatColor = InfoWad.GetColor("Menu_UnselectedBackColor").ToVector4();
            item.MySelectedText.MyFloatColor = InfoWad.GetColor("Menu_SelectedBackColor").ToVector4();
            //item.AdditionalOnSelect = () => SetBerry("cb_crying");
#else
            EzText text;
            text = new EzText(ButtonString.Go(90) + " Start", ItemFont);
            text.Pos = new Vector2(417.4604f, -159.4446f);
            text.MyFloatColor = InfoWad.GetColor("Menu_UnselectedNextColor").ToVector4();
            MyPile.Add(text);

            text = new EzText(ButtonString.X(90) + " Random", ItemFont);
            text.Pos = new Vector2(531.6831f, -389.9523f);
            text.MyFloatColor = new Color(204, 220, 255).ToVector4();
            MyPile.Add(text);

            text = new EzText(ButtonString.Back(90) + " Back", ItemFont);
            text.Pos = new Vector2(682.4761f, -622.5079f);
            text.MyFloatColor = InfoWad.GetColor("Menu_SelectedBackColor").ToVector4();
            MyPile.Add(text);
#endif
        }

        private void MakeTopText()
        {
            TopText = new EzText("   ", ItemFont, true, true);
            SetTextProperties(TopText);
            TopText.Shadow = false;
            MyPile.Add(TopText);
            TopText.Scale *= .83f;
            TopText.Pos = new Vector2(199.2069f, 746.0317f);
            TopText.Center();
        }

        protected override void MyDraw()
        {
            //Backdrop.Base.Origin = Pos.Update() + RightPanelCenter + new Vector2(170, 0);
            //Backdrop.Draw();

            Camera cam = MyGame.MyLevel.MainCamera;

            //cam.SetVertexZoom(new Vector2(.75f, .75f));
            //MyGame.MyLevel.MainCamera.RevertZoom();

            base.MyDraw();

            if (BigIcon != null)
                BigIcon.Draw(true);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (Hid) return;
        }
    }
}