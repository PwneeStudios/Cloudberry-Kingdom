using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class StartLevelMenu_MW : CkBaseMenu
    {
        protected override void SetHeaderProperties(EzText text)
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

            StartMenu.SetItemProperties_Green(item, true);

            item.ScaleText(1.25f);

            item.MyText.Shadow = false;
            item.MySelectedText.Shadow = false;
        }

        int[] Levels = { 1, 50, 100, 150 };
        public virtual string[] GetNames()
        {
            string[] names = new string[Levels.Length];

            for (int i = 0; i < Levels.Length; i++)
                names[i] = string.Format("{0:00}", Levels[i]);

            return names;
        }

        public int HighestLevel;
        public StartLevelMenu_MW() { }
        public StartLevelMenu_MW(int HighestLevel)
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
            PosAdd = new Vector2(0, -151) * 1.45f;

            SlideLength = 0;
            CallDelay = ReturnToCallerDelay = 0;

            SlideInFrom = SlideOutTo = PresetPos.Right;

            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;


            Vector2 shift = new Vector2(-200, 40);

            string[] Names = GetNames();

            for (int i = 0; i < Names.Length; i++)
            {
                int StartLevel = Levels[i];
                int MenuIndex = i;
                bool Locked = i >= IndexCutoff;

                var item = new LevelItem(new EzText(Names[i], Resources.Font_Grobold42), StartLevel, MenuIndex, Locked);
                if (!Locked) item.Go = Launch;

                AddItem(item);
                item.SelectedPos.X -= 25;

                if (Locked)
                {
                    item.MyText.Alpha = .4f;
                    item.MySelectedText.Alpha = .4f;
                }
            }
            ItemPos += PosAdd * .3f;

            this.EnsureFancy();

            MyPile = new DrawPile();

            // Backdrop
            var Backdrop = new QuadClass("Arcade_Box");
            Backdrop.ScaleYToMatchRatio(587);
            MyPile.Add(Backdrop);

            // Header
            var Header = new EzText(Localization.Words.Level, Resources.Font_Grobold42);
            MyPile.Add(Header);
            SetHeaderProperties(Header);

            SetPos();

			// Back button
			Back = new ClickableBack(MyPile, false, true);
        }

#if PC_VERSION
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
            MyMenu.Pos = new Vector2(647.2223f, -447.2223f);

            EzText _t;
            _t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-749.9998f, 761.1111f); _t.Scale = 1.30125f; }

            QuadClass _q;
            _q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(-591.6664f, -63.88891f); _q.Size = new Vector2(884.3204f, 964.1653f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}