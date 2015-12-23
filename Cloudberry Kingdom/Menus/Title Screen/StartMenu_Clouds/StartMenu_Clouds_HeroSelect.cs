
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_HeroSelect : ArcadeBaseMenu
    {
        public TitleGameData_Clouds Title;
        public ArcadeMenu Arcade;

        HeroSelectOptions_Clouds Options;

        public StartMenu_Clouds_HeroSelect(TitleGameData_Clouds Title, ArcadeMenu Arcade, ArcadeItem MyArcadeItem)
            : base()
        {
            this.Title = Title;
            this.Arcade = Arcade;
            this.MyArcadeItem = MyArcadeItem;

            NumSelectableItems = 0;

            MyMenu.SelectItem(MyMenu.Items.Count - 1);
        }

        public override void Release()
        {
            base.Release();

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
            return level < item.RequiredHeroLevel && !CloudberryKingdomGame.Unlock_Levels && !CloudberryKingdomGame.UnlockHeroesAndGames;
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

                Text _t;
                MyPile.FindText("LockedHeader").Show = true;
                _t = MyPile.FindText("RequiredHero"); _t.Show = true; _t.SubstituteText(name);
                _t = MyPile.FindText("RequiredLevel"); _t.Show = true; _t.SubstituteText(m);
                

                MyPile.FindText("ScoreHeader").Show = false;
                MyPile.FindText("Score").Show = false;
                MyPile.FindText("LevelHeader").Show = false;
                MyPile.FindText("Level").Show = false;
            }
            else
            {
                MyPile.FindText("LockedHeader").Show = false;
                MyPile.FindText("RequiredHero").Show = false;
                MyPile.FindText("RequiredLevel").Show = false;

                MyPile.FindText("ScoreHeader").Show = true;
                MyPile.FindText("Score").Show = true;
                MyPile.FindText("LevelHeader").Show = true;
                MyPile.FindText("Level").Show = true;
            }

            Challenge.ChosenHero = item.Hero;
            if (ArcadeMenu.SelectedChallenge != null) ArcadeMenu.SelectedChallenge.SetGameId();
            Challenge.LeaderboardIndex = ArcadeMenu.LeaderboardIndex(ArcadeMenu.SelectedChallenge, Challenge.ChosenHero);
            MyHeroDoll.MakeHeroDoll(item.Hero);

            UpdateScore();
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(TitleBackgroundState.Scene_Arcade_Blur);
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
            Options = new HeroSelectOptions_Clouds(this);
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

            Score = new Text("0", Resources.Font_Grobold42_2);
            Level = new Text("0", Resources.Font_Grobold42_2);
          
#if PS3
            float Brightness = .945f;
            Score.MyFloatColor = ColorHelper.Gray(Brightness);
            Level.MyFloatColor = ColorHelper.Gray(Brightness);
#endif

            // Menu
            MyMenu = new Menu();

            MyMenu.OnSelect = UpdateScore;

            FontScale *= .75f;
            Vector2 item_pos = new Vector2(-808f, 100f);
            Vector2 item_add = new Vector2(0, 105);
            foreach (var phsx in ArcadeMenu.HeroArcadeList)
            {
                var item = new HeroItem(phsx);
                item.AdditionalOnSelect = OnSelect;
                AddItem(item);
                item.Go = Go;

                item.SetPos = item_pos; item.MyText.Scale = item.MySelectedText.Scale = 0.47f;
                item_pos += item_add;
            }

            MyMenu.SortByHeight();

            MyMenu.OnB = MenuReturnToCaller;
            EnsureFancy();

            /// <summary>
            /// Left Side
            /// </summary>
            #region
            // Black box, left side
            var BackBoxLeft = new QuadClass("MediumBox");
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
            var ScoreHeader = new Text(Localization.Words.HighScore, Resources.Font_Grobold42_2);
            StartMenu.SetText_Green(ScoreHeader, true);
            MyPile.Add(ScoreHeader, "ScoreHeader");

            MyPile.Add(Score, "Score");

            var LevelHeader = new Text(Localization.Words.BestLevel, Resources.Font_Grobold42_2);
            StartMenu.SetText_Green(LevelHeader, true);
            MyPile.Add(LevelHeader, "LevelHeader");
            
            MyPile.Add(Level, "Level");

            // Locked
            var LockedHeader = new Text(Localization.Words.Required, Resources.Font_Grobold42);
            LockedHeader.Scale *= .9f;
            StartMenu.SetText_Green(LockedHeader, true);
            MyPile.Add(LockedHeader, "LockedHeader");
            LockedHeader.Show = false;

            var RequiredHero = new Text("Garbage", Resources.Font_Grobold42);
            RequiredHero.Scale *= .72f;
            StartMenu.SetText_Green(RequiredHero, true);
            MyPile.Add(RequiredHero, "RequiredHero");
            RequiredHero.Show = false;

            var RequiredLevel = new Text("Garbage", Resources.Font_Grobold42);
            RequiredLevel.Scale *= .72f;
            StartMenu.SetText_Green(RequiredLevel, true);
            MyPile.Add(RequiredLevel, "RequiredLevel");
            RequiredLevel.Show = false;

            #endregion

            // Fade in
            MyPile.FadeIn(.33f);

            SetPos();

            // Back button
            Back = new ClickableBack(MyPile, true, true);
            Back.SetPos_BR(MyPile);
        }

        Text Score, Level;

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

#if PC
            if (!Active) return;

            if (Back.UpdateBack(MyCameraZoom))
            {
                MenuReturnToCaller(MyMenu);
                return;
            }			
#endif
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

            var TopScore = PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Score(item.Hero));
            var TopLevel = PlayerManager.MaxPlayerHighScore(MyArcadeItem.MyChallenge.CalcGameId_Level(item.Hero));

            Score.SubstituteText(TopScore.ToString());
            Level.SubstituteText(TopLevel.ToString());
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
            MyMenu.Pos = new Vector2(932.0002f, -789.9999f);

            Text _t;
            _t = MyPile.FindText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-1650f, -252.7783f); _t.Scale = 0.5989171f; }
            _t = MyPile.FindText("Score"); if (_t != null) { _t.Pos = new Vector2(-727.7772f, -256.3338f); _t.Scale = 0.6110842f; }
            _t = MyPile.FindText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-1650.002f, -88.88913f); _t.Scale = 0.6094177f; }
            _t = MyPile.FindText("Level"); if (_t != null) { _t.Pos = new Vector2(-730.7778f, -88.88908f); _t.Scale = 0.6110842f; }
            _t = MyPile.FindText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(-1494.445f, 52.77753f); _t.Scale = 0.9f; }
            _t = MyPile.FindText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(-1352.778f, -241.6666f); _t.Scale = 0.72f; }
            _t = MyPile.FindText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(-1352.778f, -438.8889f); _t.Scale = 0.72f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-555.5569f, -77.77783f); _q.Size = new Vector2(2236.158f, 1222.99f); }
            _q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(2713.886f, -94.44436f); _q.Size = new Vector2(776.5515f, 846.666f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
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

            StartLevelMenu_Clouds levelmenu = new StartLevelMenu_Clouds(TopLevelForHero);

            levelmenu.MyMenu.SelectItem(ArcadeBaseMenu.PreviousMenuIndex);
            levelmenu.StartFunc = StartFunc;
            levelmenu.ReturnFunc = null;

            Call(levelmenu);
            Hide();
        }
    }
}