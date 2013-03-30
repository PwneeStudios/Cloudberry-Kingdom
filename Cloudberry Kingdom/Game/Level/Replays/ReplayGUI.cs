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




			bool WiiRemote = false;
			if (WiiRemote)
			{
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
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-315.6231f, -832.2222f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(224.444f, -832.2222f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(611.2224f, -832.2222f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(909.8889f, -1373.889f); _t.Scale = 0.3969166f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(69.44431f, -2049.777f); _q.Size = new Vector2(825.0781f, 1240.663f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(66.66653f, -2049.777f); _q.Size = new Vector2(814.4113f, 1229.996f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-575.0011f, -911.1113f); _q.Size = new Vector2(69.64276f, 69.64276f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-44.44458f, -911.1113f); _q.Size = new Vector2(71.76664f, 71.76664f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(527.7776f, -911.1113f); _q.Size = new Vector2(83.48316f, 83.48316f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(463.8887f, -1444.445f); _q.Size = new Vector2(152.0833f, 152.0833f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-329.512f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(249.4438f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(644.5557f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(982.1111f, -1160f); _t.Scale = 0.3969166f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(77.77765f, -2047f); _q.Size = new Vector2(852.9119f, 1248.496f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(74.99988f, -2047f); _q.Size = new Vector2(842.2455f, 1237.83f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-530.5568f, -905.5558f); _q.Size = new Vector2(69.64276f, 69.64276f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-0.0002441406f, -905.5558f); _q.Size = new Vector2(71.76664f, 71.76664f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(572.222f, -905.5558f); _q.Size = new Vector2(83.48316f, 83.48316f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(549.9998f, -1230.556f); _q.Size = new Vector2(152.0833f, 152.0833f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(300f, 105f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-368.4008f, -810.0001f); _t.Scale = 0.5329166f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(232.7773f, -810.0001f); _t.Scale = 0.5329166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(589f, -815.5556f); _t.Scale = 0.4961667f; }
							_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(971.0002f, -1093.334f); _t.Scale = 0.5030003f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2055.333f); _q.Size = new Vector2(948.9962f, 1244.163f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2055.333f); _q.Size = new Vector2(938.3292f, 1233.496f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-605.5565f, -911.1113f); _q.Size = new Vector2(76.39275f, 70.4761f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-22.22293f, -911.1113f); _q.Size = new Vector2(78.51664f, 72.59997f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(511.1107f, -911.1113f); _q.Size = new Vector2(90.23312f, 84.3165f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(655.555f, -1188.89f); _q.Size = new Vector2(152.0833f, 152.0833f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-365.6229f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(235.5551f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(619.5557f, -826.6667f); _t.Scale = 0.44f; }
							_t = MyPile.FindEzText("Reset"); if (_t != null) { _t.Pos = new Vector2(940.4446f, -1221.111f); _t.Scale = 0.4176667f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(55.55542f, -2055.333f); _q.Size = new Vector2(900.329f, 1244.413f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(52.77765f, -2055.333f); _q.Size = new Vector2(889.662f, 1233.746f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-575.0008f, -905.5558f); _q.Size = new Vector2(69.64276f, 69.64276f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(8.332733f, -905.5558f); _q.Size = new Vector2(71.76664f, 71.76664f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(541.6663f, -905.5558f); _q.Size = new Vector2(83.48316f, 83.48316f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(647.222f, -1294.444f); _q.Size = new Vector2(152.0833f, 152.0833f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
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
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-480.556f, -827.778f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(24.99994f, -827.778f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(511.1105f, -827.778f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(863.8886f, -827.778f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(905.5554f, -1263.889f); _t.Scale = 0.3769997f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1474.998f, -1263.889f); _t.Scale = 0.3787504f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(136.1106f, -2324.78f); _q.Size = new Vector2(1046.165f, 1524.581f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(136.1113f, -2327.556f); _q.Size = new Vector2(1028.333f, 1506.749f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-680.556f, -905.5554f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-183.333f, -905.5554f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(275.0002f, -905.5554f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(811.1104f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(650.0002f, -1333.333f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1166.667f, -1330.555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-419.444f, -833.3336f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(52.77795f, -833.3336f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(461.111f, -833.3336f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(800.0002f, -833.3336f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(705.5554f, -1708.333f); _t.Scale = 0.3271667f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1338.886f, -1702.778f); _t.Scale = 0.3494168f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(111.1106f, -2324.78f); _q.Size = new Vector2(918.5801f, 1525.996f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(111.1113f, -2327.556f); _q.Size = new Vector2(900.7482f, 1508.164f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-594.4449f, -911.111f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-108.3323f, -911.111f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(272.223f, -911.111f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(733.3336f, -908.3332f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(402.778f, -1769.444f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1075f, -1766.667f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-466.6666f, -833.3336f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-22.2226f, -833.3336f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(463.8883f, -833.3336f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(827.7778f, -833.3336f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(805.5554f, -1200f); _t.Scale = 0.3271667f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1405.553f, -1194.445f); _t.Scale = 0.3494168f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(105.555f, -2319.225f); _q.Size = new Vector2(978.4958f, 1519.663f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(105.5557f, -2322f); _q.Size = new Vector2(960.6643f, 1501.831f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-675.0006f, -911.111f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-222.2218f, -911.111f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(213.8894f, -911.111f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(763.8887f, -908.3333f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(494.4443f, -1261.111f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1133.334f, -1258.333f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-569.4445f, -830.5557f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-88.888f, -830.5557f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(475.0004f, -830.5557f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(850.0005f, -830.5557f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(791.667f, -1733.333f); _t.Scale = 0.304f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1494.442f, -1727.778f); _t.Scale = 0.3178333f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(63.88884f, -2322.002f); _q.Size = new Vector2(1039.073f, 1525.914f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(63.88953f, -2324.778f); _q.Size = new Vector2(1018.492f, 1508.082f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-802.7775f, -908.3332f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-319.4435f, -908.3332f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(163.8896f, -908.3332f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(800.0003f, -905.5555f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(405.5562f, -1794.444f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1180.556f, -1791.667f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-524.9999f, -830.5557f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-2.77771f, -830.5557f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(480.5553f, -830.5557f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(822.2222f, -830.5557f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(486.1113f, -1700f); _t.Scale = 0.3271667f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1047.22f, -1694.444f); _t.Scale = 0.3494168f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2324.78f); _q.Size = new Vector2(1044.163f, 1530.164f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2327.556f); _q.Size = new Vector2(1026.332f, 1512.331f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-713.8889f, -908.3332f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-230.5553f, -908.3332f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(252.7781f, -908.3332f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(744.4444f, -905.5555f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(227.7781f, -1800f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(800.0007f, -1758.333f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-408.3335f, -819.4447f); _t.Scale = 0.4576671f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(55.55499f, -816.6666f); _t.Scale = 0.4578336f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(508.3321f, -819.4445f); _t.Scale = 0.4533335f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(811.1106f, -830.5552f); _t.Scale = 0.3993336f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(680.5558f, -1561.111f); _t.Scale = 0.4095834f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1222.22f, -1558.333f); _t.Scale = 0.4487502f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(161.1106f, -2316.447f); _q.Size = new Vector2(942.1636f, 1519.498f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(161.1113f, -2319.223f); _q.Size = new Vector2(924.3314f, 1501.666f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-558.3335f, -905.5554f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-136.1108f, -905.5554f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(308.333f, -908.3333f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(755.555f, -902.7777f); _q.Size = new Vector2(72.98328f, 72.98328f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(383.3333f, -1641.666f); _q.Size = new Vector2(121.4832f, 121.4832f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(997.2222f, -1641.667f); _q.Size = new Vector2(120.8164f, 120.8164f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-400.0003f, -813.8892f); _t.Scale = 0.4778334f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(72.22229f, -816.667f); _t.Scale = 0.4779999f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(536.1105f, -816.667f); _t.Scale = 0.4771666f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(869.4443f, -830.5558f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(922.2226f, -2033.333f); _t.Scale = 0.4539167f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1397.221f, -2036.111f); _t.Scale = 0.4569168f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(152.7773f, -2316.447f); _q.Size = new Vector2(970.1627f, 1518.83f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(152.778f, -2319.223f); _q.Size = new Vector2(952.3304f, 1500.998f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-636.1112f, -908.3333f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-133.3329f, -908.3333f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(344.4448f, -908.3333f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(808.333f, -905.5555f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(700.0005f, -2125f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1180.556f, -2122.222f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-505.5563f, -830.5558f); _t.Scale = 0.4328336f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-44.44495f, -827.7776f); _t.Scale = 0.4330001f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(433.3322f, -830.5555f); _t.Scale = 0.4285f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(819.4438f, -841.6663f); _t.Scale = 0.3993336f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(930.5559f, -1116.667f); _t.Scale = 0.4390835f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1413.887f, -1113.889f); _t.Scale = 0.4487502f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(83.3328f, -2322.002f); _q.Size = new Vector2(989.4137f, 1529.081f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(83.3335f, -2324.778f); _q.Size = new Vector2(971.5814f, 1511.249f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-713.8892f, -908.3333f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-244.4443f, -908.3332f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(186.1108f, -911.1111f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(747.2219f, -905.5555f); _q.Size = new Vector2(72.98328f, 72.98328f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(705.5557f, -1197.222f); _q.Size = new Vector2(121.4832f, 121.4832f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1200f, -1197.222f); _q.Size = new Vector2(120.8164f, 120.8164f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(0f, 8.400001f);
						}
						else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-397.2228f, -825.0002f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(97.22205f, -827.7777f); _t.Scale = 0.4136664f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(594.4437f, -830.5555f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(947.2214f, -833.333f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(922.2219f, -1444.444f); _t.Scale = 0.3584168f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1461.11f, -1447.222f); _t.Scale = 0.354f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(199.9988f, -2324.78f); _q.Size = new Vector2(1023.164f, 1529.664f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(197.2221f, -2324.778f); _q.Size = new Vector2(1005.333f, 1511.832f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-605.5557f, -902.7777f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-127.7779f, -908.3333f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(363.8889f, -908.3333f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(869.4436f, -897.2221f); _q.Size = new Vector2(79.48331f, 79.48331f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(680.556f, -1513.889f); _q.Size = new Vector2(126.1499f, 126.1499f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1177.778f, -1508.333f); _q.Size = new Vector2(128.233f, 128.233f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1321.666f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(-55.55542f, 8.400001f);
						}
						else
						{
							EzText _t;
							_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-647.2225f, -825.0002f); _t.Scale = 0.4145834f; }
							_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-122.2223f, -825.0002f); _t.Scale = 0.4147499f; }
							_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(369.4437f, -825.0002f); _t.Scale = 0.4139166f; }
							_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(722.2219f, -825.0002f); _t.Scale = 0.3873335f; }
							_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(1316.667f, -1025f); _t.Scale = 0.4208333f; }
							_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1797.22f, -1025f); _t.Scale = 0.4238334f; }

							QuadClass _q;
							_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(-11.11169f, -2283.113f); _q.Size = new Vector2(1037.991f, 1503.489f); }
							_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-11.11096f, -2285.889f); _q.Size = new Vector2(1020.161f, 1485.661f); }
							_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-847.2225f, -902.7777f); _q.Size = new Vector2(64.55943f, 64.55943f); }
							_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-344.4443f, -902.7777f); _q.Size = new Vector2(67.34993f, 67.34993f); }
							_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(133.3335f, -902.7777f); _q.Size = new Vector2(65.58324f, 65.58324f); }
							_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(661.1106f, -899.9999f); _q.Size = new Vector2(79.56668f, 79.56668f); }
							_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(1091.667f, -1105.555f); _q.Size = new Vector2(116.8998f, 116.8998f); }
							_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1572.222f, -1102.778f); _q.Size = new Vector2(113.7331f, 113.7331f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(367.9286f, 128.775f); }
							_q = MyPile.FindQuad(""); if (_q != null) { _q.Pos = new Vector2(1277.222f, 774.4444f); _q.Size = new Vector2(255f, 128.775f); }

							MyPile.Pos = new Vector2(11.11108f, 5.622231f);
						}
					}
				}

			}
			else

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
					else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
					{
						EzText _t;
						_t = MyPile.FindEzText("Play"); if (_t != null) { _t.Pos = new Vector2(-986.1114f, -811.1114f); _t.Scale = 0.4778334f; }
						_t = MyPile.FindEzText("Back"); if (_t != null) { _t.Pos = new Vector2(-513.8888f, -813.8892f); _t.Scale = 0.4779999f; }
						_t = MyPile.FindEzText("Toggle"); if (_t != null) { _t.Pos = new Vector2(-41.66737f, -813.8892f); _t.Scale = 0.4771666f; }
						_t = MyPile.FindEzText("Speed"); if (_t != null) { _t.Pos = new Vector2(291.6665f, -827.778f); _t.Scale = 0.3873335f; }
						_t = MyPile.FindEzText("Prev"); if (_t != null) { _t.Pos = new Vector2(897.2226f, -816.6669f); _t.Scale = 0.4539167f; }
						_t = MyPile.FindEzText("Next"); if (_t != null) { _t.Pos = new Vector2(1372.221f, -819.4447f); _t.Scale = 0.4569168f; }

						QuadClass _q;
						_q = MyPile.FindQuad("Backdrop2"); if (_q != null) { _q.Pos = new Vector2(108.3328f, -2327.78f); _q.Size = new Vector2(1517.832f, 1517.832f); }
						_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(108.3335f, -2330.556f); _q.Size = new Vector2(1500f, 1500f); }
						_q = MyPile.FindQuad("Button_Go"); if (_q != null) { _q.Pos = new Vector2(-1213.889f, -905.5555f); _q.Size = new Vector2(64.55943f, 64.55943f); }
						_q = MyPile.FindQuad("Button_Back"); if (_q != null) { _q.Pos = new Vector2(-711.1108f, -905.5555f); _q.Size = new Vector2(67.34993f, 67.34993f); }
						_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(-233.333f, -905.5555f); _q.Size = new Vector2(65.58324f, 65.58324f); }
						_q = MyPile.FindQuad("Button_LR"); if (_q != null) { _q.Pos = new Vector2(230.5552f, -902.7777f); _q.Size = new Vector2(79.56668f, 79.56668f); }
						_q = MyPile.FindQuad("Button_LB"); if (_q != null) { _q.Pos = new Vector2(675.0005f, -908.3332f); _q.Size = new Vector2(116.8998f, 116.8998f); }
						_q = MyPile.FindQuad("Button_RB"); if (_q != null) { _q.Pos = new Vector2(1155.556f, -905.5555f); _q.Size = new Vector2(113.7331f, 113.7331f); }
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


			// Extra push up
			float MoveUp = 28 * CloudberryKingdomGame.GuiSqueeze;
            Vector2 MoveUp_Back = new Vector2(0, 10) * CloudberryKingdomGame.GuiSqueeze;

            if (Type == ReplayGUIType.Computer)
            {
                MoveUp += 16.5f;
            }

			MyPile.Pos = new Vector2(MyPile.Pos.X, MyPile.Pos.Y + MoveUp + 16.5f);
			QuadClass __q;
			__q = MyPile.FindQuad("Backdrop2"); if (__q != null) { __q.Pos += MoveUp_Back; }
			__q = MyPile.FindQuad("Backdrop"); if (__q != null) { __q.Pos += MoveUp_Back; }
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