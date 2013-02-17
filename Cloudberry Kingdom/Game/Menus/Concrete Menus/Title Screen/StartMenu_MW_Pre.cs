using Microsoft.Xna.Framework;

#if XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Pre : StartMenu
    {
        bool GameIsDemo;

        bool CallingOptionsMenu;
        protected override void MenuGo_Options(MenuItem item)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            Call(new StartMenu_MW_Options(Control, true), 0);
            CallingOptionsMenu = true;
        }

        protected override void Exit()
        {
            SelectSound = null;
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Blur_Dark);
            Call(new StartMenu_MW_Exit(Control), 0);
        }

        protected override void BringNextMenu()
        {
            base.BringNextMenu();

            Hide();
        }

        public TitleGameData_MW Title;
        public StartMenu_MW_Pre(TitleGameData_MW Title) : base()
        {
            this.Title = Title;
            CallingOptionsMenu = false;
        }

        protected override void MyPhsxStep()
        {
            if (GameIsDemo && !CloudberryKingdomGame.IsDemo)
            {
                GameIsDemo = false;

                // Hide 'Buy' option
                var _item = MyMenu.FindItemByName("Buy");
                if (_item != null)
                {
                    _item.Show = false;
                    _item.Selectable = false;
                    _item.SetPos = new Vector2(200000f, -200000);
                }

				MyMenu.SelectItem(0);

                // Reset position of items
                SetPos();
            }

            base.MyPhsxStep();
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Title);
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
        }

        public override void OnAdd()
        {
			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.TitleScreen);

            base.OnAdd();
        }

        public override void OnReturnTo()
        {
			CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.TitleScreen);

            if (CallingOptionsMenu)
            {
                MyMenu.SelectItem(4);
                CallingOptionsMenu = false;
            }

            base.OnReturnTo();
        }

        public override bool MenuReturnToCaller(Menu menu)
        {
            if (NoBack) return false;

            return base.MenuReturnToCaller(menu);
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            BackBox = new QuadClass("Title_Strip");
            BackBox.Alpha = .9f;
            MyPile.Add(BackBox, "Back");

            MyPile.FadeIn(.33f);

            SetPos();
        }

        void MenuGo_Play(MenuItem item)
        {
#if PC_VERSION || WINDOWS
			Call(new StartMenu_MW(Title), 0);
#else
			if (CloudberryKingdomGame.ProfilesAvailable())
			{
				Call(new StartMenu_MW(Title), 0);
			}
			else
			{
				CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Err_MustBeSignedInToPlay);
			}

			//if (CloudberryKingdomGame.OnlineFunctionalityAvailable())
			//{
			//    Call(new StartMenu_MW(Title), 0);
			//}
			//else
			//{
			//    CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Err_MustBeSignedInToPlay);
			//}
