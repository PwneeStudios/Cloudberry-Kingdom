using Microsoft.Xna.Framework;
using System;
using CoreEngine;

namespace CloudberryKingdom
{
    public class SimpleMenuBase : CkBaseMenu
    {
        protected CharacterSelect MyCharacterSelect;
        ArrowMenu Arrows;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public SimpleMenuBase(int Control, CharacterSelect Parent) : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = Parent;

            Constructor();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            Arrows = new ArrowMenu(Control, MyCharacterSelect, this);
            MyGame.AddGameObject(Arrows);
        }

        Vector2 PrevDir;
        int NoMoveCount;
        static int NoMoveDuration = 20;

        public void SimpleToCustom()
        {
            Call(new CustomizeMenu(Control, MyCharacterSelect));
            Hide();
        }

        public void SimpleToDone()
        {
            SkipCallSound = true;
            Call(new Waiting(Control, MyCharacterSelect, true));
            Hide();
        }

        public void SimpleToBack()
        {
            ReturnToCaller(true);
        }

        void ButtonPhsx()
        {
            if (!Tools.StepControl)
            {
                if (ButtonCheck.State(ControllerButtons.Y, MyCharacterSelect.PlayerIndex).Pressed)
                    SimpleToCustom();

                if (ButtonCheck.State(ControllerButtons.A, MyCharacterSelect.PlayerIndex).Pressed)
                    SimpleToDone();

                if (ButtonCheck.State(ControllerButtons.B, MyCharacterSelect.PlayerIndex).Pressed)
                    SimpleToBack();

                if (ButtonCheck.State(ControllerButtons.X, MyCharacterSelect.PlayerIndex).Pressed)
                    MyCharacterSelect.Randomize();
            }
        }

        /// <summary>
        /// Select the next premade stickman to the right
        /// </summary>    
        public void SimpleSelect_Right()
        {
            int i = MyCharacterSelect.Player.ColorSchemeIndex;

            do
            {
                i++;
                if (i >= ColorSchemeManager.ColorSchemes.Count)
                {
                    if (MyCharacterSelect.HasCustom())
                        i = -1;
                    else
                        i = 0;
                    break;
                }
            }
            while (!MyCharacterSelect.AvailableColorScheme(ColorSchemeManager.ColorSchemes[i]));
            MyCharacterSelect.Player.ColorSchemeIndex = i;

            // Jiggle the arrow
            Arrows.MyMenu.Items[1].DoActivationAnimation();

            MyCharacterSelect.SetIndex(MyCharacterSelect.Player.ColorSchemeIndex);
        }

        /// <summary>
        /// Select the next premade stickman to the left
        /// </summary>
        public void SimpleSelect_Left()
        {
            int i = MyCharacterSelect.Player.ColorSchemeIndex;
            do
            {
                i--;
                if (i <= 0) break;
            }
            while (!MyCharacterSelect.AvailableColorScheme(ColorSchemeManager.ColorSchemes[i]));
            MyCharacterSelect.Player.ColorSchemeIndex = i;

            // Jiggle the arrow
            Arrows.MyMenu.Items[0].DoActivationAnimation();
            
            int StartIndex = 0;
            if (MyCharacterSelect.HasCustom()) StartIndex = -1;
            if (MyCharacterSelect.Player.ColorSchemeIndex < StartIndex)
                MyCharacterSelect.Player.ColorSchemeIndex = ColorSchemeManager.ColorSchemes.Count - 1;

            MyCharacterSelect.SetIndex(MyCharacterSelect.Player.ColorSchemeIndex);
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            
            if (MyCharacterSelect.MyState != CharacterSelect.SelectState.Selecting)
            {
                MyCharacterSelect.Player.Exists = true;
            }

            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;


			if (Active && !MyCharacterSelect.Player.Exists) { ReturnToCaller(false); return; }

            // Buttons
            ButtonPhsx();

            // Left/Right
            Vector2 Dir = ButtonCheck.GetDir(MyCharacterSelect.PlayerIndex);
            if (NoMoveCount > 0)
                NoMoveCount--;

            if (Math.Abs(Dir.X - PrevDir.X) > ButtonCheck.ThresholdSensitivity || NoMoveCount == 0)
            {
                if (Dir.X > ButtonCheck.ThresholdSensitivity)
                    SimpleSelect_Right();
                else if (Dir.X < -ButtonCheck.ThresholdSensitivity)
                    SimpleSelect_Left();

                NoMoveCount = NoMoveDuration;
            }
            PrevDir = Dir;
        }
    }

