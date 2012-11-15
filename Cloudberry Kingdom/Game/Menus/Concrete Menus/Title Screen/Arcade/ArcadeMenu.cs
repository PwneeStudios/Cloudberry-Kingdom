using Microsoft.Xna.Framework;
using CloudberryKingdom.Awards;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class ArcadeItem : MenuItem
    {
        public Challenge MyChallenge;
        public Awardment MyPrereq;
        public bool Locked;

        public ArcadeItem(EzText Text, Challenge MyChallenge, Awardment MyPrereq) : base(Text)
        {
            this.MyChallenge = MyChallenge;
            this.MyPrereq = MyPrereq;

            Locked = MyPrereq != null && !PlayerManager.Awarded(MyPrereq) && !CloudberryKingdomGame.UnlockAll;
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

        class PlayGameProxy : Lambda
        {
            ArcadeBaseMenu abm;

            public PlayGameProxy(ArcadeBaseMenu abm)
            {
                this.abm = abm;
            }

            public void Apply()
            {
                abm.PlayGame();
            }
        }

        protected class StartFuncProxy : Lambda_1<LevelItem>
        {
            ArcadeBaseMenu abm;

            public StartFuncProxy(ArcadeBaseMenu abm)
            {
                this.abm = abm;
            }

            public void Apply(LevelItem levelitem)
            {
                abm.StartFunc(levelitem);
            }
        }

        protected virtual void StartFunc(LevelItem item)
        {
            SelectedItem = item;

            // Save the menu item index
            StartLevelMenu.PreviousMenuIndex = item.MenuIndex;

            // Start the game
            MyGame.PlayGame(new PlayGameProxy(this));
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
        }

        void SetLockColors()
        {
            foreach (MenuItem item in MyMenu.Items)
            {
                Awardment award = item.MyObject as Awardment;
                if (null != award && !PlayerManager.Awarded(award) && !CloudberryKingdomGame.UnlockAll)
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

            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

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
            item = AddChallenge(Challenge_Escalation.Instance, null, null, "Escalation");

            // Time Crisis
            item = AddChallenge(Challenge_TimeCrisis.Instance, null, Awardments.UnlockHeroRush2, "Time Crisis");

            // Hero Rush
            item = AddChallenge(Challenge_HeroRush.Instance, null, Awardments.UnlockHeroRush2, "Hero Rush");

            // Hero Rush 2
            item = AddChallenge(Challenge_HeroRush2.Instance, Awardments.UnlockHeroRush2, null, "Hero Rush 2");

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

        private MenuItem AddChallenge(Challenge challenge, Awardment prereq, Awardment goal, string itemname)
        {
            ArcadeItem item;
            Localization.Words word = challenge.MenuName != null ? challenge.MenuName : challenge.Name;
            
            item = new ArcadeItem(new EzText(word, ItemFont), challenge, prereq);

            item.Name = itemname;
            AddItem(item);

            item.Go = new GoProxy(this);

            return item;
        }

        class GoProxy : Lambda_1<MenuItem>
        {
            ArcadeMenu am;

            public GoProxy(ArcadeMenu am)
            {
                this.am = am;
            }

            public void Apply(MenuItem item)
            {
                am.Go(item);
            }
        }

        public virtual void Go(MenuItem item)
        {
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();
        }
    }
}