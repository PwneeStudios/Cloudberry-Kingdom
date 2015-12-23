using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartLevelMenu_Clouds : CkBaseMenu
    {
        protected override void SetHeaderProperties(Text text)
        {
            base.SetHeaderProperties(text);

            StartMenu.SetText_Green(text, true);

            text.Shadow = false;
        }

		ClickableBack Back;

        public Action<LevelItem> StartFunc;
        public Action ReturnFunc;

        protected virtual void Launch(MenuItem item)
        {
            LevelItem litem = item as LevelItem;
            if (null == litem) return;

            MyGame.WaitThenDo(CallDelay, () =>
            {
                // Executed once the game exits back to this menu
                MyGame.AddToDo((Func<bool>)GameReturn);

				Tools.PlayHappyMusic(MyGame);

                StartFunc(litem);
                
            }, "StartGame");
        }

        protected virtual bool GameReturn()
        {
            if (ReturnFunc != null) ReturnFunc();

            // Return to the parent menu
            MyMenu.OnB(null);

            return true;
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            StartMenu.SetItemProperties_ArcadeText(item);

            item.ScaleText(1.25f);

            item.MyText.Shadow = false;
            item.MySelectedText.Shadow = false;
        }

        int[] Levels = { 1, 25, 50, 75, 100, 125, 150 };
        public virtual string[] GetNames()
        {
            string[] names = new string[Levels.Length];

            for (int i = 0; i < Levels.Length; i++)
                names[i] = string.Format("{0:00}", Levels[i]);

            return names;
        }

        public int HighestLevel;
        public StartLevelMenu_Clouds() { }
        public StartLevelMenu_Clouds(int HighestLevel)
        {
            this.HighestLevel = HighestLevel;

            if (CloudberryKingdomGame.IsDemo)
            {
                HighestLevel = 1;
            }

            // Allow user to choose amongst any start level in the array Levels,
            // assuming they have previously gotten to that level.
            IndexCutoff = 1;
            for (int i = 0; i < Levels.Length; i++)
                if (HighestLevel >= Levels[i] || CloudberryKingdomGame.Unlock_Levels) IndexCutoff = i + 1;

            Initialize();
        }

        public int IndexCutoff = 0;

        public void Initialize()
        {
            ReturnToCallerDelay = 16;
            SlideLength = 26;

            ItemPos = new Vector2(-1317, 700);
            PosAdd = new Vector2(0, 200);

            SlideLength = 0;
            CallDelay = ReturnToCallerDelay = 0;

            SlideInFrom = SlideOutTo = PresetPos.Right;

            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;

            string[] Names = GetNames();

            for (int i = 0; i < Names.Length; i++)
            {
                int StartLevel = Levels[i];
                int MenuIndex = i;
                bool Locked = i >= IndexCutoff;

                var item = new LevelItem(new Text(Names[i], Resources.Font_Grobold42), StartLevel, Names.Length - 1 - MenuIndex, Locked);
                if (!Locked) item.Go = Launch;

                AddItem(item);

                if (Locked)
                {
                    item.MyText.Alpha = .4f;
                    item.MySelectedText.Alpha = .4f;
                }
            }
            ItemPos += PosAdd;

            MyMenu.SortByHeight();

            this.EnsureFancy();

            MyPile = new DrawPile();

            // Backdrop
            var Backdrop = new QuadClass("MediumBox");
            Backdrop.ScaleYToMatchRatio(587);
            MyPile.Add(Backdrop, "Box");

            // Header
            var Header = new Text(Localization.Words.Level, Resources.Font_Grobold42);
            MyPile.Add(Header, "Header");
            SetHeaderProperties(Header);

            SetPos();

			// Back button
			Back = new ClickableBack(MyPile, true, true);
            Back.SetPos_BR(MyPile);
        }

#if PC
		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
				return;
			}
		}
#endif

        void SetPos()
        {
            MyMenu.Pos = new Vector2(1086.111f, -1224.999f);

            Text _t;
            _t = MyPile.FindText("Header"); if (_t != null) { _t.Pos = new Vector2(-1511.111f, 27.77769f); _t.Scale = 1.30125f; }

            QuadClass _q;
            _q = MyPile.FindQuad("Box"); if (_q != null) { _q.Pos = new Vector2(-555.5569f, -77.77783f); _q.Size = new Vector2(2236.158f, 1222.99f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(83.33417f, 130.9524f);
        }
    }
}