#if PC_VERSION
    public class SimpleMenu : SimpleMenuBase
    {
        public SimpleMenu(int Control, CharacterSelect Parent) : base(Control, Parent)
        {
        }

        protected override void SetItemProperties(MenuItem item)
        {
            item.MyText.Scale = item.MySelectedText.Scale = FontScale;

            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Make the menu
            MyMenu = new Menu(false);
			MyMenu.UseMouseAndKeyboard = false;
            MyMenu.Control = Control;

            MyMenu.OnB = null;
            MenuItem item;

            // Customize
            item = new MenuItem(new EzText(Localization.Words.Custom, ItemFont, true));
            item.Name = "Custom";
            item.Go = Cast.ToItem(SimpleToCustom);
            ItemPos = new Vector2(-523, -174);
            PosAdd = new Vector2(0, -220);
            AddItem(item);

            // Random
            item = new MenuItem(new EzText(Localization.Words.Random, ItemFont, true));
            item.Name = "Random";
            item.Go = Cast.ToItem(MyCharacterSelect.Randomize);
            AddItem(item);

            // Confirm
            item = new MenuItem(new EzText(Localization.Words.Done, ItemFont, true));
            item.Name = "Done";
            item.Go = Cast.ToItem(SimpleToDone);
            AddItem(item);

            // Select "Confirm" to start with
            MyMenu.SelectItem(item);

            MyMenu.OnB = Cast.ToMenu(SimpleToBack);

            // Backdrop
            QuadClass backdrop = new QuadClass("Score_Screen", 485);
            backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "Score_Screen";
            backdrop.ScaleYToMatchRatio(485);

            backdrop.Pos = new Vector2(1198.412f, -115.0794f);
            backdrop.Size = new Vector2(647.6985f, 537.2521f);

            MyPile.Pos = new Vector2(0, 55 - 27);

            EnsureFancy();
            MyMenu.FancyPos.RelVal = new Vector2(163.8887f, -55.55554f + 55 - 27);

            // Don't draw mouse back icon if we are over the arrow menu
            //MyMenu.AdditionalCheckForOutsideClick += () => MyCharacterSelect.Arrows.MyMenu.HitTest();

            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Custom"); if (_item != null) { _item.SetPos = new Vector2(0, -87.88895f); _item.MyText.Scale = 0.6731667f; _item.MySelectedText.Scale = 0.6731667f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(0, -263.4445f); _item.MyText.Scale = 0.6948333f; _item.MySelectedText.Scale = 0.6948333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(0, -441.7778f); _item.MyText.Scale = 0.6962501f; _item.MySelectedText.Scale = 0.6962501f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(0, 0);
            MyPile.Pos = new Vector2(0, 0);

            CharacterSelect.Shift(this);
        }
    }
