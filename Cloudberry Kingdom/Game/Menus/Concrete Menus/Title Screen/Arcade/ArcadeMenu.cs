using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;


namespace CloudberryKingdom
{
    public class ArcadeItem : MenuItem
    {
        public Challenge MyChallenge;
        public Awardment MyPrereq;
        public bool IsLocked()
        {
            return MyPrereq != null && !PlayerManager.Awarded(MyPrereq) && !CloudberryKingdomGame.Unlock_Levels;
        }

        public ArcadeItem(EzText Text, Challenge MyChallenge, Awardment MyPrereq) : base(Text)
        {
            this.MyChallenge = MyChallenge;
            this.MyPrereq = MyPrereq;
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

        protected virtual void StartFunc(LevelItem item)
        {
            SelectedItem = item;

            // Save the menu item index
            StartLevelMenu.PreviousMenuIndex = item.MenuIndex;

            // Start the game
            MyGame.PlayGame(PlayGame);
        }

        protected virtual void PlayGame()
        {
            // Show title again if we're selecting from the menu
            if (!MyGame.ExecutingPreviousLoadFunction)
                //Escalation_Tutorial.ShowTitle = true;
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

        public static List<Tuple<BobPhsx, Tuple<BobPhsx, int>>> HeroArcadeList;
        public static List<Tuple<Challenge, BobPhsx>> LeaderboardList;

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

            HeroArcadeList = new List<Tuple<BobPhsx, Tuple<BobPhsx, int>>>()
            {
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxNormal.Instance,    new Tuple<BobPhsx, int>(null, 0) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxBig.Instance,       new Tuple<BobPhsx, int>(BobPhsxNormal.Instance, 30) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxRocketbox.Instance, new Tuple<BobPhsx, int>(BobPhsxBig.Instance, 30) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxInvert.Instance,    new Tuple<BobPhsx, int>(BobPhsxRocketbox.Instance, 40) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxJetman.Instance,    new Tuple<BobPhsx, int>(BobPhsxInvert.Instance, 40) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxBouncy.Instance,    new Tuple<BobPhsx, int>(BobPhsxJetman.Instance, 50) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxSpaceship.Instance, new Tuple<BobPhsx, int>(BobPhsxBouncy.Instance, 60) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxDouble.Instance,    new Tuple<BobPhsx, int>(BobPhsxSpaceship.Instance, 70) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxWheel.Instance,     new Tuple<BobPhsx, int>(BobPhsxDouble.Instance, 70) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BobPhsxSmall.Instance,     new Tuple<BobPhsx, int>(BobPhsxWheel.Instance, 80) ),
                         
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( JetpackWheelie,            new Tuple<BobPhsx, int>(BobPhsxSmall.Instance, 100) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( BigBouncy,                 new Tuple<BobPhsx, int>(JetpackWheelie, 100) ),
                new Tuple<BobPhsx, Tuple<BobPhsx, int>>( Ultimate,                  new Tuple<BobPhsx, int>(BigBouncy, 100) ),
            };

            // Compile a list of all leaderboards
            LeaderboardList = new List<Tuple<Challenge, BobPhsx>>();
            foreach (Tuple<BobPhsx, Tuple<BobPhsx, int>> hero in HeroArcadeList)
                LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_Escalation.Instance, hero.Item1));
            foreach (Tuple<BobPhsx, Tuple<BobPhsx, int>> hero in HeroArcadeList)
                LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_TimeCrisis.Instance, hero.Item1));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_HeroRush.Instance, null));
            LeaderboardList.Add(new Tuple<Challenge, BobPhsx>(Challenge_HeroRush2.Instance, null));
        }

        public static void CheckForArcadeUnlocks(ScoreEntry score)
        {
            List<PlayerData> CopyOfExistingPlayers = new List<PlayerData>(PlayerManager.ExistingPlayers);
            foreach (PlayerData player in CopyOfExistingPlayers)
            {
                int TotalArcadeLevel = player.GetTotalArcadeLevel();
                Awardments.CheckForAward_TimeCrisisUnlock(TotalArcadeLevel, player);
                Awardments.CheckForAward_HeroRushUnlock(TotalArcadeLevel, player);
                Awardments.CheckForAward_HeroRush2Unlock(TotalArcadeLevel, player);
            }

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
                if (null != award && !PlayerManager.Awarded(award) && !CloudberryKingdomGame.Unlock_Levels)
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
            item = AddChallenge(Challenge_Escalation.Instance, null, "Escalation");

            // Time Crisis
            item = AddChallenge(Challenge_TimeCrisis.Instance, Awardments.UnlockTimeCrisis, "Time Crisis");

            // Hero Rush
            item = AddChallenge(Challenge_HeroRush.Instance, Awardments.UnlockHeroRush, "Hero Rush");

            // Hero Rush 2
            item = AddChallenge(Challenge_HeroRush2.Instance, Awardments.UnlockHeroRush2, "Hero Rush 2");

            // Bungee Co-op
            //item = AddChallenge(Challenge_HeroRush2.Instance, Awardments.UnlockHeroRush2, null, "Bungee");

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

        private MenuItem AddChallenge(Challenge challenge, Awardment prereq, string itemname)
        {
            ArcadeItem item;
            Localization.Words word = challenge.MenuName;
            
            item = new ArcadeItem(new EzText(word, ItemFont), challenge, prereq);

            item.AdditionalOnSelect += OnSelect;

            item.Name = itemname;
            AddItem(item);

            item.Go = Go;

            return item;
        }

        void UpdateAfterPlaying()
        {
            int Level = PlayerManager.MaxPlayerTotalArcadeLevel();
            bool ShowLevel = Level > 0;

            if (ShowLevel)
            {
                MyPile.FindEzText("Level").Show = true;
                
                EzText _t = MyPile.FindEzText("LevelNum");
                _t.Show = true;
                _t.SubstituteText(Level.ToString());
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

        bool Lock = false;
        void OnSelect()
        {
            var item = MyMenu.CurItem as ArcadeItem;
            if (null == item) return;

            Lock = item.IsLocked();

            if (Lock)
            {
                EzText _t;
                _t = MyPile.FindEzText("Requirement2");
                //_t.Show = true;
                //_t.SubstituteText(Localization.WordString(Localization.Words.Required) + " " +
                //                  Localization.WordString(Localization.Words.Level) + " " + item.MyPrereq.MyInt.ToString());
                _t.SubstituteText(Localization.WordString(Localization.Words.Level) + " " + item.MyPrereq.MyInt.ToString());
            }
            else
            {
                //MyPile.FindEzText("Requirement").Show = false;
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