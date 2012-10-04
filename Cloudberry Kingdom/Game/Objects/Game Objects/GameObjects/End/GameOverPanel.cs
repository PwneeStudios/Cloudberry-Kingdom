using System;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class GameOverPanel : CkBaseMenu
    {
        public int DelayPhsx = 42;

        public override void Init()
        {
            base.Init();

            Core.DrawLayer = Level.AfterPostDrawLayer;

            Fast();
        }

        int Score = 0;
        public override void OnAdd()
        {
            base.OnAdd();

            // Set the aftermath data to note the failure
            Tools.CurrentAftermath = new AftermathData();
            Tools.CurrentAftermath.Success = false;

            // Absorb stats into the game's total stats
            PlayerManager.AbsorbTempStats();
            PlayerManager.AbsorbLevelStats();            

            // Update HighScore list
            Score = PlayerManager.GetGameScore();
            if (OnScoreTabulate != null)
                OnScoreTabulate(Score);
            if (OnLevelTabulate != null)
                OnLevelTabulate(Levels);
            if (OnTabulate != null)
                OnTabulate(Score, Levels);

#if NOT_PC
            AddScore();
#endif
            Create();

            // Absorb game stats into life time stats
            PlayerManager.AbsorbGameStats();

            // Initially hide the score screen
            this.SlideOut(PresetPos.Top, 0);

            MyGame.WaitThenDo(Awardments.AwardDelay(), () =>
                {
                    // Slow Rise
                    SlideInFrom = PresetPos.Bottom;
                    SlideOut(PresetPos.Bottom, 0);
                    SlideIn(70);

                    // Prevent menu interactions for a second
                    MyMenu.Active = false;
                    MyGame.WaitThenDo(DelayPhsx, () => MyMenu.Active = true);

#if PC_VERSION
                    if (MyTextBox != null)
                        MyGame.WaitThenDo(DelayPhsx, () => MyTextBox.Active = true);
                        //MyMenu.Show = MyMenu.Active = false;
#endif
                });
        }
        
        void Create()
        {
            MyPile = new DrawPile();

            // Make the backdrop
            QuadClass backdrop = new QuadClass("Score\\Score Screen");
            backdrop.Quad.SetColor(new Color(220, 220, 220));
            MyPile.Add(backdrop);
            backdrop.Pos = new Vector2(22.2233f, 10.55567f);

            // 'Game Over' text
            EzText Text = new EzText("Game Over", Tools.Font_Grobold42_2, 1450, false, true, .6f);
            Text.Scale = 1f;
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();
            Text.Pos = new Vector2(-675.6388f, 575.4443f);
            MyPile.Add(Text, "Header");
            //Text.Shadow = true;
            Text.ShadowColor = new Color(.36f, .36f, .36f, .86f);
            Text.ShadowOffset = new Vector2(-24, -20);


            // 'Levels' text
            Text = new EzText(string.Format("Level {0}", Levels), ItemFont);
            SetHeaderProperties(Text);
            Text.Pos = new Vector2(-893.4177f, 378.9999f);
            MyPile.Add(Text, "Level");

            // 'Score' text
            Text = new EzText(string.Format("Score {0}", Score), ItemFont);
            SetHeaderProperties(Text);
            Text.Pos = new Vector2(-873.9723f, 147.8889f);
            MyPile.Add(Text, "Score");

            // 'Distance' text
            //Text = new EzText("Distance: " + Distance.ToString() + " feet", ItemFont);
            //SetHeaderProperties(Text);
            //Text.Pos = new Vector2(-940.6393f, 145.6666f);
            //MyPile.Add(Text);

            MakeMenu();

            EnsureFancy();
            MyMenu.Pos = new Vector2(400, -240);

#if PC_VERSION
            MakeTextBox();
#endif

            SetPos();
        }

        void SetPos()
        {
        }

        void AddScore() { AddScore(null); }
        void AddScore(string name)
        {
            MyHighScoreList.Add(new ScoreEntry(Score, name));
            if (MyHighScoreList2 != null)
                MyHighScoreList2.Add(new ScoreEntry(Levels, name));
        }

#if PC_VERSION
        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            OnScoreTabulate = null;
            if (MyTextBox != null)
                MyTextBox.Release();
        }

        GUI_TextBox MyTextBox;
        Vector2 MenuPos;
        void MakeTextBox()
        {
            // Do nothing if the score doesn't qualify for the high score list
            if (!MyHighScoreList.Qualifies(Score) &&
                (MyHighScoreList2 == null || !MyHighScoreList2.Qualifies(Levels)))
                return;

            // Make the text box to allow the player to enter their name
            MyTextBox = new GUI_EnterName();
            MyTextBox.Active = false; // Keep inactive until parent GUI_Panel says it's OK to take input.
            MyTextBox.AutoDraw = false;
            MyGame.AddGameObject(MyTextBox);

            MyTextBox.Pos.SetCenter(Pos);
            MyTextBox.Pos.RelVal =
                //new Vector2(95.23779f, -556.3492f);
                new Vector2(95.23779f, -506);
            MyTextBox.Pos.code = 23;
            
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
#else
        protected override void ReleaseBody()
        {
            base.ReleaseBody();
            OnScoreTabulate = null;
        }
#endif

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.MyFloatColor = new Color(255, 254, 252).ToVector4();
            //text.MyFloatColor = new Color(255, 232, 77).ToVector4();
            text.Scale *= 1.48f;

            text.Shadow = false;
        }

        void MakeMenu()
        {
            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = null;


            MenuItem item;
            FontScale *= .89f * 1.16f;

            item = new MenuItem(new EzText("Play again", ItemFont));
            item.Go = Cast.ToItem(Action_PlayAgain);
            AddItem(item);

            item = new MenuItem(new EzText("High Scores", ItemFont));
            item.Go = Cast.ToItem(Action_ShowHighScores);
            AddItem(item);

            item = new MenuItem(new EzText("Done", ItemFont));
            item.Go = Cast.ToItem(Action_Done);
            AddItem(item);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MyText.MyFloatColor = new Color(255, 255, 255).ToVector4();
            item.MySelectedText.MyFloatColor = new Color(50, 220, 50).ToVector4();
        }

        void Action_Done()
        {
            SlideOut(PresetPos.Top, 13);
            Active = false;

            Tools.SongWad.FadeOut();
            MyGame.WaitThenDo(36, () => MyGame.EndGame(false));

            return;
        }

        void Action_PlayAgain()
        {
            SlideOut(PresetPos.Top, 13);
            Active = false;

            Tools.SongWad.FadeOut();

            MyGame.WaitThenDo(36, () => MyGame.EndGame(true));
            return;
        }

        void Action_ShowHighScores()
        {
            Hide(PresetPos.Bottom);
            Call(new HighScorePanel(MyHighScoreList, MyHighScoreList2));
        }

        int Levels = 0; // How many levels the player finished

        int Step = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
        }

        /// <summary>
        /// A High Scores list for the game just played.
        /// </summary>
        ScoreList MyHighScoreList, MyHighScoreList2;

        /// <summary>
        /// Called when the score is tabulated.
        /// </summary>
        Action<int> OnScoreTabulate;

        /// <summary>
        /// Called when the number of levels is tabulated.
        /// </summary>
        Action<int> OnLevelTabulate;

        Action<int, int> OnTabulate;

        public GameOverPanel() { }
        public GameOverPanel(ScoreList HighScores, ScoreList HighLevels, StringWorldGameData StringWorld,
                             Action<int> OnScoreTabulate, Action<int> OnLevelTabulate)
        {
            this.MyHighScoreList = HighScores;
            this.MyHighScoreList2 = HighLevels;
            this.OnScoreTabulate = OnScoreTabulate;
            this.OnLevelTabulate = OnLevelTabulate;

            if (StringWorld != null)
                this.Levels = StringWorld.CurLevelIndex + 1;
        }

        public GameOverPanel(ScoreList HighScores, ScoreList HighLevels, StringWorldGameData StringWorld,
                             Action<int, int> OnTabulate)
        {
            this.MyHighScoreList = HighScores;
            this.MyHighScoreList2 = HighLevels;
            this.OnTabulate = OnTabulate;

            if (StringWorld != null)
                this.Levels = StringWorld.CurLevelIndex + 1;
        }

        protected override void MyDraw()
        {
 	         base.MyDraw();

#if PC_VERSION
            if (MyTextBox != null)
                MyTextBox.ManualDraw();
#endif
        }
    }
}