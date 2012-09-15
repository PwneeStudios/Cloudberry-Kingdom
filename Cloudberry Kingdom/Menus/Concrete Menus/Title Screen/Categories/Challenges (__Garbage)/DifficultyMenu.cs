using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class DifficultyMenu : StartMenuBase
    {
        protected override void SetHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Scale = FontScale * 1.2f;
        }

        public Action<int,int> StartFunc;
        public Action ReturnFunc;

        public void SplitHide()
        {
            base.Hide();

            SlideOut(PresetPos.Left);
            RightPanel.SlideOut(PresetPos.Right);
        }

        public override void Hide()
        {
            if (Hid) return;

            base.Hide();
        }

        protected virtual void Launch(int index, int menuindex)
        {
            SplitHide();

            MyGame.WaitThenDo(CallDelay, () =>
            {
                // Executed once the game exits back to this menu
                MyGame.AddToDo((Func<bool>)GameReturn);

                StartFunc(index, menuindex);
                
            }, "StartGame");
        }

        protected virtual bool GameReturn()
        {
            ReturnFunc();

            // Return to the parent menu
            MyMenu.OnB(null);

            return true;
        }

        protected float ItemFontScaleMod = 1f;
        protected override void AddItem(MenuItem item)
        {
            base.AddItem(item);
            
            item.MyText.Scale = item.MySelectedText.Scale *= 1.1f * ItemFontScaleMod;
        }

        public virtual string[] GetNames()
        {
            string[] names = new string[] { "Easy", "Normal", "Abusive", "Hardcore" };
            names = names.Range(0, MaxDifficulty);
            return names;
        }

        public int MaxDifficulty;
        public DifficultyMenu() { }
        public DifficultyMenu(int MaxDifficulty)
        {
            this.MaxDifficulty = MaxDifficulty;

            Initialize();
        }

        public void ShowHighScore(int score)
        {
            if (score == 0) return;
            if (RightPanel == null) return;

            string str = ScoreEntry.DottedScore("High Score", score, 23, 4);
            EzText Text = new EzText(str, Tools.Font_Grobold42, 1450, false, true, .6f);
            Text.Scale = .86f;
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();
            RightPanel.MyPile.Add(Text);
            Text.Pos = new Vector2(-811.1123f, -791.5874f);
            Text.Layer = 1;
        }

        protected QuadClass Backdrop;
        protected EzText Header;

        public int IndexCutoff = 0;

        protected string HeaderText = "Choose difficulty!";
        public void Initialize()
        {
            ReturnToCallerDelay = 16;
            SlideLength = 26;

            ItemPos = new Vector2(-1317, 600);
            PosAdd = new Vector2(0, -151) * 1.37f;

            RightPanel = new DiffPics();

            SlideInFrom = SlideOutTo = PresetPos.Right;

            MyMenu = new Menu(false);

            MyMenu.Control = -1;

            MyMenu.OnB = MenuReturnToCaller;


            Vector2 shift = new Vector2(-200, 40);
            MenuItem item;

            //string[] Names = { "Training", "Normal", "Abusive", "Hardcore" };
            string[] Names = GetNames();

            for (int i = 0; i < Names.Length; i++)
            {
                item = new MenuItem(new EzText(Names[i], ItemFont));
                AddItem(item);

                int diff = i;
                int menuindex = MyMenu.Items.IndexOf(item);

                bool Unlocked = i < IndexCutoff;
                if (Unlocked)
                    item.Go = me => Launch(diff, menuindex);
                else
                {
                    item.Go = null;
                    item.MyText.MyFloatColor = new Color(255, 100, 100).ToVector4();
                    item.MySelectedText.MyFloatColor = new Color(255, 160, 160).ToVector4();
                }
                
                item.AdditionalOnSelect = () => { if (RightPanel != null) ((DiffPics)RightPanel).SetDiffPic(diff + 1); };
            }
            ItemPos += PosAdd * .3f;

            item = MakeBackButton();
            //item.MyText.Scale = item.MySelectedText.Scale *= 1.2f;
            item.MyText.Scale = item.MySelectedText.Scale *= 1.06f;

            this.EnsureFancy();
            MyMenu.FancyPos.RelVal = new Vector2 (285, -60) + shift;

            // Backdrop
            //Vector2 TR = new Vector2(0, 780);
            //Vector2 BL = new Vector2(-1335, this.ItemPos.Y - 200);
            //MakeBackdrop(TR, BL);
            //MyMenu.BackdropShift = new Vector2(-848, -7.4f);


            MyPile = new DrawPile();

            // Header
            Header = new EzText(HeaderText, ItemFont);
            MyPile.Add(Header);
            Header.Pos = new Vector2(-1751.666f, 903.8889f);
            SetHeaderProperties(Header);

            // Backdrop
            Backdrop = new QuadClass("Backplate_1500x900", 1500, true);
            //Backdrop = new QuadClass("Backplate_1500x900", 1500);
            MyPile.Add(Backdrop);
            Backdrop.Pos =
                new Vector2(-225.79859f, 24.60343f);
                //new Vector2(-597.2174f, 238.8891f);

            // Layer everything
            MyMenu.Layer = 0;
            Header.Layer = 1;
        }
    }
}