using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_PressStart : CkBaseMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_PressStart(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            Title.BackPanel.SetState(StartMenu_MW_Backpanel.State.Scene_Title);
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

        EzText Text;
        int DelayToAllowInput;
        public override void Init()
        {
 	        base.Init();

            //DelayToAllowInput = 80;
            //DelayToAllowInput = 50;
            DelayToAllowInput = 15;

            CallDelay = ReturnToCallerDelay = 0;

            MyPile = new DrawPile();

            EnsureFancy();

            //Text = new EzText("Press any key to start", Resources.Font_Grobold42, true);
            Text = new EzText(Localization.Words.PressStart, Resources.Font_Grobold42, true);
            //Text = new EzText(Localization.Words.PressStart, ItemFont);

            Text.MyFloatColor = new Color(226, 10, 83).ToVector4();
            Text.OutlineColor = Color.Black.ToVector4();
            MyPile.Add(Text);

            SetPos();
        }
        
        void SetPos()
        {
            EzText _t;
            _t = MyPile.FindEzText(""); if (_t != null) { _t.Pos = new Vector2(-11.11157f, -461.111f); _t.Scale = 0.66054f; }
            MyPile.Pos = new Vector2(-91.66675f, -250f);
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep(); if (!Active) return;

            if (DelayToAllowInput > 0)
            {
                DelayToAllowInput--;
                return;
            }

            if (ButtonCheck.AnyKey() && !ButtonCheck.State(ControllerButtons.B, -2).Down)
            {
                DelayToAllowInput = 10;

#if XDK || XBOX
                if (Gamer.SignedInGamers.Count > 0)
                {
                    foreach (var gamer in Gamer.SignedInGamers)
                    {
                        if (EzStorage.Device[(int)gamer.PlayerIndex] == null)
                        {
#if XDK || XBOX
                            if (!CloudberryKingdomGame.IsDemo)
                            {
                                SaveGroup.LoadGamers();
                            }
#endif
                            Hide();
                            MyGame.WaitThenDo(5, CallMenu);
                            return;
                        }
                    }
                }
#endif
      
                CallMenu();
            }
        }

		void CallMenu()
		{
			CloudberryKingdomGame.PastPressStart = true;

			if (CloudberryKingdomGame.SimpleMainMenu)
				Call(new StartMenu_MW_Simple(Title));
			else
				Call(new StartMenu_MW_Pre(Title));
			Hide();
		}

        float t = 0;
        protected override void MyDraw()
        {
            if (!Active) return;

            float s = CoreMath.Periodic(.85f, 1f, 3, Tools.t);
            Text.MyFloatColor = new Color((int)(226 * s), 10, 83).ToVector4();

            Text.Scale = CoreMath.Periodic(.818668f, .838668f, 3, Tools.t) * .8f;

            t += .01f;
            Text.Alpha = t;

            base.MyDraw();
        }

        public override void OnReturnTo()
        {
            base.OnReturnTo();
        }
    }
}