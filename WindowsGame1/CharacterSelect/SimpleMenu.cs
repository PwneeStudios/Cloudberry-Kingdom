using Microsoft.Xna.Framework;
using Drawing;
namespace CloudberryKingdom
{
#if PC_VERSION
    public class SimpleMenu : StartMenuBase
    {
        public FancyVector2 BobPos = new FancyVector2(), HeaderPos = new FancyVector2();

        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public SimpleMenu(int Control, CharacterSelect Parent) : base(false)
        {
            BobPos.RelVal = new Vector2(-464.2876f, 39.68256f);

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

            Control = -1;

            MyMenu.OnB = null;
            MenuItem item;

            // Customize
            item = new MenuItem(new EzText("Custom", ItemFont));
            item.Go = menuitem => Parent.SimpleToCustom();
            ItemPos = new Vector2(540f, 429.4446f);
            PosAdd = new Vector2(0, -220);
            AddItem(item);

            // Random
            item = new MenuItem(new EzText("Random", ItemFont));
            item.Go = menuitem => Parent.Randomize();
            AddItem(item);

            // Confirm
            item = new MenuItem(new EzText("Done", ItemFont));
            item.Go = menuitem => Parent.SimpleToBack();
            AddItem(item);

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

            ItemFont = Tools.Font_DylanThin42;
            //FontScale = .52f;
            FontScale = .7f;

            int ButtonSize = 95;

            // Press A to continue
            //EzText ContinueText = new EzText(ButtonString.Go(ButtonSize) + "{c188,255,176,255} to select", ItemFont, true, true);
            EzText ContinueText = new EzText(ButtonString.Go(ButtonSize) + "{c188,255,176,255} select", ItemFont, true, true);
            ContinueText.Scale = this.FontScale;
            //ContinueText.Shadow = true;
            ContinueText.ShadowOffset = new Vector2(7.5f, 7.5f);
            ContinueText.ShadowColor = new Color(30, 30, 30);
            //ContinueText.PicShadow = true;
            ContinueText.ColorizePics = true;
            ContinueText.Pos = new Vector2(23.09587f, -386.9842f);// new Vector2(94.52443f, -406.8254f);

            MyPile.Add(ContinueText);
            //ContinueText.AddBackdrop(new Vector2(.5f, .44f));
            ContinueText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(ContinueText);

            // Press Y to customize
            //EzText CustomizeText = new EzText(ButtonString.Y(ButtonSize) + "{c255,255,155,255} customize", ItemFont, true, true);
            EzText CustomizeText = new EzText(ButtonString.Y(ButtonSize) + "{c255,255,155,255} custom", ItemFont, true, true);
            CustomizeText.Scale = this.FontScale;
            //CustomizeText.Shadow = true;
            CustomizeText.ShadowOffset = new Vector2(7.5f, 7.5f);
            CustomizeText.ShadowColor = new Color(30, 30, 30);
            //CustomizeText.PicShadow = true;
            CustomizeText.ColorizePics = true;
            CustomizeText.Pos = new Vector2(105.2387f, -611.9048f);// new Vector2(125.08f, -600f);

            MyPile.Add(CustomizeText);
            //CustomizeText.AddBackdrop(new Vector2(.5f, .44f));
            CustomizeText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(CustomizeText);

            // Press X to randomize
            //EzText RandomText = new EzText(ButtonString.X(ButtonSize) + "{c194,210,255,255} Random", ItemFont, true, true);
            EzText RandomText = new EzText(ButtonString.X(ButtonSize) + "{c194,210,255,255} random", ItemFont, true, true);
            RandomText.Scale = this.FontScale;
            //RandomText.Shadow = true;
            RandomText.ShadowOffset = new Vector2(7.5f, 7.5f);
            RandomText.ShadowColor = new Color(30, 30, 30);
            //RandomText.PicShadow = true;
            RandomText.ColorizePics = true;
            RandomText.Pos = new Vector2(69.52449f, -835.7142f);// new Vector2(204.4452f, -800f);

            MyPile.Add(RandomText);
            //RandomText.AddBackdrop(new Vector2(.5f, .44f));
            RandomText.BackdropShift = new Vector2(-45, -6);
            ModBackdrop(RandomText);



            MyPile.Pos = new Vector2(-10, -37.5f - 15);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideOut(PresetPos.Bottom, 0);
        }
    }
#endif
}