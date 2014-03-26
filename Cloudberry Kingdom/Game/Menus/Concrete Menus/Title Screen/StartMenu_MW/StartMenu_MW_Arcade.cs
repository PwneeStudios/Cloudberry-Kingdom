using Microsoft.Xna.Framework;
using CloudberryKingdom.Stats;

namespace CloudberryKingdom
{
    public class StartMenu_MW_Arcade : ArcadeMenu
    {
        public TitleGameData_MW Title;
        public StartMenu_MW_Arcade(TitleGameData_MW Title)
            : base()
        {
            this.Title = Title;
        }

        public override void SlideIn(int Frames)
        {
            PlayerManager.UploadPlayerLevels();

            Title.BackPanel.SetState(TitleBackgroundState.Scene_Arcade);
            base.SlideIn(0);
        }

        public override void SlideOut(PresetPos Preset, int Frames)
        {
            base.SlideOut(Preset, 0);
        }

        protected override void SetItemProperties(MenuItem item)
        {
            base.SetItemProperties(item);

            item.MySelectedText.Shadow = item.MyText.Shadow = false;

            StartMenu.SetItemProperties_Green(item, false);

            item.MyText.Scale = item.MySelectedText.Scale = 1;
        }

        protected override void Go(MenuItem item)
        {
            MyArcadeItem = item as ArcadeItem;
            if (MyArcadeItem.IsLocked()) return;

            if (MyArcadeItem.MyChallenge == Challenge_Freeplay.Instance)
            {
                SkipCallSound = true;
                Call(new StartMenu_MW_CustomLevel(Title));
            }
            else
            {
                if (MyArcadeItem.MyChallenge == Challenge_Escalation.Instance ||
                    MyArcadeItem.MyChallenge == Challenge_TimeCrisis.Instance)
                {
                    Call(new StartMenu_MW_HeroSelect(Title, this, MyArcadeItem));
                }
                else
                {
                    Challenge.ChosenHero = null;

                    int TopLevelForHero = MyArcadeItem.MyChallenge.CalcTopGameLevel(null);

                    StartLevelMenu levelmenu = new StartLevelMenu(TopLevelForHero);

                    levelmenu.MyMenu.SelectItem(StartLevelMenu.PreviousMenuIndex);
                    levelmenu.StartFunc = StartFunc;
                    levelmenu.ReturnFunc = null;

                    Call(levelmenu);
                }
            }

            Hide();
        }

        public override void OnAdd()
        {
            CloudberryKingdomGame.SetPresence(CloudberryKingdomGame.Presence.Arcade);

            base.OnAdd();
        }

		ClickableBack Back;

		protected override void MyPhsxStep()
		{
			base.MyPhsxStep();

#if PC_VERSION
			if (!Active) return;

			// Update the back button and the scroll bar
			if (Back.UpdateBack(MyCameraZoom))
			{
				MenuReturnToCaller(MyMenu);
				return;
			}
#endif
		}

        public override void Init()
        {
 	        base.Init();

            CallDelay = ReturnToCallerDelay = 0;
            MyMenu.OnB = MenuReturnToCaller;

            SetPos();

#if PC_VERSION
			// Back button
			Back = new ClickableBack(MyPile, true, true);
#endif
        }