#else
    public class SimpleMenu : SimpleMenuBase
    {
        public SimpleMenu(int Control, CharacterSelect Parent) : base(Control, Parent)
        {
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            ItemFont = Resources.Font_Grobold42;
            FontScale = .66f;

            int ButtonSize = 95;

            string Space = "{s34,0}";
            float Shift = 25;

            QuadClass q;

            // Press A to continue
            q = new QuadClass(ButtonTexture.Go);
            MyPile.Add(q, "go");

            EzText ContinueText = new EzText(Localization.Words.Continue, ItemFont, true, true);
            ContinueText.Scale = this.FontScale;
            ContinueText.ShadowOffset = new Vector2(7.5f, 7.5f);
            ContinueText.ShadowColor = new Color(30, 30, 30);
            ContinueText.ColorizePics = true;
            ContinueText.Pos = new Vector2(23.09587f + Shift, -386.9842f);

            MyPile.Add(ContinueText, "A");

            // Press Y to customize
            q = new QuadClass(ButtonTexture.Y);
            MyPile.Add(q, "y");

            EzText CustomizeText = new EzText(Localization.Words.Custom, ItemFont, true, true);
            CustomizeText.Scale = this.FontScale;
            CustomizeText.ShadowOffset = new Vector2(7.5f, 7.5f);
            CustomizeText.ShadowColor = new Color(30, 30, 30);
            CustomizeText.ColorizePics = true;
            CustomizeText.Pos = new Vector2(105.2387f + Shift, -611.9048f);

            MyPile.Add(CustomizeText, "Y");

            // Press X to randomize
            q = new QuadClass(ButtonTexture.X);
            MyPile.Add(q, "x");

            EzText RandomText = new EzText(Localization.Words.Random, ItemFont, true, true);
            RandomText.Scale = this.FontScale;
            RandomText.ShadowOffset = new Vector2(7.5f, 7.5f);
            RandomText.ShadowColor = new Color(30, 30, 30);
            RandomText.ColorizePics = true;
            RandomText.Pos = new Vector2(69.52449f + Shift, -835.7142f);

            MyPile.Add(RandomText, "X");

            SetPos();
        }

        void SetPos()
		{
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(81.55566f, -139.7619f); _t.Scale = 0.6048333f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(128.7777f, -314.6826f); _t.Scale = 0.5580834f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(84.33325f, -491.27f); _t.Scale = 0.5925835f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-175f, -141.6667f); _q.Size = new Vector2(67.6666f, 67.6666f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-177.7776f, -319.4444f); _q.Size = new Vector2(67.6666f, 67.6666f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-174.9998f, -494.4444f); _q.Size = new Vector2(67.6666f, 67.6666f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(84.33325f, -148.0953f); _t.Scale = 0.5214169f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(31.55566f, -311.9048f); _t.Scale = 0.5206664f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(64.88892f, -471.8255f); _t.Scale = 0.5540001f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -141.6667f); _q.Size = new Vector2(69.08327f, 69.08327f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -308.3333f); _q.Size = new Vector2(68.99995f, 68.99995f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -466.6667f); _q.Size = new Vector2(67.6666f, 67.6666f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(42.54058f, -164.762f); _t.Scale = 0.4098337f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(99.68316f, -314.6826f); _t.Scale = 0.3790001f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(38.9688f, -466.2699f); _t.Scale = 0.4091668f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-244.4442f, -163.8889f); _q.Size = new Vector2(61.49994f, 61.49994f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-244.4441f, -316.6666f); _q.Size = new Vector2(60.58324f, 60.58324f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-241.6666f, -469.4445f); _q.Size = new Vector2(61.4999f, 61.4999f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(78.6516f, -159.2064f); _t.Scale = 0.5214169f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(85.79449f, -325.7938f); _t.Scale = 0.5118334f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(86.19106f, -491.2698f); _t.Scale = 0.5290835f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-261.1109f, -152.7778f); _q.Size = new Vector2(67.83331f, 67.83331f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-258.3333f, -324.9999f); _q.Size = new Vector2(67.58327f, 67.58327f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-252.778f, -486.1111f); _q.Size = new Vector2(66.91656f, 66.91656f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(34.20775f, -150.8731f); _t.Scale = 0.4363339f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(85.7934f, -284.1271f); _t.Scale = 0.4254172f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(30.63515f, -413.4923f); _t.Scale = 0.4363339f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-263.8889f, -150.0001f); _q.Size = new Vector2(59.99992f, 59.99992f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-263.8885f, -283.3335f); _q.Size = new Vector2(59.99992f, 59.99992f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-266.667f, -413.8891f); _q.Size = new Vector2(59.99992f, 59.99992f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(73.09618f, -161.9842f); _t.Scale = 0.535f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(96.90533f, -325.7938f); _t.Scale = 0.535f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(100.0798f, -491.2699f); _t.Scale = 0.535f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-169.4442f, -161.1112f); _q.Size = new Vector2(72.24997f, 72.24997f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-169.4444f, -327.7777f); _q.Size = new Vector2(72.24997f, 72.24997f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-169.4445f, -491.6667f); _q.Size = new Vector2(72.24997f, 72.24997f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(34.20727f, -161.9842f); _t.Scale = 0.535f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(84.20727f, -336.9049f); _t.Scale = 0.535f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(36.98534f, -497.9367f); _t.Scale = 0.535f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-169.4442f, -161.1112f); _q.Size = new Vector2(72.24997f, 72.24997f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-169.4444f, -327.7777f); _q.Size = new Vector2(72.24997f, 72.24997f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-169.4445f, -491.6667f); _q.Size = new Vector2(72.24997f, 72.24997f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(-10.2374f, -164.762f); _t.Scale = 0.4177502f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(96.90533f, -317.4604f); _t.Scale = 0.389917f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(13.9689f, -466.2699f); _t.Scale = 0.40025f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-244.444f, -161.1111f); _q.Size = new Vector2(62.49989f, 62.49989f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-244.4444f, -313.8889f); _q.Size = new Vector2(61.9999f, 61.9999f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-244.4445f, -469.4445f); _q.Size = new Vector2(63.41657f, 63.41657f); }
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(103.6518f, -148.0953f); _t.Scale = 0.3990837f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(88.57208f, -298.0161f); _t.Scale = 0.4368334f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(27.8578f, -452.381f); _t.Scale = 0.4505002f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-222.2217f, -144.4445f); _q.Size = new Vector2(67.83331f, 67.83331f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-222.222f, -294.4445f); _q.Size = new Vector2(67.83317f, 67.83317f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-222.222f, -450f); _q.Size = new Vector2(66.41657f, 66.41657f); }
			}
			else
			{
				EzText _t;
				_t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(76f, -148.0953f); _t.Scale = 0.5214169f; }
				_t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(87.11108f, -309.1271f); _t.Scale = 0.5048334f; }
				_t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(76f, -469.0477f); _t.Scale = 0.5540001f; }

				QuadClass _q;
				_q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -141.6667f); _q.Size = new Vector2(67.6666f, 67.6666f); }
				_q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -302.7778f); _q.Size = new Vector2(67.6666f, 67.6666f); }
				_q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-261.1111f, -461.1111f); _q.Size = new Vector2(67.6666f, 67.6666f); }
			}

			CharacterSelect.Shift(this);

			// Squeeze
			//MyPile.Pos += new Vector2(-8, 0);
			//MyPile.Scale(.65f);
		}

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
        }
    }
#endif
}
