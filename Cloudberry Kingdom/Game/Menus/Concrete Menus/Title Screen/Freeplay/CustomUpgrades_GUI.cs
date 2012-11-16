using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

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
            return new List<Upgrade>(new Upgrade[] { Upgrade.FlyBlob, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike, Upgrade.LavaDrip, Upgrade.Serpent, Upgrade.SpikeyLine, Upgrade.Fireball });
        }

        protected override void Go()
        {
            StartGame();
        }

        protected override Localization.Words HeaderText()
        {
            return Localization.Words.Aggressive;
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
            return new List<Upgrade>(new Upgrade[] { Upgrade.Jump, Upgrade.Speed, Upgrade.Ceiling, Upgrade.MovingBlock, Upgrade.GhostBlock, Upgrade.FallingBlock, Upgrade.Elevator, Upgrade.Cloud, Upgrade.BouncyBlock, Upgrade.Pendulum });
        }

        protected override void Go()
        {
            var UpgradeGui = new AggressiveUpgrades_GUI(PieceSeed, CustomLevel);
            Call(UpgradeGui, 0);
            Hide(PresetPos.Left);
            this.SlideInFrom = PresetPos.Left;
        }

        protected override Localization.Words HeaderText()
        {
            return Localization.Words.Passive;
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

        class BlockPieceSeedSetter : Lambda
        {
            CustomUpgrades_GUI gui;
            Upgrade upgrade;
            MenuSlider slider;

            public BlockPieceSeedSetter(CustomUpgrades_GUI gui, Upgrade upgrade,
                MenuSlider slider)
            {
                this.gui = gui;
                this.upgrade = upgrade;
                this.slider = slider;
            }

            public void Apply()
            {
                gui.PieceSeed.MyUpgrades1[upgrade] = slider.MyFloat.MyFloat;

                // Set the Fallingblock to an O-face when maxed out
                if (slider == gui.MyMenu.CurItem)
                {
                    PictureIcon pic = gui.BigIcon as PictureIcon;
                    if (slider.MyFloat.Val > 7.6f) pic.IconQuad.TextureName = "fblock_castle_3";
                    else if (slider.MyFloat.Val > 3.2f) pic.IconQuad.TextureName = "fblock_castle_2";
                    else pic.IconQuad.TextureName = "fblock_castle_1";
                }
            }
        }

        class PieceSeedSetter : Lambda
        {
            CustomUpgrades_GUI gui;
            Upgrade upgrade;
            MenuSlider slider;

            public PieceSeedSetter(CustomUpgrades_GUI gui, Upgrade upgrade, MenuSlider slider)
            {
                this.gui = gui;
                this.upgrade = upgrade;
                this.slider = slider;
            }

            public void Apply()
            {
                gui.PieceSeed.MyUpgrades1[upgrade] = slider.MyFloat.MyFloat;
            }
        }

        class AddUpgradeAdditionalOnSelect : Lambda
        {
            CustomUpgrades_GUI cuGui;
            MenuSlider slider;
            Upgrade upgrade;

            public AddUpgradeAdditionalOnSelect(CustomUpgrades_GUI cuGui, MenuSlider slider, Upgrade upgrade)
            {
                this.cuGui = cuGui;
                this.slider = slider;
                this.upgrade = upgrade;
            }

            public void Apply()
            {
                cuGui.TopText.SubstituteText(slider.Icon.DisplayText);
                cuGui.TopText.Pos = new Vector2(761 + 280, -46 + 771);
                cuGui.TopText.Center();

                cuGui.BigIcon = ObjectIcon.CreateIcon(upgrade, true);
                cuGui.BigIcon.SetScale(2f);
                cuGui.BigIcon.FancyPos.SetCenter(cuGui.Pos);
                cuGui.BigIcon.Pos = new Vector2(731f + 500 * (1 - cuGui.ScaleList), 198f);
                cuGui.BigIcon.MyOscillateParams.max_addition *= .25f;

                cuGui.TopText.Show = true;
            }
        }

        class UpgradesSliderLambda : LambdaFunc<float>
        {
            CustomUpgrades_GUI cu;
            Upgrade upgrade;
            public UpgradesSliderLambda(CustomUpgrades_GUI cu, Upgrade upgrade)
            {
                this.cu = cu;
                this.upgrade = upgrade;
            }

            public float Apply()
            {
                return cu.PieceSeed.MyUpgrades1[upgrade];
            }
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
            slider.MyFloat.GetCallback = new UpgradesSliderLambda(this, upgrade);
            
            // Keep the PieceSeed up to date when the slider changes
            if (upgrade == Upgrade.FallingBlock)
            {
                slider.MyFloat.SetCallback = new BlockPieceSeedSetter(
                    this, upgrade, slider
                );
            }
            else
                slider.MyFloat.SetCallback = new PieceSeedSetter(this, upgrade, slider);

            slider.AdditionalOnSelect = new AddUpgradeAdditionalOnSelect(this, slider, upgrade);
            AddItem(slider);
        }

        class StartLevelProxy : Lambda
        {
            CustomUpgrades_GUI cuGui;

            public StartLevelProxy(CustomUpgrades_GUI cuGui)
            {
                this.cuGui = cuGui;
            }

            public void Apply()
            {
                cuGui.StartLevel();
            }
        }

        void StartLevel()
        {
            CustomLevel.StartLevelFromMenuData();
        }

        class ZeroProxy : Lambda
        {
            CustomUpgrades_GUI cuGui;

            public ZeroProxy(CustomUpgrades_GUI cuGui)
            {
                this.cuGui = cuGui;
            }

            public void Apply()
            {
                cuGui.Zero();
            }
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

        class RandomizeProxy : Lambda
        {
            CustomUpgrades_GUI cuGui;

            public RandomizeProxy(CustomUpgrades_GUI cuGui)
            {
                this.cuGui = cuGui;
            }

            public void Apply()
            {
                cuGui.Randomize();
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

            ScaleList = .72f;

            Control = -1;

            ReturnToCallerDelay = 6;
            SlideInFrom = PresetPos.Right;
            SlideOutTo = PresetPos.Right;
            SlideInLength = 25;
            SlideOutLength = 20;

            // Make the menu
            MakeMenu();

            ItemPos = new Vector2(-950, 880);
            PosAdd.Y *= 1.18f * ScaleList;

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

        protected virtual Localization.Words HeaderText()
        {
            return Localization.Words.None;
        }

        void SetPos()
        {
#if PC_VERSION
            MenuItem _item;
            _item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(317.0639f, 22.30127f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(325.7295f, -155.4283f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Reset"); if (_item != null) { _item.SetPos = new Vector2(324.1416f, -326.0634f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(327.3179f, -498.3018f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-202.7773f, -122.2222f);

            EzText _t;
            _t = MyPile.FindEzText("TopText"); if (_t != null) { _t.Pos = new Vector2(508.8778f, 725f); _t.Scale = 0.664f; }
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-872.222f, 936.1112f); _t.Scale = 0.72f; }

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
            MyMenu.OnA = Cast.ToMenu(new GoProxy(this));
            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);
            MyMenu.OnX = Cast.ToMenu(new RandomizeProxy(this));
            MyMenu.OnY = new ZeroProxy(this);
            MyMenu.SelectDelay = 11;
        }

        class GoProxy : Lambda
        {
            CustomUpgrades_GUI cuGui;

            public GoProxy(CustomUpgrades_GUI cuGui)
            {
                this.cuGui = cuGui;
            }

            public void Apply()
            {
                cuGui.Go();
            }
        }

        protected virtual void Go()
        {
        }

        protected void StartGame()
        {
            MyGame.PlayGame(new StartLevelProxy(this));
        }

        private void MakeOptions()
        {
            FontScale *= .8f;

#if PC_VERSION
            // Start
            MenuItem item;
            MenuItem Start = item = new MenuItem(new EzText(Localization.Words.Start, ItemFont));
            item.Name = "Start";
            item.Go = Cast.ToItem(new GoProxy(this));
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(425.3959f, -99.92095f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedNextColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(new GoProxy(this));

            // Random
            item = new MenuItem(new EzText(Localization.Words.Random, ItemFont));
            item.Name = "Random";
            item.Go = Cast.ToItem(new RandomizeProxy(this));
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(511.8408f, -302.6506f);
            item.MyText.MyFloatColor = new Color(204, 220, 255).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(204, 220, 255).ToVector4();

            // Zero
            item = new MenuItem(new EzText(Localization.Words.Reset, ItemFont));
            item.Name = "Reset";
            item.Go = Cast.ToItem(new ZeroProxy(this));
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(599.1416f, -501.0634f);
            item.MyText.MyFloatColor = new Color(235, 255, 80).ToVector4() * .93f;
            item.MySelectedText.MyFloatColor = new Color(235, 255, 80).ToVector4();

            // Back
            item = new MenuItem(new EzText(Localization.Words.Back, ItemFont));
            item.Name = "Back";
            item.Go = null;
            AddItem(item);
            item.SelectSound = null;
            item.Go = new ItemReturnToCallerProxy(this);
            item.Pos = item.SelectedPos = new Vector2(702.3179f, -689.9683f);
            item.MyText.MyFloatColor = Menu.DefaultMenuInfo.UnselectedBackColor;
            item.MySelectedText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
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