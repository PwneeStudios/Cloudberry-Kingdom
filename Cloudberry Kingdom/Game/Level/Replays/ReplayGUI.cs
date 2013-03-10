using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public enum ReplayGUIType { Replay, Computer };
    public class ReplayGUI : CkBaseMenu
    {
        public override void ReturnToCaller()
        {
            InGameStartMenu.PreventMenu = false;
            base.ReturnToCaller();
        }

        bool SkipPhsxStep;

        EzText Play, Toggle, End, Speed, LB, RB;

        QuadClass BigPaused, BigEnd;

        public ReplayGUIType Type;

        public ReplayGUI()
        {
        }

        protected void SetGrayHeaderProperties(EzText text)
        {
            base.SetHeaderProperties(text);

            text.Shadow = false;

            text.Scale = FontScale;
            MyPile.Add(text);
        }

        protected override void SetHeaderProperties(EzText text)
        {
            text.Scale = FontScale;
            MyPile.Add(text);
        }

        public override void Init()
        {
            base.Init();

            SlideInFrom = SlideOutTo = PresetPos.Bottom;
            //SlideOutLength = 0;
        }

		public override void OnAdd()
		{
			base.OnAdd();

			InGameStartMenu.PreventMenu = true;

			FontScale = .5f;

			MyPile = new DrawPile();

			// Backrop
			QuadClass backdrop2 = new QuadClass("White", 1500);
			backdrop2.Quad.SetColor(ColorHelper.GrayColor(.1f));
			backdrop2.Alpha = .45f;
			MyPile.Add(backdrop2, "Backdrop2");

			QuadClass backdrop = new QuadClass("White", 1500);
			backdrop.Quad.SetColor(ColorHelper.GrayColor(.25f));
			backdrop.Alpha = .35f;
			MyPile.Add(backdrop, "Backdrop");

			Vector2 AdditionalAdd = Vector2.Zero;
#if PC_VERSION
            AdditionalAdd = new Vector2(-2, 0);
            MyPile.Add(new QuadClass(ButtonTexture.Go, 140, "Button_Go"));
            Play = new EzText(Localization.Words.Play, ItemFont, true);
            Play.Name = "Play";
            SetGrayHeaderProperties(Play);
#else
			MyPile.Add(new QuadClass(ButtonTexture.Go, 90, "Button_Go"));
			Play = new EzText(Localization.Words.Play, ItemFont, true);
			Play.MyFloatColor = new Color(67, 198, 48, 255).ToVector4();
			Play.Name = "Play";
			SetHeaderProperties(Play);
#endif

#if PC_VERSION
            AdditionalAdd = new Vector2(-2, 0);
            MyPile.Add(new QuadClass(ButtonTexture.Back, 140, "Button_Back"));
            End = new EzText(Localization.Words.Done, ItemFont, true);
            End.Name = "Back";
            SetGrayHeaderProperties(End);
#else
			MyPile.Add(new QuadClass(ButtonTexture.Back, 85, "Button_Back"));
			if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
				End = new EzText(Localization.Words.Back, ItemFont, true);
			else
				End = new EzText(Localization.Words.Done, ItemFont, true);
			End.MyFloatColor = new Color(239, 41, 41, 255).ToVector4();
			End.Name = "Back";
			SetHeaderProperties(End);
#endif

			if (Type == ReplayGUIType.Replay)
			{
				MyPile.Add(new QuadClass(ButtonTexture.X, 90, "Button_X"));
				Toggle = new EzText(Localization.Words.Single, ItemFont, true);
				Toggle.Name = "Toggle";
#if PC_VERSION
                SetGrayHeaderProperties(Toggle);
#else
				SetHeaderProperties(Toggle);
				//Toggle.MyFloatColor = new Color(0, 0, 255, 255).ToVector4();
				Toggle.MyFloatColor = Menu.DefaultMenuInfo.UnselectedXColor;
#endif
				SetToggleText();
			}

			MyPile.Add(new QuadClass(ButtonTexture.LeftRight, 85, "Button_LR"));
			Speed = new EzText(Localization.Words.Speed, ItemFont);
			Speed.Name = "Speed";
			SetGrayHeaderProperties(Speed);

			if (Type == ReplayGUIType.Computer)
			{
				MyPile.Add(new QuadClass(ButtonTexture.LeftBumper, 85, "Button_LB"));
				LB = new EzText(Localization.Words.Reset, ItemFont, true);
				LB.Name = "Reset";
				SetGrayHeaderProperties(LB);
			}
			else
			{
				MyPile.Add(new QuadClass(ButtonTexture.LeftBumper, 85, "Button_LB"));
				LB = new EzText(Localization.Words.Previous, ItemFont, true);
				LB.Name = "Prev";
				SetGrayHeaderProperties(LB);

				MyPile.Add(new QuadClass(ButtonTexture.RightBumper, 85, "Button_RB"));
				RB = new EzText(Localization.Words.Next, ItemFont, true);
				RB.Name = "Next";
				SetGrayHeaderProperties(RB);
			}
			SetSpeed();

			BigPaused = new QuadClass();
			BigPaused.SetToDefault();
			BigPaused.Quad.MyTexture = Tools.TextureWad.FindByName("Paused");
			BigPaused.ScaleYToMatchRatio(355);
			MyPile.Add(BigPaused);
			BigPaused.Pos = new Vector2(1210.557f, 791.1111f);

			BigEnd = new QuadClass();
			BigEnd.SetToDefault();
			BigEnd.Quad.MyTexture = Tools.TextureWad.FindByName("End");
			BigEnd.ScaleYToMatchRatio(255);
			//BigPaused.ScaleYToMatchRatio(300);
			MyPile.Add(BigEnd);
			BigEnd.Pos = new Vector2(1277.222f, 774.4444f);

			SetPlayText();







			// SetPos()
			if (Type == ReplayGUIType.Computer)
			{
				if (ButtonCheck.ControllerInUse)
				{
					////////////////////////////////////////////////////////////////////////////////
					/////// Console version, computer replay ////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////
					if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-779.512f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-239.4449f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(147.3335f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(946f, -840.5555f); _t.Scale = 0.3969166f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1038.89f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-508.3335f, -911.1113f); _q.Size = new Vector2(71.76664f, 71.76664f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(63.88873f, -911.1113f); _q.Size = new Vector2(83.48316f, 83.48316f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(499.9998f, -911.1113f); _q.Size = new Vector2(152.0833f, 152.0833f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-837.8453f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-258.8895f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(136.2224f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(932.1111f, -840.5555f); _t.Scale = 0.3969166f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1038.89f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-508.3335f, -911.1113f); _q.Size = new Vector2(71.76664f, 71.76664f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(63.88873f, -911.1113f); _q.Size = new Vector2(83.48316f, 83.48316f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(499.9998f, -911.1113f); _q.Size = new Vector2(152.0833f, 152.0833f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-693.4005f, -810.0001f); _t.Scale = 0.5253334f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-92.22247f, -810.0001f); _t.Scale = 0.5253334f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(264.0002f, -815.5556f); _t.Scale = 0.4885834f; }
						_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(971.0002f, -815.5556f); _t.Scale = 0.5030003f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-347.2227f, -911.1113f); _q.Size = new Vector2(71.76664f, 71.76664f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(186.1109f, -911.1113f); _q.Size = new Vector2(83.48316f, 83.48316f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(655.555f, -911.1113f); _q.Size = new Vector2(152.0833f, 152.0833f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-721.1783f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-120.0003f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(264.0002f, -832.2222f); _t.Scale = 0.44f; }
						_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(948.7776f, -837.7778f); _t.Scale = 0.4176667f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-347.2227f, -911.1113f); _q.Size = new Vector2(71.76664f, 71.76664f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(186.1109f, -911.1113f); _q.Size = new Vector2(83.48316f, 83.48316f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(655.555f, -911.1113f); _q.Size = new Vector2(152.0833f, 152.0833f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
				}
				else
				{
					////////////////////////////////////////////////////////////////////////////////
					/////// PC version, compuer replay /////////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////

					EzText _t;
					_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-662.845f, -832.2222f); _t.Scale = 0.44f; }
					_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-103.3335f, -835.0001f); _t.Scale = 0.44f; }
					_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(344.5559f, -832.2222f); _t.Scale = 0.44f; }
					_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(1051.556f, -840.5555f); _t.Scale = 0.44f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2058.333f); _q.Size = new Vector2(1230.664f, 1230.664f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2058.333f); _q.Size = new Vector2(1219.997f, 1219.997f); }
					_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-930.5562f, -911.1113f); _q.Size = new Vector2(130.9643f, 62.80939f); }
					_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-325.0003f, -911.1112f); _q.Size = new Vector2(73.5106f, 69.09996f); }
					_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(244.4444f, -913.8889f); _q.Size = new Vector2(73.20911f, 68.81656f); }
					_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(783.3331f, -913.8888f); _q.Size = new Vector2(76.22305f, 71.64967f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
			}
			else
			{
				if (ButtonCheck.ControllerInUse)
				{
					////////////////////////////////////////////////////////////////////////////////
					/////// Console version, player replay /////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////
					if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1069.445f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-563.889f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-77.77845f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(274.9997f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(830.5556f, -836.1113f); _t.Scale = 0.3769997f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1399.998f, -836.1113f); _t.Scale = 0.3787504f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1269.445f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-772.2219f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-313.8887f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(222.2214f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(575.0005f, -905.5554f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1091.667f, -902.7777f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1097.222f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-625.0001f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-216.6671f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(122.2222f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(772.2219f, -847.2224f); _t.Scale = 0.3271667f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1405.553f, -841.6669f); _t.Scale = 0.3494168f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1272.223f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-786.1104f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-405.5551f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(55.55546f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(469.4445f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1141.667f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1097.222f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-652.778f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-166.6671f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(197.2224f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(811.111f, -847.2224f); _t.Scale = 0.3271667f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1411.109f, -841.6669f); _t.Scale = 0.3494168f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1305.556f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-852.7772f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-416.666f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(133.3333f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(500f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1138.89f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1305.556f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-824.9996f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-261.1112f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(113.8889f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(766.667f, -847.2224f); _t.Scale = 0.304f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1469.442f, -841.6669f); _t.Scale = 0.3178333f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(27.77763f, -2327.78f); _q.Size = new Vector2(1691.408f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(27.77833f, -2330.556f); _q.Size = new Vector2(1670.826f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1538.889f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-1055.555f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-572.2219f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(63.88873f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(380.5562f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1086.111f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-563.8888f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-80.55579f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(261.1111f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(869.4445f, -847.2224f); _t.Scale = 0.3271667f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1430.553f, -841.6669f); _t.Scale = 0.3494168f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1275f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-791.6664f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-308.333f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(183.3333f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(569.4448f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1183.334f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1050f, -819.4447f); _t.Scale = 0.4576671f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-586.1115f, -816.6666f); _t.Scale = 0.4578336f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-133.3344f, -819.4445f); _t.Scale = 0.4533335f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(169.4441f, -830.5552f); _t.Scale = 0.3993336f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(850.0007f, -827.7777f); _t.Scale = 0.4095834f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1391.665f, -824.9999f); _t.Scale = 0.4487502f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1200f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-777.7773f, -905.5554f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-333.3335f, -908.3333f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(113.8885f, -902.7777f); _q.Size = new Vector2(72.98328f, 72.98328f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(552.7781f, -908.3331f); _q.Size = new Vector2(121.4832f, 121.4832f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1166.667f, -908.3333f); _q.Size = new Vector2(120.8164f, 120.8164f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-991.6671f, -827.778f); _t.Scale = 0.4328336f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-530.5558f, -824.9999f); _t.Scale = 0.4330001f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-52.77869f, -827.7778f); _t.Scale = 0.4285f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(333.333f, -838.8885f); _t.Scale = 0.3993336f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(886.1116f, -827.7777f); _t.Scale = 0.4390835f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1369.443f, -824.9999f); _t.Scale = 0.4487502f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1200f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-730.5552f, -905.5554f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-300f, -908.3333f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(261.1111f, -902.7777f); _q.Size = new Vector2(72.98328f, 72.98328f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(661.1114f, -908.3331f); _q.Size = new Vector2(121.4832f, 121.4832f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -908.3333f); _q.Size = new Vector2(120.8164f, 120.8164f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-991.6671f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-497.2223f, -830.5555f); _t.Scale = 0.4136664f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-0.000617981f, -833.3333f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(352.7771f, -836.1108f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(883.333f, -836.111f); _t.Scale = 0.3584168f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1422.221f, -838.8887f); _t.Scale = 0.354f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(144.4434f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(141.6667f, -2327.778f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1200f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-722.2222f, -911.111f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-230.5555f, -911.111f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(274.9993f, -899.9999f); _q.Size = new Vector2(79.48331f, 79.48331f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(641.6671f, -905.5554f); _q.Size = new Vector2(126.1499f, 126.1499f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1138.889f, -900f); _q.Size = new Vector2(128.233f, 128.233f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1307.777f, 779.9999f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1321.666f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(-55.55542f, 0f);
					}
					else
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-1013.889f, -827.778f); _t.Scale = 0.4145834f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-488.8888f, -827.778f); _t.Scale = 0.4147499f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(2.777214f, -827.778f); _t.Scale = 0.4139166f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(355.5554f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(900.0002f, -827.778f); _t.Scale = 0.4208333f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1380.554f, -827.778f); _t.Scale = 0.4238334f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1213.889f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-711.1108f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-233.333f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(294.4441f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(675.0005f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
						_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

						MyPile.Pos = new Vector2(0f, 0f);
					}
				}
				else
				{
					////////////////////////////////////////////////////////////////////////////////
					/////// PC version, player replay //////////////////////////////////////////////
					////////////////////////////////////////////////////////////////////////////////
					EzText _t;
					_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-991.6671f, -827.778f); _t.Scale = 0.4145834f; }
					_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-472.2223f, -838.8888f); _t.Scale = 0.4147499f; }
					_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(11.11071f, -836.1111f); _t.Scale = 0.4139166f; }
					_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(436.1108f, -836.1108f); _t.Scale = 0.3873335f; }
					_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(941.667f, -830.5555f); _t.Scale = 0.4208333f; }
					_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1361.11f, -833.3332f); _t.Scale = 0.4238334f; }

					QuadClass _q;
					_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
					_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
					_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1244.444f, -908.3333f); _q.Size = new Vector2(116.7162f, 55.97613f); }
					_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-688.8887f, -911.111f); _q.Size = new Vector2(66.06374f, 62.09991f); }
					_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-213.8887f, -908.3332f); _q.Size = new Vector2(64.18431f, 60.33325f); }
					_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(352.7776f, -905.5555f); _q.Size = new Vector2(69.04251f, 64.89996f); }
					_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(727.7779f, -902.7776f); _q.Size = new Vector2(63.90063f, 60.0666f); }
					_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -902.7778f); _q.Size = new Vector2(64.96447f, 61.0666f); }

					MyPile.Pos = new Vector2(0f, 0f);
				}
			}
		}

        public void StartUp()
        {
            SkipPhsxStep = true;
        }

        void ResetReplay(Level level)
        {
            if (Type == ReplayGUIType.Replay)
                level.SetReplay();

            level.SetToReset = true;
            level.ReplayPaused = false;
            level.FreeReset = true;
        }

        void SetToggleText()
        {
            if (MyGame.MyLevel.SingleOnly)
                Toggle.SubstituteText(Localization.Words.All);
            else
                Toggle.SubstituteText(Localization.Words.Single);
        }

        void SetPlayText()
        {
            if (StepControl)
                Play.SubstituteText(Localization.Words.Step);
            else
            {
                if (PauseSelected)
                    Play.SubstituteText(Localization.Words.Play);
                else
                    Play.SubstituteText(Localization.Words.Pause);
            }
        }

        void SetSpeed()
        {
            switch (SpeedVal)
            {
                case 0:
                    StepControl = true; // Start step control
                    Tools.PhsxSpeed = 1; Speed.SubstituteText("x 0");
                    break;
                case 1: Tools.PhsxSpeed = 0; Speed.SubstituteText("x .5"); break;
                case 2: Tools.PhsxSpeed = 1; Speed.SubstituteText("x 1"); break;
                case 3: Tools.PhsxSpeed = 2; Speed.SubstituteText("x 2"); break;
                case 4: Tools.PhsxSpeed = 3; Speed.SubstituteText("x 4"); break;
            }

            // Ensure the game is unpaused if we aren't step controlling and we aren't soliciting a pause
            // We set this to false elsewhere in this case, but not soon enough for the game to realize
            // we aren't paused and set the PhsxSpeed to 1
            if (!PauseSelected && !StepControl) PauseGame = false;
        }

        bool StepControl = false;

        int SpeedVal = 2;
        int Delay = 10;
        int PrevDir = 0;
        public void ProcessInput()
        {
            Level level = MyGame.MyLevel;

            if (SkipPhsxStep) { SkipPhsxStep = false; return; }

            if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
            {
                if (level.ReplayPaused && ReplayIsOver())
                {
                    PauseSelected = false;
                    SetPlayText();

                    level.ReplayPaused = false;

                    // Reset
                    ResetReplay(level);
                    PauseGame = false;
                }
            }

            if (Type == ReplayGUIType.Computer)
            {
                // Check for reset
                if (ButtonCheck.State(ControllerButtons.LS, -1).Pressed)
                {
                    ResetReplay(level);
                    PauseGame = false;
                }
            }
            else
            {
                // Check for switching
                int SwarmIndex = level.MySwarmBundle.SwarmIndex;
                if (ButtonCheck.State(ControllerButtons.LS, -1).Pressed)
                {
                    if (StepControl && level.CurPhsxStep < 36 ||
                        Tools.DrawCount - TimeStamp < 36)
                        level.MySwarmBundle.SetSwarm(level, SwarmIndex - 1);
                    ResetReplay(level);
                    PauseGame = false;
                }
                if (SwarmIndex < level.MySwarmBundle.NumSwarms - 1 && ButtonCheck.State(ControllerButtons.RS, -1).Pressed)
                {
                    level.MySwarmBundle.SetSwarm(level, SwarmIndex + 1);
                    ResetReplay(level);
                    PauseGame = false;
                }
            }

            float Dir = ButtonCheck.GetDir(-1).X;// ButtonCheck.State(ControllerButtons.LJ, -1).Dir.X;
            if (PrevDir != Math.Sign(Dir)) Delay = 0;
            if (Delay == 0)
            {
                bool Change = false;
                if (Dir < -ButtonCheck.ThresholdSensitivity)
                {
                    SpeedVal--;
                    Change = true;
                }
                else if (Dir > ButtonCheck.ThresholdSensitivity)
                {
                    SpeedVal++;
                    Change = true;
                }

                if (Change)
                {
                    SpeedVal = CoreMath.Restrict(0, 4, SpeedVal);

                    //if (SpeedVal == 1) Delay = 30;
                    //else if (SpeedVal == 2) Delay = 30;
                    //else Delay = 15;
                    Delay = 1000;

                    StepControl = false;

                    SetSpeed();

                    SetPlayText();

                    //if (!StepControl) PauseGame = false;
                }
            }
            else
            {
                Delay--;
            }
            PrevDir = Math.Sign(Dir);

            // Switch between swarm and single view (Toggle)
            if (Type == ReplayGUIType.Replay)
            {
                if (ButtonCheck.State(ControllerButtons.X, -1).Pressed)
                {
                    level.SingleOnly = !level.SingleOnly;
                    level.MainReplayOnly = level.SingleOnly;
                    SetToggleText();
                }
            }

            // End the replay
            bool EndReplay = false;
#if PC_VERSION
            if (Tools.Keyboard.IsKeyDownCustom(Microsoft.Xna.Framework.Input.Keys.Escape))
                EndReplay = true;
#endif

            if (ButtonCheck.State(ControllerButtons.B, -1).Pressed)
                EndReplay = true;

            if (EndReplay)
            {
                PauseGame = false;
                Tools.PhsxSpeed = 1;

                ReturnToCaller();

                level.FreeReset = true;
                if (Type == ReplayGUIType.Replay)
                {                    
                    level.EndReplay();
                }
                else
                    level.EndComputerWatch();
            }
        }

        public override void Reset(bool BoxesOnly)
        {
            base.Reset(BoxesOnly);

            TimeStamp = Tools.DrawCount;
        }

        bool PauseSelected = false;

        int TimeStamp = 0;
        protected override void MyPhsxStep()
        {
            Level level = MyGame.MyLevel;

            if (level.SuppressReplayButtons)
                return;

            base.MyPhsxStep();

            if (!Active)
                return;

            InGameStartMenu.PreventMenu = true;

            if (StepControl && !PauseSelected)
            {
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed ||
                    ButtonStats.All.DownCount(ControllerButtons.A) > 30 && Tools.DrawCount % 2 == 0)
                    PauseGame = false;
                else
                    PauseGame = true;
            }
            else
            {
                if (ButtonCheck.State(ControllerButtons.A, -1).Pressed)
                {
                    PauseSelected = !PauseSelected;
                    SetPlayText();

                    SetSpeed();
                }

                PauseGame = PauseSelected;
            }

            ProcessInput();
        }

        protected override void MyDraw()
        {
            if (MyGame == null) { Release(); return; }

            Level level = MyGame.MyLevel;

            if (level.SuppressReplayButtons)
                return;

            base.MyDraw();
           
            BigEnd.Show = BigPaused.Show = false;
            if (level.ReplayPaused)
            {
                if (ReplayIsOver())
                    BigEnd.Show = true;
            }
            else
            {
				if (PauseSelected)
				{
					BigPaused.ScaleYToMatchRatio(300);
					BigPaused.Show = true;
				}
            }
        }

        bool ReplayIsOver()
        {
            Level level = MyGame.MyLevel;

            if (Type == ReplayGUIType.Computer && level.EndOfReplay())
                return true;

            if (Type == ReplayGUIType.Replay)
                if (level.MySwarmBundle.EndCheck(level) && !level.MySwarmBundle.GetNextSwarm(level))
                    return true;

            return false;
        }
    }
}