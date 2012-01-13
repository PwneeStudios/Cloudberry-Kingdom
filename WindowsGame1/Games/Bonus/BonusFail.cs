using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class BonusFail : StartMenuBase
    {
        public BonusFail()
            : base(false)
        {
            Constructor();
        }

        public override void Init()
        {
            base.Init();

            DestinationScale *= 1.06f;

            MyPile = new DrawPile();
            EnsureFancy();
            MyPile.Pos = new Vector2(90.90889f, 313.1312f);

            var
            //quad = new QuadClass("score screen_grey");
            //MyPile.Add(quad);

            quad = new QuadClass("cloud1", 1000, true);
            quad.LightAlpha = new Vector2(970, 970);
            quad.Pos = new Vector2(-212.1216f, -353.5355f);
            quad.Size = new Vector2(1333.333f, 1020.274f);
            MyPile.Add(quad);

            quad = new QuadClass("citizen_4", 800, true);
            quad.Pos = new Vector2(-469.6973f, -282.828f);
            MyPile.Add(quad);

            var
            //text = new EzText("Challenge failed", Tools.Font_DylanThin42);
            //text.Pos = new Vector2(-676.7676f, 656.5657f);
            //MyPile.Add(text);
            //SetTextProperties(text);

            text = new EzText("Challenge", Tools.Font_DylanThin42);
            text.Pos = new Vector2(-686.8687f, 570.7072f);
            MyPile.Add(text);
            SetTextProperties(text);

            text = new EzText("failed", Tools.Font_DylanThin42);
            text.Pos = new Vector2(-10.10086f, 202.02f);
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
            SlideIn(65);
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
            SlideOut(PresetPos.Top);
            MyGame.WaitThenDo(51, () =>
            {
                // Aftermath
                Tools.CurrentAftermath = new AftermathData();
                Tools.CurrentAftermath.Success = true;

                Tools.CurGameData.EndGame(false);
            });
        }
    }
}