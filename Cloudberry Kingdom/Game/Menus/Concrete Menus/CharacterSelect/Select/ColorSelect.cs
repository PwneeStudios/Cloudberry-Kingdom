using Microsoft.Xna.Framework;
using Drawing;

namespace CloudberryKingdom
{
    public class ListSelectPanel : GUI_Panel
    {
        public MenuList MyList;
        CharacterSelect MyCharacterSelect;
        int ClrSelectIndex;

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        public void SetPos(Vector2 pos)
        {
            MyPile.Pos += pos;
            MyMenu.Pos += pos;
        }

        public void SetIndexViaAssociated(int index)
        {
            int CorrespondingIndex = MyList.MyList.FindIndex(item => (int)(item.MyObject) == index);
            if (CorrespondingIndex < 0) CorrespondingIndex = 0;
            MyList.SetIndex(CorrespondingIndex);
        }

        public int GetAssociatedIndex()
        {
            return (int)MyList.CurObj;
        }

        string Header;

        public ListSelectPanel(int Control, string Header, CharacterSelect Parent, int ClrSelectIndex)
            : base()
        {
            this.MyCharacterSelect = Parent;
            this.ClrSelectIndex = ClrSelectIndex;
            this.Control = Control;
            this.AutoDraw = false;

            this.Header = Header;

            Constructor();
        }

        void OnSelect()
        {
            MyCharacterSelect.ItemIndex[ClrSelectIndex] = GetAssociatedIndex();
            MyCharacterSelect.Customize_UpdateColors();
        }

        public override void Constructor()
        {
            base.Constructor();

            MyPile = new DrawPile();
            MyMenu = new Menu(false);
            EnsureFancy();

            MyMenu.OnB = Cast.ToMenu(Back);
            MyMenu.OnA = Cast.ToMenu(Select);

            MyList = new MenuList();
            MyList.Name = "list";
            MyList.Center = true;
            MyList.AdditionalOnSelect = OnSelect;

            MyMenu.Add(MyList);

            MyPile.Add(new EzText(Header, Tools.Font_Grobold42, true), "Header");
        }

        public override void OnAdd()
        {
 	        base.OnAdd();
        
            MenuItem _item;
            _item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(0f, 158.7301f); }

            MyMenu.Pos = new Vector2(-1418.571f, -484.127f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-3.968231f, 595.2379f); }
            MyPile.Pos = new Vector2(-1414.604f, -492.0635f);
        }

        void Back()
        {
            MyCharacterSelect.ItemIndex[ClrSelectIndex] = MyCharacterSelect.HoldIndex;
            MyMenu.BackSound.Play();
        }

        void Select()
        {
            MyMenu.SelectSound.Play();

            // Save new custom color scheme
            MyCharacterSelect.Player.CustomColorScheme = MyCharacterSelect.Player.ColorScheme;
            MyCharacterSelect.Player.ColorSchemeIndex = -1;
        }

        protected override void MyPhsxStep()
        {
 	         base.MyPhsxStep();
        }
    }
}