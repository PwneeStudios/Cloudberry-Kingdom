using Microsoft.Xna.Framework;
using CoreEngine;

namespace CloudberryKingdom
{
    public class ListSelectPanel : StartMenuBase
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
        int HoldIndex;

        public ListSelectPanel(int Control, string Header, CharacterSelect Parent, int ClrSelectIndex)
            : base(false)
        {
            this.MyCharacterSelect = Parent;
            this.ClrSelectIndex = ClrSelectIndex;
            this.Control = Control;

            this.Header = Header;

            HoldIndex = MyCharacterSelect.ItemIndex[ClrSelectIndex];

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

            SlideLength = 0;
            ReturnToCallerDelay = 0;

            MyPile = new DrawPile();
            MyMenu = new Menu(false);
            EnsureFancy();

            MyMenu.OnB = Cast.ToMenu(Back);
            MyMenu.OnA = Cast.ToMenu(Select);

            MyList = new MenuList();
            MyList.Name = "list";
            MyList.Center = true;
            MyList.OnIndexSelect = OnSelect;
            MyList.Go = Cast.ToItem(Select);
            MyMenu.Add(MyList);

            var Done = new MenuItem(new EzText("Use", ItemFont));
            Done.Name = "Done";
            Done.Go = Cast.ToItem(Select);
            AddItem(Done);

            //var BackButton = new MenuItem(new EzText("{pBackArrow2,80,?}", ItemFont));
            var BackButton = new MenuItem(new EzText("Cancel", ItemFont));
            BackButton.Name = "Cancel";
            BackButton.Go = Cast.ToItem(Back);
            AddItem(BackButton);

            MyPile.Add(new EzText(Header, Tools.Font_Grobold42, true), "Header");
            
            CharacterSelect.Shift(this);
        }

        public override void OnAdd()
        {
 	        base.OnAdd();
        
            MenuItem _item;
            _item = MyMenu.FindItemByName("list"); if (_item != null) { _item.SetPos = new Vector2(83.33337f, 153.1746f); _item.MyText.Scale = 0.375f; _item.MySelectedText.Scale = 0.375f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Done"); if (_item != null) { _item.SetPos = new Vector2(-177.4444f, 46.11105f); _item.MyText.Scale = 0.5535835f; _item.MySelectedText.Scale = 0.5535835f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Cancel"); if (_item != null) { _item.SetPos = new Vector2(-183.0001f, -79.44217f); _item.MyText.Scale = 0.5352504f; _item.MySelectedText.Scale = 0.5352504f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-1418.571f, -484.127f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(87.69827f, 534.1268f); _t.Scale = 1f; }
            MyPile.Pos = new Vector2(-1414.604f, -492.0635f);
        }

        void Back()
        {
            MyCharacterSelect.ItemIndex[ClrSelectIndex] = HoldIndex;
            MyCharacterSelect.Customize_UpdateColors();

            //MyMenu.BackSound.Play();
            ReturnToCaller();
        }

        void Select()
        {
            //MyMenu.SelectSound.Play();

            // Save new custom color scheme
            MyCharacterSelect.Player.CustomColorScheme = MyCharacterSelect.Player.ColorScheme;
            MyCharacterSelect.Player.ColorSchemeIndex = -1;

            //MyMenu.SelectSound.Play();
            ReturnToCaller();
        }

        protected override void MyPhsxStep()
        {
 	        base.MyPhsxStep();
        }
    }
}