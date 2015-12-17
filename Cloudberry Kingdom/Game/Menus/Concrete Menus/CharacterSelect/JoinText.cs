using Microsoft.Xna.Framework;

#if PC
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using CoreEngine;

namespace CloudberryKingdom
{
    public class JoinText : CkBaseMenu
    {
        CharacterSelect MyCharacterSelect;
		
#if PC
		QuadClass Overlay;
		EzText MouseClickText;
#endif
		
		EzText Text;

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

        public override void Init()
		{
			base.Init();

			SlideInLength = 0;
			SlideOutLength = 0;
			CallDelay = 0;
			ReturnToCallerDelay = 0;

			MyPile = new DrawPile();
			EnsureFancy();

			int ButtonSize = 0;
			string pressa = null;

#if PC
			Overlay = new QuadClass();
			Overlay.TextureName = "White";
			Overlay.Alpha = .13333f;

			Overlay.Pos = new Vector2(-2.777588f, 8.333324f);
			Overlay.Size = new Vector2(440.6654f, 1008.58f);
			
			MyPile.Add(Overlay, "Overlay");

			/* Alternate text for mouse hover */
			ButtonSize = 90;
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				ButtonSize = 70;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				ButtonSize = 70;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				ButtonSize = 95;
			}

			pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.LeftClick(ButtonSize));

			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				MouseClickText = new EzText(pressa, Resources.Font_Grobold42, 1000, true, true, .5f);
				MouseClickText.Pos = new Vector2(11.11133f, 63.88889f);
				MouseClickText.Scale = 0.9542501f;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				MouseClickText = new EzText(pressa, Resources.Font_Grobold42, 1000, true, true, .5f);
				MouseClickText.Pos = new Vector2(11.11133f, 63.88889f);
				MouseClickText.Scale = 0.9542501f;
			}
			else
			{
				MouseClickText = new EzText(pressa, Resources.Font_Grobold42, true, true);
				MouseClickText.Scale = .7765f;
			}

			MouseClickText.ShadowOffset = new Vector2(7.5f, 7.5f);
			MouseClickText.ShadowColor = new Color(30, 30, 30);
			MouseClickText.ColorizePics = true;

			MouseClickText.Show = false;
			MyPile.Add(MouseClickText);
#endif

			// Press A to join
			ButtonSize = 89;
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				ButtonSize = 75;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				ButtonSize = 75;
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
			{
				ButtonSize = 94;
			}

#if PC
			pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go_Controller(ButtonSize));
#elif CAFE
			pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin_WiiU), ButtonString.Go(ButtonSize));
#else
			pressa = string.Format(Localization.WordString(Localization.Words.PressToJoin), ButtonString.Go(ButtonSize));
#endif

			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
			{
				Text = new EzText(pressa, Resources.Font_Grobold42, 1000, true, true, .5f);

#if CAFE
				Text.Pos = new Vector2(11.11133f, 63.88889f); Text.Scale = 0.8230838f;
#else
				Text.Pos = new Vector2(11.11133f, 63.88889f); Text.Scale = 0.9542501f;
#endif			
			}
			else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
			{
				Text = new EzText(pressa, Resources.Font_Grobold42, 1000, true, true, .5f);

#if CAFE
				Text.Pos = new Vector2(11.11133f, 63.88889f); Text.Scale = 0.8842503f;
#else
				Text.Pos = new Vector2(11.11133f, 63.88889f); Text.Scale = 0.9542501f;
#endif			
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

#if PC
			MouseClickText.Pos = Text.Pos;
			MouseClickText.Scale = Text.Scale;
#endif
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

#if PC
			// Mouse hover
			if (Tools.MouseInWindow && ButtonCheck.MouseInUse && !CharacterSelectManager.NonGamepadJoined)
			{
				// Force the quad to show so that we can HitTest against it
				Overlay.Show = true;

				if (Overlay.HitTest(Tools.MouseWorldPos()))
				{
					Overlay.Show = true;
					MouseClickText.Show = true;
					Text.Show = false;
				}
				else
				{
					Overlay.Show = false;
					MouseClickText.Show = false;
					Text.Show = true;
				}
			}
			else
			{
				Overlay.Show = false;
				MouseClickText.Show = false;
				Text.Show = true;
			}
#endif

#if PC
			if (Overlay.Show && Tools.MouseDown() && !Tools.PrevMouseDown())
			{
				CoreKeyboard.KeyboardPlayerIndex = (PlayerIndex)Control;
			}
#endif

			if (CharacterSelectManager.Active && (ButtonCheck.State(ControllerButtons.A, Control).Pressed))//|| Control == 3 || Control == 1 || Control == 0))
            {
#if PC
				if (!Tools.MouseDown() ||
					 ButtonCheck.GetState(ControllerButtons.A, Control, false, false, false).Pressed)
				{
					// gamepad or keyboard join, so continue
				}
				else
				{
					// non-gamepad join, make sure player is highlighted by mouse
					if (Overlay.Show)
					{
						// continue
					}
					else
					{
						// abort
						return;
					}
				}
#endif

#if XDK
                if (MyCharacterSelect.Player.MyGamer == null)
                {
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
                else
                {
                    if (EzStorage.Device[MyCharacterSelect.PlayerIndex] != null &&
                        !EzStorage.Device[MyCharacterSelect.PlayerIndex].NeedsConnection)
                    {
                        // Once the gamer presses a button they should be prompted to select a storage device.
                        EzStorage.Device[MyCharacterSelect.PlayerIndex].NeedsConnection = true;
                        return;
                    }
                    else
                    {
                        MyCharacterSelect.InitColorScheme(MyCharacterSelect.PlayerIndex);
                        Call(new SimpleMenu(Control, MyCharacterSelect));
                    }
                }
#else
                Call(new SimpleMenu(Control, MyCharacterSelect));
#endif

                Hide();
            }
        }
    }
}