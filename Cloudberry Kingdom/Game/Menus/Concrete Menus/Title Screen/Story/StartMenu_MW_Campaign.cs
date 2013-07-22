using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    class CampaignChapterItem : MenuItem
    {
        public int Chapter = 0;
        public bool Locked = false;

        public CampaignChapterItem(EzText Text, int Chapter)
            : base(Text)
        {
            this.Chapter = Chapter;

            UpdateLock();
        }

        public void UpdateLock()
        {
            Locked = false;
            if (!CloudberryKingdomGame.Unlock_Levels)
            {
				int level = PlayerManager.MinPlayerTotalCampaignLevel();
				if (CampaignSequence.Instance.ChapterEnd.ContainsKey(Chapter - 1))
					Locked = level < CampaignSequence.Instance.ChapterEnd[Chapter - 1];
            }
        }
    }

    public class StartMenu_MW_Campaign : StartMenu
    {
        public override void OnReturnTo()
        {
            base.OnReturnTo();

            Update();
        }

#if PC_VERSION
		ClickableBack Back;
#endif

        void Update()
        {
			EzText _t;

            // Update level text
            int Level = PlayerManager.MinPlayerTotalCampaignLevel() + 1;
			bool ShowLevel = Level > 1;
            bool ShowContinue = Level > 1 && Level < 321;

			string template_level = Localization.WordString(Localization.Words.Continue);


			MenuItem __item = MyMenu.FindItemByName("Continue");
			if (__item != null)
			{
                if (ShowContinue)
				{
					__item.Selectable = true;
					__item.Show = true;
				}
				else
				{
					if (Level == 0) Level = 1;
					__item.Selectable = false;
					__item.Show = false;
					MyMenu.SelectItem(1);
				}

				__item.MyText.SubstituteText(string.Format(template_level, Level));
				__item.MySelectedText.SubstituteText(string.Format(template_level, Level));
			}

			if (ShowLevel)
            {
                MyPile.FindEzText("Level").Show = true;
                MyPile.FindQuad("BoxLeft").Show = true;

                _t = MyPile.FindEzText("LevelNum");
                _t.Show = true;
                _t.SubstituteText(Level.ToString());
            }
            else
            {
                MyPile.FindEzText("Level").Show = false;
                MyPile.FindEzText("LevelNum").Show = false;
                MyPile.FindQuad("BoxLeft").Show = false;
            }


            // Update menu items (faded if locked)
            foreach (MenuItem _item in MyMenu.Items)
            {
                CampaignChapterItem item = _item as CampaignChapterItem;
                if (null != item)
                {
                    item.UpdateLock();

                    if (item.Locked)
                    {
                        item.MyText.Alpha = .4f;
                        item.MySelectedText.Alpha = .4f;
                    }
                    else
                    {
                        item.MyText.Alpha = 1f;
                        item.MySelectedText.Alpha = 1f;
                    }
                }
            }
        }

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

			MyMenu.Control = -1;

            Update();

			int Level = PlayerManager.MinPlayerTotalCampaignLevel() + 1;
			bool ShowLevel = Level > 1;
			if (ShowLevel)
				MyMenu.SelectItem(0);
			else
				MyMenu.SelectItem(1);
        }

        protected virtual void CreateMenu()
        {
            MenuItem item;

			// Continue
			int level = PlayerManager.MinPlayerTotalCampaignIndex();
			item = new CampaignChapterItem(new EzText("xxx", ItemFont), -1);
			item.Name = "Continue";
			item.Go = Go;
			AddItem(item);

            // Chapters
			for (int i = 1; i <= 7; i++)
			{
				item = new CampaignChapterItem(new EzText(CampaignSequence.ChapterName[i - 1], ItemFont), i);
				item.Name = "Chapter" + i.ToString();
				item.Go = Go;
				AddItem(item);
			}

			//MyMenu.SelectItem(0);


            // Level
            var TextBack = new QuadClass("Arcade_BoxLeft", 100, true);
            TextBack.Alpha = 1f;
            TextBack.Degrees = 90;
            MyPile.Add(TextBack, "BoxLeft");

            var LevelText = new EzText(Localization.Words.Level, Resources.Font_Grobold42);
            LevelText.Scale *= .72f;
            StartMenu.SetText_Green(LevelText, true);
            MyPile.Add(LevelText, "Level");
            LevelText.Show = false;

            var LevelNum = new EzText("Garbage", Resources.Font_Grobold42);
            LevelNum.Scale *= 1.1f;
            StartMenu.SetText_Green(LevelNum, true);
            MyPile.Add(LevelNum, "LevelNum");
            LevelNum.Show = false;


            

			//// Reset
			//item = new MenuItem(new EzText(Localization.Words.Reset, ItemFont));
			//item.Name = "Reset";
			//item.Go = Reset;
			//AddItem(item);

			SetPos_WithCinematic();

#if PC_VERSION
			Back = new ClickableBack(MyPile, false, true);

			QuadClass _q;
			_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1511.109f, -817.7772f); _q.Size = new Vector2(56.24945f, 56.24945f); }
			_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }
