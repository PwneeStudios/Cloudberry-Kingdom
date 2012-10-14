using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class AggressiveUpgrades_GUI : CustomUpgrades_GUI
    {
        public AggressiveUpgrades_GUI(PieceSeedData PieceSeed, CustomLevel_GUI CustomLevel)
            : base(PieceSeed, CustomLevel)
        {
        }

        public override List<Upgrade> GetUpgradeList()
        {
            return new List<Upgrade>(new Upgrade[] { Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike, Upgrade.LavaDrip, Upgrade.Serpent, Upgrade.SpikeyLine });
        }

        protected override void Go()
        {
            StartGame();
        }

        protected override string HeaderText()
        {
            return "Aggressive";
        }
    }

    public class PassiveUpgrades_GUI : CustomUpgrades_GUI
    {
        public PassiveUpgrades_GUI(PieceSeedData PieceSeed, CustomLevel_GUI CustomLevel)
            : base(PieceSeed, CustomLevel)
        {
        }

        public override List<Upgrade> GetUpgradeList()
        {
            return new List<Upgrade>(new Upgrade[] { Upgrade.Jump, Upgrade.Speed, Upgrade.Ceiling, Upgrade.MovingBlock, Upgrade.GhostBlock, Upgrade.FlyBlob, Upgrade.FallingBlock, Upgrade.Elevator, Upgrade.Cloud, Upgrade.BouncyBlock, Upgrade.Pendulum });
        }

        protected override void Go()
        {
            var UpgradeGui = new AggressiveUpgrades_GUI(PieceSeed, CustomLevel);
            Call(UpgradeGui, 0);
            Hide(PresetPos.Left);
            this.SlideInFrom = PresetPos.Left;
        }

        protected override string HeaderText()
        {
            return "Passive";
        }
    }

    public class CustomUpgrades_GUI : CkBaseMenu
    {
        protected PieceSeedData PieceSeed;
        protected CustomLevel_GUI CustomLevel;

        EzText TopText;

        public CustomUpgrades_GUI(PieceSeedData PieceSeed, CustomLevel_GUI CustomLevel)
        {
            CustomLevel.CallingPanel = this;

            this.PieceSeed = PieceSeed;
            this.CustomLevel = CustomLevel;
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
            slider.Icon.Pos += new Vector2(110, -123.5f);
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
                        if (slider.MyFloat.Val > 7.6f) pic.IconQuad.TextureName = "fblock_castle_3";
                        else if (slider.MyFloat.Val > 3.2f) pic.IconQuad.TextureName = "fblock_castle_2";
                        else pic.IconQuad.TextureName = "fblock_castle_1";
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
                };
            AddItem(slider);
        }

        void StartLevel()
        {
            CustomLevel.StartLevelFromMenuData();
        }

        void Zero()
        {
            foreach (MenuItem item in MyMenu.Items)
            {
                MenuSlider slider = item as MenuSlider;
                if (null != slider)
                    slider.MyFloat.Val = 0;
            }
        }

        void Randomize()
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

        public virtual List<Upgrade> GetUpgradeList()
        {
            return new List<Upgrade>();
        }

        public override void Init()
        {
            base.Init();

            ScaleList = .73f;

            Control = -1;

            ReturnToCallerDelay = 6;
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;
            SlideInLength = 25;
            SlideOutLength = 20;

            // Make the menu
            MakeMenu();

            ItemPos = new Vector2(-950, 928 + 300 * (1 - ScaleList));
            PosAdd.Y *= 1.15f * ScaleList;

            // Add obstacles
            foreach (Upgrade upgrade in GetUpgradeList())
                AddUpgrade(upgrade);

            MyPile = new DrawPile();

            FontScale = 1f;
            MakeOptions();

            // Backdrop
            QuadClass backdrop;

            backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop, "Backdrop");
            backdrop.Size = new Vector2(1682.54f, 1107.681f);
            backdrop.Pos = new Vector2(347.2231f, 51.58749f);

            // Make the top text
            MakeTopText();

            EnsureFancy();

            // Header
            EzText LocationText = new EzText(HeaderText(), ItemFont);
            LocationText.Name = "Header";
            SetHeaderProperties(LocationText);
            MyPile.Add(LocationText);

            SetPos();
        }

        protected virtual string HeaderText()
        {
            return "";
        }

        void SetPos()
        {
#if PC_VERSION
            MenuItem _item;
            _item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(317.0639f, 22.30127f); }
            _item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(325.7295f, -155.4283f); }
            _item = MyMenu.FindItemByName("Reset"); if (_item != null) { _item.SetPos = new Vector2(324.1416f, -326.0634f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(327.3179f, -498.3018f); }

            MyMenu.Pos = new Vector2(-202.7773f, -122.2222f);

            EzText _t;
            _t = MyPile.FindEzText("TopText"); if (_t != null) { _t.Pos = new Vector2(508.8779f, 725f); }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }

            MyPile.Pos = new Vector2(-285f, 0f);

