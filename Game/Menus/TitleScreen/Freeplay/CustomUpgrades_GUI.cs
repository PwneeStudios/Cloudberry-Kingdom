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
            return new List<Upgrade>(new Upgrade[] { Upgrade.FlyBlob, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike, Upgrade.LavaDrip, Upgrade.Serpent, Upgrade.SpikeyLine, Upgrade.Fireball });
        }
		
        protected override void Go()
        {
			CloudberryKingdomGame.Freeplay_Count++;
			if (CloudberryKingdomGame.IsDemo && CloudberryKingdomGame.Freeplay_Count >= CloudberryKingdomGame.Freeplay_Max)
			{
				int HoldDelay = CallDelay;
				CallDelay = 0;
				Call(new UpSellMenu(Localization.Words.UpSell_FreePlay, MenuItem.ActivatingPlayer));
				Hide(PresetPos.Right, 0);
				CallDelay = HoldDelay;
				return;
			}

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

        Text TopText;

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

        protected override void SetHeaderProperties(Text text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
        }

		protected override void SetItemProperties(MenuItem item)
		{
			base.SetItemProperties(item);
			StartMenu.SetItemProperties_Red(item);
		}

        public ObjectIcon BigIcon;
        void AddUpgrade(Upgrade upgrade)
        {
            MenuSlider slider = new MenuSlider(new Text("", ItemFont));
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

            slider.AdditionalOnSelect = () => SetStuff(slider, upgrade);
            AddItem(slider);
        }

        void SetStuff(MenuSlider slider, Upgrade upgrade)
        {
            TopText.SubstituteText(slider.Icon.DisplayText);
            TopText.Pos = new Vector2(950, 0f);// new Vector2(761 + 280, -46 + 771);
            TopText.Scale = .5f;
            TopText.Center();

            BigIcon = ObjectIcon.CreateIcon(upgrade, true);
            //BigIcon.SetScale(2f);
            BigIcon.SetScale(1.55f);
            BigIcon.FancyPos.SetCenter(Pos);
            BigIcon.Pos = new Vector2(475f + 500 * (1 - ScaleList), 465f);
            BigIcon.MyOscillateParams.max_addition *= .25f;

            TopText.Show = true;
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

            // Backdrop
            QuadClass backdrop;

            backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            backdrop.Name = "Backdrop";
            MyPile.Add(backdrop, "Backdrop");
			EpilepsySafe(.5f);
            backdrop.Size = new Vector2(1682.54f, 1107.681f);
            backdrop.Pos = new Vector2(347.2231f, 51.58749f);

            // Options
            MakeOptions();

            // Make the top text
            MakeTopText();

            EnsureFancy();

            // Header
            Text LocationText = new Text(HeaderText(), ItemFont);
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
			if (true)
			//if (ButtonCheck.ControllerInUse)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(464.3106f, 22.30127f); _item.MyText.Scale = 0.7698284f; _item.MySelectedText.Scale = 0.7698284f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(464.3106f, -152.6501f); _item.MyText.Scale = 0.7698284f; _item.MySelectedText.Scale = 0.7698284f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Reset"); if (_item != null) { _item.SetPos = new Vector2(464.3106f, -327.6014f); _item.MyText.Scale = 0.7698284f; _item.MySelectedText.Scale = 0.7698284f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(464.3106f, -502.5527f); _item.MyText.Scale = 0.7698284f; _item.MySelectedText.Scale = 0.7698284f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-202.7773f, -122.2222f);

				Text _t;
				_t = MyPile.FindText("TopText"); if (_t != null) { _t.Pos = new Vector2(569.6667f, 0f); _t.Scale = 0.5f; }
				_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(-872.222f, 936.1112f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }
				_q = MyPile.FindQuad("Button_A"); if (_q != null) { _q.Pos = new Vector2(463.9663f, -252.8199f); _q.Size = new Vector2(82.41537f, 82.41537f); }
				_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(463.9663f, -427.8491f); _q.Size = new Vector2(82.41537f, 82.41537f); }
				_q = MyPile.FindQuad("Button_Y"); if (_q != null) { _q.Pos = new Vector2(463.9663f, -602.8783f); _q.Size = new Vector2(82.41537f, 82.41537f); }
				_q = MyPile.FindQuad("Button_B"); if (_q != null) { _q.Pos = new Vector2(463.9663f, -777.9075f); _q.Size = new Vector2(82.41537f, 82.41537f); }

				MyPile.Pos = new Vector2(-285f, 0f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Start"); if (_item != null) { _item.SetPos = new Vector2(317.0639f, 22.30127f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(325.7295f, -155.4283f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Reset"); if (_item != null) { _item.SetPos = new Vector2(324.1416f, -326.0634f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Back"); if (_item != null) { _item.SetPos = new Vector2(327.3179f, -498.3018f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-202.7773f, -122.2222f);

				Text _t;
				_t = MyPile.FindText("TopText"); if (_t != null) { _t.Pos = new Vector2(508.8778f, 725f); _t.Scale = 0.664f; }
				_t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(-872.222f, 936.1112f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(307.143f, -23.41241f); _q.Size = new Vector2(1741.167f, 1044.7f); }

				MyPile.Pos = new Vector2(-285f, 0f);
			}
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

            // Start
            MenuItem item;
            MenuItem Start = item = new MenuItem(new Text(Localization.Words.Start, ItemFont));
            item.Name = "Start";
            item.Go = Cast.ToItem(Go);
            item.JiggleOnGo = false;
            AddItem(item);
            item.Pos = item.SelectedPos = new Vector2(425.3959f, -99.92095f);

if (ButtonCheck.ControllerInUse)
{
			item.MyText.Shadow = item.MySelectedText.Shadow = false;
			Menu.DefaultMenuInfo.SetNext(item);
			MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_A"));
}
#if XBOX
			item.Selectable = false;
#endif

            // Select 'Start Level' when the user presses (A)
            MyMenu.OnA = Cast.ToMenu(Go);

            // Random
            item = new MenuItem(new Text(Localization.Words.Random, ItemFont));
            item.Name = "Random";
            item.Go = Cast.ToItem(Randomize);
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(511.8408f, -302.6506f);
if (ButtonCheck.ControllerInUse)
{
			item.MyText.Shadow = item.MySelectedText.Shadow = false;
			Menu.DefaultMenuInfo.SetX(item);
            MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));
}
#if XBOX
			item.Selectable = false;
