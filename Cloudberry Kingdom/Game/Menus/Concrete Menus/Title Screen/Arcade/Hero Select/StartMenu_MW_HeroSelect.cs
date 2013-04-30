using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class HeroItem : MenuItem
    {
        public BobPhsx Hero;
        public BobPhsx RequiredHero;
        public int RequiredHeroLevel;

        public HeroItem(Tuple<BobPhsx, Tuple<BobPhsx, int>> pair)
            : base(new EzText(pair.Item1.Name, Resources.Font_Grobold42_2))
        {
            this.Hero = pair.Item1;
            this.RequiredHero = pair.Item2.Item1;
            this.RequiredHeroLevel = pair.Item2.Item2;
        }
    }

    public class StartMenu_MW_HeroSelect : ArcadeBaseMenu
    {
        public TitleGameData_MW Title;
        public ArcadeMenu Arcade;

        HeroSelectOptions Options;

        public StartMenu_MW_HeroSelect(TitleGameData_MW Title, ArcadeMenu Arcade, ArcadeItem MyArcadeItem)
            : base()
        {
            this.Title = Title;
            this.Arcade = Arcade;
            this.MyArcadeItem = MyArcadeItem;

            NumSelectableItems = 0;
        }

        public override void Release()
        {
            base.Release();

            ScrollQuad = null;

            if (MyHeroDoll != null) MyHeroDoll.Release();
            if (Options != null) Options.Release();

            Title = null;
            Arcade = null;
        }

        bool Lock = false;
        public bool Locked()
        {
            var item = MyMenu.CurItem as HeroItem;
            if (null == item) return false;

            return Locked(item);
        }

        public bool Locked(HeroItem item)
        {
            if (item.RequiredHero == null) return false;

            int level = MyArcadeItem.MyChallenge.CalcTopGameLevel(item.RequiredHero);
            return level < item.RequiredHeroLevel && !CloudberryKingdomGame.Unlock_Levels;
        }

        public bool Invisible(HeroItem item)
        {
            if (item.RequiredHero == null) return false;

            int level = MyArcadeItem.MyChallenge.CalcTopGameLevel(item.RequiredHero);
            return level < item.RequiredHeroLevel && !CloudberryKingdomGame.Unlock_Levels && item.RequiredHeroLevel >= 100;
        }

        void OnSelect()
        {
            var item = MyMenu.CurItem as HeroItem;
            if (null == item) return;

            Lock = Locked();

            if (Lock)
            {
                int level = item.RequiredHeroLevel;
                string name = Localization.WordString(item.RequiredHero.Name);
                string m = Localization.WordString(Localization.Words.Level) + " " + level.ToString();

                EzText _t;
                MyPile.FindEzText("LockedHeader").Show = true;
                _t = MyPile.FindEzText("RequiredHero"); _t.Show = true; _t.SubstituteText(name);
                _t = MyPile.FindEzText("RequiredLevel"); _t.Show = true; _t.SubstituteText(m);
                

                MyPile.FindEzText("ScoreHeader").Show = false;
                MyPile.FindEzText("Score").Show = false;
                MyPile.FindEzText("LevelHeader").Show = false;
                MyPile.FindEzText("Level").Show = false;
            }
            else
            {
                MyPile.FindEzText("LockedHeader").Show = false;
                MyPile.FindEzText("RequiredHero").Show = false;
                MyPile.FindEzText("RequiredLevel").Show = false;

                MyPile.FindEzText("ScoreHeader").Show = true;
                MyPile.FindEzText("Score").Show = true;
                MyPile.FindEzText("LevelHeader").Show = true;
                MyPile.FindEzText("Level").Show = true;
            }

            Challenge.ChosenHero = item.Hero;
			if (ArcadeMenu.SelectedChallenge != null) ArcadeMenu.SelectedChallenge.SetGameId();
			Challenge.LeaderboardIndex = ArcadeMenu.LeaderboardIndex(ArcadeMenu.SelectedChallenge, Challenge.ChosenHero);
            MyHeroDoll.MakeHeroDoll(item.Hero);

            UpdateScore();
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Kobbler_Blur);
            base.SlideIn(0);

            if (MyHeroDoll != null) { MyHeroDoll.SlideIn(0); MyHeroDoll.Hid = false; }
            if (Options != null) Options.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
            
            if (MyHeroDoll != null) MyHeroDoll.SlideOut(Preset, 0);
            if (Options != null) Options.SlideOut(Preset, 0);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            SetItemProperties_FadedOnUnselect(item);
        }

        public static void SetItemProperties_FadedOnUnselect(MenuItem item)
        {
            item.MySelectedText.Shadow = item.MyText.Shadow = false;

            StartMenu.SetItemProperties_Green(item, true);

            item.MyText.OutlineColor.W *= .4f;
            item.MySelectedText.OutlineColor.W *= .7f;
        }

        public override void OnAdd()
        {
            base.OnAdd();

            // Hero Doll
            MyHeroDoll = new HeroDoll(Control);
            MyGame.AddGameObject(MyHeroDoll);

            // Options. Menu for PC, graphics only for consoles.
            Options = new HeroSelectOptions(this);
            MyGame.AddGameObject(Options);

            Update();
        }

		SimpleScroll Scroll;
		ClickableBack Back;

        QuadClass ScrollQuad, ScrollTop, ScrollBottom;

        public HeroDoll MyHeroDoll;

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            CallDelay = ReturnToCallerDelay = 0;

			Score = new EzText("0", Resources.Font_Grobold42_2);
			Level = new EzText("0", Resources.Font_Grobold42_2);
          
