using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Campaign : StartMenu
    {
        public override void OnReturnTo()
        {
            base.OnReturnTo();

            Update();
        }

#if PC
		ClickableBack Back;
#endif

        void Update()
        {
			EzText _t;

            // Update level text
            int Level = PlayerManager.MinPlayerTotalCampaignLevel() + 1;
			bool ShowLevel = Level > 1;
            bool ShowContinue = Level > 1 && Level < 321;

			string template_level = string.Format("{0}, {1} {{0}}", Localization.WordString(Localization.Words.Continue), Localization.WordString(Localization.Words.Level));

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

        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_Campaign(TitleGameData_Clouds Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(TitleBackgroundState.Scene_StoryMode);

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

            StartMenu.SetItemProperties_ArcadeText(item);
            item.MySelectedText.Shadow = item.MyText.Shadow = false;
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
				MyMenu.SelectItem(MyMenu.Items.Count - 1);
			else
                MyMenu.SelectItem(MyMenu.Items.Count - 2);
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

			SetPos();

            MyMenu.SortByHeight();

#if PC
			Back = new ClickableBack(MyPile, true, true);
            Back.SetPos_BR(MyPile);
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

			//Call(new StartMenu_Clouds_Cinematics(Title));

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

            PlayerManager.UploadCampaignLevels();

			Tools.PlayHappyMusic(MyGame);
        }

        void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Chapter7"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 1401.111f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter6"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 1238.889f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter5"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 1076.667f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter4"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 914.4443f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter3"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 752.2222f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter2"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 590f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Chapter1"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 427.7778f); _item.MyText.Scale = 0.6760836f; _item.MySelectedText.Scale = 0.6760836f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Continue"); if (_item != null) { _item.SetPos = new Vector2(720.5555f, 265.5556f); _item.MyText.Scale = 0.7355003f; _item.MySelectedText.Scale = 0.7355003f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(-1361.111f, -861.111f);

            EzText _t;
            _t = MyPile.FindEzText("Header"); if (_t != null) { _t.Pos = new Vector2(-672.2248f, 936.111f); _t.Scale = 0.9941665f; }
            _t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-1333.333f, -1047.222f); _t.Scale = 0.7490832f; }
            _t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(-798.8153f, -1007.543f); _t.Scale = 0.9310196f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-744.4449f, -1369.444f); _q.Size = new Vector2(172.6158f, 503.8864f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(1661.112f, -889.9996f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}