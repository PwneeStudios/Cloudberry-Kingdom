using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BonusWin : StartMenuBase
    {
        int Score;
        public BonusWin(int Score)
            : base(false)
        {
            this.Score = Score;
            Constructor();
        }

        public override void Init()
        {
            base.Init();

            DestinationScale *= 1.06f;

            MyPile = new DrawPile();
            EnsureFancy();
            MyPile.Pos = new Vector2(-212.1213f, 330.7575f);

            var
            //quad = new QuadClass("score screen_grey");
            //MyPile.Add(quad);

            quad = new QuadClass("cloud1", 1000, true);
            quad.LightAlpha = new Vector2(970, 970);
            quad.Pos = new Vector2(-212.1216f, -353.5355f);
            quad.Size = new Vector2(1333.333f, 1020.274f);
            MyPile.Add(quad);

            quad = new QuadClass("cb_surprised", 800, true);
            quad.Quad.MirrorUV_Horizontal();
            quad.Pos = new Vector2(-515.152f, -151.5149f);
            quad.Size = new Vector2(683.8384f, 882.0042f);
            MyPile.Add(quad);

            var
            text = new EzText("+ Score " + Score.ToString(), Tools.Font_Grobold42);
            text.Pos = new Vector2(-484.8484f, -515.1514f);
            MyPile.Add(text);
            SetTextProperties(text);
            text.Scale *= 1.15f;
            //CampaignMenu.AbusiveColor(text);
            CampaignMenu.RegularColor(text);

            text = new EzText("Oh,", Tools.Font_Grobold42);
            text.Pos = new Vector2(-55.55566f, 75.75787f);
            MyPile.Add(text);
            SetTextProperties(text);

            text = new EzText("yeah!", Tools.Font_Grobold42);
            text.Pos = new Vector2(151.5151f, -181.8183f);
            MyPile.Add(text);
            SetTextProperties(text);
        }

        protected override void SetTextProperties(EzText text)
        {
            text.Scale = 1.35f;
            CampaignMenu.EasyColor(text);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            SlideOut(PresetPos.Right, 0);
            SlideIn(20);
        }

        int Count = 0;
        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            Count++;
            if (Count > 50 && ButtonCheck.State(ControllerButtons.A, -1).Pressed)
            {
                Active = false;
                Finish();
            }
        }

        void Finish()
        {
            SlideOut(PresetPos.Left, 20);
            MyGame.WaitThenDo(41, () =>
            {
                // Aftermath
                Tools.CurrentAftermath = new AftermathData();
                Tools.CurrentAftermath.Success = true;

                Tools.CurGameData.EndGame(false);
            });
        }
    }
}