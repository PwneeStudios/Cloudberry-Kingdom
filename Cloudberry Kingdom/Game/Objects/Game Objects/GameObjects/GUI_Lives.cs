using System.Text;

using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    /// <summary>
    /// A GUI element that tells the player how many lives they have, while they play.
    /// </summary>
    public class GUI_Lives : GUI_Panel
    {
        StringBuilder MyString = new StringBuilder(50, 50);

        public int Lives { get { return MyGUI_Lives.NumLives; } }
        GUI_LivesLeft MyGUI_Lives;

        /// <summary>
        /// Return a string representation of the number of Lives
        /// </summary>
        /// <returns></returns>
        public StringBuilder BuildString()
        {
            MyString.Length = 0;

            MyString.Append('x');
            MyString.Append(' ');
            MyString.Add(Lives, 1);

            return MyString;
        }

        bool AddedOnce = false;
        public override void OnAdd()
        {
            base.OnAdd();

            if (!AddedOnce)
            {
                Hide();
                
                Show();
            }

            AddedOnce = true;
        }

        public override void Hide()
        {
            base.Hide();
            SlideOut(PresetPos.Top, 0);
        }

        public override void Show()
        {
            base.Show();
            SlideIn();
            MyPile.BubbleUp(false);
        }

        public Vector2 ApparentPos
        {
            get { return Text.FancyPos.AbsVal + Text.GetWorldSize() / 2; }
        }

        QuadClass Bob;
        EzText Text;
        void UpdateLivesText()
        {
            Text.SubstituteText(BuildString());
        }

        public GUI_Lives(GUI_LivesLeft GUI_Lives) : base(false)
        {
            MyGUI_Lives = GUI_Lives;
            Constructor();
        }
        
        public override void Init()
        {
            base.Init();

            MyPile = new DrawPile();
            EnsureFancy();

            Vector2 shift = new Vector2(-320, 0);
            MyPile.Pos = new Vector2(1356.112f, -848.889f);
            SlideInLength = 0;

            // Object is carried over through multiple levels, so prevent it from being released.
            PreventRelease = true;

            PauseOnPause = true;

            MyPile.FancyPos.UpdateWithGame = true;

            Text = new EzText(BuildString().ToString(), Resources.Font_Grobold42, 450, false, false);
            Text.Scale = .625f;
            Text.Pos = new Vector2(-67.22302f, 83.26669f);
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();
            MyPile.Add(Text);

            Bob = new QuadClass("Stickman", 64, true);
            Bob.Pos = new Vector2(200.5664f, -42.03058f) + shift;
            MyPile.Add(Bob);
        }

        protected override void MyDraw()
        {
            return;

            if (!Core.Show || Core.MyLevel.SuppressCheckpoints) return;

            base.MyDraw();
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (MyGame.SoftPause) return;
            if (Hid || !Active) return;

            if (Core.MyLevel.Watching || Core.MyLevel.Finished) return;

            UpdateLivesText();
        }
    }
}