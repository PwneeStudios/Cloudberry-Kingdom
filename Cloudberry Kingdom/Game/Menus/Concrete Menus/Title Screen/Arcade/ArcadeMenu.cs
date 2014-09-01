using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using CloudberryKingdom.Awards;

namespace CloudberryKingdom
{
    public class ArcadeItem : MenuItem
    {
        public Challenge MyChallenge;
        public Awardment MyPrereq;
        int UnlockLevel = 0;

        public bool IsLocked()
        {
            if (MyPrereq != null && CloudberryKingdomGame.IsDemo) return true;

            return MyPrereq != null && !PlayerManager.Awarded(MyPrereq) && !CloudberryKingdomGame.Unlock_Levels && !CloudberryKingdomGame.UnlockHeroesAndGames
                && (UnlockLevel != 0 && PlayerManager.MaxPlayerTotalLevel() < UnlockLevel);
        }

        public ArcadeItem(EzText Text, Challenge MyChallenge, Awardment MyPrereq, int UnlockLevel) : base(Text)
        {
            this.MyChallenge = MyChallenge;
            this.MyPrereq = MyPrereq;
            this.UnlockLevel = UnlockLevel;
        }
    }

    public class ArcadeBaseMenu : CkBaseMenu
    {
        public LevelItem SelectedItem;
        public ArcadeItem MyArcadeItem;

        public override void OnAdd()
        {
            base.OnAdd();

            MyGame.ClearPreviousLoadFunction();
        }

        /// <summary>
        /// The last difficulty selected via the difficulty select menu
        /// </summary>
        public static int PreviousMenuIndex = -1;

        protected virtual void StartFunc(LevelItem item)
        {
            SelectedItem = item;

            // Save the menu item index
            PreviousMenuIndex = item.MenuIndex;

            // Start the game
            MyGame.PlayGame(PlayGame);
        }

        protected virtual void PlayGame()
        {
            // Show title again if we're selecting from the menu
            if (!MyGame.ExecutingPreviousLoadFunction)
                HeroRush_Tutorial.ShowTitle = true;

            MyArcadeItem.MyChallenge.Start(SelectedItem.StartLevel);
        }

        public override void Release()
        {
            base.Release();

            MyArcadeItem = null;
            SelectedItem = null;
        }
    }


    public class ArcadeMenu : ArcadeBaseMenu
    {
        public static BobPhsx JetpackWheelie;
        public static BobPhsx BigBouncy;
        public static BobPhsx Ultimate;
        public static BobPhsx Transcendent;

        public static List<Tuple<BobPhsx, Tuple<BobPhsx, int>>> HeroArcadeList;
        public static List<Tuple<Challenge, BobPhsx>> LeaderboardList;
        public static List<Tuple<int, int>> ChallengeGoal;

        public const int HighestLevelNeeded = 75;
        public static BobPhsx HighestHero = BobPhsxWheel.Instance;

        public static void StaticInit()
        {
            // Heroes
            BobPhsxNormal.Instance.Id = 0;
            BobPhsxBig.Instance.Id = 1;
            BobPhsxInvert.Instance.Id = 2;
            BobPhsxDouble.Instance.Id = 3;
            BobPhsxJetman.Instance.Id = 4;
            BobPhsxBouncy.Instance.Id = 5;
            BobPhsxBox.Instance.Id = 6;
            BobPhsxScale.Instance.Id = 7;
            BobPhsxTime.Instance.Id = 8;
            BobPhsxSmall.Instance.Id = 9;
            BobPhsxSpaceship.Instance.Id = 10;
            BobPhsxWheel.Instance.Id = 11;
            BobPhsxRocketbox.Instance.Id = 15;

            ArcadeMenu.JetpackWheelie = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            ArcadeMenu.JetpackWheelie.Name = Localization.Words.JetpackWheelie;
            ArcadeMenu.JetpackWheelie.Id = 12;

            ArcadeMenu.BigBouncy = BobPhsx.MakeCustom(Hero_BaseType.Bouncy, Hero_Shape.Big, Hero_MoveMod.Jetpack);
            ArcadeMenu.BigBouncy.Name = Localization.Words.Hero;
            ArcadeMenu.BigBouncy.Id = 13;

            ArcadeMenu.Ultimate = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Classic);
            ArcadeMenu.Ultimate.Name = Localization.Words.Masochistic;
            BobPhsx.CustomPhsxData UltimatePhsx = new BobPhsx.CustomPhsxData();
            UltimatePhsx.Init();
            UltimatePhsx[BobPhsx.CustomData.accel] = 2.2f;
            UltimatePhsx[BobPhsx.CustomData.friction] = 2.2f;
            UltimatePhsx[BobPhsx.CustomData.maxspeed] = 2.35f;
            UltimatePhsx[BobPhsx.CustomData.gravity] = .575f;
            UltimatePhsx[BobPhsx.CustomData.jumplength] = 1.51f;
            UltimatePhsx[BobPhsx.CustomData.jumplength2] = 1.5212f;
            UltimatePhsx[BobPhsx.CustomData.jumpaccel] = .8f;
            UltimatePhsx[BobPhsx.CustomData.jumpaccel2] = .8f;
            UltimatePhsx[BobPhsx.CustomData.maxfall] = 2.2f;
            UltimatePhsx[BobPhsx.CustomData.numjumps] = 1;
            UltimatePhsx[BobPhsx.CustomData.size] = .2f;
            ArcadeMenu.Ultimate.SetCustomPhsx(UltimatePhsx);
            ArcadeMenu.Ultimate.Id = 14;

