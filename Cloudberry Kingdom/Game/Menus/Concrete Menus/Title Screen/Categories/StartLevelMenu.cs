using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class LevelItem : MenuItem
    {
        public int StartLevel, MenuIndex;

        public LevelItem(EzText Text, int StartLevel, int MenuIndex, bool Locked)
            : base(Text)
        {
            this.StartLevel = StartLevel - 1;
            this.MenuIndex = MenuIndex;

            if (Locked)
            {
                MyText.MyFloatColor = new Color(255, 100, 100).ToVector4();
                MySelectedText.MyFloatColor = new Color(255, 160, 160).ToVector4();
            }
        }
    }

    public class StartLevelMenu : CkBaseMenu
    {
        /// <summary>
        /// The last difficulty selected via the difficulty select menu
        /// </summary>
        public static int PreviousMenuIndex = 0;

        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            StartMenu.SetText_Green(text, true);

            text.Shadow = false;
        }

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
        //string[] LevelStr = { "Normal", "Advanced", "Expert", "Master" };
        public virtual string[] GetNames()
        {
            string[] names = new string[Levels.Length];

            for (int i = 0; i < Levels.Length; i++)
                names[i] = string.Format("{0:00}", Levels[i]);
            //names[i] = string.Format("Level {0}", Levels[i]);

            return names;
        }

        public int HighestLevel;
        public StartLevelMenu() { }
        public StartLevelMenu(int HighestLevel)
        {
            this.HighestLevel = HighestLevel;

            // Allow user to choose amongst any start level in the array Levels,
            // assuming they have previously gotten to that level.
            IndexCutoff = 1;
            for (int i = 0; i < Levels.Length; i++)
                if (HighestLevel >= Levels[i] || CloudberryKingdomGame.UnlockAll) IndexCutoff = i + 1;

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

            string HeaderText = "Level";
            string[] Names = GetNames();

            for (int i = 0; i < Names.Length; i++)
            {
                int StartLevel = Levels[i];
                int MenuIndex = i;
                bool Locked = i >= IndexCutoff;

                var item = new LevelItem(new EzText(Names[i], Tools.Font_Grobold42), StartLevel, MenuIndex, Locked);
                if (!Locked) item.Go = Launch;

                AddItem(item);
                item.SelectedPos.X -= 25;
            }
            ItemPos += PosAdd * .3f;

            //item = MakeBackButton();
            //item.MyText.Scale = item.MySelectedText.Scale *= 1.06f;

            this.EnsureFancy();

            MyPile = new DrawPile();

            // Backdrop
            var Backdrop = new QuadClass("Arcade_Box");
            MyPile.Add(Backdrop);

            // Back
            var BackButton = new QuadClass(ButtonTexture.Back);
            MyPile.Add(BackButton, "Back");
            var BackArrow = new QuadClass("BackArrow2", "BackArrow");
            MyPile.Add(BackArrow);
            BackArrow.FancyPos.SetCenter(BackButton.FancyPos);

            // Header
            var Header = new EzText(HeaderText, Tools.Font_Grobold42);
            MyPile.Add(Header);
            SetHeaderProperties(Header);

            SetPos();
        }

        void SetPos()
        {
            MyMenu.Pos = new Vector2(647.2223f, -447.2223f);

            EzText _t;
            _t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-749.9998f, 761.1111f); _t.Scale = 1.30125f; }

            QuadClass _q;
            _q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(-591.6664f, -63.88891f); _q.Size = new Vector2(884.3204f, 964.1653f); }
            _q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1102.777f, -761.1107f); _q.Size = new Vector2(56.24945f, 56.24945f); }
            _q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

            MyPile.Pos = new Vector2(0f, 0f);
        }
    }
}