#else
            MenuItem _item;
            MyMenu.Pos = new Vector2(-202.7773f, -122.2222f);

            EzText _t;
            _t = MyPile.FindEzText("Start"); if (_t != null) { _t.Pos = new Vector2(323.017f, -78.88908f); }
            _t = MyPile.FindEzText("Random"); if (_t != null) { _t.Pos = new Vector2(470.5718f, -323.2856f); }
            _t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(629.6987f, -550.2858f); }
            _t = MyPile.FindEzText("TopText"); if (_t != null) { _t.Pos = new Vector2(773.5558f, 725f); }

            QuadClass _q;
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }

            MyPile.Pos = new Vector2(-285f, 0f);
#endif
        }

        private void MakeMenu()
        {
            MyMenu = new Menu(false);
            MyMenu.OnA = Cast.ToMenu(Go);
            MyMenu.OnB = MenuReturnToCaller;
            MyMenu.OnX = Cast.ToMenu(Randomize);
            MyMenu.OnY = Zero;
            MyMenu.SelectDelay = 11;
        }

        protected virtual void Go()
        {
        }

        protected void StartGame()
        {
            MyGame.PlayGame(() => StartLevel());
        }

        private void MakeOptions()
        {
            FontScale *= .8f;

#if PC_VERSION
            // Start
            MenuItem item;
            //MenuItem Start = item = new MenuItem(new EzText(ButtonString.Go(82) + " Start", ItemFont));
            MenuItem Start = item = new MenuItem(new EzText("Start", ItemFont));
            item.Name = "Start";
            item.Go = Cast.ToItem(Go);
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(425.3959f, -99.92095f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
            //item.AdditionalOnSelect = () => SetBerry("cb_enthusiastic");

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(Go);

            // Random
            //item = new MenuItem(new EzText(ButtonString.X(82) + " Random", ItemFont));
            item = new MenuItem(new EzText("Random", ItemFont));
            item.Name = "Random";
            item.Go = Cast.ToItem(Randomize);
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(511.8408f, -302.6506f);
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();
            //item.AdditionalOnSelect = () => SetBerry("cb_surprised");

            // Zero
            //item = new MenuItem(new EzText(ButtonString.Y(82) + " Zero", ItemFont));
            item = new MenuItem(new EzText("Reset", ItemFont));
            item.Name = "Reset";
            item.Go = Cast.ToItem(Zero);
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
            item.Name = "Back";
            item.Go = null;
            AddItem(item);
            item.SelectSound = null;
            item.Go = ItemReturnToCaller;
            item.Pos = item.SelectedPos = new Vector2(702.3179f, -689.9683f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            //item.AdditionalOnSelect = () => SetBerry("cb_crying");
#else
            EzText text;
            text = new EzText(ButtonString.Go(90) + " Start", ItemFont);
            text.Pos = new Vector2(417.4604f, -159.4446f);
            text.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            MyPile.Add(text, "Start");

            text = new EzText(ButtonString.X(90) + " Random", ItemFont);
            text.Pos = new Vector2(531.6831f, -389.9523f);
            text.MyFloatColor = new Color(204, 220, 255).ToVector4();
            MyPile.Add(text, "Random");

            text = new EzText(ButtonString.Back(90) + " Back", ItemFont);
            text.Pos = new Vector2(682.4761f, -622.5079f);
            text.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
            MyPile.Add(text, "Back");
#endif
        }

        private void MakeTopText()
        {
            TopText = new EzText("   ", ItemFont, true, true);
            SetTextProperties(TopText);
            TopText.Shadow = false;
            MyPile.Add(TopText, "TopText");
            TopText.Scale *= .83f;
            TopText.Pos = new Vector2(199.2069f, 746.0317f);
            TopText.Center();
        }

        protected override void MyDraw()
        {
            Camera cam = MyGame.MyLevel.MainCamera;

            base.MyDraw();

            if (BigIcon != null)
            {
                BigIcon.Draw(true);
            }
        }
    }
}