#if PS3
			float Brightness = .945f;
			Score.MyFloatColor = ColorHelper.Gray(Brightness);
			Level.MyFloatColor = ColorHelper.Gray(Brightness);
#endif

			// Menu
            MiniMenu mini = new MiniMenu();
            MyMenu = mini;

            MyMenu.OnSelect = UpdateScore;

            mini.WrapSelect = false;
            mini.Shift = new Vector2(0, -135);
            mini.ItemsToShow = 6;
            FontScale *= .75f;
            foreach (var phsx in ArcadeMenu.HeroArcadeList)
            {
                var item = new HeroItem(phsx);
                item.AdditionalOnSelect = OnSelect;
                AddItem(item);
                item.Go = Go;
            }
            
            MyMenu.OnB = MenuReturnToCaller;
            EnsureFancy();

            /// <summary>
            /// Left Side
            /// </summary>
            #region
            // Black box, left side
            var BackBoxLeft = new QuadClass("Arcade_BoxLeft");
            BackBoxLeft.Alpha = 1f;
            MyPile.Add(BackBoxLeft, "BoxLeft");
            #endregion

            /// <summary>
            /// Right Side
            /// </summary>
            #region
            // Black box, right side
            var BackBox = new QuadClass("Arcade_Box");
            BackBox.Alpha = 1f;
            MyPile.Add(BackBox, "BoxRight");

            // Score, level
            var ScoreHeader = new EzText(Localization.Words.HighScore, Resources.Font_Grobold42_2);
            StartMenu.SetText_Green(ScoreHeader, true);
            MyPile.Add(ScoreHeader, "ScoreHeader");

            MyPile.Add(Score, "Score");

            var LevelHeader = new EzText(Localization.Words.BestLevel, Resources.Font_Grobold42_2);
            StartMenu.SetText_Green(LevelHeader, true);
            MyPile.Add(LevelHeader, "LevelHeader");
            
            MyPile.Add(Level, "Level");

            // Locked
            var LockedHeader = new EzText(Localization.Words.Required, Resources.Font_Grobold42);
            LockedHeader.Scale *= .9f;
            StartMenu.SetText_Green(LockedHeader, true);
            MyPile.Add(LockedHeader, "LockedHeader");
            LockedHeader.Show = false;

            var RequiredHero = new EzText("Garbage", Resources.Font_Grobold42);
            RequiredHero.Scale *= .72f;
            StartMenu.SetText_Green(RequiredHero, true);
            MyPile.Add(RequiredHero, "RequiredHero");
            RequiredHero.Show = false;

            var RequiredLevel = new EzText("Garbage", Resources.Font_Grobold42);
            RequiredLevel.Scale *= .72f;
            StartMenu.SetText_Green(RequiredLevel, true);
            MyPile.Add(RequiredLevel, "RequiredLevel");
            RequiredLevel.Show = false;

            #endregion

            /// <summary>
            /// Back
            /// </summary>
            MyPile.Add(new QuadClass(ButtonTexture.Back), "Back");
            MyPile.Add(new QuadClass("BackArrow2", "BackArrow"));

            MyPile.FadeIn(.33f);

            // Scroll bar
            ScrollQuad = new QuadClass("Arcade_BoxLeft", 100);
            MyPile.Add(ScrollQuad, "Scroll");

            ScrollTop = new QuadClass("Arcade_BoxLeft", 100);
            MyPile.Add(ScrollTop, "ScrollTop");
            ScrollTop.Show = false;

            ScrollBottom = new QuadClass("Arcade_BoxLeft", 100);
            MyPile.Add(ScrollBottom, "ScrollBottom");
            ScrollBottom.Show = false;

			Scroll = new SimpleScroll(ScrollQuad, ScrollTop, ScrollBottom);
			Back = new ClickableBack(MyPile);

            SetPos();
        }

        EzText Score, Level;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

			bool UpdateScrollPosition = true;