            // New Coder's Edition heroes
            BobPhsxMeat.Instance.Id = 16;
            BobPhsxBlobby.Instance.Id = 17;
            BobPhsxTimeship.Instance.Id = 18;
            Transcendent = BobPhsx.MakeCustom(Hero_BaseType.Classic, Hero_Shape.Classic, Hero_MoveMod.Classic, Hero_Special.Classic, true);
            Transcendent.Name = Localization.Words.Local;
            Transcendent.Id = 19;

            HeroArcadeList = new List<Tuple<BobPhsx, Tuple<BobPhsx, int>>>()
            {
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxNormal.Instance,    new Tuple<BobPhsx, int>(null, 0) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxJetman.Instance,    new Tuple<BobPhsx, int>(BobPhsxNormal.Instance, 25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxBouncy.Instance,	new Tuple<BobPhsx, int>(BobPhsxJetman.Instance, 25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxSpaceship.Instance, new Tuple<BobPhsx, int>(BobPhsxBouncy.Instance, 25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxBig.Instance,		new Tuple<BobPhsx, int>(BobPhsxNormal.Instance, 50) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxSmall.Instance,		new Tuple<BobPhsx, int>(BobPhsxBig.Instance,	50) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxInvert.Instance,	new Tuple<BobPhsx, int>(BobPhsxSmall.Instance,	50) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxDouble.Instance,    new Tuple<BobPhsx, int>(BobPhsxNormal.Instance, 75) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxWheel.Instance,     new Tuple<BobPhsx, int>(BobPhsxDouble.Instance, 75) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxRocketbox.Instance,	new Tuple<BobPhsx, int>(HighestHero, HighestLevelNeeded) ),
                         
                // New Coder's Edition heroes
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxBlobby.Instance,	new Tuple<BobPhsx, int>(BobPhsxNormal.Instance,	  25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( Transcendent,          	new Tuple<BobPhsx, int>(BobPhsxBlobby.Instance,   25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxMeat.Instance,		new Tuple<BobPhsx, int>(Transcendent,       	  25) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxTimeship.Instance,	new Tuple<BobPhsx, int>(BobPhsxMeat.Instance,     25) ),

                // Secret unlockable heroes
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( JetpackWheelie,            new Tuple<BobPhsx, int>(BobPhsxNormal.Instance, 100) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BigBouncy,                 new Tuple<BobPhsx, int>(JetpackWheelie, 100) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( Ultimate,                  new Tuple<BobPhsx, int>(BigBouncy, 100) ),
            };

            // Compile a list of all leaderboards
            LeaderboardList = new List<Tuple<Challenge, BobPhsx>>();
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(null, null));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_StoryMode.Instance, null));
            foreach (Tuple<BobPhsx, Tuple<BobPhsx, int>> hero in HeroArcadeList)
                LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_Escalation.Instance, hero.Item1));
            foreach (Tuple<BobPhsx, Tuple<BobPhsx, int>> hero in HeroArcadeList)
                LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_TimeCrisis.Instance, hero.Item1));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_HeroRush.Instance, null));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_Madness.Instance, null));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_HeroRush2.Instance, null));

            // Goals
            ChallengeGoal = new List<Tuple<int, int>>();
            foreach (Tuple<BobPhsx, Tuple<BobPhsx, int>> hero in HeroArcadeList)
            {
                if (hero.Item2.Item1 == null) continue;
                ChallengeGoal.Add(new Tuple<int, int>(Challenge_Escalation.Instance.CalcGameId_Level(hero.Item2.Item1), hero.Item2.Item2));
                ChallengeGoal.Add(new Tuple<int, int>(Challenge_TimeCrisis.Instance.CalcGameId_Level(hero.Item2.Item1), hero.Item2.Item2));
            }
        }

        public static int LeaderboardIndex(Challenge challenge, BobPhsx phsx)
        {
            int index = 0;
            foreach (Tuple<Challenge, BobPhsx> tuple in LeaderboardList)
            {
                if (tuple.Item1 == challenge && tuple.Item2 == phsx)
                {
                    return index;
                }

                index++;
            }

            return 0;
        }

        public static void CheckForArcadeUnlocks_OnSwapIn(int level)
        {
            // Always add new hero message (for testing)
            //Tools.CurGameData.AddGameObject(new HeroUnlockedMessage());

            bool DoSave = false;

            Tools.Assert(Challenge.CurrentId >= 0);

            List<PlayerData> CopyOfExistingPlayers = new List<PlayerData>(PlayerManager.ExistingPlayers);
            foreach (PlayerData player in CopyOfExistingPlayers)
            {
                // Check for goals
                //if (ChallengeGoal.ContainsKey(Challenge.CurrentId))
                {
                    int CurHighLevel = player.GetHighScore(Challenge.CurrentId);
                    //if (level + 1 >= Goal && CurHighLevel < Goal)
                    if (level >= CurHighLevel)
                    {
                        //DoSave = true;
                        player.AddHighScore(new ScoreEntry(player.GetName(), Challenge.CurrentId,                       level + 1,              Challenge.CurrentScore, level + 1, 0, 0, 0));
                        player.AddHighScore(new ScoreEntry(player.GetName(), Challenge.CurrentId - Challenge.LevelMask, Challenge.CurrentScore, Challenge.CurrentScore, level + 1, 0, 0, 0));
                    }

                    int Goal = 0;
                    for (int i = 0; i < ChallengeGoal.Count; i++)
                    {
                        if (ChallengeGoal[i].Item1 == Challenge.CurrentId)
                        {
                            Goal = ChallengeGoal[i].Item2;

                            if (level + 1 >= Goal && CurHighLevel < Goal)
                            {
                                DoSave = true;
                            }
                        }
                    }
                }

                // Check for awards
                //int TotalArcadeLevel = player.GetTotalArcadeLevel();
                int TotalArcadeLevel = player.GetTotalLevel();
                Awardments.CheckForAward_TimeCrisisUnlock(TotalArcadeLevel, player);
                Awardments.CheckForAward_HeroRushUnlock(TotalArcadeLevel, player);
                Awardments.CheckForAward_HeroRush2Unlock(TotalArcadeLevel, player);
            }

            if (DoSave)
            {
                SaveGroup.SaveAll();

                if (!CloudberryKingdomGame.UnlockHeroesAndGames)
                    Tools.CurGameData.AddGameObject(new HeroUnlockedMessage());
            }
        }

        public static void CheckForArcadeUnlocks(ScoreEntry score)
        {
            Awardments.CheckForAward_ArcadeScore(score.Value);
            Awardments.CheckForAward_ArcadeScore2(score.Value);
        }

        bool Long = false;

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.ScaleText(.945f);
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
            SetLockColors();

            UpdateAfterPlaying();
        }

        void SetLockColors()
        {
            foreach (MenuItem item in MyMenu.Items)
            {
                Awardment award = item.MyObject as Awardment;
                if (null != award && !PlayerManager.Awarded(award) && !CloudberryKingdomGame.Unlock_Levels && !CloudberryKingdomGame.UnlockHeroesAndGames)
                {
                    item.MyText.MyFloatColor = new Color(255, 100, 100).ToVector4();
                    item.MySelectedText.MyFloatColor = new Color(255, 160, 160).ToVector4();
                }
                else
                {
                    if (null != award)
                        SetItemProperties(item);
                }
            }
        }

        public ArcadeMenu()
        {
        }

