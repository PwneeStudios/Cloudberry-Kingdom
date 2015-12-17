using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class BungeeSwitch : CkBaseMenu
    {
        EzText Text_On, Text_Off, Text_Unavailable;

        public BungeeSwitch(int Control)
            : base(false)
        {
            AutoDraw = false;

            this.Tags += Tag.CharSelect;
            this.Control = Control;

            Constructor();
        }

        protected override void ReleaseBody()
        {
            base.ReleaseBody();
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
            string OnString, OffString, UnavailableString;

            // Position
            MyPile.FancyPos.RelVal = new Vector2(861.1111f, 891.6669f);

			// Text
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

            OnString  = string.Format("{0}: {1} {2}", Localization.WordString(Localization.Words.Bungee), Localization.WordString(Localization.Words.On),  ButtonString.RightBumper(ButtonSize));
            OffString = string.Format("{0}: {1} {2}", Localization.WordString(Localization.Words.Bungee), Localization.WordString(Localization.Words.Off), ButtonString.RightBumper(ButtonSize));
            UnavailableString = string.Format("{0}: {1} {2}\n{3}", Localization.WordString(Localization.Words.Bungee), Localization.WordString(Localization.Words.Off), EzText.ColorToMarkup(new Color(.8f, .4f, .4f, 1f)), "(Multiplayer required)");

            Text_On  = MakeText(OnString);
            Text_Off = MakeText(OffString);
            Text_Unavailable = MakeText(UnavailableString);

            Text_Unavailable.MyFloatColor = ColorHelper.Gray(.7f);
		}

        EzText Text = null;
        void SetText()
        {
            Text_On.Show = Text_Off.Show = Text_Unavailable.Show = false;

            if (PlayerManager.GetNumPlayers() <= 1)
            {
                Text = Text_Unavailable;
            }
            else
            {
                if (CloudberryKingdomGame.BungeeSwitch)
                {
                    Text = Text_On;
                }
                else
                {
                    Text = Text_Off;
                }
            }

            Text.Show = true;
        }

        private EzText MakeText(string OnText)
        {
            var Text = new EzText(OnText, Resources.Font_Grobold42, 1000, false, true, .666f);
            Text.Scale = .35f;

            Text.ShadowOffset = new Vector2(7.5f, 7.5f);
            Text.ShadowColor = new Color(30, 30, 30);
            Text.ColorizePics = true;

            MyPile.Add(Text);

            return Text;
        }

        protected override void MyDraw()
        {
            if (CharacterSelectManager.FakeHide)
                return;

            SetText();

            base.MyDraw();
        }

		public override void OnReturnTo()
		{
			base.OnReturnTo();
		}

        void SwitchMode()
        {
            CloudberryKingdomGame.BungeeSwitch = !CloudberryKingdomGame.BungeeSwitch;
        }

        protected override void MyPhsxStep()
        {
            base.MyPhsxStep();

            if (!Active) return;

            SetText();

            if (PlayerManager.GetNumPlayers() > 1)
            {
#if PC
                Text.MyFloatColor = ColorHelper.GrayColor(.8f).ToVector4();

                // Mouse hover
                if (Tools.MouseInWindow && ButtonCheck.MouseInUse)
                {
                    if (Text.HitTest(Tools.MouseWorldPos()))
                    {
                        Text.MyFloatColor = ColorHelper.GrayColor(1f).ToVector4();

                        if (Tools.MousePressed())
                        {
                            SwitchMode();
                        }
                    }
                }
#endif
                // Controll Button
                if (ButtonCheck.State(ControllerButtons.RS, -1).Pressed)
                    SwitchMode();
            }
        }
    }
}