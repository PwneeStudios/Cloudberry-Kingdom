using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class StartMenu_Clouds_PressStart : CkBaseMenu
    {
        public TitleGameData_Clouds Title;
        public StartMenu_Clouds_PressStart(TitleGameData_Clouds Title)
            : base()
        {
            CloudberryKingdomGame.PastPressStart = false;

            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(TitleBackgroundState.Scene_Title);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        Text Text;
        int DelayToAllowInput;
        public override void Init()
        {
 	        base.Init();

            DelayToAllowInput = 15;

            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            EnsureFancy();

#if PC
			if (ButtonCheck.ControllerInUse)
			{
				Text = new Text(Localization.Words.PressStart, Resources.Font_Grobold42, true);
			}
			else
			{
				Text = new Text(Localization.Words.PressAnyKey, Resources.Font_Grobold42, true);
			}
#else
			Text = new Text(Localization.Words.PressStart, Resources.Font_Grobold42, true);
#endif
            Text.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Text);

            SetPos();
        }
        
        void SetPos()
        {
            Text _t;
            _t = MyPile.FindText(""); if (_t != null) { _t.Pos = new Vector2(-11.11157f, -461.111f); _t.Scale = 0.66054f; }
            MyPile.Pos = new Vector2(38.88843f, -252.7777f);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep(); if (!Active) return;

            if (DelayToAllowInput > 0)
            {
                DelayToAllowInput--;
                return;
            }

            if (ButtonCheck.AnyKey() && !ButtonCheck.State(ControllerButtons.B, -2).Down
#if PC
				&& !Tools.CurRightMouseDown()
#endif
				)
            {
                DelayToAllowInput = 10;

                CloudberryKingdomGame.PastPressStart = true;

				Tools.Write("Forced select avoided");
                CallMenu();
            }
        }

		void CallMenu()
		{
			if (CloudberryKingdomGame.SimpleMainMenu)
				Call(new StartMenu_Clouds_Simple(Title));
			else
				Call(new StartMenu_Clouds_Pre(Title));
			Hide();
		}

        float t = 0;
        protected override void MyDraw()
        {
            if (!Active) return;

            float s = CoreMath.Periodic(.85f, 1f, 3, Tools.t);
            Text.MyFloatColor = ColorHelper.Gray(.93f * s);

            Text.Scale = CoreMath.Periodic(.818668f, .838668f, 3, Tools.t) * .8f;

            t += .01f;
            Text.Alpha = t;

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            CloudberryKingdomGame.PastPressStart = false;

            base.OnReturnTo();
        }
    }
}