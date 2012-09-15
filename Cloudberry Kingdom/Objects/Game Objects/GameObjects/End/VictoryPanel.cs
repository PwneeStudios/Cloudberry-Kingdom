using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class VictoryPanel : GUI_Panel
    {
        public override void Init()
        {
            base.Init();

            Core.DrawLayer = Level.AfterPostDrawLayer;

            Style1();
        }

        int ShowDelay = 150;
        public override void OnAdd()
        {
            base.OnAdd();

            // Add cheers
            for (int i = 0; i < 3; i++)
            {
                int Delay = 48 * i;
                MyGame.WaitThenDo(Delay, () => MyGame.AddGameObject(new SuperCheer(2)));
            }

            // End level
            Core.MyLevel.EndLevel();

            // Slide out panel before presenting
            this.SlideOut(PresetPos.Top, 0);
        }

        int Step = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            // Return to main menu when the player presses 'A'
            if (Step > ShowDelay + 25 && ButtonCheck.State(ControllerButtons.A, -1).Pressed)
            {
                MyGame.EndGame(false);
                return;
            }

            Step++;

            // Show self
            if (Step == ShowDelay)
                SlideIn(20);
        }

        public VictoryPanel()
        {
        }

        EzText TextStyle1(bool Center)
        {
            EzText Text = new EzText("Victory!", Tools.Font_Grobold42_2, 1450, Center, true, .6f);
            Text.Scale = .78f;
            Text.MyFloatColor = new Color(255, 255, 255).ToVector4();
            Text.OutlineColor = new Color(0, 0, 0).ToVector4();

            return Text;
        }

        void Style1()
        {
            MyPile = new DrawPile();

            QuadClass Berry = new QuadClass();
            Berry.SetToDefault();
            Berry.TextureName = "cb_enthusiastic";
            Berry.Scale(500);
            Berry.ScaleYToMatchRatio();

            Berry.Pos = new Vector2(0, 225);
            MyPile.Add(Berry);



            MyPile.Backdrop = new PieceQuad();
            MyPile.Backdrop.Clone(StartMenuBase.MenuTemplate);
            MyPile.Backdrop.CalcQuads(new Vector2(815, 860));
            MyPile.BackdropShift = new Vector2(0, 120);
            StartMenuBase.SetBackdropProperties(MyPile.Backdrop);



            EzText Text = TextStyle1(false);
            Text.Pos = new Vector2(0, -385);
            Text.Center();
            MyPile.Add(Text);
        }

        protected override void MyDraw()
        {
 	         base.MyDraw();
        }
    }
}