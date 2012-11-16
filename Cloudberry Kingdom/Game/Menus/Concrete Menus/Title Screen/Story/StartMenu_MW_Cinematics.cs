using Microsoft.Xna.Framework;


namespace CloudberryKingdom
{
    class CinematicsLevelItem : MenuItem
    {
        public string Movie = "";

        public CinematicsLevelItem(EzText Text, string Movie)
            : base(Text)
        {
            this.Movie = Movie;
        }
    }

    public class StartMenu_MW_Cinematics : StartMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_Cinematics(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Princess);

            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected void SetText(EzText text)
        {
            text.MyFloatColor = new Color(34, 214, 47).ToVector4();
            text.OutlineColor = new Color(0, 0, 0, 0).ToVector4();
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            SetText(item.MyText);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;
            item.MySelectedText.MyFloatColor = new Color(73, 255, 86).ToVector4(); 
            item.MySelectedText.OutlineColor = new Color(0, 0, 0, 0).ToVector4();

            item.MyOscillateParams.Set(1f, 1.01f, .005f);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = new MenuReturnToCallerLambdaFunc(this);

            MyMenu.ClearList();

            MakeHeader();

            CreateMenu();
        }

        protected virtual void CreateMenu()
        {
            MenuItem item;

            // Chapter 1
            item = new CinematicsLevelItem(new EzText("Over the Edge", ItemFont), "Cutscene_1");
            item.Name = "1";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            // Chapter 2
            item = new CinematicsLevelItem(new EzText("Into the Forest", ItemFont), "Cutscene_2");
            item.Name = "2";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            // Chapter 3
            item = new CinematicsLevelItem(new EzText("Woes of a Kidnapper", ItemFont), "Cutscene_3");
            item.Name = "3";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            // Chapter 4
            item = new CinematicsLevelItem(new EzText("Welterweight", ItemFont), "Cutscene_4");
            item.Name = "4";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            // Chapter 5
            item = new CinematicsLevelItem(new EzText("Cloudberry Pie", ItemFont), "Cutscene_5");
            item.Name = "5";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            // Chapter 6
            item = new CinematicsLevelItem(new EzText("I Always Told You", ItemFont), "Cutscene_6");
            item.Name = "6";
            item.Go = new CinematicsGoLambda(this);
            AddItem(item);

            MyMenu.SelectItem(0);

            SetPos();
        }

        protected void MakeHeader()
        {
            var Header = new EzText(Localization.Words.StoryMode, ItemFont);
            Header.Name = "Header";
            Header.Scale *= 1.3f;
            SetText(Header);
            Header.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Header);
            
            Header.Pos = new Vector2(-800.0029f, 863.8889f);
        }

        class CinematicsGoLambda : Lambda_1<MenuItem>
        {
            StartMenu_MW_Cinematics cine;
            public CinematicsGoLambda(StartMenu_MW_Cinematics cine)
            {
                this.cine = cine;
            }

            public void Apply(MenuItem item)
            {
                cine.Go(item);
            }
        }

        public void Go(MenuItem item)
        {
            CinematicsLevelItem c_item = item as CinematicsLevelItem;
            if (null == c_item) return;

            MainVideo.StartVideo_CanSkipIfWatched_OrCanSkipAfterXseconds(c_item.Movie, 1);
        }

        void SetPos()
        {
        }
    }
}