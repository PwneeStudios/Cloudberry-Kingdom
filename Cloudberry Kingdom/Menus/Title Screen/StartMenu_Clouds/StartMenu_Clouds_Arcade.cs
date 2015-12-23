using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_Arcade : ArcadeMenu
    {
        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_Arcade(TitleGameData_Clouds Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            PlayerManager.UploadPlayerLevels();

            Title.BackPanel.SetState(TitleBackgroundState.Scene_Arcade);
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

            StartMenu.SetItemProperties_ArcadeText(item);

            item.MyText.Scale = item.MySelectedText.Scale = 1;
        }

        protected override void Go(MenuItem item)
        {
            MyArcadeItem = item as ArcadeItem;
            if (MyArcadeItem.IsLocked()) return;

            if (MyArcadeItem.MyChallenge == Challenge_Freeplay.Instance)
            {
                SkipCallSound = true;
                Call(new StartMenu_Clouds_CustomLevel(Title));
            }
            else
            {
                if (MyArcadeItem.MyChallenge == Challenge_Escalation.Instance ||
                    MyArcadeItem.MyChallenge == Challenge_TimeCrisis.Instance)
                {
                    Call(new StartMenu_Clouds_HeroSelect(Title, this, MyArcadeItem));
                }
                else
                {
                    Challenge.ChosenHero = null;

                    int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(null);

                    StartLevelMenu_Clouds levelmenu = new StartLevelMenu_Clouds(TopLevelForHero);

                    levelmenu.MyMenu.SelectItem(ArcadeBaseMenu.PreviousMenuIndex);
                    levelmenu.StartFunc = StartFunc;
                    levelmenu.ReturnFunc = null;

                    Call(levelmenu);
                }
            }

            Hide();
        }

        public override void OnAdd()
        {
            CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Arcade);

            base.OnAdd();
        }

		ClickableBack Back;

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

#if PC
			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
				return;
			}
#endif
		}

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            SetPos();

            MyMenu.SelectItem(MyMenu.Items.Count - 2);

#if PC
			// Back button
			Back = new ClickableBack(MyPile, true, true);
            Back.SetPos_BR(MyPile);
#endif
        }

        protected override void SetPos()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2046.109f, 1035.238f); _item.MyText.Scale = 0.5098332f; _item.MySelectedText.Scale = 0.3598332f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, 746.5308f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Bungee"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, 507.0887f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, 267.6466f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, 28.2045f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, -211.2376f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Freeplay"); if (_item != null) { _item.SetPos = new Vector2(-1708.966f, -450.6797f); _item.MyText.Scale = 0.8514172f; _item.MySelectedText.Scale = 0.8514172f; _item.SelectIconOffset = new Vector2(0f, 0f); }
            _item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-75.63417f, -849.5727f); _item.MyText.Scale = 0.52175f; _item.MySelectedText.Scale = 0.52175f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(332f, -40f);

            EzText _t;
            _t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(243.3826f, -786.1113f); _t.Scale = 0.4618335f; }
            _t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(843.3829f, -758.3335f); _t.Scale = 0.5695835f; }
            _t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(111.1109f, 144.4438f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(-247.2222f, -163.8891f); _t.Scale = 1f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(300.0001f, -1577.779f); _q.Size = new Vector2(100f, 163.6364f); }
            _q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(1047.222f, -1016.666f); _q.Size = new Vector2(67.46449f, 63.41661f); }
            _q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-3379.364f, -234.1268f); _q.Size = new Vector2(1392.58f, 2060.664f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

			base.SetPos();
            
            if (Back != null) Back.SetPos_BR(MyPile);
            MyMenu.SortByHeight();
		}
    }
}