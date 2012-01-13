using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ArrowMenu : StartMenuBase
    {
        CharacterSelect Parent;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            Parent = null;
        }

        public ArrowMenu(int Control, CharacterSelect Parent) : base(false)
        {
            this.Control = Control;
            this.Parent = Parent;

//#if NOT_PC
            this.AutoDraw = false;
//#endif
            Constructor();
            MyMenu.AffectsOutsideMouse = false;
        }

        public override void Init()
        {
            base.Init();

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.MouseOnly = true;
            Control = -1;

            MyMenu.OnB = null;
            MenuItem item;

            // The distance between the two arrows
#if PC_VERSION
            float SeparationWidth = 300;// 375;
#else
            float SeparationWidth = 300;
#endif

            // Left/Right arros
            Vector2 padding = new Vector2(0, 80);

            // Left arrow
            item = new MenuItem(new EzText("{pcharmenu_larrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            //item.MyOscillateParams.UseGlobalCount = true;
#if PC_VERSION
            item.Padding += padding;
#endif
            item.Go = menuitem => Parent.SimpleSelect_Left();
            ItemPos = new Vector2(-SeparationWidth, 250f);
            AddItem(item);

            // Right arrow
            item = new MenuItem(new EzText("{pcharmenu_rarrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            //item.MyOscillateParams.UseGlobalCount = true;
#if PC_VERSION
            item.Padding += padding;
#endif
            item.Go = menuitem => Parent.SimpleSelect_Right();
            ItemPos = new Vector2(SeparationWidth, 250f);
            AddItem(item);

            EnsureFancy();

#if PC_VERSION
            MyMenu.AffectsOutsideMouse = false;
            MyMenu.CheckForOutsideClick = false;
            MyMenu.FancyPos.RelVal =
                //new Vector2(-27.77734f, 88.88879f - 150 + 47);
                new Vector2(84.2063f, 235f) + new Vector2(3.968262f, -126.9841f);
#else
            MyMenu.FancyPos.RelVal = new Vector2(-62.5f, -15 + 250);
            //MyMenu.FancyPos.RelVal = new Vector2(-62.5f, 155.5f);
#endif
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        public override void OnAdd()
        {
            base.OnAdd();

#if PC_VERSION
            //SlideOut(PresetPos.Right, 0);
            SlideOut(PresetPos.Bottom, 0);
#else
            SlideOut(PresetPos.Bottom, 0);
#endif
        }
    }
}