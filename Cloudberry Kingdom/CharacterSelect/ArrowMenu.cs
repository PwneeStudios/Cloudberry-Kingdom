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
            float SeparationWidth = 300;

            // Left/Right arros
            Vector2 padding = new Vector2(0, 80);

            // Left arrow
            item = new MenuItem(new EzText("{pcharmenu_larrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            //item.MyOscillateParams.UseGlobalCount = true;
            item.Go = menuitem => Parent.SimpleSelect_Left();
            ItemPos = new Vector2(-SeparationWidth, 250f);
            AddItem(item);

            // Right arrow
            item = new MenuItem(new EzText("{pcharmenu_rarrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            //item.MyOscillateParams.UseGlobalCount = true;
            item.Go = menuitem => Parent.SimpleSelect_Right();
            ItemPos = new Vector2(SeparationWidth, 250f);
            AddItem(item);

            EnsureFancy();

            MyMenu.FancyPos.RelVal = new Vector2(-62.5f, -15 + 250);
            //MyMenu.FancyPos.RelVal = new Vector2(-62.5f, 155.5f);
        }

        protected override void MyDraw()
        {
            base.MyDraw();
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideOut(PresetPos.Bottom, 0);
        }
    }
}