#endif

            // Zero
            item = new MenuItem(new Text(Localization.Words.Reset, ItemFont));
            item.Name = "Reset";
            item.Go = Cast.ToItem(Zero);
            AddItem(item);
            item.SelectSound = null;
            item.Pos = item.SelectedPos = new Vector2(599.1416f, -501.0634f);
			//item.MyText.MyFloatColor = new Color(235, 255, 80).ToVector4() * .93f;
			//item.MySelectedText.MyFloatColor = new Color(235, 255, 80).ToVector4();
if (ButtonCheck.ControllerInUse)
{
			item.MyText.Shadow = item.MySelectedText.Shadow = false;
			Menu.DefaultMenuInfo.SetY(item);
            MyPile.Add(new QuadClass(ButtonTexture.Y, 90, "Button_Y"));
}
#if XBOX
			item.Selectable = false;
#endif

            // Back
            item = new MenuItem(new Text(Localization.Words.Back, ItemFont));
            item.Name = "Back";
            item.Go = null;
            AddItem(item);
            item.SelectSound = null;
            item.Go = ItemReturnToCaller;
            item.Pos = item.SelectedPos = new Vector2(702.3179f, -689.9683f);
if (ButtonCheck.ControllerInUse)
{
			item.MyText.Shadow = item.MySelectedText.Shadow = false;
			Menu.DefaultMenuInfo.SetBack(item);
            MyPile.Add(new QuadClass(ButtonTexture.Back, 90, "Button_B"));
}
#if XBOX
			item.Selectable = false;
#endif
		}

        private void MakeTopText()
        {
            TopText = new Text("   ", ItemFont, true, true);
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