        protected override void SetPos()
        {
if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Portuguese)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2667.914f, 913.8129f); _item.MyText.Scale = 1.286351f; _item.MySelectedText.Scale = 0.3363512f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2550.636f, 487.3019f); _item.MyText.Scale = 0.8143333f; _item.MySelectedText.Scale = 0.8143333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2550.636f, 298.0817f); _item.MyText.Scale = 0.8143333f; _item.MySelectedText.Scale = 0.8143333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2550.636f, 108.8615f); _item.MyText.Scale = 0.8143333f; _item.MySelectedText.Scale = 0.8143333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2550.636f, -80.3587f); _item.MyText.Scale = 0.8143333f; _item.MySelectedText.Scale = 0.8143333f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-803.4119f, -759.5778f); _item.MyText.Scale = 0.6938335f; _item.MySelectedText.Scale = 0.6938335f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1054.222f, -37.22229f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(185.6885f, -583.3334f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1024.578f, -516.667f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(412.7824f, 694.2017f); _t.Scale = 0.6497964f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(419.8857f, 501.3502f); _t.Scale = 0.6259654f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(922.2821f, 467.227f); _q.Size = new Vector2(281.8031f, 680.5267f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(63.88892f, -922.2224f); _q.Size = new Vector2(87.85452f, 82.58324f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Italian)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.696417f; _item.MySelectedText.Scale = 0.7464166f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.7464166f; _item.MySelectedText.Scale = 0.7464166f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 139.7484f); _item.MyText.Scale = 0.7464166f; _item.MySelectedText.Scale = 0.7464166f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, -43.91629f); _item.MyText.Scale = 0.7464166f; _item.MySelectedText.Scale = 0.7464166f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, -227.5809f); _item.MyText.Scale = 0.7464166f; _item.MySelectedText.Scale = 0.7464166f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-358.9673f, -454.0223f); _item.MyText.Scale = 0.7964166f; _item.MySelectedText.Scale = 0.7964166f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-260.9079f, -749.9996f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1018.207f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(-38.02747f, 579.4897f); _t.Scale = 0.6407465f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(-35.38781f, 393.7869f); _t.Scale = 0.5899093f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(723.8219f, 367.997f); _q.Size = new Vector2(248.9382f, 824.8477f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(522.2223f, -627.7778f); _q.Size = new Vector2(100f, 94f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.French)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.767751f; _item.MySelectedText.Scale = 0.81775f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2456.191f, 348.413f); _item.MyText.Scale = 0.81775f; _item.MySelectedText.Scale = 0.81775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2456.191f, 159.1928f); _item.MyText.Scale = 0.81775f; _item.MySelectedText.Scale = 0.81775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2456.191f, -30.02734f); _item.MyText.Scale = 0.81775f; _item.MySelectedText.Scale = 0.81775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2456.191f, -219.2475f); _item.MyText.Scale = 0.81775f; _item.MySelectedText.Scale = 0.81775f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-486.7454f, -490.1335f); _item.MyText.Scale = 0.7980832f; _item.MySelectedText.Scale = 0.7980832f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1073.667f, -20.55561f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-292.5956f, -745.6355f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1018.207f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(432.6285f, 583.0642f); _t.Scale = 0.7405323f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(42.81172f, 378.305f); _t.Scale = 0.606437f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(747.6375f, 356.0894f); _q.Size = new Vector2(274.7774f, 756.8558f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(394.4445f, -638.8889f); _q.Size = new Vector2(100f, 94f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Spanish)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.717833f; _item.MySelectedText.Scale = 0.7678337f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 323.413f); _item.MyText.Scale = 0.7678337f; _item.MySelectedText.Scale = 0.7678337f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, 148.0817f); _item.MyText.Scale = 0.7678337f; _item.MySelectedText.Scale = 0.7678337f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, -27.2496f); _item.MyText.Scale = 0.7678337f; _item.MySelectedText.Scale = 0.7678337f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2458.969f, -202.5809f); _item.MyText.Scale = 0.7678337f; _item.MySelectedText.Scale = 0.7678337f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-478.4119f, -473.4667f); _item.MyText.Scale = 0.8178337f; _item.MySelectedText.Scale = 0.8178337f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-316.4103f, -737.6971f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1018.207f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(182.569f, 622.7562f); _t.Scale = 0.8550834f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(7.088623f, 354.4898f); _t.Scale = 0.6387068f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(771.4524f, 340.2126f); _q.Size = new Vector2(305.541f, 787.8156f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(425.0001f, -644.4445f); _q.Size = new Vector2(100f, 94f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.German)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2687.76f, 874.1208f); _item.MyText.Scale = 1.426941f; _item.MySelectedText.Scale = 0.4769393f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2492.302f, 417.8573f); _item.MyText.Scale = 0.7103342f; _item.MySelectedText.Scale = 0.7103342f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2492.302f, 236.9706f); _item.MyText.Scale = 0.7103342f; _item.MySelectedText.Scale = 0.7103342f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2492.302f, 56.08383f); _item.MyText.Scale = 0.7103342f; _item.MySelectedText.Scale = 0.7103342f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2492.302f, -124.8029f); _item.MyText.Scale = 0.7103342f; _item.MySelectedText.Scale = 0.7103342f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-236.7455f, -484.5779f); _item.MyText.Scale = 0.6162505f; _item.MySelectedText.Scale = 0.6162505f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1076.445f, -48.33337f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(54.26504f, -741.6663f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1027.685f, -685.7161f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(269.8915f, 630.6946f); _t.Scale = 0.6558692f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(280.9633f, 437.843f); _t.Scale = 0.5914334f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(823.0519f, 407.6891f); _q.Size = new Vector2(266.6555f, 639.9223f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(663.8886f, -652.7777f); _q.Size = new Vector2(69.50346f, 65.33325f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Russian)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.527843f; _item.MySelectedText.Scale = 0.5778447f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2672.858f, 406.7463f); _item.MyText.Scale = 0.7799169f; _item.MySelectedText.Scale = 0.7799169f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2672.858f, 211.9706f); _item.MyText.Scale = 0.7799169f; _item.MySelectedText.Scale = 0.7799169f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2672.858f, 17.19479f); _item.MyText.Scale = 0.7799169f; _item.MySelectedText.Scale = 0.7799169f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2672.858f, -177.581f); _item.MyText.Scale = 0.7799169f; _item.MySelectedText.Scale = 0.7799169f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-656.1899f, -526.2446f); _item.MyText.Scale = 0.7180001f; _item.MySelectedText.Scale = 0.7180001f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1070.889f, -45.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(-90.12628f, -794.4438f); _t.Scale = 0.6805003f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1020.985f, -730.5554f); _t.Scale = 0.9349998f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(82.20013f, 576.9747f); _t.Scale = 0.6619421f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(89.25061f, 359.518f); _t.Scale = 0.5714267f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(719.4446f, 338.8889f); _q.Size = new Vector2(290.9718f, 690.6484f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(241.6666f, -699.9999f); _q.Size = new Vector2(90.51411f, 85.08327f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Korean)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.696094f; _item.MySelectedText.Scale = 0.7460943f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 348.413f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 156.4151f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -35.58279f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -227.5807f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-556.1907f, -522.3561f); _item.MyText.Scale = 0.7837502f; _item.MySelectedText.Scale = 0.7837502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1045.889f, -20.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(76.54013f, -741.6663f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1018.207f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(496.0892f, 110.308f); _t.Scale = 0.6619421f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(280.9174f, -90.48193f); _t.Scale = 0.6619265f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(866.6669f, -116.6666f); _q.Size = new Vector2(290.9718f, 690.6484f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(341.6661f, -666.6663f); _q.Size = new Vector2(85.58329f, 85.58329f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Chinese)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.696094f; _item.MySelectedText.Scale = 0.7460943f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 348.413f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 156.4151f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -35.58279f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -227.5807f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(66.03149f, -502.9117f); _item.MyText.Scale = 0.7837502f; _item.MySelectedText.Scale = 0.7837502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1045.889f, -20.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(460.1277f, -736.1108f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1021.239f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(440.5336f, 74.19697f); _t.Scale = 0.6619421f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(225.3618f, -126.593f); _t.Scale = 0.6619265f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(811.1113f, -152.7777f); _q.Size = new Vector2(290.9718f, 690.6484f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(963.8883f, -647.2219f); _q.Size = new Vector2(85.58329f, 85.58329f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else if (Localization.CurrentLanguage.MyLanguage == Localization.Language.Japanese)
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.696094f; _item.MySelectedText.Scale = 0.7460943f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 348.413f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 156.4151f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -35.58279f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -227.5807f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-556.1907f, -522.3561f); _item.MyText.Scale = 0.7837502f; _item.MySelectedText.Scale = 0.7837502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1045.889f, -20.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(76.54013f, -741.6663f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1018.207f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(434.9782f, 38.08584f); _t.Scale = 0.6619421f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(219.8064f, -162.7042f); _t.Scale = 0.6619265f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(805.5559f, -188.8889f); _q.Size = new Vector2(290.9718f, 690.6484f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(341.6661f, -666.6663f); _q.Size = new Vector2(85.58329f, 85.58329f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}
else
{
	MenuItem _item;
	_item = MyMenu.FindItemByName("Header"); if (_item != null) { _item.SetPos = new Vector2(-2771.113f, 901.9052f); _item.MyText.Scale = 1.696094f; _item.MySelectedText.Scale = 0.7460943f; _item.SelectIconOffset = new Vector2(0f, 0f); _item.SetSelectedPos(new Vector2(-2490.635f, -1f)); }
	_item = MyMenu.FindItemByName("Escalation"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 348.413f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Time Crisis"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, 156.4151f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -35.58279f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Hero Rush 2"); if (_item != null) { _item.SetPos = new Vector2(-2483.969f, -227.5807f); _item.MyText.Scale = 0.8486663f; _item.MySelectedText.Scale = 0.8486663f; _item.SelectIconOffset = new Vector2(0f, 0f); }
	_item = MyMenu.FindItemByName("Leaderboard"); if (_item != null) { _item.SetPos = new Vector2(-556.1907f, -522.3561f); _item.MyText.Scale = 0.7837502f; _item.MySelectedText.Scale = 0.7837502f; _item.SelectIconOffset = new Vector2(0f, 0f); }

	MyMenu.Pos = new Vector2(1045.889f, -20.5556f);

	EzText _t;
	_t = MyPile.FindEzText("Level"); if (_t != null) { _t.Pos = new Vector2(75.00003f, -741.6663f); _t.Scale = 0.72f; }
	_t = MyPile.FindEzText("LevelNum"); if (_t != null) { _t.Pos = new Vector2(1016.667f, -677.7777f); _t.Scale = 0.9744995f; }
	_t = MyPile.FindEzText("Requirement"); if (_t != null) { _t.Pos = new Vector2(301.6447f, 7.530289f); _t.Scale = 0.6619421f; }
	_t = MyPile.FindEzText("Requirement2"); if (_t != null) { _t.Pos = new Vector2(86.4729f, -193.2597f); _t.Scale = 0.6619265f; }

	QuadClass _q;
	_q = MyPile.FindQuad("BoxLeft"); if (_q != null) { _q.Pos = new Vector2(672.2224f, -219.4444f); _q.Size = new Vector2(290.9718f, 690.6484f); }
	_q = MyPile.FindQuad("Button_X"); if (_q != null) { _q.Pos = new Vector2(341.6661f, -666.6663f); _q.Size = new Vector2(85.58329f, 85.58329f); }
	_q = MyPile.FindQuad("Backdrop"); if (_q != null) { _q.Pos = new Vector2(-290.4752f, -2200.793f); _q.Size = new Vector2(1234.721f, 740.8326f); }
	_q = MyPile.FindQuad("Back"); if (_q != null) { _q.Pos = new Vector2(-1291.666f, -982.063f); _q.Size = new Vector2(56.24945f, 56.24945f); }
	_q = MyPile.FindQuad("BackArrow"); if (_q != null) { _q.Pos = new Vector2(-136.1112f, -11.11111f); _q.Size = new Vector2(74.61235f, 64.16662f); }

	MyPile.Pos = new Vector2(83.33417f, 130.9524f);
}

			base.SetPos();
		}
    }
}