#endif
        }

        void MenuGo_Leaderboards(MenuItem item)
        {
            if (CloudberryKingdomGame.OnlineFunctionalityAvailable(MenuItem.ActivatingPlayerIndex()))
            {
                var gamer = CloudberryKingdomGame.IndexToSignedInGamer(MenuItem.ActivatingPlayerIndex());
                if (gamer != null)
                {
                    Call(new LeaderboardGUI(Title, gamer), 0);
                }
            }
            else
            {
                CloudberryKingdomGame.ShowError_MustBeSignedInToLive(Localization.Words.Err_MustBeSignedInToLive);
            }
        }

        void MenuGo_Achievements(MenuItem item)
        {
            if (MenuItem.ActivatingPlayer < 0 || MenuItem.ActivatingPlayer > 3) return;

            if (PlayerManager.Get(MenuItem.ActivatingPlayer).MyGamer != null)
			{
				CloudberryKingdomGame.ShowAchievements = true;
				CloudberryKingdomGame.ShowFor = (PlayerIndex)MenuItem.ActivatingPlayer;
			}
			else
			{
				CloudberryKingdomGame.ShowError_MustBeSignedIn(Localization.Words.Err_MustBeSignedIn);
			}
        }

        void MenuGo_BuyGame(MenuItem item)
        {
            CloudberryKingdomGame.ShowMarketplace = true;
            CloudberryKingdomGame.ShowFor = (PlayerIndex)MenuItem.ActivatingPlayer;
        }

        protected override void MakeMenu()
        {
			GameIsDemo = CloudberryKingdomGame.IsDemo;

            MenuItem item;

            // Play
            item = new MenuItem(new EzText(Localization.Words.PlayGame, ItemFont, true));
            item.Name = "Play";
            item.Go = MenuGo_Play;
            AddItem(item);

            // Leaderboard
            item = new MenuItem(new EzText(Localization.Words.Leaderboards, ItemFont, true));
            item.Name = "Leaderboard";
            item.Go = MenuGo_Leaderboards;
            AddItem(item);

            // Achievements
            item = new MenuItem(new EzText(Localization.Words.Achievements, ItemFont, true));
            item.Name = "Achievements";
            item.Go = MenuGo_Achievements;
            AddItem(item);

            //// Options
            //item = new MenuItem(new EzText(Localization.Words.Options, ItemFont, true));
            //item.Name = "Options";
            //item.Go = MenuGo_Options;
            //AddItem(item);

			if (GameIsDemo)
			{
				// Buy Game
				item = new MenuItem(new EzText(Localization.Words.UnlockFullGame, ItemFont, true));
				item.Name = "Buy";
				item.Go = MenuGo_BuyGame;
				AddItem(item);
			}

			// Credits
			//item = new MenuItem(new EzText(Localization.Words.Credits, ItemFont, true));
			//item.Name = "Buy";
			//item.Go = MenuGo_Credits;
			//AddItem(item);

            // Exit
            item = new MenuItem(new EzText(Localization.Words.Exit, ItemFont, true));
            item.Name = "Exit";
            item.Go = MenuGo_Exit;
            AddItem(item);

            EnsureFancy();

            this.CallToLeft = true;
        }

        protected QuadClass BackBox;

        void SetPos()
        {
            BackBox.TextureName = "White";
            BackBox.Quad.SetColor(ColorHelper.Gray(.1f));
            BackBox.Alpha = .73f;

			if (GameIsDemo)
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Play"); if (_item != null) { _item.SetPos = new Vector2(0f, 168.3334f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(0f, -29.22222f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Achievements"); if (_item != null) { _item.SetPos = new Vector2(0f, -229.5555f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Buy"); if (_item != null) { _item.SetPos = new Vector2(0f, -435.7777f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(0f, -624.9999f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-91.66698f, -19.4445f);

				QuadClass _q;
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-61.11133f, -336.1111f); _q.Size = new Vector2(608.4988f, 520.8323f); }

				MyPile.Pos = new Vector2(-27.77734f, -33.33337f);
			}
			else
			{
				MenuItem _item;
				_item = MyMenu.FindItemByName("Play"); if (_item != null) { _item.SetPos = new Vector2(0f, 168.3334f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(0f, -29.22231f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Achievements"); if (_item != null) { _item.SetPos = new Vector2(-11.11157f, -226.7778f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }
				_item = MyMenu.FindItemByName("Exit"); if (_item != null) { _item.SetPos = new Vector2(5.55542f, -438.8887f); _item.MyText.Scale = 0.605f; _item.MySelectedText.Scale = 0.605f; _item.SelectIconOffset = new Vector2(0f, 0f); }

				MyMenu.Pos = new Vector2(-88.88914f, -116.6667f);

				QuadClass _q;
				_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-61.11133f, -336.1111f); _q.Size = new Vector2(541.4985f, 466.9143f); }

				MyPile.Pos = new Vector2(-27.77734f, -33.33337f);
			}
        }
    }
}