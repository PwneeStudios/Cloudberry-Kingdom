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
            EzText _t;
            _t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(78.6516f, -159.2064f); _t.Scale = 0.5214169f; }
            _t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(80.23858f, -323.016f); _t.Scale = 0.5039168f; }
            _t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(80.63551f, -471.8254f); _t.Scale = 0.5540001f; }

            QuadClass _q;
            _q = MyPile.FindQuad("go"); if (_q != null) { _q.Pos = new Vector2(-261.1109f, -141.6667f); _q.Size = new Vector2(67.83331f, 67.83331f); }
            _q = MyPile.FindQuad("x"); if (_q != null) { _q.Pos = new Vector2(-261.1112f, -461.1111f); _q.Size = new Vector2(63.41657f, 63.41657f); }
            _q = MyPile.FindQuad("y"); if (_q != null) { _q.Pos = new Vector2(-263.8887f, -308.3333f); _q.Size = new Vector2(67.6666f, 67.6666f); }

            CharacterSelect.Shift(this);
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