#if PC_VERSION
        void BringLeaderboard()
        {
            int control = MenuItem.ActivatingPlayer;
            if (control < 0 || control > 3) control = -1;

            Call(new LeaderboardGUI(null, control), 0);
            Hide();
        }
#endif

        EzText RequiredText, RequiredText2;
        QuadClass TextBack;
        public override void  Init()
        {
            base.Init();

            SetParams();

            MyPile = new DrawPile();

            // Menu
            if (Long)
            {
                MyMenu = new LongMenu();
                MyMenu.FixedToCamera = false;
                MyMenu.WrapSelect = false;
            }
            else
                MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;

            // Level
            var LevelText = new EzText(Localization.Words.PlayerLevel, Resources.Font_Grobold42);
            LevelText.Scale *= .72f;
            StartMenu.SetText_Green(LevelText, true);
            MyPile.Add(LevelText, "Level");
            LevelText.Show = false;

            var LevelNum = new EzText("Garbage", Resources.Font_Grobold42);
            LevelNum.Scale *= 1.1f;
            StartMenu.SetText_Green(LevelNum, true);
            MyPile.Add(LevelNum, "LevelNum");
            LevelNum.Show = false;

            // Requirement
            RequiredText = new EzText(Localization.Words.Required, Resources.Font_Grobold42);
            RequiredText.Scale *= 1f;
            StartMenu.SetText_Green(RequiredText, true);
            MyPile.Add(RequiredText, "Requirement");
            RequiredText.Alpha = 0;

            RequiredText2 = new EzText("Garbage", Resources.Font_Grobold42);
            RequiredText2.Scale *= 1f;
            StartMenu.SetText_Green(RequiredText2, true);
            MyPile.Add(RequiredText2, "Requirement2");
            RequiredText2.Alpha = 0;

            TextBack = new QuadClass("Arcade_BoxLeft", 100, true);
            TextBack.Alpha = 1f;
            TextBack.Degrees = 90;
            MyPile.Add(TextBack, "BoxLeft");


            // Header
            MenuItem Header = new MenuItem(new EzText(Localization.Words.TheArcade, Resources.Font_Grobold42_2));
            Header.Name = "Header";
            MyMenu.Add(Header);
            SetItemProperties(Header);
            Header.Pos = new Vector2(-1834.998f, 999.1272f);
            Header.MyText.Scale *= 1.15f;
            Header.Selectable = false;

            MenuItem item;
            ItemPos = new Vector2(-1689.523f, 520.4127f);

            // Escalation
            item = AddChallenge(Challenge_Escalation.Instance, null, "Escalation", 0);

            // Time Crisis
            item = AddChallenge(Challenge_TimeCrisis.Instance, Awardments.UnlockTimeCrisis, "Time Crisis", Awardments.TimeCrisis_LevelUnlock);

            // Hero Rush
            item = AddChallenge(Challenge_HeroRush.Instance, Awardments.UnlockHeroRush, "Hero Rush", Awardments.HeroRush_LevelUnlock);

            // Hero Rush 2
            item = AddChallenge(Challenge_HeroRush2.Instance, Awardments.UnlockHeroRush2, "Hero Rush 2", Awardments.HeroRush2_LevelUnlock);

            // Madness
            item = AddChallenge(Challenge_Madness.Instance, Awardments.UnlockHeroRush, "Bungee", Awardments.HeroRush_LevelUnlock);

            // Freeplay
            item = AddChallenge(Challenge_Freeplay.Instance, null, "Freeplay", 0);

#if PC_VERSION
            // Leaderboards
            MyPile.Add(new QuadClass(ButtonTexture.X, 100, "Button_X"));
            item = new MenuItem(new EzText(Localization.Words.Leaderboards, ItemFont, false, true));

            item.Name = "Leaderboard";
            item.Go = Cast.ToItem(BringLeaderboard);
            MyMenu.OnX = Cast.ToMenu(BringLeaderboard);
            AddItem(item);
            item.MyOscillateParams.base_value = 1.01f;
            item.MyOscillateParams.max_addition *= .4f;
            StartMenu.SetText_Green(item.MyText, true);
            StartMenu.SetText_Green(item.MySelectedText, true);
#endif

            // Backdrop
            QuadClass backdrop;
            
            backdrop = new QuadClass("Backplate_1500x900", 1500);
            if (Long)
                backdrop.SizeY *= 1.02f;
            MyPile.Add(backdrop, "Backdrop");
            backdrop.Pos = new Vector2(9.921265f, -111.1109f) + new Vector2(-297.6191f, 15.87299f);

            // Position
            EnsureFancy();
            MyMenu.Pos = new Vector2(332, -40f);
            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

            MyMenu.SelectItem(1);

            SetLockColors();

            UpdateAfterPlaying();
        }

        private void SetParams()
        {
            CallDelay = 20;

            SlideLength = 27;

            ReturnToCallerDelay = 20;
            SlideInFrom = PresetPos.Left;
            SlideOutTo = PresetPos.Left;
        }

        Vector2 GetGoalPos()
        { 
            return new Vector2(-174.6031f, -603.1746f);
        }

        private MenuItem AddChallenge(Challenge challenge, Awardment prereq, string itemname, int UnlockLevel)
        {
            ArcadeItem item;
            Localization.Words word = challenge.MenuName;
            
            item = new ArcadeItem(new EzText(word, ItemFont), challenge, prereq, UnlockLevel);

            item.AdditionalOnSelect += OnSelect;

            item.Name = itemname;
            AddItem(item);

            item.Go = Go;

            return item;
        }

        protected virtual void SetPos()
        {
#if PC_VERSION
            EzText _t = MyPile.FindEzText("LevelNum");

            float max_width = _t.GetWorldWidth("1000");
            float width = _t.GetWorldWidth(_t.FirstString());

            float shift = max_width - width - 50;
            _t.Pos += new Vector2(shift, 0);

            _t = MyPile.FindEzText("Level");
            _t.Pos += new Vector2(shift, 0);
#endif
        }

        void UpdateAfterPlaying()
        {
            int Level = PlayerManager.MaxPlayerTotalLevel();

            bool ShowLevel = Level > 0;

            if (ShowLevel)
            {
                MyPile.FindEzText("Level").Show = true;
                
                EzText _t = MyPile.FindEzText("LevelNum");
                _t.Show = true;

#if PC_VERSION
                SetPos();
                _t.SubstituteText(Level.ToString());
#else
                _t.SubstituteText(Level.ToString());
#endif
            }
            else
            {
                MyPile.FindEzText("Level").Show = false;
                MyPile.FindEzText("LevelNum").Show = false;
            }

            foreach (MenuItem _item in MyMenu.Items)
            {
                var item = _item as ArcadeItem;
                if (null == item) continue;

                if (item.IsLocked())
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

        public static Challenge SelectedChallenge = null;

        bool Lock = false;
        void OnSelect()
        {
            var item = MyMenu.CurItem as ArcadeItem;
            if (null == item) return;

            Lock = item.IsLocked();

            // Store the selected challenge
            ArcadeMenu.SelectedChallenge = item.MyChallenge;
            Challenge.LeaderboardIndex = ArcadeMenu.LeaderboardIndex(ArcadeMenu.SelectedChallenge, null);

            if (Lock && item.MyPrereq != null)
            {
                EzText _t;
                _t = MyPile.FindEzText("Requirement2");
                _t.SubstituteText(Localization.WordString(Localization.Words.PlayerLevel) + " " + item.MyPrereq.MyInt.ToString());
            }
        }

        protected virtual void Go(MenuItem item)
        {
        }

        protected override void MyPhsxStep()
        {
            if (Lock)
            {
                RequiredText.Alpha += .2f;
                if (RequiredText.Alpha > 1) RequiredText.Alpha = 1;
            }
            else
            {
                RequiredText.Alpha -= .2f;
                if (RequiredText.Alpha < 0) RequiredText.Alpha = 0;
            }
            TextBack.Alpha = RequiredText.Alpha;
            RequiredText2.Alpha = RequiredText.Alpha;

            base.MyPhsxStep();
        }
    }
}