#endif
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

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
			}
		}

		void Reset(MenuItem item)
		{
			Call(new VerifyStoryReset(Control), 0);

			if (UseBounce)
			{
				Hid = true;
				RegularSlideOut(PresetPos.Right, 0);
			}
			else
			{
				Hide(PresetPos.Left);
			}
		}

        void Go(MenuItem item)
        {
            if (CloudberryKingdomGame.LockCampaign) return;

			// Upsell
			if (CloudberryKingdomGame.IsDemo)
			{
				Call(new UpSellMenu(Localization.Words.UpSell_Campaign, MenuItem.ActivatingPlayer));

				return;
			}

            CampaignChapterItem c_item = item as CampaignChapterItem;
            if (null == c_item) return;

            if (c_item.Locked) return;

			//Call(new StartMenu_MW_Cinematics(Title));

            Go(c_item.Chapter);

			ButtonCheck.PreLogIn = false;
        }

        int _StartLevel;
        void Go(int StartLevel)
        {
			MyGame.KillToDo("StartMusic");
            Tools.SongWad.FadeOut();
			Tools.SongWad.DisplayingInfo = false;
			
            MyGame.FadeToBlack(.0225f, 20);
            Active = false;

            _StartLevel = StartLevel;
            MyGame.WaitThenDo(75, _Go);
        }

        private void _Go()
        {
            Active = true;
            MyGame.FadeIn(.05f);
			MyGame.KillToDo("StartMusic");
            CampaignSequence.Instance.Start(_StartLevel);
            MyGame.WaitThenDo(0, OnReturnFromGame);
        }

        public void OnReturnFromGame()
        {
            Update();
			//SaveGroup.SaveAll();

            PlayerManager.UploadCampaignLevels();

			Tools.PlayHappyMusic(MyGame);
        }

        void SetPos_NoCinematic()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(686.4453f, 191.6667f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, 186.1112f)); }
            _item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(708.665f, -36.44455f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(711.4443f, -239.5557f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(714.2227f, -437.111f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }
            _item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(730.8906f, -656.889f); _item.MyText.Scale = 0.8f; _item.MySelectedText.Scale = 0.8f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(622.5566f, -1f)); }

            MyMenu.Pos = new Vector2(-783.3339f, 227.7778f);
        }

        void SetPos_WithCinematic()
        {
if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, 260.0001f); _item.MyText.Scale = 0.678417f; _item.MySelectedText.Scale = 0.678417f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, 97.22227f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -65.55551f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -228.3333f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -391.1111f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -553.889f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -716.6667f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(759.4447f, -879.4445f); _item.MyText.Scale = 0.7379997f; _item.MySelectedText.Scale = 0.7379997f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(-675.0004f, 213.889f);

	EzText _t;
	_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1241.667f, -577.7778f); _t.Scale = 0.7490832f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-798.8153f, -529.7655f); _t.Scale = 0.9310196f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-755.5557f, -702.7777f); _q.Size = new Vector2(172.6158f, 503.8864f); }

	MyPile.Pos = new Vector2(0f, 0f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1241.667f, -577.7778f); _t.Scale = 0.7490832f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-798.8156f, -521.8272f); _t.Scale = 0.9588835f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-755.5557f, -702.7777f); _q.Size = new Vector2(172.6158f, 503.8864f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1289.297f, -577.7778f); _t.Scale = 0.7490832f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-743.2467f, -521.8271f); _t.Scale = 0.9347109f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-759.525f, -694.8394f); _q.Size = new Vector2(176.9026f, 548.8981f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1293.266f, -577.7778f); _t.Scale = 0.7490832f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-739.2772f, -529.7656f); _t.Scale = 0.9334014f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-755.5557f, -702.7777f); _q.Size = new Vector2(171.5441f, 555.8043f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1360.743f, -565.8702f); _t.Scale = 0.7490832f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-747.2157f, -525.7964f); _t.Scale = 0.9230416f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-795.2476f, -698.8085f); _q.Size = new Vector2(174.1638f, 583.3118f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1261.111f, -594.4445f); _t.Scale = 0.6298336f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-704.3712f, -551.9877f); _t.Scale = 0.7738534f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-763.8892f, -702.7777f); _q.Size = new Vector2(165.1158f, 494.5531f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
else
{
			MenuItem _item;
			_item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(726.1112f, 262.7778f); _item.MyText.Scale = 0.6477503f; _item.MySelectedText.Scale = 0.6477503f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(740f, 58.33334f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(740f, -92.00005f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(742.7776f, -247.8891f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(745.5554f, -400.9998f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(740f, -548.5557f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -700.3333f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }
			_item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(750.3334f, -850.6672f); _item.MyText.Scale = 0.5883336f; _item.MySelectedText.Scale = 0.5883336f; _item.SelectIconOffset = new Vector2(0f, 0f); }

			MyMenu.Pos = new Vector2(-708.3339f, 216.6667f);

			EzText _t;
			_t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-800.0029f, 863.8889f); _t.Scale = 1.3f; }
			_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1241.667f, -577.7778f); _t.Scale = 0.7490832f; }
			_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-798.8153f, -529.7655f); _t.Scale = 0.9310196f; }

			QuadClass _q;
			_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-755.5557f, -702.7777f); _q.Size = new Vector2(172.6158f, 503.8864f); }

			MyPile.Pos = new Vector2(0f, 0f);
}
		}
    }
}