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
            EzText text = new EzText("Campaign cleared!", Tools.Font_Dylan60);
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

    public class ScoreScreen : StartMenuBase
    {
        protected virtual void MakeMenu()
        {
            if (AsMenu)
            {
                MyMenu = new Menu(false);

                MyMenu.Control = -1;

                MyMenu.OnB = null;

                MenuItem item;

                string BringNextText =
                        //"New Level";
                        //"One More";
                        "Keep settings";
                item = new MenuItem(new EzText(BringNextText, ItemFont));
                item.Go = MenuGo_NewLevel;
                AddItem(item);
                item.MyText.Scale =
                item.MySelectedText.Scale *= 1.3f;
                item.Shift(new Vector2(-86f, 65));
                item.SelectedPos.X += 6;

                string BackText =
                        //"Back";
                        "Edit Settings";
                item = new MenuItem(new EzText(BackText, ItemFont));
                item.Go = MenuGo_Continue;
                AddItem(item);

                if (MyGame.MyLevel.ReplayAvailable)
                {
                    item = new MenuItem(new EzText("Watch Replay", ItemFont));
                    item.Go = MenuGo_WatchReplay;
                    AddItem(item);
                }

                EnsureFancy();
                MyMenu.FancyPos.RelVal = new Vector2(869.0476f, -241.6667f);
            }
            else
            {
                EzText ContinueText = new EzText(ButtonString.Go(90) + " Continue", ItemFont);
                SetHeaderProperties(ContinueText);
                ContinueText.MyFloatColor = Menu.DefaultMenuInfo.SelectedNextColor;
                MyPile.Add(ContinueText);
                ContinueText.Pos = new Vector2(180f, -477.7778f) + ShiftAll;

                if (MyGame.MyLevel.ReplayAvailable)
                {
                    EzText ReplayText = new EzText(ButtonString.X(90) + " Watch Replay", ItemFont);
                    SetHeaderProperties(ReplayText);
                    ReplayText.MyFloatColor = Menu.DefaultMenuInfo.SelectedBackColor;
                    ReplayText.MyFloatColor = new Color(184, 231, 231).ToVector4();
                    MyPile.Add(ReplayText);
                    ReplayText.Pos = new Vector2(180f, -325.3333f) + ShiftAll;
                }
            }
        }

        EzSound ScoreSound, BonusSound;

        ScoreText BlobScoreText, CoinScoreText, AttemptScoreText;

        public int DelayPhsx = 5;//14;



        public ScoreScreen(bool CallBaseConstructor) : base(CallBaseConstructor) { }

        public ScoreScreen(StatGroup group, GameData game) : base(false)
        {
            MyGame = game;
            MyStatGroup = group;
            FontScale = .6f;

            Constructor();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            GreenItem(item);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;
        }

// Whether to make a menu, or a static text with key bindings
#if PC_VERSION
        static bool AsMenu = true;
#else
        static bool AsMenu = true;
#endif

        protected QuadClass LevelCleared;
        Vector2 ShiftAll = new Vector2(-110f, -20f);
        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();

            QuadClass Backdrop = new QuadClass("Score\\Score Screen");
            MyPile.Add(Backdrop);
            Backdrop.Pos = new Vector2(120, 50) + ShiftAll;

            LevelCleared = new QuadClass("Score\\LevelCleared");
            LevelCleared.Scale(.9f);
            MyPile.Add(LevelCleared);
            LevelCleared.Pos = new Vector2(10, 655) + ShiftAll;

            CoinScoreText = new ScoreText();
            CoinScoreText.Init(ScoreText.Units.Coins, Pos);
            CoinScoreText.Pos = new Vector2(-659, 322) + ShiftAll;
            CoinScoreText.SetVal(13, 22);

            BlobScoreText = new ScoreText();
            BlobScoreText.Init(ScoreText.Units.Blobs, Pos);
            BlobScoreText.Pos = new Vector2(305, 122) + ShiftAll;
            BlobScoreText.SetVal(18, 20);

            AttemptScoreText = new ScoreText();
            AttemptScoreText.Init(ScoreText.Units.Attempts, Pos);
            AttemptScoreText.Pos = new Vector2(-350, -100) + ShiftAll;
            AttemptScoreText.SetVal(68);

            MakeMenu();

            ScoreSound = Tools.SoundWad.FindByName("Coin");
            BonusSound = Tools.SoundWad.FindByName("Coin");
            ScoreSound.MaxInstances = 2;
        }

        FancyVector2 zoom = new FancyVector2();
        public static bool UseZoomIn = true;

        protected StatGroup MyStatGroup = StatGroup.Level;
        public override void OnAdd()
        {
 	        base.OnAdd();

            if (UseZoomIn)
            {
                SlideIn(0);
                //MyPile.BubbleUp(true, 16, .75f);
                //zoom.MultiLerp(7, DrawPile.BubbleScale.Map(v => (v - Vector2.One) * .5f + Vector2.One));
                zoom.MultiLerp(6, DrawPile.BubbleScale.Map(v => (v - Vector2.One) * .3f + Vector2.One));
            }

            // Calculate scores
            PlayerManager.CalcScore(MyStatGroup);

            int Coins = PlayerManager.PlayerSum(p => p.GetStats(MyStatGroup).Coins);
            int CoinTotal = PlayerManager.PlayerMax(p => p.GetStats(MyStatGroup).TotalCoins);
            int Blobs = PlayerManager.PlayerSum(p => p.GetStats(MyStatGroup).Blobs);
            int BlobTotal = PlayerManager.PlayerMax(p => p.GetStats(MyStatGroup).TotalBlobs);

            //CoinScoreText.SetVal(PlayerManager.Score_Coins, Core.MyLevel.NumCoins);
            //BlobScoreText.SetVal(PlayerManager.Score_Blobs, Core.MyLevel.NumBlobs);
            CoinScoreText.SetVal(Coins, CoinTotal);
            BlobScoreText.SetVal(Blobs, BlobTotal);

            AttemptScoreText.SetVal(PlayerManager.Score_Attempts);

            // Door data, track coins
            if (Campaign.CurData != null)
            {
                Campaign.CurData.Coins = Coins;
                Campaign.CurData.TotalCoins = CoinTotal;
            }

            // Awardments
            Awardments.CheckForAward_HoldForward();
            Awardments.CheckForAward_NoCoins();

            // Prevent menu interactions for a second
            MyMenu.Active = false;

            MyGame.WaitThenDo(DelayPhsx, () => MyMenu.Active = true);
        }

        protected override void MyDraw()
        {
            if (Core.MyLevel.Replay || Core.MyLevel.Watching) return;

            Vector2 SaveZoom = MyGame.Cam.Zoom;
            Vector2 SaveHoldZoom = MyGame.Cam.HoldZoom;
            Tools.QDrawer.Flush();

            if (zoom != null)
            {
                MyGame.Cam.Zoom = .001f * zoom.Update();
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = true;
                //MyGame.Cam.SetVertexZoom(.5f);
            }

            Pos.SetCenter(Core.MyLevel.MainCamera, true);
            Pos.Update();

            base.MyDraw();

            CoinScoreText.DrawImage();
            BlobScoreText.DrawImage();
            AttemptScoreText.DrawImage();

            CoinScoreText.DrawText();
            BlobScoreText.DrawText();
            AttemptScoreText.DrawText();

            Tools.EndSpriteBatch();

            if (zoom != null)
            {
                //MyGame.Cam.SetVertexZoom(2f);
                MyGame.Cam.Zoom = SaveZoom;
                MyGame.Cam.HoldZoom = SaveHoldZoom;
                MyGame.Cam.SetVertexCamera();
                EzText.ZoomWithCamera_Override = false;
                Tools.QDrawer.Flush();
            }
        }

        protected override void MyPhsxStep()
        {
            Level level = Core.MyLevel;

            if (level != null)
                level.PreventReset = true;

            if (level.Replay || level.Watching)
                return;

            if (Active)
            {
                if (!ShouldSkip())
                {
                    if (AsMenu)
                        base.MyPhsxStep();
                    else
                        GUI_Phsx();
                }
            }
        }

        /// <summary>
        /// Play another level with the same seed
        /// </summary>
        protected void MenuGo_NewLevel(MenuItem item)
        {
            SlideOut(PresetPos.Left);

            Active = false;

            Tools.SongWad.FadeOut();

            MyGame.WaitThenDo(36, () => MyGame.EndGame(true));
            return;
        }

        /// <summary>
        /// Called when 'Continue' is selected from the menu.
        /// The Score Screen slides out and the current game's EndGame function is called.
        /// </summary>
        protected virtual void MenuGo_Continue(MenuItem item)
        {
            SlideOut(PresetPos.Left);

            MyGame.WaitThenDo(SlideOutLength + 2, () => MyGame.EndGame(false));
        }

        protected void MenuGo_Stats(MenuItem item)
        {
            Call(new StatsMenu(MyStatGroup), 19);
        }

        /// <summary>
        /// Called when 'Watch Replay' is selected from the menu.
        /// The level's replay is loaded, with the level's current information saved.
        /// </summary>
        protected void MenuGo_WatchReplay(MenuItem item)
        {
            if (AsMenu)
            {
                Active = false;

                MyGame.WaitThenDo(35, () =>
                    {
                        OnReturnTo(); // Re-activate the Score Screen object
                        Core.MyLevel.WatchReplay(true); // Start the replay
                    });
            }
            else
            {
                Core.MyLevel.WatchReplay(true);
            }
        }

        int LastActive;
        bool ShouldSkip()
        {
            if (LastActive + 5 < Tools.TheGame.PhsxCount)
            {
                LastActive = Tools.TheGame.PhsxCount;
                return true;
            }
            else
            {
                LastActive = Tools.TheGame.PhsxCount;
                return false;
            }
        }

        public void GUI_Phsx()
        {
            Level level = Core.MyLevel;

            if (MyGame.MyLevel.ReplayAvailable)
            {
                bool WatchReplay = false;
                if (level.CanWatchReplay && ButtonCheck.State(ControllerButtons.X, -1).Pressed)
                    WatchReplay = true;
#if PC_VERSION
            if (Tools.keybState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape) ||
                Tools.PrevKeyboardState.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
                WatchReplay = false;
#endif

                if (WatchReplay)
                    MenuGo_WatchReplay(null);
            }

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
                MenuGo_Continue(null);
        }
    }
}