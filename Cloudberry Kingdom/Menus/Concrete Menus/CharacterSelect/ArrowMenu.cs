using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class ArrowMenu : StartMenuBase
    {
        CharacterSelect MyCharacterSelect;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public SimpleMenuBase MySimple;
        public ArrowMenu(int Control, CharacterSelect Parent, SimpleMenuBase MySimple)
            : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = Parent;
            this.MySimple = MySimple;

            Constructor();
            MyMenu.AffectsOutsideMouse = false;
        }

        public override void Init()
        {
            base.Init();

            SlideInLength = 0;
            SlideOutLength = 0;
            CallDelay = 0;
            ReturnToCallerDelay = 0;

            // Make the menu
            MyMenu = new Menu(false);

            MyMenu.MouseOnly = true;

            MyMenu.OnB = null;
            MenuItem item;

            // The distance between the two arrows
            float SeparationWidth = 300;

            // Left/Right arros
            Vector2 padding = new Vector2(0, 80);

            // Left arrow
            item = new MenuItem(new EzText("{pcharmenu_larrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            item.Go = Cast.ToItem(MySimple.SimpleSelect_Left);
            ItemPos = new Vector2(-SeparationWidth, 250f);
            AddItem(item);

            // Right arrow
            item = new MenuItem(new EzText("{pcharmenu_rarrow_1,70,?}", ItemFont));
            item.AlwaysDrawAsSelected = true;
            item.Go = Cast.ToItem(MySimple.SimpleSelect_Right);
            ItemPos = new Vector2(SeparationWidth, 250f);
            AddItem(item);

            EnsureFancy();

            MyMenu.FancyPos.RelVal = new Vector2(-62.5f, -15 + 250);
            CharacterSelect.Shift(this);
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

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;


        }
    }
}