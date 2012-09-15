using Microsoft.Xna.Framework;
using Drawing;
namespace CloudberryKingdom
{
#if PC_VERSION
    public class SimpleMenu : StartMenuBase
    {
        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public SimpleMenu(int Control, CharacterSelect Parent) : base(false)
        {
            this.Control = Control;
            this.Parent = Parent;

            this.AutoDraw = false;

            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            item.MyText.Scale = item.MySelectedText.Scale = FontScale;

            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            // Make the menu
            MyMenu = new Menu(false);
            MyMenu.Control = Control;
            //Control = -1;

            MyMenu.OnB = null;
            MenuItem item;

            // Customize
            item = new MenuItem(new EzText("Custom", ItemFont));
            item.Go = menuitem => Parent.SimpleToCustom();
            ItemPos = new Vector2(-523, -174);
            PosAdd = new Vector2(0, -220);
            AddItem(item);

            // Random
            item = new MenuItem(new EzText("Random", ItemFont));
            item.Go = menuitem => Parent.Randomize();
            AddItem(item);


            // Confirm
            item = new MenuItem(new EzText("Done", ItemFont));
            item.Go = menuitem =>
                //Parent.SimpleToBack();
                Parent.SetState(SelectState.Waiting);
            AddItem(item);

            // Select "Confirm" to start with
            MyMenu.SelectItem(item);

            MyMenu.OnB = menu => { Parent.SimpleToBack(); return false; };

            // Backdrop
            QuadClass backdrop = new QuadClass("score screen", 485);
            backdrop = new QuadClass(null, true, false);
            backdrop.TextureName = "score screen";
            backdrop.ScaleYToMatchRatio(485);

            backdrop.Pos = new Vector2(1198.412f, -115.0794f);// new Vector2(1194.444f, 15.87314f);
            backdrop.Size = new Vector2(647.6985f, 537.2521f);// new Vector2(679.4447f, 445.9821f);

            //backdrop.Size = backdrop.Size * new Vector2(1f, 2.03f);
            //MyPile.Add(backdrop);
            //MyPile.Add(backdrop); // Add twice to counter transparency

            MyPile.Pos = new Vector2(0, 55 - 27);

            EnsureFancy();
            MyMenu.FancyPos.RelVal = new Vector2(163.8887f, -55.55554f + 55 - 27);

            // Don't draw mouse back icon if we are over the arrow menu
            MyMenu.AdditionalCheckForOutsideClick += () => Parent.Arrows.MyMenu.HitTest();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideOut(PresetPos.Bottom, 0);
            //SlideOut(PresetPos.Right, 0);
        }
    }
#else
    public class SimpleMenu : StartMenuBase
    {
        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public SimpleMenu(int Control, CharacterSelect Parent)
            : base(false)
        {
            this.Control = Control;
            this.Parent = Parent;

#if NOT_PC
            this.AutoDraw = false;
#endif
            Constructor();
        }

        void ModBackdrop(EzText Text)
        {
            if (Text.Backdrop == null) return;

            //Text.BackdropModAlpha = .6f;
            //Text.Backdrop.SetColor(new Color(200, 200, 200));

            Text.BackdropModAlpha = .835f;
            Text.Backdrop.SetColor(new Color(255, 255, 255));
        }

        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            MyPile.FancyPos.UpdateWithGame = true;

            //ItemFont = Tools.Font_Grobold42;
            //FontScale = .7f;
            ItemFont = Tools.Font_Grobold42;
            FontScale = .66f;

            int ButtonSize = 95;

            string Space = "{s34,0}";
            float Shift = 25;

            // Press A to continue
            EzText ContinueText = new EzText(ButtonString.Go(ButtonSize) + Space + "{c188,255,176,255} select", ItemFont, true, true);
            ContinueText.Scale = this.FontScale;
            ContinueText.ShadowOffset = new Vector2(7.5f, 7.5f);
            ContinueText.ShadowColor = new Color(30, 30, 30);
            ContinueText.ColorizePics = true;
            ContinueText.Pos = new Vector2(23.09587f + Shift, -386.9842f);

            MyPile.Add(ContinueText, "A");
            ContinueText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(ContinueText);

            // Press Y to customize
            EzText CustomizeText = new EzText(ButtonString.Y(ButtonSize) + Space + "{c255,255,155,255} custom", ItemFont, true, true);
            CustomizeText.Scale = this.FontScale;
            CustomizeText.ShadowOffset = new Vector2(7.5f, 7.5f);
            CustomizeText.ShadowColor = new Color(30, 30, 30);
            CustomizeText.ColorizePics = true;
            CustomizeText.Pos = new Vector2(105.2387f + Shift, -611.9048f);

            MyPile.Add(CustomizeText, "Y");
            CustomizeText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(CustomizeText);

            // Press X to randomize
            EzText RandomText = new EzText(ButtonString.X(ButtonSize) + Space + "{c194,210,255,255} random", ItemFont, true, true);
            RandomText.Scale = this.FontScale;
            RandomText.ShadowOffset = new Vector2(7.5f, 7.5f);
            RandomText.ShadowColor = new Color(30, 30, 30);
            RandomText.ColorizePics = true;
            RandomText.Pos = new Vector2(69.52449f + Shift, -835.7142f);

            MyPile.Add(RandomText, "X");
            RandomText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(RandomText);

            MyPile.Pos = new Vector2(-10, -37.5f - 15) + new Vector2(11.90472f, 142.8571f);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideOut(PresetPos.Bottom, 0);
        }
    }
#endif
}