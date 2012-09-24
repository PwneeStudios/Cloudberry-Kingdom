using Microsoft.Xna.Framework;

using Drawing;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class ScoreScreen_CampaignEnd : ScoreScreen
    {
        CampaignList scores;
        public ScoreScreen_CampaignEnd(StatGroup group, GameData game, CampaignList scores)
            : base(false)
        {
            this.scores = scores;
            MyGame = game;
            MyStatGroup = group;
            FontScale = .6f;

            Constructor();
        }

        protected void MenuGo_HighScores(MenuItem item)
        {
            SlideInFrom = PresetPos.Bottom;
            Hide(PresetPos.Bottom);
            Call(new HighScorePanel(scores));
        }

#if PC_VERSION
        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            if (MyTextBox != null)
                MyTextBox.Release();
        }

        GUI_TextBox MyTextBox;
        Vector2 MenuPos;
        void MakeTextBox()
        {
            // Do nothing if the score doesn't qualify for the high score list
            if (!scores.Score.Qualifies(Campaign.Score) &&
                !scores.Attempts.Qualifies(Campaign.Attempts) &&
                !scores.Time.Qualifies(Campaign.Time))
                return;

            // Make the text box to allow the player to enter their name
            MyTextBox = new GUI_EnterName();
            MyTextBox.AutoDraw = false;
            MyGame.AddGameObject(MyTextBox);

            MyTextBox.Pos.SetCenter(Pos);
            MyTextBox.Pos.RelVal =
                new Vector2(95.23779f, -580.3492f);

            // Hide the menu
            MenuPos = MyMenu.Pos;
            MyMenu.Show = MyMenu.Active = false;

            // Show the menu when the user is done entering their name
            MyTextBox.OnEnter += () =>
            {
                // Add the high score
                AddScore(MyTextBox.Text);

                MyGame.WaitThenDo(35, () =>
                {
                    float width = MyGame.Cam.GetWidth();

                    MyMenu.Show = MyMenu.Active = true;
                    MyMenu.FancyPos.LerpTo(MenuPos + new Vector2(width, 0), MenuPos, 20);
                    MyTextBox.Pos.LerpTo(new Vector2(-width, 0), 20);
                });
            };
        }

        protected override void MyDraw()
        {
            base.MyDraw();

            if (MyTextBox != null)
                MyTextBox.ManualDraw();
        }
#endif

        void AddScore() { AddScore(null); }
        void AddScore(string name)
        {
            scores.Score.Add(new ScoreEntry(Campaign.Score, name));
            scores.Attempts.Add(new ScoreEntry(Campaign.Attempts, name));
            scores.Time.Add(new ScoreEntry(Campaign.Time, name));
        }

        protected override void MakeMenu()
        {
            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = null;

            MenuItem item;

            ItemPos.Y -= 100;
            PosAdd.Y *= .75f;

            item = new MenuItem(new EzText("Continue", ItemFont));
            item.Go = MenuGo_Continue;
            AddItem(item);
            item.MyText.Scale =
            item.MySelectedText.Scale *= 1.3f;
            item.Shift(new Vector2(-86f, 65));
            item.SelectedPos.X += 6;

            item = new MenuItem(new EzText("Statistics!", ItemFont));
            item.Go = MenuGo_Stats;
            AddItem(item);

            item = new MenuItem(new EzText("High Scores", ItemFont));
            item.Go = MenuGo_HighScores;
            AddItem(item);

            EnsureFancy();
            MyMenu.FancyPos.RelVal = new Vector2(869.0476f, -241.6667f);

            LevelCleared.Show = false;
            EzText text = new EzText("Campaign cleared!", Tools.Font_Grobold42_2);
            text.Scale *= .88f;
            MyPile.Add(text);
            text.Pos = new Vector2(-1311.728f, 868.0558f);

#if PC_VERSION
            MakeTextBox();
            MyMenu.Show = MyMenu.Active = false;
#else
            AddScore();
#endif
        }
    }

    public class ScoreScreen_Campaign : ScoreScreen
    {
        public ScoreScreen_Campaign(StatGroup group, GameData game) : base(group, game) { }

        protected override void MenuGo_Continue(MenuItem item)
        {
            base.MenuGo_Continue(item);
            Tools.SongWad.FadeOut();
        }

        protected override void MakeMenu()
        {
            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = null;

            MenuItem item;

            ItemPos.Y -= 100;

            item = new MenuItem(new EzText("Continue", ItemFont));
            item.Go = MenuGo_Continue;
            AddItem(item);
            item.MyText.Scale =
            item.MySelectedText.Scale *= 1.3f;
            item.Shift(new Vector2(-86f, 65));
            item.SelectedPos.X += 6;

            if (MyGame.MyLevel.ReplayAvailable)
            {
                item = new MenuItem(new EzText("Watch Replay", ItemFont));
                item.Go = MenuGo_WatchReplay;
                AddItem(item);
            }

            EnsureFancy();
            MyMenu.FancyPos.RelVal = new Vector2(869.0476f, -241.6667f);
        }
    }
}