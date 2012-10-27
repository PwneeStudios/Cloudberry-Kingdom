using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class HeroSelectOptions : ArcadeBaseMenu
    {
        StartMenu_MW_HeroSelect HeroSelect;

        public HeroSelectOptions(StartMenu_MW_HeroSelect HeroSelect)
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

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            if (ButtonCheck.State(ControllerButtons.X, Control).Pressed)
                BringLeaderboard();
        }

        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            CallDelay = ReturnToCallerDelay = 0;

            // Menu
            MyMenu = new Menu();
            MyMenu.OnB = null;

            EnsureFancy();

            // Options. Menu for PC, graphics only for consoles.
#if PC_VERSION
            MenuItem item;

            string Space = "{s34,0}";
            item = new MenuItem(new EzText(ButtonString.X(80) + Space + "{c150,189,244,255} Leaderboard", ItemFont, false, true));
            item.Name = "Leaderboard";
            item.Go = Cast.ToItem(BringLeaderboard);
            AddItem(item);
            StartMenu_MW_HeroSelect.SetItemProperties_FadedOnUnselect(item);

            MyMenu.MouseOnly = true;
            MyMenu.NoneSelected = true;
#else
            string Space = "{s34,0}";
            EzText StartText = new EzText(ButtonString.Go(80) + Space + "{c122,209,39,255} Start", ItemFont, true, true);
            MyPile.Add(StartText, "Go");

            EzText LeaderText = new EzText(ButtonString.X(80) + Space + "{c150,189,244,255} Leaderboard", ItemFont, true, true);
            MyPile.Add(LeaderText, "Leaderboard");
#endif

#if PC_VERSION
            SetPos_PC();
#else
            SetPos_Console();
#endif
        }

        EzText Score, Level;

        void SetPos_Console()
        {
            EzText _t;
            _t = MyPile.FindEzText("Go"); if (_t != null) { _t.Pos = new Vector2(513.8887f, -472.2224f); _t.Scale = 0.7423338f; }
            _t = MyPile.FindEzText("Leaderboard"); if (_t != null) { _t.Pos = new Vector2(825f, -655.5554f); _t.Scale = 0.7660002f; }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        void SetPos_PC()
        {
            MenuItem _item;
            _item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-808f, 110f); _item.MyText.Scale = 0.75f; _item.MySelectedText.Scale = 0.75f; _item.SelectIconOffset = new Vector2(0f, 0f); }

            MyMenu.Pos = new Vector2(947.2223f, -608.3333f);

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        protected virtual void Go(MenuItem item)
        {
            StartLevelMenu levelmenu = new StartLevelMenu(MyArcadeItem.MyChallenge.TopPlayerLevel());

            levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
            levelmenu.StartFunc = StartFunc;
            levelmenu.ReturnFunc = null;

            Call(levelmenu);
            HeroSelect.Hide();
        }
    }
}