#if PC_VERSION
			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
				return;
			}
			
			Scroll.PhsxStep(Tools.MouseGUIPos(MyCameraZoom));

			// Sync the mini menu's scroll position with the scroll bar
			MiniMenu mini = MyMenu as MiniMenu;
			mini.Active = true;
			if (ButtonCheck.MouseInUse)
			{
				if (Scroll.Holding)
				{
					UpdateScrollPosition = false;

					mini.Active = false;
					mini.TopItem = Scroll.tToIndex(NumSelectableItems - mini.ItemsToShow + 1);
				}
				else
				{
					if (Tools.DeltaScroll == 0)
						UpdateScrollPosition = false;
					else
						UpdateScrollPosition = true;
				}
			}
#endif

			if (UpdateScrollPosition)
			{
				//Scroll.UpdatePosFromIndex(Math.Max(MyMenu.CurIndex, mini.TopItem), NumSelectableItems - mini.ItemsToShow + 1);
				Scroll.UpdatePosFromIndex(mini.TopItem, NumSelectableItems - mini.ItemsToShow + 1);
			}
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();

			if (MyHeroDoll != null)
				MyHeroDoll.AutoDraw = true;

            UpdateScore();
            Update();
        }

        void UpdateScore()
        {
            var item = MyMenu.CurItem as HeroItem;
            if (null == item) return;

			Challenge.ChosenHero = item.Hero;

            //var TopScore = Math.Max(MyArcadeItem.MyChallenge.TopScore(), PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Score(item.Hero)));
            //var TopLevel = Math.Max(MyArcadeItem.MyChallenge.TopLevel(), PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Level(item.Hero)));

            var TopScore = PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Score(item.Hero));
            var TopLevel = PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Level(item.Hero));


			bool Center = false;
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				Center = true;
			}

			if (!Center)
			{
				Score.RightJustify = Level.RightJustify = true;
			}
			else
			{
				SetPos();
			}
            Score.SubstituteText(TopScore.ToString());
			if (Center) Score.Center();
            Level.SubstituteText(TopLevel.ToString());
			if (Center) Level.Center();
        }

        int NumSelectableItems;
        void Update()
        {
            NumSelectableItems = 0;
            foreach (MenuItem _item in MyMenu.Items)
            {
                HeroItem item = _item as HeroItem;
                if (null != item)
                {
                    if (Locked(item))
                    {
                        item.MyText.Alpha = .4f;
                        item.MySelectedText.Alpha = .4f;

                        if (Invisible(item))
                        {
                            item.Selectable = false;
                            item.MyText.Alpha = 0;
                            item.MySelectedText.Alpha = 0;
                        }
                        else
                            NumSelectableItems++;
                    }
                    else
                    {
						item.Selectable = true;
                        item.MyText.Alpha = 1f;
                        item.MySelectedText.Alpha = 1f;
                        NumSelectableItems++;
                    }
                }
            }
        }

        void SetPos()
		{
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-19.44507f, 647.2222f); _t.Scale = 1f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1161.11f, 402.7777f); _t.Scale = 0.9414999f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-19.4458f, 133.3334f); _t.Scale = 1f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1166.665f, -133.3333f); _t.Scale = 0.9644167f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(8.332886f, 655.5555f); _t.Scale = 0.9532502f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1249.999f, 405.5555f); _t.Scale = 0.90625f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-19.44611f, 127.7778f); _t.Scale = 0.9073337f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1249.998f, -127.7777f); _t.Scale = 0.9306669f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(249.9997f, 630.5557f); _t.Scale = 0.8089167f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1236.11f, 411.1112f); _t.Scale = 0.9160833f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-22.22363f, 136.1111f); _t.Scale = 0.7519999f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1236.109f, -61.11107f); _t.Scale = 0.9373333f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-5.555912f, 608.3333f); _t.Scale = 0.6490834f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1255.555f, 425.0001f); _t.Scale = 0.865916f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-0.001403809f, 130.5556f); _t.Scale = 0.6412507f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1274.998f, -47.22218f); _t.Scale = 0.8700836f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-33.33374f, 622.2222f); _t.Scale = 0.8630002f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1158.332f, 402.7778f); _t.Scale = 0.8790836f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-16.66821f, 108.3334f); _t.Scale = 0.8452501f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1166.665f, -105.5555f); _t.Scale = 0.8682501f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-30.55597f, 608.3334f); _t.Scale = 0.7484168f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1158.332f, 433.3333f); _t.Scale = 1f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-27.7793f, 108.3334f); _t.Scale = 0.7689999f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1166.665f, -97.22215f); _t.Scale = 1f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(27.33325f, 441.6666f); _t.Scale = 0.88f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(297.2219f, 658.3333f); _t.Scale = 0.8626668f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(680, 391.6665f); _t.Scale = 0.6997502f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-36.11264f, 80.55562f); _t.Scale = 0.8034166f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(680, -175f); _t.Scale = 0.8029999f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5464f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -434.0609f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else
			{
				MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

				EzText _t;
				_t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(74.99976f, 702.7775f); _t.Scale = 0.9339998f; }
				_t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1252.777f, 443.6666f); _t.Scale = 0.9260002f; }
				_t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(86.10962f, 175f); _t.Scale = 0.9667501f; }
				_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1252.554f, -80.55543f); _t.Scale = 0.9260002f; }
				_t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
				_t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
				_t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

				QuadClass _q;
				_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5463f, 1004.329f); }
				_q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
				_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }
				_q = MyPile.FindQuad("Scroll"); if (_q != null) { _q.Pos = new Vector2(-1450f, -206.803f); _q.Size = new Vector2(25.9999f, 106.8029f); }
				_q = MyPile.FindQuad("ScrollTop"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -100.0001f); _q.Size = new Vector2(27.57401f, 18.96959f); }
				_q = MyPile.FindQuad("ScrollBottom"); if (_q != null) { _q.Pos = new Vector2(-1444.444f, -752.2221f); _q.Size = new Vector2(28.7499f, 21.2196f); }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
		}

        protected virtual void Go(MenuItem item)
        {
            if (Lock) return;

            var _item = MyMenu.CurItem as HeroItem;
            if (null == _item) return;

			// Upsell
			if (CloudberryKingdomGame.IsDemo && _item.Hero != BobPhsxNormal.Instance)
			{
				Call(new UpSellMenu(Localization.Words.UpSell_Hero, MenuItem.ActivatingPlayer));
				Hide();
				
				if (MyHeroDoll != null)
					MyHeroDoll.AutoDraw = false;
				
				return;
			}

			int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(_item.Hero);

            //int TopLevelForHero = MyArcadeItem.MyChallenge.TopLevel();

            StartLevelMenu levelmenu = new StartLevelMenu(TopLevelForHero);

            levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
            levelmenu.StartFunc = StartFunc;
            levelmenu.ReturnFunc = null;

            Call(levelmenu);
            Hide();
        }
    }
}