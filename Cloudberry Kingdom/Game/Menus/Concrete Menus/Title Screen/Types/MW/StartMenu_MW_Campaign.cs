using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Campaign : StartMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_Campaign(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Princess);
            //Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_NoBob_Brighten);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;

            item.MyText.MyFloatColor = new Color(34, 214, 47).ToVector4();
            //item.MyText.OutlineColor = new Color(0, 0, 0).ToVector4();
            item.MyText.OutlineColor = new Color(0, 0, 0, 0).ToVector4();

            item.MySelectedText.MyFloatColor = new Color(73, 255, 86).ToVector4(); 
            //item.MySelectedText.OutlineColor = new Color(0, 0, 0).ToVector4();
            item.MySelectedText.OutlineColor = new Color(0, 0, 0, 0).ToVector4();

            //item.JiggleOnGo = false;
            //item.MyOscillateParams.MyType = OscillateParams.Type.GetBig;

            item.MyOscillateParams.Set(1f, 1.01f, .005f);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            MyMenu.ClearList();

            var Header = new MenuItem(new EzText("STORY MODE", ItemFont));
            Header.Name = "Header";
            Header.ScaleText(1.3f);
            SetItemProperties(Header);
            Header.MyText.OutlineColor = Color.Black.ToVector4();
            Header.Selectable = false;
            MyMenu.Add(Header);

            MenuItem item;

            // Story of the Orb
            item = new MenuItem(new EzText("The Beginning", ItemFont));
            item.Name = "MainCampaign";
            item.Go = null;
            AddItem(item);

            // Easy
            item = new MenuItem(new EzText("The Next Ninety-Nine", ItemFont));
            item.Name = "Easy";
            item.Go = null;
            AddItem(item);

            // Hard
            item = new MenuItem(new EzText("A Gauntlet of Doom", ItemFont));
            item.Name = "Hard";
            item.Go = null;
            AddItem(item);

            // Hardcore
            item = new MenuItem(new EzText("Almost Hero", ItemFont));
            item.Name = "Hardcore";
            item.Go = null;
            AddItem(item);

            // Masochistic
            item = new MenuItem(new EzText("The Masochist", ItemFont));
            item.Name = "Maso";
            item.Go = null;
            AddItem(item);

            MyMenu.SelectItem(1);
            SetPos();
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-13.89111f, 700f); _item.MyText.Scale = 1.241666f; _item.MySelectedText.Scale = 0.66f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("MainCampaign"); if (_item != null) { _item.SetPos = new Vector2(686.4453f, 191.6667f); _item.MyText.Scale = 0.85f; _item.MySelectedText.Scale = 0.8487501f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SelectedPos = new Vector2(622.5566f, 186.1112f); }
            _item = MyMenu.FindItemByName("Easy"); if (_item != null) { _item.SetPos = new Vector2(708.665f, -36.44455f); _item.MyText.Scale = 0.85f; _item.MySelectedText.Scale = 0.85f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SelectedPos = new Vector2(622.5566f, -36.44455f); }
            _item = MyMenu.FindItemByName("Hard"); if (_item != null) { _item.SetPos = new Vector2(711.4443f, -239.5557f); _item.MyText.Scale = 0.85f; _item.MySelectedText.Scale = 0.85f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SelectedPos = new Vector2(622.5566f, -239.5557f); }
            _item = MyMenu.FindItemByName("Hardcore"); if (_item != null) { _item.SetPos = new Vector2(714.2227f, -437.111f); _item.MyText.Scale = 0.85f; _item.MySelectedText.Scale = 0.85f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SelectedPos = new Vector2(622.5566f, -437.111f); }
            _item = MyMenu.FindItemByName("Maso"); if (_item != null) { _item.SetPos = new Vector2(730.8906f, -656.889f); _item.MyText.Scale = 0.85f; _item.MySelectedText.Scale = 0.85f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SelectedPos = new Vector2(622.5566f, -656.889f); }

            MyMenu.Pos = new Vector2(-852.7783f, 213.8889f);

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}