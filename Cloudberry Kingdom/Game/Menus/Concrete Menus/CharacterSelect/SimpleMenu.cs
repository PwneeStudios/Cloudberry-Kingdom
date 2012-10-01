using Microsoft.Xna.Framework;
using System;
using Drawing;

namespace CloudberryKingdom
{
    public class SimpleMenuBase : StartMenuBase
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
            Call(new Waiting(Control, MyCharacterSelect));
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

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Selecting;
            MyCharacterSelect.MyDoll.ShowBob = true;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = true;
            MyCharacterSelect.MyHeroLevel.ShowHeroLevel = true;
            MyCharacterSelect.Player.Exists = true;

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
            item = new MenuItem(new EzText("Custom", ItemFont));
            item.Name = "Custom";
            item.Go = Cast.ToItem(SimpleToCustom);
            ItemPos = new Vector2(-523, -174);
            PosAdd = new Vector2(0, -220);
            AddItem(item);

            // Random
            item = new MenuItem(new EzText("Random", ItemFont));
            item.Name = "Random";
            item.Go = Cast.ToItem(MyCharacterSelect.Randomize);
            AddItem(item);

            // Confirm
            item = new MenuItem(new EzText("Done", ItemFont));
            item.Name = "Done";
            item.Go = Cast.ToItem(SimpleToDone);
            AddItem(item);

            // Select "Confirm" to start with
            MyMenu.SelectItem(item);

            MyMenu.OnB = Cast.ToMenu(SimpleToBack);

            // Backdrop
            QuadClass backdrop = new QuadClass("score screen", 485);
            backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "score screen";
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
            _item = MyMenu.FindItemByName("Custom"); if (_item != null) { _item.SetPos = new Vector2(-309.1112f, -87.88895f); _item.MyText.Scale = 0.6731667f; _item.MySelectedText.Scale = 0.6731667f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Random"); if (_item != null) { _item.SetPos = new Vector2(-336.8889f, -263.4445f); _item.MyText.Scale = 0.6948333f; _item.MySelectedText.Scale = 0.6948333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-239.6667f, -441.7778f); _item.MyText.Scale = 0.6962501f; _item.MySelectedText.Scale = 0.6962501f; _item.SelectIconOffset = new Vector2(0f, 0f); }

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

        void ModBackdrop(EzText Text)
        {
            if (Text.Backdrop == null) return;

            Text.BackdropModAlpha = .835f;
            Text.Backdrop.SetColor(new Color(255, 255, 255));
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            ItemFont = Tools.Font_Grobold42;
            FontScale = .66f;

            int ButtonSize = 95;

            string Space = "{s34,0}";
            float Shift = 25;

            // Press A to continue
            EzText ContinueText = new EzText(ButtonString.Go(ButtonSize) + Space + "{c188,255,176,255} Select", ItemFont, true, true);
            ContinueText.Scale = this.FontScale;
            ContinueText.ShadowOffset = new Vector2(7.5f, 7.5f);
            ContinueText.ShadowColor = new Color(30, 30, 30);
            ContinueText.ColorizePics = true;
            ContinueText.Pos = new Vector2(23.09587f + Shift, -386.9842f);

            MyPile.Add(ContinueText, "A");
            ContinueText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(ContinueText);

            // Press Y to customize
            EzText CustomizeText = new EzText(ButtonString.Y(ButtonSize) + Space + "{c255,255,155,255} Custom", ItemFont, true, true);
            CustomizeText.Scale = this.FontScale;
            CustomizeText.ShadowOffset = new Vector2(7.5f, 7.5f);
            CustomizeText.ShadowColor = new Color(30, 30, 30);
            CustomizeText.ColorizePics = true;
            CustomizeText.Pos = new Vector2(105.2387f + Shift, -611.9048f);

            MyPile.Add(CustomizeText, "Y");
            CustomizeText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(CustomizeText);

            // Press X to randomize
            EzText RandomText = new EzText(ButtonString.X(ButtonSize) + Space + "{c194,210,255,255} Random", ItemFont, true, true);
            RandomText.Scale = this.FontScale;
            RandomText.ShadowOffset = new Vector2(7.5f, 7.5f);
            RandomText.ShadowColor = new Color(30, 30, 30);
            RandomText.ColorizePics = true;
            RandomText.Pos = new Vector2(69.52449f + Shift, -835.7142f);

            MyPile.Add(RandomText, "X");
            RandomText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(RandomText);

            SetPos();
        }

        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText("A"); if (_t != null) { _t.Pos = new Vector2(45.31828f, -220.3175f); _t.Scale = 0.66f; }
            _t = MyPile.FindEzText("Y"); if (_t != null) { _t.Pos = new Vector2(119.1274f, -400.7938f); _t.Scale = 0.66f; }
            _t = MyPile.FindEzText("X"); if (_t != null) { _t.Pos = new Vector2(91.74666f, -599.6032f); _t.Scale = 0.66f; }

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
