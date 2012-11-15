using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class HeroItem : MenuItem
    {
        public BobPhsx Hero;
        public bool Locked;

        public HeroItem(BobPhsx Hero)
            : base(new EzText(Hero.Name, Resources.Font_Grobold42_2))
        {
            this.Hero = Hero;

            Locked = false;
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

        class OnSelectProxy : Lambda
        {
            StartMenu_MW_HeroSelect smmwhs;

            public OnSelectProxy(StartMenu_MW_HeroSelect smmwhs)
            {
                this.smmwhs = smmwhs;
            }

            public void Apply()
            {
                smmwhs.OnSelect();
            }
        }

        void OnSelect()
        {
            var item = MyMenu.CurItem as HeroItem;
            if (null == item) return;

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
        }

        public HeroDoll MyHeroDoll;
        public override void Init()
        {
 	        base.Init();

            MyPile = new DrawPile();

            CallDelay = ReturnToCallerDelay = 0;

            Score = new EzText("0", Resources.Font_Grobold42_2);
            Level = new EzText("0", Resources.Font_Grobold42_2);

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

            BobPhsx JetpackWheelie = BobPhsx.MakeCustom(Hero_BaseType.Wheel, Hero_Shape.Classic, Hero_MoveMod.Jetpack);
            JetpackWheelie.Name = Localization.Words.JetpackWheelie;
            JetpackWheelie.Id = 12;

            //BobPhsxMeat.Instance =
            //BobPhsxRocketbox.Instance =

            var list = new BobPhsx[] { BobPhsxNormal.Instance,
                                       BobPhsxBig.Instance,
                                       BobPhsxInvert.Instance,                                       
                                       BobPhsxDouble.Instance,
                                       BobPhsxJetman.Instance,
                                       BobPhsxBouncy.Instance,
                                       BobPhsxBox.Instance,
                                       BobPhsxScale.Instance,
                                       BobPhsxTime.Instance,
                                       BobPhsxSmall.Instance,
                                       BobPhsxSpaceship.Instance,
                                       BobPhsxWheel.Instance,
                                       JetpackWheelie,
                                       //BobPhsxRocketbox.Instance,
                                       //BobPhsxMeat.Instance,
                                     };
                                        
            // Menu
            MiniMenu mini = new MiniMenu();
            MyMenu = mini;

            MyMenu.OnSelect = new UpdateScoreProxy(this);

            mini.WrapSelect = false;
            mini.Shift = new Vector2(0, -135);
            mini.ItemsToShow = 6;
            FontScale *= .75f;
            foreach (var phsx in list)
            {
                var item = new HeroItem(phsx);
                item.AdditionalOnSelect = new OnSelectProxy(this);
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
        }

        class UpdateScoreProxy : Lambda
        {
            StartMenu_MW_HeroSelect smmwhs;

            public UpdateScoreProxy(StartMenu_MW_HeroSelect smmwhs)
            {
                this.smmwhs = smmwhs;
            }

            public void Apply()
            {
                smmwhs.UpdateScore();
            }
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

        void SetPos()
        {
            MyMenu.Pos = new Vector2(-1340.222f, 104.4444f);

            EzText _t;
            _t = MyPile.FindEzText("ScoreHeader"); if (_t != null) { _t.Pos = new Vector2(-22.22266f, 636.1111f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("Score"); if (_t != null) { _t.Pos = new Vector2(1161.11f, 366.6667f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("LevelHeader"); if (_t != null) { _t.Pos = new Vector2(-2.779297f, 105.5556f); _t.Scale = 1f; }
            _t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(1163.887f, -155.5555f); _t.Scale = 1f; }

            QuadClass _q;
            _q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(-972.2227f, -127.7778f); _q.Size = new Vector2(616.5467f, 1004.329f); }
            _q = MyPile.FindQuad("BoxRight"); if (_q != null) { _q.Pos = new Vector2(666.6641f, -88.88879f); _q.Size = new Vector2(776.5515f, 846.666f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1269.443f, -1011.111f); _q.Size = new Vector2(64.49973f, 64.49973f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-1416.666f, -1016.667f); _q.Size = new Vector2(71.89921f, 61.83332f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);

        }

        protected virtual void Go(MenuItem item)
        {
            StartLevelMenu levelmenu = new StartLevelMenu(MyArcadeItem.MyChallenge.TopLevel());

            levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
            levelmenu.StartFunc = new StartFuncProxy(this);
            levelmenu.ReturnFunc = null;

            Call(levelmenu);
            Hide();
        }
    }
}