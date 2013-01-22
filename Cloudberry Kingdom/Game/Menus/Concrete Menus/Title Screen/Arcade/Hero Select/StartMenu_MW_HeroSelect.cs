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

        public HeroDoll MyHeroDoll;
        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            CallDelay = ReturnToCallerDelay = 0;

            Score = new EzText("0", Resources.Font_Grobold42_2);
            Level = new EzText("0", Resources.Font_Grobold42_2);
                                        
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

            SetPos();
        }

        EzText Score, Level;

        public override void OnReturnTo()
        {
            base.OnReturnTo();

            UpdateScore();
            Update();
        }

        void UpdateScore()
        {
            var item = MyMenu.CurItem as HeroItem;
            if (null == item) return;

            var TopScore = MyArcadeItem.MyChallenge.TopScore();
            var TopLevel = MyArcadeItem.MyChallenge.TopLevel();

            Score.RightJustify = Level.RightJustify = true;
            Score.SubstituteText(TopScore.ToString());
            Level.SubstituteText(TopLevel.ToString());
        }

        void Update()
        {
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

                    }
                    else
                    {
                        item.MyText.Alpha = 1f;
                        item.MySelectedText.Alpha = 1f;
                    }
                }
            }
        }

        void SetPos()
        {
            MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

            EzText _t;
            _t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-22.22266f, 636.1111f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1161.11f, 366.6667f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-2.779297f, 105.5556f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1163.887f, -155.5555f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("LockedHeader"); if (_t != null) { _t.Pos = new Vector2(33.33325f, 441.6666f); _t.Scale = 0.9f; }
            _t = MyPile.FindEzText("RequiredHero"); if (_t != null) { _t.Pos = new Vector2(280.5552f, 163.8889f); _t.Scale = 0.72f; }
            _t = MyPile.FindEzText("RequiredLevel"); if (_t != null) { _t.Pos = new Vector2(277.7778f, -44.44443f); _t.Scale = 0.72f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5466f, 1004.329f); }
            _q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }

        protected virtual void Go(MenuItem item)
        {
            if (Lock) return;

            var _item = MyMenu.CurItem as HeroItem;
            if (null == _item) return;
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