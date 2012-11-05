using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    class CampaignChapterItem : MenuItem
    {
        public int Chapter = 0;

        public CampaignChapterItem(EzText Text, int Chapter)
            : base(Text)
        {
            this.Chapter = Chapter;
        }
    }

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

            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected void SetText(EzText text)
        {
            text.MyFloatColor = new Color(34, 214, 47).ToVector4();
            text.OutlineColor = new Color(0, 0, 0, 0).ToVector4();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            SetText(item.MyText);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
            item.MySelectedText.MyFloatColor = new Color(73, 255, 86).ToVector4(); 
            item.MySelectedText.OutlineColor = new Color(0, 0, 0, 0).ToVector4();

            //item.MyOscillateParams.Set(1f, 1.01f, .005f);
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

            MakeHeader();

            CreateMenu();
        }

        protected virtual void CreateMenu()
        {
            MenuItem item;

            // Chapter 1
            item = new CampaignChapterItem(new EzText(Localization.Words.TheBeginning, ItemFont), 1);
            item.Name = "MainCampaign";
            item.Go = Go;
            AddItem(item);

            // Chapter 2
            item = new CampaignChapterItem(new EzText(Localization.Words.TheNextNinetyNine, ItemFont), 2);
            item.Name = "Easy";
            item.Go = Go;
            AddItem(item);

            // Chapter 3
            item = new CampaignChapterItem(new EzText(Localization.Words.AGauntletOfDoom, ItemFont), 3);
            item.Name = "Hard";
            item.Go = Go;
            AddItem(item);

            // Chapter 4
            item = new CampaignChapterItem(new EzText(Localization.Words.AlmostHero, ItemFont), 4);
            item.Name = "Hardcore";
            item.Go = Go;
            AddItem(item);

            // Chapter 5
            item = new CampaignChapterItem(new EzText(Localization.Words.TheMasochist, ItemFont), 5);
            item.Name = "Maso";
            item.Go = Go;
            AddItem(item);

            //// Cinematics
            //item = new MenuItem(new EzText("Cinematics", ItemFont));
            //item.Name = "Cine";
            //item.Go = null;
            //AddItem(item);
            //item.MyText.MyFloatColor = new Color(241, 32, 117).ToVector4();
            //item.MySelectedText.MyFloatColor = new Color(251, 52, 137).ToVector4();

            MyMenu.SelectItem(0);

            //SetPos_NoCinematic();
            SetPos_WithCinematic();
        }

        protected void MakeHeader()
        {
            var Header = new EzText(Localization.Words.StoryMode, ItemFont);
            Header.Name = "Header";
            Header.Scale *= 1.3f;
            SetText(Header);
            Header.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Header);
            
            Header.Pos = new Vector2(-800.0029f, 863.8889f);
        }

        void Go(MenuItem item)
        {
            CampaignChapterItem c_item = item as CampaignChapterItem;
            if (null == c_item) return;

            Go(c_item.Chapter);
        }

        int _StartLevel;
        void Go(int StartLevel)
        {
            Tools.SongWad.FadeOut();
            MyGame.FadeToBlack(.0225f, 20);
            Active = false;

            _StartLevel = StartLevel;
            MyGame.WaitThenDo(75, _Go);
        }

        private void _Go()
        {
            Active = true;
            MyGame.FadeIn(.05f);
            CampaignSequence.Instance.Start(_StartLevel);
        }

        void SetPos_NoCinematic()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("MainCampaign"); if (_item != null) { _item.SetPos = new Vector2(686.4453f, 191.6667f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, 186.1112f)); }
            _item = MyMenu.FindItemByName("Easy"); if (_item != null) { _item.SetPos = new Vector2(708.665f, -36.44455f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Hard"); if (_item != null) { _item.SetPos = new Vector2(711.4443f, -239.5557f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Hardcore"); if (_item != null) { _item.SetPos = new Vector2(714.2227f, -437.111f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Maso"); if (_item != null) { _item.SetPos = new Vector2(730.8906f, -656.889f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }

            MyMenu.Pos = new Vector2(-783.3339f, 227.7778f);
        }

        void SetPos_WithCinematic()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("MainCampaign"); if (_item != null) { _item.SetPos = new Vector2(686.4453f, 191.6667f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, 186.1112f)); }
            _item = MyMenu.FindItemByName("Easy"); if (_item != null) { _item.SetPos = new Vector2(708.665f, -36.44455f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Hard"); if (_item != null) { _item.SetPos = new Vector2(711.4443f, -239.5557f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Hardcore"); if (_item != null) { _item.SetPos = new Vector2(714.2227f, -437.111f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Maso"); if (_item != null) { _item.SetPos = new Vector2(730.8906f, -656.889f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Cine"); if (_item != null) { _item.SetPos = new Vector2(733.6666f, -876.6666f); _item.MyText.Scale = 0.7373331f; _item.MySelectedText.Scale = 0.7373331f; _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }

            MyMenu.Pos = new Vector2(-783.3339f, 227.7778f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}