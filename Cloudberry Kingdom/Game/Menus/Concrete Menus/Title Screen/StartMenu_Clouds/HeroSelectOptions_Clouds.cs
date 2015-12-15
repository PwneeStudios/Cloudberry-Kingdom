using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class HeroSelectOptions_Clouds : ArcadeBaseMenu
    {
        StartMenu_Clouds_HeroSelect HeroSelect;

        public HeroSelectOptions_Clouds(StartMenu_Clouds_HeroSelect HeroSelect)
            : base()
        {
            this.HeroSelect = HeroSelect;
        }

        public override void Release()
        {
            base.Release();

            HeroSelect = null;
        }

        public override void SlideIn(int Frames)
        {
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

            StartMenu.SetItemProperties_Green(item, true);

            item.MyText.OutlineColor.W *= .4f;
            item.MySelectedText.OutlineColor.W *= .7f;
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

		void BringLeaderboard()
		{
			BringLeaderboard(0);
		}
        void BringLeaderboard(int PressingPlayer)
        {
            if (PressingPlayer < 0 || PressingPlayer > 3) 
                return;

            PlayerIndex ActivatingPlayer = (PlayerIndex)PressingPlayer;

            if (CloudberryKingdomGame.SimpleLeaderboards)
            {
                var item = HeroSelect.MyMenu.CurItem as HeroItem;
                if (null == item) return;

                var challenge = HeroSelect.MyArcadeItem.MyChallenge;
                challenge.SetGameId();

                int GameId_Score = HeroSelect.MyArcadeItem.MyChallenge.GameId_Score;
                int GameId_Level = HeroSelect.MyArcadeItem.MyChallenge.GameId_Level;

                var MyHighScoreList = ScoreDatabase.GetList(GameId_Score);
                MyHighScoreList.MyFormat = ScoreEntry.Format.Score;

                var MyHighLevelList = ScoreDatabase.GetList(GameId_Level);
                MyHighLevelList.MyFormat = ScoreEntry.Format.Level;

                var panel = new HighScorePanel(true, MyHighScoreList, MyHighLevelList);
                panel.NoDelays();
                HeroSelect.Call(panel);
                HeroSelect.Hide();
                HeroSelect.MyHeroDoll.Hide();
            }
            else
            {
                if (CloudberryKingdomGame.OnlineFunctionalityAvailable(ActivatingPlayer))
                {
#if XBOX
                    var gamer = CloudberryKingdomGame.IndexToSignedInGamer(ActivatingPlayer);
                    if (gamer != null)
                    {
						HeroSelect.Call(new LeaderboardGUI(null, gamer, (int)ActivatingPlayer), 0);
                        HeroSelect.Hide();
                        HeroSelect.MyHeroDoll.Hide();
                    }
#else
                    HeroSelect.Call(new LeaderboardGUI(null, (int)ActivatingPlayer), 0);
                    HeroSelect.Hide();
                    HeroSelect.MyHeroDoll.Hide();
#endif
                }
                else
                {
                    CloudberryKingdomGame.ShowError_MustBeSignedInToLive(Localization.Words.Err_MustBeSignedInToLive);
                }
            }
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            var data = ButtonCheck.State(ControllerButtons.X, Control);
            if (data.Pressed)
                BringLeaderboard(data.PressingPlayer);
        }

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            CallDelay = ReturnToCallerDelay = 0;

            // Options. Menu for PC, graphics only for consoles.
#if PC
            // Menu
            MyMenu = new Menu();
            MyMenu.OnB = null;

            EnsureFancy();

            MenuItem item;

            MyPile.Add(new QuadClass(ButtonTexture.X, 80, "Button_X"));
            item = new MenuItem(new EzText(Localization.Words.Leaderboard, ItemFont, false, true));

            item.Name = "Leaderboard";
            item.Go = Cast.ToItem(BringLeaderboard);
            AddItem(item);

#if PC
			item.MyText.OutlineColor.W = .9f;
			item.MySelectedText.OutlineColor.W = 1f;
#else
            StartMenu_Clouds_HeroSelect.SetItemProperties_FadedOnUnselect(item);
#endif

            MyMenu.MouseOnly = true;
            MyMenu.NoneSelected = true;
#else
            EnsureFancy();

            string Space = "{s34,0}";
            EzText StartText = new EzText(ButtonString.Go(80) + Space + "{c122,209,39,255} " + Localization.WordString(Localization.Words.Start), ItemFont, true, true);
            MyPile.Add(StartText, "Go");

            EzText LeaderText = new EzText(ButtonString.X(80) + Space + "{c150,189,244,255} " + Localization.WordString(Localization.Words.Leaderboard), ItemFont, true, true);
            MyPile.Add(LeaderText, "Leaderboard");
#if PS3
			StartText.MyFloatColor = ColorHelper.Gray(.9f);
			LeaderText.MyFloatColor = ColorHelper.Gray(.9f);			
#endif

#endif

#if PC
            SetPos_PC();
#else
			SetPos_Console();
#endif
        }

        EzText Score, Level;

        void SetPos_Console()
		{
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(408.3333f, -516.6668f); _t.Scale = 0.7152506f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(491.667f, -702.7776f); _t.Scale = 0.7393336f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(602.7778f, -530.5557f); _t.Scale = 0.7182506f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(672.2222f, -711.111f); _t.Scale = 0.7266668f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(697.2224f, -525.0001f); _t.Scale = 0.7423338f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(683.3337f, -711.111f); _t.Scale = 0.7547504f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(474.9998f, -472.2224f); _t.Scale = 0.7164173f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(708.3333f, -652.7776f); _t.Scale = 0.743167f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(399.9998f, -522.2224f); _t.Scale = 0.6441671f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(744.4443f, -686.1109f); _t.Scale = 0.6407505f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(413.8887f, -525f); _t.Scale = 0.7423338f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(658.3333f, -708.3331f); _t.Scale = 0.7660002f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(427.7773f, -522.2224f); _t.Scale = 0.7423338f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(513.8887f, -711.111f); _t.Scale = 0.7660002f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(477.7776f, -538.889f); _t.Scale = 0.6902504f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(536.1111f, -705.5554f); _t.Scale = 0.6942502f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(395.6355f, -525.0002f); _t.Scale = 0.6706704f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(748.0187f, -687.6974f); _t.Scale = 0.6805836f; }
				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
			else
			{
				EzText _t;
				_t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(513.8887f, -472.2224f); _t.Scale = 0.7423338f; }
				_t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(825f, -655.5554f); _t.Scale = 0.7660002f; }

				MyPile.Pos = new Vector2(83.33417f, 130.9524f);
			}
		}

        void SetPos_PC()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-2521.89f, -220.5555f); _item.MyText.Scale = 0.6497502f; _item.MySelectedText.Scale = 0.6497502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(947.2223f, -608.3333f);

            QuadClass _q;
            _q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-655.5551f, -952.7773f); _q.Size = new Vector2(63.86522f, 60.03331f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        protected virtual void Go(MenuItem item)
        {
            var _item = HeroSelect.MyMenu.CurItem as HeroItem;
            if (null == _item) return;
            int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(_item.Hero);
            
            StartLevelMenu_Clouds levelmenu = new StartLevelMenu_Clouds(TopLevelForHero);

            levelmenu.MyMenu.SelectItem(ArcadeBaseMenu.PreviousMenuIndex);
            levelmenu.StartFunc = StartFunc;
            levelmenu.ReturnFunc = null;

            Call(levelmenu);
            HeroSelect.Hide();
        }
    }
}