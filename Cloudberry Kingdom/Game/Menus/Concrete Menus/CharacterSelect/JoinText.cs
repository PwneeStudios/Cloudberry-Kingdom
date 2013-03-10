using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using CoreEngine;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class JoinText : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
        public JoinText(int Control, CharacterSelect MyCharacterSelect)
            : base(false)
        {
            this.Tags += Tag.CharSelect;
            this.Control = Control;
            this.MyCharacterSelect = MyCharacterSelect;

            Constructor();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();

            MyCharacterSelect = null;
        }

        EzText Text;
        public override void Init()
		{
			base.Init();

			SlideInLength = 0;
			SlideOutLength = 0;
			CallDelay = 0;
			ReturnToCallerDelay = 0;

			MyPile = new DrawPile();
			EnsureFancy();

			// Press A to join
			int ButtonSize = 89;
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				ButtonSize = 75;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				ButtonSize = 94;
			}
#if PC_VERSION
			string pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go_Controller(ButtonSize));
#else
			string pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go(ButtonSize));
#endif

			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				Text = new EzText(pressa, Resources.Font_Grobold42, 1000, true, true, .5f);

				Text.Pos = new Vector2(11.11133f, 63.88889f); Text.Scale = 0.9542501f;
			}
			else
			{
				Text = new EzText(pressa, Resources.Font_Grobold42, true, true);
				Text.Scale = .7765f;
			}

			Text.ShadowOffset = new Vector2(7.5f, 7.5f);
			Text.ShadowColor = new Color(30, 30, 30);
			Text.ColorizePics = true;

			MyPile.Add(Text);

			CharacterSelect.Shift(this);


			// SetPos
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
			{
				Text.Pos = new Vector2(0f, 0f);
				Text.Scale = 0.5720017f;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
			{
				Text.Pos = new Vector2(0f, 0f);
				Text.Scale = 0.570833f;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				Text.Pos = new Vector2(0f, 0f);
				Text.Scale = 0.575f;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
			{
				Text.Pos = new Vector2(0f, 0f);
				Text.Scale = 0.5195001f;
			}
		}

        public static void ScaleGamerTag(EzText GamerTag)
        {
            GamerTag.Scale *= 850f / GamerTag.GetWorldWidth();

            float Height = GamerTag.GetWorldHeight();
            float MaxHeight = 380;
            if (Height > MaxHeight)
                GamerTag.Scale *= MaxHeight / Height;
        }

        void SetGamerTag()
        {
            Tools.StartGUIDraw();
            if (MyCharacterSelect.Player.Exists)
            {
                string name = MyCharacterSelect.Player.GetName();
                Text = new EzText(name, Resources.Font_Grobold42, true, true);
                ScaleGamerTag(Text);
            }
            else
            {
                Text = new EzText("ERROR", Resources.LilFont, true, true);
            }

            Text.Shadow = false;
            Text.PicShadow = false;

            Tools.EndGUIDraw();
        }

        protected override void MyDraw()
        {
            if (CharacterSelectManager.FakeHide)
                return;

            base.MyDraw();
        }

		public override void OnReturnTo()
		{
			base.OnReturnTo();

			MyCharacterSelect.InitColorScheme(MyCharacterSelect.PlayerIndex);
		}

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;
            MyCharacterSelect.MyState = CharacterSelect.SelectState.Beginning;
            MyCharacterSelect.MyDoll.ShowBob = false;
            MyCharacterSelect.MyGamerTag.ShowGamerTag = false;
            MyCharacterSelect.Player.Exists = false;

            // Use this if statement if you want keyboard to control all characters (For debugging)
            //if (ButtonCheck.State(ControllerButtons.A, -2).Pressed)

            if (ButtonCheck.State(ControllerButtons.A, Control).Pressed)
            {
#if XDK
                if (MyCharacterSelect.Player.MyGamer != null)
                    Call(new SimpleMenu(Control, MyCharacterSelect));
                else
                {
                    //Call(new SignInMenu(Control, MyCharacterSelect));
                    if (!Guide.IsVisible)
                    {
                        try
                        {
                            Guide.ShowSignIn(4, false);
                            return;
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
#else
                Call(new SimpleMenu(Control, MyCharacterSelect));
#endif

                //Call(new SimpleMenu(Control, MyCharacterSelect));

                Hide();
            }
        }
    }
}