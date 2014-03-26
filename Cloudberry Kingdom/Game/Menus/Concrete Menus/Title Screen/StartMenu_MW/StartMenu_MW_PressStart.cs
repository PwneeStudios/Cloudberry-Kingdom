using Microsoft.Xna.Framework;

#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif

using CoreEngine;

using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_PressStart : CkBaseMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_PressStart(TitleGameData_MW Title)
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

#if PC_VERSION
			if (ButtonCheck.ControllerInUse)
			{
				Text = new EzText(Localization.Words.PressStart, Resources.Font_Grobold42, true);
			}
			else
			{
				Text = new EzText(Localization.Words.PressAnyKey, Resources.Font_Grobold42, true);
			}
#else
			Text = new EzText(Localization.Words.PressStart, Resources.Font_Grobold42, true);
#endif
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

            if (ButtonCheck.AnyKey() && !ButtonCheck.State(ControllerButtons.B, -2).Down
#if PC_VERSION
				&& !Tools.CurRightMouseDown()
#endif
				)
            {
                DelayToAllowInput = 10;

                CloudberryKingdomGame.PastPressStart = true;

#if XDK || XBOX
                if (Gamer.SignedInGamers.Count > 0)
                {
                    bool LoadNeeded = false;

                    foreach (var gamer in Gamer.SignedInGamers)
                    {
                        int index = (int)gamer.PlayerIndex;

                        if (EzStorage.Device[index] == null ||
                            !EzStorage.Device[index].IsReady)
                        {
#if XDK || XBOX
                            if (!CloudberryKingdomGame.IsDemo)
                            {
                                if (EzStorage.Device[index] != null)
                                {
                                    Tools.GameClass.Components.Remove(EzStorage.Device[index]);
                                    EzStorage.Device[index] = null;
                                }

                                LoadNeeded = true;
                            }
#endif
                        }
                    }

                    if (LoadNeeded)
                    {
                        // Player needs a storage device
                        if (ButtonCheck.AnyKeyPlayer >= 0 && EzStorage.Device[ButtonCheck.AnyKeyPlayer] != null)
                        {
                            EzStorage.Device[ButtonCheck.AnyKeyPlayer].ChoseNotToSelectDevice = false;
                        }

                        SaveGroup.LoadGamers();

                        // Player needs a storage device
                        if (ButtonCheck.AnyKeyPlayer >= 0 && EzStorage.Device[ButtonCheck.AnyKeyPlayer] != null)
                        {
                            EzStorage.Device[ButtonCheck.AnyKeyPlayer].NeedsConnection = true;
                        }

                        Hide();
                        MyGame.WaitThenDo(1, CallMenu);
                        return;
                    }
                }
#endif
				Tools.Write("Forced select avoided");
                CallMenu();
            }
        }

		void CallMenu()
		{
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
            CloudberryKingdomGame.PastPressStart = false;

            base.OnReturnTo();
        }
    }
}